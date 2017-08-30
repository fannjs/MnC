<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="add.aspx.cs" Inherits="Maestro.views.administration.userManagement.add" %>

<link rel="stylesheet" href="../components/bootstrap3/css/bootstrap-multiselect.css">
<script src="../components/bootstrap3/js/bootstrap-multiselect.js"></script>
<script type="text/javascript">

    function addUser() {
        var uname = $('#inputUsername').val();
        var pswd = $('#inputPassword').val();
        var email = $('#inputEmail').val();
        var userType = $('#inputSelectUserType').val();
        var moduleSelect = $('#moduleSelect').val();

        $.ajax({
            type: "POST",
            contentType: "application/json; charset=utf-8",
            url: "/views/administration/userManagement/add.aspx/insertNewUser",
            data: "{uname:'" + uname + "',pswd:'" + pswd + "', email:'" + email + "', userType:'" + userType + "'}",
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
            error: function (error) {
                alert("Error " + error.status + "!");
            }
        });
    }

    $(document).ready(function () {
        loadUserRole();
    });

    $('#cancel-button').click(function () {
        showMainPage();
    });
</script>

<div>
    <%--="javascript:; to avoid page refresh to main page--%>
    <form class="form-horizontal well" role="form" action="javascript:;">
        <fieldset>
            <legend>Insert</legend>
            <div class="form-group">
                <label for="inputUsername" class="col-xs-2 control-label">Username</label>
                <div class="col-xs-4">
                    <input type="text" class=" form-control" id="inputUsername" placeholder="Username" required>
                </div>
            </div>
            <div class="form-group">
                <label for="inputPassword" class="col-xs-2 control-label">Password</label>
                <div class="col-xs-4">
                    <input type="password" class="form-control" id="inputPassword" placeholder="Password" required>
                </div>
            </div>
            <div class="form-group">
                <label for="inputSelectUserType" class="col-xs-2 control-label">Role Type</label>
                <div class="col-xs-4">
                    <select class="form-control" id="inputSelectUserType">

                    </select>
                </div>
            </div>
            <div class="form-group">
                <label for="inputEmail" class="col-xs-2 control-label">E-mail</label>
                <div class="col-xs-4">
                    <input type="email" class="form-control" id="inputEmail" placeholder="E-mail" required>
                </div>
            </div>
            <div class="form-group">
                <div class="col-xs-offset-2 col-xs-4">
                    <button type="submit" id="Button1" class="btn btn-primary" onclick="addUser()">Add</button>
                    <button type="button" id="cancel-button" class="btn btn-default">Cancel</button>                    
                </div>
            </div>
        </fieldset>
    </form>
</div>