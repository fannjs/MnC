<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="main.aspx.cs" Inherits="Maestro.views.kioskMaintenance.softwareDistribution.main" %>

<style type="text/css">
#navigation-bar > ul
{
    list-style: none;
    margin: 0;
    padding: 0;
}
#navigation-bar > ul  > li
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
#downloadedRemark
{
    padding: 0px 8px;
    margin-top: -8px;
    margin-bottom: 8px;
    color: #4264E0;
}
</style>
<link rel="stylesheet" type="text/css" href="../../../assets/styles/bootstrap-datetimepicker.css" />
<script src="../../../assets/scripts/kioskMaintenance/bootstrap-datetimepicker.js"></script>
<link rel="Stylesheet" href="../../../components/bootstrap.datepicker/css/datepicker.css" />
<script src="../../../components/bootstrap.datepicker/js/bootstrap-datepicker.js"></script>
<script type="text/javascript" src="kioskMaintenance/softwareDistribution/main.js"></script>
<div id="software-distribution-main-div">
    <div id="task-list-header">
        <div id="navigation-bar">
            <ul>
                <li class="active" data-href="#software-list-main" data-type="S">Software Version List</li>
                <li data-href="#queue-list-main" data-type="Q">Queue List</li>
            </ul>
        </div>
    </div>
    <div style="height:5px;border-top:1px solid #EEE;"></div>
    <div id="software-distribution-main-content">
        <div id="software-list-main">
            <div class="block-xs"></div>
            <a class="fn-btn" onclick="openModal(this);" data-target=".add-version" data-title="New Software Version"><i class="fa fa-plus"></i>New Software Version</a>
            <div style="height:5px;"></div>
            <div>
                <table id="softwareListTbl" class="table table-bordered">
                    <thead>
                        <th>No.</th>
                        <th>Version</th>
                        <th>File Name</th>
                        <th>Checksum</th>
                        <th></th>
                    </thead>
                    <tbody>
                    </tbody>
                </table>
            </div>
        </div>
        <div id="queue-list-main" style="display:none;">
            <div class="block-xs"></div>
            <a class="fn-btn" onclick="openModal(this);InitialAddQueue();" data-target=".add-queue" data-title="New Software Queue"><i class="fa fa-plus"></i>New Software Queue</a>
            <div style="height:5px;"></div>
            <div>
                <table id="queueList" class="table table-bordered">
                    <thead>
                        <th>No.</th>
                        <th>Kiosk ID</th>
                        <th>Version</th>
                        <th>Download At</th>
                        <th>Downloaded At</th>
                        <th>Deploy At</th>
                        <th></th>
                    </thead>
                    <tbody>
                    </tbody>
                </table>
            </div>
        </div>
    </div>
</div>

