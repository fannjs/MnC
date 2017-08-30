<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="LocStats.aspx.cs" Inherits="Maestro.app_monitoring.content.LocStats" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
	<title></title>
	<%--<link href="../Styles/smoothness/jquery-ui-1.10.3.custom.css" rel="stylesheet" />
	<link href="../Styles/dev/generalDialog.css" rel="stylesheet" />
	<link href="../Styles/dev/LocStats.css" rel="stylesheet" />
	<link href="../Styles/dev/navmenu.css" rel="stylesheet" />

	<script type="text/javascript" src="../js/lib/jquery/jquery-1.9.1.js"></script>
	<script type="text/javascript" src="../js/lib/jquery/jquery-ui-1.10.3.custom.js"></script>
	<script type="text/javascript" src="../js/lib/jquery/jquery.editable.js"></script>
	<script type="text/javascript" src="../js/lib/json/json2.js"></script>
	<script type="text/javascript" src="../js/dev/LocStats.js"></script>
	<script type="text/javascript" src="../js/lib/jquery/jquery.editable.js"></script>
	<script type="text/javascript" src="../js/lib/jquery/jquery.tablesorter.min.js"></script>
	<script type="text/javascript" src="../js/dev/generalDialog.js"></script>--%>
	
	<link href="app_monitoring/Styles/smoothness/jquery-ui-1.10.3.custom.css" rel="stylesheet" />
	<link href="app_monitoring/Styles/dev/generalDialog.css" rel="stylesheet" />
	<link href="app_monitoring/Styles/dev/LocStats.css" rel="stylesheet" />
	<link href="app_monitoring/Styles/dev/navmenu.css" rel="stylesheet" />


