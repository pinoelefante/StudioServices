using StudioServices.Data.EntityFramework;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudioServices.Data.EntityFramework.Accounting
{
    public class Invoice : DataFile
    {
        [ForeignKey(nameof(Sender))]
        public int SenderId { get; set; }
        // [OneToOne]
        // 
        public Company Sender { get; set; }

        [ForeignKey(nameof(Company))]
        public int Recipient { get; set; }

        public InvoiceType Type { get; set; }
        public double Discount { get; set; }
        public double Total { get; set; }
        public DateTime Emission { get; set; }
        public int Number { get; set; }
        public string NumberExtra { get; set; }
        public double Transport { get; set; }
        public string Note { get; set; }

        // [OneToMany]
        public List<InvoiceDetail> InvoiceDetails { get; set; } = new List<InvoiceDetail>();
    }
    public enum InvoiceType
    {
        SELL = 0,
        PURCHASE = 1
    }
}
