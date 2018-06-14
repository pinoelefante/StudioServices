using SQLite;
using SQLiteNetExtensions.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace StudioServices.Data.Accounting
{
    public class CompanyProduct : PersonReference
    {
        [ForeignKey(typeof(Company))]
        [Indexed(Name = "ProductId", Order = 1, Unique = true)]
        public int CompanyId { get; set; }
        [Indexed(Name = "ProductId", Order = 2, Unique = true)]
        public string ProductCode { get; set; }
        public string Name { get; set; }
        public double UnitPrice { get; set; }
        public InvoiceQuantity UnitMeasure { get; set; }
        public double Tax { get; set; }
        public double Quantity { get; set; }
    }
}
