using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace Maestro.app_monitoring.DataObject
{
    public class MachineTypeList
    {
        public List<MachineType> LIST
        {
            get;
            set;
        }

        public MachineTypeList()
        {
            LIST = new List<MachineType>();
        }

        public bool LoadDataTable(DataTable dt)
        {
            foreach (DataRow row in dt.Rows)
            {
                AddMachineType(
                    row[Intent.DB_ALIASE.MACHINETYPE.ToString()].ToString(),
                    row[Intent.DB_ALIASE.MACHINEMODEL.ToString()].ToString(),
                    row[Intent.DB_ALIASE.MACHINEIMAGE.ToString()].ToString()
                    );
            }

            return true;
        }

        public void AddMachineType(string mType, string mModel, string mImage)
        {
            LIST.Add(new MachineType(mType, mModel, mImage));
        }



    }

    public class MachineType
    {
        public String MACHINETYPE
        {
            get;
            set;
        }
        public String MACHINEMODEL
        {
            get;
            set;
        }
        public String MACHINEIMAGE
        {
            get;
            set;
        }

        public MachineType(string mType, string mModel, string mImage)
        {
            this.MACHINETYPE = mType;
            this.MACHINEMODEL = mModel;
            this.MACHINEIMAGE = mImage;
        }


    }
}