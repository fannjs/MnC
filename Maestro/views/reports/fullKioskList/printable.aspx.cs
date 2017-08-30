using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Services;
using System.Data.SqlClient;

namespace Maestro.views.reports.fullKioskList
{
    public partial class printable : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        public class Machine
        {
            public string MachID { get; set; }
            public string MState { get; set; }
            public string MCity { get; set; }
            public string MAddress1 { get; set; }
            public string MAddress2 { get; set; }
            public string MBranch { get; set; }
            public string MPIC { get; set; }
        }

        [WebMethod]
        public static Machine[] getMachineList()
        {
            dbconn dbc = new dbconn();
            dbc.connDB();

            SqlCommand cmd = dbc.conn.CreateCommand();
            cmd.CommandText = "SELECT M_MACH_ID, M_MACH_STATE, M_MACH_CITYDISTRICT, M_MACH_ADDRESS1, M_MACH_ADDRESS2, M_BRANCH_NO, M_CONTACT_PERSON FROM M_MACHINE_LIST";

            SqlDataReader dr = cmd.ExecuteReader();
            List<Machine> machineList = new List<Machine>();

            if (dr.HasRows)
            {
                while (dr.Read())
                {
                    Machine machine = new Machine();
                    machine.MachID = dr["M_MACH_ID"].ToString().Trim();
                    machine.MState = dr["M_MACH_STATE"].ToString().Trim();
                    machine.MCity = dr["M_MACH_CITYDISTRICT"].ToString().Trim();
                    machine.MAddress1 = dr["M_MACH_ADDRESS1"].ToString().Trim();
                    machine.MAddress2 = dr["M_MACH_ADDRESS2"].ToString().Trim();
                    machine.MBranch = dr["M_BRANCH_NO"].ToString().Trim();
                    machine.MPIC = dr["M_CONTACT_PERSON"].ToString().Trim();
                    machineList.Add(machine);
                }
            }
            dr.Close();
            dbc.closeConn();

            return machineList.ToArray();
        }
    }
}