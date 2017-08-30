var pageNumber = 1; //Hardcode to show first page when document load
var recordPerPage;  //Records to show per page
var numberOfPage;
var firstTime = true;
var CassetteNameArray = [];
var CassetteOldArray = [];

function getMachineList(pageNumber, recordPerPage) {
    $.ajax({
        type: "POST",
        contentType: "application/json; charset=utf-8",
        url: "/views/kioskMaintenance/cassette/cassetteMain.aspx/getMachList",
        data: "{pageNumber: '" + pageNumber + "', recordPerPage: '" + recordPerPage + "'}",
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
            var totalRecord = Obj.Count; //The first element of the array will return the total records in the database table. Example: 83 records
            totalRecord = totalRecord / recordPerPage; //The total record divided by the record to show per page. Example: 83 Total / 10 records per page = 8.3 
            var totalPages = Math.ceil(totalRecord); //The result should round up. Example: 8.3 pages, should round up to 9 pages. There will be no decimal number of page

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
                        for (var a = 1 + (totalPages - 10); a <= totalPages; a++) {
                            pages = pages + '<li class="page-number"><a data-paging="' + a + '">' + a + '</a></li>';
                        }
                    }
                }
                pages = pages + '<li id="last-page"><a data-paging="' + totalPages + '">&raquo;</a></li>';
            }

            var machines = "";
            var machList = Obj.List;

            if (machList.length === 0) {
                $('#full-kiosk-list-table > tbody').html("<tr><td colspan='7'>No Kiosk</td></tr>");
                $('#page-information').html("0 of 0");
            }
            else {
                for (var index = 0; index < machList.length; index++) {
                    // get casette list by machine id.

                    var cassList = machList[index].CassetteList;
                    var cassName = "";
                    var cassDate = "";
                    var cassInUse = "";
                    var cassLastReplenishDate = "";
                    for (var i = 0; i < cassList.length; i++) {
                        cassName += "<div>" + cassList[i].CassetteName + "</div>";
                        cassDate += "<div>" + cassList[i].CassetteDate + "</div>";
                        //cassInUse += "<div>" + cassList[i].CassetteInUse + "</div>";
                        if (cassList[i].CassetteInUse == 1) {
                            cassInUse += "<div>Yes</div>";
                        } else {
                            cassInUse += "<div>-</div>";
                        }
                        cassLastReplenishDate += "<div>" + cassList[i].LastReplenishDate + "</div>";
                    }
                    machines = machines + "<tr><td>" + machList[index].NumCount + "</td><td>" + machList[index].MachineID + "</td><td>" + cassName + "</td><td>" + cassDate + "</td><td>" + cassInUse + "</td><td>"
                    + cassLastReplenishDate + "</td><td style='text-align:center;'><a style='cursor: pointer;' class='.editUser-button' id='" + machList[index].MachineID + "'"
                                + "' onclick=\"editMachCassette('" + machList[index].MachineID + "')\" data-toggle=\"modal\" data-target=\"#editMachCassetteModal\"><i class=\"fa fa-pencil\"></i>Edit</a> </td></tr>";
                }

                var pageInfo = machList[0].NumCount + '-' + machList[machList.length - 1].NumCount + ' of ' + Obj.Count + '';

                $('#pagination').html(pages);
                $('#full-kiosk-list-table > tbody').html(machines);
                $('#page-information').html(pageInfo);
            }

            $('#pagination .page-number').each(function () {
                if ($(this).find('a').attr('data-paging') == currentPage) {
                    $(this).addClass('active');
                }
            });

        },
        error: function (error) {
            alert("Error occurs when trying to load kiosk list");
        }
    });
}

