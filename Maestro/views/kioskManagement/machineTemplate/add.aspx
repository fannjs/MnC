<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="add.aspx.cs" Inherits="Maestro.views.kioskManagement.machineTemplate.add" %>

<script type="text/javascript">
    $('#cancel-button').click(function () {
        showMainPage();
    });
    
    function addNewMachine() {
        var MachineType = $('#inputMachineType').val();
        var MachineModel = $('#inputMachineModel').val();
        var ImgMach = $('#imgMach').attr('src');        

        var para = {
            'MachineType': MachineType,
            'MachineModel': MachineModel,
            'ImgMach': ImgMach
        };

        if (MachineType == "" || MachineModel == "") {
            alert("Empty fields are not allow!");
            return;
        }
        if(ImgMach == "" || ImgMach == undefined){
            alert("Empty fields are not allow!");
            return;
        }

        var objurl =
        {
            'url': 'addNewMachine',
            'data': para,
        };

        
        lockAllButton();
        __JSONWEBSERVICE.getServices(objurl, addNewMachineSuccess, addNewMachineError);
    }

    function addNewMachineSuccess(msg) {

        var status = msg.d.Status;
        var msg = msg.d.Message;
        
        unlockAllButton();

        if (!status) {
            alert(msg);
            return false;
        } 

        alert("Successful. Your action has been sent to checker for approval.");
        showMainPage();
    };

    function addNewMachineError(msg) {
        alert('Error: Failed to add machine!');
        unlockAllButton();
    };

    function lockAllButton(){
        $('#btnUpdateMachine').prop('disabled',true);
        $('#cancel-button').prop('disabled',true);
    }

    function unlockAllButton(){
        $('#btnUpdateMachine').prop('disabled',false);
        $('#cancel-button').prop('disabled',false);
    }

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

    
    /*
    $('#btnSavEAMT').click(function () {

        var machinetypemock = {
            'M_MACH_TYPE': $('#inputMachineType').val(),
            'M_MACH_MODEL': $('#inputMachineModel').val(),
            'M_MACH_IMGPATH': $('#imgMach').attr('src')
        }

        fnWebAPI({
            action: 'POST',
            controller: 'machinetype',
            obj: machinetypemock,
            actionName: 'Add',//add, update,delete
            onSuccess: function () {
                console.log('nice');
                getMachineTypes();
            }

        });
    });

    var fnWebAPI = function (options) {
        jQuery.support.cors = true;

        //note by roy: we could ignore the age prop
        var settings = $.extend({
            // These are the defaults.
            pk: '',
            action: '',
            controller: '',
            obj: {},
            actionName: 'Update',//add, update,delete
            onSuccess: function () { },
            onError: function () { },
        }, options);

        var execute = true;
        if (settings.action.toUpperCase() == 'DELETE') {
            var toDelete = confirm('Are you sure to ' + settings.action.toLowerCase() + '?');

            execute = toDelete;
        }

        if (execute) {

            $.ajax({
                url: '/api/' + settings.controller + settings.pk,
                type: settings.action,
                data: JSON.stringify(settings.obj),
                contentType: "application/json;charset=utf-8",
                success: function (data) {
                    alert('Details ' + settings.actionName + 'ed Successfully');
                    settings.onSuccess.apply(this);
                },
                error: function (x, y, z) {
                    alert('Unable to ' + settings.actionName + ' for the Given ID');
                    console.log(x);
                    console.log(y);
                    console.log(z);
                    settings.onError();
                }
            });
        } 
    };*/

</script>
<div>
    <form>
        <fieldset>
            <div class="well-div">
                <legend>Insert</legend>
                <div class="field-group">
                    <label for="inputMachineType" class="form-label width-md">Kiosk Type</label>
                     <input type="text" class="input-field width-xl" id="inputMachineType" placeholder="Machine Type">
                </div>
                <div class="field-group">
                    <label for="inputMachineModel" class="form-label width-md">Kiosk Model</label>
                    <input type="text" class="input-field width-xl" id="inputMachineModel" placeholder="Machine Model">
                </div>
                <div class="field-group">
                    <label for="inputMachineImage" class="form-label width-md">Kiosk Image</label>
                    <div class="imgwrapper">
                        <img id="imgMach" class="fit" src="" runat="server"/>
                        <input type="file" id="fBrowseImage" class="fit" name="file" />
                    </div>
                </div>
                <div class="block-md"></div>
                <div class="field-group">
                    <input id="btnUpdateMachine" type="button" class="btn btn-primary" value="Add" onclick="addNewMachine()" />
                    <button type="button" id="cancel-button" class="btn btn-default">Cancel</button>
                </div>
            </div>
        </fieldset>
    </form>
</div>
