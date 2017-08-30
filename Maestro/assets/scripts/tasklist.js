var MC_Status = {
    1: "Pending",
    2: "Approved",
    3: "Rejected",
    4: "Noted",
    5: "Failed",
    6: "Deleted"
};

var MC_Action = {
    A: "Add",
    M: "Modify",
    D: "Delete"
};

var RecordPerPage = 10;

$(document).ready(function () {

    loadCheckerItem(1, RecordPerPage);

    $('#navigation-bar > ul > li').click(function (e) {

        if ($(this).hasClass('active')) {
            return false;
        }

        var target = $(this).data('href');
        var type = $(this).data('type');

        $('#navigation-bar > ul > li').removeClass('active');
        $(this).addClass('active');
        $('#task-list-main-content > div').hide();
        $(target).fadeIn('fast');
        loadItems(type);
    });

    $('#task-list-checker > table > tbody').off('click', 'tr', openCheckerItem).on('click', 'tr', openCheckerItem);
    $('#task-list-maker > table > tbody').off('click', 'tr', openMakerItem).on('click', 'tr', openMakerItem);
    $('input[name="taskList-Approval"]').change(toggleApproval);
});

function enableEnterRemark() {
    $('#taskList-Textarea-Remark').prop('disabled', false);
    $('#taskList-Textarea-Remark').focus();
}
function disableEnterRemark() {
    $('#taskList-Textarea-Remark').val("").prop('disabled', true);
}

function toggleApproval() {
    var value = $(this).val();

    if (value === "3") {
        enableEnterRemark();
    } else {
        disableEnterRemark();
    }

}

function loadItems(type) {
    if (type === 'C') {
        loadCheckerItem(1, RecordPerPage);
    }
    else if (type === 'M') {
        loadMakerItem(1, RecordPerPage);
    }
}

function loadCheckerItem(CurrentPage, RecordPerPage) {

    $.ajax({
        type: "POST",
        url: "/views/tasklist.aspx/GetCheckerTaskList",
        contentType: "application/json; charset=utf-8",
        data: JSON.stringify({ pageNumber: CurrentPage, recordPerPage: RecordPerPage }),
        dataType: "json",
        beforeSend: function () {
            $('#task-list-checker-table > tbody').html('<tr class="disabled"><td colspan="6">Loading...</td></tr>');
        },
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
                pages = pages + '<li id="first-page"><a data-paging="1" data-type="C">&laquo;</a></li>';

                if (NumberOfPage <= 10) {
                    for (var i = 1; i <= NumberOfPage; i++) {
                        if (i == iCurrentPage) {
                            pages = pages + '<li class="page-number active"><a data-paging="' + i + '" data-type="C">' + i + '</a></li>';
                        } else {
                            pages = pages + '<li class="page-number"><a data-paging="' + i + '" data-type="C">' + i + '</a></li>';
                        }
                    }
                } else if (iCurrentPage - 5 <= 1) {
                    for (var i = 1; i <= 10; i++) {
                        if (i == iCurrentPage) {
                            pages = pages + '<li class="page-number active"><a data-paging="' + i + '" data-type="C">' + i + '</a></li>';
                        } else {
                            pages = pages + '<li class="page-number"><a data-paging="' + i + '" data-type="C">' + i + '</a></li>';
                        }
                    }
                } else if (iCurrentPage + 5 <= NumberOfPage) {
                    for (var i = iCurrentPage - 5; i <= (iCurrentPage + 5); i++) {
                        if (i == iCurrentPage) {
                            pages = pages + '<li class="page-number active"><a data-paging="' + i + '" data-type="C">' + i + '</a></li>';
                        } else {
                            pages = pages + '<li class="page-number"><a data-paging="' + i + '" data-type="C">' + i + '</a></li>';
                        }
                    }
                } else {
                    for (var i = 1 + (NumberOfPage - 10); i <= NumberOfPage; i++) {
                        if (i == iCurrentPage) {
                            pages = pages + '<li class="page-number active"><a data-paging="' + i + '" data-type="C">' + i + '</a></li>';
                        } else {
                            pages = pages + '<li class="page-number"><a data-paging="' + i + '" data-type="C">' + i + '</a></li>';
                        }
                    }
                }

                pages = pages + '<li id="last-page"><a data-paging="' + NumberOfPage + '" data-type="C">&raquo;</a></li>';
            }

            var str = "";

            if (MKCK_List.length === 0) {
                str = str.concat('<tr class="disabled"><td colspan="6">No pending task</td></tr>');
            } else {
                for (var i = 0; i < MKCK_List.length; i++) {
                    var MKCK = MKCK_List[i];
                    var createdDate = moment(MKCK.CreatedDate).format("D/MM/YYYY h:mm A");
                    var actionDesc = MC_Action[MKCK.Action];
                    var statusDesc = MC_Status[MKCK.MCStatus];

                    str = str.concat('<tr data-itemid="' + MKCK.ItemID + '" data-status="' + MKCK.MCStatus + '">');
                    str = str.concat('<td>' + createdDate + '</td>');
                    str = str.concat('<td>' + MKCK.TaskName + '</td>');
                    str = str.concat('<td>' + actionDesc + '</td>');
                    str = str.concat('<td>' + MKCK.MakerName + '</td>');
                    str = str.concat('<td>' + statusDesc + '</td>');
                    str = str.concat('</tr>');
                }
            }

            if (pages == "") {
                $('#pagination').hide();
            } else {
                $('#pagination').html(pages).show();
            }

            $('#task-list-checker-table > tbody').html(str.toString());
        },
        error: function (error) {
            alert(error);
        }
    });
}

