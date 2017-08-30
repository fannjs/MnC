<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="dashboard.aspx.cs" Inherits="Maestro.views.dashboard" %>


<link rel="stylesheet" href="/components/sensors/css/sensorfield.css">
<link rel="stylesheet" href="/components/sensors/css/arrow.css">
<link rel="stylesheet" type="text/css" href="../assets/styles/charts/chart-legend.css" >

<div id="row" class="row">
    <!--First Column-->
    <div class="column col-md-4">
        <div class="portlet" id="divCountry">
            <div class="portlet-header">
                <h3>Status</h3>
                <div class="pull-right">                    
                    <ul class="portlet-tools">
                        <li>
                            <div class="btn-group">
                                <button id="select-site-btn" class="btn btn-color btn-sm dropdown-toggle" data-toggle="dropdown" type="button">
                                    <span>All Site</span>
                                    <%--<b class="caret"></b>--%>
                                </button>
                                <ul id="dllSite" class="dropdown-menu status-option" role="menu">
                                    <li><a href="javascript:;">All Site</a></li>
                                </ul>
                            </div>                            
                        </li>
                        <li>
                            <div class="btn-group">
                                <button id="select-kiosk-btn" class="btn btn-color btn-sm dropdown-toggle" data-toggle="dropdown" type="button">
                                    <span>All Kiosk</span>
                                    <%--<b class="caret"></b>--%>
                                </button>
                                <ul id="dllMachType" class="dropdown-menu status-option" role="menu">
                                    <li><a href="javascript:;">All Kiosk</a></li>
                                </ul>
                            </div>                            
                        </li>
                    </ul>
                </div>
            </div>
            <div class="portlet-content">
                <div style="display:inline-block">[Last Update: <span class="latest-time-div" id="stateLastUpdate"></span>]</div>
                <div style="display:inline-block;float:right">Refreshing in <span id="stateCountDown">0</span><i class="fa fa-refresh refresh-btn"></i></div>
                
                <table id="statusTable" class="tblLocation">
                    <thead>
                        <tr>
                            <th style="width: 52%;">State</th>
                            <th style="width: 12%;"></th>
                            <th style="width: 12%;"></th>
                            <th style="width: 12%;"></th>
                            <th style="width: 12%;"></th>
                        </tr>
                    </thead>
                    <tbody>
                    </tbody>
                </table>
            </div>
        </div>
        <div class="portlet" id="divUrgencyQueue">
            <div class="portlet-header">
                <h3>Kiosk Urgency Queue List</h3>
            </div>
            <div class="portlet-content">
                <div style="display:inline-block">[Last Update: <span class="latest-time-div" id="urgencyLastUpdate"></span>]</div>
                <div style="display:inline-block;float: right;">Refreshing in <span id="urgencyCountDown">0</span><i class="fa fa-refresh refresh-btn"></i></div>
                <table id="urgentQueueList" class="tblLocation">
                    <thead>
                        <tr>
                            <th style="width:15%">ID</th>
                            <th style="width:15%">Type</th>
                            <th style="width:50%">Desc</th>
                            <th style="width:20%">Downtime</th>
                        </tr>
                    </thead>
                    <tbody>
                        <!-- Urgent queue data -->
                    </tbody>
                </table>
            </div>
        </div>

    </div>
    <!--Second Column-->
    <div class="column col-md-4">
        <div class="portlet">
            <div class="portlet-header">
                <h3>Overall Status for <span id="portletSelectedState">All States</span></h3>
            </div>
            <div class="portlet-content">
                <table id="overall-status-table" width="100%">
                    <tr align="center">
                        <td>
                            <div id="total-error" class="circle tERROR"></div>
                        </td>
                        <td>
                            <div id="total-warning" class="circle tWARN"></div>
                        </td>
                        <td>
                            <div id="total-online" class="circle tONLINE"></div>
                        </td>
                        <td>
                            <div id="total-offline" class="circle tOFFLINE"></div>
                        </td>
                    </tr>
                </table>
