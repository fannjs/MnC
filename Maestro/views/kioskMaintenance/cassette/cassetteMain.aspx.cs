using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;
using Maestro.Classes;

namespace Maestro.views.kioskMaintenance.cassette
{
    public partial class cassetteMain : System.Web.UI.Page
    {

        public class Machine
        {
            public string MachineID { get; set; }
            public int GroupID { get; set; }
            public List<Cassette> CassetteList { get; set; }
            public string NumCount { get; set; }
            public string BranchCode { get; set; }
            public string BranchName { get; set; }
            public string Address1 { get; set; }
            public string Address2 { get; set; }
        }

        public class Cassette
        {
            public string CassetteID { get; set; }
            public string CassetteName { get; set; }
            public string CassetteDate { get; set; }
            public string CassetteInUse { get; set; }
            public string LastReplenishDate { get; set; }
        }

        [WebMethod]
        public static Machine[] getAllMachineList()
        {
            dbconn dbc = new dbconn();
            dbc.connDB();
            SqlCommand cmd = dbc.conn.CreateCommand();
            cmd.CommandText = "SELECT M_MACH_ID, B.M_BRANCH_CODE, M_BRANCH_NAME, M_ADDRESS1, M_ADDRESS2 "
                                + "FROM M_MACHINE_LIST ML, M_BRANCH B "
                                + "WHERE ML.M_BRANCH_CODE = B.M_BRANCH_CODE "
                                + "ORDER BY M_MACH_ID ASC";

            SqlDataReader dr = cmd.ExecuteReader();
            List<Machine> machineList = new List<Machine>();

            if (dr.HasRows)
            {
                while (dr.Read())
                {
                    Machine machine = new Machine();
                    machine.MachineID = dr["M_MACH_ID"].ToString().Trim();
                    machine.BranchCode = dr["M_BRANCH_CODE"].ToString().Trim();
                    machine.BranchName = dr["M_BRANCH_NAME"].ToString().Trim();
                    machine.Address1 = dr["M_ADDRESS1"].ToString().Trim();
                    machine.Address2 = dr["M_ADDRESS2"].ToString().Trim();

                    machineList.Add(machine);
                }
            }
            dbc.closeConn();

            return machineList.ToArray();
        }