$('#pagination').off('click', 'li', ChangePage).on('click', 'li', ChangePage);
function ChangePage() {
    var PageNo = $(this).find('a').attr('data-paging');
    var Type = $(this).find('a').attr('data-type');

    if ($(this).hasClass('active')) {
        return false;
    }

    if (Type === 'C') {
        loadCheckerItem(parseInt(PageNo), RecordPerPage);
    } else if(Type === 'M') {
        loadMakerItem(parseInt(PageNo), RecordPerPage);
    }
}

function loadMakerItem(CurrentPage, RecordPerPage) {
    $.ajax({
        type: "POST",
        url: "/views/tasklist.aspx/GetMakerTaskList",
        contentType: "application/json; charset=utf-8",
        data: JSON.stringify({ pageNumber: CurrentPage, recordPerPage: RecordPerPage }),
        dataType: "json",
        beforeSend: function () {
            $('#task-list-maker-table > tbody').html('<tr class="disabled"><td colspan="6">Loading...</td></tr>');
        },
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
                pages = pages + '<li id="first-page"><a data-paging="1" data-type="M">&laquo;</a></li>';

                if (NumberOfPage <= 10) {
                    for (var i = 1; i <= NumberOfPage; i++) {
                        if (i == iCurrentPage) {
                            pages = pages + '<li class="page-number active"><a data-paging="' + i + '" data-type="M">' + i + '</a></li>';
                        } else {
                            pages = pages + '<li class="page-number"><a data-paging="' + i + '" data-type="M">' + i + '</a></li>';
                        }
                    }
                } else if (iCurrentPage - 5 <= 1) {
                    for (var i = 1; i <= 10; i++) {
                        if (i == iCurrentPage) {
                            pages = pages + '<li class="page-number active"><a data-paging="' + i + '" data-type="M">' + i + '</a></li>';
                        } else {
                            pages = pages + '<li class="page-number"><a data-paging="' + i + '" data-type="M">' + i + '</a></li>';
                        }
                    }
                } else if (iCurrentPage + 5 <= NumberOfPage) {
                    for (var i = iCurrentPage - 5; i <= (iCurrentPage + 5); i++) {
                        if (i == iCurrentPage) {
                            pages = pages + '<li class="page-number active"><a data-paging="' + i + '" data-type="M">' + i + '</a></li>';
                        } else {
                            pages = pages + '<li class="page-number"><a data-paging="' + i + '" data-type="M">' + i + '</a></li>';
                        }
                    }
                } else {
                    for (var i = 1 + (NumberOfPage - 10); i <= NumberOfPage; i++) {
                        if (i == iCurrentPage) {
                            pages = pages + '<li class="page-number active"><a data-paging="' + i + '" data-type="M">' + i + '</a></li>';
                        } else {
                            pages = pages + '<li class="page-number"><a data-paging="' + i + '" data-type="M">' + i + '</a></li>';
                        }
                    }
                }

                pages = pages + '<li id="last-page"><a data-paging="' + NumberOfPage + '" data-type="M">&raquo;</a></li>';
            }

            var str = "";

            if (MKCK_List.length === 0) {
                str = str.concat('<tr class="disabled"><td colspan="6">No action</td></tr>');
            } else {
                for (var i = 0; i < MKCK_List.length; i++) {
                    var MKCK = MKCK_List[i];
                    var actionDesc = MC_Action[MKCK.Action];
                    var statusDesc = MC_Status[MKCK.MCStatus];

                    var createdDate = moment(MKCK.CreatedDate).format("D/MM/YYYY h:mm A");
                    var repliedDate = (MKCK.RepliedDate !== null) ? moment(MKCK.RepliedDate).format("D/MM/YYYY h:mm:ss A") : "-";
                    var checkerName = (MKCK.CheckerName !== null) ? MKCK.CheckerName : "-";

                    str = str.concat('<tr data-itemid="' + MKCK.ItemID + '" data-status="' + MKCK.MCStatus + '">');
                    str = str.concat('<td>' + createdDate + '</td>');
                    str = str.concat('<td>' + MKCK.TaskName + '</td>');
                    str = str.concat('<td>' + actionDesc + '</td>');
                    str = str.concat('<td>' + checkerName + '</td>');
                    str = str.concat('<td>' + repliedDate + '</td>');
                    str = str.concat('<td>' + statusDesc + '</td>');
                    str = str.concat('</tr>');
                }
            }

            if (pages == "") {
                $('#pagination').hide();
            } else {
                $('#pagination').html(pages).show();
            }

            $('#task-list-maker-table > tbody').html(str.toString());
        },
        error: function (error) {
            alert(error);
        }
    });

}

