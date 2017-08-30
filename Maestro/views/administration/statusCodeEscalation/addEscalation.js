var StatusCodeArray = []; 
var EmailUserArray = [];
var PhoneUserArray = [];
var KioskType = null;

var StatusCodeTypeOption = [];

function KT(elem) {
    KioskType = $(elem).find('option:selected');
}
function changeKioskType(elem) {

    if(StatusCodeArray.length !== 0){
        var confirmed = confirm("The selected status code will be reset. \n\nAre you sure you want to change Kiosk Type?");

        if(confirmed){
            $('#StatusCodeListing').empty();
            StatusCodeArray = [];
        }else{
            $(KioskType).prop('selected',true);
            return false;
        }
    }
}

function loadEscalationUsers() {
    $.ajax({
        type: "POST",
        url: "/views/administration/statusCodeEscalation/addEscalation.aspx/GetEscalationUserList",
        contentType: "application/json; charset=utf-8",
        data: "{}",
        dataType: "json",
        beforeSend: function () {
            $('#userListDiv').html("Loading...");
        },
        success: function (data) {

            var status = data.d.Status;
            var msg = data.d.Message;

            if (!status) {
                alert(msg);
                return false;
            }

            var EscalationUserList = data.d.Object;

            if (EscalationUserList.length === 0) {
                $('#userListDiv').html('No user was found. Please add some user.');
                $('#confirmSelectedUserBtn').hide();
            } else {
                var str = "";
                var type = $('#selectEscalationType').val();

                for (var i = 0; i < EscalationUserList.length, EU = EscalationUserList[i]; i++) {

                    str = str + '<div class="a-user';

                    if (type === '1') {
                        if ($.inArray(EU.UserId, PhoneUserArray) !== -1) {
                            str = str + ' selected disabled';
                        }
                    } else if (type === '2') {
                        if ($.inArray(EU.UserId, EmailUserArray) !== -1) {
                            str = str + ' selected disabled';
                        }
                    } else if (type === '3') {
                        if ($.inArray(EU.UserId, PhoneUserArray) !== -1 && $.inArray(EU.UserId, EmailUserArray) !== -1) {
                            str = str + ' selected disabled';
                        }
                    }

                    str = str + '" data-uid="' + EU.UserId + '" data-uname="' + EU.UserName + '" '
                                    + 'data-email="' + EU.EmailAddress + '" data-phone="' + EU.PhoneNo + '">' + EU.UserName + '</div>';

                }

                $('#userListDiv').html(str);
                $('#confirmSelectedUserBtn').show();
            }

        },
        error: function (error) {
            alert("Error " + error.status + ". Unable to load escalation user.");
        }
    });
}

function loadStatusCode(machineType) {

    UpdateSelectedStatusCodeType();

    $.ajax({
        type: "POST",
        url: "/views/administration/statusCodeEscalation/addEscalation.aspx/GetStatusCode",
        contentType: "application/json; charset=utf-8",
        data: JSON.stringify({ machineType: machineType, Option: StatusCodeTypeOption }),
        dataType: "json",
        beforeSend: function () {
            $('#statusCodesMainDiv').find('.filterOption').addClass('disabled');
            $('#statusCodesDiv').html("Loading...");
        },
        success: function (data) {

            var status = data.d.Status;
            var msg = data.d.Message;

            $('#statusCodesMainDiv').find('.filterOption').removeClass('disabled');

            if (!status) {
                alert(msg);
                return false;
            }

            var CodeList = data.d.Object;

            if (CodeList.length === 0) {
                $('#statusCodesDiv').html('No status code was found.');
                $('#confirmSelectedCode').hide();
            } else {
                var str = "";

                for (var i = 0; i < CodeList.length, MCode = CodeList[i]; i++) {

                    str = str + '<div class="a-code';

                    if ($.inArray(MCode.Code, StatusCodeArray) !== -1) {
                        str = str + ' selected disabled';
                    }
                    str = str + '" data-code="' + MCode.Code + '" data-type="' + MCode.Type + '"'
                               + ' data-desc="' + MCode.Description + '" data-device="' + MCode.Device + '">' + MCode.Code + '</div>';
                }

                $('#statusCodesDiv').html(str);
                $('#confirmSelectedCode').show();
            }

        },
        error: function (error) {
            alert("Error " + error.status + ". Unable to load escalation user.");
        }
    });
}

