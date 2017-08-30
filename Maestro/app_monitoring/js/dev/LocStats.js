


var globalErrType = '';

    function DisplayDateTime() {
        //alert("DisplayDateTime");
        if (!document.all && !document.getElementById)
            return
        timeElement = document.getElementById ? document.getElementById("curTime1") : document.all.tick2
        ddateElement = document.getElementById ? document.getElementById("curDdate1") : document.all.tick2

        var CurrentDate = new Date()
        var hours = CurrentDate.getHours()
        var minutes = CurrentDate.getMinutes()
        var seconds = CurrentDate.getSeconds()
        var DayNight = "PM"

        var year = CurrentDate.getYear()

        if (hours < 12) DayNight = "AM";
        if (hours > 12) hours = hours - 12;
        if (hours == 0) hours = 12;
        if (minutes <= 9) minutes = "0" + minutes;
        if (seconds <= 9) seconds = "0" + seconds;
        var currentTime = hours + ":" + minutes + ":" + seconds + " " + DayNight;
        timeElement.innerHTML = currentTime;

        if (year < 1000)
            year += 1900
        var day = CurrentDate.getDay()
        var month = CurrentDate.getMonth()
        var daym = CurrentDate.getDate()
        if (daym < 10)
            daym = "0" + daym
        var dayarray = new Array("Sunday", "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday")
        var montharray = new Array("January", "February", "March", "April", "May", "June", "July", "August", "September", "October", "November", "December")
        ddateElement.innerHTML =  dayarray[day] + ", " + montharray[month] + " " + daym + ", " + year;
        //setTimeout("DisplayDateTime()", 1000);
    }

    var t1 = setInterval("DisplayDateTime();", 1000);