function openCheckerItem(e) {
    var itemid = $(this).data('itemid');

    if ($(this).hasClass("disabled")) {
        return false;
    }

    openTaskListModal(itemid,"C");
}

function openMakerItem(e){
    var itemid = $(this).data('itemid');
   
    if ($(this).hasClass("disabled")) {
        return false;
    }

    openTaskListModal(itemid,"M");
}

function openTaskListModal(itemid, mkckType) {
    $('body').css('overflow', 'hidden');
    $('#task-list-modal-wrapper').fadeIn('fast');
    $('#task-list-modal-bg').show();
    $('#taskList-ItemID').val(itemid);

    $.ajax({
        type: "POST",
        url: "/views/tasklist.aspx/getMKCKDetail",
        contentType: "application/json; charset=utf-8",
        data: JSON.stringify({ itemId: itemid }),
        dataType: "json",
        beforeSend: function () {
            showTaskListLoader();
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
            var actionDesc = MC_Action[MKCK.Action];
            var statusDesc = MC_Status[MKCK.MCStatus];
            var checkerName = (MKCK.CheckerName !== null) ? MKCK.CheckerName : "-";
            var remark = (MKCK.Remark !== null) ? MKCK.Remark : "-";
            var str1 = "";
            var str2 = "";

            $('#taskList-Module').html(MKCK.TaskName);
            $('#taskList-Action').html(actionDesc);
            $('#taskList-Maker').html(MKCK.MakerName);
            $('#taskList-Status').html(statusDesc);
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

            if (mkckType === "C") {
                $('#taskList-Maker-Approval').hide();
                $('#taskList-Checker-Approval').show();

                $('.taskList-function-btn').hide();
                $('.taskList-function-btn').eq(0).show();
            } else {

                $('.taskList-function-btn').hide();
                $('#taskList-Remark-Row').hide();

                if (MKCK.MCStatus === "2") {
                    $('#taskList-Approval').html("Approved");
                } else if (MKCK.MCStatus === "3") {
                    $('#taskList-Approval').html("Rejected");
                    $('.taskList-function-btn').eq(1).show();
                    $('#taskList-Remark-Row').show();
                } else if (MKCK.MCStatus === "4") {
                    $('#taskList-Approval').html("Rejected");
                    $('#taskList-Remark-Row').show();
                } else if (MKCK.MCStatus === "5") {
                    $('#taskList-Approval').html("Approved");
                }
                $('#taskList-RDate').html(repliedDate);
                $('#taskList-Checker').html(checkerName);
                $('#taskList-Remark').html(remark);

                $('#taskList-Checker-Approval').hide();
                $('#taskList-Maker-Approval').show();

                if (MKCK.MCStatus === "1") {
                    $('#taskList-Maker-Approval').hide();
                    $('#taskList-Checker-Approval').hide();

                    //SHOW DELETE BUTTON
                    $('.taskList-function-btn').eq(2).show();
                }

            }
        },
        error: function (error) {
            alert(error);
        }
    });
}

