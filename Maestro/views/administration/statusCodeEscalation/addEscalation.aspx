<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="addEscalation.aspx.cs" Inherits="Maestro.views.administration.statusCodeEscalation.addEscalation" %>

<script src="administration/statusCodeEscalation/addEscalation.js" type="text/javascript"></script>
<div>
    <div id="addNewEscalationMainDiv" class="well-div">
        <legend>New Status Code Escalation</legend>
        <div class="field-group">
            <label class="form-label width-md">Kiosk Type</label>
            <select id="selectKioskType" onchange="changeKioskType(this);" onclick="KT(this);" class="input-field width-lg required-field" runat="server">
                <option disabled selected value=""> - Please Select - </option>
            </select>
        </div>
        <div class="field-group">
            <label class="form-label width-md">Status Code</label>
            <div id="StatusCodeListing" class="item-listing-container width-lg">
                <!-- Status Code listing here -->
            </div>
            &nbsp;
            <a class="link-btn" title="Add Status Code to the Listing" data-target="StatusCode" data-title="Status Code" onclick="openModal(this);">Click Here to Add</a>
        </div>
        <div class="field-group">
            <label class="form-label width-md">Escalation Type</label>
            <select id="selectEscalationType" class="input-field width-lg required-field">
                <option disabled selected value=""> - Please Select - </option>
                <option value="1">SMS only</option>
                <option value="2">Email only</option>
                <option value="3">SMS & Email</option>
            </select>
             &nbsp;
            <a class="link-btn" title="Add User to Email List" data-target="UserList" data-title="User List"onclick="openModal(this);">Assign User</a>
        </div>
        <div class="field-group">
            <label class="form-label width-md">User List</label>
            <div id="UserListing" class="item-listing-container" style="width:400px;">
                <!-- User listing here -->
            </div>
        </div>
        <hr />
        <div class="field-group">
            <label class="form-label width-md"></label>
            <button class="btn btn-primary addEscalationBtn" onclick="addNewEscalation();">Add</button>
            <button class="btn btn-default addEscalationBtn" onclick="showMainPage();">Cancel</button>
        </div>
    </div>
    <div id="modal-wrapper">
        <div class="modal-dialog">
            <div class="modal-header">
                <span class="modal-title"></span>
                <span class="modal-close-btn" onclick="closeModal();"><i class="fa fa-times" style="width:auto;"></i></span>
            </div>
            <div id="modal-content" style="padding:8px;">
                <div id="statusCodesMainDiv" data-role="StatusCode">
                    <div>
                        <div>
                            <span class="container-title">Select Status Code</span>
                            <span class="pull-right">
                                <span style="color:#999;">Filter by : </span>
                                <span class="filterOption selected">ERROR</span>
                                <span class="filterOption selected">WARN</span>
                            </span>
                        </div>                        
                        <div class="block-xs"></div>
                        <div id="statusCodesDiv">
                            <!-- Status Code here -->
                        </div>
                        <div class="block-xs"></div>
                        <div>
                            <button id="confirmSelectedCode" class="btn btn-primary" style="padding: 4px 12px;display:none;" onclick="confirmSelectedCode();">Confirm</button>
                            <button class="btn btn-default" style="padding: 4px 12px;" onclick="closeModal();">Close</button>
                        </div>
                    </div>
                </div>
                <div id="userListMainDiv" data-role="UserList">
                    <input type="hidden" id="userListingType" />
                    <div>
                        <div class="container-title">Select User</div>
                        <div class="block-xs"></div>
                        <div id="userListDiv">
                            <!-- Escalation User here -->
                        </div>
                        <div class="block-xs"></div>
                        <div>
                            <button id="confirmSelectedUserBtn" class="btn btn-primary" style="padding: 4px 12px;display:none;" onclick="confirmSelectedUsers();">Confirm</button>
                            <button class="btn btn-default" style="padding: 4px 12px;" onclick="closeModal();">Close</button>
                        </div>
                    </div>
                    <span style="display:block;height:10px;"></span>
                    <div>
                        <div class="container-title">Add User</div>
                        <div class="block-xs"></div>
                        <div class="field-group">
                            <label class="form-label width-md">User Name</label>
                            <input type="text" class="input-field width-xl required-field" name="userName" placeholder="User Name" maxlength="50">
                        </div>
                        <div class="field-group">
                            <label class="form-label width-md">Phone Number</label>
                            <input type="text" class="input-field width-xl required-field" name="phoneNo" placeholder="Phone Number" maxlength="20">
                        </div>
                        <div class="field-group">
                            <label class="form-label width-md">Email Address</label>
                            <input type="text" class="input-field width-xl required-field" name="emailAddress" placeholder="Email Address" maxlength="100">
                        </div>
                        <div class="field-group">
                            <label class="form-label width-md">Company</label>
                            <input type="text" class="input-field width-xl required-field" name="companyName" placeholder="Company" maxlength="50">
                        </div>
                        <div class="block-xs"></div>
                        <div class="field-group">
                            <label class="form-label width-md"></label>
                            <button class="btn btn-primary addEscalationUserBtn" onclick="addNewEscalationUser();">Add</button>
                            <button class="btn btn-danger addEscalationUserBtn" onclick="resetTextField();">Reset</button>
                        </div>
                    </div>
                </div>
            </div>
        </div>   
    </div>
</div>
