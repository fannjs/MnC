function SensorList ( id ) {
	strict([String], arguments);

	var id = id;
	var sensors = [];


	this.addSensor = function ( sensor ) {
		strict([Sensor], arguments);

		sensors.push(sensor);
	};

	this.getSensor = function ( idx ) {
		return sensors[idx];
	};

	this.getSensors = function () {
		return sensors;
	};

	this.isEmpty = function () {
		return sensors.length < 1;
	}
}

function Sensor ( cssName, pos ) {
	strict([String, Position], arguments);

	this.cssName = cssName;
	this.pos = pos;
}


//Class Pos
function Position( top, left ) {
	this.top = top;
	this.left = left;
}



//util functions
function strict( types, args ) {

	if( types.length !== args.length ) {
		throw 'Arguments\' length unmatched';
	}


	for( var i = 0; i < args.length ; i++ ) {
		if( types[i] !== args[i].constructor ) {
			throw 'Invalid argument type. Expected ' + types[i].name + 
				  ', received ' + args[i].constructor.name + ' instead';
		}
	}
}

