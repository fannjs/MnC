$(document).ready(function () {
    GetSoftwareUpdateType();
    Navigation_Fn();
    GetSoftwareList();


    /****************
    ** Initialize Datetime Picker 
    **********************/
    $('.input-datepicker').keypress(function (e) {
        e.preventDefault();
    });
    $('.input-datepicker').focus(function (e) {
        var target = $(this).attr('data-target');

        if ($(target).val() === null) {
            alert("Please select update type.");
            $(target).focus();
            return false;
        }
    });
});

function GetSoftwareUpdateType() {
    $.ajax({
        type: "POST",
        url: "/views/kioskMaintenance/softwareDistribution/main.aspx/GetSoftwareUpdateType",
        data: "{}",
        dataType: "json",
        contentType: "application/json; charset=utf-8",
        success: function (data) {
            var status = data.d.Status;
            var msg = data.d.Message;

            if (!status) {
                alert(msg);
                return false;
            }

            var TypeList = data.d.Object;
            var str = '<option selected disabled value="0"> - Please Select - </option>';

            for (var i = 0; i < TypeList.length; i++) {
                str = str + '<option value="' + TypeList[i].SU_ID + '">' + TypeList[i].SU_NAME + '</option>';
            }

            $('#selectDownloadSUID,#selectDeploySUID,#editDownloadSUID,#editDeploySUID').html(str);
        },
        error: function (error) {
            alert('Error ' + error.status);
        }
    });
}

function Navigation_Fn() {
    $('#navigation-bar > ul > li').click(function (e) {

        if ($(this).hasClass('active')) {
            return false;
        }

        var target = $(this).data('href');
        var type = $(this).data('type');

        $('#navigation-bar > ul > li').removeClass('active');
        $(this).addClass('active');

        $('#software-distribution-main-content > div').hide();
        $(target).fadeIn('fast');
        loadItems(type);
    });
}

function loadItems(type) {
    switch (type) {
        case 'S': GetSoftwareList()
            break;
        case 'Q': GetQueueList()
            break;
    }
}

function GetSoftwareList() {
    $.ajax({
        type: "POST",
        url: "/views/kioskMaintenance/softwareDistribution/main.aspx/GetSoftwareList",
        data: "{}",
        dataType: "json",
        contentType: "application/json; charset=utf-8",
        beforeSend: function () {
            $('#softwareListTbl > tbody').html('<tr><td colspan="5">Loading...</td></tr>');
        },
        success: function (data) {
            var status = data.d.Status;
            var msg = data.d.Message;

            if (!status) {
                alert(msg);
                return false;
            }

            var SoftwareList = data.d.Object;
            var str = "";

            if (SoftwareList.length === 0) {
                str = str + '<tr><td colspan="5">No Software Version</td></tr>';
            } else {
                for (var i = 0; i < SoftwareList.length, Software = SoftwareList[i]; i++) {
                    str = str + '<tr>';
                    str = str + '<td>' + (i+1) + '</td>';
                    str = str + '<td>' + Software.VersionName + '</td>';
                    str = str + '<td>' + Software.FileName + '</td>';
                    str = str + '<td>' + Software.Checksum + '</td>';
                    str = str + '<td><a class="fn-btn" onclick="removeSoftwareVersion(\'' + Software.VersionId + '\')"><i class="fa fa-trash-o"></i>Delete</a></td>';
                    str = str + '</tr>';
                }
            }

            $('#softwareListTbl > tbody').html(str);
        },
        error: function (error) {
            alert("Error " + error.status);
        }
    });
}

