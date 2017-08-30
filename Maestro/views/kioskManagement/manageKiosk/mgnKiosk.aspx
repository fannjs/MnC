<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="mgnKiosk.aspx.cs" Inherits="Maestro.views.kioskManagement.manageKiosk.mgnKiosk" %>

<link rel="stylesheet" href="../../../assets/styles/mahineslist.css" />
<div id="full-kiosk-list-main">
    <ul id="pagination" class="pagination">
    </ul>
    <div id="selection-div">
        Show
        <select id="record-per-page">
            <option selected>10</option>
            <option>20</option>
            <option>30</option>
            <option>40</option>
            <option>50</option>
            <option>60</option>
            <option>70</option>
            <option>80</option>
            <option>90</option>
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
                <th>District</th>
                <th>Branch Code</th>
                <th>Branch Name</th>
                <th>Calendar</th>
                <th>CQM Version No.</th>
                <th></th>
            </tr>
        </thead>
        <tbody>
        </tbody>
    </table>
    <div id="page-footer">
        <span id="page-information"></span>
    </div>
</div>
<!-- Modal for edit software version-->
<div class="modal" id="editMachModal" tabindex="-1" role="dialog" aria-labelledby="myModalLabel" aria-hidden="true">
    <div class="modal-dialog" style="width:800px;">
        <div class="modal-content">
            <div class="modal-header">
                <button type="button" class="close" data-dismiss="modal"><span aria-hidden="true">&times;</span><span class="sr-only">Close</span></button>
                <h4 class="modal-title" id="H2">Edit Kiosk</h4>
            </div>
            <div class="modal-body">
                <div id="edit-version-body">
                    <div>
                        <div> <%-- editing version.--%>
                            <form class="form-horizontal" action="javascript:;">
                                <fieldset>
                                    <div class="field-group">
                                        <label class="form-label width-md">Kiosk ID</label>
                                        <input type="text" class="input-field width-xl" id="txtKioskID" disabled>
                                    </div>
                                    <div class="field-group">
                                        <label class="form-label width-md">Site</label>
                                        <select id="selectSite" class="input-field width-lg" onchange="LoadState();">
                                            <option disabled selected value=""> - Please Select - </option>
                                        </select>
                                    </div>
                                    <div class="field-group">
                                        <label class="form-label width-md">State</label>
                                        <select id="selectState" class="input-field width-lg" onchange="LoadDistrict();">
                                            <option disabled selected value=""> - Please Select - </option>
                                        </select>
                                    </div>
                                    <div class="field-group">
                                        <label class="form-label width-md">District</label>
                                        <select id="selectDistrict" class="input-field width-lg" onchange="LoadCity();">
                                            <option disabled selected value=""> - Please Select - </option>
                                        </select>
                                    </div>
                                    <%--<div class="field-group">
                                        <label class="form-label width-md">City</label>
                                        <select id="selectCity" class="input-field width-lg" onchange="LoadBranch();">
                                            <option disabled selected value=""> - Please Select - </option>
                                        </select>
                                    </div>--%>
                                    <div class="field-group">
                                        <label class="form-label width-md">Branch</label>
                                        <select id="selectBranch" class="input-field width-lg" onchange="ShowBranchContactInformation();">
                                            <option disabled selected value=""> - Please Select - </option>
                                        </select>
                                    </div>  
                                    <div class="field-group">
                                        <label class="form-label width-md">Contact Number</label>
                                        <input type="text" class="input-field width-xl" id="txtContact" placeholder="Contact Number" disabled>
                                    </div>  
                                    <div class="field-group">
                                        <label class="form-label width-md">Person in Charge</label>
                                        <input type="text" class="input-field width-xl" id="txtPIC" placeholder="Person in Charge" disabled>
                                    </div>
                                    <div class="field-group">
                                        <label class="form-label width-md">Vendor</label>
                                        <select id="selectVendor" class="input-field width-lg">
                                            <option disabled selected value=""> - Please Select - </option>
                                        </select>
                                    </div> 
                                    <div class="field-group">
                                        <label class="form-label width-md">Calendar</label>
                                        <select id="selectCalendar" class="input-field width-lg">
                                            <option disabled selected value=""> - Please Select - </option>
                                        </select>
                                    </div>
                                </fieldset>
                            </form>
                        </div>
                    </div>
                </div>
                <div id="loading-screen-div">
                    Loading...
                </div>
            </div>
            <div class="modal-footer">
                <button class="btn btn-primary" id="btnUpdateMach">Update</button>
                <button type="button" class="btn btn-default" data-dismiss="modal">Close</button>
            </div>
        </div>
    </div>
</div>
<script src="../../../assets/scripts/kioskManagement/manageKiosk/machineslist.js" type="text/javascript" ></script>
