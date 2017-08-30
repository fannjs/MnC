<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="main.aspx.cs" Inherits="Maestro.views.kioskMaintenance.adsMaintenance.main" %>

<style type="text/css">

#uploadDiv
{
    display: inline-block;
    width: 170px;
    height: 150px;
    position: relative;
    background-color: #3EC9B5;
    border-radius: 8px;
    color: #FFF;
    margin: 4px;
}
#uploadDiv-text
{
    width: 100%;
    height: 100%;
    position: absolute;
    top: 0px;
    text-align: center;
}
#uploadDiv-text .fa-plus
{
    font-size: 24px;
}
#fileUploader
{
    width: 100%;
    height: 100%;
    opacity: 0;  
    position: absolute;
}
#fileUploader:hover
{
    cursor:pointer;
}
#advertisement-uploaded-div ul
{
    padding: 0px;
    margin: 0px;
    list-style: none;
}
#advertisement-uploaded-div li
{
    display: inline-block;
    vertical-align: top;
}
.image-outer-wrapper
{
    float:left;
    width: 170px;
    height: 150px;
    position: relative;
    margin: 4px;
}
.hasSequence .image-div
{
    opacity: 1;
}
.hasSequence .seqCounter
{
    opacity: 1;
}
.hasSequence .seqChecked
{
    background-color: #2EB54B;
    color: #F5F5F5;
    opacity: 1;
}
.hasSequence .seqChecked:hover
{
    background-color: #37AC50;
    color: #EEE;
}
.image-wrapper-header
{
    position: absolute;
    top: 0;
    width: 100%;
    z-index: 10;
}
.thumbnailBtn, .seqCounter
{
    opacity: 0;
    padding: 6px;
    width: 22px;
    height: 23px;
    text-align: center;
    background-color: rgba(0, 0, 0, 0.5);
    color: #FFF;
    line-height: 10px;
    font-size: 10px;
    border-radius: 4px;
    transition: background-color ease-in-out 0.15s , color ease-in-out 0.15s, opacity ease-in-out 0.15s;
}
.thumbnailBtn:hover
{
    cursor:pointer;
    background-color: rgba(0, 0, 0, 0.8);
    color: #FFF;
}
.removeAdvertisement
{
    margin-top: 2px;
    margin-right: 3px;
    float: right;
}
.seqChecked
{
    margin-top: -11px;
    margin-right: 3px;
    float: right;
}
.seqCounter
{
    float: left;
    margin-top: -11px;
    margin-left: 3px;
    color: #FFF;
    font-size: 12px;
    font-weight: 600;
}
.image-outer-wrapper:hover .thumbnailBtn
{
    opacity: 1;
}
.image-wrapper-body
{
    width: 100%;
    height: 100%;
}

.image-div
{
    opacity: 0.4;
    width: 100%;
    height: 100%;
    border-radius: 8px;    
    border: 1px solid #CCC;
    
    transition: opacity ease 0.2s;
    -webkit-transition: opacity ease 0.2s;
}
.image-wrapper-footer
{
    position: absolute;
    width:100%;
    top: 90%;
    z-index: 9999;
}
.thumbnailBtn > .fa
{
    width: auto;
}

#statusMsg
{
    position: fixed;
    top: 0;
    left: 0;
    padding: 0;
    margin: 0;
    height: 100%;
    width: 100%;
    z-index: 10000;
    background-color: rgba(0,0,0,0.4);
    color: #FFF;
    font-size: 16px;
}

.uploadingMsg
{
    background-color: #2380D2;
    border: 1px solid #2380D2;
}

.successMsg
{
    background-color: #2FC64B;
    border: 1px solid #2FC64B;
}

.errorMsg
{
    background-color: #F00;
    border: 1px solid #F00;
}

