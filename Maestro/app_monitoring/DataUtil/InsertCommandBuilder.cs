using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace Maestro.app_monitoring.DataUtil.SqlUtil
{
    public class InsertCommandBuilder
    {
        public string tableName;
        public Dictionary<string, object> colPair;
        public SqlCommand insertSql;


        public InsertCommandBuilder(string tableName, SqlCommand insertSql)
        {
            this.tableName = tableName;
            this.colPair = new Dictionary<string, object>();
            this.insertSql = insertSql;

        }

        public void SetField(string colName, object value)
        {
            //sequence is guaranteed
            colPair.Add(colName, value);
        }

        public SqlCommand BuildQuery()
        {
            string q = "INSERT INTO " + tableName + " (";
            int count = colPair.Count;

            foreach (KeyValuePair<string, object> pair in colPair)
            {
                q += pair.Key + ((--count > 0) ? ", " : ") VALUES (");
            }


            count = colPair.Count;

            foreach (KeyValuePair<string, object> pair in colPair)
            {
                q += "@" + pair.Key + ((--count > 0) ? ", " : ")");
            }
            this.insertSql.CommandText = q;


            foreach (KeyValuePair<string, object> pair in colPair)
            {
                this.insertSql.Parameters.AddWithValue("@" + pair.Key, pair.Value);
            }

            return insertSql;
        }
    }

}
