using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Text;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.IO;
using System.Xml;
using SMSLibrary;
using System.Collections.Generic;

public partial class LeadMgmt : System.Web.UI.Page
{
    public string sDataSource = string.Empty;
    string dbfileName = string.Empty;

    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            sDataSource = ConfigurationManager.ConnectionStrings[Request.Cookies["Company"].Value].ToString();
            dbfileName = sDataSource.Remove(0, sDataSource.LastIndexOf(@"App_Data\") + 9);
            dbfileName = dbfileName.Remove(dbfileName.LastIndexOf(";Persist Security Info"));

            if (!Page.IsPostBack)
            {
                //myRangeValidator.MinimumValue = System.DateTime.Now.AddYears(-100).ToShortDateString();
                //myRangeValidator.MaximumValue = System.DateTime.Now.ToShortDateString();

                BindDropdownList();
                Session["contactDs"] = null;
                Session["CompetitorDs"] = null;
                Session["ActivityDs"] = null;
                Session["ProductDs"] = null;
                Session["Date"] = null;
                loadDropDowns();
                loadStages();
                loadPotentialAmount();
                loadEmp();
                GrdViewLead.PageSize = 8;

                string connection = Request.Cookies["Company"].Value;
                string usernam = Request.Cookies["LoggedUserName"].Value;
                BusinessLogic bl = new BusinessLogic(sDataSource);

                if (bl.CheckUserHaveAdd(usernam, "LDMNGT"))
                {
                    lnkBtnAdd.Enabled = false;
                    lnkBtnAdd.ToolTip = "You are not allowed to make Add New ";
                }
                else
                {
                    lnkBtnAdd.Enabled = true;
                    lnkBtnAdd.ToolTip = "Click to Add New ";
                }
                BindGrid("Open", "DocStatus");
                drpLeadStatus.Enabled = true;
                drpStatus.Enabled = true;
                loadInformation3();
                loadInformation4();
                loadBusinessType();
                loadCategory();
                loadArea();
                loadInterestlevel();

            }
        }
        catch (Exception ex)
        {
            TroyLiteExceptionManager.HandleException(ex);
        }
    }

    private void loadInformation3()
    {
        BusinessLogic bl = new BusinessLogic(sDataSource);
        DataSet ds = new DataSet();
        string connection = ConfigurationManager.ConnectionStrings[Request.Cookies["Company"].Value].ToString();

        drpInformation3.Items.Clear();
        drpInformation3.Items.Add(new ListItem("Select Information 3", "0"));
        ds = bl.ListInformation3();
        drpInformation3.DataSource = ds;
        drpInformation3.DataBind();
        drpInformation3.DataTextField = "TextValue";
        drpInformation3.DataValueField = "ID";
    }

    private void loadInformation4()
    {
        BusinessLogic bl = new BusinessLogic(sDataSource);
        DataSet ds = new DataSet();
        string connection = ConfigurationManager.ConnectionStrings[Request.Cookies["Company"].Value].ToString();

        drpInformation4.Items.Clear();
        drpInformation4.Items.Add(new ListItem("Select Information 4", "0"));
        ds = bl.ListInformation4();
        drpInformation4.DataSource = ds;
        drpInformation4.DataBind();
        drpInformation4.DataTextField = "TextValue";
        drpInformation4.DataValueField = "ID";
    }

    private void loadBusinessType()
    {
        BusinessLogic bl = new BusinessLogic(sDataSource);
        DataSet ds = new DataSet();
        string connection = ConfigurationManager.ConnectionStrings[Request.Cookies["Company"].Value].ToString();

        drpBusinessType.Items.Clear();
        drpBusinessType.Items.Add(new ListItem("Select Business Type", "0"));
        ds = bl.ListBusinessType();
        drpBusinessType.DataSource = ds;
        drpBusinessType.DataBind();
        drpBusinessType.DataTextField = "TextValue";
        drpBusinessType.DataValueField = "ID";
    }

    private void loadCategory()
    {
        BusinessLogic bl = new BusinessLogic(sDataSource);
        DataSet ds = new DataSet();
        string connection = ConfigurationManager.ConnectionStrings[Request.Cookies["Company"].Value].ToString();

        drpCategory.Items.Clear();
        drpCategory.Items.Add(new ListItem("Select Category", "0"));
        ds = bl.ListCategory();
        drpCategory.DataSource = ds;
        drpCategory.DataBind();
        drpCategory.DataTextField = "TextValue";
        drpCategory.DataValueField = "ID";
    }

    private void loadArea()
    {
        BusinessLogic bl = new BusinessLogic(sDataSource);
        DataSet ds = new DataSet();
        string connection = ConfigurationManager.ConnectionStrings[Request.Cookies["Company"].Value].ToString();

        drpArea.Items.Clear();
        drpArea.Items.Add(new ListItem("Select Area", "0"));
        ds = bl.ListArea();
        drpArea.DataSource = ds;
        drpArea.DataBind();
        drpArea.DataTextField = "TextValue";
        drpArea.DataValueField = "ID";
    }

    private void loadInterestlevel()
    {
        BusinessLogic bl = new BusinessLogic(sDataSource);
        DataSet ds = new DataSet();
        string connection = ConfigurationManager.ConnectionStrings[Request.Cookies["Company"].Value].ToString();

        drpIntLevel.Items.Clear();
        drpIntLevel.Items.Add(new ListItem("Select Interest level", "0"));
        ds = bl.ListInterestLevel();
        drpIntLevel.DataSource = ds;
        drpIntLevel.DataBind();
        drpIntLevel.DataTextField = "TextValue";
        drpIntLevel.DataValueField = "ID";
    }

    protected void ComboBox2_SelectedIndexChanged(object sender, EventArgs e)
    {
        try
        {
            //if (ComboBox2.SelectedValue == "2")
            //{
            //    rowcall.Visible = true;
            //    ModalPopupContact.Show();
            //}
            //else
            //{
            //    rowcall.Visible = false;
            //    ModalPopupContact.Show();
            //}
        }
        catch (Exception ex)
        {
            TroyLiteExceptionManager.HandleException(ex);
        }
    }

    protected void cmbCustomer_SelectedIndexChanged(object sender, EventArgs e)
    {
        try
        {
            BusinessLogic bl = new BusinessLogic(sDataSource);
            int iLedgerID = Convert.ToInt32(cmbCustomer.SelectedItem.Value);
            DataSet customerDs = bl.getAddressInfo(iLedgerID);

            if (customerDs != null && customerDs.Tables[0].Rows.Count > 0)
            {
                if (customerDs.Tables[0].Rows[0]["Add1"] != null)
                    txtAddress.Text = customerDs.Tables[0].Rows[0]["Add1"].ToString();

                //if (customerDs.Tables[0].Rows[0]["ContactName"] != null)
                //    txtContactName.Text =  customerDs.Tables[0].Rows[0]["ContactName"].ToString();

                if (customerDs.Tables[0].Rows[0]["phone"] != null)
                    txtTelephone.Text = customerDs.Tables[0].Rows[0]["phone"].ToString();

                if (customerDs.Tables[0].Rows[0]["Mobile"] != null)
                {
                    txtMobile.Text = Convert.ToString(customerDs.Tables[0].Rows[0]["Mobile"]);
                }
            }
            else
            {
                txtAddress.Text = string.Empty;
                // txtContactName.Text = string.Empty;
                txtTelephone.Text = string.Empty;
                txtMobile.Text = string.Empty;
            }

            DataSet Ds = bl.getSalesForId(iLedgerID);
            if (customerDs != null && customerDs.Tables[0].Rows.Count > 0)
            {
                // txtTotalAmount.Text = Ds.Tables[0].Rows[0]["rate"].ToString();
            }
            else
            {
                // txtTotalAmount.Text = "0";
            }
        }
        catch (Exception ex)
        {
            TroyLiteExceptionManager.HandleException(ex);
        }

    }

    private void loadDropDowns()
    {
        //string sDataSource = Server.MapPath(ConfigurationSettings.AppSettings["DataSource"].ToString());
        BusinessLogic bl = new BusinessLogic(sDataSource);
        DataSet ds = new DataSet();
        ds = bl.ListCreditorDebitor(sDataSource);
        cmbCustomer.DataSource = ds;
        cmbCustomer.DataBind();
        cmbCustomer.DataTextField = "LedgerName";
        cmbCustomer.DataValueField = "LedgerID";


    }

    private void loadStages()
    {
        BusinessLogic bl = new BusinessLogic(sDataSource);
        DataSet dsd = new DataSet();

        // drpStageName.Items.Clear();
        //drpStageName.Items.Add(new ListItem("Select Stage Name", "0"));
        dsd = bl.ListStagesSetup(sDataSource, "N", 0);
        //drpStageName.DataSource = dsd;
        //drpStageName.DataBind();
        //drpStageName.DataTextField = "Stage_Name";
        //drpStageName.DataValueField = "Stage_Setup_Id";
    }

    private void loadPotentialAmount()
    {
        BusinessLogic bl = new BusinessLogic(sDataSource);
        DataSet dsd = new DataSet();

        //txtStagePotentialAmount.Items.Clear();
        //txtStagePotentialAmount.Items.Add(new ListItem("Select Potential Amount", "0"));
        //dsd = bl.ListPotentialAmount(sDataSource, "N", 0);
        //txtStagePotentialAmount.DataSource = dsd;
        //txtStagePotentialAmount.DataBind();
        //txtStagePotentialAmount.DataTextField = "Potential_Amount";
        //txtStagePotentialAmount.DataValueField = "Potential_Amount";
    }

    private void loadproduct()
    {
        BusinessLogic bl = new BusinessLogic(sDataSource);
        DataSet dsd = new DataSet();

        drpproduct.Items.Clear();
        drpproduct.Items.Add(new ListItem("Select Product", "0"));
        dsd = bl.ListProducts(sDataSource, "", "");
        drpproduct.DataSource = dsd;
        drpproduct.DataBind();
        drpproduct.DataTextField = "ProductName";
        drpproduct.DataValueField = "ItemCode";
    }

    private void loadActivities()
    {
        BusinessLogic bl = new BusinessLogic(sDataSource);
        DataSet dsd = new DataSet();
        DataSet ds = new DataSet();

        LeadBusinessLogic bll = new LeadBusinessLogic(sDataSource);


        drpActivityName.Items.Clear();
        drpActivityName.Items.Add(new ListItem("Select Activity Name", "0"));
        //dsd = bl.ListActivitySetup(sDataSource, "N", 0);

        dsd = bll.GetDropdownList(sDataSource, "ACTIVITY");

        drpActivityName.DataSource = dsd;
        drpActivityName.DataBind();
        //OLD code
        //drpActivityName.DataTextField = "Activity_Name";
        //drpActivityName.DataValueField = "Activity_Setup_Id";
        //New code
        drpActivityName.DataTextField = "TextValue";
        drpActivityName.DataValueField = "TextValue";

        drpNextActivity.Items.Clear();
        drpNextActivity.Items.Add(new ListItem("Select Next Activity", "0"));
        drpNextActivity.DataSource = dsd;
        drpNextActivity.DataBind();
        drpNextActivity.DataTextField = "TextValue";
        drpNextActivity.DataValueField = "TextValue";

        drpActivityEmployee.Items.Clear();
        drpActivityEmployee.Items.Add(new ListItem("Select Employee", "0"));
        ds = bl.ListExecutive();
        drpActivityEmployee.DataSource = ds;
        drpActivityEmployee.DataBind();
        drpActivityEmployee.DataTextField = "empFirstName";
        drpActivityEmployee.DataValueField = "empno";

    }

    protected void drpStageName_SelectedIndexChanged(object sender, EventArgs e)
    {
        try
        {
            BusinessLogic bl = new BusinessLogic(sDataSource);
            DataSet dsd = new DataSet();
            // int iStageID = Convert.ToInt32(drpStageName.SelectedItem.Value);

            //dsd = bl.ListStagesSetup(sDataSource, "Y", iStageID);

            if (dsd != null && dsd.Tables[0].Rows.Count > 0)
            {
                //if (dsd.Tables[0].Rows[0]["Stage_Perc"] != null)
                //txtStagePerc.Text = dsd.Tables[0].Rows[0]["Stage_Perc"].ToString();
            }
            //UpdatePanel1.Update();

            // double calculation = (Convert.ToDouble(txtStagePotentialAmount.Text) * Convert.ToDouble(txtStagePerc.Text)) / 100;
            // txtStageWeightedAmount.Text = Convert.ToString(calculation);
            // UpdatePanel8.Update();
        }
        catch (Exception ex)
        {
            TroyLiteExceptionManager.HandleException(ex);
        }

    }

    protected void txtStagePotentialAmount_TextChanged(object sender, EventArgs e)
    {
        try
        {
            // double calculation = (Convert.ToDouble(txtStagePotentialAmount.Text) * Convert.ToDouble(txtStagePerc.Text)) / 100;
            // txtStageWeightedAmount.Text = Convert.ToString(calculation);
            // UpdatePanel8.Update();
        }
        catch (Exception ex)
        {
            TroyLiteExceptionManager.HandleException(ex);
        }

    }

    protected void drpPredictedClosingPeriod_SelectedIndexChanged(object sender, EventArgs e)
    {
        try
        {
            DateTime date = Convert.ToDateTime(txtCreationDate.Text);

            //string dtaa = Convert.ToDateTime(date).ToString("dd/MM/yyyy");              
            //DateTime calculat = Convert.ToDateTime(dtaa);

            //if (drpPredictedClosingPeriod.SelectedValue == "Days")
            //{
            //    int days = Convert.ToInt32(txtPredictedClosing.Text);

            //    DateTime dat = date.AddDays(days);
            //    txtPredictedClosingDate.Text = dat.ToString("dd/MM/yyyy");
            //}
            //else if (drpPredictedClosingPeriod.SelectedValue == "Months")
            //{
            //    int Months = Convert.ToInt32(txtPredictedClosing.Text);

            //    DateTime dat = date.AddMonths(Months);
            //    txtPredictedClosingDate.Text = dat.ToString("dd/MM/yyyy");
            //}
            //else if (drpPredictedClosingPeriod.SelectedValue == "Weeks")
            //{
            //    int Weeks = Convert.ToInt32(txtPredictedClosing.Text);
            //    Weeks = 7 * Weeks;
            //    DateTime dat = date.AddDays(Weeks);
            //    txtPredictedClosingDate.Text = dat.ToString("dd/MM/yyyy");
            //}

            //UpdatePanel123.Update();
        }
        catch (Exception ex)
        {
            TroyLiteExceptionManager.HandleException(ex);
        }

    }

    protected void drpStatus_SelectedIndexChanged(object sender, EventArgs e)
    {
        try
        {
            if (drpStatus.SelectedValue == "Closed")
            {
                DateTime indianStd = TimeZoneInfo.ConvertTimeBySystemTimeZoneId(DateTime.Now, "India Standard Time");
                string dtaa = Convert.ToDateTime(indianStd).ToString("dd/MM/yyyy");

                txtClosingDate.Text = dtaa;
            }
        }
        catch (Exception ex)
        {
            TroyLiteExceptionManager.HandleException(ex);
        }

    }

    protected void txtStagePerc_TextChanged(object sender, EventArgs e)
    {
        try
        {
            // double calculation = (Convert.ToDouble(txtStagePotentialAmount.Text) * Convert.ToDouble(txtStagePerc.Text)) /100;
            // txtStageWeightedAmount.Text = Convert.ToString(calculation);
            // UpdatePanel8.Update();
        }
        catch (Exception ex)
        {
            TroyLiteExceptionManager.HandleException(ex);
        }
    }

    protected void txtPredictedClosing_TextChanged(object sender, EventArgs e)
    {
        try
        {
            DateTime date = Convert.ToDateTime(txtCreationDate.Text);

            //string dtaa = Convert.ToDateTime(date).ToString("dd/MM/yyyy");              
            //DateTime calculat = Convert.ToDateTime(dtaa);

            //if(drpPredictedClosingPeriod.SelectedValue == "Days")
            //{
            //    int days = Convert.ToInt32(txtPredictedClosing.Text);

            //    DateTime dat = date.AddDays(days);
            //    txtPredictedClosingDate.Text = dat.ToString("dd/MM/yyyy");
            //}
            //else if (drpPredictedClosingPeriod.SelectedValue == "Months")
            //{
            //    int Months = Convert.ToInt32(txtPredictedClosing.Text);

            //    DateTime dat = date.AddMonths(Months);
            //    txtPredictedClosingDate.Text = dat.ToString("dd/MM/yyyy");
            //}
            //else if (drpPredictedClosingPeriod.SelectedValue == "Weeks")
            //{
            //    int Weeks = Convert.ToInt32(txtPredictedClosing.Text);
            //    Weeks = 7 * Weeks;
            //    DateTime dat = date.AddDays(Weeks);
            //    txtPredictedClosingDate.Text = dat.ToString("dd/MM/yyyy");
            //}


            //UpdatePanel123.Update();
        }
        catch (Exception ex)
        {
            TroyLiteExceptionManager.HandleException(ex);
        }
    }

    protected void txtPredictedClosingDate_TextChanged(object sender, EventArgs e)
    {
        try
        {
            //string stdate = ((TextBox)this.FindControl("tabs2").FindControl("tabMaster").FindControl("txtCreationDate")).Text;
            //DateTime calculat = Convert.ToDateTime(stdate);
            //calculat.AddDays(Convert.ToUInt32(txtPredictedClosing.Text));
            //txtPredictedClosingDate.Text = Convert.ToString(calculat);
            //UpdatePanel10.Update();
        }
        catch (Exception ex)
        {
            TroyLiteExceptionManager.HandleException(ex);
        }
    }

