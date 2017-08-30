using Maestro.app_monitoring.DataObject;
using Maestro.app_monitoring.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Maestro.Controllers
{
    public class MachineErrorController :ApiController 
    {

        private RepositoryBase<MachineError> repoBase;


        public MachineErrorController()
        {
            //initializer
            repoBase = new RepositoryBase<MachineError>();
            repoBase.tblName = "M_CODES";
        }


        public IEnumerable<MachineError> GetAllMachineErrors()
        {
            return repoBase.GetAll();
        }


        public HttpResponseMessage GetMachineError(string id)
        {
            OID oid = new OID();
            oid.Params.Add(Intent.DB_COLUMN.M_CODE.ToString(), id);
            MachineError MachineError = repoBase.Get(oid);
            if (MachineError == null)
            {
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, "MachineError Not found for the Given id");
            }

            else
            {
                return Request.CreateResponse(HttpStatusCode.OK, MachineError);
            }
        }



        public HttpResponseMessage PostMachineError(MachineError me)
        {
            me = repoBase.Add(me);

            if (me == null)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Duplicate entry for id");
            }
            else
            {
                var response = Request.CreateResponse<MachineError>(HttpStatusCode.Created, me);
                string uri = Url.Link("DefaultApi", new { id = me.M_CODE });
                response.Headers.Location = new Uri(uri);
                return response;
            }
        }


        public HttpResponseMessage PutMachineError(string id, MachineError me)
        {
            OID oid = new OID();
            oid.Params.Add(Intent.DB_COLUMN.M_CODE.ToString(), id);

            if (!repoBase.Update(oid, me))
            {
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, "Unable to Update the MachineError for the Given id");
            }
            else
            {
                return Request.CreateResponse(HttpStatusCode.OK);
            }
        }



        public HttpResponseMessage DeleteMachineError(string id)
        {
            OID oid = new OID();
            oid.Params.Add(Intent.DB_COLUMN.M_CODE.ToString(), id);
            repoBase.Remove(oid);
            return new HttpResponseMessage(HttpStatusCode.NoContent);
        }

    }
}