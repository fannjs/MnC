"use strict";

var config = {
    "path": "/assets/scripts/configs/json/",
    "jsons": [
        {
            "name": "countries",
            "description": "a list of countries and states",
            "file": "countries.js"
        }
    ]
};
var __CONFIGFACTORY = null; //to be global

jQuery(document).ready(function ($) {

    function ConfigFactory(data) {
        data = data || {};
        ConfigFactory.prototype.countries = [];
        ConfigFactory.prototype.jsons = data.jsons || {};
        ConfigFactory.prototype.states = {};


        function $getJsonByFileName(jsonFileName, onCallBack) {
            console.log(jsonFileName);

            $.getJSON(jsonFileName, onCallBack);
        };

        function $initCountries(world) {
            var temp = {};
            var clsConfig = ConfigFactory.prototype; //to save config class's scope, in order to access in $.each()

            $.each(world[0].SimpleGeoName.Children.SimpleGeoName, function(iArrContinents, arrContinents) {
                $.each(arrContinents.Children.SimpleGeoName, function(iArrCountries, countryInfo) {
                    ConfigFactory.prototype.countries.push(countryInfo.Name);

                    if (countryInfo.Children) {//not all countries have states/province
                        ConfigFactory.prototype.states[countryInfo.Name] = countryInfo.Children.SimpleGeoName;
                    }

                });
            });

            ConfigFactory.prototype.countries.sort();
        };
        $getJsonByFileName(data.path + ConfigFactory.prototype.jsons[0].file, $initCountries);

    }

    __CONFIGFACTORY = new ConfigFactory(config);

});
/*

            $.each(world[0].SimpleGeoName.Children.SimpleGeoName, function (iArrContinents, arrContinents) {
                $.each(arrContinents.Children.SimpleGeoName, function (iArrCountries, countryInfo) {
                    if (countryInfo.Children) {//not all countries have states/province
                        $.each(countryInfo.Children.SimpleGeoName, function (iStateInfo, stateInfo) {
                            //console.log(stateInfo.Name);
                        });
                    }
                });
            });
[
    {
        "name": "Malaysia",
        "currency": "RM",
        "states":
                [
                    "Kuala Lumpur",
                    "Labuan",
                    "Putrajaya",
                    "Johor",
                    "Kedah",
                    "Kelantan",
                    "Malacca",
                    "Negeri Sembilan",
                    "Pahang",
                    "Perak",
                    "Perlis",
                    "Penang",
                    "Sabah",
                    "Sarawak",
                    "Selangor",
                    "Terengganu"
                ]
    }


]*/

/* we would have to search by world -> continent(s) -> countrie(s) -> state(s)/province(s)*/