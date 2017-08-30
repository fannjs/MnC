jQuery(document).ready(function ($) {
    
    var Interval = 30;
    
    /*START: ELEMENT ACCESSOR*/
    //var __ConfigFactory = window.ConfigFactory(config);
    function getSelectedSite() {
        return $('#select-site-btn').text().trim();
    }
    function getSelectedMachineType() {
        return $('#select-kiosk-btn').text().trim();
    }

    /*END: ELEMENT ACCESSOR*/

    function addToNavList($clicker) {

        var $parent = $('.navContainer');
        var $child = $('<div>').addClass('navChild');
        var $operator = $('<div>').addClass('navChild navDirection').html('>');
        var className = $clicker.attr('class');

        ($parent.find('.navChild').length > 0) ? $parent.append($operator) : '';
        ($parent.find('.' + className).length > 0) ? $parent.find('.' + className).parent().nextAll().andSelf().remove() : '';
        $location = $clicker.clone();
        $location.html($location.html().split('(')[0]);

        $location.appendTo($child);
        $parent.append($child);
    }

    function TranslateErrorType(errType, shape) {
        var clsErrType = 'redCol';

        if (errType == "ERROR") {
            clsErrType = 'red' + shape;
        }
        else if (errType == "ONLINE") {
            clsErrType = 'green' + shape;
        }
        else if (errType == "WARN" || errType == "WARNING") {
            clsErrType = 'yellow' + shape;
        }
        else if (errType == "OFFLINE") {
            clsErrType = 'grey' + shape;
        }
        return clsErrType;
    }
    /* 
    d:{
    NEXT_LINK:M_MACH_STATE_to_M_MACH_CITYDISTRICT,
    LIST:
    [
    {	
    COUNTRY:Selangor,
    ERROR:{
    ERROR:1,
    WARN:0,
    OK:3,
    OFFLINE:0
    }
    },
    {
    COUNTRY:Johor,
    ERROR:{
    ERROR:0,
    WARN:0,
    OK:3,
    OFFLINE:0
    }
    },
    {
    COUNTRY:Kuala Lumpur,
    ERROR:{
    ERROR:0,
    WARN:0,
    OK:4,
    OFFLINE:0}
    }
    ]
    }
    */

    


    LoadStatusOption();
    function LoadStatusOption() {
        $.ajax({
            type: "POST",
            url: "/views/dashboard.aspx/GetStatusOption",
            data: "{}",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (data) {
                var status = data.d.Status;
                var msg = data.d.Message;

                if (!status) {
                    alert(msg);
                    return false;
                }

                var StatusOption = data.d.Object;
                var Site = StatusOption.SiteList;
                var Machine = StatusOption.MachineList;

                var siteStr = '<li><a href="javascript:;">All Site</a></li>';
                var kioskStr = '<li><a href="javascript:;">All Kiosk</a></li>';

                for (var i = 0; i < Site.length; i++) {
                    siteStr = siteStr + '<li><a href="javascript:;">' + Site[i].Name + '</a></li>';
                }
                for (var j = 0; j < Machine.length; j++) {
                    kioskStr = kioskStr + '<li><a href="javascript:;">' + Machine[j].Type + '</a></li>';
                }

                $('#dllSite').html(siteStr);
                $('#dllMachType').html(kioskStr);
            },
            error: function (error) {
                alert("Error " + error.status + ". Unable to load status option.");
            }         
        });
    }

    var populateTraffics = function (traffics) {
        
        $.each(traffics.SUMMARY, function (errType, count) {
            updateTraffics(errType, count);
        });
    };

    function populateStates(aList, linkage) {
        var $errTr;
        var $errTd;
        var $errCountryLink;

        var $tblStatusBody = $("#divCountry .tblLocation > tbody");
        $tblStatusBody.empty();

        $tblStatusBody.append('<tr><td style="padding:5px"></td><td style="padding:0; border: 1px solid #E5412D; border-bottom-color: #000;" class="redCol"></td>'
                            + '<td class="yellowCol" style="padding:0; border: 1px solid #F0AD4E;border-bottom-color: #000;"></td>'
                            + '<td style="padding:0;border: 1px solid #5CB85C;border-bottom-color: #000;" class="greenCol"></td>'
                            + '<td class="greyCol" style="padding:0;border: 1px solid #999;border-bottom-color: #000;"></td></tr>');

        $.each(aList, function (i, v) {
            $errTr = $('<tr>');
            $errTd = $('<td>');
            $errCountryLink = $('<a>');
            $errCountryLink.addClass('clickable-link').addClass(linkage).attr('href', 'javascript:void(0)').html(v.COUNTRY);
            $errTd.append($errCountryLink);
            $errTr.append($errTd);

            $.each(v.ERROR, function (errType, errVal) {
                $errTd = $('<td>');
                var clsErrType = TranslateErrorType(errType, 'Col');
                
                if(errVal == "0"){
                    $errTd.html("").addClass('status-col'); //purposely put at back
                }else{
                    $errTd.html(errVal).addClass('status-col'); //purposely put at back
                }
                

                //mr saw's requirement on table 
                if (errVal > 0) {
                    $errTd.addClass(clsErrType);
                }
                else {
                    $errTd.removeClass(clsErrType);
                }

                if (errVal > 0) {
                    $errTd.css('cursor', 'pointer');
                    $errTd.addClass('clickable-link');
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
        var linkageCls = linkage.split("_LINK")[0]; //$this.attr('class');
        getLocationBreakdown(linkageCls, countryVal, getSelectedSite(), getSelectedMachineType(), OnState_AjaxReturn);

        liveOverallQuery(
        {
            'site': getSelectedSite(),
            'machType': getSelectedMachineType(),
            //'state': selectedState -- ROY removed
        });

        liveUrgencyQuery(getSelectedSite(), getSelectedMachineType(), '');

        /*
        if (errType == 'OK') {
        $tblLocationBody = $("#divUrgencyQueue .tblLocation > tbody");
        $tblLocationBody.empty();
        return;
        }
        */
    }

    /* START: ON AJAX SUCCESS-CALLBACK HANDLER */



    function OnCountry_AjaxReturn(msg) {
        /*abandoned for settimeout logics*/
    }


    function OnState_AjaxReturn(msg) {
        var data = $.parseJSON(msg.d);
        var $tblLocationBody = $("#divState .tblLocation > tbody");
        $(".portlet").not("#divCountry, #divUrgencyQueue").find(".tblLocation > tbody").empty();

        $('#divCountry').parent().nextAll().removeClass('anchor');

        var resList = data.LIST;
        var linkage = data.NEXT_LINK + pattern;

        $.each(resList, function (i, v) {
            $tblLocationBody.append("<tr><td><a href=javascript:void(0) class=\"clickable-link " + linkage + "\">" + v.LOCATION + "(" + v.COUNT + ")" + "</a></td></tr>");
        });

    }

    function OnDistrict_AjaxReturn(msg) {

        var data = $.parseJSON(msg.d);
        var $tblLocationBody = $("#divDistrict .tblLocation > tbody");

        $(".portlet").not("#divCountry, #divState, #divUrgencyQueue").find(".tblLocation > tbody").empty();


        var resList = data.LIST;
        var linkage = data.NEXT_LINK + pattern;

        $.each(resList, function (i, v) {
            $tblLocationBody.append("<tr><td><a href=javascript:void(0) class=\"clickable-link " + linkage + "\">" + v.LOCATION + "(" + v.COUNT + ")" + "</a></td></tr>");
        });

    }

    function OnBranch_AjaxReturn(msg) {
        var data = $.parseJSON(msg.d);
        var $tblLocationBody = $("#divBranch .tblLocation > tbody");

        $(".portlet").not("#divCountry, #divState, #divDistrict, #divUrgencyQueue").find(".tblLocation > tbody").empty();


        var resList = data.LIST;
        var linkage = data.NEXT_LINK + pattern;

        $.each(resList, function (i, v) {
            $tblLocationBody.append("<tr><td><a href=javascript:void(0) class=\"clickable-link " + linkage + " " + mID + "\">" + v.LOCATION + "</a></td></tr>");
        });

    }

    //temp comment for custom
    //function OnMachine_AjaxReturn(msg) {
    //    var data = $.parseJSON(msg.d);
    //    $tblLocationBody = $("#divMachine .tblLocation > tbody");

    //    $('#divBranch').nextAll().addClass('anchor');
    //    $(".anchor .tblLocation > tbody").empty();
    //    $(".anchor .tblLocation > tbody").parent().parent().addClass('empty');
    //    $('#divBranch').nextAll().removeClass('anchor');

    //    var resList = data.LIST;
    //    var linkage = data.NEXT_LINK + pattern;

    //    $.each(resList, function (i, v) {
    //        $tblLocationBody.append("<tr><td><a href=javascript:void(0) class=\"" + linkage + " " + mID + "\">" + v.LOCATION + "</a></td></tr>");
    //    });
    //    $tblLocationBody.parent().parent().removeClass('empty');

    //}


    function OnUrgentQueue_AjaxReturn(msg) {
        /*abandoned due to timer requirement.*/
    }
    /* END: ON AJAX SUCCESS-CALLBACK HANDLER */


    function showDialog(id, title) {
        $(id).dialog('option', 'title', title);
        $(id).dialog('open');
    }







    /* START: ON SELECTED HANDLER (AJAX) */

    var pattern = "_LINK";
    var mID = "mID";


    //District
    $("#divState .tblLocation").on("click", "a[class$=_LINK]", function () {
        var $this = $(this);
        var locationVal = $this.html().split('(')[0];
        var linkageCls = $this.attr('class').split("_LINK")[0].split(" ")[1];; //$this.attr('class');

        event.preventDefault();
        addToNavList($this);
        getLocationBreakdown(linkageCls, locationVal, getSelectedSite(), getSelectedMachineType(), OnDistrict_AjaxReturn);
    });

    //Branch
    $("#divDistrict .tblLocation").on("click", "a[class$=_LINK]", function () {
        var $this = $(this);
        var locationVal = $this.html().split('(')[0].split("-")[0]; //small hack
        var linkageCls = $this.attr('class').split("_LINK")[0].split(" ")[1];; //$this.attr('class');

        event.preventDefault();
        addToNavList($this);
        getLocationBreakdown(linkageCls, locationVal, getSelectedSite(), getSelectedMachineType(), OnBranch_AjaxReturn);
    });       

    //State
    $("#divCountry .tblLocation").on("click", "a[class$=_LINK]", function () {
        var $this = $(this);
        var selectedState = $this.html();

        var linkageCls = $this.attr('class').split("_LINK")[0].split(" ")[1]; //$this.attr('class');  //small hack
        globalErrType = ''; //clear
        event.preventDefault();
        addToNavList($this);

        //query town lists by state!
        liveOverallQuery(
		{
		    'site': getSelectedSite(),
		    'machType': getSelectedMachineType()		    
		});

        getLocationBreakdown(linkageCls, selectedState, getSelectedSite(), getSelectedMachineType(), OnState_AjaxReturn);

        liveUrgencyQuery(getSelectedSite(), getSelectedMachineType(), '');
    });

    $('#dllSite').off('click', 'li a').on('click', 'li a', function () {
        var selSite = $(this).text();

        $('#select-site-btn').html(selSite); 
        $("#divState, #divDistrict, #divBranch").find(".tblLocation > tbody").empty();

        liveOverallQuery(
		{
		    'site': selSite,
		    'machType': getSelectedMachineType()
		});

        liveUrgencyQuery(selSite, getSelectedMachineType(), '');
    });

    $('#dllMachType').off('click', 'li a').on('click', 'li a', function () {
        var machType = $(this).text();

        $('#select-kiosk-btn').html(machType);
        $("#divState, #divDistrict, #divBranch").find(".tblLocation > tbody").empty();

        liveOverallQuery(
		{
		    'site': getSelectedSite(),
		    'machType': machType
		});

        liveUrgencyQuery(getSelectedSite(), machType, '');

    });

    //Clicked refresh icon and refetch data 
    $('.refresh-btn').click(function () {
        
        liveOverallQuery(
		{
		    'site': getSelectedSite(),
		    'machType': getSelectedMachineType()
		});

        liveUrgencyQuery(getSelectedSite(), getSelectedMachineType(), '');
    });

    /* END: ON SELECTED HANDLER (AJAX) */




    var liveOverallQuery = function (options) {
        globalErrType = ''; //clear

        options = options || {};
        var site = options.site || 'All Sites';
        var machType = options.machType || 'All Kiosks';
        var state = options.state || 'All States';
        var stateCounter = Interval;

        $('#portletSelectedState').html('All States');

        if (site == "All Sites") {
            site = "ALL SITE";
        }
        if (machType == "All Kiosks") {
            machType = "ALL KIOSK";
        }
        if (state == "All States") {
            state = "ALL STATE";
        }  

        $.ajax({
            type: "POST",
            url: "/app_monitoring/content/LocStatsService.asmx/GetCountryStatus",
            data: JSON.stringify({ 'site': site, 'machType': machType, 'state': state }), //which is now equivalent to country
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (msg) {
               
                var data = $.parseJSON(msg.d);
                var aList = data.LIST;
                var linkage = data.NEXT_LINK + pattern;
                var traffics = data.TRAFFICS;
                var glbTtlMach = 0;
                populateTraffics(traffics);

                generatePieChart(traffics); //make use of the data to generate pie chart.
                genTopProblemsTodayChart(5);
                genMachStatusTodayChart_groupByMachErrType();

                if (state == 'ALL STATE') {
                    populateStates(aList, linkage);
                }

                $('#stateLastUpdate').html(moment().format('h:mm:ss A'));
                $('#stateCountDown').html(stateCounter);

                clearInterval(window.countdownId);
                window.countdownId = setInterval(function () { 
                    $('#stateCountDown').html(--stateCounter);

                    if(stateCounter === 0){
                        clearInterval(window.countdownId);
                        options.state = 'ALL STATE';
                        liveOverallQuery(options);
                    }                    
                }, 1000); 
            },
            error: function (msg) {
                alert("Error " + msg.status);
            }
        });
    };

    var updateTraffics = function (errType, count) {
        var clsBgColor = TranslateErrorType(errType, '-circle');
        var clsBorder = TranslateErrorType(errType, '-border');

        if (count == 0) {
            $('#overall-status-table').find('.t' + errType).html("").addClass('status-circle').addClass(clsBorder).removeClass(clsBgColor);

            if (errType == 'ERROR') {
                $('#overall-status-table').find('.t' + errType).removeClass('blink');

                $('#audio, #audio1').removeAttr('loop');
                $('#volume-div').hide();

            } else if (errType == 'WARN') {
                $('#overall-status-table').find('.t' + errType).removeClass('blink');
            }
        }
        else {
            
            $('#overall-status-table').find('.t' + errType).html(count).addClass('status-circle').addClass(clsBorder + " " + clsBgColor);

            if (errType == 'ERROR') {

                $('#overall-status-table').find('.t' + errType).addClass('blink');

                document.getElementById('audio').play();

                setTimeout(delaySound, 1000);

                $('#audio').attr({
                    'loop': 'loop'
                });

                if (audio.muted) {
                    audio.muted = true;
                }

                $('#volume-div').show();
            }
            else if (errType == 'WARN') {
               
                $('#overall-status-table').find('.t' + errType).addClass('blink');
            }
        }
    };

    function delaySound() {
        document.getElementById('audio1').play();

        $('#audio1').attr({
            'loop': 'loop'
        });

        if (audio1.muted) {
            audio1.muted = true;
        }
    }

    var liveUrgencyQuery = function (site, machType, state) {

        var urgencyCounter = Interval;

        $.ajax({
            type: "POST",
            contentType: "application/json; charset=utf-8",
            url: "/views/dashboard.aspx/GetUrgencyQueue",
            data: "{}",
            dataType: "json",
            success: function (data) {
                console.log(data);
                var status = data.d.Status;
                var msg = data.d.Message;

                if (!status) {
                    clearInterval(window.countdownIdUrgency); //IF hit error, stop the interval

                    alert(msg);
                    return false;
                }

                var KioskList = data.d.Object;

                var $tblLocationBody = $("#divUrgencyQueue .tblLocation > tbody");
                
                $tblLocationBody.empty();

                if (KioskList == 0) {
                    $tblLocationBody.html('<tr><td colspan="4">Nothing in the Kiosk Urgency Queue List</td></tr>');
                }
                else {
                    var str = "";

                    for (var i = 0; i < KioskList.length, Kiosk = KioskList[i]; i++) {
                        
                        var KioskId = Kiosk.KioskId;
                        var KioskType = Kiosk.KioskType;
                        var KioskStatus = Kiosk.Status;
                        var IsAlive = Kiosk.IsAlive;
                        
                        if (!IsAlive) {
                            var offlineDesc = "Connection Problem";
                            var currentTime = moment().format('HH:mm:ss');

                            str = str + "<tr><td class='redCol'><a class='clickable-error-code mID' href='javascript:void(0)'>" + KioskId + "</a></td>"
                                           + "<td>" + KioskType + "</td><td>" + offlineDesc + "</td><td>" + currentTime + "</td></tr>";
                        }

                        if (KioskStatus.toUpperCase() !== "ONLINE") {
                            var EventDataList = Kiosk.DataList;

                            for (var j = 0; j < EventDataList.length, EventData = EventDataList[j]; j++) {
                                var StatusDesc = EventData.StatusDescription;
                                var Downtime = EventData.Downtime;
                                var ErrorType = EventData.ErrorType;
                                var Class = "";

                                if(ErrorType.toUpperCase() === "ERROR"){
                                    Class = 'redCol';
                                }else if(ErrorType.toUpperCase() === "WARN"){
                                    Class = 'yellowCol';
                                }

                                str = str + "<tr><td class='"+Class+"'><a class='clickable-error-code mID' href='javascript:void(0)'>" + KioskId + "</a></td>"
                                           + "<td>" + KioskType + "</td><td>" + StatusDesc + "</td><td>" + Downtime + "</td></tr>";
                            }
                        } else {
                            continue;
                        }
                    }

                    $tblLocationBody.html(str);
                }

                $('#urgencyLastUpdate').html(moment().format('h:mm:ss A'));
                $('#urgencyCountDown').html(urgencyCounter);

                clearInterval(window.countdownIdUrgency);
                window.countdownIdUrgency = setInterval(function () { 
                    $('#urgencyCountDown').html(--urgencyCounter);

                    if(urgencyCounter === 0){
                        clearInterval(window.countdownIdUrgency);
                        liveUrgencyQuery(site, machType, ''); 
                    }
                }, 1000);
            },
            error: function (error) {
                alert("Error when trying to load Kiosk Urgency Queue List.");
            }
        });   
    };
    
    var getLocationBreakdown = function (linkageCls, location, site, machType, OnLocation_AjaxReturn) {

        $.ajax({
            type: "POST",
            url: "/app_monitoring/content/LocStatsService.asmx/GetLocationStatus",
            data: JSON.stringify({ 'linkage': linkageCls, 'location': location, 'site': site, 'machType': machType, 'errType': globalErrType }),
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (msg) {
                OnLocation_AjaxReturn(msg);
            },
            error: function (msg) {
                alert('error GetLocationStatus');
            }
        });
    };

    var countDown = function (id, counter) {

        $(arguments[0]).html(arguments[1]);
    };






    /* START ON CLICK*/
    /* as long i click on the machineID */


    /*come back */


    $("#divSearchByID").on("click", "#imgQuickView", onImgQuickViewClicked);
    $("body").off('click','.miniIcon',onMiniIconClicked).on("click", '.miniIcon', onMiniIconClicked);


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
            url: "/app_monitoring/content/LocStatsService.asmx/GetMachineDetails",
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
            case 'imgEditSensor':
                var errId = $(this).closest('tr').attr('id');
                showDialog('#dlgEditSensor', 'Sensor Positioning - ' + errId);
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
            url: "/app_monitoring/content/LocStatsService.asmx/GetMachineDetails",
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
        showMachineDetailDialog($v);

    };

    var recordPerPage = 15;  //Records to show per page

    function showMachineDetailDialog($v) {
        //reset
        $('a[href=#SOP-tab]').tab('show');
        $('.sensorField').reset();
        $("#CodeDescDiv > ul").empty();
        $('#SOP-info-div').empty();

        $('.sensorField').GetMachineError($v.STATUSCODE); //hack to access other files's function

        $(".tdMachineID").html($v.MACHINEID); //more than 1 
        $("#tdVendor").html($v.VENDOR);
        $(".tdBranch").html($v.BRANCH);
        $(".tdSite").html($v.CUSTOMER);
        $(".tdCountry").html($v.COUNTRY);
        $(".tdState").html($v.STATE);
        $(".tdDistrict").html($v.DISTRICT);
        $('#tdFullAddr').html($v.ADDRESS1 + ", " + $v.ADDRESS2);
        $('#tdPhone').html($v.CONTACTNUM);
        $('#tdContactPerson').html($v.CONTACTPERSON);

        if ($v.ALIVE.toUpperCase() == 'FALSE') {
            $("#machine-status-image .small-circle").attr('class', 'small-circle').addClass('red-circle');
            $('#SOP-info-div').append('<div class="info-div red-info-div"><span>Connection Error</span></div>');

            $("#tdCurrStatus").html("Offline" + "  [since " + $v.DATE + " " + $v.TIME + "]");
            $("#CodeDescDiv > ul").append("<li>Connection Error</li>");

            if ($v.ERROR.toUpperCase() == 'ERROR' || $v.ERROR.toUpperCase() == 'WARNING') {

                $.ajax({
                    type: "POST",
                    contentType: "application/json; charset=utf-8",
                    url: "/views/dashboard.aspx/getUrgencyQueueList",
                    data: "{machID: '" + $v.MACHINEID + "'}",
                    dataType: "json",
                    success: function (data) {
                    
                        for (var a = 0; a < data.d.length; a++) {
                            $('#CodeDescDiv > ul').append('<li>' + data.d[a].ErrCode + " " + data.d[a].ErrDesc + '</li>');

                            var colorClass = TranslateErrorType(data.d[a].ErrType, '-info-div');
                            $('#SOP-info-div').append('<div class="info-div ' + colorClass + '"><span>' + data.d[a].ErrDesc + '</span><p>' + data.d[a].SOP + '</p></div>');
                        }

                    },
                    error: function (error) {
                        alert("Error when trying to load machine status!");
                    }
                });
            }
        }
        else {
            $("#machine-status-image .small-circle").attr('class', 'small-circle').addClass(TranslateErrorType($v.ERROR, '-circle'));
            $("#tdCurrStatus").html($v.ERROR + "  [since " + $v.DATE + " " + $v.TIME + "]");

            if ($v.ERROR.toUpperCase() == 'ERROR' || $v.ERROR.toUpperCase() == 'WARNING') {
           
                $('#txtASOPTitle').html($v.ERRORDESC);
                $('#txtASOP').html($v.SOP);

                $.ajax({
                    type: "POST",
                    contentType: "application/json; charset=utf-8",
                    url: "/views/dashboard.aspx/getUrgencyQueueList",
                    data: "{machID: '" + $v.MACHINEID + "'}",
                    dataType: "json",
                    success: function (data) {
                    
                        for (var a = 0; a < data.d.length; a++) {
                            $('#CodeDescDiv > ul').append('<li>' + data.d[a].ErrCode + " " + data.d[a].ErrDesc + '</li>');

                            var colorClass = TranslateErrorType(data.d[a].ErrType, '-info-div');
                            $('#SOP-info-div').append('<div class="info-div ' + colorClass + '"><span>' + data.d[a].ErrDesc + '</span><p>' + data.d[a].SOP + '</p></div>');
                        }

                    },
                    error: function (error) {
                        alert("Error when trying to load machine status!");
                    }
                });
            }
            else {
                $("#CodeDescDiv > ul").append('<li>' + $v.STATUSCODE + " " + $v.ERRORDESC + '</li>');

                var colorClass = TranslateErrorType($v.ERROR, '-info-div');
                $('#SOP-info-div').append('<div class="info-div ' + colorClass + '"><span>' + $v.ERRORDESC + '</span><p>' + $v.SOP + '</p></div>');
            }
        }

        var linkage = "<b>" + $v.CUSTOMER + " > " + $v.COUNTRY + " > " + $v.STATE + " > " + $v.DISTRICT + " > " + $v.BRANCH;

        $('#machineLocation').html(linkage);
        $('#imgBranchInfo').attr('src', '../Images/branchinfo.png');
        $('#imgBranchInfo').attr('title', 'click to view branch');
        $('#imgBranchInfo').addClass('miniIcon');

        getLastTransactionDateTime($v.MACHINEID);

        var pageNumber = 1; //Hardcode to show first page when load

        getTransactionHistory($v.MACHINEID);

        /* Error Histroy load here */
        getMErrCodeHis($v.MACHINEID, pageNumber, recordPerPage);
        pagination();
        /* Error history finish loaded */

        $('#quickView-modal').modal('show');
    }

    function getLastTransactionDateTime(machineID) {
        $.ajax({
            type: "POST",
            contentType: "application/json; charset=utf-8",
            url: "/views/dashboard.aspx/GetTransactionHistory",
            data: "{machineID:'" + machineID + "'}",
            dataType: "json",
            beforeSend: function(){
                $('#tblTransactionHistory tbody').html('<tr><td colspan="3">Loading...</td></tr>');
            },
            success: function (data) {

                var status = data.d.Status;
                var msg = data.d.Message;
                
                if (!status) {
                    alert(msg);
                    return false;
                }
                
                var TransList = data.d.Object;
                var record = "";

                if(TransList.length === 0){
                    record = '<tr><td colspan="3">No transaction</td></tr>';
                }
                else{
                    for(var i = 0; i < TransList.length, Trans = TransList[i]; i++){        
                    var date = moment(Trans.Date).format("DD/MM/YYYY");
                    var totalTrans = Trans.TransactionCount;
                    var totalCheque = Trans.ChequeCount;

                    record = record + "<tr><td>" + date + "</td><td>" + totalTrans + "</td><td>" + totalCheque + "</td></tr>";
                }
                }

                $('#tblTransactionHistory tbody').html(record);
            },
            error: function (error) {
                alert("Failed. " + error.status + ". Unable to retrieve Transactiono History. Please try again.");
            }
        });
    }

    function getTransactionHistory(machineID) {
        $.ajax({
            type: "POST",
            contentType: "application/json; charset=utf-8",
            url: "/views/dashboard.aspx/GetLastTransactionDT",
            data: "{machineID:'" + machineID + "'}",
            dataType: "json",
            beforeSend: function () {
                $('#tdLastTrans').html("Loading...");
            },
            success: function (data) {

                var status = data.d.Status;
                var msg = data.d.Message;
                
                if (!status) {
                    alert(msg);
                    return false;
                }
                
                var LastTransactionDT = data.d.Object;

                if (LastTransactionDT === "") {
                    LastTransactionDT = "-";
                }
                $('#tdLastTrans').html(LastTransactionDT);
            },
            error: function (error) {
                alert("Failed. " + error.status + ". Unable to retrieve Transactiono History. Please try again.");
            }
        });
    }

    /* Get status history with pagination */
    function getMErrCodeHis(machineID, pageNumber, recordPerPage) {

        $.ajax({
            type: "POST",
            contentType: "application/json; charset=utf-8",
            url: "/views/dashboard.aspx/GetStatusHistory",
            data: "{machineID:'" + machineID + "', pageNumber: '" + pageNumber + "', recordPerPage: '" + recordPerPage + "'}",
            dataType: "json",
            success: function (data) {

                var status = data.d.Status;
                var msg = data.d.Message;
                
                if (!status) {
                    alert(msg);
                    return false;
                }

                var Obj = data.d.Object;
                var currentPage = parseInt(pageNumber);
                var totalRecord = Obj.Count;
                totalRecord = totalRecord / recordPerPage;
                var totalPages = Math.ceil(totalRecord);

                var pages = "";

                if (totalPages > 1) {
                    pages = pages + '<li id="first-page"><a data-paging="1">&laquo;</a></li>';

                    if (totalPages <= 10) {
                        for (var i = 1; i <= totalPages; i++) {
                            pages = pages + '<li class="page-number"><a data-paging="' + i + '">' + i + '</a></li>';
                        }
                    }
                    else {
                        if (currentPage - 5 <= 1) {
                            for (var j = 1; j <= 10; j++) {
                                pages = pages + '<li class="page-number"><a data-paging="' + j + '">' + j + '</a></li>';
                            }
                        }
                        else if (currentPage + 5 <= totalPages) {
                            for (var k = currentPage - 5; k <= (currentPage + 5) - 1; k++) {
                                pages = pages + '<li class="page-number"><a data-paging="' + k + '">' + k + '</a></li>';
                            }
                        }
                        else {
                            for (var a = 1 + (totalPages - 10) ; a <= totalPages; a++) {
                                pages = pages + '<li class="page-number"><a data-paging="' + a + '">' + a + '</a></li>';
                            }
                        }
                    }
                    pages = pages + '<li id="last-page"><a data-paging="' + totalPages + '">&raquo;</a></li>';
                }

                var StatusList = Obj.List;
                var statusHistory = "";

                for (var index = 0; index < StatusList.length; index++) {                    
                    statusHistory = statusHistory + "<tr><td>" + StatusList[index].CODE + "</td><td>" + StatusList[index].DESCRIPTION + "</td><td>" + StatusList[index].TYPE + "</td><td>" + StatusList[index].DATE_TIME + "</td></tr>";
                }

                $('#pagination').html(pages);
                $("#tblStatusHistory tbody").html(statusHistory);

                $('#pagination .page-number').each(function () {
                    if ($(this).find('a').attr('data-paging') == currentPage) {
                        $(this).addClass('active');
                    }
                });
            },
            error: function (error) {
                alert("Failed to retrieve History!");
            }
        });
    }

    function pagination() {
        //Status History pagination
        $('#quickView-modal').off('click', '#pagination li').on('click', '#pagination li', function () {

            var pageNo = $(this).find('a').attr('data-paging'); //Get the page number that user click on
            var currentPage = $('#pagination > .active > a').attr('data-paging'); //Get the current showing page number

            if (pageNo == currentPage) {
                return false;
            }
            else {
                var machineID = $(".tdMachineID").html();
                getMErrCodeHis(machineID, pageNo, recordPerPage);
            }
        });
    }


    $('#dllMachType li:first-child a').trigger('click');

    function initDialog(id) {
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

    initDialog('#dlgAEVErrorCodes');
    initDialog('#dlgEditSensor');


    $("#dlgAEVErrorCodes").on("click", "#lnkCreateNewError", function () {
        event.preventDefault();
        $('#divAddError').removeClass('hidden');

    });

    $('#dlgAEVErrorCodes').on("click", "#btnCancelEC", function () {
        event.preventDefault();
        $('#divAddError').addClass('hidden');
    });


    $('#dlgAEVErrorCodes').on("click", "#btnSaveEC", function () {
        event.preventDefault();
        var mTypeModel = $('#dlgAEVErrorCodes').dialog('option', 'title').split('-')[1].trim();
        var mType = mTypeModel.split(':')[0];
        var mModel = mTypeModel.split(':')[1];

        var machineerror = {
            'M_MACH_TYPE': mType,
            'M_MACH_MODEL': mModel,
            'M_CODE': $('#txtStatusCode').val(),
            'M_ERROR_DESCRIPTION': $('#txtError').val(),
            'M_ERRORTYPE': $('#ddlErrorDesc').val()
        }

        fnWebAPI({
            action: 'POST',
            controller: 'machineerror',
            obj: machineerror,
            actionName: 'Add', //add, update,delete
            onSuccess: function () {
                getErrorCodesByMachineType(mType, mModel);
            }

        });

    });


    //we have this repeat function on MachTemplate.js
    function getErrorCodesByMachineType(mType) {

        $.ajax({
            type: "POST",
            url: "/app_monitoring/content/MachTemplate.asmx/getErrorCodesByMachineType",
            contentType: "application/json; charset=utf-8",
            data: JSON.stringify({ 'mType': mType }),
            dataType: "json",
            success: function (msg) {
                onErrorCodesSuccess(msg);
            },
            error: function (msg) {
                alert('woops');
            }
        });
    }



    onErrorCodesSuccess = function (msg) {
        var $ddlError = $('<select>');
        $ddlError
		.append($('<option>', { value: 'OK' }).text('Online'))
		.append($('<option>', { value: 'ERROR' }).text('Error'))
		.append($('<option>', { value: 'WARN' }).text('Warning'))
		.append($('<option>', { value: 'OFFLINE' }).text('Offline'))


        var data = $.parseJSON(msg.d);
        var resList = data.LIST;
        var $newRow;



        $tblErrorCodesBody = $('#tblErrorCodes tbody');

        $('#tblErrorCodes tbody').empty();

        $.each(resList, function (key, val) {
            $newRow = $('<tr>').attr('id', val.STATUSCODE);

            $newRow
			.append(fnGenTdContent('text', val.STATUSCODE, 'M_CODE'))
			.append(fnGenTdContent('text', val.ERRORDESC, 'M_ERROR_DESCRIPTION'))
			.append(fnGenTdContent('select', val.ERROR, 'M_ERRORTYPE', { data: { 'OK': 'OK', 'ERROR': 'ERROR', 'WARNING': 'WARNING', 'OFFLINE': 'OFFLINE' } }))
			.append('<td><img id="imgEditSensor" img="" src="../Images/editsensor.png" title="click to edit sensor" class="miniIcon"></td>')
			.append(fnGenDivButtons('edit'))
			.append(fnGenDivButtons('normal'));

            $tblErrorCodesBody.append($newRow);
        });
    };



    showDialogs = function (id, title, $nextTo, height) {
        var x = $nextTo.offset().left;
        var y = $nextTo.offset().top;
        var dlg1 = $(id);

        dlg1.dialog('option', 'height', height || 'auto');
        dlg1.dialog('option', 'position', [x, y]);
        dlg1.dialog('option', 'title', title);
        dlg1.dialog('open');
    };



    $('#quick-view').click(function () {
        var mIdVal = $('#inputQuickView').val();
        if (mIdVal == "") {
            alert("Machine ID cannot be empty!");
        }
        else {
            $.ajax({
                type: "POST",
                url: "/app_monitoring/content/LocStatsService.asmx/GetMachineDetails",
                data: JSON.stringify({ mID: mIdVal }),
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: OnMachineDetails_AjaxReturn,
                error: function (msg) {

                }
            });
        }
    });

    // generate pie chart base on traffic and auto refresh.
    var generatePieChart = function (traffics) {       
        var pieData = [];
        var ttlMach = 0;

        $.each(traffics.SUMMARY, function (errType, count) {
            ttlMach += count;
        });
        glbTtlMach = ttlMach;
        $.each(traffics.SUMMARY, function (errType, count) {
            var option = {};

            if (errType == "ERROR") {
                option["value"] = count;
                option["color"] = "#E5412D";
                option["highlight"] = "#E5412D";
                option["label"] = "Error";
                option["percent"] = ((count / ttlMach) * 100).toFixed(2);
                pieData.push(option);
            }
            else if (errType == "ONLINE") {
                option["value"] = count;
                option["color"] = "#5CB85C";
                option["highlight"] = "#5CB85C";
                option["label"] = "Normal";
                option["percent"] = ((count / ttlMach) * 100).toFixed(2);
                pieData.push(option);
            }
            else if (errType == "WARN" || errType == "WARNING") {
                option["value"] = count;
                option["color"] = "#F0AD4E";
                option["highlight"] = "#F0AD4E";
                option["label"] = "Warning";
                option["percent"] = ((count / ttlMach) * 100).toFixed(2);
                pieData.push(option);
            }
            else if (errType == "OFFLINE") {
                option["value"] = count;
                option["color"] = "#999999";
                option["highlight"] = "#999999";
                option["label"] = "Offline";
                option["percent"] = ((count / ttlMach) * 100).toFixed(2);
                pieData.push(option);
            }
        });

        var ctx = document.getElementById("pieChart").getContext("2d");
        new Chart(ctx).Pie(pieData, { segmentShowStroke: true, animateRotate :false });
        legend(document.getElementById("pieLegend"), pieData);
    };

    // to generate BarChart for topProblemsTodayChart
    function genTopProblemsTodayChart(numOfTopProblems) {
        if (numOfTopProblems != "") {
            numOfTopProblems = numOfTopProblems;
        } else {
            numOfTopProblems = 0;
        }
        $.ajax({
            type: "POST",
            contentType: "application/json; charset=utf-8",
            url: "/views/dashboard.aspx/genTopProblemsTodayChart",
            data: "{numOfTopProblems: " + numOfTopProblems + "}",
            dataType: "json",
            success: function (data) {

                var status = data.d.Status;
                var msg = data.d.Message;

                if (!status) {
                    alert(msg);
                    return false;
                }

                var mStatusList = data.d.Object;
                //var chartDatas = [];
                //var barChartLocData = {
                //    labels: ["ErrCode2", "ErrCode2", "ErrCode3", "ErrCode4", "ErrCode5"],
                //    datasets: [{ fillColor: "lightblue", strokeColor: "blue", data: [15, 20, 35, 20, 35] }]
                //};
                var mCodeLabel = [];
                var mNumProData = [];
                for (var i = 0; i < mStatusList.length; i++) {
                    mCodeLabel.push(mStatusList[i].mCode);
                    mNumProData.push(mStatusList[i].mNumProblems);                    
                }

                var datasetsData = [];
                var colorOpt = {};
                //colorOpt["fillColor"] = "#97bbcd";
                //colorOpt["strokeColor"] = "#97bbce";

                colorOpt["fillColor"] = "rgba(151,187,205,0.5)";
                colorOpt["strokeColor"] = "rgba(151,187,205,0.8)";
                //colorOpt["highlightFill"] = "rgba(151,187,205,0.75)";
                //colorOpt["highlightStroke"] = "rgba(151,187,205,1)";
                colorOpt["data"] = mNumProData;
                datasetsData.push(colorOpt);

                var barChartData = {
                    labels: mCodeLabel,
                    datasets: datasetsData
                };
                //console.log(barChartData);
                var mybarChartLoc = new Chart(document.getElementById("topProblemsTodayBarChart").getContext("2d")).Bar(barChartData);

            },
            error: function (error) {
                alert("Error when trying to load machine status!");
            }
        });
    }

    //// to generate lineChart for machine status today
    //function genMachStatusTodayChart() {
    //    $.ajax({
    //        type: "POST",
    //        contentType: "application/json; charset=utf-8",
    //        url: "/views/dashboard.aspx/genMachStatusTodayChart",
    //        data: "{}",
    //        dataType: "json",
    //        success: function (data) {
    //            var status = data.d.Status;
    //            var msg = data.d.Message;

    //            if (!status) {
    //                alert(msg);
    //                return false;
    //            }

    //            var mStatusList = data.d.Object;

    //            var mCodeLabel = ['0-4', '4-8', '8-12','12-16','16-20','20-0'];
    //            var datasetsData = [];
    //            for (var i = 0; i < mStatusList.length; i++) {
    //                var mNumProData = [];
    //                var colorOpt = {};
    //                //alert("mErrType = " + mStatusList[i].mErrType + ", mNumProblems = " + mStatusList[i].mNumProblems_0to4 +", "+ mStatusList[i].mNumProblems_0to4)
    //                var codeType = mStatusList[i].mErrType;
    //                if (codeType == "ERROR") {

    //                    colorOpt["label"] = "Error";
    //                    colorOpt["fillColor"] = "rgba(220,220,220,0.2)";
    //                    colorOpt["strokeColor"] = "red";
    //                    colorOpt["pointColor"] = "red";
    //                    colorOpt["pointStrokeColor"] = "fff";
    //                    colorOpt["pointHighlightFill"] = "fff";
    //                    colorOpt["pointHighlightStroke"] = "red";
    //                }
    //                if (codeType == "WARN") {
    //                    colorOpt["label"] = "Warning";
    //                    colorOpt["fillColor"] = "rgba(220,220,220,0.2)";
    //                    colorOpt["strokeColor"] = "yellow";
    //                    colorOpt["pointColor"] = "yellow";
    //                    colorOpt["pointStrokeColor"] = "fff";
    //                    colorOpt["pointHighlightFill"] = "fff";
    //                    colorOpt["pointHighlightStroke"] = "yellow";
    //                }
    //                if (codeType == "OFFLINE") {
    //                    colorOpt["label"] = "Offline";
    //                    colorOpt["fillColor"] = "rgba(220,220,220,0.2)";
    //                    colorOpt["strokeColor"] = "grey";
    //                    colorOpt["pointColor"] = "grey";
    //                    colorOpt["pointStrokeColor"] = "fff";
    //                    colorOpt["pointHighlightFill"] = "fff";
    //                    colorOpt["pointHighlightStroke"] = "rgba(220,220,220,1)";
    //                }
    //                if (codeType == "OK") {
    //                    colorOpt["label"] = "Normal";
    //                    colorOpt["fillColor"] = "rgba(220,220,220,0.2)";
    //                    colorOpt["strokeColor"] = "green";
    //                    colorOpt["pointColor"] = "green";
    //                    colorOpt["pointStrokeColor"] = "fff";
    //                    colorOpt["pointHighlightFill"] = "fff";
    //                    colorOpt["pointHighlightStroke"] = "rgba(220,220,220,1)";
    //                }

    //                mNumProData.push(mStatusList[i].mNumProblems_0to4);
    //                mNumProData.push(mStatusList[i].mNumProblems_4to8);
    //                mNumProData.push(mStatusList[i].mNumProblems_8to12);
    //                mNumProData.push(mStatusList[i].mNumProblems_12to16);
    //                mNumProData.push(mStatusList[i].mNumProblems_16to20);
    //                mNumProData.push(mStatusList[i].mNumProblems_20to24);
    //                colorOpt["data"] = mNumProData;

    //                datasetsData.push(colorOpt);
    //            }

    //            var lineChartData = {
    //                labels: mCodeLabel,
    //                datasets: datasetsData
    //            };
    //            console.log(lineChartData);
    //            var myLineChartLoc = new Chart(document.getElementById("machStatusLineChart").getContext("2d")).Line(lineChartData);

    //        },
    //        error: function (error) {
    //            alert("Error when trying to load machine status!");
    //        }
    //    });
    //}

    // not yet use 
    function genMachStatusTodayChart_groupByMachErrType_4hrsGroup() {
        $.ajax({
            type: "POST",
            contentType: "application/json; charset=utf-8",
            url: "/views/dashboard.aspx/genMachStatusTodayChart_groupByMachErrType",
            data: "{}",
            dataType: "json",
            success: function (data) {
                //alert("glbTtlMach = " + glbTtlMach);
                var status = data.d.Status;
                var msg = data.d.Message;

                if (!status) {
                    alert(msg);
                    return false;
                }

                var mStatusList = data.d.Object;
                var ttlErr = 0;
                var ttlWarn = 0;
                var ttlOffline = 0;

                var ttlErr0to4 = 0;
                var ttlErr4to8 = 0;
                var ttlErr8to12 = 0;
                var ttlErr12to16 = 0;
                var ttlErr16to20 = 0;
                var ttlErr20to0 = 0;

                var ttlWarn0to4 = 0;
                var ttlWarn4to8 = 0;
                var ttlWarn8to12 = 0;
                var ttlWarn12to16 = 0;
                var ttlWarn16to20 = 0;
                var ttlWarn20to0 = 0;

                var ttlOffline0to4 = 0;
                var ttlOffline4to8 = 0;
                var ttlOffline8to12 = 0;
                var ttlOffline12to16 = 0;
                var ttlOffline16to20 = 0;
                var ttlOffline20to0 = 0;

                var ttlNormal0to4 = 0;
                var ttlNormal4to8 = 0;
                var ttlNormal8to12 = 0;
                var ttlNormal12to16 = 0;
                var ttlNormal16to20 = 0;
                var ttlNormal20to0 = 0;

                for (var i = 0; i < mStatusList.length; i++) {
                    var codeType = mStatusList[i].mErrType;

                    if (codeType == "ERROR") {
                        ttlErr = ttlErr + 1;
                        if (mStatusList[i].mNumProblems_0to4 > 0) {
                            ttlErr0to4 = ttlErr0to4 + 1;
                        }
                        if (mStatusList[i].mNumProblems_4to8 > 0) {
                            ttlErr4to8 = ttlErr4to8 + 1;
                        }
                        if (mStatusList[i].mNumProblems_8to12 > 0) {
                            ttlErr8to12 = ttlErr8to12 + 1;
                        }
                        if (mStatusList[i].mNumProblems_12to16 > 0) {
                            ttlErr12to16 = ttlErr12to16 + 1;
                        }
                        if (mStatusList[i].mNumProblems_16to20 > 0) {
                            ttlErr16to20 = ttlErr16to20 + 1;
                        }
                        if (mStatusList[i].mNumProblems_20to0 > 0) {
                            ttlErr20to0 = ttlErr20to0 + 1;
                        }
                    }
                    if (codeType == "WARN") {
                        ttlWarn += 1;
                        if (mStatusList[i].mNumProblems_0to4 > 0) {
                            ttlWarn0to4 = ttlWarn0to4 + 1;
                        }
                        if (mStatusList[i].mNumProblems_4to8 > 0) {
                            ttlWarn4to8 = ttlWarn4to8 + 1;
                        }
                        if (mStatusList[i].mNumProblems_8to12 > 0) {
                            ttlWarn8to12 = ttlWarn8to12 + 1;
                        }
                        if (mStatusList[i].mNumProblems_12to16 > 0) {
                            ttlWarn12to16 = ttlWarn12to16 + 1;
                        }
                        if (mStatusList[i].mNumProblems_16to20 > 0) {
                            ttlWarn16to20 = ttlWarn16to20 + 1;
                        }
                        if (mStatusList[i].mNumProblems_20to0 > 0) {
                            ttlWarn20to0 = ttlWarn20to0 + 1;
                        }
                    }
                    if (codeType == "OFFLINE") {
                        ttlOffline += 1;
                        if (mStatusList[i].mNumProblems_0to4 > 0) {
                            ttlOffline0to4 = ttlOffline0to4 + 1;
                        }
                        if (mStatusList[i].mNumProblems_4to8 > 0) {
                            ttlOffline4to8 = ttlOffline4to8 + 1;
                        }
                        if (mStatusList[i].mNumProblems_8to12 > 0) {
                            ttlOffline8to12 = ttlOffline8to12 + 1;
                        }
                        if (mStatusList[i].mNumProblems_12to16 > 0) {
                            ttlOffline12to16 = ttlOffline12to16 + 1;
                        }
                        if (mStatusList[i].mNumProblems_16to20 > 0) {
                            ttlOffline16to20 = ttlOffline16to20 + 1;
                        }
                        if (mStatusList[i].mNumProblems_20to0 > 0) {
                            ttlOffline20to0 = ttlOffline20to0 + 1;
                        }
                    }
                }

                if (ttlErr0to4 > 0) {
                    ttlErr0to4 = ((ttlErr0to4 / glbTtlMach) * 100).toFixed(2); // numErrInMach / ttlMach * 100%
                }
                if (ttlErr4to8 > 0) {
                    ttlErr4to8 = ((ttlErr4to8 / glbTtlMach) * 100).toFixed(2);
                }
                if (ttlErr8to12 > 0) {
                    ttlErr8to12 = ((ttlErr8to12 / glbTtlMach) * 100).toFixed(2);
                }
                if (ttlErr12to16 > 0) {
                    ttlErr12to16 = ((ttlErr12to16 / glbTtlMach) * 100).toFixed(2);
                }
                if (ttlErr16to20 > 0) {
                    ttlErr16to20 = ((ttlErr16to20 / glbTtlMach) * 100).toFixed(2);
                }
                if (ttlErr20to0 > 0) {
                    ttlErr20to0 = ((ttlErr20to0 / glbTtlMach) * 100).toFixed(2);
                }

                if (ttlWarn0to4 > 0) {
                    ttlWarn0to4 = ((ttlWarn0to4 / glbTtlMach) * 100).toFixed(2);
                }
                if (ttlWarn4to8 > 0) {
                    ttlWarn4to8 = ((ttlWarn4to8 / glbTtlMach) * 100).toFixed(2);
                }
                if (ttlWarn8to12 > 0) {
                    ttlWarn8to12 = ((ttlWarn8to12 / glbTtlMach) * 100).toFixed(2);
                }
                if (ttlWarn12to16 > 0) {
                    ttlWarn12to16 = ((ttlWarn12to16 / glbTtlMach) * 100).toFixed(2);
                }
                if (ttlWarn16to20 > 0) {
                    ttlWarn16to20 = ((ttlWarn16to20 / glbTtlMach) * 100).toFixed(2);
                }
                if (ttlWarn20to0 > 0) {
                    ttlWarn20to0 = ((ttlWarn20to0 / glbTtlMach) * 100).toFixed(2);
                }

                if (ttlOffline0to4 > 0) {
                    ttlOffline0to4 = ((ttlOffline0to4 / glbTtlMach) * 100).toFixed(2);
                }
                if (ttlOffline4to8 > 0) {
                    ttlOffline4to8 = ((ttlOffline4to8 / glbTtlMach) * 100).toFixed(2);
                }
                if (ttlOffline8to12 > 0) {
                    ttlOffline8to12 = ((ttlOffline8to12 / glbTtlMach) * 100).toFixed(2);
                }
                if (ttlOffline12to16 > 0) {
                    ttlOffline12to16 = ((ttlOffline12to16 / glbTtlMach) * 100).toFixed(2);
                }
                if (ttlOffline16to20 > 0) {
                    ttlOffline16to20 = ((ttlOffline16to20 / glbTtlMach) * 100).toFixed(2);
                }
                if (ttlOffline20to0 > 0) {
                    ttlOffline20to0 = ((ttlOffline20to0 / glbTtlMach) * 100).toFixed(2);
                }

                var ttlNormal0to4 = (100 - ttlErr0to4 - ttlWarn0to4 - ttlOffline0to4).toFixed(2);
                var ttlNormal4to8 = (100 - ttlErr4to8 - ttlWarn4to8 - ttlOffline4to8).toFixed(2);
                var ttlNormal8to12 = (100 - ttlErr8to12 - ttlWarn8to12 - ttlOffline8to12).toFixed(2);
                var ttlNormal12to16 = (100 - ttlErr12to16 - ttlWarn12to16 - ttlOffline12to16).toFixed(2);
                var ttlNormal16to20 = (100 - ttlErr16to20 - ttlWarn16to20 - ttlOffline16to20).toFixed(2);
                var ttlNormal20to0 = (100 - ttlErr20to0 - ttlWarn20to0 - ttlOffline20to0).toFixed(2);

                ////////////////////////
                // get current time to show.

                var lineChartData = {
                    labels: ["0-4", "4-8", "8-12", '12-16', '16-20', '20-0'],
                    datasets: [
                        {
                            label: "Error",
                            fillColor: "rgba(220,220,220,0.2)",
                            strokeColor: "red",
                            pointColor: "red",
                            pointStrokeColor: "#fff",
                            pointHighlightFill: "#fff",
                            pointHighlightStroke: "red",
                            data: [ttlErr0to4, ttlErr4to8, ttlErr8to12, ttlErr12to16, ttlErr16to20, ttlErr20to0]
                        },
                        {
                            label: "Warning",
                            fillColor: "rgba(151,187,205,0.2)",
                            strokeColor: "#F0AD4E",
                            pointColor: "#F0AD4E",
                            pointStrokeColor: "#fff",
                            pointHighlightFill: "#fff",
                            pointHighlightStroke: "#F0AD4E",
                            data: [ttlWarn0to4, ttlWarn4to8, ttlWarn8to12, ttlWarn12to16, ttlWarn16to20, ttlWarn20to0]
                        },
                        {
                            label: "Normal",
                            fillColor: "rgba(151,187,205,0.2)",
                            strokeColor: "#5CB85C",
                            pointColor: "#5CB85C",
                            pointStrokeColor: "#fff",
                            pointHighlightFill: "#fff",
                            pointHighlightStroke: "#5CB85C",
                            data: [ttlNormal0to4, ttlNormal4to8, ttlNormal8to12, ttlNormal12to16, ttlNormal16to20, ttlNormal20to0]
                        },
                        {
                            label: "Offline",
                            fillColor: "rgba(151,187,205,0.2)",
                            strokeColor: "#999",
                            pointColor: "#999",
                            pointStrokeColor: "#fff",
                            pointHighlightFill: "#fff",
                            pointHighlightStroke: "#999",
                            data: [ttlOffline0to4, ttlOffline4to8, ttlOffline8to12, ttlOffline12to16, ttlOffline16to20, ttlOffline20to0]
                        }
                    ]
                };

                //console.log(lineChartData);
                var myLineChartLoc = new Chart(document.getElementById("machStatusLineChart_machid").getContext("2d")).Line(lineChartData);

                legend_lineChart(document.getElementById("lineLegend_machid"), lineChartData);
                ///////////////////////////

            },
            error: function (error) {
                alert("Error when trying to generate machine status chart!");
            }
        });
    }


    function genMachStatusTodayChart_groupByMachErrType() {
        $.ajax({
            type: "POST",
            contentType: "application/json; charset=utf-8",
            url: "/views/dashboard.aspx/genMachStatusTodayChart_groupByMachErrType",
            data: "{}",
            dataType: "json",
            success: function (data) {
                //alert("glbTtlMach = " + glbTtlMach);
                var status = data.d.Status;
                var msg = data.d.Message;

                if (!status) {
                    alert(msg);
                    return false;
                }

                var mStatusList = data.d.Object;
                var ttlErr = 0;
                var ttlWarn = 0;
                var ttlOffline = 0;

                var ttlErr0to4 = 0;
                var ttlErr4to8 = 0;
                var ttlErr8to12 = 0;
                var ttlErr12to16 = 0;
                var ttlErr16to20 = 0;
                var ttlErr20to0 = 0;

                var ttlWarn0to4 = 0;
                var ttlWarn4to8 = 0;
                var ttlWarn8to12 = 0;
                var ttlWarn12to16 = 0;
                var ttlWarn16to20 = 0;
                var ttlWarn20to0 = 0;

                var ttlOffline0to4 = 0;
                var ttlOffline4to8 = 0;
                var ttlOffline8to12 = 0;
                var ttlOffline12to16 = 0;
                var ttlOffline16to20 = 0;
                var ttlOffline20to0 = 0;

                var ttlNormal0to4 = 0;
                var ttlNormal4to8 = 0;
                var ttlNormal8to12 = 0;
                var ttlNormal12to16 = 0;
                var ttlNormal16to20 = 0;
                var ttlNormal20to0 = 0;

                // count total of codeType. count total of err machine in 4 hours 
                for (var i = 0; i < mStatusList.length; i++) {
                    var codeType = mStatusList[i].mErrType; 
                    if (codeType == "ERROR") {
                        //alert("err 0to4 = " + mStatusList[i].mNumProblems_0to4 + ", 4to8 = " + mStatusList[i].mNumProblems_4to8 + ", 8to12 = " + mStatusList[i].mNumProblems_8to12
                        //        + ", 12to16 = " + mStatusList[i].mNumProblems_12to16 + ", 16to20 = " + mStatusList[i].mNumProblems_16to20 + ", 20to0 = " + mStatusList[i].mNumProblems_20to0);

                        ttlErr = ttlErr + 1;
                        if (mStatusList[i].mNumProblems_0to4 > 0) {
                            ttlErr0to4 = ttlErr0to4 + 1;
                        }
                        if (mStatusList[i].mNumProblems_4to8 > 0) {
                            ttlErr4to8 = ttlErr4to8 + 1;
                        }
                        if (mStatusList[i].mNumProblems_8to12 > 0) {
                            ttlErr8to12 = ttlErr8to12 + 1;
                        }
                        if (mStatusList[i].mNumProblems_12to16 > 0) {
                            ttlErr12to16 = ttlErr12to16 + 1;
                        }
                        if (mStatusList[i].mNumProblems_16to20 > 0) {
                            ttlErr16to20 = ttlErr16to20 + 1;
                        }
                        if (mStatusList[i].mNumProblems_20to0 > 0) {
                            ttlErr20to0 = ttlErr20to0 + 1;
                        }
                    }
                    if (codeType == "WARN") {
                        ttlWarn = ttlWarn + 1;
                        //alert("0to4 = " + mStatusList[i].mNumProblems_0to4 + ", 4to8 = " + mStatusList[i].mNumProblems_4to8 + ", 8to12 = " + mStatusList[i].mNumProblems_8to12
                        //    + ", 12to16 = " + mStatusList[i].mNumProblems_12to16 + ", 16to20 = " + mStatusList[i].mNumProblems_16to20 + ", 20to0 = " + mStatusList[i].mNumProblems_20to0);
                        if (mStatusList[i].mNumProblems_0to4 > 0) {
                            ttlWarn0to4 = ttlWarn0to4 + 1;
                        }
                        if (mStatusList[i].mNumProblems_4to8 > 0) {
                            ttlWarn4to8 = ttlWarn4to8 + 1;
                        }
                        if (mStatusList[i].mNumProblems_8to12 > 0) {
                            ttlWarn8to12 = ttlWarn8to12 + 1;
                        }
                        if (mStatusList[i].mNumProblems_12to16 > 0) {
                            ttlWarn12to16 = ttlWarn12to16 + 1;
                        }
                        if (mStatusList[i].mNumProblems_16to20 > 0) {
                            ttlWarn16to20 = ttlWarn16to20 + 1;
                        }
                        if (mStatusList[i].mNumProblems_20to0 > 0) {
                            ttlWarn20to0 = ttlWarn20to0 + 1;
                        }
                    }
                    if (codeType == "OFFLINE") {
                        //alert("off 0to4 = " + mStatusList[i].mNumProblems_0to4 + ", 4to8 = " + mStatusList[i].mNumProblems_4to8 + ", 8to12 = " + mStatusList[i].mNumProblems_8to12
                        //        + ", 12to16 = " + mStatusList[i].mNumProblems_12to16 + ", 16to20 = " + mStatusList[i].mNumProblems_16to20 + ", 20to0 = " + mStatusList[i].mNumProblems_20to0);
                        ttlOffline = ttlOffline + 1;
                        if (mStatusList[i].mNumProblems_0to4 > 0) {
                            ttlOffline0to4 = ttlOffline0to4 + 1;
                        }
                        if (mStatusList[i].mNumProblems_4to8 > 0) {
                            ttlOffline4to8 = ttlOffline4to8 + 1;
                        }
                        if (mStatusList[i].mNumProblems_8to12 > 0) {
                            ttlOffline8to12 = ttlOffline8to12 + 1;
                        }
                        if (mStatusList[i].mNumProblems_12to16 > 0) {
                            ttlOffline12to16 = ttlOffline12to16 + 1;
                        }
                        if (mStatusList[i].mNumProblems_16to20 > 0) {
                            ttlOffline16to20 = ttlOffline16to20 + 1;
                        }
                        if (mStatusList[i].mNumProblems_20to0 > 0) {
                            ttlOffline20to0 = ttlOffline20to0 + 1;
                        }
                    }
                } // end for loop.

                if (ttlErr0to4 > 0) {
                    ttlErr0to4 = ((ttlErr0to4 / glbTtlMach) * 100).toFixed(2); // numErrInMach / ttlMach * 100%
                }
                if (ttlErr4to8 > 0) {
                    ttlErr4to8 = ((ttlErr4to8 / glbTtlMach) * 100).toFixed(2);
                }
                if (ttlErr8to12 > 0) {
                    ttlErr8to12 = ((ttlErr8to12 / glbTtlMach) * 100).toFixed(2);
                }
                if (ttlErr12to16 > 0) {
                    ttlErr12to16 = ((ttlErr12to16 / glbTtlMach) * 100).toFixed(2);
                }
                if (ttlErr16to20 > 0) {
                    ttlErr16to20 = ((ttlErr16to20 / glbTtlMach) * 100).toFixed(2);
                }
                if (ttlErr20to0 > 0) {
                    ttlErr20to0 = ((ttlErr20to0 / glbTtlMach) * 100).toFixed(2);
                }

                if (ttlWarn0to4 > 0) {
                    ttlWarn0to4 = ((ttlWarn0to4 / glbTtlMach) * 100).toFixed(2);
                }
                if (ttlWarn4to8 > 0) {
                    ttlWarn4to8 = ((ttlWarn4to8 / glbTtlMach) * 100).toFixed(2);
                }
                if (ttlWarn8to12 > 0) {
                    ttlWarn8to12 = ((ttlWarn8to12 / glbTtlMach) * 100).toFixed(2);
                }
                if (ttlWarn12to16 > 0) {
                    ttlWarn12to16 = ((ttlWarn12to16 / glbTtlMach) * 100).toFixed(2);
                }
                if (ttlWarn16to20 > 0) {
                    ttlWarn16to20 = ((ttlWarn16to20 / glbTtlMach) * 100).toFixed(2);
                }
                if (ttlWarn20to0 > 0) {
                    ttlWarn20to0 = ((ttlWarn20to0 / glbTtlMach) * 100).toFixed(2);
                }

                if (ttlOffline0to4 > 0) {
                    ttlOffline0to4 = ((ttlOffline0to4 / glbTtlMach) * 100).toFixed(2);
                }
                if (ttlOffline4to8 > 0) {
                    ttlOffline4to8 = ((ttlOffline4to8 / glbTtlMach) * 100).toFixed(2);
                }
                if (ttlOffline8to12 > 0) {
                    ttlOffline8to12 = ((ttlOffline8to12 / glbTtlMach) * 100).toFixed(2);
                }
                if (ttlOffline12to16 > 0) {
                    ttlOffline12to16 = ((ttlOffline12to16 / glbTtlMach) * 100).toFixed(2);
                }
                if (ttlOffline16to20 > 0) {
                    ttlOffline16to20 = ((ttlOffline16to20 / glbTtlMach) * 100).toFixed(2);
                }
                if (ttlOffline20to0 > 0) {
                    ttlOffline20to0 = ((ttlOffline20to0 / glbTtlMach) * 100).toFixed(2);
                }

                var ttlNormal0to4 = (100 - ttlErr0to4 - ttlWarn0to4 - ttlOffline0to4).toFixed(2);
                var ttlNormal4to8 = (100 - ttlErr4to8 - ttlWarn4to8 - ttlOffline4to8).toFixed(2);
                var ttlNormal8to12 = (100 - ttlErr8to12 - ttlWarn8to12 - ttlOffline8to12).toFixed(2);
                var ttlNormal12to16 = (100 - ttlErr12to16 - ttlWarn12to16 - ttlOffline12to16).toFixed(2);
                var ttlNormal16to20 = (100 - ttlErr16to20 - ttlWarn16to20 - ttlOffline16to20).toFixed(2);
                var ttlNormal20to0 = (100 - ttlErr20to0 - ttlWarn20to0 - ttlOffline20to0).toFixed(2);

                ////////////////////////

                // get current time to show.
                var now = new Date();
                //now = now.getHours() + ':' + now.getMinutes() + ':' + now.getSeconds();
                var curHour = now.getHours();
                var curMin = now.getMinutes();
                var timeLabel = [];
                var proceed = true;

                if (curHour < 4) {
                    timeLabel.push(curHour + ":" + curMin);
                    proceed = false;
                } else {
                    timeLabel.push('04:00');
                }
                if (proceed == true) {
                    if (curHour < 8) {
                        timeLabel.push(curHour + ":" + curMin);
                        proceed = false;
                    } else {
                        timeLabel.push('08:00');
                    }
                }
                if (proceed == true) {
                    if (curHour < 12) {
                        timeLabel.push(curHour + ":" + curMin);
                        proceed = false;
                    } else {
                        timeLabel.push('12:00');
                    }
                }
                if (proceed == true) {
                    if (curHour < 16) {
                        timeLabel.push(curHour + ":" + curMin);
                        proceed = false;
                    } else {
                        timeLabel.push('16:00');
                    }
                }
                if (proceed == true) {
                    if (curHour < 20) {
                        timeLabel.push(curHour + ":" + curMin);
                        proceed = false;
                    } else {
                        timeLabel.push('20:00');
                    }
                }
                if (proceed == true) {
                    if (curHour >= 20) {
                        timeLabel.push(curHour + ":" + curMin);
                    }
                }
                var datasetsData = [];
                var mNumProData = [];
                var colorOpt = {};
                colorOpt["label"] = "Error";
                colorOpt["fillColor"] = "rgba(220,220,220,0.2)";
                colorOpt["strokeColor"] = "red";
                colorOpt["pointColor"] = "red";
                colorOpt["pointStrokeColor"] = "fff";
                colorOpt["pointHighlightFill"] = "fff";
                colorOpt["pointHighlightStroke"] = "red";
                //colorOpt["data"] = [ttlErr0to4, ttlErr4to8, ttlErr8to12, ttlErr12to16, ttlErr16to20, ttlErr20to0];
                mNumProData.push(ttlErr0to4);
                if (curHour >= 4) {
                    mNumProData.push(ttlErr4to8);
                }
                if (curHour >= 8) {
                    mNumProData.push(ttlErr8to12);
                }
                if (curHour >= 12) {
                    mNumProData.push(ttlErr12to16);
                }
                if (curHour >= 16) {
                    mNumProData.push(ttlErr16to20);
                }
                if (curHour >= 20) {
                    mNumProData.push(ttlErr20to0);
                }
                colorOpt["data"] = mNumProData;
                datasetsData.push(colorOpt);

                mNumProData = [];
                colorOpt = {};
                colorOpt["label"] = "Warning";
                colorOpt["fillColor"] = "rgba(220,220,220,0.2)";
                colorOpt["strokeColor"] = "#F0AD4E";
                colorOpt["pointColor"] = "#F0AD4E";
                colorOpt["pointStrokeColor"] = "fff";
                colorOpt["pointHighlightFill"] = "fff";
                colorOpt["pointHighlightStroke"] = "#F0AD4E";
                    mNumProData.push(ttlWarn0to4);
                if (curHour >= 4) {
                    mNumProData.push(ttlWarn4to8);
                }
                if (curHour >= 8) {
                    mNumProData.push(ttlWarn8to12);
                }
                if (curHour >= 12) {
                    mNumProData.push(ttlWarn12to16);
                }
                if (curHour >= 16) {
                    mNumProData.push(ttlWarn16to20);
                }
                if (curHour >= 20) {
                    mNumProData.push(ttlWarn20to0);
                }
                colorOpt["data"] = mNumProData;
                datasetsData.push(colorOpt);

                mNumProData = [];
                colorOpt = {};
                colorOpt["label"] = "Normal";
                colorOpt["fillColor"] = "rgba(220,220,220,0.2)";
                colorOpt["strokeColor"] = "#5CB85C";
                colorOpt["pointColor"] = "#5CB85C";
                colorOpt["pointStrokeColor"] = "fff";
                colorOpt["pointHighlightFill"] = "fff";
                colorOpt["pointHighlightStroke"] = "#5CB85C";
                    mNumProData.push(ttlNormal0to4);
                if (curHour >= 4) {
                    mNumProData.push(ttlNormal4to8);
                }
                if (curHour >= 8) {
                    mNumProData.push(ttlNormal8to12);
                }
                if (curHour >= 12) {
                    mNumProData.push(ttlNormal12to16);
                }
                if (curHour >= 16) {
                    mNumProData.push(ttlNormal16to20);
                }
                if (curHour >= 20) {
                    mNumProData.push(ttlNormal20to0);
                }
                colorOpt["data"] = mNumProData;
                datasetsData.push(colorOpt);

                mNumProData = [];
                colorOpt = {};
                colorOpt["label"] = "Offline";
                colorOpt["fillColor"] = "rgba(220,220,220,0.2)";
                colorOpt["strokeColor"] = "#999";
                colorOpt["pointColor"] = "#999";
                colorOpt["pointStrokeColor"] = "fff";
                colorOpt["pointHighlightFill"] = "fff";
                colorOpt["pointHighlightStroke"] = "#999";                
                mNumProData.push(ttlOffline0to4);                
                if (curHour >= 4) {
                    mNumProData.push(ttlOffline4to8);
                }
                if (curHour >= 8) {
                    mNumProData.push(ttlOffline8to12);
                }
                if (curHour >= 12) {
                    mNumProData.push(ttlOffline12to16);
                }
                if (curHour >= 16) {
                    mNumProData.push(ttlOffline16to20);
                }
                if (curHour >= 20) {
                    mNumProData.push(ttlOffline20to0);
                }
                colorOpt["data"] = mNumProData;
                datasetsData.push(colorOpt);

                var lineChartData = {
                    labels: timeLabel,
                    datasets: datasetsData
                };
                console.log(timeLabel);
                console.log(lineChartData);
                var myLineChartLoc = new Chart(document.getElementById("machStatusLineChart_machid").getContext("2d")).Line(lineChartData);

                legend_lineChart(document.getElementById("lineLegend_machid"), lineChartData);

            },
            error: function (error) {
                alert("Error when trying to generate machine status chart!");
            }
        });
    }
});