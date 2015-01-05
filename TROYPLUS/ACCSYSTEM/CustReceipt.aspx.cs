﻿using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Text;
using SMSLibrary;

public partial class CustReceipt : System.Web.UI.Page
{
    Double sumAmt = 0.0;
    public string sDataSource = string.Empty;
    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            sDataSource = ConfigurationManager.ConnectionStrings[Request.Cookies["Company"].Value].ToString();
            if (!Page.IsPostBack)
            {
                string connStr = string.Empty;

                if (Request.Cookies["Company"] != null)
                    connStr = System.Configuration.ConfigurationManager.ConnectionStrings[Request.Cookies["Company"].Value].ToString();
                else
                    Response.Redirect("~/Login.aspx");

                CheckSMSRequired();

                loadBanks();

                ddReceivedFrom.DataBind();

                GrdViewReceipt.PageSize = 8;

                string dbfileName = connStr.Remove(0, connStr.LastIndexOf(@"App_Data\") + 9);
                dbfileName = dbfileName.Remove(dbfileName.LastIndexOf(";Persist Security Info"));
                BusinessLogic objChk = new BusinessLogic();

                if (objChk.CheckForOffline(Server.MapPath("Offline\\" + dbfileName + ".offline")))
                {
                    lnkBtnAdd.Visible = false;
                    GrdViewReceipt.Columns[8].Visible = false;
                    GrdViewReceipt.Columns[7].Visible = false;
                }

                if (Session["SMSREQUIRED"] != null)
                {
                    if (Session["SMSREQUIRED"].ToString() == "NO")
                        hdSMSRequired.Value = "NO";
                    else
                        hdSMSRequired.Value = "YES";
                }
                else
                {
                    hdSMSRequired.Value = "NO";
                }


                string connection = Request.Cookies["Company"].Value;
                string usernam = Request.Cookies["LoggedUserName"].Value;
                BusinessLogic bl = new BusinessLogic(sDataSource);

                if (bl.CheckUserHaveAdd(usernam, "CUSTRCT"))
                {
                    lnkBtnAdd.Enabled = false;
                    lnkBtnAdd.ToolTip = "You are not allowed to make Add New ";
                }
                else
                {
                    lnkBtnAdd.Enabled = true;
                    lnkBtnAdd.ToolTip = "Click to Add New ";
                }


                pnlEdit.Visible = false;
                //myRangeValidator.MinimumValue = System.DateTime.Now.AddYears(-100).ToShortDateString();
                //myRangeValidator.MaximumValue = System.DateTime.Now.ToShortDateString();

            }
        }
        catch (Exception ex)
        {
            TroyLiteExceptionManager.HandleException(ex);
        }
    }

    bool IsFutureDate(DateTime refDate)
    {
        DateTime today = DateTime.Today;
        return (refDate.Date != today) && (refDate > today);
    }

    protected void txtDate_TextChanged(object sender, EventArgs e)
    {

    }

    protected void BtnClearFilter_Click(object sender, EventArgs e)
    {
        txtSearch.Text = "";
       
        ddCriteria.SelectedIndex = 0;
    }

    protected void chkcash_CheckedChanged(object sender, EventArgs e)
    {
        DataSet ds = new DataSet();

        DataTable dt = new DataTable();
        DataColumn dc;
        DataRow drNew;

        if (chkcash.Checked == false)
        {
            if (Session["Ds"] == null)
            {
            }
            else
            {
                DataSet dst = (DataSet)Session["Ds"];
                if (dst != null)
                {
                    if (dst.Tables.Count > 0)
                    {
                        for (int i = 0; i < dst.Tables[0].Rows.Count; i++)
                        {
                            if (dst.Tables[0].Rows[i]["Type"].ToString() == "Cash")
                            {
                                dst.Tables[0].Rows[i].Delete();
                            }
                        }
                        GrdViewItems.DataSource = dst;
                        GrdViewItems.DataBind();
                    }
                }
            }
        }
        else
        {
            if (Session["Ds"] == null)
            {
               
                dc = new DataColumn("Type");
                dt.Columns.Add(dc);

                dc = new DataColumn("RefNo");
                dt.Columns.Add(dc);

                dc = new DataColumn("Amount");
                dt.Columns.Add(dc);

                dc = new DataColumn("ChequeNo");
                dt.Columns.Add(dc);

                dc = new DataColumn("Narration");
                dt.Columns.Add(dc);

                ds.Tables.Add(dt);

                drNew = dt.NewRow();


                drNew["Type"] = "Cash";
                drNew["RefNo"] = "";
                drNew["Amount"] = "";
                drNew["ChequeNo"] = "";
                drNew["Narration"] = "";
                
                ds.Tables[0].Rows.Add(drNew);
                Session["Ds"] = ds;

            }
            else
            {
                ds = (DataSet)Session["Ds"];
                drNew = ds.Tables[0].NewRow();

                drNew["Type"] = "Cash";
                drNew["RefNo"] = "";
                drNew["Amount"] = "";
                drNew["ChequeNo"] = "";
                drNew["Narration"] = "";

                ds.Tables[0].Rows.Add(drNew);

            }

            //cmdSaveProduct.Visible = true;
            //cmdUpdateProduct.Visible = false;
            //cmdCancelProduct.Visible = false;
            GrdViewItems.DataSource = ds;
            GrdViewItems.DataBind();

            for (int vLoop = 0; vLoop < GrdViewItems.Rows.Count; vLoop++)
            {
                TextBox txtttd = (TextBox)GrdViewItems.Rows[vLoop].FindControl("txtType");
                if(txtttd.Text == "Cash")
                {

                }
            }

        }
        //UpdatePanel21.Update();
    }

    protected void chkcheque_CheckedChanged(object sender, EventArgs e)
    {
        DataSet ds = new DataSet();

        DataTable dt = new DataTable();
        DataColumn dc;
        DataRow drNew;

        if (chkcheque.Checked == false)
        {
            if (Session["Ds"] == null)
            {

            }
            else
            {
                DataSet dst = (DataSet)Session["Ds"];
                if(dst != null)
                {
                    if (dst.Tables.Count > 0)
                    {
                        for (int i = 0; i < dst.Tables[0].Rows.Count; i++)
                        {
                            if (dst.Tables[0].Rows[i]["Type"].ToString() == "Cheque")
                            {
                                dst.Tables[0].Rows[i].Delete();
                            }
                        }
                        GrdViewItems.DataSource = dst;
                        GrdViewItems.DataBind();
                    }
                }
               
            }
        }
        else
        {
            if (Session["Ds"] == null)
            {

                dc = new DataColumn("Type");
                dt.Columns.Add(dc);

                dc = new DataColumn("RefNo");
                dt.Columns.Add(dc);

                dc = new DataColumn("Amount");
                dt.Columns.Add(dc);

                dc = new DataColumn("ChequeNo");
                dt.Columns.Add(dc);

                dc = new DataColumn("Narration");
                dt.Columns.Add(dc);

                ds.Tables.Add(dt);

                drNew = dt.NewRow();


                drNew["Type"] = "Cheque";
                drNew["RefNo"] = "";
                drNew["Amount"] = "";
                drNew["ChequeNo"] = "";
                drNew["Narration"] = "";

                ds.Tables[0].Rows.Add(drNew);
                Session["Ds"] = ds;

            }
            else
            {
                ds = (DataSet)Session["Ds"];
                drNew = ds.Tables[0].NewRow();

                drNew["Type"] = "Cheque";
                drNew["RefNo"] = "";
                drNew["Amount"] = "";
                drNew["ChequeNo"] = "";
                drNew["Narration"] = "";

                ds.Tables[0].Rows.Add(drNew);

            }

            //cmdSaveProduct.Visible = true;
            //cmdUpdateProduct.Visible = false;
            //cmdCancelProduct.Visible = false;
            GrdViewItems.DataSource = ds;
            GrdViewItems.DataBind();
        }
        //UpdatePanel21.Update();
    }

    protected void chkcard_CheckedChanged(object sender, EventArgs e)
    {
        DataSet ds = new DataSet();

        DataTable dt = new DataTable();
        DataColumn dc;
        DataRow drNew;

        if (chkcard.Checked == false)
        {
            if (Session["Ds"] == null)
            {
            }
            else
            {
                DataSet dst = (DataSet)Session["Ds"];
                if (dst != null)
                {
                    if (dst.Tables.Count > 0)
                    {
                        for (int i = 0; i < dst.Tables[0].Rows.Count; i++)
                        {
                            if (dst.Tables[0].Rows[i]["Type"].ToString() == "Card")
                            {
                                dst.Tables[0].Rows[i].Delete();
                            }
                        }
                        GrdViewItems.DataSource = dst;
                        GrdViewItems.DataBind();
                    }
                }
            }
        }
        else
        {
            if (Session["Ds"] == null)
            {

                dc = new DataColumn("Type");
                dt.Columns.Add(dc);

                dc = new DataColumn("RefNo");
                dt.Columns.Add(dc);

                dc = new DataColumn("Amount");
                dt.Columns.Add(dc);

                dc = new DataColumn("ChequeNo");
                dt.Columns.Add(dc);

                dc = new DataColumn("Narration");
                dt.Columns.Add(dc);

                ds.Tables.Add(dt);

                drNew = dt.NewRow();


                drNew["Type"] = "Card";
                drNew["RefNo"] = "";
                drNew["Amount"] = "";
                drNew["ChequeNo"] = "";
                drNew["Narration"] = "";

                ds.Tables[0].Rows.Add(drNew);
                Session["Ds"] = ds;

            }
            else
            {
                ds = (DataSet)Session["Ds"];
                drNew = ds.Tables[0].NewRow();

                drNew["Type"] = "Card";
                drNew["RefNo"] = "";
                drNew["Amount"] = "";
                drNew["ChequeNo"] = "";
                drNew["Narration"] = "";

                ds.Tables[0].Rows.Add(drNew);

            }

            //cmdSaveProduct.Visible = true;
            //cmdUpdateProduct.Visible = false;
            //cmdCancelProduct.Visible = false;
            GrdViewItems.DataSource = ds;
            GrdViewItems.DataBind();
        }
        //UpdatePanel21.Update();
    }

