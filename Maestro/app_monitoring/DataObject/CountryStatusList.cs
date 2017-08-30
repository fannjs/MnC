using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
/*
 * d:{
	NEXT_LINK:M_MACH_STATE_to_M_MACH_CITYDISTRICT,
	LIST:
	[
		{	
			COUNTRY:Selangor,
			ERROR:{
				ERROR:1,
				WARN:0,
				OK:3,
				OFFLINE:0
				}
		},
		{
			COUNTRY:Johor,
			ERROR:{
				ERROR:0,
				WARN:0,
				OK:3,
				OFFLINE:0
				}
		},
		{
			COUNTRY:Kuala Lumpur,
				ERROR:{
				ERROR:0,
				WARN:0,
				OK:4,
				OFFLINE:0}
		}
	]
}
 * */
namespace Maestro.app_monitoring.DataObject
{

	public class CountryStatusList
	{
        //Gan disabled this
		//public string NEXT_LINK = Intent.DB_COLUMN.M_MACH_STATE + Intent.DELIMETER + Intent.DB_COLUMN.M_MACH_CITYDISTRICT;

        /*
         * Newly added due to change of database.
         *  - Previously machine branch information is store within M_MACHINE_LIST
         *  - Now use M_BRANCH_CODE to link to M_BRANCH and get the state, district, city, branch name and etc.
         */
        public string NEXT_LINK = Intent.DB_COLUMN.M_STATE + Intent.DELIMETER + Intent.DB_COLUMN.M_DISTRICT; 

		public List<CountryStatus> LIST
		{
			get;
			set;
		}

		public SumReport TRAFFICS;

		public CountryStatusList()
		{

			LIST = new List<CountryStatus>();
			TRAFFICS = new SumReport();

		}

		private void UpdateOne(Dictionary<string, int> newDtWithMAlive, string key)
		{
			if (newDtWithMAlive.ContainsKey(key))
			{
				newDtWithMAlive[key] = newDtWithMAlive[key] + 1;
			}
			else
			{
				newDtWithMAlive.Add(key, 1);
			}
		}

		public bool LoadDataTable(DataTable dt)
		{
			Dictionary<string, int> newDtWithMAlive = new Dictionary<string, int>();

			string state = "";
			string errType = "";
			bool mAlive = false;
			string key = ""; 

			foreach (DataRow row in dt.Rows)
			{
				state = row[Intent.DB_ALIASE.STATE.ToString()].ToString();
				errType = row[Intent.DB_ALIASE.ERROR.ToString()].ToString();
				mAlive = bool.Parse(row[Intent.DB_ALIASE.M_ALIVE.ToString()].ToString());

				key = state + "." + errType; //combine these 2 as a key to aggregate total counts by (state and error)

				//if mAlive is false, consider it as an ERROR regardless if its original type is OK,
				if(!mAlive || errType == Intent.DB_DATA.ERROR.ToString()) 
				{
					key = state + "." + Intent.DB_DATA.ERROR;
				}

				UpdateOne(newDtWithMAlive, key);
			}

			foreach(KeyValuePair<string, int> entry in newDtWithMAlive)
			{
				string s = entry.Key.Split('.')[0];//state
				string e = entry.Key.Split('.')[1];//err type

				AddCountryStatus(s, e, entry.Value.ToString());
			}          
			return true;
		}

		public void AddCountryStatus(string country, string err, string c)
		{
			bool added = false;

			TRAFFICS.UpdateSum(err, int.Parse(c));
			foreach (CountryStatus s in LIST)
			{
				if (s.COUNTRY.ToUpper().Equals(country.ToUpper()))
				{
					s.UpdateStatus(err, c);
					added = true;
					break;
				}
			}

			if (!added)
			{
				CountryStatus s = new CountryStatus(country);
				s.UpdateStatus(err, c);
				LIST.Add(s);
			}

		}
	}
	public class CountryStatus
	{

		public string COUNTRY
		{
			get;
			set;

		}
		public Dictionary<string, string> ERROR
		{
			get;
			set;
		}

		public CountryStatus(string country)
		{
			this.COUNTRY = country;
			ERROR = new Dictionary<string, string>();
			ERROR.Add(Intent.DB_DATA.ERROR.ToString(), "0");
			ERROR.Add(Intent.DB_DATA.WARN.ToString(), "0");
			ERROR.Add(Intent.DB_DATA.ONLINE.ToString(), "0");
			ERROR.Add(Intent.DB_DATA.OFFLINE.ToString(), "0");
		}

		public void UpdateStatus(string err, string c)
		{
			if (ERROR.ContainsKey(err))
			{
				ERROR[err] = c;
			}
		}

	}

	public class SumReport
	{
		public Dictionary<string, int> SUMMARY
		{
			get;
			set;
		}


		public SumReport()
		{
			SUMMARY = new Dictionary<string, int>();
			SUMMARY.Add(Intent.DB_DATA.ERROR.ToString(), 0);
			SUMMARY.Add(Intent.DB_DATA.WARN.ToString(), 0);
			SUMMARY.Add(Intent.DB_DATA.ONLINE.ToString(), 0);
			SUMMARY.Add(Intent.DB_DATA.OFFLINE.ToString(), 0);
		}

		public void UpdateSum(string statType, int val)
		{
			if (SUMMARY.ContainsKey(statType))
			{
				SUMMARY[statType] += val;
			}
		}
	}
}