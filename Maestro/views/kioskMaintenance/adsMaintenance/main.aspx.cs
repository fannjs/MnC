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
using Maestro.Classes;
using Maestro.Classes.MakerChecker;
using Maestro.Classes.Prefix;
using System.Json;
using System.Globalization;
using System.Web.Configuration;
using System.Configuration;

namespace Maestro.views.kioskMaintenance.adsMaintenance
{
    public partial class main : System.Web.UI.Page
    {
        private static string taskName = "";
        private static string ApplicationPath = HttpContext.Current.Server.MapPath("~/");
        private static string AdvFolderPath = ""; //GET FROM WEB.CONFIG

        private static string CurrentFilePath = HttpContext.Current.Server.MapPath(".");
        private static string AdvThumbnailFolderPath = CurrentFilePath + "\\Adv\\Thumbnail\\";

        private static int ThumbnailWidth = 170;
        private static int ThumbnailHeight = 150;
        private static int AdvWidth = 800;
        private static int AdvHeight = 600;

        protected void Page_Load(object sender, EventArgs e)
        {
            taskName = Request.Params["task"];

            Configuration webConfigApp = WebConfigurationManager.OpenWebConfiguration("~");
            AdvFolderPath = webConfigApp.AppSettings.Settings["AdvertisementPath"].Value;
        }

        public class AdvType
        {
            public static readonly int Global = 1;
        }

        public class M_MACHINE_LIST
        {
            public string M_MACH_ID { get; set; }
            public string M_BRANCH_CODE { get; set; }
            public string M_BRANCH_NAME { get; set; }
            public string M_ADDRESS1 { get; set; }
            public string M_ADDRESS2 { get; set; }
        }

        public class M_ADVERTISEMENT_LIST
        {
            public int Adv_Id { get; set; }
            public string Adv_FileName { get; set; }
            public int Adv_Type { get; set; }
        }

        public class Advertisement
        {
            public List<M_ADVERTISEMENT_LIST> SequenceList = new List<M_ADVERTISEMENT_LIST>();
            public List<M_ADVERTISEMENT_LIST> NoSequenceList = new List<M_ADVERTISEMENT_LIST>();
        }

        public class SpecificAdvertisement
        {
            public int GroupId { get; set; }
            public List<M_MACHINE_LIST> KioskList { get; set; }
        }

        public class AdvertisementQueue
        {
            public string M_MACH_ID { get; set; }
            public string GroupId { get; set; }
            public string Sequence { get; set; }
            public DateTime DownloadDT { get; set; }
            public DateTime DeployDT { get; set; }
        }

        private static List<int> ConvertSequence(string SequenceId)
        {
            List<int> SequenceList = new List<int>();
            int chunkSize = 2;

            for (int i = 0; i < SequenceId.Length; i += chunkSize)
            {
                if (i <= SequenceId.Length)
                {
                    int sequence = int.Parse(SequenceId.Substring(i, chunkSize));
                    SequenceList.Add(sequence);
                }
            }

            return SequenceList;
        }

        [WebMethod]
        public static McObject<Advertisement> GetAdvertisementList(string GroupId)
        {
            dbconn dbc = new dbconn();
            dbc.connDB();
            SqlCommand cmd = dbc.conn.CreateCommand();

            try
            {
                cmd.CommandText = "SELECT * FROM M_ADVERTISEMENT_LIST";
                SqlDataReader dr = cmd.ExecuteReader();
                Advertisement Ad = new Advertisement();
                List<M_ADVERTISEMENT_LIST> AdvertisementList = new List<M_ADVERTISEMENT_LIST>();

                List<M_ADVERTISEMENT_LIST> SequnceList = new List<M_ADVERTISEMENT_LIST>();
                List<M_ADVERTISEMENT_LIST> NoSequenceList = new List<M_ADVERTISEMENT_LIST>();

                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        M_ADVERTISEMENT_LIST Adv = new M_ADVERTISEMENT_LIST();
                        Adv.Adv_Id = dr.GetInt32(dr.GetOrdinal("ADVERT_ID"));
                        Adv.Adv_FileName = dr["ADVERT_FILENAME"].ToString();
                        Adv.Adv_Type = dr.GetInt16(dr.GetOrdinal("ADVERT_TYPE"));

                        AdvertisementList.Add(Adv);
                    }
                }
                else
                {
                    Ad.SequenceList = SequnceList;
                    Ad.NoSequenceList = NoSequenceList;
                    return new McObject<Advertisement>(true, "Successful.", Ad);
                }
                dr.Close();

                //If has advertisement, Check sequence
                cmd.CommandText = "SELECT ADVERT_SEQUENCE_ID FROM M_ADVERTISEMENT_GROUP WHERE ADVERT_GROUP_ID = " + GroupId;
                dr = cmd.ExecuteReader();

                if (dr.HasRows)
                {
                    if (dr.Read())
                    {
                        string SequenceId = dr["ADVERT_SEQUENCE_ID"].ToString().Trim();
                        List<int> SequenceList = ConvertSequence(SequenceId);

                        List<M_ADVERTISEMENT_LIST> NewAdvList = new List<M_ADVERTISEMENT_LIST>();
                        foreach (int seq in SequenceList)
                        {
                            for (int i = 0; i < AdvertisementList.Count; i++)
                            {
                                M_ADVERTISEMENT_LIST Adv = AdvertisementList[i];
                                if (Adv.Adv_Id.Equals(seq))
                                {
                                    NewAdvList.Add(Adv);
                                    AdvertisementList.RemoveAt(i);
                                    break;
                                }
                                else
                                {
                                    continue;
                                }
                            }
                        }

                        SequnceList = NewAdvList;
                    }
                }
                dr.Close();

                NoSequenceList = AdvertisementList;


