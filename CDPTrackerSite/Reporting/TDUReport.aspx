<%@ Page Language="C#" AutoEventWireup="True" CodeBehind="TDUReport.aspx.cs" Inherits="CDPTrackerSite.Reporting.TDUReport" %>

<%@ Register Assembly="Microsoft.ReportViewer.WebForms, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a" Namespace="Microsoft.Reporting.WebForms" TagPrefix="rsweb" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<form id="form1" runat="server">
    <asp:ScriptManager id="ScriptManager1" runat="server"/>                
    <div id="main" align="left">
        <rsweb:ReportViewer ID="report" Style="width: 100%;" runat="server"
            Font-Names="Verdana" Font-Size="8pt" Height="600px" WaitMessageFont-Names="Verdana" WaitMessageFont-Size="14pt"
            Width="100%" ViewStateMode="Enabled" CssClass="page" BorderStyle="None"
            ShowBackButton="False" ShowDocumentMapButton="False" ShowFindControls="True"
            ShowPromptAreaButton="False" ShowRefreshButton="False" ShowZoomControl="False"
            Visible="True" ProcessingMode="Local" PageCountMode="Actual">
        </rsweb:ReportViewer>
    </div>
</form>