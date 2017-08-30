//$('#addNewTask').click(function () {
//    AppendNewTaskDiv($(this));
//});

//function AppendNewTaskDiv($this) {
//    $this.before('<div class="task-div">' +
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

//$('#addUserAccessForm').off('click', '.removeTaskDivIcon', removeNewTaskDiv).on('click', '.removeTaskDivIcon', removeNewTaskDiv);
//function removeNewTaskDiv() {

//    $(this).closest('.task-div').fadeOut('fast', function () {
//        $(this).remove();
//    });

//}

//$('#btnAddAccess').click(function () {

//    var inputRoleName = $('#inputRoleName').val().trim();
//    var selectTaskName = document.getElementById("addUserAccessForm").getElementsByClassName("selectTaskName");
//    var selectOptions = [];
//    var taskList = [];
//    var c, r, u, d;
//    var isPass = true;

//    if (inputRoleName == "") {
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

//    $('.task-div').each(function () {
//        c = ($(this).find('.createCB').prop('checked')) ? 1 : 0;
//        r = ($(this).find('.readCB').prop('checked')) ? 1 : 0;
//        u = ($(this).find('.updateCB').prop('checked')) ? 1 : 0;
//        d = ($(this).find('.deleteCB').prop('checked')) ? 1 : 0;

//        var task = {
//            id: $(this).find('.selectTaskName').val(),
//            c: c,
//            r: r,
//            u: u,
//            d: d
//        };

//        taskList.push(task);
//    });

//    addNewUserAccess(inputRoleName, taskList);
//});

//function resetField() {
//    $('#inputRoleName').val("");
//    $('.task-div').remove();
//}

$('#btnAddAccess').click(function () {
    var inputRoleName = $('#inputRoleName').val().trim();
    var TaskCB = $('#tblUserAccessTaskList > tbody').find('tr');
    var taskList = [];
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

        c = ($(Task).find('.createCB').prop('checked')) ? 1 : 0;
        r = ($(Task).find('.readCB').prop('checked')) ? 1 : 0;
        u = ($(Task).find('.updateCB').prop('checked')) ? 1 : 0;
        d = ($(Task).find('.deleteCB').prop('checked')) ? 1 : 0;

        var task = {
            id: Task.getAttribute('data-subid'),
            c: c,
            r: r,
            u: u,
            d: d
        };

        taskList.push(task);
    }

    addNewUserAccess(inputRoleName, taskList);
});

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
        },
        error: function (error) {
            alert("Error " + error.status + ". Unable to load task list.");
        }
    });
}

function addNewUserAccess(roleName, taskList) {

    $.ajax({
        type: "POST",
        url: "/views/administration/userAccess/add.aspx/addNewUserAccess",
        contentType: "application/json; charset=utf-8",
        data: JSON.stringify({ roleName: roleName, taskList: taskList }),
        dataType: "json",
        beforeSend: function () {
            $('#btnAddAccess').attr('disabled', true);
        },
        success: function (data) {

            $('#btnAddAccess').attr('disabled', false);

            if (!data.d.Status) {
                alert(data.d.Description);
                return false;
            }

            alert("Successful. Your action will be sent to checker for approval.");
            showMainPage();
            
        },
        error: function (error) {
            alert("Error occurs: Add New User Access");
            $('#btnAddAccess').attr('disabled', false);
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

$('#btnCancel').click(function () {
    showMainPage();
});

$(document).ready(function () {
    LoadTaskList();
});