<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="main.aspx.cs" Inherits="Maestro.views.administration.vendorManagement.main" %>

<link rel="stylesheet" href="../components/bootstrap3/css/bootstrap-multiselect.css">
<script src="../components/bootstrap3/js/bootstrap-multiselect.js"></script>
<script type="text/javascript">

    var postData = {
        task: $('#taskNameHidden').val()
    };

    function triggerAddPage() {

        $.post("administration/vendorManagement/add.aspx", postData, function (data) {
            $("#content-subpage-container").html(data);
            showSubPage();
        });
    }
    function editVendor(VendorName) {
        var pdata = {
            VendorName: VendorName,
            task: $('#taskNameHidden').val()
        }
        $.post("/views/administration/vendorManagement/edit.aspx", pdata, function (data) {
            $("#content-subpage-container").html(data);
            showSubPage();
        });
    }
    function deleteVendor(vName) {
        var option = confirm("Are you sure you want to Delete?");
        if (option) {

            $.ajax({
                type: "POST",
                contentType: "application/json; charset=utf-8",
                url: "/views/administration/vendorManagement/main.aspx/deleteVendor",
                data: "{vName:'" + vName + "'}",
                dataType: "json",
                success: function (data) {
                    if (data.d) {
                        alert("Successful. Your action will be sent to checker for approval.");
                        loadVendorList();
                        showMainPage();
                    } else {
                        alert("Failed to delete Vendor!");
                    }
                },
                error: function (result) {
                    alert("Error to delete Vendor!");
                }
            });
        }
    }

    $(document).ready(function () {

        loadVendorList();

        $('#add-button').click(function () {
            triggerAddPage();
        });
    });

    function loadVendorList() {
        $("#tbDetails > tbody").html("<tr><td colspan='9'>Loading...</td></tr>");
        __JSONWEBSERVICE.getServices("getVendorDetails", getVendorDetailsSuccess, getVendorDetailsError);
    }

    function getVendorDetailsSuccess(data) {

        $("#tbDetails > tbody").empty();

        if (data.d.length === 0) {
            $("#tbDetails > tbody").html("<tr><td colspan='9'>No Vendor</td></tr>");
        } else {
            for (var i = 0; i < data.d.length; i++) {
                $("#tbDetails > tbody").append("<tr><td>" + data.d[i].VendorName + "</td><td>" + data.d[i].ContactPer1 + "</td><td>" + data.d[i].Tel1 +
                    "</td><td>" + data.d[i].Email1 + "</td><td>" + data.d[i].Email2 + "</td><td>" + data.d[i].Add1 + "</td><td>" + data.d[i].Add2 + "</td><td>" + data.d[i].Country +
                    "</td><td><a access-gate task='Vendor Management' permission='Edit' href='javascript:;' class='.editUser-button' id='" + data.d[i].VendorName +
                    "' onclick=\"editVendor('" + data.d[i].VendorName + "')\"><i class='fa fa-pencil'></i>Edit</a>  <span class='center-divider'></span><a access-gate task='Vendor Management' permission='Delete' class='deleteUser-button' href='javascript:;' onclick=\"deleteVendor('" + data.d[i].VendorName + "')\"><i class='fa fa-trash-o'></i>Delete</a></td> </tr>"
                    );
            }
        }
    };
    function getVendorDetailsError(data) {
        alert("Error to retrieve Vendor details!");
    };
</script>
<div>
    <input id="taskNameHidden" type="hidden" runat="server" />
    <div id="addButton">        
        <a access-gate task='Vendor Management' permission='Add' href="javascript:;" id="add-button" class="func-btn"><i class="fa fa-plus"></i>Add New Vendor</a>
    </div>
    <div class="block-xs"></div>
    <table class="table table-bordered" id="tbDetails">
        <thead>
            <tr>
                <th>Vendor</th>
                <th>Contact Person</th>
                <th>Tel No.</th>
                <th>Primary Email</th>
                <th>Secondary Email</th>
                <th>Address 1</th>
                <th>Address 2</th>
                <th>Country</th>
                <th></th>
            </tr>
        </thead>
        <tbody>
        </tbody>
    </table>
</div>
