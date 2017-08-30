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
    public partial class main : System.Web.UI.Page
    {
        private static string taskName = "";
        protected void Page_Load(object sender, EventArgs e)
        {
            taskNameHidden.Value = Request.Params["task"];
            taskName = Request.Params["task"];
        }
        [WebMethod]
        public static VendorDetails[] getVendorDetails()
        {
            //System.Windows.Forms.MessageBox.Show("VendorDetails [] ");
            dbconn dbc = new dbconn();
            dbc.connDB();
            SqlCommand cmd = dbc.conn.CreateCommand();
            //"SELECT * FROM [M_CUSTOMER] ORDER BY [M_MACH_COUNTRY], [M_CUST_NAME]" 
            cmd.CommandText = "SELECT M_VENDOR_NAME, M_VENDOR_PERSON1, M_VENDOR_PERSON2, M_VENDOR_PERSON3, M_VENDOR_TELNO1, M_VENDOR_TELNO2, M_VENDOR_TELNO3, M_VENDOR_EMAIL1, M_VENDOR_EMAIL2, M_VENDOR_EMAIL3, M_VENDOR_ADDRESS1, M_VENDOR_ADDRESS2, M_VENDOR_COUNTRY FROM M_VENDOR_LIST ORDER BY M_VENDOR_NAME";
            SqlDataReader dr = cmd.ExecuteReader();

            List<VendorDetails> details = new List<VendorDetails>();

            if (dr.HasRows)
            {
                while (dr.Read())
                {
                    VendorDetails cust = new VendorDetails();
                    cust.VendorName = dr["M_VENDOR_NAME"].ToString();
                    cust.ContactPer1 = dr["M_VENDOR_PERSON1"].ToString();
                    cust.Tel1 = dr["M_VENDOR_TELNO1"].ToString();
                    cust.Email1 = dr["M_VENDOR_EMAIL1"].ToString();
                    cust.Email2 = dr["M_VENDOR_EMAIL2"].ToString();
                    cust.Add1 = dr["M_VENDOR_ADDRESS1"].ToString();
                    cust.Add2 = dr["M_VENDOR_ADDRESS2"].ToString();
                    cust.Country = dr["M_VENDOR_COUNTRY"].ToString();
                    details.Add(cust);
                }
            }
            dr.Close();
            dbc.closeConn();

            return details.ToArray();
        }


        [WebMethod, System.Web.Script.Services.ScriptMethod]
        public static McObject<Boolean> deleteVendor(string vName)
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
                string action = MCAction.DELETE;
                int status = MCStatus.PENDING;
                string sql = "DELETE M_VENDOR_LIST WHERE M_VENDOR_NAME = '" + vName + "'";
                                
                cmd.CommandText = "SELECT * FROM M_VENDOR_LIST WHERE M_VENDOR_NAME = '" + vName + "'";
                SqlDataReader dr = cmd.ExecuteReader();

                string oldVendorName = "", oldContact1 = "", oldTel1 = "", oldEmail1 = "",
                    oldEmail2 = "", oldAdd1 = "", oldAdd2 = "", oldCountry = "";

                if (dr.HasRows)
                {
                    if (dr.Read())
                    {
                        oldVendorName = dr["M_VENDOR_NAME"].ToString();
                        oldContact1 = dr["M_VENDOR_PERSON1"].ToString();
                        oldTel1 = dr["M_VENDOR_TELNO1"].ToString();
                        oldEmail1 = dr["M_VENDOR_EMAIL1"].ToString();
                        oldEmail2 = dr["M_VENDOR_EMAIL2"].ToString();
                        oldAdd1 = dr["M_VENDOR_ADDRESS1"].ToString();
                        oldAdd2 = dr["M_VENDOR_ADDRESS2"].ToString();
                        oldCountry = dr["M_VENDOR_COUNTRY"].ToString();
                    }
                }
                else
                {
                    throw new Exception("Record not found.");
                }
                dr.Close();

                JsonObject OLD = new JsonObject();
                OLD.Add("Vendor Name", oldVendorName);
                OLD.Add("Contact Persion", oldContact1);
                OLD.Add("Tel No.", oldTel1);
                OLD.Add("Primary Email", oldEmail1);
                OLD.Add("Secondary Email", oldEmail2);
                OLD.Add("Address 1", oldAdd1);
                OLD.Add("Address 2", oldAdd2);
                OLD.Add("Country", oldCountry);

                string olddata = OLD.ToString();

                cmd.CommandText = "INSERT INTO M_MAKER_CHECKER_LIST(TASK_ID,TASK_NAME,CREATED_DATE,OLD_VALUE,ACTION_TYPE,APPROVE_SQL,MAKER_ID,MAKER_USER_NAME,APPROVAL_STATUS) "
                                    + "VALUES(" + taskID + ", '" + taskName + "', GETDATE(), '" + olddata + "','" + action + "', "
                                    + "@sql, " + sessionUID + ", '" + sessionUName + "', " + status + ")";
                cmd.Parameters.AddWithValue("sql", sql);
                cmd.ExecuteNonQuery();

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

    public class VendorDetails
    {
        public string VendorName { get; set; }
        public string ContactPer1 { get; set; }
        public string Tel1 { get; set; }
        public string Email1 { get; set; }
        public string Email2 { get; set; }
        public string Add1 { get; set; }
        public string Add2 { get; set; }
        public string Country { get; set; }
    }
}