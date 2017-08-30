using Maestro.app_monitoring.DataObject;
using Maestro.app_monitoring.DataUtil;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Script.Services;
using System.Web.Services;
using System.Windows.Forms;

namespace Maestro.app_monitoring.content
{
    /// <summary>
    /// Summary description for MachTemplate
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    [System.Web.Script.Services.ScriptService]
    public class MachTemplate : System.Web.Services.WebService
    {

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string getMachineTemplateList()
        {

            string cmdSelectMachineType =
            "SELECT " +
            Intent.DB_COLUMN.M_MACH_TYPE.ToString() + " AS " + Intent.DB_ALIASE.MACHINETYPE.ToString() + ", " +
            Intent.DB_COLUMN.M_MACH_MODEL.ToString() + " AS " + Intent.DB_ALIASE.MACHINEMODEL.ToString() + ", " +
            Intent.DB_COLUMN.M_MACH_IMGPATH.ToString() + " AS " + Intent.DB_ALIASE.MACHINEIMAGE.ToString() + " " +
            "FROM M_MACHINE_TYPE";

            MachineTypeList mtl = new MachineTypeList();
            mtl.LoadDataTable(DataAccessor.SourceDataTable(cmdSelectMachineType));

            return JsonConvert.SerializeObject(mtl);

        }

 


        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string getErrorCodesByMachineType(string mType, string mModel)
        {
            ErrorCodeList ecl = null;
            try
            {
                string cmdSelectErrorCodesByMachineType =
                "SELECT " +
                "MC." + Intent.DB_COLUMN.M_CODE.ToString() + " AS " + Intent.DB_ALIASE.STATUSCODE.ToString() + ", " +
                "MC." + Intent.DB_COLUMN.M_ERRORTYPE.ToString() + " AS " + Intent.DB_ALIASE.ERROR.ToString() + ", " +
                "MC." + Intent.DB_COLUMN.M_ERROR_DESCRIPTION.ToString() + " AS " + Intent.DB_ALIASE.ERRORDESC.ToString() + ", " +
                "MC." + Intent.DB_COLUMN.M_SOP.ToString() + " AS " + Intent.DB_ALIASE.SOP.ToString() + ", " +
                "MC." + Intent.DB_COLUMN.M_CATEGORY_ID.ToString() + " AS " + Intent.DB_ALIASE.CategoryID.ToString() + ", " +
                "MC." + Intent.DB_COLUMN.M_IMAGE_ID.ToString() + " AS " + Intent.DB_ALIASE.ImageID.ToString() + " " + 
                "FROM M_MACHINE_TYPE MT, M_CODES MC WHERE " +
                "MT.M_MACH_TYPE = MC.M_MACH_TYPE AND " +
                //"MT.M_MACH_MODEL = MC.M_MACH_MODEL AND " +
                //"(MC.M_MACH_MODEL = '" + mModel + "' OR " +
                "MC.M_MACH_TYPE = '" + mType + "' AND " +
                "(MC." + Intent.DB_COLUMN.M_ERRORTYPE.ToString() + "='ONLINE' OR " +
                "MC." + Intent.DB_COLUMN.M_ERRORTYPE.ToString() + "='OFFLINE' OR " +
                "MC." + Intent.DB_COLUMN.M_ERRORTYPE.ToString() + "='WARN' OR " +
                "MC." + Intent.DB_COLUMN.M_ERRORTYPE.ToString() + "='ERROR') " +
                "ORDER BY STATUSCODE asc";

                System.Diagnostics.Debug.Write(cmdSelectErrorCodesByMachineType);
                ecl = new ErrorCodeList();
                ecl.LoadDataTable(DataAccessor.SourceDataTable(cmdSelectErrorCodesByMachineType));
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            return JsonConvert.SerializeObject(ecl);

        }
    }
}
