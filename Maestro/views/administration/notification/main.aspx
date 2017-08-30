<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="main.aspx.cs" Inherits="Maestro.views.administration.notification.main" %>

<style type="text/css">
    #notification-tab li a {
        line-height: inherit !important;
        padding: 8px 15px !important;
    }
    #notification-div .tab-content{
        margin-top: 20px;
    }
</style>
<script type="text/javascript">
    function saveSmsConf() {
        var ComPort = $('#DdlComPort').val();
        var BaudRate = $('#ddlBaudRate').val();
        var Timeout = $('#ddlTimeout').val();
        var ChkError = $('#ChkError').is(":checked");
        var ChkWarn = $('#ChkWarn').is(":checked");
        var ChkOffline = $('#ChkOffline').is(":checked");
        var Reminder = $('#txtReminder').val();        

        $.ajax({
            type: "POST",
            contentType: "application/json; charset=utf-8",
            url: "/views/administration/notification/main.aspx/updateSmsConf",
            data: "{ComPort:'" + ComPort + "', BaudRate:'" + BaudRate + "', Timeout:'" + Timeout + "', ChkError:'" + ChkError + "', ChkWarn:'" + ChkWarn + "', ChkOffline:'" + ChkOffline + "', Reminder:'" + Reminder + "'}",
            dataType: "json",
            success: function (data) {
                if (data.d) {
                    alert("Successful. Your action has been sent to checker for approval.");
                } else {
                    alert("Failed to update record!");
                }
            },
            error: function (result) {
                alert("Error! Failed to update record.");
            }
        });
    }

    function saveEmailConf() {
        var EmailFrom = $('#txtEmailFrom').val();
        var MailServer = $('#txtMailServer').val();
        var MailPort = $('#txtMailPort').val();
        var EmailUsername = $('#txtEmailUsername').val();
        var EmailPassword = $('#txtEmailPassword').val();
        var EmailErr = $('#chkEmailErr').is(":checked");
        var EmailWarn = $('#chkEmailWarn').is(":checked");
        var EmailOffline = $('#chkEmailOffline').is(":checked");
        var EmailReminder = $('#txtEmailReminder').val();
        var chkSSL = $('#chkSSL').is(":checked");
        $.ajax({
            type: "POST",
            contentType: "application/json; charset=utf-8",
            url: "/views/administration/notification/main.aspx/updateEmailConf",
            data: "{EmailFrom:'" + EmailFrom + "', MailServer:'" + MailServer + "', MailPort:'" + MailPort + "', EmailUsername:'" + EmailUsername +
                "', EmailPassword:'" + EmailPassword + "', EmailErr:'" + EmailErr + "', EmailWarn:'" + EmailWarn + "', EmailOffline:'" + EmailOffline +
                "', EmailReminder:'" + EmailReminder + "', chkSSL:'" + chkSSL + "'}",
            dataType: "json",
            success: function (data) {
                if (data.d) {
                    alert("Successful. Your action will be sent to checker for approval.");
                } else {
                    alert("Failed to update record!");
                }
            },
            error: function (result) {
                alert("Error! Failed to update record.");
            }
        });
    }
