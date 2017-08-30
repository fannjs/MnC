using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Maestro.views.reports.statusReport
{
    public partial class mainStatusRpt : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            txtUsername.Text = Session["userName"].ToString();
            DateTime curdate = DateTime.Now;
            inputStartDate.Value = String.Format("{0:dd/MM/yyyy}", curdate);
            inputEndDate.Value = String.Format("{0:dd/MM/yyyy}", curdate); 
            GetMachineList();
        }

        private void GetMachineList()
        {
            dbconn dbc = new dbconn();
            dbc.connDB();
            SqlCommand cmd = dbc.conn.CreateCommand();

            cmd.CommandText = "SELECT M_MACH_ID FROM M_MACHINE_LIST";
            SqlDataReader dr = cmd.ExecuteReader();

            if (dr.HasRows)
            {
                while (dr.Read())
                {
                    ListItem LI = new ListItem();
                    LI.Value = dr["M_MACH_ID"].ToString();
                    LI.Text = dr["M_MACH_ID"].ToString();

                    //selectKioskId.Items.Add(LI);
                    selectKioskId.Items.Add(LI);
                }
            }

            dr.Close();
            dbc.closeConn();
        }

    }
}