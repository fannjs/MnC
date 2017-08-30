<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="index.aspx.cs" Inherits="Maestro.index" %>

<!DOCTYPE html>

<html>
    <head>
        <link rel="stylesheet" href="http://fonts.googleapis.com/css?family=Open+Sans:400italic,600italic,800italic,400,600,800" type="text/css">
        <link rel="stylesheet" href="../components/bootstrap3/css/bootstrap.min.css">
        <link rel="stylesheet" href="../components/font-awesome/font-awesome.min.css">
        <link rel="stylesheet" href="/assets/styles/index.css">
        <link rel="stylesheet" href="/assets/styles/dashboard.css"><!--put here to better viewing of look&feel for dashboard, as dboard requires a lot of queries upon loading-->
        <link rel="stylesheet" href="../components/magnific/magnific-popup.css" type="text/css" />
        <link rel="stylesheet" href="../components/jquery-ui/jquery-ui.min.css">
        <link rel="stylesheet" href="../components/jquery-ui/jquery-ui.theme.min.css">
        <!--<link rel="Stylesheet" href="../assets/styles/alert.css" type="text/css" />-->
        <title>Maestro Monitoring</title>
    </head>
    <body>
        <div>
            <!-- Header -->
            <header id="header">
                <h1 id="brand-logo">
                    <a href="index.aspx">
                        <img src="../assets/images/logo.png">
                    </a>
                </h1>
                <div id="date-time-div">
                    <div id="date-time">
                    </div>
                </div>
                <div id="content-header">
                    <h1 id="content-title"></h1>
                </div>
            </header>
            <nav id="top-bar" class="collapse top-bar-collapse">
                <ul class="nav navbar-nav pull-right">
                    <li class="dropdown">
                        <a class="dropdown-toggle" href="javascript:;" data-toggle="dropdown">
                            <i class="glyphicon glyphicon-user" style="width:20px;"></i><span id="userName" runat="server"></span><b class="caret"></b>
                        </a>
                        <ul id="user-setting" class="dropdown-menu" role="menu">
                            <li><a href="javascript:;" onclick="loadTaskList()"><i class="fa fa-tasks"></i>Task List</a></li>
                            <li><a href="javascript:;" onclick="loadUserProfile()"><i class="fa fa-user"></i>My Profile</a></li>
                            <li class="divider"></li>
                            <li><a href="javascript:;" onclick="logout()"><i class="fa fa-power-off"></i>Logout</a></li>
                        </ul>
                    </li>
                </ul>
                <div id="full-screen-btn" class="pull-right">
                    <a href="fullscreen.aspx"><i class="fa fa-external-link"></i>Full Screen</a>
                </div>
            </nav>
            <!-- Sidebar -->
            <div id="sidebar-wrapper" class="collapse">
                <div class="panel-group" id="accordion">
                    <div class="panel panel-default access-gate-main active" task="Dashboard">
                        <div class="panel-heading">
                            <h4 class="panel-title">
                                <a id="dashboard-nav" data-toggle="collapse" data-parent="#accordion" href="#collapseAll" data-url="/views/dashboard.aspx">
                                    <i class="fa fa-dashboard"></i>Dashboard 
                                </a>
                            </h4>
                        </div>
                        <div id="collapseAll" class="panel-collapse collapse" style="display:none;">
                            <li class="access-gate-sub" task="Dashboard"><a href="javascript:;" data-url="/views/dashboard.aspx">Dashboard</a></li>
                        </div>
                    </div>
                    <div class="panel panel-default access-gate-main" task="Administration">
                        <div class="panel-heading">
                            <h4 class="panel-title">
                                <a data-toggle="collapse" data-parent="#accordion" href="#collapseOne">
                                    <i class="fa fa-user"></i>Administration <span class="caret"></span>
                                </a>
                            </h4>
                        </div>
                        <div id="collapseOne" class="panel-collapse collapse">
                            <div class="panel-body">
                                <ul class="panel-body-menu">
                                    <li class="access-gate-sub" task="User Control"><a href="javascript:;" data-url="/views/administration/userManagement/main.aspx"><i class="fa fa-user-plus"></i>User Control</a></li>
                                    <li class="access-gate-sub" task="Site Operation"><a href="javascript:;" data-url="/views/administration/siteManagement/main.aspx"><i class="fa fa-sitemap"></i></i>Site Operation</a></li>
                                    <li class="access-gate-sub" task="Vendor Management"><a href="javascript:;" data-url="/views/administration/vendorManagement/main.aspx"><i class="fa fa-male"></i></i>Vendor Management</a></li>
                                    <li class="access-gate-sub" task="User Access"><a href="javascript:;" data-url="/views/administration/userAccess/main.aspx"><i class="fa fa-key"></i></i>User Access</a></li>                                   
                                    <li class="access-gate-sub" task="Status Code Escalation"><a href="javascript:;" data-url="/views/administration/statusCodeEscalation/main.aspx"><i class="fa fa-exclamation-circle"></i>Status Code Escalation</a></li>
                                </ul>
                            </div>
                        </div>
                    </div>
                    <div class="panel panel-default access-gate-main" task="Kiosk Management">
                        <div class="panel-heading">
                            <h4 class="panel-title">
                                <a data-toggle="collapse" data-parent="#accordion" href="#collapseTwo">
                                    <i class="fa fa-cube"></i>Kiosk Management <span class="caret"></span>
                                </a>
                            </h4>
                        </div>
                        <div id="collapseTwo" class="panel-collapse collapse">
                            <div class="panel-body">
                                <ul class="panel-body-menu">
                                    <li class="access-gate-sub" task="Kiosk Template"><a href="javascript:;" data-url="/views/kioskManagement/machineTemplate/main.aspx"><i class="fa fa-files-o"></i>Kiosk Template</a></li>
                                    <li class="access-gate-sub" task="Setup Branch"><a href="javascript:;" data-url="/views/kioskManagement/setupBranch/main.aspx"><i class="fa fa-building-o"></i>Setup Branch</a></li>
                                    <li class="access-gate-sub" task="Install Kiosk"><a href="javascript:;" data-url="/views/kioskManagement/installMachine/main.aspx"><i class="fa fa-floppy-o"></i>Install Kiosk</a></li>
                                    <li class="access-gate-sub" task="Manage Kiosk"><a href="javascript:;" data-url="/views/kioskManagement/manageKiosk/mgnKiosk.aspx"><i class="fa fa-cubes"></i></i>Manage Kiosk</a></li>
                                    <li class="access-gate-sub" task="Log Retrieval"><a href="javascript:;" data-url="/views/kioskManagement/logRetrieval/main.aspx"><i class="fa fa-file"></i>Log Retrieval</a></li>
                                </ul>
                            </div>
                        </div>
                    </div>
                    <div class="panel panel-default access-gate-main" task="Kiosk Maintenance">
                        <div class="panel-heading">
                            <h4 class="panel-title">
                                <a data-toggle="collapse" data-parent="#accordion" href="#collapseThree">
                                    <i class="fa fa-wrench"></i>Kiosk Maintenance <span class="caret"></span>
                                </a>
                            </h4>
                        </div>
                        <div id="collapseThree" class="panel-collapse collapse">
                            <div class="panel-body">
                                <ul class="panel-body-menu">
                                    <li class="access-gate-sub" task="Software Distribution"><a href="javascript:;" data-url="/views/kioskMaintenance/softwareDistribution/main.aspx"><i class="fa fa-cloud"></i>Software Distribution</a></li>
                                    <li class="access-gate-sub" task="Advertisement Maintenance"><a href="javascript:;" data-url="/views/kioskMaintenance/adsMaintenance/main.aspx"><i class="fa fa-file-image-o"></i>Advertisement Maintenance</a></li>
                                    <li class="access-gate-sub" task="Kiosk Cassette Master List"><a href="javascript:;" data-url="/views/kioskMaintenance/cassette/cassetteMain.aspx"><i class="fa fa-list-alt"></i></i>Kiosk Cassette Master List</a></li> 
                                    <li class="access-gate-sub" task="Rule Assignment"><a href="javascript:;" data-url="/views/kioskMaintenance/rulesAssignment/main.aspx"><i class="fa fa-globe"></i>Rule Assignment</a></li>
                                    <li class="access-gate-sub" task="Business Day"><a href="javascript:;" data-url="/views/kioskMaintenance/businessDay/main.aspx"><i class="fa fa-briefcase"></i>Business Day</a></li>
                                    <li class="access-gate-sub" task="Holiday Maintenance"><a href="javascript:;" data-url="/views/kioskMaintenance/holidayMaintenance/main.aspx"><i class="fa fa-calendar"></i>Holiday Maintenance</a></li>                                   
                                </ul>
                            </div>
                        </div>
                    </div>
                    <div class="panel panel-default access-gate-main" task="Reconciliation">
                        <div class="panel-heading">
                            <h4 class="panel-title">
                                <a data-toggle="collapse" data-parent="#accordion" href="#reconciliationMenu">
                                    <i class="fa fa-cogs"></i>Reconciliation<span class="caret"></span>
                                </a>
                            </h4>
                        </div>
                        <div id="reconciliationMenu" class="panel-collapse collapse">
                            <div class="panel-body">
                                <ul class="panel-body-menu">
                                    <li class="access-gate-sub" task="Cheque Counter"><a href="javascript:;" data-url="/views/reconciliation/chequeCounter/main.aspx">Cheque Counter</a></li>
                                    <li class="access-gate-sub" task="Cheque Counter Balancing"><a href="javascript:;" data-url="/views/reconciliation/chequeCounterBalancing/main.aspx">Cheque Counter Balancing</a></li>
                                    <li class="access-gate-sub" task="Transaction Balancing"><a href="javascript:;" data-url="/views/reconciliation/transactionBalancing/main.aspx">Transaction Balancing</a></li> 
                                </ul>
                            </div>
                        </div>
                    </div>
                    <div class="panel panel-default access-gate-main" task="Reports">
                        <div class="panel-heading">
                            <h4 class="panel-title">
                                <a data-toggle="collapse" data-parent="#accordion" href="#collapseFour">
                                    <i class="fa fa-file-text"></i>Reports <span class="caret"></span>
                                </a>
                            </h4>
                        </div>
                        <div id="collapseFour" class="panel-collapse collapse">
                            <div class="panel-body">
                                <ul class="panel-body-menu">
                                    <li class="access-gate-sub" task="Audit Trail - Monitoring & Control"><a href="javascript:;" data-url="/views/reports/auditTrail/main.aspx">Audit Trail - Monitoring & Control</a></li>
                                    <li class="access-gate-sub" task="Full Kiosk List"><a href="javascript:;" data-url="/views/reports/fullKioskList/main.aspx">Full Kiosk List</a></li>
                                    <li class="access-gate-sub" task="Kiosk - Frequent Breakdown"><a href="javascript:;" data-url="/views/reports/topFrequentBreakdown/frequentBreakdownMain.aspx">Kiosk - Frequent Breakdown</a></li>
                                    <li class="access-gate-sub" task="Kiosk - Longest Downtime"><a href="javascript:;" data-url="/views/reports/longestDowntime/longestDowntimeMain.aspx">Kiosk - Longest Downtime</a></li>
                                    <li class="access-gate-sub" task="Kiosk - Longest Response Time"><a href="javascript:;" data-url="/views/reports/longestResponse/longestResponseMain.aspx">Kiosk - Longest Response Time</a></li>
                                    <li class="access-gate-sub" task="Kiosk Problems Volume Statistic"><a href="javascript:;" data-url="/views/reports/problemStatistics/mainProblemStatistics.aspx">Kiosk - Problems Volume Statistic</a></li>
                                    <li class="access-gate-sub" task="Full Kiosk List"><a href="javascript:;" data-url="/views/reports/performance/performanceMain.aspx">Kiosk - Performance</a></li>
                                    <li class="access-gate-sub" task="Kiosk Status Report"><a href="javascript:;" data-url="/views/reports/statusReport/machStatusMain.aspx">Kiosk - Status Report</a></li>
                                    <li class="access-gate-sub" task="Kiosk Up Time and Down Time"><a href="javascript:;" data-url="/views/reports/upDowntime/upDownTimeMain.aspx">Kiosk - Up Time and Down Time</a></li>
                                </ul>
                            </div>
                        </div>
                    </div>
                    <div class="panel panel-default access-gate-main" task="Helpdesk">
                        <div class="panel-heading">
                            <h4 class="panel-title">
                                <a data-toggle="collapse" data-parent="#accordion" href="#collapseFive">
                                    <i class="fa fa-question-circle"></i>Helpdesk <span class="caret"></span>
                                </a>
                            </h4>
                        </div>
                        <div id="collapseFive" class="panel-collapse collapse">
                            <div class="panel-body">
                                <ul class="panel-body-menu">
                                    <li class="access-gate-sub" task="View Cases"><a href="javascript:;" data-url="URL">View Cases</a></li>
                                </ul>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
            <!-- Content -->
            <div id="content">
                <div id="content-container">
                    <div id="content-mainpage-container" ng-controller="ReloadCtrl">

                    </div>
                    <div id="content-subpage-container" hidden>

                    </div>
                </div>
            </div>
        </div>
        <script src="../components/jquery/jquery.js"></script>        
        <script src="../components/bootstrap3/js/bootstrap.min.js"></script>
        <script src="../components/moment/moment.min.js"></script>
        <script src="../components/jquery-ui/jquery-ui.min.js"></script>
        <script src="../components/jquery-tablesorter/jquery.tablesorter.min.js"></script>
        <script src="/assets/scripts/configs/config.factory.js"></script>
        <script src="/assets/scripts/configs/config.service.js"></script>
        <script src="/assets/scripts/plugins/plugin.selectbox.js"></script>
        <script src="/components/magnific/jquery.magnific-popup.min.js"></script>  
        <script type="text/javascript" src="../assets/scripts/charts/Chart.js"></script>
        <script type="text/javascript" src="../assets/scripts/charts/ChartLegend.js"></script>
        
        <!--<script src="../assets/scripts/alert.js" type="text/javascript"></script>-->

        <script type="text/javascript">

            function LoadUserAccessTaskPermission() {

                $.ajax({
                    type: "POST",
                    url: "index.aspx/GetCurrentUserDetails",
                    contentType: "application/json; charset=utf-8",
                    data: "{}",
                    dataType: "json",
                    success: function (data) {
                        var User = data.d;
                        var TaskList = User.UserAccessList;
                        
                        for (var i = 0; i < TaskList.length, Task = TaskList[i]; i++) {
                            var PTaskName = Task.PTaskName;
                            var SubTaskList = Task.SubTaskList;
                            var PTask = $('#sidebar-wrapper').find('.access-gate-main[task="' + PTaskName + '"]');

                            $(PTask).show();

                            for (var a = 0; a < SubTaskList.length, SubTask = SubTaskList[a]; a++) {
                                var STaskName = SubTask.TaskName;

                                $(PTask).find('.access-gate-sub[task="' + STaskName + '"]').show();
                            }
                        }

                        $('#sidebar-wrapper').find('.access-gate-main:visible').eq(0).find('.access-gate-sub a').click();
                    },
                    error: function (error) {
                        alert("Error " + error.status + ". Unable to load User Permission.");
                    }
                });
            };

            function logout() {
                window.location.href = "../logout.aspx";
            };
            function loadTaskList() {
                $('#content-title').html("Task List");
                $('#accordion').find('.panel').removeClass('active');

                $.ajax({
                    url: "tasklist.aspx",
                    beforeSend: function () {
                        clearAllTimeoutInterval();
                        $('#content-mainpage-container').html("<img id='loader-gif' src='../assets/images/loader.gif' />");
                        showMainPage();
                    },
                    success: function (data) {
                        $("#content-mainpage-container").html(data);
                        showMainPage();
                    }
                });
            };
            function loadUserProfile() {
                $('#content-title').html("User Profile");
                $('#accordion').find('.panel').removeClass('active');

                $.ajax({
                    url: "administration\\userManagement\\userProfile.aspx",
                    beforeSend: function () {
                        clearAllTimeoutInterval();
                        $('#content-mainpage-container').html("<img id='loader-gif' src='../assets/images/loader.gif' />");
                        showMainPage();
                    },
                    success: function (data) {
                        $("#content-mainpage-container").html(data);
                        showMainPage();
                    }
                });
            };

            function initLightbox() {

                if ($.fn.magnificPopup) {

                    $('.ui-lightbox-video, .ui-lightbox-iframe').magnificPopup({
                        disableOn: 700,
                        type: 'iframe',
                        mainClass: 'mfp-fade',
                        removalDelay: 160,
                        preloader: false,
                        fixedContentPos: false
                    });

                    $('.ui-lightbox-gallery').magnificPopup({
                        delegate: 'a',
                        type: 'image',
                        tLoading: 'Loading image #%curr%...',
                        mainClass: 'mfp-img-mobile',
                        gallery: {
                            enabled: true,
                            navigateByImgClick: true,
                            preload: [0, 1] // Will preload 0 - before current, and 1 after the current image
                        },
                        image: {
                            tError: '<a href="%url%">The image #%curr%</a> could not be loaded.',
                            titleSrc: function (item) {
                                return item.el.attr('title') + '<small>by Marsel Van Oosten</small>';
                            }
                        }
                    });
                }
            };

            initLightbox();
            
            function showMainPage() {
                $('#content-subpage-container').hide();
                $("#content-mainpage-container").show();
            }
            function showSubPage() {
                $('#content-subpage-container').show();
                $("#content-mainpage-container").hide();
            }
            function loadDashboard() {
                $.ajax({
                    url: "/views/dashboard.aspx",
                    beforeSend: function () {
                        $('#content-mainpage-container').html("<img id='loader-gif' src='../assets/images/loader.gif' />");
                        showMainPage();
                    },
                    success: function (data) {
                        $("#content-mainpage-container").html(data);
                        showMainPage();
                    }
                });
            }
            function sidebar_menu_logic() {
                var ajax_ready = true;

                $('#sidebar-wrapper .access-gate-sub a, #dashboard-nav').click(function () {
                    var $url = $(this).attr('data-url');
                    //This part is for MKCK module 
                    var task = $(this).parent().attr('task');
                    var pdata = {
                        task: task
                    }

                    if (ajax_ready) {
                        $('#content-title').html($(this).text());
                        $('#accordion').find('.panel').removeClass('active');
                        $(this).closest('.panel').addClass('active');

                        $.ajax({
                            type: "POST",
                            url: $url,
                            data: pdata,
                            beforeSend: function () {
                                clearAllTimeoutInterval();
                                ajax_ready = false;
                                $('#content-mainpage-container').html("<img id='loader-gif' src='../assets/images/loader.gif' />");
                                showMainPage();
                            },
                            success: function (data) {
                                $("#content-mainpage-container").html(data);

                                showMainPage();
                                ajax_ready = true;
                            },
                            error: function (jqXHR, textStatus, errorThrown) {
                                if (jqXHR.status == 404) {
                                    $("#content-mainpage-container").html('<h4>Error 404</h4> <p>Sorry requested page not found. </p>');
                                }
                                else if (jqXHR.status == 500) {
                                    $("#content-mainpage-container").html('<h4>Error 500</h4> <p>Internal server problem.</p>');
                                }
                                ajax_ready = true;
                            }
                        });
                    }
                });
            }
            function clearAllTimeoutInterval() {
                clearTimeout(window.liveUrgencyTimer); 
                clearInterval(window.countdownIdUrgency);
                clearTimeout(window.k);
                clearInterval(window.countdownId);
            }

            $(document).ready(function () {

                LoadUserAccessTaskPermission() // get User Task Permission

                realTime();
                setInterval(realTime, 1000); //for real-time clock
                sidebar_menu_logic();

                //GetServerTime();
            });
            function realTime() {
                var date_today = moment().format('dddd, MMMM D YYYY');
                var time_now = moment().format('h:mm:ss A');
                $('#date-time').html("Today is " + date_today + "<br/>It is now " + time_now);
            }

            //Temporary disable
//            var source = undefined;
//            function GetServerTime() {

//                source = new EventSource("../controller/GetDateTime.ashx");
//                source.onopen = function (event) {
////                    Connection opened
////                    do something
//                };
//                source.onerror = function (event) {
//                    CloseSSE();
//                };
//                source.onmessage = function (event) {
//                    console.log(event.data);
//                };
//            }
//            function CloseSSE() {
//                if (source != null) {
//                    source.close();
//                }
//            }
        </script>
    </body>
</html>