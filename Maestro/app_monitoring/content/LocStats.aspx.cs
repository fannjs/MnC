using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Maestro.app_monitoring.content
{
    public partial class LocStats : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                DataTable source = Source_Table("SELECT DISTINCT(M_CUSTOMER) FROM M_MACHINE_LIST");

                DataRow dr = source.NewRow();
                dr[0] = "All Sites";
                source.Rows.InsertAt(dr, 0);
                Feed_DropDownList(ddlSite, source, "M_CUSTOMER");
                //if (Session["userName"] != null)
                //{
                //    lblUserData1.Text = Session["userName"].ToString();
                //}
            }
        }
        protected DataTable Source_Table(String cmdSelect)
        {
            dbconn dbObj = new dbconn();
            dbObj.connDB();
            SqlDataAdapter adpt = new SqlDataAdapter(cmdSelect, dbObj.conn);
            DataTable dt = new DataTable();
            adpt.Fill(dt);
            dbObj.closeConn();
            return dt;
        }
        protected void Feed_DropDownList(DropDownList ddl, DataTable source, String val)
        {
            ddl.DataSource = source;
            ddl.DataTextField = val;
            ddl.DataValueField = val;
            ddl.DataBind();
        }
    }
}