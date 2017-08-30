//function loadUserAccessDetail() {

//    var roleID = $('#hiddenRoleID').val();

//    $.ajax({
//        type: "POST",
//        url: "/views/administration/userAccess/edit.aspx/getUserAccessDetails",
//        contentType: "application/json; charset=utf-8",
//        data: JSON.stringify({ roleID: roleID }),
//        dataType: "json",
//        success: function (data) {

//            var c, r, u, d;
//            var tasks = "";
//            var role;

//            $('#hiddenRoleID').val(data.d[0].RoleID);
//            $('#inputRoleName').val(data.d[0].RoleName);

//            for (var i = 0; i < data.d.length; i++) {

//                c = (data.d[i].P_Create.toUpperCase() == "TRUE") ? "checked" : "";
//                r = (data.d[i].P_Read.toUpperCase() == "TRUE") ? "checked" : "";
//                u = (data.d[i].P_Update.toUpperCase() == "TRUE") ? "checked" : "";
//                d = (data.d[i].P_Delete.toUpperCase() == "TRUE") ? "checked" : "";

//                role = data.d[i].Role;

//                tasks = tasks + '<div class="task-div oldTask">' +
//                    '<i class="fa fa-trash-o deleteTaskBtn"></i>' +
//                    '<div><label class="form-label width-md">Task Name</label>' +
//                            '<input class="selectTaskName" type="hidden" value="' + data.d[i].TaskID + '" />' +
//                            '<span class="taskName_disabled">' + data.d[i].TaskName + '</span>' +
//                        '</div>' +
////                        '<div><label class="form-label width-md">Role</label>' +
////                            '<select class="selectRole">';

////                if (role === '1') {
////                    tasks = tasks + '<option selected value="1">Maker</option>';
////                } else {
////                    tasks = tasks + '<option value="1">Maker</option>';
////                }
////                if (role === '2') {
////                    tasks = tasks + '<option selected value="2">Maker</option>';
////                } else {
////                    tasks = tasks + '<option value="2">Checker</option>';
////                }
////                if (role === '3') {
////                    tasks = tasks + '<option selected value="3">None</option>';
////                } else {
////                    tasks = tasks + '<option value="3">None</option>';
////                }

////                tasks = tasks + '</select>' +
////                        '</div>' +
//                        '<div class="permision-div">' +
//                            '<label class="form-label width-md">Permission</label>' +
//                            '<span>C <input type="checkbox" class="createCB" ' + c + ' /></span>' +
//                            '<span>R <input type="checkbox" class="readCB" ' + r + ' /></span>' +
//                            '<span>U <input type="checkbox" class="updateCB" ' + u + ' /></span>' +
//                            '<span>D <input type="checkbox" class="deleteCB" ' + d + ' /></span>' +
//                        '</div>' +
//                    '</div>';

//            }

//            $('#oldTaskDiv').html(tasks);

//        },
//        error: function (error) {
//            alert("Error occurs: Delete User Access");
//        }
//    });
//}

//$('#editUserAccessForm').off('click', '.deleteTaskBtn', deleteSingleTask).on('click', '.deleteTaskBtn', deleteSingleTask);

//function deleteSingleTask() {

//    var $trashIcon = $(this);
//    var roleID = $('#hiddenRoleID').val();
//    var taskID = $trashIcon.closest('.oldTask').find('input[type="hidden"]').val();
//    var taskName = $trashIcon.closest('.oldTask').find('.taskName_disabled').text();

//    if (taskID.trim() == "" || taskID == undefined || taskID == null) {
//        alert("Some problem occurs. Please reload and try again later.");
//        return false;
//    }
//    var confirmed = confirm("Are you sure you want to remove '" + taskName + "' from '" + $('#inputRoleName').val() + "' ?");

//    if (confirmed) {

//        $.ajax({
//            type: "POST",
//            url: "/views/administration/userAccess/edit.aspx/deleteRoleTask",
//            contentType: "application/json; charset=utf-8",
//            data: JSON.stringify({ roleID: roleID, taskID: taskID }),
//            dataType: "json",
//            success: function (data) {

//                var status = data.d.Status;
//                var msg = data.d.Message;

//                if (!status) {
//                    alert(msg);
//                    return false;
//                }

//                alert("Successful. Your action will be sent to checker for approval.");

//                /*
//                $trashIcon.closest('.oldTask').fadeOut('fast', function () {
//                $(this).remove();
//                });*/
//            },
//            error: function (error) {
//                alert("Error occurs: Delete Role-Task");
//            }
//        });

