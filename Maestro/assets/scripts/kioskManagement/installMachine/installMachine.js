jQuery(document).ready(function ($) {


    function InstallNewMachineSuccess(msg) {

        var status = msg.d.Status;
        var message = msg.d.Description;

        if (!status) {
            alert(message);
            return false;
        }

        alert("Successful. Your action has been sent to checker for approval.");
        $('#installMachine-modal').modal('hide');
        resetInstallMachineFields();
    };

    function InstallNewMachineError(data) {
        alert('Error: Failed to install Machine!');
    };

    function InstallNewMachine() {

        var installMachineID = $('#installMachineID').val().trim();
        var installMachineVendor = $('#installMachineVendor').val();
        var installMachineVendorName = $('#installMachineVendor > option:selected').text();
        var txtSelectedType  = $('#txtSelectedType').val();
        var inputInstallMachineCountry = $('#inputInstallMachineCountry').val();
        var selectInstallMachineBranchCode = $('#selectInstallMachineBranch').val();
        var selectCalendarID = $('#installCalendarID').val();
        var serialNo = $('#installSerialNo').val().trim();

        if (installMachineID == "" || serialNo == "")
        {
            alert("Empty fields are not allow!");
            return;
        }

        if(selectCalendarID == 0 || selectCalendarID == null){
            selectCalendarID = "";
        }

        if(installMachineVendor == null || installMachineVendor == ""){
            alert("Please choose a Vendor.");
            return false;
        }

        var para = {
            'M_MACH_ID': installMachineID,
            'VENDOR_ID': installMachineVendor,
            'VENDOR_NAME': installMachineVendorName,
            'M_MACH_TYPE': txtSelectedType,
            'M_MACH_COUNTRY': inputInstallMachineCountry,
            'branchCode':  selectInstallMachineBranchCode||"",
            'calendarID' : selectCalendarID,
            'serialNo' : serialNo         
        };

        var objurl =
        {
            'url': 'InstallMachine',
            'data': para,
        };

        __JSONWEBSERVICE.getServices(objurl, InstallNewMachineSuccess, InstallNewMachineError);

    }

    $('#btnSubmit').click(function () {
      
        InstallNewMachine();
        
    })

    $('#btnReset').click(function() {
        
        resetInstallMachineFields();
    });

    function resetInstallMachineFields(){
        
        $('#installMachine-modal .input-field').val("");
        $('#installCalendarID option:first-child').attr('selected', true);
        $('#installMachineVendor option:first-child').attr('selected', true);
    }
});