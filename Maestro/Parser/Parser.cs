using RototypeIntl.ExpressionEvaluator.PEG;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Linq;

namespace Maestro.Parser
{
    public class PEGParser
    {

        public static Node ast = null;

        public static bool ParseStatement(string s, StatusMessage sm)
        {
            return EEParse(s, JavaScriptGrammar.Statement, sm);
        }

        public static bool ParseScript(string s, StatusMessage sm)
        {
            return EEParse(s, JavaScriptGrammar.AnonFunc, sm);
        }

        public static string GetAstString()
        {
            return ast.ToXml.ToString(SaveOptions.DisableFormatting);
        }

        public static bool EEParse(string s, Rule r, StatusMessage sm)
        {

            bool success = false;
            StatusMessage.STATE status = StatusMessage.STATE.FAIL;
            string msg = "";

            if (sm == null) { sm = new StatusMessage(); }

            try
            {
                var nodes = r.Parse(s);

                if (nodes == null || nodes.Count != 1)
                {
                    status = StatusMessage.STATE.FAIL;
                    msg = "Parsing failed!";
                    ast = nodes[0];
                }
                else if (nodes[0].Text != s)
                {
                    status = StatusMessage.STATE.PARTIAL_SUCCESS;
                    msg = "Parsing partially succeeded!";
                    ast = nodes[0];
                }
                else
                {
                    status = StatusMessage.STATE.SUCCESS;
                    msg = "Parsing suceeded!: " + nodes[0].Text;
                    ast = nodes[0];
                    success = true;
                }
            }
            catch (Exception e)
            {
                status = StatusMessage.STATE.FAIL;
                msg = "Parsing failed with exception" + e.Message;
            }
            finally
            {

                sm.MESSAGE = msg;
                sm.STATUS = status;
            }


            return success;
        }
        public static void TestCompile(string s, params object[] args)
        {
            Console.WriteLine("Testing {0} with args ({1})", s, String.Join(", ", args));
            try
            {
                var f = JavaScriptExpressionCompiler.CompileLambda(s);
                var r = f.DynamicInvoke(args);
                Console.WriteLine("Result is {0}", r);
            }
            catch (Exception e)
            {
                Console.WriteLine("error occured " + e.Message);
            }
        }

        public static void Evaluate(Node node, params object[] args)
        {
            //Console.WriteLine("Testing {0} with args ({1})", s, String.Join(", ", args));
            try
            {
                var f = JavaScriptExpressionCompiler.CompileLambda(node);
                var r = f.DynamicInvoke(args);
                Console.WriteLine("Result is {0}", r);
                Console.WriteLine("Type of Result is {0}", r.GetType().ToString());
            }
            catch (Exception ex)
            {

                string innerMsg = (ex.InnerException != null)
                     ? ex.InnerException.Message
                     : "";
                Console.WriteLine("error occured " + innerMsg);
            }
        }
    }


    public class StatusMessage
    {
        public enum STATE
        {
            SUCCESS,
            FAIL,
            PARTIAL_SUCCESS,
        }


        public string MESSAGE { get; set; }
        public STATE STATUS { get; set; }
    }

}