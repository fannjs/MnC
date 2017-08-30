/*
This involved with fullcalendar.js
Please refer to documentation @ http://arshaw.com/fullcalendar/docs/.
*/
var headerTitle = "Holiday Maintenance";

function loadCalendar() {

    $.ajax({
        type: "POST",
        contentType: "application/json; charset=utf-8",
        url: "/views/kioskMaintenance/holidayMaintenance/main.aspx/getFirstDayOfWeek",
        data: "{}",
        dataType: "json",
        success: function (data) {

            var firstDay;

            switch (data.d.toLowerCase()) {
                case "sunday":
                    firstDay = 0;
                    break;
                case "monday":
                    firstDay = 1;
                    break;
                case "tuesday":
                    firstDay = 2;
                    break;
                case "wednesday":
                    firstDay = 3;
                    break;
                case "thursday":
                    firstDay = 4;
                    break;
                case "friday":
                    firstDay = 5;
                    break;
                case "saturday":
                    firstDay = 6;
                    break;
                default:
                    firstDay = 1; //Default set to Monday
                    break;
            }

            $('#calendar').fullCalendar({
                header: {
                    left: '',
                    center: 'title',
                    right: 'prevYear,prev today next,nextYear,'
                },
                firstDay: firstDay,
                weekMode: 'variable',
                dayClick: function (date, jsEvent, view) {

                    $(this).addClass('highlighted-cell');

                    loadModal(date);
                },
                eventClick: function (calEvent, jsEvent, view) {
                    getHolidayDetail(calEvent);
                    $('#holiday-description-tooltip').remove();
                },
                eventMouseover: function (calEvent, jsEvent, view) {
                    var x = jsEvent.pageX;
                    var y = jsEvent.pageY;

                    if (calEvent.description !== "") {
                        $('html').append('<div id="holiday-description-tooltip">' + calEvent.description + '</div>');
                    }
                    else {
                        $('html').append('<div id="holiday-description-tooltip">No Description</div>');
                    }

                    $('#holiday-description-tooltip').css({ top: y, left: x });
                },
                eventMouseout: function (calEvent, jsEvent, view) {
                    $('#remove-holiday-icon').remove();
                    $('#holiday-description-tooltip').remove();
                }
            });

            $('.fc-header .fc-header-left').html('<span><button id="show-holiday-list"><i class="fa fa-list-alt"></i>List View</button></span>');
            loadHolidays(getSelectedCalendarName());
        },
        error: function (error) {
            alert("Error occurs when trying to load holiday calendar.");
        }
    });
}

function loadHolidays(calendarName) {
    $.ajax({
        type: "POST",
        contentType: "application/json; charset=utf-8",
        url: "/views/kioskMaintenance/holidayMaintenance/main.aspx/getHolidayCalendar",
        data: "{calendarName:'" + calendarName + "'}",
        dataType: "json",
        success: function (data) {

            var status = data.d.Status;
            var msg = data.d.Message;
            var HolidayList = data.d.Object;

            if (!status) {
                alert(msg);
                return false;
            }

            $('#calendar').fullCalendar('removeEvents', function (event) {
                return true;
            });

            $('#calendar').show();

            for (var i = 0; i < HolidayList.length; i++) {
                var Holiday = HolidayList[i];
                var date = moment(Holiday.HolidayDate, "DD-MM-YYYY");

                var event = {
                    title: Holiday.HolidayName,
                    start: date.format("YYYY-MM-DD"),
                    description: Holiday.HolidayNote,
                    calID: Holiday.CalendarID
                };

                if (Holiday.CalendarID == "1") {
                    event["className"] = 'national-holiday';
                }
                else {
                    event["className"] = 'state-holiday';
                }

                $('#calendar').fullCalendar('renderEvent', event, true);
            }
        },
        error: function (error) {
            alert("Error occurs when trying to load holiday calendar.");
        }
    });
}