jQuery(document).ready(function ($) {

    /*START: ELEMENT ACCESSOR*/

    function getSelectedSite(){
        return $('#ddlSite').find('option:selected').val();
    }

    /*END: ELEMENT ACCESSOR*/

    function addToNavList($clicker)
    {
        
        var $parent = $('.navContainer');
        var $child = $('<div>').addClass('navChild');
        var $operator = $('<div>').addClass('navChild navDirection').html('>');
        var className = $clicker.attr('class');

        ($parent.find('.navChild').length > 0)? $parent.append($operator) : '';
        ($parent.find('.'+ className).length > 0)? $parent.find('.'+ className).parent().nextAll().andSelf().remove() : '';
        $location = $clicker.clone(); 
        $location.html($location.html().split('(')[0]);

        $location.appendTo($child);
        $parent.append($child);
    }



function GenerateCountryList(aList, linkage) {

    var $errTr;
    var $errTd;
    $.each(aList, function (i, v) {

        $errTr = $('<tr>');
        $errTd = $('<td>');
        $errCountryLink = $('<a>');
        $errCountryLink.addClass(linkage).attr('href', 'javascript:void(0)').html(v.COUNTRY);

        $errTr.append($errCountryLink);

        $.each(v.ERROR, function (errType, errVal) {
            $errTd = $('<td>');
            $errTd.html(errVal).addClass('s' + errType);//purposely put at back

            if (errVal > 0) {
                $errTd.css('cursor', 'pointer');
                $errTd.bind('click', { country: v.COUNTRY, linkage: linkage, errType: errType }, onErrorTypeClicked);

            }
            $errTr.append($errTd);
        });


        $tblStatusBody.append($errTr);
    });
}


function onErrorTypeClicked(event) {
    var $this = $(this);
    var countryVal = event.data.country;
    var linkage = event.data.linkage;
    var errType = event.data.errType;
    globalErrType = errType;
    var linkageCls = linkage.split("_LINK")[0];//$this.attr('class');

    $.ajax({
        type: "POST",
        //url: "LocStatsService.asmx/GetLocationStatus",
        url: "app_monitoring/content/LocStatsService.asmx/GetLocationStatus",
        data: JSON.stringify({ linkage: linkageCls, selectorVal: countryVal, errType: errType }),
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: OnState_AjaxReturn,
        error: function (msg) {
            alert('woops');
        }
    });

    if (errType == 'OK') {
        $tblLocationBody = $("#divUrgencyQueue .tblLocation > tbody");
        $tblLocationBody.empty();
        return;
    }


    $.ajax({
        type: "POST",
        url: "app_monitoring/content/LocStatsService.asmx/GetUrgencyQueue",
        data: JSON.stringify({ country: countryVal }),
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: OnUrgentQueue_AjaxReturn,
        error: function (msg) {

        }
    });
}

/* START: ON AJAX SUCCESS-CALLBACK HANDLER */
var pattern = "_LINK";
var mID = "mID";
function OnCountry_AjaxReturn(msg) {
    var data = $.parseJSON(msg.d);
    $tblStatusBody = $("#divCountry .tblLocation > tbody");
    $tblStatusBody.empty();
    $('#divCountry').nextAll().addClass('anchor');
    $(".anchor .tblLocation > tbody").empty();
    $('#divCountry').nextAll().removeClass('anchor');

    $('#divCountry').parent().nextAll().addClass('anchor');
    $(".anchor .tblLocation > tbody").empty();
    $('#divCountry').parent().nextAll().removeClass('anchor');

    var aList = data.LIST;
    var linkage = data.NEXT_LINK + pattern;

    GenerateCountryList(aList, linkage);


    var selValue = getSelectedSite();
    //if succeded, update the headTitle
    $('#divCountry span.headTitle').html("Status for " + selValue);
}


function OnState_AjaxReturn(msg) {
    var data = $.parseJSON(msg.d);
    $tblLocationBody = $("#divState .tblLocation > tbody");

    $('#divCountry').parent().nextAll().addClass('anchor');
    $(".anchor .tblLocation > tbody").empty();
    $(".anchor .tblLocation > tbody").parent().parent().addClass('empty');

    $('#divCountry').parent().nextAll().removeClass('anchor');

    var resList = data.LIST;
    var linkage = data.NEXT_LINK + pattern;

    $.each(resList, function (i, v) {
        $tblLocationBody.append("<tr><td><a href=javascript:void(0) class=\"" + linkage + "\">" + v.LOCATION + "(" + v.COUNT + ")" + "</a></td></tr>");
    });
    $tblLocationBody.parent().parent().removeClass('empty');

}

function OnDistrict_AjaxReturn(msg) {
    var data = $.parseJSON(msg.d);
    $tblLocationBody = $("#divDistrict .tblLocation > tbody");

    $('#divState').nextAll().addClass('anchor');
    $(".anchor .tblLocation > tbody").empty();
    $(".anchor .tblLocation > tbody").parent().parent().addClass('empty');
    $('#divState').nextAll().removeClass('anchor');

    var resList = data.LIST;
    var linkage = data.NEXT_LINK + pattern;

    $.each(resList, function (i, v) {
        $tblLocationBody.append("<tr><td><a href=javascript:void(0) class=\"" + linkage + "\">" + v.LOCATION + "(" + v.COUNT + ")" + "</a></td></tr>");
    });
    $tblLocationBody.parent().parent().removeClass('empty');

}

function OnBranch_AjaxReturn(msg) {
    var data = $.parseJSON(msg.d);
    $tblLocationBody = $("#divBranch .tblLocation > tbody");

    $('#divDistrict').nextAll().addClass('anchor');
    $(".anchor .tblLocation > tbody").empty();
    $(".anchor .tblLocation > tbody").parent().parent().addClass('empty');
    $('#divDistrict').nextAll().removeClass('anchor');

    var resList = data.LIST;
    var linkage = data.NEXT_LINK + pattern;

    $.each(resList, function (i, v) {
        $tblLocationBody.append("<tr><td><a href=javascript:void(0) class=\"" + linkage + "\">" + v.LOCATION + "(" + v.COUNT + ")" + "</a></td></tr>");
    });
    $tblLocationBody.parent().parent().removeClass('empty');

}

function OnMachine_AjaxReturn(msg) {
    var data = $.parseJSON(msg.d);
    $tblLocationBody = $("#divMachine .tblLocation > tbody");

    $('#divBranch').nextAll().addClass('anchor');
    $(".anchor .tblLocation > tbody").empty();
    $(".anchor .tblLocation > tbody").parent().parent().addClass('empty');
    $('#divBranch').nextAll().removeClass('anchor');

    var resList = data.LIST;
    var linkage = data.NEXT_LINK + pattern;

    $.each(resList, function (i, v) {
        $tblLocationBody.append("<tr><td><a href=javascript:void(0) class=\"" + linkage + " " + mID + "\">" + v.LOCATION + "</a></td></tr>");
    });
    $tblLocationBody.parent().parent().removeClass('empty');

}


function OnUrgentQueue_AjaxReturn(msg) {
    var data = $.parseJSON(msg.d);
    $tblLocationBody = $("#divUrgencyQueue .tblLocation > tbody");
    $tblLocationBody.empty();
    var resList = data.LIST;

    $.each(resList, function (i, v) {
        //roy come back
        $row = $('<tr>');
        
        /*machine ID col*/
        $cMachineID = $('<td>').addClass('s' + v.ERROR).append($('<a>').addClass(mID).attr('href', 'javascript:void(0)').html(v.MACHINEID));
        $row.append($cMachineID);
        /*machine details col*/
        $cMachineDetails = $('<td>').addClass('sHIGHLIGHT');
        $row.append($cMachineDetails.clone().html(v.MACHINETYPE));
        $row.append($cMachineDetails.clone().html(v.ERRORDESC));
        $row.append($cMachineDetails.clone().html(v.TIME));

        $tblLocationBody.append($row);

    });
}
    /* END: ON AJAX SUCCESS-CALLBACK HANDLER */


    function showDialog(id, title) {
        $(id).dialog('option', 'title', title);
        $(id).dialog('open');
    }


   
    $('#divNavigation').accordion({
        heightStyle: "content",
        collapsible: true,
        active: false,
    });


    var iframe = $('<iframe id="myframe" frameborder="0" marginwidth="0" marginheight="0" allowfullscreen></iframe>');
    var dialog = $("<div></div>").append(iframe).appendTo("body").dialog({
        autoOpen: false,
        modal: true,
        resizable: false,
        width: "auto",
        height: "auto",
        dialogClass: "iFrameDialog blueOceanTopWindow",
        close: function () {
            iframe.attr("src", "");
        }
    });

    iframe.load(function () {
        
        var width = iframe.get(0).contentWindow.document.body.scrollWidth;//$(this).attr("data-width");
        var height = iframe.get(0).contentWindow.document.body.scrollHeight;//$(this).attr("data-height");
        iframe.attr({
            width: width,
            height: height,
        });
    });

    $('#cssmenu a'/*'#divNavigation a'*/).click(function (e) {
        
        openIframeDialogWithLink($(this));
        e.preventDefault();
    });

    /* param: a link with href and data-title set*/
    openIframeDialogWithLink = function($link)
    {
        var $this = $link;
        var pageName = $this.attr("href");
        var title = $this.attr("data-title");
       
        //alert("pageName = "+pageName + ", title = "+title);

        if (pageName == '#' || title == 'Logout')
            return;
         
         openIframeDialog(pageName, title);
    }

    function openIframeDialog(pageName, title)
    {
        var url = pageName + '.aspx';
        //alert("url = "+url);

        iframe.attr({
            src: url
        });
        dialog.dialog("option", "title", title).dialog("open");

    }

    $('.box').draggable({ containment: "body", scroll: false, handle: 'span.headTitle,span.draggable' });
    $("#divCurrentMachineStatus .calFrom").datepicker({
        onSelect: function (selected) {
            $("#divCurrentMachineStatus .calTo").datepicker("option", "minDate", selected)
        },
        dateFormat: 'dd/mm/yy',
        showOn: "button",
        buttonImage: "../Images/cal.gif",
        buttonImageOnly: true,
    });
    $("#divCurrentMachineStatus .calTo").datepicker({
        onSelect: function (selected) {
            $("#divCurrentMachineStatus .calFrom").datepicker("option", "maxDate", selected)
        },
        dateFormat: 'dd/mm/yy',
        showOn: "button",
        buttonImage: "../Images/cal.gif",
        buttonImageOnly: true,
    });


    $("#divTopProblemToday .calFrom").datepicker({
        onSelect: function (selected) {
            $("#divTopProblemToday .calTo").datepicker("option", "minDate", selected)
        },
        dateFormat: 'dd/mm/yy',
        showOn: "button",
        buttonImage: "../Images/cal.gif",
        buttonImageOnly: true,
    });
    $("#divTopProblemToday .calTo").datepicker({
        onSelect: function (selected) {
            $("#divTopProblemToday .calFrom").datepicker("option", "maxDate", selected)
        },
        dateFormat: 'dd/mm/yy',
        showOn: "button",
        buttonImage: "../Images/cal.gif",
        buttonImageOnly: true,
    });


    $("#divMachineStatusToday .calFrom").datepicker({
        onSelect: function (selected) {
            $("#divMachineStatusToday .calTo").datepicker("option", "minDate", selected)
        },
        dateFormat: 'dd/mm/yy',
        showOn: "button",
        buttonImage: "../Images/cal.gif",
        buttonImageOnly: true,
    });
    $("#divMachineStatusToday .calTo").datepicker({
        onSelect: function (selected) {
            $("#divMachineStatusToday .calFrom").datepicker("option", "maxDate", selected)
        },
        dateFormat: 'dd/mm/yy',
        showOn: "button",
        buttonImage: "../Images/cal.gif",
        buttonImageOnly: true,
    });


    $('#dlgMachineDetails').dialog({
        autoOpen: false,
        modal: true,
        draggable: true,
        resizable: false,
        position: ['center', 'top'],
        show: 'blind',
        hide: 'blind',
        width: 466,
        dialogClass: 'blueDialog blueOceanTopWindow'
    }).css('font-size', '.9em').css('font-family', 'Arial,Helvetica,sans-serif');

    $('#dlgMachineBranchInfo').dialog({
        autoOpen: false,
        modal: true,
        draggable: true,
        resizable: false,
        show: 'blind',
        hide: 'blind',
        width: '370',
        height: 'auto',
        dialogClass: 'smallTitleBar blueOceanTopWindow blueDialog'
    }).css('font-size', '.9em').css('font-family', 'Arial,Helvetica,sans-serif');

    $('#dlgNavigation').dialog({
        autoOpen: false,
        modal: true,
        draggable: true,
        resizable: false,
        position: ['center', 'top'],
        show: 'blind',
        hide: 'blind',
        width: 'auto',
        height: 'auto',
        dialogClass: 'iFrameDialog'
    });

    /* START: ON SELECTED HANDLER (AJAX) */
    var k;
    function qLiveTraffic(selCountry, selSite)
    {
        selCountry = selCountry || '';
        selSite = selSite || '';

        //console.log(selCountry + ',' + selSite);

        clearTimeout(k);    
        $.ajax({
            type: "POST",
            url: "app_monitoring/content/LocStatsService.asmx/GetLiveTraffics",
            contentType: "application/json; charset=utf-8",
            data: JSON.stringify({ country: selCountry, site: selSite }),
            dataType: "json",
            success: function (msg) {
                var data = $.parseJSON(msg.d);
                var errList = data.ERROR;

                $.each(errList, function (errType, count) {
                    $('#liveTraffic').find('.t' + errType).html(count);

                    if(count > 0 && (errType == 'ERROR' || errType == 'WARN'))
                    {
                        $('#liveTraffic').find('.t' + errType).addClass('t' + errType + '_FLASH');

                    }
                    else
                    {
                        $('#liveTraffic').find('.t' + errType).removeClass('t' + errType + '_FLASH');
                    }
                });

                k = setTimeout(function(){
                    qLiveTraffic(selCountry, selSite);
                }, 3000);

                
            },
            error: function (msg) {
                alert('ops');
            }
        });
    }


    $("#divBranch .tblLocation").on("click", "a[class$=_LINK]", function () {
        var $this = $(this);
        var locationVal = $this.html().split('(')[0];
        var linkageCls = $this.attr('class').split("_LINK")[0];//$this.attr('class');
        event.preventDefault();
        addToNavList($this);
        $.ajax({
            type: "POST",
            url: "app_monitoring/content/LocStatsService.asmx/GetLocationStatus",
            data: JSON.stringify({ linkage: linkageCls, selectorVal: locationVal, errType: globalErrType }),
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: OnMachine_AjaxReturn,
            error: function (msg) {

            }
        });
    });

    $("#divDistrict .tblLocation").on("click", "a[class$=_LINK]", function () {
        var $this = $(this);
        var locationVal = $this.html().split('(')[0];
        var linkageCls = $this.attr('class').split("_LINK")[0];//$this.attr('class');
        event.preventDefault();
        addToNavList($this);

        $.ajax({
            type: "POST",
            url: "app_monitoring/content/LocStatsService.asmx/GetLocationStatus",
            data: JSON.stringify({ linkage: linkageCls, selectorVal: locationVal, errType: globalErrType }),
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: OnBranch_AjaxReturn,
            error: function (msg) {

            }
        });
    });

    $("#divState .tblLocation").on("click", "a[class$=_LINK]", function () {
        var $this = $(this);
        var locationVal = $this.html().split('(')[0];
        var linkageCls = $this.attr('class').split("_LINK")[0];//$this.attr('class');
        event.preventDefault();
        addToNavList($this);

        $.ajax({
            type: "POST",
            url: "app_monitoring/content/LocStatsService.asmx/GetLocationStatus",
            data: JSON.stringify({ linkage: linkageCls, selectorVal: locationVal, errType: globalErrType }),
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: OnDistrict_AjaxReturn,
            error: function (msg) {

            }
        });
    });

    $("#divCountry .tblLocation").on("click", "a[class$=_LINK]", function () {
        var $this = $(this);
        var countryVal = $this.html();
        var linkageCls = $this.attr('class').split("_LINK")[0];//$this.attr('class');
        globalErrType = '';//clear
        event.preventDefault();
        addToNavList($this);

        var siteVal = getSelectedSite();

        qLiveTraffic(countryVal, siteVal);

        $.ajax({
            type: "POST",
            url: "app_monitoring/content/LocStatsService.asmx/GetUrgencyQueue",
            data: JSON.stringify({ country: countryVal }),
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: OnUrgentQueue_AjaxReturn,
            error: function (msg) {

            }
        });



        $.ajax({
            type: "POST",
            url: "app_monitoring/content/LocStatsService.asmx/GetLocationStatus",
            data: JSON.stringify({ linkage: linkageCls, selectorVal: countryVal, errType: globalErrType }),
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: OnState_AjaxReturn,
            error: function (msg) {

            }
        });
    });

    $('#ddlSite').change(function () {
        globalErrType = '';//clear
        var selValue = $(this).find('option:selected').val();

        qLiveTraffic('', selValue);
        $.ajax({
            type: "POST",
            url: "app_monitoring/content/LocStatsService.asmx/GetCountryStatus",
            data: JSON.stringify({ country: selValue }),
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: OnCountry_AjaxReturn,
            error: function (msg) {

            }
        });
    });
    /* END: ON SELECTED HANDLER (AJAX) */




    /* START ON CLICK*/
    /* as long i click on the machineID */


    /*come back */


    $("#divSearchByID").on("click", "#imgQuickView", onImgQuickViewClicked);
    $("body").on("click", '.miniIcon', onMiniIconClicked);


    function onImgQuickViewClicked(event) {
        var $this = $(this);
        var $txtMachineID = $('#txtMachineID');
        var mIdVal = $txtMachineID.val() || '';

        if (mIdVal == '') {
            alert('Sorry! Machine ID cannot be empty.');
            return;
        }

        $.ajax({
            type: "POST",
            url: "app_monitoring/content/LocStatsService.asmx/GetMachineDetails",
            data: JSON.stringify({ mID: mIdVal }),
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: OnMachineDetails_AjaxReturn,
            error: function (msg) {

            }
        });
    }

    function onMiniIconClicked() {
        var $this = $(this);
        var id = $this.attr('id');

        switch (id) {
            case 'imgBranchInfo':
                if ($mDetailsData != null) {

                    $tbody = $('#dlgMachineBranchInfo tbody');
                    $tbody.empty();
                    $tbody.append("<tr><td><b>Address : </b></td>" + "<td>" + $mDetailsData.ADDRESS1 + "<br/>" + $mDetailsData.ADDRESS2 + "</td></tr>");
                    $tbody.append("<tr><td><b>Phone : </b></td>" + "<td>" + $mDetailsData.CONTACTNUM + "</td></tr>");
                    $tbody.append("<tr><td><b>Person in<br/>charge : </b></td>" + "<td>" + $mDetailsData.CONTACTPERSON + "</td></tr>");


                    var x = $this.offset().left;
                    var y = $this.offset().top;
                    $('#dlgMachineBranchInfo').dialog('option', 'position', [x, y]);
                    showDialog('#dlgMachineBranchInfo', 'Branch Information - ' + $mDetailsData.BRANCH);

                }
                break;

        }
    }

    /* if machine id on urgency queue is clicked*/
    $(".tblLocation").on("click", "a[class$=mID]", function () {
        $this = $(this);

        var mIdVal = $this.html();
        event.preventDefault();
        addToNavList($this);

        $.ajax({
            type: "POST",
            url: "app_monitoring/content/LocStatsService.asmx/GetMachineDetails",
            data: JSON.stringify({ mID: mIdVal }),
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: OnMachineDetails_AjaxReturn,
            error: function (msg) {

            }
        });

    });

    var $mDetailsData;
    function OnMachineDetails_AjaxReturn(msg) {

        $mDetailsData = $.parseJSON(msg.d);
        var $v = $mDetailsData || null;

        if ($v.ERROR == null) {
            alert('The requested machine does not exist.');
            return;
        }

        $("#imgAlert").attr("src", "../Images/" + $v.ERROR + ".gif");
        $("#tdCurrStatus").html($v.ERROR + " [since " + $v.DATE + " " + $v.TIME + "]");
        $("#tdStatusCode").html($v.STATUSCODE);
        $("#tdDescription").html($v.ERRORDESC);
        $("#td1").html($v.MACHINEID);
        $("#td2").html($v.VENDOR);
        $("#td3").html($v.BRANCH);

        var linkage = "<b>" + $v.CUSTOMER + " > " + $v.COUNTRY + " > " + $v.STATE + " > " + $v.DISTRICT + " > " + $v.BRANCH +
                      "</b>  " +
                      "<img id=\"imgBranchInfo\" /img>";

        $('#machineLocation').html('Machine located at : ' + linkage);
        $('#imgBranchInfo').attr('src', '../Images/branchinfo.png');
        $('#imgBranchInfo').attr('title', 'click to view branch');
        $('#imgBranchInfo').addClass('miniIcon');


        showDialog('#dlgMachineDetails', 'Machine Detail - ' + $v.MACHINEID);

        // showDialog('#dlgMachineDetails', 'Machine Detail - ' + mIdVal);
    }




    /* END ON CLICK*/

    $('#ddlSite').trigger('change');



    /* timer 

    var numberOfCalls = 5;
    (function doStuff(){

        alert('hello');

        setTimeout(doStuff, 1000);
    }());
*/

    function initDialog(id)
    {
        var dlg1 = $(id);
        
        dlg1.dialog({
            autoOpen: false,
            modal: true,
            draggable: true,
            resizable: false,
            show: 'blind',
            hide: 'blind',
            width: 'auto',
            dialogClass: 'smallTitleBar blueOceanTopWindow'
        }).css('font-size', '.9em').css('font-family', 'Arial,Helvetica,sans-serif');
    }
    initDialog('#dlgAddNewMachineType');
    initDialog('#dlgAEVErrorCodes');

    $("#dlgAEVErrorCodes").on("click", "#lnkCreateNewError", function () {
        event.preventDefault();
       $('#divAddError').removeClass('hidden');
        
    });

    $('#dlgAEVErrorCodes').on("click", "#btnCancelEC", function () {
        event.preventDefault();
       $('#divAddError').addClass('hidden');
    });


    onErrorCodesSuccess = function (msg)
    {


        var $ddlError = $('<select>');
        $ddlError
        .append($('<option>', {value : 'OK'}).text('Online'))
        .append($('<option>', {value : 'ERROR'}).text('Error'))
        .append($('<option>', {value : 'WARNING'}).text('Warning'))
        .append($('<option>', {value : 'OFFLINE'}).text('Offline'))



        var data = $.parseJSON(msg.d);
        var resList = data.LIST;

         console.log(resList);
        var $newRow;
        var $newCol;

        $tblErrorCodesBody = $('#tblErrorCodes tbody');
        $('#tblErrorCodes tbody').empty();
        $.each(resList, function (key, val) {   
        
            $newCol = $('<td>');
            $newRow = $('<tr>');
            $edSpan = $('<div>');

            $btnSave = $('<input>').attr('type', 'button').attr('value', 'Save').addClass('smallButtonFont hidden');
            $btnCancel = $('<input>').attr('type', 'button').attr('value', 'Cancel').addClass('smallButtonFont hidden');
            $divBtn = $('<div>').append($btnSave).append($btnCancel);
            $newRow
            .append($newCol.clone().html($edSpan.clone().html(val.STATUSCODE).editable(
                function (value, settings){
                    return value;
                }
            ).click(editClick)))
            .append($newCol.clone().html($edSpan.clone().html(val.ERRORDESC).editable(
                function (value, settings){
                    return value;
                }
            ).click(editClick)))
            .append($newCol.clone().html($ddlError.clone().val(val.ERROR).change(editClick)))
            .append($divBtn);
            

            $tblErrorCodesBody.append($newRow);
        });
    };


    /* Find and trigger "edit" event on correct Jeditable instance. */
 
    function editClick()
    {
        $(this).parent().parent().find('input[type="button"].smallButtonFont').removeClass('hidden');
    }

    showDialogs = function (id, title, $nextTo, height)
    {
    alert("LocStats.js - showDialogs");
        var x = $nextTo.offset().left;
        var y = $nextTo.offset().top;
        var dlg1 = $(id);

        dlg1.dialog('option', 'height', height || 'auto');
        dlg1.dialog('option', 'position', [x, y]);
        dlg1.dialog('option', 'title', title);
        dlg1.dialog('open');
    };


    $('#tblErrorCodes').tablesorter();
    ///
});