# PinPayments Library for .NET Core

The PinPayments Library is a .NET Core wrapper for http://pin.net.au, compatible with .NET Standard 1.6+

This was a fork on the [original repo by cpayne22](https://github.com/cpayne22/PinPayments)

For more information about the examples below, you can visit https://pin.net.au/docs/api for a full reference.

Quick Start
-----------

a) Obtain either your Publish key or your Secret key (see the differences here: https://pin.net.au/docs/api)

b) Put the following in appsettings.json:

```json
{
  // [...] your other settings
  // Put PinPayments section in
  "PinPayments": {
    "BaseUrl": "https://test-api.pin.net.au",
    "ApiKey": "API_KEY_GOES_HERE"
  }
}
```
	
c) In your application initialization, either

Option 1: Get configuration from config section:

```csharp
var builder = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile("appsettings.development.json", optional: true, reloadOnChange: true);
// your other configurations
IConfigurationRoot configuration = builder.Build();

var options = new PinPaymentsOptions();
configuration.GetSection("PinPayments").Bind(options);

IPinService ps = new PinService(options);
```

Option 2: Use .NET Core's dependency injection:

```csharp
// in your Startup.cs
using PinPayments.Extensions;


/* This should come with a standard .NET Core ASP.NET Site */
public Startup (IHostingEnvironment env) {
    _currentEnvironment = env;

    var builder = new ConfigurationBuilder ()
        .SetBasePath (env.ContentRootPath)
        .AddJsonFile ("appsettings.json", optional : true, reloadOnChange : true)
        .AddJsonFile ($"appsettings.{env.EnvironmentName}.json", optional : true)
        .AddEnvironmentVariables ();
    Configuration = builder.Build ();
}

/* Then register the PinPayments service */
public void ConfigureServices (IServiceCollection services) {
    services.AddPinPayments(Configuration);
}

```

Then in your controller, simply inject in `IPinService` in your constructor
	
Examples
========
Working example for PinPayments are in the PinPayments.Console app. Simply:

- Clone this repo.
- Copy `PinPayments.Console\appsettings.json` to `appsettings.development.json` and set the correct details.
- Perform a `dotnet restore` inside PinPayments.Console folder.
- Run the sample console app: `dotnet run`.

Charges
-----

### Charging a card

	IPinService ps; // inject this

	// https://pin.net.au/docs/api/test-cards
	// 5520000000000000 - Mastercard
	// 4200000000000000 - Visa

	var card = new Card();
	card.CardNumber = "5520000000000000";
	card.CVC = "111";
	card.ExpiryMonth = DateTime.Today.Month.ToString();  // Use the real Expiry
	card.ExpiryYear = (DateTime.Today.Year + 1).ToString(); // Not my defaults!
	card.Name = "Roland Robot";
	card.Address1 = "42 Sevenoaks St";
	card.Address2 = null;
	card.City = "Lathlain";
	card.Postcode = "6454";
	card.State = "WA";
	card.Country = "Australia";

    var response = ps.Charge(new PostCharge { Amount = 100, Card = card, Currency = "AUD", Description = "Desc", Email = "email@test.com", IPAddress = "127.0.0.1" });
	System.Console.WriteLine(response.Charge.success);

	
### Charge Search

	// See https://pin.net.au/docs/api/charges#search-charges for more detail
	IPinService ps; // inject this

    var cs = new PinPayments.ChargeSearch { Query = "", Sort = ChargeSearchSortEnum.Amount, SortDirection = SortDirectionEnum.Descending };
    var response = ps.ChargesSearch(cs);
    System.Console.WriteLine(response.Count.ToString() + " transactions found");
    foreach (var r in response.Response)
    {
        System.Console.WriteLine(r.description + " " + r.amount.ToString());
    }
	
	
Customers
-----

### Listing all customers
    // See https://pin.net.au/docs/api/customers#get-customers

	IPinService ps; // inject this
    var customers = ps.Customers();
	
	
