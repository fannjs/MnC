var taskNameList = "";

////Get from main.aspx function
//function LoadTaskName() {

//    $.ajax({
//        type: "POST",
//        url: "/views/administration/userAccess/main.aspx/getTask",
//        contentType: "application/json; charset=utf-8",
//        data: "{}",
//        dataType: "json",
//        success: function (data) {

//            var status = data.d.Status;
//            var msg = data.d.Message;

//            if (!status) {
//                alert(msg);
//                return false;
//            }

//            var TaskList = data.d.Object;

//            taskNameList = "";
//            taskNameList = '<option disabled selected value="0"> - Please Select - </option>';

//            for (var i = 0; i < TaskList.length; i++) {
//                taskNameList = taskNameList + '<option value="' + TaskList[i].TaskID + '">' + TaskList[i].TaskName + '</option>';
//            }
//        },
//        error: function (error) {
//            alert("Error occurs: Load Task Name");
//        }
//    });
//}

function triggerAddPage() {

    var postData = {
        task : $('#taskNameHidden').val()
    };

    $.post("/views/administration/userAccess/add.aspx", postData, function (data) {
        $("#content-subpage-container").html(data);
        showSubPage();
    });
}

function loadUserAccessList() {
    $.ajax({
        type: "POST",
        url: "/views/administration/userAccess/main.aspx/getUserAccessList",
        contentType: "application/json; charset=utf-8",
        data: "{}",
        dataType: "json",
        beforeSend: function(){
            $('#tblUserAccess > tbody').html('<tr style="text-align:center;"><td colspan="3">Loading...</td></tr>');
        },
        success: function (data) {

            var status = data.d.Status;
            var msg = data.d.Message;

            if (!status) {
                alert(msg);
                return false;
            }

            var UserAccessList = data.d.Object;

            if (UserAccessList.length === 0) {
                $('#tblUserAccess > tbody').html('<tr><td colspan="3">No record was found.</td></tr>');
            } else {
                $('#tblUserAccess > tbody').empty();

                var str = "";

                for (var i = 0; i < UserAccessList.length; i++) {
                    str = str + '<tr><td>' + UserAccessList[i].RoleName + '</td><td><a data-roleid="' + UserAccessList[i].RoleID + '" data-rolename="' + UserAccessList[i].RoleName + '" class="viewRoleTask-btn">click to view</a></td>' +
                                '<td><a href="javascript:;" class="editAccess-button" access-gate task="User Access" permission="Edit" onclick="editAccess(' + UserAccessList[i].RoleID + ')"><i class="fa fa-pencil"></i>Edit</a>' +
                                        '<span class="center-divider"></span><a class="deleteAccess-button" href="javascript:;" access-gate task="User Access" permission="Delete" onclick="deleteAccess(' + UserAccessList[i].RoleID + ')"><i class="fa fa-trash-o"></i>Delete</a></td></tr>';
                }

                $('#tblUserAccess > tbody').html(str);
            }
        }
    });
}

function deleteAccess(roleID) {

    var confirmed = confirm("Are you sure you want to delete this Role ?");

    if (confirmed) {
        $.ajax({
            type: "POST",
            url: "/views/administration/userAccess/main.aspx/deleteUserAccess",
            contentType: "application/json; charset=utf-8",
            data: JSON.stringify({ roleID: roleID }),
            dataType: "json",
            beforeSend: function () {
                $('#userAccessMainDiv .func-btn').attr('disabled', true);
            },
            success: function (data) {

                var status = data.d.Status;
                var msg = data.d.Message;

                $('#userAccessMainDiv .func-btn').attr('disabled', true);

                if (!status) {
                    alert(msg);
                    return false;
                } 

                alert("Successful. Your action will be sent to checker for approval.");
                //loadUserAccess();
            },
            error: function (error) {
                alert("Error occurs: Delete User Access");
                $('#userAccessMainDiv .func-btn').attr('disabled', true);
            }
        });
    }
}

$('#addAccess-button').click(function () {
    triggerAddPage();
})

function editAccess(roleID) {

    var pdata = {
        roleID: roleID,
        task : $('#taskNameHidden').val()
    }
    $.post("/views/administration/userAccess/edit.aspx",pdata,function (data) {
        $("#content-subpage-container").html(data);
        showSubPage();
    });
}

$(document).ready(function () {

    loadUserAccessList();
    $('#tblUserAccess').off('click', '.viewRoleTask-btn').on('click', '.viewRoleTask-btn', viewRoleTaskList);
});

function viewRoleTaskList(e) {

    e.stopPropagation();

    var roleName = $(this).data('rolename');
    var roleId = $(this).data('roleid');

    var str = '<div id="role-task-list-div">' +
                  '<div style="padding:6px 6px 2px 6px;"><label>Role Name :</label><span>' + roleName + '</span><span id="closeRoleTaskListDiv" onclick="offViewRoleTask();"><i class="fa fa-times"></i></span></div>' +
                  '<div>' + 
                  '</div>' +
              '</div>';

    $('#role-task-list-div').remove();
    $(this).parent().append(str);

    loadRoleTastList(roleId);
}

function offViewRoleTask() {
    $('#role-task-list-div').remove();
}

function loadRoleTastList(roleId) {

    $.ajax({
        type: "POST",
        url: "/views/administration/userAccess/main.aspx/getRoleTaskList",
        contentType: "application/json; charset=utf-8",
        data: JSON.stringify({ roleID: roleId }),
        dataType: "json",
        beforeSend: function () {
            $('#role-task-list-div > div').eq(1).html("Loading...");
        },
        success: function (data) {

            var status = data.d.Status;
            var msg = data.d.Message;

            if (!status) {
                alert(msg);
                return false;
            }

            var RoleTaskList = data.d.Object;

            if (RoleTaskList.length === 0) {
                $('#role-task-list-div > div').eq(1).html("No task was assigned to this role.");
            } else {

                var str = "<table id=\"role-task-list-tbl\">" +
                              "<thead>" +
                                    "<tr>" +
                                        "<th>Assigned Task</th>" +
                                        "<th>Create</th>" +
                                        "<th>Read</th>" +
                                        "<th>Update</th>" +
                                        "<th>Delete</th>" +
                                    "</tr>" +
                              "</thead>";

                str = str + "<tbody>";

                for (var i = 0; i < RoleTaskList.length; i++) {

                    var create = (RoleTaskList[i].Create) ? '<i class="fa fa-check" style="color:green;"></i>' : '<i class="fa fa-times" style="color:red;"></i>';
                    var read = (RoleTaskList[i].Read) ? '<i class="fa fa-check" style="color:green;"></i>' : '<i class="fa fa-times" style="color:red;"></i>';
                    var update = (RoleTaskList[i].Update) ? '<i class="fa fa-check" style="color:green;"></i>' : '<i class="fa fa-times" style="color:red;"></i>';
                    var del = (RoleTaskList[i].Delete) ? '<i class="fa fa-check" style="color:green;"></i>' : '<i class="fa fa-times" style="color:red;"></i>';

                    str = str + '<tr><td>' + RoleTaskList[i].TaskName + '</td>' +
                            '<td>' + create + '</td><td>' + read + '</td>' +
                            '<td>' + update + '</td><td>' + del + '</td></tr>';
                }
                $('#role-task-list-div > div').eq(1).html(str);
            }

        },
        error: function (error) {
            alert("Error occurs: Load Role Task");
        }
    });
}
