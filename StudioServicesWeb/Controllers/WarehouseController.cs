using Microsoft.AspNetCore.Mvc;
using StudioServices.Controllers.Accounting;
using StudioServices.Data.Accounting;
using StudioServices.Models.Accounting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StudioServicesWeb.Controllers.Warehouse
{
    [Route("api/warehouse")]
    public class WarehouseController : MyController
    {
        private WarehouseManager manager;
        public WarehouseController(WarehouseManager m)
        {
            manager = m;
        }
        [Route("product")]
        [HttpPost]
        public Response<bool> SaveProduct(int companyId, string productName, double unitPrice = 1.0, int unitMeasure = 0, double tax = 22.0, double quantity = 0, string productCode = null, int id = 0)
        {
            if (!_isLogged())
                return CreateLoginRequired<bool>();
            var company = manager.GetCompany(companyId);
            if (company == null || company.PersonId != _getPersonId())
                return CreateBoolean(false, ResponseCode.FAIL, "Operazione non autorizzata");

            InvoiceQuantity unit = (InvoiceQuantity)Enum.ToObject(typeof(InvoiceQuantity), unitMeasure);
            bool res = manager.SaveProduct(companyId, _getPersonId(), productName, unitPrice, unit, tax, quantity, productCode, id);
            return CreateBoolean(res);
        }
        [Route("products/{companyId}")]
        [HttpGet]
        public Response<List<CompanyProduct>> GetProducts([FromRoute]int companyId, [FromQuery]bool all=false)
        {
            if (!_isLogged())
                return CreateLoginRequired<List<CompanyProduct>>();
            var company = manager.GetCompany(companyId);
            if (company == null || company.PersonId != _getPersonId())
                return CreateResponse<List<CompanyProduct>>(null, ResponseCode.FAIL, "Operazione non autorizzata");

            var res = manager.ListCompanyProducts(companyId, all);
            return CreateResponse(res);
        }
        [Route("company")]
        [HttpPost]
        public Response<bool> CreateCompany([FromBody]Company company)
        {
            if (!_isLogged())
                return CreateLoginRequired<bool>();
            company.PersonId = _getPersonId();
            var res = manager.SaveCompany(company);
            return CreateBoolean(res, res ? ResponseCode.OK : ResponseCode.FAIL);
        }
        [Route("company/{id}")]
        [HttpPost]
        public Response<bool> UpdateCompany([FromBody]Company company)
        {
            if (!_isLogged())
                return CreateLoginRequired<bool>();
            if (company == null || company.PersonId != _getPersonId())
                return CreateBoolean(false, ResponseCode.FAIL, "Operazione non autorizzata");
            var res = manager.SaveCompany(company);
            return CreateBoolean(res, res ? ResponseCode.OK : ResponseCode.FAIL);
        }
        [Route("company")]
        [HttpGet]
        public Response<List<Company>> GetMyCompanies()
        {
            if (!_isLogged())
                return CreateLoginRequired<List<Company>>();
            return CreateResponse(manager.GetUserCompanies(_getPersonId()));
        }
        [Route("productcode_new")]
        [HttpGet]
        public Response<string> GenerateProductCode(string productName, int company)
        {
            if (!_isLogged())
                return CreateLoginRequired<string>();
            // TODO verifica azienda - person id
            return CreateResponse(manager.GenerateProductCode(productName, company));
        }
        [Route("clients_suppliers")]
        [HttpGet]
        public Response<List<Company>> GetClientsSuppliers()
        {
            if (!_isLogged())
                return CreateLoginRequired<List<Company>>();
            return CreateResponse(manager.GetClientsSuppliers(_getPersonId()));
        }
        [Route("clients_suppliers")]
        [HttpPost]
        public Response<Company> SaveClientSupplier(Company client)
        {
            if (!_isLogged())
                return CreateLoginRequired<Company>();
            var res = manager.SaveCompanyForInvoice(client, _getPersonId());
            // var res = manager.SaveCompanyForInvoice(_getPersonId(), companyName, vat, country, city, province, street, civicNumber, zipCode, out string message, addressId, companyId);
            var company = res > 0 ? manager.GetCompany(res) : null;
            return CreateResponse(company, company != null ? ResponseCode.OK : ResponseCode.FAIL);
        }
        [Route("invoices/{company}")]
        [HttpGet]
        public Response<List<Invoice>> GetInvoiceByYear([FromRoute]int company, [FromQuery]int? year = null)
        {
            if (!_isLogged())
                return CreateLoginRequired<List<Invoice>>();
            // TODO: verifica company person id
            List<Invoice> invoices = null;
            if(year == null)
            {
                // get all invoices
            }
            else
            {

            }
            return CreateResponse(invoices);
        }
    }
}
