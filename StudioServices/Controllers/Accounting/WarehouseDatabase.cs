using StudioServices.Controllers.Utils;
using StudioServices.Data.Accounting;
using StudioServices.Models.Accounting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudioServices.Controllers.Accounting
{
    public class WarehouseDatabase : Database
    {
        public bool IsProductCodeExists(string productCode, int companyId)
        {
            using (var conn = GetConnection())
            {
                return conn.Table<CompanyProduct>().Where(x => x.ProductCode == productCode && x.CompanyId == companyId).Any();
            }
        }
        public List<CompanyProduct> GetCompanyProducts(int company, bool all = false)
        {
            using (var conn = GetConnection())
            {
                return conn.Table<CompanyProduct>().Where(x => x.CompanyId == company && (all ? true : x.Enabled == true)).OrderBy(x => x.Name).ToList();
            }
        }
        public Company GetCompanyById(int company_id)
        {
            using (var conn = GetConnection())
            {
                return conn.Get<Company>(company_id);
            }
        }
        public Company GetCompanyByVAT(string vat)
        {
            using (var conn = GetConnection())
            {
                return conn.Table<Company>().Where(x => x.VATNumber == vat).FirstOrDefault();
            }
        }
    }
}