//    }
//    else {
//        return;
//    }
//}

//$('#addNewTask').click(function () {
//    AppendNewTaskDiv();
//});

//function AppendNewTaskDiv() {
//    $('#newTaskDiv').append('<div class="task-div newTask">' +
//                    '<i class="fa fa-minus-circle removeTaskDivIcon"></i>' +
//                    '<div><label class="form-label width-md">Task Name</label>' +
//                            '<select class="selectTaskName">'
//                               + taskNameList +
//                            '</select>' +
//                        '</div>' +
//                        '<div class="permision-div">' +
//                            '<label class="form-label width-md">Permission</label>' +
//                            '<span>C <input type="checkbox" class="createCB" /></span>' +
//                            '<span>R <input type="checkbox" class="readCB" /></span>' +
//                            '<span>U <input type="checkbox" class="updateCB" /></span>' +
//                            '<span>D <input type="checkbox" class="deleteCB" /></span>' +
//                        '</div>' +
//                    '</div>');
//}

//$('#editUserAccessForm').off('click', '.removeTaskDivIcon', removeNewTaskDiv).on('click', '.removeTaskDivIcon', removeNewTaskDiv);

//function removeNewTaskDiv() {

//    $(this).closest('.newTask').fadeOut('fast', function () {
//        $(this).remove();
//    });

//}

//$('#btnUpdate').click(function () {

//    var roleID = $('#hiddenRoleID').val();
//    var roleName = $('#inputRoleName').val();
//    var selectTaskName = document.getElementById("editUserAccessForm").getElementsByClassName("selectTaskName");
//    var selectOptions = [];
//    var oldRoleTask = [];
//    var newRoleTask = [];
//    var isPass = true;

//    //Check if user changed the value of hidden field
//    if (roleID.trim() == "" || roleID == null || roleID == undefined) {
//        alert("Some problem occurs. Please reload and try again later.");
//        return;
//    }

//    if (roleName.trim() == "") {
//        alert("Please enter Role Name.");
//        return;
//    }

//    if ($('.task-div').length == 0) {
//        alert("Please add Task.");
//        return;
//    }
//    //Check if any select is not selected
//    for (var i = 0; i < selectTaskName.length; i++) {

//        if (selectTaskName[i].value == 0) {
//            alert("Please select Task.");
//            return;
//            break;
//        }
//        else {
//            continue;
//        }
//    }
//    //Check if any select is duplicated
//    for (var a = 0; a < selectTaskName.length; a++) {
//        selectOptions.push(selectTaskName[a].value);
//    }
//    selectOptions.sort();
//    for (a = 1; a < selectOptions.length; a++) {

//        if (selectOptions[a] == selectOptions[a - 1]) {
//            alert("Task duplicated. \nPlease modify and try again later.");
//            return;
//            break;
//        }
//        else {
//            continue;
//        }
//    }

//    $('.task-div').each(function () {
//        if ($(this).find('input[type="checkbox"]:checked').length == 0) {
//            alert("Please grant at least one permission for each task.");
//            isPass = false;
//            return false;
//        }
//    });

//    if (!isPass) {
//        return;
//    }

//    $('.oldTask').each(function () {

//        c = ($(this).find('.createCB').prop('checked')) ? 1 : 0;
//        r = ($(this).find('.readCB').prop('checked')) ? 1 : 0;
//        u = ($(this).find('.updateCB').prop('checked')) ? 1 : 0;
//        d = ($(this).find('.deleteCB').prop('checked')) ? 1 : 0;

//        var task = {
//            id: $(this).find('.selectTaskName').val(),
//            role: $(this).find('.selectRole').val(),
//            c: c,
//            r: r,
//            u: u,
//            d: d
//        };

//        oldRoleTask.push(task);
//    });

//    $('.newTask').each(function () {

//        c = ($(this).find('.createCB').prop('checked')) ? 1 : 0;
//        r = ($(this).find('.readCB').prop('checked')) ? 1 : 0;
//        u = ($(this).find('.updateCB').prop('checked')) ? 1 : 0;
//        d = ($(this).find('.deleteCB').prop('checked')) ? 1 : 0;

//        var task = {
//            id: $(this).find('.selectTaskName').val(),
//            //role: $(this).find('.selectRole').val(),
//            c: c,
//            r: r,
//            u: u,
//            d: d
//        };

//        newRoleTask.push(task);
//    });
//    
//    updateRoleTask(roleID, roleName, oldRoleTask, newRoleTask);

