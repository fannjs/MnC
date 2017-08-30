using Maestro.app_monitoring.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Maestro.Controllers
{
    public class MachineTypeController : ApiController
    {
        private RepositoryBase<MachineType> repoBase;

        public MachineTypeController()
        {
            //initializer
            repoBase = new RepositoryBase<MachineType>();
            repoBase.tblName = "M_MACHINE_TYPE";
        }

        public IEnumerable<MachineType> GetAllMachineTypes()
        {  
            return repoBase.GetAll();
        }

  
        public HttpResponseMessage GetMachineType(string id1, string id2)
        {
            OID oid = new OID();
            oid.Params.Add(Maestro.app_monitoring.DataObject.Intent.DB_COLUMN.M_MACH_TYPE.ToString(), id1);
            oid.Params.Add(Maestro.app_monitoring.DataObject.Intent.DB_COLUMN.M_MACH_MODEL.ToString(), id2);

            MachineType MachineType = repoBase.Get(oid);
            if (MachineType == null)
            {
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, "MachineType Not found for the Given id");
            }

            else
            {
                return Request.CreateResponse(HttpStatusCode.OK, MachineType);
            }
        }

        //no nit post as we alr know the ids
        public HttpResponseMessage PutMachineType(string id1, string id2, MachineType machinetype)
        {
            OID oid = new OID();
            oid.Params.Add(Maestro.app_monitoring.DataObject.Intent.DB_COLUMN.M_MACH_TYPE.ToString(), id1);
            oid.Params.Add(Maestro.app_monitoring.DataObject.Intent.DB_COLUMN.M_MACH_MODEL.ToString(), id2);


            if (!repoBase.Update(oid, machinetype))
            {
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, "Unable to Update the MachineType for the Given id");
            }
            else
            {
                return Request.CreateResponse(HttpStatusCode.OK);
            }
        }

        public HttpResponseMessage PostMachineType(MachineType machinetype)
        {
            machinetype = repoBase.Add(machinetype);

            if (machinetype == null)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Duplicate entry for id");
            }
            else
            {
                var response = Request.CreateResponse<MachineType>(HttpStatusCode.Created, machinetype);
                string uri = Url.Link("DefaultApi", new { id1 = machinetype.M_MACH_TYPE, id2 = machinetype.M_MACH_MODEL });
                response.Headers.Location = new Uri(uri);
                return response;
            }
        }

        public string FixBase64ForImage(string Image)
        {
            System.Text.StringBuilder sbText = new System.Text.StringBuilder(Image, Image.Length);
            sbText.Replace("\r\n", String.Empty); sbText.Replace(" ", String.Empty);
            return sbText.ToString();
        }
        public HttpResponseMessage DeleteProduct(string id1, string id2)
        {

            OID oid = new OID();
            oid.Params.Add(Maestro.app_monitoring.DataObject.Intent.DB_COLUMN.M_MACH_TYPE.ToString(), id1);
            oid.Params.Add(Maestro.app_monitoring.DataObject.Intent.DB_COLUMN.M_MACH_MODEL.ToString(), id2);

            repoBase.Remove(oid);
            return new HttpResponseMessage(HttpStatusCode.NoContent);
        }

    }
}