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
    public partial class edit : System.Web.UI.Page
    {
        private static string taskName = "";
        protected void Page_Load(object sender, EventArgs e)
        {
            taskName = Request.Params["task"];

            string smType = Request.Params["pmType"];
            string smModel = Request.Params["pmModel"];
            inputMachineType.Value = smType;
            inputMachineModel.Value = smModel;
            hidOriMType.Value = smType;
            hidOriMModel.Value = smModel;
            getImg(smType, smModel);
        }

        protected void getImg(string mType, string mModel)
        {
            try
            {
                dbconn dbc = new dbconn();
                dbc.connDB();
                SqlCommand cmd = dbc.conn.CreateCommand();
                cmd.CommandText = "SELECT M_MACH_IMGPATH FROM M_MACHINE_TYPE WHERE M_MACH_TYPE = '" + mType + "' AND M_MACH_MODEL = '"+mModel+"'";
                SqlDataReader dr = cmd.ExecuteReader();

                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        imgMach.Attributes["src"] = dr["M_MACH_IMGPATH"].ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                //log
                System.Windows.Forms.MessageBox.Show("Failed to retreive image! \n Error: " + ex.Message);
            }
        }


        [WebMethod]
        public static McObject<Boolean> updateMachine(string OriMType, string OriMModel, string MachineType, string MachineModel, string ImgMach)
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
                if (String.IsNullOrEmpty(taskName))
                {
                    throw new Exception("Invalid Task Name.");
                }

                string sessionUID = HttpContext.Current.Session["userId"].ToString();
                string sessionUName = HttpContext.Current.Session["userName"].ToString();
                string taskID = MKCK_Function.GetTaskID(taskName);
                string action = MCAction.MODIFY;
                int status = MCStatus.PENDING;
                
                //Update only IMAGE because M_MACH_TYPE and M_MACH_MODEL both Primary key
                string sql = "UPDATE M_MACHINE_TYPE SET M_MACH_IMGPATH = '" + ImgMach + "'  WHERE M_MACH_TYPE = '" + OriMType + "' AND M_MACH_MODEL = '" + OriMModel + "'";
                //string sql = "UPDATE M_MACHINE_TYPE SET M_MACH_TYPE = '" + MachineType + "', M_MACH_MODEL = '" + MachineModel + "', M_MACH_IMGPATH = '" + ImgMach + "'  WHERE M_MACH_TYPE = '" + OriMType + "' AND M_MACH_MODEL = '" + OriMModel + "'";
                                
                dbconn dbc = new dbconn();
                dbc.connDB();
                SqlCommand cmd = dbc.conn.CreateCommand();
                cmd.CommandText = "SELECT * FROM M_MACHINE_TYPE WHERE M_MACH_TYPE = '" + OriMType + "' AND M_MACH_MODEL = '" + OriMModel + "'";
                SqlDataReader dr = cmd.ExecuteReader();

                string oldMachineType = "", oldMachineModel = "", oldImage = "";

                if (dr.HasRows)
                {
                    if (dr.Read())
                    {
                        oldMachineType = dr["M_MACH_TYPE"].ToString();
                        oldMachineModel = dr["M_MACH_MODEL"].ToString();
                        oldImage = dr["M_MACH_IMGPATH"].ToString();
                    }
                }
                dr.Close();

                string oldhtmlIMG = "<img class=\"mkck-thumbnail\" src=\"" + oldImage + "\" />";
                string htmlImg = "<img class=\"mkck-thumbnail\" src=\"" + ImgMach + "\" />";

                JsonObject OLD = new JsonObject();
                OLD.Add("Kiosk Type", oldMachineType);
                OLD.Add("Kiosk Model", oldMachineModel);
                OLD.Add("Kiosk Image", oldhtmlIMG);                              

                JsonObject NEW = new JsonObject();
                NEW.Add("Kiosk Type", MachineType);
                NEW.Add("Kiosk Model", MachineModel);
                NEW.Add("Kiosk Image", htmlImg);

                string olddata = OLD.ToString();
                string newdata = NEW.ToString();

                cmd.CommandText = "INSERT INTO M_MAKER_CHECKER_LIST(TASK_ID,TASK_NAME,CREATED_DATE,OLD_VALUE, NEW_VALUE,ACTION_TYPE,APPROVE_SQL,MAKER_ID,MAKER_USER_NAME,APPROVAL_STATUS) "
                                    + "VALUES(" + taskID + ", '" + taskName + "', GETDATE(), @olddata, @newdata, '" + action + "', "
                                    + "@sql, " + sessionUID + ", '" + sessionUName + "', " + status + ")";
                cmd.Parameters.AddWithValue("olddata", olddata);
                cmd.Parameters.AddWithValue("newdata", newdata);
                cmd.Parameters.AddWithValue("sql", sql);
                cmd.ExecuteNonQuery();

                return new McObject<Boolean>(true,"Successful.");
            }
            catch (Exception ex)
            {
                return new McObject<Boolean>(false, "Failed! " + ex.Message);
            }
        }
    
    }
}