    protected void drpNextActivity_SelectedIndexChanged(object sender, EventArgs e)
    {
        try
        {
        }
        catch (Exception ex)
        {
            TroyLiteExceptionManager.HandleException(ex);
        }
    }

    protected void drpActivityName_SelectedIndexChanged(object sender, EventArgs e)
    {
        try
        {
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
            string textt = string.Empty;
            string dropd = string.Empty;

            textt = txtSearch.Text;
            dropd = ddCriteria.SelectedValue;

            BindGrid(textt, dropd);
        }
        catch (Exception ex)
        {
            TroyLiteExceptionManager.HandleException(ex);
        }
    }

    //protected override void OnInit(EventArgs e)
    //{
    //    base.OnInit(e);
    //    //TextBox search = (TextBox)Accordion1.FindControl("txtSearch");
    //    GridSource.SelectParameters.Add(new CookieParameter("connection", "Company"));
    //    //DropDownList dropDown = (DropDownList)Accordion1.FindControl("ddCriteria");
    //    GridSource.SelectParameters.Add(new ControlParameter("txtSearch", TypeCode.String, txtSearch.UniqueID, "Text"));
    //    GridSource.SelectParameters.Add(new ControlParameter("dropDown", TypeCode.String, ddCriteria.UniqueID, "SelectedValue"));
    //}


    private void BindGrid(string textSearch, string dropDown)
    {
        string connection = Request.Cookies["Company"].Value;

        DataSet ds = new DataSet();
        LeadBusinessLogic bl = new LeadBusinessLogic(sDataSource);

        object usernam = Session["LoggedUserName"];

        ds = bl.ListLead(connection, textSearch, dropDown);

        if (ds != null)
        {
            if (ds.Tables[0].Rows.Count > 0)
            {
                GrdViewLead.DataSource = ds.Tables[0].DefaultView;
                GrdViewLead.DataBind();
            }
        }
        else
        {
            GrdViewLead.DataSource = null;
            GrdViewLead.DataBind();
        }
    }

    protected void GrdCompetitors_RowCreated(object sender, GridViewRowEventArgs e)
    {
        try
        {
            //if (e.Row.RowType == DataControlRowType.Pager)
            //{
            //    PresentationUtils.SetPagerButtonStates(GrdCompetitors, e.Row, this);
            //}
        }
        catch (Exception ex)
        {
            TroyLiteExceptionManager.HandleException(ex);
        }
    }