</script>
<div id="notification-div">
    <!-- Nav tabs -->
    <ul id="notification-tab" class="nav nav-tabs" style="height: 40px;">
        <li class="active"><a href="#sms-template-tab" data-toggle="tab">SMS Template</a></li>
        <li><a href="#email-template-tab" data-toggle="tab">SMTP Template</a></li>
    </ul>
            <form id="Form1" class="form-horizontal well" role="form" action="javascript:;" runat="server">
    <!-- Tab panes -->
    <div class="tab-content">
        <div class="tab-pane active" id="sms-template-tab">
                <fieldset>
                    <h4>Short Message Service (SMS) Setting</h4>
                    <hr style="border-top-color: #999;" />
                    <div class="form-group">
                        <label for="inputComPort" class="col-xs-2 control-label">Com Port</label>
                        <div class="col-xs-4">
                            <%--<select id="inputComPort" class="form-control">
                                <option>COM5 D-Link HSPADataCard Proprietary USB Modem</option>
                            </select>--%>
                                <span>
                                    <asp:DropDownList ID="DdlComPort" runat="server" class="form-control" >
                                    </asp:DropDownList>
                                </span>
                        </div>
                    </div>
                    <div class="form-group">
                        <label for="inputBaudRate" class="col-xs-2 control-label">Baud Rate</label>
                        <div class="col-xs-4">
                            <%--<select id="inputBaudRate" class="form-control">
                                <option>9600</option>
                                <option>19200</option>
                                <option>38400</option>
                                <option>57600</option>
                                <option>115200</option>
                            </select>--%>
                            
                                <span >
                                    <asp:DropDownList ID="ddlBaudRate" runat="server" class="form-control">
                                        <asp:ListItem>9600</asp:ListItem>
                                        <asp:ListItem>19200</asp:ListItem>
                                        <asp:ListItem>38400</asp:ListItem>
                                        <asp:ListItem>57600</asp:ListItem>
                                        <asp:ListItem>115200</asp:ListItem>
                                    </asp:DropDownList>
                                </span>
                        </div>
                    </div>
                    <div class="form-group">
                        <label for="inputTimeout" class="col-xs-2 control-label">Timeout(ms)</label>
                        <div class="col-xs-4">
                            <%--<select id="inputTimeout" class="form-control">
                                <option>150</option>
                                <option>300</option>
                                <option>600</option>
                                <option>900</option>
                                <option>1200</option>
                            </select>--%>
                            
                                <asp:DropDownList ID="ddlTimeout" runat="server" class="form-control">
                                    <asp:ListItem>150</asp:ListItem>
                                    <asp:ListItem>300</asp:ListItem>
                                    <asp:ListItem>600</asp:ListItem>
                                    <asp:ListItem>900</asp:ListItem>
                                    <asp:ListItem>1200</asp:ListItem>
                                    <asp:ListItem>1500</asp:ListItem>
                                    <asp:ListItem>1800</asp:ListItem>
                                    <asp:ListItem>2000</asp:ListItem>
                                </asp:DropDownList>
                        </div>
                    </div>
                    <div class="form-group">
                        <label class="col-xs-2 control-label">Status Type</label>
                        <div class="col-xs-4">
                            <label class="checkbox-inline">
                                <input type="checkbox" value="error" ID="ChkError" runat="server" > Error
                            </label>
                            <label class="checkbox-inline">
                                <input type="checkbox" value="warning" ID="ChkWarn" runat="server" > Warning
                            </label>
                            <label class="checkbox-inline">
                                <input type="checkbox" value="offline" ID="ChkOffline" runat="server"> Offline
                            </label>                      
                        </div>
                    </div>
                    <div class="form-group">
                        <label for="inputSendReminder" class="col-xs-2 control-label">Send Reminder</label>
                        <div class="col-xs-4">
                            <%--<input type="text" class=" form-control" id="inputSendReminder" placeholder="Minutes">--%>
                                <div style="display: inline-block;"><asp:TextBox class=" form-control" ID="txtReminder" runat="server" Text="30" Width="50"></asp:TextBox></div>
                                <div style="display: inline-block;"><asp:Label ID="Label1" runat="server" Text="Minutes"></asp:Label>
                                    <asp:Label ID="lblReminder" runat="server" ForeColor="Red" Font-Bold="true"></asp:Label></div>
                        </div>
                    </div>
                    <div class="form-group">
                        <div class="col-xs-offset-2 col-xs-4">
                            <button type="button" class="btn btn-primary" onclick="saveSmsConf()">Save</button>
                        </div>
                    </div>
                </fieldset>
        </div>
        <div class="tab-pane" id="email-template-tab">
                <fieldset>
                    <h4>Simple Mail Transfer Protocol (SMTP) - Outgoing Setting</h4>
                    <hr style="border-top-color: #999;" />
                    <div class="form-group">
                        <label for="inputSendFrom" class="col-xs-2 control-label">From</label>
                        <div class="col-xs-4">
                            <%--<input type="email" class=" form-control" id="inputSendFrom" placeholder="From">--%>
                            <asp:TextBox ID="txtEmailFrom" class="form-control" runat="server"></asp:TextBox>
                        </div>
                    </div>
                    <div class="form-group">
                        <label for="inputMailServerIP" class="col-xs-2 control-label">Mail Server</label>
                        <div class="col-xs-4">
                            <%--<input type="text" class=" form-control" id="inputMailServerIP" placeholder="Mail Server">--%>
                            <asp:TextBox ID="txtMailServer" runat="server" class=" form-control" placeholder="Mail Server"></asp:TextBox>
                        </div>
                    </div>
                    <div class="form-group">
                        <label for="inputMailPortNo" class="col-xs-2 control-label">Mail Port No.</label>
                        <div class="col-xs-4">
                            <%--<input type="text" class=" form-control" id="inputMailPortNo" placeholder="Mail Port No.">--%>
                            <asp:TextBox ID="txtMailPort" class="form-control" runat="server" Width="63px"></asp:TextBox>
                        </div>
                    </div>
                    <div class="form-group">
                        <label for="inputMailUsername" class="col-xs-2 control-label">Username</label>
                        <div class="col-xs-4">
                            <%--<input type="text" class=" form-control" id="inputMailUsername" placeholder="Username">--%>
                            <asp:TextBox ID="txtEmailUsername" class="form-control" runat="server"></asp:TextBox>
                        </div>
                    </div>
                    <div class="form-group">
                        <label for="inputMailPassword" class="col-xs-2 control-label">Password</label>
                        <div class="col-xs-4">
                            <%--<input type="password" class=" form-control" id="inputMailPassword" placeholder="Password">--%>
                            <asp:TextBox ID="txtEmailPassword" class="form-control" runat="server" TextMode="Password" placeholder="Password"></asp:TextBox>
                        </div>
                    </div>
                    <div class="form-group">
                        <label class="col-xs-2 control-label">Status Type</label>
                        <div class="col-xs-4">
                            <label class="checkbox-inline">
                                <%--<input type="checkbox" value="error"> Error--%>
                                <asp:CheckBox ID="chkEmailErr" runat="server" Text="Error" />
                            </label>
                            <label class="checkbox-inline">
                                <%--<input type="checkbox" value="warning"> Warning--%>
                                <asp:CheckBox ID="chkEmailWarn" runat="server" Text="Warning" />
                            </label>
                            <label class="checkbox-inline">
                                <%--<input type="checkbox" value="offline"> Offline--%>
                            <asp:CheckBox ID="chkEmailOffline" runat="server" Text="Offline" />
                            </label>                      
                        </div>
                    </div>
                    <div class="form-group">
                        <label for="inputMailSendReminder" class="col-xs-2 control-label">Reminder Interval </label>
                        <div class="col-xs-4">
                            <%--<input type="text" class=" form-control" id="inputMtoday
                                ailSendReminder" placeholder="Minutes">--%>
                            <%--<asp:TextBox ID="txtEmailReminder" runat="server" Text="30" Width="50" AutoPostBack="true" OnTextChanged="txtEmailReminder_TextChanged"></asp:TextBox>--%>
                            <div style="display: inline-block; "><asp:TextBox ID="txtEmailReminder" class="form-control" runat="server" Width="50" placeholder="Minutes"></asp:TextBox></div>
                                <div style="display: inline-block; "><asp:Label ID="Label2" runat="server" Text="Minutes"></asp:Label>
                                <%--<asp:Label ID="lblMsgEmailReminder" runat="server" ForeColor="Red" Font-Bold="true"></asp:Label>--%></div>
                        </div>
                    </div>
                    <div class="form-group">
                        <label class="col-xs-2 control-label">SSL Required</label>
                        <div class="col-xs-4">
                            <label class="checkbox-inline">
                                <%--<input type="checkbox" id="enableSSL" value="enableSSL">--%>
                                 <asp:CheckBox ID="chkSSL" runat="server" />
                            </label>              
                        </div>
                    </div>
                    <div class="form-group">
                        <div class="col-xs-offset-2 col-xs-4">
                            <button type="button" class="btn btn-primary" onclick="saveEmailConf()">Save</button>
                        </div>
                    </div>   
                </fieldset>
        <div style="color:Red; font-size:14px; font-weight: bold; ">
            <asp:Label ID="lblMsg" runat="server"></asp:Label>
        </div>
        </div>
    </div>
            </form>
</div>