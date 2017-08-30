<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="fullKioskListMain.aspx.cs" Inherits="Maestro.views.reports.fullKioskList.fullKioskListMain" %>

<link rel="Stylesheet" href="../../../components/bootstrap.datepicker/css/datepicker.css" />
<script src="../../../components/bootstrap.datepicker/js/bootstrap-datepicker.js"></script>

<form id="form1" runat="server" action="javascript:void(0);">
<div id="problems-volumn-main-div">
    <div id="selectionDiv" class="well-div">
        <h1 class="h1">Search Criteria</h1>
        <hr style="margin:10px 0px;" />
        <div class="block-xs"></div>
        <div class="field-group">
            <label for="selectKioskType" class="form-label width-lg">Kiosk Type</label>
            <asp:dropdownlist class="input-field width-lg" id="selectKioskType" runat="server">
               <asp:ListItem value="All" selected> - All - </asp:ListItem>
            </asp:dropdownlist>
        </div>
        <div class="block-xs"></div>
        <div class="field-group">
            <asp:textbox runat="server" id="txtUsername" style="display:none"></asp:textbox>
            <label class="form-label width-lg"></label>
            <button class="btn btn-primary" onclick="search();">Submit</button>
            <br />
        </div>
    </div>

</div>
    </form>
<script type="text/javascript">
    function search() {
        var userName = $("#txtUsername").val();
        var machType = $("#selectKioskType").val();

        window.open("reports/fullKioskList/fullKioskListOutput.aspx?machType=" + machType + "&userName=" + userName + "", "_blank", "height=600, width=900, left=50, top=50, " +
       "location=no, menubar=no, resizable=no, " +
       "scrollbars=no, titlebar=no, toolbar=no", true);
        return;
    }
    //$(document).ready(function () {
    //    $('#inputStartDate').datepicker({
    //        format: 'dd/mm/yyyy',
    //        autoclose: true
    //    });
    //    $('#inputEndDate').datepicker({
    //        format: 'dd/mm/yyyy',
    //        autoclose: true
    //    });
    //});
</script>