using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;
using Maestro.Classes.MakerChecker;
using Maestro.Classes.Prefix;
using System.Json;

namespace Maestro.views.kioskManagement.setupBranch
{
    public partial class main : System.Web.UI.Page
    {
        private static string taskName = "";
        protected void Page_Load(object sender, EventArgs e)
        {
            taskName = Request.Params["task"];
        }

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

        [WebMethod]
        public static Result AddNewBranch(string M_CUST_CODE, string M_STATE, string M_DISTRICT,
         string M_BRANCH_NAME, string M_BRANCH_CODE, string M_ADDRESS1, string M_ADDRESS2, string M_TEL, string M_CONTACT)
        { 
              dbconn DBCon = new dbconn();
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
                  string sql = "INSERT INTO M_BRANCH (M_CUST_CODE, M_STATE, M_DISTRICT ,M_BRANCH_NAME, M_BRANCH_CODE," +
                      "M_ADDRESS1,M_ADDRESS2,M_TEL, M_CONTACT, CREATED_DATE) VALUES ( '"
                      + M_CUST_CODE + "', '" + M_STATE + "', '" + M_DISTRICT + "','" + M_BRANCH_NAME +
                       "','" + M_BRANCH_CODE + "','" + M_ADDRESS1 + "','" + M_ADDRESS2 + "','" + M_TEL + "','" + M_CONTACT +
                      "', GETDATE())";

                  JsonObject NEW = new JsonObject();
                  NEW.Add("Site", M_CUST_CODE);
                  NEW.Add("State", M_STATE);
                  NEW.Add("District", M_DISTRICT);
                  NEW.Add("Branch Name", M_BRANCH_NAME);
                  NEW.Add("Branch Code", M_BRANCH_CODE);
                  NEW.Add("Address 1", (!string.IsNullOrEmpty(M_ADDRESS1)) ? M_ADDRESS1 : "-");
                  NEW.Add("Address 2", (!string.IsNullOrEmpty(M_ADDRESS2)) ? M_ADDRESS2 : "-");
                  NEW.Add("Tel No.", (!string.IsNullOrEmpty(M_TEL)) ? M_TEL : "-");
                  NEW.Add("Contact Person", (!string.IsNullOrEmpty(M_CONTACT)) ? M_CONTACT : "-");

                  string newdata = NEW.ToString();

                  DBCon.connDB();
                  SqlCommand cmd = DBCon.conn.CreateCommand();
                  cmd.CommandText = "INSERT INTO M_MAKER_CHECKER_LIST(TASK_ID,TASK_NAME,CREATED_DATE,NEW_VALUE,ACTION_TYPE,APPROVE_SQL,MAKER_ID,MAKER_USER_NAME,APPROVAL_STATUS) "
                                   + "VALUES(" + taskID + ", '" + taskName + "', GETDATE(), '" + newdata + "','" + action + "', "
                                   + "@sql, " + sessionUID + ", '" + sessionUName + "', " + status + ")";
                  cmd.Parameters.AddWithValue("sql", sql);
                  cmd.ExecuteNonQuery();
                  DBCon.closeConn();
                  
                  return new Result(true, "Successful.");

              }
              catch (SqlException ex)
              {
                  return new Result(false, "Insertion Error : "+ ex.Message + "("+ex.Number+")");
              }
              catch (Exception ex)
              {
                  return new Result(false,"Insertion Error : Other ("+ex.Message+")");
              }
              finally
              {
                  DBCon.closeConn();
              }




        }
    }
}