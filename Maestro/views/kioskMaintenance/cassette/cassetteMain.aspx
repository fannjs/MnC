<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="cassetteMain.aspx.cs" Inherits="Maestro.views.kioskMaintenance.cassette.cassetteMain" %>

<link rel="stylesheet" href="../../../assets/styles/mahineslist.css" />
<style type="text/css">
    /*#modal-wrapper
    {
        position: fixed;
        margin: 0;
        padding: 0;
        top: 0;
        left: 0;
        width: 100%;
        height: 100%;
        z-index: 10000;
        overflow: auto;
        
        background-color: rgba(0,0,0,0.5);
        display: none;
    }
    #modal-wrapper .modal-dialog
    {
        width: 800px;
        margin: 2em auto;   
        background-color: #FFF;    
        border-radius: 2px;
        border: 1px solid #EEE; 
        box-shadow: 0px 8px 20px 0px;
    }
    #modal-wrapper .modal-dialog .modal-title
    {
        font-size: 16px;
    }
    #modal-wrapper .modal-close-btn
    {
        float:right;
        color: #000;
        opacity: 0.5;
        
        transition: opacity ease-in-out 0.25s;
        -webkit-transition: opacity ease-in-out 0.25s;
    }
    #modal-wrapper .modal-close-btn:hover
    {
        cursor:pointer;
        opacity: 0.8;
    }
    #modal-wrapper .modal-close-btn:active
    {
        opacity: 1;
    }
    #userListMainDiv > div, #statusCodesMainDiv > div
    {
        border: 1px solid #DDD;
        border-radius: 4px;
        padding: 8px;
    }
    #userListMainDiv .container-title, #statusCodesMainDiv .container-title
    {
        font-size: 16px;
        color: #999;
    }
    #userListDiv, #statusCodesDiv
    {
        overflow: auto;
        min-height: 140px;
    }
    #userListDiv .a-user, #statusCodesDiv .a-code
    {
        display: inline-block;
        padding: 2px 12px;
        border: 1px solid #ddd;
        background-color: #F9F9F9;
        border-radius: 4px;
        margin: 2px;
    }
    #userListDiv .a-user:hover, #statusCodesDiv .a-code:hover
    {
        cursor:pointer;
    }
    #userListDiv .a-user.selected, #statusCodesDiv .a-code.selected
    {
        background-color: #5cb85c;
        border-color: #4cae4c;
        color: #fff;
        font-weight: bold;
    }
    #userListDiv .a-user.selected.disabled, #statusCodesDiv .a-code.selected.disabled
    {
        opacity: 0.6;
        cursor: default;
    }*/
    
    .item-listing-container
    {
        display: inline-block;
        padding: 4px;
        border: 1px solid #999;
        background-color: #F4f4f4;
        height: 150px;
        overflow: auto;
        
        vertical-align: top;
    }
    .item-listing-container .item-listing
    {
        border: 1px solid #7B97BC;
        background-color: #A9BFDC;
        border-radius: 2px;
        padding: 0px 4px;
        color: #135FC4;
        font-size: 14px;
        position: relative;
    }
    .item-listing-container .item-listing:hover
    {
        background-color: #96B5DE;
    }
    .item-listing-container .item-listing:not(:last-child)
    {
        margin-bottom: 2px;
    }
    .item-listing-container .remove-btn
    {
        color: #555;
        opacity: 0.7;
        position: absolute;
        left: calc(100% - 17px);
        top:0px;
        
        transition: opacity ease-in-out 0.25s;
        -webkit-transition: opacity ease-in-out 0.25s;
    }
    .item-listing-container .remove-btn:hover
    {
        opacity: 0.9;
        cursor: pointer;
    }
    .item-listing-container .remove-btn:active
    {
        opacity: 1;
    }
    .item-listing-container .remove-btn .fa
    {
        width: auto;
    }
    #machine-list-main
    {
        height: 150px;
        overflow:auto;
    }
    #machine-list-main .a-machine
    {
        border: 1px solid #999;
        text-align: center;
        margin-bottom: 6px;
        margin-right: 6px;
        border-radius: 4px; 
    }    
    #machine-list-main .a-machine:hover
    {
        background-color: #FAFAFA;
        border-color: #888;
        cursor:pointer;
    }
    #machine-list-main .a-selected-machine-id
    {
        background-color: #5cb85c;
        border-color: #4cae4c;
        color: #fff;
        font-weight: bold;
    }
    #machine-list-main .a-selected-machine-id:hover
    {
        background-color: #53A653;
        border-color: #3D8B3D;
        cursor:pointer;
    }