function pagination() {
    $('#cassette-main-div').on('click', '#pagination li', function () {

        var pageNo = $(this).find('a').attr('data-paging'); //Get the page number that user click on
        var currentPage = $('#pagination > .active > a').attr('data-paging'); //Get the current showing page number

        if (pageNo == currentPage) {
            return false;
        }
        else {
            getMachineList(pageNo, recordPerPage);
        }
    });

    $('#cassette-main-div').on('change', '#record-per-page', function () {
        recordPerPage = $(this).val();

        $('#full-kiosk-list-table > tbody').html("Loading...");

        getMachineList(pageNumber, recordPerPage);
    });

}

function getAllCassette() {
    $.ajax({
        type: "POST",
        contentType: "application/json; charset=utf-8",
        url: "/views/kioskMaintenance/cassette/cassetteMain.aspx/getAllCassette",
        data: "",
        dataType: "json",
        success: function (data) {
            var status = data.d.Status;
            var msg = data.d.Message;

            if (!status) {
                alert(msg);
                return false;
            }
            var str = "";
            var Obj = data.d.Object;

            for (var i = 0; i < Obj.length; i++) {
                //var code = MCode.getAttribute('data-code');
                var CassetteName = Obj[i].CassetteName;
                str = str + '<div class="item-listing" data-cassettename="' + CassetteName + '">'
                                   + '<span>' + CassetteName + '</span><span class="remove-btn"><i class="fa fa-times"></i></span></div>';
            }

            $('#CassetteListing').html(str);

        },
        error: function (error) {
            alert("Error occurs when trying to Edit machine version.");
            //$('#edit-version-body').html("Error occurs when trying to Edit machinse version.");
        }
    });
}

function editMachCassette(machid) {
    CassetteOldArray = [];
    $('#txtEditKioskID').val(machid);
    $.ajax({
        type: "POST",
        contentType: "application/json; charset=utf-8",
        url: "/views/kioskMaintenance/cassette/cassetteMain.aspx/editMachCassette",
        data: "{machid : '" + machid + "'}",
        dataType: "json",
        success: function (data) {
            var status = data.d.Status;
            var msg = data.d.Message;

            if (!status) {
                alert(msg);
                return false;
            }

            var Obj = data.d.Object;
            var str = "";
            for (var i = 0; i < Obj.length; i++) {
                //var code = MCode.getAttribute('data-code');
                var CassetteName = Obj[i].CassetteName;
                CassetteOldArray.push(CassetteName);
                str = str + '<div class="item-listing" data-cassettename="' + CassetteName + '">'
                                   + '<span>' + CassetteName + '</span><span class="remove-btn"><i class="fa fa-times"></i></span></div>';
            }

            $('#ExistingCassetteList').html(str);
            UpdateAddedCassetteNameArray();

        },
        error: function (error) {
            alert("Error occurs when trying to edit Cassette.");
        }
    });
}

function addNewCassetteName() {
    var CassetteName = $('#txtAddCassetteName').val();
    if (CassetteName == "") {
        alert("Please enter Cassette Name!");
        return;
    } else {
        $.ajax({
            type: "POST",
            contentType: "application/json; charset=utf-8",
            url: "/views/kioskMaintenance/cassette/cassetteMain.aspx/addCassetteName",
            data: "{CassetteName : '" + CassetteName + "'}",
            dataType: "json",
            success: function (data) {
                var status = data.d.Status;
                var msg = data.d.Message;
                alert("status = " + status + ", msg = " + msg);

                if (!status) {
                    alert(msg);
                    return false;
                }


                //var Obj = data.d.Object;
                ////console.log(Obj);
                //$('#txtEditKioskID').val(machid);
                //for (var i = 0; i < Ojb.length; i++) {

                //}

            },
            error: function (error) {
                alert("Error occurs when trying to Edit machine version.");
                //$('#edit-version-body').html("Error occurs when trying to Edit machinse version.");
            }
        });
    }
}

