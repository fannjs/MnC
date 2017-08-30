using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Services;
using Maestro.Classes;
using System.Data.SqlClient;
using System.Json;
using Maestro.Classes.MakerChecker;
using System.Globalization;

namespace Maestro.views.reports.audilTrail
{
    public partial class main : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        public class Module
        {
            public int TaskId { get; set; }
            public string TaskName { get; set; }
        }
        public class Action
        {
            public string ActionType { get; set; }
            public string ActionDesc { get; set; }
        }
        public class Status
        {
            public byte ApprovalStatus { get; set; }
            public string ApprovalDesc { get; set; }
        }
        public class User
        {
            public int UserId { get; set; }
            public string UserName { get; set; }
        }

        [WebMethod]
        public static McObject<List<Module>> GetModuleList()
        {
            dbconn dbc = new dbconn();
            dbc.connDB();
            SqlDataReader dr = null;

            try
            {
                SqlCommand cmd = dbc.conn.CreateCommand();
                cmd.CommandText = "SELECT * FROM M_TASK";
                dr = cmd.ExecuteReader();

                List<Module> ModuleList = new List<Module>();

                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        Module M = new Module();
                        M.TaskId = dr.GetInt32(dr.GetOrdinal("TASK_ID"));
                        M.TaskName = dr.GetString(dr.GetOrdinal("TASK_NAME"));

                        ModuleList.Add(M);
                    }
                }
                else
                {
                    throw new Exception("Module not found. Please contact administrator.");
                }
                dr.Close();

