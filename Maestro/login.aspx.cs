using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Security;

namespace Maestro
{
    public partial class login : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Request.Cookies["username"] != null)
                {
                    txtUserName.Text = Request.Cookies["username"].Value;
                    // txtpassword.Text = c.Values["password"];
                    txtPassword.Attributes.Add("value", Request.Cookies["password"].Value);
                }
            }
            /* If user is logged, 
             * Redirect the user to index page
             * 
            if (Session["userName"] != null)
            {
                Response.Redirect("views/index.aspx");
            }
             */

            txtUserName.Focus();
        }

        protected void btnLogin_Click(object sender, EventArgs e)
        {
            dbconn dbc = new dbconn();
            dbc.connDB();
            SqlCommand cmd = dbc.conn.CreateCommand();
            
            try
            {    
                if (cbRememberMe.Checked == true)
                {
                    Response.Cookies["username"].Value = txtUserName.Text;
                    Response.Cookies["username"].Expires = DateTime.Now.AddDays(1);
                    Response.Cookies["password"].Value = txtPassword.Text;
                    Response.Cookies["password"].Expires = DateTime.Now.AddDays(1);
                }

                string usrName = txtUserName.Text.Trim();
                string usrPwd = txtPassword.Text.Trim();

                usrPwd = FormsAuthentication.HashPasswordForStoringInConfigFile(usrPwd, "MD5");

                cmd.CommandText = "SELECT U.M_USER_ID, U.M_USER_NAME, U.M_USER_PASSWORD, R.ROLE_NAME " +
                                   "FROM M_USER U, M_USER_ROLE UR, M_ROLE R " +
                                   "WHERE UR.M_USER_ID = U.M_USER_ID " +
                                   "AND UR.ROLE_ID = R.ROLE_ID " +
                                   "AND U.M_USER_NAME = '" + usrName + "'" + " AND U.M_USER_PASSWORD = '" + usrPwd + "'";

                SqlDataReader dr = cmd.ExecuteReader();

                if (dr.HasRows)
                {
                    if (dr.Read())
                    {
                        string userRole = dr["ROLE_NAME"].ToString();
                        string userName = dr["M_USER_NAME"].ToString();
                        string userId = dr["M_USER_ID"].ToString();

                        Session["userRole"] = userRole;
                        Session["userName"] = userName;
                        Session["userId"] = userId;

                        //MaestroModule.auditLog(Session["userName"].ToString(), "User", "Logon", "Maestro Monitoring", "Perfrom Logon: Success");

                        if (userRole.Equals("Reconcile"))
                        {
                            Response.Redirect("http://126.32.3.39:8056/");
                        }
                        else if (string.IsNullOrEmpty(userRole))
                        {
                            Response.Write("<script>alert('You are not allow to enter the system!');</script>");
                        }
                        else
                        {
                            Response.Redirect("views/index.aspx");
                        }
                    }
                }
                else
                {
                    errorMessage.InnerText = "Invalid Username or Password";
                    //MaestroModule.auditLog(usrName, "User", "Logon", "Maestro Monitoring", "Perfrom Logon: Failed");
                }
                dr.Close();
            }
            catch (Exception ex)
            {
                errorMessage.InnerText = "Failed. " + ex.Message;
            }
            finally
            {
                dbc.closeConn();
            }
        }


    }
}