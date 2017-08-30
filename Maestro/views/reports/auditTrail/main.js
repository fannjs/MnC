var RecordPerPage = 10;

function DatePicker_Function() {
    $('#inputStartDate').focus(function () {
        $(this).datepicker('setEndDate', $('#inputEndDate').val());
    });
    $('#inputEndDate').focus(function () {
        $(this).datepicker('setStartDate', $('#inputStartDate').val());
    });

    $('.datepickerInput').datepicker({
        format: 'dd/mm/yyyy',
        autoclose: true
    });
}

function GetModuleList() {
    $.ajax({
        type: "POST",
        contentType: "application/json; charset=utf-8",
        url: "/views/reports/auditTrail/main.aspx/GetModuleList",
        data: "{}",
        dataType: "json",
        beforeSend: function(){
            $('#selectModule').html('<option selected disabled>Loading...</option>');
        },
        success: function (data) {
            var status = data.d.Status;
            var msg = data.d.Message;

            if (!status) {
                alert(msg);
                return false;
            }

            var ModuleList = data.d.Object;
            var str = '<option value="0" selected>All</option>';

            for (var i = 0; i < ModuleList.length; i++) {
                str = str + '<option value="' + ModuleList[i].TaskId + '">' + ModuleList[i].TaskName + '</option>'
            }

            $('#selectModule').html(str);
        },
        error: function (error) {
            alert("Error " + error.status + ". Unable to load modules.");
        }
    });
}

function GetUserList() {
    $.ajax({
        type: "POST",
        contentType: "application/json; charset=utf-8",
        url: "/views/reports/auditTrail/main.aspx/GetUserList",
        data: "{}",
        dataType: "json",
        beforeSend: function () {
            $('#selectUser').html('<option selected disabled>Loading...</option>');
        },
        success: function (data) {
            var status = data.d.Status;
            var msg = data.d.Message;

            if (!status) {
                alert(msg);
                return false;
            }

            var UserList = data.d.Object;
            var str = '<option value="0" selected>All</option>';

            for (var i = 0; i < UserList.length; i++) {
                str = str + '<option value="' + UserList[i].UserId + '">' + UserList[i].UserName + '</option>'
            }

            $('#selectUser').html(str);
        },
        error: function (error) {
            alert("Error " + error.status + ". Unable to load users.");
        }
    });
}

function Initialize() {
    GetModuleList();
    GetUserList();
    DatePicker_Function();
}

$(document).ready(function () {
    Initialize();
});

/*
    0 - None
    1 - Action
    2 - Status
*/
function loadFilterOptions(elem) {
    var value = $(elem).val();

    if (value == 0) {
        $('#FilterOptionDiv').hide();
    } else if (value == 1) {
        GetActionList();
    } else if (value == 2) {
        GetStatusList();
    }
}

function GetActionList() {
    $.ajax({
        type: "POST",
        contentType: "application/json; charset=utf-8",
        url: "/views/reports/auditTrail/main.aspx/GetActionList",
        data: "{}",
        dataType: "json",
        beforeSend: function () {
            $('#FilterOptionDiv').html('Loading...').show();
        },
        success: function (data) {
            var status = data.d.Status;
            var msg = data.d.Message;

            if (!status) {
                alert(msg);
                return false;
            }

            var ActionList = data.d.Object;
            var str = "";

            if (ActionList.length === 0) {
                str = "Action list empty. Please contact your administrator.";
            } else {
                for (var i = 0; i < ActionList.length; i++) {
                    var customId = 'action' + ActionList[i].ActionDesc;
                    str = str + '<input type="checkbox" class="actionCB" id="' + customId + '" value="' + ActionList[i].ActionType + '" checked />';
                    str = str + '<label class="checkboxLabel" for="' + customId + '">' + ActionList[i].ActionDesc + '</label> &nbsp;&nbsp;';
                }
            }

            $('#FilterOptionDiv').html(str).show();
        },
        error: function (error) {
            alert("Error " + error.status + ". Unable to load action.");
        }
    });
}