function GetQueueList() {
    $.ajax({
        type: "POST",
        url: "/views/kioskMaintenance/softwareDistribution/main.aspx/GetQueueList",
        data: "{}",
        dataType: "json",
        contentType: "application/json; charset=utf-8",
        beforeSend: function () {
            $('#queueList > tbody').html('<tr><td colspan="7">Loading...</td></tr>');
        },
        success: function (data) {
            var status = data.d.Status;
            var msg = data.d.Message;

            if (!status) {
                alert(msg);
                return false;
            }

            var QueueList = data.d.Object;
            var str = "";

            if (QueueList.length === 0) {
                str = str + '<tr><td colspan="7">Nothing in the queue</td></tr>';
            } else {
                for (var i = 0; i < QueueList.length, Queue = QueueList[i]; i++) {
                    var DownloadDT = "";
                    var DownloadedDT = (Queue.DownloadedTime == null) ? "-" : moment(Queue.DownloadedTime).format("DD/MM/YYYY h:mm A");
                    var DeployDT = "";

                    if (Queue.DownloadSUID == 1) {
                        DownloadDT = moment(Queue.DownloadTime).format("DD/MM/YYYY");
                        DownloadDT = DownloadDT + " (After CUTOFF)";
                    } else {
                        DownloadDT = moment(Queue.DownloadTime).format("DD/MM/YYYY h:mm A");
                    }

                    if (Queue.DeploySUID == 1) {
                        DeployDT = moment(Queue.DeployTime).format("DD/MM/YYYY");
                        DeployDT = DeployDT + " (After CUTOFF)";
                    } else {
                        DeployDT = moment(Queue.DeployTime).format("DD/MM/YYYY h:mm A");
                    }

                    str = str + '<tr>';
                    str = str + '<td>' + (i + 1) + '</td>';
                    str = str + '<td>' + Queue.KioskId + '</td>';
                    str = str + '<td>' + Queue.VersionName + '</td>';
                    str = str + '<td>' + DownloadDT + '</td>';
                    str = str + '<td>' + DownloadedDT + '</td>';
                    str = str + '<td>' + DeployDT + '</td>';
                    str = str + '<td><a class="fn-btn" onclick="openModal(this);editSoftwareQueue(\'' + Queue.KioskId + '\',\'' + Queue.VersionId + '\')" data-target=".edit-queue" data-title="Edit Software Queue"><i class="fa fa-pencil"></i>Edit</a>';
                    str = str + '<span class="center-divider"></span>';
                    str = str + '<a class="fn-btn" onclick="removeSoftwareQueue(\'' + Queue.KioskId + '\',\'' + Queue.VersionId + '\',\'' + Queue.VersionName + '\')"><i class="fa fa-trash-o"></i>Delete</a></td>';
                    str = str + '</tr>';
                }
            }

            $('#queueList > tbody').html(str);
        },
        error: function (error) {
            alert("Error " + error.status);
        }
    });
}

function clearAllInput() {
    $('.input-datepicker').datepicker('remove').datetimepicker('remove');
    $('#software-distribution-modal .input-field').val("").prop('disabled',false);
    $('#software-distribution-modal select > option:first-child').prop('selected', true);
}

function openModal(elem) {
    var title = $(elem).attr('data-title');
    var target = $(elem).attr('data-target');

    clearAllInput();
    $('#modal-title').html(title);
    $('#software-distribution-modal .modal-body > div').hide();
    $('#modal-footer .functional-btn').hide();
    $(target).show();
    $('#modal-footer button').prop('disabled', false);
    $('body').scrollTop(0);  //Scroll to most TOP because of the datepicker plugin position problem

    $('#software-distribution-modal').modal('show');
}

function closeModal() {
    $('#software-distribution-modal').modal('hide');
}

function ShowLoading() {
    $('#statusMsg').fadeIn('fast').focus();
}

function HideLoading() {
    $('#statusMsg').hide();
}

function LockAddNewVersion() {
    $('#software-distribution-modal').find('.modal-body .add-version input').prop('disabled', true);
    $('#software-distribution-modal').find('.modal-footer button').prop('disabled', true);
    ShowLoading();
}

function UnlockAddNewVersion() {
    HideLoading();
    $('#software-distribution-modal').find('.modal-body .add-version input').prop('disabled', false);
    $('#software-distribution-modal').find('.modal-footer button').prop('disabled', false);
}

