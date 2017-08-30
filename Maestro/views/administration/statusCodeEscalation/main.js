$(document).ready(function () {
    loadStatusCodeEscalationList();
});

function loadStatusCodeEscalationList() {

    $.ajax({
        type: "POST",
        url: "/views/administration/statusCodeEscalation/main.aspx/GetEscalationList",
        contentType: "application/json; charset=utf-8",
        data: "{}",
        dataType: "json",
        beforeSend: function () {
            $('#tblStatusCodeEscalation > tbody').html(""); //Quick fix for AngularJS
            $('#tblStatusCodeEscalation > tbody').html('<tr><td colspan="4">Loading...</td></tr>');
        },
        success: function (data) {
            var status = data.d.Message;
            var msg = data.d.Message;

            if (!status) {
                alert(msg);
                return false;
            }

            var List = data.d.Object;

            if (List.length === 0) {
                $('#tblStatusCodeEscalation tbody').html('<tr><td colspan="4">No record was found.</td></tr>');
            } else {

                var str = "";

                for (var i = 0; i < List.length, SCEscalation = List[i]; i++) {
                    var GroupId = SCEscalation.GroupId;
                    var KioskType = SCEscalation.KioskType;
                    var CodeList = SCEscalation.CodeList;
                    var UserList = SCEscalation.UserList;

                    var CodeListString = "";

                    for (var a = 0; a < CodeList.length, MCode = CodeList[a]; a++) {
                        CodeListString = CodeListString + '<span class="escalation-status-code" data-code="' + MCode.Code + '" data-type="' + MCode.Type + '"'
                               + ' data-desc="' + MCode.Description + '" data-device="' + MCode.Device + '">' + MCode.Code + '</span>';

                        CodeListString = CodeListString + ", ";
                    }

                    CodeListString = CodeListString.trim();
                    CodeListString = CodeListString.substring(0, CodeListString.length - 1);

                    str = str + '<tr data-id="' + GroupId + '">';
                    str = str + '<td>' + KioskType + '</td>';
                    str = str + '<td>' + CodeListString + '</td>';

                    var Users = "";

                    for (var j = 0; j < UserList.length, User = UserList[j]; j++) {
                        var UserName = User.UserName;
                        var EmailAddress = (User.EmailAddress !== null) ? "&#060;" + User.EmailAddress + "&#062;" : "";
                        var PhoneNo = (User.PhoneNo !== null) ? "&#060;" + User.PhoneNo + "&#062;" : "";

                        var UserDesc = UserName + " " + EmailAddress + " " + PhoneNo;

                        Users = Users + '<div>' + UserDesc + '</div>';
                    }

                    str = str + '<td>' + Users + '</td>';
                    str = str + "<td><a href='javascript:;' access-gate task='" + $('#taskNameHidden').val() + "' permission='Edit' onclick=\"editEscalation('" + KioskType + "','" + GroupId + "')\"><i class='fa fa-pencil'></i>Edit</a>" +
                                        "<span class='center-divider'></span><a href='javascript:;' access-gate task='" + $('#taskNameHidden').val() + "' permission='Delete' onclick=\"deleteEscalation('" + KioskType + "','" + GroupId + "')\"><i class='fa fa-trash-o'></i>Delete</a></td>";
                }

                $('#tblStatusCodeEscalation tbody').html(str);
            }
        },
        error: function (error) {
            alert("Error " + error.status + ". Failed to retrieve Status Code Escalation. ");
        }
    });

}

$('#statusCodeEscalation_Main').off('mouseover', '.escalation-status-code', DisplayToolTips).on('mouseover', '.escalation-status-code', DisplayToolTips);
function DisplayToolTips(e) {
    $('#detailed-tooltip').remove();
    var x = e.clientX;
    var y = e.clientY;

    var Device = $(this).data('device');
    var Type = $(this).data('type');
    var Description = $(this).data('desc');

    $('#statusCodeEscalation_Main').append('<div id="detailed-tooltip"><span class="highlighted-text">' + Device + '</span><span class="highlighted-text">' + Type + '</span><br/>'
                                        + '<span class="description-text">' + Description + '</span></div>');
    $('#detailed-tooltip').css({ 'top': y, 'left': x });
}

$('#statusCodeEscalation_Main').off('mouseout', '.escalation-status-code', HideToolTips).on('mouseout', '.escalation-status-code', HideToolTips);
function HideToolTips() {
    $('#detailed-tooltip').remove();
}

function configureEmail() {
    var postData = {
        task: $('#taskNameHidden').val()
    };

    $.post("/views/administration/statusCodeEscalation/configureEmail.aspx", postData, function (data) {
        $("#content-subpage-container").html(data);
        showSubPage();
    });
}

function configureSMS() {
    var postData = {
        task: $('#taskNameHidden').val()
    };

    $.post("/views/administration/statusCodeEscalation/configureSMS.aspx", postData, function (data) {
        $("#content-subpage-container").html(data);
        showSubPage();
    });
}

function newEscalation() {
    var postData = {
        task: $('#taskNameHidden').val()
    };

    $.post("/views/administration/statusCodeEscalation/addEscalation.aspx", postData, function (data) {
        $("#content-subpage-container").html(data);
        showSubPage();
    });
}

function deleteEscalation(kioskType,groupId) {
    var confirmed = confirm("Are you sure you want to Delete?");

    if (confirmed) {
        $.ajax({
            type: "POST",
            url: "/views/administration/statusCodeEscalation/main.aspx/DeleteEscalation",
            contentType: "application/json; charset=utf-8",
            data: JSON.stringify({ kioskType: kioskType, groupId: groupId }),
            dataType: "json",
            success: function (data) {
                var status = data.d.Status;
                var msg = data.d.Message;

                if (!status) {
                    alert(msg);
                    return false;
                }

                alert("Successful. Your action has been sent to Checker for approval.");
            },
            error: function (error) {
                alert("Error " + error.status + ". Unable to delete.");
            }
        });
    }
}

function editEscalation(kioskType, groupId) {
    var postData = {
        task: $('#taskNameHidden').val(),
        KioskType: kioskType,
        GroupId: groupId
    };

    $.post("/views/administration/statusCodeEscalation/editEscalation.aspx", postData, function (data) {
        $("#content-subpage-container").html(data);
        showSubPage();
    });
}