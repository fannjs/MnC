<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="main.aspx.cs" Inherits="Maestro.views.administration.userAccess.main" %>
<link rel="Stylesheet" href="administration/userAccess/userAccess.css" />
<div id="userAccessMainDiv">
    <input type="hidden" id="taskNameHidden" runat="server" /> 
    <div id="addButton">        
        <a access-gate task="User Access" permission="Add"  href="javascript:;" id="addAccess-button" class="func-btn"><i class="fa fa-plus"></i>Add New Role</a>
    </div>
    <div class="block-xs"></div>
    <table class="table table-bordered" id="tblUserAccess">
        <thead>
            <tr>
                <th>Role Name</th>
                <th>Task Name (C,R,U,D)</th>
                <th></th>
            </tr>
        </thead>
        <tbody>
        </tbody>
    </table>
</div>
<script src="administration/userAccess/main.js"></script>