<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="main.aspx.cs" Inherits="Maestro.views.reports.audilTrail.main" %>

<link rel="Stylesheet" href="../../../components/bootstrap.datepicker/css/datepicker.css" />
<link rel="Stylesheet" href="/views/reports/auditTrail/main.css" />
<script src="../../../components/bootstrap.datepicker/js/bootstrap-datepicker.js"></script>
<div>
    <div id="selectionDiv" class="well-div">
        <h1 class="h1">Search Criteria</h1>
        <hr style="margin:10px 0px;" />
        <div class="block-xs"></div>
        <div class="field-group">
            <label for="inputStartDate" class="form-label width-md">Date Range</label>
            <input type="text" class="width-xs datepickerInput" id="inputStartDate" />
            To
            <input type="text" class="width-xs datepickerInput" id="inputEndDate" />
        </div>
        <div class="field-group">
            <label class="form-label width-md">Filter Criteria</label>
            <select id="selectFilter" onchange="loadFilterOptions(this);" class="width-xs" style="padding:3px;">
                <option value="0">None</option>
                <option value="1">Action</option>
                <option value="2">Status</option>
            </select>
            &nbsp;
            <div id="FilterOptionDiv" style="vertical-align:top;display:inline-block;">
            
            </div>
        </div>
        <div class="field-group">
            <label class="form-label width-md">Module</label>
            <select id="selectModule" class="width-lg" style="padding:3px;">
                
            </select>
        </div>
        <div class="field-group">
            <label class="form-label width-md">User</label>
            <select id="selectUser" class="width-lg" style="padding:3px;">
                
            </select>
        </div>
        <div class="block-xs"></div>
        <div class="field-group">
            <label class="form-label width-md"></label>
            <button class="btn btn-primary" id="RetrieveBtn" onclick="RetrieveAuditLog();">Retrieve</button>            
            <button class="btn btn-primary" onclick="ResetAll();">Reset</button>
        </div>
    </div>
    <div class="block-md"></div>
    <div id="searchResultDiv" style="display:none;">
        <h1 class="h1">Audit Log</h1>
        <hr style="margin:10px 0px;" />
        <ul id="pagination" class="pagination"></ul>
        <table id="tblAuditLog" class="table table-bordered">
            <thead>
                <tr>
                    <th>Created Date/Time</th>
                    <th>Module</th>
                    <th>Action</th>
                    <th>Maker</th>
                    <th>Status</th>
                    <th></th>          
                </tr>
            </thead>
            <tbody>
            </tbody>
        </table>
    </div>
</div>

<div id="task-list-modal-bg" style="display:none;"></div>
<div id="task-list-modal-wrapper" style="display:none;">
    <div id="task-list-modal">
        <div id="task-list-modal-header">
            <span id="task-list-modal-header-title">
                Audit Log Details
            </span>
            <span id="task-list-modal-header-close">
                <i class="fa fa-times" onclick="CloseModal()"></i>
            </span>
        </div>
        <div id="task-list-modal-body">
            <div id="task-list-modal-body-content">
                <div style="padding: 8px;">
                    <div style="display:inline-block;vertical-align:top;width:calc(49% - 2px);">
                        <table class="form-table">
                            <tr>
                                <td>Module<span class="spanColon">:</span></td>
                                <td><span id="taskList-Module"></span></td>
                            </tr>
                            <tr>
                                <td>Action<span class="spanColon">:</span></td>
                                <td><span id="taskList-Action"></span></td>
                            </tr>
                            <tr>
                                <td>Maker<span class="spanColon">:</span></td>
                                <td><span id="taskList-Maker"></span></td>
                            </tr>
                        </table>
                    </div>
                    <div style="display:inline-block;vertical-align:top;width:calc(49% - 2px);">
                        <table class="form-table">
                            <tr>
                                <td>Status<span class="spanColon">:</span></td>
                                <td><span id="taskList-Status"></span></td>
                            </tr>
                            <tr>
                                <td>Date/Time<span class="spanColon">:</span></td>
                                <td><span id="taskList-CDate"></span></td>
                            </tr>
                        </table>
                    </div>
                </div>
                <div id="old-new-data-div">
                    <table>
                        <tr>
                            <td>
                                <div>
                        
                                </div>
                            </td>
                            <td>
                                <div>
                        
                                </div>
                            </td>
                        </tr>
                    </table>                   
                </div>
                <div id="taskList-Maker-Approval" style="padding:8px;">
                    <table class="form-table">
                        <tr>
                            <td>Approval<span class="spanColon">:</span></td>
                            <td><span id="taskList-Approval"></span></td>
                        </tr>
                        <tr>
                            <td>Date/Time<span class="spanColon">:</span></td>
                            <td><span id="taskList-RDate"></span></td>
                        </tr>
                        <tr>
                            <td>Checker<span class="spanColon">:</span></td>
                            <td><span id="taskList-Checker"></span></td>
                        </tr>                            
                        <tr id="taskList-Remark-Row">
                            <td>Remark<span class="spanColon">:</span></td>
                            <td><span id="taskList-Remark"></span></td>
                        </tr>
                    </table>
                </div>                                  
                <div style="padding:8px;">
                    <div>
                        <button class="btn btn-default taskList-btn" onclick="CloseModal()">Close</button> 
                    </div>
                </div>
            </div>
            <div id="task-list-modal-loader" style="display:none;">
                <table style="height:100%;width:100%;text-align:center;">
                    <tr>
                        <td><img src="../assets/images/loader.gif" /></td>
                    </tr>
                </table>
            </div>
        </div>
    </div>
</div>

<script src="/views/reports/auditTrail/main.js"></script>