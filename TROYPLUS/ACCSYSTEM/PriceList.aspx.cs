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

public partial class PriceList : System.Web.UI.Page
{
    private string sDataSource = string.Empty;

    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            //if (hdCustomerID.Value != "0")
            //    drpCustomer.SelectedValue = hdCustomerID.Value;
            //if (hdRefNumber.Value != "")
            //    txtRefNo.Text = hdRefNumber.Value;
            //if (hdDueDate.Value.ToString() != "")
            //    txtDueDate.Text = hdDueDate.Value.ToString();
            //if (hdServiceID.Value.ToString() != "")
            //    hdServiceID.Value = hdServiceID.Value.ToString();

            if (!Page.IsPostBack)
            {
                sDataSource = ConfigurationManager.ConnectionStrings[Request.Cookies["Company"].Value].ToString();

                string connStr = string.Empty;

                if (Request.Cookies["Company"] != null)
                    connStr = System.Configuration.ConfigurationManager.ConnectionStrings[Request.Cookies["Company"].Value].ToString();
                else
                    Response.Redirect("~/Login.aspx");

                string dbfileName = connStr.Remove(0, connStr.LastIndexOf(@"App_Data\") + 9);
                dbfileName = dbfileName.Remove(dbfileName.LastIndexOf(";Persist Security Info"));
                BusinessLogic objChk = new BusinessLogic();

                loadBanks(connStr);
                loadCustomerDealers(connStr);

                if (objChk.CheckForOffline(Server.MapPath("Offline\\" + dbfileName + ".offline")))
                {
                    lnkBtnAdd.Visible = false;
                    GrdViewSerVisit.Columns[7].Visible = false;
                }

                //myRangeValidator.MinimumValue = System.DateTime.Now.AddYears(-100).ToShortDateString();
                //myRangeValidator.MaximumValue = System.DateTime.Now.ToShortDateString();

                GrdViewSerVisit.PageSize = 8;


                string connection = Request.Cookies["Company"].Value;
                string usernam = Request.Cookies["LoggedUserName"].Value;
                BusinessLogic bl = new BusinessLogic(sDataSource);

                //if (bl.CheckUserHaveAdd(usernam, "CHQMST"))
                //{
                //    lnkBtnAdd.Enabled = false;
                //    lnkBtnAdd.ToolTip = "You are not allowed to make Add New ";
                //}
                //else
                //{
                //    lnkBtnAdd.Enabled = true;
                //    lnkBtnAdd.ToolTip = "Click to Add New ";
                //}


            }
        }
        catch (Exception ex)
        {
            TroyLiteExceptionManager.HandleException(ex);
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

    protected void UpdateButton_Click(object sender, EventArgs e)
    {
        try
        {
            if (Page.IsValid)
            {
                string connection = ConfigurationManager.ConnectionStrings[Request.Cookies["Company"].Value].ToString();
                int CustomerID = 0;
                int ServiceID = 0;
                DateTime DueDate;
                DateTime VisitDate;
                string AccountNo = string.Empty;
                double Amount = 0.0;
                int PayMode;
                string Visited;
                string CreditCardNo;
                int iBank = 0;
                GridViewRow row = GrdViewSerVisit.SelectedRow;

                int ID = Convert.ToInt32(GrdViewSerVisit.SelectedDataKey.Value);

                string PriceName = string.Empty;
                
                string Username = Request.Cookies["LoggedUserName"].Value;
                PriceName = txtPriceList.Text;
                string Types = "Update";

                BusinessLogic bl = new BusinessLogic(sDataSource);

                //if (bl.IsChequeAlreadyEntered(connection, ddBankID, FromNo, ToNo))
                //{
                //    ScriptManager.RegisterStartupScript(Page, Page.GetType(), Guid.NewGuid().ToString(), "alert('Given Cheque No already entered for this bank.');", true);
                //    ModalPopupExtender1.Show();
                //    return;
                //}

                //if(bl.IsChequeNoNotLess(connection, BankID, FromNo, ToNo))
                //{
                //    ScriptManager.RegisterStartupScript(Page, Page.GetType(), Guid.NewGuid().ToString(), "alert('ToCheque No Cannot be Less than FromChequeNo');", true);
                //    return;
                //}

                //if (Convert.ToDouble(txtFromNoAdd.Text) > Convert.ToDouble(txtToNoAdd.Text))
                //{
                //    ScriptManager.RegisterStartupScript(Page, Page.GetType(), Guid.NewGuid().ToString(), "alert('ToCheque No Cannot be Less than FromChequeNo');", true);
                //    ModalPopupExtender1.Show();
                //    return;
                //}

                //if (Convert.ToDouble(txtFromNoAdd.Text) == Convert.ToDouble(txtToNoAdd.Text))
                //{
                //    ScriptManager.RegisterStartupScript(Page, Page.GetType(), Guid.NewGuid().ToString(), "alert('FromChequeNo Cannot be equal to ToCheque');", true);
                //    ModalPopupExtender1.Show();
                //    return;
                //}

                try
                {
                    bl.UpdatePriceList(connection, ID, PriceName, Username, Types);


                    //MyAccordion.Visible = true;
                    pnlVisitDetails.Visible = false;
                    lnkBtnAdd.Visible = true;
                    Reset();
                    GrdViewSerVisit.DataBind();
                    GrdViewSerVisit.Visible = true;

                    ScriptManager.RegisterStartupScript(Page, Page.GetType(), Guid.NewGuid().ToString(), "alert('Price List Details Updated Successfully.');", true);
                    return;
                }
                catch (Exception ex)
                {
                    TroyLiteExceptionManager.HandleException(ex);
                }

            }
        }
        catch (Exception ex)
        {
            TroyLiteExceptionManager.HandleException(ex);
        }
    }

    private void loadCustomerDealers(string sDataSource)
    {
        BusinessLogic bl = new BusinessLogic(sDataSource);
        DataSet ds = new DataSet();
        ds = bl.ListCustomersDealers(sDataSource);
        //drpCustomer.DataSource = ds;
        //drpCustomer.DataBind();
        //drpCustomer.DataTextField = "LedgerName";
        //drpCustomer.DataValueField = "LedgerID";
    }

    private void loadBanks(string sDataSource)
    {
        //string sDataSource = Server.MapPath(ConfigurationSettings.AppSettings["DataSource"].ToString());
        //BusinessLogic bl = new BusinessLogic(sDataSource);
        //DataSet ds = new DataSet();
        //ds = bl.ListBanks();
        //ddBankName.DataSource = ds;
        //ddBankName.DataBind();
        //ddBankName.DataTextField = "LedgerName";
        //ddBankName.DataValueField = "LedgerID";
    }

    protected void DamageLeaf_Click(object sender, EventArgs e)
    {
        try
        {
            Response.Redirect("DamageCheque.aspx");
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

    protected void UnusedLeaf_Click(object sender, EventArgs e)
    {
        try
        {
            HtmlForm form = new HtmlForm();
            Response.Clear();
            Response.Buffer = true;

            string filename = "UnUsed Cheque Leaf.xls";
            string fLvlValueTemp = string.Empty;
            string tLvlValueTemp = string.Empty;

            DataTable dtf = new DataTable();
            DataColumn dc;
            DataRow drddd;
            DataSet itemDs = new DataSet();
            BusinessLogic bl = new BusinessLogic(GetConnectionString());

            DataSet ds = bl.ListUnusedLeaf(GetConnectionString());

            dc = new DataColumn("ChequeNo");
            dtf.Columns.Add(dc);

            dc = new DataColumn("Bank");
            dtf.Columns.Add(dc);

            itemDs.Tables.Add(dtf);

            DataTable dt = new DataTable();

            dt.Columns.Add(new DataColumn("ChequeNo"));
            dt.Columns.Add(new DataColumn("Bank"));

            if (ds.Tables[0] != null)
            {
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    fLvlValueTemp = dr["FromChequeNo"].ToString().ToUpper().Trim();
                    tLvlValueTemp = dr["ToChequeNo"].ToString().ToUpper().Trim();

                    int difff = Convert.ToInt32(tLvlValueTemp) - Convert.ToInt32(fLvlValueTemp);
                    int g = 0;
                    int ChequeNo = 0;

                    for (int k = 0; k <= difff; k++)
                    {
                        if (g == 0)
                        {
                            drddd = itemDs.Tables[0].NewRow();
                            DataRow dr_final8 = dt.NewRow();
                            dr_final8["ChequeNo"] = fLvlValueTemp;
                            drddd["ChequeNo"] = Convert.ToString(fLvlValueTemp);
                            dr_final8["Bank"] = dr["BankName"];
                            drddd["Bank"] = dr["BankName"];
                            dt.Rows.Add(dr_final8);
                            ChequeNo = Convert.ToInt32(fLvlValueTemp) + 1;
                            itemDs.Tables[0].Rows.Add(drddd);
                        }
                        else
                        {
                            drddd = itemDs.Tables[0].NewRow();
                            DataRow dr_final8 = dt.NewRow();
                            dr_final8["ChequeNo"] = Convert.ToString(ChequeNo);
                            drddd["ChequeNo"] = Convert.ToString(ChequeNo);
                            dr_final8["Bank"] = dr["BankName"];
                            drddd["Bank"] = dr["BankName"];
                            dt.Rows.Add(dr_final8);
                            ChequeNo = ChequeNo + 1;
                            itemDs.Tables[0].Rows.Add(drddd);
                            g = 1;
                        }
                        g = 1;
                    }
                }
            }

            DataSet dsd = bl.ListusedLeaf(GetConnectionString());

            if (dsd != null)
            {
                if (dsd.Tables[0] != null)
                {
                    DataTable dtttt = dsd.Tables[0];

                    if (itemDs.Tables[0] != null)
                    {
                        foreach (DataRow drd in dsd.Tables[0].Rows)
                        {
                            var billNo = Convert.ToUInt32(drd["ChequeNo"]);

                            for (int i = 0; i < itemDs.Tables[0].Rows.Count; i++)
                            {
                                if (billNo == Convert.ToUInt32(itemDs.Tables[0].Rows[i]["ChequeNo"]))
                                {
                                    itemDs.Tables[0].Rows[i].Delete();
                                }
                            }
                        }
                        itemDs.Tables[0].AcceptChanges();
                    }
                }
            }

            DataSet dsddd = bl.ListDamageChequeInfo(GetConnectionString(), "", "");

            if (dsddd != null)
            {
                if (dsddd.Tables[0] != null)
                {
                    DataTable dttttt = dsddd.Tables[0];

                    if (itemDs.Tables[0] != null)
                    {
                        foreach (DataRow drdddd in dsddd.Tables[0].Rows)
                        {
                            var billNo = Convert.ToUInt32(drdddd["ChequeNo"]);

                            for (int i = 0; i < itemDs.Tables[0].Rows.Count; i++)
                            {
                                //var billNoll = Convert.ToUInt32(itemDs.Tables[0].Rows[i]["ChequeNo"]);

                                if (billNo == Convert.ToUInt32(itemDs.Tables[0].Rows[i]["ChequeNo"]))
                                {
                                    itemDs.Tables[0].Rows[i].Delete();
                                }
                            }
                            itemDs.Tables[0].AcceptChanges();
                        }
                        itemDs.Tables[0].AcceptChanges();
                    }
                }
            }

            if (itemDs != null)
            {
                if (itemDs.Tables[0] != null)
                {
                    DataTable dtt = itemDs.Tables[0];

                    if (dtt.Rows.Count > 0)
                    {
                        System.IO.StringWriter tw = new System.IO.StringWriter();
                        System.Web.UI.HtmlTextWriter hw = new System.Web.UI.HtmlTextWriter(tw);

                        DataGrid dgGrid = new DataGrid();
                        dgGrid.DataSource = dtt;
                        dgGrid.DataBind();

                        //Get the HTML for the control.
                        dgGrid.RenderControl(hw);

                        //Write the HTML back to the browser.
                        Response.ContentType = "application/vnd.ms-excel";
                        Response.AppendHeader("Content-Disposition", "attachment; filename=" + filename + "");
                        this.EnableViewState = false;
                        Response.Write(tw.ToString());
                        Response.End();
                    }
                    else
                    {
                        ScriptManager.RegisterStartupScript(Page, Page.GetType(), Guid.NewGuid().ToString(), "alert('No Data Found');", true);
                        return;
                    }
                }
                else
                {
                    ScriptManager.RegisterStartupScript(Page, Page.GetType(), Guid.NewGuid().ToString(), "alert('No Data Found');", true);
                    return;
                }
            }
            else
            {
                ScriptManager.RegisterStartupScript(Page, Page.GetType(), Guid.NewGuid().ToString(), "alert('No Data Found');", true);
                return;
            }
        }
        catch (Exception ex)
        {
            TroyLiteExceptionManager.HandleException(ex);
        }
    }

    public void Reset()
    {
        txtPriceList.Text = "";

    }

    protected void UpdateCancelButton_Click(object sender, EventArgs e)
    {
        try
        {
            //MyAccordion.Visible = true;
            pnlVisitDetails.Visible = false;
            lnkBtnAdd.Visible = true;
            Reset();
            GrdViewSerVisit.Visible = true;
        }
        catch (Exception ex)
        {
            TroyLiteExceptionManager.HandleException(ex);
        }
    }

    protected void GrdViewSerVisit_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        try
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                BusinessLogic bl = new BusinessLogic(GetConnectionString());
                string connection = Request.Cookies["Company"].Value;

                //if (bl.ChequeLeafUsed(int.Parse(((HiddenField)e.Row.FindControl("ldgID")).Value)))
                //{
                //    ((ImageButton)e.Row.FindControl("btnEdit")).Visible = false;
                //    ((ImageButton)e.Row.FindControl("btnEditDisabled")).Visible = true;

                //    ((ImageButton)e.Row.FindControl("lnkB")).Visible = false;
                //    ((ImageButton)e.Row.FindControl("lnkBDisabled")).Visible = true;
                //}

                string usernam = Request.Cookies["LoggedUserName"].Value;

                //if (bl.CheckUserHaveEdit(usernam, "CHQMST"))
                //{
                //    ((ImageButton)e.Row.FindControl("btnEdit")).Visible = false;
                //    ((ImageButton)e.Row.FindControl("btnEditDisabled")).Visible = true;
                //}

                //if (bl.CheckUserHaveDelete(usernam, "CHQMST"))
                //{
                //    ((ImageButton)e.Row.FindControl("lnkB")).Visible = false;
                //    ((ImageButton)e.Row.FindControl("lnkBDisabled")).Visible = true;
                //}
            }
        }
        catch (Exception ex)
        {
            TroyLiteExceptionManager.HandleException(ex);
        }
    }

