<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="main.aspx.cs" Inherits="Maestro.views.reconciliation.chequeCounterBalancing.main" %>

<link rel="Stylesheet" href="../../../components/bootstrap.datepicker/css/datepicker.css" />
<script src="../../../components/bootstrap.datepicker/js/bootstrap-datepicker.js"></script>
<div id="cheque-counter-main-div">
    <div style="display:block;">
        <h1 class="h1">Cut Off Details</h1>
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
        <div class="block-md"></div>
        <h1 class="h1">Counter Checking</h1>
        <hr style="margin:10px 0px;" />
        <div class="block-xs"></div>
        <h5 style="color:#999;">Please enter the number of cheque(s) obtained from each sorter bin.</h5>
        <div class="field-group">
            <label for="inputBin1" class="form-label width-xs">Bin No.1</label>
            <input type="text" class="input-field width-xs input-bin" id="inputBin1" value="0" />
            <label style="width:20px;"></label>
            <label for="inputBin2" class="form-label width-xs">Bin No.2</label>
            <input type="text" class="input-field width-xs input-bin" id="inputBin2" value="0" />
        </div>
        <div class="field-group">
            <label for="inputBin3" class="form-label width-xs">Bin No.3</label>
            <input type="text" class="input-field width-xs input-bin" id="inputBin3" value="0" />
            <label style="width:20px;"></label>
            <label for="inputBin4" class="form-label width-xs">Bin No.4</label>
            <input type="text" class="input-field width-xs input-bin" id="inputBin4" value="0" />
        </div>
        <div class="block-xs"></div>
        <div class="field-group">
            <label class="form-label width-xs"></label>
            <button class="btn btn-primary counterBalanceBtn" id="validateCounterBtn" onclick="validateCounter();">Validate</button>
        </div>
        <div class="block-md"></div>
        <h1 class="h1">Confirmation</h1>
        <hr style="margin:10px 0px;" />
        <div class="block-xs"></div>
        <div class="field-group">
            <label for="selectRemark" class="form-label width-md">Remark</label>
            <select class="input-field width-xl" id="selectRemark" disabled>
                <option value="" selected disabled> - Please select - </option>
                <option>ALL Bins MATCH</option>
                <option>ALL Bins DOES NOT MATCH</option>
                <option>SOME Bins DOES NOT MATCH</option>
            </select>
        </div>
        <div class="field-group">
            <label for="inputComments" class="form-label width-md">Comments</label>
            <textarea class="input-field width-xl" rows="4" style="resize:none;" id="inputComments" disabled></textarea>
        </div>
        <div class="field-group">
            <label class="form-label width-md"></label>
            <button class="btn btn-primary counterBalanceBtn" id="confirmRemarkBtn" onclick="confirmRemark();" disabled>Confirm</button>
            <button class="btn btn-default counterBalanceBtn" id="resetCounterBtn" onclick="resetCounter();" disabled>Reset</button>
        </div>
    </div>
