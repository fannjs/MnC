var Status = {
    Fail: 0,
    Success: 1,
    Processing: 2
};
var AdvType = {
    Image: 1,
    Video: 2
};
var timer;
var selectedMachineArray = [];
var AdvGroupId = 1; // 1 - Global

$(document).ready(function () {
    GetAdvertisementByGroup();
    GetSpecific();

    $('.datetimepicker').remove();
    $('.dtpicker-input').datetimepicker({
        format: 'dd/mm/yyyy HH:ii P',
        startDate: new Date(),
        autoclose: true
    });
    $('.input-datepicker').keypress(function (e) {
        e.preventDefault();
    });
});

function loadGroupAdv(elem) {
    var $this = $(elem);
    AdvGroupId = parseInt($this.data('gid'));

    $('#machineSelectionHideDiv').slideUp('fast');
    selectedMachineArray = [];

    UnloadSpecificGroup();

    GetAdvertisementByGroup();
}

function GetAdvertisementByGroup() {
    $.ajax({
        type: "POST",
        contentType: "application/json; charset=utf-8",
        url: "/views/kioskMaintenance/adsMaintenance/main.aspx/GetAdvertisementList",
        data: JSON.stringify({ GroupId: AdvGroupId }),
        dataType: "json",
        beforeSend: function () {
            $('#advertisement-uploaded-div li:not(#liUpload)').remove(); //Quick fix AngularJS
            $('#advertisement-uploaded-div > ul').append('<li>Loading...</li>');
        },
        success: function (data) {
            var status = data.d.Status;
            var msg = data.d.Message;

            if (!status) {
                alert(msg);
                return false;
            }

            $('#advertisement-uploaded-div li:not(#liUpload)').remove();

            var Adv = data.d.Object;
            var AdvSeqList = Adv.SequenceList;
            var AdvNoSeqList = Adv.NoSequenceList;

            if (AdvSeqList.length === 0 && AdvNoSeqList.length === 0) {
                $('#advertisement-uploaded-div > ul').append('<li>(empty)</li>');

            } else {

                var media = "";
                var i = 0;
                for (i = 0; i < AdvSeqList.length, SeqAdv = AdvSeqList[i]; i++) {

                    var FileName = SeqAdv.Adv_FileName;
                    var FileNameWithoutExt = FileName.substring(0, FileName.lastIndexOf('.'));

                    media = media + '<li><div class="image-outer-wrapper hasSequence" data-advID="' + SeqAdv.Adv_Id + '" data-advName="' + FileName + '" title="' + FileName + '">'
                                     + '<div class="image-wrapper-header"><span class="thumbnailBtn removeAdvertisement" title="Delete"><i class="fa fa-times"></i></span></div>'
                                    + '<div class="image-wrapper-body">';

                    if (SeqAdv.Adv_Type === AdvType.Image) {
                        media = media + '<img class="image-div" src="../views/kioskMaintenance/adsMaintenance/Adv/Thumbnail/' + FileNameWithoutExt + '.jpg" title="' + FileName + '" />';
                    } else if (SeqAdv.Adv_Type === AdvType.Video) {
                        media = media + '<video controls class="image-div" controls preload="none" class="image-div" poster="../views/kioskMaintenance/adsMaintenance/Adv/Thumbnail/' + FileNameWithoutExt + '.jpg" '
                                      + 'src="../views/kioskMaintenance/adsMaintenance/Adv/mp4/' + FileNameWithoutExt + '.jpg" title="' + FileName + '" />';
                    }

                    media = media + '</div><div class="image-wrapper-footer"><span class="seqCounter"></span><span class="thumbnailBtn seqChecked"><i class="fa fa-check"></i></span></div></div></li>';
                }
                for (i = 0; i < AdvNoSeqList.length, NoSeqAdv = AdvNoSeqList[i]; i++) {

                    var FileName = NoSeqAdv.Adv_FileName;
                    var FileNameWithoutExt = FileName.substring(0, FileName.lastIndexOf('.'));

                    media = media + '<li><div class="image-outer-wrapper" data-advID="' + NoSeqAdv.Adv_Id + '" data-advName="' + FileName + '" title="' + FileName + '">'
                                     + '<div class="image-wrapper-header"><span class="thumbnailBtn removeAdvertisement" title="Delete"><i class="fa fa-times"></i></span></div>'
                                    + '<div class="image-wrapper-body">';

                    if (NoSeqAdv.Adv_Type === AdvType.Image) {
                        media = media + '<img class="image-div" src="../views/kioskMaintenance/adsMaintenance/Adv/Thumbnail/' + FileNameWithoutExt + '.jpg" title="' + FileName + '" />';
                    } else if (NoSeqAdv.Adv_Type === AdvType.Video) {
                        media = media + '<video controls class="image-div" controls preload="none" class="image-div" poster="../views/kioskMaintenance/adsMaintenance/Adv/Thumbnail/' + FileNameWithoutExt + '.jpg" '
                                      + 'src="../views/kioskMaintenance/adsMaintenance/Adv/mp4/' + FileNameWithoutExt + '.jpg" title="' + FileName + '" />';
                    }

                    media = media + '</div><div class="image-wrapper-footer"><span class="seqCounter"></span><span class="thumbnailBtn seqChecked"><i class="fa fa-check"></i></span></div></div></li>';
                }

                $('#advertisement-uploaded-div > ul').append(media);
                populateSeqCounter();

                $('#advertisement-uploaded-div > ul').sortable({
                    items: "li:not(#liUpload)",
                    containment: "#advertisement-div-body"
                });
                $('#advertisement-uploaded-div > ul').droppable({
                    drop: function (event, ui) {
                        timer = setTimeout(populateSeqCounter, 100);
                    }
                });
            }

        },
        error: function (error) {
            alert("Error " + error.status + ". Unable to load advertisement.");
        }
    });
}