    protected void GrdViewSerVisit_RowDeleted(object sender, GridViewDeletedEventArgs e)
    {

    }

    protected void GrdViewSerVisit_RowDeleting(object sender, GridViewDeleteEventArgs e)
    {
        try
        {
            GrdViewSerVisit.SelectedIndex = e.RowIndex;
        }
        catch (Exception ex)
        {
            TroyLiteExceptionManager.HandleException(ex);
        }
    }

    protected void GrdViewSerVisit_SelectedIndexChanged(object sender, EventArgs e)
    {
        GridViewRow row = GrdViewSerVisit.SelectedRow;
        try
        {

            int Id = Convert.ToInt32(GrdViewSerVisit.SelectedDataKey.Value);

            sDataSource = ConfigurationManager.ConnectionStrings[Request.Cookies["Company"].Value].ToString();
            BusinessLogic bl = new BusinessLogic(sDataSource);

            DataSet ds = bl.GetPriceListForId(sDataSource, Id);

            if (ds != null)
            {
                hdVisitID.Value = Convert.ToString(Id);

                txtPriceList.Text = ds.Tables[0].Rows[0]["PriceName"].ToString();

                UpdateButton.Visible = true;
                SaveButton.Visible = false;
                CancelButton.Visible = true;
                lnkBtnAdd.Visible = false;
                //MyAccordion.Visible = false;

                GrdViewSerVisit.Visible = false;
                pnlVisitDetails.Visible = true;

                ModalPopupExtender1.Show();
            }
        }
        catch (Exception ex)
        {
            TroyLiteExceptionManager.HandleException(ex);
        }
    }