//MOUSEOVER HERE
$('#statusCodesDiv').off('mouseover', '.a-code', showTooltip).on('mouseover', '.a-code', showTooltip);
function showTooltip(e) {
    $('#detailed-tooltip').remove();
    var x = e.clientX;
    var y = e.clientY;

    var Device = $(this).data('device');
    var Type = $(this).data('type');
    var Description = $(this).data('desc');

    $('#modal-wrapper').append('<div id="detailed-tooltip"><span class="highlighted-text">' + Device + '</span><span class="highlighted-text">' + Type + '</span><br/>'
                                        + '<span class="description-text">' + Description + '</span></div>');
    $('#detailed-tooltip').css({ 'top': y, 'left': x });
}

$('#statusCodesDiv').off('mouseout', '.a-code', hideTooltip).on('mouseout', '.a-code', hideTooltip);
function hideTooltip() {
    $('#detailed-tooltip').remove();
}

$('#statusCodesMainDiv').off('click', '.filterOption', selectErrorType).on('click', '.filterOption', selectErrorType);
function selectErrorType() {

    if ($(this).hasClass('disabled')) {
        return false;
    }

    if ($(this).hasClass('selected')) {
        $(this).removeClass('selected');
    } else {
        $(this).addClass('selected');
    }

    var machineType = $('#selectKioskType').val();
    loadStatusCode(machineType);
}
function UpdateSelectedStatusCodeType() {
    var SelectedOption = document.getElementById("statusCodesMainDiv").querySelectorAll('.filterOption.selected');

    StatusCodeTypeOption = [];

    for (var i = 0; i < SelectedOption.length, Option = SelectedOption[i]; i++) {
        StatusCodeTypeOption.push(Option.innerText);
    }
}

$('#addNewEscalationMainDiv').off('click', '.remove-btn', removeItem).on('click', '.remove-btn', removeItem);
function removeItem() {
    $(this).closest('.item-listing').fadeOut('fast', function () {
        $(this).remove();
        UpdateSelectedCodeArray();
        UpdateSelectedUserArray();
    });
}

function openModal(elem) {
    var target = $(elem).data('target');
    var title = $(elem).data('title');

    if (target === "UserList") {
        var eType = $('#selectEscalationType').val();
        var eDesc = $('#selectEscalationType option:selected').text();
        title = title + " (" + eDesc + ")";

        if (eType === "" || eType === null) {
            alert("Please select Escalation Type to continue.")
            $('#selectEscalationType').focus();
            return false;
        }
        loadEscalationUsers();

    } else if (target === "StatusCode") {

        var machineType = $('#selectKioskType').val();

        if (machineType === "" || machineType === null) {
            alert("Please select Kiosk Type to continue.")
            $('#selectKioskType').focus();
            return false;
        }
        loadStatusCode(machineType);
    }

    $('#modal-wrapper .modal-dialog .modal-title').html(title);

    $('#modal-content > div').hide();
    $('#modal-content').find('div[data-role = "' + target + '"]').show();
    $('#modal-wrapper').fadeIn('fast');
    $('body').css('overflow', 'hidden');
}

function closeModal() {
    $('#modal-wrapper').hide();
    $('body').css('overflow', 'auto');
}

