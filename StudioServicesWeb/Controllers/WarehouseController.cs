﻿using Microsoft.AspNetCore.Mvc;
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
        public Response<Tuple<int, string>> SaveProduct([FromBody]CompanyProduct product)
        {
            if (!_isLogged())
                return CreateLoginRequired<Tuple<int, string>>();
            if (CheckPersonCompany(product.CompanyId, manager.GetCompany))
                CreateResponse(default(Tuple<int,string>), ResponseCode.INVALID_PERSON);

            product.PersonId = _getPersonId();
            bool res = manager.SaveProduct(product);
            return CreateResponse<Tuple<int,string>>(res ? new Tuple<int,string>(product.Id,product.ProductCode) : default(Tuple<int,string>), res ? ResponseCode.OK : ResponseCode.FAIL);
        }
        [Route("products/{companyId}")]
        [HttpGet]
        public Response<List<CompanyProduct>> GetProducts([FromRoute]int companyId, [FromQuery]bool all=false)
        {
            if (!_isLogged())
                return CreateLoginRequired<List<CompanyProduct>>();
            if (CheckPersonCompany(companyId, manager.GetCompany))
                CreateResponse(default(List<CompanyProduct>), ResponseCode.INVALID_PERSON);

            var res = manager.ListCompanyProducts(companyId, _getPersonId(), all);
            return CreateResponse(res);
        }
        [Route("company")]
        [HttpPost]
        public Response<Company> CreateCompany([FromBody]Company company)
        {
            var checkCode = CheckLoginAndPerson(company);
            if (checkCode != ResponseCode.OK)
                return CreateResponse<Company>(null, checkCode);
            var res = manager.SaveCompany(company);
            return CreateResponse(res, res != null ? ResponseCode.OK : ResponseCode.FAIL);
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
            if (CheckPersonCompany(company, manager.GetCompany))
                CreateResponse(default(string), ResponseCode.INVALID_PERSON);
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
        [Route("invoices/{company}")]
        [HttpGet]
        public Response<List<Invoice>> GetInvoiceByYear([FromRoute]int company, [FromQuery]int? year = null)
        {
            if (!_isLogged())
                return CreateLoginRequired<List<Invoice>>();
            if (CheckPersonCompany(company, manager.GetCompany))
                CreateResponse(default(List<Invoice>), ResponseCode.INVALID_PERSON);

            List<Invoice> invoices = manager.GetInvoices(company, year);
            return CreateResponse(invoices);
        }
        [Route("invoice")]
        [HttpPost]
        public Response<int> SaveInvoice([FromBody]Invoice invoice)
        {
            if (!_isLogged())
                return CreateLoginRequired<int>();
            if (CheckPersonCompany(invoice.SenderId, manager.GetCompany))
                CreateResponse(default(int), ResponseCode.INVALID_PERSON);
            var response = manager.SaveInvoice(invoice);
            return CreateResponse<int>(invoice.Id);
        }
    }
}
