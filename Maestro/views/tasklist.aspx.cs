using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Services;
using System.Data.SqlClient;
using Maestro.Classes;
using Maestro.Classes.MakerChecker;
using Maestro.Classes.Prefix;

namespace Maestro.views
{
    public partial class tasklist : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            
        }
        /* SQL Condition
         *  - Maker ID cannot be the current User ID
         *  - Task ID must be IN ( Maker's user access )
         *  - Maker's permission on the module [CREATE or MODIFY or DELETE], either 1
         */ 
        [WebMethod]
        public static McObject<Pagination<List<MKCK>>> GetCheckerTaskList(string pageNumber, string recordPerPage)
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

                SqlCommand cmd = dbc.conn.CreateCommand();
                cmd.CommandText = "SELECT MRT.TASK_ID FROM M_USER_ROLE MUR, M_ROLE_TASK MRT "
                                + "WHERE MUR.ROLE_ID = MRT.ROLE_ID "
                                + "AND (MRT.PERMISSION_CREATE = 1 OR MRT.PERMISSION_UPDATE = 1 OR MRT.PERMISSION_DELETE = 1) "
                                + "AND MUR.M_USER_ID = " + userId;

                dr = cmd.ExecuteReader();
                List<string> taskIdList = new List<string>();

                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        taskIdList.Add(dr["TASK_ID"].ToString());
                    }
                }
                dr.Close();

                List<MKCK> CheckerList = new List<MKCK>();
                int Count = 0;

                string TaskStr = string.Join(",", taskIdList);
                if (!string.IsNullOrEmpty(TaskStr))
                {
                    //Get Total Count
                    cmd.CommandText = "SELECT COUNT(*) "
                                    + "FROM M_MAKER_CHECKER_LIST "
                                    + "WHERE APPROVAL_STATUS = " + MCStatus.PENDING + " "
                                    + "AND TASK_ID IN(" + TaskStr + ") "
                                    + "AND MAKER_ID != " + userId + " ";
                    Count = (Int32)cmd.ExecuteScalar();

                    cmd.CommandText = "SELECT * FROM (SELECT ROW_NUMBER() OVER(ORDER BY CREATED_DATE desc) AS NUMBER, "
                                    + "ITEM_ID, TASK_NAME, CREATED_DATE, ACTION_TYPE, MAKER_USER_NAME, APPROVAL_STATUS "
                                    + "FROM M_MAKER_CHECKER_LIST "
                                    + "WHERE APPROVAL_STATUS = " + MCStatus.PENDING + " "
                                    + "AND TASK_ID IN (" + TaskStr + ") "
                                    + "AND MAKER_ID != " + userId + ") AS CLIST "
                                    + "WHERE NUMBER BETWEEN ((@PageNumber - 1) * @PageSize + 1) AND (@PageNumber * @PageSize) "
                                    + "ORDER BY CREATED_DATE desc ";
                    cmd.Parameters.AddWithValue("PageNumber", iPageNumber);
                    cmd.Parameters.AddWithValue("PageSize", iPageSize);
                    dr = cmd.ExecuteReader();

                    if (dr.HasRows)
                    {
                        while (dr.Read())
                        {
                            MKCK mkck = new MKCK();
                            mkck.ItemID = dr["ITEM_ID"].ToString();
                            mkck.TaskName = dr["TASK_NAME"].ToString();
                            mkck.CreatedDate = dr.GetDateTime(dr.GetOrdinal("CREATED_DATE"));
                            mkck.Action = dr["ACTION_TYPE"].ToString();
                            mkck.MakerName = dr["MAKER_USER_NAME"].ToString();
                            mkck.MCStatus = dr["APPROVAL_STATUS"].ToString();

                            CheckerList.Add(mkck);
                        }
                    }
                    dr.Close();
                }                

                return new McObject<Pagination<List<MKCK>>>(true, "Successful.", new Pagination<List<MKCK>>(Count, CheckerList));
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
        public static McObject<Pagination<List<MKCK>>> GetMakerTaskList(string pageNumber, string recordPerPage)
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
                
                SqlCommand cmd = dbc.conn.CreateCommand();
                cmd.CommandText = "SELECT COUNT(*) "
                                + "FROM M_MAKER_CHECKER_LIST "
                                + "WHERE MAKER_ID = " + userId + " "
                                + "AND APPROVAL_STATUS != " + MCStatus.DELETED;
                int Count = (Int32)cmd.ExecuteScalar();
                List <MKCK> MakerList = new List<MKCK>();

                cmd.CommandText = "SELECT * FROM (SELECT ROW_NUMBER() OVER (ORDER BY CREATED_DATE desc) AS NUMBER, "
                                + "ITEM_ID, TASK_NAME, CREATED_DATE, CHECKED_DATE, ACTION_TYPE, MAKER_USER_NAME, "
                                + "APPROVAL_STATUS, CHECKER_USER_NAME "
                                + "FROM M_MAKER_CHECKER_LIST "
                                + "WHERE MAKER_ID = " + userId + " "
                                + "AND APPROVAL_STATUS != 6) AS MAKERLIST "
                                + "WHERE NUMBER BETWEEN ((@PageNumber - 1) * @PageSize + 1) AND (@PageNumber * @PageSize) "
                                + "ORDER BY CREATED_DATE desc ";
                cmd.Parameters.AddWithValue("PageNumber", iPageNumber);
                cmd.Parameters.AddWithValue("PageSize", iPageSize);
                dr = cmd.ExecuteReader();

                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        MKCK mkck = new MKCK();
                        mkck.ItemID = dr["ITEM_ID"].ToString();
                        mkck.TaskName = dr["TASK_NAME"].ToString();
                        mkck.CreatedDate = dr.GetDateTime(dr.GetOrdinal("CREATED_DATE"));
                        mkck.RepliedDate = dr.IsDBNull(dr.GetOrdinal("CHECKED_DATE")) ? (DateTime?)null : dr.GetDateTime(dr.GetOrdinal("CHECKED_DATE"));
                        mkck.Action = dr["ACTION_TYPE"].ToString();
                        mkck.CheckerName = dr.IsDBNull(dr.GetOrdinal("CHECKER_USER_NAME")) ? null : dr.GetString(dr.GetOrdinal("CHECKER_USER_NAME"));
                        mkck.MCStatus = dr["APPROVAL_STATUS"].ToString();

                        MakerList.Add(mkck);
                    }
                }

                return new McObject<Pagination<List<MKCK>>>(true, "Successful.", new Pagination<List<MKCK>>(Count, MakerList));
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
        public static McObject<MKCK> getMKCKDetail(string itemId)
        {
            try
            {
                dbconn dbc = new dbconn();
                dbc.connDB();
                SqlCommand cmd = dbc.conn.CreateCommand();
                cmd.CommandText = "SELECT * "
                                + "FROM M_MAKER_CHECKER_LIST "
                                + "WHERE ITEM_ID = " + itemId;

                SqlDataReader dr = cmd.ExecuteReader();
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
                        mkck.MakerID = dr["MAKER_ID"].ToString();
                        mkck.MakerName = dr["MAKER_USER_NAME"].ToString();
                        mkck.MCStatus = dr["APPROVAL_STATUS"].ToString();
                        mkck.CheckerID = dr.IsDBNull(dr.GetOrdinal("CHECKER_ID")) ? null : dr["CHECKER_ID"].ToString();
                        mkck.CheckerName = dr.IsDBNull(dr.GetOrdinal("CHECKER_USER_NAME")) ? null : dr.GetString(dr.GetOrdinal("CHECKER_USER_NAME"));
                        mkck.Remark = dr.IsDBNull(dr.GetOrdinal("CHECKER_REMARK")) ? null : dr.GetString(dr.GetOrdinal("CHECKER_REMARK"));
                    }
                }

                dr.Close();
                dbc.closeConn();

                return new McObject<MKCK>(true, "Successful!", mkck);
            }
            catch (Exception ex)
            {
                return new McObject<MKCK>(false, "Failed! " + ex.Message);
            }
        }

        [WebMethod]
        public static McObject<MKCK> approvalForAction(string itemId, string approval, string remark)
        {
            dbconn dbc = new dbconn();
            dbc.connDB();
            SqlCommand cmd = dbc.conn.CreateCommand();            
            SqlTransaction trans = dbc.conn.BeginTransaction();
            cmd.Connection = dbc.conn;
            cmd.Transaction = trans;

            SqlDataReader dr = null;

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

                int iItemID = int.Parse(itemId);
                int iStatus = int.Parse(approval);
                string userId = HttpContext.Current.Session["userId"].ToString();
                string userName = HttpContext.Current.Session["userName"].ToString();
                string sql = "";

                cmd.CommandText = "SELECT APPROVAL_STATUS FROM M_MAKER_CHECKER_LIST WHERE ITEM_ID = " + itemId;
                dr = cmd.ExecuteReader();

                if (dr.HasRows)
                {
                    if (dr.Read())
                    {
                        if (dr.GetByte(dr.GetOrdinal("APPROVAL_STATUS")) == 6)
                        {
                            throw new Exception("Task has been removed by the Maker.");
                        }
                    }
                }
                else
                {
                    throw new Exception("Task not found.");
                }
                dr.Close();

                cmd.CommandText = "UPDATE M_MAKER_CHECKER_LIST SET CHECKED_DATE = GETDATE(), APPROVAL_STATUS = " + approval + ", CHECKER_ID = " + userId + ", CHECKER_USER_NAME = '" + userName + "' ";

                if (iStatus.Equals(MCStatus.REJECTED))
                {
                    cmd.CommandText = cmd.CommandText + ", CHECKER_REMARK = @remark ";
                    cmd.Parameters.AddWithValue("remark", remark);
                }
                cmd.CommandText = cmd.CommandText + "WHERE ITEM_ID = " + itemId;
                cmd.ExecuteNonQuery();


                if (iStatus.Equals(MCStatus.APPROVED))
                {
                    cmd.CommandText = "SELECT APPROVE_SQL FROM M_MAKER_CHECKER_LIST WHERE ITEM_ID = " + iItemID;
                    dr = cmd.ExecuteReader();

                    if (dr.HasRows)
                    {
                        if (dr.Read())
                        {
                            sql = dr["APPROVE_SQL"].ToString().Trim();
                        }
                    }
                    dr.Close();
                }
                else if (iStatus.Equals(MCStatus.REJECTED))
                {
                    cmd.CommandText = "SELECT REJECT_SQL FROM M_MAKER_CHECKER_LIST WHERE ITEM_ID = " + iItemID;
                    dr = cmd.ExecuteReader();

                    if (dr.HasRows)
                    {
                        if (dr.Read())
                        {
                            sql = dr["REJECT_SQL"].ToString().Trim();
                        }
                    }
                    dr.Close();
                }
                else
                {
                    throw new Exception("Invalid Status.");
                }

                string[] delimiter = new string[] { MCDelimiter.SQL };
                string[] sqlList = sql.Split(delimiter, StringSplitOptions.None);

                foreach (string aSQL in sqlList)
                {
                    if (!string.IsNullOrEmpty(aSQL.Trim()))
                    {
                        cmd.CommandText = aSQL.Trim();
                        cmd.ExecuteNonQuery();
                    }
                }

                trans.Commit();
                return new McObject<MKCK>(true, "Successful.");
            } 
            catch (SqlException sqlEx)
            {
                try
                {
                    if (trans != null)
                    {
                        trans.Rollback();
                    }
                    return new McObject<MKCK>(false, "SQL Execution Error. " + Environment.NewLine + sqlEx.Message + Environment.NewLine + "Please contact your Adminitrator.");
                }
                catch
                {
                    return new McObject<MKCK>(false, "SQL Execution Error. " + Environment.NewLine + sqlEx.Message + Environment.NewLine + "Rollback Failed!");
                }
            }   
            catch (Exception ex)
            {
                try
                {
                    if (trans != null)
                    {
                        trans.Rollback();
                    }
                    return new McObject<MKCK>(false, "Failed. " + ex.Message);
                }
                catch
                {
                    return new McObject<MKCK>(false, "Failed. " + ex.Message + " Rollback Failed!");
                }
            }
            finally
            {
                dbc.closeConn();
            }
        }

        [WebMethod]
        public static McObject<MKCK> notifiedRejection(string itemId)
        {
            try
            {
                int iItemID = int.Parse(itemId);
                dbconn dbc = new dbconn();
                dbc.connDB();
                SqlCommand cmd = dbc.conn.CreateCommand();
                cmd.CommandText = "UPDATE M_MAKER_CHECKER_LIST SET APPROVAL_STATUS = @mcStatus WHERE ITEM_ID = " + iItemID;
                cmd.Parameters.AddWithValue("mcStatus", MCStatus.NOTED);
                cmd.ExecuteNonQuery();

                return new McObject<MKCK>(true, "Successful");
            }
            catch (Exception ex)
            {
                return new McObject<MKCK>(false, "Failed. " + ex.Message);
            }
        }

        [WebMethod]
        public static McObject<MKCK> deleteAction(string itemId)
        {
            dbconn dbc = new dbconn();
            dbc.connDB();
            SqlCommand cmd = dbc.conn.CreateCommand();
            SqlTransaction trans = dbc.conn.BeginTransaction();
            cmd.Connection = dbc.conn;
            cmd.Transaction = trans;

            try
            {
                int iItemID = int.Parse(itemId);
                cmd.CommandText = "SELECT APPROVAL_STATUS FROM M_MAKER_CHECKER_LIST WHERE ITEM_ID = " + itemId;
                SqlDataReader dr = cmd.ExecuteReader();

                if (dr.HasRows)
                {
                    if (dr.Read())
                    {
                        if (dr.GetByte(dr.GetOrdinal("APPROVAL_STATUS")) != 1)
                        {
                            throw new Exception("Unable to delete. Task has been checked by a checker.");
                        }
                    }
                    else
                    {
                        throw new Exception("Unable to read data.");
                    }
                }
                else
                {
                    throw new Exception("Task not found.");
                }
                dr.Close();

                cmd.CommandText = "UPDATE M_MAKER_CHECKER_LIST SET APPROVAL_STATUS = @mcStatus, MAKER_DELETED_DATE = GETDATE() WHERE ITEM_ID = " + iItemID;
                cmd.Parameters.AddWithValue("mcStatus", MCStatus.DELETED);
                cmd.ExecuteNonQuery();

                cmd.CommandText = "SELECT DELETE_SQL FROM M_MAKER_CHECKER_LIST WHERE ITEM_ID = " + iItemID;
                dr = cmd.ExecuteReader();
                string sql = "";
                if (dr.HasRows)
                {
                    if (dr.Read())
                    {
                        sql = dr["DELETE_SQL"].ToString().Trim();
                    }
                }
                dr.Close();

                string[] delimiter = new string[] { MCDelimiter.SQL };
                string[] sqlList = sql.Split(delimiter, StringSplitOptions.None);

                foreach (string aSQL in sqlList)
                {
                    if (!string.IsNullOrEmpty(aSQL.Trim()))
                    {
                        cmd.CommandText = aSQL.Trim();
                        cmd.ExecuteNonQuery();
                    }
                }

                trans.Commit();

                return new McObject<MKCK>(true, "Successful.");
            }
            catch (Exception ex)
            {
                try
                {
                    trans.Rollback();
                    return new McObject<MKCK>(false, "Failed. " + ex.Message);
                }
                catch
                {
                    return new McObject<MKCK>(false, "Failed. Unable to roll back. " + ex.Message);
                }
            }
            finally
            {
                dbc.closeConn();
            }
        }
    }
}