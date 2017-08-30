<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="advAssignment.aspx.cs" Inherits="Maestro.views.kioskMaintenance.advMaintenance.advAssignment" %>

<style type="text/css">
    #specific-assignment-rule
    {
        min-height: 600px;
        width:100%;
        padding: 10px;
    }
    #select-machine-type
    {
        margin-left: 15px;
        padding: 4px;
        width: 150px;
    }
    #assignment-type-table
    {
        width: 100%;
    }
    #assignment-type-table label
    {
        padding-left: 10px;
    }
    #list-of-machine-div, #list-of-specific-rule, #business-rule-configure-div
    {
        float: left;
        margin-right: 8px;
        border: 1px solid #999;
        border-radius: 4px;
        margin-right: 10px;
        padding: 15px;
    }
    #list-of-machine-div
    {
        width: 28%;
    }
    #list-of-specific-rule
    {
        width: 28%;
    }
    #business-rule-configure-div
    {
        width: 40%;
    }
    .specific-machine, .specific-rule
    {
        border: 1px solid #ccc;
        font-size: 14px;
        border-radius: 4px;
        margin: 3px 0px;
        padding: 4px 4px 4px 8px;
    }
    .close-div-button
    {
        color: #000;
        opacity: 0.3;
        float: right;
        position: relative;
        top: -15px;
        left: 8px;
        font-size: 14px;
        cursor: pointer;
        font-weight: bold;
    }
    #specific-assignment-rule hr
    {
        border-top-color: #CCC;
    }
    #specific-machine-list .active,
    #specific-machine-list2 .active,
    #specific-rule-list .active
    {
        /*
        border-color:  #f0ad4e; 
        background-color: #f0ad4e;
        */
        font-weight: bold;
    }
    #specific-machine-list
    {
        padding: 0px 15px 0px 10px;   
        height: 450px;
        overflow:auto;
    }
    #specific-machine-list2
    {
        padding: 0px 15px 0px 10px;   
        height: 450px;
        overflow:auto;
    }
</style>
<script type="text/javascript">
    $(document).ready(function () {
        $('.assignment-type').click(function () {
            if ($('.assignment-type:checked').length > 0) {
                $('#list-of-machine-div').show();
            }
            else {
                $('#list-of-machine-div').hide();
            }
        });
        /*
        $('.specific-machine').click(function () {
            if ($('#specific-machine-list').find('.specific-machine').hasClass('active')) {
                $('#specific-machine-list').find('.specific-machine').removeClass('active');
                close_br_div();
                $('#list-of-specific-rule').toggle();
            }
            else {
                $(this).addClass('active');
                $('#list-of-specific-rule').toggle();
            }
        });

        $('.specific-rule').click(function () {
            if ($('#specific-rule-list').find('.specific-rule').hasClass('active')) {
                $('#specific-rule-list').find('.specific-rule').removeClass('active');
                $('#business-rule-configure-div').toggle();
            }
            else {
                $(this).addClass('active');
                $('#business-rule-configure-div').toggle();
            }
        });
        $('#close-specific-rule-div').click(function () {
            close_br_div();
            $('#list-of-specific-rule').hide();
            $('#specific-machine-list').find('.specific-machine').removeClass('active');
        });
        $('.close-br-div').click(function () {
            close_br_div();
        });

        $('#save-rule-btn').click(function () {
            $('#business-rule-configure-div').hide();

            $('#specific-rule-list').find('.active').addClass('completed-rule-assignment');
            $('#specific-rule-list').find('.active').find('.ok-mark-div').html('<i class="fa fa-check" style="float:right;padding:4px;font-size:14px;"></i>');
            $('#specific-rule-list').find('.active').removeClass('incomplete-rule-assignment active');

            countCompletedRule();
        });
    });
    function close_br_div() {
        $('#business-rule-configure-div').hide();
        $('#specific-rule-list').find('.specific-rule').removeClass('active');
    }
    function countCompletedRule() {
        var totalRule = $('#specific-rule-list .specific-rule').length;
        var totalCompleted = $('#specific-rule-list .completed-rule-assignment').length;

        if (totalRule == totalCompleted) {
            $('#specific-machine-list .active').addClass('completed-rule-assignment');
            $('#specific-machine-list .active .ok-mark-div').html('<i class="fa fa-check" style="float:right;padding:4px;font-size:14px;"></i>');
            $('#specific-machine-list').find('.active').removeClass('incomplete-rule-assignment active');
        }
    }
    */
    });
</script>

                <legend>New Advertisement Assignment</legend>

