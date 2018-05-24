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
                return conn.Table<CompanyProduct>().Where(x => x.CompanyId == company && (all ? true : x.Enabled)).OrderBy(x => x.Name).ToList();
            }
        }
        public Company GetCompanyById(int company_id)
        {
            using (var conn = GetConnection())
            {
                return conn.Get<Company>(company_id);
            }
        }
        public List<Company> GetCompanyByUser(int person_id, bool all = false)
        {
            using (var conn = GetConnection())
            {
                var content = conn.Table<Company>().Where(x => x.PersonId == person_id/* && (all ? true : x.Enabled)*/);
                return content.ToList();
            }
        }
        public Company GetCompanyByVAT(string vat)
        {
            using (var conn = GetConnection())
            {
                return conn.Table<Company>().Where(x => x.VATNumber == vat).FirstOrDefault();
            }
        }
        /*
        public List<Company> GetClients(int companyId)
        {
            using (var conn = GetConnection())
            {
                var companies = conn.Table<Invoice>().Where(x => x.Sender == companyId && x.Type==InvoiceType.SELL).GroupBy(x => x.Recipient).Select(x => x.First()).Select(x => x.Recipient);
                return conn.Table<Company>().Where(z => companies.Contains(z.Id)).ToList();
            }
        }
        public List<Company> GetSuppliers(int companyId)
        {
            using (var conn = GetConnection())
            {
                var companies = conn.Table<Invoice>().Where(x => x.Sender == companyId && x.Type == InvoiceType.PURCHASE).GroupBy(x => x.Recipient).Select(x => x.First()).Select(x => x.Recipient);
                return conn.Table<Company>().Where(z => companies.Contains(z.Id)).ToList();
            }
        }
        */
        public List<Company> GetClientsSuppliets(int personId)
        {
            using (var conn = GetConnection())
            {
                return conn.Table<Company>().Where(x => x.PersonId == personId).ToList();
            }
        }
    }
}
