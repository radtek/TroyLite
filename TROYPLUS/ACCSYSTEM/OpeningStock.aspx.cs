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

public partial class OpeningStock : System.Web.UI.Page
{
    private string sDataSource = string.Empty;

    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            sDataSource = ConfigurationManager.ConnectionStrings[Request.Cookies["Company"].Value].ToString();
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
                string connStr = string.Empty;

                if (Request.Cookies["Company"] != null)
                    connStr = System.Configuration.ConfigurationManager.ConnectionStrings[Request.Cookies["Company"].Value].ToString();
                else
                    Response.Redirect("~/Login.aspx");

                string dbfileName = connStr.Remove(0, connStr.LastIndexOf(@"App_Data\") + 9);
                dbfileName = dbfileName.Remove(dbfileName.LastIndexOf(";Persist Security Info"));
                BusinessLogic objChk = new BusinessLogic();

                loadCustomerDealers(connStr);


                LoadProducts(this, null);
                loadCategories();

                //UpdatePanel16.Update();
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

                if (bl.CheckUserHaveAdd(usernam, "OPNSTK"))
                {
                    lnkBtnAdd.Enabled = false;
                    lnkBtnAdd.ToolTip = "You are not allowed to make Add New ";
                }
                else
                {
                    lnkBtnAdd.Enabled = true;
                    lnkBtnAdd.ToolTip = "Click to Add New ";
                }

            }
        }
        catch (Exception ex)
        {
            TroyLiteExceptionManager.HandleException(ex);
        }
    }

    protected void BtnClearFilter_Click(object sender, EventArgs e)
    {
        try
        {
            txtSearch.Text = "";
            ddCriteria.SelectedIndex = 0;
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
                string dmodel = string.Empty;

                int dCurrentStock = 0;
                int dopening = 0;
                int dadjustedStock = 0;
                GridViewRow row = GrdViewSerVisit.SelectedRow;

                string Itemcode = Convert.ToString(GrdViewSerVisit.SelectedDataKey.Value);

                string ditemCode = string.Empty;
                string dProductName = string.Empty;
                string dProductDesc = string.Empty;
                int dcatid = 0;

                string Username = Request.Cookies["LoggedUserName"].Value;

                ditemCode = Convert.ToString(cmbProdAdd.SelectedValue);
                dProductName = cmbProdName.SelectedItem.Text;
                dProductDesc = cmbBrand.SelectedItem.Text;
                dmodel = cmbModel.SelectedItem.Text;
                dcatid = Convert.ToInt32(cmbCategory.SelectedValue);
                dopening = Convert.ToInt32(txtOpeningStock.Text);
                dCurrentStock = Convert.ToInt32(txtCurrentStock.Text);
                dadjustedStock = Convert.ToInt32(txtadjusted.Text);

                string[] sDate;
                DateTime sDueDate;
                string delim = "/";
                char[] delimA = delim.ToCharArray();
                //CultureInfo culture = new CultureInfo("pt-BR");

                sDate = txtDueDate.Text.Trim().Split(delimA);
                sDueDate = new DateTime(Convert.ToInt32(sDate[2].ToString()), Convert.ToInt32(sDate[1].ToString()), Convert.ToInt32(sDate[0].ToString()));
                //dopening = dopening + dadjustedStock;
                BusinessLogic bl = new BusinessLogic(sDataSource);

                if (dadjustedStock == 0)
                {
                    ScriptManager.RegisterStartupScript(Page, Page.GetType(), Guid.NewGuid().ToString(), "alert('Opening stock Adjustment cannot give zero');", true);
                    ModalPopupExtender1.Show();
                    return;
                }

                if ((dopening + dadjustedStock) < 0)
                {
                    ScriptManager.RegisterStartupScript(Page, Page.GetType(), Guid.NewGuid().ToString(), "alert('Opening stock cannot go less than zero');", true);
                    ModalPopupExtender1.Show();
                    return;
                }

                if ((dCurrentStock + dadjustedStock) < 0)
                {
                    ScriptManager.RegisterStartupScript(Page, Page.GetType(), Guid.NewGuid().ToString(), "alert('Current stock cannot go less than zero');", true);
                    ModalPopupExtender1.Show();
                    return;
                }

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
                    double Stock = bl.UpdateOpeningStock(connection, ditemCode, dProductName, dProductDesc, dmodel, dcatid, dopening, dCurrentStock, dadjustedStock, Username, sDueDate);

                    pnlVisitDetails.Visible = false;
                    lnkBtnAdd.Visible = true;
                    Reset();
                    GrdViewSerVisit.DataBind();
                    GrdViewSerVisit.Visible = true;

                    ScriptManager.RegisterStartupScript(Page, Page.GetType(), Guid.NewGuid().ToString(), "alert('Opening Stock Updated Successfully For ItemCode " + ditemCode + " , New Opening Stock is " + dopening + " And New Current Stock is " + Stock + " ');", true);
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

    private string GetConnectionString()
    {
        string connStr = string.Empty;

        if (Request.Cookies["Company"] != null)
            connStr = System.Configuration.ConfigurationManager.ConnectionStrings[Request.Cookies["Company"].Value].ToString();
        else
            Response.Redirect("~/Login.aspx");

        return connStr;
    }

    public void Reset()
    {
        txtOpeningStock.Text = "";
        txtCurrentStock.Text = "";
        txtadjusted.Text = "";

        txtDueDate.Text = "";
        cmbProdAdd.Enabled=true;
        cmbModel.Enabled=true;
        cmbBrand.Enabled=true;
        cmbCategory.Enabled = true;
        cmbProdName.Enabled = true;
        txtDueDate.Enabled = true;

        cmbCategory.SelectedIndex = 0;

        if (cmbProdAdd.Items.Count > 0)
            cmbProdAdd.SelectedIndex = 0;

        if (cmbModel.Items.Count > 0)
            cmbModel.SelectedIndex = 0;

        if (cmbBrand.Items.Count > 0)
            cmbBrand.SelectedIndex = 0;

        if (cmbProdName.Items.Count > 0)
            cmbProdName.SelectedIndex = 0;

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

                if (bl.CheckUserHaveEdit(usernam, "OPNSTK"))
                {
                    ((ImageButton)e.Row.FindControl("btnEdit")).Visible = false;
                    ((ImageButton)e.Row.FindControl("btnEditDisabled")).Visible = true;
                }

                if (bl.CheckUserHaveDelete(usernam, "OPNSTK"))
                {
                    ((ImageButton)e.Row.FindControl("lnkB")).Visible = false;
                    ((ImageButton)e.Row.FindControl("lnkBDisabled")).Visible = true;
                }

            }
        }
        catch (Exception ex)
        {
            TroyLiteExceptionManager.HandleException(ex);
        }
    }

    private void loadCategories()
    {
        //string sDataSource = Server.MapPath(ConfigurationSettings.AppSettings["DataSource"].ToString());
        BusinessLogic bl = new BusinessLogic();
        DataSet ds = new DataSet();
        ds = bl.ListCategory(sDataSource,"");
        cmbCategory.DataTextField = "CategoryName";
        cmbCategory.DataValueField = "CategoryID";
        cmbCategory.DataSource = ds;
        cmbCategory.DataBind();
    }

    protected void LoadProducts(object sender, EventArgs e)
    {
        sDataSource = ConfigurationManager.ConnectionStrings[Request.Cookies["Company"].Value].ToString();
        string CategoryID = cmbCategory.SelectedValue;
        BusinessLogic bl = new BusinessLogic(sDataSource);
        DataSet ds = new DataSet();
        ds = bl.ListProductsForCategoryID(CategoryID,"");
        cmbProdAdd.Items.Clear();
        cmbProdAdd.DataSource = ds;
        cmbProdAdd.Items.Insert(0, new ListItem("Select Product Code", "0"));
        cmbProdAdd.DataTextField = "ItemCode";
        cmbProdAdd.DataValueField = "ItemCode";
        cmbProdAdd.DataBind();

        ds = bl.ListModelsForCategoryID(CategoryID, "");
        cmbModel.Items.Clear();
        cmbModel.DataSource = ds;
        cmbModel.Items.Insert(0, new ListItem("Select Model", "0"));
        cmbModel.DataTextField = "Model";
        cmbModel.DataValueField = "Model";
        cmbModel.DataBind();

        ds = bl.ListBrandsForCategoryID(CategoryID, "");
        cmbBrand.Items.Clear();
        cmbBrand.DataSource = ds;
        cmbBrand.Items.Insert(0, new ListItem("Select Brand", "0"));
        cmbBrand.DataTextField = "ProductDesc";
        cmbBrand.DataValueField = "ProductDesc";
        cmbBrand.DataBind();

        ds = bl.ListProdNameForCategoryID(CategoryID, "");
        cmbProdName.Items.Clear();
        cmbProdName.DataSource = ds;
        cmbProdName.Items.Insert(0, new ListItem("Select Product Name", "0"));
        cmbProdName.DataTextField = "ProductName";
        cmbProdName.DataValueField = "ProductName";
        cmbProdName.DataBind();

        LoadForProduct(this, null);

        
    }

    protected void LoadForBrand(object sender, EventArgs e)
    {
        sDataSource = ConfigurationManager.ConnectionStrings[Request.Cookies["Company"].Value].ToString();
        BusinessLogic bl = new BusinessLogic(sDataSource);
        string brand = cmbBrand.SelectedValue;
        string CategoryID = cmbCategory.SelectedValue;
        //DataSet catData = bl.GetProductForId(sDataSource, itemCode);
        //cmbProdAdd.SelectedValue = itemCode;
        //cmbModel.SelectedValue = itemCode;
        DataSet ds = new DataSet();
        ds = bl.ListModelsForBrand(brand, CategoryID, "");
        if (ds != null && ds.Tables[0].Rows.Count > 0)
        {
            cmbModel.Items.Clear();
            cmbModel.DataSource = ds;
            cmbModel.DataTextField = "Model";
            cmbModel.DataValueField = "Model";
            cmbModel.DataBind();
        }

        ds = bl.ListProdcutsForBrand(brand, CategoryID, "");
        if (ds != null && ds.Tables[0].Rows.Count > 0)
        {
            cmbProdAdd.Items.Clear();
            cmbProdAdd.DataSource = ds;
            cmbProdAdd.DataTextField = "ItemCode";
            cmbProdAdd.DataValueField = "ItemCode";
            cmbProdAdd.DataBind();
        }

        ds = bl.ListProdcutNameForBrand(brand, CategoryID, "");
        if (ds != null && ds.Tables[0].Rows.Count > 0)
        {
            cmbProdName.Items.Clear();
            cmbProdName.DataSource = ds;
            cmbProdName.DataTextField = "ProductName";
            cmbProdName.DataValueField = "ProductName";
            cmbProdName.DataBind();
        }
        cmbProdAdd_SelectedIndexChanged(this, null);

    }

    protected void LoadForModel(object sender, EventArgs e)
    {
        sDataSource = ConfigurationManager.ConnectionStrings[Request.Cookies["Company"].Value].ToString();
        BusinessLogic bl = new BusinessLogic(sDataSource);
        string model = cmbModel.SelectedValue;
        string CategoryID = cmbCategory.SelectedValue;
        DataSet ds = new DataSet();

        ds = bl.ListProdcutsForModel(model, CategoryID, "");
        if (ds != null && ds.Tables[0].Rows.Count > 0)
        {
            cmbProdAdd.Items.Clear();
            cmbProdAdd.DataSource = ds;
            cmbProdAdd.DataTextField = "ItemCode";
            cmbProdAdd.DataValueField = "ItemCode";
            cmbProdAdd.DataBind();
        }

        ds = bl.ListBrandsForModel(model, CategoryID, "");
        if (ds != null && ds.Tables[0].Rows.Count > 0)
        {
            cmbBrand.Items.Clear();
            cmbBrand.DataSource = ds;
            cmbBrand.DataTextField = "ProductDesc";
            cmbBrand.DataValueField = "ProductDesc";
            cmbBrand.DataBind();
        }

        ds = bl.ListProductNameForModel(model, CategoryID, "");
        if (ds != null && ds.Tables[0].Rows.Count > 0)
        {
            cmbProdName.Items.Clear();
            cmbProdName.DataSource = ds;
            cmbProdName.DataTextField = "ProductName";
            cmbProdName.DataValueField = "ProductName";
            cmbProdName.DataBind();
        }
        cmbProdAdd_SelectedIndexChanged(this, null);
    }

    protected void LoadForProductName(object sender, EventArgs e)
    {
        sDataSource = ConfigurationManager.ConnectionStrings[Request.Cookies["Company"].Value].ToString();
        BusinessLogic bl = new BusinessLogic(sDataSource);
        string prodName = cmbProdName.SelectedValue;
        string CategoryID = cmbCategory.SelectedValue;
        DataSet ds = new DataSet();

        ds = bl.ListProdcutsForProductName(prodName, CategoryID, "");

        if (ds != null && ds.Tables[0].Rows.Count > 0)
        {
            cmbProdAdd.Items.Clear();
            cmbProdAdd.DataSource = ds;
            cmbProdAdd.DataTextField = "ItemCode";
            cmbProdAdd.DataValueField = "ItemCode";
            cmbProdAdd.DataBind();
        }

        ds = bl.ListBrandsForProductName(prodName, CategoryID, "");
        if (ds != null && ds.Tables[0].Rows.Count > 0)
        {
            cmbBrand.Items.Clear();
            cmbBrand.DataSource = ds;
            cmbBrand.DataTextField = "ProductDesc";
            cmbBrand.DataValueField = "ProductDesc";
            cmbBrand.DataBind();
        }

        ds = bl.ListModelsForProductName(prodName, CategoryID, "");
        if (ds != null && ds.Tables[0].Rows.Count > 0)
        {
            cmbModel.Items.Clear();
            cmbModel.DataSource = ds;
            cmbModel.DataTextField = "Model";
            cmbModel.DataValueField = "Model";
            cmbModel.DataBind();
        }
        cmbProdAdd_SelectedIndexChanged(this, null);
    }

    protected void cmbProdAdd_SelectedIndexChanged(object sender, EventArgs e)
    {
        try
        {
            sDataSource = ConfigurationManager.ConnectionStrings[Request.Cookies["Company"].Value].ToString();
            BusinessLogic bl = new BusinessLogic(sDataSource);
            DataSet ds = new DataSet();
            DataSet roleDs = new DataSet();
            string itemCode = string.Empty;
            DataSet checkDs;

            if (cmbProdAdd.SelectedItem != null)
            {
                itemCode = cmbProdAdd.SelectedItem.Value.Trim();

                DataSet catData = bl.GetProductForId(sDataSource, cmbProdAdd.SelectedItem.Value.Trim());

                if (catData != null)
                {
                    if ((catData.Tables[0].Rows[0]["Model"] != null) && (catData.Tables[0].Rows[0]["Model"].ToString() != ""))
                    {
                        if (cmbModel.Items.FindByValue(catData.Tables[0].Rows[0]["Model"].ToString().Trim()) != null)
                        {
                            cmbModel.ClearSelection();
                            if (!cmbModel.Items.FindByValue(catData.Tables[0].Rows[0]["Model"].ToString().Trim()).Selected)
                                cmbModel.Items.FindByValue(catData.Tables[0].Rows[0]["Model"].ToString().Trim()).Selected = true;

                        }
                    }

                    if ((catData.Tables[0].Rows[0]["ProductDesc"] != null) && (catData.Tables[0].Rows[0]["ProductDesc"].ToString() != ""))
                    {
                        if (cmbBrand.Items.FindByValue(catData.Tables[0].Rows[0]["ProductDesc"].ToString().Trim()) != null)
                        {
                            cmbBrand.ClearSelection();
                            if (!cmbBrand.Items.FindByValue(catData.Tables[0].Rows[0]["ProductDesc"].ToString().Trim()).Selected)
                                cmbBrand.Items.FindByValue(catData.Tables[0].Rows[0]["ProductDesc"].ToString().Trim()).Selected = true;
                        }
                    }

                    if ((catData.Tables[0].Rows[0]["ProductName"] != null) && (catData.Tables[0].Rows[0]["ProductName"].ToString() != ""))
                    {
                        if (cmbProdName.Items.FindByValue(catData.Tables[0].Rows[0]["ProductName"].ToString().Trim()) != null)
                        {
                            cmbProdName.ClearSelection();
                            if (!cmbProdName.Items.FindByValue(catData.Tables[0].Rows[0]["ProductName"].ToString().Trim()).Selected)
                                cmbProdName.Items.FindByValue(catData.Tables[0].Rows[0]["ProductName"].ToString().Trim()).Selected = true;
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

    protected void LoadForProduct(object sender, EventArgs e)
    {
        //string itemCode = cmbProdAdd.SelectedValue;
        //cmbModel.SelectedValue = itemCode;
        //cmbBrand.SelectedValue = itemCode;
        cmbProdAdd_SelectedIndexChanged(sender, e);
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
        try
        {
            rowstk.Visible = true;

            string strItemCode = string.Empty;

            GridViewRow row = GrdViewSerVisit.SelectedRow;

            txtOpeningStock.Enabled = false;

            string ItemCode = Convert.ToString(GrdViewSerVisit.SelectedDataKey.Value);

            sDataSource = ConfigurationManager.ConnectionStrings[Request.Cookies["Company"].Value].ToString();
            BusinessLogic bl = new BusinessLogic(sDataSource);

            strItemCode = row.Cells[0].Text.Replace("&quot;", "\"").Trim();

            DataSet ds = bl.GetOpeningStockForItemcode(sDataSource, strItemCode);

            if (ds != null)
            {
                if (ds.Tables[0].Rows.Count > 0)
                {
                    cmbProdAdd.ClearSelection();
                    ListItem li = cmbProdAdd.Items.FindByValue(System.Web.HttpUtility.HtmlDecode(strItemCode.Trim()));
                    if (li != null) li.Selected = true;
                    cmbProdAdd.Enabled = false;

                    DataSet catData = bl.GetProductForId(sDataSource, strItemCode);
                    if (catData != null)
                    {
                        cmbCategory.SelectedValue = catData.Tables[0].Rows[0]["CategoryID"].ToString();
                        cmbModel.Enabled = false;
                        cmbBrand.Enabled = false;
                        cmbProdName.Enabled = false;
                        cmbCategory.Enabled = false;
                        LoadProducts(this, null);
                        txtDueDate.Enabled = false;
                    }

                    if (ds.Tables[0].Rows[0]["DueDate"] != null)
                        txtDueDate.Text = Convert.ToDateTime(ds.Tables[0].Rows[0]["DueDate"].ToString()).ToString("dd/MM/yyyy");

                    if ((catData.Tables[0].Rows[0]["ItemCode"] != null) && (catData.Tables[0].Rows[0]["ItemCode"].ToString() != ""))
                    {
                        if (cmbProdAdd.Items.FindByValue(catData.Tables[0].Rows[0]["ItemCode"].ToString().Trim()) != null)
                        {
                            cmbProdAdd.ClearSelection();
                            cmbProdAdd.Items.FindByValue(catData.Tables[0].Rows[0]["ItemCode"].ToString().Trim()).Selected = true;
                        }
                    }

                    if ((catData.Tables[0].Rows[0]["ProductDesc"] != null) && (catData.Tables[0].Rows[0]["ProductDesc"].ToString() != ""))
                    {
                        if (cmbBrand.Items.FindByValue(catData.Tables[0].Rows[0]["ProductDesc"].ToString().Trim()) != null)
                        {
                            cmbBrand.ClearSelection();
                            if (!cmbBrand.Items.FindByValue(catData.Tables[0].Rows[0]["ProductDesc"].ToString().Trim()).Selected)
                                cmbBrand.Items.FindByValue(catData.Tables[0].Rows[0]["ProductDesc"].ToString().Trim()).Selected = true;
                        }
                    }

                    if ((catData.Tables[0].Rows[0]["ProductName"] != null) && (catData.Tables[0].Rows[0]["ProductName"].ToString() != ""))
                    {
                        if (cmbProdName.Items.FindByValue(catData.Tables[0].Rows[0]["ProductName"].ToString().Trim()) != null)
                        {
                            cmbProdName.ClearSelection();
                            if (!cmbProdName.Items.FindByValue(catData.Tables[0].Rows[0]["ProductName"].ToString().Trim()).Selected)
                                cmbProdName.Items.FindByValue(catData.Tables[0].Rows[0]["ProductName"].ToString().Trim()).Selected = true;
                        }
                    }

                    if ((catData.Tables[0].Rows[0]["Model"] != null) && (catData.Tables[0].Rows[0]["Model"].ToString() != ""))
                    {
                        if (cmbModel.Items.FindByValue(catData.Tables[0].Rows[0]["Model"].ToString().Trim()) != null)
                        {
                            cmbModel.ClearSelection();
                            if (!cmbModel.Items.FindByValue(catData.Tables[0].Rows[0]["Model"].ToString().Trim()).Selected)
                                cmbModel.Items.FindByValue(catData.Tables[0].Rows[0]["Model"].ToString().Trim()).Selected = true;
                        }
                    }

                    txtOpeningStock.Text = row.Cells[5].Text;
                    txtCurrentStock.Text = row.Cells[6].Text;
                    txtadjusted.Focus();

                    UpdateButton.Visible = true;
                    SaveButton.Visible = false;
                    CancelButton.Visible = true;
                    lnkBtnAdd.Visible = false;
                    GrdViewSerVisit.Visible = false;
                    pnlVisitDetails.Visible = true;

                    ModalPopupExtender1.Show();
                }
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
            //UpdatePanel16.Update();
            rowstk.Visible = false;
            txtOpeningStock.Enabled = true;
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
                e.InputParameters["ItemCode"] = GrdViewSerVisit.SelectedDataKey.Value;

            e.InputParameters["Username"] = Request.Cookies["LoggedUserName"].Value;
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
                int dcatid = 0;
                string AccountNo = string.Empty;
                int dopening = 0;
                string ditemCode = string.Empty;
                string dProductName = string.Empty;
                string dProductDesc = string.Empty;
                string dmodel = string.Empty;

                BusinessLogic bl = new BusinessLogic(sDataSource);

                string[] sDate;
                DateTime sDueDate;
                string delim = "/";
                char[] delimA = delim.ToCharArray();
                //CultureInfo culture = new CultureInfo("pt-BR");

                string Username = Request.Cookies["LoggedUserName"].Value;
                ditemCode = Convert.ToString(cmbProdAdd.SelectedValue);
                dProductName = cmbProdName.SelectedItem.Text;
                dProductDesc = cmbBrand.SelectedItem.Text;
                dmodel = cmbModel.SelectedItem.Text;
                dcatid = Convert.ToInt32(cmbCategory.SelectedValue);
                dopening = Convert.ToInt32(txtOpeningStock.Text);
                sDate = txtDueDate.Text.Trim().Split(delimA);
                sDueDate = new DateTime(Convert.ToInt32(sDate[2].ToString()), Convert.ToInt32(sDate[1].ToString()), Convert.ToInt32(sDate[0].ToString()));
                
                if (bl.IsItemAlreadyInOpening(connection, ditemCode))
                {
                    ScriptManager.RegisterStartupScript(Page, Page.GetType(), Guid.NewGuid().ToString(), "alert('Product code " + ditemCode + " already added in opening stock');", true);
                    ModalPopupExtender1.Show();
                    return;
                }

                if (dopening < 0)
                {
                    ScriptManager.RegisterStartupScript(Page, Page.GetType(), Guid.NewGuid().ToString(), "alert('Opening stock cannot be negative value');", true);
                    ModalPopupExtender1.Show();
                    return;
                }

                if (dopening == 0)
                {
                    ScriptManager.RegisterStartupScript(Page, Page.GetType(), Guid.NewGuid().ToString(), "alert('Opening stock cannot be zero');", true);
                    ModalPopupExtender1.Show();
                    return;
                }

                try
                {
                    double Stock = bl.InsertOpeningStock(connection, ditemCode, dProductName, dProductDesc, dmodel, dcatid, dopening, Username, sDueDate);

                    pnlVisitDetails.Visible = false;
                    lnkBtnAdd.Visible = true;
                    Reset();
                    GrdViewSerVisit.DataBind();
                    GrdViewSerVisit.Visible = true;

                    ScriptManager.RegisterStartupScript(Page, Page.GetType(), Guid.NewGuid().ToString(), "alert('Opening Stock Added Successfully For ItemCode " + ditemCode + " , Opening Stock is " + dopening + " And Current Stock is " + Stock + " ');", true);
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
