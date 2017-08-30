using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;
using Maestro.Classes;
using Maestro.Classes.MakerChecker;
using Maestro.Classes.Prefix;
using System.Json;

namespace Maestro.views.administration.vendorManagement
{
    public partial class edit : System.Web.UI.Page
    {
        private static string taskName = "";
        protected void Page_Load(object sender, EventArgs e)
        {
            string sVendorName = Request.Params["VendorName"];
            inputVendor.Value = sVendorName;
            taskName = Request.Params["task"];
        }

        [WebMethod]
        public static VendorDetails[] getVendorByName(string sVendorName)
        {
            List<VendorDetails> details = new List<VendorDetails>();
            try
            {
                dbconn dbc = new dbconn();
                dbc.connDB();
                SqlCommand cmd = dbc.conn.CreateCommand();
                cmd.CommandText = "SELECT M_VENDOR_PERSON1, M_VENDOR_PERSON2, M_VENDOR_PERSON3, M_VENDOR_TELNO1, M_VENDOR_TELNO2, M_VENDOR_TELNO3, M_VENDOR_EMAIL1, M_VENDOR_EMAIL2, M_VENDOR_EMAIL3, M_VENDOR_ADDRESS1, M_VENDOR_ADDRESS2, M_VENDOR_COUNTRY FROM M_VENDOR_LIST WHERE M_VENDOR_NAME = '" + sVendorName + "'";
                SqlDataReader dr = cmd.ExecuteReader();

                
                VendorDetails vDet;
                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        vDet = new VendorDetails();
                        vDet.VendorName = sVendorName;
                        vDet.ContactPer1 = dr["M_VENDOR_PERSON1"].ToString();
                        vDet.Tel1 = dr["M_VENDOR_TELNO1"].ToString();
                        vDet.Email1 = dr["M_VENDOR_EMAIL1"].ToString();
                        vDet.Email2 = dr["M_VENDOR_EMAIL2"].ToString();
                        vDet.Add1 = dr["M_VENDOR_ADDRESS1"].ToString();
                        vDet.Add2 = dr["M_VENDOR_ADDRESS2"].ToString();
                        vDet.Country = dr["M_VENDOR_COUNTRY"].ToString();
                        details.Add(vDet);
                    }
                }
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show("Failed to retriev user details! \r\n Error: " + ex.Message);
            }

            return details.ToArray();
        }

        //[WebMethod, System.Web.Script.Services.ScriptMethod]
        [WebMethod]
        public static McObject<Boolean> updateVendor(string Vendor, string ContactPerson, string TelNo, string PrimaryEmail, string SecondaryEmail, string VendorAddress1, string VendorAddress2, string Country)
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
                string action = MCAction.MODIFY;
                int status = MCStatus.PENDING;
                string sql = "UPDATE M_VENDOR_LIST SET M_VENDOR_PERSON1 = '" + ContactPerson + "', M_VENDOR_TELNO1 = '" + TelNo + "', M_VENDOR_EMAIL1 = '" + PrimaryEmail + "', M_VENDOR_EMAIL2 = '" + SecondaryEmail + "', M_VENDOR_ADDRESS1 = '" + VendorAddress1 + "', M_VENDOR_ADDRESS2 = '" + VendorAddress2 + "', M_VENDOR_COUNTRY = '" + Country + "' WHERE M_VENDOR_NAME = '" + Vendor + "'";
                                
                cmd.CommandText = "SELECT * FROM M_VENDOR_LIST WHERE M_VENDOR_NAME = '" + Vendor + "'";
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
                    throw new Exception("Vendor not found.");
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

                JsonObject NEW = new JsonObject();
                NEW.Add("Vendor Name", Vendor);
                NEW.Add("Contact Persion", ContactPerson);
                NEW.Add("Tel No.", TelNo);
                NEW.Add("Primary Email", PrimaryEmail);
                NEW.Add("Secondary Email", SecondaryEmail);
                NEW.Add("Address 1", VendorAddress1);
                NEW.Add("Address 2", VendorAddress2);
                NEW.Add("Country", Country);

                string olddata = OLD.ToString();
                string newdata = NEW.ToString();

                cmd.CommandText = "INSERT INTO M_MAKER_CHECKER_LIST(TASK_ID,TASK_NAME,CREATED_DATE,OLD_VALUE,NEW_VALUE,ACTION_TYPE,APPROVE_SQL,MAKER_ID,MAKER_USER_NAME,APPROVAL_STATUS) "
                                   + "VALUES(" + taskID + ", '" + taskName + "', GETDATE(), '" + olddata + "','" + newdata + "','" + action + "', "
                                   + "@sql, " + sessionUID + ", '" + sessionUName + "', " + status + ")";
                cmd.Parameters.AddWithValue("sql", sql);
                cmd.ExecuteNonQuery();
                dbc.closeConn();

                return new McObject<bool>(true,"Successful.");
            }
            catch (Exception ex)
            {
                return new McObject<bool>(false, "Failed. " + ex.Message);
            }
        }
    }
}