</head>
<body style="background-color:black;">
	<form id="form1" runat="server">

		<!--<div style="width:100%;">
			<img ID="imgHeader" src="../Images/maestroLogo.png" />
			<div id="dlg" style="width:100%">-->
				<%--<div style="float:left">
					<img ID="img1" src="app_monitoring/Images/maestroLogo.png" />
				</div>--%>
				<%--<div style="float:left;width:254px;" class="box tighter">
					<%if(Session["userName"] != null) { %>
   
					<div><span class="small draggable">You are logged on as </span>
						<asp:label ID="lblUserData1" runat="server" CssClass="small"/></div>
					<%} %>
					<div> 
						<span class="small">Today is </span>
						<asp:label ID="curDdate1"  runat="server" CssClass="small" />
					</div>
		  
					<div>
						<span class="small">It's now </span>
						<asp:label ID="curTime1" runat="server" CssClass="small"/>
					</div>
				</div>--%>
				<%--<div id='cssmenu' style="float:right">
					<ul>
			<%
				if (Session["userRole"] != null)
				{
				%>

				<%
				if (Session["userRole"].ToString() == "Administrator")
					{
				%>


				   <li class='has-sub'><a href='#' data-title="Monitoring Management"><span>MONITORING MANAGEMENT</span></a>
					  <ul>
						 <li><a href='UserMgmt' data-title="User Management"><span>User Management</span></a></li>
						 <li><a href='customerMgmt' data-title="Customer Management"><span>Customer Management</span></a></li>
						 <li><a href='VendorMgmt' data-title="Vendor Management"><span>Vendor Management</span></a></li>
						 <li><a href='SysMgmt' data-title="Notification Management"><span>Notification Management</span></a></li>
					  </ul>
				   </li>

				   <li class='has-sub'><a href='#'  data-title="Terminal Management"><span>TERMINAL MANAGEMENT</span></a>
					  <ul>

						<li class='has-sub'><a href='#' data-title="Terminal Definition"><span>Terminal Configuration</span></a>
							<ul>
								<li><a href='SetupBranch' data-title="Setup Branch"><span>Setup Branch</span></a></li>
								<li><a href='InstallTerminal' data-title="Install Terminal"><span>Install Terminal</span></a></li>
							</ul>
						</li>
						 <li><a href='TerminalConfiguration' data-title="Terminal Configuration"><span>Terminal Configuration</span></a></li>
						 <li><a href='TerminalInformation' data-title="Terminal Information"><span>Terminal Information</span></a></li>
						 <li><a href='TerminalSchedule2' data-title="Terminal Scheduling"><span>Terminal Scheduling</span></a></li>
						 <li><a href='MachTemptConf' data-title="Terminal Template Management"><span>Terminal Template Management</span></a></li>
						 <li><a href='MachCodeConf' data-title="Terminal Code Management"><span>Terminal Code Management</span></a></li>
						 <li><a href='copyFileMgmt' data-title="File Management"><span>File Management</span></a></li>
						 <li><a href='EjournalSearch' data-title="E-Journel Search"><span>E-Journel Search</span></a></li>
						 <li><a href='MachTemplate' data-title="Machine Template"><span>Machine Template</span></a></li>
						 <li><a href='../BPI/locationAssign' data-title="FTP Configuration">FTP Configuration</a></li>
						 <li><a href='../BPI/FTPManagement' data-title="Transaction Broadcast">Transaction Broadcast</a></li>

					  </ul>
				   </li>
				   <li class='has-sub'><a href='#' data-title="Reports"><span>REPORTS</span></a>
					  <ul>
						 <li><a href='TechReport' data-title="Technical Reports"><span>TECHNICAL REPORTS</span></a></li>
						 <li><a href='../auditTrail/reports' data-title="Audit Trail"><span>AUDIT TRAIL</span></a></li>
					  </ul>
				   </li>
				   <li class='has-sub'><a href='#'  data-title="Helpdesk"><span>HELPDESK</span></a>
					  <ul>
						 <li><a href='../Helpdesk/listCase' data-title="View Cases"><span>VIEW CASES</span></a></li>
						 <!--li><a href='../Helpdesk/AddCase' data-title="Add New Case"><span>ADD CASE [under construction]</span></a></li-->
					  </ul>
				   </li>
				<%
					}
					else if (Session["userRole"].ToString() == "HelpDesk")
					{
				%>
				   <li class='active '><a href='#' data-title="Location Status"><span>LOCATION STATUS</span></a></li>
				<%
					}
					else if (Session["userRole"].ToString() == "Technical")
					{
				%>
					<li class="active "><a href="#" data-title="Location Status"><span>
						LOCATION STATUS</span></a></li>
					<li><a class="parent" href="#" data-title="Technical Reports">
						<span>TECHNICAL REPORTS</span></a></li>
					<%
					}
					else
					{
				%>
				   <li class='active '><a href='#' data-title="Location Status"><span>LOCATION STATUS</span></a></li>
				<%
					}
				%>
				<!--li><a href="login.aspx" class="parent">LOGOUT</a></li>
				<li><a href="#" onclick="Logout()" class="last">LOGOUT</a></li-->
				<%
				}
				%>
				   <li><a href="../Login.aspx" data-title="Logout"><span>LOGOUT</span></a></li>
				   <%--<li><a href="#" onclick="Logout()" class="last">LOGOUT</a></li>
				</ul>
				</div>

			</div>--%>

			<%--<div style="clear:both"></div> --%><!--very useful to allow floating elements be contained in the container.-->
		</div>
	   
		<div class="float">
			<div class="box loose">
				<span class="headTitle">Kindly select to proceed : </span>
				<div>
					<asp:DropDownList ID="ddlSite" runat="server">
					</asp:DropDownList>
				</div>
			</div>
			<div class="box loose" id="divCountry">
				<span class="headTitle">Status for All Sites : </span>
				<table class="tblLocation">
					<thead>
						<tr>  
							<td style="width:60%"><b>Country</b></td>
							<td style="width:10%"></td>
							<td style="width:10%"></td>
							<td style="width:10%"></td>
							<td style="width:10%"></td>
						</tr>
					</thead>
					<tbody></tbody>

				</table>
			</div>
			<div class="box loose" id="divUrgencyQueue">
				<span class="headTitle">Kiosk Urgency Queue List : </span>
				<table class="tblLocation">
					<thead style="background: #000000">
						<tr>  
							<td style="width:15%; text-align:center;"><b>ID</b></td>
							<td style="width:15%"><b>Type</b></td>
							<td style="width:40%"><b>Desc</b></td>
							<td style="width:30%"><b>Time</b></td>
						</tr>
					</thead>
					<tbody></tbody>
				</table>
			</div>
			<div class="box loose" id="divSearchByID">
				<span class="headTitle">Quick Machine Preview Detail : </span>
				<table style="color:white;">
					<thead>
						<tr>  
							<td style="width:45%;"></td>
							<td style="width:55%"></td>
						</tr>
					</thead>
					<tbody>
						<tr>  
							<td>Enter Machine ID :</td>
							<td><input id="txtMachineID" type="text" size="9" /></td>
						</tr>
						<tr>  
							<td><img id="imgQuickView" title="Quick View" alt="Quick View" onmouseup="this.src='app_monitoring/Images/btn_quickview_on.gif'" onmousedown="this.src='app_monitoring/Images/btn_quickview_off.gif'" onmouseout="this.src='app_monitoring/Images/btn_quickview_off.gif'" onmouseover="this.style.cursor='pointer';this.src='app_monitoring/images/btn_quickview_on.gif'" src="app_monitoring/images/btn_quickview_off.gif" class="imageBtn" id="imageBtn" style="cursor: pointer;" /></td>
						</tr>
					</tbody>

				</table>
			</div>
		</div> 
		<div class="float">
		   <div class="box tight">
				<div class="navContainer"></div>
				<div id="liveTraffic" class="divCenter">
					<div class="traffic tERROR">1</div>
					<div class="traffic tWARN">2</div>
					<div class="traffic tOK">3</div>
					<div class="traffic tOFFLINE">4</div>
				</div>
			</div>

			<div class="box loose empty" id="divState">
				<span class="headTitle">State : </span>
				<table class="tblLocation">
					<thead><tr><td style="width:100%"></td></tr></thead>
					<tbody></tbody>
				</table>
			</div>
			<div class="box loose empty" id="divDistrict">
				<span class="headTitle">Town/City : </span>
				<table class="tblLocation">
					<thead><tr><td style="width:100%"></td></tr></thead>
					<tbody></tbody>
				</table>
			</div>
			<div class="box loose empty" id="divBranch">
				<span class="headTitle">Branch : </span>
				<table class="tblLocation">
					<thead><tr><td style="width:100%"></td></tr></thead>
					<tbody></tbody>
				</table>
			</div>
			<div class="box loose empty" id="divMachine">
				<span class="headTitle">Machine(s): </span>
				<table class="tblLocation">
					<thead><tr><td style="width:100%"></td></tr></thead>
					<tbody></tbody>
				</table>
			</div>
		</div>




		<div class="float">
			<div class="box tight" id="divCurrentMachineStatus" style="color:white;">
				<span class="headTitle">Current Machine Status %</span>
				<table class="tblStats">
					<thead>
						<tr>
							<td style="width:40%"></td>
							<td style="width:40%"></td>
							<td style="width:20%"></td>
						</tr>
					</thead>
					<tbody>
						<tr>
							<td>
								From<input type="text" size="9" class="calFrom" />
							 </td>
							<td>
								To<input type="text" size="9" class="calTo"/>
							</td>
							<td>
								<input type="button" style="width:50px;" value="go" />                       
							</td>
						</tr>
					</tbody>
				</table>
				<div>Server Busy<br />Please try again later</div>
			</div>
			<div class="box tight" id="divMachineStatusToday" style="color:white;">
				<span class="headTitle">Machine Status Today</span>
				<table class="tblStats">
					<thead>
						<tr>
							<td style="width:40%"></td>
							<td style="width:40%"></td>
							<td style="width:20%"></td>
						</tr>
					</thead>
					<tbody>
						<tr>
							<td>
								From<input type="text" size="9" class="calFrom" />
							 </td>
							<td>
								To<input type="text" size="9" class="calTo"/>
							</td>
							<td>
								<input type="button" style="width:50px;" value="go" />                       
							</td>
						</tr>
					</tbody>
				</table>
				<div>Server Busy<br />Please try again later</div>
			</div>
			<div class="box tight" id="divTopProblemToday" style="color:white;">
				<span class="headTitle">Top 4 Problems Today</span>
				<table class="tblStats">
					<thead>
						<tr>
							<td style="width:40%"></td>
							<td style="width:40%"></td>
							<td style="width:20%"></td>
						</tr>
					</thead>
					<tbody>
						<tr>
							<td>
								From<input type="text" size="9" class="calFrom" />
							 </td>
							<td>
								To<input type="text" size="9" class="calTo"/>
							</td>
							<td>
								<input type="button" style="width:50px;" value="go" />                       
							</td>
						</tr>
					</tbody>
				</table>
				<div>Server Busy<br />Please try again later</div>
			</div>
		</div>
		<div class="float" style="display:none;">
			<div class="box loose">
				<span class="headTitle">Administration Menu</span>
				<div id="divNavigation">
					<h5>Monitoring Management</h5>
					<div>
						<ul>
							<li><a id="UserMgmt" href="javascript:void(0)">User Management</a></li>
							<li><a id="customerMgmt"  href="javascript:void(0)">Customer Management</a></li>
							<li><a id="VendorMgmt" href="javascript:void(0)">Vendor Management</a></li>
							<li><a id="SysMgmt" href="javascript:void(0)">Notification Management</a></li>
						</ul>
					</div>
					<h5>Terminal Management</h5>
					<div>
						<ul>
							<li><a id="TerminalDefinition" href="javascript:void(0)">Terminal Definition</a></li>
							<li><a id="TerminalConfiguration" href="javascript:void(0)">Terminal Configuration</a></li>
							<li><a id="TerminalInformation" href="javascript:void(0)">Full Terminal Listing</a></li>
							<li><a id="TerminalSchedule2" href="javascript:void(0)">Terminal Scheduling</a></li>
							<li><a id="MachTemptConf" href="javascript:void(0)">Terminal Template <br />Management</a></li>
							<li><a id="MachCodeConf" href="javascript:void(0)">Terminal Code <br />Management</a></li>
							<li><a id="copyFileMgmt" href="javascript:void(0)">File Management</a></li>
							<li><a id="EjournalSearch" href="javascript:void(0)">E-journel Search</a></li>
							<li><a id="bpi/locationAssign" href="javascript:void(0)">FTP Configuration</a></li>
							<li><a id="MachTemplate" href="javascript:void(0)">Machine Template</a></li>
						</ul>
					</div>
					<h5>Reports</h5>
					<div>
						<ul>
							<li><a id="TechReport" href="javascript:void(0)">Technical Reports</a></li>
							<li><a id="auditTrail/reports" href="javascript:void(0)">Audit Trail</a></li>
						</ul>
					</div>
					<h5>Help Desk</h5>
					<div>
						<ul>
							<li><a href="javascript:void(0)">View Cases</a></li>
						</ul>
					</div>
				</div>
			</div>
		</div>



		<div id="dlgMachineBranchInfo">
			<div class="machineDetailsContainer">
				<div id="machineBranchInfo" class="innerMachineDetails" style="border: 0;">
					<table style="width:100%;height:110px" cellpadding="1" border="0" cellspacing="1">
						<thead>
							<tr>
								<td style="width:20%"></td>
								<td style="width:80%"></td>
							</tr>
						</thead>
						<tbody> 
						</tbody>
					</table>
				</div>
			</div>
		</div>
		<div id="dlgMachineDetails">
			<div class="machineDetailsContainer">
				<div id="machineStatusListing" class="innerMachineDetails">
					<table width="100%" height="75px" cellpadding="1" cellspacing="1">
						<tbody>
							<tr>
								<td rowspan="3" valign="middle" width="70px"> 
									<img id="imgAlert">
								</td>
								<td width="100px"> 
									<b>Current Status :</b> 
								</td>
								<td id="tdCurrStatus"> 
									Unknown Since
								</td>
							</tr>
							<tr>
								<td> 
									<b>Status Code :</b>
								</td>
								<td id="tdStatusCode"> 
									
								</td>
							</tr>
							<tr>
								<td> 
									<b>Description:</b> 
								</td>
								<td id="tdDescription"> 
									
								</td>
							</tr>
						</tbody>
					</table>
				</div>
				<div id="machineFieldListing" class="innerMachineDetails">
					<table width="100%" height="90px" cellpadding="1" cellspacing="1">
						<tbody>
							<tr>
								<td width="100px"> 
									<b>Machine ID :</b> 
								</td>
								<td id="td1"> 
									
								</td>
							</tr>
							<tr>
								<td> 
									<b>Vendor :</b>
								</td>
								<td id="td2"> 
									
								</td>
							</tr>
							<tr>
								<td> 
									<b>Branch ID :</b> 
								</td>
								<td id="td3"> 
									
								</td>
							</tr>
						</tbody>
					</table>
				</div>
				<div id="machineLocation" class="innerMachineDetails">
				</div>
			</div>
		</div>
		<div id="dlgNavigation"></div>

  
