using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Script.Services;
using System.Web.Services;
using Maestro.Parser;
using Maestro.Classes.Prefix;
using System.Json;

namespace Maestro.views.kioskMaintenance.rulesAssignment
{
    public partial class main : System.Web.UI.Page
    {
        private static string taskName = "";

        protected void Page_Load(object sender, EventArgs e)
        {
            taskName = Request.Params["task"];
        }

        public class Rule
        {
            public string RuleID { get; set; }
            public string RuleName { get; set; }
            public string ResultType { get; set; }
            public string RuleLabel { get; set; }
            public string RuleRemark { get; set; }
            public string IsCondition { get; set; }
            public string IntegerMin { get; set; }
            public string IntegerMax { get; set; }

            public string RuleExpression { get; set; }
            public string UserViewExpr { get; set; }

            public string SpecificMach { get; set; }
            public string SpecificExpr { get; set; }
            public string SpecificUVExpr { get; set; }

            public string RuleSeq { get; set; }
        }

        public class Variable
        {
            public string VariableName { get; set; }
            public string VariableType { get; set; }
        }

        public class Operator
        {
            public string OperatorType { get; set; }
            public string OperatorDesc { get; set; }
        }

        public class Machine
        {
            public string MachineID { get; set; }
            public string BranchCode { get; set; }
            public string BranchName { get; set; }
            public string Address1 { get; set; }
            public string Address2 { get; set; }
        }

        public static string GetTaskID(string task_name)
        {
            string task_id = "";

            dbconn dbc = new dbconn();
            dbc.connDB();
            SqlCommand cmd = dbc.conn.CreateCommand();
            cmd.CommandText = "SELECT TASK_ID FROM M_TASK WHERE TASK_NAME = '" + task_name + "'";
            SqlDataReader dr = cmd.ExecuteReader();
            if (dr.HasRows)
            {
                while (dr.Read())
                {
                    task_id = dr["TASK_ID"].ToString();
                }
            }
            dbc.closeConn();

            return task_id;
        }

        [WebMethod, System.Web.Script.Services.ScriptMethod]
        public static Rule[] getRulesList()
        {
            dbconn dbc = new dbconn();
            dbc.connDB();
            SqlCommand cmd = dbc.conn.CreateCommand();

            cmd.CommandText = "SELECT BR_RULE_LIST.RULE_ID, BR_RULE_EXPRESSION.USER_VIEW_EXPR, BR_RULE_LIST.RULE_NAME, M_MACHINE_RULES.M_MACH_ID, M_MACHINE_RULES.USER_VIEW_EXPRESSION, BR_RULE_LIST.RULE_SEQ "
                                +"FROM BR_RULE_EXPRESSION "
                                +"INNER JOIN BR_RULE_LIST ON BR_RULE_EXPRESSION.RULE_ID = BR_RULE_LIST.RULE_ID "
                                +"LEFT OUTER JOIN M_MACHINE_RULES ON BR_RULE_LIST.RULE_ID = M_MACHINE_RULES.RULE_ID "
                                + "ORDER BY BR_RULE_LIST.RULE_SEQ asc, USER_VIEW_EXPRESSION";
            
            SqlDataReader dr = cmd.ExecuteReader();
            List<Rule> rulesList = new List<Rule>();

            if (dr.HasRows)
            {
                while (dr.Read())
                {
                    Rule rule = new Rule();
                    rule.RuleID = dr["RULE_ID"].ToString().Trim();
                    rule.RuleName = dr["RULE_NAME"].ToString().Trim();
                    rule.UserViewExpr = dr["USER_VIEW_EXPR"].ToString().Trim();
                    rule.SpecificMach = dr["M_MACH_ID"].ToString().Trim();
                    rule.SpecificUVExpr = dr["USER_VIEW_EXPRESSION"].ToString().Trim();
                    rule.RuleSeq = dr["RULE_SEQ"].ToString().Trim();

                    rulesList.Add(rule);
                }
            }
            dbc.closeConn();

            return rulesList.ToArray();
        }

