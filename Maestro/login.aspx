<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="login.aspx.cs" Inherits="Maestro.login" %>

<!DOCTYPE html>

<html>
    <head>
        <link rel="stylesheet" href="components/bootstrap3/css/bootstrap.min.css"> 
        <style type="text/css">
            body{
                color: #333333;
                font-family: "Helvetica Neue",Helvetica,Arial,sans-serif;
                font-size: 14px;
                line-height: 20px;
            }
            #background{
                left: 0px; 
                top: 0px; 
                overflow: hidden; 
                margin: 0px; 
                padding: 0px; 
                
                filter: blur(2px); 
                -webkit-filter: blur(2px); 
                -moz-filter: blur(2px);
            }
            #loginDiv{
                background-color: #FFF;
                border-radius: 15px;
                box-shadow: 0 0 8px rgba(0, 0, 0, 0.4);
                height: 370px;
                padding: 25px;
                text-align: center;
                width: 500px;
                margin: 3em auto;
            }
            
            .login-form legend{
                margin-bottom: 35px;
                margin-top: 5px;
                padding-bottom: 25px;
                color: #333333;
                font-size: 36px;
                font-weight: 300;
            }
            
            .form-body{
                border-bottom: 1px solid #EEEEEE;
                height: 189px;
            }
            .form-footer{
                margin-top: 20px;
            }
            
            input[type="text"],input[type="password"]{
                width: 280px;
                border-radius: 4px;
                color: #555555;
                display: inline-block;
                font-size: 14px;
                line-height: 20px;
                height: 34px;
                margin-bottom: 10px;
                padding: 6px 12px;
                vertical-align: middle;
                background-color: #FFFFFF;
                border: 1px solid #CCCCCC;
                box-shadow: 0 1px 1px rgba(0, 0, 0, 0.075) inset;
                transition: border 0.2s linear 0s, box-shadow 0.2s linear 0s;
            }
            
            input[type="text"]:focus,input[type="password"]:focus{
                border-color: rgba(82, 168, 236, 0.8);
                box-shadow: 0 1px 1px rgba(0, 0, 0, 0.075) inset, 0 0 8px rgba(82, 168, 236, 0.6);
                outline: 0 none;
            }
            .navbar
            {
                margin:0px;
                border-radius: 0px;
            }
            .navbar-header{
                height:70px;
                margin-left: 5px;
                margin-top: 10px;
            }
            
            #footerPanel{
                padding-top: 10px;
                height:50px;
                text-align: center;
                line-height: 18px;
            }
            
            .btn-black{
                background-color: #222222;
                background-image: none;
                box-shadow: none;
                color: #FFFFFF;
                padding: 5px 20px;
                text-shadow: none;
            }
            
            label{
                font-size: 20px;
            }
            
        </style>
        <script src="components/jquery/jquery.js" type="text/javascript"></script>
        <%--<script type="text/javascript">
            $(document).ready(function () {
                //Auto set width and height for background according to window size
                background_autoresize(); 

                //Call auto resize function when the browser size changed
                $(window).resize(function () {
                    background_autoresize();
                });
            });

            function background_autoresize() {
                var window_height = $(window).height();
                var navbar_height = $('.navbar').height();
                var bg_height = window_height - navbar_height;

                $('#background').css("height", bg_height + "px");
                $('#background').css("width", $(window).width() + "px");
            }
        </script>--%>
    </head>
    <body>
        <div class="navbar navbar-default" role="navigation">
            <div class="navbar-header">
                <img ID="img1" src="app_monitoring/Images/maestroLogo.png" />
            </div>
        </div>
        <div>
            <div id="loginDiv">
                <form id="Form1" class="form login-form" runat="server">
                    <legend>
                        Sign in to M&C
                    </legend>
                    <div class="form-body">
                        <label>Username</label><br/>
                        <asp:TextBox ID="txtUserName" runat="server"></asp:TextBox><br/>
                        <%--<asp:TextBox ID="txtUserName" runat="server">admin</asp:TextBox><br/>--%>
                        <%--<input type="text"><br/>--%>
                        <label>Password</label><br/>
                        <%--<input type="password"><br/>--%>
                        <asp:TextBox ID="txtPassword" runat="server" type="password" ></asp:TextBox><br/>
                        <span id="errorMessage" runat="server" style="color:Red;"></span>
                    </div>
                    <div class="form-footer">
                        <input type="checkbox" id="cbRememberMe" runat="server">
                        Remember me
                        <asp:Button ID="btnLogin" runat="server" class="btn btn-success" Text="Login" OnClick="btnLogin_Click"/>
                    </div>
                </form>
            </div>
        </div>
    </body>
</html>