<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="edit.aspx.cs" Inherits="Maestro.views.administration.vendorManagement.edit" %>

<link rel="stylesheet" href="../components/bootstrap3/css/bootstrap-multiselect.css">
<script src="../components/bootstrap3/js/bootstrap-multiselect.js"></script>
<script src="/assets/scripts/administration/vendorManagement/edit.js"></script>
<script type="text/javascript">
    $('#cancel-button').click(function () {
        showMainPage();
    });

    function updateVendor() {
        var Vendor = $('#inputVendor').val();
        var ContactPerson = $('#inputVendorContactPerson').val();
        var TelNo = $('#inputVendorTelNo').val();
        var PrimaryEmail = $('#inputPrimaryEmail').val();
        var SecondaryEmail = $('#inputVendorSecondaryEmail').val();
        var VendorAddress1 = $('#inputVendorAddress1').val();
        var VendorAddress2 = $('#inputVendorAddress2').val();
        var Country = $('#selectVendorCountry option:selected').val();
        $.ajax({
            type: "POST",
            contentType: "application/json; charset=utf-8",
            url: "/views/administration/vendorManagement/edit.aspx/updateVendor",
            data: "{Vendor:'" + Vendor + "', ContactPerson:'" + ContactPerson + "', TelNo:'" + TelNo + "', PrimaryEmail:'" + PrimaryEmail + "', SecondaryEmail:'" + SecondaryEmail + "', VendorAddress1:'" + VendorAddress1 + "', VendorAddress2:'" + VendorAddress2 + "', Country:'" + Country + "'}",
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
</script>
<div class="well">
    <form class="form-horizontal" role="form" action="javascript:;">
        <fieldset>
            <legend>Edit</legend>
            <div class="form-group">
                <label for="inputVendor" class="col-xs-2 control-label">Vendor</label>
                <div class="col-xs-4">
                    <input type="text" runat="server" disabled="disabled" class=" form-control" id="inputVendor" placeholder="Vendor">
                </div>
            </div>
            <div class="form-group">
                <label for="inputVendorContactPerson" class="col-xs-2 control-label">Contact Person</label>
                <div class="col-xs-4">
                    <input type="text" runat="server" class="form-control" id="inputVendorContactPerson" placeholder="Contact Person">
                </div>
            </div>
            <div class="form-group">
                <label for="inputVendorTelNo" class="col-xs-2 control-label">Tel No.</label>
                <div class="col-xs-4">
                    <input type="text" runat="server" class="form-control" id="inputVendorTelNo" placeholder="Tel No.">
                </div>
            </div>
            <div class="form-group">
                <label for="inputPrimaryEmail" class="col-xs-2 control-label">Primary Email</label>
                <div class="col-xs-4">
                    <input type="text" runat="server" class="form-control" id="inputPrimaryEmail" placeholder="Primary Email">
                </div>
            </div>
            <div class="form-group">
                <label for="inputVendorSecondaryEmail" class="col-xs-2 control-label">Secondary Email</label>
                <div class="col-xs-4">
                    <input type="text" runat="server" class="form-control" id="inputVendorSecondaryEmail" placeholder="Secondary Email">
                </div>
            </div>
            <div class="form-group">
                <label for="inputVendorAddress1" class="col-xs-2 control-label">Address 1</label>
                <div class="col-xs-4">
                    <input type="text" runat="server" class="form-control" id="inputVendorAddress1" placeholder="Address 1">
                </div>
            </div>
            <div class="form-group">
                <label for="inputVendorAddress2" class="col-xs-2 control-label">Address 2</label>
                <div class="col-xs-4">
                    <input type="text" runat="server" class="form-control" id="inputVendorAddress2" placeholder="Address 2">
                </div>
            </div>
            <div class="form-group">
                <label for="selectVendorCountry" class="col-xs-2 control-label">Country</label>
                <div id="divCountry" runat="server" class="col-xs-4">
                    <select class="form-control" id="selectVendorCountry">
                        <option>Loading...</option>
                    </select>
                </div>
            </div>
            <div class="form-group">
                <div class="col-xs-offset-2 col-xs-4">
                    <button type="button" class="btn btn-primary" onclick="updateVendor()">Update</button>
                    <button type="button" id="cancel-button" class="btn btn-default">Cancel</button>
                </div>
            </div>
        </fieldset>
    </form>
</div>