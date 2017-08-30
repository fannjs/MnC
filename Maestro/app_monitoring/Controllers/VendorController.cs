using Maestro.app_monitoring.DataObject;
using Maestro.app_monitoring.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Maestro.ControllersIn
{
    public class VendorController : ApiController
    {
        private RepositoryBase<Vendor> repoBase;


        public VendorController()
        {
            //initializer
            repoBase = new RepositoryBase<Vendor>();
            repoBase.tblName = "M_VENDOR_LIST";
        }


        public IEnumerable<Vendor> GetAllVendors()
        {
            return repoBase.GetAll();
        }

        public HttpResponseMessage GetVendor(string id)
        {
            OID oid = new OID();
            oid.Params.Add(Intent.DB_COLUMN.M_VENDOR_NAME.ToString(), id);
            Vendor vendor = repoBase.Get(oid);
            if (vendor == null)
            {
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, "Vendor Not found for the Given name");
            }

            else
            {
                return Request.CreateResponse(HttpStatusCode.OK, vendor);
            }
        }



        public HttpResponseMessage PostVendor(Vendor vendor)
        {
            vendor = repoBase.Add(vendor);

            if (vendor == null)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Duplicate entry for id");
            }
            else
            {
                var response = Request.CreateResponse<Vendor>(HttpStatusCode.Created, vendor);
                string uri = Url.Link("DefaultApi", new { id = vendor.M_VENDOR_NAME });
                response.Headers.Location = new Uri(uri);
                return response;
            }
        }


        public HttpResponseMessage PutVendor(string id, Vendor vendor)
        {
            OID oid = new OID();
            oid.Params.Add(Intent.DB_COLUMN.M_VENDOR_NAME.ToString(), id);

            if (!repoBase.Update(oid, vendor))
            {
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, "Unable to Update the Vendor for the Given id");
            }
            else
            {
                return Request.CreateResponse(HttpStatusCode.OK);
            }
        }



        public HttpResponseMessage DeleteVendor(string id)
        {
            OID oid = new OID();
            oid.Params.Add(Intent.DB_COLUMN.M_VENDOR_NAME.ToString(), id);
            repoBase.Remove(oid);
            return new HttpResponseMessage(HttpStatusCode.NoContent);
        }

    }
}