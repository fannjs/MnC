using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Maestro.views.reports.fullKioskList
{
    public partial class fullKioskListMain : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            txtUsername.Text = Session["userName"].ToString();
            GetMachineType();
        }


        private void GetMachineType()
        {
            dbconn dbc = new dbconn();
            dbc.connDB();
            SqlCommand cmd = dbc.conn.CreateCommand();
            try
            {
                cmd.CommandText = "SELECT distinct M_MACH_TYPE FROM M_CODES";
                SqlDataReader dr = cmd.ExecuteReader();

                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        ListItem LI = new ListItem();
                        LI.Value = dr["M_MACH_TYPE"].ToString();
                        LI.Text = dr["M_MACH_TYPE"].ToString();

                        selectKioskType.Items.Add(LI);
                    }
                }

                dr.Close();
            }
            catch (Exception ex)
            {
                //lblmsg.Text = ex.ToString();
            }
            finally
            {
                if (dbc.conn != null)
                {
                    dbc.closeConn();
                }
            }
        }
    }
}