function addNewVersion() {
    var version = $('#inputVersion').val().trim();
    var checksum = $('#inputChecksum').val().trim();
    var files = document.getElementById('inputFile').files;

    if (version === "") {
        alert("Please enter Version.");
        $('#inputVersion').focus();
        return false;
    }
    checksum = checksum.replace(" ", "");
    if (checksum === "") {
        alert("Please enter Checksum.");
        $('#inputChecksum').focus();
        return false;
    } else if (checksum.length !== 64) {
        alert("Invalid Checksum. Please re-enter again. ");
        $('#inputChecksum').focus();
        return false;
    }
    if (files.length === 0) {
        alert("Please select a file.");
        $('#inputFile').focus();
        return false;
    }

    var file = files[0];
    var formData = new FormData();
    formData.append('VersionName', version);
    formData.append('Checksum', checksum);
    formData.append(file.name, file);

    $.ajax({
        type: "POST",
        url: "/views/kioskMaintenance/softwareDistribution/UploadFiles.ashx",
        data: formData,
        dataType: "json",
        processData: false,
        contentType: false,
        beforeSend: function () {
            LockAddNewVersion();
        },
        success: function (data) {
            var status = data.Status;
            var msg = data.Message;

            if (!status) {
                alert(msg);
                UnlockAddNewVersion();
                return false;
            }

            UnlockAddNewVersion();
            alert("New software version has added successfully.");
            closeModal();
            GetSoftwareList();
        },
        error: function (error) {
            alert("Error " + error.status);
            UnlockAddNewVersion();
        }
    });
}

function removeSoftwareVersion(VersionId) {
    var confirmed = confirm("Are you sure you want to Delete?");

    if (confirmed) {
        $.ajax({
            type: "POST",
            url: "/views/kioskMaintenance/softwareDistribution/main.aspx/DeleteSoftwareVersion",
            data: JSON.stringify({ VersionId: VersionId }),
            dataType: "json",
            contentType: "application/json; charset=utf-8",
            success: function (data) {
                var status = data.d.Status;
                var msg = data.d.Message;

                if (!status) {
                    alert(msg);
                    return false;
                }

                alert("Successful.");
                GetSoftwareList();
            },
            error: function (error) {
                alert("Error " + error.status);
            }
        });
    }
}

function InitialAddQueue() {
    loadVersionList();
    GetMachineList();
}

function loadVersionList() {
    $.ajax({
        type: "POST",
        url: "/views/kioskMaintenance/softwareDistribution/main.aspx/GetSoftwareList",
        data: "{}",
        dataType: "json",
        contentType: "application/json; charset=utf-8",
        beforeSend: function () {
            $('#selectVersion').html('<option selected disabled>Loading...</option>');
        },
        success: function (data) {
            var status = data.d.Status;
            var msg = data.d.Message;

            if (!status) {
                alert(msg);
                return false;
            }

            var SoftwareList = data.d.Object;
            var str = "";

            if (SoftwareList.length === 0) {
                str = str + '<option selected disabled>None</option>';
            } else {
                str = str + '<option selected disabled> - Please Select - </option>';

                for (var i = 0; i < SoftwareList.length, Software = SoftwareList[i]; i++) {
                    str = str + '<option value="' + Software.VersionId + '">' + Software.VersionName + '</option>';
                }
            }

            $('#selectVersion').html(str);
        },
        error: function (error) {
            alert("Error " + error.status);
        }
    });
}