<%--    <div id="dlgAddNewMachineType">
		<fieldset>
			<legend>Add New Machine Type</legend>
			<div>
				<span>Machine Type:</span>
				<input type="text" />
			</div>
			<div>
				<span>Machine Model:</span>
				<input type="text" />
			</div>
			<div>
				<span>Machine Image:</span>
				<input type="text" />
			</div>
			<div>
				<input type="button" value="Submit"/>
			</div>
		</fieldset>
	</div>--%>
	<%--<div id="dlgAEVErrorCodes">
		<a class="blueSubLink" id="lnkCreateNewError" href="#">Create New Error</a>
		<div id="divAddError" class="hidden tBorder tMargin">
			<table id="tblAddError">
				<thead>
					<tr class="theader">
						<td style="width: 15%">Error Code</td>
						<td style="width: 67%">Error Description</td>
						<td style="width: 18%">Status</td>
					</tr>
				</thead>
				<tbody>
					<tr>
						<td><input type="text"/></td>
						<td><input type="text"/></td>
						<td>
						  <select>
							  <option value="Online">Online</option>
							  <option value="Error">Error</option>
							  <option value="Warning">Warning</option>
							  <option value="Offline">Offline</option>
						  </select>
						</td>
					</tr>
				</tbody>
			</table>

			<input id="btnSaveEC" type="button" class="smallButtonFont" value="Save"/>
			<input id="btnResetEC" type="button" class="smallButtonFont" value="Reset"/>
			<input id="btnCancelEC" type="button" class="smallButtonFont" value="Cancel"/>
		</div>
		<div class="tBorder tMargin">
			<table id="tblErrorCodes">
				<thead>
					<tr class="theader">
						<th style="width: 15%">Error Code</th>
						<th style="width: 67%">Error Description</th>
						<th style="width: 18%">Status</th>
					</tr>
				</thead>
				<tbody>
				</tbody>
			</table>

		</div>
	</div>--%>
  </form>

	<script type="text/javascript" src="app_monitoring/js/lib/jquery/jquery-1.9.1.js"></script>
	<script type="text/javascript" src="app_monitoring/js/lib/jquery/jquery-ui-1.10.3.custom.js"></script>
	<script type="text/javascript" src="app_monitoring/js/lib/jquery/jquery.editable.js"></script>
	<script type="text/javascript" src="app_monitoring/js/lib/json/json2.js"></script>
	<script type="text/javascript" src="app_monitoring/js/dev/LocStats.js"></script>
	<script type="text/javascript" src="app_monitoring/js/lib/jquery/jquery.editable.js"></script>
	<script type="text/javascript" src="app_monitoring/js/lib/jquery/jquery.tablesorter.min.js"></script>
	<script type="text/javascript" src="app_monitoring/js/dev/generalDialog.js"></script>
</body>
</html>
