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
using System.Runtime.InteropServices;
using RototypeIntl.KioskFileHandler;
using System.Diagnostics;

namespace Maestro.views.kioskMaintenance.advertisement
{
    public partial class main : System.Web.UI.Page
    {

        public class advDetails
        {
            public string ADV_ID { get; set; }
            public string ADV_PATH { get; set; }
            public string ADV_FILENAME { get; set; }
            public string ADV_TYPE { get; set; }
            public string ADV_FILE_SRC { get; set; }
            public string ADV_THUMBNAME { get; set; }
        }

        public class advDefaultSeq
        {
            public string ADV_ID { get; set; }
            public string ADV_PATH { get; set; }
            public string ADV_FILENAME { get; set; }
            public string ADV_TYPE { get; set; }
            public string ADV_FILE_SRC { get; set; }
            public string ADV_THUMBNAME { get; set; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
        }

        //testing
        public bool ThumbnailCallback()
        {
            return true;
        }

        //testing
        // Resize a Bitmap  
        private static Bitmap ResizeImage(Bitmap image, int width, int height)
        {
            Bitmap resizedImage = new Bitmap(width, height);
            using (Graphics gfx = Graphics.FromImage(resizedImage))
            {
                gfx.DrawImage(image, new Rectangle(0, 0, width, height),
                    new Rectangle(0, 0, image.Width, image.Height), GraphicsUnit.Pixel);
            }
            return resizedImage;
        } 

