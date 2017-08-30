<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="edit.aspx.cs" Inherits="Maestro.views.kioskManagement.machineTemplate.edit" %>

<script type="text/javascript">
    $('#cancel-button').click(function () {
        showMainPage();
    });

    function lockButton(){
        $('#btnUpdateMachine').prop('disabled',true);
        $('#cancel-button').prop('disabled',true);
    }
    function unlockButton(){
        $('#btnUpdateMachine').prop('disabled',false);
        $('#cancel-button').prop('disabled',false);
    }

    $('#btnUpdateMachine').click(function () {
        
        var OriMType = $('#hidOriMType').val();
        var OriMModel = $('#hidOriMModel').val();
        var MachineType = $('#inputMachineType').val();
        var MachineModel = $('#inputMachineModel').val();
        var ImgMach = $('#imgMach').attr('src');
        var para = {'OriMType':OriMType, 'OriMModel':OriMModel, 'MachineType': MachineType, 'MachineModel': MachineModel, 'ImgMach': ImgMach };
       
        var objurl =
        {
            'url': 'updateMachine',
            'data': para,
        };

        lockButton();
        __JSONWEBSERVICE.getServices(objurl, updateMachineSuccess, updateMachineError);
        
    });
    function updateMachineSuccess(msg) {
        
        var status = msg.d.Status;
        var msg = msg.d.Message;

        unlockButton();

        if(!status){
            alert(msg);
            return false;
        }

        alert("Successful. Your action has been sent to checker for approval.");
        showMainPage();
    };

    function updateMachineError(data) {
        unlockButton();
        alert('Error: Failed to update Kiosk!');
    };

    /*html5 js to handle local fiole*/
    function handleFileSelect(evt) {
        var files = evt.target.files; // FileList object

        // Loop through the FileList and render image files as thumbnails.
        for (var i = 0, f; f = files[i]; i++) {

            // Only process image files.
            if (!f.type.match('image.*')) {
                continue;
            }

            var reader = new FileReader();

            // Closure to capture the file information.
            reader.onload = (function (theFile) {
                return function (e) {
                    // Render thumbnail
                    document.getElementById('imgMach').src = e.target.result;
                };
            })(f);

            // Read in the image file as a data URL.
            reader.readAsDataURL(f);
        }
    }

    document.getElementById('fBrowseImage').addEventListener('change', handleFileSelect, false);

</script>
<div>
    <form>
        <fieldset>
            <div class="well-div">
                <legend>Edit</legend>
                <div class="field-group">
                    <label for="inputMachineType" class="form-label width-md">Kiosk Type</label>
                    <input type="text" runat="server" class="input-field width-xl" id="inputMachineType" placeholder="Kiosk Type" disabled>
                </div>
                <div class="field-group">
                    <label for="inputMachineModel" class="form-label width-md">Kiosk Model</label>
                    <input type="text" runat="server" class="input-field width-xl" id="inputMachineModel" placeholder="Kiosk Model" disabled>
                </div>
                <div class="field-group">
                    <label for="inputMachineImage" class="form-label width-md">Kiosk Image</label>
                    <div class="imgwrapper thumb">
                        <img id="imgMach" class="fit" runat="server">
                        <input id="fBrowseImage" class="fit" type="file" name="file">
                    </div>
                </div>
                <div class="block-md"></div>
                <div class="field-group">
                    <input type="hidden" runat="server" class=" form-control" id="hidOriMType" />
                    <input type="hidden" runat="server" class=" form-control" id="hidOriMModel" />
                    <%--<button type="button" class="btn btn-default" onclick="updateMachine()">Update</button>--%>
                    <input id="btnUpdateMachine" type="button" class="btn btn-primary" value="Update"/>
                    <button type="button" id="cancel-button" class="btn btn-default">Cancel</button>
                </div>
            </div>
        </fieldset>
    </form>
</div>