        [WebMethod, System.Web.Script.Services.ScriptMethod]
        public static StatusMessage parseStatement(string stmt)
        {
            StatusMessage sm = new StatusMessage();
            PEGParser.ParseStatement(stmt, sm);

            return sm;
        }


        [WebMethod]
        public static StatusMessage parseScript(string ruleID, string userRepresentative, string expression, string script)
        {
            StatusMessage sm = new StatusMessage();
            bool result = PEGParser.ParseScript(script, sm);

            if (result)
            {
                insertGlobalDefaultValue(ruleID, userRepresentative, expression, PEGParser.GetAstString());
            }
            else
            {
                return sm;
            }

            return sm;
        }

        [WebMethod, System.Web.Script.Services.ScriptMethod]
        public static StatusMessage parseScript_Specific(string[] machineList, string ruleID, string userRepresentative, string expression, string script)
        {
            StatusMessage sm = new StatusMessage();
            bool result = PEGParser.ParseScript(script, sm);

            if (result)
            {
                insertSpecificRule(machineList, ruleID, userRepresentative, expression, PEGParser.GetAstString());
            }
            else
            {
                return sm;
            }

            return sm;
        }


        [WebMethod, System.Web.Script.Services.ScriptMethod]
        public static Rule[] getRuleDetail(string ruleID)
        {
            dbconn dbc = new dbconn();
            dbc.connDB();
            SqlCommand cmd = dbc.conn.CreateCommand();
            cmd.CommandText = "SELECT BR_RULE_LIST.RULE_ID, BR_RULE_LIST.RESULT_TYPE, BR_RULE_LIST.RULE_LABEL, BR_RULE_LIST.RULE_REMARK, BR_RULE_LIST.IS_CONDITION, BR_RULE_EXPRESSION.USER_VIEW_EXPR, BR_RULE_EXPRESSION.RULE_EXPR, BR_RULE_LIST.INTEGER_MIN, BR_RULE_LIST.INTEGER_MAX  "
                                + "FROM BR_RULE_LIST INNER JOIN BR_RULE_EXPRESSION ON BR_RULE_LIST.RULE_ID = BR_RULE_EXPRESSION.RULE_ID "
                                + "WHERE BR_RULE_LIST.RULE_ID = " + ruleID;

            SqlDataReader dr = cmd.ExecuteReader();
            List<Rule> rulesList = new List<Rule>();

            if (dr.HasRows)
            {
                while (dr.Read())
                {
                    Rule rule = new Rule();
                    rule.RuleID = dr["RULE_ID"].ToString().Trim();
                    rule.ResultType = dr["RESULT_TYPE"].ToString().Trim();
                    rule.RuleLabel = dr["RULE_LABEL"].ToString().Trim();
                    rule.RuleRemark = dr["RULE_REMARK"].ToString().Trim();
                    rule.IsCondition = dr["IS_CONDITION"].ToString().Trim();
                    rule.IntegerMin = dr["INTEGER_MIN"].ToString().Trim();
                    rule.IntegerMax = dr["INTEGER_MAX"].ToString().Trim();
                    rule.RuleExpression = dr["RULE_EXPR"].ToString().Trim();
                    rule.UserViewExpr = dr["USER_VIEW_EXPR"].ToString().Trim();

                    rulesList.Add(rule);
                }
            }
            dbc.closeConn();

            return rulesList.ToArray();
        }

        [WebMethod, System.Web.Script.Services.ScriptMethod]
        public static Variable[] getVariableList()
        {
            dbconn dbc = new dbconn();
            dbc.connDB();
            SqlCommand cmd = dbc.conn.CreateCommand();
            cmd.CommandText = "SELECT * FROM BR_VARIABLE_LIST";

            SqlDataReader dr = cmd.ExecuteReader();
            List<Variable> variableList = new List<Variable>();

            if (dr.HasRows)
            {
                while (dr.Read())
                {
                    Variable variable = new Variable();
                    variable.VariableName = dr["VARIABLE_NAME"].ToString().Trim();
                    variable.VariableType = dr["VARIABLE_TYPE"].ToString().Trim();

                    variableList.Add(variable); 
                }
            }
            dbc.closeConn();

            return variableList.ToArray();
        }

