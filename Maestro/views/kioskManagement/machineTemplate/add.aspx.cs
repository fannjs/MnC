using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Json;
using Maestro.Classes;
using Maestro.Classes.MakerChecker;
using Maestro.Classes.Prefix;

namespace Maestro.views.kioskManagement.machineTemplate
{
    public partial class add : System.Web.UI.Page
    {
        private static string taskName = "";
        protected void Page_Load(object sender, EventArgs e)
        {
            taskName = Request.Params["task"];
        }

        [WebMethod]         
        public static McObject<Boolean> addNewMachine(string MachineType, string MachineModel, string ImgMach)
        {
            dbconn dbc = new dbconn();
            dbc.connDB();
            SqlCommand cmd = dbc.conn.CreateCommand();

            try
            {
                cmd.CommandText = "SELECT COUNT(*) FROM M_MACHINE_TYPE WHERE M_MACH_TYPE = '" + MachineType + "' AND M_MACH_MODEL = '" + MachineModel + "'";

                int count = (int)cmd.ExecuteScalar();
                if (count > 0)
                {
                    throw new Exception("Record existed!");
                }

                //MKCK starts here
                if (HttpContext.Current.Session["userId"] == null)
                {
                    throw new Exception("Session timeout. User not found!");
                }
                if (HttpContext.Current.Session["userName"] == null)
                {
                    throw new Exception("Session timeout. User not found!");
                }
                if (String.IsNullOrEmpty(taskName))
                {
                    throw new Exception("Invalid Task Name.");
                }

                string sessionUID = HttpContext.Current.Session["userId"].ToString();
                string sessionUName = HttpContext.Current.Session["userName"].ToString();
                string taskID = MKCK_Function.GetTaskID(taskName);
                string action = MCAction.ADD;
                int status = MCStatus.PENDING;
                string sql = "INSERT INTO M_MACHINE_TYPE (M_MACH_TYPE, M_MACH_MODEL, M_MACH_IMGPATH, CREATED_DATE) " +
                    " VALUES  ('" + MachineType + "', '" + MachineModel + "', '" + ImgMach + "', GETDATE())";

                string htmlImg = "<img class=\"mkck-thumbnail\" src=\"" + ImgMach + "\" />";

                JsonObject NEW = new JsonObject();
                NEW.Add("Kiosk Type", MachineType);
                NEW.Add("Kiosk Model", MachineModel);
                NEW.Add("Kiosk Image", htmlImg);

                string newdata = NEW.ToString();

                cmd.CommandText = "INSERT INTO M_MAKER_CHECKER_LIST(TASK_ID,TASK_NAME,CREATED_DATE,NEW_VALUE,ACTION_TYPE,APPROVE_SQL,MAKER_ID,MAKER_USER_NAME,APPROVAL_STATUS) "
                                    + "VALUES(" + taskID + ", '" + taskName + "', GETDATE(), @newdata, '" + action + "', "
                                    + "@sql, " + sessionUID + ", '" + sessionUName + "', " + status + ")";
                cmd.Parameters.AddWithValue("newdata", newdata);
                cmd.Parameters.AddWithValue("sql", sql);
                cmd.ExecuteNonQuery();

                return new McObject<Boolean>(true, "Successful.");
            }
            catch (Exception ex)
            {
                return new McObject<Boolean>(false, "Failed! " + ex.Message);
            }
            finally
            {
                dbc.closeConn();
            }
        }
    }
}