jQuery(document).ready(function ($) {
    var mID = null;

	$('button#btnResetSensor').click(function () {

		$('.sensorField').reset();

		if(mID != null)
		    GetMachineErrorInner(mID);
	});

	$('button#btnSaveSensor').click(function () {

		if(mID == null) {
			return;
		}


		var sensorList = new SensorList(mID);	

		$('.sensor').each(function(i, sensor) {
			var sensorCls = $(sensor).find('div').attr('class');
			var sensorPos = $(sensor).position();

			sensorList.addSensor( new Sensor(sensorCls, new Position(sensorPos.top, sensorPos.left)));
	
		});
		PutMachineError(mID, sensorList.getSensors());

	});

	//save coords to db
	$('div#dlgEditSensor').bind('dialogclose', function(event) {
	     $('.sensorField').reset();
	     mID = null;
	 });

	//load coords from db
	$('div#dlgEditSensor').bind('dialogopen', function(event) {
	    
	    $('.sensorField').reset();

	   	mID = $(this).dialog('option','title').split('-')[1].trim();
	   	GetMachineErrorInner(mID);
	 
	 });	

	$('.itemDrag').draggable({
		helper : 'clone',
		revert: 'invalid' 
	});



	$('.sensorField').droppable({

		accept: '.itemDrag',
		tolerance: 'fit',
		drop: function(e, ui){
			var $this = $(this); 
			var $newSensor = ui.draggable.clone();
			var relPos = calcRelativePos(ui.helper, $this);
            
			$newSensor = $newSensor.removeClass($newSensor.attr('class')).addClass('sensor').attachBlink();//reset
			$this.addNewDraggable($newSensor, relPos);
		}
	});

	//data access

    //Updates the ErrorCodes Details in the List
    function PutMachineError( id, coordList ) {
		jQuery.support.cors = true;

        var machineerror = {
            M_CODE: id,
            M_SENSOR_XY: JSON.stringify(coordList),
        };

        //note by roy: we could ignore the age prop

        $.ajax({
            url: '/api/machineerror/' + id,
            type: 'PUT',
            data: JSON.stringify(machineerror),
            contentType: "application/json;charset=utf-8",
            success: function (data) {
                alert('Details Updated Successfully');
            },
            error: function (data) {
                console.log(data);
                alert('Unable to Update for the Given ID');
            }
        });
    }



	//functions

	function calcRelativePos($inner, $parent){

	    var relPos = {};
	    relPos.top = Math.abs($inner.offset().top - $parent.offset().top);
	    relPos.left = Math.abs($inner.offset().left - $parent.offset().left);

	    return relPos;

	}
	
	function getMaxZindex($group)
	{
		var maxZindex = 0;
		$.each($group, function(i, item) {
			var currZindex = $(item).css('z-index') ;
			if(currZindex > maxZindex) {
				maxZindex  = currZindex;
			}
		});

		return maxZindex;
	}

	$.fn.incrZIndex = function($group) {
	 	this.css('z-index', parseInt(getMaxZindex($group)) + 1);
		return this;
	}


	$.fn.attachCore = function(cssName) {
		
		var $core = $('<div>');   
	    $core.addClass(cssName);
		this.append($core);

		console.log(cssName);

	    return this;
	}

	//works for arrow only
	$.fn.attachBlink = function() {
		var $core = this.find('div');
	    $core.addClass('blink_' + $core.attr('class'));

	    return this;
	}


	//works for arrow only
	$.fn.removeBlink = function() {
		var $core = this.find('div');
		var core  = $core[0];//js object

		//regex anything start with blink_
		var matches = core.className.match(/blink_\w+/);

		if(matches) {
			$core.removeClass(matches[0]);
		}

	    return this;
	}

	$.fn.reset = function () {
		var $core = this;

		var $img = $core.find('img');
		$core.empty();
		$core.append($img);

	}


	$.fn.rounded = function() {
		//that is js obj
		var that = this.get(0);

		that.top = Math.round(that.top);
		that.left = Math.round(that.left);
	}

	$.fn.addNewDraggable = function($newItem, pos)
	{
		var $parent = this;
		
		//formatting pos within parent
		$newItem.css('position', 'absolute')
			 .css('z-index', 0)
			 .css('top', pos.top)
		     .css('left', pos.left);

		$newItem.draggable({
			'containment': $parent,
			drag: function(e, ui) {
				var dragPos = ui.position;
				$(dragPos).rounded();
				//$('#txtCoord').val('X: ' + dragPos.left + ', ' + 'Y: ' + dragPos.top);

			},
			stop: function(e, ui) {
				$(this).css('opacity', 1);
			}
		});

		//to be distinguished among the rest
		$newItem.mousedown(function(e) {
			var $otherItems = $parent.find('.sensor');//must always be called
		 	$(this).incrZIndex($otherItems)
		 		   .removeBlink()
		 		   .css('opacity', 1);

		});

		$newItem.mouseup(function(e) {
			$(this).attachBlink();	
		});

		$parent.append($newItem);
		
		return this;
	}

	function GetMachineErrorInner(id) {
	    mID = id;
	    //note by roy: we could ignore the age prop
	    $.ajax({
	        url: '/api/machineerror/' + id,
	        type: 'GET',
	        contentType: "application/json;charset=utf-8",
	        success: onMachineErrorReturn,
	        error: function () {
	            alert('Unable to Update for the Given ID');
	        }
	    });
	}

	$.fn.GetMachineError = function (id) {
	    

	    GetMachineErrorInner(id);
	};

	function onMachineErrorReturn(machineError) {

	    console.log('masul on machineerror return');
	    var sensorObj = machineError.M_SENSOR_XY;
	    if (sensorObj == null) {
	        return;
	    }

	    sensorObj = $.parseJSON(sensorObj);


	    var sensorList = new SensorList(machineError.M_CODE);

	    $.each(sensorObj, function (i, sensorProp) {

	        console.log(sensorProp);

	        sensorList.addSensor(new Sensor(sensorProp.cssName, new Position(sensorProp.pos.top, sensorProp.pos.left)));
	    });


	    $.each(sensorList.getSensors(), function (i, sensor) {

	        var $sensor = $('<div>').addClass('sensor').attachCore(sensor.cssName);
	        $('.sensorField').addNewDraggable($sensor, sensor.pos);
	    });
	}

});	


