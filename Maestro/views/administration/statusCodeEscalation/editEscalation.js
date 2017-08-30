/*
    Some backend function is calling from addEscalation.aspx CODE BEHIND
    Due to poor initial planning,
       - Most of the function from ADD and EDIT are the same
       - Instead of reuse them, duplicate the function (due to poor planning)
       - Any changes in one file must apply to another one (addEscalation.js & editEscalation.js)
*/

var StatusCodeArray = [];
var EmailUserArray = [];
var PhoneUserArray = [];

var OldStatusCodeArray = [];
var OldEmailUserArray = [];
var OldPhoneUserArray = [];

var StatusCodeTypeOption = [];

loadEscalation();

function loadEscalation() {
    var GroupId = $('#groupIdHidden').val();

    $.ajax({
        type: "POST",
        url: "/views/administration/statusCodeEscalation/editEscalation.aspx/GetEscalation",
        contentType: "application/json; charset=utf-8",
        data: JSON.stringify({ GroupId: GroupId }),
        dataType: "json",
        success: function (data) {
            var status = data.d.Status;
            var msg = data.d.Message;

            if (!status) {
                alert(msg);
                return false;
            }

            var Escalation = data.d.Object;
            var CodeList = Escalation.CodeList;
            var UserList = Escalation.UserList;

            var CodeListString = "";
            var UserListString = "";

            for (var i = 0; i < CodeList.length, Code = CodeList[i]; i++) {
                CodeListString = CodeListString + '<div class="item-listing" data-code="' + Code.Code + '" data-type="Code">'
                           + '<span>' + Code.Code + '</span><span class="remove-btn"><i class="fa fa-times"></i></span></div>';

                OldStatusCodeArray.push(Code.Code);
            }
            console.log(UserList);
            for (var j = 0; j < UserList.length, User = UserList[j]; j++) {
                var UserId = User.UserId;
                var UserName = User.UserName;
                var EmailAddress = (User.UserEmail !== null) ? "&#060;" + User.UserEmail + "&#062;" : "";
                var PhoneNo = (User.UserPhone !== null) ? "&#060;" + User.UserPhone + "&#062;" : "";
                var UserDesc = UserName + " " + EmailAddress + " " + PhoneNo;

                var classes = "";
                if (User.UserEmail !== null) {
                    classes = classes + " email-listing";

                    OldEmailUserArray.push(UserId);
                }
                if (User.UserPhone !== null) {
                    classes = classes + " sms-listing";

                    OldPhoneUserArray.push(UserId);
                }

                UserListString = UserListString + '<div class="item-listing ' + classes + '" data-uid="' + UserId + '" data-type="User">'
                           + '<span>' + UserDesc + '</span><span class="remove-btn"><i class="fa fa-times"></i></span></div>';
            }

            $('#StatusCodeListing').html(CodeListString);
            $('#UserListing').html(UserListString);

            UpdateSelectedCodeArray();
            UpdateSelectedUserArray();
        },
        error: function (error) {
            alert("Error " + error.status + ". Unable to load escalation detail.");
        }
    });
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

    var machineType = $('#selectKioskType').text();
    loadStatusCode(machineType);
}
function UpdateSelectedStatusCodeType() {
    var SelectedOption = document.getElementById("statusCodesMainDiv").querySelectorAll('.filterOption.selected');

    StatusCodeTypeOption = [];

    for (var i = 0; i < SelectedOption.length, Option = SelectedOption[i]; i++) {
        StatusCodeTypeOption.push(Option.innerText);
    }
}

$('#editEscalationMainDiv').off('click', '.remove-btn', removeItem).on('click', '.remove-btn', removeItem);
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
        var machineType = $('#selectKioskType').text();
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

//If OLD is not in NEW = DELETE
//If NEW is not in OLD = ADD
function updateEscalation() {
    /*
    var AddCode = [];
    var AddEmail = [];
    var AddPhone = [];
    var DeleteCode = [];
    var DeleteEmail = [];
    var DeletePhone = [];

    var i = 0;

    for (i = 0; i < OldStatusCodeArray.length; i++) {
        if ($.inArray(OldStatusCodeArray[i], StatusCodeArray) === -1) {
            DeleteCode.push(OldStatusCodeArray[i]);
        }
    }
    for (i = 0; i < StatusCodeArray.length; i++) {
        if ($.inArray(StatusCodeArray[i], OldStatusCodeArray) === -1) {
            AddCode.push(StatusCodeArray[i]);
        }
    }
    for (i = 0; i < OldEmailUserArray.length; i++) {
        if ($.inArray(OldEmailUserArray[i], EmailUserArray) === -1) {
            DeleteEmail.push(OldEmailUserArray[i]);
        }
    }
    for (i = 0; i < EmailUserArray.length; i++) {
        if ($.inArray(EmailUserArray[i], OldEmailUserArray) === -1) {
            AddEmail.push(EmailUserArray[i]);
        }
    }
    for (i = 0; i < OldPhoneUserArray.length; i++) {
        if ($.inArray(OldPhoneUserArray[i], PhoneUserArray) === -1) {
            DeletePhone.push(OldPhoneUserArray[i]);
        }
    }
    for (i = 0; i < PhoneUserArray.length; i++) {
        if ($.inArray(PhoneUserArray[i], OldPhoneUserArray) === -1) {
            AddPhone.push(PhoneUserArray[i]);
        }
    }

    console.log("Old Status Code : " + OldStatusCodeArray);
    console.log("New Status Code : " + StatusCodeArray);
    console.log("Add Status : " + AddCode + " | Delete Code : " + DeleteCode);
    console.log("");
    console.log("Old Email User : " + OldEmailUserArray);
    console.log("New Email User : " + EmailUserArray);
    console.log("Add Email : " + AddEmail + " | Delete Email : " + DeleteEmail);
    console.log("");
    console.log("Old Phone User : " + OldPhoneUserArray);
    console.log("New Phone User : " + PhoneUserArray);
    console.log("Add Phone : " + AddPhone + " | Delete Phone : " + DeletePhone);
    */

    if (StatusCodeArray.length === 0) {
        alert("Please add status code.");
        return false;
    }

    if (EmailUserArray.length === 0 && PhoneUserArray.length === 0) {
        alert("Please add user into the list.");
        return false;
    }

    $('.addEscalationBtn').prop('disabled', true);

    var KioskType = $('#selectKioskType').text();
    var GroupId = $('#groupIdHidden').val();

    $.ajax({
        type: "POST",
        url: "/views/administration/statusCodeEscalation/editEscalation.aspx/UpdateEscalation",
        contentType: "application/json; charset=utf-8",
        data: JSON.stringify({ KioskType: KioskType, GroupId: GroupId, NewCodeList: StatusCodeArray, NewEmailList: EmailUserArray, NewPhoneList: PhoneUserArray }),
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
            alert("Error " + error.status + ". Unable to update new escalation.");
        }
    }).always(function () {
        $('.addEscalationBtn').prop('disabled', false);
    });
}