        [WebMethod, System.Web.Script.Services.ScriptMethod]
        public static Operator[] getOperatorList()
        {
            dbconn dbc = new dbconn();
            dbc.connDB();
            SqlCommand cmd = dbc.conn.CreateCommand();
            cmd.CommandText = "SELECT * FROM BR_OPERATOR_LIST ORDER BY OPERATOR_DESC ASC";

            SqlDataReader dr = cmd.ExecuteReader();
            List<Operator> operatorList = new List<Operator>();

            if (dr.HasRows)
            {
                while (dr.Read())
                {
                    Operator oprtr = new Operator();
                    oprtr.OperatorType = dr["OPERATOR_TYPE"].ToString().Trim();
                    oprtr.OperatorDesc = dr["OPERATOR_DESC"].ToString().Trim();

                    operatorList.Add(oprtr);
                }
            }
            dbc.closeConn();

            return operatorList.ToArray();
        }

        [WebMethod, System.Web.Script.Services.ScriptMethod]
        public static Boolean insertGlobalDefaultValue(string ruleID, string userRepresentative, string ruleExpression, string ruleExpressionTree)
        {
            try
            {
                if (HttpContext.Current.Session["userId"] == null)
                {
                    throw new Exception("Session timeout. User not found!");
                }
                if (HttpContext.Current.Session["userName"] == null)
                {
                    throw new Exception("Session timeout. User not found!");
                }

                string userID = HttpContext.Current.Session["userId"].ToString();
                string userName = HttpContext.Current.Session["userName"].ToString();

                int iRuleID = int.Parse(ruleID.ToString());

                string taskID = GetTaskID(taskName);
                string action = MCAction.MODIFY;
                string sql = "UPDATE BR_RULE_EXPRESSION SET USER_VIEW_EXPR = '" + userRepresentative + "', RULE_EXPR = '" + ruleExpression + "', RULE_EXPR_TREE = '" + ruleExpressionTree + "' WHERE RULE_ID = " + iRuleID;
                
                int status = MCStatus.PENDING;

                dbconn dbc = new dbconn();
                dbc.connDB();
                SqlCommand cmd = dbc.conn.CreateCommand();

                cmd.CommandText = "SELECT BRRL.RULE_NAME, BRRE.USER_VIEW_EXPR FROM BR_RULE_LIST BRRL, BR_RULE_EXPRESSION BRRE "
                                 + "WHERE BRRL.RULE_ID = BRRE.RULE_ID AND BRRL.RULE_ID = " + iRuleID;

                SqlDataReader dr = cmd.ExecuteReader();
                JsonObject OLD = new JsonObject();
                JsonObject NEW = new JsonObject();

                if (dr.HasRows)
                {
                    if (dr.Read())
                    {
                        OLD.Add("Rule", dr["RULE_NAME"].ToString());
                        OLD.Add("Value", (string.IsNullOrEmpty(dr["USER_VIEW_EXPR"].ToString())? "-" : dr["USER_VIEW_EXPR"].ToString()));

                        NEW.Add("Rule", dr["RULE_NAME"].ToString());
                        NEW.Add("Value", userRepresentative);
                    }
                }
                else
                {
                    //IF NO ROW = error
                    return false;
                }

                dr.Close(); 

                string olddata = OLD.ToString();
                string newdata = NEW.ToString();

                cmd.CommandText = "INSERT INTO M_MAKER_CHECKER_LIST(TASK_ID,TASK_NAME,CREATED_DATE,OLD_VALUE,NEW_VALUE,ACTION_TYPE,APPROVE_SQL,MAKER_ID,MAKER_USER_NAME,APPROVAL_STATUS) "
                                    + "VALUES(" + taskID + ", '" + taskName + "', GETDATE(), '" + olddata + "', '" + newdata + "', '" + action + "', "
                                    + "@sql, " + userID + ", '" + userName + "', " + status + ")";
                cmd.Parameters.AddWithValue("sql", sql);
                //cmd.CommandText = "UPDATE BR_RULE_EXPRESSION SET USER_VIEW_EXPR = '" + userRepresentative + "', RULE_EXPR = '" + ruleExpression + "', RULE_EXPR_TREE = '" + ruleExpressionTree + "' WHERE RULE_ID = '" + iRuleID + "'";
                cmd.ExecuteNonQuery();
                dbc.closeConn();

                return true;
            }
            catch(Exception ex)
            {
                //log
                return false;
            }
        }
        [WebMethod, System.Web.Script.Services.ScriptMethod]
        public static Rule[] getGlobalDefault(string ruleID)
        {
            dbconn dbc = new dbconn();
            dbc.connDB();
            SqlCommand cmd = dbc.conn.CreateCommand();
            cmd.CommandText = "SELECT BR_RULE_LIST.RULE_ID, BR_RULE_LIST.RULE_NAME, BR_RULE_EXPRESSION.USER_VIEW_EXPR "
                                + "FROM BR_RULE_LIST, BR_RULE_EXPRESSION "
                                + "WHERE BR_RULE_LIST.RULE_ID = BR_RULE_EXPRESSION.RULE_ID "
                                + "AND BR_RULE_LIST.RULE_ID = '" + ruleID + "'";

            SqlDataReader dr = cmd.ExecuteReader();
            List<Rule> rulesList = new List<Rule>();

            if (dr.HasRows)
            {
                while (dr.Read())
                {
                    Rule rule = new Rule();
                    rule.RuleID = dr["RULE_ID"].ToString().Trim();
                    rule.RuleName = dr["RULE_NAME"].ToString().Trim();
                    rule.UserViewExpr = dr["USER_VIEW_EXPR"].ToString().Trim();

                    rulesList.Add(rule);
                }
            }
            dbc.closeConn();

            return rulesList.ToArray();
        }

