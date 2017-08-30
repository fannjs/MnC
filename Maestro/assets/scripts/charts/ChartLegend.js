function legend(parent, data) {
    parent.className = 'legend';
    var datas = data.hasOwnProperty('datasets') ? data.datasets : data;

    // remove possible children of the parent
    while (parent.hasChildNodes()) {
        parent.removeChild(parent.lastChild);
    }

    //var tblStatus = document.createElement('table');
    //parent.appendChild(tblStatus);
    //var trStatus = document.createElement('tr');
    //tblStatus.appendChild(trStatus);

    var countStatus = 0;
    var htmlStatus = "";
    datas.forEach(function (d) {

        if (countStatus == 0) {
            countStatus = 1;
            //var tdStatus = document.createElement('td');
            //trStatus.appendChild(tdStatus);
            //var title = document.createElement('span');
            //title.className = 'title';
            //tdStatus.appendChild(title);
            //var colorSample = document.createElement('div');
            //colorSample.className = 'color-sample';
            //colorSample.style.backgroundColor = d.hasOwnProperty('strokeColor') ? d.strokeColor : d.color;
            //colorSample.style.borderColor = d.hasOwnProperty('fillColor') ? d.fillColor : d.color;
            //title.appendChild(colorSample);
            //var text = document.createTextNode(d.label + " = " + d.percent + "%");
            //title.appendChild(text);

            var bgColor = d.hasOwnProperty('strokeColor') ? d.strokeColor : d.color;
            var borderColor = d.hasOwnProperty('fillColor') ? d.fillColor : d.color;
            var text = d.label + " = " + d.percent + "%";
            htmlStatus = htmlStatus+ '<tr><td ><span class="title"><div class="color-sample" '+
                    ' style="border-color: ' + bgColor + '; background-color: ' + borderColor + '; ">' +
                    ' </div>' + text + '</span></td>';
        } else {
            countStatus = 0;
            var bgColor = d.hasOwnProperty('strokeColor') ? d.strokeColor : d.color;
            var borderColor = d.hasOwnProperty('fillColor') ? d.fillColor : d.color;
            var text = d.label + " = " + d.percent + "%";

            htmlStatus = htmlStatus + '<td ><span class="title"><div class="color-sample" ' +
                    ' style="border-color: ' + bgColor + '; background-color: ' + borderColor + '; ">' +
                    ' </div>' + text + '</span></td></tr>';
        }
        //alert(htmlStatus);
        //tblStatus.appendChild(htmlStatus);
    });
    htmlStatus = "<table width='100%'> " + htmlStatus + "</table>";
    //parent.appendChild(htmlStatus);
    $('#pieLegend').append(htmlStatus);
}

function legend_lineChart(parent, data) {
    parent.className = 'legend';
    var datas = data.hasOwnProperty('datasets') ? data.datasets : data;

    // remove possible children of the parent
    while (parent.hasChildNodes()) {
        parent.removeChild(parent.lastChild);
    }

    //var tblStatus = document.createElement('table');
    //parent.appendChild(tblStatus);
    //var trStatus = document.createElement('tr');
    //tblStatus.appendChild(trStatus);

    var countStatus = 0;
    var htmlStatus = "";
    datas.forEach(function (d) {

        if (countStatus == 0) {
            countStatus = 1;

            var bgColor = d.hasOwnProperty('strokeColor') ? d.strokeColor : d.color;
            var borderColor = d.hasOwnProperty('strokeColor') ? d.strokeColor : d.color;
            var text = d.label ;
            htmlStatus = htmlStatus + '<tr><td ><span class="title"><div class="color-sample" ' +
                    ' style="border-color: ' + bgColor + '; background-color: ' + borderColor + '; ">' +
                    ' </div>' + text + '</span></td>';
        } else {
            countStatus = 0;
            var bgColor = d.hasOwnProperty('strokeColor') ? d.strokeColor : d.color;
            var borderColor = d.hasOwnProperty('strokeColor') ? d.strokeColor : d.color;
            var text = d.label ;

            htmlStatus = htmlStatus + '<td ><span class="title"><div class="color-sample" ' +
                    ' style="border-color: ' + bgColor + '; background-color: ' + borderColor + '; ">' +
                    ' </div>' + text + '</span></td></tr>';
        }
    });
    htmlStatus = "<table width='100%'> " + htmlStatus + "</table>";
    $('#lineLegend_machid').append(htmlStatus);
}