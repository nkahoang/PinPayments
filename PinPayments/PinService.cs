using PinPayments.Models;
using System.Net;
using Newtonsoft.Json;
using PinPayments.Infrastructure;
using PinPayments.Actions;
using Microsoft.Extensions.Options;

namespace PinPayments
{
    public class PinService: IPinService
    {
        PinPaymentsOptions _options;
        Requestor _requestor;

        public PinService(PinPaymentsOptions options)
        {
            _options = options;
            _requestor = new Requestor(_options);

            Urls.BaseUrl = _options.BaseUrl;
        }

        public PinService(IOptions<PinPaymentsOptions> options) : this(options.Value) { }

        Requestor Requestor
        {
            get
            {
                return _requestor;
            }
        }

        public CardCreateResponse CardCreate(Card c)
        {
            var url = Urls.Card;
            var postData = ParameterBuilder.ApplyAllParameters(c, "");

            var response = Requestor.PostString(url, postData);
            return JsonConvert.DeserializeObject<CardCreateResponse>(response);
        }

        public Charges Charges()
        {
            var url = Urls.Charges;
            var response = Requestor.GetString(url);

            var result = JsonConvert.DeserializeObject<Charges>(response);
            return result;
        }

        public Charges CustomerCharges(string customerToken)
        {
            var url = Urls.CustomerCharges.Replace("{token}", customerToken);
            var response = Requestor.GetString(url);

            var result = JsonConvert.DeserializeObject<Charges>(response);
            return result;
        }

        public ChargeDetail Charge(string token)
        {
            var url = Urls.Charges + token;
            var response = Requestor.GetString(url);
            
            var result = JsonConvert.DeserializeObject<ChargeDetail>(response);
            return result;
        }

        public ChargeResponse Charge(PostCharge c)
        {
            var url = Urls.Charge;
            var postData = ParameterBuilder.ApplyAllParameters(c, "");

            if (c.Card != null)
            {
                postData += "&card[number]=" + c.Card.CardNumber + "&card[expiry_month]=" + c.Card.ExpiryMonth + "&card[expiry_year]=" + c.Card.ExpiryYear + "&card[cvc]=" + c.Card.CVC + "&card[name]=" + c.Card.Name;
                postData += "&card[address_line1]=" + c.Card.Address1 + "&card[address_line2]=" + c.Card.Address2 + "&card[address_city]=" + c.Card.City + "&card[address_postcode]=" + c.Card.Postcode;
                postData += "&card[address_state]=" + c.Card.State + "&card[address_country]=" + c.Card.Country;
            }
            else if (c.CustomerToken != null)
            {
                postData += "&customer_token=" + c.CustomerToken;
            }
            else if (c.CardToken != null)
            {
                postData += "&card_token=" + c.CardToken;
            }
            else
            {
                throw new PinException(HttpStatusCode.BadRequest, null, "You need to supply either the Card, the Customer Token or a Card Token for payment");
            }

            var response = Requestor.PostString(url, postData);
            return JsonConvert.DeserializeObject<ChargeResponse>(response);
        }

        public Charges ChargesSearch(ChargeSearch cs)
        {
            var url = ParameterBuilder.ApplyAllParameters(cs, Urls.ChargesSearch);

            var response = Requestor.GetString(url);
            return JsonConvert.DeserializeObject<Charges>(response);
        }

        public CustomerAdd CustomerAdd(Customer c)
        {
            var url = Urls.CustomerAdd;
            var postData = ParameterBuilder.ApplyAllParameters(c, "");

            var response = Requestor.PostString(url, postData);
            var customerAdd = JsonConvert.DeserializeObject<CustomerAdd>(response);

            return customerAdd;
        }

        public CustomerUpdate CustomerUpate(Customer c)
        {
            var url = Urls.CustomerAdd + "/" + c.Token;
            var postData = ParameterBuilder.ApplyAllParameters(c, "");

            var response = Requestor.PutString(url, postData);
            var result = JsonConvert.DeserializeObject<CustomerUpdate>(response);
            return result;
        }

        public Customers Customers()
        {
            return Customers(null);
        }

        public Customers Customers(int? page)
        {
            var url = Urls.Customers;
            if (page != null)
            {
                url += "?page=" + page.ToString();
            }
            var response = Requestor.GetString(url);


            var result = JsonConvert.DeserializeObject<Customers>(response);
            return result;
        }

        public RefundResponse Refund(string chargeToken, int amount)
        {
            var url = Urls.Refund;
            var response = Requestor.PostString(url.Replace("{token}", chargeToken), "amount=" + amount.ToString());
            var result = JsonConvert.DeserializeObject<RefundResponse>(response);
            return result;
        }

        public RefundsResponse Refunds(string chargeToken)
        {
            var url = Urls.Refund;
            var response = Requestor.GetString(url.Replace("{token}", chargeToken));
            var result = JsonConvert.DeserializeObject<RefundsResponse>(response);
            return result;
        }

        public Customer Customer(string token)
        {
            var url = Urls.Customers + "/" + token;

            var response = Requestor.GetString(url);
            var customer = JsonConvert.DeserializeObject<CustomerAdd>(response);
            return customer.Response;
        }
    }
}