function GetSpecific() {
    $.ajax({
        type: "POST",
        contentType: "application/json; charset=utf-8",
        url: "/views/kioskMaintenance/adsMaintenance/main.aspx/GetSpecificAdv",
        data: JSON.stringify({ GroupId: AdvGroupId }),
        dataType: "json",
        beforeSend: function () {
            $('#specific-adv-div').html('Loading...');
        },
        success: function (data) {
            var status = data.d.Status;
            var msg = data.d.Message;

            if (!status) {
                alert(msg);
                return false;
            }

            var SpecAdvList = data.d.Object;

            if (SpecAdvList.length !== 0) {

                var str = "";

                for (var i = 0; i < SpecAdvList.length, SpecAdv = SpecAdvList[i]; i++) {

                    var GroupId = SpecAdv.GroupId;
                    var KioskList = SpecAdv.KioskList;
                    var KioskListArray = [];

                    if (KioskList.length === 0) {
                        continue;
                    } else {
                        for (var a = 0; a < KioskList.length; a++) {
                            KioskListArray.push(KioskList[a].M_MACH_ID);
                        }

                        var KioskDesc = KioskListArray.join(", ");

                        str = str + '<div class="specific-adv" data-gid="' + GroupId + '" data-kiosk="' + KioskDesc + '" onclick="loadSpecificGroup(this);">' + KioskDesc + '</div>';
                    }
                }

                $('#specific-adv-div').html(str);
            } else {
                $('#specific-adv-div').empty();
            }
        },
        error: function (error) {
            alert("Error " + error.status + ". Unable to load specific advertisement.");
        }
    });
}

function loadSpecificGroup(elem) {
    var $this = $(elem);
    AdvGroupId = parseInt($this.data('gid'));

    var Kiosk = $this.data('kiosk');
    selectedMachineArray = Kiosk.split(", ");

    LoadSpecificKioskList();
    GetAdvertisementByGroup();
    
    //Show Delete Btn
    $('#deleteSpecificAdv').fadeIn('fast');
}

function UnloadSpecificGroup() {
    $('#deleteSpecificAdv').hide();
}