    protected void GrdViewSerVisit_RowCommand(object sender, GridViewCommandEventArgs e)
    {

    }

    protected void GrdViewSerVisit_RowCreated(object sender, GridViewRowEventArgs e)
    {
        try
        {
            if (e.Row.RowType == DataControlRowType.Pager)
            {
                PresentationUtils.SetPagerButtonStates(GrdViewSerVisit, e.Row, this);
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
            UpdateButton.Visible = false;
            SaveButton.Visible = true;
            //btnSearchService.Enabled = true;
            //drpCustomer.Enabled = true;
            //bankPanel.Update();
            //pnlBank.Visible = false;
            ModalPopupExtender1.Show();
            pnlVisitDetails.Visible = true;
            Reset();
        }
        catch (Exception ex)
        {
            TroyLiteExceptionManager.HandleException(ex);
        }
    }

    protected void btnSearch_Click(object sender, EventArgs e)
    {

    }

    protected void GridSource_Deleting(object sender, ObjectDataSourceMethodEventArgs e)
    {
        try
        {
            if (GrdViewSerVisit.SelectedDataKey != null)
                e.InputParameters["ID"] = GrdViewSerVisit.SelectedDataKey.Value;

            e.InputParameters["Username"] = Request.Cookies["LoggedUserName"].Value;

            e.InputParameters["Types"] = "Delete";
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
            if (Page.IsValid)
            {
                string connection = ConfigurationManager.ConnectionStrings[Request.Cookies["Company"].Value].ToString();
                int CustomerID = 0;
                int ServiceID = 0;
                DateTime DueDate;
                DateTime VisitDate;
                string AccountNo = string.Empty;
                double Amount = 0.0;
                int PayMode;
                string PriceName = string.Empty;
                
                string Username = Request.Cookies["LoggedUserName"].Value;
                
                string Types = "New";
                PriceName = txtPriceList.Text.Trim();
               
                BusinessLogic bl = new BusinessLogic(sDataSource);

                //if (bl.IsChequeAlreadyEntered(connection, BankID, FromNo, ToNo))
                //{
                //    ScriptManager.RegisterStartupScript(Page, Page.GetType(), Guid.NewGuid().ToString(), "alert('Given Cheque No already entered for this bank.');", true);
                //    ModalPopupExtender1.Show();
                //    return;
                //}

                //if(bl.IsChequeNoNotLess(connection, BankID, FromNo, ToNo))
                //{
                //    ScriptManager.RegisterStartupScript(Page, Page.GetType(), Guid.NewGuid().ToString(), "alert('ToCheque No Cannot be Less than FromChequeNo');", true);
                //    return;
                //}

                //if (Convert.ToDouble(txtFromNoAdd.Text) > Convert.ToDouble(txtToNoAdd.Text))
                //{
                //    ScriptManager.RegisterStartupScript(Page, Page.GetType(), Guid.NewGuid().ToString(), "alert('ToCheque No Cannot be Less than FromChequeNo');", true);
                //    ModalPopupExtender1.Show();
                //    return;
                //}

                //if (Convert.ToDouble(txtFromNoAdd.Text) == Convert.ToDouble(txtToNoAdd.Text))
                //{
                //    ScriptManager.RegisterStartupScript(Page, Page.GetType(), Guid.NewGuid().ToString(), "alert('FromChequeNo Cannot be equal to ToCheque');", true);
                //    ModalPopupExtender1.Show();
                //    return;
                //}

                try
                {
                    bl.InsertPriceList(connection, PriceName, Username, Types);

                    //MyAccordion.Visible = true;
                    pnlVisitDetails.Visible = false;
                    lnkBtnAdd.Visible = true;
                    Reset();
                    GrdViewSerVisit.DataBind();
                    GrdViewSerVisit.Visible = true;

                    ScriptManager.RegisterStartupScript(Page, Page.GetType(), Guid.NewGuid().ToString(), "alert('Price List Details Saved Successfully.');", true);
                    return;
                }
                catch (Exception ex)
                {
                    ScriptManager.RegisterStartupScript(Page, Page.GetType(), Guid.NewGuid().ToString(), "alert('Exception Occured: " + ex.Message + "')", true);
                }

            }
        }
        catch (Exception ex)
        {
            TroyLiteExceptionManager.HandleException(ex);
        }
    }
}