function switchHoliday() {
    $('#holiday-type').change(function () {
        var type = $(this).val();

        if (type !== '0') {

            $('#calendar').hide();
            switchCalendarName();

            $.ajax({
                type: "POST",
                contentType: "application/json; charset=utf-8",
                url: "/views/kioskMaintenance/holidayMaintenance/main.aspx/getSpecificCalendar",
                data: "{}",
                dataType: "json",
                beforeSend: function () {
                    $('#holiday-type').attr('disabled', true);
                    $('#holiday-type').after('<select id="select-calendar-id" class="calendar-selection" style="margin-left: 2px;"></select><span id="new-calendar-div" style="margin-left: 2px;"><a><i class="fa fa-plus" style="width:15px;"></i>new calendar</a></span>');
                },
                success: function (data) {

                    var str = "";

                    if (data.d.length == 0) {
                        str = str + '<option selected disabled>No Calendar yet</option>';
                    }
                    else {
                        str = str + '<option selected disabled> - Please choose - </option>';
                        for (var i = 0; i < data.d.length; i++) {
                            str = str + '<option value="' + data.d[i].CalendarID + '">' + data.d[i].CalendarName + '</option>';
                        }
                    }

                    $('#select-calendar-id').html(str);
                    $('#holiday-type').attr('disabled', false);
                },
                error: function (error) {
                    alert("Error occurs when trying to load calendar IDs.");
                    $('#holiday-type').attr('disabled', false);
                }
            });
        }
        else {
            switchCalendarName();
            loadHolidays(type);
            $('#select-calendar-id').remove();
            $('#new-calendar-div').remove();
        }
    });

    $('#calendar-div').on('change', '#select-calendar-id', function () {
        var calendarName = $(this).find('option:selected').text();

        $('#select-calendar-id option[disabled]').remove();

        switchCalendarName();
        loadHolidays(calendarName);
    });

    $('#calendar-div').on('click', '#new-calendar-div > a', function () {
        $(this).parent().html('<input type="text" id="calendar-id-TF" placeholder="New calendar name" /><button id="cancel-new-calendar" class="new-calendar-btn"><i class="fa fa-times"></i></button><button id="save-new-calendar" class="new-calendar-btn" style="display:none;"><i class="fa fa-check"></i></button>');
        $('#calendar-id-TF').focus();

        $('#select-calendar-id').attr('disabled', true);
        $('#calendar').hide();
        $('#content-title').html(headerTitle);

    }).on('keyup', '#calendar-id-TF', function () {
        if ($(this).val().trim() !== "") {
            $('#save-new-calendar').fadeIn('fast');
        }
        else {
            $('#save-new-calendar').hide();
        }
    });

    $('#calendar-div').on('click', '#save-new-calendar', function () {

        var calendarID_TF = $('#calendar-id-TF');
        var saveBtn = $('#save-new-calendar');
        var calendarOptions = document.getElementById("select-calendar-id").getElementsByTagName("option");

        if (calendarID_TF.val().trim() == "") {
            alert("Please enter calendar ID.");
        }
        else {
            var same = false;

            saveBtn.attr('disabled', true);
            calendarID_TF.attr('disabled', true);

            for (var i = 0; i < calendarOptions.length; i++) {

                if (calendarOptions[i].innerText.toLowerCase() == calendarID_TF.val().toLowerCase()) {
                    same = true;
                    break;
                }
            }

            if (same) {
                alert("This calendar name has been used. Please enter another.");

                saveBtn.attr('disabled', false);
                calendarID_TF.attr('disabled', false);
            }
            else {
                alert("Calendar name saved.");

                $('#select-calendar-id option[disabled]').remove();

                $('#select-calendar-id').append('<option selected>' + calendarID_TF.val() + '</option>');
                $('#new-calendar-div').html('<a>new calendar</a>');
                $('#select-calendar-id').attr('disabled', false);

                loadHolidays($('#select-calendar-id').val());
                switchCalendarName();
            }
        }
    }).on('click', '#cancel-new-calendar', function () {

        var calendarID = $('#select-calendar-id').val();

        $('#new-calendar-div').html('<a><i class="fa fa-plus" style="width:15px;"></i>new calendar</a>');
        $('#select-calendar-id').attr('disabled', false);

        if (calendarID !== "" && calendarID !== null) {
            loadHolidays(calendarID);
            switchCalendarName();
        }
    });
}
/*
When a calendar is selected, the content title will show the calendar name.
For example: "Calendar (National Holiday)"
*/
function switchCalendarName() {
    var calendarName = getSelectedCalendarName();
    
    if (calendarName !== undefined && calendarName !== null && calendarName !== "") {
        $('#content-title').html(headerTitle + ' (' + calendarName + ')');
    }
    else {
        $('#content-title').html(headerTitle);
    }
}

