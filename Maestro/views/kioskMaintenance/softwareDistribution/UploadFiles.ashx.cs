using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using System.Text.RegularExpressions;
using Maestro.Classes;
using System.Web.Services;
using System.Json;
using System.Web.Configuration;
using System.Configuration;
using System.Data.SqlClient;

namespace Maestro.views.kioskMaintenance.softwareDistribution
{
    /// <summary>
    /// Summary description for UploadFiles
    /// </summary>
    public class UploadFiles : IHttpHandler, System.Web.SessionState.IReadOnlySessionState
    {
        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "application/json";
            JsonObject JSON = new JsonObject();

            dbconn dbc = new dbconn();
            dbc.connDB();
            SqlCommand cmd = dbc.conn.CreateCommand();
            SqlTransaction trans = dbc.conn.BeginTransaction();
            cmd.Connection = dbc.conn;
            cmd.Transaction = trans;

            try
            {
                if (context.Session["userId"] == null)
                {
                    throw new Exception("Session timeout. User not found!");
                }

                if (context.Request.Params["VersionName"] == null)
                {
                    throw new Exception("Invalid Version.");
                }
                if (context.Request.Params["Checksum"] == null)
                {
                    throw new Exception("Invalid Checksum.");
                }                
                if (context.Request.Files.Count == 0)
                {
                    throw new Exception("No file was found.");
                }

                string VersionName = context.Request.Params["VersionName"];
                string Checksum = context.Request.Params["Checksum"];
                HttpPostedFile postedFile = context.Request.Files[0];

                /******************
                 * SQL start here
                 */
                cmd.CommandText = "INSERT INTO M_SOFTWARE_VERSION (M_VERSION_NAME, M_FILENAME, M_CHECKSUM, CREATED_DATE) " +
                                    "VALUES (@VersionName, @FileName, @Checksum, GETDATE())";
                cmd.Parameters.AddWithValue("VersionName", VersionName);
                cmd.Parameters.AddWithValue("FileName", postedFile.FileName);
                cmd.Parameters.AddWithValue("Checksum", Checksum);
                cmd.ExecuteNonQuery();


                /**********************************
                 * Save the uploaded file at Server
                 * --------------------------------
                 * 1. Get the path from web.config (If not found, throw an exception)
                 * 2. Check if the folder existed, create a new one if it is not found
                 * 3. Save the file into the folder. 
                 */
                McObject<string> McObj = GetPath();
                if (!McObj.isSuccessful())
                {
                    throw new Exception(McObj.getMessage());
                }
                string FolderPath = McObj.getObject();
                if (!Directory.Exists(FolderPath))
                {
                    Directory.CreateDirectory(FolderPath);
                }     
        
                if (postedFile != null && postedFile.ContentLength > 0)
                {
                    var fileName = Path.GetFileName(postedFile.FileName);
                    fileName = Regex.Replace(fileName, @"\s|\$|\#\%", "");
                    var path = Path.Combine(FolderPath, fileName);

                    postedFile.SaveAs(path);
                }

                /* Commit if everything goes well */
                trans.Commit();

                JSON.Add("Status", true);
                JSON.Add("Message", "Successful");
            }
            catch (Exception ex)
            {
                /* Rollback if hit error */
                trans.Rollback();

                JSON.Add("Status", false);
                JSON.Add("Message", "Failed. " + ex.Message);
            }
            finally
            {
                dbc.closeConn();
                context.Response.Write(JSON);
            }
        }

        private McObject<string> GetPath()
        {
            try
            {
                Configuration WebConfig = WebConfigurationManager.OpenWebConfiguration("~");

                if (WebConfig.AppSettings.Settings["NewSoftVerPath"] == null)
                {
                    throw new Exception("Software Path not found. Please do the setting in web.config.");
                }
                else
                {
                    return new McObject<string>(true, "Successful.", WebConfig.AppSettings.Settings["NewSoftVerPath"].Value);
                }                
            }
            catch (Exception ex)
            {
                return new McObject<string>(false, ex.Message);
            }
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}