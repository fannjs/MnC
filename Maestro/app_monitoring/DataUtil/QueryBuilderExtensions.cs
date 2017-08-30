using CodeEngine.Framework.QueryBuilder;
using CodeEngine.Framework.QueryBuilder.Enums;
using Maestro.app_monitoring.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Maestro.app_monitoring.DataUtil.Extensions
{
    public static class QueryBuilderExtensions
    {

        public static SelectQueryBuilder MapTableKeys(this SelectQueryBuilder query, OID oid)
        {
            foreach (KeyValuePair<string, object> pair in oid.Params)
	        {
                query.AddWhere(pair.Key, Comparison.Equals, pair.Value, 1);
	        }

	        return query;
        }

        public static UpdateQueryBuilder MapTableKeys(this UpdateQueryBuilder query, OID oid)
        {
            foreach (KeyValuePair<string, object> pair in oid.Params)
            {
                query.AddWhere(pair.Key, Comparison.Equals, pair.Value, 1);
            }

            return query;
        }

        public static DeleteQueryBuilder MapTableKeys(this DeleteQueryBuilder query, OID oid)
        {
            foreach (KeyValuePair<string, object> pair in oid.Params)
            {
                query.AddWhere(pair.Key, Comparison.Equals, pair.Value, 1);
            }

            return query;
        }  
    }
}