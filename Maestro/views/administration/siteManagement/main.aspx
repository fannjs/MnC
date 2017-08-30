<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="main.aspx.cs" Inherits="Maestro.views.administration.siteManagement.main" %>

<script type="text/javascript">

    var postData = {
        task: $('#taskNameHidden').val()
    };

    function triggerAddPage() {
        $.post("administration/siteManagement/add.aspx", postData, function (data) {
            $("#content-subpage-container").html(data);
            showSubPage();
        });
    }
    function deleteCust(custCode) {
        var option = confirm("Are you sure you want to Delete?");
        if (option) {

            $.ajax({
                type: "POST",
                contentType: "application/json; charset=utf-8",
                url: "/views/administration/siteManagement/main.aspx/deleteCust",
                data: "{custCode:'" + custCode + "'}",
                dataType: "json",
                success: function (data) {
                    if (data.d) {
                        alert("Successful. Your action will be sent to checker for approval.");
                        __JSONWEBSERVICE.getServices("getCustDetails", getCustDetailsSuccess, getCustDetailsError);
                        showMainPage();
                    } else {
                        alert("Failed to delete user!");
                    }
                },
                error: function (result) {
                    alert("Error to delete user!");
                }
            });
        }
    }
    function editCust(custCode) {

        var pdata = {
            custCode: custCode,
            task: $('#taskNameHidden').val()
        };

        $.post("/views/administration/siteManagement/edit.aspx", pdata, function (data) {
            $("#content-subpage-container").html(data);
            showSubPage();
        });
    }
    $(document).ready(function () {

        loadCustomerList();

        $('#add-button').click(function () {
            triggerAddPage();
        });
    });

    function loadCustomerList() {
        $("#tbDetails > tbody").html('<tr><td colspan="4">Loading...</td></tr>');
        __JSONWEBSERVICE.getServices("getCustDetails", getCustDetailsSuccess, getCustDetailsError);
    }

    function getCustDetailsSuccess(data) {

        $("#tbDetails > tbody").empty();

        if (data.d.length === 0) {
            $("#tbDetails > tbody").html('<tr><td colspan="4">No site</td></tr>');
        }
        else {
            for (var i = 0; i < data.d.length; i++) {
                $("#tbDetails > tbody").append("<tr><td>" + data.d[i].CustCode + "</td><td>" + data.d[i].CustName + "</td><td>"
                    + data.d[i].Country + "</td><td><a access-gate task='Vendor Management' permission='Edit' href='javascript:;' class='.editUser-button' id='" + data.d[i].CustCode
                    + "' onclick=\"editCust('" + data.d[i].CustCode + "')\"><i class='fa fa-pencil'></i>Edit</a> <span class='center-divider'></span><a access-gate task='Site Operation' permission='Delete' class='deleteUser-button' href='javascript:;' onclick=\"deleteCust('" + data.d[i].CustCode + "')\"><i class='fa fa-trash-o'></i>Delete</a></td> </tr>"
                    );
            }
        }
    };

    function getCustDetailsError(data) {
        alert("Error " + data.status);
    };
</script>
<div>
    <input id="taskNameHidden" type="hidden" runat="server" />
    <div id="addButton">        
        <a access-gate task="Site Operation" permission="Add"  href="javascript:;" id="add-button" class="func-btn"><i class="fa fa-plus"></i>Add New Site</a>
    </div>
    <div class="block-xs"></div>
    <table class="table table-bordered" id="tbDetails">
        <thead>
            <tr>
                <th>Code</th>
                <th>Name</th>
                <th>Country</th>
                <th></th>
            </tr>
        </thead>
        <tbody>
        </tbody>
    </table>
</div>