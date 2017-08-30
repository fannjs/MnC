<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="main.aspx.cs" Inherits="Maestro.views.kioskMaintenance.businessDay.main" %>

<style type="text/css">
    #setup-day-option
    {
        padding: 10px 0px;
    }
    #select-first-day
    {
        padding: 6px 8px;
        border-color: #999;
        color: #555;
        background-color: #FDFDFD;
    }
    .label-name
    {
        padding-right: 8px;
    }
    #setup-day-table-div
    {
        padding: 10px 0px;
    }
    .day-month-table
    {
        width: 100%;
        border: 1px solid #999;
    } 
    .day-month-table > thead > tr > th
    {
        border-bottom: 1px solid #999; 
        background-color: #eee;
        padding: 8px;
    }
    .day-month-table > tbody > tr > td
    {
        padding: 8px;
    }
    #setup-calendar-btn-div
    {
        padding: 10px;
    }
    #setup-day-table
    {
        width: 800px;
    }
    @media (max-width: 1120px)
    {
        #setup-day-table
        {
            width:100%;
        }
    }
</style>
<script type="text/javascript">

    function getDayList() {

        var str = "";
        var options = "";

        $.ajax({
            type: "POST",
            contentType: "application/json; charset=utf-8",
            url: "/views/kioskMaintenance/businessDay/main.aspx/getDayList",
            data: "{}",
            dataType: "json",
            success: function (data) {

                str = str + '<thead><tr><th>Day Number</th><th style="width: 70%;">Day Name</th><th>Business Day</th></tr></thead>';
                str = str + '<tbody>';

                for (var i = 0; i < data.d.length; i++) {

                    options = options + '<option value="' + data.d[i].DayID + '">' + data.d[i].DayName + '</option>';

                    str = str + '<tr><td class="day-number">' + data.d[i].DayID + '</td><td class="day-name" data-day="' + data.d[i].DayName + '">' + data.d[i].DayName + '</td>';

                    if (data.d[i].IsBusinessDay.toLowerCase() == "true") {
                        str = str + '<td><input type="checkbox" checked /></td>';
                    }
                    else {
                        str = str + '<td><input type="checkbox" /></td>';
                    }

                    str = str + '</tr>';
                }

                str = str + '</tbody>';

                $('#select-first-day').html(options);
                $('#setup-day-table').html(str);
            },
            error: function (error) {
                alert("Error occurs when trying to loading list of days.");
            }
        });
    }

    function selectFirstDayOfTheWeek() {

        $('#select-first-day').change(function () {

            var trArray = [];
            var option = $(this).find('option:selected').text();
            var startPoint = 0;
            var TRs = "";
            var incrementNo = 0;

            $('#setup-day-table > tbody tr').each(function () {
                var dayName = $(this).find('.day-name').text();
                var isBusinessDay;

                if ($(this).find('input:checkbox').is(':checked')) {
                    isBusinessDay = true;
                }
                else {
                    isBusinessDay = false;
                }

                var aDay = {
                    DayName: dayName,
                    isBusiness: isBusinessDay
                };

                trArray.push(aDay);
            });

            for (var i = 0; i < trArray.length; i++) {

                if (trArray[i].DayName == option) {
                    startPoint = i;
                    break;
                }
                else {
                    continue;
                }
            }

            for (var j = i; j < trArray.length; j++) {
                TRs = TRs + '<tr><td class="day-number">' + (incrementNo = incrementNo + 1) + '</td><td class="day-name">' + trArray[j].DayName + '</td>'

                if (trArray[j].isBusiness) {
                    TRs = TRs + '<td><input type="checkbox" checked /></td>';
                }
                else {
                    TRs = TRs + '<td><input type="checkbox" /></td>';
                }
            }

            for (var k = 0; k < i; k++) {
                TRs = TRs + '<tr><td class="day-number">' + (incrementNo = incrementNo + 1) + '</td><td class="day-name">' + trArray[k].DayName + '</td>'

                if (trArray[k].isBusiness) {
                    TRs = TRs + '<td><input type="checkbox" checked /></td>';
                }
                else {
                    TRs = TRs + '<td><input type="checkbox" /></td>';
                }
            }

            $('#setup-day-table > tbody').html(TRs);
        });
    }

    function Day(dayID, dayName, isBusinessDay) {
        this.dayID = dayID;
        this.dayName = dayName;
        this.isBusinessDay = isBusinessDay;
    }

    function saveToDatabase() {

        $('#save-calendar-btn').click(function () {

            var aWeek = [];

            $('#setup-day-table > tbody tr').each(function () {
                var dayID = $(this).find('.day-number').text();
                var dayName = $(this).find('.day-name').text();
                var isBusinessDay;

                if ($(this).find('input:checkbox').is(':checked')) {
                    isBusinessDay = true;
                }
                else {
                    isBusinessDay = false;
                }

                aWeek.push(new Day(dayID, dayName, isBusinessDay));
            });

            $.ajax({
                type: "POST",
                contentType: "application/json; charset=utf-8",
                url: "/views/kioskMaintenance/businessDay/main.aspx/saveCalendarSetting",
                data: JSON.stringify({ aWeek: aWeek }),
                dataType: "json",
                success: function (data) {

                    var status = data.d.Status;
                    var msg = data.d.Message;

                    if (!status) {
                        alert(msg);
                        return false;
                    }

                    alert("Successful. Your action will be sent to checker for approval.");
                },
                error: function (error) {
                    alert("Error occurs when trying to save the calendar setting. Error " + error.status);
                }
            });
        });
    }

    function off_events() {
        $('#setup-calendar-main').off();
    }

    $(document).ready(function () {

        off_events();

        /* Day & Month list will be populated when document load */
        getDayList();

        selectFirstDayOfTheWeek();

        saveToDatabase();
    });

</script>
<div id="setup-calendar-main">
    <div id="setup-day-tab">
        <h4>Day Setup</h4>
        <hr />
        <div id="setup-day-option">
            <span class="label-name">First day of the week: </span>
            <select id="select-first-day"></select>
        </div>
        <div id="setup-day-table-div">
            <table id="setup-day-table" class="table table-bordered">

            </table>
        </div>
        <button class="btn btn-primary" id="save-calendar-btn">Save</button>
    </div> 
</div>

