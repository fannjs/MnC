(function($) {

    /*accepts key-value pair or array*/
    $.fn.initDropDown = function (data) {
        processKvPair(this, data);
        return this; //chainable 
    };


    $.fn.initDropDownFormat = function (data, displayFormat) {
        processKvPairFormat(this, data, displayFormat);
        return this; //chainable 
    };

    //better one:use this next time and eliminate the rest!

    $.fn.initDropDownWithFormat = function (data, displayFormat) {
        processKvPairWithFormat(this, data, displayFormat);
        return this; //chainable 
    };

    function processKvPairWithFormat($select, obj, displayFormat) {
        var $option = {};
        displayFormat = displayFormat || {};
        var valFormat = displayFormat.valFormat;
        var textFormat = displayFormat.textFormat;

        $select.empty();
        for (var key in obj) {
            // spesifically to filter object's unnecessarily properties
            if (!obj.hasOwnProperty(key)) {
                continue;
            }

            $option = $('<option />').val((valFormat) ? valFormat(key, obj) : key).append((textFormat) ? textFormat(key, obj) : obj[key]);
            $select.append($option);
        }
    };

    function processKvPairFormat($select, obj, displayFormat) {
        var $option = {};

        $select.empty();
        for (var key in obj) {
            // spesifically to filter object's unnecessarily properties
            if (!obj.hasOwnProperty(key)) {
                continue;
            }

            $option = $('<option />').val(key).append(displayFormat(obj[key]));
            $select.append($option);
        }
    };

    function processKvPair($select, kvPair) {
        var $option;
        $select.empty();
        for (var key in kvPair) {
            // spesifically to filter object's unnecessarily properties
            if (!kvPair.hasOwnProperty(key)) {
                continue;
            }
            $option = $('<option />').val(key).append(kvPair[key]);
            $select.append($option);
        }
    };

}(jQuery));