        [WebMethod]
        public static int UploadAdvertisement(object[] files)
        {
            try
            {
                foreach (Dictionary<string, object> temp in files)
                {
                    object name;
                    object src;
                    object type;
                    object imgSizeWidth;
                    object imgSizeHeight;
                    object thumbWidth;
                    object thumbHeight;
                    temp.TryGetValue("name", out name);
                    temp.TryGetValue("src", out src);
                    temp.TryGetValue("type", out type);
                    temp.TryGetValue("imgSizeWidth", out imgSizeWidth);
                    temp.TryGetValue("imgSizeHeight", out imgSizeHeight);
                    temp.TryGetValue("thumbWidth", out thumbWidth);
                    temp.TryGetValue("thumbHeight", out thumbHeight);

                    string FileName = name.ToString();
                    string ImgSrc = src.ToString();
                    string FileType = type.ToString();

                    int imgNewWidth = int.Parse(imgSizeWidth.ToString());
                    int imgNewHeight = int.Parse(imgSizeHeight.ToString());
                    int thumbNewWidth = int.Parse(thumbWidth.ToString());
                    int thumbNewHeight = int.Parse(thumbHeight.ToString());

                    string fType = FileType.Substring(0, FileType.IndexOf("/"));

                    string appPath = HttpContext.Current.Server.MapPath(".");
                    string advPath = appPath + "\\Adv\\"; // store full in order for file distributor to pick up.
                    //string filepath = advPath + FileName;
                    string filepath = advPath + FileName;
                    string thumbname = "";
                    string oriFileExt = Path.GetExtension(FileName);
                    string fileExtName = oriFileExt.Substring(1, oriFileExt.Length - 1);

                    dbconn dbc = new dbconn();
                    dbc.connDB();
                    SqlTransaction myTrans;

                    myTrans = dbc.conn.BeginTransaction();
                    SqlCommand cmd = new SqlCommand();
                    cmd.Connection = dbc.conn;
                    cmd.Transaction = myTrans;

                    List<int> listAdvID = new List<int>();

                    cmd.CommandText = "SELECT ADV_ID, ADV_FILENAME FROM M_ADVERT_DATA"; 
                    SqlDataReader dr = cmd.ExecuteReader();

                    if (dr.HasRows)
                    {
                        while (dr.Read())
                        {
                            listAdvID.Add(int.Parse(dr["ADV_ID"].ToString()));
                            string existingFile = dr["ADV_FILENAME"].ToString().ToLower();
                            if (existingFile.Equals(FileName.ToLower()))
                            {
                                dr.Close();
                                dbc.closeConn();
                                return -1;
                            }
                        }
                    }
                    dr.Close();

                    // generate new id.
                    bool getNewID = true;
                    int newAdvID = 1;
                    if (listAdvID.Count > 0)
                    {
                        while (getNewID)
                        {
                            Random r = new Random();
                            newAdvID = r.Next(1, 99);
                            foreach (int adv_id in listAdvID) // Loop through List with foreach
                            {
                                if (adv_id == newAdvID)
                                {
                                    getNewID = true;
                                    break;
                                }
                                else
                                {
                                    getNewID = false;
                                }
                            }
                        }
                    }

                    if (fType.Equals("image"))
                    {
                        // using regex replace to remove `data:image...` prefix
                        string pattern = @"data:image/(gif|png|jpeg|jpg|bmp);base64,";
                        string imgString = Regex.Replace(ImgSrc, pattern, string.Empty);

                        byte[] imageBytes = Convert.FromBase64CharArray(imgString.ToCharArray(), 0, imgString.Length);
                        using (MemoryStream ms = new MemoryStream(imageBytes))
                        {
                            System.Drawing.Image img = System.Drawing.Image.FromStream(ms);
                            System.Drawing.Image pThumbnail = img.GetThumbnailImage(thumbNewWidth, thumbNewHeight, null, new IntPtr());
                            string fileName = Path.GetFileNameWithoutExtension(FileName);
                            //thumbname = fileName + "_"+ fileExtName + ".jpg";
                            thumbname = newAdvID + ".jpg";
                            pThumbnail.Save(appPath + "\\Adv\\Thumbnail\\" + thumbname);
                            
                            int imgOriWidth = img.Size.Width;
                            int imgOriHeight = img.Height;
                            int newHeight;  
                            int newWidth;
                            int LargestWidth = imgNewWidth;
                            int LargestHeight = imgNewHeight;

                            if (imgOriWidth > imgOriHeight && imgOriWidth > LargestWidth)
                            {
                                newWidth = LargestWidth;
                                newHeight = (newWidth * imgOriHeight) / imgOriWidth;
                            }
                            else if (imgOriHeight > imgOriWidth && imgOriHeight > LargestHeight)
                            {
                                newHeight = LargestHeight;
                                newWidth = (newHeight * imgOriWidth / imgOriHeight);
                            }
                            else
                            {
                                newHeight = imgOriHeight;
                                newWidth = imgOriWidth;
                            }
                            
                            Bitmap startBitmap = new Bitmap(ms);
                            Bitmap resizedImage = new Bitmap(newWidth, newHeight);
                            using (Graphics gfx = Graphics.FromImage(resizedImage))
                            {
                                gfx.DrawImage(startBitmap, new Rectangle(0, 0, newWidth, newHeight),
                                    new Rectangle(0, 0, startBitmap.Width, startBitmap.Height), GraphicsUnit.Pixel);
                            }
                            Bitmap newBitmap = resizedImage;
                            newBitmap.Save(appPath + "\\Adv\\" + FileName);
                        }

                        //// write to file
                        //using (FileStream file = new FileStream(filepath, FileMode.Create, FileAccess.Write))
                        //{
                        //    file.Write(imageBytes, 0, imageBytes.Length);
                        //}
                    }
                    else if (fType.Equals("video"))
                    {
                        // using regex replace to remove `data:image...` prefix
                        string pattern = @"data:video/(mp4|avi|wmv|x-ms-wmv);base64,";
                        string imgString = Regex.Replace(ImgSrc, pattern, string.Empty);

                        //byte[] imageBytes = Convert.FromBase64CharArray(imgString.ToCharArray(), 0, imgString.Length);
                        byte[] imageBytes = Convert.FromBase64String(imgString);
                        using (FileStream file = new FileStream(filepath, FileMode.Create, FileAccess.Write))
                        {
                            file.Write(imageBytes, 0, imageBytes.Length);
                        }

                        string videoName = Path.GetFileNameWithoutExtension(FileName);
                        string thumbpath = "";
                        string thumbargs = "";
                        thumbpath = advPath + "Thumbnail\\";
                        thumbname = newAdvID + ".jpg";
                        string ffmpegPath = "C:\\ffmpeg\\ffmpeg.exe";
                        string thumbname_fullpath = thumbpath + thumbname;
                        //thumbargs = "-i " + inputfile + " -vframes 1 -ss 00:00:03 -s 150x130 -f image2 " + thumbname_fullpath;
                        thumbargs = "-i " + filepath + " -vframes 1 -ss 00:00:01 -s " + thumbNewWidth + "x" + thumbNewHeight + " -f image2 " + thumbname_fullpath;
                        Process thumbproc = new Process();
                        thumbproc = new Process();
                        thumbproc.StartInfo.FileName = ffmpegPath; // can include to project ?
                        thumbproc.StartInfo.UseShellExecute = false;
                        thumbproc.StartInfo.CreateNoWindow = false;
                        thumbproc.StartInfo.RedirectStandardOutput = false;

                            thumbproc.StartInfo.Arguments = thumbargs;
                            thumbproc.Start();
                            if (thumbproc != null)
                            {
                                thumbproc.Close();
                            }
                        
                        //convert avi to mp4 in order to play on web in html5
                            //if (!Path.GetExtension(FileName).ToLower().Equals(".mp4"))
                            //{
                                //var fileName = fileinfor.Name;
                                var outputPath = advPath + "\\mp4\\";
                                string cmdmp4 = " -i \"" + filepath + "\" \"" + outputPath + "\\" + newAdvID + ".mp4" + "\"";

                                System.Diagnostics.Process proc = new System.Diagnostics.Process();
                                proc.StartInfo.FileName = ffmpegPath;
                                //Path of exe that will be executed, only for "filebuffer" it will be "flvtool2.exe"
                                proc.StartInfo.Arguments = cmdmp4;
                                //The command which will be executed
                                proc.StartInfo.UseShellExecute = false;
                                proc.StartInfo.CreateNoWindow = true;
                                proc.StartInfo.RedirectStandardOutput = false;
                                proc.Start();
                                if (proc != null)
                                {
                                    proc.Close();
                                }
                            //}
                    }
                    

                    DateTime dt = System.DateTime.Now;
                    string curtime = String.Format("{0:MM/dd/yyyy HH:mm:ss}", dt); // mssql datatime format is MM/dd/yyyy
                    try
                    {
                        // mssql v2005 not suppot
                        //If Not Exists(select * from M_ADVERT_DATA where ADV_FILENAME='CSD1000.jpg')
                        //Begin
                        //insert into M_ADVERT_DATA (ADV_FILENAME,ADV_PATH, ADV_TYPE) values ('1448523', 'aa','aaa')
                        //End

                        //string sql = "insert into M_ADVERT_DATA (ADV_PATH, ADV_FILENAME, ADV_TYPE, ADV_FILE_SRC, ADV_SEQ, CREATED_DATE, ADV_THUMBNAME) values ('"
                        //    + advPath + "', '" + FileName + "', '" + fType + "', '" + ImgSrc + "', '0', '" + curtime + "', '" + thumbname + "')";
                        string sql = "insert into M_ADVERT_DATA (ADV_ID, ADV_PATH, ADV_FILENAME, ADV_TYPE, ADV_SEQ, CREATED_DATE, ADV_THUMBNAME) values ('"
                           + newAdvID + "', '" + advPath + "', '" + FileName + "', '" + fType + "', '0', '" + curtime + "', '" + thumbname + "')";
                        cmd.CommandText = sql;
                        cmd.ExecuteNonQuery();
                        myTrans.Commit();
                        dbc.closeConn();
                    }
                    catch (Exception ex)
                    {
                        myTrans.Rollback();
                        dbc.closeConn();
                        return 0;
                    }
                }

                return 1;

            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show("UploadAd err: " + ex.Message);
                return 0;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="FileName"></param>
        /// <param name="ImgSrc"></param>
        /// <param name=""></param>
        /// <returns>return 1: Insert success, -1: Failed to insert due to record exists. 0: Failed to insert due to Database or SQL error.</returns>
        /*
        [WebMethod]
        public static int uploadAdv(string FileName, string ImgSrc, string FileType)
        {
            try
            {
                // original source string
                //string src = @"data:image/gif;base64,R0lGODlhEAAOALMAAOazToeHh0tLS/7LZv/0jvb29t/ f3//Ub//ge8WSLf/rhf/3kdbW1mxsbP//mf///yH5BAAAAAAALAAAAAAQAA4AAARe8L1Ekyky67 QZ1hLnjM5UUde0ECwLJoExKcppV0aCcGCmTIHEIUEqjgaORCMxIC6e0CcguWw6aFjsVMkkIr7g7 7ZKPJjPZqIyd7sJAgVGoEGv2xsBxqNgYPj/gAwXEQA7";
                //string src = ImgSrc;

                string fType = FileType.Substring(0, FileType.IndexOf("/"));
                //To do: check file type from FileName;

                string advPath = @"C:\Adv\Default\";
                string filepath = advPath + FileName;

                if (fType.Equals("image"))
                {
                    // using regex replace to remove `data:image...` prefix
                    string pattern = @"data:image/(gif|png|jpeg|jpg|bmp);base64,";
                    string imgString = Regex.Replace(ImgSrc, pattern, string.Empty);

                    byte[] imageBytes = Convert.FromBase64CharArray(imgString.ToCharArray(), 0, imgString.Length);
                    //using (MemoryStream ms = new MemoryStream(imageBytes))
                    //{
                    //    System.Drawing.Image image = System.Drawing.Image.FromStream(ms);
                    //    //pictureBox1.Image = image;
                    //}

                    // write to file
                    using (FileStream file = new FileStream(filepath, FileMode.Create, FileAccess.Write))
                    {
                        file.Write(imageBytes, 0, imageBytes.Length);
                    }
                }
                else if (fType.Equals("video"))
                {
                    // using regex replace to remove `data:image...` prefix
                    string pattern = @"data:video/(mp4|avi|wmv|x-ms-wmv);base64,";
                    string imgString = Regex.Replace(ImgSrc, pattern, string.Empty);

                    //byte[] imageBytes = Convert.FromBase64CharArray(imgString.ToCharArray(), 0, imgString.Length);
                    byte[] imageBytes = Convert.FromBase64String(imgString);
                    using (FileStream file = new FileStream(filepath, FileMode.Create, FileAccess.Write))
                    {
                        file.Write(imageBytes, 0, imageBytes.Length);
                    }
                }

                dbconn dbc = new dbconn();
                dbc.connDB();
                SqlTransaction myTrans;

                myTrans = dbc.conn.BeginTransaction();
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = dbc.conn;
                cmd.Transaction = myTrans;

                DateTime dt = System.DateTime.Now;
                string curtime = String.Format("{0:MM/dd/yyyy HH:mm:ss}", dt); // mssql datatime format is MM/dd/yyyy
                try
                {
                    string sql = "insert into M_ADVERT_DATA (ADV_PATH, ADV_FILENAME, ADV_TYPE, ADV_FILE_SRC, ADV_SEQ, CREATED_DATE) values ('"
                        + advPath + "', '" + FileName + "', '" + fType + "', '" + ImgSrc + "', '0', '" + curtime + "')";
                    cmd.CommandText = sql;
                    cmd.ExecuteNonQuery();
                    myTrans.Commit();
                    dbc.closeConn();
                }
                catch (Exception ex)
                {
                    dbc.closeConn();
                }

                return 1;

            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show("UploadAd err: " + ex.Message);
                return 0;
            }
            //return true;
        }
        */

        [WebMethod]
        public static int setAdvSeq(string[] arrAdv)
        {
            dbconn dbc = new dbconn();
            dbc.connDB();
            SqlTransaction myTrans;

            myTrans = dbc.conn.BeginTransaction();
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = dbc.conn;
            cmd.Transaction = myTrans;
            SqlDataReader dr = null;

            try
            {
                string sqlUpdate = "update M_ADVERT_DATA set ADV_SEQ = '0'";
                cmd.CommandText = sqlUpdate;
                cmd.ExecuteNonQuery();

                for (int i = 0; i < arrAdv.Length; i++)
                {
                    int iadvid = int.Parse(arrAdv[i].ToString());
                    string sSeq = (i + 1).ToString();
                    string sql = "update M_ADVERT_DATA set ADV_SEQ = '" + sSeq + "' where ADV_ID = '" + iadvid + "'";
                    cmd.CommandText = sql;
                    cmd.ExecuteNonQuery();
                }
                
                myTrans.Commit();

                // delete all file from default folder.

                string appPath = HttpContext.Current.Server.MapPath(".");
                string dirFrom = appPath + "\\Adv\\Default\\";
                string[] filesList = Directory.GetFiles(dirFrom);
                if (!Directory.Exists(dirFrom))
                {
                    Directory.CreateDirectory(dirFrom);
                }
                else
                {
                    // delete all file from directory.
                    foreach (string file in Directory.GetFiles(dirFrom))
                    {
                        File.Delete(file);
                    }
                }

                // copy ad files to Default folder.
                for (int i = 0; i < arrAdv.Length; i++)
                {
                    int iadvid = int.Parse(arrAdv[i].ToString()); 
                    string sSeq = (i + 1).ToString();

                    cmd.CommandText = "SELECT ADV_ID, ADV_PATH, ADV_FILENAME, ADV_TYPE, ADV_FILE_SRC FROM M_ADVERT_DATA where ADV_ID = '" + iadvid + "'";
                    dr = cmd.ExecuteReader();


                    if (dr.HasRows)
                    {
                        while (dr.Read())
                        {
                            string ADV_ID = dr["ADV_ID"].ToString();
                            if (ADV_ID.Length == 1)
                            {
                                ADV_ID = "0" + ADV_ID;
                            }
                            string ADV_PATH = dr["ADV_PATH"].ToString();
                            string ADV_FILENAME = dr["ADV_FILENAME"].ToString();
                            //string dest_path = @"C:\Adv\Upload\";
                            string dest_path = appPath + "\\Adv\\Default\\"; //string dest_path = appPath + "\\Adv\\Upload\\";
                            //string source_path = @"C:\Adv\Default\";
                            string fileExt = Path.GetExtension(ADV_FILENAME);
                            string fullsourcepath = ADV_PATH + ADV_FILENAME;
                            string fulldestpath = dest_path + "K"+ ADV_ID+ fileExt;
                            File.Copy(fullsourcepath, fulldestpath, true);
                            
                        }
                    }
                    dr.Close();
                }

                dbc.closeConn();
                return 1;
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show("setAdvSeq err: " + ex.Message);
                myTrans.Rollback();
                dbc.closeConn();
                return 0;
            }

        }
        
        [WebMethod]
        public static advDetails[] getAdDetails()
        {
            //System.Windows.Forms.MessageBox.Show("VendorDetails [] ");
            dbconn dbc = new dbconn();
            dbc.connDB();
            SqlCommand cmd = dbc.conn.CreateCommand();
            //"SELECT * FROM [M_CUSTOMER] ORDER BY [M_MACH_COUNTRY], [M_CUST_NAME]" 
            cmd.CommandText = "SELECT ADV_ID, ADV_PATH, ADV_FILENAME, ADV_TYPE, ADV_FILE_SRC, ADV_THUMBNAME FROM M_ADVERT_DATA ORDER BY CREATED_DATE";
            SqlDataReader dr = cmd.ExecuteReader();

            List<advDetails> details = new List<advDetails>();

            if (dr.HasRows)
            {
                while (dr.Read())
                {
                    advDetails imgdetail = new advDetails();
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

        [WebMethod]
        public static advDefaultSeq[] getAdDefaultSeq()
        {
            dbconn dbc = new dbconn();
            dbc.connDB();
            SqlCommand cmd = dbc.conn.CreateCommand();
            cmd.CommandText = "SELECT ADV_ID, ADV_PATH, ADV_FILENAME, ADV_TYPE, ADV_FILE_SRC, ADV_THUMBNAME FROM M_ADVERT_DATA WHERE ADV_SEQ != '0' ORDER BY ADV_SEQ";
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

        
        [WebMethod]
        public static int delAdvDefaultSeq(string advID)
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
                cmd.CommandText = "SELECT DISTINCT MACH_ID FROM M_MACH_ADV_SEQ";
                SqlDataReader dr = cmd.ExecuteReader();

                List<string> listMID = new List<string>();
                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        listMID.Add( dr["MACH_ID"].ToString());
                    }
                }
                dr.Close();


                int iadvId = int.Parse(advID);
                string sqlUpdate = "update M_ADVERT_DATA set ADV_SEQ = '0' WHERE ADV_ID = " + iadvId;
                cmd.CommandText = sqlUpdate;
                cmd.ExecuteNonQuery();

                // delete from M_MACH_ADV_SEQ
                //string sqlDelete = "delete from M_MACH_ADV_SEQ where ADV_ID = " + iadvId;
                string sqlDelete = "update M_MACH_ADV_SEQ set ADV_SEQ = '0' WHERE ADV_ID = " + iadvId;
                cmd.CommandText = sqlDelete;
                cmd.ExecuteNonQuery();

                for (int k = 0; k < listMID.Count; k++)
                {
                    string mid = listMID[k];

                    cmd.CommandText = "SELECT ADV_ID FROM M_MACH_ADV_SEQ WHERE MACH_ID = '" + mid + 
                        "' AND ADV_SEQ <> '0' ORDER BY ADV_SEQ";
                    SqlDataReader dr2 = cmd.ExecuteReader();
                    string strseq = "";
                    if (dr2.HasRows)
                    {
                        while (dr2.Read())
                        {
                            string advid = dr2["ADV_ID"].ToString();
                            if (advid.Length == 1)
                            {
                                strseq += "0" + advid;
                            }
                            else
                            {
                                strseq += advid;
                            }
                        }
                    }
                    dr2.Close();

                    if (strseq.Length < 30)
                    {
                        for (var i = 0; i < 30; i++)
                        {
                            strseq = strseq + "0";
                            if (strseq.Length == 30)
                            {
                                break;
                            }
                        }
                    }

                    string sqlUpdateMachSeq = "update M_MACHINE_LIST set ADV_SEQ = '" + strseq + "' where M_MACH_ID = '" + mid + "'";
                    cmd.CommandText = sqlUpdateMachSeq;
                    cmd.ExecuteNonQuery();
                }


                myTrans.Commit();
                dbc.closeConn();

                return 1;
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show("delAdvDefaultSeq err: " + ex.Message);
                myTrans.Rollback();
                dbc.closeConn();
                return 0;
            }

        }

        [WebMethod]
        public static int delAdvFile(string advID)
        {
            dbconn dbc = new dbconn();
            dbc.connDB();
            SqlTransaction myTrans;

            myTrans = dbc.conn.BeginTransaction();
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = dbc.conn;
            cmd.Transaction = myTrans;
            int iadvId = int.Parse(advID);
            try
            {
                cmd.CommandText = "SELECT DISTINCT ADV_ID FROM M_MACH_ADV_SEQ WHERE ADV_SEQ <> '0' AND ADV_ID = "+iadvId +"";
                SqlDataReader dr = cmd.ExecuteReader();

                if (dr.HasRows)
                {
                    dr.Close();
                    dbc.closeConn();
                    return 0;
                }
                dr.Close();

                //delete physical file.
                cmd.CommandText = "SELECT ADV_FILENAME, ADV_PATH, ADV_THUMBNAME FROM M_ADVERT_DATA WHERE ADV_ID = " + iadvId + "";
                dr = cmd.ExecuteReader();
                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        string fileAdv = HttpContext.Current.Server.MapPath("Adv\\"+dr["ADV_FILENAME"].ToString());
                        string fileThumb = HttpContext.Current.Server.MapPath("Adv\\Thumbnail\\" + dr["ADV_THUMBNAME"].ToString());

                        string fileExt = Path.GetExtension(dr["ADV_FILENAME"].ToString());
                        string fileName = "";
                        if (iadvId.ToString().Length == 1)
                        {
                            fileName = "K0" + iadvId + fileExt;
                        }
                        else
                        {
                            fileName = "K" + iadvId + fileExt;
                        }
                        string fileDefault = HttpContext.Current.Server.MapPath("Adv\\Default\\" + fileName);
                        string fileMp4 = HttpContext.Current.Server.MapPath("Adv\\mp4\\" + iadvId+ ".mp4");
                        if (File.Exists(fileAdv))
                        {
                            File.Delete(fileAdv);
                        }
                        if (File.Exists(fileThumb))
                        {
                            File.Delete(fileThumb);
                        }
                        if (File.Exists(fileDefault))
                        {
                            File.Delete(fileDefault);
                        }
                        if (File.Exists(fileMp4))
                        {
                            File.Delete(fileMp4);
                        }
                    }
                }
                dr.Close();


                // delete from M_ADVERT_DATA
                string sqlDelete = "delete from M_ADVERT_DATA where ADV_ID = " + iadvId;
                cmd.CommandText = sqlDelete;
                cmd.ExecuteNonQuery();

                myTrans.Commit();
                dbc.closeConn();

                return 1;
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show("delAdvFile err: " + ex.Message);
                myTrans.Rollback();
                dbc.closeConn();
                return 0;
            }

        }


