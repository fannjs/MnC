using RototypeIntl.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HolidayFileModule.db
{
    public abstract class AbstractDataLayer
    {
        protected IDbConnector dbCon;
        protected string tblName;

        public AbstractDataLayer(string tblName, IDbConnector dbCon)
        {
            this.tblName = tblName;
            this.dbCon = dbCon;
        }
    }

}
