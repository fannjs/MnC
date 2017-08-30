using Maestro.app_monitoring.DataObject;
using Maestro.app_monitoring.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Maestro.app_monitoring.Controllers
{
    public class CustomerController :ApiController 
    {

        private RepositoryBase<Customer> repoBase;


        public CustomerController()
        {
            //initializer
            repoBase = new RepositoryBase<Customer>();
            repoBase.tblName = "M_CUSTOMER";
        }

        
        public IEnumerable<Customer> GetAllCustomers()
        {
            return repoBase.GetAll();
        }


        public HttpResponseMessage GetCustomer(string id)
        {
            OID oid = new OID();
            oid.Params.Add(Intent.DB_COLUMN.M_CUST_CODE.ToString(), id);
            Customer Customer = repoBase.Get(oid);
            if (Customer == null)
            {
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, "Customer Not found for the Given id");
            }

            else
            {
                return Request.CreateResponse(HttpStatusCode.OK, Customer);
            }
        }



        public HttpResponseMessage PostCustomer(Customer customer)
        {
            customer = repoBase.Add(customer);

            if (customer == null)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Duplicate entry for id");
            }
            else
            {
                var response = Request.CreateResponse<Customer>(HttpStatusCode.Created, customer);
                string uri = Url.Link("DefaultApi", new { id = customer.M_CUST_CODE });
                response.Headers.Location = new Uri(uri);
                return response;
            }
        }


        public HttpResponseMessage PutCustomer(string id, Customer customer)
        {
            OID oid = new OID();
            oid.Params.Add(Intent.DB_COLUMN.M_CUST_CODE.ToString(), id);

            if (!repoBase.Update(oid, customer))
            {
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, "Unable to Update the Customer for the Given id");
            }
            else
            {
                return Request.CreateResponse(HttpStatusCode.OK);
            }
        }



        public HttpResponseMessage DeleteProduct(string id)
        {
            OID oid = new OID();
            oid.Params.Add(Intent.DB_COLUMN.M_CUST_CODE.ToString(), id);
            repoBase.Remove(oid);
            return new HttpResponseMessage(HttpStatusCode.NoContent);
        }

    }
}