        [WebMethod]
        public static Machine[] getMachineList(string ruleID)
        {
            dbconn dbc = new dbconn();
            dbc.connDB();
            SqlCommand cmd = dbc.conn.CreateCommand();
            cmd.CommandText = "SELECT M_MACH_ID, ML.M_BRANCH_CODE, M_BRANCH_NAME, M_ADDRESS1, M_ADDRESS2 " +
                                "FROM M_MACHINE_LIST ML, M_BRANCH B " + 
                                "WHERE ML.M_BRANCH_CODE = B.M_BRANCH_CODE " +
                                "ORDER BY M_MACH_ID asc";

            SqlDataReader dr = cmd.ExecuteReader();
            List<Machine> machineList = new List<Machine>();

            if (dr.HasRows)
            {
                while (dr.Read())
                {
                    Machine machine = new Machine();
                    machine.MachineID = dr["M_MACH_ID"].ToString().Trim();
                    machine.BranchCode = dr["M_BRANCH_CODE"].ToString().Trim();
                    machine.BranchName = dr["M_BRANCH_NAME"].ToString().Trim();
                    machine.Address1 = dr["M_ADDRESS1"].ToString().Trim();
                    machine.Address2 = dr["M_ADDRESS2"].ToString().Trim();

                    machineList.Add(machine);
                }
            }
            dbc.closeConn();

            return machineList.ToArray();
        }

