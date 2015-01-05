﻿<%@ Page Title="" Language="C#" MasterPageFile="~/PageMaster.master" AutoEventWireup="true" CodeFile="LeadMgmt.aspx.cs" Inherits="LeadMgmt" EnableEventValidation="false" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cplhTab" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cplhControlPanel" runat="Server">
    <script language="javascript" type="text/javascript">

        /*@cc_on@*/
        /*@if (@_win32 && @_jscript_version>=5)
    
        function window.confirm(str) {
            execScript('n = msgbox("' + str + '","4132")', "vbscript");
            return (n == 6);
        }
    
        @end@*/


        function pageLoad() {
            //  get the behavior associated with the tab control
            var tabContainer = $find('ctl00_cplhControlPanel_tabs2');

            //        if (tabContainer == null)
            //            tabContainer = $find('ctl00_cplhControlPanel_tabPanel2');

            if (tabContainer != null) {
                //  get all of the tabs from the container
                var tabs = tabContainer.get_tabs();

                //  loop through each of the tabs and attach a handler to
                //  the tab header's mouseover event
                for (var i = 0; i < tabs.length; i++) {
                    var tab = tabs[i];

                    $addHandler(
                    tab.get_headerTab(),
                    'mouseover',
                    Function.createDelegate(tab, function () {
                        tabContainer.set_activeTab(this);
                    }
                ));
                }
            }
        }

        function OnKeyPress(args) {
            if (args.keyCode == Sys.UI.Key.esc) {
                $find("ctl00_cplhControlPanel_ModalPopupExtender2").hide();
            }
        }

        $("#ctl00_cplhControlPanel_UpdateCancelButton").live("click", function () {
            $find("ctl00_cplhControlPanel_ModalPopupExtender2").hide();
        });

        function CheckLeadContact() {

        }

    </script>

    <style id="Style1" runat="server">
        .someClass td {
            font-size: 12px;
            border: 1px solid Gray;
        }

        .fancy-green .ajax__tab_header {
            background: url(App_Themes/NewTheme/Images/green_bg_Tab.gif) repeat-x;
            cursor: pointer;
        }

        .fancy-green .ajax__tab_hover .ajax__tab_outer, .fancy-green .ajax__tab_active .ajax__tab_outer {
            background: url(App_Themes/NewTheme/Images/green_left_Tab.gif) no-repeat left top;
        }

        .fancy-green .ajax__tab_hover .ajax__tab_inner, .fancy-green .ajax__tab_active .ajax__tab_inner {
            background: url(App_Themes/NewTheme/Images/green_right_Tab.gif) no-repeat right top;
        }

        .fancy .ajax__tab_header {
            font-size: 13px;
            font-weight: bold;
            color: #000;
            font-family: sans-serif;
        }

            .fancy .ajax__tab_active .ajax__tab_outer, .fancy .ajax__tab_header .ajax__tab_outer, .fancy .ajax__tab_hover .ajax__tab_outer {
                height: 46px;
            }

            .fancy .ajax__tab_active .ajax__tab_inner, .fancy .ajax__tab_header .ajax__tab_inner, .fancy .ajax__tab_hover .ajax__tab_inner {
                height: 46px;
                margin-left: 16px; /* offset the width of the left image */
            }

            .fancy .ajax__tab_active .ajax__tab_tab, .fancy .ajax__tab_hover .ajax__tab_tab, .fancy .ajax__tab_header .ajax__tab_tab {
                margin: 16px 16px 0px 0px;
            }

        .fancy .ajax__tab_hover .ajax__tab_tab, .fancy .ajax__tab_active .ajax__tab_tab {
            color: #fff;
        }

        .fancy .ajax__tab_body {
            font-family: Arial;
            font-size: 10pt;
            border-top: 0;
            border: 1px solid #999999;
            padding: 8px;
            background-color: #ffffff;
        }
    </style>


    <asp:UpdatePanel ID="UpdatePanelPage" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <table style="width: 100%;" align="center">
                <tr style="width: 100%">
                    <td style="width: 100%" valign="middle">

                        <%--<div class="mainConHd">
                                <table cellspacing="0" cellpadding="0" border="0">
                                    <tr valign="middle">
                                        <td>
                                            <span>Lead Management</span>
                                        </td>
                                    </tr>
                                </table>
                            </div>--%>
                        <%--<table class="mainConHd" style="width: 994px;">
                                <tr valign="middle">
                                    <td style="font-size: 20px;">
                                        Lead Management
                                    </td>
                                </tr>
                            </table>--%>
                        <div class="mainConBody">
                            <div>
                                <table cellspacing="2px" cellpadding="0" border="0" width="99.8%" style="margin: -2px 0px 0px 1px;"
                                    class="searchbg">
                                    <tr>
                                        <td style="width: 2%"></td>
                                        <td style="width: 25%; font-size: 22px; color: White;">Lead Management
                                        </td>
                                        <td style="width: 16%">
                                            <div style="text-align: right;">
                                                <asp:Panel ID="pnlSearch" runat="server" Width="100px">
                                                    <asp:Button ID="lnkBtnAdd" runat="server" OnClick="lnkBtnAdd_Click" CssClass="ButtonAdd66" CausesValidation="false"
                                                        EnableTheming="false" Width="80px" Text=""></asp:Button>
                                                </asp:Panel>
                                            </div>
                                        </td>
                                        <td style="width: 5%; color: White;" align="right">Search
                                        </td>
                                        <td style="width: 19%" class="NewBox">
                                            <asp:TextBox ID="txtSearch" runat="server" CssClass="cssTextBox" Width="92%"></asp:TextBox>
                                        </td>

                                        <td style="width: 19%" class="NewBox">
                                            <div style="width: 150px; font-family: 'Trebuchet MS';">
                                                <asp:DropDownList ID="ddCriteria" runat="server" BackColor="White" Width="157px" Height="23px" Style="text-align: center; border: 1px solid White">
                                                    <asp:ListItem Value="0">All</asp:ListItem>
                                                    <asp:ListItem Value="LeadName">Lead Name</asp:ListItem>
                                                    <asp:ListItem Value="BPName">BP Name</asp:ListItem>
                                                    <asp:ListItem Value="DocStatus">Doc Status</asp:ListItem>
                                                    <asp:ListItem Value="LeadStatus">Lead Status</asp:ListItem>
                                                    <asp:ListItem Value="Mobile">Mobile</asp:ListItem>
                                                    <asp:ListItem Value="Branch">Branch</asp:ListItem>
                                                </asp:DropDownList>
                                        </td>
                                        <%--<td style="width: 22%" class="Box">
                                                <div style="width: 100px; font-family: 'Trebuchet MS';">
                                                    <asp:DropDownList ID="ddmethods" runat="server"   Width="154px" Height="23px" style="text-align:center;border:1px solid White ">
                                                        <asp:ListItem Value="0" style="background-color: White">All</asp:ListItem>
                                                    </asp:DropDownList>
                                                </div>
                                            </td>--%>
                                        <td style="width: 12%; text-align: left">
                                            <asp:Button ID="btnSearch" runat="server" Text="" OnClick="btnSearch_Click" CausesValidation="false"
                                                CssClass="ButtonSearch6" EnableTheming="false" ForeColor="White" />
                                        </td>
                                        <td style="width: 16%" class="tblLeftNoPad">                                           
                                            <asp:Button ID="BtnClearFilter" runat="server" EnableTheming="false" Text="" CssClass="ClearFilter6" OnClick="BtnClearFilter_Click" CausesValidation="false" />
                                        </td>
                                        <td style="width: 5%">
                                            <div style="text-align: right;">

                                                <asp:Button ID="AddTheRef" runat="server" OnClick="AddTheRef_Click" CssClass="addReferencebutton6" CausesValidation="false"
                                                    EnableTheming="false" Width="80px" Text="" Visible="false"></asp:Button>

                                            </div>
                                        </td>
                                    </tr>
                                </table>
                            </div>
                        </div>

                        <input id="dummy" type="button" style="display: none" runat="server" />
                        <input id="Button1" type="button" style="display: none" runat="server" />
                        <cc1:ModalPopupExtender ID="ModalPopupExtender2" runat="server" BackgroundCssClass="modalBackground"
                            CancelControlID="Button1" DynamicServicePath="" Enabled="True" PopupControlID="popUp" RepositionMode="RepositionOnWindowResizeAndScroll"
                            TargetControlID="dummy">
                        </cc1:ModalPopupExtender>
                        <asp:Panel runat="server" ID="popUp" Style="width: 60%; display: none">
                            <asp:UpdatePanel ID="UpdatePanel2" runat="server" UpdateMode="Always">
                                <ContentTemplate>
                                    <div id="contentPopUp">
                                        <asp:Panel ID="pnlEdit" CssClass="pnlPopUp" runat="server">
                                            <table class="tblLeft" cellpadding="0" cellspacing="0" style="border: 0px solid #5078B3; background-color: #fff; color: #000;"
                                                width="100%">
                                                <tr>
                                                    <td>
                                                        <div class="divArea">
                                                            <table class="tblLeft" cellpadding="3" cellspacing="3" style="padding-left: 2px; width: 100%;">
                                                                <tr>
                                                                    <td colspan="5">
                                                                        <table class="headerPopUp">
                                                                            <tr>
                                                                                <td>Lead Management
                                                                                </td>
                                                                            </tr>
                                                                        </table>
                                                                    </td>
                                                                </tr>
                                                                <tr>
                                                                    <td colspan="5">
                                                                        <cc1:TabContainer ID="tabs2" runat="server" Width="100%" ActiveTabIndex="0" CssClass="fancy fancy-green">
                                                                            <cc1:TabPanel ID="tabMaster" runat="server" HeaderText="General">
                                                                                <HeaderTemplate>
                                                                                    <div>
                                                                                        <table>
                                                                                            <tr>
                                                                                                <td><b>General </b></td>
                                                                                            </tr>
                                                                                        </table>
                                                                                    </div>
                                                                                </HeaderTemplate>
                                                                                <ContentTemplate>
                                                                                    <div>
                                                                                        <table style="width: 770px; border: 0px solid #86b2d1" align="center" cellpadding="0" cellspacing="2">
                                                                                            <tr>
                                                                                                <td class="ControlLabel" style="width: 23%;">Lead Number
                                                                                                </td>
                                                                                                <td class="ControlTextBox3" style="width: 30%;">
                                                                                                    <asp:TextBox ID="txtLeadNo" runat="server" Enabled="False"
                                                                                                        SkinID="skinTxtBoxGrid"></asp:TextBox>
                                                                                                </td>
                                                                                                <td class="ControlLabel" style="width: 18%;">
                                                                                                    <asp:RequiredFieldValidator ID="rvStock" runat="server" ControlToValidate="txtCreationDate"
                                                                                                        ErrorMessage="Start Date is mandatory" Display="Dynamic">*</asp:RequiredFieldValidator>
                                                                                                    <asp:CompareValidator ControlToValidate="txtCreationDate" Operator="DataTypeCheck" Type="Date"
                                                                                                        ErrorMessage="Please enter a valid date" runat="server" ID="cmpValtxtDate">*</asp:CompareValidator>
                                                                                                    Start Date *
                                                                                                </td>
                                                                                                <td class="ControlTextBox3" style="width: 30%;">
                                                                                                    <asp:TextBox ID="txtCreationDate" runat="server" TabIndex="1" Enabled="False"
                                                                                                        CssClass="cssTextBox"></asp:TextBox>
                                                                                                </td>
                                                                                                <td style="width: 10%;" align="left">
                                                                                                    <cc1:CalendarExtender ID="calExtender3" runat="server" Format="dd/MM/yyyy"
                                                                                                        PopupButtonID="btnBillDate" TargetControlID="txtCreationDate" Enabled="True">
                                                                                                    </cc1:CalendarExtender>
                                                                                                    <asp:ImageButton ID="btnBillDate" ImageUrl="App_Themes/NewTheme/images/cal.gif"
                                                                                                        CausesValidation="False" Width="20px" runat="server" />
                                                                                                </td>
                                                                                            </tr>
                                                                                            <tr style="height: 30px">
                                                                                                <td class="ControlLabel" style="width: 23%;">Lead Name *
                                                                                                <asp:RequiredFieldValidator ID="RequiredFieldValidator3" runat="server" ControlToValidate="txtLeadName" Display="Dynamic" ErrorMessage="Lead Name is mandatory">*</asp:RequiredFieldValidator>
                                                                                                </td>
                                                                                                <td class="ControlTextBox3" style="width: 22%;">
                                                                                                    <asp:TextBox ID="txtLeadName" runat="server" SkinID="skinTxtBoxGrid" TabIndex="2"></asp:TextBox>
                                                                                                </td>
                                                                                                <td class="ControlLabel" style="width: 18%;">BP Name * </td>
                                                                                                <td class="ControlDrpBorder" style="width: 22%;">
                                                                                                    <asp:UpdatePanel ID="UpdatePanel21" runat="server" UpdateMode="Conditional">
                                                                                                        <ContentTemplate>
                                                                                                            <asp:DropDownList ID="cmbCustomer" runat="server" AppendDataBoundItems="true" AutoPostBack="true" BackColor="#e7e7e7" CssClass="drpDownListMedium" DataTextField="LedgerName" DataValueField="LedgerID" Height="26px" OnSelectedIndexChanged="cmbCustomer_SelectedIndexChanged" Style="border: 1px solid #e7e7e7" TabIndex="3" Width="100%">
                                                                                                                <asp:ListItem style="background-color: #e7e7e7" Text="Select Customer" Value="0"></asp:ListItem>
                                                                                                            </asp:DropDownList>
                                                                                                        </ContentTemplate>
                                                                                                    </asp:UpdatePanel>
                                                                                                    <asp:TextBox ID="txtBPName" runat="server" SkinID="skinTxtBoxGrid" TabIndex="4" Visible="False"></asp:TextBox>
                                                                                                </td>
                                                                                                <td style="width: 10%;">
                                                                                                    <asp:UpdatePanel ID="UpdatePanel10" runat="server" UpdateMode="Conditional">
                                                                                                        <ContentTemplate>
                                                                                                            <asp:CheckBox ID="chk" runat="server" AutoPostBack="true" OnCheckedChanged="chk_CheckedChanged" Text="Existing" />
                                                                                                        </ContentTemplate>
                                                                                                    </asp:UpdatePanel>
                                                                                                </td>
                                                                                            </tr>
                                                                                            <tr style="height: 30px">
                                                                                                <td class="ControlLabel" style="width: 23%;">Contact Name
                                                                                                </td>
                                                                                                <td class="ControlTextBox3" style="width: 22%;">
                                                                                                    <asp:TextBox ID="txtContactName" runat="server" TabIndex="4" SkinID="skinTxtBoxGrid"></asp:TextBox>
                                                                                                </td>
                                                                                                <td class="ControlLabel" style="width: 18%;">Address
                                                                                                </td>
                                                                                                <td class="ControlTextBox3" style="width: 22%;">
                                                                                                    <asp:TextBox ID="txtAddress" runat="server" TabIndex="5" SkinID="skinTxtBoxGrid"></asp:TextBox>
                                                                                                </td>
                                                                                                <td style="width: 10%;"></td>
                                                                                            </tr>
                                                                                            <tr style="height: 30px">
                                                                                                <td class="ControlLabel" style="width: 23%;">Mobile </td>
                                                                                                <td class="ControlTextBox3" style="width: 22%;">
                                                                                                    <asp:TextBox ID="txtMobile" runat="server" MaxLength="10" SkinID="skinTxtBoxGrid" TabIndex="6"></asp:TextBox>
                                                                                                    <cc1:FilteredTextBoxExtender ID="FilteredTextBoxExtender1" runat="server" Enabled="True" FilterType="Numbers" TargetControlID="txtMobile">
                                                                                                    </cc1:FilteredTextBoxExtender>
                                                                                                </td>
                                                                                                <td class="ControlLabel" style="width: 18%;">Telephone </td>
                                                                                                <td class="ControlTextBox3" style="width: 22%;">
                                                                                                    <asp:TextBox ID="txtTelephone" runat="server" SkinID="skinTxtBoxGrid" TabIndex="7"></asp:TextBox>
                                                                                                </td>
                                                                                                <td style="width: 10%;"></td>
                                                                                            </tr>
                                                                                            <tr>
                                                                                                <td class="ControlLabel" style="width: 23%;">
                                                                                                    <asp:CompareValidator ID="cvModeOfContact" runat="server" ControlToValidate="drpStatus" Display="Dynamic" ErrorMessage="Please Select Doc Status" Operator="GreaterThan" Text="*" ValueToCompare="0"></asp:CompareValidator>
                                                                                                    Doc Status * &nbsp;Employee *
                                                                                               <asp:CompareValidator ID="CompareValidator3" runat="server" ControlToValidate="drpIncharge"
                                                                                                   Display="Dynamic" ErrorMessage="Please Select Employee" Operator="GreaterThan"
                                                                                                   Text="*" ValueToCompare="0"></asp:CompareValidator>
                                                                                                </td>
                                                                                                <td class="ControlDrpBorder" style="width: 18%;">
                                                                                                    <asp:DropDownList ID="drpIncharge" TabIndex="8" EnableTheming="False" AppendDataBoundItems="True" CssClass="drpDownListMedium"
                                                                                                        runat="server" Width="100%" DataTextField="empFirstName" BackColor="#E7E7E7" Style="border: 1px solid #e7e7e7" Height="26px"
                                                                                                        DataValueField="empno">
                                                                                                    </asp:DropDownList>
                                                                                                </td>
                                                                                                <td class="ControlLabel" style="width: 18%;">
                                                                                                    <asp:RequiredFieldValidator ID="RequiredFieldValidator14" runat="server" ControlToValidate="txtClosingPer" Display="Dynamic" ErrorMessage="Closing Date is mandatory.">*</asp:RequiredFieldValidator>
                                                                                                    Closing % *
                                                                                                </td>
                                                                                                <td class="ControlDrpBorder" style="width: 22%;">
                                                                                                    <asp:UpdatePanel ID="UpdatePanel7" runat="server" UpdateMode="Conditional">
                                                                                                        <ContentTemplate>
                                                                                                            <asp:DropDownList ID="drpStatus" TabIndex="9" Enabled="True" EnableTheming="false" AppendDataBoundItems="true" CssClass="drpDownListMedium"
                                                                                                                runat="server" Width="100%" BackColor="#e7e7e7" Style="border: 1px solid #e7e7e7" Height="26px" AutoPostBack="True" OnTextChanged="drpStatus_SelectedIndexChanged">
                                                                                                                <asp:ListItem Text="Open" Value="Open"></asp:ListItem>
                                                                                                                <asp:ListItem Text="Closed" Value="Closed"></asp:ListItem>
                                                                                                            </asp:DropDownList>
                                                                                                        </ContentTemplate>
                                                                                                    </asp:UpdatePanel>
                                                                                                </td>
                                                                                                <td style="width: 10%;"></td>
                                                                                            </tr>
                                                                                            <tr style="height: 30px">
                                                                                                <td class="ControlLabel" style="width: 23%;">Total Amount Invoiced </td>
                                                                                                <td class="ControlTextBox3" style="width: 22%;">
                                                                                                    <asp:TextBox ID="txtTotalAmount" runat="server" Enabled="False" SkinID="skinTxtBoxGrid" TabIndex="10"></asp:TextBox>
                                                                                                </td>
                                                                                                <td class="ControlLabel" style="width: 18%;">Closing Date </td>
                                                                                                <td class="ControlTextBox3" style="width: 22%">
                                                                                                    <asp:TextBox ID="txtClosingDate" runat="server" Enabled="False" SkinID="skinTxtBoxGrid" TabIndex="11"></asp:TextBox>
                                                                                                </td>
                                                                                                <td style="width: 10%;">
                                                                                                    <cc1:CalendarExtender ID="CalendarExtender1" runat="server" Enabled="True" Format="dd/MM/yyyy" PopupButtonID="ImageButton1" TargetControlID="txtClosingDate">
                                                                                                    </cc1:CalendarExtender>
                                                                                                    <asp:ImageButton ID="ImageButton1" runat="server" CausesValidation="False" ImageUrl="App_Themes/NewTheme/images/cal.gif" Visible="False" Width="20px" />
                                                                                                </td>
                                                                                            </tr>
                                                                                            <tr style="height: 30px">
                                                                                                <td class="ControlLabel" style="width: 23%;">
                                                                                                    <asp:RequiredFieldValidator ID="RequiredFieldValidator15" runat="server" ControlToValidate="txtBranch" Display="Dynamic" ErrorMessage="Branch is mandatory.">*</asp:RequiredFieldValidator>
                                                                                                </td>
                                                                                                <td class="ControlDrpBorder" style="width: 22%;">
                                                                                                    <asp:TextBox ID="txtBranch" runat="server" SkinID="skinTxtBoxGrid"
                                                                                                        TabIndex="12" Enabled="False"></asp:TextBox>
                                                                                                </td>
                                                                                                <td class="ControlLabel" style="width: 18%;">Branch *</td>
                                                                                                <td class="ControlDrpBorder" style="width: 22%;">
                                                                                                    <asp:TextBox ID="txtClosingPer" runat="server" TabIndex="13" SkinID="skinTxtBoxGrid" Enabled="False"></asp:TextBox>
                                                                                                </td>
                                                                                                <td style="width: 10%;"></td>
                                                                                            </tr>
                                                                                            <tr style="height: 30px">
                                                                                                <td class="ControlLabel" style="width: 23%;">
                                                                                                    <asp:CompareValidator ID="CompareValidator5" runat="server" ControlToValidate="drpLeadStatus" Display="Dynamic" ErrorMessage="Please Select Lead Status" Operator="GreaterThan" Text="*" ValueToCompare="0"></asp:CompareValidator>
                                                                                                    Lead Status * </td>
                                                                                                <td class="ControlDrpBorder" style="width: 22%">
                                                                                                    <asp:DropDownList ID="drpLeadStatus" runat="server" AppendDataBoundItems="True" BackColor="#E7E7E7" CssClass="drpDownListMedium" EnableTheming="False" Height="26px" Style="border: 1px solid #e7e7e7" TabIndex="14" Width="100%">
                                                                                                        <asp:ListItem Text="Open" Value="Open"></asp:ListItem>
                                                                                                        <asp:ListItem Text="Won" Value="Won"></asp:ListItem>
                                                                                                        <asp:ListItem Text="Lost" Value="Lost"></asp:ListItem>
                                                                                                    </asp:DropDownList>
                                                                                                </td>
                                                                                                <td class="ControlLabel" style="width: 18%;"></td>
                                                                                                <td style="width: 22%"></td>
                                                                                                <td style="width: 10%;"></td>
                                                                                            </tr>
                                                                                            <tr>
                                                                                                <td>
                                                                                                    <asp:Label runat="server" ID="Error" ForeColor="Red"></asp:Label>
                                                                                                </td>
                                                                                            </tr>
                                                                                        </table>
                                                                                        <asp:ValidationSummary ID="valSum" ShowMessageBox="True" ShowSummary="False" HeaderText="Validation Messages"
                                                                                            Font-Names="'Trebuchet MS'" Font-Size="12pt" runat="server" />
                                                                                    </div>
                                                                                </ContentTemplate>
                                                                            </cc1:TabPanel>
                                                                            <cc1:TabPanel ID="TabPanel2" runat="server" HeaderText="Potential">
                                                                                <HeaderTemplate>
                                                                                    <div>
                                                                                        <table>
                                                                                            <tr>
                                                                                                <td><b>Potential</b> </td>
                                                                                            </tr>
                                                                                        </table>
                                                                                    </div>
                                                                                </HeaderTemplate>
                                                                                <ContentTemplate>
                                                                                    <table style="width: 770px" cellpadding="2" cellspacing="1">
                                                                                        <tr style="height: 9px">
                                                                                        </tr>
                                                                                        <tr style="height: 30px">
                                                                                            <td class="ControlLabel" style="width: 20%;">Predicted Closing *
                                                                                                <asp:RequiredFieldValidator ID="RequiredFieldValidator12" runat="server" ControlToValidate="txtPredictedClosing"
                                                                                                    ErrorMessage="Predicted Closing is mandatory" Display="Dynamic">*</asp:RequiredFieldValidator>

                                                                                            </td>
                                                                                            <td class="ControlNumberBox3" style="width: 28%;">
                                                                                                <asp:UpdatePanel ID="UpdatePanel9" runat="server" UpdateMode="Conditional">
                                                                                                    <ContentTemplate>
                                                                                                        <cc1:FilteredTextBoxExtender ID="OBvalid" runat="server" FilterType="Numbers" TargetControlID="txtPredictedClosing" />
                                                                                                        <asp:TextBox ID="txtPredictedClosing" runat="server" Width="100%" TabIndex="1" AutoPostBack="True" OnTextChanged="txtPredictedClosing_TextChanged"
                                                                                                            CssClass="cssTextBox"></asp:TextBox>
                                                                                                    </ContentTemplate>
                                                                                                </asp:UpdatePanel>
                                                                                                <asp:UpdatePanel ID="UpdatePanel13" runat="server" UpdateMode="Conditional">
                                                                                                    <ContentTemplate>
                                                                                                        <asp:DropDownList ID="drpPredictedClosingPeriod" TabIndex="2" Enabled="True" EnableTheming="false" AppendDataBoundItems="true" CssClass="drpDownListMedium"
                                                                                                            runat="server" Width="62px" BackColor="#e7e7e7" Style="border: 1px solid #e7e7e7" AutoPostBack="True" OnTextChanged="drpPredictedClosingPeriod_SelectedIndexChanged">
                                                                                                            <asp:ListItem Text="Days" Value="Days"></asp:ListItem>
                                                                                                            <asp:ListItem Text="Months" Value="Months"></asp:ListItem>
                                                                                                            <asp:ListItem Text="Weeks" Value="Weeks"></asp:ListItem>
                                                                                                        </asp:DropDownList>
                                                                                                    </ContentTemplate>
                                                                                                </asp:UpdatePanel>
                                                                                            </td>
                                                                                            <td style="width: 5%;"></td>
                                                                                            <td class="ControlLabel" style="width: 17%;">Predicted Closing Date *
                                                                                                <asp:RequiredFieldValidator ID="RequiredFieldValidator13" runat="server" ControlToValidate="txtPredictedClosingDate"
                                                                                                    ErrorMessage="Predicted Closing Date is mandatory" Display="Dynamic">*</asp:RequiredFieldValidator>
                                                                                            </td>
                                                                                            <td class="ControlTextBox3" style="width: 28%;">
                                                                                                <asp:UpdatePanel ID="UpdatePanel123" runat="server" UpdateMode="Conditional">
                                                                                                    <ContentTemplate>
                                                                                                        <asp:TextBox ID="txtPredictedClosingDate" runat="server" TabIndex="2" Enabled="false"
                                                                                                            CssClass="cssTextBox"></asp:TextBox>
                                                                                                        <cc1:CalendarExtender ID="CalendarExtender4" runat="server" Animated="true" Format="dd/MM/yyyy"
                                                                                                            PopupButtonID="ImageButton2" PopupPosition="BottomLeft" TargetControlID="txtPredictedClosingDate">
                                                                                                        </cc1:CalendarExtender>
                                                                                                    </ContentTemplate>
                                                                                                </asp:UpdatePanel>
                                                                                            </td>
                                                                                            <td style="width: 7%;">

                                                                                                <asp:ImageButton ID="ImageButton2" ImageUrl="App_Themes/NewTheme/images/cal.gif"
                                                                                                    CausesValidation="False" Width="20px" runat="server" Visible="false" />
                                                                                            </td>
                                                                                        </tr>
                                                                                        <tr style="height: 2px">
                                                                                        </tr>
                                                                                        <tr style="height: 30px">
                                                                                            <td class="ControlLabel" style="width: 20%;">Potential Amount *
                                                                                                <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" ControlToValidate="txtPotentialPotAmount"
                                                                                                    ErrorMessage="Potential Amount is mandatory" Display="Dynamic">*</asp:RequiredFieldValidator>
                                                                                            </td>
                                                                                            <td class="ControlTextBox3" style="width: 28%;">
                                                                                                <asp:TextBox ID="txtPotentialPotAmount" runat="server" TabIndex="3"
                                                                                                    CssClass="cssTextBox"></asp:TextBox>
                                                                                            </td>
                                                                                            <td style="width: 5%;"></td>
                                                                                            <td class="ControlLabel" style="width: 17%;">Weighted Amount *
                                                                                                <asp:RequiredFieldValidator ID="RequiredFieldValidator11" runat="server" ControlToValidate="txtPotentialWeightedAmount"
                                                                                                    ErrorMessage="Potential Weighted Amount is mandatory" Display="Dynamic">*</asp:RequiredFieldValidator>
                                                                                            </td>
                                                                                            <td class="ControlTextBox3" style="width: 28%;">
                                                                                                <asp:TextBox ID="txtPotentialWeightedAmount" runat="server" TabIndex="4" Enabled="false"
                                                                                                    CssClass="cssTextBox"></asp:TextBox>
                                                                                            </td>
                                                                                            <td style="width: 7%;"></td>
                                                                                        </tr>
                                                                                        <tr style="height: 2px">
                                                                                        </tr>
                                                                                        <tr style="height: 30px">
                                                                                            <td class="ControlLabel" style="width: 20%;">
                                                                                                <%--Predicted Closing Period *
                                                                                                <asp:CompareValidator ID="CompareValidator1" runat="server" ControlToValidate="drpPredictedClosingPeriod"
                                                                                                    Display="Dynamic" ErrorMessage="Please Select Predicted Closing Period" Operator="GreaterThan"
                                                                                                    Text="*" ValueToCompare="0"></asp:CompareValidator>--%>
                                                                                                Interest Level *
                                                                                                <asp:CompareValidator ID="CompareValidator2" runat="server" ControlToValidate="drpInterestLevel"
                                                                                                    Display="Dynamic" ErrorMessage="Please Select Interest Level" Operator="GreaterThan"
                                                                                                    Text="*" ValueToCompare="0"></asp:CompareValidator>
                                                                                            </td>
                                                                                            <td style="width: 28%;" class="ControlDrpBorder">
                                                                                                <asp:DropDownList ID="drpInterestLevel" TabIndex="6" Enabled="True" EnableTheming="false" AppendDataBoundItems="true" CssClass="drpDownListMedium"
                                                                                                    runat="server" Width="100%" BackColor="#e7e7e7" Style="border: 1px solid #e7e7e7" Height="26px">
                                                                                                    <asp:ListItem Text="Hot" Value="Hot"></asp:ListItem>
                                                                                                    <asp:ListItem Text="Warm" Value="Warm"></asp:ListItem>
                                                                                                    <asp:ListItem Text="Cool" Value="Cool"></asp:ListItem>
                                                                                                </asp:DropDownList>
                                                                                            </td>
                                                                                            <td style="width: 5%;"></td>
                                                                                            <td style="width: 17%;"></td>
                                                                                            <td style="width: 28%;"></td>
                                                                                            <td style="width: 7%;"></td>
                                                                                        </tr>
                                                                                        <tr style="height: 9px">
                                                                                        </tr>
                                                                                    </table>
                                                                                    <table style="width: 750px" cellpadding="1" cellspacing="1">
                                                                                        <tr style="height: 5px">
                                                                                        </tr>
                                                                                        <tr>
                                                                                            <td style="width: 750px;" align="left">

                                                                                                <asp:Label ID="ttt" runat="server" Text="Interested Products : " Font-Bold="true" Font-Size="Larger" ForeColor="Black"></asp:Label>
                                                                                            </td>
                                                                                        </tr>
                                                                                        <tr>
                                                                                            <td style="text-align: left; width: 750px">
                                                                                                <asp:ValidationSummary ID="ValidationSummary3" runat="server" Font-Names="'Trebuchet MS'" Font-Size="12pt"
                                                                                                    HeaderText="Validation Messages" ShowMessageBox="true" ValidationGroup="product" ShowSummary="false" />
                                                                                                <asp:Panel ID="pnlproduct" runat="server" Visible="false">
                                                                                                    <div style="text-align: left; width: 750px">
                                                                                                        <table style="width: 750px; border: 0px solid #86b2d1" align="center" cellpadding="2" cellspacing="2">
                                                                                                            <tr style="height: 8px">
                                                                                                                <td>
                                                                                                                    <asp:HiddenField ID="HiddenField5" runat="server" Value="0" />
                                                                                                                </td>
                                                                                                            </tr>

                                                                                                            <tr>
                                                                                                                <td class="ControlLabel" style="width: 24%;"></td>
                                                                                                                <td class="ControlLabel" style="width: 20%;">Product Name *
                                                                                                                    <asp:CompareValidator ID="CompareValidator13" runat="server" ControlToValidate="drpproduct"
                                                                                                                        ValidationGroup="product" ErrorMessage="Please Select Product Name" Operator="GreaterThan"
                                                                                                                        Text="*" ValueToCompare="0"></asp:CompareValidator>
                                                                                                                </td>
                                                                                                                <td style="width: 25%;" class="ControlDrpBorder">
                                                                                                                    <asp:DropDownList ID="drpproduct" runat="server" AppendDataBoundItems="true" BackColor="#e7e7e7" CssClass="drpDownListMedium" DataTextField="ProductName" DataValueField="ItemCode" Height="26px" Style="border: 1px solid #e7e7e7" TabIndex="1" Width="100%">
                                                                                                                    </asp:DropDownList>
                                                                                                                </td>
                                                                                                                <td style="width: 30%;"></td>
                                                                                                            </tr>

                                                                                                            <tr>
                                                                                                                <td colspan="5">
                                                                                                                    <table style="width: 100%">
                                                                                                                        <tr>
                                                                                                                            <td style="width: 30%;"></td>
                                                                                                                            <td style="width: 20%;" align="right">
                                                                                                                                <asp:Button ID="cmdSaveproduct" runat="server" CssClass="savebutton1231"
                                                                                                                                    EnableTheming="false" OnClick="cmdSaveproduct_Click" Text="" Height="45px"
                                                                                                                                    ValidationGroup="product" />

                                                                                                                                <asp:Button ID="cmdUpdateproduct" runat="server" CssClass="Updatebutton1231"
                                                                                                                                    EnableTheming="false" Height="45px"
                                                                                                                                    OnClick="cmdUpdateproduct_Click" Text="" ValidationGroup="product"
                                                                                                                                    Width="45px" />

                                                                                                                            </td>
                                                                                                                            <td style="width: 20%;" align="left">
                                                                                                                                <asp:Button ID="cmdCancelproduct" runat="server" CssClass="CloseWindow6" Height="45px" OnClick="cmdCancelproduct_Click" CausesValidation="false"
                                                                                                                                    EnableTheming="false" />
                                                                                                                            </td>
                                                                                                                            <td style="width: 30%;"></td>
                                                                                                                        </tr>
                                                                                                                    </table>
                                                                                                                </td>
                                                                                                            </tr>
                                                                                                        </table>
                                                                                                    </div>
                                                                                                </asp:Panel>
                                                                                            </td>
                                                                                        </tr>
                                                                                        <tr>
                                                                                            <td style="width: 750px" align="center">
                                                                                                <asp:Button ID="BtnAddproduct" runat="server" OnClick="BtnAddproduct_Click" CssClass="ButtonAdd66" CausesValidation="False"
                                                                                                    Text="" EnableTheming="false"></asp:Button>
                                                                                                <asp:HiddenField ID="HiddenField6" runat="server" Value="0" />
                                                                                            </td>
                                                                                        </tr>
                                                                                        <tr style="width: 750px">
                                                                                            <td style="width: 750px">
                                                                                                <div class="mainGridHold" id="searchGrid123" style="width: 750px" align="center">
                                                                                                    <asp:GridView ID="GrdViewLeadproduct" runat="server" AllowSorting="True" AutoGenerateColumns="False" OnRowDeleting="GrdViewLeadproduct_RowDeleting" OnSelectedIndexChanged="GrdViewLeadproduct_SelectedIndexChanged"
                                                                                                        Width="100%" DataKeyNames="Product_interest_Id" AllowPaging="True" EmptyDataText="No Interested Products found." CssClass="someClass">
                                                                                                        <EmptyDataRowStyle CssClass="GrdContent" />
                                                                                                        <Columns>
                                                                                                            <asp:BoundField DataField="SlNo" HeaderText="SlNo" HeaderStyle-BorderColor="Gray" />
                                                                                                            <asp:BoundField DataField="Product_Name" HeaderText="Product Name" HeaderStyle-BorderColor="Gray" />
                                                                                                            <asp:BoundField DataField="Product_Id" HeaderText="Product Id" HeaderStyle-BorderColor="Gray" />
                                                                                                            <asp:TemplateField HeaderStyle-Width="30px" ItemStyle-CssClass="command" HeaderStyle-BorderColor="Gray">
                                                                                                                <ItemTemplate>
                                                                                                                    <asp:ImageButton ID="btnEdit" runat="server" CommandName="Select" SkinID="edit" CausesValidation="false" />
                                                                                                                </ItemTemplate>
                                                                                                                <ItemStyle HorizontalAlign="Center" />
                                                                                                            </asp:TemplateField>
                                                                                                        </Columns>
                                                                                                        <PagerTemplate>
                                                                                                            <table style="border-color: white">
                                                                                                                <tr style="border-color: white; height: 1px">
                                                                                                                </tr>
                                                                                                                <tr style="border-color: white">
                                                                                                                    <td style="border-color: white">Goto Page
                                                                                                                    </td>
                                                                                                                    <td style="border-color: white">
                                                                                                                        <asp:DropDownList ID="ddlPageSelector" runat="server" Style="border: 1px solid blue" AutoPostBack="true" SkinID="skinPagerDdlBox">
                                                                                                                        </asp:DropDownList>
                                                                                                                    </td>
                                                                                                                    <td style="border-color: white">
                                                                                                                        <asp:Button Text="" CommandName="Page" CommandArgument="First" runat="server" CssClass="NewFirst" EnableTheming="false" Width="22px" Height="18px"
                                                                                                                            ID="btnFirst" />
                                                                                                                    </td>
                                                                                                                    <td style="border-color: white">
                                                                                                                        <asp:Button Text="" CommandName="Page" CommandArgument="Prev" runat="server" CssClass="NewPrev" EnableTheming="false" Width="22px" Height="18px"
                                                                                                                            ID="btnPrevious" />
                                                                                                                    </td>
                                                                                                                    <td style="border-color: white">
                                                                                                                        <asp:Button Text="" CommandName="Page" CommandArgument="Next" runat="server" CssClass="NewNext" EnableTheming="false" Width="22px" Height="18px"
                                                                                                                            ID="btnNext" />
                                                                                                                    </td>
                                                                                                                    <td style="border-color: white">
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
                                                                                </ContentTemplate>
                                                                            </cc1:TabPanel>
                                                                            <cc1:TabPanel ID="TabPanel1" runat="server" HeaderText="Stages">
                                                                                <HeaderTemplate>
                                                                                    <div>
                                                                                        <table>
                                                                                            <tr>
                                                                                                <td><b>Stages</b> </td>
                                                                                            </tr>
                                                                                        </table>
                                                                                    </div>
                                                                                </HeaderTemplate>
                                                                                <ContentTemplate>
                                                                                    <div style="text-align: left;">
                                                                                        <table style="width: 770px" cellpadding="1" cellspacing="1">
                                                                                            <tr>
                                                                                                <td style="text-align: left; width: 770px">
                                                                                                    <asp:ValidationSummary ID="VSContact" runat="server" Font-Names="'Trebuchet MS'" Font-Size="12pt"
                                                                                                        HeaderText="Validation Messages" ShowMessageBox="true" ValidationGroup="contact" ShowSummary="false" />
                                                                                                    <asp:Panel ID="pnlStage" runat="server" Visible="false">
                                                                                                        <div style="text-align: left; width: 750px">
                                                                                                            <table style="width: 750px; border: 0px solid #86b2d1" align="center" cellpadding="2" cellspacing="2">
                                                                                                                <tr style="height: 8px">
                                                                                                                    <td>
                                                                                                                        <asp:HiddenField ID="hdCurrentRow" runat="server" Value="0" />
                                                                                                                    </td>
                                                                                                                </tr>
                                                                                                                <tr>
                                                                                                                    <td class="ControlLabel" style="width: 23%;">
                                                                                                                        <asp:RequiredFieldValidator ID="RequiredFieldValidator6" runat="server" ControlToValidate="txtStageStartDate"
                                                                                                                            ErrorMessage="Stage Start Date is mandatory" ValidationGroup="contact" Text="*"></asp:RequiredFieldValidator>
                                                                                                                        Start Date *
                                                                                                                    </td>
                                                                                                                    <td class="ControlTextBox3" style="width: 25%;">
                                                                                                                        <asp:TextBox ID="txtStageStartDate" runat="server" TabIndex="1"
                                                                                                                            CssClass="cssTextBox"></asp:TextBox>
                                                                                                                    </td>
                                                                                                                    <td style="width: 8%;" align="left">
                                                                                                                        <cc1:CalendarExtender ID="CalendarExtender3" runat="server" Animated="true" Format="dd/MM/yyyy"
                                                                                                                            PopupButtonID="ImageButton4" PopupPosition="BottomLeft" TargetControlID="txtStageStartDate">
                                                                                                                        </cc1:CalendarExtender>
                                                                                                                        <asp:ImageButton ID="ImageButton4" ImageUrl="App_Themes/NewTheme/images/cal.gif"
                                                                                                                            CausesValidation="False" Width="20px" runat="server" />
                                                                                                                    </td>
                                                                                                                    <td class="ControlLabel" style="width: 14%;">
                                                                                                                        <asp:RequiredFieldValidator ID="RequiredFieldValidator5" runat="server" ControlToValidate="txtStageEndDate"
                                                                                                                            ErrorMessage="Stage End Date is mandatory" ValidationGroup="contact">*</asp:RequiredFieldValidator>
                                                                                                                        End Date *
                                                                                                                    </td>
                                                                                                                    <td class="ControlTextBox3" style="width: 25%;">
                                                                                                                        <asp:TextBox ID="txtStageEndDate" runat="server" TabIndex="2"
                                                                                                                            CssClass="cssTextBox"></asp:TextBox>
                                                                                                                    </td>
                                                                                                                    <td style="width: 10%;" align="left">
                                                                                                                        <cc1:CalendarExtender ID="CalendarExtender2" runat="server" Animated="true" Format="dd/MM/yyyy"
                                                                                                                            PopupButtonID="ImageButton3" PopupPosition="BottomLeft" TargetControlID="txtStageEndDate">
                                                                                                                        </cc1:CalendarExtender>
                                                                                                                        <asp:ImageButton ID="ImageButton3" ImageUrl="App_Themes/NewTheme/images/cal.gif"
                                                                                                                            CausesValidation="False" Width="20px" runat="server" />
                                                                                                                    </td>
                                                                                                                </tr>
                                                                                                                <tr style="height: 2px">
                                                                                                                </tr>
                                                                                                                <tr>
                                                                                                                    <td class="ControlLabel" style="width: 23%;">Stage Name *
                                                                                                                    <asp:CompareValidator ID="CompareValidator6" runat="server" ControlToValidate="drpStageName"
                                                                                                                        ValidationGroup="contact" ErrorMessage="Please Select Stage Name" Operator="GreaterThan"
                                                                                                                        Text="*" ValueToCompare="0"></asp:CompareValidator>
                                                                                                                    </td>
                                                                                                                    <td class="ControlDrpBorder" style="width: 25%;">
                                                                                                                        <asp:UpdatePanel ID="UpdatePanel3" runat="server" UpdateMode="Conditional">
                                                                                                                            <ContentTemplate>
                                                                                                                                <asp:DropDownList ID="drpStageName" runat="server" AppendDataBoundItems="true" AutoPostBack="true" BackColor="#e7e7e7" CssClass="drpDownListMedium" DataTextField="Stage_Name" DataValueField="Stage_Setup_Id" Height="26px" OnSelectedIndexChanged="drpStageName_SelectedIndexChanged" Style="border: 1px solid #e7e7e7" TabIndex="3" Width="100%">
                                                                                                                                </asp:DropDownList>
                                                                                                                            </ContentTemplate>
                                                                                                                        </asp:UpdatePanel>
                                                                                                                    </td>
                                                                                                                    <td style="width: 8%;"></td>
                                                                                                                    <td class="ControlLabel" style="width: 14%;">Stage Perc *
                                                                                                                    <asp:RequiredFieldValidator ID="RequiredFieldValidator8" runat="server" ControlToValidate="txtStagePerc"
                                                                                                                        ErrorMessage="Stage Perc is mandatory" ValidationGroup="contact" Display="Dynamic">*</asp:RequiredFieldValidator>
                                                                                                                    </td>
                                                                                                                    <td class="ControlTextBox3" style="width: 25%;">
                                                                                                                        <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional">
                                                                                                                            <ContentTemplate>
                                                                                                                                <asp:TextBox ID="txtStagePerc" runat="server" TabIndex="4" AutoPostBack="True" OnTextChanged="txtStagePerc_TextChanged"
                                                                                                                                    CssClass="cssTextBox" Enabled="false"></asp:TextBox>
                                                                                                                            </ContentTemplate>
                                                                                                                        </asp:UpdatePanel>
                                                                                                                    </td>
                                                                                                                    <td style="width: 10%;"></td>
                                                                                                                </tr>
                                                                                                                <tr style="height: 2px">
                                                                                                                </tr>
                                                                                                                <tr>
                                                                                                                    <td class="ControlLabel" style="width: 23%;">Potential Amount *
                                                                                                                   <asp:CompareValidator ID="CompareValidator12" runat="server" ControlToValidate="txtStagePotentialAmount"
                                                                                                                       ValidationGroup="contact" ErrorMessage="Please Select Potential Amount" Operator="GreaterThan"
                                                                                                                       Text="*" ValueToCompare="0"></asp:CompareValidator>
                                                                                                                    </td>
                                                                                                                    <td class="ControlDrpBorder" style="width: 25%;">
                                                                                                                        <%--<asp:TextBox ID="txtStagePotentialAmount" runat="server" TabIndex="5"
                                                                                                                        CssClass="cssTextBox" ></asp:TextBox>--%>
                                                                                                                        <%--<asp:UpdatePanel ID="UpdatePanel7" runat="server" UpdateMode="Conditional">
                                                                                                                        <ContentTemplate>
                                                                                                                             <asp:DropDownList ID="txtStagePotentialAmount" runat="server" AppendDataBoundItems="true" AutoPostBack="true" BackColor="#90c9fc" CssClass="drpDownListMedium" DataTextField="Potential_Amount" DataValueField="Potential_Amount" height="26px" OnSelectedIndexChanged="txtStagePotentialAmount_SelectedIndexChanged" style="border: 1px solid #90c9fc" TabIndex="5" Width="100%">
                                                                                                                             </asp:DropDownList>
                                                                                                                        </ContentTemplate>
                                                                                                                    </asp:UpdatePanel>--%>
                                                                                                                        <asp:TextBox ID="txtStagePotentialAmount" runat="server" TabIndex="5" AutoPostBack="True" OnTextChanged="txtStagePotentialAmount_TextChanged"
                                                                                                                            CssClass="cssTextBox"></asp:TextBox>
                                                                                                                    </td>
                                                                                                                    <td style="width: 8%;"></td>
                                                                                                                    <td class="ControlLabel" style="width: 14%;">Weighted Amount *
                                                                                                                    <asp:RequiredFieldValidator ID="RequiredFieldValidator10" runat="server" ControlToValidate="txtStageWeightedAmount"
                                                                                                                        ErrorMessage="Stage Weighted Amount is mandatory" ValidationGroup="contact" Display="Dynamic">*</asp:RequiredFieldValidator>
                                                                                                                    </td>
                                                                                                                    <td class="ControlTextBox3" style="width: 25%;">
                                                                                                                        <asp:UpdatePanel ID="UpdatePanel8" runat="server" UpdateMode="Conditional">
                                                                                                                            <ContentTemplate>
                                                                                                                                <asp:TextBox ID="txtStageWeightedAmount" runat="server" TabIndex="6"
                                                                                                                                    CssClass="cssTextBox"></asp:TextBox>
                                                                                                                            </ContentTemplate>
                                                                                                                        </asp:UpdatePanel>
                                                                                                                    </td>
                                                                                                                    <td style="width: 10%;"></td>
                                                                                                                </tr>
                                                                                                                <tr style="height: 2px">
                                                                                                                </tr>
                                                                                                                <tr>
                                                                                                                    <td colspan="6">
                                                                                                                        <table style="width: 100%">
                                                                                                                            <tr>
                                                                                                                                <td class="ControlLabel" style="width: 22%;">
                                                                                                                                    <asp:RequiredFieldValidator ID="RequiredFieldValidator16" runat="server" ControlToValidate="txtStageRemarks"
                                                                                                                                        ErrorMessage="Remarks is mandatory" ValidationGroup="contact" Display="Dynamic">*</asp:RequiredFieldValidator>
                                                                                                                                    Remarks *
                                                                                                                                </td>
                                                                                                                                <td class="ControlTextBox3" style="width: 72%;">
                                                                                                                                    <asp:TextBox ID="txtStageRemarks" runat="server"
                                                                                                                                        Style="overflow: hidden; padding: 0px; font-family: 'Trebuchet MS'; font-size: 13px; background-color: #e7e7e7" TextMode="MultiLine" Width="105%" Height="100px" TabIndex="7"></asp:TextBox>
                                                                                                                                </td>
                                                                                                                                <td style="width: 5%;"></td>
                                                                                                                            </tr>

                                                                                                                        </table>
                                                                                                                    </td>
                                                                                                                </tr>
                                                                                                                <tr>
                                                                                                                    <td colspan="6">
                                                                                                                        <table style="width: 100%">
                                                                                                                            <tr>
                                                                                                                                <td style="width: 30%;"></td>
                                                                                                                                <td style="width: 20%;" align="right">

                                                                                                                                    <asp:Button ID="cmdSaveContact" runat="server" CssClass="savebutton1231"
                                                                                                                                        EnableTheming="false" OnClick="cmdSaveContact_Click" Text="" Height="45px"
                                                                                                                                        ValidationGroup="contact" />

                                                                                                                                    <asp:Button ID="cmdUpdateContact" runat="server" CssClass="Updatebutton1231"
                                                                                                                                        EnableTheming="false" Height="45px"
                                                                                                                                        OnClick="cmdUpdateContact_Click" Text="" ValidationGroup="contact"
                                                                                                                                        Width="45px" />

                                                                                                                                </td>
                                                                                                                                <td style="width: 20%;" align="left">
                                                                                                                                    <asp:Button ID="cmdCancelContact" runat="server" CssClass="CloseWindow6" Height="45px" OnClick="cmdCancelContact_Click" CausesValidation="false"
                                                                                                                                        EnableTheming="false" />
                                                                                                                                </td>
                                                                                                                                <td style="width: 30%;"></td>
                                                                                                                            </tr>
                                                                                                                        </table>
                                                                                                                    </td>
                                                                                                                </tr>
                                                                                                            </table>
                                                                                                        </div>
                                                                                                    </asp:Panel>
                                                                                                </td>
                                                                                            </tr>
                                                                                            <tr>
                                                                                                <td style="width: 750px" align="center">
                                                                                                    <asp:Button ID="BtnAddStage" runat="server" OnClick="BtnAddStage_Click" CssClass="ButtonAdd66" CausesValidation="False"
                                                                                                        Text="" EnableTheming="false"></asp:Button>
                                                                                                    <asp:HiddenField ID="hdBtnAddStage" runat="server" Value="0" />
                                                                                                </td>
                                                                                            </tr>
                                                                                            <tr style="width: 750px">
                                                                                                <td style="width: 750px">
                                                                                                    <div class="mainGridHold" id="searchGrid1" style="width: 750px" align="center">
                                                                                                        <asp:GridView ID="GrdViewLeadStage" runat="server" AllowSorting="True" AutoGenerateColumns="False" OnRowDeleting="GrdViewLeadStage_RowDeleting" OnSelectedIndexChanged="GrdViewLeadStage_SelectedIndexChanged"
                                                                                                            Width="100%" DataKeyNames="Stage_Id" AllowPaging="True" EmptyDataText="No Stages found." CssClass="someClass">
                                                                                                            <EmptyDataRowStyle CssClass="GrdContent" />
                                                                                                            <Columns>
                                                                                                                <asp:BoundField DataField="Start_Date" HeaderText="Start Date" DataFormatString="{0:dd/MM/yyyy}" HeaderStyle-BorderColor="Gray" />
                                                                                                                <asp:BoundField DataField="End_Date" HeaderText="End Date" DataFormatString="{0:dd/MM/yyyy}" HeaderStyle-BorderColor="Gray" />
                                                                                                                <asp:BoundField DataField="Stage_Name" HeaderText="Stage Name" HeaderStyle-BorderColor="Gray" />
                                                                                                                <asp:BoundField DataField="Stage_Setup_Id" HeaderText="Stage Setup Id" HeaderStyle-BorderColor="Gray" />
                                                                                                                <asp:BoundField DataField="Stage_Perc" HeaderText="Stage Perc" HeaderStyle-BorderColor="Gray" />
                                                                                                                <asp:BoundField DataField="Potential_Amount" HeaderText="Potential Amount" HeaderStyle-BorderColor="Gray" />
                                                                                                                <asp:BoundField DataField="Weighted_Amount" HeaderText="Weighted Amount" HeaderStyle-BorderColor="Gray" />
                                                                                                                <asp:BoundField DataField="Remarks" HeaderText="Remarks" HeaderStyle-BorderColor="Gray" />
                                                                                                                <asp:TemplateField HeaderStyle-Width="30px" ItemStyle-CssClass="command" HeaderStyle-BorderColor="Gray">
                                                                                                                    <ItemTemplate>
                                                                                                                        <asp:ImageButton ID="btnEdit" runat="server" CommandName="Select" SkinID="edit" CausesValidation="false" />
                                                                                                                    </ItemTemplate>
                                                                                                                    <ItemStyle HorizontalAlign="Center" />
                                                                                                                </asp:TemplateField>
                                                                                                            </Columns>
                                                                                                            <PagerTemplate>
                                                                                                                <table style="border-color: white">
                                                                                                                    <tr style="border-color: white; height: 1px">
                                                                                                                    </tr>
                                                                                                                    <tr style="border-color: white">
                                                                                                                        <td style="border-color: white">Goto Page
                                                                                                                        </td>
                                                                                                                        <td style="border-color: white">
                                                                                                                            <asp:DropDownList ID="ddlPageSelector" runat="server" Style="border: 1px solid blue" AutoPostBack="true" SkinID="skinPagerDdlBox">
                                                                                                                            </asp:DropDownList>
                                                                                                                        </td>
                                                                                                                        <td style="border-color: white">
                                                                                                                            <asp:Button Text="" CommandName="Page" CommandArgument="First" runat="server" CssClass="NewFirst" EnableTheming="false" Width="22px" Height="18px"
                                                                                                                                ID="btnFirst" />
                                                                                                                        </td>
                                                                                                                        <td style="border-color: white">
                                                                                                                            <asp:Button Text="" CommandName="Page" CommandArgument="Prev" runat="server" CssClass="NewPrev" EnableTheming="false" Width="22px" Height="18px"
                                                                                                                                ID="btnPrevious" />
                                                                                                                        </td>
                                                                                                                        <td style="border-color: white">
                                                                                                                            <asp:Button Text="" CommandName="Page" CommandArgument="Next" runat="server" CssClass="NewNext" EnableTheming="false" Width="22px" Height="18px"
                                                                                                                                ID="btnNext" />
                                                                                                                        </td>
                                                                                                                        <td style="border-color: white">
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
                                                                                    </div>
                                                                                </ContentTemplate>
                                                                            </cc1:TabPanel>
                                                                            <cc1:TabPanel ID="TabPanel3" runat="server" HeaderText="Competitors">
                                                                                <HeaderTemplate>
                                                                                    <div>
                                                                                        <table>
                                                                                            <tr>
                                                                                                <td><b>Competitors</b> </td>
                                                                                            </tr>
                                                                                        </table>
                                                                                    </div>
                                                                                </HeaderTemplate>
                                                                                <ContentTemplate>
                                                                                    <div style="text-align: left;">
                                                                                        <table style="width: 770px" cellpadding="1" cellspacing="1">
                                                                                            <tr>
                                                                                                <td style="text-align: left; width: 750px">
                                                                                                    <asp:ValidationSummary ID="ValidationSummary1" runat="server" Font-Names="'Trebuchet MS'" Font-Size="12pt"
                                                                                                        HeaderText="Validation Messages" ShowMessageBox="true" ValidationGroup="Competitor" ShowSummary="false" />
                                                                                                    <asp:Panel ID="pnlCompetitor" runat="server" Visible="false">
                                                                                                        <div style="text-align: left; width: 750px">
                                                                                                            <table style="width: 750px; border: 0px solid #86b2d1" align="center" cellpadding="2" cellspacing="2">
                                                                                                                <tr style="height: 8px">
                                                                                                                    <td>
                                                                                                                        <asp:HiddenField ID="HiddenField1" runat="server" Value="0" />
                                                                                                                    </td>
                                                                                                                </tr>
                                                                                                                <tr>
                                                                                                                    <td class="ControlLabel" style="width: 23%;">
                                                                                                                        <asp:RequiredFieldValidator ID="RequiredFieldValidator7" runat="server" ControlToValidate="txtCompetitorName"
                                                                                                                            ErrorMessage="Competitor Name is mandatory" ValidationGroup="Competitor" Text="*"></asp:RequiredFieldValidator>
                                                                                                                        Competitor Name *
                                                                                                                    </td>
                                                                                                                    <td class="ControlTextBox3" style="width: 25%;">
                                                                                                                        <asp:TextBox ID="txtCompetitorName" runat="server" TabIndex="1"
                                                                                                                            CssClass="cssTextBox"></asp:TextBox>
                                                                                                                    </td>
                                                                                                                    <td style="width: 8%;" align="left"></td>
                                                                                                                    <td class="ControlLabel" style="width: 13%;">
                                                                                                                        <asp:CompareValidator ID="CompareValidator7" runat="server" ControlToValidate="drpThreatLevel"
                                                                                                                            ValidationGroup="Competitor" ErrorMessage="Please Select Threat Level" Operator="GreaterThan"
                                                                                                                            Text="*" ValueToCompare="0"></asp:CompareValidator>
                                                                                                                        Threat Level *
                                                                                                                    </td>
                                                                                                                    <td class="ControlDrpBorder" style="width: 25%;">
                                                                                                                        <asp:UpdatePanel ID="UpdatePanel4" runat="server" UpdateMode="Conditional">
                                                                                                                            <ContentTemplate>
                                                                                                                                <asp:DropDownList ID="drpThreatLevel" TabIndex="6" Enabled="True" EnableTheming="false" AppendDataBoundItems="true" CssClass="drpDownListMedium"
                                                                                                                                    runat="server" Width="100%" BackColor="#e7e7e7" Style="border: 1px solid #e7e7e7" Height="26px">
                                                                                                                                    <asp:ListItem Text="High" Value="High"></asp:ListItem>
                                                                                                                                    <asp:ListItem Text="Low" Value="Low"></asp:ListItem>
                                                                                                                                    <asp:ListItem Text="Medium" Value="Medium"></asp:ListItem>
                                                                                                                                </asp:DropDownList>
                                                                                                                            </ContentTemplate>
                                                                                                                        </asp:UpdatePanel>
                                                                                                                    </td>
                                                                                                                    <td style="width: 12%;" align="left"></td>
                                                                                                                </tr>
                                                                                                                <tr style="height: 2px">
                                                                                                                </tr>
                                                                                                                <tr>
                                                                                                                    <td colspan="6">
                                                                                                                        <table style="width: 100%">
                                                                                                                            <tr>
                                                                                                                                <td class="ControlLabel" style="width: 22%;">
                                                                                                                                    <%--<asp:RequiredFieldValidator ID="RequiredFieldValidator21" runat="server" ControlToValidate="txtCompetitorRemarks"
                                                                                                                                        ErrorMessage="Remarks is mandatory" ValidationGroup="Competitor" Display="Dynamic">*</asp:RequiredFieldValidator>--%>
                                                                                                                                Remarks
                                                                                                                                </td>
                                                                                                                                <td class="ControlTextBox3" style="width: 72%;">
                                                                                                                                    <asp:TextBox ID="txtCompetitorRemarks" runat="server"
                                                                                                                                        Style="overflow: hidden; padding: 0px; font-family: 'Trebuchet MS'; font-size: 13px; background-color: #e7e7e7" TextMode="MultiLine" Width="105%" Height="100px" TabIndex="7"></asp:TextBox>
                                                                                                                                </td>
                                                                                                                                <td style="width: 5%;"></td>
                                                                                                                            </tr>

                                                                                                                        </table>
                                                                                                                    </td>
                                                                                                                </tr>
                                                                                                                <tr>
                                                                                                                    <td colspan="6">
                                                                                                                        <table style="width: 100%">
                                                                                                                            <tr>
                                                                                                                                <td style="width: 30%;"></td>
                                                                                                                                <td style="width: 20%;" align="right">

                                                                                                                                    <asp:Button ID="cmdSaveCompetitor" runat="server" CssClass="savebutton1231"
                                                                                                                                        EnableTheming="false" OnClick="cmdSaveCompetitor_Click" Text="" Height="45px"
                                                                                                                                        ValidationGroup="Competitor" />

                                                                                                                                    <asp:Button ID="cmdUpdateCompetitor" runat="server" CssClass="Updatebutton1231"
                                                                                                                                        EnableTheming="false" Height="45px"
                                                                                                                                        OnClick="cmdUpdateCompetitor_Click" Text="" ValidationGroup="Competitor"
                                                                                                                                        Width="45px" />
                                                                                                                                </td>
                                                                                                                                <td style="width: 20%;" align="left">
                                                                                                                                    <asp:Button ID="cmdCancelCompetitor" runat="server" CssClass="CloseWindow6" Height="45px" OnClick="cmdCancelCompetitor_Click" CausesValidation="false"
                                                                                                                                        EnableTheming="false" />
                                                                                                                                </td>
                                                                                                                                <td style="width: 30%;"></td>
                                                                                                                            </tr>
                                                                                                                        </table>
                                                                                                                    </td>
                                                                                                                </tr>
                                                                                                            </table>
                                                                                                        </div>
                                                                                                    </asp:Panel>
                                                                                                </td>
                                                                                            </tr>
                                                                                            <tr>
                                                                                                <td style="width: 750px" align="center">
                                                                                                    <asp:Button ID="BtnAddCompetitor" runat="server" OnClick="BtnAddCompetitor_Click" CssClass="ButtonAdd66" CausesValidation="False"
                                                                                                        Text="" EnableTheming="false"></asp:Button>
                                                                                                    <asp:HiddenField ID="HiddenField2" runat="server" Value="0" />
                                                                                                </td>
                                                                                            </tr>
                                                                                            <tr style="width: 750px">
                                                                                                <td style="width: 750px">
                                                                                                    <div class="mainGridHold" id="searchGrid2" style="width: 750px" align="center">
                                                                                                        <asp:GridView ID="GrdViewLeadCompetitor" runat="server" AllowSorting="True" AutoGenerateColumns="False" OnRowDeleting="GrdViewLeadCompetitor_RowDeleting" OnSelectedIndexChanged="GrdViewLeadCompetitor_SelectedIndexChanged"
                                                                                                            Width="100%" DataKeyNames="Competitor_Id" AllowPaging="True" EmptyDataText="No Competitors found." CssClass="someClass">
                                                                                                            <EmptyDataRowStyle CssClass="GrdContent" />
                                                                                                            <Columns>
                                                                                                                <asp:BoundField DataField="Competitor_Name" HeaderText="Competitor Name" HeaderStyle-BorderColor="Gray" />
                                                                                                                <asp:BoundField DataField="Threat_Level" HeaderText="Threat Level" HeaderStyle-BorderColor="Gray" />
                                                                                                                <asp:BoundField DataField="Remarks" HeaderText="Remarks" HeaderStyle-BorderColor="Gray" />
                                                                                                                <asp:TemplateField HeaderStyle-Width="30px" ItemStyle-CssClass="command" HeaderStyle-BorderColor="Gray">
                                                                                                                    <ItemTemplate>
                                                                                                                        <asp:ImageButton ID="btnEdit" runat="server" CommandName="Select" SkinID="edit" CausesValidation="false" />
                                                                                                                    </ItemTemplate>
                                                                                                                    <ItemStyle HorizontalAlign="Center" />
                                                                                                                </asp:TemplateField>
                                                                                                            </Columns>
                                                                                                            <PagerTemplate>
                                                                                                                <table style="border-color: white">
                                                                                                                    <tr style="border-color: white; height: 1px">
                                                                                                                    </tr>
                                                                                                                    <tr style="border-color: white">
                                                                                                                        <td style="border-color: white">Goto Page
                                                                                                                        </td>
                                                                                                                        <td style="border-color: white">
                                                                                                                            <asp:DropDownList ID="ddlPageSelector" runat="server" Style="border: 1px solid blue" AutoPostBack="true" SkinID="skinPagerDdlBox">
                                                                                                                            </asp:DropDownList>
                                                                                                                        </td>
                                                                                                                        <td style="border-color: white">
                                                                                                                            <asp:Button Text="" CommandName="Page" CommandArgument="First" runat="server" CssClass="NewFirst" EnableTheming="false" Width="22px" Height="18px"
                                                                                                                                ID="btnFirst" />
                                                                                                                        </td>
                                                                                                                        <td style="border-color: white">
                                                                                                                            <asp:Button Text="" CommandName="Page" CommandArgument="Prev" runat="server" CssClass="NewPrev" EnableTheming="false" Width="22px" Height="18px"
                                                                                                                                ID="btnPrevious" />
                                                                                                                        </td>
                                                                                                                        <td style="border-color: white">
                                                                                                                            <asp:Button Text="" CommandName="Page" CommandArgument="Next" runat="server" CssClass="NewNext" EnableTheming="false" Width="22px" Height="18px"
                                                                                                                                ID="btnNext" />
                                                                                                                        </td>
                                                                                                                        <td style="border-color: white">
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
                                                                                    </div>
                                                                                </ContentTemplate>
                                                                            </cc1:TabPanel>
                                                                            <cc1:TabPanel ID="TabPanel4" runat="server" HeaderText="Activities">
                                                                                <HeaderTemplate>
                                                                                    <div>
                                                                                        <table>
                                                                                            <tr>
                                                                                                <td><b>Activities</b> </td>
                                                                                            </tr>
                                                                                        </table>
                                                                                    </div>
                                                                                </HeaderTemplate>
                                                                                <ContentTemplate>
                                                                                    <div style="text-align: left;">
                                                                                        <table style="width: 770px" cellpadding="1" cellspacing="1">
                                                                                            <tr>
                                                                                                <td style="text-align: left; width: 750px">
                                                                                                    <asp:ValidationSummary ID="ValidationSummary2" runat="server" Font-Names="'Trebuchet MS'" Font-Size="12pt"
                                                                                                        HeaderText="Validation Messages" ShowMessageBox="true" ValidationGroup="Activities" ShowSummary="false" />
                                                                                                    <asp:Panel ID="pnlActivity" runat="server" Visible="false">
                                                                                                        <div style="text-align: left; width: 750px">
                                                                                                            <table style="width: 750px; border: 0px solid #86b2d1" align="center" cellpadding="2" cellspacing="2">
                                                                                                                <tr style="height: 8px">
                                                                                                                    <td>
                                                                                                                        <asp:HiddenField ID="HiddenField3" runat="server" Value="0" />
                                                                                                                    </td>
                                                                                                                </tr>
                                                                                                                <tr>
                                                                                                                    <td class="ControlLabel" style="width: 23%;">
                                                                                                                        <asp:CompareValidator ID="CompareValidator9" runat="server" ControlToValidate="drpActivityName"
                                                                                                                            ValidationGroup="Activities" ErrorMessage="Please Select Activity Name" Operator="GreaterThan"
                                                                                                                            Text="*" ValueToCompare="0"></asp:CompareValidator>
                                                                                                                        Activity Name *
                                                                                                                    </td>
                                                                                                                    <td class="ControlDrpBorder" style="width: 25%;">
                                                                                                                        <asp:UpdatePanel ID="UpdatePanel6" runat="server" UpdateMode="Conditional">
                                                                                                                            <ContentTemplate>
                                                                                                                                <asp:DropDownList ID="drpActivityName" runat="server" AppendDataBoundItems="true" AutoPostBack="true" BackColor="#e7e7e7" CssClass="drpDownListMedium" DataTextField="Activity_Name" DataValueField="Activity_Setup_Id" Height="26px" OnSelectedIndexChanged="drpActivityName_SelectedIndexChanged" Style="border: 1px solid #e7e7e7" TabIndex="1" Width="100%">
                                                                                                                                </asp:DropDownList>
                                                                                                                            </ContentTemplate>
                                                                                                                        </asp:UpdatePanel>
                                                                                                                    </td>
                                                                                                                    <td style="width: 8%;" align="left"></td>
                                                                                                                    <td class="ControlLabel" style="width: 13%;">
                                                                                                                        <asp:RequiredFieldValidator ID="RequiredFieldValidator20" runat="server" ControlToValidate="txtActivityLocation"
                                                                                                                            ErrorMessage="Activity Location is mandatory" ValidationGroup="Activities" Text="*"></asp:RequiredFieldValidator>
                                                                                                                        Activity Location *
                                                                                                                    </td>
                                                                                                                    <td class="ControlTextBox3" style="width: 25%;">
                                                                                                                        <asp:TextBox ID="txtActivityLocation" runat="server" TabIndex="2"
                                                                                                                            CssClass="cssTextBox"></asp:TextBox>
                                                                                                                    </td>
                                                                                                                    <td style="width: 12%;" align="left"></td>
                                                                                                                </tr>
                                                                                                                <tr style="height: 2px">
                                                                                                                </tr>
                                                                                                                <tr>
                                                                                                                    <td class="ControlLabel" style="width: 23%;">
                                                                                                                        <asp:RequiredFieldValidator ID="RequiredFieldValidator17" runat="server" ControlToValidate="txtActivityStartDate"
                                                                                                                            ErrorMessage="Activity Start Date is mandatory" ValidationGroup="Activities" Text="*"></asp:RequiredFieldValidator>
                                                                                                                        Start Date *
                                                                                                                    </td>
                                                                                                                    <td class="ControlTextBox3" style="width: 25%;">
                                                                                                                        <asp:TextBox ID="txtActivityStartDate" runat="server" TabIndex="3"
                                                                                                                            CssClass="cssTextBox"></asp:TextBox>
                                                                                                                    </td>
                                                                                                                    <td style="width: 8%;" align="left">
                                                                                                                        <cc1:CalendarExtender ID="CalendarExtender5" runat="server" Animated="true" Format="dd/MM/yyyy"
                                                                                                                            PopupButtonID="ImageButton5" PopupPosition="BottomLeft" TargetControlID="txtActivityStartDate">
                                                                                                                        </cc1:CalendarExtender>
                                                                                                                        <asp:ImageButton ID="ImageButton5" ImageUrl="App_Themes/NewTheme/images/cal.gif"
                                                                                                                            CausesValidation="False" Width="20px" runat="server" />
                                                                                                                    </td>
                                                                                                                    <td class="ControlLabel" style="width: 14%;">
                                                                                                                        <asp:RequiredFieldValidator ID="RequiredFieldValidator19" runat="server" ControlToValidate="txtActivityEndDate"
                                                                                                                            ErrorMessage="Activity End Date is mandatory" ValidationGroup="Activities">*</asp:RequiredFieldValidator>
                                                                                                                        End Date *
                                                                                                                    </td>
                                                                                                                    <td class="ControlTextBox3" style="width: 25%;">
                                                                                                                        <asp:TextBox ID="txtActivityEndDate" runat="server" TabIndex="4"
                                                                                                                            CssClass="cssTextBox"></asp:TextBox>
                                                                                                                    </td>
                                                                                                                    <td style="width: 10%;" align="left">
                                                                                                                        <cc1:CalendarExtender ID="CalendarExtender6" runat="server" Animated="true" Format="dd/MM/yyyy"
                                                                                                                            PopupButtonID="ImageButton6" PopupPosition="BottomLeft" TargetControlID="txtActivityEndDate">
                                                                                                                        </cc1:CalendarExtender>
                                                                                                                        <asp:ImageButton ID="ImageButton6" ImageUrl="App_Themes/NewTheme/images/cal.gif"
                                                                                                                            CausesValidation="False" Width="20px" runat="server" />
                                                                                                                    </td>
                                                                                                                </tr>
                                                                                                                <tr style="height: 2px">
                                                                                                                </tr>
                                                                                                                <tr>
                                                                                                                    <td class="ControlLabel" style="width: 23%;">
                                                                                                                        <asp:CompareValidator ID="CompareValidator8" runat="server" ControlToValidate="drpNextActivity"
                                                                                                                            ValidationGroup="Activities" ErrorMessage="Please Select Next Activity" Operator="GreaterThan"
                                                                                                                            Text="*" ValueToCompare="0"></asp:CompareValidator>
                                                                                                                        Next Activity *
                                                                                                                    </td>
                                                                                                                    <td class="ControlDrpBorder" style="width: 25%;">
                                                                                                                        <asp:UpdatePanel ID="UpdatePanel5" runat="server" UpdateMode="Conditional">
                                                                                                                            <ContentTemplate>
                                                                                                                                <asp:DropDownList ID="drpNextActivity" runat="server" AppendDataBoundItems="true" AutoPostBack="true" BackColor="#e7e7e7" CssClass="drpDownListMedium" DataTextField="Activity_Name" DataValueField="Activity_Setup_Id" Height="26px" OnSelectedIndexChanged="drpNextActivity_SelectedIndexChanged" Style="border: 1px solid #e7e7e7" TabIndex="5" Width="100%">
                                                                                                                                </asp:DropDownList>
                                                                                                                            </ContentTemplate>
                                                                                                                        </asp:UpdatePanel>
                                                                                                                    </td>
                                                                                                                    <td style="width: 8%;" align="left"></td>
                                                                                                                    <td class="ControlLabel" style="width: 14%;">
                                                                                                                        <asp:RequiredFieldValidator ID="RequiredFieldValidator23" runat="server" ControlToValidate="txtNextActivityDate"
                                                                                                                            ErrorMessage="Next Activity Date is mandatory" ValidationGroup="Activities">*</asp:RequiredFieldValidator>
                                                                                                                        Next Activity Date *
                                                                                                                    </td>
                                                                                                                    <td class="ControlTextBox3" style="width: 25%;">
                                                                                                                        <asp:TextBox ID="txtNextActivityDate" runat="server" TabIndex="6"
                                                                                                                            CssClass="cssTextBox"></asp:TextBox>
                                                                                                                    </td>
                                                                                                                    <td style="width: 10%;" align="left">
                                                                                                                        <cc1:CalendarExtender ID="CalendarExtender8" runat="server" Animated="true" Format="dd/MM/yyyy"
                                                                                                                            PopupButtonID="ImageButton8" PopupPosition="BottomLeft" TargetControlID="txtNextActivityDate">
                                                                                                                        </cc1:CalendarExtender>
                                                                                                                        <asp:ImageButton ID="ImageButton8" ImageUrl="App_Themes/NewTheme/images/cal.gif"
                                                                                                                            CausesValidation="False" Width="20px" runat="server" />
                                                                                                                    </td>
                                                                                                                </tr>
                                                                                                                <tr style="height: 2px">
                                                                                                                </tr>
                                                                                                                <tr style="height: 30px">
                                                                                                                    <td class="ControlLabel" style="width: 20%;">Employee *
                                                                                                                    <asp:CompareValidator ID="CompareValidator10" runat="server" ControlToValidate="drpActivityEmployee"
                                                                                                                        ValidationGroup="Activities" ErrorMessage="Please Select Employee" Operator="GreaterThan"
                                                                                                                        Text="*" ValueToCompare="0"></asp:CompareValidator>
                                                                                                                    </td>
                                                                                                                    <td class="ControlDrpBorder" style="width: 25%;">
                                                                                                                        <asp:DropDownList ID="drpActivityEmployee" TabIndex="8" Enabled="True" EnableTheming="false" AppendDataBoundItems="true" CssClass="drpDownListMedium"
                                                                                                                            runat="server" Width="100%" DataTextField="empFirstName" BackColor="#e7e7e7" Style="border: 1px solid #e7e7e7" Height="26px"
                                                                                                                            DataValueField="empno">
                                                                                                                        </asp:DropDownList>
                                                                                                                    </td>
                                                                                                                    <td style="width: 8%;"></td>
                                                                                                                    <td class="ControlLabel" style="width: 17%;">Follow Up *
                                                                                                                    <asp:CompareValidator ID="CompareValidator11" runat="server" ControlToValidate="drpFollowUp"
                                                                                                                        ValidationGroup="Activities" ErrorMessage="Please Select Follow Up" Operator="GreaterThan"
                                                                                                                        Text="*" ValueToCompare="0"></asp:CompareValidator>
                                                                                                                    </td>
                                                                                                                    <td class="ControlDrpBorder" style="width: 25%;">
                                                                                                                        <asp:DropDownList ID="drpFollowUp" TabIndex="6" Enabled="True" EnableTheming="false" AppendDataBoundItems="true" CssClass="drpDownListMedium"
                                                                                                                            runat="server" Width="100%" BackColor="#e7e7e7" Style="border: 1px solid #e7e7e7" Height="26px">
                                                                                                                            <asp:ListItem Text="Yes" Value="Yes"></asp:ListItem>
                                                                                                                            <asp:ListItem Text="No" Value="No"></asp:ListItem>
                                                                                                                        </asp:DropDownList>
                                                                                                                    </td>
                                                                                                                    <td style="width: 10%;"></td>
                                                                                                                </tr>
                                                                                                                <tr style="height: 2px">
                                                                                                                </tr>
                                                                                                                <tr>
                                                                                                                    <td colspan="6">
                                                                                                                        <table style="width: 100%">
                                                                                                                            <tr>
                                                                                                                                <td class="ControlLabel" style="width: 22%;">
                                                                                                                                    <asp:RequiredFieldValidator ID="RequiredFieldValidator18" runat="server" ControlToValidate="txtActivityRemarks"
                                                                                                                                        ErrorMessage="Remarks is mandatory" ValidationGroup="Activities" Display="Dynamic">*</asp:RequiredFieldValidator>
                                                                                                                                    Remarks *
                                                                                                                                </td>
                                                                                                                                <td class="ControlTextBox3" style="width: 72%;">
                                                                                                                                    <asp:TextBox ID="txtActivityRemarks" runat="server"
                                                                                                                                        Style="overflow: hidden; padding: 0px; font-family: 'Trebuchet MS'; font-size: 13px; background-color: #e7e7e7" TextMode="MultiLine" Width="105%" Height="100px" TabIndex="4"></asp:TextBox>
                                                                                                                                </td>
                                                                                                                                <td style="width: 5%;"></td>
                                                                                                                            </tr>

                                                                                                                        </table>
                                                                                                                    </td>
                                                                                                                </tr>
                                                                                                                <tr>
                                                                                                                    <td colspan="6">
                                                                                                                        <table style="width: 100%">
                                                                                                                            <tr>
                                                                                                                                <td style="width: 30%;"></td>
                                                                                                                                <td style="width: 20%;" align="right">

                                                                                                                                    <asp:Button ID="cmdSaveActivity" runat="server" CssClass="savebutton1231"
                                                                                                                                        EnableTheming="false" OnClick="cmdSaveActivity_Click" Text="" Height="45px"
                                                                                                                                        ValidationGroup="Activities" />

                                                                                                                                    <asp:Button ID="cmdUpdateActivity" runat="server" CssClass="Updatebutton1231"
                                                                                                                                        EnableTheming="false" Height="45px"
                                                                                                                                        OnClick="cmdUpdateActivity_Click" Text="" ValidationGroup="Activities"
                                                                                                                                        Width="45px" />

                                                                                                                                </td>
                                                                                                                                <td style="width: 20%;" align="left">
                                                                                                                                    <asp:Button ID="cmdCancelActivity" runat="server" CssClass="CloseWindow6" Height="45px" OnClick="cmdCancelActivity_Click" CausesValidation="false"
                                                                                                                                        EnableTheming="false" />
                                                                                                                                </td>
                                                                                                                                <td style="width: 30%;"></td>
                                                                                                                            </tr>
                                                                                                                        </table>
                                                                                                                    </td>
                                                                                                                </tr>
                                                                                                            </table>
                                                                                                        </div>
                                                                                                    </asp:Panel>
                                                                                                </td>
                                                                                            </tr>
                                                                                            <tr>
                                                                                                <td style="width: 750px" align="center">
                                                                                                    <asp:Button ID="BtnAddActivity" runat="server" OnClick="BtnAddActivity_Click" CssClass="ButtonAdd66" CausesValidation="False"
                                                                                                        Text="" EnableTheming="false"></asp:Button>
                                                                                                    <asp:HiddenField ID="HiddenField4" runat="server" Value="0" />
                                                                                                </td>
                                                                                            </tr>
                                                                                            <tr style="width: 750px">
                                                                                                <td style="width: 750px">
                                                                                                    <div class="mainGridHold" id="searchGrid3" style="width: 750px" align="center">
                                                                                                        <asp:GridView ID="GrdViewLeadActivity" runat="server" AllowSorting="True" AutoGenerateColumns="False" OnRowDeleting="GrdViewLeadActivity_RowDeleting" OnSelectedIndexChanged="GrdViewLeadActivity_SelectedIndexChanged"
                                                                                                            Width="100%" DataKeyNames="Activity_Id" AllowPaging="True" EmptyDataText="No Activities found." CssClass="someClass">
                                                                                                            <EmptyDataRowStyle CssClass="GrdContent" />
                                                                                                            <Columns>
                                                                                                                <asp:BoundField DataField="Activity_Name" HeaderText="Activity Name" HeaderStyle-BorderColor="Gray" />
                                                                                                                <asp:BoundField DataField="Activity_Name_Id" HeaderText="Activity Name Id" HeaderStyle-BorderColor="Gray" />
                                                                                                                <asp:BoundField DataField="Start_Date" HeaderText="Start Date" HeaderStyle-BorderColor="Gray" DataFormatString="{0:dd/MM/yyyy}" />
                                                                                                                <asp:BoundField DataField="End_Date" HeaderText="End Date" HeaderStyle-BorderColor="Gray" DataFormatString="{0:dd/MM/yyyy}" />
                                                                                                                <asp:BoundField DataField="Activity_Location" HeaderText="Activity Location" HeaderStyle-BorderColor="Gray" />
                                                                                                                <asp:BoundField DataField="Next_Activity" HeaderText="Next Activity" HeaderStyle-BorderColor="Gray" />
                                                                                                                <asp:BoundField DataField="Next_Activity_Id" HeaderText="Next Activity Id" HeaderStyle-BorderColor="Gray" />
                                                                                                                <asp:BoundField DataField="NextActivity_Date" HeaderText="Next Activity Date" HeaderStyle-BorderColor="Gray" DataFormatString="{0:dd/MM/yyyy}" />
                                                                                                                <asp:BoundField DataField="FollowUp" HeaderText="FollowUp" HeaderStyle-BorderColor="Gray" />
                                                                                                                <asp:BoundField DataField="Remarks" HeaderText="Remarks" HeaderStyle-BorderColor="Gray" />
                                                                                                                <asp:BoundField DataField="Emp_Name" HeaderText="Emp_Name" HeaderStyle-BorderColor="Gray" />
                                                                                                                <asp:BoundField DataField="Emp_No" HeaderText="Emp No" HeaderStyle-BorderColor="Gray" />
                                                                                                                <asp:TemplateField HeaderStyle-Width="30px" ItemStyle-CssClass="command" HeaderStyle-BorderColor="Gray">
                                                                                                                    <ItemTemplate>
                                                                                                                        <asp:ImageButton ID="btnEdit" runat="server" CommandName="Select" SkinID="edit" CausesValidation="false" />
                                                                                                                    </ItemTemplate>
                                                                                                                    <ItemStyle HorizontalAlign="Center" />
                                                                                                                </asp:TemplateField>
                                                                                                            </Columns>
                                                                                                            <PagerTemplate>
                                                                                                                <table style="border-color: white">
                                                                                                                    <tr style="border-color: white; height: 1px">
                                                                                                                    </tr>
                                                                                                                    <tr style="border-color: white">
                                                                                                                        <td style="border-color: white">Goto Page
                                                                                                                        </td>
                                                                                                                        <td style="border-color: white">
                                                                                                                            <asp:DropDownList ID="ddlPageSelector" runat="server" Style="border: 1px solid blue" AutoPostBack="true" SkinID="skinPagerDdlBox">
                                                                                                                            </asp:DropDownList>
                                                                                                                        </td>
                                                                                                                        <td style="border-color: white">
                                                                                                                            <asp:Button Text="" CommandName="Page" CommandArgument="First" runat="server" CssClass="NewFirst" EnableTheming="false" Width="22px" Height="18px"
                                                                                                                                ID="btnFirst" />
                                                                                                                        </td>
                                                                                                                        <td style="border-color: white">
                                                                                                                            <asp:Button Text="" CommandName="Page" CommandArgument="Prev" runat="server" CssClass="NewPrev" EnableTheming="false" Width="22px" Height="18px"
                                                                                                                                ID="btnPrevious" />
                                                                                                                        </td>
                                                                                                                        <td style="border-color: white">
                                                                                                                            <asp:Button Text="" CommandName="Page" CommandArgument="Next" runat="server" CssClass="NewNext" EnableTheming="false" Width="22px" Height="18px"
                                                                                                                                ID="btnNext" />
                                                                                                                        </td>
                                                                                                                        <td style="border-color: white">
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
                                                                                    </div>
                                                                                </ContentTemplate>
                                                                            </cc1:TabPanel>
                                                                        </cc1:TabContainer>
                                                                    </td>
                                                                </tr>
                                                                <tr>
                                                                    <td colspan="5">
                                                                        <table style="width: 100%" cellpadding="3" cellspacing="2">
                                                                            <tr>
                                                                                <td style="width: 32%"></td>
                                                                                <td style="width: 18%">

                                                                                    <asp:Button ID="UpdateButton" runat="server" SkinID="skinBtnSave" OnClientClick="javascript:CheckLeadContact();"
                                                                                        OnClick="UpdateButton_Click" CssClass="Updatebutton1231" EnableTheming="false"></asp:Button>
                                                                                    <asp:Button ID="AddButton" runat="server" SkinID="skinBtnSave" OnClientClick="javascript:CheckLeadContact()"
                                                                                        OnClick="AddButton_Click" CssClass="savebutton1231" EnableTheming="false"></asp:Button>
                                                                                </td>
                                                                                <td style="width: 18%">
                                                                                    <asp:Button ID="UpdateCancelButton" runat="server" CausesValidation="False" CommandName="Cancel"
                                                                                        OnClick="UpdateCancelButton_Click" SkinID="skinBtnCancel" CssClass="cancelbutton6"
                                                                                        EnableTheming="false"></asp:Button>
                                                                                </td>
                                                                                <td style="width: 32%" align="center"></td>
                                                                            </tr>
                                                                            <tr>
                                                                                <td>
                                                                                    <asp:ObjectDataSource ID="srcBanks" runat="server" SelectMethod="ListBanks" TypeName="BusinessLogic">
                                                                                        <SelectParameters>
                                                                                            <asp:CookieParameter Name="connection" CookieName="Company" Type="String" />
                                                                                        </SelectParameters>
                                                                                    </asp:ObjectDataSource>
                                                                                </td>
                                                                                <td>
                                                                                    <asp:ObjectDataSource ID="srcCreditorDebitor" runat="server" SelectMethod="ListSundryDebitors"
                                                                                        TypeName="BusinessLogic">
                                                                                        <SelectParameters>
                                                                                            <asp:CookieParameter Name="connection" CookieName="Company" Type="String" />
                                                                                        </SelectParameters>
                                                                                    </asp:ObjectDataSource>
                                                                                </td>
                                                                                <td></td>
                                                                                <td></td>
                                                                            </tr>
                                                                        </table>
                                                                    </td>
                                                                </tr>
                                                            </table>
                                                        </div>
                                                    </td>
                                                </tr>
                                            </table>
                                        </asp:Panel>
                                    </div>
                                </ContentTemplate>
                            </asp:UpdatePanel>
                        </asp:Panel>
                    </td>
                </tr>
                <tr style="width: 100%">
                    <td style="width: 100%">
                        <table width="100%" style="margin: -6px 0px 0px 0px;">
                            <tr style="width: 100%">
                                <td>
                                    <div class="mainGridHold" id="searchGrid">
                                        <asp:GridView ID="GrdViewLead" runat="server" AllowSorting="false" AutoGenerateColumns="False"
                                            OnRowCreated="GrdViewLead_RowCreated" Width="100.4%"
                                            AllowPaging="True" DataKeyNames="Lead_No" EmptyDataText="No Records found"
                                            OnRowCommand="GrdViewLead_RowCommand" OnRowDataBound="GrdViewLead_RowDataBound" OnPageIndexChanging="GrdViewLead_PageIndexChanging"
                                            OnSelectedIndexChanged="GrdViewLead_SelectedIndexChanged" OnRowDeleting="GrdViewLead_RowDeleting"
                                            OnRowDeleted="GrdViewLead_RowDeleted" CssClass="someClass">
                                            <EmptyDataRowStyle CssClass="GrdContent" />
                                            <Columns>
                                                <asp:BoundField DataField="Lead_No" HeaderText="Lead No" HeaderStyle-Wrap="false" HeaderStyle-BorderColor="Gray" />
                                                <asp:BoundField DataField="Lead_Name" HeaderText="Lead Name" HeaderStyle-Wrap="false" HeaderStyle-BorderColor="Gray" />
                                                <%--<asp:BoundField DataField="Start_Date" HeaderText="Start Date" HeaderStyle-Wrap="false"
                                        DataFormatString="{0:dd/MM/yyyy}" HeaderStyle-BorderColor="Gray" />--%>
                                                <asp:BoundField DataField="BP_Name" HeaderText="BP Name" HeaderStyle-Wrap="false" HeaderStyle-BorderColor="Gray" />
                                                <asp:BoundField DataField="Mobile" HeaderText="Mobile" HeaderStyle-Wrap="false" HeaderStyle-BorderColor="Gray" />
                                                <asp:BoundField DataField="Doc_Status" HeaderText="Doc Status" HeaderStyle-Wrap="false" HeaderStyle-BorderColor="Gray" />
                                                <asp:BoundField DataField="Lead_Status" HeaderText="Lead Status" HeaderStyle-Wrap="false" HeaderStyle-BorderColor="Gray" />
                                                <asp:BoundField DataField="Activity_Name" HeaderText="Activity Name" HeaderStyle-Wrap="false" HeaderStyle-BorderColor="Gray" />
                                                <asp:BoundField DataField="Next_Activity" HeaderText="Next Activity" HeaderStyle-Wrap="false" HeaderStyle-BorderColor="Gray" />
                                                <asp:TemplateField ItemStyle-CssClass="command" HeaderStyle-Width="50px" HeaderText="Edit" HeaderStyle-BorderColor="Gray">
                                                    <ItemTemplate>
                                                        <asp:ImageButton ID="btnEdit" CausesValidation="false" runat="server" SkinID="edit"
                                                            CommandName="Select" />
                                                        <asp:ImageButton ID="btnEditDisabled" Enabled="false" SkinID="editDisable" runat="Server"></asp:ImageButton>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <%--<asp:TemplateField ItemStyle-CssClass="command" HeaderStyle-Width="50px" HeaderText="Delete" HeaderStyle-BorderColor="Gray">
                                        <ItemTemplate>
                                            <asp:ImageButton ID="lnkB" SkinID="delete" runat="Server" OnClientClick="return confirm('Are you sure you want to delete?');" CommandName="Delete"></asp:ImageButton>
                                            <asp:ImageButton ID="lnkBDisabled" Enabled="false" SkinID="deleteDisable" runat="Server"></asp:ImageButton>
                                        </ItemTemplate>
                                    </asp:TemplateField>--%>
                                                <%--<asp:TemplateField ItemStyle-CssClass="command" HeaderStyle-Width="50px" HeaderText="Print">
                                        <ItemTemplate>
                                            <a href='<%# DataBinder.Eval(Container, "DataItem.LeadID", "javascript:PrintItem({0});") %>'>
                                                <img alt="Print" border="0" src="App_Themes/DefaultTheme/Images/Print.png">
                                            </a>
                                        </ItemTemplate>
                                    </asp:TemplateField>--%>
                                                <%--<asp:TemplateField Visible="false">
                                        <ItemTemplate>
                                        </ItemTemplate>
                                    </asp:TemplateField>--%>
                                            </Columns>
                                            <PagerTemplate>
                                                <table style="border-color: white">
                                                    <tr style="border-color: white; height: 1px">
                                                    </tr>
                                                    <tr style="border-color: white">
                                                        <td style="border-color: white">Goto Page
                                                        </td>
                                                        <td style="border-color: white">
                                                            <asp:DropDownList ID="ddlPageSelector" runat="server" AutoPostBack="true" Width="70px" Style="border: 1px solid Gray" Height="23px" BackColor="#e7e7e7" OnSelectedIndexChanged="ddlPageSelector_SelectedIndexChanged">
                                                            </asp:DropDownList>
                                                        </td>
                                                        <td style="border-color: white; width: 5px"></td>
                                                        <td style="border-color: white">
                                                            <asp:Button Text="" CommandName="Page" CommandArgument="First" runat="server" CausesValidation="false" CssClass="NewFirst" EnableTheming="false" Width="22px" Height="18px"
                                                                ID="btnFirst" />
                                                        </td>
                                                        <td style="border-color: white">
                                                            <asp:Button Text="" CommandName="Page" CommandArgument="Prev" runat="server" CausesValidation="false" CssClass="NewPrev" EnableTheming="false" Width="22px" Height="18px"
                                                                ID="btnPrevious" />
                                                        </td>
                                                        <td style="border-color: white">
                                                            <asp:Button Text="" CommandName="Page" CommandArgument="Next" runat="server" CausesValidation="false" CssClass="NewNext" EnableTheming="false" Width="22px" Height="18px"
                                                                ID="btnNext" />
                                                        </td>
                                                        <td style="border-color: white">
                                                            <asp:Button Text="" CommandName="Page" CommandArgument="Last" runat="server" CausesValidation="false" CssClass="NewLast" EnableTheming="false" Width="22px" Height="18px"
                                                                ID="btnLast" />
                                                        </td>
                                                    </tr>
                                                </table>
                                            </PagerTemplate>
                                        </asp:GridView>
                                    </div>
                                </td>
                            </tr>
                            <tr>
                                <td></td>
                            </tr>
                        </table>
                    </td>
                </tr>
                <%--<tr style="width:100%">
                    <td align="left">--%>
                <%--<asp:ObjectDataSource ID="GridSource" runat="server" SelectMethod="ListLeadMaster"
                            TypeName="LeadBusinessLogic">--%>
                <%--<SelectParameters>
                                <%--<asp:ControlParameter ControlID="GrdViewLead" Name="LeadId" PropertyName="SelectedValue"
                                    Type="Int32" />--%>
                <%--<asp:CookieParameter CookieName="Company" Type="String" Name="connection" />
                            </SelectParameters>--%>
                <%--   <SelectParameters>
                                <asp:CookieParameter Name="LeadID" CookieName="LeadID" Type="Int32" />
                            </SelectParameters>--%>
                <%--</asp:ObjectDataSource>
                    </td>--%>
                <%--</tr>--%>
                <tr>
                    <td style="width: 100%"></td>
                </tr>
            </table>
            <input type="hidden" id="hidAdvancedState" runat="server" />
            <asp:HiddenField ID="hdSMS" runat="server" Value="NO" />
            <asp:HiddenField ID="hdText" runat="server" />
            <asp:HiddenField ID="hdMobile" runat="server" />
            <asp:HiddenField ID="hdSMSRequired" runat="server" Value="NO" />
            <asp:HiddenField ID="hdPendingCount" runat="server" Value="0" />
        </ContentTemplate>
    </asp:UpdatePanel>
    <div align="center">
        <asp:Button ID="btnExportToExcel" runat="server" CssClass="exportexl6" Height="45px" OnClientClick="window.open('ReportExcelLeadManagement.aspx','Summary', 'toolbar=no,status=no,menu=no,location=no,height=280,width=500,left=405,top=220 ,resizable=yes, scrollbars=yes');" CausesValidation="false"
            EnableTheming="false" />
    </div>
</asp:Content>