<%--                <div id="volum e-div"><i class="fa fa-volume-up volume-icon" onclick="muteAudio()"></i></div>--%>
                <div id="volume-div" hidden><i class="fa fa-volume-up volume-icon" onclick="muteAudio()"></i></div>
            </div>
        </div>
        <div class="portlet" id="divState">
            <div class="portlet-header">
                <h3>Town/City</h3>
            </div>
            <div class="portlet-content">
                <table class="tblLocation">
                    <thead><tr><td style="width:100%"></td></tr></thead>
                    <tbody></tbody>
                </table>
            </div>
        </div>
        <div class="portlet" id="divDistrict">
            <div class="portlet-header">
                <h3>Branch</h3>
            </div>
            <div class="portlet-content">
                <table class="tblLocation">
                    <thead><tr><td style="width:100%"></td></tr></thead>
                    <tbody></tbody>
                </table>
            </div>
        </div>
        <div class="portlet" id="divBranch">
            <div class="portlet-header">
                <h3>Kiosk(s)</h3>
            </div>
            <div class="portlet-content">
                <table class="tblLocation">
                    <thead><tr><td style="width:100%"></td></tr></thead>
                    <tbody></tbody>
                </table>
            </div>
        </div>
    </div>
    <!--Third Column-->
    <div class="column col-md-4">
        <div class="portlet" style="display:none;">
            <div class="portlet-header">
                <h3></h3>
            </div>
            <div class="portlet-content">
                <button onclick="addNumber()">Add</button>
                <button onclick="removeNumber()">Remove</button>
            </div>
        </div>
        
        <div class="portlet">
            <div class="portlet-header">
                <h3>Quick Kiosk Detail Preview</h3>
            </div>
            <div class="portlet-content">
                <form class="form-inline" action="javascript:;">
                    <div class="form-group">
                        <label for="inputQuickView" class="control-label">Kiosk ID &nbsp;</label>
                    </div>
                    <div class="form-group">
                        <input type="text" class=" form-control" id="inputQuickView" placeholder="Kiosk ID">
                    </div> 
                    <div class="form-group">
                        <button class="btn btn-default" id="quick-view" data-toggle="modal" data-target="">Quick View</button>
                    </div>
                </form>
            </div>
        </div>
        
        <div class="portlet">
            <div class="portlet-header">
                <h3>Current Machine Status %</h3>
            </div>
            <div class="portlet-content">
                <div>
                    <div class="canvas-chart" >
                        <canvas id="pieChart" width="150" height="150"></canvas>
                    </div>
                    <div id="pieLegend"></div>
                </div>
            </div>
        </div>
        
        <div class="portlet">
            <div class="portlet-header">
                <h3>Machine Status Today</h3>
            </div>
            <div class="portlet-content">
                <div>
                    <div class="canvas-chart" >
                        <canvas id="machStatusLineChart_machid" width="250" height="200"></canvas>
                    </div>
                </div>
                    <div id="lineLegend_machid"></div>
            </div>
        </div>
            

        <div class="portlet">
            <div class="portlet-header">
                <h3>Top 5 Problems Today</h3>
            </div>
            <div class="portlet-content">
                <div>
                    <div class="canvas-chart" >
                        <canvas id="topProblemsTodayBarChart" width="250" height="200"></canvas>
                    </div>
                </div>
            </div>
        </div>
        
    </div>
</div>
<!-- Audio in hidden div -->
<div id="audio-hidden-div" style="display:none;">
    <audio id="audio" src="/assets/audios/ding.wav" controls loop></audio>
    <audio id="audio1" src="/assets/audios/ding.wav" controls loop></audio>
