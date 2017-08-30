jQuery(document).ready(function ($) {


    $('#selectSiteCountry').initDropDownWithFormat(__CONFIGFACTORY.countries, {
        'valFormat': function (key, obj) {
            return obj[key];
        }
    });
    

    $('#cancel-button').click(function () {
         showMainPage();
    });

    para = {
        "url": "getCustDetailsByID",
        "data": {
            "sCustCode": $('#inputSiteCode').val()
        }
    };

    __JSONWEBSERVICE.getServices(para, onGetCustDetailsByIDSuccess, onGetCustDetailsByIDError);

    function onGetCustDetailsByIDSuccess(msg){
        var cust = msg.d[0];
        $('#inputSiteName').val(cust.CustName);
        $('#selectSiteCountry').val(cust.Country);
    };

    function onGetCustDetailsByIDError(msg){

    };

});


function updateUser() {
    var siteCode = $('#inputSiteCode').val();
    var siteName = $('#inputSiteName').val();
    var siteCountry = $('#selectSiteCountry option:selected').text();

    $.ajax({
        type: "POST",
        contentType: "application/json; charset=utf-8",
        url: "/views/administration/siteManagement/edit.aspx/updateCust",
        data: "{siteCode:'" + siteCode + "', siteName:'" + siteName + "', siteCountry:'" + siteCountry + "'}",
        dataType: "json",
        success: function (data) {

            var status = data.d.Status;
            var msg = data.d.Message;

            if (!status) {
                alert(msg);
                return false;
            }

            alert("Successful. Your action will be sent to checker for approval.");
            loadCustomerList();
            showMainPage();
        },
        error: function (error) {
            alert("Error " + error.status);
        }
    });
}