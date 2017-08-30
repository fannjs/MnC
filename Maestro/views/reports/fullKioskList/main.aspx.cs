using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.SqlClient;
using System.Web.Services;
using System.Web.Script.Services;

namespace Maestro.views.reports.fullKioskList
{
    public partial class main : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        public class Machine
        {
            public string TotalMachine { get; set; }
            public string MCount { get; set; } //Count
            public string MachID { get; set; }
            public string MState { get; set; }
            public string MCity { get; set; }
            public string MAddress1 { get; set; }
            public string MAddress2 { get; set; }
            public string MBranch { get; set; }
            public string MPIC { get; set; }
        }

        [WebMethod, ScriptMethod]
        public static int getCount()
        {
            dbconn dbc = new dbconn();
            dbc.connDB();

            SqlCommand cmd = dbc.conn.CreateCommand();
            cmd.CommandText = "SELECT COUNT(*) FROM M_MACHINE_LIST";

            int count = (Int32)cmd.ExecuteScalar();
            
            dbc.closeConn();

            return count;
        }

        [WebMethod, ScriptMethod]
        public static Machine[] getMachineList(string pageNumber, string pageSize)
        {
            int iPageNumber = Int32.Parse(pageNumber);
            int iPageSize = Int32.Parse(pageSize);
            int count = getCount();

            dbconn dbc = new dbconn();
            dbc.connDB();

            SqlCommand cmd = dbc.conn.CreateCommand();
            cmd.CommandText = "SELECT * FROM (SELECT ROW_NUMBER() OVER(ORDER BY M_MACH_ID) AS NUMBER,"
                                +"M_MACH_ID,M_STATE,M_DISTRICT,M_ADDRESS1,M_ADDRESS2,ML.M_BRANCH_CODE,M_CONTACT "
                                +"FROM M_MACHINE_LIST ML, M_BRANCH B "
                                +"WHERE ML.M_BRANCH_CODE = B.M_BRANCH_CODE) AS MLIST "
                                +"WHERE NUMBER BETWEEN ((@PageNumber - 1) * @PageSize + 1) AND (@PageNumber * @PageSize) "
                                +"ORDER BY M_MACH_ID";

            cmd.Parameters.AddWithValue("PageNumber", iPageNumber);
            cmd.Parameters.AddWithValue("PageSize", iPageSize);

            SqlDataReader dr = cmd.ExecuteReader();
            List<Machine> machineList = new List<Machine>();

            Machine m = new Machine();
            m.TotalMachine = count.ToString();
            machineList.Add(m);

            if (dr.HasRows)
            {
                while (dr.Read())
                {
                    Machine machine = new Machine();
                    machine.MCount = dr["NUMBER"].ToString().Trim();
                    machine.MachID = dr["M_MACH_ID"].ToString().Trim();
                    machine.MState = dr["M_STATE"].ToString().Trim();
                    machine.MCity = dr["M_DISTRICT"].ToString().Trim();
                    machine.MAddress1 = dr["M_ADDRESS1"].ToString().Trim();
                    machine.MAddress2 = dr["M_ADDRESS2"].ToString().Trim();
                    machine.MBranch = dr["M_BRANCH_CODE"].ToString().Trim();
                    machine.MPIC = dr["M_CONTACT"].ToString().Trim();

                    machineList.Add(machine);
                }
            }
            dr.Close();

            dbc.closeConn();

            return machineList.ToArray();
        }
    }
}