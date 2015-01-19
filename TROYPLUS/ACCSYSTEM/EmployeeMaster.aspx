<%@ Page Language="C#" MasterPageFile="~/PageMaster.master" AutoEventWireup="true"
    CodeFile="EmployeeMaster.aspx.cs" Inherits="EmployeeMaster" Title="Others > Business Partner" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cplhTab" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cplhControlPanel" runat="Server">
    <asp:UpdatePanel ID="UpdatePanelPage" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <table style="width: 695px;">
                <tr style="width: 100%">
                    <td style="width: 100%">
                        <div>
                            
                                <%--<div class="mainConHd">
                                    <table cellspacing="0" cellpadding="0" border="0" style="text-align: center">
                                        <tr valign="middle" align="center">
                                            <td>
                                                <span>Business Partners</span>
                                            </td>
                                        </tr>
                                    </table>
                                </div>--%>
                                <%--<table class="mainConHd" style="width: 994px;">
                                    <tr valign="middle">
                                        <td style="font-size: 20px;">
                                            Business Partners
                                        </td>
                                    </tr>
                                </table>--%>
                                <div class="mainConBody">
                                    <table style="width: 100%; margin: -2px 0px 0px 1px;" cellpadding="3" cellspacing="2" class="searchbg">
                                        <tr style=" vertical-align: middle">
                                            <td style="width: 2%;"></td>
                                            <td style="width: 35%; font-size: 22px; color: white;" >
                                                Employee Master
                                            </td>
                                            <td style="width: 16%">
                                                <div style="text-align: right;">
                                                    <%--<asp:Panel ID="pnlSearch" runat="server" Width="100px">--%>
                                                        <asp:Button ID="lnkBtnAdd" runat="server" OnClick="lnkBtnAdd_Click" CssClass="ButtonAdd66"
                                                            EnableTheming="false" Text=""></asp:Button>
                                                    <%--</asp:Panel>--%>
                                                </div>
                                            </td>
                                            <td style="width: 13%; color: white;" align="right">
                                                Search
                                            </td>
                                            <td style="width: 20%" class="NewBox">
                                                <asp:TextBox ID="txtSEmpno" runat="server" CssClass="cssTextBox" Visible="False"></asp:TextBox>
                                                <asp:TextBox ID="txtSearch" runat="server" SkinID="skinTxtBoxSearch"></asp:TextBox>
                                                <asp:TextBox ID="txtSDesig" runat="server" Width="125px" Height="16px" CssClass="cssTextBox" Visible="False"> </asp:TextBox>
                                                <asp:TextBox ID="txtSDoj" Enabled="false" MaxLength="10" runat="server" CssClass="cssTextBox" Visible="False" />
                                                <cc1:CalendarExtender ID="CalendarExtender2" runat="server" Animated="true" Format="dd/MM/yyyy"
                                                    PopupButtonID="btnSDate3" PopupPosition="BottomLeft" TargetControlID="txtSDoj">
                                                </cc1:CalendarExtender>
                                                <asp:ImageButton ID="btnSDate3" ImageUrl="App_Themes/NewTheme/images/cal.gif" CausesValidation="false"
                                                    Width="20px" runat="server" Visible="False" />
                                                <asp:TextBox ID="txtEmpMName" runat="server" CssClass="txtBox" Width="150px" Visible="false"></asp:TextBox>
                                                <asp:HiddenField ID="hdEmp" runat="server" Value="0" />
                                            </td>
                                            <td style="width: 1%" align="right">
                                                
                                            </td>
                                            <td style="width: 20%" class="NewBox">
                                                <asp:TextBox ID="txtSEmpName" runat="server" CssClass="cssTextBox" Visible="False"></asp:TextBox>
                                                <div style="width: 145px; font-family: 'Trebuchet MS';">
                                                    <asp:DropDownList ID="ddCriteria" runat="server" Width="145px" BackColor="White" Height="23px" style="text-align:center;border:1px solid white">
                                                        <asp:ListItem Value="0">All</asp:ListItem>
                                                        <asp:ListItem Value="Partner">Partner</asp:ListItem>
                                                        <asp:ListItem Value="PartnerNo">Partner No.</asp:ListItem>
                                                        <asp:ListItem Value="Designation">Designation</asp:ListItem>
                                                        <asp:ListItem Value="DOJ">DOJ</asp:ListItem>
                                                    </asp:DropDownList>
                                                </div>
                                            </td>
                                            <td style="width: 18%" class="tblLeftNoPad">
                                                <asp:Button ID="btnSearch" runat="server" Text="" OnClick="btnSearch_Click"
                                                    EnableTheming="false" CssClass="ButtonSearch6" />
                                            </td>
                                             <td style="width: 16%" class="tblLeftNoPad">
                                            <asp:Button ID="BtnClearFilter" runat="server" OnClick="BtnClearFilter_Click" EnableTheming="false" Text="" CssClass="ClearFilter6" />
                                        </td>
                                        </tr>
                                    </table>
                                </div>
                                <%--<div class="mainConBody">
                                    <table style="width: 100%;" cellpadding="3" cellspacing="2" class="searchbg">
                                        <tr style="height: 20px; vertical-align: middle">
                                            <%-- style="display: none">--%>
                                            <%--<td style="width: 21%">
                                            </td>
                                            <td style="width: 10%" align="right">
                                                
                                            </td>
                                            <td style="width: 20%" class="cssTextBoxbg">
                                                
                                            </td>
                                            <td style="width: 8%" align="right">
                                                
                                            </td>
                                            <td style="width: 20%" class="cssTextBoxbg">
                                                
                                            </td>
                                            <td align="left" style="width: 3%">
                                                
                                            </td>
                                            <%--<td class="tblLeft">
                                            </td>--%>
                                            <%--<td style="width: 20%; text-align: left" class="cssTextBoxbgnew2">
                                                
                                            </td>
                                        </tr>
                                    </table>
                                </div>--%>
                            </div>
                        
                        <input id="dummy" type="button" style="display: none" runat="server" />
                        <input id="Button1" type="button" style="display: none" runat="server" />
                        <cc1:ModalPopupExtender ID="ModalPopupExtender2" runat="server" BackgroundCssClass="modalBackground"
                            CancelControlID="Button1" DynamicServicePath="" Enabled="True" PopupControlID="popUp"
                            TargetControlID="dummy">
                        </cc1:ModalPopupExtender>
                        <asp:Panel runat="server" ID="popUp" Style="width: 60%">
                            <asp:UpdatePanel ID="UpdatePanel2" runat="server" UpdateMode="Always">
                                <ContentTemplate>
                                    <div id="contentPopUp">
                                        <asp:Panel ID="pnlEmp" runat="server" Visible="false">
                                            <div class="divArea">
                                                <table class="tblLeft" cellpadding="3" cellspacing="5" style="border: 1px solid #5078B3;"
                                                    width="100%">
                                                    <tr>
                                                        <td colspan="4">
                                                            <table class="headerPopUp" style="border: 1px solid #86b2d1" width="100%">
                                                                <tr>
                                                                    <td>
                                                                        Business Partners
                                                                    </td>
                                                                </tr>
                                                            </table>
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td>
                                                            <div align="left">
                                                                <table style="width: 100%; border: 0px solid #86b2d1" align="center" cellpadding="3" cellspacing="1">
                                                                    <tr style="height: 20px">
                                                                        <td style="width: 25%" class="ControlLabel">
                                                                            <asp:RequiredFieldValidator ValidationGroup="Save" ID="rq" runat="server" ErrorMessage="Emp no is mandatory"
                                                                                Text="*" ControlToValidate="txtEmpno"></asp:RequiredFieldValidator>
                                                                            <cc1:FilteredTextBoxExtender ID="FilteredTextBoxExtender1" runat="server" TargetControlID="txtEmpno"
                                                                                FilterType="Numbers" />
                                                                            Partner No. *
                                                                        </td>
                                                                        <td style="width: 25%" class="ControlTextBox3">
                                                                            <asp:TextBox ID="txtEmpno" runat="server" SkinID="skinTxtBox"></asp:TextBox>
                                                                        </td>
                                                                        <td style="width: 15%" class="ControlLabel">
                                                                            Type
                                                                            <%--<asp:RequiredFieldValidator ID="reqSuppllier" runat="server" ControlToValidate="drptype"
                                                                                ErrorMessage="Partner Type is mandatory" InitialValue="0" Text="*"
                                                                                ValidationGroup="Save"></asp:RequiredFieldValidator>--%>
                                                                        </td>
                                                                        <td style="width: 25%" class="ControlDrpBorder">
                                                                            <asp:DropDownList ID="drptype" runat="server" Width="100%" CssClass="drpDownListMedium" BackColor = "#e7e7e7" style="border: 1px solid #e7e7e7" height="26px">
                                                                                <asp:ListItem Value="0" Selected="True">Select Type</asp:ListItem>
                                                                                <asp:ListItem Value="Partner">Partner</asp:ListItem>
                                                                                <asp:ListItem Value="Employee">Employee</asp:ListItem>
                                                                                <asp:ListItem Value="Others">Others</asp:ListItem>
                                                                            </asp:DropDownList>
                                                                        </td>
                                                                        <td>
                                                                            
                                                                        </td>
                                                                    </tr>
                                                                    <tr>
                                                                    </tr>
                                                                    <tr style="vertical-align: bottom">
                                                                        <td style="width: 25%" class="ControlLabel">
                                                                            Title
                                                                        </td>
                                                                        <td style="width: 25%" class="ControlDrpBorder">
                                                                            <asp:DropDownList ID="drpTitle" runat="server" Width="100%" CssClass="drpDownListMedium" BackColor = "#e7e7e7" style="border: 1px solid #e7e7e7" height="26px">
                                                                                <asp:ListItem Value="Mr" Selected="True">Mr</asp:ListItem>
                                                                                <asp:ListItem Value="Mrs">Mrs</asp:ListItem>
                                                                                <asp:ListItem Value="Miss">Miss</asp:ListItem>
                                                                            </asp:DropDownList>
                                                                        </td>
                                                                        <td style="width: 15%" class="ControlLabel">
                                                                            <asp:RequiredFieldValidator ValidationGroup="Save" ID="RequiredFieldValidator1" runat="server"
                                                                                ErrorMessage="Emp First Name is mandatory" Text="*" ControlToValidate="txtEmpFName"></asp:RequiredFieldValidator>
                                                                            Partner Name *
                                                                        </td>
                                                                        <td style="width: 25%" class="ControlTextBox3">
                                                                            <asp:TextBox ID="txtEmpFName" runat="server" CssClass="cssTextBox" Width="92%"></asp:TextBox>
                                                                        </td>
                                                                    </tr>
                                                                    <tr>
                                                                    </tr>
                                                                    <tr>
                                                                        <td style="width: 25%" class="ControlLabel">
                                                                            Surname
                                                                        </td>
                                                                        <td style="width: 25%" class="ControlTextBox3">
                                                                            <asp:TextBox ID="txtEmpSName" runat="server" SkinID="skinTxtBox"></asp:TextBox>
                                                                        </td>
                                                                        <td style="width: 15%" class="ControlLabel">
                                                                            <asp:RequiredFieldValidator ValidationGroup="Save" ID="RequiredFieldValidator4" runat="server"
                                                                                Text="*" ErrorMessage="Date Of Birth is mandatory" ControlToValidate="txtDOB"></asp:RequiredFieldValidator>
                                                                            Date Of Birth *
                                                                        </td>
                                                                        <td style="width: 25%" class="ControlTextBox3">
                                                                            <asp:TextBox ID="txtDOB" Enabled="false" Width="100px" MaxLength="10" runat="server" CssClass="cssTextBox" />
                                                                            <cc1:CalendarExtender ID="calExtender3" runat="server" Animated="true" Format="dd/MM/yyyy"
                                                                                PopupButtonID="btnDate3" PopupPosition="BottomLeft" TargetControlID="txtDOB">
                                                                            </cc1:CalendarExtender>
                                                                        </td>
                                                                        <td>
                                                                            <asp:ImageButton ID="btnDate3" ImageUrl="App_Themes/NewTheme/images/cal.gif" CausesValidation="false"
                                                                                Width="20px" runat="server" />
                                                                        </td>
                                                                    </tr>
                                                                    <tr>
                                                                    </tr>
                                                                    <tr style="width: 100%">
                                                                        <td style="width: 25%" class="ControlLabel">
                                                                            <asp:RequiredFieldValidator ValidationGroup="Save" ID="RequiredFieldValidator3" runat="server"
                                                                                Text="*" ErrorMessage="Designation is mandatory" ControlToValidate="txtDesig"></asp:RequiredFieldValidator>
                                                                            Designation *
                                                                        </td>
                                                                        <td style="width: 25%" class="ControlTextBox3">
                                                                            <asp:TextBox ID="txtDesig" runat="server" SkinID="skinTxtBox"> </asp:TextBox>
                                                                        </td>
                                                                         <td style="width: 15%" class="ControlLabel">
                                                                            <asp:RequiredFieldValidator ValidationGroup="Save" ID="RequiredFieldValidator2" runat="server"
                                                                                Text="*" ErrorMessage="Date OF Joining is mandatory" ControlToValidate="txtDoj"></asp:RequiredFieldValidator>
                                                                            Date Of Joining *
                                                                        </td>
                                                                        <td style="width: 25%" class="ControlTextBox3">
                                                                            <asp:TextBox ID="txtDoj" Enabled="false" Width="100px" MaxLength="10" runat="server" CssClass="cssTextBox" />
                                                                            <cc1:CalendarExtender ID="CalendarExtender1" runat="server" Animated="true" Format="dd/MM/yyyy"
                                                                                PopupButtonID="btnD3" PopupPosition="BottomLeft" TargetControlID="txtDoj">
                                                                            </cc1:CalendarExtender>
                                                                        </td>
                                                                        <td>
                                                                            <asp:ImageButton ID="btnD3" ImageUrl="App_Themes/NewTheme/images/cal.gif" CausesValidation="false"
                                                                                Width="20px" runat="server" />
                                                                        </td>
                                                                    </tr>
                                                                    <tr>
                                                                    </tr>
                                                                    <tr style="vertical-align: bottom">
                                                                        <td style="width: 25%" class="ControlLabel">
                                                                            Manager
                                                                        </td>
                                                                        <td style="width: 25%" class="ControlDrpBorder">
                                                                            <asp:DropDownList ID="drpIncharge" TabIndex="11" Enabled="True" EnableTheming="false" AppendDataBoundItems="true" CssClass="drpDownListMedium"
                                                                                            runat="server" Width="100%" DataTextField="empFirstName" backcolor = "#e7e7e7" style="border: 1px solid #e7e7e7" height="26px"
                                                                                            DataValueField="empno" >
                                                                                            <asp:ListItem Text="Select Manager" Value="0"></asp:ListItem>
                                                                                        </asp:DropDownList>
                                                                        </td>
                                                                        <td style="width: 15%" class="ControlLabel">
                                                                            <%--<asp:RequiredFieldValidator ValidationGroup="Save" ID="RequiredFieldValidator5" runat="server"
                                                                                ErrorMessage="User Group is mandatory" Text="*" ControlToValidate="txtUserGroup"></asp:RequiredFieldValidator>--%>
                                                                            User Group
                                                                        </td>
                                                                        <td style="width: 25%" class="ControlTextBox3">
                                                                            <asp:TextBox ID="txtUserGroup" runat="server" CssClass="cssTextBox" Width="92%"></asp:TextBox>
                                                                        </td>
                                                                    </tr>
                                                                    <tr style="width: 100%">
                                                                        <td colspan="4" style="width: 100%">
                                                                            <table cellpadding="3" cellspacing="1" style="width:100%" >
                                                                                <tr runat="server" id="rowremarks" style="width: 100%">
                                                                                    <td style="width: 25%" class="ControlLabel">
                                                                                        Remarks
                                                                                    </td>
                                                                                    <td style="width: 25%" class="ControlTextBox3">
                                                                                        <asp:TextBox ID="txtRemarks" Height="24px" TextMode="MultiLine" MaxLength="10"
                                                                                            runat="server" CssClass="cssTextBox" />
                                                                                    </td>
                                                                                    <td style="width: 15%">
                                                                                    </td>
                                                                                    <td style="width: 25%">
                                                                                    </td>
                                                                                </tr>
                                                                            </table>
                                                                        </td>
                                                                        
                                                                    </tr>
                                                                </table>
                                                                <table align="center" width="100%">
                                                                    <tr style="width:20px">

                                                                    </tr>
                                                                    <tr>
                                                                        <td style="width: 30%">
                                                                        </td>
                                                                        <td style="width: 20%;" align="center">                                                                           
                                                                            <asp:Button ID="btnSave" ValidationGroup="Save" runat="server" CssClass="savebutton1231"
                                                                                EnableTheming="false" SkinID="skinBtnSave" OnClick="btnSave_Click" />
                                                                            <asp:Button ID="btnUpdate" runat="server" ValidationGroup="Save" CssClass="Updatebutton1231"
                                                                                EnableTheming="false" SkinID="skinBtnSave" OnClick="btnUpdate_Click" />
                                                                        </td>
                                                                        <td style="width: 20%;" align="center">
                                                                             <asp:Button ID="btnCancel" runat="server" CssClass="cancelbutton6" EnableTheming="false"
                                                                                SkinID="skinBtnCancel" OnClick="btnCancel_Click" />
                                                                        </td>
                                                                        <td style="width: 30%">
                                                                        </td>
                                                                    </tr>
                                                                </table>
                                                                <asp:ValidationSummary ID="ValidationSummary1" DisplayMode="BulletList" ShowMessageBox="true"
                                                                    ShowSummary="false" HeaderText="Validation Messages" ValidationGroup="Save" Font-Names="'Trebuchet MS'"
                                                                    Font-Size="12" runat="server" />
                                                            </div>
                                                        </td>
                                                    </tr>
                                                </table>
                                            </div>
                                        </asp:Panel>
                                    </div>
                                </ContentTemplate>
                            </asp:UpdatePanel>
                        </asp:Panel>
                    </td>
                </tr>
                <%--<tr>
                    
                </tr>--%>
                <tr style="width: 100%">
                    <td style="width: 100%">
                    <table width="100%" style="margin: -4px 0px 0px 0px;">
                                <tr style="width: 100%">
                                    <td>
                        <div class="mainGridHold" id="searchGrid">
                            <asp:GridView ID="GrdEmp" runat="server" AllowSorting="True" AutoGenerateColumns="False"
                                Width="100.3%" AllowPaging="True" OnPageIndexChanging="GrdEmp_PageIndexChanging" OnRowDataBound="GrdEmp_RowDataBound"
                                OnRowCreated="GrdEmp_RowCreated" DataKeyNames="Empno" PageSize="7" EmptyDataText="No Business Partner found"
                                OnSelectedIndexChanged="GrdEmp_SelectedIndexChanged" OnRowDeleting="GrdEmp_RowDeleting" CssClass="someClass">
                                <EmptyDataRowStyle CssClass="GrdContent" />
                                <Columns>
                                    <asp:BoundField DataField="empno" HeaderText="Partner No"  HeaderStyle-BorderColor="Gray"/>
                                    <asp:BoundField DataField="empTitle" HeaderText="Title"  HeaderStyle-BorderColor="Gray"/>
                                    <asp:BoundField DataField="empFirstname" HeaderText="First Name"  HeaderStyle-BorderColor="Gray"/>
                                    <asp:BoundField DataField="empMiddlename" HeaderText="Middle Name" Visible="false"  HeaderStyle-BorderColor="Gray"/>
                                    <asp:BoundField DataField="empSurname" HeaderText="Sur Name"  HeaderStyle-BorderColor="Gray"/>
                                    <asp:BoundField DataField="empDOJ" HeaderText="Date of Join" DataFormatString="{0:dd/MM/yyyy}"  HeaderStyle-BorderColor="Gray"/>
                                    <asp:BoundField DataField="empDOB" HeaderText="Date of Birth" Visible="false" DataFormatString="{0:dd/MM/yyyy}"  HeaderStyle-BorderColor="Gray"/>
                                    <%--<asp:BoundField DataField="empType" HeaderText="Type"  HeaderStyle-BorderColor="Gray"/>--%>
                                    <asp:BoundField DataField="empDesig" HeaderText="Designation"  HeaderStyle-BorderColor="Gray"/>
                                    <asp:BoundField DataField="empRemarks" HeaderText="Remarks" Visible="false"  HeaderStyle-BorderColor="Gray"/>
                                    <asp:TemplateField ItemStyle-CssClass="command" HeaderStyle-Width="50px" HeaderText="Edit" HeaderStyle-BorderColor="Gray">
                                        <ItemTemplate>
                                            <asp:ImageButton ID="btnEdit" runat="server" SkinID="edit" CommandName="Select" />
                                            <asp:ImageButton ID="btnEditDisabled" Enabled="false" SkinID="editDisable" runat="Server"></asp:ImageButton>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField ItemStyle-CssClass="command" HeaderStyle-Width="50px" HeaderText="Delete" HeaderStyle-BorderColor="Gray">
                                        <ItemTemplate>
                                            <cc1:ConfirmButtonExtender ID="CnrfmDel" TargetControlID="lnkB" ConfirmText="Are you sure to Delete this Employee Details?"
                                                runat="server">
                                            </cc1:ConfirmButtonExtender>
                                            <asp:ImageButton ID="lnkB" SkinID="delete" runat="Server" CommandName="Delete"></asp:ImageButton>
                                            <asp:ImageButton ID="lnkBDisabled" Enabled="false" SkinID="deleteDisable" runat="Server">
                                            </asp:ImageButton>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                </Columns>
                                <PagerTemplate>
                                    <table style=" border-color:white">
                                        <tr style=" border-color:white">
                                            <td style=" border-color:white">
                                                Goto Page
                                            </td>
                                            <td style=" border-color:white">
                                                <asp:DropDownList ID="ddlPageSelector" runat="server" AutoPostBack="true" BackColor="#e7e7e7"  Width="70px" Height="24px" style="border:1px solid blue"
                                                    OnSelectedIndexChanged="ddlPageSelector_SelectedIndexChanged">
                                                </asp:DropDownList>
                                            </td>
                                            <td  style=" border-color:white;Width:5px">
                                            
                                            </td>
                                            <td style=" border-color:white">
                                                <asp:Button Text="" CommandName="Page" CommandArgument="First" runat="server" CssClass="NewFirst" EnableTheming="false" Width="22px" Height="18px"
                                                    ID="btnFirst" />
                                            </td>
                                            <td style=" border-color:white">
                                                <asp:Button Text="" CommandName="Page" CommandArgument="Prev" runat="server" CssClass="NewPrev" EnableTheming="false" Width="22px" Height="18px"
                                                    ID="btnPrevious" />
                                            </td>
                                            <td style=" border-color:white">
                                                <asp:Button Text="" CommandName="Page" CommandArgument="Next" runat="server" CssClass="NewNext" EnableTheming="false" Width="22px" Height="18px"
                                                    ID="btnNext" />
                                            </td>
                                            <td style=" border-color:white">
                                                <asp:Button Text="" CommandName="Page" CommandArgument="Last" runat="server" CssClass="NewLast" EnableTheming="false" Width="22px" Height="18px"
                                                    ID="btnLast" />
                                            </td>
                                        </tr>
                                    </table>
                                </PagerTemplate>
                            </asp:GridView>
                        </div>
                        </td>
                        </tr>
                        </table>
                    </td>
                </tr>
            </table>
        </ContentTemplate>
    </asp:UpdatePanel>
    <table width="100%">
        <tr>
            <td align="center">
                                            <asp:Button ID="btnExportToExcel" runat="server" CssClass="exportexl6" OnClientClick="window.open('ReportExcelEmployee.aspx ','billSummary', 'toolbar=no,status=no,menu=no,location=no,resizable=yes,height=310,width=500,left=425,top=220, scrollbars=yes');"
                                                                    EnableTheming="false"></asp:Button>
                                       </td>
        </tr>
    </table>
</asp:Content>
