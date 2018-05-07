using SQLite;
using StudioServices.Data;
using StudioServices.Data.Accounting;
using System;
using System.Collections.Generic;
using System.Text;

namespace StudioServices.Models.Accounting
{
    public class CompanyProduct : PersonReference
    {
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