</div>
<!-- Quick View - Pop up -->
<div class="modal" id="quickView-modal" tabindex="-1" role="dialog" aria-labelledby="modal-title" aria-hidden="true">
    <div class="modal-dialog">
        <div class="modal-content">
            <div class="modal-header">
                <button type="button" class="close" data-dismiss="modal" aria-hidden="true">&times;</button>
                <h4 class="modal-title" id="modal-title">Kiosk Detail - <span class="tdMachineID"></span></h4>
            </div>
            <div class="modal-body">
                <div id="machine-detail-status">
                    <div style="display:inline-block; width: 65px; vertical-align: top;">
                        <div id="machine-status-image">
                            <div class="small-circle"></div>
                        </div>
                    </div>
                    <div style="display:inline-block;margin-left:15px; width:440px;vertical-align: top;">
                        <table style="padding:0px;">
                            <tr>
                                <td class="td-title" style="width:124px;">Current Status</td>
                                <td style="width:10px;">:</td>
                                <td id="tdCurrStatus"></td>
                            </tr>
                            <tr>
                                <td class="td-title">Last Transaction</td>
                                <td style="width:10px;">:</td>
                                <td id="tdLastTrans"></td>
                            </tr>
                        </table>
                        <span style="font-weight:bold;text-decoration:underline;">Code & Description</span>
                        <div id="CodeDescDiv">
                            <ul></ul>
                        </div>
                    </div>
                </div>
                <div style="height: 20px;"></div>
                <ul id="nav-tabs" class="nav nav-tabs">
                    <li><a href="#overview-tab" data-toggle="tab">Overview</a></li>
                    <li><a href="#detail-tab" data-toggle="tab">Detail</a></li>
                    <li><a href="#branch-tab" data-toggle="tab">Branch</a></li>
                    <li><a href="#transaction-tab" data-toggle="tab">Transaction History</a></li>
                    <li><a href="#history-tab" data-toggle="tab">Status History</a></li>
                    <li><a href="#diagram-tab" data-toggle="tab">Diagram</a></li>  
                    <li class="active"><a href="#SOP-tab" data-toggle="tab">S.O.P</a></li>  
                </ul>
                <div class="tab-content">
                    <div class="tab-pane" id="overview-tab">
                        <table>
                            <tr>
                                <td class="td-title" width="20%">Kiosk ID</td>
                                <td width="5%">:</td>
                                <td class="tdMachineID" width="75%"></td>
                            </tr>
                            <tr>
                                <td class="td-title">Vendor</td>
                                <td>:</td>
                                <td id="tdVendor"></td>
                            </tr>
                            <tr>
                                <td class="td-title">Branch ID</td>
                                <td>:</td>
                                <td  class="tdBranch"></td>
                            </tr>
                        </table>
                    </div>
                    <div class="tab-pane" id="detail-tab">
                        <table>
                            <tr>
                                <td class="td-title" width="20%">Kiosk ID</td>
                                <td width="5%">:</td>
                                <td class="tdMachineID" width="75%"></td>
                            </tr>
                            <tr>
                                <td class="td-title">Kiosk Type</td>
                                <td>:</td>
                                <td>CSD</td>
                            </tr>
                            <tr>
                                <td class="td-title">Company</td>
                                <td>:</td>
                                <td class="tdSite"></td>
                            </tr>
                            <tr>
                                <td class="td-title">Country</td>
                                <td>:</td>
                                <td class="tdCountry"></td>
                            </tr>
                            <tr>
                                <td class="td-title">State</td>
                                <td>:</td>
                                <td class="tdState"></td>
                            </tr>
                            <tr>
                                <td class="td-title">District</td>
                                <td>:</td>
                                <td class="tdDistrict"></td>
                            </tr>
                            <tr>
                                <td class="td-title">Branch</td>
                                <td>:</td>
                                <td class="tdBranch"></td>
                            </tr>
                        </table>
                    </div>
                    <div class="tab-pane" id="branch-tab">
                        <table>
                            <tr>
                                <td class="td-title" width="20%">Address</td>
                                <td width="5%">:</td>
                                <td id="tdFullAddr" width="75%">-</td>
                            </tr>
                            <tr>
                                <td class="td-title">Phone</td>
                                <td>:</td>
                                <td id="tdPhone">-</td>
                            </tr>
                            <tr>
                                <td class="td-title">Person in Charge</td>
                                <td class="vertically-centered">:</td>
                                <td class="vertically-centered" id="tdContactPerson">-</td>
                            </tr>
                        </table>
                    </div>
                    <div class="tab-pane" id="transaction-tab">
                        <h5>Transaction History for Last 30 Days</h5>
                        <table id="tblTransactionHistory">
                            <thead>
                                <tr>
                                    <th>Date</th>
                                    <th>No of Transactions</th>
                                    <th>No of Cheques</th>
                                </tr>
                            </thead>
                            <tbody>

                            </tbody>
                        </table>
                        <ul id="Ul1" class="pagination" style="margin: 10px 0px 0px 0px;"></ul>
                    </div>
                    <div class="tab-pane" id="history-tab">
                        <table id="tblStatusHistory">
                            <thead>
                                <tr>
                                    <th>Error Code</th>
                                    <th>Error Description</th>
                                    <th>Status</th>
                                    <th>Date Time</th>
                                </tr>
                            </thead>
                            <tbody>

                            </tbody>
                        </table>
                        <ul id="pagination" class="pagination" style="margin: 10px 0px 0px 0px;"></ul>
                    </div>
                    <div class="tab-pane" id="diagram-tab">
                        <div class="sensorField">
                            <img alt="Gallery Image" src="/assets/images/machine_diagram.png">
                        </div>
                    </div>
                    <div class="tab-pane active" id="SOP-tab">
                        <div style="height:10px;"></div>
                        <div id="SOP-info-div">

                        </div>
                    </div>
                </div>
            </div>
            <div class="modal-footer">
                <div id="machine-location-div">
                    Kiosk located at: <span id="machineLocation"></span>
                </div>
            </div>
        </div><!-- /.modal-content -->
    </div><!-- /.modal-dialog -->
