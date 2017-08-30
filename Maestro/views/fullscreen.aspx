<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="index.aspx.cs" Inherits="Maestro.index" %>
<!DOCTYPE html>
<html>
    <head>
        <link rel="stylesheet" href="http://fonts.googleapis.com/css?family=Open+Sans:400italic,600italic,800italic,400,600,800" type="text/css">
        <link rel="stylesheet" href="../components/bootstrap3/css/bootstrap.min.css">
        <link rel="stylesheet" href="../components/font-awesome/font-awesome.min.css">
        <link rel="stylesheet" href="../assets/styles/dashboard.css">
        <style type="text/css">
            body{
                color: #fff;
                font: 13px/1.7em "Open Sans", "trebuchet ms", arial, sans-serif;
                background: linear-gradient(to bottom, #202020 0%, #000000 100%) repeat-x scroll 0 0 #222222;
            }
            #header{
                position: relative;
                width: 100%;
                height: 70px;
            }
            #logged-user-info, userName{
                position: relative;
                color: #fff;
                float: right;
                top: 5px;
                width: 250px;
                text-align: left;
            }
            #date-time-div{
                position: relative;
                color: #fff;
                float: right;
                text-align: left;
                width: 250px;
                top: 30px;
                left: 250px;
            }
            #brand-logo{
                margin-top: 0;
                left: 5px;
                position: absolute;
                top: 10px;
            }
            #content-mainpage-container{
                padding: 20px;
            }
            .portlet-placeholder
            {
                border: 1px dotted white !important;
            }
            /*
            .ui-sortable-placeholder { 
                border: 1px dotted white !important;
            }
            */
            /* Overriding dashboard CSS */
            .portlet .portlet-content {
                background-color: #4C4C4C !important;
                border: none !important;
                background-image: none !important;
                margin-top: 0px !important;
            }
            .portlet .portlet-header{
                background-color: #3E3E3E !important;
                border: none !important;
                background-image: none !important;
            }
            .portlet .portlet-header h3{
                text-shadow: none !important;
                color: #fff !important;
            }
            #statusTable a,
            #statusTable a:hover,
            .clickable-link,
            .clickable-link:hover{
                color: #fff !important;
            }
            .circle{
                width: auto !important;
                height: auto !important;
                font-size: 20px !important;
                color: #fff !important;
            }
            .btn-color{
                background-color: #292929!important;
                color: #fff !important;
            }
            .modal-body .well table,
            #quickView-modal .tab-content .tab-pane table,
            #machine-location-div{
                color: #000 !important;
            } 
            .nav-tabs li a:active,
            .nav-tabs li a:focus{
                outline:none;
            }

            /*            #statusTable td,
                        #urgentQueueList td{
                            color: #fff !important;
                        }*/
            .modal{
                top: -10px;
            }
            .modal-header{
                background: linear-gradient(to bottom, #FFFFFF 0%, #EEEEEE 100%);
                color: #000;
                padding-top: 10px;
                padding-bottom: 10px;
                padding-left: 15px;
            }
            .modal-content{
                border-radius: 0;;
            }
            .modal-footer{
                text-align: left;
                margin-top: 0px;
                padding: 10px;
            }
        </style>
    </head>
    <body>
        <header id="header">
            <div id="logged-user-info">You are logged on as <span id="userName" runat="server">username</span>
            </div>
            <div id="date-time-div">
                <div id="date-time">
                </div>
            </div> 
            <h1 id="brand-logo">
                <a href="index.aspx">
                    <img src="/assets/images/logo.png">
                </a>
            </h1>
        </header>
        <div id="content">
            <div id="content-container">
                <div id="content-mainpage-container">

                </div>
            </div>
        </div>
        
        <script src="../components/jquery/jquery.js"></script>
        <script src="../components/bootstrap3/js/bootstrap.min.js"></script>
        <script src="../components/moment/moment.min.js"></script>
        <script src="../components/jquery-ui/jquery-ui.min.js"></script>
        <script src="../components/jquery-tablesorter/jquery.tablesorter.min.js"></script>
        <script type="text/javascript" src="../assets/scripts/charts/Chart.js"></script>
        <script type="text/javascript" src="../assets/scripts/charts/ChartLegend.js"></script>
        <script type="text/javascript">
            function realTime() {
                var date_today = moment().format('dddd, MMMM D YYYY');
                var time_now = moment().format('h:mm:ss A');
                $('#date-time').html("Today is " + date_today + "<br/>It is now " + time_now);
            }
            function populateSizeForContent() {
                var $window_height = $(window).height();
                var $header_height = $("#header").height();
                var ComputeSize = $window_height - $header_height;

                $("#content-mainpage-container").css("min-height", ComputeSize);
            }
            $(document).ready(function () {
                realTime();
                setInterval(realTime, 1000); //for real-time clock
                populateSizeForContent();
                console.log($(document).height());
                console.log($(window).height());
                $(window).resize(function () {
                    populateSizeForContent();
                });

                $.post("dashboard.aspx", function (data) {
                    $("#content-mainpage-container").html(data);
                });
            });
        </script>
    </body>
</html>