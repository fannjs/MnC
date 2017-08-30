var pageNumber = 1; //Hardcode to show first page when document load
var recordPerPage;  //Records to show per page
var numberOfPage;
var firstTime = true;

function getMachineList(currentPage, recordEachPage) {
    $.ajax({
        type: "POST",
        contentType: "application/json; charset=utf-8",
        url: "/views/kioskManagement/manageKiosk/mgnKiosk.aspx/getMachineList",
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

            if (totalRecord == 0) {
                $('#full-kiosk-list-table > tbody').html("<tr><td colspan='9'>No Kiosk</td></tr>");
                $('#page-information').html('0 of 0');

            } else {

                for (var index = 1; index < data.d.length; index++) {
                    machines = machines + '<tr><td>' + data.d[index].MCount + '</td><td>' + data.d[index].MachineID + '</td><td>' + data.d[index].MState + '</td><td>' + data.d[index].MDistrict + '</td>'
                                + '<td>' + data.d[index].MBranchNo + '</td> <td>' + data.d[index].MBranchName + '</td><td>' + data.d[index].CalendarName + '</td><td>' + data.d[index].CurrentVer + '</td>'

                                + '<td><a style="cursor: pointer;" class=".editUser-button" id="' + data.d[index].MachineID + '"'
                                + "' onclick=\"editMach('" + data.d[index].MachineID + "')\" data-toggle=\"modal\" data-target=\"#editMachModal\"><i class=\"fa fa-pencil\"></i>Edit</a>  </td> </tr>";
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
            pageNumber = pageNo;
            getMachineList(pageNumber, recordPerPage);
        }
    });

    $('#full-kiosk-list-main').on('change', '#record-per-page', function () {
        recordPerPage = $(this).val();

        $('#full-kiosk-list-table > tbody').html("Loading...");

        getMachineList(pageNumber, recordPerPage);
    });
}

var Machine = {};

function editMach(machid) {
    $('input[type=text]').val('');
    loadCalendar();

    $.ajax({
        type: "POST",
        contentType: "application/json; charset=utf-8",
        url: "/views/kioskManagement/manageKiosk/mgnKiosk.aspx/editMach",
        data: "{machid : '" + machid + "'}",
        dataType: "json",
        beforeSend: function () {
            $('#edit-version-body').hide();
            $('#loading-screen-div').show();
        },
        success: function (data) {

            if (data.d.length === 0) {
                alert("Unable to find Kiosk");
                return false;
            }

            $('#txtKioskID').val(machid);
            $('#txtContact').val(data.d[0].MTel);
            $('#txtPIC').val(data.d[0].MPIC);

            $('#selectVendor').find('option[value="' + data.d[0].MVendor + '"]').prop('selected', true);

            Machine["Site"] = data.d[0].CustCode;
            Machine["State"] = data.d[0].MState;
            Machine["District"] = data.d[0].MDistrict;
            Machine["City"] = data.d[0].MCity;
            Machine["BranchNo"] = data.d[0].MBranchNo;


            if (data.d[0].MCalendar != "") {
                $("#selectCalendar").find('option').each(function (i, opt) {
                    if (opt.value === data.d[0].MCalendar)
                        $(opt).attr('selected', 'selected');
                });
            }

            LoadCustomer();

            $('#loading-screen-div').hide();
            $('#edit-version-body').fadeIn('fast');

        },
        error: function (error) {
            alert("Error occurs when trying to edit kiosk.");
            //$('#edit-version-body').html("Error occurs when trying to Edit machinse version.");
        }
    });
}

function updateMach(machid) {
//    var MState = $('#txtState').val();
//    var MDistrict = $('#txtDistrict').val();
//    var MCity = $('#txtCity').val();
//    var MAddress1 = $('#txtAddress1').val();
//    var MAddress2 = $('#txtAddress2').val();
//    var MBranchNoOri = $('#txtBranchNoOri').val();
//    var MBranchNo = $('#txtBranchNo').val();
//    var MBranchName = $('#txtBranchName').val();
//    var MTel = $('#txtContact').val();
//    var MPIC = $('#txtPIC').val();
//    var MVendor = $('#txtVendor').val();

    var BranchCode = $('#selectBranch').val();
    var VendorId = $('#selectVendor').val();
    var VendorName = $('#selectVendor > option:selected').text();
    var CalendarId = $('#selectCalendar').val();
    var CalendarName = $('#selectCalendar > option:selected').text();

    if (CalendarId == null) {
        CalendarId = "";
        CalendarName = "";
    }
    if (VendorId == null) {
        VendorId = "";
        VendorName = "";
    }

    $.ajax({
        type: "POST",
        contentType: "application/json; charset=utf-8",
        url: "/views/kioskManagement/manageKiosk/mgnKiosk.aspx/UpdateMachine",
        data: JSON.stringify({ MachId: machid, BranchCode: BranchCode, VendorId: VendorId, VendorName:VendorName,CalendarId: CalendarId, CalendarName:CalendarName }),
        dataType: "json",
        success: function (data) {

            var status = data.d.Status;
            var msg = data.d.Message;

            if (!status) {
                alert(msg);
                return false;
            }

            alert("Successful. Your action has been sent to checker for approval.");

            $('#editMachModal').modal('hide');

            getMachineList(pageNumber, recordPerPage);
        },
        error: function (error) {
            alert("Error occurs when trying to Update Machine.");
        }
    });
}

function loadCalendar() {
    $.ajax({
        type: "POST",
        contentType: "application/json; charset=utf-8",
        url: "/views/kioskManagement/manageKiosk/mgnKiosk.aspx/GetCalendar",
        data: "{}",
        dataType: "json",
        success: function (data) {

            var status = data.d.Status;
            var msg = data.d.Message;

            if (!status) {
                alert(msg);
                return false;
            }

            var CalendarList = data.d.Object;
            var str = '<option selected disabled value=""> - Please Select - </option>';

            if (CalendarList.length == 0) {
                str = str + '<option disabled> No Calendar </option>';
            }
            else {
                for (var i = 0; i < CalendarList.length, Calendar = CalendarList[i]; i++) {
                    str = str + '<option value="' + Calendar.ID + '">' + Calendar.Name + '</option>';
                }
            }
            str = str + '</select>';

            $('#selectCalendar').html(str);
        },
        error: function (error) {
            alert("Error " + error.status + ". Unable to load Calendar. ");
        }
    });
}

$('#btnUpdateMach').click(function () {
    var machid = $('#txtKioskID').val();
    updateMach(machid);
});

$(document).ready(function () {
    $('#full-kiosk-list-main').off();

    recordPerPage = $('#record-per-page').val();

    getMachineList(pageNumber, recordPerPage);
    pagination();

    LoadVendor();
});

function LoadVendor() {
    $.ajax({
        type: "POST",
        contentType: "application/json; charset=utf-8",
        url: "/views/kioskManagement/manageKiosk/mgnKiosk.aspx/GetVendor",
        data: "{}",
        dataType: "json",
        success: function (data) {

            var status = data.d.Status;
            var msg = data.d.Message;

            if (!status) {
                alert(msg);
                return false;
            }

            var VendorList = data.d.Object;
            var str = '<option selected disabled value=""> - Please Select - </option>';

            if (VendorList.length == 0) {
                str = str + '<option disabled> (empty) </option>';
            }
            else {
                for (var i = 0; i < VendorList.length, Vendor = VendorList[i]; i++) {
                    str = str + '<option value="' + Vendor.ID + '">' + Vendor.Name + '</option>';
                }
            }
            str = str + '</select>';

            $('#selectVendor').html(str);
        },
        error: function (error) {
            alert("Error " + error.status + ". Unable to load Vendor. ");
        }
    });
}

//Changes at below
function LoadCustomer() {
    $.ajax({
        type: "POST",
        contentType: "application/json; charset=utf-8",
        url: "/views/kioskManagement/manageKiosk/mgnKiosk.aspx/GetCustomer",
        data: "{}",
        dataType: "json",
        success: function (data) {

            var status = data.d.Status;
            var msg = data.d.Message;

            if (!status) {
                alert(msg);
                return false;
            }

            var CustomerList = data.d.Object;
            var str = '<option selected disabled value=""> - Please Select - </option>';

            if (CustomerList === null) {
                str = str + '<option disabled> (empty) </option>';
            } else {
                if (CustomerList.length == 0) {
                    str = str + '<option disabled> (empty) </option>';
                }
                else {
                    for (var i = 0; i < CustomerList.length, Customer = CustomerList[i]; i++) {
                        if (Customer.Code === Machine["Site"]) {
                            str = str + '<option selected value="' + Customer.Code + '">' + Customer.Code + ' - ' + Customer.Name + '</option>';
                        } else {
                            str = str + '<option value="' + Customer.Code + '">' + Customer.Code + ' - ' + Customer.Name + '</option>';
                        }
                    }
                }
            }
            $('#selectSite').html(str);

            LoadState();
        },
        error: function (error) {
            alert("Error " + error.status + ". Unable to load customer. ");
        }
    });
}

function LoadState() {
    var CustCode = $('#selectSite').val();

    $.ajax({
        type: "POST",
        contentType: "application/json; charset=utf-8",
        url: "/views/kioskManagement/manageKiosk/mgnKiosk.aspx/GetState",
        data: JSON.stringify({ CustCode: CustCode }),
        dataType: "json",
        success: function (data) {

            var status = data.d.Status;
            var msg = data.d.Message;

            if (!status) {
                alert(msg);
                return false;
            }

            var StateList = data.d.Object;
            var str = '<option selected disabled value=""> - Please Select - </option>';

            if (StateList === null) {
                str = str + '<option disabled> (empty) </option>';
            } else {

                if (StateList.length == 0) {
                    str = str + '<option disabled> (empty) </option>';
                }
                else {
                    for (var i = 0; i < StateList.length, State = StateList[i]; i++) {
                        if (State.Name === Machine["State"]) {
                            str = str + '<option selected value="' + State.Name + '">' + State.Name + '</option>';
                        } else {
                            str = str + '<option value="' + State.Name + '">' + State.Name + '</option>';
                        }
                    }
                }
            }

            $('#selectState').html(str);

            LoadDistrict();
        },
        error: function (error) {
            alert("Error " + error.status + ". Unable to load State. ");
        }
    });
}

function LoadDistrict() {
    var CustCode = $('#selectSite').val();
    var State = $('#selectState').val();

    $.ajax({
        type: "POST",
        contentType: "application/json; charset=utf-8",
        url: "/views/kioskManagement/manageKiosk/mgnKiosk.aspx/GetDistrict",
        data: JSON.stringify({ CustCode: CustCode, State: State }),
        dataType: "json",
        success: function (data) {

            var status = data.d.Status;
            var msg = data.d.Message;

            if (!status) {
                alert(msg);
                return false;
            }

            var DistrictList = data.d.Object;
            var str = '<option selected disabled value=""> - Please Select - </option>';

            if (DistrictList === null) {
                str = str + '<option disabled> (empty) </option>';
            } else {
                if (DistrictList.length == 0) {
                    str = str + '<option disabled> (empty) </option>';
                }
                else {
                    for (var i = 0; i < DistrictList.length, District = DistrictList[i]; i++) {
                        if (District.Name === Machine["District"]) {
                            str = str + '<option selected value="' + District.Name + '">' + District.Name + '</option>';
                        } else {
                            str = str + '<option value="' + District.Name + '">' + District.Name + '</option>';
                        }
                    }
                }
            }

            $('#selectDistrict').html(str);

            LoadBranch();
        },
        error: function (error) {
            alert("Error " + error.status + ". Unable to load District. ");
        }
    });
}
//LoadCity - Temporary not using 
function LoadCity() {
    var CustCode = $('#selectSite').val();
    var State = $('#selectState').val();
    var District = $('#selectDistrict').val();

    $.ajax({
        type: "POST",
        contentType: "application/json; charset=utf-8",
        url: "/views/kioskManagement/manageKiosk/mgnKiosk.aspx/GetCity",
        data: JSON.stringify({ CustCode: CustCode, State: State, District: District }),
        dataType: "json",
        success: function (data) {

            var status = data.d.Status;
            var msg = data.d.Message;

            if (!status) {
                alert(msg);
                return false;
            }

            var CityList = data.d.Object;
            var str = '<option selected disabled value=""> - Please Select - </option>';

            if (CityList === null) {
                str = str + '<option disabled> (empty) </option>';
            } else {
                if (CityList.length == 0) {
                    str = str + '<option disabled> (empty) </option>';
                }
                else {
                    for (var i = 0; i < CityList.length, City = CityList[i]; i++) {
                        if (City.Name === Machine["City"]) {
                            str = str + '<option selected value="' + City.Name + '">' + City.Name + '</option>';
                        }
                        else {
                            str = str + '<option value="' + City.Name + '">' + City.Name + '</option>';
                        }
                    }
                }
            }

            $('#selectCity').html(str);

            LoadBranch();
        },
        error: function (error) {
            alert("Error " + error.status + ". Unable to load City. ");
        }
    });
}

function LoadBranch() {
    var CustCode = $('#selectSite').val();
    var State = $('#selectState').val();
    var District = $('#selectDistrict').val();
    //var City = $('#selectCity').val();

    $.ajax({
        type: "POST",
        contentType: "application/json; charset=utf-8",
        url: "/views/kioskManagement/manageKiosk/mgnKiosk.aspx/GetBranch",
        data: JSON.stringify({ CustCode: CustCode, State: State, District: District }),
        dataType: "json",
        success: function (data) {

            var status = data.d.Status;
            var msg = data.d.Message;

            if (!status) {
                alert(msg);
                return false;
            }

            var BranchList = data.d.Object;
            var str = '<option selected disabled value=""> - Please Select - </option>';

            if (BranchList === null) {
                str = str + '<option disabled> (empty) </option>';
            } else {
                if (BranchList.length == 0) {
                    str = str + '<option disabled> (empty) </option>';
                }
                else {
                    for (var i = 0; i < BranchList.length, Branch = BranchList[i]; i++) {
                        if (Branch.Code.trim() === Machine["BranchNo"].trim()) {
                            str = str + '<option selected value="' + Branch.Code + '" data-pic="' + Branch.PIC + '" data-tel="' + Branch.ContactNo + '">' + Branch.Code.trim() + ' - ' + Branch.Name + '</option>';
                        }
                        else {
                            str = str + '<option value="' + Branch.Code + '" data-pic="' + Branch.PIC + '" data-tel="' + Branch.ContactNo + '">' + Branch.Code.trim() + ' - ' + Branch.Name + '</option>';
                        }
                    }
                }
            }

            $('#selectBranch').html(str);
        },
        error: function (error) {
            alert("Error " + error.status + ". Unable to load Branch. ");
        }
    });
}

function ShowBranchContactInformation() {

    var PersonInCharge = $('#selectBranch option:selected').attr('data-pic');
    var ContactNumber = $('#selectBranch option:selected').attr('data-tel');

    if (PersonInCharge.trim() === "") {
        PersonInCharge = "-";
    }
    if (ContactNumber.trim() === "") {
        ContactNumber = "-";
    }

    $('#txtPIC').val(PersonInCharge);
    $('#txtContact').val(ContactNumber);

}