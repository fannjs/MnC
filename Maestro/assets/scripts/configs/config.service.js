
var __JSONWEBSERVICE = null; //to be global
var __SERVICEAPI = {
    "webapis": {
        "getCustDetails": "/views/administration/siteManagement/main.aspx/getCustDetails",
        "getVendorDetails": "/views/administration/vendorManagement/main.aspx/getVendorDetails",
        "addNewMachine": "/views/kioskManagement/machineTemplate/add.aspx/addNewMachine",
        "updateMachine": "/views/kioskManagement/machineTemplate/edit.aspx/updateMachine",
        "deleteMachine": "/views/kioskManagement/machineTemplate/main.aspx/deleteMachine",
        "addNewMCode": "/views/kioskManagement/machineTemplate/main.aspx/addNewMCode",
        "updateMCode": "/views/kioskManagement/machineTemplate/main.aspx/updateMCode",
        "deleteMCode": "/views/kioskManagement/machineTemplate/main.aspx/deleteMCode",
        "getDeploySites": "/views/kioskManagement/installMachine/main.aspx/getDeploySites",
        "getDeployStates": "/views/kioskManagement/installMachine/main.aspx/getDeployStates",
        "getDeployDistricts": "/views/kioskManagement/installMachine/main.aspx/getDeployDistricts",
        "getDeployBranches": "/views/kioskManagement/installMachine/main.aspx/getDeployBranches",
        "getCalendarID": "/views/kioskManagement/installMachine/main.aspx/getCalendarID",
        "getVendor": "/views/kioskManagement/installMachine/main.aspx/getVendor",
        "getMErrCodeHistory": "/views/dashboard.aspx/getMErrCodeHistory",
        "AddNewBranch": "/views/kioskManagement/setupBranch/main.aspx/AddNewBranch",
        "getCustDetailsByID": "/views/administration/siteManagement/edit.aspx/getCust",
        "getVendorByName": "/views/administration/vendorManagement/edit.aspx/getVendorByName",
        "InstallMachine": "/views/kioskManagement/installMachine/main.aspx/InstallMachine",
        "uploadAdv": "/views/kioskMaintenance/advertisement/main.aspx/uploadAdv",
        "getAdDetails": "/views/kioskMaintenance/advertisement/main.aspx/getAdDetails",
        "setAdvSeq": "/views/kioskMaintenance/advertisement/main.aspx/setAdvSeq",
        "delAdvDefaultSeq": "/views/kioskMaintenance/advertisement/main.aspx/delAdvDefaultSeq",
        "setMachAdvSeq": "/views/kioskMaintenance/advertisement/specific.aspx/setMachAdvSeq",
        "setMachinesAdvSeq": "/views/kioskMaintenance/advertisement/specific.aspx/setMachinesAdvSeq",
        "getAdDefaultSeq": "/views/kioskMaintenance/advertisement/main.aspx/getAdDefaultSeq",
        "delAdvFile": "/views/kioskMaintenance/advertisement/main.aspx/delAdvFile",
        "regenSeqToAll": "/views/kioskMaintenance/advertisement/main.aspx/regenSeqToAll",
        "updateVersion": "/views/kioskMaintenance/softwareDistribution/main.aspx/updateVersion",
        "MultiUpdateVersion": "/views/kioskMaintenance/softwareDistribution/main.aspx/MultiUpdateVersion"
    }
};



jQuery(document).ready(function ($) {

    function ConfigService(options) {

        options = options || {};
        ConfigService.prototype.getServices = function (optionsTrans, successCallBack, errorCallBack) {

            optionsTrans = optionsTrans || {};
            var serviceUrl = ""; 

            serviceUrl = __SERVICEAPI.webapis[optionsTrans.url] || __SERVICEAPI.webapis[optionsTrans] || optionsTrans;
            var dataToSend = JSON.stringify(optionsTrans.data) || "{}"; //must be a string

            $.ajax({
                type: options.type,
                contentType: options.contentType,
                url: serviceUrl,
                data: dataToSend,
                dataType: options.dataType,
                success: successCallBack,
                error: errorCallBack
            });
        };

    };
    __JSONWEBSERVICE = new ConfigService(
                                            {
                                                type: "POST",
                                                contentType: "application/json; charset=utf-8",
                                                dataType: "json"
                                            }
                                        );


});


