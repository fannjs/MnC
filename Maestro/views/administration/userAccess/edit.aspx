<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="edit.aspx.cs" Inherits="Maestro.views.administration.userAccess.edit" %>

<div id="editUserAccessMainDiv">
    <form id="editUserAccessForm" class="form-horizontal" role="form">
        <fieldset>
            <div class="well-div">
                <legend>Edit User Access</legend>
                <input id="hiddenRoleID" type="hidden" runat="server" disabled="disabled" class="form-control">
                <!--<input id="hiddenRoleID" type="hidden" />-->
                <div class="field-group">
                    <label for="inputRoleName" class="form-label width-md">Role Name</label>
                    <input type="text" class="input-field width-xl" id="inputRoleName" placeholder="Role Name">
                </div>
                <div class="block-xs"></div>
                <div>
                    <table id="tblUserAccessTaskList" class="table table-bordered">
                        <thead>
                            <tr>
                                <th rowspan="2" style="text-align:center;">Module</th>
                                <th rowspan="2" style="text-align:center;">Task</th>
                                <th colspan="4" style="text-align:center;">Role</th>
                            </tr>
                            <tr>
                                <th>CREATE</th>
                                <th>READ</th>
                                <th>UPDATE</th>
                                <th>DELETE</th>
                            </tr>
                        </thead>
                        <tbody>
                        </tbody>
                    </table>
                </div>
                <%--<hr style="margin: 10px 0px;" />
                <div class="field-group">
                    <!-- Task here -->
                    <div id="oldTaskDiv">
                    
                    </div>
                    <hr style="margin:10px 0px;" />
                    <a id="addNewTask" class="link-btn"><i class="fa fa-plus" style="width:15px;"></i>new task</a>
                    <div id="newTaskDiv">
                        
                    </div>
                </div>--%>
                <div class="block-md"></div>
                <div class="field-group">
                    <button type="button" class="btn btn-primary"  id="btnUpdate">Update</button>
                    <button type="button" class="btn btn-default" id="btnCancel">Back</button>
                </div>
            </div>
        </fieldset>
    </form>
</div>
<script src="administration/userAccess/edit.js"></script>