### Adding a new customer
	
    // See https://pin.net.au/docs/api/customers#post-customers for more detail

	IPinService ps; // inject this

    var customer = new Customer();
    customer.Email = "roland@pin.net.au";
    customer.Card = new Card();
    customer.Card.CardNumber = "5520000000000000";
    customer.Card.ExpiryMonth = "05";
    customer.Card.ExpiryYear = "2014";
    customer.Card.CVC = "123";
    customer.Card.Name = "Roland Robot";
    customer.Card.Address1 = "42 Sevenoaks St";
    customer.Card.Address2 = "";
    customer.Card.City = "Lathlain";
    customer.Card.Postcode = "6454";
    customer.Card.State = "WA";
    customer.Card.Country = "Australia";

    var response = ps.CustomerAdd(customer);

    System.Console.WriteLine("Customer token: " + response.Customer.Token);

	
### Updating a customer

    // See https://pin.net.au/docs/api/customers#put-customer

	IPinService ps; // inject this
    var customers = ps.Customers();
    customer = customers.Customer[0];
    customer.Card = new Card();
    customer.Card.CardNumber = "5520000000000000";
    customer.Card.ExpiryMonth = "05";
    customer.Card.ExpiryYear = "2014";
    customer.Card.CVC = "123";
    customer.Card.Name = "Roland Robot";
    customer.Card.Address1 = "42 Sevenoaks St";
    customer.Card.Address2 = "";
    customer.Card.City = "Lathlain";
    customer.Card.Postcode = "6454";
    customer.Card.State = "WA";
    customer.Card.Country = "Australia";

    customer.State = "NSW";
    var customerUpate = ps.CustomerUpate(customer);            


### Refunds

    // Refunds - Pin supports partial refunds
    // https://pin.net.au/docs/api/customers#get-customers-charges

    var refund = ps.Refund("INSERT CHARGE TOKEN", 200);
    refund = ps.Refund("INSERT CHARGE TOKEN", 100);

	
	// Lists all refunds for a particular charge
    var refunds = ps.Refunds("INSERT CHARGE TOKEN");
	
	
### Card Tokens

    // Card Token
    // https://pin.net.au/docs/api/cards
    // 5520000000000000 - Test Mastercard
    // 4200000000000000 - Test Visa

    card = new Card();
    card.APIKey = ""; // OPTIONAL.  Your publishable API key, if requesting from an insecure environment.
    card.CardNumber = "5520000000000000";
    card.CVC = "111";
    card.ExpiryMonth = DateTime.Today.Month.ToString();  // Use the real Expiry
    card.ExpiryYear = (DateTime.Today.Year + 1).ToString(); // Not my defaults!
    card.Name = "Roland Robot";
    card.Address1 = "42 Sevenoaks St";
    card.Address2 = "";
    card.City = "Lathlain";
    card.Postcode = "6454";
    card.State = "WA";
    card.Country = "Australia";

    var respCardCreate = ps.CardCreate(card);

    response = ps.Charge(new PostCharge { Amount = 1500, CardToken = card.Token, Currency = "AUD", Description = "Desc", Email = "email@test.com", IPAddress = "127.0.0.1" });
    System.Console.WriteLine(response.Charge.Success);	
	
	
    // Card tokens can only be used once.
    // If you try and use it a second time, you will get the following message:
    response = ps.Charge(new PostCharge { Amount = 1500, CardToken = card.Token, Currency = "AUD", Description = "Desc", Email = "email@test.com", IPAddress = "127.0.0.1" });
    System.Console.WriteLine(response.Error); // "token_already_used"
    System.Console.WriteLine(response.Description); // "Token already used. Card tokens can only be used once, to create a charge or assign a card to a customer."
	
	
	
Errors
------

Any errors that occur on any of the services will throw a PinException with the message returned from Pin. It is a good idea to run your service calls in a try and catch PinExceptions.