</div><!-- /.modal -->

<script type="text/javascript">

    function muteAudio() {
        var audio = document.getElementById('audio');
        var audio1 = document.getElementById('audio1');

        if (audio.muted != true && audio1.muted != true) {
            $('#volume-div').html('<i class="fa fa-volume-off volume-icon" onclick="muteAudio()"></i>');
            audio.muted = true;
            audio1.muted = true;
        }
        else {
            $('#volume-div').html('<i class="fa fa-volume-up volume-icon" onclick="muteAudio()"></i>');
            audio.muted = false;
            audio1.muted = false;
        }
    }

    function autoResizeCircle() {
        var circle_width = $(".circle").width();
        var circle_height = circle_width + 10; //10 is getting from circle's padding-top (5) and padding-bottom (5)

        $(".circle").css("line-height", circle_width + "px").css("height",circle_height + "px");
        
    };

    $(function () {
        $(".column").sortable({
            connectWith: ".column",
            handle: ".portlet-header",
            containment: "#content",
            placeholder: "portlet-placeholder ui-corner-all",
            scroll: true
        });

        $('.status-option').find('a').click(function () {
            $(this).closest('.btn-group').find('.btn-color').html($(this).text() + '&nbsp;<b class="caret"></b>');
        });
        var latest_time = moment().format('h:mm:ss A');
        $('.latest-time-div').html(latest_time);

        //checkAlert();
        //setInterval(checkAlert, 1000);

        $('.status-col').each(function () {
            if ($(this).text() > 0) {
                $(this).addClass('clickable-link');
            }
        });

        autoResizeCircle();
        $(window).resize(function () {
            autoResizeCircle();
        });

        var chartWidth = $('.portlet-content').width();
        //$('#trChart').attr('width', chartWidth).attr('height', chartWidth);
        $('#pieChart').attr('width', chartWidth);
        $('#pieChart').attr('height', 200);
        $('#topProblemsTodayBarChart').attr('width', chartWidth);
        $('#topProblemsTodayBarChart').attr('height', 200);
        //$('#machStatusLineChart').attr('width', chartWidth);
        //$('#machStatusLineChart').attr('height', 150);
        $('#machStatusLineChart_machid').attr('width', chartWidth);
        $('#machStatusLineChart_machid').attr('height', 200);
        //createBarChart();
        //genTopProblemsTodayChart(4);
    });

    //function createBarChart() {
    //    var data = {
    //        labels: ["Apr", "May", "Jun", "Jul", "Aug", "Sep"],
    //        datasets: [
    //        {
    //            fillColor: "rgba(225,0,0,1)",
    //            strokeColor: "rgba(225,0,0,1)",
    //            data: [12, 16, 13, 15, 18, 21]
    //        },
    //        {
    //            fillColor: "rgba(0,26,225,1)",
    //            strokeColor: "rgba(0,26,225,1)",
    //            data: [18, 16, 17, 17, 18, 16]
    //        },
    //        {
    //            fillColor: "rgba(24,31,28,0.5)",
    //            strokeColor: "rgba(24,31,28,0.5)",
    //            data: [11, 16, 9, 10, 17, 17]
    //        }]
    //    }
    //    var cht = document.getElementById('topProblemsTodayChart');
    //    var ctx = cht.getContext('2d');
    //    var barChart = new Chart(ctx).Bar(data, { responsive: true, showTooltips: true});
    //}

</script> 
<script type="text/javascript" src="/components/sensors/js/sensor.js"></script>
<script type="text/javascript" src="/components/sensors/js/sensorfield.js"></script>
<script type="text/javascript" src="../assets/scripts/dashboard.js"></script>