function loadModal(date) {

    var str = "";
    var selectedDate = date.format("Do MMMM YYYY");

    str = str + '<div id="setup-holiday-modal">';
    str = str + '<div id="setup-holiday-bg"></div>';
    str = str + '<div id="setup-holiday-main-div">';
    str = str + '<h4>Setup Holiday for <span id="selected-date" data-date="' + date + '">' + selectedDate + '</span></h4><hr/>';
    str = str + '<table><tr><td>';
    str = str + '<label>Holiday Name</label></td><td><input type="text" id="holidayNameTF" class="holiday-calendar-input" /></td></tr>';
    str = str + '<tr><td>';
    str = str + '<label>Holiday Description</label><br/>(Optional)</td><td><textarea id="holidayNoteTA" rows="4" class="holiday-calendar-input" ></textarea></td></tr>';
    str = str + '<tr><td></td><td id="button-section"><button type="button" class="btn btn-primary" id="save-setting-btn">Save</button></td></tr></table>';
    str = str + '</div>';
    str = str + '</div>';

    $('#calendar-div').append(str);

    $('#holidayNameTF').focus();
}

function closeModal() {
    $('#calendar-div').on('click', '#setup-holiday-bg', function () {
        $('.fc-day').removeClass('highlighted-cell');
        $('#setup-holiday-modal').remove();
    });
}

function getHolidayDetail(calEvent) {

    var holidayDate = calEvent.start.format("DD/MM/YYYY");
    var calendarID = calEvent.calID;

    $.ajax({
        type: "POST",
        contentType: "application/json; charset=utf-8",
        url: "/views/kioskMaintenance/holidayMaintenance/main.aspx/getHolidayDetail",
        data: JSON.stringify({ holidayDate: holidayDate, calendarID: calendarID }),
        dataType: "json",
        success: function (data) {

            var hDate = moment(data.d[0].HolidayDate, "DD-MM-YYYY");

            loadModal(hDate);

            $('#setup-holiday-main-div > h4').html(data.d[0].HolidayName + ' - <span id="selected-date" data-date="' + hDate + '" data-calID="' + calendarID + '">' + hDate.format("Do MMMM YYYY") + '</span>');
            $('#holidayNameTF').val(data.d[0].HolidayName);
            $('#holidayNoteTA').val(data.d[0].HolidayNote);
            $('#holidayNameTF').focus();

            $('#button-section').html('<button type="button" class="btn btn-primary" id="update-setting-btn">Update</button><button type="button" class="btn btn-danger" id="delete-holiday-btn"><i class="fa fa-trash-o"></i>Delete</button>');
        },
        error: function (error) {
            alert("Error occurs when trying to load holiday detail.");
        }
    });
}

function getSelectedCalendarName() {
    var calID;
    if ($('#holiday-type').val() == '0') {
        calID = $('#holiday-type option:selected').text();
    }
    else {
        calID = $('#select-calendar-id option:selected').text();
    }

    return calID;
}

