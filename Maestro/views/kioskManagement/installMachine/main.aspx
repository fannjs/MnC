<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="main.aspx.cs" Inherits="Maestro.views.kioskManagement.installMachine.main" %>


<style type="text/css">
    #machine-model-name,
    #install-machine-site,
    #install-machine-country,
    #install-machine-state,
    #install-machine-district,
    #install-machine-branch{
        font-weight:bold;
    }

    #installMachine-modal .modal-dialog{
        width: 700px;
    }

    .modal-body p{
        font-size: 13px;
    }
    #tblMachTempList th
    {
        background-color: #333;
        color: #fff;
    }
    #tblMachTempList .clickable:hover
    {
        background-color: #F6F6F6;
    }
    
    .input-field[readonly]
    {
        background-color: #F4f4f4;
        opacity: 0.5;
    }
    
    #tblMachTempList
    {
        width: 800px;
    }
    @media (max-width: 1120px)
    {
        #tblMachTempList
        {
            width:100%;
        }
    }
</style>
<div>
    <form role="form">
        <fieldset>
            <div id="selectionDiv" class="well-div">
                <h1 class="h1">Please select</h1>
                <div class="block-md"></div>
                <div class="field-group">
                    <label for="selectInstallMachineSite" class="form-label width-xs">Site</label>
                    <select class="input-field install-machine-select width-xl" id="selectInstallMachineSite">
                        <option value="0">-Please select-</option>
                    </select>
                </div>
                <div class="field-group">
                    <label for="inputInstallMachineCountry" class="form-label width-xs">Country</label>
                    <input name="inputUsername" type="text" id="inputInstallMachineCountry"  class="input-field width-xl" placeholder="Country" readonly>
                </div>
                <div class="field-group">
                    <label for="selectInstallMachineState" class="form-label width-xs">State</label>
                    <select class="input-field install-machine-select width-xl"  id="selectInstallMachineState">
                        <option value="0">-Please select-</option>
                    </select>
                </div>
                <div class="field-group">
                    <label for="selectInstallMachineDistrict" class="form-label width-xs">District</label>
                    <select class="input-field install-machine-select width-xl"   id="selectInstallMachineDistrict">
                        <option value="0">-Please select-</option>
                    </select>
                </div>
                <div class="field-group">
                    <label for="selectInstallMachineBranch" class="form-label width-xs">Branch</label>
                    <select class="input-field install-machine-select width-xl" id="selectInstallMachineBranch">
                        <option value="0">-Please select-</option>
                    </select>
                </div>
            </div>
            <hr/>
            <div id="machine-list-table" style="display:none;">
                <h1 class="h1">Select a model to install</h1>
                <table class="table table-bordered" id="tblMachTempList">
                    <thead>
                        <tr>
                            <%--<th>#</th>--%>
                            <th>Kiosk Type</th>
                            <th>Kiosk Model</th>
                        </tr>
                    </thead>
                    <tbody>
                    </tbody>
                </table>
            </div>
        </fieldset>
    </form>
</div>
<div class="modal" id="installMachine-modal" tabindex="-1" role="dialog" aria-labelledby="modal-title" aria-hidden="true">
    <div class="modal-dialog">
        <div class="modal-content">
            <div class="modal-header">
                <button type="button" class="close" data-dismiss="modal" aria-hidden="true">&times;</button>
                <h4 class="modal-title" id="modal-title">Install Kiosk</h4>
            </div>
            <div class="modal-body">
                <p>You are installing <span id="machine-model-name"></span> at: 
                    <span id="install-machine-site"></span> > <span id="install-machine-country"></span>
                    > <span id="install-machine-state"></span> > <span id="install-machine-district"></span>
                    > <span id="install-machine-branch"></span></p>
                <hr/>
                <p>
                    Kindly fill in the particulars below:
                </p>
                <form class="form-horizontal" role="form" action="javascript:;">
                    <fieldset>
                        <div class="field-group">
                            <label for="installMachineID" class="form-label width-xs">Kiosk ID</label>
                            <input type="text" class="input-field width-xl" id="installMachineID">
                        </div>
                        <div class="field-group">
                            <label for="installSerialNo" class="form-label width-xs">Serial No.</label>
                            <input type="text" class="input-field width-xl" id="installSerialNo">
                        </div>
                        <div class="field-group">
                            <label for="installCalendarID" class="form-label width-xs">Calendar</label>
                            <select class="input-field width-xl" id="installCalendarID">
                                    
                            </select>
                        </div>
                        <div class="field-group">
                            <label for="installMachineVendor" class="form-label width-xs">Vendor</label>
                            <select class="input-field width-xl" id="installMachineVendor">
                                    
                            </select>
                        </div>
                        <div class="block-xs"></div>
                        <div class="field-group">
                            <span style="padding: 0px 50px;"></span>
                            <button type="submit" access-gate permission="Add" task="Install Kiosk" class="btn btn-primary" id="btnSubmit">Submit</button>
                            <button class="btn btn-default" id="btnReset">Reset</button>
                        </div>
                        <input type="hidden" id="txtSelectedType" />
                    </fieldset>
                </form>
            </div>
        </div>
    </div>
</div>
<script type="text/javascript">
    $(document).ready(function () {
        $('.install-machine-select').change(function () {
            var selectBox = $(".install-machine-select");
            var totalSelectBox = selectBox.size();
            var selected = 0;
            //$(this).find("option[value=0]").hide();
            $(selectBox).each(function () {
                if ($(this).val() !== undefined)
                    selected++;
            });

            if (selected == totalSelectBox)
                $("#machine-list-table").show();
        });
    });
</script>
<script src="/assets/scripts/kioskManagement/installMachine/main.js"></script>
<script src="/assets/scripts/kioskManagement/installMachine/installMachine.js"></script>