        [WebMethod, System.Web.Script.Services.ScriptMethod]
        public static Machine[] searchMachine(string ruleID, string searchPattern)
        {
            dbconn dbc = new dbconn();
            dbc.connDB();
            SqlCommand cmd = dbc.conn.CreateCommand();
            cmd.CommandText = "SELECT M_MACHINE_LIST.M_MACH_ID, M_MACHINE_LIST.M_BRANCH_NO, M_BRANCH.M_BRANCH_NAME, M_BRANCH.M_ADDRESS1, M_BRANCH.M_ADDRESS2 "
                                + "FROM M_MACHINE_LIST "
                                + "LEFT OUTER JOIN M_BRANCH "
                                + "ON M_MACHINE_LIST.M_BRANCH_NO = M_BRANCH.M_BRANCH_CODE "
                                + "WHERE M_MACH_ID NOT IN(SELECT M_MACH_ID FROM M_MACHINE_RULES WHERE RULE_ID = " + ruleID + ") "
                                + "AND M_MACH_ID LIKE '" + searchPattern + "%' "
                                + "ORDER BY M_MACHINE_LIST.M_MACH_ID ASC";

            SqlDataReader dr = cmd.ExecuteReader();
            List<Machine> machineList = new List<Machine>();

            if (dr.HasRows)
            {
                while (dr.Read())
                {
                    Machine machine = new Machine();
                    machine.MachineID = dr["M_MACH_ID"].ToString().Trim();
                    machine.BranchCode = dr["M_BRANCH_NO"].ToString().Trim();
                    machine.BranchName = dr["M_BRANCH_NAME"].ToString().Trim();
                    machine.Address1 = dr["M_ADDRESS1"].ToString().Trim();
                    machine.Address2 = dr["M_ADDRESS2"].ToString().Trim();

                    machineList.Add(machine);
                }
            }
            dbc.closeConn();

            return machineList.ToArray();
        }

        [WebMethod, System.Web.Script.Services.ScriptMethod]
        public static Boolean validateSpecificValue(string ruleID, string ruleExpression)
        {
            try
            {
                int iRuleID = int.Parse(ruleID.ToString());

                dbconn dbc = new dbconn();
                dbc.connDB();
                SqlCommand cmd = dbc.conn.CreateCommand();

                cmd.CommandText = "SELECT RULE_EXPR FROM BR_RULE_EXPRESSION WHERE RULE_ID = " + iRuleID;
                SqlDataReader dr = cmd.ExecuteReader();

                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        if (dr["RULE_EXPR"].ToString() == ruleExpression)
                        {
                            return false;
                        }
                    }
                }

                dr.Close();
                dbc.closeConn();

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        [WebMethod, System.Web.Script.Services.ScriptMethod]
        public static Boolean insertSpecificRule(string[] machineList, string ruleID, string userRepresentative, string ruleExpression, string ruleExpressionTree)
        {
            try
            {
                if (HttpContext.Current.Session["userId"] == null)
                {
                    throw new Exception("Session timeout. User not found!");
                }
                if (HttpContext.Current.Session["userName"] == null)
                {
                    throw new Exception("Session timeout. User not found!");
                }

                string userID = HttpContext.Current.Session["userId"].ToString();
                string userName = HttpContext.Current.Session["userName"].ToString();

                int iRuleID = int.Parse(ruleID.ToString());

                string taskID = GetTaskID(taskName);
                string action = MCAction.ADD;
                int status = MCStatus.PENDING;
                string sql = "";
                string machines = "";

                for (int i = 0; i < machineList.Length; i++)
                {
                    sql += "INSERT INTO M_MACHINE_RULES (M_MACH_ID, RULE_ID, USER_VIEW_EXPRESSION, EXPRESSION, EXPRESSION_TREE, CREATED_DATE) "
                        +"VALUES " + "('" + machineList[i].Trim() + "'," + iRuleID + ",'" + userRepresentative + "','" + ruleExpression.Trim() + "','" + ruleExpressionTree.Trim() + "', GETDATE())";

                    machines += machineList[i].Trim() + " ";
                }

                machines = machines.Trim().Replace(" ", ", ");

                dbconn dbc = new dbconn();
                dbc.connDB();
                SqlCommand cmd = dbc.conn.CreateCommand();
                cmd.CommandText = "SELECT RULE_NAME FROM BR_RULE_LIST "
                                 + "WHERE RULE_ID = " + iRuleID;

                SqlDataReader dr = cmd.ExecuteReader();
                JsonObject NEW = new JsonObject();

                if (dr.HasRows)
                {
                    if (dr.Read())
                    {
                        NEW.Add("Rule", dr["RULE_NAME"].ToString());
                        NEW.Add("Value", userRepresentative);
                        NEW.Add("Machines", machines);
                    }
                }
                else
                {
                    return false;
                }

                dr.Close();

                string newdata = NEW.ToString();

                cmd.CommandText = "INSERT INTO M_MAKER_CHECKER_LIST(TASK_ID,TASK_NAME,CREATED_DATE,NEW_VALUE,ACTION_TYPE,APPROVE_SQL,MAKER_ID,MAKER_USER_NAME,APPROVAL_STATUS) "
                                   + "VALUES(" + taskID + ", '" + taskName + "', GETDATE(),'" + newdata + "', '" + action + "', "
                                   + "@sql, " + userID + ", '" + userName + "', " + status + ")";
                cmd.Parameters.AddWithValue("sql", sql);
                cmd.ExecuteNonQuery();

                dbc.closeConn();

                return true;
            }
            catch (Exception ex)
            {
                //log
                return false;
            }
        }