                return new McObject<List<Module>>(true, "Successful.", ModuleList);
            }
            catch (Exception ex)
            {
                return new McObject<List<Module>>(false, "Failed. " + ex.Message);
            }
            finally
            {
                if (dr != null)
                {
                    dr.Close();
                }
                dbc.closeConn();
            }
        }

        [WebMethod]
        public static McObject<List<Status>> GetStatusList()
        {
            dbconn dbc = new dbconn();
            dbc.connDB();
            SqlDataReader dr = null;

            try
            {
                SqlCommand cmd = dbc.conn.CreateCommand();
                cmd.CommandText = "SELECT * FROM M_MAKER_CHECKER_STATUS";
                dr = cmd.ExecuteReader();

                List<Status> StatusList = new List<Status>();

                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        Status S = new Status();
                        S.ApprovalStatus = dr.GetByte(dr.GetOrdinal("APPROVAL_STATUS"));
                        S.ApprovalDesc = dr.GetString(dr.GetOrdinal("APPROVAL_DESC"));

                        StatusList.Add(S);
                    }
                }
                else
                {
                    throw new Exception("Status not found. Please contact administrator.");
                }
                dr.Close();

                return new McObject<List<Status>>(true, "Successful.", StatusList);
            }
            catch (Exception ex)
            {
                return new McObject<List<Status>>(false, "Failed. " + ex.Message);
            }
            finally
            {
                if (dr != null)
                {
                    dr.Close();
                }
                dbc.closeConn();
            }
        }

        [WebMethod]
        public static McObject<List<Action>> GetActionList()
        {
            dbconn dbc = new dbconn();
            dbc.connDB();
            SqlDataReader dr = null;

            try
            {
                SqlCommand cmd = dbc.conn.CreateCommand();
                cmd.CommandText = "SELECT * FROM M_MAKER_CHECKER_ACTION";
                dr = cmd.ExecuteReader();

                List<Action> ActionList = new List<Action>();

                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        Action S = new Action();
                        S.ActionType = dr.GetString(dr.GetOrdinal("ACTION_TYPE"));
                        S.ActionDesc = dr.GetString(dr.GetOrdinal("ACTION_DESC"));

                        ActionList.Add(S);
                    }
                }
                else
                {
                    throw new Exception("Action not found. Please contact administrator.");
                }
                dr.Close();

                return new McObject<List<Action>>(true, "Successful.", ActionList);
            }
            catch (Exception ex)
            {
                return new McObject<List<Action>>(false, "Failed. " + ex.Message);
            }
            finally
            {
                if (dr != null)
                {
                    dr.Close();
                }
                dbc.closeConn();
            }
        }

        [WebMethod]
        public static McObject<List<User>> GetUserList()
        {
            dbconn dbc = new dbconn();
            dbc.connDB();
            SqlDataReader dr = null;

            try
            {
                SqlCommand cmd = dbc.conn.CreateCommand();
                cmd.CommandText = "SELECT * FROM M_USER";
                dr = cmd.ExecuteReader();

                List<User> UserList = new List<User>();

                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        User U = new User();
                        U.UserId = dr.GetInt32(dr.GetOrdinal("M_USER_ID"));
                        U.UserName = dr.GetString(dr.GetOrdinal("M_USER_NAME"));

                        UserList.Add(U);
                    }
                }
                dr.Close();

                return new McObject<List<User>>(true, "Successful.", UserList);
            }
            catch (Exception ex)
            {
                return new McObject<List<User>>(false, "Failed. " + ex.Message);
            }
            finally
            {
                if (dr != null)
                {
                    dr.Close();
                }
                dbc.closeConn();
            }
        }

        [WebMethod]
        public static McObject<Pagination<List<MKCK>>> GetAuditLog(string StartDate, string EndDate, string[] ActionList, string[] StatusList, string Module, string User, string pageNumber, string recordPerPage)
        {
            dbconn dbc = new dbconn();
            dbc.connDB();
            SqlDataReader dr = null;

            try
            {
                if (HttpContext.Current.Session["userId"] == null)
                {
                    throw new Exception("Session timeout. User not found!");
                }
                string userId = HttpContext.Current.Session["userId"].ToString();

                int iPageNumber = int.Parse(pageNumber);
                int iPageSize = int.Parse(recordPerPage);

                if (string.IsNullOrEmpty(StartDate)) { throw new Exception("Invalid Start Date."); }
                if (string.IsNullOrEmpty(EndDate)) { throw new Exception("Invalid End Date."); }

                DateTime startDT = DateTime.ParseExact(StartDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                DateTime endDT = DateTime.ParseExact(EndDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                endDT = endDT.AddDays(1).AddSeconds(-1);

                string ActionFilter = "";
                string StatusFilter = "";
                string ModuleFilter = "";
                string UserFilter = "";

                if (ActionList.Length != 0)
                {
                    List<string> FormattedActionList = new List<string>();
                    foreach (string A in ActionList)
                    {
                        FormattedActionList.Add("'" + A + "'");
                    }
                    ActionFilter = "MCL.ACTION_TYPE IN (" + string.Join(",", FormattedActionList) + ") AND ";
                }
                if (StatusList.Length != 0)
                {
                    StatusFilter = "MCL.APPROVAL_STATUS IN (" + string.Join(",", StatusList) + ") AND ";
                }
                if (!string.IsNullOrEmpty(Module))
                {
                    ModuleFilter = "TASK_ID = " + Module + " AND ";
                }
                if (!string.IsNullOrEmpty(User))
                {
                    UserFilter = "MAKER_ID = " + User + " AND ";
                }

                SqlCommand cmd = dbc.conn.CreateCommand();
                int Count = 0;

                cmd.CommandText = "SELECT COUNT(*) "
                                + "FROM M_MAKER_CHECKER_LIST MCL "
                                + "WHERE " + ActionFilter + StatusFilter + ModuleFilter + UserFilter
                                + "CREATED_DATE BETWEEN @SDate AND @EDate";
                cmd.Parameters.AddWithValue("SDate", startDT);
                cmd.Parameters.AddWithValue("EDate", endDT);
                Count = (Int32)cmd.ExecuteScalar();

                cmd.CommandText = "SELECT * FROM (SELECT ROW_NUMBER() OVER(ORDER BY MCL.CREATED_DATE asc) AS NUMBER, "
                                + "ITEM_ID, TASK_NAME, MCL.CREATED_DATE, ACTION_DESC, MAKER_USER_NAME, CHECKER_USER_NAME, CHECKED_DATE, APPROVAL_DESC "
                                + "FROM M_MAKER_CHECKER_LIST MCL, M_MAKER_CHECKER_STATUS MCS, M_MAKER_CHECKER_ACTION MCA "
                                + "WHERE MCL.ACTION_TYPE = MCA.ACTION_TYPE AND MCL.APPROVAL_STATUS = MCS.APPROVAL_STATUS AND " 
                                + ActionFilter + StatusFilter + ModuleFilter + UserFilter
                                + "MCL.CREATED_DATE BETWEEN @StartDate AND @EndDate) AS MKCKLIST "
                                + "WHERE NUMBER BETWEEN ((@PageNumber - 1) * @PageSize + 1) AND (@PageNumber * @PageSize) "
                                + "ORDER BY CREATED_DATE asc ";
                cmd.Parameters.AddWithValue("PageNumber", iPageNumber);
                cmd.Parameters.AddWithValue("PageSize", iPageSize);
                cmd.Parameters.AddWithValue("StartDate", startDT);
                cmd.Parameters.AddWithValue("EndDate", endDT);
                dr = cmd.ExecuteReader();

                List<MKCK> MKCK_List = new List<MKCK>();

                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        MKCK mkck = new MKCK();
                        mkck.ItemID = dr["ITEM_ID"].ToString();
                        mkck.TaskName = dr["TASK_NAME"].ToString();
                        mkck.CreatedDate = dr.GetDateTime(dr.GetOrdinal("CREATED_DATE"));
                        //mkck.RepliedDate = dr.IsDBNull(dr.GetOrdinal("CHECKED_DATE")) ? (DateTime?)null : dr.GetDateTime(dr.GetOrdinal("CHECKED_DATE"));
                        mkck.Action = dr["ACTION_DESC"].ToString();
                        mkck.MakerName = dr["MAKER_USER_NAME"].ToString();
                        //mkck.CheckerName = dr.IsDBNull(dr.GetOrdinal("CHECKER_USER_NAME")) ? null : dr.GetString(dr.GetOrdinal("CHECKER_USER_NAME"));
                        mkck.MCStatus = dr["APPROVAL_DESC"].ToString();

                        MKCK_List.Add(mkck);
                    }
                }
                dr.Close();

                return new McObject<Pagination<List<MKCK>>>(true, "Successful.", new Pagination<List<MKCK>>(Count, MKCK_List));
            }
            catch (Exception ex)
            {
                return new McObject<Pagination<List<MKCK>>>(false, "Failed. " + ex.Message);
            }
            finally
            {
                if (dr != null)
                {
                    dr.Close();
                }

                dbc.closeConn();
            }
        }

        [WebMethod]
        public static McObject<MKCK> GetLogDetail(string ItemId)
        {
            dbconn dbc = new dbconn();
            dbc.connDB();
            SqlDataReader dr = null;

            try
            {
                SqlCommand cmd = dbc.conn.CreateCommand();
                cmd.CommandText = "SELECT ITEM_ID, TASK_NAME, MCL.CREATED_DATE, CHECKED_DATE, OLD_VALUE, NEW_VALUE, "
                                + "MCL.ACTION_TYPE, ACTION_DESC, MAKER_USER_NAME, MCL.APPROVAL_STATUS, APPROVAL_DESC, "
                                + "CHECKER_USER_NAME, CHECKER_REMARK "
                                + "FROM M_MAKER_CHECKER_LIST MCL, M_MAKER_CHECKER_STATUS MCS, M_MAKER_CHECKER_ACTION MCA "
                                + "WHERE MCL.ACTION_TYPE = MCA.ACTION_TYPE AND MCL.APPROVAL_STATUS = MCS.APPROVAL_STATUS "
                                + "AND ITEM_ID = " + ItemId;
                dr = cmd.ExecuteReader();

                MKCK mkck = new MKCK();

                if (dr.HasRows)
                {
                    if (dr.Read())
                    {
                        mkck.ItemID = dr["ITEM_ID"].ToString();
                        mkck.TaskName = dr["TASK_NAME"].ToString();
                        mkck.CreatedDate = dr.GetDateTime(dr.GetOrdinal("CREATED_DATE"));
                        mkck.RepliedDate = dr.IsDBNull(dr.GetOrdinal("CHECKED_DATE")) ? (DateTime?)null : dr.GetDateTime(dr.GetOrdinal("CHECKED_DATE"));
                        mkck.OldData = dr["OLD_VALUE"].ToString();
                        mkck.NewData = dr["NEW_VALUE"].ToString();
                        mkck.Action = dr["ACTION_TYPE"].ToString();
                        mkck.ActionDesc = dr["ACTION_DESC"].ToString();
                        //mkck.MakerID = dr["MAKER_ID"].ToString();
                        mkck.MakerName = dr["MAKER_USER_NAME"].ToString();
                        mkck.MCStatus = dr["APPROVAL_STATUS"].ToString();
                        mkck.StatusDesc = dr["APPROVAL_DESC"].ToString();
                        //mkck.CheckerID = dr.IsDBNull(dr.GetOrdinal("CHECKER_ID")) ? null : dr["CHECKER_ID"].ToString();
                        mkck.CheckerName = dr.IsDBNull(dr.GetOrdinal("CHECKER_USER_NAME")) ? null : dr.GetString(dr.GetOrdinal("CHECKER_USER_NAME"));
                        mkck.Remark = dr.IsDBNull(dr.GetOrdinal("CHECKER_REMARK")) ? null : dr.GetString(dr.GetOrdinal("CHECKER_REMARK"));
                    }
                }
                
                return new McObject<MKCK>(true, "Successful.", mkck);
            }
            catch (Exception ex)
            {
                return new McObject<MKCK>(false, "Failed. " + ex.Message);
            }
            finally
            {
                if (dr != null)
                {
                    dr.Close();
                }

                dbc.closeConn();
            }
        }
    }
}