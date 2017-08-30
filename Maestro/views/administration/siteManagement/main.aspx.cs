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

namespace Maestro.views.administration.siteManagement
{

    public partial class main : System.Web.UI.Page
    {
        private static string taskName = "";
        protected void Page_Load(object sender, EventArgs e)
        {
            taskNameHidden.Value = Request.Params["task"];
            taskName = Request.Params["task"];
        }        

        [WebMethod]
        public static CustDetails[] getCustDetails()
        {
            //System.Windows.Forms.MessageBox.Show("CustDetails [] ");
            dbconn dbc = new dbconn();
            dbc.connDB();
            try
            {
                SqlCommand cmd = dbc.conn.CreateCommand();
                //"SELECT * FROM [M_CUSTOMER] ORDER BY [M_MACH_COUNTRY], [M_CUST_NAME]" 
                cmd.CommandText = "SELECT * FROM M_CUSTOMER ORDER BY M_MACH_COUNTRY, M_CUST_NAME";
                SqlDataReader dr = cmd.ExecuteReader();

                List<CustDetails> details = new List<CustDetails>();

                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        CustDetails cust = new CustDetails();
                        cust.CustCode = dr["M_CUST_CODE"].ToString();
                        cust.CustName = dr["M_CUST_NAME"].ToString();
                        cust.Country = dr["M_MACH_COUNTRY"].ToString();
                        details.Add(cust);
                    }
                }
                dr.Close();
                dbc.closeConn();
                return details.ToArray();
            }
            catch (Exception ex)
            {
                return null;
            }
            finally
            {
                if (dbc.conn != null)
                {
                    dbc.closeConn();
                }
            }
        }


        [WebMethod, System.Web.Script.Services.ScriptMethod]
        public static Boolean deleteCust(string custCode)
        {
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
                string action = MCAction.DELETE;
                int status = MCStatus.PENDING;
                string sql = "DELETE M_CUSTOMER WHERE M_CUST_CODE = '" + custCode + "'";
                
                dbconn dbc = new dbconn();
                dbc.connDB();
                SqlCommand cmd = dbc.conn.CreateCommand();
                cmd.CommandText = "SELECT * FROM M_CUSTOMER WHERE M_CUST_CODE = '" + custCode + "'";
                SqlDataReader dr = cmd.ExecuteReader();

                string oldSiteCode = "";
                string oldSiteName = "";
                string oldSiteCountry = "";

                if (dr.HasRows)
                {
                    if (dr.Read())
                    {
                        oldSiteCode = dr["M_CUST_CODE"].ToString();
                        oldSiteName = dr["M_CUST_NAME"].ToString();
                        oldSiteCountry = dr["M_MACH_COUNTRY"].ToString();
                    }
                }
                else
                {
                    return false;
                }
                dr.Close();

                JsonObject OLD = new JsonObject();
                OLD.Add("Site Code", oldSiteCode);
                OLD.Add("Site Name", oldSiteName);
                OLD.Add("Country", oldSiteCountry);

                string olddata = OLD.ToString();

                cmd.CommandText = "INSERT INTO M_MAKER_CHECKER_LIST(TASK_ID,TASK_NAME,CREATED_DATE,OLD_VALUE,ACTION_TYPE,APPROVE_SQL,MAKER_ID,MAKER_USER_NAME,APPROVAL_STATUS) "
                                    + "VALUES(" + taskID + ", '" + taskName + "', GETDATE(), '" + olddata + "','" + action + "', "
                                    + "@sql, " + sessionUID + ", '" + sessionUName + "', " + status + ")";
                cmd.Parameters.AddWithValue("sql", sql);
                cmd.ExecuteNonQuery();
                dbc.closeConn();

                return true;
            }
            catch (Exception ex)
            {
                //log
                return false;
            }
        }
    }

    public class CustDetails
    {
        public string CustCode { get; set; }
        public string CustName { get; set; }
        public string Country { get; set; }
    }


}