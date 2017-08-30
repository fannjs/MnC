using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Maestro.views
{
    /// <summary>
    /// Summary description for GetDateTime
    /// </summary>
    public class GetDateTime : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            try
            {
                context.Response.ContentType = "text/event-stream";
                context.Response.Expires = -1;

                while (true)
                {
                    context.Response.Write(string.Format("data: {0}\n\n", DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt")));
                    context.Response.Flush();
                    System.Threading.Thread.Sleep(1000);
                }
            }
            catch (Exception ex)
            {
                context.Response.Close();
            }
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}