<!-- Modal here -->
<div class="modal" id="software-distribution-modal" tabindex="-1" role="dialog" aria-labelledby="myModalLabel" aria-hidden="true">
    <div class="modal-dialog" style="width:800px;">
        <div class="modal-content">
            <div class="modal-header">
                <span class="close" data-dismiss="modal"><span aria-hidden="true">&times;</span><span class="sr-only">Close</span></span>
                <h4 class="modal-title" id="modal-title"></h4>
            </div>
            <div class="modal-body">
                <div class="add-version" style="display:none;">
                    <div class="field-group">
                        <label for="inputVersion" class="form-label width-md">Version</label>
                        <input type="text" class="input-field width-md" id="inputVersion" placeholder="Version" maxlength="6">
                    </div>  
                    <div class="field-group">
                        <label for="inputChecksum" class="form-label width-md">Checksum</label>
                        <input type="text" class="input-field width-xxl" id="inputChecksum" placeholder="Checksum" maxlength="64">
                    </div>
                    <div class="field-group">
                        <label for="inputFile" class="form-label width-md">File</label>
                        <input type="file" class="input-field width-xxl" style="display:inline-block" id="inputFile">
                    </div>        
                </div>
                <div class="add-queue" style="display:none;">
                    <div class="field-group">
                        <label for="selectVersion" class="form-label width-md">Version</label>
                        <select id="selectVersion" class="input-field width-md">
                        
                        </select>
                    </div>  
                    <div class="field-group">
                        <label for="selectDownloadSUID" class="form-label width-md">Download Time</label>
                        <select id="selectDownloadSUID" class="input-field width-lg select-update-type" data-target="#inputDownloadTime" style="width:250px">
                        
                        </select>
                        <input type="text" class="form-control input-field width-lg input-datepicker" data-target="#selectDownloadSUID" id="inputDownloadTime" placeholder="Download Time" style="display:inline-block;vertical-align:top;line-height:normal;border-radius:2px;height:35px;">
                    </div>
                    <div class="field-group">
                        <label for="selectDeploySUID" class="form-label width-md">Deploy Time</label>
                        <select id="selectDeploySUID" class="input-field width-lg select-update-type" data-target="#inputDeployTime" style="width:250px">
                        
                        </select>
                        <input type="text" class="form-control input-field width-lg input-datepicker" data-target="#selectDeploySUID" id="inputDeployTime" placeholder="Deploy Time" style="display:inline-block;vertical-align:top;line-height:normal;border-radius:2px;height:35px;">
                    </div>
                    <div class="block-xs"></div>
                    <div class="field-group">
                        <h4 style="border-bottom:1px solid #EEE;padding-bottom:5px;">Select Kiosk ID</h4>                
                        <div id="selectMachineDiv">
                            
                        </div>
                    </div>
                </div>
                <div class="edit-queue" style="display:none;">
                    <div class="field-group">
                        <label for="editKioskId" class="form-label width-md">Kiosk ID</label>
                        <span type="text" id="editKioskId"></span>
                    </div>
                    <div class="field-group">
                        <label for="editVersion" class="form-label width-md">Version</label>
                        <span type="text" id="editVersion"></span>
                    </div>                      
                    <div class="field-group">
                        <label for="editDownloadSUID" class="form-label width-md">Download Time</label>
                        <select id="editDownloadSUID" class="input-field width-lg select-update-type" data-target="#editDownloadTime" style="width:250px">
                        
                        </select>
                        <input type="text" class="form-control input-field width-lg input-datepicker" data-target="#editDownloadSUID" id="editDownloadTime" placeholder="Download Time" style="display:inline-block;vertical-align:top;line-height:normal;border-radius:2px;height:35px;">
                    </div>  
                    <div id="downloadedRemark" style="display:none;"><label class="width-md"></label><span></span></div>                  
                    <div class="field-group">
                        <label for="editDeploySUID" class="form-label width-md">Deploy Time</label>
                        <select id="editDeploySUID" class="input-field width-lg select-update-type" data-target="#editDeployTime" style="width:250px">
                        
                        </select>
                        <input type="text" class="form-control input-field width-lg input-datepicker" data-target="#editDeploySUID" id="editDeployTime" placeholder="Deploy Time" style="display:inline-block;vertical-align:top;line-height:normal;border-radius:2px;height:35px;">
                    </div>                    
                </div>
            </div>
            <div class="modal-footer" id="modal-footer">        
                <button class="btn btn-primary add-version functional-btn" onclick="addNewVersion();">Add</button>
                <button class="btn btn-primary add-queue functional-btn" onclick="addNewQueue(this);">Add</button>
                <button class="btn btn-primary edit-queue functional-btn" onclick="editQueue(this);">Update</button>
                <button class="btn btn-default" data-dismiss="modal">Close</button>
            </div>
        </div>
    </div>
</div>
    
<div id="statusMsg" style="display:none;" tabindex="1">
    <table style="height: 100%;width:100%;text-align:center;">
        <tr>
            <td><span style="padding-right: 10px;">Uploading</span><i style="width:auto;" class="fa fa-spinner fa-spin"></i></td>
        </tr>
    </table>
</div>