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
        function switchViews(obj, imG) {
            var div = document.getElementById(obj);
            var img = document.getElementById(imG);

            if (div.style.display == "none") {
                div.style.display = "inline";


                img.src = "App_Themes/DefaultTheme/Images/minus.gif";

            }
            else {
                div.style.display = "none";
                img.src = "App_Themes/DefaultTheme/Images/plus.gif";

            }
        }

    </script>
</head>
<body style="font-family: 'Trebuchet MS'; font-size: 11px;">
    <form id="form1" method="post" runat="server">
        <br />
        <div id="div1" visible="false" runat="server">
            <table style="border: 1px solid blue; background-color: White;" width="700px">
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
                    <td colspan="5" class="headerPopUp">Project Report
                    </td>
                </tr>
                <tr style="height: 6px">
                </tr>
                <tr>
                    <td colspan="5">
                        <table style="width: 100%">
                            <tr>
                                <td style="width: 25%; font-family: 'ARIAL'; font-size: 11px; font-weight: normal; color: #000000; text-align: right; text-decoration: none; padding-right: 5px; padding-left: 5px; padding-top: 5px;" height="27px">Select the Employee
                                </td>
                                <td style="text-align: left; width: 20%" class="ControlDrpBorder">

                                    <asp:DropDownList ID="drpEmployee" TabIndex="3" Enabled="True" EnableTheming="false" AppendDataBoundItems="true" CssClass="drpDownListMedium"
                                        runat="server" Width="100%" DataTextField="empFirstName" BackColor="#e7e7e7" Style="border: 1px solid #e7e7e7" Height="26px"
                                        DataValueField="empno">
                                        <%--<asp:ListItem Text="Select Employee" Value="0"></asp:ListItem>--%>
                                    </asp:DropDownList>


                                </td>
                                <td style="width: 25%; font-family: 'ARIAL'; font-size: 11px; font-weight: normal; color: #000000; text-align: right; text-decoration: none; padding-right: 5px; padding-left: 5px; padding-top: 5px;" height="27px">Select the Project
                                </td>
                                <td style="text-align: left; width: 20%" class="ControlDrpBorder">

                                    <asp:DropDownList ID="drpproject" runat="server" DataTextField="Project_Name" DataValueField="Project_Id" BackColor="#e7e7e7" Style="border: 1px solid #e7e7e7" Height="26px" Width="100%"
                                        CssClass="textbox">
                                        <%--<asp:ListItem Selected="True" Value="0" style="background-color: #bce1fe">Select Project</asp:ListItem>--%>
                                    </asp:DropDownList>

                                </td>
                                <td style="width: 10%"></td>
                            </tr>
                            <tr style="height: 2px;" />
                            <tr>
                                <td style="width: 25%; font-family: 'ARIAL'; font-size: 11px; font-weight: normal; color: #000000; text-align: right; text-decoration: none; padding-right: 5px; padding-left: 5px; padding-top: 5px;" height="27px">Blocked Task
                                </td>
                                <td style="text-align: left; width: 20%" class="ControlDrpBorder">

                                    <asp:RadioButtonList ID="radblocktask" runat="server">
                                        <asp:ListItem Text="NO" Value="NO" Selected="true"></asp:ListItem>
                                        <asp:ListItem Text="YES" Value="YES"></asp:ListItem>
                                    </asp:RadioButtonList>

                                </td>
                                <td style="width: 25%; font-family: 'ARIAL'; font-size: 11px; font-weight: normal; color: #000000; text-align: right; text-decoration: none; padding-right: 5px; padding-left: 5px; padding-top: 5px;" height="27px">Task Status
                                </td>
                                <td style="text-align: left; width: 20%" class="ControlDrpBorder">
                                    <asp:DropDownList ID="drpTaskStatus" TabIndex="5" Enabled="True" EnableTheming="false" AppendDataBoundItems="true" CssClass="drpDownListMedium"
                                        runat="server" Width="100%" DataTextField="Task_Status_Name" BackColor="#e7e7e7" Style="border: 1px solid #e7e7e7" Height="26px"
                                        DataValueField="Task_Status_Id">
                                        <asp:ListItem Text="Select Task Status" Value="0"></asp:ListItem>
                                    </asp:DropDownList>
                                </td>
                                <td style="width: 10%"></td>
                            </tr>

                            <tr style="height: 2px;" />
                            <tr>
                                <td style="width: 25%; font-family: 'ARIAL'; font-size: 11px; font-weight: normal; color: #000000; text-align: right; text-decoration: none; padding-right: 5px; padding-left: 5px; padding-top: 5px;" height="27px">Select task
                                </td>
                                <td style="text-align: left; width: 20%" class="ControlDrpBorder">

                                    <asp:DropDownList ID="drptask" runat="server" DataTextField="Task_Name" DataValueField="Task_Id" CssClass="drpDownListMedium" BackColor="#e7e7e7" Style="border: 1px solid #e7e7e7" Height="26px" Width="100%" AppendDataBoundItems="True">
                                    </asp:DropDownList>

                                </td>
                                <td style="width: 25%; font-family: 'ARIAL'; font-size: 11px; font-weight: normal; color: #000000; text-align: right; text-decoration: none; padding-right: 5px; padding-left: 5px; padding-top: 5px;" height="27px">Select Dependency Task
                                </td>
                                <td style="text-align: left; width: 20%" class="ControlDrpBorder">

                                    <asp:DropDownList ID="drpdependencytask" runat="server" DataTextField="Task_Name" DataValueField="Task_Id" CssClass="drpDownListMedium" BackColor="#e7e7e7" Style="border: 1px solid #e7e7e7" Height="26px" Width="100%" AppendDataBoundItems="True">
                                    </asp:DropDownList>

                                </td>
                                <td style="width: 10%"></td>
                            </tr>
                            <tr style="height: 2px;" />

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
                        <table style="width: 100%">
                            <tr>
                                <td style="width: 30%;"></td>
                                <td style="width: 20%;">
                                    <asp:Button ID="btnstockageing" runat="server" CssClass="NewReport6"
                                        ValidationGroup="btnAgeing" EnableTheming="false" />
                                </td>
                                <td style="width: 20%;">
                                    <asp:Button ID="btnExcel" runat="server" CssClass="exportexl6"
                                        EnableTheming="false" ValidationGroup="btnAgeing" />
                                </td>
                                <td style="width: 30%;"></td>
                            </tr>
                        </table>
                    </td>
                </tr>
                <tr>
                    <td class="style1" colspan="4"></td>
                </tr>
            </table>
        </div>
        <div runat="server" id="divmain" visible="true">
            <table width="600px">
                <tr>
                    <td colspan="3">
                        <table width="100%">
                            <tr>
                                <td style="width: 45%"></td>
                                <td style="width: 19%">
                                    <input type="button" value="" id="Button1" runat="Server" onclick="javascript: CallPrint('divPrint')"
                                        class="printbutton6" />
                                </td>
                                <td style="width: 2%">
                                    <asp:Button ID="btndet" Visible="false" CssClass="GoBack" EnableTheming="false" runat="server" />
                                </td>
                                <td style="width: 31%"></td>
                            </tr>
                        </table>
                    </td>
                </tr>
            </table>
            <br />
            <div id="divPrint" align="center" visible="false" style="font-family: 'Trebuchet MS'; font-size: 11px; width: 100%"
                runat="server">
                <table width="600px" border="0" style="font-family: Trebuchet MS; font-size: 14px;">
                    <tr>
                        <td width="140px" align="left">TIN#:
                        <asp:Label ID="lblTNGST" runat="server"></asp:Label>
                        </td>
                        <td align="center" width="320px" style="font-size: 20px;">
                            <asp:Label ID="lblCompany" runat="server"></asp:Label>
                        </td>
                        <td width="140px" align="left">Ph:
                        <asp:Label ID="lblPhone" runat="server"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td align="left">GST#:
                        <asp:Label ID="lblGSTno" runat="server"></asp:Label>
                        </td>
                        <td align="center">
                            <asp:Label ID="lblAddress" runat="server"></asp:Label>
                        </td>
                        <td align="left">Date:
                        <asp:Label ID="lblBillDate" runat="server"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td>&nbsp;
                        </td>
                        <td align="center">
                            <asp:Label ID="lblCity" runat="server" />
                            -
                        <asp:Label ID="lblPincode" runat="server"></asp:Label>
                        </td>
                        <td>&nbsp;
                        </td>
                    </tr>
                    <tr>
                        <td>&nbsp;
                        </td>
                        <td align="center">
                            <asp:Label ID="lblState" runat="server"> </asp:Label>
                        </td>
                        <td>&nbsp;
                        </td>
                    </tr>
                    <tr>
                        <td colspan="3">
                            <br />
                            <h5>
                            <asp:Label ID="lblproject" runat="server"></asp:Label>
                                <br />
                               
                            <asp:Label ID="lblStartDate" runat="server"> </asp:Label>
                               
                            <asp:Label ID="lblEndDate" runat="server"> </asp:Label></h5>
                        </td>
                    </tr>
                </table>
                <br />
                <br />
                <table width="1000px"  border="0" style="font-family: Trebuchet MS; font-size: 14px;">
                    <tr>
                        <td>
                            <wc:ReportGridView runat="server" BorderWidth="1" ID="gvOuts1" GridLines="Both"
                                AlternatingRowStyle-CssClass="even" AutoGenerateColumns="false"
                                EmptyDataText="No Data Found" CssClass="someClass"
                                Width="100%" OnRowDataBound="gvOuts1_RowDataBound">
                                     <HeaderStyle Height="30px" HorizontalAlign="Center" Font-Bold="true" BackColor="#cccccc" BorderColor="Gray" Font-Size="Small"/>
                                <RowStyle Font-Bold="true" HorizontalAlign="Center" Height="30px" Font-Size="Small" ForeColor="#0567AE"/>
                              <%--  <RowStyle CssClass="dataRow" />
                                <SelectedRowStyle CssClass="SelectdataRow" />
                                <AlternatingRowStyle CssClass="altRow" />
                                <EmptyDataRowStyle CssClass="HeadataRow" Font-Bold="true" />
                                <HeaderStyle CssClass="HeadataRow" Wrap="false" BorderStyle="Solid" BorderColor="Gray" BorderWidth="1px" />
                                <FooterStyle CssClass="dataRow" />
                                <PagerStyle CssClass="footer-row allPad" VerticalAlign="Middle" HorizontalAlign="Left" />--%>

                                <Columns>
                                    <asp:TemplateField HeaderText="Project name" ItemStyle-Width="62%" ItemStyle-HorizontalAlign="Left">
                                        <ItemTemplate>
                                            <br />

                                            <a href="javascript:switchViews('div<%# Eval("ProjectName") %>', 'imgdiv<%# Eval("ProjectName") %>');"
                                                style="text-decoration: none;">
                                                <img id="imgdiv<%# Eval("ProjectName") %>" alt="Show" border="0" src="App_Themes/DefaultTheme/Images/plus.gif" />
                                            </a>
                                            <%--<a style="text-decoration:none" href='BalanceSheetLevel2.aspx?HeadingName=<%# Eval("HeadingName") %>&HeadingID=<%# Eval("HeadingID") %>'><asp:Label style="font-family:'Trebuchet MS'; font-size:11px;  " ID="lblparticulars" runat="server" Text = '<%# Eval("HeadingName") %>' /></a>--%>
                                            <asp:Label Style="font-family: 'Trebuchet MS';" ID="lblprojectname" runat="server"
                                                Text='<%# Eval("ProjectName") %>' />
                                            <br />
                                            <div id="div<%# Eval("ProjectName") %>" style="display: none; position: relative; left: 1px;">
                                                <wc:ReportGridView runat="server" BorderWidth="1" ID="gvProducts" 
                                                    AutoGenerateColumns="false" EmptyDataText="No Data Found"  Width="75%"
                                                    OnRowDataBound="gvProducts_RowDataBound">
                                                         <HeaderStyle Height="30px" HorizontalAlign="Center" Font-Bold="true" BackColor="#cccccc" BorderColor="Gray" Font-Size="Small"/>
                                <RowStyle Font-Bold="true" HorizontalAlign="Center" Height="30px" Font-Size="Small" ForeColor="#0567AE"/>
                                                   <%-- <HeaderStyle CssClass="ReportHeadataRow" />
                                                    <RowStyle CssClass="ReportdataRow" />
                                                    <AlternatingRowStyle CssClass="ReportAltdataRow" />
                                                    <FooterStyle CssClass="ReportFooterRow" />--%>
                                                    <PageHeaderTemplate>
                                                        <br />
                                                        <br />
                                                    </PageHeaderTemplate>
                                                    <Columns>
                                                        <asp:TemplateField HeaderText="Task Name" ItemStyle-Width="3%">
                                                            <ItemTemplate>
                                                                <asp:Label ID="lblItemCode" runat="server" Text='<%# Eval("Task_Name") %>' />
                                                            </ItemTemplate>
                                                        </asp:TemplateField>
                                                        <asp:TemplateField HeaderText="Task Date" ItemStyle-Width="5%">
                                                            <ItemTemplate>
                                                                <asp:Label ID="lblProductName" runat="server" Text='<%# Eval("Task_Date","{0:dd/MM/yyyy}") %>' /><br />


                                                            </ItemTemplate>
                                                        </asp:TemplateField>
                                                        <asp:TemplateField HeaderText="Expected start date" ItemStyle-Width="5%">
                                                            <ItemTemplate>
                                                                <asp:Label ID="lblQty" runat="server" Text='<%# Eval("Expected_Start_Date","{0:dd/MM/yyyy}") %>' />
                                                            </ItemTemplate>
                                                        </asp:TemplateField>
                                                        <asp:TemplateField HeaderText="Expected End Date" ItemStyle-Width="10%">
                                                            <ItemTemplate>
                                                                <asp:Label ID="lblRate" runat="server" Text='<%# Eval("Expected_End_Date","{0:dd/MM/yyyy}") %>' />
                                                            </ItemTemplate>
                                                        </asp:TemplateField>
                                                        <asp:TemplateField HeaderText="Owner" ItemStyle-Width="5%">
                                                            <ItemTemplate>
                                                                <asp:Label ID="lblDisc" runat="server" Text='<%# Eval("Ownername") %>' />
                                                            </ItemTemplate>
                                                        </asp:TemplateField>
                                                        <asp:TemplateField HeaderText="Task Types" ItemStyle-Width="5%">
                                                            <ItemTemplate>
                                                                <asp:Label ID="lblVat" runat="server" Text='<%# Eval("TaskName") %>' />
                                                            </ItemTemplate>
                                                        </asp:TemplateField>
                                                        <asp:TemplateField HeaderText="IsActive" ItemStyle-Width="5%">
                                                            <ItemTemplate>
                                                                <asp:Label ID="lblCst" runat="server" Text='<%# Eval("IsActive") %>' />
                                                            </ItemTemplate>
                                                        </asp:TemplateField>

                                                    </Columns>
                                                    <PagerTemplate>
                                                    </PagerTemplate>
                                                    <PageFooterTemplate>
                                                        <br />
                                                    </PageFooterTemplate>
                                                </wc:ReportGridView>
                                            </div>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <%-- <asp:BoundField ItemStyle-Width="15%" DataField="Managername" HeaderText="Manager Name" ItemStyle-HorizontalAlign="Center" />--%>
                                    <%--  <asp:BoundField ItemStyle-Width="15%" DataField="empfirstname" HeaderText="Employee Name" ItemStyle-HorizontalAlign="Center" />--%>
                                    <%--  <asp:BoundField ItemStyle-Width="12%" DataField="ProjectName" HeaderStyle-HorizontalAlign="Right"
                                        HeaderText="Project Name" ItemStyle-HorizontalAlign="Right" />--%>
                                    <%--  <asp:BoundField ItemStyle-Width="15%" DataField="Project_Date" HeaderText="Project date" ItemStyle-HorizontalAlign="Center" />--%>
                                    <asp:TemplateField HeaderText="Project date" ItemStyle-Width="5%">
                                        <ItemTemplate>
                                            <asp:Label ID="lblProjectdate" runat="server" Text='<%# Eval("Project_Date","{0:dd/MM/yyyy}") %>' />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <%--  <asp:BoundField ItemStyle-Width="15%" DataField="Expected_Start_Date" HeaderText="Expected start date" ItemStyle-HorizontalAlign="Center" />--%>
                                    <asp:TemplateField HeaderText="Expected Start Date" ItemStyle-Width="5%">
                                        <ItemTemplate>
                                            <asp:Label ID="lblExpectedStartDate" runat="server" Text='<%# Eval("Expected_Start_Date","{0:dd/MM/yyyy}") %>' />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <%-- <asp:BoundField ItemStyle-Width="15%" DataField="Expected_End_Date" HeaderText="Expected end date" ItemStyle-HorizontalAlign="Center" />--%>
                                    <asp:TemplateField HeaderText="Expected End Date" ItemStyle-Width="5%">
                                        <ItemTemplate>
                                            <asp:Label ID="lblExpectedEndDate" runat="server" Text='<%# Eval("Expected_End_Date","{0:dd/MM/yyyy}") %>' />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:BoundField ItemStyle-Width="15%" DataField="Project_Status" HeaderText="Project status" ItemStyle-HorizontalAlign="Center" />


                                    <%--<asp:BoundField ItemStyle-Width="15%" DataField="Project_Date" HeaderText="project date" ItemStyle-HorizontalAlign="Center" />--%>
                                    <%-- <asp:BoundField ItemStyle-Width="12%" DataField="Task_Name" HeaderStyle-HorizontalAlign="Right"
                            HeaderText="Task Name" DataFormatString="{0:f2}" ItemStyle-HorizontalAlign="Right" />--%>
                                </Columns>

                            </wc:ReportGridView>
                        </td>
                    </tr>
                </table>
            </div>
            &nbsp;
            
    
          
            <br />
        </div>
    </form>
</body>
</html>