function LoadSpecificKioskList() {
    
    $('#machineSelectionHideDiv').slideDown('fast');

    $.ajax({
        type: "POST",
        contentType: "application/json; charset=utf-8",
        url: "/views/kioskMaintenance/adsMaintenance/main.aspx/GetSpecificMachineList",
        data: JSON.stringify({ GroupId: AdvGroupId }),
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
                    if ($.inArray(MachineList[i].M_MACH_ID, selectedMachineArray) != -1) {
                        machines = machines + '<div class="a-machine selectedMachine" data-id="' + MachineList[i].M_MACH_ID + '" branch-code="' + MachineList[i].M_BRANCH_CODE + '"' +
                                        ' branch-name="' + MachineList[i].M_BRANCH_NAME + '" address1="' + MachineList[i].M_ADDRESS1 + '" address2="' + MachineList[i].M_ADDRESS2 + '">' + MachineList[i].M_MACH_ID + '</div>';
                    }
                    else {
                        machines = machines + '<div class="a-machine" data-id="' + MachineList[i].M_MACH_ID + '" branch-code="' + MachineList[i].M_BRANCH_CODE + '"' +
                                        ' branch-name="' + MachineList[i].M_BRANCH_NAME + '" address1="' + MachineList[i].M_ADDRESS1 + '" address2="' + MachineList[i].M_ADDRESS2 + '">' + MachineList[i].M_MACH_ID + '</div>';
                    }
                }

                $('#selectMachineDiv').html(machines);
            }
        },
        error: function (error) {
            alert("Error " + error.status + ". Unable to load machine list.");
        }
    });
}

document.getElementById('fileUploader').addEventListener('change', handleFileSelect, false);

//Multiple upload, reference http://www.html5rocks.com/en/tutorials/file/dndfiles/
function handleFileSelect(evt) {

    var files = evt.target.files; // FileList object
    var uploadedFiles = 0;
    var filesToSend = [];

    // Loop through the FileList and render image files as thumbnails.
    for (var i = 0, f; f = files[i]; i++) {

        // Only process these files
        if (!f.type.match('image.*') && !f.type.match('video.*')) {
            continue;
        }
        else {

            var reader = new FileReader();
            // Closure to capture the file information.
            reader.onload = (function (theFile) {
                return function (e) {
               
                    var aFile = {
                        name: theFile.name,
                        type: theFile.type,
                        src: e.target.result
                    };

                    filesToSend.push(aFile);
                };
            })(f);

            reader.onloadend = function (e) {

                uploadedFiles++;

                if (uploadedFiles >= files.length) {
                    uploadAdvertisement(filesToSend);
                }
            };

            // Read in the image file as a data URL.
            reader.readAsDataURL(f);
        }
    }
}

function uploadAdvertisement(filesToSend) {

    $.ajax({
        type: "POST",
        contentType: "application/json; charset=utf-8",
        url: "/views/kioskMaintenance/adsMaintenance/main.aspx/UploadAdvertisement",
        data: JSON.stringify({ files: filesToSend }),
        dataType: "json",
        beforeSend: function () {
            $('#statusMsg').show();
            $('body').addClass('modal-open');
        },
        success: function (data) {

            var status = data.d.Status;
            var msg = data.d.Message;

            $('#statusMsg').hide();
            $('body').removeClass('modal-open');
            $('#fileUploader').val("");

            if (!status) {
                alert(msg);
                return false;
            }

            alert("Successful. Your action has been sent to checker for approval.");
        },
        error: function (error) {
            alert("Error. Unable to upload.");
        }
    });
}

$('#advertisement-uploaded-div').on('click', '.removeAdvertisement', removeAdvertisement);
function removeAdvertisement(event) {
    var r = confirm("Are you sure you want to delete this advertisement?");
    var advID = $(this).closest('.image-outer-wrapper').attr('data-advID');

    if (r) {
        $.ajax({
            type: "POST",
            contentType: "application/json; charset=utf-8",
            url: "/views/kioskMaintenance/adsMaintenance/main.aspx/DeleteAdvertisementFile",
            data: JSON.stringify({ AdvID: advID }),
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
                alert("Error " + error.status + ". Unable to delete.");
            }
        });
    }
}

//Check (Add to sequence)
$('#advertisement-uploaded-div').on('click', '.seqChecked', checkSeq);
function checkSeq() {
    if ($(this).closest('.image-outer-wrapper').hasClass('hasSequence')) {
        $(this).closest('.image-outer-wrapper').removeClass('hasSequence');
    }
    else {
        $(this).closest('.image-outer-wrapper').addClass('hasSequence');
    }

    populateSeqCounter();
}

function populateSeqCounter() {

    var sequenceAdv = document.getElementsByClassName("hasSequence");

    for (var i = 0; i < sequenceAdv.length; i++) {

        var seqCounter = sequenceAdv[i].getElementsByClassName("seqCounter");
        seqCounter[0].innerText = (i + 1);
    }
}