        [WebMethod]
        public static int regenSeqToAll(string[] arrAdv)
        {
            return regenAllMachSeq(arrAdv);
        }

        private static int regenAllMachSeq(string[] arrAdv)
        {
            dbconn dbc = new dbconn();
            dbc.connDB();
            SqlTransaction myTrans;

            myTrans = dbc.conn.BeginTransaction();
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = dbc.conn;
            cmd.Transaction = myTrans;
            SqlDataReader dr = null;

            try
            {

                string sqlUpdate = "update M_ADVERT_DATA set ADV_SEQ = '0'";
                cmd.CommandText = sqlUpdate;
                cmd.ExecuteNonQuery();
                string defSeq = "";

                for (int i = 0; i < arrAdv.Length; i++)
                {
                    int iadvid = int.Parse(arrAdv[i].ToString());
                    string sSeq = (i + 1).ToString();
                    string sql = "update M_ADVERT_DATA set ADV_SEQ = '" + sSeq + "' where ADV_ID = '" + iadvid + "'";
                    cmd.CommandText = sql;
                    cmd.ExecuteNonQuery();
                    if (iadvid.ToString().Length == 1)
                    {
                        defSeq += "0" + iadvid;
                    }
                    else
                    {
                        defSeq += iadvid;
                    }
                }

                if (defSeq.Length < 30)
                {
                    for (var i = 0; i < 30; i++)
                    {
                        defSeq = defSeq + "0";
                        if (defSeq.Length == 30)
                        {
                            break;
                        }
                    }
                }

                // delete all file from default folder.

                string appPath = HttpContext.Current.Server.MapPath(".");
                string dirFrom = appPath + "\\Adv\\Default\\";
                //string[] filesList = Directory.GetFiles(dirFrom);
                if (!Directory.Exists(dirFrom))
                {
                    Directory.CreateDirectory(dirFrom);
                }
                else
                {
                    // delete all file from directory.
                    foreach (string file in Directory.GetFiles(dirFrom))
                    {
                        File.Delete(file);
                    }
                }

                // first, update the machines to default seq.

                string sqlUpdateAllMachDefSeq = "update M_MACHINE_LIST set ADV_SEQ = '" + defSeq + "' where M_MACH_TYPE = 'CSD'";
                cmd.CommandText = sqlUpdateAllMachDefSeq;
                cmd.ExecuteNonQuery();

                // copy ad files to Default folder.
                for (int i = 0; i < arrAdv.Length; i++)
                {
                    int iadvid = int.Parse(arrAdv[i].ToString());
                    string sSeq = (i + 1).ToString();

                    cmd.CommandText = "SELECT ADV_ID, ADV_PATH, ADV_FILENAME, ADV_TYPE, ADV_FILE_SRC FROM M_ADVERT_DATA where ADV_ID = '" + iadvid + "'";
                    dr = cmd.ExecuteReader();


                    if (dr.HasRows)
                    {
                        while (dr.Read())
                        {
                            string ADV_ID = dr["ADV_ID"].ToString();
                            if (ADV_ID.Length == 1)
                            {
                                ADV_ID = "0" + ADV_ID;
                            }
                            string ADV_PATH = dr["ADV_PATH"].ToString();
                            string ADV_FILENAME = dr["ADV_FILENAME"].ToString();
                            //string dest_path = @"C:\Adv\Upload\";
                            string dest_path = appPath + "\\Adv\\Default\\"; //string dest_path = appPath + "\\Adv\\Upload\\";
                            //string source_path = @"C:\Adv\Default\";
                            string fileExt = Path.GetExtension(ADV_FILENAME);
                            string fullsourcepath = ADV_PATH + ADV_FILENAME;
                            string fulldestpath = dest_path + "K" + ADV_ID + fileExt;
                            File.Copy(fullsourcepath, fulldestpath, true);

                        }
                    }
                    dr.Close();
                }

                // copy all the specific machine adv files to each folder in order to push files to machines.

                string sqlDelAdvSeq = "delete t1 from M_MACH_ADV_SEQ t1, M_ADVERT_DATA t2 where t2.ADV_SEQ = 0 and t1.ADV_ID = t2.ADV_ID";
                cmd.CommandText = sqlDelAdvSeq;
                cmd.ExecuteNonQuery();

                //check specific machine adv seq, and upload latest seq.
                cmd.CommandText = "SELECT DISTINCT MACH_ID FROM M_MACH_ADV_SEQ";
                dr = cmd.ExecuteReader();

                List<string> listMID = new List<string>();
                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        listMID.Add(dr["MACH_ID"].ToString());

                        string dirDest = appPath + "\\Adv\\" + dr["MACH_ID"].ToString() + "\\";
                        //filesList = Directory.GetFiles(dirDest);
                        if (!Directory.Exists(dirDest))
                        {
                            Directory.CreateDirectory(dirDest);
                        }
                        else
                        {
                            // delete all file from directory.
                            foreach (string file in Directory.GetFiles(dirDest))
                            {
                                File.Delete(file);
                            }
                        }
                    }
                }
                dr.Close();

