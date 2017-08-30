<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="main.aspx.cs" Inherits="Maestro.views.kioskManagement.manageKiosk.main" %>

<style type="text/css">
#kiosk-list-div
{
    display: inline-block;
    vertical-align: top;
    border-radius: 4px;
}
#search-kiosk-div
{
    border: 1px solid #CCC;
    border-top-left-radius: 4px;
    border-top-right-radius: 4px;
    padding: 16px 8px;
    background-color: #F0F0F0;
    border-bottom-color: #BBB;
}
#search-icon
{
    position: absolute;
    margin-left: 8px;
    margin-top: 2px;
    opacity: 0.5;
}
#searchKioskTF
{
    padding: 2px 12px 2px 28px;
    color: #555;
    border: 1px solid #CCC;
    background-color: #FFFFFE;
    border-radius: 2px;
}
#list-of-kiosk
{
    padding: 4px 0px;
    text-align: center;
    height: 450px;
    background-color: #FCFCFC;
    overflow: auto;
    border: 1px solid #CCC;
    border-top:none;
}
.m-id
{
    padding: 2px;
    margin: 2px 0px;
}
.m-id:hover
{
    background-color: #FDE0D8;
    cursor: pointer;
}
.m-id.active
{
    background-color: #D44C23;
    color:#FFF;
    font-weight: bold;
    box-shadow: 0px 1px 4px 0px #000;
}
.m-id.active
{
    cursor: default;
}
#k-id-span
{
    font-weight: bold;
    padding: 0px 4px;
}
.selection-input
{
    padding: 3px 6px;
    border: 1px solid #999;
}
#updt-btn
{
    
}
</style>
<script type="text/javascript">

    function toggleEditKioskForm() {

        $('#manage-kiosk-main-div').on('click', '.m-id', function () {

            if ($(this).hasClass('active')) {
                return false;
            }
            else {
                $('#edit-kiosk-div').hide();
                $('.m-id').removeClass('active');
                $(this).addClass('active');
                $('#k-id-span').html($(this).text());
                loadCalendar();
                $('#edit-kiosk-div').fadeIn('fast');
            }
        });
    }

    function loadMachines() {

        $.ajax({
            type: "POST",
            contentType: "application/json; charset=utf-8",
            url: "/views/kioskManagement/manageKiosk/main.aspx/getMachines",
            data: "{}",
            dataType: "json",
            success: function (data) {

                var str = "";

                for (var i = 0; i < data.d.length; i++) {
                    str = str + '<div class="m-id">' + data.d[i].machID + '</div>';
                }

                $('#list-of-kiosk').html(str);
            },
            error: function (jqXHR, textStatus, errorThrown) {

            }
        });
    }

    function searchMachines() {

        $('#searchKioskTF').keyup(function () {
            var pattern = $(this).val();
            var mID = $('#k-id-span').text();

            $.ajax({
                type: "POST",
                contentType: "application/json; charset=utf-8",
                url: "/views/kioskManagement/manageKiosk/main.aspx/searchMachine",
                data: JSON.stringify({ pattern: pattern }),
                dataType: "json",
                success: function (data) {

                    var str = "";

                    if (data.d.length == 0) {
                        str = str + 'No record was found.';
                    }
                    else {
                        for (var i = 0; i < data.d.length; i++) {
                            str = str + '<div class="m-id">' + data.d[i].machID + '</div>';
                        }
                    }
                    $('#list-of-kiosk').html(str);

                    var machID_List = document.getElementById("list-of-kiosk").getElementsByClassName("m-id");
                    for (var j = 0; j < machID_List.length; j++) {
                        if (machID_List[j].innerText == mID) {
                            machID_List[j].className = machID_List[j].className + " active";
                            break;
                        }
                        else {
                            continue;
                        }
                    }

                },
                error: function (jqXHR, textStatus, errorThrown) {

                }
            });
        });
    }

    function loadCalendar() {
        $.ajax({
            type: "POST",
            contentType: "application/json; charset=utf-8",
            url: "/views/kioskManagement/manageKiosk/main.aspx/getCalendar",
            data: "{}",
            dataType: "json",
            success: function (data) {
                var str = '<div class="field-group"><label class="form-label form-label-width-xs">Calendar</label><select id="selectCalendar" class="selection-input"><option selected disabled> - Please Select - </option>';

                if (data.d.length == 0) {
                    str = str + '<option disabled> No Calendar </option>';
                }
                else {
                    for (var i = 0; i < data.d.length; i++) {
                        str = str + '<option value="' + data.d[i].calendarID + '">' + data.d[i].calendarName + '</option>';
                    }
                }

                str = str + '</select></div>';

                $('#edit-kiosk-form').html(str);

                loadMachineDetails();
            },
            error: function (jqXHR, textStatus, errorThrown) {

            }
        });
    }

    function loadMachineDetails() {
        var mID = $('#k-id-span').text();

        $.ajax({
            type: "POST",
            contentType: "application/json; charset=utf-8",
            url: "/views/kioskManagement/manageKiosk/main.aspx/getMachineDetail",
            data: JSON.stringify({ mID: mID }),
            dataType: "json",
            success: function (data) {

                if (data.d[0].calID !== "") {
                    var calendars = document.getElementById("selectCalendar").getElementsByTagName("option");

                    calendars[0].remove();

                    for (var i = 0; i < calendars.length; i++) {
                        if (calendars[i].value == data.d[0].calID) {
                            calendars[i].setAttribute("selected", "selected");
                            break;
                        }
                        else {
                            continue;
                        }
                    }
                }
            },
            error: function (jqXHR, textStatus, errorThrown) {

            }
        });
    }

    function update() {
        var mID = $('#k-id-span').text();
        var cal = $('#selectCalendar').val();

        if (cal == null) {
            alert("Please choose a calendar.");
        }
        else {

            $.ajax({
                type: "POST",
                contentType: "application/json; charset=utf-8",
                url: "/views/kioskManagement/manageKiosk/main.aspx/updateMachine",
                data: JSON.stringify({ mID: mID, cal: cal }),
                dataType: "json",
                success: function (data) {
                    if (data.d.Status) {
                        alert(data.d.Description);
                        $('#edit-kiosk-div').fadeOut('fast');
                        $('.m-id').removeClass('active');
                        $('#k-id-span').empty();

                        $('#searchKioskTF').val("");
                        loadMachines();
                    }
                    else {
                        alert(data.d.Description);
                    }
                },
                error: function (jqXHR, textStatus, errorThrown) {

                }
            });

        }
    }

    function cancelUpdate() {
        $('#edit-kiosk-div').fadeOut('fast');
        $('.m-id').removeClass('active');
        $('#k-id-span').empty();
    }

    $(document).ready(function () {
        toggleEditKioskForm();
        loadMachines();
        searchMachines();
    });

</script>
<div id="manage-kiosk-main-div">
    <div id="kiosk-list-div">
        <div id="search-kiosk-div">
            <span id="search-icon"><i class="fa fa-search"></i></span><input type="text" id="searchKioskTF" placeholder="Kiosk ID"/>
        </div>
        <div id="list-of-kiosk">

        </div>
    </div>
    <div style="width: 5px;display:inline-block;"></div>
    <div id="edit-kiosk-div" style="display:none;" class="well-div">
        <h4>Kiosk ID: <span id="k-id-span"></span></h4>
        <hr style="margin-top: 10px; margin-bottom: 10px;"/>
        <div class="block-xs"></div>
        <div id="edit-kiosk-form">
        
        </div>
        <div class="block-xs"></div>
        <hr style="margin-top: 10px; margin-bottom: 10px;"/>
        <div>
            <button id="updt-btn" access-gate task='Manage Kiosk' permission='Edit' onclick="update()" class="btn btn-primary size-default">Update</button>
            <button id="ccl-updt-btn" onclick="cancelUpdate()" class="btn btn-default size-default">Cancel</button>
        </div>
    </div>
</div>
