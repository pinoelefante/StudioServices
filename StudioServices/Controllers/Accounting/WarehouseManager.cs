using StudioServices.Controllers.Utils;
using StudioServices.Data.Accounting;
using StudioServices.Data.Registry;
using StudioServices.Models.Accounting;
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
        private WarehouseDatabase db;
        private Random random;
        public WarehouseManager()
        {
            db = new WarehouseDatabase();
            random = new Random();
        }
        // TODO add warehouse log (file saving all operations)
        public bool SaveCompany(int personId, string companyName, string vatnumber, string country, string city, string province, string street, string civicnumber, out string message, int company_id = 0, int address_id = 0)
        {
            message = string.Empty;
            Address addr = new Address()
            {
                Id = address_id,
                AddressType = AddressType.WORK,
                City = city,
                CivicNumber = civicnumber,
                Country = country,
                Enabled = true,
                PersonId = personId,
                Province = province,
                Street = street
            };
            if (!db.SaveItem(addr))
            {
                message = "Impossibile salvare l'indirizzo";
                return false;
            }
            Company company = new Company()
            {
                Id = company_id,
                AddressId = addr.Id,
                Enabled = true,
                Name = companyName,
                VATNumber = vatnumber,
                PersonId = personId
            };
            if (db.SaveItem(company))
            {
                message = "Impossibile salvare la compagnia";
                if (company_id > 0)
                    db.Delete<Address>(addr);
                return false;
            }
            return true;
        }
        public Company GetCompany(int company_id)
        {

        }
        public bool SaveProduct(int companyId, int personId, string productName, double unitPrice = 1.0, InvoiceQuantity unitMeasure = InvoiceQuantity.PZ, double tax = 22.0, double quantity = 0, string productCode = null, int id = 0)
        {
            if (string.IsNullOrEmpty(productCode))
                productCode = GenerateProductCode(productName, companyId);
            CompanyProduct product = new CompanyProduct()
            {
                Id = id,
                CompanyId = companyId,
                PersonId = personId,
                Name = productName,
                UnitPrice = unitPrice,
                UnitMeasure = unitMeasure,
                Tax = tax,
                Quantity = quantity,
                ProductCode = productCode,
                Enabled = true
            };
            return db.SaveItem(product, true);
        }
        public List<CompanyProduct> ListCompanyProducts(int companyId, bool all = false)
        {
            return db.GetCompanyProducts(companyId, all);
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
                if (db.IsProductCodeExists("", companyId))
                {
                    code += i.ToString();
                    break;
                }
            }
            return code;
        }
    }
}
