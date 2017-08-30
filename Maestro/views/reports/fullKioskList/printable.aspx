<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="printable.aspx.cs" Inherits="Maestro.views.reports.fullKioskList.printable" %>

<html xmlns="http://www.w3.org/1999/xhtml">
<head>
    <script src="../../../components/jquery/jquery.js"></script>
    <script type="text/javascript">
        function loadKioskList() {

            $.ajax({
                type: "POST",
                contentType: "application/json; charset=utf-8",
                url: "/views/reports/fullKioskList/printable.aspx/getMachineList",
                data: "{}",
                dataType: "json",
                success: function (data) {
                    var str = "";

                    str = str + '<table><thead><th>No.</th><th>Kiosk ID</th><th>State</th><th>City</th><th>Address1</th><th>Address2</th><th>Branch No.</th><th>Contact Person</th></thead>';
                    str = str + '<tbody>';

                    for (var i = 0; i < data.d.length; i++) {
                        str = str + '<tr><td>' + (i + 1) + '</td><td>' + data.d[i].MachID + '</td><td>' + data.d[i].MState + '</td><td>' + data.d[i].MCity + '</td>';
                        str = str + '<td>' + data.d[i].MAddress1 + '</td><td>' + data.d[i].MAddress2 + '</td><td>' + data.d[i].MBranch + '</td><td>' + data.d[i].MPIC + '</td></tr>';
                    }

                    str = str + '</tbody></table>';

                    $('body').html(str);

                    window.print();
                },
                error: function (error) {
                    alert("Some problem occurs. Unable to print.");
                }
            });
        }
    </script>
    <style type="text/css">
        table
        {
            font-size: 12px;
            border-collapse: collapse;
            border-spacing: 0px;
        }
        td
        {
            border-top: 1px solid #000;
            border-left: 1px solid #000;
            border-bottom: 1px solid #000;
            border-right: 1px solid #000;
        }
    </style>
</head>
<body onload="loadKioskList()">
</body>
</html>
