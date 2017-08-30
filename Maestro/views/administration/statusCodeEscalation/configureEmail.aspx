<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="configureEmail.aspx.cs" Inherits="Maestro.views.administration.statusCodeEscalation.configureEmail" %>
<script>
    $(document).ready(function () {

        $.ajax({
            type: "POST",
            url: "/views/administration/statusCodeEscalation/configureEmail.aspx/GetEmailConfiguration",
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

                var EmailSetting = data.d.Object;
                console.log(EmailSetting);
                if (EmailSetting !== null) {

                    $('#configureEmailMainDiv').find('input[name="senderEmail"]').val(EmailSetting.SenderEmail);
                    $('#configureEmailMainDiv').find('input[name="serverAddress"]').val(EmailSetting.ServerAddress);
                    $('#configureEmailMainDiv').find('input[name="portNo"]').val(EmailSetting.PortNo);
                    $('#configureEmailMainDiv').find('input[name="mailUser"]').val(EmailSetting.MailUsername);
                    $('#configureEmailMainDiv').find('input[name="mailPassword"]').val(EmailSetting.MailPassword);

                    (EmailSetting.SSL) ? $('#configureEmailMainDiv').find('select[name="sslRequired"] > option[value="1"]').prop('selected', true)
                     : $('#configureEmailMainDiv').find('select[name="sslRequired"] > option[value="0"]').prop('selected', true);

                    $('#configureEmailMainDiv').find('input[name="mailSubject"]').val(EmailSetting.EmailSubject);
                    $('#configureEmailMainDiv').find('textarea[name="mailContent"]').val(EmailSetting.EmailContent);

                }

            },
            error: function (error) {
                alert("Error " + error.status + ". Unable to load email setting.");
            }
        });
    });

    function updateEmailSetting() {

        var requiredFields = document.getElementById("configureEmailMainDiv").querySelectorAll(".required-field");

        for (var i = 0; i < requiredFields.length; i++) {
            var value = requiredFields[i].value.trim();

            if (value === "" || value === null || value === undefined) {
                alert("Please do not leave blank.");
                requiredFields[i].focus();
                return false;
            }
        }

        var EmailSetting = {};

        var textFields = document.getElementById("configureEmailMainDiv").querySelectorAll(".input-field");

        for (var j = 0; j < textFields.length; j++) {
            var name = textFields[j].getAttribute("name");
            var value = textFields[j].value;

            EmailSetting[name] = value;
        }

        $.ajax({
            type: "POST",
            url: "/views/administration/statusCodeEscalation/configureEmail.aspx/UpdateEmailSetting",
            contentType: "application/json; charset=utf-8",
            data: JSON.stringify({ emailSetting: EmailSetting }),
            dataType: "json",
            beforeSend: function () {
                $('.configureEmailBtn').prop('disabled', true);
            },
            success: function (data) {

                var status = data.d.Status;
                var msg = data.d.Message;

                $('.configureEmailBtn').prop('disabled', false);

                if (!status) {
                    alert(msg);
                    return false;
                }

                alert("Successful. Your action has been sent to Checker for approval.");
                showMainPage();
            },
            error: function (error) {
                alert("Error " + error.status + ". Unable to update email setting.");
                $('.configureEmailBtn').prop('disabled', false);
            }
        });
    }
</script>
<div>
    <div id="configureEmailMainDiv" class="well-div">
        <legend>SMTP - Outgoing Setting</legend>
        <div class="field-group">
            <label class="form-label width-lg">Sender Email Address</label>
            <input type="text" class="input-field width-xl required-field" name="senderEmail" placeholder="Sender Email Address">
        </div>
        <div class="field-group">
            <label class="form-label width-lg">Mail Server Address</label>
            <input type="text" class="input-field width-xl required-field" name="serverAddress" placeholder="Mail Server Address">
            <label class="form-label width-xs">Port No.</label>
            <input type="text" class="input-field width-xs required-field" name="portNo" placeholder="Port No.">
        </div>
        <div class="field-group">
            <label class="form-label width-lg">Username</label>
            <input type="text" class="input-field width-xl required-field"  name="mailUser" placeholder="Username">
        </div>
        <div class="field-group">
            <label class="form-label width-lg">Password</label>
            <input type="password" class="input-field width-xl required-field" name="mailPassword" placeholder="Password">
        </div>
        <div class="field-group">
            <label class="form-label width-lg">SSL Required</label>
            <select name="sslRequired" class="input-field width-md required-field">
                <option disabled selected value=""> - Please Select - </option>
                <option value="1">Yes</option>
                <option value="0">No</option>
            </select>
        </div>
        <hr style="border-top: 1px dashed #CCC;" />
        <div class="field-group">
            <label class="form-label width-lg">Email Subject</label>
            <input type="text" class="input-field width-xl" name="mailSubject" placeholder="Email Subject">
        </div>
        <div class="field-group">
            <label class="form-label width-lg">Email Content</label>
            <textarea class="input-field" name="mailContent" style="resize:none;width:500px;" placeholder="Email Content" rows="6"></textarea>
        </div>
        <hr />
        <div class="field-group">
            <label class="form-label width-lg"></label>
            <button class="btn btn-primary configureEmailBtn" onclick="updateEmailSetting();">Update</button>
            <button class="btn btn-default configureEmailBtn" onclick="showMainPage();">Cancel</button>
        </div>
    </div>
</div>
