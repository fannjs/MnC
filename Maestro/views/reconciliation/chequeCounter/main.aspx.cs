using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Json;
using Maestro.Classes;
using System.Globalization;

namespace Maestro.views.reconciliation.chequeCounter
{
    public partial class main : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            GetMachineList();
        }

        public class M_MACHINE_LIST
        {
            public string M_MACH_ID { get; set; }
        }
        public class COLLECTION_BIN
        {
            public string Cassette_ID { get; set; }
            public string Cassette_Name { get; set; }
        }
        public class BIN_STATUS
        {
            public int Bin0 { get; set; }
            public int Bin1 { get; set; }
            public int Bin2 { get; set; }
            public int Bin3 { get; set; }
            public int Bin4 { get; set; }
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

                    selectKioskId.Items.Add(LI);
                }
            }

            dr.Close();
            dbc.closeConn();
        }

        [WebMethod]
        public static McObject<List<COLLECTION_BIN>> GetCollectionBin(string MachID)
        {
            dbconn dbc = new dbconn();
            dbc.connDB();
            SqlCommand cmd = dbc.conn.CreateCommand();

            try
            {
                cmd.CommandText = "SELECT M_CASSETTE_ID, M_CASSETTE_NAME FROM M_CASSETTE_LIST WHERE M_MACH_ID = @MachID";
                cmd.Parameters.AddWithValue("MachID", MachID);
                SqlDataReader dr = cmd.ExecuteReader();
                List<COLLECTION_BIN> BinList = new List<COLLECTION_BIN>();

                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        COLLECTION_BIN Bin = new COLLECTION_BIN();
                        Bin.Cassette_ID = dr["M_CASSETTE_ID"].ToString();
                        Bin.Cassette_Name = dr["M_CASSETTE_NAME"].ToString();

                        BinList.Add(Bin);
                    }
                }
                dr.Close();

                return new McObject<List<COLLECTION_BIN>>(true, "Successful.", BinList);
            }
            catch (Exception ex)
            {
                return new McObject<List<COLLECTION_BIN>>(false, "Failed. " + ex.Message);
            }
            finally
            {
                dbc.closeConn();
            }
        }

        [WebMethod]
        public static McObject<List<BIN_STATUS>> GetBinStatus(string BinID, string Date)
        {
            dbconn dbc = new dbconn();
            dbc.connDB();
            SqlCommand cmd = dbc.conn.CreateCommand();

            try
            {
                DateTime ReconDate = DateTime.ParseExact(Date, "dd/MM/yyyy", CultureInfo.InvariantCulture);

                cmd.CommandText = "SELECT * FROM M_CASSETTE_BIN WHERE M_CASSETTE_ID = @BinID AND M_RECONCILE_DATE = @Date";
                cmd.Parameters.AddWithValue("BinID", BinID);
                cmd.Parameters.AddWithValue("Date", ReconDate.ToString("yyyy/MM/dd"));
                SqlDataReader dr = cmd.ExecuteReader();

                List<BIN_STATUS> BinStatusList = new List<BIN_STATUS>();

                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        BIN_STATUS Bin = new BIN_STATUS();
                        Bin.Bin0 = dr.GetInt32(dr.GetOrdinal("M_BIN_0"));
                        Bin.Bin1 = dr.GetInt32(dr.GetOrdinal("M_BIN_1"));
                        Bin.Bin2 = dr.GetInt32(dr.GetOrdinal("M_BIN_2"));
                        Bin.Bin3 = dr.GetInt32(dr.GetOrdinal("M_BIN_3"));
                        Bin.Bin4 = dr.GetInt32(dr.GetOrdinal("M_BIN_4"));

                        BinStatusList.Add(Bin);
                    }
                }

                return new McObject<List<BIN_STATUS>>(true, "Successful.", BinStatusList);
            }
            catch (Exception ex)
            {
                return new McObject<List<BIN_STATUS>>(false, "Failed. " + ex.Message);
            }
            finally 
            {
                dbc.closeConn();
            }
        }
    }
}