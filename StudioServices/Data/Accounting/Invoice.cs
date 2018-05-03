using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudioServices.Data.Accounting
{
    public class Invoice : PersonReference
    {
        public int Sender { get; set; }
        public int Recipient { get; set; }
        public InvoiceType Type { get; set; }
        public double Discount { get; set; }
        public double Total { get; set; }
        public DateTime Emission { get; set; }
        public int Number { get; set; }
        public string NumberExtra { get; set; }
        public double Transport { get; set; }
        public List<InvoiceDetail> InvoiceDetails { get; } = new List<InvoiceDetail>();
    }
    public enum InvoiceType
    {
        SELL=1,
        PURCHASE = 2
    }
}
