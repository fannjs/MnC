<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="advAssignedList.aspx.cs" Inherits="Maestro.views.kioskMaintenance.advMaintenance.advAssignedList" %>


<script type="text/javascript">
    var pUserid;
    function triggerUploadNewAdv() {
        $.post("/views/kioskMaintenance/advMaintenance/advList.aspx", function (data) {
            $("#content-subpage-container").html(data);
            showSubPage();
        });
    }

    function triggerNewAssign() {
        $.post("/views/kioskMaintenance/advMaintenance/advAssign.aspx", function (data) {
            $("#content-subpage-container").html(data);
            showSubPage();
        });
    }

    function triggerNewAssign2() {
        $.post("/views/kioskMaintenance/advMaintenance/advAssignment.aspx", function (data) {
            $("#content-subpage-container").html(data);
            showSubPage();
        });
    }

    function triggerEditAssign() {
        $.post("/views/kioskMaintenance/advMaintenance/advEdit.aspx", function (data) {
            $("#content-subpage-container").html(data);
            showSubPage();
        });
    }


    $(document).ready(function () {
        $('#uploadNewAdv-button').click(function () {
            triggerUploadNewAdv();
        })
        $('#NewAdvAssign-button').click(function () {
            triggerNewAssign();
        })
        $('#NewAdvAssignment-button').click(function () {
            triggerNewAssign2();
        })
        $('#EditAdvAssignment-button').click(function () {
            triggerEditAssign();
        })
    });
</script>

        
<div>
    <div id="addButton">        
        <a href="javascript:;" id="uploadNewAdv-button" class="func-btn"><i class="fa fa-plus"></i>Upload New Advert</a> | 
        <%--<a href="javascript:;" id="NewAdvAssign-button" class="func-btn"><i class="fa fa-plus"></i>Advert Assignment</a> | --%>
        <a href="javascript:;" id="NewAdvAssignment-button" class="func-btn"><i class="fa fa-plus"></i>Advert Assignment</a>
        
    </div>
    <hr>
    <table class="table" id="tbDetails">
        <thead>
            <tr>
                <th>Machine ID</th>
                <th>Advert Sequence</th>
                <th>File Name</th>
                <th></th>
            </tr>
        </thead>
        <tbody>
        <tr><td>0101</td><td>010203040506070809100000000000</td><td>CSDAdv0101.zip</td><td><a href="javascript:;" id="EditAdvAssignment-button" class="func-btn"><i class="fa fa-plus"></i>Edit</a>  <span class="center-divider"></span><a class="deleteUser-button" href="javascript:;" onclick="deleteUser(1)"><i class="fa fa-trash-o"></i>Delete</a></td> </tr>
        <tr><td>0102</td><td>010203040506070809101100000000</td><td>CSDAdv0102.zip</td><td><a href="javascript:;" class=".editUser-button" id="A2" onclick="editUser(43)"><i class="fa fa-plus"></i>Edit</a>  <span class="center-divider"></span><a class="deleteUser-button" href="javascript:;" onclick="deleteUser(43)"><i class="fa fa-trash-o"></i>Delete</a></td> </tr>
        <tr><td>1001</td><td>010203040000000000000000000000</td><td>CSDAdv1001.zip</td><td><a href="javascript:;" class=".editUser-button" id="A3" onclick="editUser(44)"><i class="fa fa-plus"></i>Edit</a>  <span class="center-divider"></span><a class="deleteUser-button" href="javascript:;" onclick="deleteUser(44)"><i class="fa fa-trash-o"></i>Delete</a></td> </tr>
        <tr><td>1002</td><td>010203040506070809101112131415</td><td>CSDAdv1002.zip</td><td><a href="javascript:;" class=".editUser-button" id="A4" onclick="editUser(45)"><i class="fa fa-pencil"></i>Edit</a>  <span class="center-divider"></span><a class="deleteUser-button" href="javascript:;" onclick="deleteUser(45)"><i class="fa fa-trash-o"></i>Delete</a></td> </tr></tbody>
        <tr><td>2200</td><td>010203040506070809101112131415</td><td>CSDAdv2200.zip</td><td><a href="javascript:;" class=".editUser-button" id="A5" onclick="editUser(1)"><i class="fa fa-pencil"></i>Edit</a>  <span class="center-divider"></span><a class="deleteUser-button" href="javascript:;" onclick="deleteUser(1)"><i class="fa fa-trash-o"></i>Delete</a></td> </tr>
        <tr><td>2201</td><td>010203040506070809101100000000</td><td>CSDAdv2201.zip</td><td><a href="javascript:;" class=".editUser-button" id="A6" onclick="editUser(43)"><i class="fa fa-pencil"></i>Edit</a>  <span class="center-divider"></span><a class="deleteUser-button" href="javascript:;" onclick="deleteUser(43)"><i class="fa fa-trash-o"></i>Delete</a></td> </tr>
        <tr><td>2211</td><td>010203040000000000000000000000</td><td>CSDAdv2211.zip</td><td><a href="javascript:;" class=".editUser-button" id="44" onclick="editUser(44)"><i class="fa fa-pencil"></i>Edit</a>  <span class="center-divider"></span><a class="deleteUser-button" href="javascript:;" onclick="deleteUser(44)"><i class="fa fa-trash-o"></i>Delete</a></td> </tr>
        <tr><td>2345</td><td>010203040506070809101112131415</td><td>CSDAdv2345.zip</td><td><a href="javascript:;" class=".editUser-button" id="45" onclick="editUser(45)"><i class="fa fa-pencil"></i>Edit</a>  <span class="center-divider"></span><a class="deleteUser-button" href="javascript:;" onclick="deleteUser(45)"><i class="fa fa-trash-o"></i>Delete</a></td> </tr></tbody>
        <tr><td>3344</td><td>010203040506070809101112131415</td><td>CSDAdv3344.zip</td><td><a href="javascript:;" class=".editUser-button" id="1" onclick="editUser(1)"><i class="fa fa-pencil"></i>Edit</a>  <span class="center-divider"></span><a class="deleteUser-button" href="javascript:;" onclick="deleteUser(1)"><i class="fa fa-trash-o"></i>Delete</a></td> </tr>
        <tr><td>3456</td><td>010203040506070809101100000000</td><td>CSDAdv3456.zip</td><td><a href="javascript:;" class=".editUser-button" id="43" onclick="editUser(43)"><i class="fa fa-pencil"></i>Edit</a>  <span class="center-divider"></span><a class="deleteUser-button" href="javascript:;" onclick="deleteUser(43)"><i class="fa fa-trash-o"></i>Delete</a></td> </tr>
    </table>
</div>