//});

//function updateRoleTask(roleID,roleName,oldRoleTask,newRoleTask) {

//    $.ajax({
//        type: "POST",
//        url: "/views/administration/userAccess/edit.aspx/updateRoleTask",
//        contentType: "application/json; charset=utf-8",
//        data: JSON.stringify({ roleID: roleID, roleName: roleName, oldRoleTask: oldRoleTask, newRoleTask: newRoleTask }),
//        dataType: "json",
//        beforeSend: function () {
//            $('#btnUpdate').attr('disabled', true);
//        },
//        success: function (data) {

//            var status = data.d.Status;
//            var msg = data.d.Message;

//            $('#btnUpdate').attr('disabled', false);

//            if (!status) {
//                alert(msg);
//                return false;
//            }

//            alert("Successful. Your action will be sent to checker for approval.");
//            showMainPage();
//        },
//        error: function (error) {
//            alert("Error occurs: Delete Role-Task");
//            $('#btnUpdate').attr('disabled', false);
//        }
//    });

//}
var taskList = [];
var CurrentTaskId = [];

function LoadTaskList() {
    $.ajax({
        type: "POST",
        url: "/views/administration/userAccess/main.aspx/GetTaskList",
        contentType: "application/json; charset=utf-8",
        data: "{}",
        dataType: "json",
        beforeSend: function () {
            $('#tblUserAccessTaskList > tbody').html('<tr><td colspan="7">Loading Task...</td></tr>');
        },
        success: function (data) {
            var status = data.d.Status;
            var msg = data.d.Message;

            if (!status) {
                alert(msg);
                return false;
            }

            var MainTaskList = data.d.Object;
            var str = "";
            if (MainTaskList.length === 0) {
                str = str + '<tr><td colspan="7">No Task was found. Please contact Administrator.</td></tr>';
            } else {

                for (var i = 0; i < MainTaskList.length, MainTask = MainTaskList[i]; i++) {
                    var PTaskId = MainTask.TaskId;
                    var PTaskName = MainTask.TaskName;
                    var SubTaskList = MainTask.SubTaskList;

                    str = str + '<tr data-pid="' + PTaskId + '" data-subid="' + SubTaskList[0].TaskId + '"><td rowspan="' + SubTaskList.length + '"><input type="checkbox" class="parentCB" />' + PTaskName + '</td>';

                    for (var a = 0; a < SubTaskList.length, SubTask = SubTaskList[a]; a++) {

                        if (a > 0) {
                            str = str + '<tr data-pid="' + PTaskId + '" data-subid="' + SubTask.TaskId + '">';
                        }
                        str = str + '<td><input type="checkbox" class="childCB" />' + SubTask.TaskName + '</td>';
                        str = str + '<td style="text-align:center;"><input type="checkbox" class="taskCB createCB" /></td>';
                        str = str + '<td style="text-align:center;"><input type="checkbox" class="taskCB readCB" /></td>';
                        str = str + '<td style="text-align:center;"><input type="checkbox" class="taskCB updateCB" /></td>';
                        str = str + '<td style="text-align:center;"><input type="checkbox" class="taskCB deleteCB" /></td>';
                        str = str + '</tr>';
                    }

                }
            }

            $('#tblUserAccessTaskList > tbody').html(str);
            LoadUserAccess();
        },
        error: function (error) {
            alert("Error " + error.status + ". Unable to load task list.");
        }
    });
}

function LoadUserAccess() {

    var roleID = $('#hiddenRoleID').val();

    $.ajax({
        type: "POST",
        url: "/views/administration/userAccess/edit.aspx/GetUserAccess",
        contentType: "application/json; charset=utf-8",
        data: JSON.stringify({ roleID: roleID }),
        dataType: "json",
        beforeSend: function () {

        },
        success: function (data) {
            var status = data.d.Status;
            var msg = data.d.Message;

            if (!status) {
                alert(msg);
                return false;
            }

            var UserAccess = data.d.Object;
            var RoleId = UserAccess.RoleID;
            var RoleName = UserAccess.RoleName;
            var TaskList = UserAccess.TaskList;

            $('#hiddenRoleID').val(RoleId);
            $('#inputRoleName').val(RoleName);

            if (TaskList.length !== 0) {

                for (var i = 0; i < TaskList.length, Task = TaskList[i]; i++) {

                    var TaskId = Task.TaskId;
                    var PTaskId = Task.PTaskId;
                    var TR = $('#tblUserAccessTaskList').find('tr[data-pid="' + PTaskId + '"][data-subid="' + TaskId + '"]');

                    if (Task.Create) {
                        $(TR).find('.createCB').prop('checked', true);
                    }
                    if (Task.Read) {
                        $(TR).find('.readCB').prop('checked', true);
                    }
                    if (Task.Update) {
                        $(TR).find('.updateCB').prop('checked', true);
                    }
                    if (Task.Delete) {
                        $(TR).find('.deleteCB').prop('checked', true);
                    }
                }
            }
        },
        error: function (error) {
            alert("Error " + error.status + ". Unable to load task list.");
        }
    });
}

