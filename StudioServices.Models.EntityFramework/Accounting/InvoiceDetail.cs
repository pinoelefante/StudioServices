using StudioServices.Data.EntityFramework;
using System.ComponentModel.DataAnnotations.Schema;

namespace StudioServices.Data.EntityFramework.Accounting
{
    public class InvoiceDetail : DataFile
    {
        [ForeignKey(nameof(Product))]
        public int ProductId { get; set; }

        // [ForeignKey(nameof(Invoice))]
        public int InvoiceId { get; set; }

        public double UnitPrice { get; set; }
        public float Quantity { get; set; }
        public double UnitPriceDiscount { get; set; }
        public float Tax { get; set; }

        // public Invoice Invoice { get; set; }
        public CompanyProduct Product { get; set; }
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