// Add & Assign Cassette 
function addCassetteNameToList() {
    var CassetteName = $('#txtAddCassetteName').val().trim();
    var str = "";
    if (CassetteName == "") {
        alert("Please enter Cassette Name!");
        return;
    }

    if (CassetteNameArray.length !== 0) {
        for (var i = 0; i < CassetteNameArray.length, CassetteExistingName = CassetteNameArray[i]; i++) {
            if (CassetteName.toUpperCase() == CassetteExistingName.toUpperCase()) {
                alert("The Cassette Name is existed in the list! Please try again.");
                return;
            }
        }
    }

    str = str + '<div class="item-listing" data-cassetteName="' + CassetteName + '">'
                        + '<span>' + CassetteName + '</span><span class="remove-btn"><i class="fa fa-times"></i></span></div>';

    $('#CassetteListing').append(str);
    UpdateSelectedCassetteNameArray();

    $('#txtAddCassetteName').val("");
}

// Edit from main
function addCassetteNameToExistingList() {
    var CassetteName = $('#txtEditCassetteName').val().trim();
    var str = "";

    if (CassetteName == "") {
        alert("Please enter Cassette Name!");
        return;
    }

    if (CassetteNameArray.length !== 0) {
        for (var i = 0; i < CassetteNameArray.length, CassetteExistingName = CassetteNameArray[i]; i++) {
            if (CassetteName.toUpperCase() == CassetteExistingName.toUpperCase()) {
                alert("The Cassette Name is existed in the list! Please try again.");
                return;
            }
        }
    }
    
        str = str + '<div class="item-listing" data-cassetteName="' + CassetteName + '">'
                           + '<span>' + CassetteName + '</span><span class="remove-btn"><i class="fa fa-times"></i></span></div>';
    
    $('#ExistingCassetteList').append(str);
    UpdateAddedCassetteNameArray();

    $('#txtEditCassetteName').val("");
}


function removeItem() {
    $(this).closest('.item-listing').fadeOut('fast', function () {
        $(this).remove();
        UpdateSelectedCassetteNameArray();
    });
}

function removeExistingItem() {
    $(this).closest('.item-listing').fadeOut('fast', function () {
        $(this).remove();
        UpdateAddedCassetteNameArray();
    });
}

function UpdateSelectedCassetteNameArray() {
    var CassetteListing = document.getElementById("CassetteListing").querySelectorAll(".item-listing");

    CassetteNameArray = [];

    if (CassetteListing.length !== 0) {

        for (var i = 0; i < CassetteListing.length, CassetteName = CassetteListing[i]; i++) {
            var cassName = CassetteName.getAttribute('data-cassetteName');
            CassetteNameArray.push(cassName);
        }
    }
}

function UpdateAddedCassetteNameArray() {
    var CassetteListing = document.getElementById("ExistingCassetteList").querySelectorAll(".item-listing");

    CassetteNameArray = [];

    if (CassetteListing.length !== 0) {

        for (var i = 0; i < CassetteListing.length, CassetteName = CassetteListing[i]; i++) {
            var cassName = CassetteName.getAttribute('data-cassetteName');
            CassetteNameArray.push(cassName);
        }
    }
}

function saveCassetteConfig() {

    if (CassetteNameArray.length == 0) {
        alert("Please add cassette!");
        return;
    }

    var machid = "";
    var selectedMachines = document.getElementById("machine-list-main").querySelectorAll(".a-selected-machine-id");
    if (selectedMachines.length == 0) {
        alert("Please select a Kiosk ID!");
        return false;
    } else if (selectedMachines.length > 1) {
        alert("Multiple Kiosk ID has been selected.");
        return false;
    } else {
        machid = selectedMachines[0].innerText;
    }

    $.ajax({
        type: "POST",
        contentType: "application/json; charset=utf-8",
        url: "/views/kioskMaintenance/cassette/cassetteMain.aspx/saveAddAssignCassette",
        data: "{machid: '" + machid + "', CassetteNameArray: '" + CassetteNameArray + "'}",
        dataType: "json",
        success: function (data) {
            var status = data.d.Status;
            var msg = data.d.Message;

            if (!status) {
                alert(msg);
                return false;
            }
            getMachineList(pageNumber, recordPerPage);
            alert("Cassettes added successfully!");

            $('#addCassetteModal').modal('hide');
        },
        error: function (error) {
            alert("Error occurs when trying to save cassette configuration.");
            //$('#edit-version-body').html("Error occurs when trying to Edit machinse version.");
        }
    });
}



