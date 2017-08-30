var pageNumber = 1; //Hardcode to show first page when document load
var recordPerPage;  //Records to show per page
var numberOfPage;
var firstTime = true;

function getMachineList(currentPage, recordEachPage) {
    $.ajax({
        type: "POST",
        contentType: "application/json; charset=utf-8",
        url: "/views/kioskMaintenance/softwareDistribution/main.aspx/getMachineList",
        data: "{pageNumber: '" + currentPage + "', pageSize: '" + recordEachPage + "'}",
        dataType: "json",
        success: function (data) {

            var iCurrentPage = parseInt(currentPage); //Current page 
            var totalRecord = data.d[0].TotalMachine; //The first element of the array will return the total records in the database table. Example: 83 records
            totalRecord = totalRecord / recordEachPage; //The total record divided by the record to show per page. Example: 83 Total / 10 records per page = 8.3 
            numberOfPage = Math.ceil(totalRecord); //The result should round up. Example: 8.3 pages, should round up to 9 pages. There will be no decimal number of page

            var pages = "";

            if (numberOfPage > 1) {
                pages = pages + '<li id="first-page"><a data-paging="1">&laquo;</a></li>';

                /* 
                If less than 10, then show all page number. 
                - Example: total number of page is 9. Just show all.
                */
                if (numberOfPage <= 10) {
                    for (var i = 1; i <= numberOfPage; i++) {
                        pages = pages + '<li class="page-number"><a data-paging="' + i + '">' + i + '</a></li>';
                    }
                }
                else {
                    /*
                    If current page - 5 is less than or equal to 1
                    then show the page start from first to 10
                    - Example: Current page is 2
                    2 - 5 is less than 0
                    - Therefore: Start from first number, stop at 10;
                    */
                    if (iCurrentPage - 5 <= 1) {
                        for (var j = 1; j <= 10; j++) {
                            pages = pages + '<li class="page-number"><a data-paging="' + j + '">' + j + '</a></li>';
                        }
                    }
                    /* 
                    If current page + 5 less than or equal to total number of page
                    - Example: current page is 7, Total number of page is 20
                    - Therefore: start count from 2 (7-5), stop count until 11 (7+5)-1 [To remain as showing 10 page number only]                        
                    */
                    else if (iCurrentPage + 5 <= numberOfPage) {
                        for (var k = iCurrentPage - 5; k <= (iCurrentPage + 5) - 1; k++) {
                            pages = pages + '<li class="page-number"><a data-paging="' + k + '">' + k + '</a></li>';
                        }
                    }
                    /* 
                    If current page + 5 is greater than total number of page
                    - Example: current page is 17, Total number of page is 20  
                    17 + 5 = 22, which is more than 20
                    - Therefore: start count from 11 (1 + (total number of page - 10)) , stop count until 20 (total number of page) [Remain showing 10 pages number]               
                    */
                    else {
                        for (var a = 1 + (numberOfPage - 10); a <= numberOfPage; a++) {
                            pages = pages + '<li class="page-number"><a data-paging="' + a + '">' + a + '</a></li>';
                        }
                    }
                }
                pages = pages + '<li id="last-page"><a data-paging="' + numberOfPage + '">&raquo;</a></li>';
            }

            var machines = "";

            for (var index = 1; index < data.d.length; index++) {

                machines = machines + '<tr><td>' + data.d[index].MCount + '</td><td>' + data.d[index].MachineID + '</td><td>' + data.d[index].CurrentVer + '</td><td>' + data.d[index].NewVer + '</td>'
                                + '<td>' + data.d[index].DownloadDT + '</td><td>' + data.d[index].DownloadedDT + '</td><td>' + data.d[index].EffectiveDT + '</td><td>' + data.d[index].VerUpdatedDT + '</td>'

                                + '<td style="text-align:center;"><a style="cursor: pointer;" class=".editUser-button" id="' + data.d[index].MachineID + '"'
                                + "' onclick=\"editMachVer('" + data.d[index].MachineID + "')\" data-toggle=\"modal\" data-target=\"#editMachVerModal\"><i class=\"fa fa-pencil\"></i>Edit</a> </td> </tr>";
            }

            var pageInfo = data.d[1].MCount + '-' + data.d[data.d.length - 1].MCount + ' of ' + data.d[0].TotalMachine + '';

            $('#pagination').html(pages);
            $('#full-kiosk-list-table > tbody').html(machines);
            $('#page-information').html(pageInfo);

            if (firstTime) {
                firstTime = false;
                $('#pagination .page-number').first().addClass('active');
            }
            else {
                $('#pagination .page-number').each(function () {
                    if ($(this).find('a').attr('data-paging') == currentPage) {
                        $(this).addClass('active');
                    }
                });
            }
        },
        error: function (error) {
            alert("Error occurs when trying to load kiosk list");
        }
    });
}

function pagination() {
    $('#full-kiosk-list-main').on('click', '#pagination li', function () {

        var pageNo = $(this).find('a').attr('data-paging'); //Get the page number that user click on
        var currentPage = $('#pagination > .active > a').attr('data-paging'); //Get the current showing page number

        /*
        If user click the page number same as the current showing page number
        -Stop action
        */
        if (pageNo == currentPage) {
            return false;
        }
        else {
            getMachineList(pageNo, recordPerPage);
        }
    });

    $('#full-kiosk-list-main').on('change', '#record-per-page', function () {
        recordPerPage = $(this).val();

        $('#full-kiosk-list-table > tbody').html("Loading...");

        getMachineList(pageNumber, recordPerPage);
    });
}

