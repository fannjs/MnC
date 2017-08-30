<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="edit.aspx.cs" Inherits="Maestro.views.administration.siteManagement.edit" %>

<div class="well">
    <form class="form-horizontal" role="form" action="javascript:;">
        <fieldset>
            <legend>Edit</legend>
            <div class="form-group">
                <label for="inputSiteCode" class="col-xs-2 control-label">Code</label>
                <div class="col-xs-4">
                    <input type="text" class=" form-control" runat="server" disabled="disabled" id="inputSiteCode" placeholder="Code">
                </div>
            </div>
            <div class="form-group">
                <label for="inputSiteName" class="col-xs-2 control-label">Name</label>
                <div class="col-xs-4">
                    <input type="text" class="form-control" runat="server" id="inputSiteName" placeholder="Name">
                </div>
            </div>
            <div class="form-group">
                <label for="selectSiteCountry" class="col-xs-2 control-label">Country</label>
                <div class="col-xs-4" id="divCountry" runat="server">
                    <select class="form-control" id="selectSiteCountry">
                        <option>Loading...</option>
                    </select>
                </div>
            </div>
<%--            <div class="form-group">
                <label for="inputSiteState" class="col-xs-2 control-label" runat="server">State</label>
                <div class="col-xs-4">
                    <input type="text" class="form-control" runat="server" id="inputSiteState" placeholder="State" />
                </div>
            </div>--%>
            <div class="form-group">
                <div class="col-xs-offset-2 col-xs-4">
                    <button type="submit" class="btn btn-primary" onclick="updateUser()">Update</button>
                    <button type="button" id="cancel-button" class="btn btn-default">Cancel</button>
                </div>
            </div>
        </fieldset>
    </form>
</div>
<script src="/assets/scripts/administration/siteManagement/edit.js"></script>

