jQuery(document).ready(function ($) {

    var tempCode_DeploySite = {};


    $('#selectInstallMachineSite').change(function () {
        var tempArr = [];
        var custCode = $(this).val();
        var selectedDeploySite = tempCode_DeploySite[custCode];

        $('#inputInstallMachineCountry').val(selectedDeploySite['Country']);

        var serviceData = { "url": "getDeployStates", "data": { "custCode": custCode} };

        __JSONWEBSERVICE.getServices(serviceData, getDeployStatesSuccess, getDeployStatesError);
    });

    $('#selectInstallMachineState').change(function () {
        var tempArr = [];
        var state = $(this).find("option:selected").text(); //
        var custCode = $('#selectInstallMachineSite option:selected').val();

        var serviceData = { "url": "getDeployDistricts", "data": { "custCode": custCode, "state": state} };

        __JSONWEBSERVICE.getServices(serviceData, getDeployDistrictsSuccess, getDeployDistrictsError);
    });

    $('#selectInstallMachineDistrict').change(function () {
        var tempArr = [];
        var state = $('#selectInstallMachineState option:selected').text();
        var custCode = $('#selectInstallMachineSite option:selected').val();
        var district = $(this).find("option:selected").text(); //

        var serviceData = { "url": "getDeployBranches", "data": { "custCode": custCode, "state": state, "district": district} };

        __JSONWEBSERVICE.getServices(serviceData, getDeployBranchesSuccess, getDeployBranchesError);


    });


    function getDeploySitesSuccess(msg) {
        var deploySites = msg.d; // need not $.parseJSON(); as auto formatted to object

        for (var i = 0; i < deploySites.length; i++) {
            tempCode_DeploySite[deploySites[i].CustCode] = deploySites[i]; //deployInfo[i].CustCode + ' - ' + deployInfo[i].CustName;
        }

        $('#selectInstallMachineSite').initDropDownFormat(tempCode_DeploySite, function (obj)/*function pointer*/{
            return obj['CustCode'] + ' - ' + obj['CustName'];
        });


        $('#selectInstallMachineSite').trigger('change');

    };
    function getDeploySitesError(msg) {
        alert('error: getDeploySitesError');

    };
    function getDeployStatesSuccess(msg) {
        var arrState = msg.d;
        $('#selectInstallMachineState').initDropDown(arrState);
        $('#selectInstallMachineState').trigger('change');
    };
    function getDeployStatesError(msg) {
        alert('error: getDeployStatesError');
    };
    function getDeployDistrictsSuccess(msg) {
        var arrDistrict = msg.d;
        $('#selectInstallMachineDistrict').initDropDown(arrDistrict);
        $('#selectInstallMachineDistrict').trigger('change');

    };
    function getDeployDistrictsError(msg) {
        alert('error: getDeployBranchesError');
    };
    function getDeployBranchesSuccess(msg) {
        var branchList = msg.d;
        var temp = {};
        for (var i = 0; i < branchList.length; i++) {
            temp[branchList[i].BranchCode] = branchList[i].BranchCode + ' - ' + branchList[i].BranchName;
        }

        $('#selectInstallMachineBranch').initDropDown(temp);

    };
    function getDeployBranchesError(msg) {

    };

    function getCalendar() {

    };
    function getCalendarSuccess(msg) {
        var options = "";

        options = options + '<option selected disabled value="0"> - Please select - </option>';

        for (var index = 0; index < msg.d.length; index++) {
            options = options + '<option value="' + msg.d[index].CalendarID + '">' + msg.d[index].CalendarName + '</option>';
        }

        $('#installCalendarID').html(options);
    };
    function getCalendarError(msg) {
        alert("Error: getCalendarError ");
    };

    function getVendorSuccess(data) {
        var status = data.d.Status;
        var msg = data.d.Message;

        if (!status) {
            alert(msg);
            return false;
        }

        var VendorList = data.d.Object;
        var options = "";

        options = options + '<option selected disabled> - Please select - </option>';

        for (var index = 0; index < VendorList.length; index++) {
            options = options + '<option value="' + VendorList[index].VendorId + '">' + VendorList[index].VendorName + '</option>';
        }

        $('#installMachineVendor').html(options);
    };
    function getVendorError(msg) {
        alert("Error: getVendorError ");
    };

    var getMachineTypes = function () {

        jQuery.support.cors = true;
        $.ajax({
            url: '/api/machinetype',
            type: 'GET',
            success: onMachineTypesReturn,
            error: function (msg) {
                alert('woops');
                console.log(msg);
            }
        });
    };
    function onMachineTypesReturn(machineTypes) {

        var $tblMachTemplateBody = $('#tblMachTempList > tbody');

        var $newRow;
        var $newCol;
        var $lnk = $('<a>').attr('href', '#');
        $tblMachTemplateBody.empty();

        if (machineTypes.length === 0) {
            $tblMachTemplateBody.html("<tr><td colspan='2'>None. Please add in Machine Template.</td></tr>");
        } else {

            $.each(machineTypes, function (index, machineType) {

                $newCol = $('<td>');
                $newRow = $('<tr>');

                $newRow.addClass('clickable').bind('click', { 'machType': machineType.M_MACH_TYPE, 'machModel': machineType.M_MACH_MODEL }, onInstallModelClicked)
                //.append($newCol.clone().html(index + 1))
                .append($newCol.clone().html(machineType.M_MACH_TYPE))
                .append($newCol.clone().html(machineType.M_MACH_MODEL));
                $tblMachTemplateBody.append($newRow);
            });
        }
    };

    function onInstallModelClicked(evt) {
        var machType = evt.data.machType;
        var machModel = evt.data.machModel;

        $('#txtSelectedType').val(machType);
        $('#machine-model-name').html(machModel);
        $('#install-machine-site').html($('#selectInstallMachineSite option:selected').text());
        $('#install-machine-country').html($('#inputInstallMachineCountry').val());
        $('#install-machine-state').html($('#selectInstallMachineState option:selected').text());
        $('#install-machine-district').html($('#selectInstallMachineDistrict option:selected').text());
        $('#install-machine-branch').html($('#selectInstallMachineBranch option:selected').text());



        $('#installMachine-modal').modal('show');






    }

    __JSONWEBSERVICE.getServices("getDeploySites", getDeploySitesSuccess, getDeploySitesError);
    getMachineTypes();

    __JSONWEBSERVICE.getServices("getCalendarID", getCalendarSuccess, getCalendarError);
    __JSONWEBSERVICE.getServices("getVendor", getVendorSuccess, getVendorError);
});