function GetStatusList() {
    $.ajax({
        type: "POST",
        contentType: "application/json; charset=utf-8",
        url: "/views/reports/auditTrail/main.aspx/GetStatusList",
        data: "{}",
        dataType: "json",
        beforeSend: function () {
            $('#FilterOptionDiv').html('Loading...').show();
        },
        success: function (data) {
            var status = data.d.Status;
            var msg = data.d.Message;

            if (!status) {
                alert(msg);
                return false;
            }

            var StatusList = data.d.Object;
            var str = "";

            if (StatusList.length === 0) {
                str = "Status list empty. Please contact your administrator";
            } else {
                for (var i = 0; i < StatusList.length; i++) {
                    var customId = 'status' + StatusList[i].ApprovalDesc;
                    str = str + '<input type="checkbox" class="statusCB" id="' + customId + '" value="' + StatusList[i].ApprovalStatus + '" checked />';
                    str = str + '<label class="checkboxLabel" for="' + customId + '">' + StatusList[i].ApprovalDesc + '</label> &nbsp;&nbsp;';
                }
            }

            $('#FilterOptionDiv').html(str).show();
        },
        error: function (error) {
            alert("Error " + error.status + ". Unable to load status.");
        }
    });
}

function ResetAll() {
    $('#selectionDiv').find('input[type="text"]').val("");
    $('#selectionDiv').find('select > option:first-child').prop('selected', true).change();
}

function RetrieveAuditLog() {
    var StartDate = $('#inputStartDate').val().trim();
    var EndDate = $('#inputEndDate').val().trim();
    var FilterOption = $('#selectFilter').val();
    var ActionList = [];
    var StatusList = [];
    var Module = "";
    var User = "";

    if (StartDate === "") {
        alert("Please specify the start date.");
        $('#inputStartDate').focus();
        return false;
    }
    if (EndDate === "") {
        alert("Please specify the end date.");
        $('#inputEndDate').focus();
        return false;
    }

    if (FilterOption == 1) {
        //Checking 
        if ($('#FilterOptionDiv').find('.actionCB:checked').length === 0) {
            alert("Please tick at least ONE option.");
            return false;
        }

        $('#FilterOptionDiv').find('.actionCB:checked').each(function () {
            ActionList.push($(this).val());
        });
    } else if (FilterOption == 2) {
        //Checking
        if ($('#FilterOptionDiv').find('.statusCB:checked').length === 0) {
            alert("Please tick at least ONE option.");
            return false;
        }

        $('#FilterOptionDiv').find('.statusCB:checked').each(function () {
            StatusList.push($(this).val());
        });
    }
    if ($('#selectModule').val() != 0) {
        Module = $('#selectModule').val();
    }
    if ($('#selectUser').val() != 0) {
        User = $('#selectUser').val();
    }

    LoadAuditLogFromDB(StartDate, EndDate, ActionList, StatusList, Module, User, 1, RecordPerPage);
}