function saveHolidaySetting() {
    $('#calendar-div').on('click', '#save-setting-btn', function () {
        var date = moment(parseInt($('#selected-date').attr('data-date')));

        var holidayDate = date.format("DD/MM/YYYY");
        var dayOfMonthID = date.format("D");
        var monthOfYearID = date.format("M");
        var holidayYear = date.format("YYYY");
        var dayOfWeekID = date.format("E");
        var calendarName = getSelectedCalendarName();

        var holidayName = $('#holidayNameTF').val();
        var holidayNote = $('#holidayNoteTA').val();

        $(this).attr("disabled", true);

        if (holidayName.trim() == "") {
            alert("Please enter Holiday Name.");
            $(this).attr("disabled", false);

            return false;
        }

        $.ajax({
            type: "POST",
            contentType: "application/json; charset=utf-8",
            url: "/views/kioskMaintenance/holidayMaintenance/main.aspx/insertHoliday",
            data: JSON.stringify({
                holidayDate: holidayDate,
                dayOfMonthID: dayOfMonthID,
                monthOfYearID: monthOfYearID,
                holidayYear: holidayYear,
                dayOfWeekID: dayOfWeekID,
                calendarName: calendarName,
                holidayName: holidayName,
                holidayNote: holidayNote
            }),
            dataType: "json",
            success: function (data) {

                var status = data.d.Status;
                var msg = data.d.Message;

                if (!status) {
                    alert(msg);
                    $(this).attr("disabled", false);
                    return false;
                }

                alert("Successful. Your action has been sent to checker for approval.");
                //loadHolidays(calendarName); //refetch events from database
                $('.fc-day').removeClass('highlighted-cell');
                $('#setup-holiday-modal').remove();
            },
            error: function (error) {
                alert("Error occurs when trying to setup holiday.");
                $(this).attr("disabled", false);
            }
        });
    });
}

function updateHolidaySetting() {
    $('#calendar-div').on('click', '#update-setting-btn', function () {
        var date = moment(parseInt($('#selected-date').attr('data-date')));

        var holidayDate = date.format("DD/MM/YYYY");
        var calendarID = $('#selected-date').attr('data-calID');
        var holidayName = $('#holidayNameTF').val();
        var holidayNote = $('#holidayNoteTA').val();

        $(this).attr("disabled", true);

        if (holidayName.trim() == "") {
            alert("Please enter Holiday Name.");
            $(this).attr("disabled", false);

            return false;
        }

        $.ajax({
            type: "POST",
            contentType: "application/json; charset=utf-8",
            url: "/views/kioskMaintenance/holidayMaintenance/main.aspx/updateHoliday",
            data: JSON.stringify({
                holidayDate: holidayDate,
                calendarID: calendarID,
                holidayName: holidayName,
                holidayNote: holidayNote
            }),
            dataType: "json",
            success: function (data) {

                var status = data.d.Status;
                var msg = data.d.Message;

                if (!status) {
                    alert(msg);
                    $(this).attr("disabled", false);
                    return false;
                }

                alert("Successful. Your action has been sent to checker for approval.");

                //loadHolidays(getSelectedCalendarName()); //refetch events from database

                $('#setup-holiday-modal').remove();
            },
            error: function (error) {
                alert("Error occurs when trying to update the holiday detail.");
                $(this).attr("disabled", false);
            }
        });
    });
}

function deleteHoliday() {
    $('#calendar-div').on('click', '#delete-holiday-btn', function (event) {
        $(this).attr("disabled", true);
        var confirmed = confirm("Are you sure you want to delete?");

        if (confirmed) {
            var calendarID = $('#selected-date').attr('data-calID');
            var date = moment(parseInt($('#selected-date').attr('data-date')));
            var holidayDate = date.format("DD/MM/YYYY");

            $.ajax({
                type: "POST",
                contentType: "application/json; charset=utf-8",
                url: "/views/kioskMaintenance/holidayMaintenance/main.aspx/deleteHoliday",
                data: JSON.stringify({ holidayDate: holidayDate, calendarID: calendarID }),
                dataType: "json",
                success: function (data) {

                    var status = data.d.Status;
                    var msg = data.d.Message;

                    if (!status) {
                        alert(msg);
                        $(this).attr("disabled", false);
                        return false
                    }

                    alert("Successful. Your action has been sent to checker for approval.");

                    //loadHolidays(getSelectedCalendarName()); //refetch events from database

                    $('#setup-holiday-modal').remove();
                },
                error: function (error) {
                    alert("Error occurs when trying to delete the holiday.");
                    $(this).attr("disabled", false);
                }
            });
        }
        else {
            $(this).attr("disabled", false);
        }
    });
}

