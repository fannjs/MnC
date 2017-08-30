<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="configureSMS.aspx.cs" Inherits="Maestro.views.administration.statusCodeEscalation.configureSMS" %>
<script type="text/javascript">
    $(document).ready(function () {

        $.ajax({
            type: "POST",
            url: "/views/administration/statusCodeEscalation/configureSMS.aspx/GetSMSConfiguration",
            contentType: "application/json; charset=utf-8",
            data: "{}",
            dataType: "json",
            success: function (data) {

                var status = data.d.Status;
                var msg = data.d.Message;

                if (!status) {
                    alert(msg);
                    return false;
                }

                var SMSConfig = data.d.Object;
                if (SMSConfig !== null) {

                    var ComPortList = document.getElementById("selectCOMPort").getElementsByTagName("option");
                    var BaudRateList = document.getElementById("selectBaudRate").getElementsByTagName("option");
                    var TimeoutList = document.getElementById("selectTimeout").getElementsByTagName("option");
                    var i = 0;
                    var found = false;

                    for (i = 0; i < ComPortList.length, ComPort = ComPortList[i]; i++) {

                        if (ComPort.value === SMSConfig.ComPort) {
                            ComPort.selected = true;
                            found = true;
                            break;
                        } else {
                            continue;
                        }
                    }

                    if (!found) {
                        $('#selectCOMPort').append('<option selected>' + SMSConfig.ComPort + '</option>');
                    } 

                    for (i = 0; i < BaudRateList.length, BaudRate = BaudRateList[i]; i++) {

                        if (BaudRate.value === SMSConfig.BaudRate) {
                            BaudRate.selected = true;
                            break;
                        } else {
                            continue;
                        }
                    }

                    for (i = 0; i < TimeoutList.length, Timeout = TimeoutList[i]; i++) {

                        if (Timeout.value === SMSConfig.DeviceTimeout) {
                            Timeout.selected = true;
                            break;
                        } else {
                            continue;
                        }
                    }
                    $('#configureEmailMainDiv').find('input[name="smsContent"]').val(SMSConfig.SMSContent);
                }

            },
            error: function (error) {
                alert("Error " + error.status + ". Unable to load email setting.");
            }
        });
    });

    function updateSMSSetting() {

        var requiredFields = document.getElementById("configureSMSMainDiv").querySelectorAll(".required-field");

        for (var i = 0; i < requiredFields.length; i++) {
            var value = requiredFields[i].value.trim();

            if (value === "" || value === null || value === undefined) {
                alert("Please do not leave blank.");
                requiredFields[i].focus();
                return false;
            }
        }

        var SMSConfig = {};

        SMSConfig["ComPort"] = $('#selectCOMPort').val();
        SMSConfig["BaudRate"] = $('#selectBaudRate').val();
        SMSConfig["Timeout"] = $('#selectTimeout').val();
        SMSConfig["SMSContent"] = $('textarea[name="smsContent"]').val();

        $.ajax({
            type: "POST",
            url: "/views/administration/statusCodeEscalation/configureSMS.aspx/UpdateSMSConfiguration",
            contentType: "application/json; charset=utf-8",
            data: JSON.stringify({ SMSConfig: SMSConfig }),
            dataType: "json",
            beforeSend: function () {
                $('.configureSMSBtn').prop('disabled', true);
            },
            success: function (data) {

                var status = data.d.Status;
                var msg = data.d.Message;

                $('.configureSMSBtn').prop('disabled', false);

                if (!status) {
                    alert(msg);
                    return false;
                }

                alert("Successful. Your action has been sent to Checker for approval.");
                showMainPage();
            },
            error: function (error) {
                alert("Error " + error.status + ". Unable to update email setting.");
                $('.configureSMSBtn').prop('disabled', false);
            }
        });
    }
</script>
<div>
    <div id="configureSMSMainDiv" class="well-div">
        <legend>SMS Setting</legend>
        <div class="field-group">
            <label class="form-label width-lg">Device COM Port</label>
            <select id="selectCOMPort" class="input-field width-lg required-field" runat="server">
                <option disabled selected value=""> - Please Select - </option>
            </select>
        </div>
        <div class="field-group">
            <label class="form-label width-lg">Baud Rate</label>
            <select id="selectBaudRate" class="input-field width-lg required-field">
                <option disabled selected value=""> - Please Select - </option>
                <option value="9600">9600</option>
                <option value="19200">19200</option>
                <option value="38400">38400</option>
                <option value="57600">57600</option>
                <option value="115200">115200</option>
            </select>
        </div>
        <div class="field-group">
            <label class="form-label width-lg">Timeout(ms)</label>
            <select id="selectTimeout" class="input-field width-lg required-field">
                <option disabled selected value=""> - Please Select - </option>
                <option value="150">150</option>
                <option value="300">300</option>
                <option value="600">600</option>
                <option value="900">900</option>
                <option value="1200">1200</option>
                <option value="1500">1500</option>
                <option value="2000">2000</option>
            </select>
        </div>
        <hr style="border-top: 1px dashed #CCC;" />
        <div class="field-group">
            <label class="form-label width-lg">SMS Content</label>
            <textarea class="input-field" name="smsContent" style="resize:none;width:500px;" placeholder="SMS Content" rows="6"></textarea>
        </div>
        <hr />
        <div class="field-group">
            <label class="form-label width-lg"></label>
            <button class="btn btn-primary configureSMSBtn" onclick="updateSMSSetting();">Update</button>
            <button class="btn btn-default configureSMSBtn" onclick="showMainPage();">Cancel</button>
        </div>
    </div>
</div>
