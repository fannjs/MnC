using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;
using Maestro.Classes;
using System.Configuration;

namespace Maestro.views
{
    public partial class dashboard : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            
        }

        public class StatusOption
        {
            public List<Site> SiteList { get; set; }
            public List<Machine> MachineList { get; set; }
        }
        public class Site
        {
            public string Name { get; set; }
        }
        public class Machine
        {
            public string Type { get; set; }
        }
        public class StatusHistory
        {
            public string DATE_TIME { get; set; }
            public string CODE { get; set; }
            public string DESCRIPTION { get; set; }
            public string TYPE { get; set; }
        }

        public class TransactionHistory
        {
            public DateTime Date { get; set; } // Not sure string or datetime is better
            public int TransactionCount { get; set; }
            public int ChequeCount { get; set; }
        }

        public class machErrStatus
        {
            public string mCode { get; set; }
            public string mCodeDesc { get; set; }
            public string mErrType { get; set; }
            public int mNumProblems { get; set; }
            public int mNumProblems_0to4 { get; set; }
            public int mNumProblems_4to8 { get; set; }
            public int mNumProblems_8to12 { get; set; }
            public int mNumProblems_12to16 { get; set; }
            public int mNumProblems_16to20 { get; set; }
            public int mNumProblems_20to24 { get; set; }
        }

        [WebMethod]
        public static McObject<StatusOption> GetStatusOption()
        {
            dbconn dbc = new dbconn();
            dbc.connDB();
            SqlCommand cmd = dbc.conn.CreateCommand();

            try
            {
                cmd.CommandText = "SELECT M_MACH_TYPE FROM M_MACHINE_TYPE";
                SqlDataReader dr = cmd.ExecuteReader();
                List<Machine> MachineList = new List<Machine>();

                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        Machine m = new Machine();
                        m.Type = dr["M_MACH_TYPE"].ToString().Trim();

                        MachineList.Add(m);
                    }
                }
                dr.Close();

                cmd.CommandText = "SELECT M_CUST_NAME FROM M_CUSTOMER";
                dr = cmd.ExecuteReader();
                List<Site> SiteList = new List<Site>();

                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        Site s = new Site();
                        s.Name = dr["M_CUST_NAME"].ToString().Trim();

                        SiteList.Add(s);
                    }
                }
                dr.Close();

                StatusOption SO = new StatusOption();
                SO.SiteList = SiteList;
                SO.MachineList = MachineList;

                return new McObject<StatusOption>(true, "Successful.", SO);
            }
            catch (Exception ex)
            {
                return new McObject<StatusOption>(false, "Failed. " + ex.Message);
            }
            finally
            {
                dbc.closeConn();
            }
        }

        [WebMethod]
        public static McObject<string> GetLastTransactionDT(string machineID)
        {
            SqlConnection conn = null;
            machineID = "Q" + machineID; //HARDCODE FOR DEMO PURPOSE

            try
            {
                using (conn = new SqlConnection(ConfigurationManager.ConnectionStrings["CSDMS"].ConnectionString))
                {
                    conn.Open();

                    SqlCommand cmd = conn.CreateCommand();
                    cmd.CommandText = "SELECT TOP 1 TimeOfDeposit FROM tblScanned " +
                                    "WHERE MachineID = @machineID " +
                                    "ORDER BY TimeOfDeposit desc";
                    cmd.Parameters.AddWithValue("machineID", machineID);

                    SqlDataReader dr = cmd.ExecuteReader();
                    string LastTransactionDT = "";

                    if (dr.HasRows)
                    {
                        if (dr.Read())
                        {
                            LastTransactionDT = dr["TimeOfDeposit"].ToString();
                        }
                    }

                    return new McObject<string>(true, "Successful.", LastTransactionDT);
                }                
            }
            catch (Exception ex)
            {
                return new McObject<string>(false, "Failed. " + ex.Message);
            }
            finally
            {
                if (conn != null)
                {
                    conn.Close();
                }
            }
        }

        [WebMethod]
        public static McObject<List<TransactionHistory>> GetTransactionHistory(string machineID)
        {
            SqlConnection conn = null;
            machineID = "Q" + machineID; //HARDCODE FOR DEMO PURPOSE

            try
            {
                using (conn = new SqlConnection(ConfigurationManager.ConnectionStrings["CSDMS"].ConnectionString))
                {
                    conn.Open();

                    SqlCommand cmd = conn.CreateCommand();
                    cmd.CommandText = "SELECT ScanDate, MachineID,TrnNo,SubTrnNo from tblScanned " +
                                        "WHERE ScanDate >= DATEADD(day, -30, GETDATE()) " +
                                        "AND MachineID = @machineID " +
                                        "ORDER BY ScanDate desc";
                    cmd.Parameters.AddWithValue("machineID", machineID);
                    SqlDataReader dr = cmd.ExecuteReader();

                    List<TransactionHistory> TransList = new List<TransactionHistory>();
                    Dictionary<DateTime, string> date = new Dictionary<DateTime, string>();
                    Dictionary<string, string> transaction = new Dictionary<string, string>();

                    TransactionHistory newTrans = new TransactionHistory();
                    int TransCount = 0;
                    int ChequeCount = 0;
                    bool firstTime = true;
                    bool pending = true;

                    if (dr.HasRows)
                    {
                        while (dr.Read())
                        {
                            DateTime Dt = dr.GetDateTime(dr.GetOrdinal("ScanDate"));
                            String TransNo = dr["TrnNo"].ToString();

                            pending = true;

                            if (date.ContainsKey(Dt))
                            {
                                if (!transaction.ContainsKey(TransNo))
                                {
                                    transaction.Add(TransNo, "");
                                    TransCount = TransCount + 1;
                                }
                            }
                            else
                            {
                                if (firstTime)
                                {
                                    firstTime = false;
                                }
                                else
                                {
                                    newTrans.TransactionCount = TransCount;
                                    newTrans.ChequeCount = ChequeCount;

                                    TransList.Add(newTrans);
                                    transaction.Clear();
                                    pending = false;
                                }

                                date.Add(Dt, "");
                                transaction.Add(TransNo, "");
                                TransCount = TransCount + 1;

                                newTrans = new TransactionHistory();
                                newTrans.Date = Dt;
                            }
                            ChequeCount = ChequeCount + 1;
                        }

                        if (pending)
                        {
                            newTrans.TransactionCount = TransCount;
                            newTrans.ChequeCount = ChequeCount;

                            TransList.Add(newTrans);
                        }
                    }
                    dr.Close();

                    return new McObject<List<TransactionHistory>>(true, "Successful.", TransList);
                }
            }
            catch (Exception ex)
            {
                return new McObject<List<TransactionHistory>>(false, "Failed. " + ex.Message);
            }
            finally
            {
                if (conn != null)
                {
                    conn.Close();
                }
            }
        }

        [WebMethod]
        public static McObject<Pagination<List<StatusHistory>>> GetStatusHistory(string machineID, string pageNumber, string recordPerPage)
        {
            dbconn dbc = new dbconn();
            dbc.connDB();
            SqlCommand cmd = dbc.conn.CreateCommand();

            try
            {
                int iPageNumber = Int32.Parse(pageNumber);
                int iPageSize = Int32.Parse(recordPerPage);

                cmd.CommandText = "SELECT COUNT(*) FROM M_INCOMING_DATA MI, M_CODES MC WHERE MI.M_CODE = MC.M_CODE AND MI.M_MACH_ID = '" + machineID + "'";
                int count = (Int32)cmd.ExecuteScalar();

                cmd.CommandText = "SELECT * FROM (SELECT ROW_NUMBER() OVER(ORDER BY MI.M_DATE DESC, MI.M_TIME DESC) AS NUMBER, (MI.M_DATE + ' ' + rtrim(MI.M_TIME)) As 'DATETIME', MC.M_DEVICE As 'DEVICES', " +
                "MC.M_CODE As 'ERROR CODE', MC.M_ERROR_DESCRIPTION As 'ERROR DESCRIPTION', MC.M_ERRORTYPE As 'ERROR TPYE'" +
                "FROM M_INCOMING_DATA MI, M_CODES MC " +
                "WHERE MI.M_CODE = MC.M_CODE AND MI.M_MACH_ID = '" + machineID + "' " +
                ")AS MLIST " +
                "WHERE NUMBER BETWEEN ((@PageNumber-1) * @RowspPage +1) AND (@PageNumber * @RowspPage ) " +
                "ORDER BY 'DATETIME' desc";

                cmd.Parameters.AddWithValue("PageNumber", iPageNumber);
                cmd.Parameters.AddWithValue("RowspPage", iPageSize);

                SqlDataReader dr = cmd.ExecuteReader();

                List<StatusHistory> statusList = new List<StatusHistory>();

                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        StatusHistory status = new StatusHistory();
                        status.DATE_TIME = dr["DATETIME"].ToString();
                        status.CODE = dr["ERROR CODE"].ToString();
                        status.DESCRIPTION = dr["ERROR DESCRIPTION"].ToString();
                        status.TYPE = dr["ERROR TPYE"].ToString();

                        statusList.Add(status);
                    }
                }
                dr.Close();

                Pagination<List<StatusHistory>> pr = new Pagination<List<StatusHistory>>(count, statusList);

                return new McObject<Pagination<List<StatusHistory>>>(true, "Successful.", pr);
            }
            catch (Exception ex)
            {
                return new McObject<Pagination<List<StatusHistory>>>(false, "Failed. " + ex.Message);
            }
            finally
            {
                dbc.closeConn();
            }
        }

        public class UrgencyKiosk
        {
            public string KioskId { get; set; }
            public string KioskType { get; set; }
            public string Status { get; set; }
            public bool IsAlive { get; set; }
            public List<EventData> DataList { get; set; }
        }
        public class EventData
        {
            public string StatusDescription { get; set; }
            public string Downtime { get; set; }
            public string ErrorType { get; set; }
        }

        [WebMethod]
        public static McObject<List<UrgencyKiosk>> GetUrgencyQueue()
        {
            dbconn dbc = new dbconn();
            dbc.connDB();
            SqlCommand cmd = dbc.conn.CreateCommand();

            try
            {
                cmd.CommandText = "SELECT * FROM (SELECT ME.M_MACH_ID, ML.M_MACH_TYPE, MC.M_ERRORTYPE, ME.M_ALIVE " +
                                "FROM M_EVENT_DATA ME, M_MACHINE_LIST ML, M_CODES MC " +
                                "WHERE ME.M_MACH_ID = ML.M_MACH_ID AND MC.M_CODE = ME.M_CODE) AS UQL " +
                                "WHERE M_ALIVE = 'FALSE' OR M_ERRORTYPE IN('ERROR','WARN') " +
                                "ORDER BY M_ALIVE ASC";
                SqlDataReader dr = cmd.ExecuteReader();
                List<UrgencyKiosk> KioskList = new List<UrgencyKiosk>();

                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        UrgencyKiosk Kiosk = new UrgencyKiosk();
                        Kiosk.KioskId = dr["M_MACH_ID"].ToString();
                        Kiosk.KioskType = dr["M_MACH_TYPE"].ToString();
                        Kiosk.Status = dr["M_ERRORTYPE"].ToString();
                        Kiosk.IsAlive = dr.GetBoolean(dr.GetOrdinal("M_ALIVE"));

                        KioskList.Add(Kiosk);
                    }
                }
                dr.Close();

                foreach (UrgencyKiosk Kiosk in KioskList)
                {
                    cmd.CommandText = "SELECT MI.M_DATE, MI.M_TIME, MI.M_MACH_ID,MC.M_CODE, MC.M_ERRORTYPE, MC.M_ERROR_DESCRIPTION, MC.M_SOP " +
                                "FROM M_INCOMING_DATA MI, M_CODES MC " +
                                "WHERE MI.M_CODE = MC.M_CODE AND MI.M_MACH_ID = '" + Kiosk.KioskId + "' " +
                                "AND MC.M_ERRORTYPE IN('ERROR','WARN','ONLINE') " +
                                "ORDER BY MI.M_DATE desc, MI.M_TIME desc";
                    dr = cmd.ExecuteReader();
                    List<EventData> DataList = new List<EventData>();

                    if (dr.HasRows)
                    {
                        while (dr.Read())
                        {
                            if (dr["M_ERRORTYPE"].ToString().ToUpper().Equals("ONLINE"))
                            {
                                break;
                            }
                            else
                            {
                                EventData Data = new EventData();
                                Data.StatusDescription = dr["M_ERROR_DESCRIPTION"].ToString();
                                Data.Downtime = dr["M_TIME"].ToString();
                                Data.ErrorType = dr["M_ERRORTYPE"].ToString();
                                
                                DataList.Add(Data);
                            }
                        }
                    }
                    Kiosk.DataList = DataList;
                    dr.Close();
                }

                return new McObject<List<UrgencyKiosk>>(true, "Successful. ", KioskList);
            }
            catch (Exception ex)
            {
                return new McObject<List<UrgencyKiosk>>(false, "Failed. " + ex.Message);
            }
            finally
            {
                dbc.closeConn();
            }
        }

        //Do not remove this 
        //Get urgency queue of one particular Kiosk
        [WebMethod]
        public static UrgencyQueueList[] getUrgencyQueueList(string machID)
        {
            dbconn dbc = new dbconn();
            dbc.connDB();
            SqlCommand cmd = dbc.conn.CreateCommand();
            cmd.CommandText = "SELECT MI.M_DATE, MI.M_TIME, MI.M_MACH_ID, ML.M_MACH_TYPE,MC.M_CODE, MC.M_ERRORTYPE, MC.M_ERROR_DESCRIPTION, MC.M_SOP " +
                                "FROM M_INCOMING_DATA MI, M_MACHINE_LIST ML, M_CODES MC " +
                                "WHERE MI.M_MACH_ID = ML.M_MACH_ID AND MI.M_CODE = MC.M_CODE AND MI.M_MACH_ID = '" + machID + "' " +
                                "AND MC.M_ERRORTYPE IN('ERROR','WARN','ONLINE') " +
                                "ORDER BY MI.M_DATE desc, MI.M_TIME desc";

            SqlDataReader dr = cmd.ExecuteReader();
            List<UrgencyQueueList> uqList = new List<UrgencyQueueList>();

            if (dr.HasRows)
            {
                while (dr.Read())
                {
                    if (dr["M_ERRORTYPE"].ToString().Trim() == "ONLINE")
                    {
                        dr.Close();
                        break;
                    }
                    else
                    {
                        UrgencyQueueList uq = new UrgencyQueueList();
                        uq.MachID = dr["M_MACH_ID"].ToString();
                        uq.MachType = dr["M_MACH_TYPE"].ToString();
                        uq.ErrCode = dr["M_CODE"].ToString();
                        uq.ErrDesc = dr["M_ERROR_DESCRIPTION"].ToString();
                        uq.ErrTime = dr["M_TIME"].ToString();
                        uq.ErrType = dr["M_ERRORTYPE"].ToString();
                        uq.SOP = dr["M_SOP"].ToString();

                        uqList.Add(uq);
                    }
                }
            }

            dr.Close();
            dbc.closeConn();
            return uqList.ToArray();
        }

        public class UrgencyQueueList
        {
            public string MachID { get; set; }
            public string MachType { get;set; }
            public string ErrType { get; set; }
            public string ErrCode { get; set; }
            public string ErrDesc { get; set; }
            public string ErrTime { get; set; }
            public string SOP { get; set; }
            public string Alive { get; set; }
        }

        /// <summary>
        /// Deskboad top problems chart
        /// </summary>
        /// <param name="numOfTopProblems"></param>
        /// <returns></returns>
        [WebMethod]
        public static McObject<List<machErrStatus>> genTopProblemsTodayChart(int numOfTopProblems)
        {
            dbconn dbc = new dbconn();
            dbc.connDB();
            SqlCommand cmd = dbc.conn.CreateCommand();

            int numProblems = 4;

            if (string.IsNullOrEmpty(numOfTopProblems.ToString()))
            {
                numOfTopProblems = numProblems;
            }

            try
            {
                cmd.CommandText = "select top(" + numOfTopProblems + ") MRD.M_CODE,  MC.M_ERROR_DESCRIPTION, COUNT(*) AS mCount" +
                        " from M_REPORTING_DATA as MRD, M_CODES as MC " +
                        " where MRD.M_CODE = MC.M_CODE " +
                        " and MC.M_ERRORTYPE = 'ERROR' " +
                        //" and MRD.M_DATE = '2015-01-16' " +
                        " and MRD.M_DATE = convert(varchar,getdate(),23) "+
                        " group by MRD.M_CODE, MC.M_ERROR_DESCRIPTION " +
                        " HAVING COUNT(*) >=1 " +
                        " ORDER BY mCount DESC ";
                SqlDataReader dr = cmd.ExecuteReader();

                List<machErrStatus> errStatusList = new List<machErrStatus>();

                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        machErrStatus mErr = new machErrStatus();
                        mErr.mCode = dr["M_CODE"].ToString().Trim();
                        mErr.mCodeDesc = dr["M_ERROR_DESCRIPTION"].ToString().Trim();
                        mErr.mNumProblems = int.Parse(dr["mCount"].ToString());
                        errStatusList.Add(mErr);
                    }
                }
                dr.Close();

                return new McObject<List<machErrStatus>>(true, "Successful.", errStatusList);
            }
            catch (Exception ex)
            {
                return new McObject<List<machErrStatus>>(false, "Failed. " + ex.Message);
            }
            finally
            {
                dbc.closeConn();
            }
        }

        /// <summary>
        /// Deskboard linechart 
        /// this is get total number of error in 4 hrs group.
        /// </summary>
        /// <returns></returns>
        //[WebMethod]
        //public static McObject<List<machErrStatus>> genMachStatusTodayChart()
        //{
        //    dbconn dbc = new dbconn();
        //    dbc.connDB();
        //    SqlCommand cmd = dbc.conn.CreateCommand();

        //    try
        //    {
        //        cmd.CommandText = "SELECT M_CODES.M_ERRORTYPE, " +
        //            " COUNT(CASE WHEN DATEPART(hour, MRD.M_TIME) BETWEEN 0 AND 3 THEN 1 ELSE NULL END) AS [0to4], " +
        //            " COUNT(CASE WHEN DATEPART(hour, MRD.M_TIME) BETWEEN 4 AND 7 THEN 1 ELSE NULL END) AS [4to8], " +
        //            " COUNT(CASE WHEN DATEPART(hour, MRD.M_TIME) BETWEEN 8 AND 11 THEN 1 ELSE NULL END) AS [8to12], " +
        //            " COUNT(CASE WHEN DATEPART(hour, MRD.M_TIME) BETWEEN 12 AND 15 THEN 1 ELSE NULL END) AS [12to16], " +
        //            " COUNT(CASE WHEN DATEPART(hour, MRD.M_TIME) BETWEEN 15 AND 19 THEN 1 ELSE NULL END) AS [16to20], " +
        //            " COUNT(CASE WHEN DATEPART(hour, MRD.M_TIME) BETWEEN 20 AND 24 THEN 1 ELSE NULL END) AS [20to24] " +
        //            " FROM M_REPORTING_DATA AS MRD " +
        //            " INNER JOIN M_CODES " +
        //            " ON MRD.M_CODE = M_CODES.M_CODE " +
        //            //" WHERE (MRD.M_DATE = CONVERT(varchar, GETDATE(), 23)) " +
        //            " WHERE (MRD.M_DATE = '2015-01-16') AND (M_CODES.M_ERRORTYPE = 'ERROR' OR"+
        //            //" M_CODES.M_ERRORTYPE = 'OK' OR "+
        //            " M_CODES.M_ERRORTYPE = 'WARN' OR "+
        //            " M_CODES.M_ERRORTYPE = 'OFFLINE') " +
        //            " GROUP BY  M_CODES.M_ERRORTYPE ";

        //        SqlDataReader dr = cmd.ExecuteReader();

        //        List<machErrStatus> errStatusList = new List<machErrStatus>();

        //        if (dr.HasRows)
        //        {
        //            while (dr.Read())
        //            {
        //                machErrStatus mErr = new machErrStatus();
        //               // mErr.mCode = dr["M_CODE"].ToString().Trim();
        //                mErr.mErrType = dr["M_ERRORTYPE"].ToString().Trim();
        //                mErr.mNumProblems_0to4 = int.Parse(dr["0to4"].ToString());
        //                mErr.mNumProblems_4to8 = int.Parse(dr["4to8"].ToString());
        //                mErr.mNumProblems_8to12 = int.Parse(dr["8to12"].ToString());
        //                mErr.mNumProblems_12to16 = int.Parse(dr["12to16"].ToString());
        //                mErr.mNumProblems_16to20 = int.Parse(dr["16to20"].ToString());
        //                mErr.mNumProblems_20to24 = int.Parse(dr["20to24"].ToString());
        //                errStatusList.Add(mErr);
        //            }
        //        }
        //        dr.Close();

        //        return new McObject<List<machErrStatus>>(true, "Successful.", errStatusList);
        //    }
        //    catch (Exception ex)
        //    {
        //        return new McObject<List<machErrStatus>>(false, "Failed. " + ex.Message);
        //    }
        //    finally
        //    {
        //        dbc.closeConn();
        //    }
        //}


        /// <summary>
        /// Deskboard linechart 
        /// this is get total number of error in 4 hrs group.
        /// </summary>
        /// <returns></returns>
        [WebMethod]
        public static McObject<List<machErrStatus>> genMachStatusTodayChart_groupByMachErrType()
        {
            dbconn dbc = new dbconn();
            dbc.connDB();
            SqlCommand cmd = dbc.conn.CreateCommand();

            try
            {
                cmd.CommandText = "SELECT MC.M_ERRORTYPE, MRD.M_MACH_ID," +
                    " COUNT(CASE WHEN DATEPART(hour, MRD.M_TIME) BETWEEN 0 AND 3 THEN 1 ELSE NULL END) AS [0to4], " +
                    " COUNT(CASE WHEN DATEPART(hour, MRD.M_TIME) BETWEEN 4 AND 7 THEN 1 ELSE NULL END) AS [4to8], " +
                    " COUNT(CASE WHEN DATEPART(hour, MRD.M_TIME) BETWEEN 8 AND 11 THEN 1 ELSE NULL END) AS [8to12], " +
                    " COUNT(CASE WHEN DATEPART(hour, MRD.M_TIME) BETWEEN 12 AND 15 THEN 1 ELSE NULL END) AS [12to16], " +
                    " COUNT(CASE WHEN DATEPART(hour, MRD.M_TIME) BETWEEN 15 AND 19 THEN 1 ELSE NULL END) AS [16to20], " +
                    " COUNT(CASE WHEN DATEPART(hour, MRD.M_TIME) BETWEEN 20 AND 24 THEN 1 ELSE NULL END) AS [20to24], " +
                    " COUNT(MC.M_ERRORTYPE) AS ttlmach "+
                    " FROM M_REPORTING_DATA AS MRD  "+
                    " INNER JOIN M_CODES AS MC ON MRD.M_CODE = MC.M_CODE "+
                    //" WHERE (MRD.M_DATE = '2015-01-16')  " +
                    " WHERE (MRD.M_DATE = CONVERT(varchar, GETDATE(), 23)) " +
                    " AND (MC.M_ERRORTYPE = 'ERROR'  "+
                    " OR MC.M_ERRORTYPE = 'WARN'  "+
                    " OR MC.M_ERRORTYPE = 'OFFLINE') " +
                    " GROUP BY MC.M_ERRORTYPE, MRD.M_MACH_ID ";

                SqlDataReader dr = cmd.ExecuteReader();

                List<machErrStatus> errStatusList = new List<machErrStatus>();

                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        machErrStatus mErr = new machErrStatus();
                        mErr.mErrType = dr["M_ERRORTYPE"].ToString().Trim();
                        mErr.mNumProblems_0to4 = int.Parse(dr["0to4"].ToString());
                        mErr.mNumProblems_4to8 = int.Parse(dr["4to8"].ToString());
                        mErr.mNumProblems_8to12 = int.Parse(dr["8to12"].ToString());
                        mErr.mNumProblems_12to16 = int.Parse(dr["12to16"].ToString());
                        mErr.mNumProblems_16to20 = int.Parse(dr["16to20"].ToString());
                        mErr.mNumProblems_20to24 = int.Parse(dr["20to24"].ToString());
                        errStatusList.Add(mErr);
                    }
                }
                dr.Close();

                string sql = " SELECT MED.M_MACH_ID, MED.M_DATE, MED.M_TIME, MED.M_CODE, MC.M_ERRORTYPE " +
                            " FROM M_EVENT_DATA AS MED INNER JOIN "+
                            " M_CODES AS MC ON MED.M_CODE = MC.M_CODE "+
                            " WHERE (MC.M_ERRORTYPE = 'ERROR') OR (MC.M_ERRORTYPE = 'WARN') OR (MC.M_ERRORTYPE = 'OFFLINE')";

                cmd.CommandText = sql;
                dr = cmd.ExecuteReader();

                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        string mid = dr["M_MACH_ID"].ToString();
                        //// find diff days
                        DateTime curDate = DateTime.Now;
                        DateTime prvDate = Convert.ToDateTime(dr["M_DATE"].ToString().Trim());

                        int i0to4 = 0;
                        int i4to8 = 0;
                        int i8to12 = 0;
                        int i12to16 = 0;
                        int i16to20 = 0;
                        int i20to24 = 0;

                        double diffDays = (curDate - prvDate).TotalDays;
                        if (diffDays > 1) // find n show last err happen 1 day before.
                        {
                            machErrStatus mErr = new machErrStatus();
                            mErr.mErrType = dr["M_ERRORTYPE"].ToString().Trim();
                            mErr.mNumProblems_0to4 = 1;
                            mErr.mNumProblems_4to8 = 1;
                            mErr.mNumProblems_8to12 = 1;
                            mErr.mNumProblems_12to16 = 1;
                            mErr.mNumProblems_16to20 = 1;
                            mErr.mNumProblems_20to24 = 1;
                            errStatusList.Add(mErr);
                        }
                        else if (diffDays < 1) // show today error from start to current.
                        {
                            string prvTime = dr["M_TIME"].ToString().Trim();
                            TimeSpan prvHour = TimeSpan.Parse(prvTime);
                            double lastHappen = prvHour.TotalHours / 4; // fall in which group
                            double currTimeGroup = (curDate.Hour / 4) + 1; // extra 1 for current time group;

                            for (int i = 1; i <= currTimeGroup; i++)
                            {
                                if (currTimeGroup > lastHappen)
                                {
                                    if (lastHappen < i && (i == 1))
                                    {
                                        i0to4 = 1;
                                    }
                                    if (lastHappen < i && (i == 2))
                                    {
                                        i4to8 = 1;
                                    }
                                    if (lastHappen < i && (i == 3))
                                    {
                                        i8to12 = 1;
                                    }
                                    if (lastHappen < i && (i == 4))
                                    {
                                        i12to16 = 1;
                                    }
                                    if (lastHappen < i && (i == 5))
                                    {
                                        i16to20 = 1;
                                    }
                                    if (lastHappen < i && (i == 6))
                                    {
                                        i20to24 = 1;
                                    }
                                }
                            }
                            machErrStatus mErr = new machErrStatus();
                            mErr.mErrType = dr["M_ERRORTYPE"].ToString().Trim();
                            mErr.mNumProblems_0to4 = i0to4;
                            mErr.mNumProblems_4to8 = i4to8;
                            mErr.mNumProblems_8to12 = i8to12;
                            mErr.mNumProblems_12to16 = i12to16;
                            mErr.mNumProblems_16to20 = i16to20;
                            mErr.mNumProblems_20to24 = i20to24;
                            errStatusList.Add(mErr);
                        }
                    }
                }
                dr.Close();


                return new McObject<List<machErrStatus>>(true, "Successful.", errStatusList);
            }
            catch (Exception ex)
            {
                return new McObject<List<machErrStatus>>(false, "Failed. " + ex.Message);
            }
            finally
            {
                dbc.closeConn();
            }
        }

    }
}