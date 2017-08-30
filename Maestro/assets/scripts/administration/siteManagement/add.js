jQuery(document).ready(function ($) {

    $('#selectSiteCountry').initDropDown(__CONFIGFACTORY.countries);

    $('#cancel-button').click(function () {
         showMainPage();
    });
});


function addCustomer() {
    var siteCode = $('#inputSiteCode').val();
    var siteName = $('#inputSiteName').val();
    var siteCountry = $('#selectSiteCountry option:selected').text();

    $.ajax({
        type: "POST",
        contentType: "application/json; charset=utf-8",
        url: "/views/administration/siteManagement/add.aspx/insertNewCust",
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
            alert("Error to add record! Error " + error.status);
        }
    });
}