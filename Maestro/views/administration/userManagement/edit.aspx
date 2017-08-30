<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="edit.aspx.cs" Inherits="Maestro.views.administration.userManagement.edit" %>

<link rel="stylesheet" href="../components/bootstrap3/css/bootstrap-multiselect.css">
<script src="../components/bootstrap3/js/bootstrap-multiselect.js"></script>
<script type="text/javascript">
    $(document).ready(function () {
        loadUserRole();
        loadUserDetail();
    });

    function loadUserDetail() {

        var pdata = {
            userID: pUserid
        };

        $.ajax({
            type: "POST",
            contentType: "application/json; charset=utf-8",
            url: "/views/administration/userManagement/edit.aspx/getUserDetail",
            data: JSON.stringify(pdata),
            dataType: "json",
            success: function (data) {
                var status = data.d.Status;
                var msg = data.d.Message;
                var User = data.d.Object;

                if (!status) {
                    alert(msg);
                    return false;
                }

                $('#txtUserID').val(User.UserID);
                $('#inputUsername').val(User.UserName);
                $('#inputEmail').val(User.Email);
                $('#inputSelectUserType option[value="' + User.UserRole.RoleID + '"]').prop('selected', true);
            },
            error: function (error) {
                alert("Error " + error.status);
            }
        });   
    }

    function updateUser() {
        var userID = $('#txtUserID').val();
        var uname = $('#inputUsername').val();
        var pswd = $('#inputPassword').val().trim();
        var email = $('#inputEmail').val().trim();
        var userType = $('#inputSelectUserType').val();
        var userTypeDesc = $('#inputSelectUserType option:selected').text();

        if (email === "") {
            alert("Please enter Email Address.");
            $('#inputEmail').focus();
            return false;
        }

        $.ajax({
            type: "POST",
            contentType: "application/json; charset=utf-8",
            url: "/views/administration/userManagement/edit.aspx/updateUser",
            data: "{userID:'" + userID + "', uname:'" + uname + "',pswd:'" + pswd + "', email:'" + email + "', userType:'" + userType + "', userTypeDesc:'" + userTypeDesc + "'}",
            dataType: "json",
            success: function (data) {

                var status = data.d.Status;
                var msg = data.d.Message;

                if (!status) {
                    alert(msg);
                    return false;
                }

                alert("Successful. Your action has been sent to checker for approval.");
                getUserFromBackEnd();
                showMainPage();
            },
            error: function (result) {
                alert("Error " + result.status);
            }
        });
    }

    $('#cancel-button').click(function () {
        showMainPage();
    });
</script>
<div class="well">
    <form class="form-horizontal" role="form" action="javascript:;">
        <fieldset>
            <legend>
                Edit
            </legend>
            <div><input runat="server" id="txtUserID" value="0" style="display:none"/></div>
            <div class="form-group">
                <label for="inputUsername" class="col-xs-2 control-label">Username</label>
                <div class="col-xs-4">
                    <input type="text" disabled="disabled" runat="server" class="form-control" id="inputUsername" placeholder="Username" />
                </div>
            </div>
            <div class="form-group">
                <label for="inputPassword" class="col-xs-2 control-label">Password</label>
                <div class="col-xs-4">
                    <input type="password" class="form-control" id="inputPassword" placeholder="Password" />
                </div>
            </div>
            <div class="form-group">
                <label id="Label1" for="inputUserType" runat="server" class="col-xs-2 control-label">Role Type</label>
                <div class="col-xs-4" id="divUserType" runat="server">
                    <select class="form-control" id="inputSelectUserType" runat="server">

                    </select>
                </div>
            </div>
            <div class="form-group">
                <label for="inputEmail" class="col-xs-2 control-label">E-mail</label>
                <div class="col-xs-4">
                    <input type="text" runat="server" class="form-control" id="inputEmail" placeholder="E-mail"/>
                </div>
            </div>
            <div class="form-group">
                <div class="col-xs-offset-2 col-xs-4">
                    <button type="submit" class="btn btn-primary" onclick="updateUser();">Update</button>
                    <button type="button" id="cancel-button" class="btn btn-default">Cancel</button>
                </div>
            </div>
        </fieldset>
    </form>
</div>