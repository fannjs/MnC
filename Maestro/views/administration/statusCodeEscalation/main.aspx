<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="main.aspx.cs" Inherits="Maestro.views.administration.statusCodeEscalation.main" %>

<link rel="Stylesheet" href="administration/statusCodeEscalation/addEdit.css" />
<div id="statusCodeEscalation_Main">
    <div>
        <input type="hidden" id="taskNameHidden" runat="server" /> 
        <a access-gate task="User Access" permission="Add"  href="javascript:;" class="func-btn" onclick="newEscalation();"><i class="fa fa-plus"></i>Add New Escalation</a>
        <div style="display:inline-block;width:30px;"></div>
        <a access-gate task="User Access" permission="Add"  href="javascript:;" class="func-btn" onclick="configureEmail();"><i class="fa fa-plus"></i>Configure Email</a>
        <div style="display:inline-block;width:30px;"></div>
        <a access-gate task="User Access" permission="Add"  href="javascript:;" class="func-btn" onclick="configureSMS();"><i class="fa fa-plus"></i>Configure SMS</a>
        <%--<button style="float:right;" onclick="TestEmail();">Test Send Email</button>--%>
    </div>
    <div class="block-xs"></div>
    <div>
        <table class="table table-bordered" id="tblStatusCodeEscalation">
            <thead>
                <tr>
                    <th>Kiosk Type</th>
                    <th>Status Code</th>
                    <th>Escalation List</th>
                    <th></th>
                </tr>
            </thead>
            <tbody>
            </tbody>
        </table>
    </div>
</div>
<script type="text/javascript" src="administration/statusCodeEscalation/main.js"></script>
