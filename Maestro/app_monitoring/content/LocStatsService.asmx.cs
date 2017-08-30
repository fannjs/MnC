using System.Web.Script.Services;
using System.Web.Services;
using Maestro.app_monitoring.DataObject;
using Maestro.app_monitoring.DataUtil;
using Newtonsoft.Json;

namespace Maestro.app_monitoring.content
{
    /// <summary>
    /// Summary description for LocStatsService
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    [System.Web.Script.Services.ScriptService]


    public class LocStatsService : System.Web.Services.WebService
    {
        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string GetCountryStatus(string site, string machType, string state = "")
        {
            //string qSite = "AND ML." + Intent.DB_COLUMN.M_CUSTOMER.ToString() + " = '" + site + "' ";
            //string qMachType = "AND ML." + Intent.DB_COLUMN.M_MACH_TYPE.ToString() + " = '" + machType + "' ";
            //string qState = "AND ML." + Intent.DB_COLUMN.M_MACH_STATE.ToString() + " = '" + state + "' ";

            string qSite = "AND C." + Intent.DB_COLUMN.M_CUST_NAME.ToString() + " = '" + site + "' ";
            string qMachType = "AND ML." + Intent.DB_COLUMN.M_MACH_TYPE.ToString() + " = '" + machType + "' ";
            string qState = "AND B." + Intent.DB_COLUMN.M_STATE.ToString() + " = '" + state + "' ";
            if (site.ToUpper().Equals("ALL SITE"))
            {
                qSite = "";
            }
            if (machType.ToUpper().Equals("ALL KIOSK"))
            {
                qMachType = "";
            }
            if (state.ToUpper().Equals("ALL STATE"))
            {
                qState = "";
            }

            string cmdSelectCountryStatus =
                "SELECT ME.M_MACH_ID AS " + Intent.DB_ALIASE.MACHINEID + "," +
                "M_STATE AS " + Intent.DB_ALIASE.STATE.ToString() + "," +
                "MC.M_ERRORTYPE AS " + Intent.DB_ALIASE.ERROR + "," +
                "ME.M_ALIVE AS " + Intent.DB_ALIASE.M_ALIVE + " " +
                "FROM M_EVENT_DATA ME, M_MACHINE_LIST ML, M_CODES MC, M_BRANCH B, M_CUSTOMER C " +
                "WHERE ME.M_MACH_ID = ML.M_MACH_ID AND MC.M_CODE = ME.M_CODE " +
                "AND ML.M_BRANCH_CODE = B.M_BRANCH_CODE " +
                "AND B.M_CUST_CODE = C.M_CUST_CODE " + 
                qSite + qMachType + qState +
                "ORDER BY M_ALIVE asc, M_ERRORTYPE asc, ME.M_MACH_ID asc";

            CountryStatusList csl = new CountryStatusList();
            csl.LoadDataTable(DataAccessor.SourceDataTable(cmdSelectCountryStatus));

            return JsonConvert.SerializeObject(csl);
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string GetLocationStatus(string linkage, string location, string site, string machType, string errType)
        {
            string nextLink;
            LocationStatusList ssl = new LocationStatusList();
            ssl.LoadDataTable(DataAccessor.SourceDataTable(LocationCommandGenerator(linkage, location, site, machType, errType, out nextLink)));
            ssl.NEXT_LINK = nextLink;

            return JsonConvert.SerializeObject(ssl);
        }



        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string GetMachineDetails(string mID)
        {
            MachineDetails mds = new MachineDetails();

            //string cmdSelectMachineDetails =
            //    "SELECT" +
            //    "  MC.M_ERRORTYPE AS " + Intent.DB_ALIASE.ERROR +
            //    ", MC.M_CODE AS " + Intent.DB_ALIASE.STATUSCODE +
            //    ", MC." + Intent.DB_COLUMN.M_SOP + " AS " + Intent.DB_ALIASE.SOP +
            //    ", MC.M_ERROR_DESCRIPTION AS " + Intent.DB_ALIASE.ERRORDESC +
            //    ", ME.M_DATE AS " + Intent.DB_ALIASE.DATE +
            //    ", ME.M_TIME AS " + Intent.DB_ALIASE.TIME +
            //    ", ME.M_ALIVE AS " + Intent.DB_ALIASE.M_ALIVE + 
            //    ", ML.M_MACH_COUNTRY AS " + Intent.DB_ALIASE.COUNTRY +
            //    ", ML.M_MACH_STATE AS " + Intent.DB_ALIASE.STATE +
            //    ", ML.M_MACH_CITYDISTRICT AS " + Intent.DB_ALIASE.DISTRICT +
            //    ", ML.M_BRANCH_NO AS " + Intent.DB_ALIASE.BRANCH +
            //    ", ML.M_CONTACT_PERSON AS " + Intent.DB_ALIASE.CONTACTPERSON +
            //    ", ML.M_CONTACT_NO AS " + Intent.DB_ALIASE.CONTACTNUM +
            //    ", ML.M_MACH_ID AS " + Intent.DB_ALIASE.MACHINEID +
            //    ", ML.VENDOR_NAME AS " + Intent.DB_ALIASE.VENDOR +
            //    ", ML.M_CUSTOMER AS " + Intent.DB_ALIASE.CUSTOMER +
            //    ", ML.M_MACH_ADDRESS1 AS " + Intent.DB_ALIASE.ADDRESS1 +
            //    ", ML.M_MACH_ADDRESS2 AS " + Intent.DB_ALIASE.ADDRESS2 +
            //    " FROM M_MACHINE_LIST AS ML,M_EVENT_DATA ME, M_CODES MC" +
            //    " WHERE ML.M_MACH_ID = ME.M_MACH_ID" +
            //    " AND ME.M_CODE = MC.M_CODE" +
            //    " AND ME.M_MACH_ID = '" + mID + "'";

            string cmdSelectMachineDetails =
                "SELECT" +
                " MC.M_ERRORTYPE AS " + Intent.DB_ALIASE.ERROR + 
                ", MC.M_CODE AS " + Intent.DB_ALIASE.STATUSCODE +
                ", MC.M_SOP AS " + Intent.DB_ALIASE.SOP +
                ", MC.M_ERROR_DESCRIPTION AS " + Intent.DB_ALIASE.ERRORDESC +
                ", ME.M_DATE " + Intent.DB_ALIASE.DATE +
                ", ME.M_TIME AS " + Intent.DB_ALIASE.TIME +
                ", ME.M_ALIVE AS " + Intent.DB_ALIASE.M_ALIVE + 
                ", C.M_MACH_COUNTRY AS " + Intent.DB_ALIASE.COUNTRY +
                ", B.M_STATE AS " + Intent.DB_ALIASE.STATE +
                ", B.M_DISTRICT AS " + Intent.DB_ALIASE.DISTRICT +
                ", B.M_BRANCH_CODE AS " + Intent.DB_ALIASE.BRANCH +
                ", B.M_CONTACT AS " + Intent.DB_ALIASE.CONTACTPERSON +
                ", B.M_TEL AS " + Intent.DB_ALIASE.CONTACTNUM +
                ", ML.M_MACH_ID AS " + Intent.DB_ALIASE.MACHINEID +
                ", VL.M_VENDOR_NAME AS " + Intent.DB_ALIASE.VENDOR +
                ", C.M_CUST_NAME AS " + Intent.DB_ALIASE.CUSTOMER +
                ", B.M_ADDRESS1 AS " + Intent.DB_ALIASE.ADDRESS1 +
                ", B.M_ADDRESS2 AS " + Intent.DB_ALIASE.ADDRESS2 +
                " FROM M_MACHINE_LIST AS ML,M_EVENT_DATA ME, M_CODES MC, M_BRANCH B, M_CUSTOMER C, M_VENDOR_LIST VL" +
                " WHERE ML.M_MACH_ID = ME.M_MACH_ID" +
                " AND ME.M_CODE = MC.M_CODE" +
                " AND ML.M_BRANCH_CODE = B.M_BRANCH_CODE" +
                " AND B.M_CUST_CODE = C.M_CUST_CODE" +
                " AND ML.M_VENDOR_ID = VL.M_VENDOR_ID" +
                " AND ME.M_MACH_ID = '" + mID + "'";

            mds.LoadDataTable(DataAccessor.SourceDataTable(cmdSelectMachineDetails));


            return JsonConvert.SerializeObject(mds);
        }


        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        //query by 3 criterials: customer site, state and machine type. IGNORE COUNTRY(HARDCODE)
        public string GetUrgencyQueue(string site, string machType, string state)
        {
            //now country know as customer site
            string qState = "AND ML." + Intent.DB_COLUMN.M_MACH_STATE.ToString() + " = '" + state + "' ";
            string qSite = "AND ML." + Intent.DB_COLUMN.M_CUSTOMER.ToString() + " = '" + site + "' ";
            string qMachType = "AND ML." + Intent.DB_COLUMN.M_MACH_TYPE.ToString() + " = '" + machType + "' ";

            if (site.ToUpper().Equals("ALL SITE") || site.Trim() == "")
            {
                qSite = "";
            }
            if (machType.ToUpper().Equals("ALL KIOSK") || machType.Trim() == "")
            {
                qMachType = "";
            }
            if (state.Trim() == "")
            {
                qState = "";
            }

            string cmdSelectUrgency =
            "SELECT ML.M_MACH_ID AS " + Intent.DB_ALIASE.MACHINEID.ToString() + ", " +
            "ML.M_MACH_TYPE AS " + Intent.DB_ALIASE.MACHINETYPE.ToString() + ", " +
            "MC.M_ERRORTYPE AS " + Intent.DB_ALIASE.ERROR.ToString() + ", " +
            "MC.M_ERROR_DESCRIPTION AS " + Intent.DB_ALIASE.ERRORDESC.ToString() + ", " +
            "ME.M_TIME AS " + Intent.DB_ALIASE.TIME.ToString() + " " +
            "FROM M_EVENT_DATA ME, M_MACHINE_LIST ML, M_CODES MC " +
            "WHERE ME.M_MACH_ID = ML.M_MACH_ID AND MC.M_CODE = ME.M_CODE " +
            qState + qSite + qMachType +
            "AND MC.M_ERRORTYPE <> 'OK' " + //SUBJECT TO MODIFY
             "AND MC.M_ERRORTYPE <> 'NETWORK' " + //SUBJECT TO MODIFY
            "ORDER BY MC.M_ERRORTYPE ASC";
            UrgencyQueueList uql = new UrgencyQueueList();

            uql.LoadDataTable(DataAccessor.SourceDataTable(cmdSelectUrgency));

            return JsonConvert.SerializeObject(uql);
        }
        private string LocationCommandGenerator(string linkage, string selectorVal, string site, string machType, string errType, out string nextLink)
        {
            string selector = Intent.SplitWithDelimeter(linkage, Intent.DELIMETER)[0].Trim();
            string selectee = Intent.SplitWithDelimeter(linkage, Intent.DELIMETER)[1].Trim();
            //string errTypeFilter = "AND M_ERRORTYPE = '" + errType + "' "; //Modified for 'Red Alert' features
            string errTypeFilter = "";
            string qSite = "AND ML." + Intent.DB_COLUMN.M_CUSTOMER.ToString() + " = '" + site + "' ";
            string qMachType = "AND ML." + Intent.DB_COLUMN.M_MACH_TYPE.ToString() + " = '" + machType + "' ";

            nextLink = selectee + Intent.DELIMETER;
            //if (selector.Equals(Intent.DB_COLUMN.M_MACH_COUNTRY.ToString()))
            //{
            //    nextLink += Intent.DB_COLUMN.M_MACH_CITYDISTRICT;
            //}
            //else if (selector.Equals(Intent.DB_COLUMN.M_MACH_STATE.ToString()))
            //{
            //    nextLink += Intent.DB_COLUMN.M_BRANCH_NO;
            //}
            //else if (selector.Equals(Intent.DB_COLUMN.M_MACH_CITYDISTRICT.ToString()))
            //{
            //    nextLink += Intent.DB_COLUMN.M_MACH_ID;
            //}

            if (selector.Equals(Intent.DB_COLUMN.M_MACH_COUNTRY.ToString()))
            {
                nextLink += Intent.DB_COLUMN.M_MACH_CITYDISTRICT;
            }
            else if (selector.Equals(Intent.DB_COLUMN.M_STATE.ToString()))
            {
                nextLink += Intent.DB_COLUMN.M_BRANCH_CODE;
            }
            else if (selector.Equals(Intent.DB_COLUMN.M_DISTRICT.ToString()))
            {
                nextLink += Intent.DB_COLUMN.M_MACH_ID;
            }

            //If selecting DISTRINT OR BRANCH, get from M_BRANCH.
            //If selecting MachineID, get from M_MACHINE_LIST
            if (selectee.Equals(Intent.DB_COLUMN.M_DISTRICT.ToString()))
            {
                selectee = "B." + selectee;
            }
            else if (selectee.Equals(Intent.DB_COLUMN.M_BRANCH_CODE.ToString()))
            {
                selectee = "(B." + selectee + " + ' - ' + B.M_BRANCH_NAME)";
            }
            else if (selectee.Equals(Intent.DB_COLUMN.M_MACH_ID.ToString()))
            {
                selectee = "ML." + selectee;
            }


            //if (errType.Equals(""))
            //{
            //    errTypeFilter = "";
            //}
            if(errType.Equals(""))
            {
                errTypeFilter = "";
            }
            else if (errType.ToUpper().Equals("ERROR"))
            {
                errTypeFilter = "AND (M_ERRORTYPE = '" + errType + "' OR M_ALIVE = 'FALSE') ";
            }
            else if (errType.ToUpper().Equals("OK"))
            {
                errTypeFilter = "AND M_ERRORTYPE = '" + errType + "' AND M_ALIVE = 'TRUE' ";
            }
            else
            {
                errTypeFilter = "AND M_ERRORTYPE = '" + errType + "' ";
            }

            if (site.ToUpper().Equals("ALL SITE"))
            {
                qSite = "";
            }
            if (machType.ToUpper().Equals("ALL KIOSK"))
            {
                qMachType = "";
            }

            string cmdSelectStatus =
            "SELECT " + selectee + " AS " +
            Intent.DB_ALIASE.LOCATION.ToString() + ", count(M_ERRORTYPE) AS " +
            Intent.DB_ALIASE.TOTALCOUNT.ToString() + " " +
            "FROM M_EVENT_DATA ME, M_MACHINE_LIST ML, M_CODES MC, M_BRANCH B " +
            "WHERE ME.M_MACH_ID = ML.M_MACH_ID AND MC.M_CODE = ME.M_CODE AND ML.M_BRANCH_CODE = B.M_BRANCH_CODE " +
            "AND M_ERRORTYPE <> 'NETWORK' " +
            "AND B." + selector + "= '" + selectorVal + "' " +
            qSite + qMachType +
            errTypeFilter +
            "GROUP BY " + selectee + " ORDER BY " + selectee + " ASC";

            return cmdSelectStatus;

        }

        //format: select selectee where selector = "asdad";
        private string LocationCommandGenerator(string linkage, string selectorVal, out string nextLink)
        {
            string selector = Intent.SplitWithDelimeter(linkage, Intent.DELIMETER)[0].Trim();
            string selectee = Intent.SplitWithDelimeter(linkage, Intent.DELIMETER)[1].Trim();

            nextLink = selectee + Intent.DELIMETER;
            if (selector.Equals(Intent.DB_COLUMN.M_MACH_COUNTRY.ToString()))
            {
                nextLink += Intent.DB_COLUMN.M_MACH_CITYDISTRICT;
            }
            else if (selector.Equals(Intent.DB_COLUMN.M_MACH_STATE.ToString()))
            {
                nextLink += Intent.DB_COLUMN.M_BRANCH_NO;
            }
            else if (selector.Equals(Intent.DB_COLUMN.M_MACH_CITYDISTRICT.ToString()))
            {
                nextLink += Intent.DB_COLUMN.M_MACH_ID;
            }

            string cmdSelectStatus =
            "SELECT ML." + selectee + " AS " +
            Intent.DB_ALIASE.LOCATION.ToString() + ", count(M_ERRORTYPE) AS " +
            Intent.DB_ALIASE.TOTALCOUNT.ToString() + " " +
            "FROM M_EVENT_DATA ME, M_MACHINE_LIST ML, M_CODES MC " +
            "WHERE ME.M_MACH_ID = ML.M_MACH_ID AND MC.M_CODE = ME.M_CODE " +
            "AND " + selector + "= '" + selectorVal + "' " +
            "GROUP BY ML." + selectee + " ORDER BY ML." + selectee + " ASC";

            return cmdSelectStatus;

        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string GetLiveTraffics(string country, string site)
        {


            string qCountry = "AND M_MACH_STATE = '" + country + "' ";
            string qSite = "AND M_CUSTOMER = '" + site + "' ";


            //the reason i put all site is because the 'all site' is came from server side.
            if (site.ToUpper().Equals("ALL SITE") || site.Equals(""))
            {
                qSite = "";
            }
            if (country.Equals(""))
            {
                qCountry = "";
            }

            string cmdSelectStatus =
                "SELECT " +
                "MC.M_ERRORTYPE AS " + Intent.DB_ALIASE.ERROR.ToString() + ", " +
                "COUNT(MC.M_ERRORTYPE) AS " + Intent.DB_ALIASE.TOTALCOUNT.ToString() + " " +
                "FROM M_EVENT_DATA ME, M_MACHINE_LIST ML, M_CODES MC " +
                "WHERE ME.M_MACH_ID = ML.M_MACH_ID AND MC.M_CODE = ME.M_CODE " +
                qCountry +
                qSite +
                "GROUP BY MC.M_ERRORTYPE " +
                "ORDER BY MC.M_ERRORTYPE ASC";

            LiveTraffic ltraffic = new LiveTraffic();
            ltraffic.LoadDataTable(DataAccessor.SourceDataTable(cmdSelectStatus));
            return JsonConvert.SerializeObject(ltraffic);
        }
    }
}