                Ad.SequenceList = SequnceList;
                Ad.NoSequenceList = NoSequenceList;                
                return new McObject<Advertisement>(true, "Successful.", Ad);
            }
            catch (Exception ex)
            {
                return new McObject<Advertisement>(false, "Failed. " + ex.Message);
            }
            finally
            {
                dbc.closeConn();
            }
        }

        [WebMethod]
        public static McObject<List<SpecificAdvertisement>> GetSpecificAdv()
        {
            dbconn dbc = new dbconn();
            dbc.connDB();
            SqlCommand cmd = dbc.conn.CreateCommand();

            try
            {
                cmd.CommandText = "SELECT ADVERT_GROUP_ID FROM M_ADVERTISEMENT_GROUP WHERE ADVERT_GROUP_ID != 1"; //Filter by 1 (Global)
                SqlDataReader dr = cmd.ExecuteReader();
                List<SpecificAdvertisement> SpecificAdv = new List<SpecificAdvertisement>();

                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        SpecificAdvertisement SA = new SpecificAdvertisement();
                        SA.GroupId = dr.GetInt32(dr.GetOrdinal("ADVERT_GROUP_ID"));

                        SpecificAdv.Add(SA);
                    }
                }
                dr.Close();

                foreach (SpecificAdvertisement SA in SpecificAdv)
                {
                    cmd.CommandText = "SELECT M_MACH_ID FROM M_MACHINE_LIST WHERE ADVERT_GROUP_ID = " + SA.GroupId;
                    dr = cmd.ExecuteReader();

                    List<M_MACHINE_LIST> MachList = new List<M_MACHINE_LIST>();

                    if (dr.HasRows)
                    {
                        while (dr.Read())
                        {
                            M_MACHINE_LIST Mach = new M_MACHINE_LIST();
                            Mach.M_MACH_ID = dr["M_MACH_ID"].ToString();

                            MachList.Add(Mach);
                        }
                    }                    
                    dr.Close();

                    SA.KioskList = MachList;
                }

                return new McObject<List<SpecificAdvertisement>>(true, "Successful.", SpecificAdv);
            }
            catch (Exception ex)
            {
                return new McObject<List<SpecificAdvertisement>>(false, "Failed." + ex.Message);
            }
            finally
            {
                dbc.closeConn();
            }
        }

        [WebMethod]
        public static McObject<Boolean> UploadAdvertisement(object[] files)
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
                string sql = "";
                string htmlImages = "";

                /*****
                 * Check if file name has existed in the Advertisement Repository folder 
                 *****/
                //List<string> ExistedFileName = new List<string>();
                //foreach (Dictionary<string, object> file in files)
                //{
                //    object name;
                //    file.TryGetValue("name", out name);

                //    string FilePath = AdvFolderPath + name.ToString();

                //    if (File.Exists(FilePath))
                //    {
                //        ExistedFileName.Add(name.ToString());
                //    }
                //    else
                //    {
                //        continue;
                //    }
                //}
                //if (ExistedFileName.Count != 0)
                //{
                //    string fileNames = string.Join(" , ", ExistedFileName.ToArray());
                //    throw new Exception("File Name duplicated. " + Environment.NewLine + "[" + fileNames + "] existed.");
                //}

                cmd.CommandText = "SELECT ADVERT_ID FROM M_ADVERTISEMENT_LIST";
                SqlDataReader dr = cmd.ExecuteReader();
                List<int> AdvIdList = new List<int>();

                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        AdvIdList.Add(dr.GetInt32(dr.GetOrdinal("ADVERT_ID")));
                    }
                }
                dr.Close();

                /*
                 * Getting information
                 */
                foreach (Dictionary<string, object> temp in files)
                {
                    object name;
                    object src;
                    object type;
                    temp.TryGetValue("name", out name);
                    temp.TryGetValue("src", out src);
                    temp.TryGetValue("type", out type);

                    string FileName = name.ToString();
                    string ImgSrc = src.ToString();
                    string FileType = type.ToString();

                    string fType = FileType.Substring(0, FileType.IndexOf("/"));

                    string appPath = CurrentFilePath;
                    string advPath = appPath + "\\Adv\\"; // store full in order for file distributor to pick up.
                    string filepath = advPath + FileName;
                    string thumbname = "";

                    List<int> CurrentGeneratedId = new List<int>();
                    bool FoundNewId = false;
                    int NewAdvId = 1;
                    if (AdvIdList.Count > 0)
                    {
                        while (!FoundNewId)
                        {
                            Random r = new Random();
                            NewAdvId = r.Next(1, 99);
                            if (!AdvIdList.Contains(NewAdvId))
                            {
                                FoundNewId = true;
                                break;
                            }
                            else
                            {
                                FoundNewId = false;
                                continue;
                            }
                        }
                    }
                    else
                    {
                        while (!FoundNewId)
                        {
                            Random r = new Random();
                            NewAdvId = r.Next(1, 99);
                            if (!CurrentGeneratedId.Contains(NewAdvId))
                            {
                                FoundNewId = true;
                                CurrentGeneratedId.Add(NewAdvId);
                                break;
                            }
                            else
                            {
                                FoundNewId = false;
                                continue;
                            }
                        }
                    }

                    string AdvId = NewAdvId.ToString();
                    if (AdvId.Length == 1)
                    {
                        AdvId = "0" + AdvId;
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
                            System.Drawing.Image pThumbnail = img.GetThumbnailImage(ThumbnailWidth, ThumbnailHeight, null, new IntPtr());
                            //string fileName = Path.GetFileNameWithoutExtension(FileName);                            
                            thumbname = AdvId + ".jpg";
                            pThumbnail.Save(appPath + "\\Adv\\Thumbnail\\" + thumbname);

                            Bitmap startBitmap = new Bitmap(ms);
                            Bitmap resizedImage = new Bitmap(AdvWidth, AdvHeight);
                            using (Graphics gfx = Graphics.FromImage(resizedImage))
                            {
                                gfx.DrawImage(startBitmap, new Rectangle(0, 0, AdvWidth, AdvHeight),
                                    new Rectangle(0, 0, startBitmap.Width, startBitmap.Height), GraphicsUnit.Pixel);
                            }
                            Bitmap newBitmap = resizedImage;
                            //string fileName = Path.GetFileNameWithoutExtension(FileName);        
                            string FileExt = Path.GetExtension(FileName);
                            newBitmap.Save(AdvFolderPath + AdvId + FileExt);
                        }
                    }
                    else if (fType.Equals("video"))
                    {
                        // using regex replace to remove `data:image...` prefix
                        string pattern = @"data:video/(mp4|avi|wmv|x-ms-wmv);base64,";
                        string imgString = Regex.Replace(ImgSrc, pattern, string.Empty);

                        string FileExt = Path.GetExtension(FileName);
                        //byte[] imageBytes = Convert.FromBase64CharArray(imgString.ToCharArray(), 0, imgString.Length);
                        byte[] imageBytes = Convert.FromBase64String(imgString);
                        using (FileStream file = new FileStream(AdvFolderPath + AdvId + FileExt, FileMode.Create, FileAccess.Write))
                        {
                            file.Write(imageBytes, 0, imageBytes.Length);
                        }

                        string videoName = Path.GetFileNameWithoutExtension(FileName);
                        string thumbpath = "";
                        string thumbargs = "";
                        thumbpath = advPath + "Thumbnail\\";
                        thumbname = AdvId + ".jpg";
                        string ffmpegPath = "C:\\ffmpeg\\ffmpeg.exe";
                        string thumbname_fullpath = thumbpath + thumbname;
                        //thumbargs = "-i " + inputfile + " -vframes 1 -ss 00:00:03 -s 150x130 -f image2 " + thumbname_fullpath;
                        thumbargs = "-i " + AdvFolderPath + AdvId + FileExt + " -vframes 1 -ss 00:00:01 -s " + ThumbnailWidth + "x" + ThumbnailWidth + " -f image2 " + thumbname_fullpath;
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
                        string cmdmp4 = " -i \"" + AdvFolderPath + FileName + "\" \"" + outputPath + "\\" + videoName + ".mp4" + "\"";

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

                    int? AdvertType = null;
                    string FileNameWithoutExt = Path.GetFileNameWithoutExtension(FileName);
                    if (fType.Equals("image"))
                    {
                        AdvertType = AdvFileType.IMAGE;
                        string CustomFileName = AdvId + ".jpg";
                        htmlImages = htmlImages + "<div class=\"mkck-thumbnail-div\"><img class=\"mkck-thumbnail\" src=\"../views/kioskMaintenance/adsMaintenance/Adv/Thumbnail/" + CustomFileName + "\" /></div>";
                    }
                    else if (fType.Equals("video"))
                    {
                        AdvertType = AdvFileType.VIDEO;
                        string CustomeFileName = AdvId + ".mp4";
                        string CustomeThumbName = AdvId + ".jpg";
                        htmlImages = htmlImages + "<div class=\"mkck-thumbnail-div\"><video controls class=\"mkck-thumbnail\" src=\"../views/kioskMaintenance/adsMaintenance/Adv/mp4/" + CustomeFileName + "\" poster=\"../views/kioskMaintenance/adsMaintenance/Adv/Thumbnail/" + CustomeThumbName + "\" /></div>";
                    }

                    string FileExtension = Path.GetExtension(FileName);
                    string FormattedFileName = AdvId + FileExtension;
                    sql = sql + "INSERT INTO M_ADVERTISEMENT_LIST VALUES (" + NewAdvId + ",'" + FormattedFileName + "', " + AdvertType + ", GETDATE())";
                }
                
                JsonObject NEW = new JsonObject();
                NEW.Add("New File(s) Added", htmlImages);

                string newdata = NEW.ToString();

                cmd.CommandText = "INSERT INTO M_MAKER_CHECKER_LIST(TASK_ID,TASK_NAME,CREATED_DATE,NEW_VALUE,ACTION_TYPE,APPROVE_SQL,MAKER_ID,MAKER_USER_NAME,APPROVAL_STATUS) "
                                    + "VALUES(" + taskID + ", '" + taskName + "', GETDATE(), @newdata, '" + action + "', "
                                    + "@sql, " + sessionUID + ", '" + sessionUName + "', " + status + ")";
                cmd.Parameters.AddWithValue("newdata", newdata);
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

        [WebMethod]
        public static McObject<Boolean> DeleteAdvertisementFile(string AdvID)
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

                int Adv_Id = int.Parse(AdvID);

                string sql = "DELETE FROM M_ADVERTISEMENT_LIST WHERE ADVERT_ID = " + Adv_Id;
                string htmlImages = "";


                cmd.CommandText = "SELECT ADVERT_SEQUENCE_ID FROM M_ADVERTISEMENT_GROUP";
                SqlDataReader dr = cmd.ExecuteReader();

                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        string SequenceId = dr["ADVERT_SEQUENCE_ID"].ToString().Trim();
                        List<int> SequenceList = ConvertSequence(SequenceId);

                        if (SequenceList.Contains(Adv_Id))
                        {
                            throw new Exception("Unable to delete! This file is part of the sequences. Please remove it from the sequences first.");
                        }
                    }
                }
                dr.Close();

                cmd.CommandText = "SELECT ADVERT_ID, ADVERT_FILENAME, ADVERT_TYPE FROM M_ADVERTISEMENT_LIST WHERE ADVERT_ID = " + Adv_Id;
                dr = cmd.ExecuteReader();

                JsonObject OLD = new JsonObject();
                if (dr.HasRows)
                {
                    if (dr.Read())
                    {
                        int AdvId = dr.GetInt32(dr.GetOrdinal("ADVERT_ID"));
                        int Type = dr.GetInt16(dr.GetOrdinal("ADVERT_TYPE"));
                        string FileName = dr["ADVERT_FILENAME"].ToString();
                        string FileNameWithoutExt = Path.GetFileNameWithoutExtension(FileName);
                        string newHTMLImage = "";
                        string FileType = "";

                        if (Type.Equals(AdvFileType.IMAGE))
                        {
                            FileType = "IMAGE";
                            string CustomFileName = FileNameWithoutExt + ".jpg";
                            newHTMLImage = newHTMLImage + "<div class=\"mkck-thumbnail-div\"><img class=\"mkck-thumbnail\" src=\"../views/kioskMaintenance/adsMaintenance/Adv/Thumbnail/" + CustomFileName + "\" /></div>";
                        }
                        else if (Type.Equals(AdvFileType.VIDEO))
                        {
                            FileType = "VIDEO";
                            string CustomeFileName = FileNameWithoutExt + ".mp4";
                            string CustomeThumbName = FileNameWithoutExt + ".jpg";
                            newHTMLImage = newHTMLImage + "<div class=\"mkck-thumbnail-div\"><video controls class=\"mkck-thumbnail\" src=\"../views/kioskMaintenance/adsMaintenance/Adv/mp4/" + CustomeFileName + "\" poster=\"../views/kioskMaintenance/adsMaintenance/Adv/Thumbnail/" + CustomeThumbName + "\" /></div>";
                        }

                        OLD.Add("Type", FileType);
                        OLD.Add("File", newHTMLImage);
                    }
                }
                else
                {
                    throw new Exception("File not found.");
                }
                dr.Close();

                string olddata = OLD.ToString();

                cmd.CommandText = "INSERT INTO M_MAKER_CHECKER_LIST(TASK_ID,TASK_NAME,CREATED_DATE,OLD_VALUE,ACTION_TYPE,APPROVE_SQL,MAKER_ID,MAKER_USER_NAME,APPROVAL_STATUS) "
                                    + "VALUES(" + taskID + ", '" + taskName + "', GETDATE(), @olddata, '" + action + "', "
                                    + "@sql, " + sessionUID + ", '" + sessionUName + "', " + status + ")";
                cmd.Parameters.AddWithValue("olddata", olddata);
                cmd.Parameters.AddWithValue("sql", sql);
                cmd.ExecuteNonQuery();

                return new McObject<bool>(true, "Successful.");
            }
            catch (Exception ex)
            {
                return new McObject<bool>(false, "Failed. " + ex.Message);
            }
            finally
            {
                dbc.closeConn();
            }
        }
        
        [WebMethod]
        public static McObject<Boolean> SetGlobalSequence(string[] AdvArray, string DownloadDT, string DeployDT)
        {
            dbconn dbc = new dbconn();
            dbc.connDB();
            SqlCommand cmd = dbc.conn.CreateCommand();

            string SQLDelimiter = MCDelimiter.SQL;

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
                string sql = "";
                string newTaskName = taskName + " (Global Sequence)";

                //Start Populating data
                string SequenceId = "";
                foreach (string Id in AdvArray)
                {
                    string AdvId = Id;
                    if (AdvId.Length == 1)
                    {
                        AdvId = "0" + AdvId;
                    }
                    SequenceId = SequenceId + AdvId;
                }

                DateTime DownloadDateTime = DateTime.ParseExact(DownloadDT, "dd/MM/yyyy hh:mm tt", CultureInfo.InvariantCulture);
                DateTime DeployDateTime = DateTime.ParseExact(DownloadDT, "dd/MM/yyyy hh:mm tt", CultureInfo.InvariantCulture);

                sql = sql + "UPDATE M_ADVERTISEMENT_GROUP SET ADVERT_SEQUENCE_ID = '" + SequenceId + "' " +
                                    "WHERE ADVERT_GROUP_ID = " + AdvType.Global;
                sql = sql + SQLDelimiter;
                sql = sql + "INSERT INTO M_ADVERTISEMENT_QUEUE (M_MACH_ID, ADVERT_GROUP_ID, M_DOWNLOAD_AT, M_DEPLOY_AT, CREATED_DATE) "
                                    + "VALUES ('All', " + AdvType.Global + ", '" + DownloadDateTime.ToString("yyyy-MM-dd HH:mm:ss") + "', '" + DeployDateTime.ToString("yyyy-MM-dd HH:mm:ss") + "', GetDate())";
                sql = sql + SQLDelimiter;
                //sql = sql + "UPDATE M_MACHINE_LIST SET ADVERT_GROUP_ID = 1 WHERE ADVERT_GROUP_ID = 1 OR ADVERT_GROUP_ID IS NULL";
                //sql = sql + SQLDelimiter;

                
                
                //MKCK part
                string condition = string.Join(",", AdvArray);
                cmd.CommandText = "SELECT ADVERT_ID, ADVERT_FILENAME, ADVERT_TYPE FROM M_ADVERTISEMENT_LIST " +
                                    "WHERE ADVERT_ID IN (" + condition + ")";
                SqlDataReader dr = cmd.ExecuteReader();

                JsonObject temp = new JsonObject();

                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        int AdvId = dr.GetInt32(dr.GetOrdinal("ADVERT_ID"));
                        int Type = dr.GetInt16(dr.GetOrdinal("ADVERT_TYPE"));
                        string FileName = dr["ADVERT_FILENAME"].ToString();
                        string FileNameWithoutExt = Path.GetFileNameWithoutExtension(FileName);
                        string newHTMLImage = "";

                        if (Type.Equals(AdvFileType.IMAGE))
                        {
                            string CustomFileName = FileNameWithoutExt + ".jpg";
                            newHTMLImage = newHTMLImage + "<div class=\"mkck-thumbnail-div\"><img class=\"mkck-thumbnail\" src=\"../views/kioskMaintenance/adsMaintenance/Adv/Thumbnail/" + CustomFileName + "\" /></div>";
                        }
                        else if (Type.Equals(AdvFileType.VIDEO))
                        {
                            string CustomeFileName = FileNameWithoutExt + ".mp4";
                            string CustomeThumbName = FileNameWithoutExt + ".jpg";
                            newHTMLImage = newHTMLImage + "<div class=\"mkck-thumbnail-div\"><video controls class=\"mkck-thumbnail\" src=\"../views/kioskMaintenance/adsMaintenance/Adv/mp4/" + CustomeFileName + "\" poster=\"../views/kioskMaintenance/adsMaintenance/Adv/Thumbnail/" + CustomeThumbName + "\" /></div>";
                        }

                        temp.Add(AdvId.ToString(), newHTMLImage);
                    }
                }
                dr.Close();

                JsonObject NEW = new JsonObject();
                NEW.Add("Download Date Time", DownloadDateTime.ToString());
                NEW.Add("Deploy Date Time", DeployDateTime.ToString());

                int seqNo = 0;
                foreach (string id in AdvArray)
                {
                    seqNo = seqNo + 1;
                    string htmlImage = (string)temp[id];
                    NEW.Add("Seq." + seqNo.ToString(), htmlImage);
                }

                //Getting Old data 
                cmd.CommandText = "SELECT ADVERT_SEQUENCE_ID FROM M_ADVERTISEMENT_GROUP WHERE ADVERT_GROUP_ID = " + AdvType.Global;
                dr = cmd.ExecuteReader();

                string OldSequenceId = "";
                if (dr.HasRows)
                {
                    if (dr.Read())
                    {
                        OldSequenceId = dr["ADVERT_SEQUENCE_ID"].ToString().Trim();
                    }
                }
                else
                {
                    throw new Exception("Error. Global Advertisement Setting not found. Please contact Administration.");
                }
                dr.Close();

                JsonObject OLD = new JsonObject();
                JsonObject temp1 = new JsonObject();
                List<int> SequenceList = ConvertSequence(OldSequenceId);
                if (SequenceList.Count != 0)
                {
                    string OldSeqCondition = string.Join(",", SequenceList.ToArray());
                    cmd.CommandText = "SELECT ADVERT_ID, ADVERT_FILENAME, ADVERT_TYPE FROM M_ADVERTISEMENT_LIST " +
                                        "WHERE ADVERT_ID IN (" + OldSeqCondition + ")";
                    dr = cmd.ExecuteReader();

                    if (dr.HasRows)
                    {
                        while (dr.Read())
                        {
                            int AdvId = dr.GetInt32(dr.GetOrdinal("ADVERT_ID"));
                            int Type = dr.GetInt16(dr.GetOrdinal("ADVERT_TYPE"));
                            string FileName = dr["ADVERT_FILENAME"].ToString();
                            string FileNameWithoutExt = Path.GetFileNameWithoutExtension(FileName);
                            string newHTMLImage = "";

                            if (Type.Equals(AdvFileType.IMAGE))
                            {
                                string CustomFileName = FileNameWithoutExt + ".jpg";
                                newHTMLImage = newHTMLImage + "<div class=\"mkck-thumbnail-div\"><img class=\"mkck-thumbnail\" src=\"../views/kioskMaintenance/adsMaintenance/Adv/Thumbnail/" + CustomFileName + "\" /></div>";
                            }
                            else if (Type.Equals(AdvFileType.VIDEO))
                            {
                                string CustomeFileName = FileNameWithoutExt + ".mp4";
                                string CustomeThumbName = FileNameWithoutExt + ".jpg";
                                newHTMLImage = newHTMLImage + "<div class=\"mkck-thumbnail-div\"><video controls class=\"mkck-thumbnail\" src=\"../views/kioskMaintenance/adsMaintenance/Adv/mp4/" + CustomeFileName + "\" poster=\"../views/kioskMaintenance/adsMaintenance/Adv/Thumbnail/" + CustomeThumbName + "\" /></div>";
                            }

                            temp1.Add(AdvId.ToString(), newHTMLImage);
                        }
                    }
                    dr.Close();

                    seqNo = 0;
                    foreach (int Id in SequenceList)
                    {
                        seqNo = seqNo + 1;
                        string htmlImage = (string)temp1[Id.ToString()];
                        OLD.Add("Seq." + seqNo.ToString(), htmlImage);
                    }
                }
                else
                {
                    OLD.Add("No Sequence", "");
                }

                string olddata = OLD.ToString();
                string newdata = NEW.ToString();

                cmd.CommandText = "INSERT INTO M_MAKER_CHECKER_LIST(TASK_ID,TASK_NAME,CREATED_DATE,OLD_VALUE,NEW_VALUE,ACTION_TYPE,APPROVE_SQL,MAKER_ID,MAKER_USER_NAME,APPROVAL_STATUS) "
                                    + "VALUES(" + taskID + ", '" + newTaskName + "', GETDATE(), @olddata, @newdata, '" + action + "', "
                                    + "@sql, " + sessionUID + ", '" + sessionUName + "', " + status + ")";
                cmd.Parameters.AddWithValue("olddata", olddata);
                cmd.Parameters.AddWithValue("newdata", newdata);
                cmd.Parameters.AddWithValue("sql", sql);
                cmd.ExecuteNonQuery();

                return new McObject<bool>(true, "Successful.");
            }
            catch (Exception ex)
            {
                return new McObject<bool>(false, "Failed. " + ex.Message);
            }
            finally
            {
                dbc.closeConn();
            }
        }

        [WebMethod]
        public static McObject<Boolean> SetSpecificSequence(string GroupId, string[] KioskList, string[] AdvArray, string DownloadDT, string DeployDT)
        {
            dbconn dbc = new dbconn();
            dbc.connDB();
            SqlCommand cmd = dbc.conn.CreateCommand();

            try
            {
                if (string.IsNullOrEmpty(GroupId)) {throw new Exception("Invalid Group Id."); }
                if (KioskList.Length == 0) { throw new Exception("No kiosk was selected."); }
                if (AdvArray.Length == 0) { throw new Exception("No sequence was selected."); }
                if (string.IsNullOrEmpty(DownloadDT)) { throw new Exception("Invalid Download Date Time."); }
                if (string.IsNullOrEmpty(DeployDT)) { throw new Exception("Invalid Deploy Date Time."); }

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
                string action ="";
                int status = MCStatus.PENDING;
                string sql = "";
                string newTaskName = taskName + " (Specific Sequence)";


                // Formatting Kiosk ID
                List<string> FormattedKioskList = new List<string>();
                foreach (string Kiosk in KioskList)
                {
                    FormattedKioskList.Add("'" + Kiosk + "'");
                }
                string KioskCondition = string.Join(",", FormattedKioskList);
                // Completed
                

                //Formatting Advertisement Sequence
                string SequenceId = "";
                foreach (string Id in AdvArray)
                {
                    string AdvId = Id;
                    if (AdvId.Length == 1)
                    {
                        AdvId = "0" + AdvId;
                    }
                    SequenceId = SequenceId + AdvId;
                }                
                // Completed

                //Convert DateTime
                DateTime DownloadDateTime = DateTime.ParseExact(DownloadDT, "dd/MM/yyyy hh:mm tt", CultureInfo.InvariantCulture);
                DateTime DeployDateTime = DateTime.ParseExact(DownloadDT, "dd/MM/yyyy hh:mm tt", CultureInfo.InvariantCulture);
                //Completed


                JsonObject NEW = new JsonObject();
                JsonObject OLD = new JsonObject();


                //Get NEW sequence images
                string SeqCondition = string.Join(",", AdvArray);
                cmd.CommandText = "SELECT ADVERT_ID, ADVERT_FILENAME, ADVERT_TYPE FROM M_ADVERTISEMENT_LIST " +
                                    "WHERE ADVERT_ID IN (" + SeqCondition + ")";
                SqlDataReader dr = cmd.ExecuteReader();

                JsonObject temp = new JsonObject();

                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        int AdvId = dr.GetInt32(dr.GetOrdinal("ADVERT_ID"));
                        int Type = dr.GetInt16(dr.GetOrdinal("ADVERT_TYPE"));
                        string FileName = dr["ADVERT_FILENAME"].ToString();
                        string FileNameWithoutExt = Path.GetFileNameWithoutExtension(FileName);
                        string newHTMLImage = "";

                        if (Type.Equals(AdvFileType.IMAGE))
                        {
                            string CustomFileName = FileNameWithoutExt + ".jpg";
                            newHTMLImage = newHTMLImage + "<div class=\"mkck-thumbnail-div\"><img class=\"mkck-thumbnail\" src=\"../views/kioskMaintenance/adsMaintenance/Adv/Thumbnail/" + CustomFileName + "\" /></div>";
                        }
                        else if (Type.Equals(AdvFileType.VIDEO))
                        {
                            string CustomeFileName = FileNameWithoutExt + ".mp4";
                            string CustomeThumbName = FileNameWithoutExt + ".jpg";
                            newHTMLImage = newHTMLImage + "<div class=\"mkck-thumbnail-div\"><video controls class=\"mkck-thumbnail\" src=\"../views/kioskMaintenance/adsMaintenance/Adv/mp4/" + CustomeFileName + "\" poster=\"../views/kioskMaintenance/adsMaintenance/Adv/Thumbnail/" + CustomeThumbName + "\" /></div>";
                        }

                        temp.Add(AdvId.ToString(), newHTMLImage);
                    }
                }
                dr.Close();

                string NewKisokDesc = string.Join(", ", KioskList);
                NEW.Add("Kiosk", NewKisokDesc);
                NEW.Add("Download Date Time", DownloadDateTime.ToString());
                NEW.Add("Deploy Date Time", DeployDateTime.ToString());

                int seqNo = 0;
                foreach (string id in AdvArray)
                {
                    seqNo = seqNo + 1;
                    string htmlImage = (string)temp[id];
                    NEW.Add("Seq." + seqNo.ToString(), htmlImage);
                }

                
                /*** End of NEW ***/


                cmd.CommandText = "SELECT COUNT(*) FROM M_ADVERTISEMENT_GROUP WHERE ADVERT_GROUP_ID = " + GroupId;
                int count = (int)cmd.ExecuteScalar();
                if (count == 0)
                {
                    action = MCAction.ADD;

                    sql = "DECLARE @id int " +
                            "INSERT INTO M_ADVERTISEMENT_GROUP(ADVERT_SEQUENCE_ID, PACKAGE_FILE_PATH, CREATED_DATE) " +
                            "VALUES('" + SequenceId + "', '', GETDATE())" +
                            "SET @id = SCOPE_IDENTITY() ";
                    //Insert into queue first, the Group in MachineList will be updated by separate service
                    //sql = sql +     "UPDATE M_MACHINE_LIST SET ADVERT_GROUP_ID = @id WHERE M_MACH_ID IN (" + KioskCondition + ")";

                    foreach (string K in KioskList)
                    {
                        sql = sql + "INSERT INTO M_ADVERTISEMENT_QUEUE (M_MACH_ID, ADVERT_GROUP_ID, M_DOWNLOAD_AT, M_DEPLOY_AT, CREATED_DATE) "
                                    + "VALUES ('" + K + "', @id, '" + DownloadDateTime.ToString("yyyy-MM-dd HH:mm:ss") + "', '" + DownloadDateTime.ToString("yyyy-MM-dd HH:mm:ss") + "', GetDate())";
                    }
                }
                else
                {
                    action = MCAction.MODIFY;
                    
                    //Get OLD assigned kiosk
                    cmd.CommandText = "SELECT M_MACH_ID FROM M_MACHINE_LIST WHERE ADVERT_GROUP_ID = " + GroupId;
                    dr = cmd.ExecuteReader();
                    List<string> OldKioskList = new List<string>();
                    if (dr.HasRows)
                    {
                        while (dr.Read())
                        {
                            OldKioskList.Add(dr["M_MACH_ID"].ToString());
                        }
                    }
                    dr.Close();
                    string OldKioskDesc = string.Join(", ", OldKioskList.ToArray());
                    OLD.Add("Kiosk", OldKioskDesc);

                    //Compare Old and New Kiosk - To get the kiosk that need to update to specific or global
                    List<string> KioskToAdd = new List<string>();                    
                    foreach (string KioskId in KioskList)
                    {
                        if (!OldKioskList.Contains(KioskId))
                        {
                            KioskToAdd.Add(KioskId);
                        }
                    }

                    List<string> KioskToRemove = new List<string>();
                    foreach (string KioskId in OldKioskList)
                    {
                        if (!KioskList.Contains(KioskId))
                        {
                            KioskToRemove.Add(KioskId);
                        }
                    }

                    //For update new sequence, make sure the package_file_path set to blank (For the service to compress the file)                    
                    sql = "UPDATE M_ADVERTISEMENT_GROUP SET ADVERT_SEQUENCE_ID = '" + SequenceId + "', PACKAGE_FILE_PATH = ''" +
                                    "WHERE ADVERT_GROUP_ID = " + GroupId;
                    sql = sql + MCDelimiter.SQL;
                    
 
                    if (KioskToAdd.Count != 0)
                    {
                        foreach (string K in KioskToAdd)
                        {
                            sql = sql + "INSERT INTO M_ADVERTISEMENT_QUEUE (M_MACH_ID, ADVERT_GROUP_ID, M_DOWNLOAD_AT, M_DEPLOY_AT, CREATED_DATE) "
                                    + "VALUES ('" + K + "', " + GroupId + ", '" + DownloadDateTime.ToString("yyyy-MM-dd HH:mm:ss") + "', '" + DeployDateTime.ToString("yyyy-MM-dd HH:mm:ss") + "', GetDate())";
                            sql = sql + MCDelimiter.SQL;
                        }
                    }
                    if (KioskToRemove.Count != 0)
                    {
                        foreach (string K in KioskToRemove)
                        {
                            sql = sql + "INSERT INTO M_ADVERTISEMENT_QUEUE (M_MACH_ID, ADVERT_GROUP_ID, M_DOWNLOAD_AT, M_DEPLOY_AT, CREATED_DATE) "
                                    + "VALUES ('" + K + "', " + AdvType.Global + ", '" + DownloadDateTime.ToString("yyyy-MM-dd HH:mm:ss") + "', '" + DeployDateTime.ToString("yyyy-MM-dd HH:mm:ss") + "', GetDate())";
                            sql = sql + MCDelimiter.SQL;
                        }
                    }
                    

                    cmd.CommandText = "SELECT ADVERT_SEQUENCE_ID FROM M_ADVERTISEMENT_GROUP WHERE ADVERT_GROUP_ID = " + GroupId;
                    dr = cmd.ExecuteReader();
                    string OldSequenceId = "";
                    if (dr.HasRows)
                    {
                        if (dr.Read())
                        {
                            OldSequenceId = dr["ADVERT_SEQUENCE_ID"].ToString().Trim();
                        }
                    }
                    dr.Close();

                    JsonObject temp1 = new JsonObject();
                    List<int> SequenceList = ConvertSequence(OldSequenceId);
                    if (SequenceList.Count != 0)
                    {
                        string OldSeqCondition = string.Join(",", SequenceList.ToArray());
                        cmd.CommandText = "SELECT ADVERT_ID, ADVERT_FILENAME, ADVERT_TYPE FROM M_ADVERTISEMENT_LIST " +
                                            "WHERE ADVERT_ID IN (" + OldSeqCondition + ")";
                        dr = cmd.ExecuteReader();

                        if (dr.HasRows)
                        {
                            while (dr.Read())
                            {
                                int AdvId = dr.GetInt32(dr.GetOrdinal("ADVERT_ID"));
                                int Type = dr.GetInt16(dr.GetOrdinal("ADVERT_TYPE"));
                                string FileName = dr["ADVERT_FILENAME"].ToString();
                                string FileNameWithoutExt = Path.GetFileNameWithoutExtension(FileName);
                                string newHTMLImage = "";

                                if (Type.Equals(AdvFileType.IMAGE))
                                {
                                    string CustomFileName = FileNameWithoutExt + ".jpg";
                                    newHTMLImage = newHTMLImage + "<div class=\"mkck-thumbnail-div\"><img class=\"mkck-thumbnail\" src=\"../views/kioskMaintenance/adsMaintenance/Adv/Thumbnail/" + CustomFileName + "\" /></div>";
                                }
                                else if (Type.Equals(AdvFileType.VIDEO))
                                {
                                    string CustomeFileName = FileNameWithoutExt + ".mp4";
                                    string CustomeThumbName = FileNameWithoutExt + ".jpg";
                                    newHTMLImage = newHTMLImage + "<div class=\"mkck-thumbnail-div\"><video controls class=\"mkck-thumbnail\" src=\"../views/kioskMaintenance/adsMaintenance/Adv/mp4/" + CustomeFileName + "\" poster=\"../views/kioskMaintenance/adsMaintenance/Adv/Thumbnail/" + CustomeThumbName + "\" /></div>";
                                }

                                temp1.Add(AdvId.ToString(), newHTMLImage);
                            }
                        }
                        dr.Close();

                        seqNo = 0;
                        foreach (int Id in SequenceList)
                        {
                            seqNo = seqNo + 1;
                            string htmlImage = (string)temp1[Id.ToString()];
                            OLD.Add("Seq."+seqNo.ToString(), htmlImage);
                        }
                    }
                    else
                    {
                        OLD.Add("No Sequence", "");
                    }
                }

                string olddata = OLD.ToString();
                string newdata = NEW.ToString();

                cmd.CommandText = "INSERT INTO M_MAKER_CHECKER_LIST(TASK_ID,TASK_NAME,CREATED_DATE,OLD_VALUE,NEW_VALUE,ACTION_TYPE,APPROVE_SQL,MAKER_ID,MAKER_USER_NAME,APPROVAL_STATUS) "
                                    + "VALUES(" + taskID + ", '" + newTaskName + "', GETDATE(), @olddata, @newdata, '" + action + "', "
                                    + "@sql, " + sessionUID + ", '" + sessionUName + "', " + status + ")";
                cmd.Parameters.AddWithValue("olddata", olddata);
                cmd.Parameters.AddWithValue("newdata", newdata);
                cmd.Parameters.AddWithValue("sql", sql);
                cmd.ExecuteNonQuery();

                return new McObject<bool>(true, "Successful.");
            }
            catch(Exception ex)
            {
                return new McObject<bool>(false, "Failed. " + ex.Message);
            }
            finally
            {
                dbc.closeConn();
            }
        }

        [WebMethod]
        public static McObject<Boolean> DeleteSpecificSequence(string GroupId, string DownloadDT, string DeployDT)
        {
            dbconn dbc = new dbconn();
            dbc.connDB();
            SqlCommand cmd = dbc.conn.CreateCommand();

            try
            {
                if (string.IsNullOrEmpty(GroupId)) { throw new Exception("Invalid Group Id."); }

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
                string sql = "";

                string newTaskName = taskName + " (Specific Sequence)";

                cmd.CommandText = "SELECT M_MACH_ID FROM M_MACHINE_LIST WHERE ADVERT_GROUP_ID = " + GroupId;
                SqlDataReader dr = cmd.ExecuteReader();
                List<string> OldKioskList = new List<string>();
                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        OldKioskList.Add(dr["M_MACH_ID"].ToString());
                    }
                }
                dr.Close();
                string OldKioskDesc = string.Join(", ", OldKioskList.ToArray());

                //Convert DateTime
                DateTime DownloadDateTime = DateTime.ParseExact(DownloadDT, "dd/MM/yyyy hh:mm tt", CultureInfo.InvariantCulture);
                DateTime DeployDateTime = DateTime.ParseExact(DownloadDT, "dd/MM/yyyy hh:mm tt", CultureInfo.InvariantCulture);
                //Completed

                //Formatting SQL
                sql = sql + "DELETE FROM M_ADVERTISEMENT_GROUP WHERE ADVERT_GROUP_ID = " + GroupId;
                sql = sql + MCDelimiter.SQL;

                foreach (string K in OldKioskList)
                {
                    sql = sql + "INSERT INTO M_ADVERTISEMENT_QUEUE (M_MACH_ID, ADVERT_GROUP_ID, M_DOWNLOAD_AT, M_DEPLOY_AT, CREATED_DATE) "
                                   + "VALUES ('" + K + "', " + AdvType.Global + ", '" + DownloadDateTime.ToString("yyyy-MM-dd HH:mm:ss") + "', '" + DeployDateTime.ToString("yyyy-MM-dd HH:mm:ss") + "', GetDate())";
                    sql = sql + MCDelimiter.SQL;
                }


                cmd.CommandText = "SELECT ADVERT_SEQUENCE_ID FROM M_ADVERTISEMENT_GROUP WHERE ADVERT_GROUP_ID = " + GroupId;
                dr = cmd.ExecuteReader();
                string OldSequenceId = "";
                if (dr.HasRows)
                {
                    if (dr.Read())
                    {
                        OldSequenceId = dr["ADVERT_SEQUENCE_ID"].ToString().Trim();
                    }
                }
                dr.Close();

                JsonObject OLD = new JsonObject();
                JsonObject temp1 = new JsonObject();
                List<int> SequenceList = ConvertSequence(OldSequenceId);
                if (SequenceList.Count != 0)
                {
                    string OldSeqCondition = string.Join(",", SequenceList.ToArray());
                    cmd.CommandText = "SELECT ADVERT_ID, ADVERT_FILENAME, ADVERT_TYPE FROM M_ADVERTISEMENT_LIST " +
                                        "WHERE ADVERT_ID IN (" + OldSeqCondition + ")";
                    dr = cmd.ExecuteReader();

                    if (dr.HasRows)
                    {
                        while (dr.Read())
                        {
                            int AdvId = dr.GetInt32(dr.GetOrdinal("ADVERT_ID"));
                            int Type = dr.GetInt16(dr.GetOrdinal("ADVERT_TYPE"));
                            string FileName = dr["ADVERT_FILENAME"].ToString();
                            string FileNameWithoutExt = Path.GetFileNameWithoutExtension(FileName);
                            string newHTMLImage = "";

                            if (Type.Equals(AdvFileType.IMAGE))
                            {
                                string CustomFileName = FileNameWithoutExt + ".jpg";
                                newHTMLImage = newHTMLImage + "<div class=\"mkck-thumbnail-div\"><img class=\"mkck-thumbnail\" src=\"../views/kioskMaintenance/adsMaintenance/Adv/Thumbnail/" + CustomFileName + "\" /></div>";
                            }
                            else if (Type.Equals(AdvFileType.VIDEO))
                            {
                                string CustomeFileName = FileNameWithoutExt + ".mp4";
                                string CustomeThumbName = FileNameWithoutExt + ".jpg";
                                newHTMLImage = newHTMLImage + "<div class=\"mkck-thumbnail-div\"><video controls class=\"mkck-thumbnail\" src=\"../views/kioskMaintenance/adsMaintenance/Adv/mp4/" + CustomeFileName + "\" poster=\"../views/kioskMaintenance/adsMaintenance/Adv/Thumbnail/" + CustomeThumbName + "\" /></div>";
                            }

                            temp1.Add(AdvId.ToString(), newHTMLImage);
                        }
                    }
                    dr.Close();

                    OLD.Add("Note", "These Kiosk will be removed from Specific and set to Global.");
                    OLD.Add("Kiosk", OldKioskDesc);
                    OLD.Add("Download Date Time", DownloadDateTime.ToString());
                    OLD.Add("Deploy Date Time", DeployDateTime.ToString());

                    int seqNo = 0;
                    foreach (int Id in SequenceList)
                    {
                        seqNo = seqNo + 1;
                        string htmlImage = (string)temp1[Id.ToString()];
                        OLD.Add("Seq." + seqNo.ToString(), htmlImage);
                    }
                }

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
                return new McObject<bool>(false, "Failed." + ex.Message);
            }
            finally
            {
                dbc.closeConn();
            }
        }

        [WebMethod]
        public static McObject<List<M_MACHINE_LIST>> GetAllMachineList()
        {
            dbconn dbc = new dbconn();
            dbc.connDB();
            SqlCommand cmd = dbc.conn.CreateCommand();

            try
            {
                cmd.CommandText = "SELECT M_MACH_ID, ML.M_BRANCH_CODE, M_BRANCH_NAME, M_ADDRESS1, M_ADDRESS2 " +
                                    "FROM M_MACHINE_LIST ML, M_BRANCH B " + 
                                    "WHERE ML.M_BRANCH_CODE = B.M_BRANCH_CODE " +
                                    "AND (ADVERT_GROUP_ID = 1 OR ADVERT_GROUP_ID IS NULL) " +
                                    "ORDER BY M_MACH_ID asc";
                SqlDataReader dr = cmd.ExecuteReader();

                List<M_MACHINE_LIST> M_MACH_LIST = new List<M_MACHINE_LIST>();

                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        M_MACHINE_LIST M = new M_MACHINE_LIST();
                        M.M_MACH_ID = dr["M_MACH_ID"].ToString();
                        M.M_BRANCH_CODE = dr["M_BRANCH_CODE"].ToString();
                        M.M_BRANCH_NAME = dr["M_BRANCH_NAME"].ToString();
                        M.M_ADDRESS1 = dr["M_ADDRESS1"].ToString();
                        M.M_ADDRESS2 = dr["M_ADDRESS2"].ToString();

                        M_MACH_LIST.Add(M);
                    }
                }

                return new McObject<List<M_MACHINE_LIST>>(true, "Successful.", M_MACH_LIST);
            }
            catch (Exception ex)
            {
                return new McObject<List<M_MACHINE_LIST>>(false, "Failed. " + ex.Message);
            }
            finally
            {
                dbc.closeConn();
            }
        }

        [WebMethod]
        public static McObject<List<M_MACHINE_LIST>> GetSpecificMachineList(string GroupId)
        {
            dbconn dbc = new dbconn();
            dbc.connDB();
            SqlCommand cmd = dbc.conn.CreateCommand();

            try
            {
                List<string> GroupIdList = new List<string>();
                GroupIdList.Add(AdvType.Global.ToString());
                GroupIdList.Add(GroupId);

                string condition = string.Join(",", GroupIdList.ToArray());
                cmd.CommandText = "SELECT M_MACH_ID, ML.M_BRANCH_CODE, M_BRANCH_NAME, M_ADDRESS1, M_ADDRESS2 " +
                                    "FROM M_MACHINE_LIST ML, M_BRANCH B " +
                                    "WHERE ML.M_BRANCH_CODE = B.M_BRANCH_CODE " +
                                    "AND ADVERT_GROUP_ID IN(" + condition + ") " +
                                    "ORDER BY M_MACH_ID asc";
                SqlDataReader dr = cmd.ExecuteReader();

                List<M_MACHINE_LIST> M_MACH_LIST = new List<M_MACHINE_LIST>();

                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        M_MACHINE_LIST M = new M_MACHINE_LIST();
                        M.M_MACH_ID = dr["M_MACH_ID"].ToString();
                        M.M_BRANCH_CODE = dr["M_BRANCH_CODE"].ToString();
                        M.M_BRANCH_NAME = dr["M_BRANCH_NAME"].ToString();
                        M.M_ADDRESS1 = dr["M_ADDRESS1"].ToString();
                        M.M_ADDRESS2 = dr["M_ADDRESS2"].ToString();

                        M_MACH_LIST.Add(M);
                    }
                }

                return new McObject<List<M_MACHINE_LIST>>(true, "Successful.", M_MACH_LIST);
            }
            catch (Exception ex)
            {
                return new McObject<List<M_MACHINE_LIST>>(false, "Failed. " + ex.Message);
            }
            finally
            {
                dbc.closeConn();
            }
        }

        [WebMethod]
        public static McObject<List<AdvertisementQueue>> GetAdvQueue()
        {
            dbconn dbc = new dbconn();
            dbc.connDB();
            SqlCommand cmd = dbc.conn.CreateCommand();

            try
            {
                cmd.CommandText = "SELECT M_MACH_ID, AG.ADVERT_GROUP_ID, ADVERT_SEQUENCE_ID, M_DOWNLOAD_AT, M_DEPLOY_AT " +
                                    "FROM M_ADVERTISEMENT_GROUP AG, M_ADVERTISEMENT_QUEUE AQ " +
                                    "WHERE AG.ADVERT_GROUP_ID = AQ.ADVERT_GROUP_ID";
                SqlDataReader dr = cmd.ExecuteReader();
                List<AdvertisementQueue> AQ_List = new List<AdvertisementQueue>();

                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        AdvertisementQueue AQ = new AdvertisementQueue();
                        AQ.M_MACH_ID = dr["M_MACH_ID"].ToString();
                        AQ.GroupId = dr["ADVERT_GROUP_ID"].ToString();
                        AQ.Sequence = dr["ADVERT_SEQUENCE_ID"].ToString().Trim();
                        AQ.DownloadDT = dr.GetDateTime(dr.GetOrdinal("M_DOWNLOAD_AT"));
                        AQ.DeployDT = dr.GetDateTime(dr.GetOrdinal("M_DEPLOY_AT"));

                        AQ_List.Add(AQ);
                    }
                }

                return new McObject<List<AdvertisementQueue>>(true, "Successful.", AQ_List);
            }
            catch (Exception ex)
            {
                return new McObject<List<AdvertisementQueue>>(false, "Failed. " + ex.Message);
            }
            finally
            {
                dbc.closeConn();
            }
        }
    }
}