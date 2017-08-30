<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="main.aspx.cs" Inherits="Maestro.views.kioskMaintenance.holidayMaintenance.main" %>

<link rel="Stylesheet" href="../../../components/fullcalendar/fullcalendar.css" />
<link rel="Stylesheet" href="../../../components/fullcalendar/fullcalendar.print.css" media="print" />
<link rel="Stylesheet" href="../../../assets/styles/calendar.css" />
<div id="calendar-div">
    <div id="calendar-setting-div">
        <div id="option-div">
            <div>
                <select id="holiday-type" class="calendar-selection">
                    <option value="0">National Holiday</option>
                    <option value="1">Specific Holiday</option>
                </select>
            </div>
        </div>
        <hr />
        <div id="calendar"></div>
    </div>    
    <div id="holiday-list"></div>
</div>
<script src="../../../components/fullcalendar/fullcalendar.min.js" type="text/javascript"></script>
<script src="../../../assets/scripts/kioskMaintenance/holdiay.js"></script>
