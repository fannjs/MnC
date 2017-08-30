using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace Maestro.app_monitoring.Models
{
    public interface IRepository<TType>
    {

        TType Add(TType entity);

        bool Update(OID oid, TType entity);

        bool Remove(OID oid);

        TType Get(OID oid);

        IEnumerable<TType> GetAll();
    }

    public class OID
    {
        public Dictionary<string, object> Params
        {
            get;
            set;
        }

        public OID()
        {
            Params = new Dictionary<string, object>();
        }
    }
}