    protected void txtTransDate_TextChanged(object sender, EventArgs e)
    {
        try
        {
            string refDate = string.Empty;
            refDate = txtTransDate.Text;
            ViewState.Add("TransDate", refDate);

            if (IsFutureDate(Convert.ToDateTime(refDate)))
            {
                hddatecheck.Value = "1";
                UP1.Update();
                return;
            }
            else
            {
                hddatecheck.Value = "0";
                UP1.Update();
            }
        }
        catch (Exception ex)
        {
            TroyLiteExceptionManager.HandleException(ex);
        }
    }

    private void ShowPendingBills()
    {
        string connStr = string.Empty;

        if (Request.Cookies["Company"] != null)
            connStr = System.Configuration.ConfigurationManager.ConnectionStrings[Request.Cookies["Company"].Value].ToString();
        else
            Response.Redirect("~/Login.aspx");

        BusinessLogic bl = new BusinessLogic();
        var customerID = ddReceivedFrom.SelectedValue.Trim();

        var dsSales = bl.ListCreditSales(connStr.Trim(), customerID);

        var receivedData = bl.GetCustReceivedAmount(connStr);

        if (dsSales != null)
        {

            foreach (DataRow dr in receivedData.Tables[0].Rows)
            {
                var billNo = dr["BillNo"].ToString();
                var billAmount = dr["TotalAmount"].ToString();

                for (int i = 0; i < dsSales.Tables[0].Rows.Count; i++)
                {
                    if (billNo.Trim() == dsSales.Tables[0].Rows[i]["BillNo"].ToString())
                    {
                        dsSales.Tables[0].Rows[i].BeginEdit();
                        double val = (double.Parse(dsSales.Tables[0].Rows[i]["Amount"].ToString()) - double.Parse(billAmount));
                        dsSales.Tables[0].Rows[i]["Amount"] = val;
                        dsSales.Tables[0].Rows[i].EndEdit();

                        if (val == 0.0)
                            dsSales.Tables[0].Rows[i].Delete();
                    }
                }
                dsSales.Tables[0].AcceptChanges();
            }

        }
        GrdViewSales.DataSource = dsSales;
        GrdViewSales.DataBind();

        //if (dsSales != null)
        //{
        //    hdPendingCount.Value = dsSales.Tables[0].Rows.Count.ToString();
        //}

    }

    protected void btnrec_Click(object sender, EventArgs e)
    {
        try
        {
            string CustRec = "CustRec";
            Response.Write("<script language='javascript'> window.open('ReportExcelReceipts.aspx?ID=" + CustRec + "' , 'window','toolbar=no,status=no,menu=no,location=no,height=320,width=700,left=320,top=220 ,resizable=yes, scrollbars=yes');</script>");
        }
        catch (Exception ex)
        {
            TroyLiteExceptionManager.HandleException(ex);
        }
    }

    private void loadBanks()
    {
        BusinessLogic bl = new BusinessLogic(sDataSource);
        DataSet ds = new DataSet();
        //ds = bl.ListBanks();
        ds = bl.ListBankLedgerpaymnet();
        ddBanks.DataSource = ds;
        ddBanks.DataTextField = "LedgerName";
        ddBanks.DataValueField = "LedgerID";
        ddBanks.DataBind();
    }

    private void checkPendingBills(DataSet ds)
    {
        foreach (GridViewRow tt in GrdViewSales.Rows)
        {
            if (tt.RowType == DataControlRowType.DataRow)
            {
                string billNo = tt.Cells[0].Text;

                bool exists = false;

                if (ds != null)
                {
                    foreach (DataRow d in ds.Tables[0].Rows)
                    {
                        string bNo = d[1].ToString();

                        if (bNo == billNo)
                        {
                            exists = true;
                        }

                    }
                }

                if (!exists)
                {
                    hdPendingCount.Value = "1";
                    UpdatePanelPage.Update();
                    return;
                }

            }
        }

        hdPendingCount.Value = "0";
        UpdatePanelPage.Update();
    }

