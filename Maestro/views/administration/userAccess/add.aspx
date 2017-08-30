
<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="add.aspx.cs" Inherits="Maestro.views.administration.userAccess.add" %>

<div>
    <form id="addUserAccessForm" class="form-horizontal" role="form">
        <fieldset>
            <div class="well-div">
                <legend>New User Access</legend>
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
                    <a id="addNewTask" class="link-btn"><i class="fa fa-plus" style="width:15px;"></i>new task</a>
                </div>
                <div class="block-md"></div>--%>
                <div class="field-group">
                    <button type="button" class="btn btn-primary"  id="btnAddAccess">Add</button>
                    <button type="button" class="btn btn-default" id="btnCancel">Back</button>
                </div>
            </div>
        </fieldset>
    </form>
</div>
<script src="administration/userAccess/add.js"></script>
