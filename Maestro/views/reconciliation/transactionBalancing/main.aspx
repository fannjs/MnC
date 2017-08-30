<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="main.aspx.cs" Inherits="Maestro.views.reconciliation.transactionBalancing.main" %>

<link rel="Stylesheet" href="../../../components/bootstrap.datepicker/css/datepicker.css" />
<script src="../../../components/bootstrap.datepicker/js/bootstrap-datepicker.js"></script>
<div id="cheque-counter-main-div">
    <div style="display:block;">
        <h1 class="h1">Reconcilation Information</h1>
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
        <h1 class="h1">Transaction Information</h1>
        <hr style="margin:10px 0px;" />
        <div class="block-xs"></div>
        <h5 style="color:#999;">Please enter the number of cheque(s) and transaction(s) stated on the printed journal.</h5>
        <div class="field-group">
            <label for="inputCheque" class="form-label width-lg">Cheque Count</label>
            <input type="text" class="input-field width-xs input-chq-trans" id="inputCheque" value="0" />
        </div>
        <div class="field-group">
            <label for="inputTransaction" class="form-label width-lg">Transaction Count</label>
            <input type="text" class="input-field width-xs input-chq-trans" id="inputTransaction" value="0" />
        </div>
        <div class="block-xs"></div>
        <div class="field-group">
            <label class="form-label width-lg"></label>
            <button class="btn btn-primary transBalanceBtn" id="validateTransBtn" onclick="validateTransaction();">Validate</button>
        </div>
        <div class="block-md"></div>
        <h1 class="h1">Confirmation</h1>
        <hr style="margin:10px 0px;" />
        <div class="block-xs"></div>
        <div class="field-group">
            <label for="selectRemark" class="form-label width-md">Remark</label>
            <select class="input-field width-xl" id="selectRemark" disabled>
                <option value="" selected disabled> - Please select - </option>
                <option>Cheque and Transaction count MATCH with Data received</option>
                <option>Cheque and Transaction count DOES NOT MATCH with Data received</option>
                <option>Cheque count MATCH with Data received</option>
                <option>Transaction count MATCH with Data received</option>
            </select>
        </div>
        <div class="field-group">
            <label for="inputComments" class="form-label width-md">Comments</label>
            <textarea class="input-field width-xl" rows="4" style="resize:none;" id="inputComments" disabled></textarea>
        </div>
        <div class="field-group">
            <label class="form-label width-md"></label>
            <button class="btn btn-primary transBalanceBtn" id="confirmRemarkBtn" onclick="confirmRemark();" disabled>Confirm</button>
            <button class="btn btn-default transBalanceBtn" id="resetBalancingBtn" onclick="resetBalancing();" disabled>Reset</button>
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
            url: "/views/reconciliation/transactionBalancing/main.aspx/UpdateRemark",
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
                resetBalancing();
            },
            error: function (error) {
                alert("Error " + error.status + ". Unable to update.");
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
        $('#confirmRemarkBtn,#resetBalancingBtn').prop('disabled', true);
    }
    function UnlockConfirmationDiv() {
        $('#selectRemark').prop('disabled', false);
        $('#inputComments').prop('disabled', false);
        $('#confirmRemarkBtn,#resetBalancingBtn').prop('disabled', false);
    }
    function LockButton() {
        $('.transBalanceBtn').prop('disabled', true);
    }
    function UnlockButton() {
        $('.transBalanceBtn').prop('disabled', false);
    }
    function LockTransInput() {
        $('.input-chq-trans').prop('disabled', true);
    }
    function UnlockTransInput() {
        $('.input-chq-trans').prop('disabled', false);
    }
    function resetBalancing() {
        UnlockSearchCriteria();
        $('#selectKioskId option:first-child').prop('selected', true);
        $('#selectCollectionBinID option:first-child').prop('selected', true);
        $('#inputReconDate').val("");
        $('.input-chq-trans').removeClass('is-successful is-failed');
        $('.input-chq-trans').val(0);

        $('#selectRemark option:first-child').prop('selected', true);
        $('#inputComments').val("");
        LockConfirmationDiv();
    }
    function validateTransaction() {
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
        var InputCheque = document.getElementById("inputCheque");
        var InputTransaction = document.getElementById("inputTransaction");
        var ChequeCount = InputCheque.value.trim();
        var TransactionCount = InputTransaction.value.trim();

        if (ChequeCount === "") {
            alert("Please enter the number of cheque(s) stated on the printed journal.");
            InputCheque.focus();
            return false;
        } else if (TransactionCount === "") {
            alert("Please enter the number of transaction(s) stated on the printed journal.");
            InputTransaction.focus();
            return false;
        }

        if (!NumberRegex.test(ChequeCount)) {
            alert("Invalid. Please enter number only.");
            InputCheque.value = "";
            InputCheque.focus();
            return false;
        } else if (!NumberRegex.test(TransactionCount)) {
            alert("Invalid. Please enter number only.");
            InputTransaction.value = "";
            InputTransaction.focus();
            return false;
        }

        $.ajax({
            type: "POST",
            contentType: "application/json; charset=utf-8",
            url: "/views/reconciliation/transactionBalancing/main.aspx/GetTransaction",
            data: JSON.stringify({ BinID: BinID, Date: ReconDate }),
            dataType: "json",
            beforeSend: function () {
                LockButton();
                LockSearchCriteria();
                LockTransInput();
            },
            success: function (data) {

                var status = data.d.Status;
                var msg = data.d.Message;

                if (!status) {
                    alert(msg);
                    UnlockSearchCriteria();
                    UnlockTransInput();
                    return false;
                }

                var Trans = data.d.Object;
                var str = "";

                if (Trans === null) {
                    alert("Record not found.");
                    $('#validateTransBtn').prop('disabled', false);
                    UnlockSearchCriteria();
                    UnlockTransInput();
                    return false;
                } else {
                    $('.input-chq-trans').removeClass('is-successful is-failed');
                    UnlockButton();
                    UnlockTransInput();

                    if (parseInt(ChequeCount) === Trans.CCount) {
                        $(InputCheque).addClass('is-successful');
                    } else {
                        $(InputCheque).addClass('is-failed');
                    }
                    if (parseInt(TransactionCount) === Trans.TCount) {
                        $(InputTransaction).addClass('is-successful');
                    } else {
                        $(InputTransaction).addClass('is-failed');
                    }

                    LockSearchCriteria();
                    UnlockConfirmationDiv();
                }
            },
            error: function (error) {
                alert("Error " + error.status + ". Unable to load Transaction detail.");
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