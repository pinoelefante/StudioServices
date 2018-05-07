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
        [Route("products")]
        [HttpGet]
        public Response<List<CompanyProduct>> GetProducts([FromQuery]int companyId, [FromQuery]bool all=false)
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
        public Response<bool> CreateCompany(string name, string vat, string country, string city, string province, string street, string civicNumber)
        {
            if (!_isLogged())
                return CreateLoginRequired<bool>();
            var res = manager.SaveCompany(_getPersonId(), name, vat, country, city, province, street, civicNumber, out string message);
            return CreateBoolean(res, res ? ResponseCode.OK : ResponseCode.FAIL, message);
        }
        [Route("company/{id}")]
        [HttpPost]
        public Response<bool> UpdateCompany([FromRoute]int id, string name, string vat, int addressId, string country, string city, string province, string street, string civicNumber)
        {
            if (!_isLogged())
                return CreateLoginRequired<bool>();
            var company = manager.GetCompany(id);
            if (company == null || company.PersonId != _getPersonId())
                return CreateBoolean(false, ResponseCode.FAIL, "Operazione non autorizzata");

            var res = manager.SaveCompany(_getPersonId(), name, vat, country, city, province, street, civicNumber, out string message, id, addressId);
            return CreateBoolean(res, res ? ResponseCode.OK : ResponseCode.FAIL, message);
        }
    }
}