function closeQueueConfig() {

    $('#modalWrapper').hide();
    $('.dtpicker-input').val("");
    $('body').removeClass('modal-open');
}
function LoadQueueConfigDiv() {

    var Advertisement = document.getElementById("advertisement-uploaded-div").querySelectorAll('.hasSequence');
    //If is not global, get machine List
    if (AdvGroupId !== 1) {
        var SelectedKiosks = document.getElementById("selectMachineDiv").querySelectorAll('.selectedMachine');
        if (SelectedKiosks.length === 0) {
            alert("Please select kiosk.");
            return false;
        }
    }
    if (Advertisement.length === 0) {
        alert("Please define the sequence.");
        return false;
    }

    $('#saveSequenceSubmit').show();
    $('#confirmDeleteSpecificAdv').hide();
    $('body').scrollTop(0).addClass('modal-open'); //Scroll to most TOP because of the datepicker plugin position problem
    $('#modalWrapper').fadeIn('fast');
}

function saveSequence(elem) {

    var Advertisement = document.getElementById("advertisement-uploaded-div").querySelectorAll('.hasSequence');
    var KioskList = [];
    var AdvArray = [];
    var DownloadDT = $('#inputDownloadDT').val();
    var DeployDT = $('#inputDeployDT').val();

    //If is not global, get machine List
    if (AdvGroupId !== 1) {
        var SelectedKiosks = document.getElementById("selectMachineDiv").querySelectorAll('.selectedMachine');
        if (SelectedKiosks.length === 0) {
            alert("Please select kiosk.");
            return false;
        }

        for (var i = 0; i < SelectedKiosks.length, Kiosk = SelectedKiosks[i]; i++) {
            var id = Kiosk.getAttribute('data-id');

            KioskList.push(id);
        }
    }

    if (Advertisement.length === 0) {
        alert("Please define the sequence.");
        return false;
    }
    for (var i = 0; i < Advertisement.length; i++) {
        var id = Advertisement[i].getAttribute("data-advid");
        AdvArray.push(id);
    }

    if (DownloadDT.trim() === "") {
        alert("Please define the Download Date Time.");
        $('#inputDownloadDT').focus();
        return false;
    }
    if (DeployDT.trim() === "") {
        alert("Please define the Deploy Date Time.");
        $('#inputDeployDT').focus();
        return false;
    }

    var CDT = moment();
    var DLDT = moment(DownloadDT, "DD/MM/YYYY hh:mm:ss A");
    var DPDT = moment(DeployDT, "DD/MM/YYYY hh:mm:ss A");

    if (CDT > DLDT) {
        alert("Download Date Time cannot be earlier than current date time.");
        $('#inputDownloadDT').focus();
        return false;
    }
    if (DLDT > DPDT) {
        alert("Deploy Date Time cannot be earlier than download date time.");
        $('#inputDeployDT').focus();
        return false;
    }

    if (AdvGroupId === 1) {
        $.ajax({
            type: "POST",
            contentType: "application/json; charset=utf-8",
            url: "/views/kioskMaintenance/adsMaintenance/main.aspx/SetGlobalSequence",
            data: JSON.stringify({ AdvArray: AdvArray, DownloadDT : DownloadDT, DeployDT : DeployDT }),
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
                closeQueueConfig();
            },
            error: function (error) {
                alert("Error " + error.status + " occurs.");
            }
        });
    }
    else {
        $.ajax({
            type: "POST",
            contentType: "application/json; charset=utf-8",
            url: "/views/kioskMaintenance/adsMaintenance/main.aspx/SetSpecificSequence",
            data: JSON.stringify({ GroupId: AdvGroupId, KioskList: KioskList, AdvArray: AdvArray, DownloadDT: DownloadDT, DeployDT: DeployDT }),
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
                closeQueueConfig();
                closeSelectMachineModal();
            },
            error: function (error) {
                alert("Error " + error.status + " occurs.");
            }
        });
    }
}

