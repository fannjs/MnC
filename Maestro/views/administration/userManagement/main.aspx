<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="main.aspx.cs" Inherits="Maestro.views.administration.userManagement.main" %>

<script type="text/javascript">
    var pUserid;
    var postData = {
        task: $('#taskNameHidden').val()
    };
    function triggerAddPage() {

        $.post("/views/administration/userManagement/add.aspx", postData, function (data) {
            $("#content-subpage-container").html(data);
            //alert("triggerAddPage");
            showSubPage();
        });
    }
    function triggerEditPage(userid) {

        var pdata = {
            task: $('#taskNameHidden').val()
        };
        pUserid = userid;

        $.post("/views/administration/userManagement/edit.aspx", pdata, function (data) {
            $("#content-subpage-container").html(data);
            showSubPage();
        });
    }

    function editUser(userid) {
        triggerEditPage(userid);
    }
    function getUserFromBackEnd() {
        $.ajax({
            type: "POST",
            contentType: "application/json; charset=utf-8",
            url: "/views/administration/userManagement/main.aspx/getUserDetails",
            data: "{}",
            dataType: "json",
            beforeSend: function(){
                $("#tbDetails > tbody").empty();
                $("#tbDetails > tbody").html("<tr><td colspan='4'>Loading...</td></tr>");
            },
            success: function (data) {

                $("#tbDetails > tbody").empty();

                if (data.d.length === 0) {
                    $("#tbDetails > tbody").html("<tr><td colspan='4'>No user</td></tr>");
                } else {
                    for (var i = 0; i < data.d.length; i++) {
                        $("#tbDetails > tbody").append("<tr><td>" + data.d[i].UserName + "</td><td>" + data.d[i].RoleName + "</td><td>" + data.d[i].UserEmail +
                        "</td><td><a href='javascript:;' class='.editUser-button' id='" + data.d[i].UserId +
                        "' onclick='editUser(" + data.d[i].UserId + ")'><i class='fa fa-pencil'></i>Edit</a>  <span class='center-divider'></span>" +
                        "<a class='deleteUser-button' href='javascript:;' onclick='deleteUser(" + data.d[i].UserId + ")'><i class='fa fa-trash-o'></i>Delete</a></td> </tr>"
                        );
                    }
                }
            },
            error: function (result) {
                alert("Error " + result.status);
            }
        });
    }

    $(document).ready(function () {
        getUserFromBackEnd();

        $('#addUser-button').click(function () {
            triggerAddPage();
        })
    });

    function deleteUser(userID) {
        var option = confirm("Are you sure you want to Delete?");
        if (option) {

            $.ajax({
                type: "POST",
                contentType: "application/json; charset=utf-8",
                url: "/views/administration/userManagement/main.aspx/deleteUser",
                data: "{userID:'" + userID + "'}",
                dataType: "json",
                success: function (data) {
                    if (data.d) {
                        alert("Successful. Your action has been sent to checker for approval.");
                        getUserFromBackEnd();
                        showMainPage();
                    } else {
                        alert("Failed to delete user!");
                    }
                },
                error: function (result) {
                    alert("Error " + result.status);
                }
            });
        }
    }

    function loadUserRole() {

        $.ajax({
            type: "POST",
            contentType: "application/json; charset=utf-8",
            url: "/views/administration/userManagement/main.aspx/loadUserRole",
            data: "{}",
            dataType: "json",
            success: function (data) {
                $('#inputSelectUserType').empty();
                $('#inputSelectUserType').append('<option selected disabled> - Please select - </option>');

                for (var i = 0; i < data.d.length; i++) {
                    $('#inputSelectUserType').append('<option value="' + data.d[i].RoleID + '">' + data.d[i].RoleName + ' </option>');
                }
            },
            error: function (result) {
                alert("Error loading User Role.");
            }
        });
    }

</script>
<div>
    <input id="taskNameHidden" type="hidden" runat="server" />
    <div id="addButton">        
        <a access-gate task='User Control' permission='Add' href="javascript:;" id="addUser-button" class="func-btn"><i class="fa fa-plus"></i>Add New User</a>
    </div>
    <div class="block-xs"></div>
    <table class="table table-bordered" id="tbDetails">
        <thead>
            <tr>
                <th>User Name</th>
                <th>Role Type</th>
                <th>Email</th>
                <th></th>
            </tr>
        </thead>
        <tbody>
        </tbody>
    </table>
</div>

