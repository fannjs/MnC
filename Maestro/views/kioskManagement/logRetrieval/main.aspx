<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="main.aspx.cs" Inherits="Maestro.views.kioskManagement.logRetrieval.main" %>

<link rel="Stylesheet" href="../../../components/bootstrap.datepicker/css/datepicker.css" />
<link rel="Stylesheet" href="/views/kioskManagement/logRetrieval/main.css" />

<script src="../../../components/bootstrap.datepicker/js/bootstrap-datepicker.js"></script>
<div>
    <div id="selectionDiv" class="well-div">
        <h1 class="h1">Select Filter</h1>
        <hr style="margin:10px 0px;" />
        <div class="block-xs"></div>
        <div class="field-group">
            <label for="selectLogType" class="form-label width-md">Kiosk Log Type</label>
            <div style="display:inline-block;vertical-align:top;">
                <input type="checkbox" class="kioskLogCB" id="eventLogCB" /><label class="checkboxLabel" for="eventLogCB">Event Log</label> &nbsp;&nbsp;
                <input type="checkbox" class="kioskLogCB" id="transactionLogCB" /><label class="checkboxLabel" for="transactionLogCB">Transaction Log</label> &nbsp;&nbsp;
                <input type="checkbox" class="kioskLogCB" id="fileTransferLogCB" /><label class="checkboxLabel" for="fileTransferLogCB">File Transfer Log</label>
            </div>
        </div>
        <div class="field-group">
            <label for="inputStartDate" class="form-label width-md">Start Date</label>
            <input type="text" class="datepickerInput" id="inputStartDate" />
        </div>
        <div class="field-group">
            <label for="inputEndDate" class="form-label width-md">End Date</label>
            <input type="text" class="datepickerInput" id="inputEndDate" />
        </div>
        <div class="field-group">
            <label class="form-label width-md">Select Kiosk</label>
            <div id="selectMachineDiv" style="display:inline-block;vertical-align:top;width:calc(100% - 200px);max-height:100px;overflow:auto;">
            
            </div>
        </div>
        <div class="block-xs"></div>
        <div class="field-group">
            <label class="form-label width-md"></label>
            <button class="btn btn-primary" onclick="addToList(this);">Add to List</button>
        </div>
    </div>
    <div class="block-md"></div>
    <div>
        <div>
            <h4 style="display:inline-block;">Kiosk List</h4>
            <div style="float:right;">
                <a class="fn-btn" onclick="openSendLogConfig();" title="Send Logs via Email"><i class="fa fa-envelope"></i>Send Logs</a>
                <span class='center-divider'></span> 
                <a class="fn-btn" title="Download All Logs"><i class="fa fa-arrow-down"></i>Download All Logs</a>
                <span class='center-divider'></span> 
                <a class="fn-btn" onclick="clearAllLog();" title="Remove All Logs"><i class="fa fa-eraser"></i>Clear All</a>
            </div>
        </div>        
        <table class="table table-bordered" id="tblLogList">
            <thead>
                <tr>
                    <th>Kiosk ID</th>
                    <th>Start Date</th>
                    <th>End Date</th>
                    <th>Event Log</th>
                    <th>Transaction Log</th>
                    <th>File Transfer Log</th>
                    <th></th>
                </tr>
            </thead>
            <tbody>
            </tbody>
        </table>
    </div>
</div>
<!-- Modal Here -->
<div id="modal-wrapper">
    <div class="modal-dialog">
        <div class="modal-header">
            <span class="modal-title"></span>
            <span class="modal-close-btn" onclick="closeModal();"><i class="fa fa-times" style="width:auto;"></i></span>
        </div>
        <div id="modal-content" style="padding:8px;">
            <div class="sub-content" data-name="LogViewer"> 
                <div id="date-selection-div">
                    <div class="custom-table" style="width:250px;border: 1px solid #CCC;">
                        <div class="custom-table-header">
                            <ul>
                                <li>
                                    <span>Date</span>
                                    <span></span>
                                </li>
                            </ul>
                        </div>
                        <div class="custom-table-body" style="max-height: 100px;">
                            <ul>
                                
                            </ul>
                        </div>
                    </div>
                </div>                
            </div>
            <div class="sub-content" data-name="SendEmail">
                <div class="field-group">
                    <label for="inputRecipient" class="form-label width-md">To</label>
                    <input type="text" class="input-field width-xl" id="inputRecipient" />
                </div>
                <div class="field-group">
                    <label for="inputSubject" class="form-label width-md">Subject</label>
                    <input type="text" class="input-field width-xl" id="inputSubject" />
                </div>
                <div class="field-group">
                    <label for="inputEmailText" class="form-label width-md">Email Text</label>
                    <textarea rows="6" class="input-field width-xl" id="inputEmailText"></textarea>
                </div>
                <div class="field-group">
                    <label for="inputAttachment" class="form-label width-md">Attachments</label>
                    <input type="file" class="input-field width-xl" id="inputAttachment" style="display:inline-block" />
                </div>
                <hr style="margin:10px 0px;" />
                <div>
                    <label for="inputAttachment" class="form-label width-md"></label>
                    <button class="btn btn-primary">Send</button>
                    <button class="btn btn-default" onclick="closeModal();">Cancel</button>
                </div>
            </div>
        </div>
    </div>   
</div>

<script type="text/javascript" src="/views/kioskManagement/logRetrieval/main.js"></script>