function addNewEscalationUser() {

    var requiredFields = document.getElementById("userListMainDiv").querySelectorAll(".required-field");

    $('.addEscalationUserBtn').prop('disabled', true);
    for (var i = 0; i < requiredFields.length; i++) {
        var value = requiredFields[i].value.trim();

        if (value === "" || value === null || value === undefined) {
            alert("Please do not leave blank.");
            requiredFields[i].focus();

            $('.addEscalationUserBtn').prop('disabled', false);
            return false;
        }
    }

    // < Specific checking perform here >

    var EscalationUser = {
        UserName: $('#userListMainDiv input[name="userName"]').val().trim(),
        PhoneNo: $('#userListMainDiv input[name="phoneNo"]').val().trim(),
        EmailAddress: $('#userListMainDiv input[name="emailAddress"]').val().trim(),
        Company: $('#userListMainDiv input[name="companyName"]').val().trim()
    };

    $.ajax({
        type: "POST",
        url: "/views/administration/statusCodeEscalation/addEscalation.aspx/AddEscalationUser",
        contentType: "application/json; charset=utf-8",
        data: JSON.stringify({ EscalationUser: EscalationUser }),
        dataType: "json",
        success: function (data) {

            var status = data.d.Status;
            var msg = data.d.Message;

            if (!status) {
                alert(msg);
                return false;
            }

            alert("Successful.");
            loadEscalationUsers();
            resetTextField();
            //alert("Successful. Your action has been sent to Checker for approval.");
            //showMainPage();
        },
        error: function (error) {
            alert("Error " + error.status + ". Unable to add escalation user.");
        }
    }).always(function () {
        $('.addEscalationUserBtn').prop('disabled', false);
    });
}

function resetTextField() {
    var requiredFields = document.getElementById("userListMainDiv").querySelectorAll(".required-field");

    for (var i = 0; i < requiredFields.length; i++) {
        requiredFields[i].value = "";
    }
}

$('#userListDiv').off('click', '.a-user', selectItem).on('click', '.a-user', selectItem);
$('#statusCodesDiv').off('click', '.a-code', selectItem).on('click', '.a-code', selectItem);

function selectItem() {

    if ($(this).hasClass('disabled')) {
        return false;
    }

    if ($(this).hasClass('selected')) {
        $(this).removeClass('selected');
    } else {
        $(this).addClass('selected');
    }
}

function confirmSelectedCode() {

    var selectedCodes = document.getElementById("statusCodesDiv").querySelectorAll('.a-code.selected:not(.disabled)');
    var str = "";

    for (var i = 0; i < selectedCodes.length, MCode = selectedCodes[i]; i++) {
        var code = MCode.getAttribute('data-code');

        str = str + '<div class="item-listing" data-code="' + code + '">'
                           + '<span>' + code + '</span><span class="remove-btn"><i class="fa fa-times"></i></span></div>';
    }

    $('#StatusCodeListing').append(str);

    closeModal();
    UpdateSelectedCodeArray();
}

/*
* Add code to array for checking later 
*/
function UpdateSelectedCodeArray() {
    var StatusCodeListing = document.getElementById("StatusCodeListing").querySelectorAll(".item-listing");

    StatusCodeArray = [];

    if (StatusCodeListing.length !== 0) {

        for (var i = 0; i < StatusCodeListing.length, StatusCode = StatusCodeListing[i]; i++) {
            var code = StatusCode.getAttribute('data-code');
            StatusCodeArray.push(code);
        }
    }
}

