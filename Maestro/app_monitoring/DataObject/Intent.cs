using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Maestro.app_monitoring.DataObject
{
    public class Intent
    {

        public static string DELIMETER = "_to_";

        public static string[] SplitWithDelimeter(string input, string delim)
        {
            int startIdx = input.IndexOf(delim);
            int delimLen = delim.Length;
            string[] output = new string[2];
            output[0] = input.Substring(0, startIdx);
            output[1] = input.Substring(startIdx + delim.Length, input.Length - (startIdx + delim.Length));

            return output;
        }

        public enum DB_ALIASE
        {
            CategoryID,
            ImageID,
            M_ALIVE,
            ERROR,
            TOTALCOUNT,
            LOCATION,
            MACHINEID,
            MACHINETYPE,
            MACHINEMODEL,
            MACHINEIMAGE,
            TIME,
            STATUSCODE,
            ERRORDESC,
            DATE,
            SOP,

            CONTACTPERSON,
            CONTACTNUM,
            VENDOR,
            ADDRESS1,
            ADDRESS2,
            CUSTOMER,

            COUNTRY,
            STATE,
            DISTRICT,
            BRANCH,



        }

        public enum DB_COLUMN
        {
            M_CATEGORY_ID,
            M_IMAGE_ID,
            M_CUSTOMER,
            M_MACH_STATE,
            M_STATE,
            M_MACH_COUNTRY,
            M_MACH_CITYDISTRICT,
            M_DISTRICT,
            M_CITY,
            M_BRANCH_NO,
            M_BRANCH_CODE,
            M_ERROR_DESCRIPTION,
            M_MACH_ID,
            M_TIME,
            M_CODE,
            M_ERRORTYPE,
            M_DATE,
            M_MACH_TYPE,
            M_MACH_MODEL,
            M_MACH_IMGPATH,
            M_SOP,
            LOCATEID,
            LOCATIONNAME,
            IPSTART,
            IPEND,
            COMMTYPE,
            USERNAME,
            PASSWORD,
            ROLE,
            UPASSWORD,
            SHAREDNAME,
            PORTNUMBER,
            ENABLE,

            RULEID,
            RULETYPE,
            EXPRESSION,
            RULENAME,
            VALUE,

            M_CUST_CODE,
            M_CUST_NAME,
            M_MACH_ERRORTYPE,

            M_VENDOR_NAME,
            M_VENDOR_PERSON1,
            M_VENDOR_TELNO1,
            M_VENDOR_EMAIL1,
            M_VENDOR_EMAIL2,
            M_VENDOR_ADDRESS1,
            M_VENDOR_ADDRESS2,
            M_VENDOR_COUNTRY,
        }

        //note: there's a priority here
        public enum DB_DATA
        {
            ERROR,
            WARN,
            OFFLINE,
            ONLINE,
            NETWORK,
        }

    }
}