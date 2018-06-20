using Microsoft.AspNetCore.Mvc;
using StudioServices.Controllers.Accounting;
using StudioServices.Data.EntityFramework.Accounting;
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
        public Response<bool> SaveProduct([FromBody]CompanyProduct product)
        {
            if (!_isLogged())
                return CreateLoginRequired<bool>();
            var company = manager.GetCompany(product.CompanyId);
            if (company == null || company.PersonId != _getPersonId())
                return CreateBoolean(false, ResponseCode.FAIL, "Operazione non autorizzata");

            bool res = manager.SaveProduct(product);
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

            var res = manager.ListCompanyProducts(companyId, _getPersonId(), all);
            return CreateResponse(res);
        }
        [Route("company")]
        [HttpPost]
        public Response<bool> CreateCompany([FromBody]Company company)
        {
            var checkCode = CheckLoginAndPerson(company);
            if (checkCode != ResponseCode.OK)
                return CreateBoolean(false, checkCode);
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
        public Response<Company> SaveClientSupplier([FromBody]Company client)
        {
            if (!_isLogged())
                return CreateLoginRequired<Company>();
            var res = manager.SaveCompanyForInvoice(client, _getPersonId());
            return CreateResponse(res > 0 ? client : null, res > 0 ? ResponseCode.OK : ResponseCode.FAIL);
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