function showTaskListLoader() {
    $('#task-list-modal-body-content').hide();
    $('#task-list-modal-loader').show();
}
function hideTaskListLoader() {
    $('#task-list-modal-loader').hide();
    $('#task-list-modal-body-content').fadeIn('fast');
}

function closeTaskListModal() {
    $('#task-list-modal-wrapper').hide();
    $('#task-list-modal-bg').hide();
    $('body').css('overflow', 'auto');
    resetTaskListModalInput();
}

function resetTaskListModalInput() {
    $('input[name="taskList-Approval"]').prop('checked', false);
    $('#taskList-Textarea-Remark').val("").prop('disabled',true);
}

function lockModalButton() {
    $('.taskList-btn').prop('disabled', true);
}

function unlockModalButton() {
    $('.taskList-btn').prop('disabled', false);
}

function confirmCheckerApproval(elem) {
    var itemId = $('#taskList-ItemID').val();
    var approval = $('input[name="taskList-Approval"]:checked').val();
    var remark = $('#taskList-Textarea-Remark').val().trim();
    var proceed = true;

    if (approval === undefined || approval === null || approval === "") {
        alert("Please provide approval for this Action.");
        return false;
    }

    if (approval === "3") {
        if (remark === "") {
            var confirmed = confirm("Are you sure you want to Proceed with no Remark?");

            if (confirmed) {
                proceed = true;
            } else {
                proceed = false;
            }
        }
    }

    if (!proceed) {
        return false;
    }
    
    $.ajax({
        type: "POST",
        url: "/views/tasklist.aspx/approvalForAction",
        contentType: "application/json; charset=utf-8",
        data: JSON.stringify({ itemId: itemId, approval: approval, remark: remark }),
        dataType: "json",
        beforeSend: function () {
            lockModalButton();
        },
        success: function (data) {
            var status = data.d.Status;
            var msg = data.d.Message;

            alert(msg);

            if (!status) {
                return false;
            }
            closeTaskListModal();
            loadCheckerItem(1, RecordPerPage);
        },
        error: function (error) {
            alert("Error " + error.status + "! Please try again.");
        }
    }).always(function () {
        unlockModalButton();
    });
}

function notifiedRejection() {
    var itemId = $('#taskList-ItemID').val();

    $.ajax({
        type: "POST",
        url: "/views/tasklist.aspx/notifiedRejection",
        contentType: "application/json; charset=utf-8",
        data: JSON.stringify({ itemId: itemId }),
        dataType: "json",
        beforeSend: function () {
            lockModalButton();
        },
        success: function (data) {
            var status = data.d.Status;
            var msg = data.d.Message;

            alert(msg);

            if (!status) {                
                return false;
            }
            closeTaskListModal();
            loadMakerItem(1, RecordPerPage);
        },
        error: function (error) {
            alert("Error " + error.status + "! Please try again.");
        }
    }).always(function () {
        unlockModalButton();
    });
}

function deleteAction() {
    var itemId = $('#taskList-ItemID').val();

    $.ajax({
        type: "POST",
        url: "/views/tasklist.aspx/deleteAction",
        contentType: "application/json; charset=utf-8",
        data: JSON.stringify({ itemId: itemId }),
        dataType: "json",
        beforeSend: function () {
            lockModalButton();
        },
        success: function (data) {
            var status = data.d.Status;
            var msg = data.d.Message;

            unlockModalButton();

            if (!status) {
                alert(msg);
                return false;
            }

            alert("Successful. Your action has been Deleted.");
            closeTaskListModal();
            loadMakerItem(1, RecordPerPage);
        },
        error: function (error) {
            alert("Error " + error.status + "! Please try again.");
        }
    })
}