<div id="specific-assignment-rule">
    <div id="specific-assigment-first-div">
        <label>Select Machine Type</label>
        <select id="select-machine-type">
            <option>CSD</option>
            <option>CJD</option>
            <option>BPS</option>
        </select>
    </div>
    <hr />
    <div id="specific-assigment-second-div">
        <table id="assignment-type-table">
            <tr>
                <td><label>Assignment Type</label></td>
                <td><input type="checkbox" class="assignment-type" /><label>By Group</label></td>
                <td><input type="checkbox" class="assignment-type" /><label>By Machine</label></td>
                <td><input type="checkbox" class="assignment-type" /><label>By Region</label></td>
                <td><input type="checkbox" class="assignment-type" /><label>By State</label></td>
            </tr>
        </table>        
    </div>
    <hr />
    <div id="specific-assigment-third-div">
        <div id="list-of-machine-div" style="display:block;">
            <h4>Machine List</h4>
            <hr />
            <div id="specific-machine-list">
                <div class="specific-machine incomplete-rule-assignment"><input type="checkbox" name="vehicle" value="All" checked="checked" />01CD<span class="ok-mark-div"></span></div>
                <div class="specific-machine incomplete-rule-assignment"><input type="checkbox" name="vehicle" value="All" checked="checked" />8001<span class="ok-mark-div"></span></div>
                <div class="specific-machine incomplete-rule-assignment"><input type="checkbox" name="vehicle" value="All" checked="checked" />9001<span class="ok-mark-div"></span></div>
                <div class="specific-machine incomplete-rule-assignment"><input type="checkbox" name="vehicle" value="All" checked="checked" />9002<span class="ok-mark-div"></span></div>
                <div class="specific-machine incomplete-rule-assignment"><input type="checkbox" name="vehicle" value="All" checked="checked" />9003<span class="ok-mark-div"></span></div>
                <div class="specific-machine incomplete-rule-assignment"><input type="checkbox" name="vehicle" value="All" checked="checked" />51CD<span class="ok-mark-div"></span></div>
                <div class="specific-machine incomplete-rule-assignment"><input type="checkbox" name="vehicle" value="All" checked="checked" />8001<span class="ok-mark-div"></span></div>
                <div class="specific-machine incomplete-rule-assignment"><input type="checkbox" name="vehicle" value="All" checked="checked" />9001<span class="ok-mark-div"></span></div>
                <div class="specific-machine incomplete-rule-assignment"><input type="checkbox" name="vehicle" value="All" checked="checked" />9002<span class="ok-mark-div"></span></div>
                <div class="specific-machine incomplete-rule-assignment"><input type="checkbox" name="vehicle" value="All" checked="checked" />9003<span class="ok-mark-div"></span></div>
                <div class="specific-machine incomplete-rule-assignment"><input type="checkbox" name="vehicle" value="All" checked="checked" />01CD<span class="ok-mark-div"></span></div>
                <div class="specific-machine incomplete-rule-assignment"><input type="checkbox" name="vehicle" value="All" checked="checked" />8001<span class="ok-mark-div"></span></div>
                <div class="specific-machine incomplete-rule-assignment"><input type="checkbox" name="vehicle" value="All" checked="checked" />9001<span class="ok-mark-div"></span></div>
                <div class="specific-machine incomplete-rule-assignment"><input type="checkbox" name="vehicle" value="All" checked="checked" />9002<span class="ok-mark-div"></span></div>
                <div class="specific-machine incomplete-rule-assignment"><input type="checkbox" name="vehicle" value="All" checked="checked" />9003<span class="ok-mark-div"></span></div>
                <div class="specific-machine incomplete-rule-assignment"><input type="checkbox" name="vehicle" value="All" checked="checked" />01CD<span class="ok-mark-div"></span></div>
                <div class="specific-machine incomplete-rule-assignment"><input type="checkbox" name="vehicle" value="All" checked="checked" />8001<span class="ok-mark-div"></span></div>
                <div class="specific-machine incomplete-rule-assignment"><input type="checkbox" name="vehicle" value="All" checked="checked" />9001<span class="ok-mark-div"></span></div>
                <div class="specific-machine incomplete-rule-assignment"><input type="checkbox" name="vehicle" value="All" checked="checked" />9002<span class="ok-mark-div"></span></div>
                <div class="specific-machine incomplete-rule-assignment"><input type="checkbox" name="vehicle" value="All" checked="checked" />9003<span class="ok-mark-div"></span></div>
                <div class="specific-machine incomplete-rule-assignment"><input type="checkbox" name="vehicle" value="All" checked="checked" />01CD<span class="ok-mark-div"></span></div>
                <div class="specific-machine incomplete-rule-assignment"><input type="checkbox" name="vehicle" value="All" checked="checked" />8001<span class="ok-mark-div"></span></div>
                <div class="specific-machine incomplete-rule-assignment"><input type="checkbox" name="vehicle" value="All" checked="checked" />9001<span class="ok-mark-div"></span></div>
                <div class="specific-machine incomplete-rule-assignment"><input type="checkbox" name="vehicle" value="All" checked="checked" />9002<span class="ok-mark-div"></span></div>
                <div class="specific-machine incomplete-rule-assignment"><input type="checkbox" name="vehicle" value="All" checked="checked" />9003<span class="ok-mark-div"></span></div>
            </div>
            
        </div>
        
        <div id="list-of-specific-rule" style="display:block;">
            
            <div id="specific-rule-list">
        <table class="table" id="tbDetails">
            <thead>
                <tr>
                    <th>Sequence</th>
                    <th>File Name</th>
                    <th>&nbsp;</th>
                </tr>
            </thead>
            <tbody>
                <tr><td>1</td><td>ABC.jpg</td><td><input type="checkbox" name="vehicle"  checked="checked"></td> </tr>
                <tr><td>2</td><td>BCD.png</td><td><input type="checkbox" name="vehicle" checked="checked" ></td> </tr>
                <tr><td>3</td><td>CDE.avi</td><td><input type="checkbox" name="vehicle" checked="checked" ></td> </tr>
                <tr><td>4</td><td>DEF.mp4</td><td><input type="checkbox" name="vehicle" checked="checked" ></td> </tr>
                <tr><td>5</td><td>EFC.jpg</td><td><input type="checkbox" name="vehicle" checked="checked" ></td> </tr>
                <tr><td>6</td><td>MYA.png</td><td><input type="checkbox" name="vehicle" checked="checked" ></td> </tr>
                <tr><td>7</td><td>SDF.avi</td><td><input type="checkbox" name="vehicle" ></td> </tr>
                <tr><td>8</td><td>TTD.mp4</td><td><input type="checkbox" name="vehicle"  ></td> </tr>
                <tr><td>9</td><td>EFC.jpg</td><td><input type="checkbox" name="vehicle" checked="checked" ></td> </tr>
                <tr><td>10</td><td>MYA.png</td><td><input type="checkbox" name="vehicle" checked="checked" ></td> </tr>
                <tr><td>11</td><td>EBC.jpg</td><td><input type="checkbox" name="vehicle" checked="checked" ></td> </tr>
                <tr><td>12</td><td>DDD.png</td><td><input type="checkbox" name="vehicle" checked="checked" ></td> </tr>

            </tbody>
        </table>
                </div>
        </div>

       <%-- <div id="list-of-specific-rule" style="display:none;">
            <span class="close-div-button" id="close-specific-rule-div">x</span>
            <h4>List of Specific Rule</h4>
            <hr />
            <div id="specific-rule-list">
                <div class="specific-rule incomplete-rule-assignment">Sorter Bin<span class="ok-mark-div"></span></div>
                <div class="specific-rule incomplete-rule-assignment">Advertisement<span class="ok-mark-div"></span></div>
            </div>
        </div>--%>
        <div id="business-rule-configure-div" style="display:none;">
            <span class="close-div-button close-br-div">x</span>
            <p style="font-size:16px;">Define Expression for &#060;Rule Name&#062; of &#060;Data Type&#062;</p>
            <hr />
            <form class="form-horizontal" role="form" action="javascript:;">
                <fieldset>
                    <div class="form-group">
                        <label for="inputCondition" class="col-xs-3 control-label">Condition</label>
                        <div class="col-xs-9">
                            <input type="text" class=" form-control" id="inputCondition" placeholder="Condition">
                        </div>
                    </div>
                    <div class="form-group">
                        <label for="inputReturn" class="col-xs-3 control-label">Return</label>
                        <div class="col-xs-9">
                            <input type="text" class=" form-control" id="inputReturn" placeholder="Return">
                        </div>
                    </div>
                    <div>
                    <div class="form-group">
                        <div class="col-xs-offset-3 col-xs-3">
                            <button type="submit" class="btn btn-primary">Add</button>
                        </div>
                    </div>
                    <hr />
                    <div class="form-group">
                        <label for="inputExpression" class="col-xs-3 control-label">Expression</label>
                        <div class="col-xs-9">
                            <input type="text" class ="form-control" id="inputExpression" value="If  &#060;Condition&#062; return &#060;Return&#062;" readonly />
                        </div>
                    </div>
                    <div class="form-group">
                        <div class="col-xs-offset-3 col-xs-9">
                            <button type="button" id="save-rule-btn" class="btn btn-primary">Save</button>
                            <button type="reset" class="btn btn-primary">Reset</button>
                            <button type="button" class="btn btn-primary close-br-div">Cancel</button>
                        </div>
                    </div>
                </fieldset>            
            </form>
        </div>
    </div>
    <div id="specific-assigment-last-div">
        <table id="Table1">
            <tr>
                <button type="button" class="btn btn-default" onclick="addCustomer()">Save</button>
            </tr>
        </table>        
    </div>
</div>