using StudioServices.Controllers.Utils;
using StudioServices.Data.EntityFramework.Accounting;
using StudioServicesWeb.DataControllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudioServices.Controllers.Accounting
{
    public class WarehouseManager
    {
        private const int PRODUCT_CODE_LENGTH = 3;
        private DatabaseEF db;
        private Random random;
        public WarehouseManager(DatabaseEF d)
        {
            db = d;
            random = new Random();
        }
        // TODO add warehouse log (file saving all operations)

        public Company SaveCompany(Company company)
        {
            if (db.Save(company))
                return company;
            return null;
        }
        public Company GetCompany(int company_id)
        {
            return db.Get<Company>(company_id);
        }
        public List<Company> GetUserCompanies(int person_id)
        {
            return db.Warehouse_GetUserCompanies(person_id);
        }
        public bool SaveProduct(CompanyProduct product)
        {
            if (string.IsNullOrEmpty(product.ProductCode))
                product.ProductCode = GenerateProductCode(product.Name, product.CompanyId);
            return db.Save(product);
        }
        public List<CompanyProduct> ListCompanyProducts(int companyId, int person_id, bool all = false)
        {
            return db.GetList<CompanyProduct>(person_id).Where(x => x.CompanyId == companyId).ToList();
        }
        public string GenerateProductCode(string name, int companyId)
        {
            var words = name.Split(new char[]{' '}, StringSplitOptions.RemoveEmptyEntries);
            int chars = 0;
            int round = 0;
            StringBuilder sb = new StringBuilder(10);
            while (chars < PRODUCT_CODE_LENGTH)
            {
                bool char_ok = false;
                for (int i = 0; i < words.Length; i++)
                {
                    if (chars == PRODUCT_CODE_LENGTH)
                        break;
                    if(words[i].Length > round)
                    {
                        var newChar = words[i][round];
                        if(Char.IsLetter(newChar))
                        {
                            sb.Append(newChar);
                            chars++;
                            char_ok = true;
                        }
                    }
                }
                round++;
                if (!char_ok)
                    break;
            }
            if(sb.Length < PRODUCT_CODE_LENGTH)
            {
                var remainChars = StringUtils.RandomLetters(PRODUCT_CODE_LENGTH - sb.Length);
                sb.Append(remainChars);
            }
            string code = sb.ToString();
            for(int i=1;i<=99;i++)
            {
                if (db.Warehouse_IsProductCodeExists(code, companyId))
                {
                    code += i.ToString();
                    break;
                }
            }
            return code;
        }
        public List<Company> GetClientsSuppliers(int personId)
        {
            return db.Warehouse_GetClientSupplierList(personId);
        }
        public List<Invoice> GetInvoices(int company, int? year)
        {
            return db.Warehouse_GetInvoices(company, year);
        }
        public bool SaveInvoice(Invoice invoice)
        {
            return db.Save(invoice);
        }
    }
}
