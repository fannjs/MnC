$('#inputStartDate').focus(function () {
    $(this).datepicker('setEndDate', $('#inputEndDate').val());
});
$('#inputEndDate').focus(function () {
    $(this).datepicker('setStartDate', $('#inputStartDate').val());
});

$(document).ready(function () {
    loadSelectMachineDiv();
    loadLogList();

    $('.datepickerInput').datepicker({
        format: 'dd/mm/yyyy',
        autoclose: true
    });
});
function loadSelectMachineDiv() {
    $.ajax({
        type: "POST",
        contentType: "application/json; charset=utf-8",
        url: "/views/kioskManagement/logRetrieval/main.aspx/GetAllMachineList",
        data: "{}",
        dataType: "json",
        beforeSend: function () {
            $('#selectMachineDiv').html("Loading...");
        },
        success: function (data) {

            var status = data.d.Status;
            var msg = data.d.Message;
            var MachineList = data.d.Object;

            if (!status) {
                alert(msg);
                return false;
            }

            if (MachineList.length === 0) {
                $('#selectMachineDiv').html("No kiosk");
            } else {
                var machines = "";

                for (var i = 0; i < MachineList.length; i++) {
                    machines = machines + '<div class="a-machine" data-id="' + MachineList[i].M_MACH_ID + '" branch-code="' + MachineList[i].M_BRANCH_CODE + '"' +
                                        ' branch-name="' + MachineList[i].M_BRANCH_NAME + '" address1="' + MachineList[i].M_ADDRESS1 + '" address2="' + MachineList[i].M_ADDRESS2 + '">' + MachineList[i].M_MACH_ID + '</div>';
                }

                $('#selectMachineDiv').html(machines);
            }
        },
        error: function (error) {
            alert("Error " + error.status + ". Unable to load machine list.");
        }
    });
}

$('#selectMachineDiv').off('mouseover').on('mouseover', '.a-machine', function (e) {
    $('#machine-detail-tooltip').remove();
    var x = e.clientX;
    var y = e.clientY;

    var branchCode = $(this).attr('branch-code');
    var branchName = $(this).attr('branch-name');
    var address1 = $(this).attr('address1');
    var address2 = $(this).attr('address2');

    $('#selectMachineDiv').append('<div id="machine-detail-tooltip"><span class="machine-detail-branch-code">' + branchCode + '</span><span class="machine-detail-branch-name">' + branchName + '</span><br/>'
                                        + '<span class="machine-detail-address">' + address1 + '<br/>' + address2 + '</span></div>');
    $('#machine-detail-tooltip').css({ 'top': y, 'left': x });

}).mouseout(function () {
    $('#machine-detail-tooltip').remove();
});

$('#selectMachineDiv').off('click', '.a-machine').on('click', '.a-machine', selectMachine);
function selectMachine(event) {
    if ($(this).hasClass('selectedMachine')) {
        $(this).removeClass('selectedMachine');
    } else {
        $(this).addClass('selectedMachine');
    }
}

function addToList(elem) {

    if ($('.kioskLogCB:checked').length === 0) {
        alert("Please tick at least ONE.");
        return false;
    }
    if ($('#inputStartDate').val().trim() === "") {
        alert("Please enter start date.");
        $('#inputStartDate').focus();
        return false;
    }
    if ($('#inputEndDate').val().trim() === "") {
        alert("Please enter end date.");
        $('#inputEndDate').focus();
        return false;
    }
    if ($('#selectMachineDiv > .a-machine.selectedMachine').length === 0) {
        alert("Please select kiosk.");
        return false;
    }

    var eventLog = ($('#eventLogCB').prop('checked')) ? 1 : 0;
    var transLog = ($('#transactionLogCB').prop('checked')) ? 1 : 0;
    var fileTransLog = ($('#fileTransferLogCB').prop('checked')) ? 1 : 0;
    var startDate = $('#inputStartDate').val().trim();
    var endDate = $('#inputEndDate').val().trim();
    var MachineArr = [];

    $('#selectMachineDiv > .a-machine.selectedMachine').each(function () {
        MachineArr.push($(this).attr('data-id'));
    });

    $.ajax({
        type: "POST",
        contentType: "application/json; charset=utf-8",
        url: "/views/kioskManagement/logRetrieval/main.aspx/AddToList",
        data: JSON.stringify({ eventLog: eventLog, transLog: transLog, fileTransLog: fileTransLog,
            startDate: startDate, endDate: endDate, MachineArr: MachineArr
        }),
        dataType: "json",
        beforeSend: function () {
            $(elem).prop('disabled', true);
        },
        success: function (data) {
            var status = data.d.Status;
            var msg = data.d.Message;

            if (!status) {
                alert(msg);
                return false;
            }

            alert("Successful.");
            loadLogList();
            resetOptions();
        },
        error: function (error) {
            console.log(error);
        }
    }).always(function () {
        $(elem).prop('disabled', false);
    });
}

