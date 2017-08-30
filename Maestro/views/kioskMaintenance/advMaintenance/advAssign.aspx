<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="advAssign.aspx.cs" Inherits="Maestro.views.kioskMaintenance.advMaintenance.advAssign" %>


<div>
    <form class="form-horizontal well" role="form" action="javascript:;">
        <fieldset>
            <legend>New Advertisement Assignment</legend>
            <div class="form-group">
                <label for="selectBranchSite" class="col-xs-2 control-label">Machine Type</label>
                <div class="col-xs-4">
                    <select class="form-control" id="selectMachineType">
                        <option>CSD</option>
                        <option>CJD</option>
                    </select>
                </div>
            </div>
            <div class="form-group">
                <label for="selectBranchSite" class="col-xs-2 control-label">Category Type</label>
                <div class="col-xs-4">
                    <input type="checkbox" name="vehicle" value="All">All
                    <input type="checkbox" name="vehicle" value="Group">Group
                    <input type="checkbox" name="vehicle" value="Region">Region
                    <input type="checkbox" name="vehicle" value="State">State
                    <input type="checkbox" name="vehicle" value="Machine ID">Machine ID
                </div>
            </div>

    <div>
        <table class="table" id="tbDetails">
            <thead>
                <tr>
                    <th>Sequence</th>
                    <th>File Name</th>
                    <th>&nbsp;</th>
                </tr>
            </thead>
            <tbody>
                <tr><td>1</td><td>ABC.jpg</td><td><input type="checkbox" name="vehicle" ></td> </tr>
                <tr><td>2</td><td>BCD.png</td><td><input type="checkbox" name="vehicle" ></td> </tr>
                <tr><td>3</td><td>CDE.avi</td><td><input type="checkbox" name="vehicle" ></td> </tr>
                <tr><td>4</td><td>DEF.mp4</td><td><input type="checkbox" name="vehicle" ></td> </tr>
                <tr><td>5</td><td>EFC.jpg</td><td><input type="checkbox" name="vehicle" ></td> </tr>
                <tr><td>6</td><td>MYA.png</td><td><input type="checkbox" name="vehicle" ></td> </tr>
                <tr><td>7</td><td>SDF.avi</td><td><input type="checkbox" name="vehicle" ></td> </tr>
                <tr><td>8</td><td>TTD.mp4</td><td><input type="checkbox" name="vehicle" ></td> </tr>
                <tr><td>9</td><td>EFC.jpg</td><td><input type="checkbox" name="vehicle" ></td> </tr>
                <tr><td>10</td><td>MYA.png</td><td><input type="checkbox" name="vehicle" ></td> </tr>

            </tbody>
        </table>
                            <button type="button" class="btn btn-default" onclick="addCustomer()">Save</button>

    </div>
    </fieldset>
    </form>
</div>