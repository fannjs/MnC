<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="MachTemplate.aspx.cs" Inherits="Maestro.app_monitoring.content.MachTemplate1" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title></title>
	<link href="../Styles/smoothness/jquery-ui-1.10.3.custom.css" rel="stylesheet" />
    <link href="../Styles/dev/MachTemplate.css" rel="stylesheet" />
	<script type="text/javascript" src="../js/lib/jquery/jquery-1.9.1.js"></script>
	<script type="text/javascript" src="../js/lib/jquery/jquery-ui-1.10.3.custom.js"></script>
    <script type="text/javascript" src="../js/lib/json/json2.js"></script>
    <script type="text/javascript" src="../js/dev/MachTemplate.js"></script>

</head>
<body>
    <form id="form1" runat="server">

    
        <div class="wrapper"> 
            <a id="addNewMachineType" class="blueSubLink" href="#">Add New Machine Type</a>
            <fieldset>
                <legend>Machine Template List</legend>
                <table id="tblMachTempList">
                    <thead>
                        <tr class="theader">
                            <td>MACHINE TYPE</td>
                            <td>MODEL</td>
                            <td>ASSIGNED IMAGE</td>
                        </tr>
                    </thead>
                    <tbody>
                    </tbody>
                </table>
            </fieldset>
        </div>
    </form>


    <div class="hidden dlgContent" id="dlgAddNewMachineType">

    </div>
    <div class="hidden dlgContent" id="dlgAEVErrorCodes">

    </div>

</body>
</html>