                // then, update specific machines 
                for (int k = 0; k < listMID.Count; k++)
                {
                    string mid = listMID[k];

                    cmd.CommandText = "SELECT t1.ADV_ID, t2.ADV_FILENAME FROM M_MACH_ADV_SEQ t1, M_ADVERT_DATA t2 " +
                                        "WHERE t1.MACH_ID = '01CD' " +
                                        "AND t1.ADV_ID = t2.ADV_ID " +
                                        "AND t1.ADV_SEQ <> '0' " +
                                        "ORDER BY t1.ADV_SEQ";
                    SqlDataReader dr2 = cmd.ExecuteReader();
                    string strseq = "";
                    if (dr2.HasRows)
                    {
                        while (dr2.Read())
                        {
                            string advid = dr2["ADV_ID"].ToString();
                            if (advid.Length == 1)
                            {
                                advid = "0" + advid;
                                strseq += advid;
                            }
                            else
                            {
                                strseq += advid;
                            }
                            // copy from default to specific folder.
                            string ADV_FILENAME = dr2["ADV_FILENAME"].ToString();
                            string fileExt = Path.GetExtension(ADV_FILENAME);
                            string fullsourcepath = appPath + "\\Adv\\" + ADV_FILENAME;
                            string fulldestpath = appPath + "\\Adv\\" + mid + "\\" + "K" + advid + fileExt;
                            File.Copy(fullsourcepath, fulldestpath, true);
                        }
                    }
                    dr2.Close();

                    if (strseq.Length < 30)
                    {
                        for (var i = 0; i < 30; i++)
                        {
                            strseq = strseq + "0";
                            if (strseq.Length == 30)
                            {
                                break;
                            }
                        }
                    }

                    string sqlUpdateMachSeq = "update M_MACHINE_LIST set ADV_SEQ = '" + strseq + "' where M_MACH_ID = '" + mid + "'";
                    cmd.CommandText = sqlUpdateMachSeq;
                    cmd.ExecuteNonQuery();
                }



                myTrans.Commit();

                dbc.closeConn();
                return 1;
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show("setAdvSeq err: " + ex.Message);
                myTrans.Rollback();
                dbc.closeConn();
                return 0;
            }

        }
    }
}