    protected void GrdCompetitors_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        try
        {
            //if (e.CommandName == "Cancel")
            //{
            //    if (GrdCompetitors.Rows.Count == 0)
            //    {
            //        DataSet ds = new DataSet();
            //        DataTable dt = new DataTable();
            //        DataColumn dc;

            //        dc = new DataColumn("Competitor_Name");
            //        dt.Columns.Add(dc);
            //        dc = new DataColumn("Competitor_Id");
            //        dt.Columns.Add(dc);
            //        ds.Tables.Add(dt);

            //        DataRow dr = ds.Tables[0].NewRow();

            //        dr["Competitor_Name"] = "";
            //        dr["Competitor_Id"] = 0;
            //        ds.Tables[0].Rows.InsertAt(dr, 0);

            //        GrdCompetitors.DataSource = ds;
            //        GrdCompetitors.DataBind();
            //        GrdCompetitors.Rows[0].Visible = false;
            //    }

            //    GrdCompetitors.FooterRow.Visible = false;
            //    lnkBtnCompetitor.Visible = true;
            //}
            //else if (e.CommandName == "Insert")
            //{
            //    if (!Page.IsValid)
            //    {
            //        foreach (IValidator validator in Page.Validators)
            //        {
            //            if (!validator.IsValid)
            //            {
            //                //errDisp.AddItem(validator.ErrorMessage, DisplayIcons.Error, true);
            //            }
            //        }
            //    }
            //    else
            //    {
            //        BusinessLogic objBus = new BusinessLogic();

            //        string unit = ((TextBox)GrdCompetitors.FooterRow.FindControl("txtAddUnit")).Text;

            //        string sQl = string.Format("Insert Into tblCompetitors(Competitor_Name) Values('{0}')", unit);
            //        string connection = Request.Cookies["Company"].Value;

            //        //srcGridView.InsertParameters.Add("sQl", TypeCode.String, sQl);
            //        //srcGridView.InsertParameters.Add("connection", TypeCode.String, GetConnectionString());

            //        try
            //        {
            //            //srcGridView.Insert();
            //            objBus.InsertUnitRecord(sQl, connection);
            //            System.Threading.Thread.Sleep(1000);
            //            GrdCompetitors.DataBind();
            //            lnkBtnCompetitor.Visible = true;
            //        }
            //        catch (Exception ex)
            //        {
            //            if (ex.InnerException != null)
            //            {
            //                StringBuilder script = new StringBuilder();
            //                script.Append("alert('Competitors with this name already exists, Please try with a different name.');");

            //                if (ex.InnerException.Message.IndexOf("duplicate values in the index") > -1)
            //                    ScriptManager.RegisterStartupScript(Page, Page.GetType(), Guid.NewGuid().ToString(), script.ToString(), true);

            //                return;
            //            }
            //        }
            //        //lnkBtnAdd.Visible = true;
            //    }


            //}
            //else if (e.CommandName == "Update")
            //{
            //    if (!Page.IsValid)
            //    {
            //        foreach (IValidator validator in Page.Validators)
            //        {
            //            if (!validator.IsValid)
            //            {
            //                //errDisp.AddItem(validator.ErrorMessage, DisplayIcons.Error, true);
            //            }
            //        }
            //        return;
            //    }
            //}
            //else if (e.CommandName == "Edit")
            //{
            //    lnkBtnCompetitor.Visible = false;
            //}
            //else if (e.CommandName == "Page")
            //{
            //    //lnkBtnAdd.Visible = true;
            //}
        }
        catch (Exception ex)
        {
            TroyLiteExceptionManager.HandleException(ex);
        }
    }

    protected void GrdCompetitors_RowUpdated(object sender, GridViewUpdatedEventArgs e)
    {
        try
        {
            //System.Threading.Thread.Sleep(1000);
            //GrdCompetitors.DataBind();
            //lnkBtnCompetitor.Visible = true;
        }
        catch (Exception ex)
        {
            TroyLiteExceptionManager.HandleException(ex);
        }
    }

    protected void GrdCompetitors_RowUpdating(object sender, GridViewUpdateEventArgs e)
    {
        try
        {
            //if (!Page.IsValid)
            //{
            //    foreach (IValidator validator in Page.Validators)
            //    {
            //        if (!validator.IsValid)
            //        {
            //            //errDisp.AddItem(validator.ErrorMessage, DisplayIcons.Error, true);
            //        }
            //    }

            //}
            //else
            //{

            //    string unit = ((TextBox)GrdCompetitors.Rows[e.RowIndex].FindControl("txtUnit")).Text;
            //    int Id = Convert.ToInt32(GrdCompetitors.DataKeys[e.RowIndex].Value);

            //    BusinessLogic objBus = new BusinessLogic();
            //    string connection = Request.Cookies["Company"].Value;
            //    objBus.UpdateCompetitors(connection, unit, Id);

            //    //srcGridView.UpdateMethod = "UpdateCompetitors";
            //    //srcGridView.UpdateParameters.Add("connection", TypeCode.String, GetConnectionString());
            //    //srcGridView.UpdateParameters.Add("Unit", TypeCode.String, unit);
            //    //srcGridView.UpdateParameters.Add("ID", TypeCode.Int32, Id);
            //    //lnkBtnAdd.Visible = true;

            //}
        }
        catch (Exception ex)
        {
            TroyLiteExceptionManager.HandleException(ex);
        }
    }

    private void BindDropdownList()
    {
        LeadBusinessLogic bl = new LeadBusinessLogic(sDataSource);
        DataSet ds = new DataSet();

        //ds = bl.GetDropdownList(sDataSource, "CONTACT");
        //cmbModeOfContact.DataSource = ds;
        //cmbModeOfContact.DataBind();
        //cmbModeOfContact.DataTextField = "TextValue";
        //cmbModeOfContact.DataValueField = "TextValue";

        //ds = bl.GetDropdownList(sDataSource, "PERRES");
        //cmbPersonalResp.DataSource = ds;
        //cmbPersonalResp.DataBind();
        //cmbPersonalResp.DataTextField = "TextValue";
        //cmbPersonalResp.DataValueField = "TextValue";

        //ds = bl.GetDropdownList(sDataSource, "BUSTYPE");
        //cmbBussType.DataSource = ds;
        //cmbBussType.DataBind();
        //cmbBussType.DataTextField = "TextValue";
        //cmbBussType.DataValueField = "TextValue";

        //ds = bl.GetDropdownList(sDataSource, "BRNCH");
        //cmbBranch.DataSource = ds;
        //cmbBranch.DataBind();
        //cmbBranch.DataTextField = "TextValue";
        //cmbBranch.DataValueField = "TextValue";

        //ds = bl.GetDropdownList(sDataSource, "LSTCMP");
        //cmbLastCompAction.DataSource = ds;
        //cmbLastCompAction.DataBind();
        //cmbLastCompAction.DataTextField = "TextValue";
        //cmbLastCompAction.DataValueField = "TextValue";

        //ds = bl.GetDropdownList(sDataSource, "LSTCMP");
        //cmblastaction.DataSource = ds;
        //cmblastaction.DataBind();
        //cmblastaction.DataTextField = "TextValue";
        //cmblastaction.DataValueField = "TextValue";

        //ds = bl.GetDropdownList(sDataSource, "NXTAXN");
        //cmbNextAction.DataSource = ds;
        //cmbNextAction.DataBind();
        //cmbNextAction.DataTextField = "TextValue";
        //cmbNextAction.DataValueField = "TextValue";

        //ds = bl.GetDropdownList(sDataSource, "NXTAXN");
        //cmbnxtaction.DataSource = ds;
        //cmbnxtaction.DataBind();
        //cmbnxtaction.DataTextField = "TextValue";
        //cmbnxtaction.DataValueField = "TextValue";

        //ds = bl.GetDropdownList(sDataSource, "CATEGRY");
        //cmbCategory.DataSource = ds;
        //cmbCategory.DataBind();
        //cmbCategory.DataTextField = "TextValue";
        //cmbCategory.DataValueField = "TextValue";

        //ds = bl.GetDropdownList(sDataSource, "STATUS");
        //cmbStatus.DataSource = ds;
        //cmbStatus.DataBind();
        //cmbStatus.DataTextField = "TextValue";
        //cmbStatus.DataValueField = "TextValue";

        //ds = bl.GetDropdownList(sDataSource, "STATUS");
        //cmbnewstatus.DataSource = ds;
        //cmbnewstatus.DataBind();
        //cmbnewstatus.DataTextField = "TextValue";
        //cmbnewstatus.DataValueField = "TextValue";

        //ds = bl.GetDropdownList(sDataSource, "INFO3");
        //ddlinfo3.DataSource = ds;
        //ddlinfo3.DataBind();
        //ddlinfo3.DataTextField = "TextValue";
        //ddlinfo3.DataValueField = "TextValue";

        //ds = bl.GetDropdownList(sDataSource, "INFO4");
        //ddlinfo4.DataSource = ds;
        //ddlinfo4.DataBind();
        //ddlinfo4.DataTextField = "TextValue";
        //ddlinfo4.DataValueField = "TextValue";

        //ds = bl.GetDropdownList(sDataSource, "INFO5");
        //ddlinfo5.DataSource = ds;
        //ddlinfo5.DataBind();
        //ddlinfo5.DataTextField = "TextValue";
        //ddlinfo5.DataValueField = "TextValue";

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
            Reset();
            ModalPopupExtender2.Show();
            UpdateButton.Visible = false;
            AddButton.Visible = true;
            UpdateButton.Visible = false;
            Session["Lead_No"] = "0";
            Session["contactDs"] = null;
            ShowLeadContactInfo();

            Session["CompetitorDs"] = null;
            GrdViewLeadCompetitor.DataSource = null;
            GrdViewLeadCompetitor.DataBind();

            Session["ActivityDs"] = null;
            GrdViewLeadActivity.DataSource = null;
            GrdViewLeadActivity.DataBind();

            Session["ProductDs"] = null;
            GrdViewLeadproduct.DataSource = null;
            GrdViewLeadproduct.DataBind();

            Session["Date"] = "Add";

            //txtCreationDate.Text = DateTime.Now.ToShortDateString();

            // BtnAddStage.Visible = true;
            // pnlStage.Visible = false;
            // GrdViewLeadStage.Visible = true;
            loadEmp();

            // BtnAddproduct.Visible = true;
            GrdViewLeadproduct.Visible = true;
            pnlproduct.Visible = false;

            // BtnAddCompetitor.Visible = true;
            GrdViewLeadCompetitor.Visible = true;
            pnlCompetitor.Visible = false;

            // BtnAddActivity.Visible = true;
            GrdViewLeadActivity.Visible = true;
            pnlActivity.Visible = false;

            DataSet ds = new DataSet();
            DataTable dt = new DataTable();
            DataColumn dc;

            dc = new DataColumn("Competitor_Name");
            dt.Columns.Add(dc);
            dc = new DataColumn("Competitor_Id");
            dt.Columns.Add(dc);
            ds.Tables.Add(dt);

            DataRow dr = ds.Tables[0].NewRow();

            dr["Competitor_Name"] = "";
            dr["Competitor_Id"] = 0;
            ds.Tables[0].Rows.InsertAt(dr, 0);

            //GrdCompetitors.DataSource = ds;
            //GrdCompetitors.DataBind();
            //GrdCompetitors.Rows[0].Visible = false;

            DateTime indianStd = TimeZoneInfo.ConvertTimeBySystemTimeZoneId(DateTime.Now, "India Standard Time");
            string dtaa = Convert.ToDateTime(indianStd).ToString("dd/MM/yyyy");
            txtCreationDate.Text = dtaa;


            string DBname = string.Empty;
            DBname = GetCurrentDBName(sDataSource);
            string a = string.Empty;
            string b = string.Empty;
            for (int i = 0; i < DBname.Length; i++)
            {
                if (Char.IsDigit(DBname[i]))
                    b += DBname[i];
                else
                    a += DBname[i];
            }


            //txtBranch.Text = a;
            drpLeadStatus.Enabled = false;
            drpStatus.Enabled = false;

            txtLeadNo.Text = "- TBA -";
            txtCreationDate.Focus();
            BindStage();
            FirstGridViewRow_ProductTab();
            FirstGridViewRow_CompetitorsTab();
            FirstGridViewRow_ActivityTab();
            //DropDownList1.SelectedItem.Text = "NO";
        }
        catch (Exception ex)
        {
            TroyLiteExceptionManager.HandleException(ex);
        }
    }

    private void loadEmp()
    {
        BusinessLogic bl = new BusinessLogic(sDataSource);
        DataSet ds = new DataSet();
        string connection = ConfigurationManager.ConnectionStrings[Request.Cookies["Company"].Value].ToString();

        drpIncharge.Items.Clear();
        drpIncharge.Items.Add(new ListItem("Select Employee", "0"));
        ds = bl.ListExecutive();
        drpIncharge.DataSource = ds;
        drpIncharge.DataBind();
        drpIncharge.DataTextField = "empFirstName";
        drpIncharge.DataValueField = "empno";
    }

    public string GetCurrentDBName(string con)
    {
        string str = con; // "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=E:\\DemoDev\\Accsys\\App_Data\\sairama.mdb;Persist Security Info=True;Jet OLEDB:Database Password=moonmoon";
        string str2 = string.Empty;
        if (str.Contains(".mdb"))
        {
            str2 = str.Substring(str.IndexOf("Data Source"), str.IndexOf("Persist", 0));
            str2 = str2.Substring(str2.LastIndexOf("\\") + 1, str2.IndexOf(";") - str2.LastIndexOf("\\"));
            if (str2.EndsWith(";"))
            {
                str2 = str2.Remove(str2.Length - 5, 5);
            }
        }
        return str2;
    }

    protected void lnkBtnCompetitor_Click(object sender, EventArgs e)
    {
        try
        {

            //if (GrdCompetitors.Rows.Count == 0)
            //{
            //    DataSet ds = new DataSet();
            //    DataTable dt = new DataTable();
            //    DataColumn dc;

            //    dc = new DataColumn("Competitor_Name");
            //    dt.Columns.Add(dc);
            //    dc = new DataColumn("Competitor_Id");
            //    dt.Columns.Add(dc);
            //    ds.Tables.Add(dt);

            //    DataRow dr = ds.Tables[0].NewRow();

            //    dr["Competitor_Name"] = "";
            //    dr["Competitor_Id"] = 0;
            //    ds.Tables[0].Rows.InsertAt(dr, 0);

            //    GrdCompetitors.DataSource = ds;
            //    GrdCompetitors.DataBind();
            //    GrdCompetitors.Rows[0].Visible = false;
            //}


            //GrdCompetitors.FooterRow.Visible = true;
            //lnkBtnCompetitor.Visible = false;
        }
        catch (Exception ex)
        {
            TroyLiteExceptionManager.HandleException(ex);
        }
    }


    protected void GrdStage_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        try
        {
            //GrdStage.PageIndex = e.NewPageIndex;
            //BindStage();
        }
        catch (Exception ex)
        {
            TroyLiteExceptionManager.HandleException(ex);
        }
    }
    protected void GrdStage_RowCreated(object sender, GridViewRowEventArgs e)
    {
        try
        {
            //if (e.Row.RowType == DataControlRowType.Pager)
            //{
            //    PresentationUtils.SetPagerButtonStates(GrdStage, e.Row, this);
            //}
        }
        catch (Exception ex)
        {
            TroyLiteExceptionManager.HandleException(ex);
        }
    }

    protected void GrdStage_DataBound(object sender, EventArgs e)
    {


    }


    protected void GrdStage_RowDeleting(object sender, GridViewDeleteEventArgs e)
    {
        try
        {
            //sDataSource = ConfigurationManager.ConnectionStrings[Request.Cookies["Company"].Value].ToString();
            //BusinessLogic bl = new BusinessLogic(sDataSource);
            //int divisionID = (int)GrdStage.DataKeys[e.RowIndex].Value;
            //bl.DeleteDivision(sDataSource, divisionID);
            //GrdStage.DataSource = bl.ListDivisions();
            //GrdStage.DataBind();
        }
        catch (Exception ex)
        {
            TroyLiteExceptionManager.HandleException(ex);
        }
    }

    protected void GrdStage_SelectedIndexChanged(object sender, EventArgs e)
    {
        try
        {
            //pnlStage.Visible = true;
            //btnDivSave.Visible = false;
            //btnDivUpdate.Visible = true;
            //GrdStage.Visible = false;
            //BtnAddStage.Visible = false;

            //sDataSource = ConfigurationManager.ConnectionStrings[Request.Cookies["Company"].Value].ToString();
            //BusinessLogic bl = new BusinessLogic(sDataSource);
            //hdBtnAddStage.Value = GrdStage.SelectedDataKey.Value.ToString();
            //DataSet ds = bl.GetDivisionForId(sDataSource, int.Parse(GrdStage.SelectedDataKey.Value.ToString()));

            //if (ds != null)
            //{
            //txtDivision.Text = ds.Tables[0].Rows[0]["DivisionName"].ToString();
            //txtDivAddress.Text = ds.Tables[0].Rows[0]["Address"].ToString();
            //txtDivCity.Text = ds.Tables[0].Rows[0]["City"].ToString();
            //txtDivState.Text = ds.Tables[0].Rows[0]["State"].ToString();
            //txtDivPinNo.Text = ds.Tables[0].Rows[0]["PinCode"].ToString();
            //txtDivPhoneNo.Text = ds.Tables[0].Rows[0]["Phone"].ToString();
            //txtDivEmail.Text = ds.Tables[0].Rows[0]["eMail"].ToString();
            //txtDivFax.Text = ds.Tables[0].Rows[0]["Fax"].ToString();
            //txtDivTinNo.Text = ds.Tables[0].Rows[0]["TINNo"].ToString();
            //txtDivGSTNo.Text = ds.Tables[0].Rows[0]["GSTNo"].ToString();
            //}
        }
        catch (Exception ex)
        {
            TroyLiteExceptionManager.HandleException(ex);
        }
    }

    protected void btnDivSave_Click(object sender, EventArgs e)
    {
        try
        {
            //sDataSource = ConfigurationManager.ConnectionStrings[Request.Cookies["Company"].Value].ToString();
            //BusinessLogic bl = new BusinessLogic(sDataSource);

            ////bl.InsertDivision(sDataSource, txtDivision.Text, txtDivAddress.Text, txtDivCity.Text, txtDivState.Text, txtDivPinNo.Text, txtDivPhoneNo.Text, txtDivFax.Text, txtDivEmail.Text, txtDivTinNo.Text, txtDivGSTNo.Text);
            //System.Threading.Thread.Sleep(1000);
            //DataSet ds = bl.ListDivisions();
            //GrdStage.DataSource = ds;
            //GrdStage.DataBind();
            //pnlStage.Visible = false;
            //GrdStage.Visible = true;
            //BtnAddStage.Visible = true;
        }
        catch (Exception ex)
        {
            TroyLiteExceptionManager.HandleException(ex);
        }
    }

    protected void BtnAddStage_Click(object sender, EventArgs e)
    {
        try
        {
            ResetStage();
            //BtnAddStage.Visible = false;
            //cmdSaveContact.Visible = true;
            //cmdUpdateContact.Visible = false;
            //pnlStage.Visible = true;
            //GrdViewLeadStage.Visible = false;
            loadStages();

            //txtStagePotentialAmount.Text = txtPotentialPotAmount.Text;

        }
        catch (Exception ex)
        {
            TroyLiteExceptionManager.HandleException(ex);
        }
    }

    protected void BtnAddCompetitor_Click(object sender, EventArgs e)
    {
        try
        {
            ResetCompetitor();
            // BtnAddCompetitor.Visible = false;
            cmdSaveCompetitor.Visible = true;
            cmdUpdateCompetitor.Visible = false;
            pnlCompetitor.Visible = true;
            GrdViewLeadCompetitor.Visible = false;
        }
        catch (Exception ex)
        {
            TroyLiteExceptionManager.HandleException(ex);
        }
    }

    protected void BtnAddActivity_Click(object sender, EventArgs e)
    {
        try
        {
            loadActivities();
            ResetActivity();
            // BtnAddActivity.Visible = false;
            cmdSaveActivity.Visible = true;
            cmdUpdateActivity.Visible = false;
            pnlActivity.Visible = true;
            GrdViewLeadActivity.Visible = false;

        }
        catch (Exception ex)
        {
            TroyLiteExceptionManager.HandleException(ex);
        }
    }

    protected void BtnAddproduct_Click(object sender, EventArgs e)
    {
        try
        {
            loadproduct();
            Resetproduct();
            BtnAddproduct.Visible = false;
            cmdSaveproduct.Visible = true;
            cmdUpdateproduct.Visible = false;
            pnlproduct.Visible = true;
            GrdViewLeadproduct.Visible = false;
            //ModalPopupExtender2.Show();
            //ModalPopupProduct.Show();
            //updatePnlProduct.Update();
        }
        catch (Exception ex)
        {
            TroyLiteExceptionManager.HandleException(ex);
        }
    }

    protected void btnDivCancel_Click(object sender, EventArgs e)
    {
        try
        {
            //pnlStage.Visible = false;
            //GrdStage.Visible = true;
            //BtnAddStage.Visible = true;
            //ResetStage();
            //BindStage();
        }
        catch (Exception ex)
        {
            TroyLiteExceptionManager.HandleException(ex);
        }
    }

    private void BindStage()
    {
        //sDataSource = ConfigurationManager.ConnectionStrings[Request.Cookies["Company"].Value].ToString();
        //BusinessLogic bl = new BusinessLogic(sDataSource);

        //GrdStage.DataSource = bl.ListDivisions();
        ///*if(ds.Tables[0].Rows.Count > 1)
        //    GrdDiv.DataSource = ds;
        //else
        //    GrdDiv.DataSource = null;*/

        //GrdStage.DataBind();
    }

    private void ResetStage()
    {
        // hdBtnAddStage.Value = string.Empty;
        // txtStageStartDate.Text = string.Empty;
        //  txtStageEndDate.Text = string.Empty;
        //  drpStageName.SelectedIndex = 0;
        // txtStagePerc.Text = "0";
        // txtStagePotentialAmount.Text = "0";
        // txtStageWeightedAmount.Text = "0";
        //  txtStageRemarks.Text = string.Empty;
    }

    private void ResetCompetitor()
    {
        HiddenField2.Value = string.Empty;
        drpThreatLevel.SelectedIndex = 0;
        txtCompetitorName.Text = string.Empty;
        txtCompetitorRemarks.Text = string.Empty;
    }

    private void Resetproduct()
    {
        HiddenField6.Value = string.Empty;
        drpproduct.SelectedIndex = 0;
    }

    private void ResetActivity()
    {
        HiddenField4.Value = string.Empty;
        drpActivityName.SelectedIndex = 0;
        drpNextActivity.SelectedIndex = 0;
        txtActivityDate.Text = string.Empty;
        txtActivityLocation.Text = string.Empty;
        //txtActivityEndDate.Text = string.Empty;
        drpActivityEmployee.SelectedIndex = 0;
        txtNextActivityDate.Text = string.Empty;
        //drpFollowUp.SelectedIndex = 0;
        txtActivityRemarks.Text = string.Empty;
    }

    protected void btnDivUpdate_Click(object sender, EventArgs e)
    {
        try
        {
            //sDataSource = ConfigurationManager.ConnectionStrings[Request.Cookies["Company"].Value].ToString();
            //BusinessLogic bl = new BusinessLogic(sDataSource);

            ////bl.UpdateDivision(sDataSource, int.Parse(hdDivision.Value.ToString()), txtDivision.Text, txtDivAddress.Text, txtDivCity.Text, txtDivState.Text, txtDivPinNo.Text, txtDivPhoneNo.Text, txtDivFax.Text, txtDivEmail.Text, txtDivTinNo.Text, txtDivGSTNo.Text);
            //System.Threading.Thread.Sleep(1000);
            //DataSet ds = bl.ListDivisions();
            //GrdStage.DataSource = ds;
            //GrdStage.DataBind();
            //pnlStage.Visible = false;
            //GrdStage.Visible = true;
            //BtnAddStage.Visible = true;
        }
        catch (Exception ex)
        {
            TroyLiteExceptionManager.HandleException(ex);
        }
    }


    private void Reset()
    {
        txtLeadNo.Text = "";
        txtAddress.Text = "";
        cmbCustomer.SelectedIndex = 0;
        txtMobile.Text = "";
        txtLeadName.Text = "";
        txtTelephone.Text = "";
        //drpInterestLevel.SelectedIndex = 0;
        drpIncharge.SelectedIndex = 0;
        drpLeadStatus.SelectedIndex = 0;
        drpStatus.SelectedIndex = 0;
        //drpPredictedClosingPeriod.SelectedIndex = 0;
        //txtTotalAmount.Text = "0";
        // txtClosingPer.Text = "0";
        txtClosingDate.Text = "";
        //txtContactName.Text = "";
        //txtPotentialPotAmount.Text = "";
        //txtPotentialWeightedAmount.Text = "";
        //txtPredictedClosingDate.Text = "";
        //txtPredictedClosing.Text = "";
        //txtBranch.Text = "";
        chk.Checked = true;
        txtBPName.Text = "";
        cmbCustomer.Visible = true;
        txtBPName.Visible = false;
    }

    protected void chk_CheckedChanged(object sender, EventArgs e)
    {
        if (chk.Checked == false)
        {
            txtBPName.Visible = true;
            cmbCustomer.Visible = false;
            // txtContactName.Enabled = true;
            txtAddress.Enabled = true;
            txtMobile.Enabled = true;
            txtTelephone.Enabled = true;
        }
        else
        {
            cmbCustomer.Visible = true;
            txtBPName.Visible = false;
            // txtContactName.Enabled = false;
            txtAddress.Enabled = false;
            txtMobile.Enabled = false;
            txtTelephone.Enabled = false;
        }
    }

    private void ShowLeadContactInfo()
    {
        string connStr = GetConnectionString();

        if (Session["Lead_No"] != null && Session["Lead_No"].ToString() != "0")
        {
            LeadBusinessLogic bl = new LeadBusinessLogic(connStr);
            //DataSet ds = bl.ListLeadContact(Session["Lead_No"].ToString());

            //if (ds != null)
            //{
            //    GrdViewLeadStage.DataSource = ds.Tables[0];
            //    GrdViewLeadStage.DataBind();
            //}
            // GrdViewLeadStage.DataSource = null;
            // GrdViewLeadStage.DataBind();
        }
        else
        {
            // GrdViewLeadStage.DataSource = null;
            // GrdViewLeadStage.DataBind();
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

    protected void cmdSaveContact_Click(object sender, EventArgs e)
    {
        try
        {
            DataSet ds;
            DataTable dt;
            DataRow drNew;
            DataColumn dc;
            ds = new DataSet();

            if (Session["contactDs"] == null)
            {
                dt = new DataTable();

                dc = new DataColumn("Stage_Id");
                dt.Columns.Add(dc);

                dc = new DataColumn("Stage_Name");
                dt.Columns.Add(dc);

                dc = new DataColumn("Stage_Perc");
                dt.Columns.Add(dc);

                dc = new DataColumn("Start_Date");
                dt.Columns.Add(dc);

                dc = new DataColumn("End_Date");
                dt.Columns.Add(dc);

                dc = new DataColumn("Potential_Amount");
                dt.Columns.Add(dc);

                dc = new DataColumn("Weighted_Amount");
                dt.Columns.Add(dc);

                dc = new DataColumn("Remarks");
                dt.Columns.Add(dc);

                dc = new DataColumn("Stage_Setup_Id");
                dt.Columns.Add(dc);

                ds.Tables.Add(dt);

                drNew = dt.NewRow();

                drNew["Stage_Id"] = 1;
                // drNew["Stage_Name"] = drpStageName.SelectedItem.Text;
                // drNew["Stage_Setup_Id"] = drpStageName.SelectedValue;
                // drNew["Stage_Perc"] = txtStagePerc.Text;
                //txtClosingPer.Text = txtStagePerc.Text;
                // txtPotentialWeightedAmount.Text = txtStageWeightedAmount.Text;
                // drNew["Remarks"] = txtStageRemarks.Text;
                // drNew["Weighted_Amount"] = txtStageWeightedAmount.Text;
                // drNew["Potential_Amount"] = txtStagePotentialAmount.Text;
                // drNew["Start_Date"] = txtStageStartDate.Text;
                // drNew["End_Date"] = txtStageEndDate.Text;
                ds.Tables[0].Rows.Add(drNew);
                Session["contactDs"] = ds;
            }
            else
            {
                ds = (DataSet)Session["contactDs"];

                int maxID = 0;

                if (ds.Tables[0].Rows.Count > 0)
                    maxID = int.Parse(ds.Tables[0].Rows[ds.Tables[0].Rows.Count - 1]["Stage_Id"].ToString());

                drNew = ds.Tables[0].NewRow();
                drNew["Stage_Id"] = maxID + 1;
                //drNew["Stage_Name"] = drpStageName.SelectedItem.Text;
                // drNew["Stage_Setup_Id"] = drpStageName.SelectedValue;
                // drNew["Stage_Perc"] = txtStagePerc.Text;
                //txtClosingPer.Text = txtStagePerc.Text;
                //txtPotentialWeightedAmount.Text = txtStageWeightedAmount.Text;
                //drNew["Remarks"] = txtStageRemarks.Text;
                //drNew["Weighted_Amount"] = txtStageWeightedAmount.Text;
                //drNew["Potential_Amount"] = txtStagePotentialAmount.Text;
                //drNew["Start_Date"] = txtStageStartDate.Text;
                //drNew["End_Date"] = txtStageEndDate.Text;
                ds.Tables[0].Rows.Add(drNew);
                Session["contactDs"] = ds;
            }

            //GrdViewLeadStage.DataSource = ds.Tables[0];
            //GrdViewLeadStage.DataBind();

            //pnlStage.Visible = false;
            //GrdViewLeadStage.Visible = true;
            //BtnAddStage.Visible = true;
        }
        catch (Exception ex)
        {
            TroyLiteExceptionManager.HandleException(ex);
        }
    }

    protected void cmdSaveCompetitor_Click(object sender, EventArgs e)
    {
        try
        {
            DataSet ds;
            DataTable dt;
            DataRow drNew;
            DataColumn dc;
            ds = new DataSet();

            if (Session["CompetitorDs"] == null)
            {
                dt = new DataTable();

                dc = new DataColumn("Competitor_Id");
                dt.Columns.Add(dc);

                dc = new DataColumn("Competitor_Name");
                dt.Columns.Add(dc);

                dc = new DataColumn("Threat_Level");
                dt.Columns.Add(dc);

                dc = new DataColumn("Remarks");
                dt.Columns.Add(dc);

                ds.Tables.Add(dt);

                drNew = dt.NewRow();

                drNew["Competitor_Id"] = 1;
                drNew["Competitor_Name"] = txtCompetitorName.Text;
                drNew["Threat_Level"] = drpThreatLevel.SelectedItem.Text;
                if (txtCompetitorRemarks.Text == "")
                {
                    drNew["Remarks"] = "";
                }
                else
                {
                    drNew["Remarks"] = txtCompetitorRemarks.Text;
                }
                ds.Tables[0].Rows.Add(drNew);
                Session["CompetitorDs"] = ds;
            }
            else
            {
                ds = (DataSet)Session["CompetitorDs"];

                int maxID = 0;

                if (ds.Tables[0].Rows.Count > 0)
                    maxID = int.Parse(ds.Tables[0].Rows[ds.Tables[0].Rows.Count - 1]["Competitor_Id"].ToString());

                drNew = ds.Tables[0].NewRow();
                drNew["Competitor_Id"] = maxID + 1;
                drNew["Competitor_Name"] = txtCompetitorName.Text;
                drNew["Threat_Level"] = drpThreatLevel.SelectedItem.Text;
                if (txtCompetitorRemarks.Text == "")
                {
                    drNew["Remarks"] = "";
                }
                else
                {
                    drNew["Remarks"] = txtCompetitorRemarks.Text;
                }
                ds.Tables[0].Rows.Add(drNew);
                Session["CompetitorDs"] = ds;
            }

            GrdViewLeadCompetitor.DataSource = ds.Tables[0];
            GrdViewLeadCompetitor.DataBind();

            pnlCompetitor.Visible = false;
            GrdViewLeadCompetitor.Visible = true;
            //BtnAddCompetitor.Visible = true;
        }
        catch (Exception ex)
        {
            TroyLiteExceptionManager.HandleException(ex);
        }
    }

    protected void cmdSaveproduct_Click(object sender, EventArgs e)
    {
        try
        {
            DataSet ds;
            DataTable dt;
            DataRow drNew;
            DataColumn dc;
            ds = new DataSet();

            if (Session["ProductDs"] == null)
            {
                dt = new DataTable();

                dc = new DataColumn("Product_interest_Id");
                dt.Columns.Add(dc);

                dc = new DataColumn("Product_Name");
                dt.Columns.Add(dc);

                dc = new DataColumn("SlNo");
                dt.Columns.Add(dc);

                dc = new DataColumn("Product_Id");
                dt.Columns.Add(dc);

                ds.Tables.Add(dt);

                drNew = dt.NewRow();

                drNew["Product_interest_Id"] = 1;
                drNew["Product_Name"] = drpproduct.SelectedItem.Text;
                drNew["SlNo"] = 1;
                drNew["Product_Id"] = drpproduct.SelectedValue;
                ds.Tables[0].Rows.Add(drNew);
                Session["ProductDs"] = ds;
            }
            else
            {
                ds = (DataSet)Session["ProductDs"];

                int maxID = 0;

                if (ds.Tables[0].Rows.Count > 0)
                    maxID = int.Parse(ds.Tables[0].Rows[ds.Tables[0].Rows.Count - 1]["Product_interest_Id"].ToString());

                drNew = ds.Tables[0].NewRow();
                drNew["Product_interest_Id"] = maxID + 1;
                drNew["Product_Name"] = drpproduct.SelectedItem.Text;
                drNew["SlNo"] = maxID + 1;
                drNew["Product_Id"] = drpproduct.SelectedValue;
                ds.Tables[0].Rows.Add(drNew);
                Session["ProductDs"] = ds;
            }

            GrdViewLeadproduct.DataSource = ds.Tables[0];
            GrdViewLeadproduct.DataBind();

            pnlproduct.Visible = false;
            GrdViewLeadproduct.Visible = true;
            // BtnAddproduct.Visible = true;
        }
        catch (Exception ex)
        {
            TroyLiteExceptionManager.HandleException(ex);
        }
    }

    protected void cmdUpdateContact_Click(object sender, EventArgs e)
    {
        try
        {
            var ds = (DataSet)Session["contactDs"];
            // int currentRow = int.Parse(hdCurrentRow.Value);

            //ds.Tables[0].Rows[currentRow]["Stage_Name"] = drpStageName.SelectedItem.Text;
            //ds.Tables[0].Rows[currentRow]["Stage_Setup_Id"] = drpStageName.SelectedValue;
            //ds.Tables[0].Rows[currentRow]["Stage_Perc"] = txtStagePerc.Text;
            //ds.Tables[0].Rows[currentRow]["Remarks"] = txtStageRemarks.Text;
            //ds.Tables[0].Rows[currentRow]["Weighted_Amount"] = txtStageWeightedAmount.Text;
            //ds.Tables[0].Rows[currentRow]["Potential_Amount"] = txtStagePotentialAmount.Text;
            //ds.Tables[0].Rows[currentRow]["Start_Date"] = txtStageStartDate.Text;
            //ds.Tables[0].Rows[currentRow]["End_Date"] = txtStageEndDate.Text;

            //ds.Tables[0].Rows[currentRow].EndEdit();
            // ds.Tables[0].Rows[currentRow].AcceptChanges();

            //GrdViewLeadStage.DataSource = ds.Tables[0];
            //GrdViewLeadStage.DataBind();
            //pnlStage.Visible = false;
            Session["contactDs"] = ds;

            if (ds != null)
            {
                if (ds.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow dr in ds.Tables[0].Rows)
                    {
                        //txtPotentialWeightedAmount.Text = dr["Weighted_Amount"].ToString();
                    }
                }
            }

            // pnlStage.Visible = false;
            //GrdViewLeadStage.Visible = true;
            //BtnAddStage.Visible = true;

        }
        catch (Exception ex)
        {
            TroyLiteExceptionManager.HandleException(ex);
        }
    }

    protected void cmdUpdateCompetitor_Click(object sender, EventArgs e)
    {
        try
        {
            var ds = (DataSet)Session["CompetitorDs"];
            int currentRow = int.Parse(HiddenField1.Value);

            ds.Tables[0].Rows[currentRow]["Competitor_Name"] = txtCompetitorName.Text;
            ds.Tables[0].Rows[currentRow]["Threat_Level"] = drpThreatLevel.SelectedItem.Text;
            if (txtCompetitorRemarks.Text == "")
            {
                ds.Tables[0].Rows[currentRow]["Remarks"] = "";
            }
            else
            {
                ds.Tables[0].Rows[currentRow]["Remarks"] = txtCompetitorRemarks.Text;
            }

            ds.Tables[0].Rows[currentRow].EndEdit();
            ds.Tables[0].Rows[currentRow].AcceptChanges();

            GrdViewLeadCompetitor.DataSource = ds.Tables[0];
            GrdViewLeadCompetitor.DataBind();
            pnlCompetitor.Visible = false;
            Session["CompetitorDs"] = ds;

            pnlCompetitor.Visible = false;
            GrdViewLeadCompetitor.Visible = true;
            // BtnAddCompetitor.Visible = true;

        }
        catch (Exception ex)
        {
            TroyLiteExceptionManager.HandleException(ex);
        }
    }

    protected void cmdUpdateproduct_Click(object sender, EventArgs e)
    {
        try
        {
            var ds = (DataSet)Session["ProductDs"];
            int currentRow = int.Parse(HiddenField5.Value);

            ds.Tables[0].Rows[currentRow]["Product_Name"] = drpproduct.SelectedItem.Text;
            ds.Tables[0].Rows[currentRow]["Product_Id"] = drpproduct.SelectedValue;
            //ds.Tables[0].Rows[currentRow]["SlNo"] = txtCompetitorRemarks.Text;

            ds.Tables[0].Rows[currentRow].EndEdit();
            ds.Tables[0].Rows[currentRow].AcceptChanges();

            GrdViewLeadproduct.DataSource = ds.Tables[0];
            GrdViewLeadproduct.DataBind();
            pnlproduct.Visible = false;
            Session["ProductDs"] = ds;

            pnlproduct.Visible = false;
            GrdViewLeadproduct.Visible = true;
            //BtnAddproduct.Visible = true;

        }
        catch (Exception ex)
        {
            TroyLiteExceptionManager.HandleException(ex);
        }
    }

    protected void cmdSaveActivity_Click(object sender, EventArgs e)
    {
        try
        {
            DataSet ds;
            DataTable dt;
            DataRow drNew;
            DataColumn dc;
            ds = new DataSet();

            if (Session["ActivityDs"] == null)
            {
                dt = new DataTable();

                dc = new DataColumn("Activity_Id");
                dt.Columns.Add(dc);

                dc = new DataColumn("Activity_Name");
                dt.Columns.Add(dc);

                dc = new DataColumn("Activity_Name_Id");
                dt.Columns.Add(dc);

                dc = new DataColumn("Start_Date");
                dt.Columns.Add(dc);

                dc = new DataColumn("End_Date");
                dt.Columns.Add(dc);

                dc = new DataColumn("Activity_Location");
                dt.Columns.Add(dc);

                dc = new DataColumn("Next_Activity");
                dt.Columns.Add(dc);

                dc = new DataColumn("Next_Activity_Id");
                dt.Columns.Add(dc);

                dc = new DataColumn("NextActivity_Date");
                dt.Columns.Add(dc);

                dc = new DataColumn("FollowUp");
                dt.Columns.Add(dc);

                dc = new DataColumn("Emp_Name");
                dt.Columns.Add(dc);

                dc = new DataColumn("Emp_No");
                dt.Columns.Add(dc);

                dc = new DataColumn("Remarks");
                dt.Columns.Add(dc);

                ds.Tables.Add(dt);

                drNew = dt.NewRow();

                drNew["Activity_Id"] = 1;
                drNew["Activity_Name"] = drpActivityName.SelectedItem.Text;
                drNew["Activity_Name_Id"] = drpActivityName.SelectedValue;
                drNew["Start_Date"] = txtActivityDate.Text;
                //drNew["End_Date"] = txtActivityEndDate.Text;
                drNew["Activity_Location"] = txtActivityLocation.Text;
                drNew["Next_Activity"] = drpNextActivity.SelectedItem.Text;
                drNew["Next_Activity_Id"] = drpNextActivity.SelectedValue;
                drNew["NextActivity_Date"] = txtNextActivityDate.Text;
                // drNew["FollowUp"] = drpFollowUp.SelectedValue;
                drNew["Emp_Name"] = drpActivityEmployee.SelectedItem.Text;
                drNew["Emp_No"] = drpActivityEmployee.SelectedValue;
                drNew["Remarks"] = txtActivityRemarks.Text;
                ds.Tables[0].Rows.Add(drNew);
                Session["ActivityDs"] = ds;
            }
            else
            {
                ds = (DataSet)Session["ActivityDs"];

                int maxID = 0;

                if (ds.Tables[0].Rows.Count > 0)
                    maxID = int.Parse(ds.Tables[0].Rows[ds.Tables[0].Rows.Count - 1]["Activity_Id"].ToString());

                drNew = ds.Tables[0].NewRow();
                drNew["Activity_Id"] = maxID + 1;
                drNew["Activity_Name"] = drpActivityName.SelectedItem.Text;
                drNew["Activity_Name_Id"] = drpActivityName.SelectedValue;
                drNew["Start_Date"] = txtActivityDate.Text;
                //drNew["End_Date"] = txtActivityEndDate.Text;
                drNew["Activity_Location"] = txtActivityLocation.Text;
                drNew["Next_Activity"] = drpNextActivity.SelectedItem.Text;
                drNew["Next_Activity_Id"] = drpNextActivity.SelectedValue;
                drNew["NextActivity_Date"] = txtNextActivityDate.Text;
                // drNew["FollowUp"] = drpFollowUp.SelectedValue;
                drNew["Emp_Name"] = drpActivityEmployee.SelectedItem.Text;
                drNew["Emp_No"] = drpActivityEmployee.SelectedValue;
                drNew["Remarks"] = txtActivityRemarks.Text;
                ds.Tables[0].Rows.Add(drNew);
                Session["ActivityDs"] = ds;
            }

            GrdViewLeadActivity.DataSource = ds.Tables[0];
            GrdViewLeadActivity.DataBind();

            pnlActivity.Visible = false;
            GrdViewLeadActivity.Visible = true;
            //BtnAddActivity.Visible = true;
        }
        catch (Exception ex)
        {
            TroyLiteExceptionManager.HandleException(ex);
        }
    }

    protected void cmdUpdateActivity_Click(object sender, EventArgs e)
    {
        try
        {
            var ds = (DataSet)Session["ActivityDs"];
            int currentRow = int.Parse(HiddenField3.Value);

            ds.Tables[0].Rows[currentRow]["Activity_Name"] = drpActivityName.SelectedItem.Text;
            ds.Tables[0].Rows[currentRow]["Activity_Name_Id"] = drpActivityName.SelectedValue;
            ds.Tables[0].Rows[currentRow]["Start_Date"] = txtActivityDate.Text;
            //ds.Tables[0].Rows[currentRow]["End_Date"] = txtActivityEndDate.Text;
            ds.Tables[0].Rows[currentRow]["Activity_Location"] = txtActivityLocation.Text;
            ds.Tables[0].Rows[currentRow]["Next_Activity"] = drpNextActivity.SelectedItem.Text;
            ds.Tables[0].Rows[currentRow]["Next_Activity_Id"] = drpNextActivity.SelectedValue;
            ds.Tables[0].Rows[currentRow]["NextActivity_Date"] = txtNextActivityDate.Text;
            // ds.Tables[0].Rows[currentRow]["FollowUp"] = drpFollowUp.SelectedValue;
            ds.Tables[0].Rows[currentRow]["Emp_Name"] = drpActivityEmployee.SelectedItem.Text;
            ds.Tables[0].Rows[currentRow]["Emp_No"] = drpActivityEmployee.SelectedValue;
            ds.Tables[0].Rows[currentRow]["Remarks"] = txtActivityRemarks.Text;

            ds.Tables[0].Rows[currentRow].EndEdit();
            ds.Tables[0].Rows[currentRow].AcceptChanges();

            GrdViewLeadActivity.DataSource = ds.Tables[0];
            GrdViewLeadActivity.DataBind();
            pnlActivity.Visible = false;
            Session["ActivityDs"] = ds;

            pnlActivity.Visible = false;
            GrdViewLeadActivity.Visible = true;
            //BtnAddActivity.Visible = true;
        }
        catch (Exception ex)
        {
            TroyLiteExceptionManager.HandleException(ex);
        }
    }


    protected void lnkAddContact_Click(object sender, EventArgs e)
    {
        try
        {
            //cmdSaveContact.Visible = true;
            //cmdUpdateContact.Visible = false;
            //updatePnlContact.Update();

            //txtContactedDate.Text = string.Empty;
            //txtContactSummary.Text = string.Empty;
            //ComboBox2.SelectedValue = "1";
            //txtcallback.Text = string.Empty;
            //rowcall.Visible = false;

            //ModalPopupContact.Show();
        }
        catch (Exception ex)
        {
            TroyLiteExceptionManager.HandleException(ex);
        }
    }

    protected void GrdViewLeadStage_SelectedIndexChanged(object sender, EventArgs e)
    {
        try
        {
            loadStages();
            DataSet ds = new DataSet();
            // GridViewRow row = GrdViewLeadStage.SelectedRow;

            //hdCurrentRow.Value = Convert.ToString(row.DataItemIndex);

            //txtStageEndDate.Text = row.Cells[1].Text;
            //txtStageStartDate.Text = row.Cells[0].Text;
            //drpStageName.SelectedItem.Text = row.Cells[2].Text;
            //drpStageName.SelectedValue = row.Cells[3].Text;
            //txtStagePerc.Text = row.Cells[4].Text;
            //txtStageRemarks.Text = row.Cells[7].Text;
            //txtStageWeightedAmount.Text = row.Cells[6].Text;
            //txtStagePotentialAmount.Text = row.Cells[5].Text;

            //cmdSaveContact.Visible = false;
            //cmdUpdateContact.Visible = true;
            //pnlStage.Visible = true;
            //GrdViewLeadStage.Visible = false;
            //BtnAddStage.Visible = false;
        }
        catch (Exception ex)
        {
            TroyLiteExceptionManager.HandleException(ex);
        }
    }

    protected void GrdViewLeadCompetitor_SelectedIndexChanged(object sender, EventArgs e)
    {
        try
        {
            DataSet ds = new DataSet();
            GridViewRow row = GrdViewLeadCompetitor.SelectedRow;

            HiddenField1.Value = Convert.ToString(row.DataItemIndex);

            txtCompetitorName.Text = row.Cells[0].Text;
            drpThreatLevel.SelectedValue = row.Cells[1].Text;
            if (row.Cells[2].Text == "&nbsp;")
            {
                txtCompetitorRemarks.Text = "";
            }
            else
            {
                txtCompetitorRemarks.Text = row.Cells[2].Text;
            }

            cmdSaveCompetitor.Visible = false;
            cmdUpdateCompetitor.Visible = true;
            pnlCompetitor.Visible = true;
            GrdViewLeadCompetitor.Visible = false;
            // BtnAddCompetitor.Visible = false;
        }
        catch (Exception ex)
        {
            TroyLiteExceptionManager.HandleException(ex);
        }
    }

    protected void GrdViewLeadproduct_SelectedIndexChanged(object sender, EventArgs e)
    {
        try
        {
            DataSet ds = new DataSet();
            GridViewRow row = GrdViewLeadproduct.SelectedRow;

            HiddenField5.Value = Convert.ToString(row.DataItemIndex);

            drpproduct.SelectedValue = row.Cells[2].Text;

            cmdSaveproduct.Visible = false;
            cmdUpdateproduct.Visible = true;
            pnlproduct.Visible = true;
            GrdViewLeadproduct.Visible = false;
            // BtnAddproduct.Visible = false;
        }
        catch (Exception ex)
        {
            TroyLiteExceptionManager.HandleException(ex);
        }
    }

    protected void GrdViewLeadActivity_SelectedIndexChanged(object sender, EventArgs e)
    {
        try
        {
            loadActivities();
            DataSet ds = new DataSet();
            GridViewRow row = GrdViewLeadActivity.SelectedRow;

            HiddenField3.Value = Convert.ToString(row.DataItemIndex);

            drpActivityName.SelectedValue = row.Cells[1].Text;
            txtActivityDate.Text = row.Cells[2].Text;
            // txtActivityEndDate.Text = row.Cells[3].Text;
            txtNextActivityDate.Text = row.Cells[7].Text;
            txtActivityLocation.Text = row.Cells[4].Text;
            drpNextActivity.SelectedValue = row.Cells[6].Text;
            drpActivityEmployee.SelectedValue = row.Cells[11].Text;
            //drpFollowUp.SelectedValue = row.Cells[8].Text;
            txtActivityRemarks.Text = row.Cells[9].Text;

            cmdSaveActivity.Visible = false;
            cmdUpdateActivity.Visible = true;
            pnlActivity.Visible = true;
            GrdViewLeadActivity.Visible = false;
            //  BtnAddActivity.Visible = false;
        }
        catch (Exception ex)
        {
            TroyLiteExceptionManager.HandleException(ex);
        }
    }

    //protected void GrdViewLeadContact_RowCreated(object sender, GridViewRowEventArgs e)
    //{
    //    if (e.Row.RowType == DataControlRowType.Pager)
    //    {
    //        PresentationUtils.SetPagerButtonStates(GrdViewLeadContact, e.Row, this);
    //    }
    //}

    //protected void GrdViewLeadContact_RowDataBound(object sender, GridViewRowEventArgs e)
    //{
    //    if (e.Row.RowType == DataControlRowType.DataRow)
    //    {
    //        DataRow product = ((System.Data.DataRowView)e.Row.DataItem).Row;
    //    }
    //}


    protected void GrdViewLeadStage_RowDeleting(object sender, GridViewDeleteEventArgs e)
    {
        try
        {
            if (Session["contactDs"] != null)
            {
                string connStr = string.Empty;
                DataSet ds;

                /*
                if (Request.Cookies["Company"] != null)
                    connStr = System.Configuration.ConfigurationManager.ConnectionStrings[Request.Cookies["Company"].Value].ToString();
                else
                    Response.Redirect("~/Login.aspx");

                GridViewRow row = GrdViewLeadContact.Rows[e.RowIndex];
                string refID = row.Cells[0].Text;
                LeadBusinessLogic bl = new LeadBusinessLogic(connStr);
                bl.DeleteLeadContact(refID);*/

                //ds = (DataSet)Session["contactDs"];
                //ds.Tables[0].Rows[GrdViewLeadContact.Rows[e.RowIndex].DataItemIndex].Delete();
                //ds.Tables[0].AcceptChanges();
                //GrdViewLeadContact.DataSource = ds;
                //GrdViewLeadContact.DataBind();
                //Session["contactDs"] = ds;
            }
        }
        catch (Exception ex)
        {
            TroyLiteExceptionManager.HandleException(ex);
        }
    }

    protected void GrdViewLeadCompetitor_RowDeleting(object sender, GridViewDeleteEventArgs e)
    {
        SetRowDataCompetitors();
        if (ViewState["CurrentTable"] != null)
        {
            DataTable dt = (DataTable)ViewState["CurrentTable"];
            DataRow drCurrentRow = null;
            int rowIndex = Convert.ToInt32(e.RowIndex);
            if (dt.Rows.Count > 1)
            {
                dt.Rows.Remove(dt.Rows[rowIndex]);
                drCurrentRow = dt.NewRow();
                ViewState["CurrentTable"] = dt;
                GrdViewLeadCompetitor.DataSource = dt;
                GrdViewLeadCompetitor.DataBind();

                for (int i = 0; i < GrdViewLeadCompetitor.Rows.Count - 1; i++)
                {
                    GrdViewLeadCompetitor.Rows[i].Cells[0].Text = Convert.ToString(i + 1);
                }
                SetPreviousDataCompetitors();
            }
        }
    }

    protected void GrdViewLeadproduct_RowDeleting(object sender, GridViewDeleteEventArgs e)
    {
        SetRowDataProduct();
        if (ViewState["CurrentTable"] != null)
        {
            DataTable dt = (DataTable)ViewState["CurrentTable"];
            DataRow drCurrentRow = null;
            int rowIndex = Convert.ToInt32(e.RowIndex);
            if (dt.Rows.Count > 1)
            {
                dt.Rows.Remove(dt.Rows[rowIndex]);
                drCurrentRow = dt.NewRow();
                ViewState["CurrentTable"] = dt;
                GrdViewLeadproduct.DataSource = dt;
                GrdViewLeadproduct.DataBind();

                for (int i = 0; i < GrdViewLeadproduct.Rows.Count - 1; i++)
                {
                    GrdViewLeadproduct.Rows[i].Cells[0].Text = Convert.ToString(i + 1);
                }
                SetPreviousDataProduct();
            }
        }
    }

    protected void GrdViewLeadActivity_RowDeleting(object sender, GridViewDeleteEventArgs e)
    {
        SetRowDataActivity();
        if (ViewState["CurrentTable"] != null)
        {
            DataTable dt = (DataTable)ViewState["CurrentTable"];
            DataRow drCurrentRow = null;
            int rowIndex = Convert.ToInt32(e.RowIndex);
            if (dt.Rows.Count > 1)
            {
                dt.Rows.Remove(dt.Rows[rowIndex]);
                drCurrentRow = dt.NewRow();
                ViewState["CurrentTable"] = dt;
                GrdViewLeadActivity.DataSource = dt;
                GrdViewLeadActivity.DataBind();

                for (int i = 0; i < GrdViewLeadCompetitor.Rows.Count - 1; i++)
                {
                    GrdViewLeadActivity.Rows[i].Cells[0].Text = Convert.ToString(i + 1);
                }
                SetPreviousDataActivity();
            }
        }
    }

    protected void cmdCancelContact_Click(object sender, EventArgs e)
    {
        try
        {
            //ModalPopupContact.Hide();
            //pnlStage.Visible = false;
            //GrdViewLeadStage.Visible = true;
            //BtnAddStage.Visible = true;
        }
        catch (Exception ex)
        {
            TroyLiteExceptionManager.HandleException(ex);
        }
    }

    protected void cmdCancelCompetitor_Click(object sender, EventArgs e)
    {
        try
        {
            pnlCompetitor.Visible = false;
            GrdViewLeadCompetitor.Visible = true;
            // BtnAddCompetitor.Visible = true;
        }
        catch (Exception ex)
        {
            TroyLiteExceptionManager.HandleException(ex);
        }
    }

    protected void cmdCancelActivity_Click(object sender, EventArgs e)
    {
        try
        {
            pnlActivity.Visible = false;
            GrdViewLeadActivity.Visible = true;
            // BtnAddActivity.Visible = true;
        }
        catch (Exception ex)
        {
            TroyLiteExceptionManager.HandleException(ex);
        }
    }

    protected void cmdCancelproduct_Click(object sender, EventArgs e)
    {
        try
        {
            pnlproduct.Visible = false;
            GrdViewLeadproduct.Visible = true;
            //BtnAddproduct.Visible = true;
        }
        catch (Exception ex)
        {
            TroyLiteExceptionManager.HandleException(ex);
        }
    }

    protected void btnExportToExcel_Click(object sender, EventArgs e)
    {
        try
        {
            HtmlForm form = new HtmlForm();
            Response.Clear();
            Response.Buffer = true;
            string filename = "LeadManagement_" + DateTime.Now.ToString() + ".xls";

            LeadBusinessLogic bl = new LeadBusinessLogic(GetConnectionString());

            int leadid = 0;

            DataSet ds = bl.ListLeadMasterContacts(GetConnectionString(), txtSearch.Text, ddCriteria.SelectedValue);

            if (ds != null)
            {
                if (ds.Tables[0].Rows.Count > 0)
                {
                    DataTable dt = new DataTable();
                    dt.Columns.Add(new DataColumn("LeadID"));
                    dt.Columns.Add(new DataColumn("CreationDate"));
                    dt.Columns.Add(new DataColumn("ProspectCustName"));
                    dt.Columns.Add(new DataColumn("Address"));
                    dt.Columns.Add(new DataColumn("Mobile"));
                    dt.Columns.Add(new DataColumn("Landline"));
                    dt.Columns.Add(new DataColumn("Email"));
                    dt.Columns.Add(new DataColumn("ModeOfContact"));
                    dt.Columns.Add(new DataColumn("PersonalResponsible"));
                    dt.Columns.Add(new DataColumn("BusinessType"));
                    dt.Columns.Add(new DataColumn("Branch"));
                    dt.Columns.Add(new DataColumn("Status"));
                    dt.Columns.Add(new DataColumn("LastCompletedAction"));
                    dt.Columns.Add(new DataColumn("NextAction"));
                    dt.Columns.Add(new DataColumn("Category"));
                    dt.Columns.Add(new DataColumn("ContactedDate"));
                    dt.Columns.Add(new DataColumn("ContactSummary"));

                    DataRow dr_export1 = dt.NewRow();
                    dt.Rows.Add(dr_export1);

                    foreach (DataRow dr in ds.Tables[0].Rows)
                    {
                        if (leadid == Convert.ToInt32(dr["LeadId"]))
                        {
                            DataRow dr_export111 = dt.NewRow();
                            dr_export111["LeadID"] = "";
                            dr_export111["CreationDate"] = "";
                            dr_export111["ProspectCustName"] = "";
                            dr_export111["Address"] = "";
                            dr_export111["Mobile"] = "";
                            dr_export111["Landline"] = "";
                            dr_export111["Email"] = "";
                            dr_export111["ModeOfContact"] = "";
                            dr_export111["PersonalResponsible"] = "";
                            dr_export111["BusinessType"] = "";
                            dr_export111["Branch"] = "";
                            dr_export111["Email"] = "";
                            dr_export111["Status"] = "";
                            dr_export111["LastCompletedAction"] = "";
                            dr_export111["NextAction"] = "";
                            dr_export111["Category"] = "";
                            dr_export111["ContactedDate"] = dr["ContactedDate"];
                            dr_export111["ContactSummary"] = dr["ContactSummary"];
                            dt.Rows.Add(dr_export111);
                        }
                        else
                        {
                            DataRow dr_export = dt.NewRow();
                            dr_export["LeadID"] = dr["LeadID"];
                            dr_export["CreationDate"] = dr["CreationDate"];
                            dr_export["ProspectCustName"] = dr["ProspectCustName"];
                            dr_export["Address"] = dr["Address"];
                            dr_export["Mobile"] = dr["Mobile"];
                            dr_export["Landline"] = dr["Landline"];
                            dr_export["Email"] = dr["Email"];
                            dr_export["ModeOfContact"] = dr["ModeOfContact"];
                            dr_export["PersonalResponsible"] = dr["PersonalResponsible"];
                            dr_export["BusinessType"] = dr["BusinessType"];
                            dr_export["Branch"] = dr["Branch"];
                            dr_export["Email"] = dr["Email"];
                            dr_export["Status"] = dr["Status"];
                            dr_export["LastCompletedAction"] = dr["LastCompletedAction"];
                            dr_export["NextAction"] = dr["NextAction"];
                            dr_export["Category"] = dr["Category"];
                            dr_export["ContactedDate"] = dr["ContactedDate"];
                            dr_export["ContactSummary"] = dr["ContactSummary"];
                            dt.Rows.Add(dr_export);
                        }
                        leadid = Convert.ToInt32(dr["LeadId"]);
                    }

                    ExportToExcel(filename, dt);
                }
            }
            else
            {
                ScriptManager.RegisterStartupScript(Page, typeof(Button), "MyScript", "alert('No Data Found');", true);
            }

            //if (dt.Rows.Count > 0)
            //{
            //    DataTable dt = ds.Tables[0];

            //    System.IO.StringWriter tw = new System.IO.StringWriter();
            //    System.Web.UI.HtmlTextWriter hw = new System.Web.UI.HtmlTextWriter(tw);

            //    DataGrid dgGrid = new DataGrid();
            //    dgGrid.DataSource = dt;
            //    dgGrid.DataBind();

            //    //Get the HTML for the control.
            //    dgGrid.RenderControl(hw);

            //    //Write the HTML back to the browser.
            //    Response.ContentType = "application/vnd.ms-excel";
            //    Response.AppendHeader("Content-Disposition", "attachment; filename=" + filename + "");
            //    this.EnableViewState = false;
            //    Response.Write(tw.ToString());
            //    Response.End();
            //    UpdatePanelPage.Update();
            //}
        }
        catch (Exception ex)
        {
            TroyLiteExceptionManager.HandleException(ex);
        }
    }

    public void ExportToExcel(string filename, DataTable dt)
    {

        if (dt.Rows.Count > 0)
        {
            System.IO.StringWriter tw = new System.IO.StringWriter();
            System.Web.UI.HtmlTextWriter hw = new System.Web.UI.HtmlTextWriter(tw);
            DataGrid dgGrid = new DataGrid();
            dgGrid.DataSource = dt;
            dgGrid.DataBind();
            dgGrid.HeaderStyle.ForeColor = System.Drawing.Color.Black;
            dgGrid.HeaderStyle.BackColor = System.Drawing.Color.LightSkyBlue;
            dgGrid.HeaderStyle.BorderColor = System.Drawing.Color.RoyalBlue;
            dgGrid.HeaderStyle.Font.Bold = true;
            //Get the HTML for the control.
            dgGrid.RenderControl(hw);
            //Write the HTML back to the browser.
            Response.ContentType = "application/vnd.ms-excel";
            Response.AppendHeader("Content-Disposition", "attachment; filename=" + filename + "");
            this.EnableViewState = false;
            Response.Write(tw.ToString());
            Response.End();
        }
    }

    public override void VerifyRenderingInServerForm(Control control)
    {

    }

    protected void lnkAddBills_Click(object sender, EventArgs e)
    {
        /*
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
            var ds = bl.GetReceivedAmountId(conn, 0);
            GrdBills.DataSource = ds;
            GrdBills.DataBind();
            GrdBills.Rows[0].Visible = false;
            checkPendingBills(ds);
        }
        pnlEdit.Visible = true;
        GrdBills.FooterRow.Visible = true;
        lnkAddBills.Visible = true;
        Session["RMode"] = "Add";
        //lnkBtnAdd.Visible = false;*/
    }

    protected void GrdViewLead_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        try
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                BusinessLogic bl = new BusinessLogic(sDataSource);
                string connection = Request.Cookies["Company"].Value;
                string usernam = Request.Cookies["LoggedUserName"].Value;

                if (bl.CheckUserHaveEdit(usernam, "LDMNGT"))
                {
                    ((ImageButton)e.Row.FindControl("btnEdit")).Visible = false;
                    ((ImageButton)e.Row.FindControl("btnEditDisabled")).Visible = true;
                }

                //if (bl.CheckUserHaveDelete(usernam, "LDMNGT"))
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

    protected void GrdViewLead_RowDeleted(object sender, GridViewDeletedEventArgs e)
    {

    }

    protected void GrdViewLead_RowDeleting(object sender, GridViewDeleteEventArgs e)
    {
        try
        {
            //GridViewRow row = GrdViewLead.Rows[e.RowIndex];
            //string leadID = row.Cells[0].Text;


            //string userID = string.Empty;
            //userID = Page.User.Identity.Name;
            //LeadBusinessLogic bl = new LeadBusinessLogic(GetConnectionString());
            //bl.DeleteLeadMaster(leadID, userID);
            //ScriptManager.RegisterStartupScript(Page, Page.GetType(), Guid.NewGuid().ToString(), "alert('Lead Management Details Deleted Successfully')", true);
            //BindGrid("", "");
        }
        catch (Exception ex)
        {
            TroyLiteExceptionManager.HandleException(ex);
        }
    }

    protected void GrdViewLead_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        try
        {
            GrdViewLead.PageIndex = e.NewPageIndex;

            string textt = string.Empty;
            string dropd = string.Empty;

            textt = txtSearch.Text;
            dropd = ddCriteria.SelectedValue;

            BindGrid(textt, dropd);
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
            GrdViewLead.PageIndex = ((DropDownList)sender).SelectedIndex;

            string textt = string.Empty;
            string dropd = string.Empty;

            textt = txtSearch.Text;
            dropd = ddCriteria.SelectedValue;

            BindGrid(textt, dropd);
        }
        catch (Exception ex)
        {
            TroyLiteExceptionManager.HandleException(ex);
        }
    }

    protected void GrdViewLead_SelectedIndexChanged(object sender, EventArgs e)
    {
        try
        {
            GridViewRow row = GrdViewLead.SelectedRow;
            int LeadNo = Convert.ToInt32(GrdViewLead.SelectedDataKey.Value.ToString());
            LeadBusinessLogic bl = new LeadBusinessLogic(GetConnectionString());

            DataSet dsDetails = bl.GetLeadDetails(LeadNo);

            if (dsDetails != null && dsDetails.Tables[0].Rows.Count > 0)
            {
                Session["Date"] = "Edit";

                txtLeadNo.Text = dsDetails.Tables[0].Rows[0]["Lead_No"].ToString();
                txtCreationDate.Text = Convert.ToDateTime(dsDetails.Tables[0].Rows[0]["Start_Date"]).ToString("dd/MM/yyyy");

                if (dsDetails.Tables[0].Rows[0]["chec"].ToString() == "Y")
                {
                    cmbCustomer.SelectedValue = dsDetails.Tables[0].Rows[0]["Bp_Id"].ToString();
                    txtBPName.Visible = false;
                    cmbCustomer.Visible = true;
                    chk.Checked = true;
                }
                else
                {
                    txtBPName.Text = dsDetails.Tables[0].Rows[0]["Bp_Name"].ToString();
                    txtBPName.Visible = true;
                    cmbCustomer.Visible = false;
                    chk.Checked = false;
                }

                //chk.Checked = Convert.ToBoolean(dsDetails.Tables[0].Rows[0]["check"]);
                txtMobile.Text = dsDetails.Tables[0].Rows[0]["Mobile"].ToString();
                txtAddress.Text = dsDetails.Tables[0].Rows[0]["Address"].ToString();
                txtTelephone.Text = dsDetails.Tables[0].Rows[0]["Telephone"].ToString();
                txtLeadName.Text = dsDetails.Tables[0].Rows[0]["Lead_Name"].ToString();
                // txtTotalAmount.Text = dsDetails.Tables[0].Rows[0]["InvoicedAmt"].ToString();
                // txtContactName.Text = dsDetails.Tables[0].Rows[0]["Contact_Name"].ToString();
                // txtClosingPer.Text = dsDetails.Tables[0].Rows[0]["Closing_Per"].ToString();

                if ((Convert.ToDateTime(dsDetails.Tables[0].Rows[0]["Closing_Date"])) == Convert.ToDateTime("01/01/2000"))
                {
                    txtClosingDate.Text = "";
                }
                else
                {
                    txtClosingDate.Text = Convert.ToDateTime(dsDetails.Tables[0].Rows[0]["Closing_Date"]).ToString("dd/MM/yyyy");
                }

                drpLeadStatus.SelectedValue = dsDetails.Tables[0].Rows[0]["Lead_Status"].ToString();
                drpStatus.SelectedValue = dsDetails.Tables[0].Rows[0]["Doc_Status"].ToString();
                drpIncharge.SelectedValue = dsDetails.Tables[0].Rows[0]["Emp_Id"].ToString();
                //txtBranch.Text = dsDetails.Tables[0].Rows[0]["Branch"].ToString();
                drpLeadStatus.Enabled = true;
                drpStatus.Enabled = true;

                DataSet ds = bl.GetLeadPotential(LeadNo);

                if (ds != null && ds.Tables[0].Rows.Count > 0)
                {
                    // txtPredictedClosing.Text = ds.Tables[0].Rows[0]["Predicted_Closing"].ToString();
                    // drpPredictedClosingPeriod.SelectedValue = ds.Tables[0].Rows[0]["Predicted_Closing_Period"].ToString();
                    // txtPredictedClosingDate.Text = Convert.ToDateTime(ds.Tables[0].Rows[0]["Predicted_Closing_Date"]).ToString("dd/MM/yyyy");
                    // drpInterestLevel.SelectedValue = ds.Tables[0].Rows[0]["Interest_Level"].ToString();
                    // txtPotentialPotAmount.Text = ds.Tables[0].Rows[0]["Potential_Amount"].ToString();
                    // txtPotentialWeightedAmount.Text = ds.Tables[0].Rows[0]["Weighted_Amount"].ToString();
                }


                DataSet dsStages = bl.GetLeadStages(LeadNo);

                if (dsStages != null && dsStages.Tables[0].Rows.Count > 0)
                {
                    // GrdViewLeadStage.DataSource = dsStages.Tables[0];
                    //GrdViewLeadStage.DataBind();
                    Session["contactDs"] = dsStages;
                }
                else
                {
                    //GrdViewLeadStage.DataSource = null;
                    //GrdViewLeadStage.DataBind();
                    Session["contactDs"] = null;
                }
                //GrdViewLeadStage.Visible = true;
                //BtnAddStage.Visible = true;
                //pnlStage.Visible = false;


                DataSet dsCompetitor = bl.GetLeadCompetitor(LeadNo);

                if (dsCompetitor != null && dsCompetitor.Tables[0].Rows.Count > 0)
                {
                    GrdViewLeadCompetitor.DataSource = dsCompetitor.Tables[0];
                    GrdViewLeadCompetitor.DataBind();
                    Session["CompetitorDs"] = dsCompetitor;
                }
                else
                {
                    GrdViewLeadCompetitor.DataSource = null;
                    GrdViewLeadCompetitor.DataBind();
                    Session["CompetitorDs"] = null;
                }
                GrdViewLeadCompetitor.Visible = true;
                // BtnAddCompetitor.Visible = true;
                pnlCompetitor.Visible = false;


                DataSet dsActivity = bl.GetLeadActivity(LeadNo);

                if (dsActivity != null && dsActivity.Tables[0].Rows.Count > 0)
                {
                    GrdViewLeadActivity.DataSource = dsActivity.Tables[0];
                    GrdViewLeadActivity.DataBind();
                    Session["ActivityDs"] = dsActivity;
                }
                else
                {
                    GrdViewLeadActivity.DataSource = null;
                    GrdViewLeadActivity.DataBind();
                    Session["ActivityDs"] = null;
                }
                GrdViewLeadActivity.Visible = true;
                //  BtnAddActivity.Visible = true;
                pnlActivity.Visible = false;

                DataSet dsProduct = bl.GetLeadProduct(LeadNo);

                if (dsProduct != null && dsProduct.Tables[0].Rows.Count > 0)
                {
                    GrdViewLeadproduct.DataSource = dsProduct.Tables[0];
                    GrdViewLeadproduct.DataBind();
                    Session["ProductDs"] = dsProduct;
                }
                else
                {
                    GrdViewLeadproduct.DataSource = null;
                    GrdViewLeadproduct.DataBind();
                    Session["ProductDs"] = null;
                }
                GrdViewLeadproduct.Visible = true;
                // BtnAddproduct.Visible = true;
                pnlproduct.Visible = false;


                UpdateButton.Visible = true;
                AddButton.Visible = false;
                ModalPopupExtender2.Show();
            }
        }
        catch (Exception ex)
        {
            TroyLiteExceptionManager.HandleException(ex);
        }
    }

    protected void GrdViewLead_RowCommand(object sender, GridViewCommandEventArgs e)
    {

    }

    protected void GrdViewLead_RowCreated(object sender, GridViewRowEventArgs e)
    {
        try
        {
            if (e.Row.RowType == DataControlRowType.Pager)
            {
                PresentationUtils.SetPagerButtonStates(GrdViewLead, e.Row, this);
            }
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
            ModalPopupExtender2.Hide();
        }
        catch (Exception ex)
        {
            TroyLiteExceptionManager.HandleException(ex);
        }
    }

    protected void UpdateButton_Click(object sender, EventArgs e)
    {
        DateTime startDate;
        DateTime ClosingDate;
        int LeadNo = 0;
        string LeadName = string.Empty;
        string address = string.Empty;
        string mobile = string.Empty;
        string Telephone = string.Empty;
        string BpName = string.Empty;
        string EmpName = string.Empty;
        string LeadStatus = string.Empty;
        string Status = string.Empty;
        string branch = string.Empty;
        string status = string.Empty;
        string ContactName = string.Empty;
        DataSet dsStages;
        int BpId = 0;
        int EmpId = 0;
        double TotalAmount = 0;
        int ClosingPer = 0;
        DataSet dsCompetitor;
        DataSet dsActivity;

        DataSet dsProduct;


        DateTime PredictedClosingDate;
        int PredictedClosing = 0;
        string PredictedClosingPeriod = string.Empty;
        string InterestLevel = string.Empty;
        double PotentialPotAmount = 0;
        double PotentialWeightedAmount = 0;

        try
        {
            if (Page.IsValid)
            {
                if (txtLeadNo.Text != string.Empty)
                    LeadNo = int.Parse(txtLeadNo.Text);

                startDate = DateTime.Parse(txtCreationDate.Text);
                LeadName = txtLeadName.Text;
                address = txtAddress.Text;
                mobile = txtMobile.Text;
                Telephone = txtTelephone.Text;

                string check = string.Empty;

                if (chk.Checked == false)
                {
                    BpName = txtBPName.Text;
                    BpId = 0;
                    check = "N";
                }
                else
                {
                    BpName = cmbCustomer.SelectedItem.Text;
                    BpId = Convert.ToInt32(cmbCustomer.SelectedValue);
                    check = "Y";
                }

                // ContactName = txtContactName.Text;
                EmpId = Convert.ToInt32(drpIncharge.SelectedValue);
                EmpName = drpIncharge.SelectedItem.Text;
                Status = drpStatus.SelectedValue;
                // branch = txtBranch.Text;
                LeadStatus = drpLeadStatus.SelectedValue;
                // TotalAmount = Convert.ToDouble(txtTotalAmount.Text);
                //ClosingPer = Convert.ToInt32(txtClosingPer.Text);

                if (txtClosingDate.Text == "")
                {
                    ClosingDate = DateTime.Parse("01/01/2000");
                }
                else
                {
                    ClosingDate = DateTime.Parse(txtClosingDate.Text);
                }

                // PredictedClosing = Convert.ToInt32(txtPredictedClosing.Text);
                // PredictedClosingDate = DateTime.Parse(txtPredictedClosingDate.Text);
                // PotentialPotAmount = Convert.ToDouble(txtPotentialPotAmount.Text);
                // PotentialWeightedAmount = Convert.ToDouble(txtPotentialWeightedAmount.Text);
                // PredictedClosingPeriod = drpPredictedClosingPeriod.SelectedValue;
                // InterestLevel = drpInterestLevel.SelectedValue;

                string connStr = GetConnectionString();
                LeadBusinessLogic bl = new LeadBusinessLogic(connStr);
                string usernam = Request.Cookies["LoggedUserName"].Value;



                dsStages = (DataSet)Session["contactDs"];
                dsCompetitor = (DataSet)Session["CompetitorDs"];
                dsActivity = (DataSet)Session["ActivityDs"];

                dsProduct = (DataSet)Session["ProductDs"];

                // as per new functionality remove PredictedClosing, PredictedClosingDate
                //bl.UpdateLead(LeadNo, startDate, LeadName, address, mobile, Telephone, BpName, BpId, ContactName, EmpId, EmpName, Status, branch, LeadStatus, TotalAmount, ClosingPer, ClosingDate, PredictedClosing, PredictedClosingDate, PotentialPotAmount, PotentialWeightedAmount, PredictedClosingPeriod, InterestLevel, usernam, dsStages, dsCompetitor, dsActivity, dsProduct, check);

                //GrdViewLead.DataBind();
                //System.Threading.Thread.Sleep(1000);
                ScriptManager.RegisterStartupScript(Page, Page.GetType(), Guid.NewGuid().ToString(), "alert('Lead Details Updated successfully.')", true);

                //UpdatePanelPage.Update();
                BindGrid("", "");

                ModalPopupExtender2.Hide();


                return;

            }
        }
        catch (Exception ex)
        {
            TroyLiteExceptionManager.HandleException(ex);
            return;
        }
    }

    protected void AddTheRef_Click(object sender, EventArgs e)
    {
        try
        {
            Response.Redirect("LeadReference.aspx");
        }
        catch (Exception ex)
        {
            TroyLiteExceptionManager.HandleException(ex);
        }
    }

    protected void AddButton_Click(object sender, EventArgs e)
    {
        DateTime startDate;
        DateTime ClosingDate;
        int LeadNo = 0;
        string LeadName = string.Empty;
        string address = string.Empty;
        string mobile = string.Empty;
        string Telephone = string.Empty;
        string BpName = string.Empty;
        string EmpName = string.Empty;
        string LeadStatus = string.Empty;
        string Status = string.Empty;
        string branch = string.Empty;
        string status = string.Empty;
        string ContactName = string.Empty;
        string info1 = string.Empty;

        DataSet dsStages;
        DataSet dss;
        DataSet dss1;
        DataSet dss2; 
        int BpId = 0;
        int EmpId = 0;
        int info3 = 0;
        int info4 = 0;
        int businesstype = 0;
        int category = 0;
        int area = 0;
        int intLevel = 0;

        double TotalAmount = 0;
        int ClosingPer = 0;
        DataSet dsActivity;
        DataSet dsCompetitor;

        DataSet dsProduct;
        DateTime PredictedClosingDate;
        int PredictedClosing = 0;
        string PredictedClosingPeriod = string.Empty;
        string InterestLevel = string.Empty;
        double PotentialPotAmount = 0;
        double PotentialWeightedAmount = 0;

        try
        {

            if (Page.IsValid)
            {
                //if (txtLeadNo.Text != string.Empty)
                //    LeadNo = int.Parse(txtLeadNo.Text);

                startDate = DateTime.Parse(txtCreationDate.Text);
                LeadName = txtLeadName.Text;//lead ref
                address = txtAddress.Text;//
                mobile = txtMobile.Text;//
                Telephone = txtTelephone.Text;//

                string check = string.Empty;

                if (chk.Checked == false)//
                {
                    BpName = txtBPName.Text;
                    BpId = 0;
                    check = "N";
                }
                else//
                {
                    BpName = cmbCustomer.SelectedItem.Text;
                    BpId = Convert.ToInt32(cmbCustomer.SelectedValue);
                    check = "Y";
                }

                ContactName = txtContactName.Text;
                EmpId = Convert.ToInt32(drpIncharge.SelectedValue);//
                EmpName = drpIncharge.SelectedItem.Text;
                Status = drpStatus.SelectedValue;//
                LeadStatus = drpLeadStatus.SelectedValue;//   

                if (txtClosingDate.Text == "")//
                {
                    ClosingDate = DateTime.Parse("01/01/2000");
                }
                else
                {
                    ClosingDate = DateTime.Parse(txtClosingDate.Text);
                }

                PredictedClosingDate = DateTime.Parse(txtPredictedClosingDate.Text);//
                info1 = txtInformation1.Text;
                info3 = Convert.ToInt32(drpInformation3.SelectedValue);//
                info4 = Convert.ToInt32(drpInformation4.SelectedValue);//
                businesstype = Convert.ToInt32(drpBusinessType.SelectedValue);//
                category = Convert.ToInt32(drpCategory.SelectedValue);//
                area = Convert.ToInt32(drpArea.SelectedValue);//
                intLevel = Convert.ToInt32(drpIntLevel.SelectedValue);//


                string usernam = Request.Cookies["LoggedUserName"].Value;

                dsStages = (DataSet)Session["contactDs"];


                dss = (DataSet)Session["ProductDs"];
                //&&&&&& Product tab Insert Dataset &&&&&&&&&&&&&&&&& 
                if (Session["ProductDs"] == null) // New code
                {
                    dss = (DataSet)Session["ProductDs"];

                    if (dss == null)
                    {

                        for (int vLoop = 0; vLoop < GrdViewLeadproduct.Rows.Count; vLoop++)
                        {
                            DropDownList drpProduct = (DropDownList)GrdViewLeadproduct.Rows[vLoop].FindControl("drpproduct");
                            TextBox txtPrdID = (TextBox)GrdViewLeadproduct.Rows[vLoop].FindControl("txtPrdId");
                        }


                        //DataSet dss;
                        DataTable dt;
                        DataRow drNew;

                        DataColumn dc;

                        dss = new DataSet();
                        dt = new DataTable();

                        dc = new DataColumn("Prd");
                        dt.Columns.Add(dc);

                        dc = new DataColumn("PrdID");
                        dt.Columns.Add(dc);                       

                        dss.Tables.Add(dt);

                        for (int vLoop = 0; vLoop < GrdViewLeadproduct.Rows.Count; vLoop++)
                        {
                            DropDownList drpProduct = (DropDownList)GrdViewLeadproduct.Rows[vLoop].FindControl("drpproduct");
                            TextBox txtPrdID = (TextBox)GrdViewLeadproduct.Rows[vLoop].FindControl("txtPrdId");



                            drNew = dt.NewRow();
                            drNew["Prd"] = Convert.ToString(drpProduct.SelectedItem.Text);
                            drNew["PrdID"] = txtPrdID.Text;
                            dss.Tables[0].Rows.Add(drNew);
                        }
                    }
                }
                //&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&


                dss1 = (DataSet)Session["CompetitorDs"];
                //&&&&&& Competitors tab Insert Dataset &&&&&&&&&&&&&&&&& 
                if (Session["CompetitorDs"] == null) // New code
                {
                    dss1 = (DataSet)Session["CompetitorDs"];

                    if (dss1 == null)
                    {
                        for (int vLoop = 0; vLoop < GrdViewLeadCompetitor.Rows.Count; vLoop++)
                        {
                            TextBox txtComName = (TextBox)GrdViewLeadCompetitor.Rows[vLoop].FindControl("txtComeName");
                            TextBox txtThrlvl = (TextBox)GrdViewLeadCompetitor.Rows[vLoop].FindControl("txtThrLvl");
                            TextBox txtOuestrweak = (TextBox)GrdViewLeadCompetitor.Rows[vLoop].FindControl("txtOurStrWeakness");
                            TextBox txtComstrweak = (TextBox)GrdViewLeadCompetitor.Rows[vLoop].FindControl("txtCompStrWeakness");
                            TextBox txtremarks = (TextBox)GrdViewLeadCompetitor.Rows[vLoop].FindControl("txtRemarks");
                        }

                        //DataSet dss1;
                        DataTable dt;
                        DataRow drNew;

                        DataColumn dc;

                        dss1 = new DataSet();
                        dt = new DataTable();

                        dc = new DataColumn("ComName");
                        dt.Columns.Add(dc);

                        dc = new DataColumn("ThrLvl");
                        dt.Columns.Add(dc);

                        dc = new DataColumn("OurStrWeak");
                        dt.Columns.Add(dc);

                        dc = new DataColumn("ComStrWeak");
                        dt.Columns.Add(dc);

                        dc = new DataColumn("Remarks");
                        dt.Columns.Add(dc);

                        dss1.Tables.Add(dt);

                        for (int vLoop = 0; vLoop < GrdViewLeadproduct.Rows.Count; vLoop++)
                        {
                            TextBox txtComName = (TextBox)GrdViewLeadCompetitor.Rows[vLoop].FindControl("txtComeName");
                            TextBox txtThrlvl = (TextBox)GrdViewLeadCompetitor.Rows[vLoop].FindControl("txtThrLvl");
                            TextBox txtOuestrweak = (TextBox)GrdViewLeadCompetitor.Rows[vLoop].FindControl("txtOurStrWeakness");
                            TextBox txtComstrweak = (TextBox)GrdViewLeadCompetitor.Rows[vLoop].FindControl("txtCompStrWeakness");
                            TextBox txtremarks = (TextBox)GrdViewLeadCompetitor.Rows[vLoop].FindControl("txtRemarks");


                            drNew = dt.NewRow();
                            drNew["ComName"] = txtComName.Text;
                            drNew["ThrLvl"] = txtThrlvl.Text;
                            drNew["OurStrWeak"] = txtOuestrweak.Text;
                            drNew["ComStrWeak"] = txtComstrweak.Text;
                            drNew["Remarks"] = txtremarks.Text;
                            dss1.Tables[0].Rows.Add(drNew);
                        }
                    }
                }
                //&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&

                dss2 = (DataSet)Session["ActivityDs"];
                //&&&&&& Activity tab Insert Dataset &&&&&&&&&&&&&&&&& 
                if (Session["CompetitorDs"] == null) // New code
                {
                    dss2 = (DataSet)Session["ActivityDs"];

                    if (dss2 == null)
                    {
                        for (int vLoop = 0; vLoop < GrdViewLeadActivity.Rows.Count; vLoop++)
                        {
                            DropDownList drpactivityName = (DropDownList)GrdViewLeadActivity.Rows[vLoop].FindControl("drpactivityName");
                            TextBox txtactivityLoc = (TextBox)GrdViewLeadActivity.Rows[vLoop].FindControl("txtActiLoc");
                            TextBox txtactivityDate = (TextBox)GrdViewLeadActivity.Rows[vLoop].FindControl("txtActiDate");
                            DropDownList drpnxtActivity = (DropDownList)GrdViewLeadActivity.Rows[vLoop].FindControl("drpnxtActivity");
                            TextBox txtnxtActDate = (TextBox)GrdViewLeadActivity.Rows[vLoop].FindControl("txtNxtActyDate");
                            DropDownList drpemployee = (DropDownList)GrdViewLeadActivity.Rows[vLoop].FindControl("drpemployee");
                            TextBox txtmodeofCnt = (TextBox)GrdViewLeadActivity.Rows[vLoop].FindControl("txtModrofcnt");
                            DropDownList drpinfo2 = (DropDownList)GrdViewLeadActivity.Rows[vLoop].FindControl("drpinfo2");
                            DropDownList drpinfo5 = (DropDownList)GrdViewLeadActivity.Rows[vLoop].FindControl("drpinfo5");
                            TextBox txtremarks = (TextBox)GrdViewLeadActivity.Rows[vLoop].FindControl("txtremarks");
                        }

                       // DataSet dss2;
                        DataTable dt;
                        DataRow drNew;

                        DataColumn dc;

                        dss2 = new DataSet();
                        dt = new DataTable();

                        dc = new DataColumn("ActName");
                        dt.Columns.Add(dc);

                        dc = new DataColumn("ActLoc");
                        dt.Columns.Add(dc);

                        dc = new DataColumn("ActDate");
                        dt.Columns.Add(dc);

                        dc = new DataColumn("NxtAct");
                        dt.Columns.Add(dc);

                        dc = new DataColumn("NxtActDte");
                        dt.Columns.Add(dc);

                        dc = new DataColumn("Emp");
                        dt.Columns.Add(dc);

                        dc = new DataColumn("MdeofCnt");
                        dt.Columns.Add(dc);

                        dc = new DataColumn("Info2");
                        dt.Columns.Add(dc);

                        dc = new DataColumn("Info5");
                        dt.Columns.Add(dc);

                        dc = new DataColumn("Remarks");
                        dt.Columns.Add(dc);

                        dss2.Tables.Add(dt);

                        for (int vLoop = 0; vLoop < GrdViewLeadActivity.Rows.Count; vLoop++)
                        {
                            DropDownList drpactivityName = (DropDownList)GrdViewLeadActivity.Rows[vLoop].FindControl("drpactivityName");
                            TextBox txtactivityLoc = (TextBox)GrdViewLeadActivity.Rows[vLoop].FindControl("txtActiLoc");
                            TextBox txtactivityDate = (TextBox)GrdViewLeadActivity.Rows[vLoop].FindControl("txtActiDate");
                            DropDownList drpnxtActivity = (DropDownList)GrdViewLeadActivity.Rows[vLoop].FindControl("drpnxtActivity");
                            TextBox txtnxtActDate = (TextBox)GrdViewLeadActivity.Rows[vLoop].FindControl("txtNxtActyDate");
                            DropDownList drpemployee = (DropDownList)GrdViewLeadActivity.Rows[vLoop].FindControl("drpemployee");
                            TextBox txtmodeofCnt = (TextBox)GrdViewLeadActivity.Rows[vLoop].FindControl("txtModrofcnt");
                            DropDownList drpinfo2 = (DropDownList)GrdViewLeadActivity.Rows[vLoop].FindControl("drpinfo2");
                            DropDownList drpinfo5 = (DropDownList)GrdViewLeadActivity.Rows[vLoop].FindControl("drpinfo5");
                            TextBox txtremarks = (TextBox)GrdViewLeadActivity.Rows[vLoop].FindControl("txtremarks");


                            drNew = dt.NewRow();
                            drNew["ActName"] = Convert.ToString(drpactivityName.SelectedItem.Value);
                            drNew["ActLoc"] = txtactivityLoc.Text;
                            drNew["ActDate"] = txtactivityDate.Text;
                            drNew["NxtAct"] = Convert.ToString(drpnxtActivity.SelectedItem.Value);
                            drNew["NxtActDte"] = txtnxtActDate.Text;
                            drNew["Emp"] = Convert.ToString(drpemployee.SelectedItem.Value);
                            drNew["MdeofCnt"] = txtmodeofCnt.Text;
                            drNew["Info2"] = Convert.ToString(drpinfo2.SelectedItem.Value);
                            drNew["Info5"] = Convert.ToString(drpinfo5.SelectedItem.Value);
                            drNew["Remarks"] = txtremarks.Text;
                            dss2.Tables[0].Rows.Add(drNew);
                        }
                    }
                }
                //&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&



                string connStr = GetConnectionString();

                LeadBusinessLogic bl = new LeadBusinessLogic(connStr);
                //pass dss for prodct tab
                //pass dss1 for completor tab
                //pass dss2 for activity tab

                bl.AddLead(LeadNo, startDate, LeadName, address, mobile, Telephone, BpName, BpId, ContactName, EmpId, EmpName, Status, branch, LeadStatus, TotalAmount, ClosingPer, ClosingDate, PredictedClosing, PredictedClosingDate, PotentialPotAmount, PotentialWeightedAmount, PredictedClosingPeriod, InterestLevel, usernam, dsStages, dss1, dss2, dss, check);

                GrdViewLead.DataBind();

                ScriptManager.RegisterStartupScript(Page, Page.GetType(), Guid.NewGuid().ToString(), "alert('Lead Details saved successfully.')", true);

                BindGrid("", "");
                UpdatePanelPage.Update();
                ModalPopupExtender2.Hide();

                return;
            }
        }
        catch (Exception ex)
        {
            TroyLiteExceptionManager.HandleException(ex);
            return;
        }
    }
    protected void BtnClearFilter_Click(object sender, EventArgs e)
    {
        txtSearch.Text = "";
        ddCriteria.SelectedIndex = 0;
        BindGrid("", "");
    }


    private void AddNewRowProduct()
    {
        int rowIndex = 0;

        if (ViewState["CurrentTable1"] != null)
        {
            DataTable dtCurrentTable1 = (DataTable)ViewState["CurrentTable1"];
            DataRow drCurrentRow = null;
            if (dtCurrentTable1.Rows.Count > 0)
            {
                for (int i = 1; i <= dtCurrentTable1.Rows.Count; i++)
                {
                    DropDownList drpProduct =
                     (DropDownList)GrdViewLeadproduct.Rows[rowIndex].Cells[1].FindControl("drpproduct");
                    TextBox txtPrdID =
                      (TextBox)GrdViewLeadproduct.Rows[rowIndex].Cells[2].FindControl("txtPrdId");

                    drCurrentRow = dtCurrentTable1.NewRow();
                    drCurrentRow["RowNumber"] = i + 1;

                    dtCurrentTable1.Rows[i - 1]["Col1"] = drpProduct.SelectedValue;
                    dtCurrentTable1.Rows[i - 1]["Col2"] = txtPrdID.Text;


                    rowIndex++;
                }
                dtCurrentTable1.Rows.Add(drCurrentRow);
                ViewState["CurrentTable"] = dtCurrentTable1;

                GrdViewLeadproduct.DataSource = dtCurrentTable1;
                GrdViewLeadproduct.DataBind();
            }
        }
        else
        {
            Response.Write("ViewState is null");
        }
        SetPreviousDataProduct();
    }

    private void AddNewRowCompetitors()
    {
        int rowIndex = 0;

        if (ViewState["CurrentTable2"] != null)
        {
            DataTable dtCurrentTable2 = (DataTable)ViewState["CurrentTable2"];
            DataRow drCurrentRow = null;
            if (dtCurrentTable2.Rows.Count > 0)
            {
                for (int i = 1; i <= dtCurrentTable2.Rows.Count; i++)
                {
                    TextBox txtComName =
                      (TextBox)GrdViewLeadCompetitor.Rows[rowIndex].Cells[1].FindControl("txtComeName");
                    TextBox txtThrlvl =
                      (TextBox)GrdViewLeadCompetitor.Rows[rowIndex].Cells[2].FindControl("txtThrLvl");
                    TextBox txtOuestrweak =
                      (TextBox)GrdViewLeadCompetitor.Rows[rowIndex].Cells[2].FindControl("txtOurStrWeakness");
                    TextBox txtComstrweak =
                      (TextBox)GrdViewLeadCompetitor.Rows[rowIndex].Cells[3].FindControl("txtCompStrWeakness");
                    TextBox txtremarks =
                      (TextBox)GrdViewLeadCompetitor.Rows[rowIndex].Cells[4].FindControl("txtRemarks");


                    drCurrentRow = dtCurrentTable2.NewRow();
                    drCurrentRow["RowNumber"] = i + 1;

                    dtCurrentTable2.Rows[i - 1]["Col1"] = txtComName.Text;
                    dtCurrentTable2.Rows[i - 1]["Col2"] = txtThrlvl.Text;
                    dtCurrentTable2.Rows[i - 1]["Col3"] = txtOuestrweak.Text;
                    dtCurrentTable2.Rows[i - 1]["Col4"] = txtComstrweak.Text;
                    dtCurrentTable2.Rows[i - 1]["Col5"] = txtremarks.Text;

                    rowIndex++;
                }
                dtCurrentTable2.Rows.Add(drCurrentRow);
                ViewState["CurrentTable2"] = dtCurrentTable2;

                GrdViewLeadCompetitor.DataSource = dtCurrentTable2;
                GrdViewLeadCompetitor.DataBind();
            }
        }
        else
        {
            Response.Write("ViewState is null");
        }
        SetPreviousDataCompetitors();
    }

    private void AddNewRowActivity()
    {
        int rowIndex = 0;

        if (ViewState["CurrentTable3"] != null)
        {
            DataTable dtCurrentTable3 = (DataTable)ViewState["CurrentTable3"];
            DataRow drCurrentRow = null;
            if (dtCurrentTable3.Rows.Count > 0)
            {
                for (int i = 1; i <= dtCurrentTable3.Rows.Count; i++)
                {
                    DropDownList drpactivityName =
                     (DropDownList)GrdViewLeadActivity.Rows[rowIndex].Cells[1].FindControl("drpactivityName");
                    TextBox txtactivityLoc =
                      (TextBox)GrdViewLeadActivity.Rows[rowIndex].Cells[2].FindControl("txtActiLoc");
                    TextBox txtactivityDate =
                      (TextBox)GrdViewLeadActivity.Rows[rowIndex].Cells[2].FindControl("txtActiDate");
                    DropDownList drpnxtActivity =
                     (DropDownList)GrdViewLeadActivity.Rows[rowIndex].Cells[1].FindControl("drpnxtActivity");
                    TextBox txtnxtActDate =
                     (TextBox)GrdViewLeadActivity.Rows[rowIndex].Cells[3].FindControl("txtNxtActyDate");
                    DropDownList drpemployee =
                     (DropDownList)GrdViewLeadActivity.Rows[rowIndex].Cells[1].FindControl("drpemployee");
                    TextBox txtmodeofCnt =
                      (TextBox)GrdViewLeadActivity.Rows[rowIndex].Cells[4].FindControl("txtModrofcnt");
                    DropDownList drpinfo2 =
                       (DropDownList)GrdViewLeadActivity.Rows[rowIndex].Cells[1].FindControl("drpinfo2");
                    DropDownList drpinfo5 =
                       (DropDownList)GrdViewLeadActivity.Rows[rowIndex].Cells[1].FindControl("drpinfo5");
                    TextBox txtremarks =
                      (TextBox)GrdViewLeadActivity.Rows[rowIndex].Cells[4].FindControl("txtremarks");


                    drCurrentRow = dtCurrentTable3.NewRow();
                    drCurrentRow["RowNumber"] = i + 1;

                    dtCurrentTable3.Rows[i - 1]["Col1"] = drpactivityName.SelectedValue;
                    dtCurrentTable3.Rows[i - 1]["Col2"] = txtactivityLoc.Text;
                    dtCurrentTable3.Rows[i - 1]["Col3"] = txtactivityDate.Text;
                    dtCurrentTable3.Rows[i - 1]["Col4"] = drpnxtActivity.SelectedValue;
                    dtCurrentTable3.Rows[i - 1]["Col5"] = txtnxtActDate.Text;
                    dtCurrentTable3.Rows[i - 1]["Col6"] = drpemployee.SelectedValue;
                    dtCurrentTable3.Rows[i - 1]["Col7"] = txtmodeofCnt.Text;
                    dtCurrentTable3.Rows[i - 1]["Col8"] = drpinfo2.SelectedValue;
                    dtCurrentTable3.Rows[i - 1]["Col9"] = drpinfo5.SelectedValue;
                    dtCurrentTable3.Rows[i - 1]["Col10"] = txtremarks.Text;

                    rowIndex++;
                }
                dtCurrentTable3.Rows.Add(drCurrentRow);
                ViewState["CurrentTable3"] = dtCurrentTable3;

                GrdViewLeadActivity.DataSource = dtCurrentTable3;
                GrdViewLeadActivity.DataBind();
            }
        }
        else
        {
            Response.Write("ViewState is null");
        }
        SetPreviousDataActivity();
    }

    private void SetPreviousDataProduct()
    {
        int rowIndex = 0;
        if (ViewState["CurrentTable1"] != null)
        {
            DataTable dt = (DataTable)ViewState["CurrentTable1"];
            if (dt.Rows.Count > 0)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    DropDownList drpProduct =
                     (DropDownList)GrdViewLeadproduct.Rows[rowIndex].Cells[1].FindControl("drpproduct");
                    TextBox txtPrdID =
                      (TextBox)GrdViewLeadproduct.Rows[rowIndex].Cells[2].FindControl("txtPrdId");

                    drpProduct.SelectedValue = dt.Rows[i]["Col1"].ToString();
                    txtPrdID.Text = dt.Rows[i]["Col2"].ToString();

                    rowIndex++;
                }
            }
        }
    }

    private void SetPreviousDataCompetitors()
    {
        int rowIndex = 0;
        if (ViewState["CurrentTable2"] != null)
        {
            DataTable dt = (DataTable)ViewState["CurrentTable2"];
            if (dt.Rows.Count > 0)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    TextBox txtComName =
                     (TextBox)GrdViewLeadCompetitor.Rows[rowIndex].Cells[2].FindControl("txtComeName");
                    TextBox txtThrlvl =
                      (TextBox)GrdViewLeadCompetitor.Rows[rowIndex].Cells[2].FindControl("txtThrLvl");
                    TextBox txtOuestrweak =
                      (TextBox)GrdViewLeadCompetitor.Rows[rowIndex].Cells[2].FindControl("txtOurStrWeakness");
                    TextBox txtComstrweak =
                      (TextBox)GrdViewLeadCompetitor.Rows[rowIndex].Cells[2].FindControl("txtCompStrWeakness");
                    TextBox txtremarks =
                      (TextBox)GrdViewLeadCompetitor.Rows[rowIndex].Cells[2].FindControl("txtRemarks");


                    txtComName.Text = dt.Rows[i]["Col1"].ToString();
                    txtThrlvl.Text = dt.Rows[i]["Col2"].ToString();
                    txtOuestrweak.Text = dt.Rows[i]["Col3"].ToString();
                    txtComstrweak.Text = dt.Rows[i]["Col4"].ToString();
                    txtremarks.Text = dt.Rows[i]["Col5"].ToString();

                    rowIndex++;
                }
            }
        }
    }

    private void SetPreviousDataActivity()
    {
        int rowIndex = 0;
        if (ViewState["CurrentTable3"] != null)
        {
            DataTable dt = (DataTable)ViewState["CurrentTable3"];
            if (dt.Rows.Count > 0)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    DropDownList drpactivityName =
                    (DropDownList)GrdViewLeadActivity.Rows[rowIndex].Cells[1].FindControl("drpactivityName");
                    TextBox txtactivityLoc =
                      (TextBox)GrdViewLeadActivity.Rows[rowIndex].Cells[2].FindControl("txtActiLoc");
                    TextBox txtactivityDate =
                      (TextBox)GrdViewLeadActivity.Rows[rowIndex].Cells[2].FindControl("txtActiDate");
                    DropDownList drpnxtActivity =
                     (DropDownList)GrdViewLeadActivity.Rows[rowIndex].Cells[1].FindControl("drpnxtActivity");
                    TextBox txtnxtActDate =
                     (TextBox)GrdViewLeadActivity.Rows[rowIndex].Cells[3].FindControl("txtNxtActyDate");
                    DropDownList drpemployee =
                     (DropDownList)GrdViewLeadActivity.Rows[rowIndex].Cells[1].FindControl("drpemployee");
                    TextBox txtmodeofCnt =
                      (TextBox)GrdViewLeadActivity.Rows[rowIndex].Cells[4].FindControl("txtModrofcnt");
                    DropDownList drpinfo2 =
                       (DropDownList)GrdViewLeadActivity.Rows[rowIndex].Cells[1].FindControl("drpinfo2");
                    DropDownList drpinfo5 =
                       (DropDownList)GrdViewLeadActivity.Rows[rowIndex].Cells[1].FindControl("drpinfo5");
                    TextBox txtremarks =
                      (TextBox)GrdViewLeadActivity.Rows[rowIndex].Cells[4].FindControl("txtremarks");


                    drpactivityName.SelectedValue = dt.Rows[i]["Col1"].ToString();
                    txtactivityLoc.Text = dt.Rows[i]["Col2"].ToString();
                    txtactivityDate.Text = dt.Rows[i]["Col3"].ToString();
                    drpnxtActivity.SelectedValue = dt.Rows[i]["Col4"].ToString();
                    txtnxtActDate.Text = dt.Rows[i]["Col5"].ToString();
                    drpemployee.SelectedValue = dt.Rows[i]["Col6"].ToString();
                    txtmodeofCnt.Text = dt.Rows[i]["Col7"].ToString();
                    drpinfo2.SelectedValue = dt.Rows[i]["Col8"].ToString();
                    drpinfo5.SelectedValue = dt.Rows[i]["Col9"].ToString();
                    txtremarks.Text = dt.Rows[i]["Col10"].ToString();

                    rowIndex++;
                }
            }
        }
    }

    private void SetRowDataProduct()
    {
        int rowIndex = 0;

        if (ViewState["CurrentTable1"] != null)
        {
            DataTable dtCurrentTable1 = (DataTable)ViewState["CurrentTable1"];
            DataRow drCurrentRow = null;
            if (dtCurrentTable1.Rows.Count > 0)
            {
                for (int i = 1; i <= dtCurrentTable1.Rows.Count; i++)
                {
                    DropDownList DrpProduct =
                    (DropDownList)GrdViewLeadproduct.Rows[rowIndex].Cells[1].FindControl("drpproduct");
                    TextBox txtprdID =
                     (TextBox)GrdViewLeadproduct.Rows[rowIndex].Cells[2].FindControl("txtPrdId");


                    drCurrentRow = dtCurrentTable1.NewRow();
                    drCurrentRow["RowNumber"] = i + 1;

                    dtCurrentTable1.Rows[i - 1]["Col1"] = DrpProduct.SelectedValue;
                    dtCurrentTable1.Rows[i - 1]["Col2"] = txtprdID.Text;
                    rowIndex++;

                }

                ViewState["CurrentTable1"] = dtCurrentTable1;
                GrdViewLeadproduct.DataSource = dtCurrentTable1;
                GrdViewLeadproduct.DataBind();
            }
        }
        else
        {
            Response.Write("ViewState is null");
        }
        SetPreviousDataProduct();
    }

    private void SetRowDataCompetitors()
    {
        int rowIndex = 0;

        if (ViewState["CurrentTable2"] != null)
        {
            DataTable dtCurrentTable2 = (DataTable)ViewState["CurrentTable2"];
            DataRow drCurrentRow = null;
            if (dtCurrentTable2.Rows.Count > 0)
            {
                for (int i = 1; i <= dtCurrentTable2.Rows.Count; i++)
                {
                    TextBox txtComName =
                    (TextBox)GrdViewLeadCompetitor.Rows[rowIndex].Cells[2].FindControl("txtComeName");
                    TextBox txtThrlvl =
                      (TextBox)GrdViewLeadCompetitor.Rows[rowIndex].Cells[2].FindControl("txtThrLvl");
                    TextBox txtOuestrweak =
                      (TextBox)GrdViewLeadCompetitor.Rows[rowIndex].Cells[2].FindControl("txtOurStrWeakness");
                    TextBox txtComstrweak =
                      (TextBox)GrdViewLeadCompetitor.Rows[rowIndex].Cells[2].FindControl("txtCompStrWeakness");
                    TextBox txtremarks =
                      (TextBox)GrdViewLeadCompetitor.Rows[rowIndex].Cells[2].FindControl("txtRemarks");


                    drCurrentRow = dtCurrentTable2.NewRow();
                    drCurrentRow["RowNumber"] = i + 1;

                    dtCurrentTable2.Rows[i - 1]["Col1"] = txtComName.Text;
                    dtCurrentTable2.Rows[i - 1]["Col2"] = txtThrlvl.Text;
                    dtCurrentTable2.Rows[i - 1]["Col3"] = txtOuestrweak.Text;
                    dtCurrentTable2.Rows[i - 1]["Col4"] = txtComstrweak.Text;
                    dtCurrentTable2.Rows[i - 1]["Col5"] = txtremarks.Text;
                    rowIndex++;

                }

                ViewState["CurrentTable2"] = dtCurrentTable2;
                GrdViewLeadCompetitor.DataSource = dtCurrentTable2;
                GrdViewLeadCompetitor.DataBind();
            }
        }
        else
        {
            Response.Write("ViewState is null");
        }
        SetPreviousDataCompetitors();
    }

    private void SetRowDataActivity()
    {
        int rowIndex = 0;

        if (ViewState["CurrentTable3"] != null)
        {
            DataTable dtCurrentTable3 = (DataTable)ViewState["CurrentTable3"];
            DataRow drCurrentRow = null;
            if (dtCurrentTable3.Rows.Count > 0)
            {
                for (int i = 1; i <= dtCurrentTable3.Rows.Count; i++)
                {
                    DropDownList drpactivityName =
                   (DropDownList)GrdViewLeadActivity.Rows[rowIndex].Cells[1].FindControl("drpactivityName");
                    TextBox txtactivityLoc =
                      (TextBox)GrdViewLeadActivity.Rows[rowIndex].Cells[2].FindControl("txtActiLoc");
                    TextBox txtactivityDate =
                      (TextBox)GrdViewLeadActivity.Rows[rowIndex].Cells[2].FindControl("txtActiDate");
                    DropDownList drpnxtActivity =
                     (DropDownList)GrdViewLeadActivity.Rows[rowIndex].Cells[1].FindControl("drpnxtActivity");
                    TextBox txtnxtActDate =
                     (TextBox)GrdViewLeadActivity.Rows[rowIndex].Cells[3].FindControl("txtNxtActyDate");
                    DropDownList drpemployee =
                     (DropDownList)GrdViewLeadActivity.Rows[rowIndex].Cells[1].FindControl("drpemployee");
                    TextBox txtmodeofCnt =
                      (TextBox)GrdViewLeadActivity.Rows[rowIndex].Cells[4].FindControl("txtModrofcnt");
                    DropDownList drpinfo2 =
                       (DropDownList)GrdViewLeadActivity.Rows[rowIndex].Cells[1].FindControl("drpinfo2");
                    DropDownList drpinfo5 =
                       (DropDownList)GrdViewLeadActivity.Rows[rowIndex].Cells[1].FindControl("drpinfo5");
                    TextBox txtremarks =
                      (TextBox)GrdViewLeadActivity.Rows[rowIndex].Cells[4].FindControl("txtremarks");


                    drCurrentRow = dtCurrentTable3.NewRow();
                    drCurrentRow["RowNumber"] = i + 1;

                    dtCurrentTable3.Rows[i - 1]["Col1"] = drpactivityName.SelectedValue;
                    dtCurrentTable3.Rows[i - 1]["Col2"] = txtactivityLoc.Text;
                    dtCurrentTable3.Rows[i - 1]["Col3"] = txtactivityDate.Text;
                    dtCurrentTable3.Rows[i - 1]["Col4"] = drpnxtActivity.SelectedValue;
                    dtCurrentTable3.Rows[i - 1]["Col5"] = txtnxtActDate.Text;
                    dtCurrentTable3.Rows[i - 1]["Col6"] = drpemployee.SelectedValue;
                    dtCurrentTable3.Rows[i - 1]["Col7"] = txtmodeofCnt.Text;
                    dtCurrentTable3.Rows[i - 1]["Col8"] = drpinfo2.SelectedValue;
                    dtCurrentTable3.Rows[i - 1]["Col9"] = drpinfo5.SelectedValue;
                    dtCurrentTable3.Rows[i - 1]["Col10"] = txtremarks.Text;

                    rowIndex++;

                }

                ViewState["CurrentTable3"] = dtCurrentTable3;
                GrdViewLeadActivity.DataSource = dtCurrentTable3;
                GrdViewLeadActivity.DataBind();
            }
        }
        else
        {
            Response.Write("ViewState is null");
        }
        SetPreviousDataActivity();
    }


    protected void GrdViewLeadproduct_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        try
        {
            BusinessLogic bl = new BusinessLogic(sDataSource);
            DataSet ds = new DataSet();

            ds = bl.ListProducts(sDataSource, "", "");

            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                var ddl = (DropDownList)e.Row.FindControl("drpproduct");
                ddl.Items.Clear();
                ListItem lifzzh = new ListItem("Select Product", "0");
                lifzzh.Attributes.Add("style", "color:Black");
                ddl.Items.Add(lifzzh);
                ddl.DataSource = ds;
                ddl.Items[0].Attributes.Add("background-color", "color:#bce1fe");
                ddl.DataBind();
                ddl.DataTextField = "ProductName";
                ddl.DataValueField = "ItemCode";
            }
        }
        catch (Exception ex)
        {
            TroyLiteExceptionManager.HandleException(ex);
        }
    }

    private void FirstGridViewRow_ProductTab()
    {
        DataTable dt = new DataTable();
        DataRow dr = null;
        dt.Columns.Add(new DataColumn("RowNumber", typeof(string)));
        dt.Columns.Add(new DataColumn("Col1", typeof(string)));
        dt.Columns.Add(new DataColumn("Col2", typeof(string)));
        dr = dt.NewRow();
        dr["RowNumber"] = 1;
        dr["Col1"] = string.Empty;
        dr["Col2"] = string.Empty;

        dt.Rows.Add(dr);

        ViewState["CurrentTable1"] = dt;

        GrdViewLeadproduct.DataSource = dt;
        GrdViewLeadproduct.DataBind();
        GrdViewLeadproduct.Visible = true;
    }

    private void FirstGridViewRow_CompetitorsTab()
    {
        DataTable dt = new DataTable();
        DataRow dr = null;
        dt.Columns.Add(new DataColumn("RowNumber", typeof(string)));
        dt.Columns.Add(new DataColumn("Col1", typeof(string)));
        dt.Columns.Add(new DataColumn("Col2", typeof(string)));
        dt.Columns.Add(new DataColumn("Col3", typeof(string)));
        dt.Columns.Add(new DataColumn("Col4", typeof(string)));
        dt.Columns.Add(new DataColumn("Col5", typeof(string)));

        dr = dt.NewRow();
        dr["RowNumber"] = 1;
        dr["Col1"] = string.Empty;
        dr["Col2"] = string.Empty;
        dr["Col3"] = string.Empty;
        dr["Col4"] = string.Empty;
        dr["Col5"] = string.Empty;

        dt.Rows.Add(dr);

        ViewState["CurrentTable2"] = dt;

        GrdViewLeadCompetitor.DataSource = dt;
        GrdViewLeadCompetitor.DataBind();
        GrdViewLeadCompetitor.Visible = true;
    }

    private void FirstGridViewRow_ActivityTab()
    {
        DataTable dt = new DataTable();
        DataRow dr = null;
        dt.Columns.Add(new DataColumn("RowNumber", typeof(string)));
        dt.Columns.Add(new DataColumn("Col1", typeof(string)));
        dt.Columns.Add(new DataColumn("Col2", typeof(string)));
        dt.Columns.Add(new DataColumn("Col3", typeof(string)));
        dt.Columns.Add(new DataColumn("Col4", typeof(string)));
        dt.Columns.Add(new DataColumn("Col5", typeof(string)));
        dt.Columns.Add(new DataColumn("Col6", typeof(string)));
        dt.Columns.Add(new DataColumn("Col7", typeof(string)));
        dt.Columns.Add(new DataColumn("Col8", typeof(string)));
        dt.Columns.Add(new DataColumn("Col9", typeof(string)));
        dt.Columns.Add(new DataColumn("Col10", typeof(string)));

        dr = dt.NewRow();
        dr["RowNumber"] = 1;
        dr["Col1"] = string.Empty;
        dr["Col2"] = string.Empty;
        dr["Col3"] = string.Empty;
        dr["Col4"] = string.Empty;
        dr["Col5"] = string.Empty;
        dr["Col6"] = string.Empty;
        dr["Col7"] = string.Empty;
        dr["Col8"] = string.Empty;
        dr["Col9"] = string.Empty;
        dr["Col10"] = string.Empty;

        dt.Rows.Add(dr);

        ViewState["CurrentTable3"] = dt;

        GrdViewLeadActivity.DataSource = dt;
        GrdViewLeadActivity.DataBind();
        GrdViewLeadActivity.Visible = true;
    }
    protected void GrdViewLeadCompetitor_RowDataBound(object sender, GridViewRowEventArgs e)
    {

    }
    protected void GrdViewLeadActivity_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        try
        {
            BusinessLogic bl = new BusinessLogic(sDataSource);
            DataSet dsactname = new DataSet();
            DataSet dsnextacty = new DataSet();
            DataSet dsEmp = new DataSet();
            DataSet dsinfo2 = new DataSet();
            DataSet dsinfo5 = new DataSet();
            string connection = ConfigurationManager.ConnectionStrings[Request.Cookies["Company"].Value].ToString();


            dsactname = bl.ListActivityName();
            //dsnextacty = bl.ListNextActivity();
            dsEmp = bl.ListExecutive();
            dsinfo2 = bl.ListInformation2();
            dsinfo5 = bl.ListInformation5();

            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                var ddl3 = (DropDownList)e.Row.FindControl("drpactivityName");
                ddl3.Items.Clear();
                ListItem lifzzh = new ListItem("Select Activity Name", "0");
                lifzzh.Attributes.Add("style", "color:Black");
                ddl3.Items.Add(lifzzh);
                ddl3.DataSource = dsactname;
                ddl3.Items[0].Attributes.Add("background-color", "color:#bce1fe");
                ddl3.DataBind();
                ddl3.DataTextField = "TextValue";
                ddl3.DataValueField = "ID";
            }

            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                var ddl1 = (DropDownList)e.Row.FindControl("drpnxtActivity");
                ddl1.Items.Clear();
                ListItem lifzzh1 = new ListItem("Select Next Activity", "0");
                lifzzh1.Attributes.Add("style", "color:Black");
                ddl1.Items.Add(lifzzh1);
                ddl1.DataSource = dsactname;
                ddl1.Items[0].Attributes.Add("background-color", "color:#bce1fe");
                ddl1.DataBind();
                ddl1.DataTextField = "TextValue";
                ddl1.DataValueField = "ID";
            }

            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                var ddl2 = (DropDownList)e.Row.FindControl("drpemployee");
                ddl2.Items.Clear();
                ListItem lifzzh2 = new ListItem("Select Employee", "0");
                lifzzh2.Attributes.Add("style", "color:Black");
                ddl2.Items.Add(lifzzh2);
                ddl2.DataSource = dsEmp;
                ddl2.Items[0].Attributes.Add("background-color", "color:#bce1fe");
                ddl2.DataBind();
                ddl2.DataTextField = "empFirstName";
                ddl2.DataValueField = "empno";
            }

            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                var ddl3 = (DropDownList)e.Row.FindControl("drpinfo2");
                ddl3.Items.Clear();
                ListItem lifzzh3 = new ListItem("Select Information1", "0");
                lifzzh3.Attributes.Add("style", "color:Black");
                ddl3.Items.Add(lifzzh3);
                ddl3.DataSource = dsinfo2;
                ddl3.Items[0].Attributes.Add("background-color", "color:#bce1fe");
                ddl3.DataBind();
                ddl3.DataTextField = "TextValue";
                ddl3.DataValueField = "ID";
            }

            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                var ddl5 = (DropDownList)e.Row.FindControl("drpinfo5");
                ddl5.Items.Clear();
                ListItem lifzzh5 = new ListItem("Select Information5", "0");
                lifzzh5.Attributes.Add("style", "color:Black");
                ddl5.Items.Add(lifzzh5);
                ddl5.DataSource = dsinfo5;
                ddl5.Items[0].Attributes.Add("background-color", "color:#bce1fe");
                ddl5.DataBind();
                ddl5.DataTextField = "TextValue";
                ddl5.DataValueField = "ID";
            }
        }
        catch (Exception ex)
        {
            TroyLiteExceptionManager.HandleException(ex);
        }
    }
    protected void ButtonAdd_Click(object sender, EventArgs e)
    {
        AddNewRowProduct();
    }
    protected void drpproduct_SelectedIndexChanged(object sender, EventArgs e)
    {
        for (int i = GrdViewLeadproduct.Rows.Count; i == GrdViewLeadproduct.Rows.Count; i++)
        {
            DropDownList DrpProduct =
              (DropDownList)GrdViewLeadproduct.Rows[i - 1].Cells[1].FindControl("drpproduct");
            TextBox txtPrdID =
              (TextBox)GrdViewLeadproduct.Rows[i - 1].Cells[2].FindControl("txtPrdId");

            txtPrdID.Text = DrpProduct.SelectedValue;
        }
    }
    protected void ButtonAddCom_Click(object sender, EventArgs e)
    {
        AddNewRowCompetitors();
    }
    protected void ButtonAddActivity_Click(object sender, EventArgs e)
    {
        AddNewRowActivity();
    }
}