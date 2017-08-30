using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.SqlClient;
using System.Text;
using System.Data;
using System.Linq;

namespace Maestro.app_monitoring
{
    public partial class EventStatus : System.Web.UI.Page
    {

        protected void Page_Load(object sender, EventArgs e)
        {
            string error = "Display Red, Sound siren.mp3";
            string warning = "Display Yellow, Sound alarm_clock.mp3";

            foreach (string loc in LocationList())
            {
                ErrorAction_Init ea_i = GetErrorAction_Init(loc);

                ImageButton imgBtn = new ImageButton();
                imgBtn.ID = loc;
                form1.Controls.Add(imgBtn);
                imgBtn.Click += new ImageClickEventHandler(this.LocationButton_Click);
              
                if (ea_i.ErrAct.Contains(error))
                {
                    imgBtn.ImageUrl = "~/assets/images/ERROR_FLASH.gif";
                }
                else if (ea_i.ErrAct.Contains(warning))
                {
                    imgBtn.ImageUrl = "~/assets/images/WARNING_FLASH.gif";
                }
                else
                {
                    imgBtn.ImageUrl = "~/assets/images/OK.gif";
                }
            }

            
        }

        public class ErrorPair
        {
            public string ErrCode { get; set; }
            public int CritLvl { get; set; }
        }

        public class ErrorAction_Init
        {
            public List<string> Objid { get; set; }
            public List<string> ErrDesc { get; set; }
            public List<string> ErrAct { get; set; }
           
        }
       
        public class ErrorAction
        {
            public string Objid { get; set; }
            public string ErrDesc { get; set; }
            public string ErrAct { get; set; }
        }

        public List<string> LocationList()
        {
            List<string> locIdList = new List<string>();
            DataTable dt = GetLocation();

            foreach (DataRow row in dt.Rows)
            {
                string id = row.ItemArray[0].ToString();
                locIdList.Add(id);
            }
            return locIdList;
        }
        
        

        


        private DataTable GetLocation()
        {
            string constr = @"Data Source=HTSVN\SQLEXPRESS;database=HTExpress;User id=SSTAuto;Password=1qQA2wWS3eED;";
            using (SqlConnection con = new SqlConnection(constr))
            {

                using (SqlCommand cmd = new SqlCommand("SELECT PRINT_LOC_CODE, LOCATION_NAME FROM [PRINTING_LOCATION]"))
                {
                    using (SqlDataAdapter sda = new SqlDataAdapter())
                    {
                        cmd.Connection = con;
                        sda.SelectCommand = cmd;
                        using (DataTable dt = new DataTable())
                        {
                            sda.Fill(dt);
                            con.Close();
                            return dt;
                        }
                    }
                }
            }
        }
        

        //To be triggered/called by database.M_EVENT_STATUS
        public static ErrorAction GetErrorAction()
        {
            dbconn dbc = new dbconn();
            dbc.connDB();
            SqlCommand cmd = dbc.conn.CreateCommand();

            cmd.CommandText = "SELECT M_EVENT_STATUS.M_EVENT_OBJID, M_ERROR_CODES.M_ERR_DESC, M_ERROR_CODES.M_ERR_ACTIONS " +
                                "FROM M_EVENT_STATUS, M_ERROR_CODES " +
                                "WHERE M_EVENT_STATUS.M_EVENT_ERRCODE = M_ERROR_CODES.M_ERR_CODE " +
                                "AND M_EVENT_STATUS.M_EVENT_CRITLVL = M_ERROR_CODES.M_CRITICAL_LVL";
            SqlDataReader dr = cmd.ExecuteReader();
           
            string id = dr["M_EVENT_OBJID"].ToString();
            string errDesc = dr["M_ERR_DESC"].ToString();
            string errAct = dr["M_ERR_ACTIONS"].ToString();

            dbc.closeConn();
            ErrorAction ea = new ErrorAction() { Objid = id, ErrDesc = errDesc, ErrAct = errAct };
            return ea;
        }
        
        //Get Error Details when machine start off 
        public ErrorAction_Init GetErrorAction_Init(string objid)
        {
            dbconn dbc = new dbconn();
            dbc.connDB();
            SqlCommand cmd = dbc.conn.CreateCommand();

            cmd.CommandText = "SELECT M_EVENT_STATUS.M_EVENT_OBJID, M_ERROR_CODES.M_ERR_DESC, M_ERROR_CODES.M_ERR_ACTIONS " +
                                "FROM M_EVENT_STATUS, M_ERROR_CODES " +
                                "WHERE M_EVENT_STATUS.M_EVENT_ERRCODE = M_ERROR_CODES.M_ERR_CODE " +
                                "AND M_EVENT_STATUS.M_EVENT_CRITLVL = M_ERROR_CODES.M_CRITICAL_LVL " +
                                "AND M_EVENT_STATUS.M_EVENT_OBJID = " + objid;
                                
            SqlDataReader dr = cmd.ExecuteReader();
            
            List<string> id = new List<string>();
            List<string> errDesc = new List<string>();
            List<string> errAct = new List<string>();

            if (dr.HasRows)
            {
                while (dr.Read())
                {
                    id.Add(dr["M_EVENT_OBJID"].ToString());
                    errDesc.Add(dr["M_ERR_DESC"].ToString());
                    errAct.Add(dr["M_ERR_ACTIONS"].ToString());
                }
            }

            dbc.closeConn();
            ErrorAction_Init ea_i = new ErrorAction_Init() { Objid = id, ErrDesc = errDesc, ErrAct = errAct};
            return ea_i;
        }

        public void DisplayErrDetails(object sender)
        {
            string args = ((ImageButton)sender).ID;

            ErrorAction_Init ea_i = GetErrorAction_Init(args);
            StringBuilder s_table = new StringBuilder();

            if (ea_i.Objid.Contains(args))
            {

                s_table.Append("<table border = '1'>");
                s_table.Append("<tr><th>Machine ID</th><th>Error Description</th><th>Error Actions</th>");

                for (int i = 0; i < ea_i.Objid.Count(); i++)
                {
                    s_table.Append("<tr><td>" + ea_i.Objid[i] + "</td><td>" + ea_i.ErrDesc[i] + "</td><td>" + ea_i.ErrAct[i] + "</td></tr>");
                }
                s_table.Append("</table>");
                
                PlaceHolder2.Controls.Add(new Literal { Text = s_table.ToString() });
            }
        }

       

        protected void LocationButton_Click(object sender, ImageClickEventArgs e)
        {

            DisplayErrDetails(sender);
        }





























        //public void DisplayLocation()
        //{

        //    StringBuilder html = new StringBuilder();
        //    DataTable dt = GetLocation();

        //    if (dt != null)
        //    {
        //        html.Append("<table>");
        //        html.Append("<tr>");
        //        foreach (DataRow row in dt.Rows)
        //        {
        //            html.Append("<td class='mnccentertext'>");
        //            html.Append("<div class='mnccircle-online'></div>");
        //            foreach (DataColumn column in dt.Columns)
        //            {
        //                html.Append("&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;");
        //                html.Append(row[column.ColumnName]);
        //                html.Append("<br/>");
        //            }
        //            html.Append("</td>");
        //        }

        //        html.Append("</tr>");
        //        //Table end.
        //        html.Append("</table>");

        //        //Append the HTML string to Placeholder.
        //        PlaceHolder1.Controls.Add(new Literal { Text = html.ToString() });

        //    }
        //    else
        //    {
        //        html.Append("<div class='err_circ'>HT E DB CON PB</div>");
        //        PlaceHolder1.Controls.Add(new Literal { Text = html.ToString() });
        //    }


        //}






    }
}