function showListOfHoliday() {
    $('#calendar-div').on('click', '#show-holiday-list', function () {
        var view = $('#calendar').fullCalendar('getView');
        var str = "";

        var calTitle = view.title;
        var holidayYear = calTitle.substring(calTitle.search(/[0-9]/i));

        $('#content-title').html(headerTitle + ' (' + holidayYear + ' Holiday)');

        $.ajax({
            type: "POST",
            contentType: "application/json; charset=utf-8",
            url: "/views/kioskMaintenance/holidayMaintenance/main.aspx/getHolidayList",
            data: JSON.stringify({ holidayYear: holidayYear }),
            dataType: "json",
            beforeSend: function () {
                $('#calendar-setting-div').hide();
                $('#holiday-list').show();
                $('#holiday-list').html('Loading...');
            },
            success: function (data) {

                str = str + '<h4>Holday of Year: ' + holidayYear + '<button id="view-full-calendar"><i class="fa fa-calendar"></i>Full View</button></h4>';
                str = str + '<table id="holiday-list-table" class="table"><thead><tr><th>No.</th><th>Date</th><th>Day</th><th>Holiday</th><th>Description</th><th>Calendar ID</th></tr></thead>';
                str = str + '<tbody>';

                if (data.d.length == 0) {

                    str = str + '<tr><td colspan="4">No Holiday found.</td></tr>';
                }
                else {
                    for (var i = 0; i < data.d.length; i++) {

                        var hDate = moment(data.d[i].HolidayDate, "DD-MM-YYYY");

                        if (data.d[i].CalendarID == '1') {
                            str = str + '<tr class="national-holiday"><td>' + (i + 1) + '</td><td>' + hDate.format("DD MMM") + '</td><td>' + hDate.format("ddd") + '</td><td>' + data.d[i].HolidayName + '</td><td>' + data.d[i].HolidayNote + '</td><td>' + data.d[i].CalendarName + '</td></tr>';
                        }
                        else {
                            str = str + '<tr class="state-holiday"><td>' + (i + 1) + '</td><td>' + hDate.format("DD MMM") + '</td><td>' + hDate.format("ddd") + '</td><td>' + data.d[i].HolidayName + '</td><td>' + data.d[i].HolidayNote + '</td><td>' + data.d[i].CalendarName + '</td></tr>';
                        }
                    }
                }

                str = str + '</tbody></table>';
                str = str + '<div><ul id="holiday-info-icons"><li><div class="info-icon national-holiday"></div>National Holiday</li><li><div class="info-icon state-holiday"></div>Specific Holiday</li></ul></div>';

                $('#holiday-list').html(str);
            },
            error: function (error) {
                alert("Error occurs when trying to load the holiday " + holidayYear + ".");
            }
        });
    });

    $('#calendar-div').on('click', '#view-full-calendar', function () {
        $('#holiday-list').hide();
        $('#calendar-setting-div').show();
        switchCalendarName();
    });
}

function off_eventHandlers() {
    $('#calendar-div').off();
}

$(document).ready(function () {
    off_eventHandlers();

    switchHoliday();
    switchCalendarName();
    loadCalendar();
    closeModal();
    saveHolidaySetting();
    updateHolidaySetting();
    deleteHoliday();
    showListOfHoliday();

    $('#calendar-div').keydown(function (e) {

        if (e.keyCode == '27') {
            if ($('#setup-holiday-modal').length > 0) {
                $('.fc-day').removeClass('highlighted-cell');
                $('#setup-holiday-modal').remove();
            }
        }
    });
});