function confirmSelectedUsers() {

    var selectedUsers = document.getElementById("userListDiv").querySelectorAll('.a-user.selected:not(.disabled)');
    var type = $('#selectEscalationType').val();
    var str = "";

    if (type === '1') {

        for (var i = 0; i < selectedUsers.length, User = selectedUsers[i]; i++) {
            var uid = User.getAttribute('data-uid');
            var uname = User.getAttribute('data-uname');
            var phone = User.getAttribute('data-phone');
            var desc = uname + " &#060;" + phone + "&#062;";

            if ($.inArray(uid, EmailUserArray) !== -1) {
                var email = User.getAttribute('data-email');
                desc = uname + " &#060;" + email + "&#062;" + " &#060;" + phone + "&#062;";

                $('#UserListing').find('.email-listing[data-uid="' + uid + '"]').addClass('sms-listing').find('span:eq(0)').html(desc);

            } else {
                str = str + '<div class="item-listing sms-listing" data-uid="' + uid + '">'
                           + '<span>' + desc + '</span><span class="remove-btn"><i class="fa fa-times"></i></span></div>';
            }
        }


    } else if (type === '2') {

        for (var i = 0; i < selectedUsers.length, User = selectedUsers[i]; i++) {
            var uid = User.getAttribute('data-uid');
            var uname = User.getAttribute('data-uname');
            var email = User.getAttribute('data-email');
            var desc = uname + " &#060;" + email + "&#062;";

            if ($.inArray(uid, PhoneUserArray) !== -1) {
                var phone = User.getAttribute('data-phone');
                desc = uname + " &#060;" + email + "&#062;" + " &#060;" + phone + "&#062;";

                $('#UserListing').find('.sms-listing[data-uid="' + uid + '"]').addClass('email-listing').find('span:eq(0)').html(desc);

            } else {
                str = str + '<div class="item-listing email-listing" data-uid="' + uid + '">'
                           + '<span>' + desc + '</span><span class="remove-btn"><i class="fa fa-times"></i></span></div>';
            }
        }
    } else if (type === '3') {

        for (var i = 0; i < selectedUsers.length, User = selectedUsers[i]; i++) {
            var uid = User.getAttribute('data-uid');
            var uname = User.getAttribute('data-uname');
            var email = User.getAttribute('data-email');
            var phone = User.getAttribute('data-phone');
            var desc = uname + " &#060;" + email + "&#062;" + " &#060;" + phone + "&#062;";

            if ($.inArray(uid, EmailUserArray) !== -1) {
                $('#UserListing').find('.email-listing[data-uid="' + uid + '"]').addClass('sms-listing').find('span:eq(0)').html(desc);
            } else if ($.inArray(uid, PhoneUserArray) !== -1) {
                $('#UserListing').find('.sms-listing[data-uid="' + uid + '"]').addClass('email-listing').find('span:eq(0)').html(desc);
            } else {
                str = str + '<div class="item-listing sms-listing email-listing" data-uid="' + uid + '">'
                           + '<span>' + desc + '</span><span class="remove-btn"><i class="fa fa-times"></i></span></div>';
            }
        }
    }

    $('#UserListing').append(str);
    closeModal();
    UpdateSelectedUserArray();
}

/*
* Add userID to array for checking later 
*/
function UpdateSelectedUserArray() {
    var EmailListingUsers = document.getElementById("UserListing").querySelectorAll(".email-listing");
    var PhoneListingUsers = document.getElementById("UserListing").querySelectorAll(".sms-listing");

    EmailUserArray = [];
    PhoneUserArray = [];

    if (EmailListingUsers.length !== 0) {

        for (var i = 0; i < EmailListingUsers.length, EmailUser = EmailListingUsers[i]; i++) {
            var uid = EmailUser.getAttribute('data-uid');
            EmailUserArray.push(uid);
        }
    }

    if (PhoneListingUsers.length !== 0) {
        for (var i = 0; i < PhoneListingUsers.length, PhoneUser = PhoneListingUsers[i]; i++) {
            var uid = PhoneUser.getAttribute('data-uid');
            PhoneUserArray.push(uid);
        }
    }
}


function addNewEscalation() {
    if (StatusCodeArray.length === 0) {
        alert("Please add status code.");
        return false;
    }

    if (EmailUserArray.length === 0 && PhoneUserArray.length === 0) {
        alert("Please add user into the list.");
        return false;
    }

    $('.addEscalationBtn').prop('disabled', true);

    var MachineType = $('#selectKioskType').val();

    $.ajax({
        type: "POST",
        url: "/views/administration/statusCodeEscalation/addEscalation.aspx/AddNewEscalation",
        contentType: "application/json; charset=utf-8",
        data: JSON.stringify({ MachineType: MachineType, StatusCodeArray: StatusCodeArray, EmailUserArray: EmailUserArray, PhoneUserArray: PhoneUserArray }),
        dataType: "json",
        success: function (data) {

            var status = data.d.Status;
            var msg = data.d.Message;

            if (!status) {
                alert(msg);
                return false;
            }

            alert("Successful. Your action has been sent to Checker for approval.");
            showMainPage();
        },
        error: function (error) {
            alert("Error " + error.status + ". Unable to add new escalation.");
        }
    }).always(function () {
        $('.addEscalationBtn').prop('disabled', false);
    });
}