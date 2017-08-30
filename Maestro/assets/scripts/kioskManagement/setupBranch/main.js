jQuery(document).ready(function($) {

    //console.log(__CONFIGFACTORY.states);

    __JSONWEBSERVICE.getServices("getCustDetails", getCustDetailsSuccess, getCustDetailsError);

    var siteCountryInfo = {};

    function getCustDetailsSuccess(msg) {
        var custInfo = msg.d; // need not $.parseJSON(); as auto formatted to object

        var siteList = {};

        for (var i = 0; i < custInfo.length; i++) {
            siteList[custInfo[i].CustCode] = custInfo[i].CustCode + ' - ' + custInfo[i].CustName;
            siteCountryInfo[custInfo[i].CustCode] = custInfo[i].Country;
        }
        $('#selectBranchSite').initDropDown(siteList);
        $('#selectBranchSite').trigger('change');
    };


    function getCustDetailsError(data) {
        alert('error: getCustDetails');
    };


    $('#selectBranchSite').change(function () {
       // $('#selectBranchState').initDropDown(__CONFIGFACTORY.states[]);

        var stateObjList = __CONFIGFACTORY.states[siteCountryInfo[$(this).val()]];
        var temp = [];

        for (var i = 0; i < stateObjList.length; i++) {
            temp.push(stateObjList[i].Name);
        }
        $('#selectBranchState').initDropDown(temp);

    });


   
function addNewBranchSuccess(msg) {

    var data = msg.d;

    if (data.Status == true) {
        alert("Successful. Your action will be sent to checker for approval.");

        $('#addBranchForm input').val("");
    }
    else {
        alert(data.Description);
    }
};

function addNewBranchError(data) {
    alert('Error: Failed to add Branch!');
};

function AddNewBranch() {

    var BranchSite = $('#selectBranchSite').val();
    var BranchState = $('#selectBranchState option:selected').text();
    var BranchDistrict = $('#inputBranchDistrict').val();
    var inputBranchName = $('#inputBranchName').val();
    var inputBranchCode = $('#inputBranchCode').val();
    var inputBranchAddress1 = $('#inputBranchAddress1').val();
    var inputBranchAddress2 = $('#inputBranchAddress2').val();
    var inputBranchPhone = $('#inputBranchPhone').val();
    var inputBranchPIC = $('#inputBranchPIC').val();

    if(BranchSite.trim() === "" || BranchSite === null){
        $('#selectBranchSite').focus();
        return false;
    }else if(BranchState.trim() === "" || BranchState === null){
        $('#selectBranchState').focus();
        return false;
    }else if(BranchDistrict.trim() === ""){
        $('#inputBranchDistrict').focus();
        return false;
    }else if(inputBranchName.trim() === ""){
        $('#inputBranchName').focus();
        return false;
    }else if(inputBranchCode.trim() === ""){
        $('#inputBranchCode').focus();
        return false;
    }else if(inputBranchAddress1.trim() === ""){
        $('#inputBranchAddress1').focus();
        return false;
    }else if(inputBranchAddress2.trim() === ""){
        $('#inputBranchAddress2').focus();
        return false;
    }else if(inputBranchPhone.trim() === ""){
        $('#inputBranchPhone').focus();
        return false;
    }else if(inputBranchPIC.trim() === ""){
        $('#inputBranchPIC').focus();
        return false;
    }

    var para = {
        'M_CUST_CODE': BranchSite,
        'M_STATE': BranchState,
        'M_DISTRICT': BranchDistrict,
        'M_BRANCH_NAME': inputBranchName,
        'M_BRANCH_CODE': inputBranchCode,
        'M_ADDRESS1': inputBranchAddress1,
        'M_ADDRESS2': inputBranchAddress2,
        'M_TEL': inputBranchPhone,
        'M_CONTACT': inputBranchPIC
    };

    var objurl =
    {
        'url': 'AddNewBranch',
        'data': para,
    };

    __JSONWEBSERVICE.getServices(objurl, addNewBranchSuccess, addNewBranchError);
}


   
    $('#btnAdd').click(function () {
        AddNewBranch();
    });

});

