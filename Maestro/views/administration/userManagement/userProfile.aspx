<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="userProfile.aspx.cs" Inherits="Maestro.views.administration.userManagement.userProfile" %>

<script type="text/javascript">

    $(document).ready(function () {
        GetUserAccessDetails();
    });

    function GetUserAccessDetails() {
        $.ajax({
            type: "POST",
            contentType: "application/json; charset=utf-8",
            url: "/views/administration/userManagement/userProfile.aspx/GetUserAccess",
            data: "{}",
            dataType: "json",
            success: function (data) {

                var status = data.d.Status;
                var msg = data.d.Message;

                if (!status) {
                    alert(msg);
                    return false;
                }

                var UserAccess = data.d.Object;
                var AccessLevel = UserAccess.RoleName;
                var Tasks = UserAccess.TaskList;
                var str = "";

                for (var i = 0; i < Tasks.length; i++) {
                    str = str + "- " + Tasks[i].TaskName;

                    if (i !== Tasks.length - 1) {
                        str = str + "\n";
                    }
                }

                $('#txtAccessLevel').val(AccessLevel);
                $('#txtTaskAccess').val(str);
            },
            error: function (error) {
                alert("Error " + error.status + "!");
            }
        });
    }
    
    function updateProfile() {
        var pswd = $('#txtNewPassword').val();
        var reenterpswd = $('#txtReenterPassword').val();

        if (pswd == "") {
            alert("Please enter New Password.");
            $('#txtNewPassword').focus();
            return false;
        }
        if (reenterpswd == "") {
            alert("Please re-enter the New Password.");
            $('#txtReenterPassword').focus();
            return false;
        }

        if (pswd == reenterpswd) {

            $.ajax({
                type: "POST",
                contentType: "application/json; charset=utf-8",
                url: "/views/administration/userManagement/userProfile.aspx/UpdateProfile",
                data: JSON.stringify({ password: pswd }),
                dataType: "json",
                success: function (data) {

                    var status = data.d.Status;
                    var msg = data.d.Message;

                    if (!status) {
                        alert(msg);
                        return false;
                    }

                    alert("Your profile has updated successfully. You are required to re-login.");
                    logout();
                },
                error: function (error) {
                    alert("Error " + error.status + "!");
                }
            });

        } else {
            alert("Password is not match! Please enter again.");
            return false;
        }
    }

    function resetInput() {
        $('#txtNewPassword').val("");
        $('#txtReenterPassword').val("");
    }
</script>
<div>
    <div class="field-group">
        <label for="inputUsername" class="form-label width-lg">New Password</label>
        <input type="password" class="input-field width-xl" id="txtNewPassword" placeholder="New Password">
    </div>
    <div class="field-group">
        <label for="inputPassword" class="form-label width-lg">Re-Enter Password</label>
        <input type="password" class="input-field width-xl" id="txtReenterPassword" placeholder="Re-Enter Password">
    </div>
    <div class="field-group">
        <label for="inputUsername" class="form-label width-lg">Access Level</label>
        <input type="text" class="input-field width-xl" id="txtAccessLevel" Access Level" disabled>
    </div>
    <div class="field-group">
        <label for="inputSelectUserType" class="form-label width-lg">Task Accessibility</label>
        <textarea name="textarea" rows="6" class="input-field width-xl" id="txtTaskAccess" placeholder="Access Level" disabled></textarea>
    </div>
    <div class="field-group">
        <label class="form-label width-lg"></label>
        <button class="btn btn-primary" onclick="updateProfile();">Update</button>
        <button class="btn btn-default" onclick="resetInput();">Reset</button>
    </div>
</div>