</style>
<div id="cassette-main-div">
    <div>
        <a class="func-btn ng-isolate-scope" id="btnAddAssignCassette" data-toggle="modal" data-target="#addCassetteModal" style="cursor: pointer;"><i class="fa fa-plus"></i>Add & Assign Cassette</a>            
    </div>
    <div class="block-xs"></div>
    <ul id="pagination" class="pagination">
    </ul>
    <div id="selection-div">
        Show
        <select id="record-per-page">
            <option selected>10</option>
            <option>20</option>
            <option>30</option>
            <option>40</option>
            <option>50</option>
            <option>60</option>
            <option>70</option>
            <option>80</option>
            <option>90</option>
            <option>100</option>
        </select>
        per page
    </div>  
    <table id="full-kiosk-list-table">
        <thead>
            <tr>
                <th>No.</th>
                <th>Kiosk ID</th>
                <th>Cassette ID</th>
                <th>Assigned Date</th>
                <th>Bin in Use</th>
                <th>Last Replenish at</th>
                <th></th>
            </tr>
        </thead>
        <tbody>
        </tbody>
    </table>
    <div id="addMachine">
        <div id="page-footer">
            <span id="page-information"></span>
        </div>
    </div>
</div>

<!-- Modal for list machines-->
<div class="modal" id="addCassetteModal" tabindex="-1" role="dialog" aria-labelledby="myModalLabel" aria-hidden="true">
  <div class="modal-dialog" style="width:800px;">
    <div class="modal-content">
      <div class="modal-header">
        <button type="button" class="close" data-dismiss="modal"><span aria-hidden="true">&times;</span><span class="sr-only">Close</span></button>
        <h4 class="modal-title" id="H1">Add & Assign Cassette</h4>
      </div>
      <div class="modal-body">            
                    
                    <div > <%-- cassette.--%>
                        
                            <form class="form-horizontal" action="javascript:;">
                                <div id="addAssignCassetteMainDiv"> <%-- cassette.--%>
                                <fieldset>
                                    <div class="form-group">
                                        <label class="col-xs-3 control-label">Kiosk Type</label>
                                        <div class="col-xs-4">
                                        <select id="selectKioskType" onchange="" onclick="" class="form-control" runat="server">
                                            <option disabled selected value=""> - Please Select - </option>
                                        </select>
                                        </div>
                                    </div>
                                    <div class="form-group">
                                        <label class="col-xs-3 control-label">Cassette Name</label>
                                        <div class="col-xs-4">
                                            <input type="text" class="form-control" id="txtAddCassetteName" placeholder="Cassette Name">
                                        </div>
                                        <div class="col-xs-2">
                                            <button class="btn btn-primary" id="btnAddCassetteName">Add</button>
                                        </div>
                                    </div>
                                    <div class="form-group">
                                        <label class="col-xs-3 control-label"></label>
                                        <div class="col-xs-4">
                                            <div id="CassetteListing" class="item-listing-container width-lg">
                                                <!-- Cassette listing here -->
                                            </div>
                                        </div>
                                        &nbsp;
                                    </div>
                                    <hr style="margin:10px 0px;" />
                                    <div class="form-group">
                                        <label class="col-xs-3 control-label">Assign to Kiosk</label>
                                        <div class="col-xs-9">
                                            <div id="machine-list-main"> <%-- to show all machine id for selection.--%>

                                            </div>
                                        </div>
                                        <%--<a class="link-btn" title="Add Status Code to the Listing" data-target="StatusCode" data-title="Status Code" onclick="openModal(this);">Add Machine ID</a>
                                        &nbsp;--%>
                                    </div>
                                </fieldset>
                                </div>
                            </form>
                    </div><%-- end cassette.--%>

      </div>
      <div class="modal-footer">
            <button class="btn btn-primary" id="add-btn">Add</button>
            <button type="button" class="btn btn-default" data-dismiss="modal">Cancel</button>
            <button class="btn btn-primary" id="insertVer-btn" style="display:none;">Save</button>
      </div>
    </div>
  </div>
