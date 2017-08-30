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
    public class UserInfoController :ApiController 
    {

        private RepositoryBase<UserInfo> repoBase;


        public UserInfoController()
        {
            //initializer
            repoBase = new RepositoryBase<UserInfo>();
            repoBase.tblName = "CSD_LOGIN";
        }

        
        public IEnumerable<UserInfo> GetAllUserInfos()
        {
            return repoBase.GetAll();
        }


        public HttpResponseMessage GetUserInfo(string id)
        {
            OID oid = new OID();
            oid.Params.Add(Intent.DB_COLUMN.USERNAME.ToString(), id);
            UserInfo UserInfo = repoBase.Get(oid);
            if (UserInfo == null)
            {
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, "UserInfo Not found for the Given id");
            }

            else
            {
                return Request.CreateResponse(HttpStatusCode.OK, UserInfo);
            }
        }



        public HttpResponseMessage PostUserInfo(UserInfo userinfo)
        {
            userinfo = repoBase.Add(userinfo);

            if (userinfo == null)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Duplicate entry for id");
            }
            else
            {
                var response = Request.CreateResponse<UserInfo>(HttpStatusCode.Created, userinfo);
                string uri = Url.Link("DefaultApi", new { id = userinfo.USERNAME });
                response.Headers.Location = new Uri(uri);
                return response;
            }

        }


        public HttpResponseMessage PutUserInfo(string id, UserInfo userinfo)
        {
            OID oid = new OID();
            oid.Params.Add(Intent.DB_COLUMN.USERNAME.ToString(), id);

            if (!repoBase.Update(oid, userinfo))
            {
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, "Unable to Update the UserInfo for the Given id");
            }
            else
            {
                return Request.CreateResponse(HttpStatusCode.OK);
            }
        }



        public HttpResponseMessage DeleteProduct(string id)
        {
            OID oid = new OID();
            oid.Params.Add(Intent.DB_COLUMN.USERNAME.ToString(), id);
            repoBase.Remove(oid);
            return new HttpResponseMessage(HttpStatusCode.NoContent);
        }

    }
}