    private void CheckSMSRequired()
    {
        DataSet appSettings;
        string smsRequired = string.Empty;
        string emailRequired = string.Empty;

        if (Session["AppSettings"] != null)
        {
            appSettings = (DataSet)Session["AppSettings"];

            for (int i = 0; i < appSettings.Tables[0].Rows.Count; i++)
            {
                if (appSettings.Tables[0].Rows[i]["KEY"].ToString() == "SMSREQ")
                {
                    smsRequired = appSettings.Tables[0].Rows[i]["KEYVALUE"].ToString();
                    Session["SMSREQUIRED"] = smsRequired.Trim().ToUpper();
                }
                if (appSettings.Tables[0].Rows[i]["KEY"].ToString() == "EMAILREQ")
                {
                    emailRequired = appSettings.Tables[0].Rows[i]["KEYVALUE"].ToString();
                    Session["EMAILREQUIRED"] = emailRequired.Trim().ToUpper();
                }
            }
        }

    }

    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);
        //TextBox search = (TextBox)Accordion1.FindControl("txtSearch");
        GridSource.SelectParameters.Add(new CookieParameter("connection", "Company"));
        //DropDownList dropDown = (DropDownList)Accordion1.FindControl("ddCriteria");
        GridSource.SelectParameters.Add(new ControlParameter("txtSearch", TypeCode.String, txtSearch.UniqueID, "Text"));
        GridSource.SelectParameters.Add(new ControlParameter("dropDown", TypeCode.String, ddCriteria.UniqueID, "SelectedValue"));
    }

    protected void GrdViewReceipt_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        if (e.CommandName == "Select")
        {
            /*//MyAccordion.Visible = false;
            frmViewAdd.Visible = true;
            frmViewAdd.DataBind();
            frmViewAdd.ChangeMode(FormViewMode.Edit);
            //GrdViewReceipt.Columns[7].Visible = false;
            lnkBtnAdd.Visible = false;
            GrdViewReceipt.Visible = false;
            //if (frmViewAdd.CurrentMode == FormViewMode.Edit)
                //Accordion1.SelectedIndex = 1;*/
        }
    }

    protected void GrdViewReceipt_SelectedIndexChanged(object sender, EventArgs e)
    {
        try
        {
            GridViewRow Row = GrdViewReceipt.SelectedRow;
            string connection = Request.Cookies["Company"].Value;
            BusinessLogic bl = new BusinessLogic();
            string recondate = Row.Cells[2].Text;
            Session["BillData"] = null;
            //hd.Value = Convert.ToString(GrdViewReceipt.SelectedDataKey.Value);

            if (!bl.IsValidDate(connection, Convert.ToDateTime(recondate)))
            {
                ScriptManager.RegisterStartupScript(Page, Page.GetType(), Guid.NewGuid().ToString(), "alert('Date is invalid')", true);
                return;
            }
            else
            {
                //pnlEdit.Visible = true;
                DataSet ds = bl.GetReceiptForId(connection, int.Parse(GrdViewReceipt.SelectedDataKey.Value.ToString()));
                if (ds != null)
                {
                    txtRefNo.Text = ds.Tables[0].Rows[0]["RefNo"].ToString();
                    txtTransDate.Text = DateTime.Parse(ds.Tables[0].Rows[0]["TransDate"].ToString()).ToShortDateString();

                    ddReceivedFrom.SelectedValue = ds.Tables[0].Rows[0]["CreditorID"].ToString();
                    txtAmount.Text = ds.Tables[0].Rows[0]["Amount"].ToString();
                    txtMobile.Text = ds.Tables[0].Rows[0]["Mobile"].ToString();
                    chkPayTo.SelectedValue = ds.Tables[0].Rows[0]["paymode"].ToString();
                    txtNarration.Text = ds.Tables[0].Rows[0]["Narration"].ToString();
                    if (chkPayTo.SelectedItem != null)
                    {
                        if (chkPayTo.SelectedItem.Text == "Cheque")
                        {
                            tblBank.Attributes.Add("class", "AdvancedSearch");
                        }
                        else
                        {
                            tblBank.Attributes.Add("class", "hidden");
                        }
                    }
                    else
                    {
                        tblBank.Attributes.Add("class", "hidden");
                    }

                    txtChequeNo.Text = ds.Tables[0].Rows[0]["ChequeNo"].ToString();

                    string creditorID = ds.Tables[0].Rows[0]["DebtorID"].ToString();

                    ddBanks.ClearSelection();

                    ListItem li = ddBanks.Items.FindByValue(creditorID);
                    if (li != null) li.Selected = true;

                    DataSet billsData = bl.GetReceivedAmountId(connection, int.Parse(GrdViewReceipt.SelectedDataKey.Value.ToString()));

                    Session["BillData"] = billsData;

                    if (billsData.Tables[0].Rows[0]["BillNo"].ToString() == "0")
                    {
                        billsData = null;
                    }
                    GrdBills.DataSource = billsData;
                    GrdBills.DataBind();
                    Session["RMode"] = "Edit";
                    ShowPendingBills();
                    checkPendingBills(billsData);
                }

                //GrdViewReceipt.Visible = false;
                ////MyAccordion.Visible = false;
                //lnkBtnAdd.Visible = false;
                pnlEdit.Visible = true;
                UpdateButton.Visible = true;
                SaveButton.Visible = false;
                ModalPopupExtender2.Show();

            }
        }
        catch (Exception ex)
        {
            TroyLiteExceptionManager.HandleException(ex);
        }
    }

    protected void GrdBillsCancelEdit(object sender, GridViewCancelEditEventArgs e)
    {
        GrdBills.EditIndex = -1;
        if (Session["BillData"] != null)
        {
            GrdBills.DataSource = (DataSet)Session["BillData"];
            GrdBills.DataBind();
            checkPendingBills((DataSet)Session["BillData"]);
        }
    }

    protected void lnkAddBills_Click(object sender, EventArgs e)
    {
        try
        {
            pnlEdit.Visible = false;
            BusinessLogic bl = new BusinessLogic();
            string conn = GetConnectionString();
            ModalPopupExtender2.Show();
            pnlEdit.Visible = true;
            if (txtAmount.Text == "")
            {

                //ScriptManager.RegisterStartupScript(Page, Page.GetType(), Guid.NewGuid().ToString(), "alert('Please enter the Receipt Amount before Adding BillNo')", true);
                //CnrfmDel.ConfirmText = "Please enter the Receipt Amount before Adding BillNo";
                //CnrfmDel.TargetControlID = "lnkAddBills";
                txtAmount.Focus();
                return;
            }

            if (ddReceivedFrom.SelectedValue == "0")
            {
                //ScriptManager.RegisterStartupScript(Page, Page.GetType(), Guid.NewGuid().ToString(), "alert('Please Select the Customer before Adding Bills')", true);
                //pnlEdit.Visible = true;
                txtAmount.Focus();
                return;
            }

            if (GrdBills.Rows.Count == 0)
            {
                var ds = bl.GetReceivedAmountId(conn, -1);
                GrdBills.DataSource = ds;
                GrdBills.DataBind();
                GrdBills.Rows[0].Visible = false;
                checkPendingBills(ds);
            }
            pnlEdit.Visible = true;
            GrdBills.FooterRow.Visible = true;
            lnkAddBills.Visible = true;
            Session["RMode"] = "Add";
            //lnkBtnAdd.Visible = false;
        }
        catch (Exception ex)
        {
            TroyLiteExceptionManager.HandleException(ex);
        }
    }

    protected void UpdateCancelButton_Click(object sender, EventArgs e)
    {
        try
        {
            //MyAccordion.Visible = true;
            pnlEdit.Visible = false;
            ModalPopupExtender2.Hide();
            //lnkBtnAdd.Visible = true;
            //lnkAddBills.Visible = true;
            GrdViewReceipt.Visible = true;
            GrdViewReceipt.Columns[8].Visible = true;
            ClearPanel();

        }
        catch (Exception ex)
        {
            TroyLiteExceptionManager.HandleException(ex);
        }
    }


    protected void GrdViewReceipt_RowCreated(object sender, GridViewRowEventArgs e)
    {
        try
        {
            if (e.Row.RowType == DataControlRowType.Pager)
            {
                PresentationUtils.SetPagerButtonStates(GrdViewReceipt, e.Row, this);
            }
        }
        catch (Exception ex)
        {
            TroyLiteExceptionManager.HandleException(ex);
        }
    }

    protected void ddlPageSelector_SelectedIndexChanged(object sender, EventArgs e)
    {
        try
        {
            GrdViewSales.PageIndex = ((DropDownList)sender).SelectedIndex;

            ModalPopupExtender1.Show();
            ShowPendingBills();
        }
        catch (Exception ex)
        {
            TroyLiteExceptionManager.HandleException(ex);
        }
    }

    protected void GrdViewSales_RowCreated(object sender, GridViewRowEventArgs e)
    {
        try
        {
            if (e.Row.RowType == DataControlRowType.Pager)
            {
                PresentationUtils.SetPagerButtonStates(GrdViewSales, e.Row, this);
            }
        }
        catch (Exception ex)
        {
            TroyLiteExceptionManager.HandleException(ex);
        }
    }

    protected void GrdViewSales_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            
        }
    }

    protected void GrdViewSales_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        try
        {
            GrdViewSales.PageIndex = e.NewPageIndex;

            ShowPendingBills();
            ModalPopupExtender1.Show();
        }
        catch (Exception ex)
        {
            TroyLiteExceptionManager.HandleException(ex);
        }
    }

    protected void GrdViewReceipt_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        try
        {
            GridView gridView = (GridView)sender;

            if (e.Row.RowType == DataControlRowType.Header)
            {
                int cellIndex = -1;

                foreach (DataControlField field in gridView.Columns)
                {
                    if (field.SortExpression == gridView.SortExpression)
                    {
                        cellIndex = gridView.Columns.IndexOf(field);
                    }
                    else if (field.SortExpression != "")
                    {
                        e.Row.Cells[gridView.Columns.IndexOf(field)].CssClass = "headerstyle";
                    }

                }

                if (cellIndex > -1)
                {
                    //  this is a header row,
                    //  set the sort style
                    e.Row.Cells[cellIndex].CssClass =
                        gridView.SortDirection == SortDirection.Ascending
                        ? "sortascheaderstyle" : "sortdescheaderstyle";
                }




            }

            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                BusinessLogic bl = new BusinessLogic(sDataSource);
                string connection = Request.Cookies["Company"].Value;
                string usernam = Request.Cookies["LoggedUserName"].Value;

                if (bl.CheckUserHaveEdit(usernam, "CUSTRCT"))
                {
                    ((ImageButton)e.Row.FindControl("btnEdit")).Visible = false;
                    ((ImageButton)e.Row.FindControl("btnEditDisabled")).Visible = true;
                }

                if (bl.CheckUserHaveDelete(usernam, "CUSTRCT"))
                {
                    ((ImageButton)e.Row.FindControl("lnkB")).Visible = false;
                    ((ImageButton)e.Row.FindControl("lnkBDisabled")).Visible = true;
                }

                if (bl.CheckUserHaveView(usernam, "CUSTRCT"))
                {
                    ((Image)e.Row.FindControl("lnkprint")).Visible = false;
                    ((ImageButton)e.Row.FindControl("btnViewDisabled")).Visible = true;
                }
                else
                {
                    ((ImageButton)e.Row.FindControl("btnViewDisabled")).Visible = false;
                }
            }
        }
        catch (Exception ex)
        {
            TroyLiteExceptionManager.HandleException(ex);
        }
    }

    protected void lnkBtnAdd_Click(object sender, EventArgs e)
    {
        try
        {
            //if (!Helper.IsLicenced(Request.Cookies["Company"].Value))
            //{
            //    ScriptManager.RegisterStartupScript(Page, Page.GetType(), Guid.NewGuid().ToString(), "alert('This is Trial Version, Please upgrade to Full Version of this Software. Thank You.');", true);
            //    return;
            //}
            ModalPopupExtender2.Show();
            ModalPopupExtender3.Show();
            pnlEdit.Visible = true;
            Session["Ds"] = null;
            //lnkBtnAdd.Visible = false;
            ////MyAccordion.Visible = false;
            //GrdViewReceipt.Visible = false;
            UpdateButton.Visible = false;
            SaveButton.Visible = true;
            ClearPanel();
            ShowPendingBills();

            drpLedger.SelectedIndex = 0;

            //txtTransDate.Text = DateTime.Now.ToShortDateString();

            DateTime indianStd = TimeZoneInfo.ConvertTimeBySystemTimeZoneId(DateTime.Now, "India Standard Time");
            string dtaa = Convert.ToDateTime(indianStd).ToString("dd/MM/yyyy");
            txtTransDate.Text = dtaa;
            txtDate.Text = dtaa;

            drpLedger.Focus();
            chkPayTo.SelectedValue = "Cash";

            if (chkPayTo.SelectedItem != null)
            {
                if (chkPayTo.SelectedItem.Text == "Cheque")
                {
                    tblBank.Attributes.Add("class", "AdvancedSearch");
                }
                else
                {
                    tblBank.Attributes.Add("class", "hidden");
                }
            }
            else
            {
                if (tblBank != null)
                    tblBank.Attributes.Add("class", "hidden");
            }

            //Button clickedbutton = (Button)sender;
            //clickedbutton.Enabled = true;
        }
        catch (Exception ex)
        {
            TroyLiteExceptionManager.HandleException(ex);
        }
    }

    protected void GrdViewItems_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        try
        {
            BusinessLogic bl = new BusinessLogic(sDataSource);
            DataSet ds = new DataSet();

            ds = bl.ListBanks();

            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                var ddl = (DropDownList)e.Row.FindControl("drpBank");
                ddl.Items.Clear();
                ListItem lifzzh = new ListItem("Select Ledger", "0");
                lifzzh.Attributes.Add("style", "color:Black");
                ddl.Items.Add(lifzzh);
                ddl.DataSource = ds;
                ddl.Items[0].Attributes.Add("background-color", "color:#bce1fe");
                ddl.DataBind();
                ddl.DataTextField = "LedgerName";
                ddl.DataValueField = "LedgerID";

                var ddll = (TextBox)e.Row.FindControl("txtType");
                if(ddll.Text == "Cash")
                {
                    var dd = (TextBox)e.Row.FindControl("txtChequeNo");
                    dd.Enabled = false;

                    var ddlll = (DropDownList)e.Row.FindControl("drpBank");
                    ddlll.Enabled = false;
                }
            }
        }
        catch (Exception ex)
        {
            TroyLiteExceptionManager.HandleException(ex);
        }
    }

    protected void UpdCancelButton_Click(object sender, EventArgs e)
    {
        ModalPopupExtender1.Hide();
        ModalPopupExtender2.Hide();
    }

    protected void UpdButton_Click(object sender, EventArgs e)
    {
        if ((chkcard.Checked == false) && (chkcheque.Checked == false) && ( chkcash.Checked == false))
        {
            ScriptManager.RegisterStartupScript(Page, Page.GetType(), Guid.NewGuid().ToString(), "alert('Please select any one')", true);
            return;
        }

        string connection = string.Empty;
        connection = Request.Cookies["Company"].Value;

        string[] sDate;
        DateTime sBilldate;

        string delim = "/";
        char[] delimA = delim.ToCharArray();
        //CultureInfo culture = new CultureInfo("pt-BR");
        string sPath = string.Empty;
        BusinessLogic bl = new BusinessLogic(sDataSource);

        if (!bl.IsValidDate(connection, Convert.ToDateTime(txtDate.Text)))
        {
            ScriptManager.RegisterStartupScript(Page, Page.GetType(), Guid.NewGuid().ToString(), "alert('Date is invalid')", true);
            return;
        }

        if (Request.Cookies["Company"] != null)
            sDataSource = Request.Cookies["Company"].Value;

        sPath = sDataSource;
        string usernam = Request.Cookies["LoggedUserName"].Value;

        

        for (int vLoop = 0; vLoop < GrdViewItems.Rows.Count; vLoop++)
        {
            TextBox txtttd = (TextBox)GrdViewItems.Rows[vLoop].FindControl("txtType");
            TextBox txttt = (TextBox)GrdViewItems.Rows[vLoop].FindControl("txtRefNo");
            TextBox txt = (TextBox)GrdViewItems.Rows[vLoop].FindControl("txtAmount");
            TextBox txtt = (TextBox)GrdViewItems.Rows[vLoop].FindControl("txtNarration");
            DropDownList txttd = (DropDownList)GrdViewItems.Rows[vLoop].FindControl("drpBank");
            TextBox txttdd = (TextBox)GrdViewItems.Rows[vLoop].FindControl("txtChequeNo");

            int col = vLoop + 1;

            if (txttt.Text == "")
            {
                ScriptManager.RegisterStartupScript(Page, Page.GetType(), Guid.NewGuid().ToString(), "alert('Please fill RefNo in row " + col + " ')", true);
                return;
            }
            else if (txt.Text == "")
            {
                ScriptManager.RegisterStartupScript(Page, Page.GetType(), Guid.NewGuid().ToString(), "alert('Please fill Amount in row " + col + " ')", true);
                return;
            }
            else if (txtt.Text == "")
            {
                ScriptManager.RegisterStartupScript(Page, Page.GetType(), Guid.NewGuid().ToString(), "alert('Please fill Narration in row " + col + " ')", true);
                return;
            }
            else if (txttd.SelectedValue == "0")
            {
                if (txtttd.Text != "Cash")
                {
                    ScriptManager.RegisterStartupScript(Page, Page.GetType(), Guid.NewGuid().ToString(), "alert('Please select Bank in row " + col + " ')", true);
                    return;
                }
            }
            else if (txttdd.Text == "")
            {
                if (txtttd.Text != "Cash")
                {
                    ScriptManager.RegisterStartupScript(Page, Page.GetType(), Guid.NewGuid().ToString(), "alert('Please fill ChequeNo in row " + col + " ')", true);
                    return;
                }
            }                       
        }



        DataSet ds;
        DataTable dt;
        DataRow drNew;

        DataColumn dc;

        ds = new DataSet();

        dt = new DataTable();

        dc = new DataColumn("RefNo");
        dt.Columns.Add(dc);

        dc = new DataColumn("Date");
        dt.Columns.Add(dc);

        dc = new DataColumn("DebitorID");
        dt.Columns.Add(dc);

        dc = new DataColumn("Paymode");
        dt.Columns.Add(dc);

        dc = new DataColumn("Amount");
        dt.Columns.Add(dc);

        dc = new DataColumn("Narration");
        dt.Columns.Add(dc);

        dc = new DataColumn("VoucherType");
        dt.Columns.Add(dc);

        dc = new DataColumn("ChequeNo");
        dt.Columns.Add(dc);

        ds.Tables.Add(dt);

        for (int vLoop = 0; vLoop < GrdViewItems.Rows.Count; vLoop++)
        {
            TextBox txttt = (TextBox)GrdViewItems.Rows[vLoop].FindControl("txtRefNo");
            TextBox txtttd = (TextBox)GrdViewItems.Rows[vLoop].FindControl("txtType");
            TextBox txt = (TextBox)GrdViewItems.Rows[vLoop].FindControl("txtAmount");
            TextBox txtt = (TextBox)GrdViewItems.Rows[vLoop].FindControl("txtNarration");
            DropDownList txttd = (DropDownList)GrdViewItems.Rows[vLoop].FindControl("drpBank");
            TextBox txttdd = (TextBox)GrdViewItems.Rows[vLoop].FindControl("txtChequeNo");

            sDate = txtDate.Text.Trim().Split(delimA);
            sBilldate = new DateTime(Convert.ToInt32(sDate[2].ToString()), Convert.ToInt32(sDate[1].ToString()), Convert.ToInt32(sDate[0].ToString()));

            drNew = dt.NewRow();
            drNew["RefNo"] = txttt.Text;
            drNew["Date"] = sBilldate;
            drNew["ChequeNo"] = txttdd.Text;

            if (txtttd.Text == "Cash")
            {
                drNew["DebitorID"] = 1;
                drNew["Paymode"] = "Cash";
            }
            else if (txtttd.Text == "Cheque")
            {
                drNew["DebitorID"] = int.Parse(txttd.SelectedValue);
                drNew["Paymode"] = "Cheque";
            }
            else if (txtttd.Text == "Card")
            {
                drNew["DebitorID"] = int.Parse(txttd.SelectedValue);
                drNew["Paymode"] = "Card";
            }

            //drNew["Creditor"] = Convert.ToInt32(txttd.SelectedItem.Value);
            drNew["Amount"] = txt.Text;
            drNew["Narration"] = txtt.Text;
            drNew["VoucherType"] = "Receipt";
            ds.Tables[0].Rows.Add(drNew);
        }

        string conn = GetConnectionString();

        DataSet dst = (DataSet)Session["BillData"];
        int CreditorID = int.Parse(drpLedger.SelectedValue);

        bl.InsertMultipleCustReceipt(conn, ds, CreditorID, dst, usernam);

        ScriptManager.RegisterStartupScript(Page, Page.GetType(), Guid.NewGuid().ToString(), "alert('Receipt Saved Successfully.');", true);

        ModalPopupExtender1.Hide();
        ModalPopupExtender2.Hide();

        GrdViewReceipt.DataBind();
        ClearPanel();
        UpdatePanelPage.Update();
    }


    private void ClearPanel()
    {
        txtRefNo.Text = "";
        txtTransDate.Text = "";
        txtNarration.Text = "";
        txtChequeNo.Text = "";
        txtAmount.Text = "";
        ddReceivedFrom.SelectedValue = "0";
        txtMobile.Text = "";
        ddBanks.SelectedValue = "0";
        GrdBills.DataSource = null;
        GrdBills.DataBind();
        Session["BillData"] = null;
    }

    protected void ComboBox2_SelectedIndexChanged(object sender, EventArgs e)
    {
        try
        {
            string debtorID = ddReceivedFrom.SelectedValue;
            BusinessLogic objBus = new BusinessLogic();

            string Mobile = objBus.GetLedgerMobileForId(Request.Cookies["Company"].Value, int.Parse(debtorID));

            txtMobile.Text = Mobile;

            txtAmount.Focus();
            GrdViewSales.PageSize = 6;

            if (chkPayTo.SelectedItem != null)
            {
                if (chkPayTo.SelectedItem.Text == "Cheque")
                {
                    tblBank.Attributes.Add("class", "AdvancedSearch");
                }
                else
                {
                    tblBank.Attributes.Add("class", "hidden");
                }
            }
            else
            {
                tblBank.Attributes.Add("class", "hidden");
            }

            ShowPendingBills();
            ModalPopupExtender2.Show();

            GrdViewSales.PageSize = 6;
        }
        catch (Exception ex)
        {
            TroyLiteExceptionManager.HandleException(ex);
        }
    }

    protected void chkPayTo_SelectedIndexChanged(object sender, EventArgs e)
    {
        try
        {
            if (chkPayTo.SelectedItem.Text == "Cheque")
            {
                tblBank.Visible = true;
            }
            else
            {
                tblBank.Visible = false;
            }
        }
        catch (Exception ex)
        {
            TroyLiteExceptionManager.HandleException(ex);
        }
    }


    protected void btnSearch_Click(object sender, EventArgs e)
    {
        try
        {
            if (Page.IsValid)
            {
                GrdViewReceipt.DataBind();
            }
        }
        catch (Exception ex)
        {
            TroyLiteExceptionManager.HandleException(ex);
        }
    }
    protected void GrdViewReceipt_RowDeleting(object sender, GridViewDeleteEventArgs e)
    {
        try
        {
            GrdViewReceipt.SelectedIndex = e.RowIndex;
        }
        catch (Exception ex)
        {
            TroyLiteExceptionManager.HandleException(ex);
        }
    }

    protected void GridSource_Deleting(object sender, ObjectDataSourceMethodEventArgs e)
    {
        try
        {
            if (GrdViewReceipt.SelectedDataKey != null)
                e.InputParameters["TransNo"] = GrdViewReceipt.SelectedDataKey.Value;

            e.InputParameters["Username"] = Request.Cookies["LoggedUserName"].Value;

            string salestype = string.Empty;
            int ScreenNo = 0;
            string ScreenName = string.Empty;

            BusinessLogic bl = new BusinessLogic();

            string connection = Request.Cookies["Company"].Value;
            salestype = "Customer Receipt";
            ScreenName = "Customer Receipt";
            int DebitorID = 0;
            string TransDate = string.Empty;
            double Amount = 0;
            string PayTo = string.Empty;
            DataSet ds = bl.GetReceiptForId(connection, int.Parse(GrdViewReceipt.SelectedDataKey.Value.ToString()));
            if (ds != null)
            {
                TransDate = Convert.ToString(ds.Tables[0].Rows[0]["TransDate"].ToString());
                DebitorID = Convert.ToInt32(ds.Tables[0].Rows[0]["DebtorID"]);
                Amount = Convert.ToDouble(ds.Tables[0].Rows[0]["Amount"]);
                PayTo = ds.Tables[0].Rows[0]["paymode"].ToString();
            }

            DataSet dsddd = bl.GetScreenNoForScreenName(connection, ScreenName);
            if (dsddd != null)
            {
                if (dsddd.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow dr in dsddd.Tables[0].Rows)
                    {
                        ScreenNo = Convert.ToInt32(dr["ScreenNo"]);
                    }
                }
            }

            if (hdEmailRequired.Value == "YES")
            {
                DataSet dsd = bl.GetLedgerInfoForId(connection, DebitorID);
                var toAddress = "";
                var toAdd = "";
                Int32 ModeofContact = 0;
                string Active = string.Empty;

                if (dsd != null)
                {
                    if (dsd.Tables[0].Rows.Count > 0)
                    {
                        foreach (DataRow dr in dsd.Tables[0].Rows)
                        {
                            toAdd = dr["EmailId"].ToString();
                            ModeofContact = Convert.ToInt32(dr["ModeofContact"]);
                        }
                    }
                }

                DataSet dsdd = bl.GetDetailsForScreenNo(connection, ScreenNo, "Email");
                if (dsdd != null)
                {
                    if (dsdd.Tables[0].Rows.Count > 0)
                    {
                        foreach (DataRow dr in dsdd.Tables[0].Rows)
                        {
                            Active = dr["Active"].ToString();
                            if (Active == "YES")
                            {
                                if (dr["EmailId"].ToString() == "Customer")
                                {
                                    toAddress = toAdd;
                                }
                                else
                                {
                                    toAddress = dr["EmailId"].ToString();
                                }

                                if (ModeofContact == 2)
                                {
                                    string subject = "Deleted - Customer Receipt in Branch " + Request.Cookies["Company"].Value;

                                    string body = "\n";
                                    body += " Branch           : " + Request.Cookies["Company"].Value + "\n";
                                    body += " Trans No         : " + GrdViewReceipt.SelectedDataKey.Value + "\n";
                                    body += " User Name        : " + Request.Cookies["LoggedUserName"].Value + "\n";
                                    body += " Trans Date       : " + TransDate + "\n";
                                    body += " Amount           : " + Amount + "\n";
                                    body += " PayTo            : " + PayTo + "\n";

                                    string smtphostname = ConfigurationManager.AppSettings["SmtpHostName"].ToString();
                                    int smtpport = Convert.ToInt32(ConfigurationManager.AppSettings["SmtpPortNumber"]);
                                    var fromAddress = ConfigurationManager.AppSettings["FromAddress"].ToString();

                                    string fromPassword = ConfigurationManager.AppSettings["FromPassword"].ToString();

                                    EmailLogic.SendEmail(smtphostname, smtpport, fromAddress, toAddress, subject, body, fromPassword);

                                    //ScriptManager.RegisterStartupScript(Page, Page.GetType(), Guid.NewGuid().ToString(), "alert('Email sent successfully')", true);
                                }

                            }
                        }
                    }
                }
            }

        }
        catch (Exception ex)
        {
            TroyLiteExceptionManager.HandleException(ex);
        }
    }

    protected void GrdViewReceipt_RowDeleted(object sender, GridViewDeletedEventArgs e)
    {
        try
        {
            if (e.Exception == null)
            {
                GrdViewReceipt.DataBind();
            }
            else
            {
                if (e.Exception.InnerException != null)
                {
                    StringBuilder script = new StringBuilder();
                    script.Append("alert('You are not allowed to delete the record. Please contact Administrator.');");

                    if (e.Exception.InnerException.Message.IndexOf("Invalid Date") > -1)
                        ScriptManager.RegisterStartupScript(Page, Page.GetType(), Guid.NewGuid().ToString(), script.ToString(), true);

                    e.ExceptionHandled = true;
                }
            }
        }
        catch (Exception ex)
        {
            TroyLiteExceptionManager.HandleException(ex);
        }
    }

    protected void EditBill(object sender, GridViewEditEventArgs e)
    {
        GrdBills.EditIndex = e.NewEditIndex;
        DataRow row = ((DataSet)Session["BillData"]).Tables[0].Rows[e.NewEditIndex];
        Session["EditedRow"] = e.NewEditIndex.ToString();
        Session["EditedAmount"] = row["Amount"].ToString();
        GrdBills.DataSource = (DataSet)Session["BillData"];
        GrdBills.DataBind();
    }

    private void calcSum()
    {
        var ds = (DataSet)GrdBills.DataSource;

        if (ds != null)
        {
            if (ds.Tables.Count > 0)
            {
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    if (dr["Amount"] != null)
                    {
                        sumAmt = sumAmt + Convert.ToDouble(dr["Amount"].ToString());
                    }
                }
            }
        }
    }

    private double calcDatasetSum(DataSet ds)
    {
        double total = 0.0;

        if (ds != null)
        {
            if (ds.Tables.Count > 0)
            {
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    if (dr["Amount"] != null)
                    {
                        total = total + Convert.ToDouble(dr["Amount"].ToString());
                    }
                }
            }
        }

        return total;
    }

    protected void GrdBills_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        if (e.CommandName == "Cancel")
        {
            GrdBills.FooterRow.Visible = false;
            var ds = (DataSet)Session["BillData"];
            GrdBills.EditIndex = -1;
            if (ds != null)
            {
                GrdBills.DataSource = ds;
            }
            GrdBills.DataBind();
            lnkAddBills.Visible = true;
            ModalPopupExtender2.Show();
            pnlEdit.Visible = true;
            Error.Text = "";
        }
        else if (e.CommandName == "Edit")
        {
            ModalPopupExtender2.Show();
            lnkAddBills.Visible = false;
        }
        else if (e.CommandName == "Insert")
        {
            try
            {
                ModalPopupExtender2.Show();
                DataTable dt;
                DataRow drNew;
                DataColumn dc;
                DataSet ds;
                BusinessLogic bl = new BusinessLogic(GetConnectionString());

                string billNo = ((TextBox)GrdBills.FooterRow.FindControl("txtAddBillNo")).Text;
                string amount = ((TextBox)GrdBills.FooterRow.FindControl("txtAddBillAmount")).Text;
                string CustomerID = ddReceivedFrom.SelectedValue.ToString().Trim();
                string TransNo = string.Empty;

                if (GrdViewReceipt.SelectedDataKey != null)
                    TransNo = GrdViewReceipt.SelectedDataKey.Value.ToString();
                else
                    TransNo = "";

                if (bl.GetIfBillNoExists(int.Parse(billNo), CustomerID) == 0)
                {
                    ScriptManager.RegisterStartupScript(Page, Page.GetType(), Guid.NewGuid().ToString(), "alert('BillNo does not Exists. Please check BillNo.')", true);
                    //Error.Text = "BillNo does not Exists. Please check BillNo.";
                    pnlEdit.Visible = true;
                    ModalPopupExtender2.Show();
                    return;
                }

                var isBillExists = CheckIfBillExists(billNo);

                if (isBillExists)
                {
                    ScriptManager.RegisterStartupScript(Page, Page.GetType(), Guid.NewGuid().ToString(), "alert('BillNo already Exists')", true);
                    //Error.Text = "BillNo already Exists";
                    ModalPopupExtender2.Show();
                    return;
                }


                double eligibleAmount = bl.GetSalesPendingAmount(int.Parse(billNo));

                if (double.Parse(amount) > eligibleAmount)
                {
                    ScriptManager.RegisterStartupScript(Page, Page.GetType(), Guid.NewGuid().ToString(), "alert('The Amount you entered for BillNo:" + billNo + " is Greater than Pending Sales Amount of " + eligibleAmount.ToString() + ". Please check the Bill Amount')", true);
                    //Error.Text = "The Amount you entered for BillNo:" + billNo + " is Greater than Pending Sales Amount of " + eligibleAmount.ToString() + ". Please check the Bill Amount";
                    ModalPopupExtender2.Show();
                    return;
                }

                if ((Session["BillData"] == null) || (((DataSet)Session["BillData"]).Tables[0].Rows.Count == 0))
                {

                    if (double.Parse(amount) > double.Parse(txtAmount.Text))
                    {
                        ScriptManager.RegisterStartupScript(Page, Page.GetType(), Guid.NewGuid().ToString(), "alert('Total Bills amount is exceeding the Receipt Amount. Please check the Bill Amount')", true);
                        //Error.Text = "Total Bills amount is exceeding the Receipt Amount. Please check the Bill Amount";
                        ModalPopupExtender2.Show();
                        return;
                    }

                    ds = new DataSet();
                    dt = new DataTable();

                    dc = new DataColumn("ReceiptNo");
                    dt.Columns.Add(dc);

                    dc = new DataColumn("BillNo");
                    dt.Columns.Add(dc);

                    dc = new DataColumn("Amount");
                    dt.Columns.Add(dc);

                    ds.Tables.Add(dt);

                    drNew = dt.NewRow();

                    drNew["ReceiptNo"] = TransNo;
                    drNew["BillNo"] = billNo;
                    drNew["Amount"] = amount;

                    ds.Tables[0].Rows.Add(drNew);

                    Session["BillData"] = ds;
                    GrdBills.DataSource = ds;
                    GrdBills.DataBind();
                    GrdBills.EditIndex = -1;
                    lnkAddBills.Visible = true;

                }
                else
                {
                    ds = (DataSet)Session["BillData"];

                    if ((calcDatasetSum(ds) + double.Parse(amount)) > double.Parse(txtAmount.Text))
                    {
                        ScriptManager.RegisterStartupScript(Page, Page.GetType(), Guid.NewGuid().ToString(), "alert('Total Bills amount is exceeding the Receipt Amount. Please check the Bill Amount')", true);
                        //Error.Text = "Total Bills amount is exceeding the Receipt Amount. Please check the Bill Amount";
                        ModalPopupExtender2.Show();
                        return;
                    }

                    if (ds.Tables[0].Rows[0]["ReceiptNo"].ToString() == "0")
                    {
                        ds.Tables[0].Rows[0].Delete();
                        ds.Tables[0].AcceptChanges();
                    }

                    drNew = ds.Tables[0].NewRow();
                    drNew["ReceiptNo"] = TransNo;
                    drNew["BillNo"] = billNo;
                    drNew["Amount"] = amount;

                    ds.Tables[0].Rows.Add(drNew);
                    Session["BillData"] = ds;
                    //System.Threading.Thread.Sleep(1000);
                    GrdBills.DataSource = ds;
                    GrdBills.DataBind();
                    GrdBills.EditIndex = -1;
                    lnkAddBills.Visible = true;
                    ModalPopupExtender2.Show();
                    checkPendingBills(ds);
                }

            //}
            //catch (Exception ex)
            //{
            //    if (ex.InnerException != null)
            //    {
            //        StringBuilder script = new StringBuilder();
            //        script.Append("alert('Unit with this name already exists, Please try with a different name.');");

            //        if (ex.InnerException.Message.IndexOf("duplicate values in the index") > -1)
            //            ScriptManager.RegisterStartupScript(Page, Page.GetType(), Guid.NewGuid().ToString(), script.ToString(), true);

            //        ModalPopupExtender2.Show();
            //        return;
            //    }
            //}
            }
            catch (Exception ex)
            {
                TroyLiteExceptionManager.HandleException(ex);
                ModalPopupExtender2.Show();
                return;
            }
            finally
            {
                //checkPendingBills();
            }
        }
    }

    protected void GrdBills_RowUpdated(object sender, GridViewUpdatedEventArgs e)
    {
        try
        {
            System.Threading.Thread.Sleep(1000);
            GrdBills.DataBind();
            lnkAddBills.Visible = true;
            //checkPendingBills();
        }
        catch (Exception ex)
        {
            TroyLiteExceptionManager.HandleException(ex);
        }
    }

    private bool CheckIfBillExists(string billNo)
    {
        bool dupFlag = false;

        if (Session["BillData"] != null)
        {
            var checkDs = (DataSet)Session["BillData"];

            foreach (DataRow dR in checkDs.Tables[0].Rows)
            {
                if (dR["BillNo"] != null)
                {
                    if (dR["BillNo"].ToString().Trim() == billNo)
                    {
                        dupFlag = true;
                        break;
                    }
                }
            }
        }

        return dupFlag;
    }

    private int CheckNoOfBillExists(string billNo)
    {
        int count = 0;

        if (Session["BillData"] != null)
        {
            var checkDs = (DataSet)Session["BillData"];

            foreach (DataRow dR in checkDs.Tables[0].Rows)
            {
                if (dR["BillNo"] != null)
                {
                    if (dR["BillNo"].ToString().Trim() == billNo)
                    {
                        count = count + 1;
                    }
                }
            }
        }

        return count;
    }


    protected void GrdBills_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        //checkPendingBills();
    }

    protected void GrdBills_RowDeleting(object sender, GridViewDeleteEventArgs e)
    {
        DataSet ds;

        try
        {
            if (Session["BillData"] != null)
            {
                GridViewRow row = GrdBills.Rows[e.RowIndex];
                ds = (DataSet)Session["BillData"];
                ds.Tables[0].Rows[GrdBills.Rows[e.RowIndex].DataItemIndex].Delete();
                ds.Tables[0].AcceptChanges();
                GrdBills.DataSource = ds;
                GrdBills.DataBind();
                Session["BillData"] = ds;
                ModalPopupExtender2.Show();
            }
        }
        catch (Exception ex)
        {
            TroyLiteExceptionManager.HandleException(ex);
        }
    }

    protected void GrdBills_RowUpdating(object sender, GridViewUpdateEventArgs e)
    {
        try
        {
            int curRow = 0;
            string billNo = ((TextBox)GrdBills.Rows[e.RowIndex].FindControl("txtBillNo")).Text;
            string amount = ((TextBox)GrdBills.Rows[e.RowIndex].FindControl("txtBillAmount")).Text;
            //string Id = GrdBills.DataKeys[e.RowIndex].Value.ToString();
            string CustomerID = ddReceivedFrom.SelectedValue.ToString().Trim();
            string TransNo = "0";
            ModalPopupExtender2.Show();

            if (GrdViewReceipt.SelectedDataKey != null)
                TransNo = GrdViewReceipt.SelectedDataKey.Value.ToString();


            DataSet ds = (DataSet)Session["BillData"];

            if ((calcDatasetSum(ds) + double.Parse(amount) - double.Parse(Session["EditedAmount"].ToString())) > double.Parse(txtAmount.Text))
            {
                ScriptManager.RegisterStartupScript(Page, Page.GetType(), Guid.NewGuid().ToString(), "alert('Total Bills amount is exceeding the Receipt Amount. Please check the Bill Amount')", true);
                //Error.Text = "Total Bills amount is exceeding the Receipt Amount. Please check the Bill Amount";
                return;
            }

            BusinessLogic bl = new BusinessLogic(GetConnectionString());

            if (bl.GetIfBillNoExists(int.Parse(billNo), CustomerID) == 0)
            {
                ScriptManager.RegisterStartupScript(Page, Page.GetType(), Guid.NewGuid().ToString(), "alert('BillNo does not Exists. Please check BillNo.')", true);
                //Error.Text = "BillNo does not Exists. Please check BillNo.";
                pnlEdit.Visible = true;
                ModalPopupExtender2.Show();
                return;
            }

            double eligibleAmount = bl.GetSalesPendingAmount(int.Parse(billNo));


            if ((double.Parse(amount) - double.Parse(Session["EditedAmount"].ToString())) > eligibleAmount)
            {
                var eliAmount = double.Parse(Session["EditedAmount"].ToString()) + eligibleAmount;
                ScriptManager.RegisterStartupScript(Page, Page.GetType(), Guid.NewGuid().ToString(), "alert('The Amount you entered for BillNo:" + billNo + " is Greater than Pending Sales Amount of " + eliAmount.ToString() + ". Please check the Bill Amount')", true);
                //Error.Text = "The Amount you entered for BillNo:" + billNo + " is Greater than Pending Sales Amount of " + eliAmount.ToString() + ". Please check the Bill Amount";
                return;
            }

            curRow = Convert.ToInt32(Session["EditedRow"].ToString());

            ds.Tables[0].Rows[curRow].BeginEdit();
            ds.Tables[0].Rows[curRow]["BillNo"] = billNo;
            ds.Tables[0].Rows[curRow]["Amount"] = amount;
            ds.Tables[0].Rows[curRow]["ReceiptNo"] = TransNo;

            var isBillExists = CheckNoOfBillExists(billNo);

            if (isBillExists > 1)
            {
                ds.Tables[0].Rows[curRow].RejectChanges();
                ScriptManager.RegisterStartupScript(Page, Page.GetType(), Guid.NewGuid().ToString(), "alert('BillNo already Exists')", true);
                //Error.Text = "BillNo already Exists";
                return;
            }

            ds.Tables[0].Rows[curRow].EndEdit();

            ds.Tables[0].Rows[curRow].AcceptChanges();
            GrdBills.DataSource = ds;
            GrdBills.EditIndex = -1;
            GrdBills.DataBind();
            Session["BillData"] = ds;
            lnkAddBills.Visible = true;
            checkPendingBills(ds);
        }
        catch (Exception ex)
        {
            TroyLiteExceptionManager.HandleException(ex);
        }
    }

    private string GetConnectionString()
    {
        string connStr = string.Empty;

        if (Request.Cookies["Company"] != null)
            connStr = System.Configuration.ConfigurationManager.ConnectionStrings[Request.Cookies["Company"].Value].ToString();
        else
            Response.Redirect("~/Login.aspx");

        return connStr;
    }

    protected void GrdBills_RowCreated(object sender, GridViewRowEventArgs e)
    {
        try
        {
            if (e.Row.RowType == DataControlRowType.Pager)
            {
                PresentationUtils.SetPagerButtonStates(GrdBills, e.Row, this);
            }
        }
        catch (Exception ex)
        {
            TroyLiteExceptionManager.HandleException(ex);
        }
    }
    protected void SaveButton_Click(object sender, EventArgs e)
    {
        try
        {
            //Button clickedbutton = (Button)sender;
            //clickedbutton.ToolTip = "Button Already clicked";
            //clickedbutton.Enabled = false;

            DataSet dsData = (DataSet)Session["BillData"];

            string ChequeNot = string.Empty;
            ChequeNot = txtChequeNo.Text;

            if ((chkPayTo.SelectedValue == "Cheque") || (chkPayTo.SelectedValue == "Card"))
            {
                if ((ChequeNot == "") && (int.Parse(ddBanks.SelectedValue) == 0))
                {
                    ScriptManager.RegisterStartupScript(Page, Page.GetType(), Guid.NewGuid().ToString(), "alert('Bank Name And Cheque No Mandatory');", true);
                    ModalPopupExtender2.Show();
                    return;
                }
                else if (ChequeNot == "")
                {
                    ScriptManager.RegisterStartupScript(Page, Page.GetType(), Guid.NewGuid().ToString(), "alert('Cheque No Mandatory');", true);
                    ModalPopupExtender2.Show();
                    return;
                }
                else if (int.Parse(ddBanks.SelectedValue) == 0)
                {
                    ScriptManager.RegisterStartupScript(Page, Page.GetType(), Guid.NewGuid().ToString(), "alert('Bank Name Mandatory');", true);
                    ModalPopupExtender2.Show();
                    return;
                }
            }

            if (calcDatasetSum(dsData) > double.Parse(txtAmount.Text))
            {
                ScriptManager.RegisterStartupScript(Page, Page.GetType(), Guid.NewGuid().ToString(), "alert('Total Bills amount is exceeding the Receipt Amount. Please check the Bill Amount')", true);
                return;
            }


            //if (chkPayTo.SelectedValue == "Cheque")
            //{
            //    cvBank.Enabled = true;
            //    rvChequeNo.Enabled = true;
            //}
            //else
            //{
            //    cvBank.Enabled = false;
            //    rvChequeNo.Enabled = false;
            //}

            Page.Validate();

            if (Page.IsValid)
            {

                int CreditorID = int.Parse(ddReceivedFrom.SelectedValue);

                string RefNo = txtRefNo.Text;

                DateTime TransDate = DateTime.Parse(txtTransDate.Text);

                int DebitorID = 0;
                string Paymode = string.Empty;
                double Amount = 0.0;
                string Narration = string.Empty;
                string VoucherType = string.Empty;
                string ChequeNo = string.Empty;

                if (chkPayTo.SelectedValue == "Cash")
                {
                    DebitorID = 1;
                    Paymode = "Cash";
                }
                else if (chkPayTo.SelectedValue == "Cheque")
                {
                    DebitorID = int.Parse(ddBanks.SelectedValue);
                    Paymode = "Cheque";
                }
                else if (chkPayTo.SelectedValue == "Card")
                {
                    DebitorID = int.Parse(ddBanks.SelectedValue);
                    Paymode = "Card";
                }

                Amount = double.Parse(txtAmount.Text);
                Narration = txtNarration.Text;
                VoucherType = "Receipt";
                ChequeNo = txtChequeNo.Text;

                BusinessLogic bl = new BusinessLogic();
                string connection = Request.Cookies["Company"].Value;

                //if (chkPayTo.SelectedValue == "Cheque")
                //{
                //    if (ChequeNo != "")
                //    {
                //        if (bl.IsChequeNoAlreadyPresent(connection, ChequeNo))
                //        {
                //            ScriptManager.RegisterStartupScript(Page, Page.GetType(), Guid.NewGuid().ToString(), "alert('Cheque No Already Billed')", true);
                //            return;
                //        }

                //        DataSet dsdat = new DataSet();
                //        dsdat = bl.GetChequeNoGiven(connection, ChequeNo);
                //        string datad = string.Empty;

                //        Int32 set = 0;
                //        Int32 setdd = 0;

                //        Int32 Cheque = 0;
                //        Cheque = Convert.ToInt32(txtChequeNo.Text);

                //        if (dsdat != null)
                //        {
                //            if (dsdat.Tables[0].Rows.Count > 0)
                //            {
                //                foreach (DataRow dr in dsdat.Tables[0].Rows)
                //                {
                //                    set = Convert.ToInt32(dr["FromChequeNo"]);
                //                    setdd = Convert.ToInt32(dr["ToChequeNo"]);

                //                    if ((Cheque >= set) && (Cheque <= setdd))
                //                    {
                //                        datad = "Y";
                //                        break;
                //                    }
                //                    else
                //                    {
                //                        datad = "N";
                //                    }
                //                }
                //                if (datad == "N")
                //                {
                //                    ScriptManager.RegisterStartupScript(Page, Page.GetType(), Guid.NewGuid().ToString(), "alert('Cheque No Not Found in Cheque Book')", true);
                //                    return;
                //                }
                //            }
                //        }
                //    }
                //}

                if (hdSMSRequired.Value == "YES")
                {

                    if (txtMobile.Text != "")
                        hdMobile.Value = txtMobile.Text;

                    hdText.Value = "Thank you for Payment of Rs." + txtAmount.Text;

                }


                string conn = GetConnectionString();
                int OutPut = 0;

                DataSet ds = (DataSet)Session["BillData"];


                //if (ds == null)
                //{
                //    ScriptManager.RegisterStartupScript(Page, Page.GetType(), Guid.NewGuid().ToString(), "alert('Not given the Bill Details')", true);

                //}

                string usernam = Request.Cookies["LoggedUserName"].Value;

                bl.InsertCustReceipt(out OutPut, conn, RefNo, TransDate, DebitorID, CreditorID, Amount, Narration, VoucherType, ChequeNo, Paymode, ds, usernam);

                ScriptManager.RegisterStartupScript(Page, Page.GetType(), Guid.NewGuid().ToString(), "alert('Receipt Saved Successfully. Transaction No : " + OutPut.ToString() + "');", true);

                if (hdSMS.Value == "YES")
                {
                    UtilitySMS utilSMS = new UtilitySMS(conn);
                    string UserID = Page.User.Identity.Name;

                    if (Session["Provider"] != null)
                        utilSMS.SendSMS(Session["Provider"].ToString(), Session["Priority"].ToString(), Session["SenderID"].ToString(), Session["UserName"].ToString(), Session["Password"].ToString(), hdMobile.Value, hdText.Value, true, UserID);
                    else
                    {
                        if (hdMobile.Value != "")
                            ScriptManager.RegisterStartupScript(Page, Page.GetType(), Guid.NewGuid().ToString(), "alert('you are not configured to send SMS. Please contact Administrator.');", true);
                    }
                }


                string salestype = string.Empty;
                int ScreenNo = 0;
                string ScreenName = string.Empty;

                salestype = "Customer Receipt";
                ScreenName = "Customer Receipt";
                
                DataSet dsddd = bl.GetScreenNoForScreenName(connection, ScreenName);
                if (dsddd != null)
                {
                    if (dsddd.Tables[0].Rows.Count > 0)
                    {
                        foreach (DataRow dr in dsddd.Tables[0].Rows)
                        {
                            ScreenNo = Convert.ToInt32(dr["ScreenNo"]);
                        }
                    }
                }

                if (hdEmailRequired.Value == "YES")
                {
                    DataSet dsd = bl.GetLedgerInfoForId(connection, DebitorID);
                    var toAddress = "";
                    var toAdd = "";
                    Int32 ModeofContact = 0;
                    string Active = string.Empty;

                    if (dsd != null)
                    {
                        if (dsd.Tables[0].Rows.Count > 0)
                        {
                            foreach (DataRow dr in dsd.Tables[0].Rows)
                            {
                                toAdd = dr["EmailId"].ToString();
                                ModeofContact = Convert.ToInt32(dr["ModeofContact"]);
                            }
                        }
                    }

                    DataSet dsdd = bl.GetDetailsForScreenNo(connection, ScreenNo, "Email");
                    if (dsdd != null)
                    {
                        if (dsdd.Tables[0].Rows.Count > 0)
                        {
                            foreach (DataRow dr in dsdd.Tables[0].Rows)
                            {
                                Active = dr["Active"].ToString();
                                if (Active == "YES")
                                {
                                    if (dr["EmailId"].ToString() == "Customer")
                                    {
                                        toAddress = toAdd;
                                    }
                                    else
                                    {
                                        toAddress = dr["EmailId"].ToString();
                                    }

                                    if (ModeofContact == 2)
                                    {
                                        string subject = "Added - Customer Receipt in Branch " + Request.Cookies["Company"].Value;

                                        string body = "\n";
                                        body += " Branch           : " + Request.Cookies["Company"].Value + "\n";
                                        body += " Trans No         : " + GrdViewReceipt.SelectedDataKey.Value + "\n";
                                        body += " User Name        : " + Request.Cookies["LoggedUserName"].Value + "\n";
                                        body += " Trans Date       : " + TransDate + "\n";
                                        body += " Amount           : " + Amount + "\n";
                                        body += " Narration        : " + Narration + "\n";
                                        body += " Customer         : " + ddReceivedFrom.SelectedItem.Text + "\n";
                                        
                                        string smtphostname = ConfigurationManager.AppSettings["SmtpHostName"].ToString();
                                        int smtpport = Convert.ToInt32(ConfigurationManager.AppSettings["SmtpPortNumber"]);
                                        var fromAddress = ConfigurationManager.AppSettings["FromAddress"].ToString();

                                        string fromPassword = ConfigurationManager.AppSettings["FromPassword"].ToString();

                                        EmailLogic.SendEmail(smtphostname, smtpport, fromAddress, toAddress, subject, body, fromPassword);

                                        //ScriptManager.RegisterStartupScript(Page, Page.GetType(), Guid.NewGuid().ToString(), "alert('Email sent successfully')", true);
                                    }

                                }
                            }
                        }
                    }
                }

                pnlEdit.Visible = false;
                ModalPopupExtender2.Hide();
                lnkBtnAdd.Visible = true;
                //MyAccordion.Visible = true;
                GrdViewReceipt.Visible = true;
                GrdViewReceipt.DataBind();
                ClearPanel();
                UpdatePanelPage.Update();

            }
        }
        catch (Exception ex)
        {
            TroyLiteExceptionManager.HandleException(ex);
        }
    }

    protected void ShowPendingSales_Click(object sender, EventArgs e)
    {
        try
        {
            ModalPopupExtender1.Show();
            GrdViewSales.PageSize = 6;
        }
        catch (Exception ex)
        {
            TroyLiteExceptionManager.HandleException(ex);
        }
    }

    protected void UpdateButton_Click(object sender, EventArgs e)
    {
        try
        {
            DataSet dsData = (DataSet)Session["BillData"];

            string ChequeNot = string.Empty;
            ChequeNot = txtChequeNo.Text;

            if ((chkPayTo.SelectedValue == "Cheque") || (chkPayTo.SelectedValue == "Card"))
            {
                if ((ChequeNot == "") && (int.Parse(ddBanks.SelectedValue) == 0))
                {
                    ScriptManager.RegisterStartupScript(Page, Page.GetType(), Guid.NewGuid().ToString(), "alert('Bank Name And Cheque No Mandatory');", true);
                    ModalPopupExtender2.Show();
                    return;
                }
                else if (ChequeNot == "")
                {
                    ScriptManager.RegisterStartupScript(Page, Page.GetType(), Guid.NewGuid().ToString(), "alert('Cheque No Mandatory');", true);
                    ModalPopupExtender2.Show();
                    return;
                }
                else if (int.Parse(ddBanks.SelectedValue) == 0)
                {
                    ScriptManager.RegisterStartupScript(Page, Page.GetType(), Guid.NewGuid().ToString(), "alert('Bank Name Mandatory');", true);
                    ModalPopupExtender2.Show();
                    return;
                }
            }


            if (calcDatasetSum(dsData) > double.Parse(txtAmount.Text))
            {
                ScriptManager.RegisterStartupScript(Page, Page.GetType(), Guid.NewGuid().ToString(), "alert('Total Bills amount is exceeding the Receipt Amount. Please check the Bill Amount')", true);
                return;
            }


            //if (chkPayTo.SelectedValue == "Cheque")
            //{
            //    cvBank.Enabled = true;
            //    rvChequeNo.Enabled = true;
            //}
            //else
            //{
            //    cvBank.Enabled = false;
            //    rvChequeNo.Enabled = false;
            //}

            Page.Validate();

            if (Page.IsValid)
            {

                int CreditorID = int.Parse(ddReceivedFrom.SelectedValue);

                string RefNo = txtRefNo.Text;

                DateTime TransDate = DateTime.Parse(txtTransDate.Text);

                int DebitorID = 0;
                string Paymode = string.Empty;
                double Amount = 0.0;
                string Narration = string.Empty;
                string VoucherType = string.Empty;
                string ChequeNo = string.Empty;
                int TransNo = 0;

                if (chkPayTo.SelectedValue == "Cash")
                {
                    DebitorID = 1;
                    Paymode = "Cash";
                }
                else if (chkPayTo.SelectedValue == "Cheque")
                {
                    DebitorID = int.Parse(ddBanks.SelectedValue);
                    Paymode = "Cheque";
                }
                else if (chkPayTo.SelectedValue == "Card")
                {
                    DebitorID = int.Parse(ddBanks.SelectedValue);
                    Paymode = "Card";
                }

                Amount = double.Parse(txtAmount.Text);
                Narration = txtNarration.Text;
                VoucherType = "Receipt";
                ChequeNo = txtChequeNo.Text;

                BusinessLogic bl = new BusinessLogic();

                string connection = Request.Cookies["Company"].Value;

                //if (chkPayTo.SelectedValue == "Cheque")
                //{
                //    if (ChequeNo != "")
                //    {
                //        if (bl.IsChequeNoAlreadyPresent(connection, ChequeNo))
                //        {
                //            ScriptManager.RegisterStartupScript(Page, Page.GetType(), Guid.NewGuid().ToString(), "alert('Cheque No Already Billed')", true);
                //            return;
                //        }

                //        DataSet dsdat = new DataSet();
                //        dsdat = bl.GetChequeNoGiven(connection, ChequeNo);
                //        string datad = string.Empty;

                //        Int32 set = 0;
                //        Int32 setdd = 0;

                //        Int32 Cheque = 0;
                //        Cheque = Convert.ToInt32(txtChequeNo.Text);

                //        if (dsdat != null)
                //        {
                //            if (dsdat.Tables[0].Rows.Count > 0)
                //            {
                //                foreach (DataRow dr in dsdat.Tables[0].Rows)
                //                {
                //                    set = Convert.ToInt32(dr["FromChequeNo"]);
                //                    setdd = Convert.ToInt32(dr["ToChequeNo"]);

                //                    if ((Cheque >= set) && (Cheque <= setdd))
                //                    {
                //                        datad = "Y";
                //                        break;
                //                    }
                //                    else
                //                    {
                //                        datad = "N";
                //                    }
                //                }
                //                if (datad == "N")
                //                {
                //                    ScriptManager.RegisterStartupScript(Page, Page.GetType(), Guid.NewGuid().ToString(), "alert('Cheque No Not Found in Cheque Book')", true);
                //                    return;
                //                }
                //            }
                //        }
                //    }
                //}


                TransNo = int.Parse(GrdViewReceipt.SelectedDataKey.Value.ToString());

                if (hdSMSRequired.Value == "YES")
                {

                    if (txtMobile.Text != "")
                        hdMobile.Value = txtMobile.Text;

                    hdText.Value = "Thank you for Payment of Rs." + txtAmount.Text;

                }


                string conn = GetConnectionString();
                int OutPut = 0;

                DataSet ds = (DataSet)Session["BillData"];

                string usernam = Request.Cookies["LoggedUserName"].Value;

                bl.UpdateCustReceipt(out OutPut, conn, TransNo, RefNo, TransDate, DebitorID, CreditorID, Amount, Narration, VoucherType, ChequeNo, Paymode, ds, usernam);

                ScriptManager.RegisterStartupScript(Page, Page.GetType(), Guid.NewGuid().ToString(), "alert('Receipt Updated Successfully. Transaction No : " + OutPut.ToString() + "');", true);

                if (hdSMS.Value == "YES")
                {
                    UtilitySMS utilSMS = new UtilitySMS(conn);
                    string UserID = Page.User.Identity.Name;

                    if (Session["Provider"] != null)
                        utilSMS.SendSMS(Session["Provider"].ToString(), Session["Priority"].ToString(), Session["SenderID"].ToString(), Session["UserName"].ToString(), Session["Password"].ToString(), hdMobile.Value, hdText.Value, true, UserID);
                    else
                    {
                        if (hdMobile.Value != "")
                            ScriptManager.RegisterStartupScript(Page, Page.GetType(), Guid.NewGuid().ToString(), "alert('you are not configured to send SMS. Please contact Administrator.');", true);
                    }
                }

                string salestype = string.Empty;
                int ScreenNo = 0;
                string ScreenName = string.Empty;

                salestype = "Customer Receipt";
                ScreenName = "Customer Receipt";

                DataSet dsddd = bl.GetScreenNoForScreenName(connection, ScreenName);
                if (dsddd != null)
                {
                    if (dsddd.Tables[0].Rows.Count > 0)
                    {
                        foreach (DataRow dr in dsddd.Tables[0].Rows)
                        {
                            ScreenNo = Convert.ToInt32(dr["ScreenNo"]);
                        }
                    }
                }

                if (hdEmailRequired.Value == "YES")
                {
                    DataSet dsd = bl.GetLedgerInfoForId(connection, DebitorID);
                    var toAddress = "";
                    var toAdd = "";
                    Int32 ModeofContact = 0;
                    string Active = string.Empty;

                    if (dsd != null)
                    {
                        if (dsd.Tables[0].Rows.Count > 0)
                        {
                            foreach (DataRow dr in dsd.Tables[0].Rows)
                            {
                                toAdd = dr["EmailId"].ToString();
                                ModeofContact = Convert.ToInt32(dr["ModeofContact"]);
                            }
                        }
                    }

                    DataSet dsdd = bl.GetDetailsForScreenNo(connection, ScreenNo, "Email");
                    if (dsdd != null)
                    {
                        if (dsdd.Tables[0].Rows.Count > 0)
                        {
                            foreach (DataRow dr in dsdd.Tables[0].Rows)
                            {
                                Active = dr["Active"].ToString();
                                if (Active == "YES")
                                {
                                    if (dr["EmailId"].ToString() == "Customer")
                                    {
                                        toAddress = toAdd;
                                    }
                                    else
                                    {
                                        toAddress = dr["EmailId"].ToString();
                                    }

                                    if (ModeofContact == 2)
                                    {
                                        string subject = "Updated - Customer Receipt in Branch " + Request.Cookies["Company"].Value;

                                        string body = "\n";
                                        body += " Branch           : " + Request.Cookies["Company"].Value + "\n";
                                        body += " Trans No         : " + GrdViewReceipt.SelectedDataKey.Value + "\n";
                                        body += " User Name        : " + Request.Cookies["LoggedUserName"].Value + "\n";
                                        body += " Trans Date       : " + TransDate + "\n";
                                        body += " Amount           : " + Amount + "\n";
                                        body += " Narration        : " + Narration + "\n";
                                        body += " Customer         : " + ddReceivedFrom.SelectedItem.Text + "\n";

                                        string smtphostname = ConfigurationManager.AppSettings["SmtpHostName"].ToString();
                                        int smtpport = Convert.ToInt32(ConfigurationManager.AppSettings["SmtpPortNumber"]);
                                        var fromAddress = ConfigurationManager.AppSettings["FromAddress"].ToString();

                                        string fromPassword = ConfigurationManager.AppSettings["FromPassword"].ToString();

                                        EmailLogic.SendEmail(smtphostname, smtpport, fromAddress, toAddress, subject, body, fromPassword);

                                        //ScriptManager.RegisterStartupScript(Page, Page.GetType(), Guid.NewGuid().ToString(), "alert('Email sent successfully')", true);
                                    }

                                }
                            }
                        }
                    }
                }


                pnlEdit.Visible = false;
                //lnkBtnAdd.Visible = true;
                ////MyAccordion.Visible = true;
                //GrdViewReceipt.Visible = true;
                //ModalPopupExtender2.Hide();
                //popUp.Visible = false;
                GrdViewReceipt.DataBind();
                ClearPanel();
                UpdatePanelPage.Update();

            }
        }
        catch (Exception ex)
        {
            TroyLiteExceptionManager.HandleException(ex);
        }
    }
}