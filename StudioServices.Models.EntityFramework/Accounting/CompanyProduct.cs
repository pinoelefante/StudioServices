using StudioServices.Data.EntityFramework;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace StudioServices.Data.EntityFramework.Accounting
{
    public class CompanyProduct : PersonReference
    {
        // [Indexed(Name = "ProductId", Order = 1, Unique = true), ForeignKey(typeof(Company))]
        [ForeignKey(nameof(Company))]
        public int CompanyId { get; set; }
        // [Indexed(Name = "ProductId", Order = 2, Unique = true)]
        public string ProductCode { get; set; }

        public string Name { get; set; }
        public double UnitPrice { get; set; }
        public InvoiceQuantity UnitMeasure { get; set; }
        public double Tax { get; set; }
        public double Quantity { get; set; }

        // [OneToOne]
        public Company Company { get; set; }
    }
}
