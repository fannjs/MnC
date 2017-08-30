<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="machStatusOutput.aspx.cs" Inherits="Maestro.views.reports.statusReport.machStatusOutput" %>

<%@ Register assembly="CrystalDecisions.Web, Version=13.0.2000.0, Culture=neutral, PublicKeyToken=692fbea5521e1304" namespace="CrystalDecisions.Web" tagprefix="CR" %>

<form id="form1" runat="server">
    <asp:label runat="server" id ="lblmsg" text=""></asp:label>
    <CR:CrystalReportViewer ID="CrystalReportViewer1" runat="server" AutoDataBind="true" />
</form>
