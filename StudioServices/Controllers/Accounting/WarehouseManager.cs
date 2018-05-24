using StudioServices.Controllers.Persons;
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
        public bool SaveCompany(int personId, string companyName, string vatnumber, int address_id, out string message, int company_id = 0)
        {
            message = string.Empty;
            //TODO verifica address_id
            Company company = new Company()
            {
                Id = company_id,
                AddressId = address_id,
                Enabled = true,
                Name = companyName,
                VATNumber = vatnumber,
                PersonId = personId
            };
            return db.SaveItem(company);
        }
        public int SaveCompanyForInvoice(int person_id, string companyName, string vatNumber,string country, string city, string province, string street, string civicNum, string zipCode, out string message, int address_id = 0, int company_id = 0)
        {
            message = string.Empty;
            Address addr = new Address()
            {
                AddressType = AddressType.WORK,
                City = city,
                CivicNumber = civicNum,
                Country = country,
                Description = string.Empty,
                Id = address_id,
                PersonId = -person_id,
                Province = province,
                Street = street,
                ZipCode = zipCode
            };
            if (!db.SaveItem(addr))
            {
                message = "Salvataggio indirizzo fallito";
                return 0;
            }
            Company company = new Company()
            {
                AddressId = addr.Id,
                Id = company_id,
                Name = companyName,
                PersonId = -person_id,
                VATNumber = vatNumber
            };
            if (db.SaveItem(company))
                return company.Id;
            return 0;
        }
        public Company GetCompany(int company_id)
        {
            return db.GetCompanyById(company_id);
        }
        public List<Company> GetUserCompanies(int user_id)
        {
            return db.GetCompanyByUser(user_id);
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
        /*
        public List<Company> GetClients(int companyId)
        {
            return db.GetClients(companyId);
        }
        public List<Company> GetSuppliers(int companyId)
        {
            return db.GetSuppliers(companyId);
        }
        */
        public List<Company> GetClientsSuppliers(int personId)
        {
            return db.GetClientsSuppliets(personId);
        }
    }
}
