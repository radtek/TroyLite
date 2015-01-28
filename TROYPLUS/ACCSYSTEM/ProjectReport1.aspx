<%@ Page Language="C#" AutoEventWireup="true" CodeFile="ProjectReport1.aspx.cs" Inherits="ProjectReport1" %>


<%@ Register Assembly="Shared.WebControls" Namespace="Shared.WebControls" TagPrefix="wc" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <meta http-equiv="X-UA-Compatible" content="IE=EmulateIE8" />
    <title>Ledger Report</title>
    <link href="App_Themes/DefaultTheme/calendar.css" rel="stylesheet" type="text/css" />
    <link rel="Stylesheet" href="App_Themes/DefaultTheme/DefaultTheme.css" />
    <script language="javascript" type="text/javascript" src="Scripts/calendar_eu.js"></script>
    <script type="text/javascript" language="JavaScript">
        function CallPrint(strid) {
            var prtContent = document.getElementById(strid);
            var WinPrint = window.open('', '', 'letf=0,top=0,width=600,height=400,toolbar=0,scrollbars=1,status=0');
            WinPrint.document.write(prtContent.innerHTML);
            WinPrint.document.close();
            WinPrint.focus();
            WinPrint.print();
            WinPrint.close();

        }
    </script>
</head>
<body style="font-family: 'Trebuchet MS'; font-size: 11px;">
    <form id="form1" method="post" runat="server">
    <br />
    <div id="div1" visible="false" runat="server">
        <table style="border: 1px solid blue; background-color:White;" width="700px">
            <%--<tr>
                <td colspan="5">
                    
                        <table cellspacing="0" cellpadding="0" border="0"  class="headerPopUp">
                            <tr valign="middle">
                                <td>
                                    <span>Stock Ageing Report</span>
                                </td>
                            </tr>
                        </table>
                    
                </td>--%>
                <%--<td colspan="4" class="mainConHd" style="text-align:center; vertical-align:middle">
                    <span>Stock Ageing Report</span>
                </td>--%>
            <%--</tr>--%>
            <tr>
                <td colspan="5" class="headerPopUp">
                       Project Report
                </td>
            </tr>
            <tr style="height:6px">
                    
            </tr>
            <tr>
                <td colspan="5">
                    <table style="width:100%">
                        <tr>
                            <td  style="width:25%; font-family:'ARIAL';font-size:11px;font-weight:normal; color: #000000;text-align:right;text-decoration:none;padding-right:5px;padding-left:5px;padding-top:5px;" height="27px">
                               Select the Employee
                            </td>
                            <td style="text-align: left; width:20%" class="ControlDrpBorder">
                                 
                               <asp:DropDownList ID="drpEmployee" TabIndex="3" Enabled="True" EnableTheming="false" AppendDataBoundItems="true" CssClass="drpDownListMedium"
                                                                                                runat="server" Width="100%" DataTextField="empFirstName" BackColor="#e7e7e7" Style="border: 1px solid #e7e7e7" Height="26px"
                                                                                                DataValueField="empno">
                                                                                                <%--<asp:ListItem Text="Select Employee" Value="0"></asp:ListItem>--%>
                                                                                            </asp:DropDownList>
                                                                                                          

                            </td>
                            <td  style="width:25%; font-family:'ARIAL';font-size:11px;font-weight:normal; color: #000000;text-align:right;text-decoration:none;padding-right:5px;padding-left:5px;padding-top:5px;" height="27px">
                                Select the Project
                            </td>
                            <td style="text-align: left; width:20%" class="ControlDrpBorder">
                                
                                <asp:DropDownList ID="drpproject"  runat="server" DataTextField="Project_Name" DataValueField="Project_Id"  BackColor = "#e7e7e7" style="border: 1px solid #e7e7e7" height="26px" Width="100%"
                                    CssClass="textbox">
                                    <%--<asp:ListItem Selected="True" Value="0" style="background-color: #bce1fe">Select Project</asp:ListItem>--%>
                                </asp:DropDownList>
                                                                                                  
                            </td>
                            <td style="width:10%">
                            </td>
                        </tr>
                        <tr style="height: 2px;"/> 
                        <tr>
                            <td  style="width:25%; font-family:'ARIAL';font-size:11px;font-weight:normal; color: #000000;text-align:right;text-decoration:none;padding-right:5px;padding-left:5px;padding-top:5px;" height="27px">
                                Blocked Task
                            </td>
                            <td style="text-align: left; width:20%" class="ControlDrpBorder">
                                
                               <asp:RadioButtonList id="radblocktask"   runat="server">
                                   <asp:listitem   Text="NO"  value="NO" selected="true"></asp:ListItem>
                                   <asp:listitem   Text="YES" value="YES"></asp:ListItem>
                                       </asp:RadioButtonList>
                                                                                                     
                            </td>
                            <td  style="width:25%; font-family:'ARIAL';font-size:11px;font-weight:normal; color: #000000;text-align:right;text-decoration:none;padding-right:5px;padding-left:5px;padding-top:5px;" height="27px">
                                Task Status
                            </td>
                            <td style="text-align: left; width:20%" class="ControlDrpBorder">
                                <asp:DropDownList ID="drpTaskStatus"  TabIndex="5" Enabled="True" EnableTheming="false" AppendDataBoundItems="true" CssClass="drpDownListMedium"
                                                                                            runat="server" Width="100%" DataTextField="Task_Status_Name" backcolor = "#e7e7e7" style="border: 1px solid #e7e7e7" height="26px"
                                                                                            DataValueField="Task_Status_Id" >
                                                                                            <asp:ListItem Text="Select Task Status" Value="0"></asp:ListItem>
                                                                                        </asp:DropDownList>
                            </td>
                            <td style="width:10%">
                            </td>
                        </tr>
                      
                        <tr style="height: 2px;"/> 
                        <tr>
                            <td  style="width:25%; font-family:'ARIAL';font-size:11px;font-weight:normal; color: #000000;text-align:right;text-decoration:none;padding-right:5px;padding-left:5px;padding-top:5px;" height="27px">
                                Select task
                            </td>
                            <td style="text-align: left; width:20%" class="ControlDrpBorder">
                               
                                <asp:DropDownList ID="drptask" runat="server" DataTextField="Task_Name" DataValueField="Task_Id"  CssClass="drpDownListMedium" BackColor = "#e7e7e7" style="border: 1px solid #e7e7e7" height="26px" Width="100%" AppendDataBoundItems="True">
                                </asp:DropDownList>
                                                                                                     
                            </td>
                         <td  style="width:25%; font-family:'ARIAL';font-size:11px;font-weight:normal; color: #000000;text-align:right;text-decoration:none;padding-right:5px;padding-left:5px;padding-top:5px;" height="27px">
                              Select Dependency Task
                            </td>
                            <td style="text-align: left ;width:20%" class="ControlDrpBorder">
                               
                                <asp:DropDownList ID="drpdependencytask" runat="server" DataTextField="Task_Name" DataValueField="Task_Id"  CssClass="drpDownListMedium" BackColor = "#e7e7e7" style="border: 1px solid #e7e7e7" height="26px" Width="100%" AppendDataBoundItems="True">
                                </asp:DropDownList>
                                                                                                     
                            </td>
                            <td style="width:10%">
                            </td>
                        </tr>
                        <tr style="height: 2px;"/> 
                      
                        <tr>
                            <td colspan="4">
                                <asp:Label ID="lblDate" runat="server" CssClass="label"></asp:Label>
                            </td>
                        </tr>
                    </table>
                </td>
                
            </tr>
            
            <tr>
                <td colspan="4">
                    <table style="width:100%">
                        <tr>
                            <td style="width:30%;" >
                            </td>
                            <td style="width:20%;">
                                <asp:Button ID="btnstockageing" runat="server" CssClass="NewReport6"
                                    ValidationGroup="btnAgeing" EnableTheming="false"/>
                            </td>
                            <td  style="width:20%;">
                                <asp:Button ID="btnExcel" runat="server" CssClass="exportexl6"
                                    EnableTheming="false" ValidationGroup="btnAgeing" />
                            </td>
                            <td style="width:30%;">
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
            <tr>
                <td class="style1" colspan="4">
                </td>
            </tr>
            </table>
            </div>
     <div runat="server" id="divmain" visible="true">
                <table width="600px">
                    <tr>
                        <td colspan="3">
                            <table width="100%">
                                <tr>
                                    <td style="width:31%">

                                    </td>
                                    <td style="width:19%">
                                        <input type="button" value="" id="Button1" runat="Server" onclick="javascript: CallPrint('divPrint')"
                                            class="printbutton6" />
                                    </td>
                                    <td style="width:19%">
                                        <asp:Button ID="btndet" CssClass="GoBack" EnableTheming="false" runat="server"
                                             />
                                    </td>
                                    <td style="width:31%">

                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                </table>
                <br />
                <div id="divPrint" visible="false" style="font-family: 'Trebuchet MS'; font-size: 11px; width: 100%"
                    runat="server">
                    <table width="600px" border="0" style="font-family: Trebuchet MS; font-size: 14px;">
                         <tr>
                    <td width="140px" align="left">
                        TIN#:
                        <asp:Label ID="lblTNGST" runat="server"></asp:Label>
                    </td>
                    <td align="center" width="320px" style="font-size: 20px;">
                        <asp:Label ID="lblCompany" runat="server"></asp:Label>
                    </td>
                    <td width="140px" align="left">
                        Ph:
                        <asp:Label ID="lblPhone" runat="server"></asp:Label>
                    </td>
                </tr>
                         <tr>
                    <td align="left">
                        GST#:
                        <asp:Label ID="lblGSTno" runat="server"></asp:Label>
                    </td>
                    <td align="center">
                        <asp:Label ID="lblAddress" runat="server"></asp:Label>
                    </td>
                    <td align="left">
                        Date:
                        <asp:Label ID="lblBillDate" runat="server"></asp:Label>
                    </td>
                </tr>
                          <tr>
                    <td>
                        &nbsp;
                    </td>
                    <td align="center">
                        <asp:Label ID="lblCity" runat="server" />
                        -
                        <asp:Label ID="lblPincode" runat="server"></asp:Label>
                    </td>
                    <td>
                        &nbsp;
                    </td>
                </tr>
                        <tr>
                    <td>
                        &nbsp;
                    </td>
                    <td align="center">
                        <asp:Label ID="lblState" runat="server"> </asp:Label>
                    </td>
                    <td>
                        &nbsp;
                    </td>
                </tr>
                       <tr>
                    <td colspan="3">
                        <br />
                        <h5>
                            Project Of
                            <asp:Label ID="lblproject" runat="server"></asp:Label>
                            <br />
                            Date From
                            <asp:Label ID="lblStartDate" runat="server"> </asp:Label>
                            To
                            <asp:Label ID="lblEndDate" runat="server"> </asp:Label></h5>
                    </td>
                </tr>
                    </table>
                    <br />
                    <br />
                </div>
                &nbsp;
            
       <%-- <asp:GridView ID="proOuts" runat="server" BackColor="White" BorderColor="White" BorderStyle="Ridge"
            BorderWidth="2px" CellPadding="3" CellSpacing="1" Font-Size="Small" ShowFooter="True"
             AutoGenerateColumns="True">
            <RowStyle BackColor="#DEDFDE" ForeColor="Black" />
            <FooterStyle BackColor="#C6C3C6" ForeColor="Black" />
            <PagerStyle BackColor="#C6C3C6" ForeColor="Black" HorizontalAlign="Right" />
            <SelectedRowStyle BackColor="#9471DE" Font-Bold="True" ForeColor="White" />
            <HeaderStyle BackColor="#4A3C8C" Font-Bold="True" ForeColor="#E7E7FF" />
         <%--   <PageHeaderTemplate>
                <br />
                <br />
            </PageHeaderTemplate>--%>
            <%-- <Columns>
                        <asp:BoundField ItemStyle-Width="15%" DataField="empfirstname" HeaderText="Employee Name" ItemStyle-HorizontalAlign="Center" />
                        <asp:BoundField ItemStyle-Width="12%" DataField="Project_Name" HeaderStyle-HorizontalAlign="Right"
                            HeaderText="Project Name" DataFormatString="{0:f2}" ItemStyle-HorizontalAlign="Right" />
                        <asp:BoundField ItemStyle-Width="12%" DataField="Task_Name" HeaderStyle-HorizontalAlign="Right"
                            HeaderText="Task Name" DataFormatString="{0:f2}" ItemStyle-HorizontalAlign="Right" />
                        <asp:TemplateField ItemStyle-Width="10%"  HeaderText="Balance" HeaderStyle-HorizontalAlign="Right"
                            ItemStyle-HorizontalAlign="Right">
                            <ItemTemplate>
                                <asp:Label ID="lblBalance" runat="server" CssClass="lblFont" Font-Bold="true" ForeColor="Blue"
                                    Text="0.00"> </asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:BoundField ItemStyle-Width="10%" DataField="LedgerID" Visible="false" />
                    </Columns>
            <PagerTemplate>
            </PagerTemplate>--%>
           <%-- <PageFooterTemplate>
                <br />
            </PageFooterTemplate>--%>
      <%--  </asp:GridView>--%>
       <wc:ReportGridView runat="server" BorderWidth="1" ID="gvOuts1" GridLines="Both"
                                AlternatingRowStyle-CssClass="even" AutoGenerateColumns="true"
                                AllowPrintPaging="true" EmptyDataText="No Data Found" SkinID="gridview" CssClass="someClass"
                                Width="100%" >
                                <RowStyle CssClass="dataRow" />
                                <SelectedRowStyle CssClass="SelectdataRow" />
                                <AlternatingRowStyle CssClass="altRow" />
                                <EmptyDataRowStyle CssClass="HeadataRow" Font-Bold="true" />
                                <HeaderStyle CssClass="HeadataRow" Wrap="false" BorderStyle="Solid" BorderColor="Gray" BorderWidth="1px" />
                                <FooterStyle CssClass="dataRow" />
                                <PagerStyle CssClass="footer-row allPad" VerticalAlign="Middle" HorizontalAlign="Left" />
                               
                                <Columns>
                                    <asp:BoundField ItemStyle-Width="15%" DataField="empfirstname" HeaderText="Employee Name" ItemStyle-HorizontalAlign="Center" />
                                    <asp:BoundField ItemStyle-Width="12%" DataField="ProjectName" HeaderStyle-HorizontalAlign="Right"
                                        HeaderText="Project Name" ItemStyle-HorizontalAlign="Right" />
                                     <asp:BoundField ItemStyle-Width="15%" DataField="Expected_Start_Date" HeaderText="Expected start date" ItemStyle-HorizontalAlign="Center" />
                                     <asp:BoundField ItemStyle-Width="15%" DataField="Expected_End_Date" HeaderText="Expected end date" ItemStyle-HorizontalAlign="Center" />
                                     <asp:BoundField ItemStyle-Width="15%" DataField="Project_Status" HeaderText="project status" ItemStyle-HorizontalAlign="Center" />
                                     <asp:BoundField ItemStyle-Width="15%" DataField="Project_Date" HeaderText="project date" ItemStyle-HorizontalAlign="Center" />
                                       <%--<asp:BoundField ItemStyle-Width="15%" DataField="Project_Date" HeaderText="project date" ItemStyle-HorizontalAlign="Center" />--%>
                       <%-- <asp:BoundField ItemStyle-Width="12%" DataField="Task_Name" HeaderStyle-HorizontalAlign="Right"
                            HeaderText="Task Name" DataFormatString="{0:f2}" ItemStyle-HorizontalAlign="Right" />--%>
                       
                    </Columns>
                             
                            </wc:ReportGridView>
                            <br />
    </div>
    </form>
</body>
</html>

