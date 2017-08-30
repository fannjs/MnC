jQuery(document).ready(function($) {

    var getMachineTypes = function() {

        jQuery.support.cors = true;
        $.ajax({
            url: '/api/machinetype',
            type: 'GET',
            success: onMachineTypesReturn,
            error: function(error) {
                alert("Error. " + error.status);
            }
        });
    };

    function onMachineTypesReturn(machineTypes) {

        var $tblMachTemplateBody = $('#tblMachTempList > tbody');

        var $newRow;
        var $newCol;
        var $lnk = $('<a>').attr('href', '#');
        $tblMachTemplateBody.empty();

        if(machineTypes.length === 0){
            $tblMachTemplateBody.html('<tr><td colspan="4">No Record</td></tr>');
        }else{
            $.each(machineTypes, function(index, machineType) {

                $newCol = $('<td>');
                $newRow = $('<tr>');

                $newRow
                    .append($newCol.clone().html(machineType.M_MACH_TYPE))
                    .append($newCol.clone().html(machineType.M_MACH_MODEL))
                    //.append($newCol.clone().html(displayImage(machineType.M_MACH_IMGPATH))) //todo:image
                    .append($newCol.clone()
                        .append($lnk.clone().html('<i class="fa fa-pencil"></i>Edit').bind('click', { mType: machineType.M_MACH_TYPE, mModel: machineType.M_MACH_MODEL }, onEditMachTypeClicked))
                        .append(' <span class="center-divider"></span>')
                        .append($lnk.clone().html('<i class="fa fa-trash-o"></i>Delete').bind('click', { mType: machineType.M_MACH_TYPE, mModel: machineType.M_MACH_MODEL }, onDeleteMachTypeClicked)))
                    .append($newCol.clone()
                        .append($lnk.clone().html('<i class="fa fa-wrench"></i>Configure Status Code').bind('click', { mType: machineType.M_MACH_TYPE, mModel: machineType.M_MACH_MODEL }, onAEVErrorCodeClicked)));
                //data-toggle="modal" data-target="#errorCode-modal"
                $tblMachTemplateBody.append($newRow);
            });
        }
    }

    function onEditMachTypeClicked(event) {
        var mType = event.data.mType;
        var mModel = event.data.mModel;
        //alert("mType = " + mType + ", mModel = " + mModel);
        triggerEditPage(mType, mModel);
        //getErrorCodesByMachineType(mType, mModel, $(this));
    }

    function onDeleteMachTypeClicked(event) {
        var option = confirm("Are you sure you want to Delete?");
        if (option) {
            var mType = event.data.mType;
            var mModel = event.data.mModel;

            var para = {'MachineType': mType, 'MachineModel': mModel};

            var objurl =
            {
                'url': 'deleteMachine',
                'data': para,
            };

            __JSONWEBSERVICE.getServices(objurl, deleteMachineSuccess, deleteMachineError);
        }
        return;
    }

    function deleteMachineSuccess(msg) {
        
        var status = msg.d.Status;
        var msg = msg.d.Message;

        if(!status){
            alert(msg);
            return false;
        }
        
        alert("Successful. Your action has been sent to checker for approval.");
        showMainPage();
    };

    function deleteMachineError(data) {
        alert('Error: Failed to deleted machine!');
    };

    function onAEVErrorCodeClicked(event) {
        
        // {mType: machineType.M_MACH_TYPE, mModel: machineType.M_MACH_MODEL}
        var mType = event.data.mType;
        var mModel = event.data.mModel;
        // work in this two ways
        $('input[id=hidMType]').val(mType)
        $("#hidMModel").val(mModel);
        hideEditDiv();
        event.preventDefault();

        getErrCodeByMachTypeAndModel(mType, mModel, onErrCodesReturn);
    }

    var getErrCodeByMachTypeAndModel = function(mType, mModel, successCallBack) {
        $.ajax({
            type: "POST",
            url: "/app_monitoring/content/MachTemplate.asmx/getErrorCodesByMachineType",
            contentType: "application/json; charset=utf-8",
            data: JSON.stringify({ 'mType': mType, 'mModel': mModel }),
            dataType: "json",
            success: function (msg) {
                successCallBack(msg, mType, mModel);
            },
            error: function(error) {
                alert('Error ' + error.status);
            }
        });
    };


    var onErrCodesReturn = function (msg, mType, mModel) {

        
        var data = $.parseJSON(msg.d);
        var resList = data.LIST;
        var $newRow;
        var $newCol;
        var $lnk = $('<a>').attr('href', '#');
        //var $tblWrapper = $('<div>').css('overflow', 'auto');

        var $tblErrorCodesBody = $('#tblErrorCodes tbody');
        $('#tblErrorCodes tbody').empty();

        if(resList.length === 0){
            $tblErrorCodesBody.html("No record");
        }else{

            $.each(resList, function(key, val) {

                $newRow = $('<tr>'); 
                $newCol = $('<td>');
                $newSpan = $('<span>');
                $newRow.bind('click', { mType: mType, mModel: mModel, eCode: val.STATUSCODE, eDesc: val.ERRORDESC, eType: val.ERROR, eSOP: val.SOP, cID: val.CategoryID, imgID: val.ImageID }, onEditErrClicked)
                    .append($newCol.clone().html(val.STATUSCODE))
                    .append($newCol.clone().html(val.ERRORDESC))
                    .append($newCol.clone().html(val.ERROR))
                    .append($newCol.clone().html($newSpan.on('mouseover', {eSOP: val.SOP }, ViewSOP).on('mouseout', CloseSOP).html('View').addClass('view-sop')));
                    //.append($newCol.clone()
                       // .append($lnk.clone().html('<i class="fa fa-pencil-square-o"></i>Edit'))
                       // .append(' <span class="center-divider"></span>')
                       // .append($lnk.clone().html('<i class="fa fa-minus-circle"></i>Delete').bind('click', { mType: mType, mModel: mModel, eCode: val.STATUSCODE, eDesc: val.ERRORDESC, eType: val.ERROR }, onDeleteErrClicked))
                   // );

                $tblErrorCodesBody.append($newRow);
            });
        }

        $('#errorCode-modal').modal('show');
        sortTable();
    };

    //TableSorting has problem. Have to fix this soon.
    function sortTable(){
        /*Quick fix*/
        //Remove sorting each time before attach table sorting function, due to data duplication
        //Reference - http://stackoverflow.com/questions/8171530/remove-jquery-tablesorter-from-table
        $('#tblErrorCodes')
             .unbind('appendCache applyWidgetId applyWidgets sorton update updateCell')
             .removeClass('tablesorter')
             .find('thead th')
             .unbind('click mousedown')
             .removeClass('header headerSortDown headerSortUp');
        
        //Attach sorting to this table
        $("#tblErrorCodes").tablesorter();
    }

    function ViewSOP(event){
        var top = event.pageY - 70; //Deduct 70px because of margin
        var SOP = "";

        if(event.data.eSOP == ""){
            SOP = "No data";

            $(this).parent().append('<div id="sop-tooltips">' + SOP + '</div>');
            $('#sop-tooltips').css({top:top});
        }
        else{
            var steps = event.data.eSOP.split("\n");
            
            SOP = SOP + '<ul style="list-style: none; margin: 0px; padding: 0px;">';
            for(var i = 0 ; i < steps.length; i++){
                if(steps[i] !== ""){
                    SOP = SOP + '<li>'+steps[i]+'</li>';
                }
            }
            SOP = SOP + '</ul>';

            $(this).parent().append('<div id="sop-tooltips">' + SOP + '</div>');
            $('#sop-tooltips').css({top:top, width: 150});
        }
    }
    function CloseSOP(){
        $('#sop-tooltips').remove();
    }

    function onEditErrClicked(event) {
        //{mType: mType, mModel: mModel, eCode: val.STATUSCODE, eDesc: val.ERRORDESC, eType: val.ERROR
        if($(this).hasClass('active-td')){
            return;
        }
        var MType = $('#hidMType').val();
        var MModel = $('#hidMModel').val();

        $("#editErrorCode").val(event.data.eCode);
        $("#editErrorDescription").val(event.data.eDesc);
        $("#hidOriMCode").val(event.data.eCode);
        $("#editSOP").val(event.data.eSOP);

        if (event.data.eType == "ERROR") {
            //$('#editCodeStatus option').val("ERROR").attr('selected', 'selected');
            $('#ERROR').attr('selected', 'selected');
        } else {
            $('#ERROR').removeAttr('selected');
        }
        if (event.data.eType == "WARNING") {
            $('#WARN').attr('selected', 'selected');
        } else {
            $('#WARN').removeAttr('selected');
        }
        if (event.data.eType == "ONLINE") {
            $('#ONLINE').attr('selected', 'selected');
        } else {
            $('#ONLINE').removeAttr('selected');
        }
        if (event.data.eType == "OFFLINE") {
            $('#OFFLINE').attr('selected', 'selected');
        } else {
            $('#OFFLINE').removeAttr('selected');
        }

        $('#tblErrorCodes tr').removeClass('active-td');
        $(this).closest('tr').addClass('active-td');

        // Attach function to the delete button with event data
        $('#btnDeleteMCode').off().on('click',{ mType: event.data.mType, mModel: event.data.mModel, eCode: event.data.eCode, eDesc: event.data.eDesc, eType: event.data.eType }, onDeleteErrClicked);

        showCodeCategoryWithImages(event.data.cID, event.data.imgID);

        showEditDiv(event.data.eCode);

        
    };

    function onDeleteErrClicked(event) {
        
        //{mType: mType, mModel: mModel, eCode: val.STATUSCODE, eDesc: val.ERRORDESC, eType: val.ERROR
        var mType = event.data.mType;
        var mModel = event.data.mModel;
        var mCode = event.data.eCode;
        
        var para = {'MType': mType, 'MModel': mModel, 'MCode': mCode};

        var option = confirm("Are you sure you want to Delete?");
        if (option) {
            
            var objurl =
            {
                'url': 'deleteMCode',
                'data': para,
            };

            __JSONWEBSERVICE.getServices(objurl, deleteMCodeSuccess, deleteMCodeError);
        }
        return;
    }

    function deleteMCodeSuccess(msg) {

        var status = msg.d.Status;
        var msg = msg.d.Message;

        if(!status){
            alert(msg);
            return false;
        }
        
        alert("Successful. Your action has been sent to checker for approval.");

        var mType = $('#hidMType').val();
        var mModel = $('#hidMModel').val();

        getErrCodeByMachTypeAndModel(mType, mModel, onErrCodesReturn);
        $('#edit-code-div').hide();
    };

    function deleteMCodeError(data) {
        alert('Error: Failed to deleted machine code!');
    };

    function showCodeCategoryWithImages(cID, imgID){
        
        //Load code category
        $.ajax({
            type: "POST",
            url: "/views/kioskManagement/machineTemplate/main.aspx/getCC",
            contentType: "application/json; charset=utf-8",
            data: "{}",
            dataType: "json",
            success: function(data){

                var ccOptions = "";
                    
                ccOptions = ccOptions + '<option selected value="0"> - None - </option>';

                for(var i = 0; i < data.d.length; i++){
                        
                    if(data.d[i].CCID == cID){
                        ccOptions = ccOptions + '<option value="' + data.d[i].CCID + '" selected>' + data.d[i].CCName + '</option>';
                    }
                    else{
                        ccOptions = ccOptions + '<option value="' + data.d[i].CCID + '">' + data.d[i].CCName + '</option>';
                    }
                }

                $('#editCategory').html(ccOptions);
            },
            error: function(error){
                alert("Error occurs while loading Code Category.");
            }
         });

         if(cID !== ""){

             //Load images 
             $.ajax({
                type: "POST",
                url: "/views/kioskManagement/machineTemplate/main.aspx/loadImageCodeCategory",
                contentType: "application/json; charset=utf-8",
                data: JSON.stringify({ccID: cID}),
                dataType: "json",
                beforeSend: function(){
                    $('#editSelectImgHide').fadeIn('fast');
                    $('#editSelectImgDiv').html('Retriving images. Please wait.');
                },
                success: function(data){
                
                    var ImageCodeCategoryList = data.d;

                    $('#editSelectImgDiv').empty();

                    for(var i = 0; i < ImageCodeCategoryList.length; i++){
                        
                        if(ImageCodeCategoryList[i].ImgID == imgID){
                            $('#editSelectImgDiv').append('<div class="outer-thumbnail-container selected-img"><div class="checkedIcon"><i class="fa fa-check"></i></div><input type="hidden" value="' + ImageCodeCategoryList[i].ImgID + '" />'
                                                            +'<div class="thumbnail-container"><img src="' + ImageCodeCategoryList[i].ImgPath + '" /><div></div>');
                        }
                        else{
                            $('#editSelectImgDiv').append('<div class="outer-thumbnail-container"><div class="checkedIcon"><i class="fa fa-check"></i></div><input type="hidden" value="' + ImageCodeCategoryList[i].ImgID + '" />'
                                                            +'<div class="thumbnail-container"><img src="' + ImageCodeCategoryList[i].ImgPath + '" /><div></div>');
                        }
                    }
               
                },
                error: function(error){
                    alert("Error occurs while retriving Images.");
                }
            });
        }
        else{
            $('#editSelectImgHide').hide();
            $('#editSelectImgDiv').empty();
        }
    }

    $('#editCategory').change(function(){
        var cID = $(this).val();

        if(cID == 0){
            $('#editSelectImgHide').hide();
            return false;
        }

        $.ajax({
            type: "POST",
            url: "/views/kioskManagement/machineTemplate/main.aspx/loadImageCodeCategory",
            contentType: "application/json; charset=utf-8",
            data: JSON.stringify({ccID: cID}),
            dataType: "json",
            beforeSend: function(){
                $('#editSelectImgHide').fadeIn('fast');
                $('#editSelectImgDiv').html('Retriving images. Please wait.');
            },
            success: function(data){
                
                var ImageCodeCategoryList = data.d;

                $('#editSelectImgDiv').empty();

                if(ImageCodeCategoryList.length == 0){
                    $('#editSelectImgDiv').html('<span style="opacity:0.7;">No images was found.</span>');
                }
                else{
                    //Load images
                    for(var i = 0; i < ImageCodeCategoryList.length; i++){
                        $('#editSelectImgDiv').append('<div class="outer-thumbnail-container"><div class="checkedIcon"><i class="fa fa-check"></i></div><input type="hidden" value="' + ImageCodeCategoryList[i].ImgID + '" />'
                                                        +'<div class="thumbnail-container"><img src="' + ImageCodeCategoryList[i].ImgPath + '" /><div></div>');
                    }
                }
               
            },
            error: function(error){
                alert("Error occurs while retriving Images.");
            }
        });
    });

    $('#editSelectImgDiv').on('click','.outer-thumbnail-container',Update_SelectOneImg);

    function Update_SelectOneImg(){
        if($(this).hasClass('selected-img')){
            $(this).removeClass('selected-img');
        }
        else{
            $('#editSelectImgDiv .outer-thumbnail-container').removeClass('selected-img');
            $(this).addClass('selected-img');
        }
    }

    $('#btnUpdateMCode').click(function () {
        var ErrorCode = $('#editErrorCode').val();
        var ErrorDesc = $('#editErrorDescription').val();
        var CodeStatus = $('#editCodeStatus').val();
        var SOP = $('#editSOP').val();
        var CodeCategory = $('#editCategory').val();
        var imageSelected;

        if (ErrorCode == "") {
            alert("Please enter Error Code field!");
            return;
        }
        if (ErrorDesc == "") {
            alert("Please enter Error Description field!");
            return;
        }

        if(CodeCategory !== null){
            
            if($('#editSelectImgDiv .selected-img').length == 0){
                var confirmed = confirm("No image was selected. Are you sure you want to continue?");

                if(!confirmed){
                    return;
                }
                else{
                    imageSelected = "";
                }
            }
            else{
                imageSelected = $('#editSelectImgDiv .selected-img').find('input[type="hidden"]').val()
            }
        }
        else{
            //If Code Category is null, set these to empty
            CodeCategory = "";
            imageSelected = "";
        }

        var MType = $('#hidMType').val();
        var MModel = $('#hidMModel').val();
        var OriMCode = $('#hidOriMCode').val();


        var para = { 'OriMCode': OriMCode, 'MType': MType, 'MModel': MModel, 'ErrorCode': ErrorCode, 'ErrorDesc': ErrorDesc, 'CodeStatus': CodeStatus, 'Sop': SOP, 'CodeCategory': CodeCategory, 'ImageID': imageSelected };
        var objurl =
        {
            'url': 'updateMCode',
            'data': para,
        };

        __JSONWEBSERVICE.getServices(objurl, updateMCodeSuccess, updateMCodeError);
    });

    function updateMCodeSuccess(msg) {
        
        var status = msg.d.Status;
        var msg = msg.d.Message;

        if(!status){
            alert(msg);
            return false;
        }
            
        alert("Successful. Your action has been sent to checker for approval.");

        var mType = $('#hidMType').val();
        var mModel = $('#hidMModel').val();
        getErrCodeByMachTypeAndModel(mType, mModel, onErrCodesReturn);
        
        $('#edit-code-div').hide();
    };

    function updateMCodeError(data) {
        alert('Error: Failed to add machine!');
    };

    var displayImage = function (base64Data) {

        //bootstrap image zooming
        var $imgOutWrapper = $('<div>').addClass('thumbnail').css('width', '90px') ;
        var $imgWrapper = $('<div>').addClass('thumbnail-view');
        var $aImg = $('<a>').addClass('thumbnail-view-hover').addClass('ui-lightbox').attr('href', base64Data).magnificPopup({
            type: 'image',
            closeOnContentClick: false,
            closeBtnInside: true,
            fixedContentPos: true,
            mainClass: 'mfp-no-margins mfp-with-zoom', // class to remove default margin from left and right side
            image: {
                verticalFit: true,
                tError: '<a href="%url%">The image #%curr%</a> could not be loaded.'
            }
        });



        var imag = "<img "
                 + "src='"
                 + base64Data + "' width='90' height='90'/>";


        //"data:image/jpg;base64,"
        $imgWrapper.append($aImg).append(imag);
        $imgOutWrapper.append($imgWrapper);


   

        return $imgOutWrapper;
    };

    getMachineTypes();


    function loadCC(){
        $.ajax({
            type: "POST",
            url: "/views/kioskManagement/machineTemplate/main.aspx/getCC",
            contentType: "application/json; charset=utf-8",
            data: "{}",
            dataType: "json",
            success: function(data){

                var ccOptions = "";

                ccOptions = ccOptions + '<option selected value="0"> - None - </option>';

                for(var i = 0; i < data.d.length; i++){
                    ccOptions = ccOptions + '<option value="' + data.d[i].CCID + '">' + data.d[i].CCName + '</option>';
                }

                $('#inputSelectCategory').html(ccOptions);
            },
            error: function(error){
                alert("Error occurs while loading Code Category.");
            }
         });

         $('#selectImageHide').hide();
         $('#selectImageDiv').empty();
    }

    $('#toggleAddCodeTab').click(function(){
        if($(this).parent().hasClass('active')){
            return false;
        }

        $('#edit-code-div').hide(); //Hide edit code div
		$('#tblErrorCodes tr').removeClass('active-td'); //Remove active TD
        loadCC();
    });

    $('#inputSelectCategory').off().on('change', fetchRelatedImages);

    function fetchRelatedImages(){
        var ccID = $(this).val();

        if(ccID == 0){
            $('#selectImageHide').hide();
            return false;
        }

        $.ajax({
            type: "POST",
            url: "/views/kioskManagement/machineTemplate/main.aspx/loadImageCodeCategory",
            contentType: "application/json; charset=utf-8",
            data: JSON.stringify({ccID: ccID}),
            dataType: "json",
            beforeSend: function(){
                $('#selectImageHide').fadeIn('fast');
                $('#selectImageDiv').html('Retriving images. Please wait.');
            },
            success: function(data){
                
                var ImageCodeCategoryList = data.d;

                $('#selectImageDiv').empty();

                if(ImageCodeCategoryList.length == 0){
                    $('#selectImageDiv').html('<span style="opacity:0.7;">No images was found.</span>');
                }
                else{
                    //Load images
                    for(var i = 0; i < ImageCodeCategoryList.length; i++){
                        $('#selectImageDiv').append('<div class="outer-thumbnail-container"><div class="checkedIcon"><i class="fa fa-check"></i></div><input type="hidden" value="' + ImageCodeCategoryList[i].ImgID + '" />'
                                                    +'<div class="thumbnail-container"><img src="' + ImageCodeCategoryList[i].ImgPath + '" /><div></div>');
                    }
                }
               
            },
            error: function(error){
                alert("Error occurs while retriving Images.");
            }
        });
    }

    $('#selectImageDiv').off().on('click','.outer-thumbnail-container',MCode_SelectOneImage);

    function MCode_SelectOneImage(){
        if($(this).hasClass('selected-img')){
            $(this).removeClass('selected-img');
        }
        else{
            $('#selectImageDiv .outer-thumbnail-container').removeClass('selected-img');
            $(this).addClass('selected-img');
        }
    }

    $('#btnResetMCode').click(function(){
        
        $('#add-code-tab .input-field').val("");
        $('#inputCodeStatus option:first-child').attr('selected',true);
        loadCC();
    });

    $('#btnAddNewMCode').click(function () {
        var ErrorCode = $('#inputErrorCode').val();
        var ErrorDesc = $('#inputErrorDescription').val();
        var CodeStatus = $('#inputCodeStatus').val();
        var SOP = $('#inputSOP').val();
        var CodeCategory = $('#inputSelectCategory').val();
        var imgSelected;

        if (ErrorCode == "") {
            alert("Please enter Error Code field!");
            return;
        }
        if (ErrorDesc == "") {
            alert("Please enter Error Description field!");
            return;
        }
        if(CodeStatus == "" || CodeStatus == null){
            alert("Please choose status!");
            return;
        }
        var MType = $('#hidMType').val();
        var MModel = $('#hidMModel').val();

        if(CodeCategory !== "0"){
            
            if($('#selectImageDiv .selected-img').length == 0){
                var confirmed = confirm("No image was selected. Are you sure you want to continue?");

                if(!confirmed){
                    return;
                }
                else{
                    imgSelected = "";
                }
            }
            else{
                imgSelected = $('#selectImageDiv .selected-img').find('input[type="hidden"]').val()
            }
        }
        else{
            //If Code Category is null, set these to empty
            CodeCategory = "";
            imgSelected = "";
        }

        var para = {'MType': MType, 'MModel': MModel, 'ErrorCode': ErrorCode, 'ErrorDesc': ErrorDesc, 'CodeStatus': CodeStatus, 'Sop': SOP, 'CodeCategory': CodeCategory, 'ImageCodeCategory' : imgSelected};
        var objurl =
        {
            'url': 'addNewMCode',
            'data': para,
        };

        __JSONWEBSERVICE.getServices(objurl, addNewMCodeSuccess, addNewMCodeError);
    });

    function addNewMCodeSuccess(msg) {

        var status = msg.d.Status;
        var msg = msg.d.Message;

        if(!status){
            alert(msg);
            return false;
        }

        alert("Successful. Your action has been sent to checker for approval.");

        $('#add-code-tab .input-field').val("");
        $('#inputCodeStatus option:first-child').attr('selected',true);
        loadCC();

        var mType = $('#hidMType').val();
        var mModel = $('#hidMModel').val();

        getErrCodeByMachTypeAndModel(mType, mModel, onErrCodesReturn);
    };

    function addNewMCodeError(data) {
        alert('Error: Failed to add status code!');
    };

    //show 2 tabs upon edit is clicked: edit info and edit sensor
    function showEditDiv(eCode) {
        $('#edit-code-div').show();
        $('a[href=#edit-info-tab]').tab('show');

        $('.sensorField').reset();
        $('.sensorField').GetMachineError(eCode); //hack to access other files's function

    };


    //From main.aspx
    function triggerAddPage() {

        var postData = {
            task : $('#taskNameHidden').val()
        };
        
		$.post("kioskManagement/machineTemplate/add.aspx", postData, function (data) {
			$("#content-subpage-container").html(data);
			showSubPage();
		});
	}
	function triggerEditPage(mType, mModel) {
		pmType = mType;
		var pdata = {
			pmType: pmType,
			pmModel: mModel,
            task : $('#taskNameHidden').val()
		};
		$.post("/views/kioskManagement/machineTemplate/edit.aspx", pdata, function (data) {
			$("#content-subpage-container").html(data);
			showSubPage();
		});
	}
	function hideEditDiv() {
        $('#img-uploaded-div').empty();
        $('#addEditCC').hide(); //Hide add or edit Code Category

	    $('#divCodeCategory').hide(); //Hide Code Category div
	    $('#divErrCode').show(); //Show error code table

	    $('#divErrCode > .nav-tabs li').removeClass('active'); //Remove active from all LI in navigation
	    $('#divErrCode > .nav-tabs li:first-child').addClass('active'); //Add active to List of Error Code nav

	    $('#divErrCode > .tab-content > .tab-pane').removeClass('active'); //Remove all active tab content
	    $('#list-code-tab').addClass('active'); //Activate List of Error Code content

		$('#edit-code-div').hide(); //Hide edit code div
		$('#tblErrorCodes tr').removeClass('active-td'); //Remove active TD
	};

    $('#add-button').click(function () {
	    triggerAddPage();
	});

	$('#closeDiv').click(function () {
		hideEditDiv();
	});
	$('.cancel-btn').click(function () {
		hideEditDiv();
	});
	$(".arrow").draggable({
		helper: 'clone'
	});
	$("#containment-wrapper").droppable({
		accept: '.arrow',
		drop: function (event, ui) {
			$(ui.helper).clone().appendTo("#containment-wrapper");
			$("#containment-wrapper .arrow").addClass("dropped-arrow");
			$("#containment-wrapper .dropped-arrow").removeClass("ui-draggable-dragging arrow");
			$(".dropped-arrow").draggable({ containment: "#containment-wrapper", scroll: false });
		}
	});


    //Code Category functions start here
    var modalTitle = $('#modal-title').html();

    $('#manage-category-btn').click(function(){
        $('#modal-title').html(modalTitle + ' > Manage Category');
        $('.toggle-div').toggle();

        loadCodeCategory();
        loadMachineType();
    });

    function loadCodeCategory(){
        $.ajax({
            type: "POST",
            url: "/views/kioskManagement/machineTemplate/main.aspx/loadCodeCategory",
            contentType: "application/json; charset=utf-8",
            data: "{}",
            dataType: "json",
            success: function(data){   
                
                if(data.d.length == 0){
                    $('#tblCodeCategory > tbody').html('<tr class="emptyTableMsg"><td style="display:block; width:100%;">(empty)</td></tr>');
                }
                else{
                    var trs = "";

                    for(var i = 0; i < data.d.length; i++){
                        trs = trs + '<tr data-id=' + data.d[i].CCID + '><td>' + data.d[i].CCName + '</td><td>' + data.d[i].CCDesc + '</td><td>' + data.d[i].CCType + '</td><td>' + data.d[i].TotalImages + '</td></tr>';
                    }

                    $('#tblCodeCategory > tbody').html(trs);
                }
                                
            },
            error: function(error){
                alert("Failed to load Code Category list.");
            } 
        });
    }

    function loadMachineType(){
        var mtOptions = "";

        $.ajax({
            type: "POST",
            url: "/views/kioskManagement/machineTemplate/main.aspx/loadMachineType",
            contentType: "application/json; charset=utf-8",
            data: "{}",
            dataType: "json",
            success: function(data){   

                mtOptions = mtOptions + '<option selected disabled> - Please Select - </option>';
                
                for(var i = 0; i < data.d.length; i++){
                    mtOptions = mtOptions + '<option>' + data.d[i].MType + '</option>';
                }

                $('#inputCType').html(mtOptions);
            },
            error: function(error){
                alert("Failed to load Machine Type.");
            } 
        });
    }

    $('#backward-btn').click(function(){
        $('#modal-title').html(modalTitle);
        $('#tblCodeCategory > tbody > .active-td').removeClass('active-td');
        $('.toggle-div').toggle();
        $('#addEditCC').hide();

        $('#img-uploaded-div').empty();
        loadCC();
    });
    $('#addNewCC').click(function(){
        $('#tblCodeCategory > tbody > .active-td').removeClass('active-td');
        loadAddNewCCDiv();
    });
    $('.closeAddEditCC').click(function(){
        $('#addEditCC').hide();
        $('#tblCodeCategory > tbody > .active-td').removeClass('active-td');
    });

    function loadAddNewCCDiv(){
        $('#addEditCC').hide();

        $('#addEditCC > .nav-tabs').hide();
        $('#addEditCC > .nav-tabs li').removeClass('active'); //Remove active from all LI in navigation
	    $('#addEditCC > .nav-tabs li:first-child').addClass('active'); //Add active to List of Error Code nav
	    $('#addEditCC > .tab-content > .tab-pane').removeClass('active'); //Remove all active tab content
	    $('#cc-addEdit-tab').addClass('active'); //Activate List of Error Code content

        $('#addEditTitle').show();

        $('.ccTF').val("");
        $('#inputCType option:first-child').attr("selected","selected");
        $('#add-uploadImg').show();

        filesToSend = []; //Array shared, reset after using it.

        $('.editCCbtn').hide();

        $('#addCCBtn').show();
        $('#addEditCC').fadeIn(450);
    }

    $('#inputCImages').off().on('change', handlerFileSelect);

    var filesToSend = [];
    function handlerFileSelect(evt) { 
        
        var files = evt.target.files; // FileList object
        filesToSend = [];

        // Loop through the FileList and render image files as thumbnails.
        for (var i = 0, f; f = files[i]; i++) {

            // Only process these files
            if (!f.type.match('image.*')) {
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
                        };

                        filesToSend.push(aFile);
                    };
                })(f);

                reader.onloadend = function(e){
                    
                    //Load ended , do something
                    
                };
                // Read in the image file as a data URL.
                reader.readAsDataURL(f);
            }
        }  
    }

    $('#addCCBtn').click(function(){

        var ccName = $('#inputCName').val();
        var ccDesc = $('#inputCDesc').val();
        var ccType = $('#inputCType').val();

        if(ccName.trim() == ""){
            alert("Please enter name for Code Category.");
            return false;
        }
        if(ccDesc.trim() == ""){
            alert("Please enter description for Code Category.");
            return false;
        }
        if(ccType == null){
            alert("Please select Machine Type.");
            return false;
        }

        addCodeCategory(ccName, ccDesc, ccType, filesToSend, $(this));
    });

    function addCodeCategory(ccName, ccDesc, ccType, ccImages, trigger){

        $.ajax({
            type: "POST",
            url: "/views/kioskManagement/machineTemplate/main.aspx/insertNewCodeCategory",
            contentType: "application/json; charset=utf-8",
            data: JSON.stringify({ccName: ccName, ccDesc: ccDesc, ccType: ccType, ccImages: ccImages}),
            dataType: "json",
            beforeSend: function(){
                trigger.attr('disabled',true);
            },
            success: function(data){
                var StatusDescription = data.d.Description;
                var ResultStatus = data.d.Status;

                trigger.attr('disabled',false);

                if(!ResultStatus){
                    alert(StatusDescription);
                    return false;
                }
                
                alert("Successful. Your action has been sent to checker for approval.");
                $('#addEditCC').hide();
                //loadCodeCategory();
            },
            error: function(error){
                alert("Error occurs while inserting data into database.");
                trigger.attr('disabled',false);
            }
        });
    }

    $('#tblCodeCategory > tbody').off().on('click','tr:not(.emptyTableMsg)', onCodeCategoryClicked);

    function onCodeCategoryClicked(){
        
        var ccID = $(this).attr('data-id');

        if($(this).hasClass('active-td')){
            return false;
        }else{
            $('#tblCodeCategory > tbody > .active-td').removeClass('active-td');
            $(this).addClass('active-td');
        }
       
        $.ajax({
            type: "POST",
            url: "/views/kioskManagement/machineTemplate/main.aspx/loadCodeCategoryDetail",
            contentType: "application/json; charset=utf-8",
            data: JSON.stringify({ccID: ccID}),
            dataType: "json",
            success: function(data){
                var CodeCategory = data.d;

                loadAddNewCCDiv();
                toggleEditMode();

                //Populate data into the fields
                $('#inputCName').val(CodeCategory.CCName);
                $('#inputCDesc').val(CodeCategory.CCDesc);
                $("#inputCType option").filter(function() {
                    return $(this).text() == CodeCategory.CCType; 
                }).prop('selected', true);
                //Populate data completed
            },
            error: function(error){
                alert("Error occurs while loading Code Category details.");
            }
        });
    }

    function toggleEditMode(){
        $('#addEditCC > .nav-tabs')
        $('#addEditCC > .nav-tabs').show();
        $('#addEditTitle').hide();
        $('#addCCBtn').hide();
        $('.editCCbtn').show();
        $('#add-uploadImg').hide();
    }

    function getCodeCategoryID(){
        var ccID = $('#tblCodeCategory > tbody > .active-td').attr('data-id');

        return ccID;
    }

    $('#updateCCBtn').click(function(){
        var ccID = getCodeCategoryID();

        if(ccID == "" || ccID == undefined || ccID == null){
            alert("Failed. Please reload and try again later.");
            return false;
        }

        var ccName = $('#inputCName').val();
        var ccDesc = $('#inputCDesc').val();
        var ccType = $('#inputCType').val();

        if(ccName.trim() == ""){
            alert("Please enter name for Code Category.");
            return false;
        }
        if(ccDesc.trim() == ""){
            alert("Please enter description for Code Category.");
            return false;
        }
        if(ccType == null){
            alert("Please select Machine Type.");
            return false;
        }

        updateCodeCategory(ccID,ccName, ccDesc, ccType, $(this));
    });

    function updateCodeCategory(ccID, ccName, ccDesc, machType, trigger){
        $.ajax({
            type: "POST",
            url: "/views/kioskManagement/machineTemplate/main.aspx/updateCodeCategory",
            contentType: "application/json; charset=utf-8",
            data: JSON.stringify({ccID: ccID, ccName: ccName, ccDesc: ccDesc, machType: machType}),
            dataType: "json",
            beforeSend: function(){
                trigger.attr('disabled',true);
            },
            success: function(data){
                var StatusDescription = data.d.Description;
                var ResultStatus = data.d.Status;

                trigger.attr('disabled',false);

                if(!ResultStatus){
                    alert(StatusDescription);
                    return false;
                }

                alert("Successful. Your action has been sent to checker for approval.");

                $('#addEditCC').hide();
                loadCodeCategory();
            },
            error: function(error){
                alert("Error occurs while updating data to database.");
                trigger.attr('disabled',false);
            }
        });
    }

    $('#deleteCCBtn').click(function(){
        var ccID = getCodeCategoryID();

        if(ccID == "" || ccID == undefined || ccID == null){
            alert("Failed. Please reload and try again later.");
            return false;
        }

        var confirmed = confirm("Images of this category will be removed permanently. \n\nAre you sure you want to delete?");
        
        if(confirmed){
           deleteCodeCategory(ccID, $(this));
        }
    });

    function deleteCodeCategory(ccID, trigger){
         $.ajax({
            type: "POST",
            url: "/views/kioskManagement/machineTemplate/main.aspx/deleteCodeCategory",
            contentType: "application/json; charset=utf-8",
            data: JSON.stringify({ccID: ccID}),
            dataType: "json",
            beforeSend: function(){
                trigger.attr('disabled',true);
            },
            success: function(data){
                var StatusDescription = data.d.Description;
                var ResultStatus = data.d.Status;
                
                trigger.attr('disabled',false);

                if(!ResultStatus){
                    alert(StatusDescription);
                    return false;
                }

                alert("Successful. Your action has been sent to checker for approval.");

                $('#addEditCC').hide();
                loadCodeCategory();
            },
            error: function(error){
                alert("Error occurs while deleting data.");
                trigger.attr('disabled',false);
            }
         });
    }

    $('#toggleImageTab').click(function(){

        if($(this).parent().hasClass('active')){
            return false;
        }
        var ccID = getCodeCategoryID();
        loadImagesOfCodeCategory(ccID);
    });

    function loadImagesOfCodeCategory(ccID){

        $.ajax({
            type: "POST",
            url: "/views/kioskManagement/machineTemplate/main.aspx/loadImageCodeCategory",
            contentType: "application/json; charset=utf-8",
            data: JSON.stringify({ccID: ccID}),
            dataType: "json",
            beforeSend: function(){
                $('#img-uploaded-div').html('Retriving images. Please wait.');
            },
            success: function(data){

                var ImageCodeCategoryList = data.d;

                $('#img-uploaded-div').empty();

                if(ImageCodeCategoryList.length == 0){
                    $('#img-uploaded-div').html('<span style="opacity:0.7;">No images was found.</span>');
                }
                else{
                    //Load images
                    for(var i = 0; i < ImageCodeCategoryList.length; i++){
                        $('#img-uploaded-div').append('<div class="outer-thumbnail-container"><div class="checkedIcon"><i class="fa fa-check"></i></div><input type="hidden" value="' + ImageCodeCategoryList[i].ImgID + '" />'
                                                    +'<div class="thumbnail-container"><img src="' + ImageCodeCategoryList[i].ImgPath + '" /><div></div>');
                    }
                }
            },
            error: function(error){
                alert("Error occurs while loading Code Category Images.");
            }
        });
    }

    $('#inputEdit_UploadImages').off().on('change', handlerFileSelect);

    $('#edit_UploadImagesBtn').click(function(){
        var ccID = getCodeCategoryID();

        if(ccID == "" || ccID == undefined || ccID == null){
            alert("Failed. Please reload and try again later.");
            return false;
        }

        if($('#inputEdit_UploadImages').val() == ""){
            alert("Please select file(s) to upload.");
        }
        else{
            uploadImages(ccID, filesToSend, $(this));
        }
    });

    function uploadImages(ccID, ccImages, trigger){
        $.ajax({
            type: "POST",
            url: "/views/kioskManagement/machineTemplate/main.aspx/uploadImages",
            contentType: "application/json; charset=utf-8",
            data: JSON.stringify({ccID: ccID, ccImages: ccImages}),
            dataType: "json",
            beforeSend: function(){
                trigger.attr('disabled',true);
            },
            success: function(data){
                var StatusDescription = data.d.Description;
                var ResultStatus = data.d.Status;
                                
                trigger.attr('disabled',false);

                if(!ResultStatus){
                    alert(StatusDescription);
                    return false;
                }

                alert("Successful. Your action has been sent to checker for approval.");

                filesToSend = [];
                $('#inputEdit_UploadImages').val("");
                //loadImagesOfCodeCategory(ccID);
            },
            error: function(error){
                alert("Error occurs while uploading images.");
                trigger.attr('disabled',false);
            }
        });
    }

    $('#img-uploaded-div').off().on('click', '.outer-thumbnail-container', onImageSelected);

    function onImageSelected(){

        if($(this).hasClass('selected-img')){
            $(this).removeClass('selected-img');
        }
        else{
            $(this).addClass('selected-img');
        }
    }

    $('#selectAllImages').click(function(){
        var selectedAll = $(this).attr('data-flag');

        if(selectedAll.toUpperCase() == 'FALSE'){
            $(this).addClass('highlighted-link');
            $('#img-uploaded-div .outer-thumbnail-container').addClass('selected-img');
            $(this).attr('data-flag','true');
        }
        else{
            $(this).removeClass('highlighted-link');
            $('#img-uploaded-div .outer-thumbnail-container').removeClass('selected-img');
            $(this).attr('data-flag','false');
        }
    });

    var selectedImages = [];

    $('#removeImageBtn').click(function(){
        
        selectedImages = [];

        if($('#img-uploaded-div .selected-img').length == 0){
            alert("Please select image(s).");
        }
        else{
            
            var confirmed = confirm("Are you sure you want to remove selected image(s)?");

            if(confirmed){

                $('#img-uploaded-div .selected-img').each(function(){
            
                    var image = {
                        imgID : $(this).find('input[type="hidden"]').val()
                    };

                    selectedImages.push(image);
                });

                removeSelectedImages(selectedImages, $(this));
            }
            else{
                return false;
            }
        }
    });

    function removeSelectedImages(selectedImages, trigger){
        $.ajax({
            type: "POST",
            url: "/views/kioskManagement/machineTemplate/main.aspx/removeImages",
            contentType: "application/json; charset=utf-8",
            data: JSON.stringify({selectedImages: selectedImages}),
            dataType: "json",
            beforeSend: function(){
                trigger.attr('disabled',true);
            },
            success: function(data){
                var StatusDescription = data.d.Description;
                var ResultStatus = data.d.Status;

                trigger.attr('disabled',false);

                if(!ResultStatus){
                    alert(StatusDescription);
                    return false;
                }
                
                alert("Successful. Your action has been sent to checker for approval.");
                
                selectedImages = [];

                var ccID = getCodeCategoryID();

                if(ccID == "" || ccID == undefined || ccID == null){
                    alert("Failed. Please reload and try again later.");
                    return false;
                }

                loadImagesOfCodeCategory(ccID);
            },
            error: function(error){
                alert("Error occurs while removing images from database.");
                trigger.attr('disabled',false);
            }
        });
    }
});