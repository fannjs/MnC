<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="main.aspx.cs" Inherits="Maestro.views.reports.fullKioskList.main" %>

<link rel="Stylesheet" href="../../../assets/styles/fullkiosklist.css" />

<div id="full-kiosk-list-main">
    <ul id="pagination" class="pagination">
    </ul>
    <div id="selection-div">
        Show
        <select id="record-per-page">
            <option>5</option>
            <option>10</option>
            <option>20</option>
            <option>50</option>
            <option>100</option>
        </select>
        per page
    </div>   
    <table id="full-kiosk-list-table">
        <thead>
            <tr>
                <th>No.</th>
                <th>Kiosk ID</th>
                <th>State</th>
                <th>City</th>
                <th>Address 1</th>
                <th>Address 2</th>
                <th>Branch No.</th>
                <th>Contact Person</th>
            </tr>
        </thead>
        <tbody>
        </tbody>
    </table>
    <div id="page-footer">
        <span id="page-information"></span>
    </div>
    <div id="printingDiv" style="display:none;"></div>
</div>
<script src="../../../assets/scripts/reports/fullkiosklist.js" type="text/javascript"></script>
<script type="text/javascript">
    $(document).ready(function () {

        $('#full-kiosk-list-main').off();

        recordPerPage = $('#record-per-page').val();

        getMachineList(pageNumber, recordPerPage);
        pagination();
    });
</script>