function LoadAuditLogFromDB(StartDate, EndDate, ActionList, StatusList, Module, User, CurrentPage, RecordPerPage) {
    $.ajax({
        type: "POST",
        contentType: "application/json; charset=utf-8",
        url: "/views/reports/auditTrail/main.aspx/GetAuditLog",
        data: JSON.stringify({ StartDate: StartDate, EndDate: EndDate, ActionList: ActionList, StatusList: StatusList, Module: Module, User: User, pageNumber: CurrentPage, recordPerPage: RecordPerPage }),
        dataType: "json",
        success: function (data) {
            var status = data.d.Status;
            var msg = data.d.Message;

            if (!status) {
                alert(msg);
                return false;
            }

            var MKCK_List = data.d.Object.List;
            var TotalRecord = data.d.Object.Count;
            var iCurrentPage = parseInt(CurrentPage);
            var iRecordPerPage = parseInt(RecordPerPage);
            TotalRecord = TotalRecord / RecordPerPage;
            var NumberOfPage = Math.ceil(TotalRecord);
            var pages = "";

            if (NumberOfPage > 1) {
                pages = pages + '<li id="first-page"><a data-paging="1">&laquo;</a></li>';

                if (NumberOfPage <= 10) {
                    for (var i = 1; i <= NumberOfPage; i++) {
                        if (i == iCurrentPage) {
                            pages = pages + '<li class="page-number active"><a data-paging="' + i + '">' + i + '</a></li>';
                        } else {
                            pages = pages + '<li class="page-number"><a data-paging="' + i + '">' + i + '</a></li>';
                        }
                    }
                } else if (iCurrentPage - 5 <= 1) {
                    for (var i = 1; i <= 10; i++) {
                        if (i == iCurrentPage) {
                            pages = pages + '<li class="page-number active"><a data-paging="' + i + '">' + i + '</a></li>';
                        } else {
                            pages = pages + '<li class="page-number"><a data-paging="' + i + '"">' + i + '</a></li>';
                        }
                    }
                } else if (iCurrentPage + 5 <= NumberOfPage) {
                    for (var i = iCurrentPage - 5; i <= (iCurrentPage + 5); i++) {
                        if (i == iCurrentPage) {
                            pages = pages + '<li class="page-number active"><a data-paging="' + i + '">' + i + '</a></li>';
                        } else {
                            pages = pages + '<li class="page-number"><a data-paging="' + i + '>' + i + '</a></li>';
                        }
                    }
                } else {
                    for (var i = 1 + (NumberOfPage - 10); i <= NumberOfPage; i++) {
                        if (i == iCurrentPage) {
                            pages = pages + '<li class="page-number active"><a data-paging="' + i + '">' + i + '</a></li>';
                        } else {
                            pages = pages + '<li class="page-number"><a data-paging="' + i + '">' + i + '</a></li>';
                        }
                    }
                }

                pages = pages + '<li id="last-page"><a data-paging="' + NumberOfPage + '"">&raquo;</a></li>';
            }

            var str = "";

            if (MKCK_List.length === 0) {
                str = str.concat('<tr class="disabled"><td colspan="7">No log</td></tr>');
            } else {
                for (var i = 0; i < MKCK_List.length; i++) {
                    var MKCK = MKCK_List[i];
                    var createdDate = moment(MKCK.CreatedDate).format("D/MM/YYYY h:mm A");

                    str = str.concat('<tr data-itemid="' + MKCK.ItemID + '" data-status="' + MKCK.MCStatus + '">');
                    str = str.concat('<td>' + createdDate + '</td>');
                    str = str.concat('<td>' + MKCK.TaskName + '</td>');
                    str = str.concat('<td>' + MKCK.Action + '</td>');
                    str = str.concat('<td>' + MKCK.MakerName + '</td>');
                    str = str.concat('<td>' + MKCK.MCStatus + '</td>');
                    str = str.concat('<td><a class="fn-btn" onclick="ViewLogDetail(' + MKCK.ItemID + ');">View Detail</a></td>');
                    str = str.concat('</tr>');
                }
            }

            if (pages == "") {
                $('#pagination').hide();
            } else {
                $('#pagination').html(pages).show();
            }

            $('#tblAuditLog > tbody').html(str.toString());
            $('#searchResultDiv').fadeIn('fast');

            //Store data in the search result div for pagination
            $('#searchResultDiv').data({
                StartDate: StartDate,
                EndDate: EndDate,
                ActionList: ActionList,
                StatusList: StatusList,
                Module: Module,
                User: User
            });
        },
        error: function (error) {
            alert("Error " + error.status + ". Unable to retrieve audit log.");
        }
    });
}

$('#pagination').off('click', 'li', ChangePage).on('click', 'li', ChangePage);
function ChangePage() {
    var PageNo = $(this).find('a').attr('data-paging');

    if ($(this).hasClass('active')) {
        return false;
    }

    var StartDate = $('#searchResultDiv').data('StartDate');
    var EndDate = $('#searchResultDiv').data('EndDate');
    var ActionList = $('#searchResultDiv').data('ActionList');
    var StatusList = $('#searchResultDiv').data('StatusList');
    var Module = $('#searchResultDiv').data('Module');
    var User = $('#searchResultDiv').data('User');

    LoadAuditLogFromDB(StartDate, EndDate, ActionList, StatusList, Module, User, PageNo, RecordPerPage);
}