        [WebMethod, System.Web.Script.Services.ScriptMethod]
        public static Boolean deleteSpecificRule(string[] machineList, string ruleID)
        {
            try
            {
                if (HttpContext.Current.Session["userId"] == null)
                {
                    throw new Exception("Session timeout. User not found!");
                }
                if (HttpContext.Current.Session["userName"] == null)
                {
                    throw new Exception("Session timeout. User not found!");
                }

                string userID = HttpContext.Current.Session["userId"].ToString();
                string userName = HttpContext.Current.Session["userName"].ToString();

                int iRuleID = int.Parse(ruleID.ToString());

                string taskID = GetTaskID(taskName);
                string action = MCAction.DELETE;
                int status = MCStatus.PENDING;
                string sql = "";
                string machines = "";

                for (int i = 0; i < machineList.Length; i++)
                {
                    sql += "DELETE FROM M_MACHINE_RULES WHERE M_MACH_ID = '" + machineList[i].Trim() + "' AND RULE_ID = '" + iRuleID + "'";

                    machines += machineList[i].Trim() + " ";
                }

                machines = machines.Trim().Replace(" ", ", ");

                dbconn dbc = new dbconn();
                dbc.connDB();
                SqlCommand cmd = dbc.conn.CreateCommand();
                cmd.CommandText = "SELECT BRL.RULE_NAME, MMR.USER_VIEW_EXPRESSION FROM BR_RULE_LIST BRL, M_MACHINE_RULES MMR "
                                 + "WHERE BRL.RULE_ID = MMR.RULE_ID AND MMR.RULE_ID = " + iRuleID + " AND MMR.M_MACH_ID = '" + machineList[0] + "' ";

                SqlDataReader dr = cmd.ExecuteReader();
                JsonObject OLD = new JsonObject();

                if (dr.HasRows)
                {
                    if (dr.Read())
                    {
                        OLD.Add("Rule", dr["RULE_NAME"].ToString());
                        OLD.Add("Value", dr["USER_VIEW_EXPRESSION"].ToString());
                        OLD.Add("Machines", machines);
                    }
                }
                else
                {
                    return false;
                }

                dr.Close();

                string olddata = OLD.ToString();

                cmd.CommandText = "INSERT INTO M_MAKER_CHECKER_LIST(TASK_ID,TASK_NAME,CREATED_DATE,OLD_VALUE,ACTION_TYPE,APPROVE_SQL,MAKER_ID,MAKER_USER_NAME,APPROVAL_STATUS) "
                                   + "VALUES(" + taskID + ", '" + taskName + "', GETDATE(),'" + olddata + "', '" + action + "', "
                                   + "@sql, " + userID + ", '" + userName + "', " + status + ")";
                cmd.Parameters.AddWithValue("sql", sql);
                cmd.ExecuteNonQuery();

                dbc.closeConn();

                return true;
            }
            catch (Exception ex)
            {
                //log
                return false;
            }
        }
    }
}