#optionDiv table
{
    width: 100%;
}
#optionDiv table thead tr th span
{
    position: absolute;
    background-color: #FFF;
    font-weight: bold;
    font-size: 14px;
    margin-top: -10px;
    margin-left: 12px;
}
#optionDiv table tbody tr td
{
    border: 1px solid #CCC;
    padding: 16px 12px 8px;
}
#optionDiv table tbody tr td:first-child
{
    width: 150px;
}
#optionDiv table tbody tr td a
{
    cursor: pointer;
}
#advertisement-div-header
{
    padding: 6px 12px;
    font-size: 16px;
    border: 1px solid #CCC;
    background: linear-gradient(#fff, #ccc);
    border-bottom-color: #AAA;
}
#advertisement-div-body
{
    border: 1px solid #CCC;
    border-top: 0;
}
#advertisement-button-div
{
    padding: 4px;
    text-align: right;
    background-color: #EEE;
    border-bottom: 1px solid #DDD;
}
#advertisement-uploaded-div
{
    padding: 8px;
}

#closeSelectMachineModalBtn
{
    opacity: 0.5;
    float: right;
    font-size: 16px;
    margin-top: -5px;
    
    transition: opacity ease-in-out 0.25s;
    -webkit-transition: opacity ease-in-out 0.25s;
}
#closeSelectMachineModalBtn:hover
{
    cursor: pointer;
    opacity: 1;
}

#machineSelectionHideDiv
{
    border: 1px solid #DDD;
    margin-top: 10px;
    padding: 12px;
    display:none;
}
#selectMachineDiv
{
    height: 150px;
    overflow: auto;
}
#selectMachineDiv > .a-machine
{
    display: inline-block;
    padding: 1px 12px;
    border: 1px solid #999;
    margin: 2px;
    border-radius: 4px;
    min-width: 80px;
    text-align: center;
}
#selectMachineDiv > .a-machine:hover
{
    cursor: pointer;
}
#selectMachineDiv > .a-machine.selectedMachine
{
    background-color: #5cb85c;
    border-color: #4cae4c;
    color: #fff;
    font-weight: bold;
}

#machine-detail-tooltip
{
    position: fixed;
    border-radius: 3px;
    padding: 8px;
    z-index: 9999;
    background-color: #FFF;
    margin-left: -30px;
    margin-top: 20px;
    box-shadow: 2px 2px 4px 0px rgba(0,0,0,0.2);
    
    border-width: 1px;
    border-style: solid;
    border-color: #ddd #ccc #bbb;
}
.machine-detail-branch-code,
.machine-detail-branch-name
{
    padding: 4px 8px;
    color: #316CE5;
    font-weight: bold;
}
.machine-detail-address
{
    padding: 4px 8px;
    color: #0091FF;
    display:block;
}

.specific-adv
{
    display: inline-block;
    border: 1px solid #0792CF;
    background-color: #BBE5F8;
    border-radius: 3px;
    padding: 8px;
    color: #333;
    
    transition: opacity ease-in-out 0.25s;
    -webkit-transition: opacity ease-in-out 0.25s;
}
.specific-adv:hover
{
    cursor:pointer;
}
.removeSpecificAdv
{
    float: right;
    margin-top: -20px;
    margin-right: -22px;
    color: red;
    font-size: 14px;
    opacity: 1;
}
.removeSpecificAdv:hover
{
    cursor:pointer;
}


.modalWrapper
{
    position: fixed;
    border: 0;
    padding: 0;
    margin: 0;
    top: 0;
    left: 0;
    height: 100%;
    width:100%;
    background-color: rgba(0,0,0,0.4);
    
    z-index: 9999;
}
.modalDialog
{
    width: 500px;
    margin: 2em auto;
    background-color: #FFF;
    box-shadow: 0 0 8px 1px rgba(0,0,0,0.2);
    border: 1px solid #CCC;
    border-radius: 2px;
}
#closeModalBtn
{
    float: right;
    font-size: 16px;
    opacity: 0.5;
    
    transition: opacity ease-in-out 0.25s;
    -webkit-transition: opacity ease-in-out 0.25s;
}
#closeModalBtn:hover
{
    cursor:pointer;
    opacity: 1;
}

#navigation-bar ul
{
    list-style: none;
    margin: 0;
    padding: 0;
}
#navigation-bar li
{
    display: inline-block;
    padding: 4px 8px;
    margin: 0px 4px;
    font-size: 14px;
    opacity: 0.3;
    cursor: pointer;
    transition: border-bottom ease 0.15s, opacity ease 0.15s;
    -webkit-transition: border-bottom ease 0.15s, opacity ease 0.15s;
}
#navigation-bar > ul  > li:hover
{
    opacity: 0.5;
}
#navigation-bar > ul  > li.active
{
    color: #1B6BB6;
    font-weight: 600;
    opacity: 1;
    cursor:default;
}
</style>
<link rel="stylesheet" type="text/css" href="../../../assets/styles/bootstrap-datetimepicker.css" />
<script src="../../../assets/scripts/kioskMaintenance/bootstrap-datetimepicker.js"></script>
<script type="text/javascript" src="/views/kioskMaintenance/adsMaintenance/main.js"></script>
<div id="advertisement-maintenance-main-div">
    <div id="navigation-bar">
        <ul>
            <li class="active" data-href="#thumbnailViewDiv">Configuration</li>
            <li data-href="#advQueueViewDiv">Queue List</li>
        </ul>
    </div>
    <div class="block-md" style="border-top:1px solid #EEE;"></div>
    <div id="thumbnailViewDiv" class="tab-content">
        <div id="optionDiv">
            <table>
                <thead>
                    <tr>
                        <th><span>Default</span></th>
                        <th><span>Exception</span></th>
                    </tr>
                </thead>
                <tbody>
                    <tr>
                        <td>
                            <a data-gid="1" onclick="loadGroupAdv(this);">All Kiosk</a>
                        </td>
                        <td>
                            <div id="specific-adv-div" style="display:inline-block;">
                            </div>
                            <a onclick="triggerSpecificConfig();">Click if Kiosk Advertisement differs</a>
                        </td>
                    </tr>
                </tbody>
            </table>
        </div>
        <div id="machineSelectionHideDiv">
            <div>
                <h4 style="display:inline-block;margin:0px;">Select Kiosk ID</h4>
                <span id="closeSelectMachineModalBtn" onclick="closeSelectMachineModal();"><i style="width:auto" class="fa fa-times"></i></span>
            </div>
            <div class="block-xs"></div>
            <div id="selectMachineDiv">
            
            </div>
        </div>
        <div class="block-xs"></div>
        <div>
            <div id="advertisement-div-header">
                <span>Advertisement Assignment</span>
            </div>            
            <div id="advertisement-div-body">
                <div id="advertisement-button-div">
                    <button class="btn btn-danger advConfigBtn" id="deleteSpecificAdv" onclick="deleteSpecificAdv(this);" style="display:none;">Delete</button>
                    <button class="btn btn-primary advConfigBtn" onclick="LoadQueueConfigDiv();">Save</button>
                </div>
                <div id="advertisement-uploaded-div">
                    <ul>
                        <li id="liUpload">
                            <div id="uploadDiv">
                                <div id="uploadDiv-text">
                                    <div style="margin-top:48px;">
                                        <i class="fa fa-plus"></i><br />
                                        Photos & Videos
                                    </div>
                                </div>
                                <input type="file" id="fileUploader" multiple />
                            </div>
                        </li>
                    </ul>
                </div>
            </div>            
        </div>
    </div>
    <div id="advQueueViewDiv" class="tab-content" style="display:none;">
        <table id="advertisement-queue-table" class="table table-bordered">
            <thead>
                <tr>
                    <th>No.</th>
                    <th>Kiosk ID</th>
                    <th>Group ID</th>
                    <th>Sequence</th>
                    <th>Download Date/Time</th>
                    <th>Deploy Date/Time</th>
                </tr>
            </thead>
            <tbody>
            </tbody>
        </table>
    </div>
</div>
<div id="modalWrapper" class="modalWrapper" style="display:none;">
    <div id="configureQueueDiv" class="modalDialog">
        <div class="block-md"><span id="closeModalBtn" onclick="closeQueueConfig();"><i class="fa fa-times"></i></span></div>
        <div class="block-xs"></div>
        <div style="padding:12px;">
            <div class="field-group">
                <label class="form-label width-lg">Download Date Time</label>
                <input type="text" id="inputDownloadDT" class="form-control width-lg dtpicker-input" style="display:inline-block" />
            </div>
            <div class="field-group">
                <label class="form-label width-lg">Deploy Date Time</label>
                <input type="text" id="inputDeployDT" class="form-control width-lg dtpicker-input" style="display:inline-block"  />
            </div>
            <div class="block-xs"></div>
            <hr style="margin:10px 0px;" />
            <div>
                <label class="width-lg"></label>
                <button class="btn btn-primary" id="saveSequenceSubmit" onclick="saveSequence(this);">Submit</button>
                <button class="btn btn-danger" id="confirmDeleteSpecificAdv" onclick="confirmDeleteSpecificAdv(this);">Confirm Delete</button>
                <button class="btn btn-default" onclick="closeQueueConfig();">Cancel</button>
            </div>
        </div>
    </div>
</div>
<div id="statusMsg" style="display:none;">
    <table style="height: 100%;width:100%;text-align:center;">
        <tr>
            <td><span style="padding-right: 10px;">Uploading</span><i style="width:auto;" class="fa fa-spinner fa-spin"></i></td>
        </tr>
    </table>
</div>