function deleteSpecificAdv() {
    var confirmed = confirm("Are you sure you want to remove this Specific Advertisement?");

    if (confirmed) {

        $('#saveSequenceSubmit').hide();
        $('#confirmDeleteSpecificAdv').show();

        $('body').scrollTop(0).addClass('modal-open'); //Scroll to most TOP because of the datepicker plugin position problem
        $('#modalWrapper').fadeIn('fast');
    } 
}
function confirmDeleteSpecificAdv(elem) {

    var DownloadDT = $('#inputDownloadDT').val();
    var DeployDT = $('#inputDeployDT').val();

    if (DownloadDT.trim() === "") {
        alert("Please define the Download Date Time.");
        $('#inputDownloadDT').focus();
        return false;
    }
    if (DeployDT.trim() === "") {
        alert("Please define the Deploy Date Time.");
        $('#inputDeployDT').focus();
        return false;
    }

    var CDT = moment();
    var DLDT = moment(DownloadDT, "DD/MM/YYYY hh:mm:ss A");
    var DPDT = moment(DeployDT, "DD/MM/YYYY hh:mm:ss A");

    if (CDT > DLDT) {
        alert("Download Date Time cannot be earlier than current date time.");
        $('#inputDownloadDT').focus();
        return false;
    }
    if (DLDT > DPDT) {
        alert("Deploy Date Time cannot be earlier than download date time.");
        $('#inputDeployDT').focus();
        return false;
    }
    
    $.ajax({
        type: "POST",
        contentType: "application/json; charset=utf-8",
        url: "/views/kioskMaintenance/adsMaintenance/main.aspx/DeleteSpecificSequence",
        data: JSON.stringify({ GroupId: AdvGroupId, DownloadDT: DownloadDT, DeployDT: DeployDT }),
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

            closeQueueConfig();
            closeSelectMachineModal();
            GetSpecific();
        },
        error: function (error) {
            alert("Error " + error.status + " occurs.");
        }
    });
}

function closeSelectMachineModal() {
    $('#machineSelectionHideDiv').slideUp('fast');
    selectedMachineArray = [];

    //Load Global if select kiosk is cancelled
    AdvGroupId = 1;
    GetAdvertisementByGroup();

    UnloadSpecificGroup();
}

//Set it to 0 because 0 will never be a group id  (group id start from 1)

function triggerSpecificConfig() {
    AdvGroupId = 0;
    GetAdvertisementByGroup();

    selectedMachineArray = [];

    loadSelectMachineDiv();

    UnloadSpecificGroup();
}

function loadSelectMachineDiv() {

    $('#machineSelectionHideDiv').slideDown('fast');

    $.ajax({
        type: "POST",
        contentType: "application/json; charset=utf-8",
        url: "/views/kioskMaintenance/adsMaintenance/main.aspx/GetAllMachineList",
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


$('#navigation-bar ul li').click(function (e) {

    if ($(this).hasClass('active')) {
        return false;
    }

    $('#navigation-bar ul li').removeClass('active');
    $(this).addClass('active');

    var target = $(this).data('href');

    if (target === '#advQueueViewDiv') {
        loadAdvQueue();
    }

    $('#advertisement-maintenance-main-div .tab-content').hide();
    $(target).fadeIn('fast');
});

function loadAdvQueue() {
    $.ajax({
        type: "POST",
        contentType: "application/json; charset=utf-8",
        url: "/views/kioskMaintenance/adsMaintenance/main.aspx/GetAdvQueue",
        data: "{}",
        dataType: "json",
        beforeSend: function () {
            $('#advertisement-queue-table > tbody').html("<tr><td colspan='6'>Loading...</td></tr>");
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
                str = "<tr><td colspan='6'>No Queue.</td></tr>";
            } else {

                for (var i = 0; i < QueueList.length, Queue = QueueList[i]; i++) {

                    var DownloadDT = moment(Queue.DownloadDT).format("D/MM/YYYY h:mm:ss A");
                    var DeployDT = moment(Queue.DeployDT).format("D/MM/YYYY h:mm:ss A");

                    str = str + '<tr><td>' + (i + 1) + '</td><td>' + Queue.M_MACH_ID + '</td>'
                                + '<td>' + Queue.GroupId + '</td><td>' + Queue.Sequence + '</td>'
                                + '<td>' + DownloadDT + '</td><td>' + DeployDT + '</td></tr>'
                }
            }

            $('#advertisement-queue-table > tbody').html(str);
        },
        error: function (error) {
            alert("Error " + error.status + ". Unable to load machine list.");
        }
    });
}



