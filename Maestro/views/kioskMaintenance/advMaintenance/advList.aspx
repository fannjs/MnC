<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="advList.aspx.cs" Inherits="Maestro.views.kioskMaintenance.advMaintenance.advList" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        
    <div>
        <form class="form-horizontal well" role="form" action="javascript:;">
            <fieldset>
                <legend>Select Files/Zip to upload</legend>
                <div class="form-group">
                    <label for="inputSiteCode" class="col-xs-2 control-label">1</label>
                    <div class="col-xs-4">
                        <input type="text" class=" form-control" id="inputSiteCode" placeholder="File name"/>
                    </div>
                     <button type="button" id="Button1" class="btn btn-default">Browse</button>
                </div>
                <div class="form-group">
                    <label for="inputSiteCode" class="col-xs-2 control-label">2</label>
                    <div class="col-xs-4">
                        <input type="text" class=" form-control" id="Text1" placeholder="File name"/>
                    </div>
                     <button type="button" id="Button2" class="btn btn-default">Browse</button>
                </div>
                <div class="form-group">
                    <label for="inputSiteCode" class="col-xs-2 control-label">3</label>
                    <div class="col-xs-4">
                        <input type="text" class=" form-control" id="Text2" placeholder="File name"/>
                    </div>
                     <button type="button" id="Button3" class="btn btn-default">Browse</button>
                <button type="submit" class="btn btn-primary" onclick="addNewUpload()">Add New</button>
                </div>
                <div class="form-group">
                    <div class="col-xs-offset-2 col-xs-4">
                        <button type="button" class="btn btn-default" onclick="addCustomer()">Upload</button>
                        <button type="button" id="cancel-button" class="btn btn-default">Cancel</button>
                    </div>
                </div>
            </fieldset>
        </form>
    </div>
    <hr>
    <table class="table" id="tbDetails">
        <thead>
            <tr>
                <th>No.</th>
                <th>File Name</th>
                <th></th>
            </tr>
        </thead>
        <tbody>
            <tr><td>1</td><td>ABC.jpg</td><td>&nbsp;<a class="deleteUser-button" href="javascript:;" onclick="deleteUser(1)"><i class="fa fa-trash-o"></i>Delete</a></td> </tr>
            <tr><td>2</td><td>BCD.png</td><td>&nbsp;<a class="deleteUser-button" href="javascript:;" onclick="deleteUser(43)"><i class="fa fa-trash-o"></i>Delete</a></td> </tr>
            <tr><td>3</td><td>CDE.avi</td><td>&nbsp;<a class="deleteUser-button" href="javascript:;" onclick="deleteUser(44)"><i class="fa fa-trash-o"></i>Delete</a></td> </tr>
            <tr><td>4</td><td>DEF.mp4</td><td>&nbsp;<a class="deleteUser-button" href="javascript:;" onclick="deleteUser(45)"><i class="fa fa-trash-o"></i>Delete</a></td> </tr>
            <tr><td>5</td><td>ABC.jpg</td><td>&nbsp;<a class="deleteUser-button" href="javascript:;" onclick="deleteUser(1)"><i class="fa fa-trash-o"></i>Delete</a></td> </tr>
            <tr><td>6</td><td>BCD.png</td><td>&nbsp;<a class="deleteUser-button" href="javascript:;" onclick="deleteUser(43)"><i class="fa fa-trash-o"></i>Delete</a></td> </tr>
            <tr><td>7</td><td>CDE.avi</td><td>&nbsp;<a class="deleteUser-button" href="javascript:;" onclick="deleteUser(44)"><i class="fa fa-trash-o"></i>Delete</a></td> </tr>
            <tr><td>8</td><td>DEF.mp4</td><td>&nbsp;<a class="deleteUser-button" href="javascript:;" onclick="deleteUser(45)"><i class="fa fa-trash-o"></i>Delete</a></td> </tr>
            <tr><td>9</td><td>ABC.jpg</td><td>&nbsp;<a class="deleteUser-button" href="javascript:;" onclick="deleteUser(1)"><i class="fa fa-trash-o"></i>Delete</a></td> </tr>
            <tr><td>10</td><td>BCD.png</td><td>&nbsp;<a class="deleteUser-button" href="javascript:;" onclick="deleteUser(43)"><i class="fa fa-trash-o"></i>Delete</a></td> </tr>
        </tbody>
    </table>
</div>
    </form>
</body>
</html>
