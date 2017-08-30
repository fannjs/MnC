<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="add.aspx.cs" Inherits="Maestro.views.administration.vendorManagement.add" %>

<link rel="stylesheet" href="../components/bootstrap3/css/bootstrap-multiselect.css">
<script src="../components/bootstrap3/js/bootstrap-multiselect.js"></script>

<script type="text/javascript">
    function addVendor() {        
        var vendorName = $('#inputVendor').val();
        var contactPerson = $('#inputVendorContactPerson').val();
        var telNo = $('#inputVendorTelNo').val();
        var primaryEmail = $('#inputVendorPrimaryEmail').val();
        var secondaryEmail = $('#inputVendorSecondaryEmail').val();
        var add1 = $('#inputVendorAddress1').val();
        var add2 = $('#inputVendorAddress2').val();
        var country = $('#selectVendorCountry option:selected').text();

        $.ajax({
            type: "POST",
            contentType: "application/json; charset=utf-8",
            url: "/views/administration/vendorManagement/add.aspx/insertNewVendor",
            data: "{vendorName:'" + vendorName + "', contactPerson:'" + contactPerson + "', telNo:'" + telNo + "', primaryEmail:'" + primaryEmail + "', secondaryEmail:'" + secondaryEmail + "', add1:'" + add1 + "', add2:'" + add2 + "', country:'" + country + "'}",
            dataType: "json",
            success: function (data) {

                var status = data.d.Status;
                var msg = data.d.Message;

                if (!status) {
                    alert(msg);
                    return false;
                }

                alert("Successful. Your action will be sent to checker for approval.");
                loadVendorList();
                showMainPage();
            },
            error: function (result) {
                alert("Error " + result.status);
            }
        });
    }

    $('#cancel-button').click(function () {
        showMainPage();
    });

    $('#selectVendorCountry').initDropDown(__CONFIGFACTORY.countries);
</script>
<div>
    <form class="form-horizontal well" role="form" action="javascript:;">
        <fieldset>
            <legend>Insert</legend>
            <div class="form-group">
                <label for="inputVendor" class="col-xs-2 control-label">Vendor</label>
                <div class="col-xs-4">
                    <input type="text" class=" form-control" id="inputVendor" placeholder="Vendor">
                </div>
            </div>
            <div class="form-group">
                <label for="inputVendorContactPerson" class="col-xs-2 control-label">Contact Person</label>
                <div class="col-xs-4">
                    <input type="text" class="form-control" id="inputVendorContactPerson" placeholder="Contact Person">
                </div>
            </div>
            <div class="form-group">
                <label for="inputVendorTelNo" class="col-xs-2 control-label">Tel No.</label>
                <div class="col-xs-4">
                    <input type="text" class="form-control" id="inputVendorTelNo" placeholder="Tel No.">
                </div>
            </div>
            <div class="form-group">
                <label for="inputVendorPrimaryEmail" class="col-xs-2 control-label">Primary Email</label>
                <div class="col-xs-4">
                    <input type="email" class="form-control" id="inputVendorPrimaryEmail" placeholder="Primary Email">
                </div>
            </div>
            <div class="form-group">
                <label for="inputVendorSecondaryEmail" class="col-xs-2 control-label">Secondary Email</label>
                <div class="col-xs-4">
                    <input type="email" class="form-control" id="inputVendorSecondaryEmail" placeholder="Secondary Email">
                </div>
            </div>
            <div class="form-group">
                <label for="inputVendorAddress1" class="col-xs-2 control-label">Address 1</label>
                <div class="col-xs-4">
                    <input type="text" class="form-control" id="inputVendorAddress1" placeholder="Address 1">
                </div>
            </div>
            <div class="form-group">
                <label for="inputVendorAddress2" class="col-xs-2 control-label">Address 2</label>
                <div class="col-xs-4">
                    <input type="text" class="form-control" id="inputVendorAddress2" placeholder="Address 2">
                </div>
            </div>
            <div class="form-group">
                <label for="inputVendorCountry" class="col-xs-2 control-label">Country</label>
                <div class="col-xs-4">
                    <select class="form-control" id="selectVendorCountry">
                        <option>Loading...</option>
                    </select>
                </div>
            </div>
            <div class="form-group">
                <div class="col-xs-offset-2 col-xs-4">
                    <button type="submit" class="btn btn-primary" onclick="addVendor()">Add</button>
                    <button type="button" id="cancel-button" class="btn btn-default">Cancel</button>
                </div>
            </div>
        </fieldset>
    </form>
</div>
