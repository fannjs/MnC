using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Script.Services;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;
using Maestro.Classes.MakerChecker;
using Maestro.Classes.Prefix;
using System.Json;
using Maestro.Classes;

namespace Maestro.views.administration.vendorManagement
{
    public partial class add : System.Web.UI.Page
    {
        private static string taskName = "";
        protected void Page_Load(object sender, EventArgs e)
        {
            taskName = Request.Params["task"];
        }

        [WebMethod]
        public static McObject<Boolean> insertNewVendor(string vendorName, string contactPerson, string telNo, string primaryEmail, string secondaryEmail, string add1, string add2, string country)
        {
            dbconn dbc = new dbconn();
            dbc.connDB();
            SqlCommand cmd = dbc.conn.CreateCommand();

            try
            {
                if (HttpContext.Current.Session["userId"] == null)
                {
                    throw new Exception("Session timeout. User not found!");
                }
                if (HttpContext.Current.Session["userName"] == null)
                {
                    throw new Exception("Session timeout. User not found!");
                }

                string sessionUID = HttpContext.Current.Session["userId"].ToString();
                string sessionUName = HttpContext.Current.Session["userName"].ToString();
                string taskID = MKCK_Function.GetTaskID(taskName);
                string action = MCAction.ADD;
                int status = MCStatus.PENDING;
                string sql = "INSERT INTO M_VENDOR_LIST (M_VENDOR_NAME, M_VENDOR_PERSON1, M_VENDOR_TELNO1, M_VENDOR_EMAIL1, M_VENDOR_EMAIL2, M_VENDOR_ADDRESS1, M_VENDOR_ADDRESS2, M_VENDOR_COUNTRY) VALUES ( '"
                    + vendorName + "', '" + contactPerson + "', '" + telNo + "', '" + primaryEmail + "', '" + secondaryEmail + "', '" + add1 + "', '" + add2 + "', '" + country + "' )";

                JsonObject NEW = new JsonObject();
                NEW.Add("Vendor Name", vendorName);
                NEW.Add("Contact Persion", contactPerson);
                NEW.Add("Tel No.", telNo);
                NEW.Add("Primary Email", primaryEmail);
                NEW.Add("Secondary Email", secondaryEmail);
                NEW.Add("Address 1", add1);
                NEW.Add("Address 2", add2);
                NEW.Add("Country", country);

                string newdata = NEW.ToString();

                cmd.CommandText = "INSERT INTO M_MAKER_CHECKER_LIST(TASK_ID,TASK_NAME,CREATED_DATE,NEW_VALUE,ACTION_TYPE,APPROVE_SQL,MAKER_ID,MAKER_USER_NAME,APPROVAL_STATUS) "
                                    + "VALUES(" + taskID + ", '" + taskName + "', GETDATE(), '" + newdata + "', '" + action + "', "
                                    + "@sql, " + sessionUID + ", '" + sessionUName + "', " + status + ")";
                cmd.Parameters.AddWithValue("sql", sql);
                cmd.ExecuteNonQuery();
                dbc.closeConn();

                return new McObject<bool>(true, "Successful.");
            }
            catch (Exception ex)
            {
                return new McObject<bool>(false, "Failed. " + ex.Message);
            }
            finally
            {
                dbc.closeConn();
            }
        }
    }
}