</div>

<!-- Modal for edit cassette list-->
<div class="modal" id="editMachCassetteModal" tabindex="-1" role="dialog" aria-labelledby="myModalLabel" aria-hidden="true">
  <div class="modal-dialog" style="width:800px;">
    <div class="modal-content">
      <div class="modal-header">
        <button type="button" class="close" data-dismiss="modal"><span aria-hidden="true">&times;</span><span class="sr-only">Close</span></button>
        <h4 class="modal-title" id="H2">Configure Cassette</h4>
      </div>
      <div class="modal-body">
            
         <div id="edit-version-body">
                <div>

                    <div> <%-- cassette.--%>
                        
                            <form class="form-horizontal" action="javascript:;">
                                <div id="existingCassetteListingDiv">
                                <fieldset>
                                    <div class="form-group">
                                        <label class="col-xs-3 control-label">Kiosk ID</label>
                                        <div class="col-xs-5">
                                            <input type="text" class=" form-control" id="txtEditKioskID" placeholder="Kiosk ID" readonly>
                                            <input type="hidden" id="txtEditCurrentVer" >
                                        </div>
                                    </div>
                                    <div class="form-group">
                                        <label class="col-xs-3 control-label">Cassette Name</label>
                                        <div class="col-xs-5">
                                            <input type="text" class="form-control" id="txtEditCassetteName" placeholder="Cassette Name">
                                        </div>
                                        <div class="col-xs-2">
                                            <button class="btn btn-primary" id="btnAddCassetteToExistingList">Add</button>
                                        </div>
                                    </div>
                                    <div class="form-group">
                                        <label class="col-xs-3 control-label"></label>
                                        <div class="col-xs-5">
                                            <div id="ExistingCassetteList" class="item-listing-container width-lg">
                                                <!-- Existing Cassette listing here -->
                                            </div>
                                        </div>
                                        &nbsp;
                                    </div>

                                </fieldset>
                                </div>
                            </form>
                    </div><%-- end cassette.--%>
               </div>
          </div>
      </div>
      <div class="modal-footer">
            <button class="btn btn-primary" id="btnUpdateCassette">Update</button>
            <button type="button" class="btn btn-default" data-dismiss="modal">Close</button>
      </div>
    </div>
  </div>
</div>

<script type="text/javascript" src="kioskMaintenance/cassette/cassette.js"></script>

<script type="text/javascript">
    $(document).ready(function () {
        $('#full-kiosk-list-main').off();

        recordPerPage = $('#record-per-page').val();
        getMachineList(pageNumber, recordPerPage);
        pagination();

        
        $('#btnAddAssignCassette').click(function () {
            $('#txtAddCassetteName').val('');
            $('#CassetteListing').empty();
            CassetteNameArray = [];
            CassetteOldArray = [];
            getAllMachines();
        });

        $('#btnAddCassetteName').click(function () {
            // addNewCassetteName();
            addCassetteNameToList();
        });

        $('#btnAddCassetteToExistingList').click(function () {
            addCassetteNameToExistingList();
        });
        
        $('#addAssignCassetteMainDiv').off('click', '.remove-btn', removeItem).on('click', '.remove-btn', removeItem);
        $('#existingCassetteListingDiv').off('click', '.remove-btn', removeExistingItem).on('click', '.remove-btn', removeExistingItem);

        $('#add-btn').click(function () {
            saveCassetteConfig();
        });

        $('#btnUpdateCassette').click(function () {
            updateCassette();
        });

    });

</script>