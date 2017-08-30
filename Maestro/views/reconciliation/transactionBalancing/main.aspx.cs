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


namespace Maestro.views.reconciliation.transactionBalancing
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
        public class Transaction
        {
            public int TCount { get; set; }
            public int CCount { get; set; }
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
        public static McObject<Transaction> GetTransaction(string BinID, string Date)
        {
            dbconn dbc = new dbconn();
            dbc.connDB();
            SqlCommand cmd = dbc.conn.CreateCommand();

            try
            {
                DateTime ReconDate = DateTime.ParseExact(Date, "dd/MM/yyyy", CultureInfo.InvariantCulture);

                cmd.CommandText = "SELECT * FROM M_CASSETTE_BIN WHERE M_CASSETTE_ID = @BinID AND M_RECONCILE_DATE = @Date";
                //cmd.CommandText = "SELECT * FROM M_CHEQ_TRANS_INFO WHERE M_CASSETTE_ID = @BinID AND M_RECONCILE_DATE = @Date";
                cmd.Parameters.AddWithValue("BinID", BinID);
                cmd.Parameters.AddWithValue("Date", ReconDate.ToString("yyyy/MM/dd"));
                SqlDataReader dr = cmd.ExecuteReader();
                Transaction trans = null;

                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        trans = new Transaction();
                        trans.CCount = dr.GetInt32(dr.GetOrdinal("M_CHEQUE_COUNT"));
                        trans.TCount = dr.GetInt32(dr.GetOrdinal("M_TRANS_COUNT"));
                    }
                }

                return new McObject<Transaction>(true, "Successful.", trans);
            }
            catch (Exception ex)
            {
                return new McObject<Transaction>(false, "Failed. " + ex.Message);
            }
            finally
            {
                dbc.closeConn();
            }
        }

        [WebMethod]
        public static McObject<Boolean> UpdateRemark(string BinID, string Date, string Remark)
        {
            dbconn dbc = new dbconn();
            dbc.connDB();
            SqlCommand cmd = dbc.conn.CreateCommand();

            try
            {
                DateTime ReconDate = DateTime.ParseExact(Date, "dd/MM/yyyy", CultureInfo.InvariantCulture);

                cmd.CommandText = "UPDATE M_CASSETTE_BIN SET M_TB_REMARKS = @Remark WHERE M_CASSETTE_ID = @BinID AND M_RECONCILE_DATE = @Date";
                //cmd.CommandText = "UPDATE M_CHEQ_TRANS_INFO SET M_REMARKS = @Remark WHERE M_CASSETTE_ID = @BinID AND M_RECONCILE_DAT = @Date";
                cmd.Parameters.AddWithValue("Remark", Remark);
                cmd.Parameters.AddWithValue("BinID", BinID);
                cmd.Parameters.AddWithValue("Date", ReconDate.ToString("yyyy/MM/dd"));
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
    }
}