using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Windows.Forms;
using System.IO.Compression;

namespace Maestro.views.kioskMaintenance.advertisement
{
    public partial class specific : System.Web.UI.Page
    {

        public class advDefaultSeq
        {
            public string ADV_ID { get; set; }
            public string ADV_PATH { get; set; }
            public string ADV_FILENAME { get; set; }
            public string ADV_TYPE { get; set; }
            //public string ADV_FILE_SRC { get; set; }
            public string ADV_THUMBNAME { get; set; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {

        }

        [WebMethod]
        public static int setMachAdvSeq(string machID, string[] arrAdv, string machSeq)
        {
            dbconn dbc = new dbconn();
            dbc.connDB();
            SqlTransaction myTrans;

            myTrans = dbc.conn.BeginTransaction();
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = dbc.conn;
            cmd.Transaction = myTrans;
            try
            {
                // To do: get ADV_VER first, and increase version.
                string sqlUpdate = "update M_MACHINE_LIST set ADV_SEQ = '" + machSeq + "' where M_MACH_ID = '" + machID + "'";
                cmd.CommandText = sqlUpdate;
                cmd.ExecuteNonQuery();

                string sqlDelete = "delete from M_MACH_ADV_SEQ where MACH_ID = '" + machID + "'";
                cmd.CommandText = sqlDelete;
                cmd.ExecuteNonQuery();

                DateTime dt = System.DateTime.Now;
                string curtime = String.Format("{0:MM/dd/yyyy HH:mm:ss}", dt); // mssql datatime format is MM/dd/yyyy
                for (int i = 0; i < arrAdv.Length; i++)
                {
                    int iadvid = int.Parse(arrAdv[i].ToString());
                    string sSeq = (i + 1).ToString();
                    string sql = "insert into M_MACH_ADV_SEQ (MACH_ID, ADV_ID, ADV_SEQ, CREATED_DATE) VALUES ('" + machID + "', " + iadvid + ", '" + sSeq + "', GetDate())";
                    // string sql = "insert into M_MACH_ADV_SEQ (MACH_ID, ADV_ID, ADV_SEQ) VALUES ('" + machID + "', " + iadvid + ", '" + sSeq + "')";
                    cmd.CommandText = sql;
                    cmd.ExecuteNonQuery();
                }
                myTrans.Commit();
                dbc.closeConn();

                return 1;
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show("setMachAdvSeq err: " + ex.Message);
                myTrans.Rollback();
                dbc.closeConn();
                return 0;
            }

        }

        [WebMethod]
        public static int setMachinesAdvSeq(string[] arrMachid, string[] arrAdv, string machSeq)
        {
            dbconn dbc = new dbconn();
            dbc.connDB();
            SqlTransaction myTrans;

            myTrans = dbc.conn.BeginTransaction();
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = dbc.conn;
            cmd.Transaction = myTrans;
            string machID = "";
            string appPath = HttpContext.Current.Server.MapPath(".");
            try
            {
                for (int i = 0; i < arrMachid.Length; i++)
                {
                    machID = arrMachid[i].ToString();
                    if (!string.IsNullOrEmpty(machID))
                    {
                        // To do: get ADV_VER first, and increase version.
                        string sqlUpdate = "update M_MACHINE_LIST set ADV_SEQ = '" + machSeq + "' where M_MACH_ID = '" + machID + "'";
                        cmd.CommandText = sqlUpdate;
                        cmd.ExecuteNonQuery();

                        string sqlDelete = "delete from M_MACH_ADV_SEQ where MACH_ID = '" + machID + "'";
                        cmd.CommandText = sqlDelete;
                        cmd.ExecuteNonQuery();

                        string dest_path = appPath + "\\Adv\\" + machID + "\\"; //string dest_path = appPath + "\\Adv\\Upload\\";
                        string dirFrom = appPath + "\\Adv\\";
                       // string[] filesList = Directory.GetFiles(dirFrom);
                        if (!Directory.Exists(dest_path))
                        {
                            Directory.CreateDirectory(dest_path);
                        }
                        else
                        {
                            // delete all file from directory.
                            foreach (string file in Directory.GetFiles(dest_path))
                            {
                                File.Delete(file);
                            }
                        }

                        DateTime dt = System.DateTime.Now;
                        string curtime = String.Format("{0:MM/dd/yyyy HH:mm:ss}", dt); // mssql datatime format is MM/dd/yyyy
                        for (int j = 0; j < arrAdv.Length; j++)
                        {
                            int iadvid = int.Parse(arrAdv[j].ToString());
                            string sSeq = (j + 1).ToString();
                            string sql = "insert into M_MACH_ADV_SEQ (MACH_ID, ADV_ID, ADV_SEQ, CREATED_DATE) VALUES ('" + machID + "', " + iadvid + ", '" + sSeq + "', GetDate())";
                            // string sql = "insert into M_MACH_ADV_SEQ (MACH_ID, ADV_ID, ADV_SEQ) VALUES ('" + machID + "', " + iadvid + ", '" + sSeq + "')";
                            cmd.CommandText = sql;
                            cmd.ExecuteNonQuery();


                            cmd.CommandText = "SELECT ADV_FILENAME FROM M_ADVERT_DATA WHERE ADV_ID = " + iadvid + "";
                            SqlDataReader dr = cmd.ExecuteReader();
                            string advFilename = "";
                            if (dr.HasRows)
                            {
                                while (dr.Read())
                                {
                                    advFilename = dr["ADV_FILENAME"].ToString().ToLower();
                                }
                            }
                            dr.Close();

                            // copy file to specify machine foloder, eg. 01CD

                            string filename = "";
                            string extname = Path.GetExtension(advFilename);
                            if (iadvid.ToString().Length == 1)
                            {
                                filename = "K0" + iadvid + extname;
                            }
                            else
                            {
                                filename = "K" + iadvid + extname;
                            }

                            string fullsourcepath = appPath + "\\Adv\\"+ advFilename;
                            string fulldestpath = dest_path + filename;
                            File.Copy(fullsourcepath, fulldestpath, true);
                        }
                    }
                }
                myTrans.Commit();
                dbc.closeConn();

                return 1;
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show("setMachAdvSeq err: " + ex.Message);
                myTrans.Rollback();
                dbc.closeConn();
                return 0;
            }

        }

        //[WebMethod]
        //public static advDetails[] getAdDetails()
        //{
        //    //System.Windows.Forms.MessageBox.Show("VendorDetails [] ");
        //    dbconn dbc = new dbconn();
        //    dbc.connDB();
        //    SqlCommand cmd = dbc.conn.CreateCommand();
        //    //"SELECT * FROM [M_CUSTOMER] ORDER BY [M_MACH_COUNTRY], [M_CUST_NAME]" 
        //    cmd.CommandText = "SELECT ADV_ID, ADV_PATH, ADV_FILENAME, ADV_TYPE, ADV_FILE_SRC FROM M_ADVERT_DATA ORDER BY ADV_ID";
        //    SqlDataReader dr = cmd.ExecuteReader();

        //    List<advDetails> details = new List<advDetails>();

        //    if (dr.HasRows)
        //    {
        //        while (dr.Read())
        //        {
        //            advDetails imgdetail = new advDetails();
        //            imgdetail.ADV_ID = dr["ADV_ID"].ToString();
        //            imgdetail.ADV_PATH = dr["ADV_PATH"].ToString();
        //            imgdetail.ADV_FILENAME = dr["ADV_FILENAME"].ToString();
        //            imgdetail.ADV_TYPE = dr["ADV_TYPE"].ToString();
        //            imgdetail.ADV_FILE_SRC = dr["ADV_FILE_SRC"].ToString();
        //            details.Add(imgdetail);
        //        }
        //    }
        //    return details.ToArray();
        //}

        [WebMethod]
        public static advDefaultSeq[] getAdDefaultSeq()
        {
            dbconn dbc = new dbconn();
            dbc.connDB();
            SqlCommand cmd = dbc.conn.CreateCommand();
            cmd.CommandText = "SELECT ADV_ID, ADV_PATH, ADV_FILENAME, ADV_TYPE, ADV_FILE_SRC FROM M_ADVERT_DATA WHERE ADV_SEQ != '0' ORDER BY ADV_SEQ";
            SqlDataReader dr = cmd.ExecuteReader();

            List<advDefaultSeq> details = new List<advDefaultSeq>();

            if (dr.HasRows)
            {
                while (dr.Read())
                {
                    advDefaultSeq imgdetail = new advDefaultSeq();
                    imgdetail.ADV_ID = dr["ADV_ID"].ToString();
                    imgdetail.ADV_PATH = dr["ADV_PATH"].ToString();
                    imgdetail.ADV_FILENAME = dr["ADV_FILENAME"].ToString();
                    imgdetail.ADV_TYPE = dr["ADV_TYPE"].ToString();
                    //imgdetail.ADV_FILE_SRC = dr["ADV_FILE_SRC"].ToString();
                    imgdetail.ADV_THUMBNAME = dr["ADV_THUMBNAME"].ToString();
                    details.Add(imgdetail);
                }
            }
            return details.ToArray();
        }


    }
}