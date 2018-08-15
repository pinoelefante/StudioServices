using SQLite;
using SQLiteNetExtensions.Attributes;

namespace StudioServices.Data.Sqlite.Accounting
{
    public class CompanyProduct : PersonReference
    {
        private string name, pcode;
        private double price, tax, quantity;

        [Indexed(Name = "ProductId", Order = 1, Unique = true), ForeignKey(typeof(Company))]
        public int CompanyId { get; set; }
        [Indexed(Name = "ProductId", Order = 2, Unique = true)]
        public string ProductCode { get => pcode; set => Set(ref pcode, value); }

        public string Name { get => name; set => Set(ref name, value); }
        public double UnitPrice { get => price; set => Set(ref price, value); }
        public InvoiceQuantity UnitMeasure { get; set; }
        public double Tax { get => tax; set => Set(ref tax, value); }
        public double Quantity { get => quantity; set => Set(ref quantity, value); }

        [OneToOne]
        public Company Company { get; set; }
    }
}
