﻿namespace StudioServices.Data.Sqlite.Payment
{
    public class PaymentHistory : PersonReference
    {
        public double Amount { get; set; }
        public PaymentMethod Type { get; set; }
        public string Description { get; set; }
        public string TransactionId { get; set; }
    }
    public enum PaymentMethod
    {
        CASH = 0,
        CHECK = 1,
        PAYPAL = 2,
        BANK_TRANSFER = 3
    }
}
