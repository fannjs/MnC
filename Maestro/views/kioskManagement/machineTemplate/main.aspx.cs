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
    public partial class main : System.Web.UI.Page
    {
        private static string taskName = "";
        protected void Page_Load(object sender, EventArgs e)
        {
            taskName = Request.Params["task"];
            taskNameHidden.Value = Request.Params["task"];
        }

        [WebMethod, System.Web.Script.Services.ScriptMethod]
        public static McObject<Boolean> deleteMachine(string MachineType, string MachineModel)
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
                if (String.IsNullOrEmpty(taskName))
                {
                    throw new Exception("Invalid Task Name.");
                }

                string sessionUID = HttpContext.Current.Session["userId"].ToString();
                string sessionUName = HttpContext.Current.Session["userName"].ToString();
                string taskID = MKCK_Function.GetTaskID(taskName);
                string action = MCAction.DELETE;
                int status = MCStatus.PENDING;
                string sql = "DELETE M_MACHINE_TYPE WHERE M_MACH_TYPE = '" + MachineType + "' AND M_MACH_MODEL = '" + MachineModel + "'";

                cmd.CommandText = "SELECT * FROM M_MACHINE_TYPE WHERE M_MACH_TYPE = '" + MachineType + "' AND M_MACH_MODEL = '" + MachineModel + "'";
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

                JsonObject OLD = new JsonObject();
                OLD.Add("Kiosk Type", oldMachineType);
                OLD.Add("Kiosk Model", oldMachineModel);
                OLD.Add("Kiosk Image", oldhtmlIMG);

                string olddata = OLD.ToString();

                cmd.CommandText = "INSERT INTO M_MAKER_CHECKER_LIST(TASK_ID,TASK_NAME,CREATED_DATE,OLD_VALUE,ACTION_TYPE,APPROVE_SQL,MAKER_ID,MAKER_USER_NAME,APPROVAL_STATUS) "
                                    + "VALUES(" + taskID + ", '" + taskName + "', GETDATE(), @olddata, '" + action + "', "
                                    + "@sql, " + sessionUID + ", '" + sessionUName + "', " + status + ")";
                cmd.Parameters.AddWithValue("olddata", olddata);
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

        [WebMethod, System.Web.Script.Services.ScriptMethod]
        public static McObject<Boolean> addNewMCode(string MType, string MModel, string ErrorCode, string ErrorDesc, string CodeStatus, string Sop, string CodeCategory, string ImageCodeCategory)
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
                if (String.IsNullOrEmpty(taskName))
                {
                    throw new Exception("Invalid Task Name.");
                }

                string sessionUID = HttpContext.Current.Session["userId"].ToString();
                string sessionUName = HttpContext.Current.Session["userName"].ToString();
                string taskID = MKCK_Function.GetTaskID(taskName);
                string action = MCAction.ADD;
                int status = MCStatus.PENDING;

                //To be more specific, the module name has to be customized so that it more understandable.
                string newTaskName = taskName + " (Status Code)";

                string M_Category = (String.IsNullOrEmpty(CodeCategory)) ? "NULL" : CodeCategory;
                string M_Image = (String.IsNullOrEmpty(ImageCodeCategory)) ? "NULL" : ImageCodeCategory;
                string sql = "INSERT INTO M_CODES (M_MACH_TYPE, M_CODE, M_ERROR_DESCRIPTION, M_ERRORTYPE, M_SOP, M_CATEGORY_ID, M_IMAGE_ID, M_IS_PENDING, CREATED_DATE) VALUES ( '"
                    + MType + "', '" + ErrorCode + "', '" + ErrorDesc + "', '" + CodeStatus + "', '" + Sop + "', " + M_Category + ", " + M_Image + ", 0, GETDATE()) ";

                string categoryName = "";
                if (!String.IsNullOrEmpty(CodeCategory))
                {
                    cmd.CommandText = "SELECT M_CATEGORY_NAME FROM M_CODES_CATEGORY WHERE M_CATEGORY_ID = " + CodeCategory;
                    SqlDataReader dr = cmd.ExecuteReader();

                    if (dr.HasRows)
                    {
                        if (dr.Read())
                        {
                            categoryName = dr["M_CATEGORY_NAME"].ToString();
                        }
                    }
                    dr.Close();
                }

                string imgSrc = "";
                if (!String.IsNullOrEmpty(ImageCodeCategory))
                {
                    cmd.CommandText = "SELECT M_IMAGE_PATH FROM M_CODES_IMAGE_CATEGORY WHERE M_IMAGE_ID = " + ImageCodeCategory;
                    SqlDataReader dr = cmd.ExecuteReader();

                    if (dr.HasRows)
                    {
                        if (dr.Read())
                        {
                            imgSrc = dr["M_IMAGE_PATH"].ToString();
                        }
                    }
                    dr.Close();
                }

                JsonObject NEW = new JsonObject();
                NEW.Add("Kiosk Type", MType);
                NEW.Add("Kiosk Model", MModel);
                NEW.Add("Status Code", ErrorCode);
                NEW.Add("Description", ErrorDesc);
                NEW.Add("Status", CodeStatus);
                NEW.Add("S.O.P", (String.IsNullOrEmpty(Sop)) ? "-" : Sop);
                NEW.Add("Category", (String.IsNullOrEmpty(CodeCategory)) ? "-" : categoryName);
                NEW.Add("Image", (String.IsNullOrEmpty(ImageCodeCategory)) ? "-" : "<img class=\"mkck-thumbnail\" src=\"" + imgSrc + "\" />");

                string newdata = NEW.ToString();

                cmd.CommandText = "INSERT INTO M_MAKER_CHECKER_LIST(TASK_ID,TASK_NAME,CREATED_DATE,NEW_VALUE,ACTION_TYPE,APPROVE_SQL,MAKER_ID,MAKER_USER_NAME,APPROVAL_STATUS) "
                                    + "VALUES(" + taskID + ", '" + newTaskName + "', GETDATE(), @newdata, '" + action + "', "
                                    + "@sql, " + sessionUID + ", '" + sessionUName + "', " + status + ")";
                cmd.Parameters.AddWithValue("newdata", newdata);
                cmd.Parameters.AddWithValue("sql", sql);
                cmd.ExecuteNonQuery();

                return new McObject<bool>(true, "Successful.");
            }
            catch (Exception ex)
            {
                return new McObject<bool>(false, "Failed! " + ex.Message);
            }
            finally
            {
                dbc.closeConn();
            }
        }

        [WebMethod, System.Web.Script.Services.ScriptMethod]
        public static McObject<Boolean> updateMCode(string OriMCode, string MType, string MModel, string ErrorCode, string ErrorDesc, string CodeStatus, string Sop, string CodeCategory, string ImageID)
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
                if (String.IsNullOrEmpty(taskName))
                {
                    throw new Exception("Invalid Task Name.");
                }

                string sessionUID = HttpContext.Current.Session["userId"].ToString();
                string sessionUName = HttpContext.Current.Session["userName"].ToString();
                string taskID = MKCK_Function.GetTaskID(taskName);
                string action = MCAction.MODIFY;
                int status = MCStatus.PENDING;

                //To be more specific, the module name has to be customized so that it more understandable.
                string newTaskName = taskName + " (Status Code)";

                string M_Category = (String.IsNullOrEmpty(CodeCategory)) ? "NULL" : CodeCategory;
                string M_Image = (String.IsNullOrEmpty(ImageID)) ? "NULL" : ImageID;
                string sql = "UPDATE M_CODES SET M_CODE = '" + ErrorCode + "' , M_ERROR_DESCRIPTION = '" + ErrorDesc + "' , M_SOP = '" + Sop +
                    "' , M_ERRORTYPE  = '" + CodeStatus + "', M_CATEGORY_ID = " + M_Category + ", M_IMAGE_ID = " + M_Image + " WHERE M_MACH_TYPE = '" + MType + "' AND M_MACH_MODEL = '" + MModel + "' AND M_CODE = '" + OriMCode + "'";

                cmd.CommandText = "SELECT C.M_ERROR_DESCRIPTION, C.M_SOP, C.M_ERRORTYPE, CC.M_CATEGORY_NAME, ICC.M_IMAGE_PATH " +
                                    "FROM M_CODES C " +
                                    "LEFT JOIN M_CODES_CATEGORY CC ON C.M_CATEGORY_ID = CC.M_CATEGORY_ID " +
                                    "LEFT JOIN M_CODES_IMAGE_CATEGORY ICC ON C.M_IMAGE_ID = ICC.M_IMAGE_ID " +
                                    "WHERE M_MACH_TYPE = '" + MType + "' AND M_MACH_MODEL = '" + MModel + "' AND M_CODE = '" + OriMCode + "'";

                SqlDataReader dr = cmd.ExecuteReader();

                string oldErrDesc = "", oldSOP = "", oldErrType = "", oldCC = "", oldCCImage = "";

                if (dr.HasRows)
                {
                    if (dr.Read())
                    {
                        oldErrDesc = dr["M_ERROR_DESCRIPTION"].ToString();
                        oldErrType = dr["M_ERRORTYPE"].ToString();
                        oldSOP = dr["M_SOP"].ToString();
                        oldCC = (dr.IsDBNull(dr.GetOrdinal("M_CATEGORY_NAME"))) ? "" : dr["M_CATEGORY_NAME"].ToString();
                        oldCCImage = (dr.IsDBNull(dr.GetOrdinal("M_IMAGE_PATH"))) ? "" : dr["M_IMAGE_PATH"].ToString();
                    }
                }
                dr.Close();

                JsonObject OLD = new JsonObject();
                OLD.Add("Kiosk Type", MType);
                OLD.Add("Kiosk Model", MModel);
                OLD.Add("Status Code", OriMCode);
                OLD.Add("Description", oldErrDesc);
                OLD.Add("Status", oldErrType);
                OLD.Add("S.O.P", (String.IsNullOrEmpty(oldSOP)) ? "-" : oldSOP);
                OLD.Add("Category", (String.IsNullOrEmpty(oldCC)) ? "-" : oldCC);
                OLD.Add("Image", (String.IsNullOrEmpty(oldCCImage)) ? "-" : "<img class=\"mkck-thumbnail\" src=\"" + oldCCImage + "\" />");

                string categoryName = "";
                if (!String.IsNullOrEmpty(CodeCategory))
                {
                    cmd.CommandText = "SELECT M_CATEGORY_NAME FROM M_CODES_CATEGORY WHERE M_CATEGORY_ID = " + CodeCategory;
                    dr = cmd.ExecuteReader();

                    if (dr.HasRows)
                    {
                        if (dr.Read())
                        {
                            categoryName = dr["M_CATEGORY_NAME"].ToString();
                        }
                    }
                    dr.Close();
                }

                string imgSrc = "";
                if (!String.IsNullOrEmpty(ImageID))
                {
                    cmd.CommandText = "SELECT M_IMAGE_PATH FROM M_CODES_IMAGE_CATEGORY WHERE M_IMAGE_ID = " + ImageID;
                    dr = cmd.ExecuteReader();

                    if (dr.HasRows)
                    {
                        if (dr.Read())
                        {
                            imgSrc = dr["M_IMAGE_PATH"].ToString();
                        }
                    }
                    dr.Close();
                }

                JsonObject NEW = new JsonObject();
                NEW.Add("Kiosk Type", MType);
                NEW.Add("Kiosk Model", MModel);
                NEW.Add("Status Code", ErrorCode);
                NEW.Add("Description", ErrorDesc);
                NEW.Add("Status", CodeStatus);
                NEW.Add("S.O.P", (String.IsNullOrEmpty(Sop)) ? "-" : Sop);
                NEW.Add("Category", (String.IsNullOrEmpty(CodeCategory)) ? "-" : categoryName);
                NEW.Add("Image", (String.IsNullOrEmpty(ImageID)) ? "-" : "<img class=\"mkck-thumbnail\" src=\"" + imgSrc + "\" />");

                string olddata = OLD.ToString();
                string newdata = NEW.ToString();

                cmd.CommandText = "INSERT INTO M_MAKER_CHECKER_LIST(TASK_ID,TASK_NAME,CREATED_DATE,OLD_VALUE,NEW_VALUE,ACTION_TYPE,APPROVE_SQL,MAKER_ID,MAKER_USER_NAME,APPROVAL_STATUS) "
                                    + "VALUES(" + taskID + ", '" + newTaskName + "', GETDATE(), @olddata, @newdata, '" + action + "', "
                                    + "@sql, " + sessionUID + ", '" + sessionUName + "', " + status + ")";
                cmd.Parameters.AddWithValue("olddata", olddata);
                cmd.Parameters.AddWithValue("newdata", newdata);
                cmd.Parameters.AddWithValue("sql", sql);
                cmd.ExecuteNonQuery();

                return new McObject<bool>(true, "Successful");
            }
            catch (Exception ex)
            {
                return new McObject<bool>(false, "Failed! " + ex.Message);
            }
            finally
            {
                dbc.closeConn();
            }
        }

        [WebMethod, System.Web.Script.Services.ScriptMethod]
        public static McObject<Boolean> deleteMCode(string MType, string MModel, string MCode)
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
                if (String.IsNullOrEmpty(taskName))
                {
                    throw new Exception("Invalid Task Name.");
                }

                string sessionUID = HttpContext.Current.Session["userId"].ToString();
                string sessionUName = HttpContext.Current.Session["userName"].ToString();
                string taskID = MKCK_Function.GetTaskID(taskName);
                string action = MCAction.DELETE;
                int status = MCStatus.PENDING;

                //To be more specific, the module name has to be customized so that it more understandable.
                string newTaskName = taskName + " (Status Code)";

                string sql = "DELETE M_CODES WHERE M_MACH_TYPE = '" + MType + "' AND M_CODE = '" + MCode + "'";

                cmd.CommandText = "SELECT C.M_ERROR_DESCRIPTION, C.M_SOP, C.M_ERRORTYPE, CC.M_CATEGORY_NAME, ICC.M_IMAGE_PATH " +
                                    "FROM M_CODES C " +
                                    "LEFT JOIN M_CODES_CATEGORY CC ON C.M_CATEGORY_ID = CC.M_CATEGORY_ID " +
                                    "LEFT JOIN M_CODES_IMAGE_CATEGORY ICC ON C.M_IMAGE_ID = ICC.M_IMAGE_ID " +
                                    "WHERE C.M_MACH_TYPE = '" + MType + "' AND C.M_CODE = '" + MCode + "'";

                SqlDataReader dr = cmd.ExecuteReader();

                string oldErrDesc = "", oldSOP = "", oldErrType = "", oldCC = "", oldCCImage = "";

                if (dr.HasRows)
                {
                    if (dr.Read())
                    {
                        oldErrDesc = dr["M_ERROR_DESCRIPTION"].ToString();
                        oldErrType = dr["M_ERRORTYPE"].ToString();
                        oldSOP = dr["M_SOP"].ToString();
                        oldCC = (dr.IsDBNull(dr.GetOrdinal("M_CATEGORY_NAME"))) ? "" : dr["M_CATEGORY_NAME"].ToString();
                        oldCCImage = (dr.IsDBNull(dr.GetOrdinal("M_IMAGE_PATH"))) ? "" : dr["M_IMAGE_PATH"].ToString();
                    }
                }
                dr.Close();

                JsonObject OLD = new JsonObject();
                OLD.Add("Kiosk Type", MType);
                OLD.Add("Kiosk Model", MModel);
                OLD.Add("Status Code", MCode);
                OLD.Add("Description", oldErrDesc);
                OLD.Add("Status", oldErrType);
                OLD.Add("S.O.P", (String.IsNullOrEmpty(oldSOP)) ? "-" : oldSOP);
                OLD.Add("Category", (String.IsNullOrEmpty(oldCC)) ? "-" : oldCC);
                OLD.Add("Image", (String.IsNullOrEmpty(oldCCImage)) ? "-" : "<img class=\"mkck-thumbnail\" src=\"" + oldCCImage + "\" />");

                string olddata = OLD.ToString();

                cmd.CommandText = "INSERT INTO M_MAKER_CHECKER_LIST(TASK_ID,TASK_NAME,CREATED_DATE,OLD_VALUE,ACTION_TYPE,APPROVE_SQL,MAKER_ID,MAKER_USER_NAME,APPROVAL_STATUS) "
                                    + "VALUES(" + taskID + ", '" + newTaskName + "', GETDATE(), @olddata, '" + action + "', "
                                    + "@sql, " + sessionUID + ", '" + sessionUName + "', " + status + ")";
                cmd.Parameters.AddWithValue("olddata", olddata);
                cmd.Parameters.AddWithValue("sql", sql);
                cmd.ExecuteNonQuery();

                return new McObject<bool>(true, "Successful.");
            }
            catch (Exception ex)
            {
                return new McObject<bool>(false, "Failed! " + ex.Message);
            }
            finally
            {
                dbc.closeConn();
            }            
        }

        [WebMethod]
        public static CodeCategory[] getCC()
        {
            dbconn dbc = new dbconn();
            dbc.connDB();
            SqlCommand cmd = dbc.conn.CreateCommand();
            cmd.CommandText = "SELECT M_CATEGORY_ID, M_CATEGORY_NAME " +
                                "FROM M_CODES_CATEGORY " +
                                "ORDER BY M_CATEGORY_NAME asc";

            SqlDataReader dr = cmd.ExecuteReader();
            List<CodeCategory> ccList = new List<CodeCategory>();

            if (dr.HasRows)
            {
                while (dr.Read())
                {
                    CodeCategory cc = new CodeCategory();
                    cc.CCID = dr["M_CATEGORY_ID"].ToString().Trim();
                    cc.CCName = dr["M_CATEGORY_NAME"].ToString().Trim();

                    ccList.Add(cc);
                }
            }

            dr.Close();
            dbc.closeConn();
            return ccList.ToArray();
        }

        /* Code Category functions */
        public class Result
        {
            public Result(bool status, string des)
            {
                Status = status;
                Description = des;
            }
            public bool Status { get; set; }
            public string Description { get; set; }
 
        }

        public class CodeCategory
        {
            public string CCID {get;set;}
            public string CCName {get;set;}
            public string CCDesc {get;set;}
            public string CCType {get;set;}
            public string TotalImages { get; set; }
        }

        public class ImageCodeCategory
        {
            public string ImgID { get; set; }
            public string ImgPath { get; set; }
            public string HasSensor { get; set; }
        }
        public class Machine
        {
            public string MType {get;set;}
        }

        [WebMethod]
        public static CodeCategory[] loadCodeCategory()
        {
            dbconn dbc = new dbconn();
            dbc.connDB();
            SqlCommand cmd = dbc.conn.CreateCommand();
            cmd.CommandText = "SELECT CC.M_CATEGORY_ID, M_CATEGORY_NAME, M_CATEGORY_DESC, M_MACH_TYPE, count(M_IMAGE_ID) AS 'Total Images' " +
                                "FROM M_CODES_CATEGORY CC " +
                                "LEFT JOIN M_CODES_IMAGE_CATEGORY ICC " +
                                "ON CC.M_CATEGORY_ID = ICC.M_CATEGORY_ID " +
                                "GROUP BY CC.M_CATEGORY_ID, M_CATEGORY_NAME, M_CATEGORY_DESC, M_MACH_TYPE " +
                                "ORDER BY M_CATEGORY_NAME asc";
            
            SqlDataReader dr = cmd.ExecuteReader();
            List<CodeCategory> ccList = new List<CodeCategory>();

            if (dr.HasRows)
            {
                while (dr.Read())
                {
                    CodeCategory cc = new CodeCategory();
                    cc.CCID = dr["M_CATEGORY_ID"].ToString().Trim();
                    cc.CCName = dr["M_CATEGORY_NAME"].ToString().Trim();
                    cc.CCDesc = dr["M_CATEGORY_DESC"].ToString().Trim();
                    cc.CCType = dr["M_MACH_TYPE"].ToString().Trim();
                    cc.TotalImages = dr["Total Images"].ToString().Trim();

                    ccList.Add(cc);
                }
            }

            dr.Close();
            dbc.closeConn();
            return ccList.ToArray();
        }

        [WebMethod]
        public static Machine[] loadMachineType()
        {
            dbconn dbc = new dbconn();
            dbc.connDB();
            SqlCommand cmd = dbc.conn.CreateCommand();
            cmd.CommandText = "SELECT M_MACH_TYPE FROM M_MACHINE_TYPE";

            SqlDataReader dr = cmd.ExecuteReader();
            List<Machine> mtList = new List<Machine>();

            if (dr.HasRows)
            {
                while (dr.Read())
                {
                    Machine mt = new Machine();
                    mt.MType = dr["M_MACH_TYPE"].ToString().Trim();

                    mtList.Add(mt);
                }
            }
            dr.Close();
            dbc.closeConn();
            return mtList.ToArray();
        }

        [WebMethod]
        public static Result insertNewCodeCategory(string ccName, string ccDesc, string ccType, object[] ccImages)
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
                if (String.IsNullOrEmpty(taskName))
                {
                    throw new Exception("Invalid Task Name.");
                }

                string sessionUID = HttpContext.Current.Session["userId"].ToString();
                string sessionUName = HttpContext.Current.Session["userName"].ToString();
                string taskID = MKCK_Function.GetTaskID(taskName);
                string action = MCAction.ADD;
                int status = MCStatus.PENDING;

                //To be more specific, the module name has to be customized so that it more understandable.
                string newTaskName = taskName + " (Code Category)";

                List<string> imgArr = new List<string>();

                //Formatting SQL
                string sql = "DECLARE @id int " +
                             "INSERT INTO M_CODES_CATEGORY(M_CATEGORY_NAME, M_CATEGORY_DESC, M_MACH_TYPE, CREATED_DATE) " +
                             "VALUES('" + ccName + "', '" + ccDesc + "', '" + ccType + "',GETDATE()) " +
                             "SET @id = SCOPE_IDENTITY() ";
                foreach (Dictionary<string, object> img in ccImages)
                {
                    object imgSrc;
                    img.TryGetValue("src", out imgSrc);

                    sql = sql + "INSERT INTO M_CODES_IMAGE_CATEGORY(M_CATEGORY_ID, M_IMAGE_PATH, CREATED_DATE) VALUES(@id, '" + imgSrc.ToString() + "', GETDATE())";

                    imgArr.Add(imgSrc.ToString());
                }                

                string htmlImages = "";
                foreach (var img in imgArr)
                {
                    htmlImages += "<div class=\"mkck-thumbnail-div\"><img class=\"mkck-thumbnail\" src=\"" + img + "\" /></div>";
                }

                JsonObject NEW = new JsonObject();
                NEW.Add("Category", ccName);
                NEW.Add("Description", ccDesc);
                //NEW.Add("Kiosk Type", ccType);
                NEW.Add("Images", htmlImages);

                string newdata = NEW.ToString();

                cmd.CommandText = "INSERT INTO M_MAKER_CHECKER_LIST(TASK_ID,TASK_NAME,CREATED_DATE,NEW_VALUE,ACTION_TYPE,APPROVE_SQL,MAKER_ID,MAKER_USER_NAME,APPROVAL_STATUS) "
                                    + "VALUES(" + taskID + ", '" + newTaskName + "', GETDATE(), @newdata, '" + action + "', "
                                    + "@sql, " + sessionUID + ", '" + sessionUName + "', " + status + ")";
                cmd.Parameters.AddWithValue("newdata", newdata);
                cmd.Parameters.AddWithValue("sql", sql);
                cmd.ExecuteNonQuery();
             
                return new Result(true, "Successful.");
            }
            catch (Exception ex)
            {
                return new Result(false, "Failed. " + ex.Message);
            }
            finally
            {
                dbc.closeConn();
            }
        }

        [WebMethod]
        public static CodeCategory loadCodeCategoryDetail(string ccID)
        {
            int categoryID = int.Parse(ccID);

            dbconn dbc = new dbconn();
            dbc.connDB();
            SqlCommand cmd = dbc.conn.CreateCommand();

            cmd.CommandText = "SELECT M_CATEGORY_NAME, M_CATEGORY_DESC, M_MACH_TYPE FROM M_CODES_CATEGORY WHERE M_CATEGORY_ID = " + categoryID;

            SqlDataReader dr = cmd.ExecuteReader();
            CodeCategory cc = new CodeCategory();

            dr.Read();
            cc.CCName = dr["M_CATEGORY_NAME"].ToString().Trim();
            cc.CCDesc = dr["M_CATEGORY_DESC"].ToString().Trim();
            cc.CCType = dr["M_MACH_TYPE"].ToString().Trim();
            dr.Close();
            dbc.closeConn();

            return cc;
        }

        [WebMethod]
        public static ImageCodeCategory[] loadImageCodeCategory(string ccID)
        {
            List<ImageCodeCategory> iccList = new List<ImageCodeCategory>();

            if (ccID == "")
            {
                return iccList.ToArray();
            }
            else
            {
                int categoryID = int.Parse(ccID);

                dbconn dbc = new dbconn();
                dbc.connDB();
                SqlCommand cmd = dbc.conn.CreateCommand();
                cmd.CommandText = "SELECT M_IMAGE_ID, M_IMAGE_PATH FROM M_CODES_IMAGE_CATEGORY WHERE M_CATEGORY_ID = " + categoryID;

                SqlDataReader dr = cmd.ExecuteReader();

                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        ImageCodeCategory icc = new ImageCodeCategory();
                        icc.ImgID = dr["M_IMAGE_ID"].ToString().Trim();
                        icc.ImgPath = dr["M_IMAGE_PATH"].ToString().Trim();
                        iccList.Add(icc);
                    }
                }

                dr.Close();
                dbc.closeConn();

                return iccList.ToArray();
            }
        }

        [WebMethod]
        public static Result updateCodeCategory(string ccID, string ccName, string ccDesc, string machType)
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
                if (String.IsNullOrEmpty(taskName))
                {
                    throw new Exception("Invalid Task Name.");
                }

                string sessionUID = HttpContext.Current.Session["userId"].ToString();
                string sessionUName = HttpContext.Current.Session["userName"].ToString();
                string taskID = MKCK_Function.GetTaskID(taskName);
                string action = MCAction.MODIFY;
                int status = MCStatus.PENDING;

                //To be more specific, the module name has to be customized so that it more understandable.
                string newTaskName = taskName + " (Code Category)";

                string sql = "UPDATE M_CODES_CATEGORY SET M_CATEGORY_NAME = '" + ccName + "', M_CATEGORY_DESC = '" + ccDesc + "', M_MACH_TYPE = '" + machType + "' WHERE M_CATEGORY_ID = " + ccID;

                cmd.CommandText = "SELECT M_CATEGORY_NAME, M_CATEGORY_DESC, M_MACH_TYPE FROM M_CODES_CATEGORY WHERE M_CATEGORY_ID = " + ccID;
                SqlDataReader dr = cmd.ExecuteReader();

                string oldCategoryName = "", oldCategoryDesc = "", oldMachineType = "";

                if (dr.HasRows)
                {
                    if (dr.Read())
                    {
                        oldCategoryName = dr["M_CATEGORY_NAME"].ToString();
                        oldCategoryDesc = dr["M_CATEGORY_DESC"].ToString();
                        oldMachineType = dr["M_MACH_TYPE"].ToString();
                    }
                }
                dr.Close();

                JsonObject OLD = new JsonObject();
                OLD.Add("Category", oldCategoryName);
                OLD.Add("Description", oldCategoryDesc);
                OLD.Add("Kiosk Type", oldMachineType);

                JsonObject NEW = new JsonObject();
                NEW.Add("Category", ccName);
                NEW.Add("Description", ccDesc);
                NEW.Add("Kiosk Type", machType);

                string olddata = OLD.ToString();
                string newdata = NEW.ToString();

                cmd.CommandText = "INSERT INTO M_MAKER_CHECKER_LIST(TASK_ID,TASK_NAME,CREATED_DATE,OLD_VALUE,NEW_VALUE,ACTION_TYPE,APPROVE_SQL,MAKER_ID,MAKER_USER_NAME,APPROVAL_STATUS) "
                                    + "VALUES(" + taskID + ", '" + newTaskName + "', GETDATE(),@olddata, @newdata, '" + action + "', "
                                    + "@sql, " + sessionUID + ", '" + sessionUName + "', " + status + ")";
                cmd.Parameters.AddWithValue("olddata", olddata);
                cmd.Parameters.AddWithValue("newdata", newdata);
                cmd.Parameters.AddWithValue("sql", sql);
                cmd.ExecuteNonQuery();

                return new Result(true, "Updated successful.");
            }
            catch (Exception ex)
            {
                return new Result(false, "Failed. " + ex.Message);
            }
            finally
            {
                dbc.closeConn();
            }
        }

        [WebMethod]
        public static Result uploadImages(string ccID, object[] ccImages)
        {
            dbconn dbc = new dbconn();
            dbc.connDB();

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
                string action = MCAction.ADD;
                int status = MCStatus.PENDING;

                //To be more specific, the module name has to be customized so that it more understandable.
                string newTaskName = taskName + " (Code Category Image)";

                string sql = "";
                List<string> imgArr = new List<string>();

                foreach (Dictionary<string, object> image in ccImages)
                {
                    object imgSrc;
                    image.TryGetValue("src", out imgSrc);

                    sql += "INSERT INTO M_CODES_IMAGE_CATEGORY(M_CATEGORY_ID, M_IMAGE_PATH, CREATED_DATE) VALUES (" + ccID + ", '" + imgSrc.ToString() + "', GETDATE())";
                    imgArr.Add(imgSrc.ToString());
                }

                SqlCommand cmd = dbc.conn.CreateCommand();
                cmd.CommandText = "SELECT M_CATEGORY_NAME FROM M_CODES_CATEGORY WHERE M_CATEGORY_ID = " + ccID;
                SqlDataReader dr = cmd.ExecuteReader();

                string ccName = "";

                if (dr.HasRows)
                {
                    if (dr.Read())
                    {
                        ccName = dr["M_CATEGORY_NAME"].ToString();
                    }
                }
                dr.Close();

                string htmlImages = "";
                foreach (var img in imgArr)
                {
                    htmlImages += "<div class=\"mkck-thumbnail-div\"><img class=\"mkck-thumbnail\" src=\"" + img + "\" /></div>";
                }

                JsonObject NEW = new JsonObject();
                NEW.Add("Category", ccName);
                NEW.Add("New Image(s)", htmlImages);

                string newdata = NEW.ToString();

                cmd.CommandText = "INSERT INTO M_MAKER_CHECKER_LIST(TASK_ID,TASK_NAME,CREATED_DATE,NEW_VALUE,ACTION_TYPE,APPROVE_SQL,MAKER_ID,MAKER_USER_NAME,APPROVAL_STATUS) "
                                    + "VALUES(" + taskID + ", '" + newTaskName + "', GETDATE(), @newdata, '" + action + "', "
                                    + "@sql, " + sessionUID + ", '" + sessionUName + "', " + status + ")";
                cmd.Parameters.AddWithValue("newdata", newdata);
                cmd.Parameters.AddWithValue("sql", sql);
                cmd.ExecuteNonQuery();

                return new Result(true, "Uploaded successful.");
            }
            catch (Exception ex)
            {
                return new Result(false, "Failed. " + Environment.NewLine + ex.Message);
            }
            finally
            {
                dbc.closeConn();
            }
        }

        [WebMethod]
        public static Result removeImages(object[] selectedImages)
        {            
            dbconn dbc = new dbconn();
            dbc.connDB();
            SqlCommand cmd = dbc.conn.CreateCommand();
            //SqlTransaction trans = dbc.conn.BeginTransaction();
            //cmd.Connection = dbc.conn;
            //cmd.Transaction = trans;

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
                string action = MCAction.DELETE;
                int status = MCStatus.PENDING;

                //To be more specific, the module name has to be customized so that it more understandable.
                string newTaskName = taskName + " (Code Category Image)";

                string sql = "";
                List<string> idList = new List<string>();

                foreach (Dictionary<string, object> image in selectedImages)
                {
                    object imgID;
                    image.TryGetValue("imgID", out imgID);

                    cmd.CommandText = "SELECT COUNT(*) FROM M_CODES WHERE M_IMAGE_ID = " + imgID.ToString();
                    int count = (int)cmd.ExecuteScalar();

                    if (count > 0)
                    {
                        throw new Exception("Image is using by " + count + " Error Code(s). " + Environment.NewLine + "Please remove from there first.");
                    }
                    else
                    {
                        sql += "DELETE FROM M_CODES_IMAGE_CATEGORY WHERE M_IMAGE_ID = " + imgID.ToString();
                        idList.Add(imgID.ToString());
                    }
                }

                string condition = string.Join(",", idList.ToArray());
                cmd.CommandText = "SELECT M_IMAGE_PATH, M_CATEGORY_NAME " +
                                "FROM M_CODES_IMAGE_CATEGORY ICC, M_CODES_CATEGORY CC " +
                                "WHERE ICC.M_CATEGORY_ID = CC.M_CATEGORY_ID AND M_IMAGE_ID IN(" + condition + ")";
                SqlDataReader dr = cmd.ExecuteReader();

                string ccName = "";
                string htmlImages = "";
                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        ccName = dr["M_CATEGORY_NAME"].ToString();
                        htmlImages += "<div class=\"mkck-thumbnail-div\"><img class=\"mkck-thumbnail\" src=\"" + dr["M_IMAGE_PATH"].ToString() +"\" /></div>";
                    }
                }
                dr.Close();

                JsonObject OLD = new JsonObject();
                OLD.Add("Category", ccName);
                OLD.Add("New Image(s)", htmlImages);

                string olddata = OLD.ToString();

                cmd.CommandText = "INSERT INTO M_MAKER_CHECKER_LIST(TASK_ID,TASK_NAME,CREATED_DATE,OLD_VALUE,ACTION_TYPE,APPROVE_SQL,MAKER_ID,MAKER_USER_NAME,APPROVAL_STATUS) "
                                    + "VALUES(" + taskID + ", '" + newTaskName + "', GETDATE(), @olddata, '" + action + "', "
                                    + "@sql, " + sessionUID + ", '" + sessionUName + "', " + status + ")";
                cmd.Parameters.AddWithValue("olddata", olddata);
                cmd.Parameters.AddWithValue("sql", sql);
                cmd.ExecuteNonQuery();

                //trans.Commit();

                return new Result(true, "Selected Image(s) has been removed successfully.");
            }
            catch (Exception ex)
            {
                //trans.Rollback();
                return new Result(false, "Failed. " + ex.Message);
            }
            finally
            {
                dbc.closeConn();
            }
        }

        [WebMethod]
        public static Result deleteCodeCategory(string ccID)
        {
            dbconn dbc = new dbconn();
            dbc.connDB();
            SqlCommand cmd = dbc.conn.CreateCommand();
            //SqlTransaction trans = dbc.conn.BeginTransaction();
            //cmd.Connection = dbc.conn;
            //cmd.Transaction = trans;

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
                string action = MCAction.DELETE;
                int status = MCStatus.PENDING;

                //To be more specific, the module name has to be customized so that it more understandable.
                string newTaskName = taskName + " (Code Category)";

                string sql = "";

                cmd.CommandText = "SELECT COUNT(*) FROM M_CODES WHERE M_CATEGORY_ID = " + ccID;
                int count = (int)cmd.ExecuteScalar();

                if (count > 0)
                {
                    throw new Exception(count + " Error Code(s) using this Category. " + Environment.NewLine + "Please remove from there first.");
                }
                else
                {
                    sql += "DELETE FROM M_CODES_CATEGORY WHERE M_CATEGORY_ID = " + ccID;
                    sql += MCDelimiter.SQL;
                    sql += "DELETE FROM M_CODES_IMAGE_CATEGORY WHERE M_CATEGORY_ID = " + ccID;
                }

                cmd.CommandText = "SELECT M_CATEGORY_NAME, M_CATEGORY_DESC, M_MACH_TYPE FROM M_CODES_CATEGORY WHERE M_CATEGORY_ID = " + ccID;
                SqlDataReader dr = cmd.ExecuteReader();

                string oldCategoryName = "", oldCategoryDesc = "", oldMachineType = "";

                if (dr.HasRows)
                {
                    if (dr.Read())
                    {
                        oldCategoryName = dr["M_CATEGORY_NAME"].ToString();
                        oldCategoryDesc = dr["M_CATEGORY_DESC"].ToString();
                        oldMachineType = dr["M_MACH_TYPE"].ToString();
                    }
                }
                dr.Close();

                JsonObject OLD = new JsonObject();
                OLD.Add("Category", oldCategoryName);
                OLD.Add("Description", oldCategoryDesc);
                OLD.Add("Kiosk Type", oldMachineType);

                string olddata = OLD.ToString();

                cmd.CommandText = "INSERT INTO M_MAKER_CHECKER_LIST(TASK_ID,TASK_NAME,CREATED_DATE,OLD_VALUE,ACTION_TYPE,APPROVE_SQL,MAKER_ID,MAKER_USER_NAME,APPROVAL_STATUS) "
                                    + "VALUES(" + taskID + ", '" + newTaskName + "', GETDATE(), @olddata, '" + action + "', "
                                    + "@sql, " + sessionUID + ", '" + sessionUName + "', " + status + ")";
                cmd.Parameters.AddWithValue("olddata", olddata);
                cmd.Parameters.AddWithValue("sql", sql);
                cmd.ExecuteNonQuery();

                //trans.Commit();

                return new Result(true, "Deleted successful.");
            }
            catch (Exception ex)
            {
                //trans.Rollback();
                return new Result(false, "Failed. " + Environment.NewLine + ex.Message);
            }
            finally
            {
                dbc.closeConn();
            }
        }
    }
}