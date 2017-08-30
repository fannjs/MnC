jQuery(document).ready(function ($) {
 

    $('#selectVendorCountry').initDropDownWithFormat(__CONFIGFACTORY.countries, {
        'valFormat': function (key, obj) {
            return obj[key];
        }
    });


    para = {
        "url": "getVendorByName",
        "data": {
            "sVendorName": $('#inputVendor').val()
        }
    };

    __JSONWEBSERVICE.getServices(para, onGetVendorDetailsByNameSuccess, onGetVendorDetailsByNameError);

    function onGetVendorDetailsByNameSuccess(msg) {
        var vend = msg.d[0] || {};

        $('#inputVendor').val(vend.VendorName);
        $('#selectVendorCountry').val(vend.Country);

        $('#inputVendorContactPerson').val(vend.ContactPer1); 
        $('#inputVendorTelNo').val(vend.Tel1); 
        $('#inputPrimaryEmail').val(vend.Email1); 
        $('#inputVendorSecondaryEmail').val(vend.Email2);
        $('#inputVendorAddress1').val(vend.Add1); 
        $('#inputVendorAddress2').val(vend.Add2);




    };

    function onGetVendorDetailsByNameError(msg) {

    };


});