function resetOptions() {
    $('.kioskLogCB').prop('checked', false);
    $('.datepickerInput').val("");
    $('#selectMachineDiv > .a-machine.selectedMachine').removeClass('selectedMachine');
}

function loadLogList() {
    $.ajax({
        type: "POST",
        contentType: "application/json; charset=utf-8",
        url: "/views/kioskManagement/logRetrieval/main.aspx/GetLogList",
        data: "{}",
        dataType: "json",
        success: function (data) {
            var status = data.d.Status;
            var msg = data.d.Message;

            if (!status) {
                alert(msg);
                return false;
            }

            var str = "";
            var LogList = data.d.Object;

            if (LogList.length === 0) {
                str = str + "<tr><td colspan='6'>No Log<td></tr>";
            } else {
                for (var i = 0; i < LogList.length; i++) {
                    var startDate = moment(LogList[i].StartDate).format("DD/MM/YYYY");
                    var endDate = moment(LogList[i].EndDate).format("DD/MM/YYYY");
                    var eventLog = (LogList[i].EventLog) ? "<a onclick=\"viewEventLog('LogViewer'," + LogList[i].LogID + ",'" + LogList[i].MachID + "','" + startDate + "','" + endDate + "');\" title=\"View Event Logs\">View</a>" : "-";
                    var transLog = (LogList[i].TransLog) ? "<a title=\"View Transaction Logs\">View</a>" : "-";
                    var fileTransferLog = (LogList[i].FileTransferLog) ? "<a title=\"View File Transfer Logs\">View</a>" : "-";

                    str = str + "<tr><td>" + LogList[i].MachID + "</td><td>" + startDate + "</td><td>" + endDate + "</td>";
                    str = str + "<td>" + eventLog + "</td><td>" + transLog + "</td><td>" + fileTransferLog + "</td>";
                    str = str + "<td><a title=\"Download this Log\">Download</a> <span class='center-divider'></span> <a title=\"Remove this Log\" onclick=\"clearLog(" + LogList[i].LogID + ");\">Clear</a></tr>";
                }
            }

            $('#tblLogList > tbody').html(str);
        },
        error: function (error) {
            alert("Error " + error.status + ". Unable to load log.");
        }
    });
}

function clearLog(logID) {
    $.ajax({
        type: "POST",
        contentType: "application/json; charset=utf-8",
        url: "/views/kioskManagement/logRetrieval/main.aspx/clearLog",
        data: JSON.stringify({ logID: logID }),
        dataType: "json",
        success: function (data) {
            var status = data.d.Status;
            var msg = data.d.Message;

            if (!status) {
                alert(msg);
                return false;
            }

            loadLogList();
        },
        error: function (error) {
            alert("Error " + error.status + ". Unable to remove log.");
        }
    });
}

function clearAllLog() {

    var confirmed = confirm("Are you sure you want to Clear all logs?");

    if (confirmed) {
        $.ajax({
            type: "POST",
            contentType: "application/json; charset=utf-8",
            url: "/views/kioskManagement/logRetrieval/main.aspx/clearAllLogs",
            data: "{}",
            dataType: "json",
            success: function (data) {
                var status = data.d.Status;
                var msg = data.d.Message;
                
                if (!status) {
                    alert(msg);                   
                    return false;
                }

                loadLogList();
            },
            error: function (error) {
                alert("Error " + error.status + ". Unable to remove all log.");
            }
        });
    }
}

function viewEventLog(dataName,logID, machID, startDate, endDate) {
    var title = "Log Viewer - " + machID + " &#060;Event Log&#062;";
    openModal(title, dataName);

    var momentStart = moment(startDate, "DD-MM-YYYY");
    var momentEnd = moment(endDate, "DD-MM-YYYY");
    var diffs = momentEnd.diff(momentStart, 'days');
    var str = "";

    for (var i = 0; i < diffs + 1 ; i++) {
        var date = moment(momentStart).add(i, 'days').format("DD-MM-YYYY");

        str = str + '<li>';
        str = str + '<span>'+date+'</span>';
        str = str + '<span><a class="fn-btn">Click to View</a></span>';
        str = str + '</li>';
    }

    $('#date-selection-div').find('.custom-table-body > ul').html(str);
}

function openSendLogConfig() {
    var title = "Send Email - Logs";
    var dataName = "SendEmail";

    openModal(title, dataName);
}

//Modal Functions
function closeModal() {
    $('#modal-wrapper').hide();
    $('body').css('overflow', 'auto');
}
function openModal(title, dataName) {
    $('body').css('overflow', 'hidden');
    $('#modal-wrapper').find('.modal-title').html(title);
    $('#modal-wrapper').find('.sub-content').hide();
    $('#modal-wrapper').find('.sub-content[data-name="' + dataName + '"]').show();
    $('#modal-wrapper').fadeIn('fast');
}