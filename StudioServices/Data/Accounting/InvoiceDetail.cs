namespace StudioServices.Data.Accounting
{
    public class InvoiceDetail
    {
        public int InvoiceId { get; set; }
        public int ProductId { get; set; }
        public double UnitPrice { get; set; }
        public float Quantity { get; set; }
        public double UnitPriceDiscount { get; set; }
        public float Tax { get; set; }
    }
}