<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="main.aspx.cs" Inherits="Maestro.views.kioskMaintenance.advertisement.main" %>

<link rel="Stylesheet" href="../../../assets/styles/advertisement.css" />
<script type="text/javascript">
    var filesToSend = [];
    //Multiple upload, reference http://www.html5rocks.com/en/tutorials/file/dndfiles/
    function handleFileSelect(evt) { 
        
        var files = evt.target.files; // FileList object
        filesToSend = [];
        //addMultiFiles(files);

        // Loop through the FileList and render image files as thumbnails.
        for (var i = 0, f; f = files[i]; i++) {

            // Only process these files
            if (!f.type.match('image.*') && !f.type.match('video.*')) {
                continue;
            }
            else{

                var reader = new FileReader();

                // Closure to capture the file information.
                reader.onload = (function (theFile) {
                    return function (e) {
                        // Render thumbnail.

                        var aFile = {
                            name: theFile.name,
                            type: theFile.type,
                            src: e.target.result,
                            size: theFile.size,
                            imgSizeWidth: 800,
                            imgSizeHeight: 600,
                            thumbWidth: 150,
                            thumbHeight: 120
                        };

                        filesToSend.push(aFile);
                        /*
                        num++
                        var images = '<div class="image-wrapper image-uploaded"><div class="image-wrapper-header">' + theFile.name + '</div>'
                            +'<div class="image-wrapper-body"><img class="image-div" src="' + e.target.result + '" title="' + theFile.name + '" />'
                                    + '</div><div class="image-wrapper-footer"><i class="fa fa-plus" style="color:green;"></i><i class="fa fa-times" style="color:red;"></i></div></div>'

                        $('#advertisement-uploaded').append(images);
                        */
                    };
                })(f);

                reader.onloadend = function(e){
                    
                    if(files.length > 1){
                        $('#total-uploaded-files').val(files.length + " files"); 
                    }
                    else{
                        $('#total-uploaded-files').val(files.length + " file"); 
                    }
                };
                // Read in the image file as a data URL.
                reader.readAsDataURL(f);
            }
        }     
    }

    function uploadAdv() {
        $('#upload-btn').click(function () {

            /*
            for(var i = 0; i < filesToSend.length; i++){
                
                addNewAdv(filesToSend[i].name, filesToSend[i].src, filesToSend[i].type);
            }
            */

            uploadNewAdvertisement(filesToSend);
        });
    }

    //Gan added
    function uploadNewAdvertisement(filesArray){
        $.ajax({
            type: "POST",
            contentType: "application/json; charset=utf-8",
            url: "/views/kioskMaintenance/advertisement/main.aspx/UploadAdvertisement",
            data: JSON.stringify({ files: filesArray }),
            dataType: "json",
            success: function (data) {
                addNewAdvSuccess(data);
                $('#total-uploaded-files').val(""); 
            },
            error: function (error) {
                addNewAdvError(error);
            }
        });
    }

    function addNewAdv(fileName, imgsrc, filetype) {
        //alert("fileName = " + fileName + ", imgsrc = " +imgsrc);
        var FileName = fileName;
        var ImgSrc = imgsrc;
        //var para = "{MachineType:'" + MachineType + "', MachineModel:'" + MachineModel + "', ImgMach:'" + ImgMach + "'}";
        var para = {
            'FileName': FileName,
            'ImgSrc': ImgSrc,
            'FileType': filetype
        };

        var objurl =
        {
            'url': 'uploadAdv',
            'data': para,
        };

        __JSONWEBSERVICE.getServices(objurl, addNewAdvSuccess, addNewAdvError);
    }

    function addNewAdvSuccess(msg) {
        //alert("msg = " + msg.d);
        if (msg.d == 1) {
            alert("Records added successfully!");
            __JSONWEBSERVICE.getServices("getAdDetails", getAdDetailsSuccess, getAdDetailsError);
        } else if (msg.d == -1) {
            alert("Failed to add due to record exists!");
        } else {
            alert("Failed to insert due to database error!");
        }
    };

    function addNewAdvError(msg) {
        alert('Error: Failed to upload advertisement!');
    };

    function addIntoSeq() {
        $('#advertisement-maintenance-main-div').on('click', '.fa-plus', function () {
            
            var advId = $(this).closest('.image-wrapper').attr('data-advID');
            var advName = $(this).closest('.image-wrapper').attr('data-advName');  
            var advSrc = $(this).closest('.image-wrapper').find('.image-div').attr('src');     
            var fileType = $(this).closest('.image-wrapper').find('.image-div').get(0).tagName;

            var li = document.getElementById("advertisement-seq").getElementsByTagName("li");

            var proceed = true;

            if($('#seq-empty-msg').length == 1){
                $('#advertisement-seq').empty();
                $('#advertisement-seq').append('<ul></ul');
            }

            for(var i = 0; i < li.length; i++){
                
                var seqAdvId = $(li[i]).children('.seq-img').attr('data-advID');

                if(advId == seqAdvId){
                    alert("It already added in the sequence.");
                    proceed = false;
                    break;
                }
                else{
                    continue;
                }
            }

            if(proceed){

                var str = "";

                str = str + '<li><div class="image-wrapper seq-img" data-advID="' + advId + '" data-advName="' + advName + '" title="'+advName+'">';
                str = str + '<div class="image-wrapper-header"><span class="remove-seq-img"><i class="fa fa-minus-circle"></i></span>' + advName + '</div>'
                str = str + '<div class="image-wrapper-body">'
                
                if(fileType == "IMG"){
                    str = str + '<img class="image-div" src="' + advSrc + '" title="' + advName + '" />';
                }
                else if(fileType == "VIDEO"){
                    str = str + '<video controls class="image-div" src="' + advSrc + '" title="' + advName + '" />';
                }

                str = str + '</div></div></li>';

                $('#advertisement-seq > ul').append(str);
                                        
                $('#advertisement-seq > ul').sortable({
                    containment: "#advertisement-seq-div"
                });
            }
        });
    }
    
    function removeSeqImg(){
        $('#advertisement-maintenance-main-div').on('click', '.remove-seq-img', function(event){

            $(this).closest('li').remove();

            if($('#advertisement-seq > ul li').length == 0){
                $('#advertisement-seq').html('<span id="seq-empty-msg" style="color:#999;">Nothing in the sequence.</span><div class="block-xs"></div>');
            }
        });
    }

    function getAdDetailsSuccess(data) {
        $('#advertisement-uploaded').empty();

        if(data.d.length == 0){
            $('#advertisement-uploaded').html('<span id="seq-empty-msg" style="color:#999;">No file uploaded yet.</span><div class="block-xs"></div>');
        }
        else
        {
            for (var i = 0; i < data.d.length; i++) {
                if (data.d[i].ADV_TYPE == "image") {
                    $('#advertisement-uploaded').append('<div class="image-wrapper image-uploaded" data-advID="' + data.d[i].ADV_ID + '" data-advName="' + data.d[i].ADV_FILENAME + '"><div class="image-wrapper-header" title="' + data.d[i].ADV_FILENAME + '">' + data.d[i].ADV_FILENAME + '</div>'
                                    + '<div class="image-wrapper-body"><img class="image-div" src="../views/kioskMaintenance/advertisement/Adv/Thumbnail/' + data.d[i].ADV_THUMBNAME + '" title="' + data.d[i].ADV_FILENAME + '" id="' + data.d[i].ADV_ID + '" />'
                                    + '</div><div class="image-wrapper-footer"><i class="fa fa-plus" style="color:#275ACE;"></i><i class="fa fa-trash-o" style="color:red;" onclick="delAdv(' + data.d[i].ADV_ID + ')"></i></div></div>');
                   //$('#advertisement-uploaded').append('<div class="image-wrapper image-uploaded" data-advID="'+data.d[i].ADV_ID+'" data-advName="' + data.d[i].ADV_FILENAME + '"><div class="image-wrapper-header" title="' + data.d[i].ADV_FILENAME + '">' + data.d[i].ADV_FILENAME + '</div>'
                   //             + '<div class="image-wrapper-body"><img class="image-div" src="' + data.d[i].ADV_FILE_SRC + '" title="' + data.d[i].ADV_FILENAME + '" id="' + data.d[i].ADV_ID + '" />'
                   //             + '</div><div class="image-wrapper-footer"><i class="fa fa-plus" style="color:#275ACE;"></i><i class="fa fa-trash-o" style="color:red;" onclick="delAdv(' + data.d[i].ADV_ID + ')"></i></div></div>');
                } else if (data.d[i].ADV_TYPE == "video") {
                    //$('#advertisement-uploaded').append('<div class="image-wrapper image-uploaded" data-advID="'+data.d[i].ADV_ID+'" data-advName="' + data.d[i].ADV_FILENAME + '"><div class="image-wrapper-header" title="' + data.d[i].ADV_FILENAME + '">' + data.d[i].ADV_FILENAME + '</div>'
                    //                + '<div class="image-wrapper-body"><video controls class="image-div" src="../views/kioskMaintenance/advertisement/Adv/Thumbnail/' + data.d[i].ADV_FILENAME + '" title="' + data.d[i].ADV_FILENAME + '" id="' + data.d[i].ADV_ID + '" /></div>'
                    //                + '<div class="image-wrapper-footer"><i class="fa fa-plus" style="color:#275ACE;"></i><i class="fa fa-trash-o" style="color:red;" onclick="delAdv(' + data.d[i].ADV_ID + ')"></i></div></div>');
                    //$('#advertisement-uploaded').append('<div class="image-wrapper image-uploaded" data-advID="' + data.d[i].ADV_ID + '" data-advName="' + data.d[i].ADV_FILENAME + '"><div class="image-wrapper-header" title="' + data.d[i].ADV_FILENAME + '">' + data.d[i].ADV_FILENAME + '</div>'
                    //                    + '<div class="image-wrapper-body"><img class="image-div" src="../views/kioskMaintenance/advertisement/Adv/Thumbnail/' + data.d[i].ADV_THUMBNAME + '" title="' + data.d[i].ADV_FILENAME + '" id="' + data.d[i].ADV_ID + '" />'
                    //                    + '</div><div class="image-wrapper-footer"><i class="fa fa-plus" style="color:#275ACE;"></i><i class="fa fa-trash-o" style="color:red;" onclick="delAdv(' + data.d[i].ADV_ID + ')"></i></div></div>');
                    
                    //<video controls preload="none" width="640" height="360" poster="perigny-poster.jpg">
                    //  <source src="perigny.mp4"  type="video/mp4">
                    //</video>
                   // var filename = data.d[i].ADV_FILENAME.substr(0, filename1.lastIndexOf('.')) || data.d[i].ADV_FILENAME;
                    var filename = data.d[i].ADV_FILENAME.substr(0, data.d[i].ADV_FILENAME.lastIndexOf('.'));
                    $('#advertisement-uploaded').append('<div class="image-wrapper image-uploaded" data-advID="' + data.d[i].ADV_ID + '" data-advName="' + data.d[i].ADV_FILENAME + '"><div class="image-wrapper-header" title="' + data.d[i].ADV_FILENAME + '">' + data.d[i].ADV_FILENAME + '</div>'
                                   + '<div class="image-wrapper-body"><video controls preload="none" poster="../views/kioskMaintenance/advertisement/Adv/Thumbnail/' + data.d[i].ADV_THUMBNAME + '" class="image-div" src="../views/kioskMaintenance/advertisement/Adv/mp4/' + data.d[i].ADV_ID + '.mp4" title="' + data.d[i].ADV_FILENAME + '" id="' + data.d[i].ADV_ID + '" /></div>'
                                   + '<div class="image-wrapper-footer"><i class="fa fa-plus" style="color:#275ACE;"></i><i class="fa fa-trash-o" style="color:red;" onclick="delAdv(' + data.d[i].ADV_ID + ')"></i></div></div>');

                }
            }
        }
    };

    function getAdDetailsError(data) {
        alert("Error to retrieve advertisement details!");
    };

    function delAdv(advId) {
        // check default adv exist or not?
        var li = document.getElementById("advertisement-seq").getElementsByTagName("li");
        var proceed = true;

        for(var i = 0; i < li.length; i++){   
            var seqAdvId = $(li[i]).children('.seq-img').attr('data-advID');

            if(advId == seqAdvId){
                alert("Please remove from the sequence first.");
                proceed = false;
                break;
            }
            else{
                continue;
            }
        }

        if(proceed){
            var r = confirm("Are you sure you want to delete this advertisement?");

            if (r == true) {
                var para = {
                    'advID': advId
                };

                var objurl =
                {
                    'url': 'delAdvFile',
                    'data': para
                };
                __JSONWEBSERVICE.getServices(objurl, delAdvSuccess, delAdvError);
                // __JSONWEBSERVICE.getServices(objurl, checkAdvAppliedSuccess, checkAdvAppliedError);
            } else {
                return;
            }
        }
    }

    function delAdvSuccess(msg) {
        if (msg.d == 0) {
            alert("Sorry! Unable to delete this advertisement file due to some of the machines are applying this advertisement. \r\n Please delete from default sequence first!");
            return;
        } else if (msg.d == 1) {
            alert("Advertisement deleted successfully!");
            __JSONWEBSERVICE.getServices("getAdDetails", getAdDetailsSuccess, getAdDetailsError);
        } else if (msg.d == -1) {
            alert("Failed to delete default sequence due to record exists!");
        } else {
            alert("Failed to delete default sequence due to database error!");
        }
    };

    function delAdvError(msg) {
        alert('Error: Failed to delete advert file!');
    };

    function saveSequence() {
        var items = document.getElementById("advertisement-seq").getElementsByTagName("ul");
        var arrAdv = [];

        $('#advertisement-seq > ul li').each(function(){
            var advSeq = $(this).children('.seq-img').attr('data-advID');
            arrAdv.push(advSeq);
        });

        if(arrAdv.length == 0){
            var confirmed = confirm("The sequence is empty. Are you sure you want to save?")
            
            if(confirmed){
                insertSeqToDB(arrAdv);
            }
        }
        else
        {
            insertSeqToDB(arrAdv);
        }
        
    }
    function insertSeqToDB(arrAdv) {

        var para = {
            'arrAdv': arrAdv
        };

        var objurl =
        {
            'url': 'setAdvSeq',
            'data': para
        };

        __JSONWEBSERVICE.getServices(objurl, setAdvSeqSuccess, setAdvSeqError);
    }

    function setAdvSeqSuccess(msg) {
        if (msg.d == 1) {
            alert("Sequence saved successfully!");
        } else if (msg.d == -1) {
            alert("Failed to saved the sequence due to record exists!");
        } else {
            alert("Failed to saved the sequence due to database error!");
        }
    };

    function setAdvSeqError(msg) {
        alert('Error: Failed to saved the sequence!');
    };


    function getAdDefaultSeqSuccess(data) {
        
        $('#advertisement-seq').empty();

        if(data.d.length == 0){
            $('#advertisement-seq').html('<span id="seq-empty-msg" style="color:#999;">Nothing in the sequence.</span><div class="block-xs"></div>');
        }
        else{
            $('#advertisement-seq').append('<ul></ul>');

            for (var i = 0; i < data.d.length; i++) {
                if (data.d[i].ADV_TYPE == "image") {
                    $('#advertisement-seq > ul').append('<li><div class="image-wrapper seq-img" data-advID="' + data.d[i].ADV_ID + '" data-advName="' + data.d[i].ADV_FILENAME + '" title="' + data.d[i].ADV_FILENAME + '">'
                                + '<div class="image-wrapper-header"><span class="remove-seq-img"><i class="fa fa-minus-circle"></i></span>' + data.d[i].ADV_FILENAME + '</div>'
                                + '<div class="image-wrapper-body"><img class="image-div" src="../views/kioskMaintenance/advertisement/Adv/Thumbnail/' + data.d[i].ADV_THUMBNAME + '" title="' + data.d[i].ADV_FILENAME + '" />'
                                + '</div></div></li>');
                } else if (data.d[i].ADV_TYPE == "video") {
                    var filename = data.d[i].ADV_FILENAME.substr(0, data.d[i].ADV_FILENAME.lastIndexOf('.'));
                    $('#advertisement-seq > ul').append('<li><div class="image-wrapper seq-img" data-advID="' + data.d[i].ADV_ID + '" data-advName="' + data.d[i].ADV_FILENAME + '" title="' + data.d[i].ADV_FILENAME + '">'
                                + '<div class="image-wrapper-header"><span class="remove-seq-img"><i class="fa fa-minus-circle"></i></span>' + data.d[i].ADV_FILENAME + '</div>'
                                + '<div class="image-wrapper-body"><video controls class="image-div" controls preload="none" poster="../views/kioskMaintenance/advertisement/Adv/Thumbnail/' + data.d[i].ADV_THUMBNAME + '" class="image-div" src="../views/kioskMaintenance/advertisement/Adv/mp4/' + data.d[i].ADV_ID + '.mp4"  title="' + data.d[i].ADV_FILENAME + '" />'
                                + '</div></div></li>');
                }
            }

            $('#advertisement-seq > ul').sortable({
                containment: "#advertisement-seq-div"
            });

            $('#saveSequenceBtn').show();
        }
    };
    function getAdDefaultSeqError(data) {
        alert("Error to retrieve advertisement sequence!");
    };

    function delDefaultSeqAdv(advid) {
        var r = confirm("Are you sure you want to delete the sequence?");

        if (r == true) {
            delDefaultSeq(advid);
        } else {
            return;
        }
    }

    function delDefaultSeq(advid) {
        var para = {
            'advID': advid
        };

        var objurl =
        {
            'url': 'delAdvDefaultSeq',
            'data': para
        };
        __JSONWEBSERVICE.getServices(objurl, delDefaultSeqSuccess, delDefaultSeqError);
    }

    function delDefaultSeqSuccess(msg) {
        if (msg.d == 1) {
            alert("Advertisement default sequence deleted successfully!");
            __JSONWEBSERVICE.getServices("getAdDefaultSeq", getAdDefaultSeqSuccess, getAdDefaultSeqError);
        } else if (msg.d == -1) {
            alert("Failed to delete default sequence due to record exists!");
        } else {
            alert("Failed to delete default sequence due to database error!");
        }
    };

    function delDefaultSeqError(msg) {
        alert('Error: Failed to set machine sequence!');
    };

    function setMachinesAdvSeqSuccess(msg) {
        if (msg.d == 1) {
            alert("Specific machine advertisement sequence saved successfully!");
        } else if (msg.d == -1) {
            alert("Failed to save machine sequence due to record exists!");
        } else {
            alert("Failed to save machine sequence due to database error!");
        }
    };

    function setMachinesAdvSeqError(msg) {
        alert('Error: Failed to set machine sequence!');
    };

    function setMachAdvSeqSuccess(msg) {
        if (msg.d == 1) {
            alert("Specific machine advertisement sequence saved successfully!");
        } else if (msg.d == -1) {
            alert("Failed to save machine sequence due to record exists!");
        } else {
            alert("Failed to save machine sequence due to database error!");
        }
    };

    function setMachAdvSeqError(msg) {
        alert('Error: Failed to set machine sequence!');
    };

    function saveSeqToAll() {
        var items = document.getElementById("advertisement-seq").getElementsByTagName("ul");
        var arrAdv = [];

        $('#advertisement-seq > ul li').each(function () {
            var advSeq = $(this).children('.seq-img').attr('data-advID');

            arrAdv.push(advSeq);
        });

        if (arrAdv.length == 0) {

            var confirmed = confirm("Are you sure you want to apply the new changes to all machines ?")
            if (confirmed) {
                applySeqToAll(arrAdv);
            }
        }
        else {
            applySeqToAll(arrAdv);
        }

    }

    function applySeqToAll(arrAdv) {

        var para = {
            'arrAdv': arrAdv
        };

        var objurl =
        {
            'url': 'regenSeqToAll',
            'data': para
        };

        __JSONWEBSERVICE.getServices(objurl, regenSeqToAllSuccess, regenSeqToAllError);
    }

    function regenSeqToAllSuccess(msg) {
        if (msg.d == 1) {
            alert("Default sequence applied to all machines successfully!");
        } else if (msg.d == -1) {
            alert("Failed to apply Default sequence due to record exists!");
        } else {
            alert("Failed to apply Default sequence due to database error!");
        }
    };

    function regenSeqToAllError(msg) {
        alert('Error: Failed to apply Default sequence!');
    };


    $(document).ready(function () {
        
        $('#advertisement-maintenance-main-div').off();

        document.getElementById('image-uploader').addEventListener('change', handleFileSelect, false);
        //$('#image-uploader').on('change', handleFileSelect);
        __JSONWEBSERVICE.getServices("getAdDetails", getAdDetailsSuccess, getAdDetailsError);
        __JSONWEBSERVICE.getServices("getAdDefaultSeq", getAdDefaultSeqSuccess, getAdDefaultSeqError);
        uploadAdv();
        addIntoSeq();
        removeSeqImg();
    });
</script>

<div id="advertisement-maintenance-main-div">
    <div id="advertisement-upload-btn-section">
        <span style="position: absolute;margin-left: -6px;margin-top: -32px;background-color: #FFF;font-weight: bold;">Upload File</span>
        <input type="text" id="total-uploaded-files" readonly />
        <div id="browse-btn">
            <span>Browse</span>
            <input type="file" id="image-uploader" multiple />
        </div>
        <button type="button" id="upload-btn">
        <i class="fa fa-upload"></i>Upload
        </button>
    </div>
    <div class="block-md"></div>
    <div id="advertisement-upload-div">
        <h4>Advertisement Media Uploaded</h4>
        <div id="advertisement-uploaded">
            Loading...
        </div>
    </div>
    <hr style="margin-top:0px" />
    <div id="advertisement-seq-div">
        <h4>Display Sequence</h4>
        <div id="advertisement-seq">
            Loading...
        </div>
    <button id="saveSequenceBtn" class="btn btn-primary" onclick="saveSequence()">Save Sequences</button>
    <button id="saveSeqToAllBtn" class="btn btn-primary" onclick="saveSeqToAll()">Apply to All</button>
    </div>
    <br />
</div>