$('#tblUserAccessTaskList').off('change', '.parentCB', selectAllTask).on('change', '.parentCB', selectAllTask);
function selectAllTask() {
    var pid = $(this).closest('tr').data('pid');
    var checked = $(this).prop('checked');

    if (checked) {
        $('#tblUserAccessTaskList').find('tr[data-pid="' + pid + '"] .childCB').prop('checked', true);
        $('#tblUserAccessTaskList').find('tr[data-pid="' + pid + '"] .taskCB').prop('checked', true);
    } else {
        $('#tblUserAccessTaskList').find('tr[data-pid="' + pid + '"] .childCB').prop('checked', false);
        $('#tblUserAccessTaskList').find('tr[data-pid="' + pid + '"] .taskCB').prop('checked', false);
    }
}

$('#tblUserAccessTaskList').off('change', '.childCB', selectTask).on('change', '.childCB', selectTask);
function selectTask() {
    var subid = $(this).closest('tr').data('subid');
    var checked = $(this).prop('checked');

    if (checked) {
        $('#tblUserAccessTaskList').find('tr[data-subid="' + subid + '"] .taskCB').prop('checked', true);
    } else {
        $('#tblUserAccessTaskList').find('tr[data-subid="' + subid + '"] .taskCB').prop('checked', false);
    }
}

$('#btnUpdate').click(function () {
    var roleID = $('#hiddenRoleID').val();
    var roleName = $('#inputRoleName').val().trim();
    var TaskCB = $('#tblUserAccessTaskList > tbody').find('tr');
    var taskList = [];
    var CurrentTaskId = [];
    var id = "";
    var c, r, u, d;

    if (inputRoleName == "") {
        alert("Please enter Role Name.");
        return;
    }

    if ($('#tblUserAccessTaskList > tbody').find('.taskCB:checked').length === 0) {
        alert("Please grant access for at least ONE task.");
        return false;
    }

    for (var i = 0; i < TaskCB.length, Task = TaskCB[i]; i++) {

        if ($(Task).find('.taskCB:checked').length === 0) {
            continue;
        }

        var id = Task.getAttribute('data-subid');
        var c = ($(Task).find('.createCB').prop('checked')) ? 1 : 0;
        var r = ($(Task).find('.readCB').prop('checked')) ? 1 : 0;
        var u = ($(Task).find('.updateCB').prop('checked')) ? 1 : 0;
        var d = ($(Task).find('.deleteCB').prop('checked')) ? 1 : 0;

        var task = {
            id: id,
            c: c,
            r: r,
            u: u,
            d: d
        };

        taskList.push(task);
        CurrentTaskId.push(id);
    }
    
    UpdateUserAccess(roleID, roleName, taskList, CurrentTaskId);
});

function UpdateUserAccess(roleID, roleName, taskList, CurrentTaskId) {
    $.ajax({
        type: "POST",
        url: "/views/administration/userAccess/edit.aspx/UpdateUserAccess",
        contentType: "application/json; charset=utf-8",
        data: JSON.stringify({ roleID: roleID, roleName: roleName, TaskList: taskList, CurrentTaskId: CurrentTaskId }),
        dataType: "json",
        beforeSend: function () {
            $('#btnUpdate').attr('disabled', true);
        },
        success: function (data) {

            var status = data.d.Status;
            var msg = data.d.Message;

            $('#btnUpdate').attr('disabled', false);

            if (!status) {
                alert(msg);
                return false;
            }

            alert("Successful. Your action will be sent to checker for approval.");
            showMainPage();
        },
        error: function (error) {
            alert("Error occurs: Delete Role-Task");
            $('#btnUpdate').attr('disabled', false);
        }
    });
}

$('#btnCancel').click(function () {
    showMainPage();
});

$(document).ready(function () {
    LoadTaskList();  
});