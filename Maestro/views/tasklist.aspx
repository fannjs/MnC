<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="tasklist.aspx.cs" Inherits="Maestro.views.tasklist" %>
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
#task-list-main-content
{
    padding-top: 5px;
}
#task-list-checker > table > tbody > tr:hover,
#task-list-maker > table > tbody > tr:hover
{
    cursor:pointer;
    background-color: #EEE;
}
#task-list-modal-wrapper 
{
    position:fixed;
    top: 0;
    left: 0;
    width: 100%;
    height: 100%;
    z-index: 1000;
    overflow:auto;
}
#task-list-modal-bg
{
    position: fixed;
    height: 100%;
    width: 100%;
    padding: 0;
    margin: 0;
    top: 0;
    left: 0;
    background-color: rgba(0,0,0,0.4);
    z-index: 800;
}
#task-list-modal
{
    width: 800px;
    margin: 2em auto;
    background-color: #FFF;
    box-shadow: 0px 4px 8px rgba(0,0,0,0.5);
}
#task-list-modal-header
{
    padding: 5px 10px;
    background: linear-gradient(#FFF,#CCC);
    border-bottom: 1px solid #B7B7B7;
}
#task-list-modal-header-title
{
    color: #000;
    font-size: 16px;
}
#task-list-modal-header-close
{
    float:right;
}
#task-list-modal-header-close > .fa
{
    width: auto;
}
#task-list-modal-header-close:hover
{
    cursor:pointer;
}
#task-list-modal-body
{
    position: relative;
    min-height: 200px;
}
#task-list-modal-loader
{
    position: absolute;
    top: 0;
    left: 0;
    height: 100%;
    width: 100%;
}
#task-list-modal-footer
{
    padding: 4px;
}
#task-list-modal-footer > button
{
    margin:4px;
}
.form-table
{
    width: auto;
}
.form-table td
{
    vertical-align: top;
    padding: 4px;
}
.form-table tr > td:first-child
{
    font-weight: bold;
    text-align: right;
}
.spanColon
{
    padding-left: 10px;
}
.radioBtnLabel
{
    font-weight: normal;
}
.radioBtnLabel:hover
{
    cursor: pointer;
}
#taskList-Textarea-Remark
{
    padding: 4px;
    border-radius: 2px;
    border: 1px solid #CCC;
    resize: none;
}
#old-new-data-div
{
    background-color: #EEE;
    padding: 8px 4px;
    border-top: 1px solid #DDD;
    border-bottom: 1px solid #DDD;
}
#old-new-data-div > table > tbody > tr > td > div
{
    padding: 8px;
}
#old-new-data-div > div
{
    border-left: 1px solid #DDD;
}
#old-new-data-div > table > tbody > tr > td > div .form-table td:first-child
{
    width: 40%;
    text-align: left;
}
#old-new-data-div > table > tbody > tr > td > div .form-table td:last-child
{
    width: 60%;
}
#old-new-data-div > table > tbody > tr > td > div .form-table td
{
    word-wrap: break-word;
}
#old-new-data-div > table > tbody > tr > td > div .form-table
{

}
#old-new-data-div > table 
{
    width:100%;
}
#old-new-data-div > table > tbody > tr > td
{
    vertical-align:top;
    width: 50%;
}
#old-new-data-div > table > tbody > tr > td:nth-child(2)
{
    border-left: 1px solid #DDD;
}
.mkck-thumbnail-div
{
    margin: 1px 0px;
}
.mkck-thumbnail
{
    width: 150px;
    height: 138px;
    border: 1px solid #000;
    border-radius: 4px;
}

#pagination
{
    margin-top: 0px;
    margin-bottom: 5px;
    float: left;
    display:none;
}
#pagination > li
{
    cursor: pointer;
}
#pagination active
{
    cursor: default;
}
#record-per-page
{
    margin: 0px 5px;
    padding: 5px 8px;
    background-color: #fff;
    border: 1px solid #ddd;
}
#page-footer
{
    margin-top: 20px;
    text-align: right;
}
#page-information
{
    color: #000;
    opacity: 0.5;
    font-size: 14px;
}
</style>
<script src="../assets/scripts/tasklist.js" type="text/javascript"></script>
<div>
    <div id="task-list-header">
        <div id="navigation-bar">
            <ul>
                <li class="active" data-href="#task-list-checker" data-type="C">Checker List</li>
                <li data-href="#task-list-maker" data-type="M">My Action List</li>
            </ul>
        </div>
    </div> 
    <div style="height:5px;border-top:1px solid #EEE;"></div>   
    <div id="task-list-main-content">
        <ul id="pagination" class="pagination"></ul>
        <div id="task-list-checker">
            <table id="task-list-checker-table" class="table table-bordered">
                <thead>
                    <tr>
                        <%--<th>No.</th>--%>
                        <th>Created Date/Time</th>
                        <th>Module</th>
                        <th>Action</th>
                        <th>Maker</th>
                        <th>Status</th>
                    </tr>
                </thead>
                <tbody>
                </tbody>
            </table>
        </div>
        <div id="task-list-maker" style="display:none;">
           <table id="task-list-maker-table" class="table table-bordered">
                <thead>
                    <tr>
                        <th>Created Date/Time</th>
                        <th>Module</th>
                        <th>Action</th>
                        <th>Checker</th>
                        <th>Approval Date/Time</th>
                        <th>Status</th>
                    </tr>
                </thead>
                <tbody>
                </tbody>
            </table>
        </div>
    </div>
</div>
<div id="task-list-modal-bg" style="display:none;"></div>
<div id="task-list-modal-wrapper" style="display:none;">
    <div id="task-list-modal">
        <div id="task-list-modal-header">
            <span id="task-list-modal-header-title">
                Action Details
            </span>
            <span id="task-list-modal-header-close">
                <i class="fa fa-times" onclick="closeTaskListModal()"></i>
            </span>
        </div>
        <div id="task-list-modal-body">
            <div id="task-list-modal-body-content">
                <div style="padding: 8px;">
                    <input type="hidden" id="taskList-ItemID" />
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
                <div style="padding:8px;" id="taskList-Checker-div">
                    <div id="taskList-Checker-Approval">
                        <table class="form-table">
                            <tr>
                                <td>Approval<span class="spanColon">:</span></td>
                                <td>
                                    <input type="radio" id="taskList-Approval-first" name="taskList-Approval" value="2" /><label for="taskList-Approval-first" class="radioBtnLabel">Approve</label>
                                    &nbsp;
                                    <input type="radio" id="taskList-Approval-second" name="taskList-Approval" value="3" /><label for="taskList-Approval-second" class="radioBtnLabel">Reject</label>
                                </td>
                            </tr>
                            <tr>
                                <td>Remark<span class="spanColon">:</span></td>
                                <td><textarea id="taskList-Textarea-Remark" cols="50" rows="4" disabled></textarea></td>
                            </tr>
                        </table>                        
                    </div>
                    <div id="taskList-Maker-Approval" style="display: none;">
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
                </div>
                <div style="padding:8px;">
                    <div>
                        <button class="btn btn-primary taskList-function-btn taskList-btn" onclick="confirmCheckerApproval()">Confirm</button>
                        <button class="btn btn-primary taskList-function-btn taskList-btn" onclick="notifiedRejection()">Noted</button>
                        <button class="btn btn-danger taskList-function-btn taskList-btn pull-right" onclick="deleteAction()"><i class="fa fa-trash-o"></i>Delete Action</button>
                        &nbsp;
                        <button class="btn btn-default taskList-btn" onclick="closeTaskListModal()">Close</button> 
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