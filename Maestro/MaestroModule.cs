using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web;

namespace Maestro
{
    public class MaestroModule
    {
        public static int countbeep = 0;
        public static bool beepStop = false;
        public static bool allStop = false;

        public static bool boolSendingNotification = false;

        public struct LocationStatus
        {
            public string loc;
            public int onlineMach;
            public int errMach;
            public int warnMach;
            public int offlineMach;
        }

        public struct location
        {
            public string country;
            public LocationStatus[] locStatusList;
        }

        public static location[] locationList;


        // ------------
        public struct LocationBranchStatus
        {
            public string locBr;
            public int onlineMachBr;
            public int errMachBr;
            public int warnMachBr;
            public int offlineMachBr;
        }

        public struct locCityDistrict
        {
            //public string country2;
            //public LocationStatus2[] locStatusList2;
            public string citydistrict;
            public LocationBranchStatus[] locBrStatusList;
        }

        public static locCityDistrict[] locationCityDistrictList;

        //--------------

        public struct Outer
        {
            public decimal tem;
            public string MachineName;
            //public DLT dlt;
        }
        public static bool startPage = true;
        public static string comport;
        public static string baudrate;
        public static string timeout;
        public static string mailserver;
        public static string mailport;
        public static string emailFrom;
        public static string Mailusername;
        public static string Mailpassword;

        public static bool MailSSL;
        public static bool smsError;
        public static bool smsWarn;

        public static bool smsOffline;

        public static Outer[] aryList;
        public struct fileDetail
        {
            public string StrFilePath;
            public string StrFileName;
            public string StrFileLoc;
            public bool BlnReg32;
            public bool blnfileExist;
        }

        public static string[] column4gridView = 
        {
		"File Path",
		"File Name",
		"File Send To",
		"Register",
		"Replace Old file"
	    };

        public static fileDetail[] file2send = new fileDetail[1];
        public struct clientDetail
        {
            public string clientIP;
            public string clientID;
            public string clientPassword;
        }

        public static clientDetail[] clientInfo;

        //public static void Log(string logMessage, TextWriter w)
        //{
        //    w.Write("\r\nLog Entry : ");
        //    w.WriteLine("{0} {1}", DateTime.Now.ToLongTimeString(),
        //        DateTime.Now.ToLongDateString());
        //    w.WriteLine("  :");
        //    w.WriteLine("  :{0}", logMessage);
        //    w.WriteLine("-------------------------------");
        //    // Update the underlying file.
        //    w.Flush();
        //}

        //public static void DumpLog(StreamReader r)
        //{
        //    // While not at the end of the file, read and write lines.
        //    string line;
        //    while ((line = r.ReadLine()) != null)
        //    {
        //        Console.WriteLine(line);
        //    }
        //    r.Close();
        //}

        //public static void CreateLog_old(String mgs)
        //{
        //    try
        //    {
        //        string LogFile = System.AppDomain.CurrentDomain.BaseDirectory + "Logs";
        //        System.IO.DirectoryInfo folder = new System.IO.DirectoryInfo(LogFile);

        //        if (!folder.Exists)
        //        {
        //            folder.Create();
        //        }

        //        string logAdd = folder + "\\logfile.txt";
        //        // Create a writer and open the file:
        //        StreamWriter log;
        //        //string fileLocMove = LogFile + "\\logfile2.txt";

        //        if (!File.Exists(logAdd))
        //        {
        //            log = new StreamWriter(logAdd);
        //            //log.WriteLine("logAdd   = " + logAdd);
        //        }
        //        else
        //        {
        //            FileInfo f = new FileInfo(logAdd);
        //            long s1 = f.Length;
        //            if (s1 > 5000)
        //            {
        //                DateTime time = DateTime.Now;
        //                string format = "yyyyMMdd_HHmmss";
        //                //Rename previous file
        //                File.Move(logAdd, LogFile + "\\bk\\Log_" + time.ToString(format) + ".log");
        //            }
        //            log = File.AppendText(logAdd);
        //            //log.WriteLine("logAdd   = " + logAdd);
        //            //log.WriteLine("f size   = " + s1);
        //        }