/*
Get all machines
*/
function getAllMachines() {
    $.ajax({
        type: "POST",
        contentType: "application/json; charset=utf-8",
        url: "/views/kioskMaintenance/cassette/cassetteMain.aspx/getAllMachineList",
        data: "{}",
        dataType: "json",
        beforeSend: function () {
            $('#machine-list-main').html('Loading...');
        },
        success: function (data) {

            var machines = "";

            for (var i = 0; i < data.d.length, Machine = data.d[i]; i++) {
                machines = machines + '<div branch-code="' + Machine.BranchCode + '" branch-name="' + Machine.BranchName + '" address1="' + Machine.Address1 + '" address2="' + Machine.Address2 + '" class="col-md-2 a-machine">' + data.d[i].MachineID + '</div>';
            }

            $('#machine-list-main').html(machines);
        },
        error: function (error) {
            alert("Error occurs when trying to load the machine list.");
        }
    });
}

$('#addAssignCassetteMainDiv').on('mouseover', '#machine-list-main .a-machine', function (e) {
    $('#machine-detail-tooltip').remove();
    var x = e.clientX;
    var y = e.clientY;

    var branchCode = $(this).attr('branch-code');
    var branchName = $(this).attr('branch-name');
    var address1 = $(this).attr('address1');
    var address2 = $(this).attr('address2');

    $('#cassette-main-div').append('<div id="machine-detail-tooltip" style="position:fixed;"><span class="machine-detail-branch-code">' + branchCode + '</span><span class="machine-detail-branch-name">' + branchName + '</span><br/>'
                                        + '<span class="machine-detail-address">' + address1 + '<br/>' + address2 + '</span></div>');
    $('#machine-detail-tooltip').css({ 'top': y, 'left': x });

}).mouseout(function () {
    $('#machine-detail-tooltip').remove();
});

$('#addAssignCassetteMainDiv').on('click', '#machine-list-main .a-machine', function () {

    if ($(this).hasClass('a-selected-machine-id')) {
        $(this).removeClass('a-selected-machine-id');
    } else {
        $('#machine-list-main .a-machine').removeClass('a-selected-machine-id');
        $(this).addClass('a-selected-machine-id');
    }
});

function updateCassette() {
    if (CassetteNameArray.length == 0) {        
        if (CassetteOldArray.length == 0) {
            alert("Please enter cassette name!");
            return;
        } else {
            var r = confirm("There is no Cassette assign. \n Are you sure to proceed?");
            if (r != true) {
                return;
            } 
        }
    }

    var machid = $('#txtEditKioskID').val();

    $.ajax({
        type: "POST",
        contentType: "application/json; charset=utf-8",
        url: "/views/kioskMaintenance/cassette/cassetteMain.aspx/updateCassette",
        data: "{machid: '" + machid + "', CassetteNameArray: '" + CassetteNameArray + "', CassetteOldArray: '" + CassetteOldArray + "'}",
        dataType: "json",
        success: function (data) {
            var status = data.d.Status;
            var msg = data.d.Message;

            if (!status) {
                alert(msg);
                return false;
            }
            $('#txtEditCassetteName').val('');
            alert("Cassettes updated successfully!");

            getMachineList(pageNumber, recordPerPage);
            $('#editMachCassetteModal').modal('hide');
        },
        error: function (error) {
            alert("Error occurs when trying to update cassette.");
        }
    });
}