        [WebMethod]
        public static Machine[] getCassetteDetail(string cassetteGroupID)
        {
            dbconn dbc = new dbconn();
            dbc.connDB();
            SqlCommand cmd = dbc.conn.CreateCommand();
            try
            {
                cmd.CommandText = "SELECT * " +
                                    " FROM M_CASSETTE_LIST WHERE M_CASSETTE_GROUP_ID = '" + cassetteGroupID + "'";

                SqlDataReader dr = cmd.ExecuteReader();
                List<Machine> details = new List<Machine>();

                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        Machine mach = new Machine();
                        //mach.CassetteName = dr["M_CASSETTE_NAME"].ToString();
                        //mach.CassetteDate = dr["M_CASSETTE_DATE"].ToString();
                        //mach.CassetteInUse = dr["M_CASSETTE_IN_USE"].ToString();
                        //mach.LastReplenishDate = dr["M_LAST_REPLENISH_DATE"].ToString();
                        details.Add(mach);
                    }
                }
                dr.Close();
                dbc.closeConn();
                return details.ToArray();
            }
            catch (Exception ex)
            {
                return null;
            }
            finally
            {
                if (dbc.conn != null)
                {
                    dbc.closeConn();
                }

            }
        }

        [WebMethod]
        public static McObject<Pagination<List<Machine>>> getMachList(string pageNumber, string recordPerPage)
        {
            dbconn dbc = new dbconn();
            dbc.connDB();
            //SqlCommand cmd = dbc.conn.CreateCommand();
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = dbc.conn;

            try
            {
                int iPageNumber = Int32.Parse(pageNumber);
                int iPageSize = Int32.Parse(recordPerPage);
                int count = getCount();


                cmd.CommandText = "SELECT * FROM (SELECT ROW_NUMBER() OVER(ORDER BY M_MACH_ID) AS NUMBER,"
                        + "M_MACH_ID "
                        + "FROM M_MACHINE_LIST) AS MLIST "
                        + "WHERE NUMBER BETWEEN ((@PageNumber-1) * @RowspPage +1) AND (@PageNumber * @RowspPage ) "
                        + "ORDER BY M_MACH_ID";
                
                cmd.Parameters.AddWithValue("PageNumber", iPageNumber);
                cmd.Parameters.AddWithValue("RowspPage", iPageSize);

                SqlDataReader dr = cmd.ExecuteReader();

                List<Machine> machineList = new List<Machine>();


                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        Machine mach = new Machine();
                        mach.MachineID = dr["M_MACH_ID"].ToString();
                        /*
                        if (!string.IsNullOrEmpty(dr["M_CASSETTE_GROUP_ID"].ToString()))
                        {
                            mach.GroupID = int.Parse(dr["M_CASSETTE_GROUP_ID"].ToString());
                        }*/

                        mach.NumCount = dr["NUMBER"].ToString();
                        machineList.Add(mach);
                    }
                }
                dr.Close();

                foreach (Machine m in machineList){
                    int groupid = 0;
                    string machid = m.MachineID;
                    if (!string.IsNullOrEmpty(m.GroupID.ToString()))
                    {
                        groupid = m.GroupID;
                    }
                       
                    cmd.CommandText = "SELECT M_CASSETTE_ID, M_CASSETTE_NAME, M_ASSIGNED_DATE, M_CASSETTE_IN_USE, M_LAST_REPLENISH_DATE, M_MACH_ID " +
                                    " FROM M_CASSETTE_LIST " +
                                    " WHERE M_MACH_ID = '" + machid + "' "+
                                    " ORDER BY M_CASSETTE_NAME ";
                        dr = cmd.ExecuteReader();
                        List<Cassette> CassetteList = new List<Cassette>();

                        if (dr.HasRows)
                        {
                            while (dr.Read())
                            {
                                Cassette cas = new Cassette();
                                cas.CassetteName = dr["M_CASSETTE_NAME"].ToString();
                                cas.CassetteDate = dr["M_ASSIGNED_DATE"].ToString();
                                cas.CassetteInUse = dr["M_CASSETTE_IN_USE"].ToString();
                                cas.LastReplenishDate = dr["M_LAST_REPLENISH_DATE"].ToString();

                                CassetteList.Add(cas);
                            }
                        }
                        m.CassetteList = CassetteList;

                        dr.Close();
                }

                Pagination<List<Machine>> pr = new Pagination<List<Machine>>(count, machineList);

                return new McObject<Pagination<List<Machine>>>(true, "Successful.", pr);
            }
            catch (Exception ex)
            {
                return new McObject<Pagination<List<Machine>>>(false, "Failed. " + ex.Message);
            }
            finally
            {
                dbc.closeConn();
            }
        }


        [WebMethod]
        public static McObject<List<Cassette>> editMachCassette(string machid)
        {
            dbconn dbc = new dbconn();
            dbc.connDB();
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = dbc.conn;

            try
            {
                cmd.CommandText = "SELECT * " +
                         " FROM M_CASSETTE_LIST " +
                         " WHERE (M_MACH_ID = '" + machid + "') ORDER BY M_CASSETTE_NAME";

                SqlDataReader dr = cmd.ExecuteReader();

                List<Cassette> cassetteList = new List<Cassette>();


                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        Cassette cass = new Cassette();
                        cass.CassetteName = dr["M_CASSETTE_NAME"].ToString();
                        cass.CassetteID = dr["M_CASSETTE_ID"].ToString();
                        cassetteList.Add(cass);
                    }
                }
                dr.Close();


                return new McObject<List<Cassette>>(true, "Successful.", cassetteList);
            }
            catch (Exception ex)
            {
                return new McObject<List<Cassette>>(false, "Failed. " + ex.Message);
            }
            finally
            {
                dbc.closeConn();
            }
        }

        public static int getCount()
        {
            dbconn dbc = new dbconn();
            dbc.connDB();
            try
            {
                SqlCommand cmd = dbc.conn.CreateCommand();
                cmd.CommandText = "SELECT COUNT(*) FROM M_MACHINE_LIST";

                int count = (Int32)cmd.ExecuteScalar();

                dbc.closeConn();

                return count;
            }
            catch (Exception ex)
            {
                return 0;
            }
            finally
            {
                if (dbc.conn != null){
                dbc.closeConn();
                }
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {

            GetMachineType();
        }

        private void GetMachineType()
        {
            dbconn dbc = new dbconn();
            dbc.connDB();
            SqlCommand cmd = dbc.conn.CreateCommand();
            try
            {
                cmd.CommandText = "SELECT * FROM M_MACHINE_TYPE";
                SqlDataReader dr = cmd.ExecuteReader();

                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        selectKioskType.Items.Add(dr["M_MACH_TYPE"].ToString());
                    }
                }
                dr.Close();
            }
            catch (Exception ex)
            {
            }
            finally
            {
                if (dbc.conn != null)
                {
                    dbc.closeConn();
                }
            }
        }

        [WebMethod]
        public static McObject<List<Cassette>> getAllCassette()
        {
            dbconn dbc = new dbconn();
            dbc.connDB();
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = dbc.conn;

            try
            {

                cmd.CommandText = "SELECT * " +
                         " FROM M_CASSETTE_LIST ";

                SqlDataReader dr = cmd.ExecuteReader();

                List<Cassette> cassetteList = new List<Cassette>();


                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        Cassette cass = new Cassette();
                        cass.CassetteName = dr["M_CASSETTE_NAME"].ToString();
                        cass.CassetteID = dr["M_CASSETTE_ID"].ToString();
                        cassetteList.Add(cass);
                    }
                }
                dr.Close();


                return new McObject<List<Cassette>>(true, "Successful.", cassetteList);
            }
            catch (Exception ex)
            {
                return new McObject<List<Cassette>>(false, "Failed. " + ex.Message);
            }
            finally
            {
                dbc.closeConn();
            }
        }


        [WebMethod]
        public static McObject<bool> addCassetteName(string CassetteName)
        {
            dbconn dbc = new dbconn();
            dbc.connDB();
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = dbc.conn;
            try
            {
                DateTime curDT = DateTime.Now;
                cmd.CommandText = "INSERT INTO M_CASSETTE_LIST (M_CASSETTE_NAME, M_ASSIGNED_DATE, M_CASSETTE_IN_USE, CREATED_DATE)" +
                         " VALUES('" + CassetteName + "', '" + curDT + "', 0, GETDATE())";
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
        public static McObject<bool> saveAddAssignCassette(string machid, string CassetteNameArray)
        {
            dbconn dbc = new dbconn();
            dbc.connDB();
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = dbc.conn;
            //int cassGroupID = 0;
            try
            {
                int sizeCass = CassetteNameArray.Length;

                string[] cassettesName = CassetteNameArray.Split(',');
                foreach (string cassName in cassettesName)
                {
                    cmd.CommandText = "select M_CASSETTE_NAME from M_CASSETTE_LIST where M_CASSETTE_NAME = '" + cassName + "'";
                    SqlDataReader dr = cmd.ExecuteReader();
                    bool proceedInsert = true;

                    if (dr.HasRows)
                    {
                        proceedInsert = false;
                    }
                    dr.Close();

                    if (proceedInsert)
                    {
                        cmd.CommandText = "INSERT INTO M_CASSETTE_LIST (M_CASSETTE_NAME, M_ASSIGNED_date, M_CASSETTE_IN_USE, M_MACH_ID, CREATED_DATE)" +
                                 " VALUES('" + cassName + "', GETDATE() , 0, '" + machid + "', GETDATE())";
                        cmd.ExecuteNonQuery();
                    }
                }
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
        public static McObject<bool> updateCassette(string machid, string CassetteNameArray, string CassetteOldArray)
        {
            dbconn dbc = new dbconn();
            dbc.connDB();
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = dbc.conn;
            //int cassGroupID = 0;
            try
            {
                string[] cassettesName = CassetteNameArray.Split(',');
                string[] cassettesOldName = CassetteOldArray.Split(',');
                foreach (string cassName in cassettesName)
                {
                    bool proceedInsert = false;
                    //string cassetteOldName = "";
                    foreach (string cassOldName in cassettesOldName)
                    {
                        if (cassOldName.Equals(cassName))
                        {
                            proceedInsert = false;
                            break;
                        }
                        proceedInsert = true;
                    }
                    if (proceedInsert)
                    {
                        if (!string.IsNullOrEmpty(cassName))
                        {
                            DateTime curDT = DateTime.Now;
                            cmd.CommandText = "INSERT INTO M_CASSETTE_LIST (M_CASSETTE_NAME, M_ASSIGNED_date, M_CASSETTE_IN_USE, M_MACH_ID, CREATED_DATE)" +
                                     " VALUES('" + cassName + "', '" + curDT + "', 0, '" + machid + "', GETDATE())";
                            cmd.ExecuteNonQuery();
                        }
                    }
                }

                foreach (string cassOldName in cassettesOldName)
                {
                    bool proceedDelete = false;
                    foreach (string cassName in cassettesName)
                    {
                        if (cassName.Equals(cassOldName))
                        {
                            proceedDelete = false;
                            break;
                        }
                        proceedDelete = true;
                    }
                    if (proceedDelete)
                    {
                        cmd.CommandText = "DELETE FROM M_CASSETTE_LIST " +
                                 " WHERE M_CASSETTE_NAME = '" + cassOldName + "' AND M_MACH_ID = '" + machid + "'";
                        cmd.ExecuteNonQuery();
                    }
                }

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
    }
}