function showTaskListLoader() {
    $('#task-list-modal-body-content').hide();
    $('#task-list-modal-loader').show();
}
function hideTaskListLoader() {
    $('#task-list-modal-loader').hide();
    $('#task-list-modal-body-content').fadeIn('fast');
}
function CloseModal() {
    $('#task-list-modal-wrapper').hide();
    $('#task-list-modal-bg').hide();
    $('body').css('overflow', 'auto');
}
function OpenModal() {
    $('body').css('overflow', 'hidden');
    $('#task-list-modal-wrapper').fadeIn('fast');
    $('#task-list-modal-bg').show();
}
function ViewLogDetail(ItemId) {
    $.ajax({
        type: "POST",
        url: "/views/reports/auditTrail/main.aspx/GetLogDetail",
        contentType: "application/json; charset=utf-8",
        data: JSON.stringify({ ItemId: ItemId }),
        dataType: "json",
        beforeSend: function () {
            showTaskListLoader();
            OpenModal();
        },
        success: function (data) {
            var status = data.d.Status;
            var msg = data.d.Message;

            if (!status) {
                alert(msg);
                return false;
            }

            hideTaskListLoader();

            var MKCK = data.d.Object;
            var createdDate = moment(MKCK.CreatedDate).format("D/MM/YYYY h:mm:ss A");
            var repliedDate = (MKCK.RepliedDate !== null) ? moment(MKCK.RepliedDate).format("D/MM/YYYY h:mm:ss A") : "-";
            var checkerName = (MKCK.CheckerName !== null) ? MKCK.CheckerName : "-";
            var remark = (MKCK.Remark !== null) ? MKCK.Remark : "-";
            var str1 = "";
            var str2 = "";

            $('#taskList-Module').html(MKCK.TaskName);
            $('#taskList-Action').html(MKCK.ActionDesc);
            $('#taskList-Maker').html(MKCK.MakerName);
            $('#taskList-Status').html(MKCK.StatusDesc);
            $('#taskList-CDate').html(createdDate);

            $('#old-new-data-div > table > tbody > tr > td').hide();

            switch (MKCK.Action) {
                case "A":
                    var NewData = $.parseJSON(MKCK.NewData);

                    str1 = str1.concat('<h4 style="color:#999";>Data</h4>');
                    str1 = str1.concat('<table class="form-table">');
                    for (var i in NewData) {
                        str1 = str1.concat('<tr>');
                        str1 = str1.concat('<td>' + i + '</td>');
                        str1 = str1.concat('<td>' + NewData[i] + '</td>');
                        str1 = str1.concat('</tr>');
                    }
                    str1 = str1.concat('</table>');

                    $('#old-new-data-div > table > tbody > tr > td:nth-child(1) > div').html(str1);
                    $('#old-new-data-div > table > tbody > tr > td:nth-child(1)').fadeIn('fast');

                    break;

                case "M":

                    var OldData = $.parseJSON(MKCK.OldData);
                    var NewData = $.parseJSON(MKCK.NewData);

                    str1 = str1.concat('<h4 style="color:#999";>Current</h4>');
                    str1 = str1.concat('<table class="form-table">');
                    for (var i in OldData) {
                        str1 = str1.concat('<tr>');
                        str1 = str1.concat('<td>' + i + '</td>');
                        str1 = str1.concat('<td>' + OldData[i] + '</td>');
                        str1 = str1.concat('</tr>');
                    }
                    str1 = str1.concat('</table>');

                    str2 = str2.concat('<h4 style="color:#999";>New*</h4>');
                    str2 = str2.concat('<table class="form-table">');
                    for (var i in NewData) {
                        str2 = str2.concat('<tr>');
                        str2 = str2.concat('<td>' + i + '</td>');
                        str2 = str2.concat('<td>' + NewData[i] + '</td>');
                        str2 = str2.concat('</tr>');
                    }
                    str2 = str2.concat('</table>');

                    $('#old-new-data-div > table > tbody > tr > td:nth-child(1) > div').html(str1);
                    $('#old-new-data-div > table > tbody > tr > td:nth-child(2) > div').html(str2);
                    $('#old-new-data-div > table > tbody > tr > td:nth-child(1)').fadeIn('fast');
                    $('#old-new-data-div > table > tbody > tr > td:nth-child(2)').fadeIn('fast');

                    break;

                case "D":

                    var OldData = $.parseJSON(MKCK.OldData);

                    str1 = str1.concat('<h4 style="color:#999";>Data to be Deleted</h4>');
                    str1 = str1.concat('<table class="form-table">');
                    for (var i in OldData) {
                        str1 = str1.concat('<tr>');
                        str1 = str1.concat('<td>' + i + '</td>');
                        str1 = str1.concat('<td>' + OldData[i] + '</td>');
                        str1 = str1.concat('</tr>');
                    }
                    str1 = str1.concat('</table>');

                    $('#old-new-data-div > table > tbody > tr > td:nth-child(1) > div').html(str1);
                    $('#old-new-data-div > table > tbody > tr > td:nth-child(1)').fadeIn('fast');

                    break;
            }

            var StatusDesc = "-";

            if (MKCK.MCStatus === "2" || MKCK.MCStatus === "5") {
                StatusDesc = "Approved";
            } else if (MKCK.MCStatus === "3" || MKCK.MCStatus === "4") {
                StatusDesc = "Rejected";                
                $('#taskList-Remark-Row').show();
            }

            $('#taskList-Approval').html(StatusDesc);
            $('#taskList-RDate').html(repliedDate);
            $('#taskList-Checker').html(checkerName);
            $('#taskList-Remark').html(remark);
        },
        error: function (error) {
            alert("Error " + error.status + ". Unable to load log detail.");
        }
    });
}