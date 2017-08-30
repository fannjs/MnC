 <%@ Page Language="C#" AutoEventWireup="true" CodeBehind="main.aspx.cs" Inherits="Maestro.views.kioskManagement.machineTemplate.main" %>

<link rel="Stylesheet" href="../../../assets/styles/machineTemplate.css" />
<link rel="stylesheet" href="/components/sensors/css/sensorfield.css" />
<link rel="stylesheet" href="/components/sensors/css/arrow.css" />

<div>
    <input type="hidden" runat="server" id="taskNameHidden" />
	<div id="addButton">        
		<a href="javascript:;" id="add-button" class="func-btn"><i class="fa fa-plus"></i>Add New Kiosk Type</a>
	</div>
	<div class="block-xs"></div>
	<table class="table table-bordered" id="tblMachTempList">
		<thead>
			<tr>
				<th>Kiosk Type</th>
				<th>Kiosk Model</th>
				<th></th>
				<th></th>
			</tr>
		</thead>
		<tbody>
			<tr><td colspan="4">Loading...</td></tr>
		</tbody>
	</table>
</div>
<div class="modal" id="errorCode-modal" tabindex="-1" role="dialog" aria-labelledby="modal-title" aria-hidden="true">
	<div class="modal-dialog">
		<div class="modal-content">
			<div class="modal-header">
				<button type="button" class="close" data-dismiss="modal" aria-hidden="true">&times;</button>
				<h4 class="modal-title" id="modal-title">Status Code</h4>
			</div>
			<div class="modal-body">
                <div id="divErrCode" class="toggle-div">
				    <ul class="nav nav-tabs">
					    <li class="active"><a href="#list-code-tab" data-toggle="tab">List of Status Codes</a></li>
					    <li><a href="#add-code-tab" id="toggleAddCodeTab" data-toggle="tab">Add New Code</a></li>
				    </ul>
				    <div class="tab-content">
					    <div class="tab-pane active" id="list-code-tab">
                            <div>
                                <table id="tblErrorCodes" class="table-scrollable">
							        <thead>
								        <tr>
									        <th>Status Code</th>
									        <th>Description</th>
									        <th>Status</th>
									        <th>S.O.P</th>
								        </tr>
							        </thead>
                                    <tbody>
                                    </tbody>
						        </table>
                            </div>	
                            <div class="block-md"></div>			
						    <div id="edit-code-div">
							    <ul class="nav nav-tabs">
								    <li class="active"><a href="#edit-info-tab" data-toggle="tab">Edit Info</a></li>
								    <li><a href="#edit-sensor-tab" data-toggle="tab">Edit Sensor</a></li>
								    <li class="pull-right"><button id="closeDiv" class="close">&times;</button></li>
							    </ul>
							    <div class="tab-content">
								    <div class="tab-pane active" id="edit-info-tab">
									    <div class="field-group">
										    <label for="editErrorCode" class="form-label width-lg">Error Code</label>
										    <input type="text" class="input-field width-xl" id="editErrorCode" placeholder="Error Code">
									    </div>
									    <div class="field-group">
										    <label for="editErrorDescription" class="form-label width-lg">Error Description</label>
										    <input type="text" class="input-field width-xl" id="editErrorDescription" placeholder="Error Description">
									    </div>
									    <div class="field-group">
										    <label for="editCodeStatus" class="form-label width-lg">Status</label>
										    <select class="input-field width-xl" id="editCodeStatus">
											    <option id="ONLINE" value="ONLINE">Online</option>
											    <option id="ERROR" value="ERROR">Error</option>
											    <option id="WARN" value="WARN">Warning</option>
											    <option id="OFFLINE" value="OFFLINE">Offline</option>
										    </select>
										    <input type="hidden" id="hidOriMCode" />
									    </div>
									    <div class="field-group">
										    <label for="editSOP" class="form-label width-lg">Standard Operation Procedure (S.O.P)</label>
										    <textarea id="editSOP" class="input-field width-xl" rows="4"></textarea>
									    </div>
                                        <div class="field-group">
							                <label for="inputCategory" class="form-label width-lg">Image Type<br /><span style="font-weight:normal;opacity:0.5;">(optional)</span></label>
							                <select class="input-field width-xl" id="editCategory">
                                            </select>
						                </div>
                                        <div id="editSelectImgHide" style="display:none;">
                                            <div id="editSelectImgDiv">
                                                
                                            </div>
                                        </div>
                                        <div class="block-md"></div>
									    <div class="field-group">
                                            <label class="form-label width-lg"></label>
                                            <div style="display:inline-block;" class="width-xl">
                                                <button type="button" id="btnUpdateMCode" class="btn btn-primary" title="Update">Update</button>
										        <button type="button" class="btn btn-default cancel-btn" title="Cancel Update">Cancel</button>
                                                <button type="button" id="btnDeleteMCode" class="btn btn-danger" style="float:right;" title="Delete"><i class="fa fa-trash-o"></i>Delete</button>
                                            </div>
									    </div>
								    </div>
								    <div class="tab-pane" id="edit-sensor-tab">
									    <div class="row">
										    <div class="col-md-7">
											    <div class="sensorField">
												    <img src="/assets/images/machine_diagram.png">
											    </div>
										    </div>
										    <div class="col-md-5">
											    <div style='margin-top: 10px; font-size:15px'>
												    <span>Please drag to place sensor location: </span>
												    <br />
																		  

												    <br />
												    <div>
													    <div class="arrow arrowWrap itemDrag" style='margin-bottom: -3px'>
														    <div class="top"></div>
													    </div>
													    <div class="arrow arrowWrap itemDrag" style='margin-right: 5px'>
														    <div class="right"></div>
													    </div>
													    <div class="arrow arrowWrap itemDrag" style='margin-right: 5px; margin-bottom: 3px'>
														    <div class="bottom"></div>
													    </div>
													    <div class="arrow arrowWrap itemDrag">
														    <div class="left"></div>
													    </div>      
												    </div>
												    <br />
												    <div>
													    <button type="button" id="btnSaveSensor" class="btn btn-default">Save</button>
													    <button type="reset" id="btnResetSensor" class="btn btn-default">Reset</button>
													    <button type="button" class="btn btn-default cancel-btn">Cancel</button>
												    </div>
											    </div>
										    </div>
									    </div>
								    </div>
							    </div>
						    </div>
					    </div>
					    <div class="tab-pane" id="add-code-tab">
						    <div class="field-group">
							    <label for="inputErrorCode" class="form-label width-lg">Status Code</label>
							    <input type="text" class="input-field width-xl" id="inputErrorCode" placeholder="Status Code">
						    </div>
						    <div class="field-group">
							    <label for="inputErrorDescription" class="form-label width-lg">Description</label>
							    <input type="text" class="input-field width-xl" id="inputErrorDescription" placeholder="Description">
						    </div>
						    <div class="field-group">
							    <label for="inputCodeStatus" class="form-label width-lg">Status</label>
							    <select class="input-field width-xl" id="inputCodeStatus">
								    <option value="ONLINE">Online</option>
								    <option value="ERROR">Error</option>
								    <option value="WARN">Warning</option>
								    <option value="OFFLINE">Offline</option>
							    </select>
						    </div>
						    <div class="field-group">
							    <label for="inputSOP" class="form-label width-lg">Standard Operation Procedure (S.O.P)</label>
							    <textarea id="inputSOP" class="input-field width-xl" rows="4"></textarea>
						    </div>
                            <div class="field-group">
							    <label for="inputCategory" class="form-label width-lg">Image Type<br /><span style="font-weight:normal;opacity:0.5;">(optional)</span></label>
							    <select class="input-field width-xl" id="inputSelectCategory">
                                </select>
                                &nbsp;
                                <a id="manage-category-btn" class="mtLinkBtn">Manage Image Category</a>
						    </div>
                            <div id="selectImageHide" style="display:none;">
                                <span style="text-decoration:underline;">Please select one:</span>
                                <div id="selectImageDiv">
                                
                                </div>
                            </div>
                            <div class="block-md"></div>
						    <div class="field-group">
                                <label class="form-label width-lg"></label>
							    <button type="submit" id="btnAddNewMCode" class="btn btn-primary">Add</button>
                                <button type="button" id="btnResetMCode" class="btn btn-default">Reset</button>
						    </div>
						    <input type="hidden" class=" form-control" id="hidMType" />
						    <input type="hidden" class=" form-control" id="hidMModel"/>
					    </div>
				    </div>
                </div>
                <div class="toggle-div" id="divCodeCategory" style="display:none;">
                    <div><a id="backward-btn" class="mtLinkBtn"><i class="fa fa-arrow-left" style="width:15px;"></i>Back</a><a id="addNewCC" class="mtLinkBtn pull-right"><i class="fa fa-plus" style="padding-right:4px;"></i>New</a></div>
                    <div>
                        <table id="tblCodeCategory" class="table-scrollable">
                            <thead>
                                <tr>
                                    <th>Category Name</th>
                                    <th>Category Desc</th>
                                    <th>Category Type</th>
                                    <th>Total Images</th>
                                </tr>
                            </thead>
                            <tbody></tbody>
                        </table>
                    </div>
                    <div class="block-xs"></div>
                    <div id="addEditCC" style="display:none;">
                        <div class="block-xs"><span class="modalCloseBtn closeAddEditCC"><i class="fa fa-times"></i></span></div>
                        <ul class="nav nav-tabs">
					        <li class="active"><a href="#cc-addEdit-tab" data-toggle="tab">Overview</a></li>
					        <li><a href="#images-tab" data-toggle="tab" id="toggleImageTab">Images</a></li>
				        </ul>
                        <div class="tab-content">
					        <div class="tab-pane active" id="cc-addEdit-tab">
                                <h4 id="addEditTitle">New Code Category</h4>
                                <div class="field-group">
							        <label for="inputCName" class="form-label width-lg">Category Name</label>
							        <input type="text" class="input-field width-xl ccTF" id="inputCName" placeholder="Category Name">
						        </div>
						        <div class="field-group">
							        <label for="inputCDesc" class="form-label width-lg">Category Description</label>
							        <input type="text" class="input-field width-xl ccTF" id="inputCDesc" placeholder="Category Description">
						        </div>
                                <div class="field-group">
							        <label for="inputCType" class="form-label width-lg">Kiosk Type</label>
							        <select id="inputCType" class="input-field width-xl">
                                        <option selected disabled> - Please Select - </option>
                                    </select>
						        </div>  
                                <div class="field-group" id="add-uploadImg">
							        <label class="form-label width-lg">Images<br /><span style="opacity:0.5;font-weight:normal;">(optional)</span></label>
                                    <input type="file" class="input-field width-xl ccTF" id="inputCImages" style="display:inline-block;" multiple>
						        </div>
                                <div class="block-md"></div>
                                <div class="field-group">
							        <label class="form-label width-lg"></label>
                                    <div class="width-xl" style="display:inline-block">
                                        <button class="btn btn-primary" id="addCCBtn">Add</button>
                                        <button class="btn btn-primary editCCbtn" id="updateCCBtn" style="display:none;">Update</button>
                                        <button class="btn btn-default closeAddEditCC">Cancel</button>
                                        <button class="btn btn-danger editCCbtn" id="deleteCCBtn" style="display:none;float:right;">Delete</button>
                                    </div>
						        </div>
                            </div>
                            <div class="tab-pane" id="images-tab">
                                <div id="editCC-div">
                                    <div>
                                        <input type="file" class="ccTF" id="inputEdit_UploadImages" style="display:inline-block;" multiple>
                                        <button id="edit_UploadImagesBtn">Upload</button>
                                    </div>
                                    <div class="block-md" style="text-align:right;"><a id="selectAllImages" class="mtLinkBtn" data-flag="false"><i class="fa fa-check"></i>Select All</a>&nbsp;<a id="removeImageBtn" class="mtLinkBtn"><i class="fa fa-trash-o" style="padding-right:4px;"></i>Remove</a></div>
                                    <div id="img-uploaded-div">
                                
                                    </div>
                                    <hr style="margin: 10px 0px;" />
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
			</div>
		</div><!-- /.modal-content -->
	</div><!-- /.modal-dialog -->
</div><!-- /.modal -->

<script src="/components/sensors/js/sensor.js"></script>
<script src="/components/sensors/js/sensorfield.js"></script>
<script src="/assets/scripts/kioskManagement/machineTemplate/main.js"></script>

