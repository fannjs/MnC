<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="mainStatusRpt.aspx.cs" Inherits="Maestro.views.reports.statusReport.mainStatusRpt" %>


<link rel="Stylesheet" href="../../../components/bootstrap.datepicker/css/datepicker.css" />
<script src="../../../components/bootstrap.datepicker/js/bootstrap-datepicker.js"></script>

<form id="form1" runat="server" action="javascript:void(0);">
<div id="problems-volumn-main-div">
    <div id="selectionDiv" class="well-div">
        <h1 class="h1">Search Criteria</h1>
        <hr style="margin:10px 0px;" />
        <div class="block-xs"></div>
        <div class="field-group">
            <label for="inputStartDate" class="form-label width-lg">Start Date</label>
            <input type="text" class="input-field width-lg datepicker" id="inputStartDate" runat="server" />
        </div>
        <div class="field-group">
            <label for="inputEndDate" class="form-label width-lg">End Date</label>
            <input type="text" class="input-field width-lg datepicker" id="inputEndDate" runat="server" />
        </div>
        <div class="field-group">
            <label for="selectKioskId" class="form-label width-lg">Kiosk ID</label>
            <asp:dropdownlist class="input-field width-lg" id="selectKioskId" runat="server">
               <asp:ListItem value="All" selected> - All - </asp:ListItem>
            </asp:dropdownlist>
        </div>
        <div class="field-group">
            <label for="selectKioskType" class="form-label width-lg">Kiosk Type</label>
            <asp:dropdownlist class="input-field width-lg" id="selectKioskType" runat="server">
               <asp:ListItem value="" disabled> - Please select - </asp:ListItem>
               <asp:ListItem value="CSD" selected > CSD </asp:ListItem>
               <asp:ListItem value="CJD" > CJD </asp:ListItem>
            </asp:dropdownlist>
        </div>
        <%--<div class="field-group">
            <label for="selectChartType" class="form-label width-lg">Chart Type</label>
            <asp:dropdownlist class="input-field width-lg" id="selectChartType" runat="server">
               <asp:ListItem value="BarChart" selected> Bar Chart </asp:ListItem>
               <asp:ListItem value="PieChart" > Pie Chart </asp:ListItem>
            </asp:dropdownlist>
        </div>--%>
        <div class="block-xs"></div>
        <div class="field-group">
            <asp:textbox runat="server" id="txtUsername" style="display:none"></asp:textbox>
            <label class="form-label width-lg"></label>
            <button class="btn btn-primary" onclick="search();">Submit</button>
            <%--<asp:Button ID="btnSearch" class="btn btn-primary" runat="server" Text="Search" OnClick="btnSearch_Click" />--%>
            <%--<asp:button id="btnMachListReport" class="btn btn-primary" runat="server" text="machListReport" OnClick="btnMachListReport_Click" />--%>
            <br />
        </div>
    </div>

</div>
    </form>
<script type="text/javascript">
    function search() {
        var userName = $("#txtUsername").val();
        var startDate = $("#inputStartDate").val();
        var endDate = $("#inputEndDate").val();
        var machType = $("#selectKioskType").val();
        var machID = $("#selectKioskId").val();
        var chartType = $("#selectChartType").val();
        //14/02/2015
        var sdataSplit = startDate.split('/');
        var sDate = sdataSplit[2] + "-" + sdataSplit[1] + "-" + sdataSplit[0];

        var edataSplit = endDate.split('/');
        var eDate = edataSplit[2] + "-" + edataSplit[1] + "-" + edataSplit[0];

         window.open("reports/statusReport/statusReportOutput.aspx?startDate=" + sDate + "&endDate=" + eDate + "&machType=" + machType + "&machID=" + machID + "&chartType=barChart&userName=" + userName + "", "_blank", "height=600, width=900, left=50, top=50, " +
        "location=no, menubar=no, resizable=no, " +
        "scrollbars=no, titlebar=no, toolbar=no", true);
        return;
    }
    $(document).ready(function () {
        $('#inputStartDate').datepicker({
            format: 'dd/mm/yyyy',
            autoclose: true
        });
        $('#inputEndDate').datepicker({
            format: 'dd/mm/yyyy',
            autoclose: true
        });
    });
</script>
