using System;
using System.Collections.Generic;
using PinPayments.Models;
using PinPayments.Infrastructure;
using PinPayments.Actions;

namespace PinPayments
{
    public interface IPinService
    {
        CardCreateResponse CardCreate(Card c);
        Charges Charges();
        Charges CustomerCharges(string customerToken);
        ChargeDetail Charge(string token);
        ChargeResponse Charge(PostCharge c);
        Charges ChargesSearch(ChargeSearch cs);
        CustomerAdd CustomerAdd(Customer c);
        CustomerUpdate CustomerUpate(Customer c);
        Customers Customers();
        Customers Customers(int? page);
        RefundResponse Refund(string chargeToken, int amount);
        RefundsResponse Refunds(string chargeToken);
        Customer Customer(string token);
    }
}