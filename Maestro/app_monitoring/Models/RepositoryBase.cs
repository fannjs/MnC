using CodeEngine.Framework.QueryBuilder;
using CodeEngine.Framework.QueryBuilder.Enums;
using Maestro.app_monitoring.DataUtil.Extensions;
using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Reflection;
using System.Data.Common;
using System.Data.SqlClient;
using Maestro.app_monitoring.DataUtil.SqlUtil;

namespace Maestro.app_monitoring.Models
{
    public class RepositoryBase<TType> : IRepository<TType>
    {
        public string tblName = typeof(TType).Name;
  

        public IEnumerable<TType> GetAll()
        {
            using (IDbConnection connection = SqlMapperUtil.GetOpenConnection())
            {
                SelectQueryBuilder query = new SelectQueryBuilder();
                query.SelectFromTable(tblName);
                return connection.Query<TType>(query.BuildQuery());
            }
        }

        public TType Get(OID oid)
        {
            using (IDbConnection connection = SqlMapperUtil.GetOpenConnection())
            {
                SelectQueryBuilder query = new SelectQueryBuilder();
                query.SelectFromTable(tblName);
                query.MapTableKeys(oid);
                
                return connection.Query<TType>(query.BuildQuery()).FirstOrDefault();
            }

        }

        public TType Add(TType entity)
        {
           
            using (SqlConnection connection = SqlMapperUtil.GetOpenConnection())
            using (SqlCommand sqlCommand = new SqlCommand("", connection))
            {
                int affectedRow = 0;
                try
                {
                    InsertCommandBuilder query = new InsertCommandBuilder(tblName, sqlCommand);
                    Type myType = entity.GetType();
                    IList<PropertyInfo> props = new List<PropertyInfo>(myType.GetProperties());

                    foreach (PropertyInfo prop in props)
                    {
                        object propValue = prop.GetValue(entity, null);
                        if (propValue != null)
                        {
                            query.SetField(prop.Name, propValue);
                        }
                    }
                    affectedRow = query.BuildQuery().ExecuteNonQuery();
                }
                catch (Exception)
                {

                }
                return (affectedRow > 0)?entity : default(TType);
            }
        }

        public bool Remove(OID oid)
        {
            using (IDbConnection connection = SqlMapperUtil.GetOpenConnection())
            {
                DeleteQueryBuilder query = new DeleteQueryBuilder();
                query.Table = tblName;
                query.MapTableKeys(oid);

                int affectedRow = connection.Execute(query.BuildQuery());

                return (affectedRow > 0);
            }
        }

        public bool Update(OID oid, TType entity)
        {
            using (IDbConnection connection = SqlMapperUtil.GetOpenConnection())
            {
                UpdateQueryBuilder query = new UpdateQueryBuilder();
                query.Table = tblName;
                Type myType = entity.GetType();
                IList<PropertyInfo> props = new List<PropertyInfo>(myType.GetProperties());

                foreach (PropertyInfo prop in props)
                {
                    object propValue = prop.GetValue(entity, null);

                    if(propValue != null)
                        query.SetField(prop.Name, propValue);
                }


                query.MapTableKeys(oid);
                int affectedRow = 0;

                try
                {
                    affectedRow = connection.Execute(query.BuildQuery());
                }
                catch (Exception)
                {
                    
                }
                return (affectedRow > 0);


            }
        }


    }
}