function GetMachineList() {
    $.ajax({
        type: "POST",
        contentType: "application/json; charset=utf-8",
        url: "/views/kioskMaintenance/softwareDistribution/main.aspx/GetAllMachineList",
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

    $('#software-distribution-main-div').append('<div id="machine-detail-tooltip"><span class="machine-detail-branch-code">' + branchCode + '</span><span class="machine-detail-branch-name">' + branchName + '</span><br/>'
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

/* Switch date/datetime input based on the selection */
$('.select-update-type').change(switchDatetimeInput);
function switchDatetimeInput() {
    var target = $(this).attr('data-target');
    var updateType = $(this).val();

    $(target).val("").datepicker('remove').datetimepicker('remove');

    if (updateType == 1) {
        $(target).datepicker({
            format: 'dd/mm/yyyy',
            startDate: new Date(),
            autoclose: true
        });
    } else if (updateType == 2) {
        $(target).datetimepicker({
            format: 'dd/mm/yyyy hh:ii P',
            startDate: new Date(),
            autoclose: true
        });
    }
}

function addNewQueue(elem) {
    var VersionId = $('#selectVersion').val();
    var VersionName = $('#selectVersion > option:selected').text();
    var DownloadSUID = $('#selectDownloadSUID').val();
    var DownloadTime = $('#inputDownloadTime').val().trim();
    var DeploySUID = $('#selectDeploySUID').val();
    var DeployTime = $('#inputDeployTime').val().trim();
    var KioskList = [];

    if (VersionId === null) {
        alert("Please select a version.");
        $('#selectVersion').focus();
        return false;
    }
    if (DownloadSUID === null) {
        alert("Please select download type.");
        $('#selectDownloadSUID').focus();
        return false;
    }
    if (DownloadTime === "") {
        alert("Please specify the download time.");
        $('#inputDownloadTime').focus();
        return false;
    }
    if (DeploySUID === null) {
        alert("Please select deploy type.");
        $('#selectDownloadSUID').focus();
        return false;
    }
    if (DeployTime === "") {
        alert("Please specify the deployment time.");
        $('#inputDeployTime').focus();
        return false;
    }

    var DLDT_Format = (DownloadSUID == 1) ? 'DD/MM/YYYY' : 'DD/MM/YYYY hh:mm A';
    var DPDT_Format = (DeploySUID == 1) ? 'DD/MM/YYYY' : 'DD/MM/YYYY hh:mm A'; 
    var DLDT = moment(DownloadTime, DLDT_Format);
    var DPDT = moment(DeployTime, DPDT_Format);

    DownloadTime = moment(DLDT).format("DD/MM/YYYY hh:mm A");
    DeployTime = moment(DPDT).format("DD/MM/YYYY hh:mm A");

    if (DLDT > DPDT) {
        alert("Deploy Date Time cannot be earlier than download date time.");
        $('#inputDeployTime').focus();
        return false;
    }

    if ($('#selectMachineDiv').find('.selectedMachine').length === 0) {
        alert("Please select a kiosk.");
        return false;
    }

    $('#selectMachineDiv').find('.selectedMachine').each(function (e) {
        KioskList.push($(this).attr('data-id'));
    });

    $.ajax({
        type: "POST",
        contentType: "application/json; charset=utf-8",
        url: "/views/kioskMaintenance/softwareDistribution/main.aspx/AddNewQueue",
        data: JSON.stringify({ VersionId: VersionId, VersionName: VersionName, KioskList: KioskList, DownloadSUID: DownloadSUID, DLDT: DownloadTime, DeploySUID: DeploySUID, DPDT: DeployTime }),
        dataType: "json",
        beforeSend: function () {
            $(elem).prop('disabled', true);
        },
        success: function (data) {
            var status = data.d.Status;
            var msg = data.d.Message;

            if (!status) {
                alert(msg);
                $(elem).prop('disabled', false);
                return false;
            }

            alert("Successful. Your action has been sent to checker for approval.");
            $(elem).prop('disabled', false);
            closeModal();
        },
        error: function (error) {
            alert("Error " + error.status);
            $(elem).prop('disabled', false);
        }
    });
}

function editSoftwareQueue(KioskId, VersionId) {
    $.ajax({
        type: "POST",
        contentType: "application/json; charset=utf-8",
        url: "/views/kioskMaintenance/softwareDistribution/main.aspx/GetQueue",
        data: JSON.stringify({ KioskId: KioskId, VersionId: VersionId }),
        dataType: "json",
        beforeSend: function () {
            $('#downloadedRemark').hide();
            $('#editKioskId').html("Loading...");
            $('#editVersion').html("Loading...");
        },
        success: function (data) {
            var status = data.d.Status;
            var msg = data.d.Message;

            if (!status) {
                alert(msg);
                return false;
            }

            var Queue = data.d.Object;
            var VersionName = Queue.VersionName;
            var DownloadSUID = Queue.DownloadSUID;
            var DownloadTime = (Queue.DownloadSUID == 1) ? moment(Queue.DownloadTime).format("DD/MM/YYYY") : moment(Queue.DownloadTime).format("DD/MM/YYYY h:mm A");
            var DownloadedTime = (Queue.DownloadedTime == null) ? "" : moment(Queue.DownloadedTime).format("DD/MM/YYYY h:mm A");
            var DeploySUID = Queue.DeploySUID;
            var DeployTime = (Queue.DeploySUID == 1) ? moment(Queue.DeployTime).format("DD/MM/YYYY") : moment(Queue.DeployTime).format("DD/MM/YYYY h:mm A");

            $('#editKioskId').html(KioskId).attr('data-id', KioskId);
            $('#editVersion').html(VersionName).attr('data-id', VersionId);
            $('#editDownloadSUID').find('option[value="' + DownloadSUID + '"]').prop('selected', true).change();
            $('#editDeploySUID').find('option[value="' + DeploySUID + '"]').prop('selected', true).change();
            $('#editDownloadTime').val(DownloadTime);
            $('#editDeployTime').val(DeployTime);

            if (DownloadedTime !== "") {
                $('#editDownloadSUID').prop('disabled', true);
                $('#editDownloadTime').prop('disabled', true);
                $('#downloadedRemark > span').html('This software has been downloaded at <span style="font-weight:600;text-decoration:underline">' + DownloadedTime + '.');
                $('#downloadedRemark').fadeIn('fast');
            }
        },
        error: function (error) {
            alert("Error " + error.status);
        }
    });
}

function editQueue(elem) {
    var VersionId = $('#editVersion').attr('data-id');
    var VersionName = $('#editVersion').text();
    var KioskId = $('#editKioskId').attr('data-id');
    var DownloadSUID = $('#editDownloadSUID').val();
    var DownloadTime = $('#editDownloadTime').val().trim();
    var DeploySUID = $('#editDeploySUID').val();
    var DeployTime = $('#editDeployTime').val().trim();

    if (DownloadSUID === null) {
        alert("Please select download type.");
        $('#editDownloadSUID').focus();
        return false;
    }
    if (DownloadTime === "") {
        alert("Please specify the download time.");
        $('#editDownloadTime').focus();
        return false;
    }
    if (DeploySUID === null) {
        alert("Please select deploy type.");
        $('#editDeploySUID').focus();
        return false;
    }
    if (DeployTime === "") {
        alert("Please specify the deployment time.");
        $('#editDeployTime').focus();
        return false;
    }

    var DLDT_Format = (DownloadSUID == 1) ? 'DD/MM/YYYY' : 'DD/MM/YYYY hh:mm A';
    var DPDT_Format = (DeploySUID == 1) ? 'DD/MM/YYYY' : 'DD/MM/YYYY hh:mm A';
    var DLDT = moment(DownloadTime, DLDT_Format);
    var DPDT = moment(DeployTime, DPDT_Format);

    if (DLDT > DPDT) {
        alert("Deploy Date Time cannot be earlier than download date time.");
        $('#editDeployTime').focus();
        return false;
    }

    //Format it so that server can accept it
    DownloadTime = moment(DLDT).format("DD/MM/YYYY hh:mm A");
    DeployTime = moment(DPDT).format("DD/MM/YYYY hh:mm A");

    $.ajax({
        type: "POST",
        contentType: "application/json; charset=utf-8",
        url: "/views/kioskMaintenance/softwareDistribution/main.aspx/EditNewQueue",
        data: JSON.stringify({ VersionId: VersionId, VersionName: VersionName, KioskId: KioskId, DownloadSUID: DownloadSUID, DLDT: DownloadTime, DeploySUID: DeploySUID, DPDT: DeployTime }),
        dataType: "json",
        beforeSend: function () {
            $(elem).prop('disabled', true);
        },
        success: function (data) {
            var status = data.d.Status;
            var msg = data.d.Message;

            if (!status) {
                alert(msg);
                $(elem).prop('disabled', false);
                return false;
            }

            alert("Successful. Your action has been sent to checker for approval.");
            $(elem).prop('disabled', false);
            closeModal();
        },
        error: function (error) {
            alert("Error " + error.status);
            $(elem).prop('disabled', false);
        }
    });
}

function removeSoftwareQueue(KioskId, VersionId, VersionName) {
    var confirmed = confirm("Are you sure you want to Remove?");

    if (confirmed) {
        $.ajax({
            type: "POST",
            contentType: "application/json; charset=utf-8",
            url: "/views/kioskMaintenance/softwareDistribution/main.aspx/DeleteQueue",
            data: JSON.stringify({ VersionId: VersionId, VersionName: VersionName, KioskId: KioskId }),
            dataType: "json",
            success: function (data) {
                var status = data.d.Status;
                var msg = data.d.Message;

                if (!status) {
                    alert(msg);
                    return false;
                }

                alert("Successful. Your action has been sent to checker for approval.");
            },
            error: function (error) {
                alert("Error " + error.status);
            }
        });
    }
}