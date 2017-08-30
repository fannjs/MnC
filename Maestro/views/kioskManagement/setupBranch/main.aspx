<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="main.aspx.cs" Inherits="Maestro.views.kioskManagement.setupBranch.main" %>
<style>

</style>
<div>
    <form id="addBranchForm" class="form-horizontal" role="form">
        <fieldset>
            <div id="selectionDiv" class="well-div">
                <legend>Add Branch</legend>
                <div class="field-group">
                    <label for="selectBranchSite" class="form-label width-md">Site</label>
                    <select class="input-field width-xl" id="selectBranchSite">
                        <option>Loading...</option>
                    </select>
                </div>
                <div class="field-group">
                    <label for="selectBranchState" class="form-label width-md">State</label>
                    <select class="input-field width-xl" id="selectBranchState"></select>
                </div>
                <div class="field-group">
                    <label for="inputBranchDistrict" class="form-label width-md">District</label>
                    <input type="text" class="input-field width-xl" id="inputBranchDistrict" placeholder="District">
                </div>
                <div class="field-group">
                    <label for="inputBranchName" class="form-label width-md">Branch Name</label>
                    <input type="text" class="input-field width-xl" id="inputBranchName" placeholder="Branch Name">
                </div>
                <div class="field-group">
                    <label for="inputBranchCode" class="form-label width-md">Branch Code</label>
                    <input type="text" class="input-field width-xl" id="inputBranchCode" placeholder="Branch Code">
                </div>
                <div id="addInfoSection">
                    <div class="field-group">
                        <label for="inputBranchAddress1" class="form-label width-md">Address 1</label>
                        <input type="text" class="input-field width-xl" id="inputBranchAddress1" maxlength="50" placeholder="Address">
                    </div>
                    <div class="field-group">
                        <label for="inputBranchAddress2" class="form-label width-md">Address 2</label>
                        <input type="text" class="input-field width-xl" id="inputBranchAddress2" maxlength="50" placeholder="Address">
                    </div>
                    <div class="field-group">
                        <label for="inputBranchPhone" class="form-label width-md">Phone</label>
                        <input type="text" class="input-field width-xl" id="inputBranchPhone" placeholder="Phone">
                    </div>
                    <div class="field-group">
                        <label for="inputBranchPIC" class="form-label width-md">Contact Person</label>
                        <input type="text" class="input-field width-xl" id="inputBranchPIC" placeholder="Contact Person">
                    </div>
                </div>
                <div class="field-group">
                    <button type="button" class="btn btn-primary" access-gate task='Setup Branch' permission='Add'  id="btnAdd" >Add</button>
                </div>
            </div>
        </fieldset>
    </form>
</div>
<script src="/assets/scripts/kioskManagement/setupBranch/main.js"></script>
