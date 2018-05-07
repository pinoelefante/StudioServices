namespace StudioServices.Data.Accounting
{
    public class InvoiceDetail
    {
        public int ProductId { get; set; }
        public double UnitPrice { get; set; }
        public float Quantity { get; set; }
        public double UnitPriceDiscount { get; set; }
        public float Tax { get; set; }
    }
    public enum InvoiceQuantity
    {
        PZ = 0, //pezzi
        Kg = 1, //Kilogrammi
        g = 2,  //grammi
        m = 3,  //metri
        mq = 4, //metro quadrato
    }
}