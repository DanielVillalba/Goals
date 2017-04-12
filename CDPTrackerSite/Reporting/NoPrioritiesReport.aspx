<%@ Page Language="C#" CodeBehind="NoPrioritiesReport.aspx.cs" AutoEventWireup="True" Inherits="CDPTrackerSite.Reporting.NoPrioritiesReport" %>

<%@ Register assembly="Microsoft.ReportViewer.WebForms, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a" namespace="Microsoft.Reporting.WebForms" tagprefix="rsweb" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <asp:ScriptManager ID="ScriptManager1" runat="server">
            </asp:ScriptManager>
            <rsweb:ReportViewer ID="report" runat="server"
                Font-Names="Verdana" Font-Size="8pt" Height="600px" WaitMessageFont-Names="Verdana" WaitMessageFont-Size="14pt"
                Width="100%" ViewStateMode="Enabled" CssClass="page" BorderStyle="None"
                ShowBackButton="False" ShowDocumentMapButton="False" ShowFindControls="True"
                ShowPromptAreaButton="False" ShowRefreshButton="False" ShowZoomControl="False"
                Visible="True" ProcessingMode="Local" PageCountMode="Actual">
            </rsweb:ReportViewer>
        </div>
    </form>
</body>
</html>
