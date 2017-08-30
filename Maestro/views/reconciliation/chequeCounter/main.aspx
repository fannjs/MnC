<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="main.aspx.cs" Inherits="Maestro.views.reconciliation.chequeCounter.main" %>

<link rel="Stylesheet" href="../../../components/bootstrap.datepicker/css/datepicker.css" />
<script src="../../../components/bootstrap.datepicker/js/bootstrap-datepicker.js"></script>
<div id="cheque-counter-main-div">
    <div id="selectionDiv">
        <h1 class="h1">Search Criteria</h1>
        <hr style="margin:10px 0px;" />
        <div class="block-xs"></div>
        <div class="field-group">
            <label for="selectKioskId" class="form-label width-lg">Kiosk ID</label>
            <select class="input-field width-lg" id="selectKioskId" onchange="selectKioskEvent(this)" runat="server">
                <option value="" selected disabled> - Please select - </option>
            </select>
        </div>
        <div class="field-group">
            <label for="selectCollectionBinID" class="form-label width-lg">Collection Bin ID</label>
            <select class="input-field width-lg" id="selectCollectionBinID" disabled>
                <option value="" selected disabled> - Please select - </option>
            </select>
        </div>
        <div class="field-group">
            <label for="inputReconDate" class="form-label width-lg">Reconciliation Date</label>
            <input type="text" class="input-field width-lg" id="inputReconDate" />
        </div>
        <div class="block-xs"></div>
        <div class="field-group">
            <label class="form-label width-lg"></label>
            <button class="btn btn-primary" onclick="searchBinStatus(this);">Search</button>
        </div>
    </div>
    <div class="block-md"></div>
    <div id="searchResultDiv" style="display:none;">
        <h4>Search Result</h4>
        <hr style="margin:10px 0px;" />
        <table id="tblCollectionBin" class="table table-bordered">
            <thead>
                <tr>
                    <th>Retract Bin</th>
                    <th>Sorter Bin 1</th>
                    <th>Sorter Bin 2</th>
                    <th>Sorter Bin 3</th>
                    <th>Sorter Bin 4</th>
                </tr>
            </thead>
            <tbody>
            </tbody>
        </table>
    </div>
</div>
<script type="text/javascript">
    function searchBinStatus(elem) {
        var SelectKioskID = document.getElementById("selectKioskId");
        var SelectBinID = document.getElementById("selectCollectionBinID");
        var InputReconDate = document.getElementById("inputReconDate");
        var MachID = SelectKioskID.value.trim();
        var BinID = SelectBinID.value.trim();
        var ReconDate = InputReconDate.value.trim();

        if (MachID === "") {
            alert("Please select a Kiosk.");
            SelectKioskID.focus();
            return false;
        }
        else if (BinID === "") {
            alert("Please select a Collection Bin.");
            SelectBinID.focus();
            return false;
        }
        else if (ReconDate === "") {
            alert("Please choose a date.");
            InputReconDate.focus();
            return false;
        }

        $.ajax({
            type: "POST",
            contentType: "application/json; charset=utf-8",
            url: "/views/reconciliation/chequeCounter/main.aspx/GetBinStatus",
            data: JSON.stringify({ BinID: BinID, Date: ReconDate }),
            dataType: "json",
            beforeSend: function () {
                $(elem).prop('disabled', true);
                $('#tblCollectionBin > tbody').html('<tr><td colspan="5">Loading...</td></tr>');
                $('#searchResultDiv').fadeIn('fast');
            },
            success: function (data) {

                var status = data.d.Status;
                var msg = data.d.Message;

                $(elem).prop('disabled', false);

                if (!status) {
                    alert(msg);
                    return false;
                }

                var BinList = data.d.Object;
                var str = "";

                if (BinList.length === 0) {
                    str = str + '<tr><td colspan="5">Record not found.</td></tr>';
                } else {

                    for (var i = 0; i < BinList.length, Bin = BinList[i]; i++) {
                        var RetractBin = Bin.Bin0;
                        var Bin1 = Bin.Bin1;
                        var Bin2 = Bin.Bin2;
                        var Bin3 = Bin.Bin3;
                        var Bin4 = Bin.Bin4;

                        str = str + '<tr><td>' + RetractBin + '</td><td>' + Bin1 + '</td><td>' + Bin2 + '</td><td>' + Bin3 + '</td><td>' + Bin4 + '</td></tr>';
                    }
                }
                $('#tblCollectionBin > tbody').html(str);
            },
            error: function (error) {
                alert("Error " + error.status + ". Unable to load Bin.");
                $(elem).prop('disabled', false);
            }
        });
    }

    function selectKioskEvent(elem) {
        var $this = $(elem);
        var MachID = $this.val();

        if (MachID === "") {
            $('#selectCollectionBinID').prop('disabled', true);
            return false;
        }

        $.ajax({
            type: "POST",
            contentType: "application/json; charset=utf-8",
            url: "/views/reconciliation/chequeCounter/main.aspx/GetCollectionBin",
            data: JSON.stringify({ MachID: MachID }),
            dataType: "json",
            beforeSend: function(){
                $('#selectCollectionBinID').prop('disabled', true);
            },
            success: function (data) {

                var status = data.d.Status;
                var msg = data.d.Message;

                if (!status) {
                    alert(msg);                    
                    return false;
                }

                var BinList = data.d.Object;
                var str = '<option value="" disabled selected> - Please select - </option>';

                if (BinList.length === 0) {
                    str = str + '<option value="" disabled selected> Collection Bin not found </option>';
                } else {

                    for (var i = 0; i < BinList.length, Bin = BinList[i]; i++) {
                        var Id = Bin.Cassette_ID;
                        var Name = Bin.Cassette_Name;

                        str = str + '<option value="' + Id + '">' + Name + '</option>';
                    }
                }

                $('#selectCollectionBinID').html(str);
            },
            error: function (error) {
                alert("Error " + error.status + ". Unable to load Bin.");
            }
        }).always(function(){
            $('#selectCollectionBinID').prop('disabled', false);
        });
    }

    $(document).ready(function () {
        $('#inputReconDate').datepicker({
            format: 'dd/mm/yyyy',
            autoclose: true
        });
    });
</script>
