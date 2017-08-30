using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Windows.Forms;

namespace Maestro.app_monitoring.DataUtil
{
    public class DataAccessor
    {
        public static DataTable SourceDataTable(String cmdSelect)
        {
            DataTable dt = null;
            try
            {
                dbconn dbObj = new dbconn();
                dbObj.connDB();
                SqlDataAdapter adpt = new SqlDataAdapter(cmdSelect, dbObj.conn);
                dt = new DataTable();
                adpt.Fill(dt);
                dbObj.closeConn();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());

            }
            return dt;
        }
    }
}