</div>
<script type="text/javascript">
    function confirmRemark() {
        var SelectKioskID = document.getElementById("selectKioskId");
        var SelectBinID = document.getElementById("selectCollectionBinID");
        var InputReconDate = document.getElementById("inputReconDate");
        var SelectRemark = document.getElementById("selectRemark");
        var InputComment = document.getElementById("inputComments");
        var MachID = SelectKioskID.value.trim();
        var BinID = SelectBinID.value.trim();
        var ReconDate = InputReconDate.value.trim();
        var Remark = SelectRemark.value.trim();
        var Comment = InputComment.value.trim();

        if (MachID === "") {
            alert("Invalid Kiosk ID.");
            return false;
        } else if (BinID === "") {
            alert("Invalid Bin ID.");
            return false;
        } else if (ReconDate === "") {
            alert("Invalid Date.");
            return false;
        } else if (Remark === "") {
            alert("Please choose a Remark.");
            SelectRemark.focus();
            return false;
        } else if (Comment === "") {
            alert("Please enter comment.");
            InputComment.focus();
            return false;
        }

        var FinalRemark = Remark + "\n" + Comment;

        $.ajax({
            type: "POST",
            contentType: "application/json; charset=utf-8",
            url: "/views/reconciliation/chequeCounterBalancing/main.aspx/UpdateRemark",
            data: JSON.stringify({ BinID: BinID, Date: ReconDate, Remark: FinalRemark }),
            dataType: "json",
            beforeSend: function () {
                LockButton();
            },
            success: function (data) {

                var status = data.d.Status;
                var msg = data.d.Message;

                UnlockButton();

                if (!status) {
                    alert(msg);
                    return false;
                }

                alert("Successful.");
                resetCounter();
            },
            error: function (error) {
                alert("Error " + error.status + ". Unable to load Bin.");
                $(elem).prop('disabled', false);
            }
        });
    }
    function LockSearchCriteria() {
        $('#selectKioskId').prop('disabled', true);
        $('#selectCollectionBinID').prop('disabled', true);
        $('#inputReconDate').prop('disabled', true);
    }
    function UnlockSearchCriteria() {
        $('#selectKioskId').prop('disabled', false);
        $('#selectCollectionBinID').prop('disabled', false);
        $('#inputReconDate').prop('disabled', false);
    }
    function LockConfirmationDiv() {
        $('#selectRemark').prop('disabled', true);
        $('#inputComments').prop('disabled', true);
        $('#confirmRemarkBtn,#resetCounterBtn').prop('disabled', true);
    }
    function UnlockConfirmationDiv() {
        $('#selectRemark').prop('disabled', false);
        $('#inputComments').prop('disabled', false);
        $('#confirmRemarkBtn,#resetCounterBtn').prop('disabled', false);
    }
    function LockButton() {
        $('.counterBalanceBtn').prop('disabled', true);
    }
    function UnlockButton() {
        $('.counterBalanceBtn').prop('disabled', false);
    }
    function LockBinInput() {
        $('.input-bin').prop('disabled', true);
    }
    function UnlockBinInput() {
        $('.input-bin').prop('disabled', false);
    }
    function resetCounter() {
        UnlockSearchCriteria();
        $('#selectKioskId option:first-child').prop('selected', true);
        $('#selectCollectionBinID option:first-child').prop('selected', true);
        $('#inputReconDate').val("");
        $('.input-bin').removeClass('is-successful is-failed');
        $('.input-bin').val(0);

        $('#selectRemark option:first-child').prop('selected', true);
        $('#inputComments').val("");
        LockConfirmationDiv();
    }
    function validateCounter() {
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

        var NumberRegex = /^[0-9]+$/;
        var InputBin1 = document.getElementById("inputBin1");
        var InputBin2 = document.getElementById("inputBin2");
        var InputBin3 = document.getElementById("inputBin3");
        var InputBin4 = document.getElementById("inputBin4");
        var Bin1 = InputBin1.value.trim();
        var Bin2 = InputBin2.value.trim();
        var Bin3 = InputBin3.value.trim();
        var Bin4 = InputBin4.value.trim();

        if (Bin1 === "") {
            alert("Please enter the number of cheque(s) obtained from Collection Bin 1.");
            InputBin1.focus();
            return false;
        } else if (Bin2 === "") {
            alert("Please enter the number of cheque(s) obtained from Collection Bin 2.");
            InputBin2.focus();
            return false;
        } else if (Bin3 === "") {
            alert("Please enter the number of cheque(s) obtained from Collection Bin 3.");
            InputBin3.focus();
            return false;
        } else if (Bin4 === "") {
            alert("Please enter the number of cheque(s) obtained from Collection Bin 4.");
            InputBin4.focus();
            return false;
        }

        if (!NumberRegex.test(Bin1)) {
            alert("Invalid. Please enter number only.");
            InputBin1.value = "";
            InputBin1.focus();
            return false;
        } else if (!NumberRegex.test(Bin2)) {
            alert("Invalid. Please enter number only.");
            InputBin2.value = "";
            InputBin2.focus();
            return false;
        } else if (!NumberRegex.test(Bin3)) {
            alert("Invalid. Please enter number only.");
            InputBin3.value = "";
            InputBin3.focus();
            return false;
        } else if (!NumberRegex.test(Bin4)) {
            alert("Invalid. Please enter number only.");
            InputBin4.value = "";
            InputBin4.focus();
            return false;
        }

        $.ajax({
            type: "POST",
            contentType: "application/json; charset=utf-8",
            url: "/views/reconciliation/chequeCounterBalancing/main.aspx/GetBinStatus",
            data: JSON.stringify({ BinID: BinID, Date: ReconDate }),
            dataType: "json",
            beforeSend: function () {
                LockButton();
                LockBinInput();
                LockSearchCriteria();
            },
            success: function (data) {

                var status = data.d.Status;
                var msg = data.d.Message;

                if (!status) {
                    alert(msg);
                    UnlockBinInput();
                    UnlockSearchCriteria();             
                    return false;
                }

                var BinStatus = data.d.Object;
                var str = "";

                if (BinStatus === null) {
                    alert("Record not found.");
                    UnlockBinInput();
                    UnlockSearchCriteria();
                    $('#validateCounterBtn').prop('disabled', false);
                    return false;
                } else {
                    $('.input-bin').removeClass('is-successful is-failed');
                    UnlockButton();
                    UnlockBinInput();

                    if (parseInt(Bin1) === BinStatus.Bin1) {
                        $(InputBin1).addClass('is-successful');
                    } else {
                        $(InputBin1).addClass('is-failed');
                    }
                    if (parseInt(Bin2) === BinStatus.Bin2) {
                        $(InputBin2).addClass('is-successful');
                    } else {
                        $(InputBin2).addClass('is-failed');
                    }
                    if (parseInt(Bin3) === BinStatus.Bin3) {
                        $(InputBin3).addClass('is-successful');
                    } else {
                        $(InputBin3).addClass('is-failed');
                    }
                    if (parseInt(Bin4) === BinStatus.Bin4) {
                        $(InputBin4).addClass('is-successful');
                    } else {
                        $(InputBin4).addClass('is-failed');
                    }
                    UnlockConfirmationDiv();
                    LockSearchCriteria();
                }
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
            url: "/views/reconciliation/chequeCounterBalancing/main.aspx/GetCollectionBin",
            data: JSON.stringify({ MachID: MachID }),
            dataType: "json",
            beforeSend: function () {
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
        }).always(function () {
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