        //        // Write to the file:
        //        //log.WriteLine("DATE AND TIME: " + DateTime.Now);
        //        log.WriteLine(DateTime.Now + ": " + mgs);
        //        log.WriteLine();

        //        // Close the stream:
        //        if (log != null)
        //        {
        //            log.Close();
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        MaestroModule.CreateLog(ex.Message.ToString(), "CreateLog_old()");
        //    }

        //}

        /// <summary>
        /// Creates log file for system track.
        /// </summary>
        /// <param name="errMsg">Message</param>
        /// <param name="functionName">Function name</param>
        public static void CreateLog(string errMsg, string functionName)
        {
            string LogFile = System.AppDomain.CurrentDomain.BaseDirectory + "Logs";
            System.IO.DirectoryInfo folder = new System.IO.DirectoryInfo(LogFile);
            //StreamWriter log = default(StreamWriter);

            try
            {
                StreamWriter log;
                DateTime time = DateTime.Now;
                if (!folder.Exists)
                {
                    folder.Create();
                }

                string logAdd = folder + "\\logfile.txt";

                if (!File.Exists(logAdd))
                {
                    log = new StreamWriter(logAdd);
                    //log.WriteLine("logAdd   = " + logAdd);
                }
                else
                {
                    FileInfo f = new FileInfo(logAdd);
                    long s1 = f.Length;
                    if (s1 > 5000)
                    {
                        string LogFile2 = System.AppDomain.CurrentDomain.BaseDirectory + "Logs\\archive";
                        System.IO.DirectoryInfo arcFolder = new System.IO.DirectoryInfo(LogFile2);
                        if (!arcFolder.Exists)
                        {
                            arcFolder.Create();
                        }

                        string format = "yyyyMMdd_HHmmss";
                        //Rename previous file
                        File.Move(logAdd, LogFile + "\\archive\\Log_" + time.ToString(format) + ".log");
                    }
                    log = File.AppendText(logAdd);
                }

                string format2 = "dd-MM-yyyy HH:mm:ss";
                log.WriteLine(time.ToString(format2) + ": " + errMsg + " [" + functionName + "]");
                if (log != null)
                {
                    log.Close();
                }
            }
            catch (Exception ex)
            {
                MaestroModule.CreateLog(ex.Message.ToString(), "CreateLog");
            }
        }

        /// <summary>
        /// This is a function to log in the audit trail.
        /// </summary>
        /// <param name="userid">User name</param>
        /// <param name="session">User (Perform by user) / System (System or Application exception). </param>
        /// <param name="action">View, Add, Update, Delete</param>
        /// <param name="Event_item">Event title name</param>
        /// <param name="Event_desc">Event action been performed.</param>
        public static void auditLog(string userid, string session, string action, string Event_item, string Event_desc)
        {
            DateTime dt = System.DateTime.Now;
            string curtime = String.Format("{0:MM/dd/yyyy HH:mm:ss}", dt); // mssql datatime format is MM/dd/yyyy

            dbconn dbc = new dbconn();
            dbc.connDB();
            SqlTransaction myTrans;

            myTrans = dbc.conn.BeginTransaction();
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = dbc.conn;
            cmd.Transaction = myTrans;

            try
            {
                string sql = "insert into M_auditTrail (UserID, datetime, Session, Action, Event_item, Event_Desc ) values ('" + userid + "', '" + curtime + "', '" + session + "', '" + action + "', '" + Event_item + "', '" + Event_desc + "')";
                cmd.CommandText = sql;
                cmd.ExecuteNonQuery();
                myTrans.Commit();
                dbc.closeConn();
            }
            catch (Exception ex)
            {
                string sql = "insert into M_auditTrail (UserID, datetime, Session, Action, Event_item, Event_desc ) values ('" + userid + "', '" + curtime + "', '" + session + "', '" + action + "', '" + Event_item + "', '" + ex.Message + "')";
                cmd.CommandText = sql;
                cmd.ExecuteNonQuery();
                myTrans.Commit();
                dbc.closeConn();
            }

        }

    }
}