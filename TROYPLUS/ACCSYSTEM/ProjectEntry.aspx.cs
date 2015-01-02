using System;
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
using System.Xml.Linq;

public partial class ProjectEntry : System.Web.UI.Page
{
    private string sDataSource = string.Empty;
    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            if (!IsPostBack)
            {
                if (Request.Cookies["Company"] != null)
                    sDataSource = System.Configuration.ConfigurationManager.ConnectionStrings[Request.Cookies["Company"].Value].ToString();
                else
                    Response.Redirect("~/Login.aspx");

                BindWME("", "");
                loadEmp();
                DisableForOffline();
                //rvASDate.MinimumValue = System.DateTime.Now.AddYears(-100).ToShortDateString();
                //rvASDate.MaximumValue = System.DateTime.Now.ToShortDateString();
                //rvAEDate.MinimumValue = System.DateTime.Now.AddYears(-100).ToShortDateString();
                //rvAEDate.MaximumValue = System.DateTime.Now.ToShortDateString();


                GrdWME.PageSize = 8;

                BusinessLogic bl = new BusinessLogic(sDataSource);
                string connection = Request.Cookies["Company"].Value;
                string usernam = Request.Cookies["LoggedUserName"].Value;

                //if (bl.CheckUserHaveAdd(usernam, "WMENTRY"))
                //{
                //    lnkBtnAdd.Enabled = false;
                //    lnkBtnAdd.ToolTip = "You are not allowed to make Add New ";
                //}
                //else
                //{
                //    lnkBtnAdd.Enabled = true;
                //    lnkBtnAdd.ToolTip = "Click to Add New item ";
                //}


            }
        }
        catch (Exception ex)
        {
            TroyLiteExceptionManager.HandleException(ex);
        }
    }

    private void DisableForOffline()
    {
        string dbfileName = sDataSource.Remove(0, sDataSource.LastIndexOf(@"App_Data\") + 9);
        dbfileName = dbfileName.Remove(dbfileName.LastIndexOf(";Persist Security Info"));

        BusinessLogic objChk = new BusinessLogic(sDataSource);

        if (objChk.CheckForOffline(Server.MapPath("Offline\\" + dbfileName + ".offline")))
        {
            btnSave.Enabled = false;
            btnUpdate.Enabled = false;
            lnkBtnAdd.Visible = false;
            GrdWME.Columns[10].Visible = false;
            GrdWME.Columns[11].Visible = false;
        }
    }

    private void loadEmp()
    {
        BusinessLogic bl = new BusinessLogic(sDataSource);
        DataSet ds = new DataSet();

        ds = bl.ListExecutive();
        drpIncharge.DataSource = ds;
        drpIncharge.DataBind();
        drpIncharge.DataTextField = "empFirstName";
        drpIncharge.DataValueField = "empno";

        //drpsIncharge.DataSource = ds;
        //drpsIncharge.DataBind();
        //drpsIncharge.DataTextField = "empFirstName";
        //drpsIncharge.DataValueField = "empno";
    }

    private void BindWME(string textSearch, string dropDown)
    {
        sDataSource = ConfigurationManager.ConnectionStrings[Request.Cookies["Company"].Value].ToString();
        BusinessLogic bl = new BusinessLogic(sDataSource);

        string connection = Request.Cookies["Company"].Value;

        DataSet ds = bl.GetProjectList(connection, textSearch, dropDown);

        if (ds != null)
        {
            if (ds.Tables[0].Rows.Count > 0)
            {
                GrdWME.DataSource = ds;
                GrdWME.DataBind();
            }
        }
        else
        {
            GrdWME.DataSource = null;
            GrdWME.DataBind();
        }
    }
    protected void btnSearch_Click(object sender, EventArgs e)
    {
        try
        {
            string textSearch = string.Empty;
            string dropDown = string.Empty;

            textSearch = txtSearch.Text;
            dropDown = ddCriteria.SelectedValue;

            BindWME(textSearch, dropDown);
        }
        catch (Exception ex)
        {
            TroyLiteExceptionManager.HandleException(ex);
        }
    }
    protected void btnSave_Click(object sender, EventArgs e)
    {
        sDataSource = ConfigurationManager.ConnectionStrings[Request.Cookies["Company"].Value].ToString();
        BusinessLogic bl = new BusinessLogic(sDataSource);

        string ProjectDate = string.Empty;
        string EWStartDate = string.Empty;
        string EWEndDate = string.Empty;
        int empNO = 0;
        string ProjectName = string.Empty;
        string ActStartDate = string.Empty;
        string ActEndDate = string.Empty;
        string ProjectDesc = string.Empty;
        string Projectstatus = string.Empty;
        string ProjectCode = string.Empty;
        int EffortDays = 0;

        try
        {
            if (Page.IsValid)
            {
                if (txtProjectCode.Text.Trim() != string.Empty)
                    ProjectCode = txtProjectCode.Text.Trim();

                string connection = ConfigurationManager.ConnectionStrings[Request.Cookies["Company"].Value].ToString();

                if (bl.IsProjectCodeAlreadyFound(connection, ProjectCode))
                {
                    ScriptManager.RegisterStartupScript(Page, Page.GetType(), Guid.NewGuid().ToString(), "alert('Project Code already found');", true);

                    ModalPopupExtender1.Show();
                    tbMain.Visible = true;
                    return;

                    
                }

                
                if (txtCDate.Text.Trim() != string.Empty)
                    ProjectDate = txtCDate.Text.Trim();
                if (txtEWstartDate.Text.Trim() != string.Empty)
                    EWStartDate = txtEWstartDate.Text.Trim();
                if (txtEWEndDate.Text.Trim() != string.Empty)
                    EWEndDate = txtEWEndDate.Text.Trim();
                if (drpIncharge.Text.Trim() != string.Empty)
                    empNO = Convert.ToInt32(drpIncharge.Text.Trim());
                if (txtProjectName.Text.Trim() != string.Empty)
                    ProjectName = txtProjectName.Text.Trim();
                if (txtEffortDays.Text.Trim() != string.Empty)
                    EffortDays = Convert.ToInt32(txtEffortDays.Text.Trim());
                if (drpProjectstatus.Text.Trim() != string.Empty)
                    Projectstatus = drpProjectstatus.Text.Trim();
                if (txtProjectDesc.Text.Trim() != string.Empty)
                    ProjectDesc = txtProjectDesc.Text.Trim();

                string Username = Request.Cookies["LoggedUserName"].Value;

                //DataSet checkemp = bl.SearchWME(WorkId, empNO, EWStartDate, EWEndDate, CreationDate, status);

                //if (checkemp == null || checkemp.Tables[0].Rows.Count == 0)
                //{
                bl.InsertProjectEntry(ProjectCode, ProjectDate, EWStartDate, EWEndDate, empNO, ProjectName, EffortDays, Projectstatus, ProjectDesc, Username);

                    ScriptManager.RegisterStartupScript(Page, Page.GetType(), Guid.NewGuid().ToString(), "alert('Project Entry Details Saved Successfully');", true);
                    Reset();
                    ResetSearch();
                    BindWME("", "");
                    //MyAccordion.Visible = true;
                    tbMain.Visible = false;
                    GrdWME.Visible = true;
                //}
                //else
                //{
                //    ScriptManager.RegisterStartupScript(Page, Page.GetType(), Guid.NewGuid().ToString(), "alert('Duplicate work Management Entry EmpNo " + empNO + " ');", true);
                //}
            }
        }
        catch (Exception ex)
        {
            TroyLiteExceptionManager.HandleException(ex);
        }

    }
    protected void btnUpdate_Click(object sender, EventArgs e)
    {
        try
        {
            sDataSource = ConfigurationManager.ConnectionStrings[Request.Cookies["Company"].Value].ToString();
            BusinessLogic bl = new BusinessLogic(sDataSource);
            int WorkId = 0;
            string ProjectDate = string.Empty;
            string EWStartDate = string.Empty;
            string EWEndDate = string.Empty;
            int empNO = 0;
            string Workdet = string.Empty;
            string ActStartDate = string.Empty;
            string ActEndDate = string.Empty;
            string ProjectName = string.Empty;
            string status = string.Empty;
            string ProjectDesc = string.Empty;
            string Projectstatus = string.Empty;
            string ProjectCode = string.Empty;
            int EffortDays = 0;
            int Project_Id = 0;

            if (Page.IsValid)
            {              
                if (txtProjectCode.Text.Trim() != string.Empty)
                    ProjectCode = txtProjectCode.Text.Trim();
                if (txtCDate.Text.Trim() != string.Empty)
                    ProjectDate = txtCDate.Text.Trim();
                if (txtEWstartDate.Text.Trim() != string.Empty)
                    EWStartDate = txtEWstartDate.Text.Trim();
                if (txtEWEndDate.Text.Trim() != string.Empty)
                    EWEndDate = txtEWEndDate.Text.Trim();
                if (drpIncharge.Text.Trim() != string.Empty)
                    empNO = Convert.ToInt32(drpIncharge.Text.Trim());
                if (txtProjectName.Text.Trim() != string.Empty)
                    ProjectName = txtProjectName.Text.Trim();
                if (txtEffortDays.Text.Trim() != string.Empty)
                    EffortDays = Convert.ToInt32(txtEffortDays.Text.Trim());
                if (drpProjectstatus.Text.Trim() != string.Empty)
                    Projectstatus = drpProjectstatus.Text.Trim();
                if (txtProjectDesc.Text.Trim() != string.Empty)
                    ProjectDesc = txtProjectDesc.Text.Trim();

                string Username = Request.Cookies["LoggedUserName"].Value;

                Project_Id = int.Parse(GrdWME.SelectedDataKey.Value.ToString());

                bl.UpdateProjectEntry(ProjectCode, ProjectDate, EWStartDate, EWEndDate, empNO, ProjectName, EffortDays, Projectstatus, ProjectDesc, Username, Project_Id);


                ScriptManager.RegisterStartupScript(Page, Page.GetType(), Guid.NewGuid().ToString(), "alert('Project Entry Details Updated Successfully');", true);
                Reset();
                ResetSearch();
                BindWME("", "");
                //MyAccordion.Visible = true;
                tbMain.Visible = false;
                GrdWME.Visible = true;

            }
        }
        catch (Exception ex)
        {
            TroyLiteExceptionManager.HandleException(ex);
        }
    }
    protected void btnCancel_Click(object sender, EventArgs e)
    {
        try
        {
            //MyAccordion.Visible = true;
            tbMain.Visible = false;
            GrdWME.Visible = true;
            Reset();
        }
        catch (Exception ex)
        {
            TroyLiteExceptionManager.HandleException(ex);
        }
    }

    private void ResetSearch()
    {
        
        txtStartDate.Text = "";
        txtEndDate.Text = "";
        txtsCreationDate.Text = "";
        drpsStatus.SelectedIndex = 0;
    }
    public void Reset()
    {

        drpIncharge.SelectedIndex = 0;

        pnsSave.Visible = false;
        lnkBtnAdd.Visible = true;
        btnSave.Enabled = true;
        btnCancel.Enabled = true;
        btnUpdate.Enabled = false;
        txtProjectCode.Text = "";
        txtProjectDesc.Text = "";
        txtCDate.Text = "";
        txtEWstartDate.Text = "";
        txtEWEndDate.Text = "";
        txtProjectName.Text = "";
        txtEffortDays.Text = "0";
        drpProjectstatus.SelectedIndex = 0;
        txtProjectCode.Enabled = true;

    }
    protected void ddlPageSelector_SelectedIndexChanged(object sender, EventArgs e)
    {
        try
        {
            GrdWME.PageIndex = ((DropDownList)sender).SelectedIndex;
            BindWME("", "");

        }
        catch (Exception ex)
        {
            TroyLiteExceptionManager.HandleException(ex);
        }
    }
    protected void GrdWME_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        try
        { 
            GrdWME.PageIndex = e.NewPageIndex;
            BindWME("","");
        }
        catch (Exception ex)
        {
            TroyLiteExceptionManager.HandleException(ex);
        }
    }
    protected void GrdWME_RowCreated(object sender, GridViewRowEventArgs e)
    {
        try
        {
            if (e.Row.RowType == DataControlRowType.Pager)
            {
                PresentationUtils.SetPagerButtonStates(GrdWME, e.Row, this);
            }
        }
        catch (Exception ex)
        {
            TroyLiteExceptionManager.HandleException(ex);
        }
    }

    protected void GrdWME_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        try
        {
            GridView gridView = (GridView)sender;

            //if (e.Row.RowType == DataControlRowType.DataRow)
            //{
            //    BusinessLogic bl = new BusinessLogic(sDataSource);
            //    string connection = Request.Cookies["Company"].Value;
            //    string usernam = Request.Cookies["LoggedUserName"].Value;

            //    if (bl.CheckUserHaveEdit(usernam, "WMENTRY"))
            //    {
            //        ((ImageButton)e.Row.FindControl("btnEdit")).Visible = false;
            //        ((ImageButton)e.Row.FindControl("btnEditDisabled")).Visible = true;
            //    }

            //    if (bl.CheckUserHaveDelete(usernam, "WMENTRY"))
            //    {
            //        ((ImageButton)e.Row.FindControl("lnkB")).Visible = false;
            //        ((ImageButton)e.Row.FindControl("lnkBDisabled")).Visible = true;
            //    }
            //}
        }
        catch (Exception ex)
        {
            TroyLiteExceptionManager.HandleException(ex);
        }
    }

    protected void GrdWME_RowDeleting(object sender, GridViewDeleteEventArgs e)
    {
        try
        {
            sDataSource = ConfigurationManager.ConnectionStrings[Request.Cookies["Company"].Value].ToString();

            int Project_Id = Convert.ToInt32(GrdWME.Rows[e.RowIndex].Cells[0].Text);

            BusinessLogic bl = new BusinessLogic(sDataSource);
            bl.DeleteProjectDetails(Project_Id);
            BindWME("", "");
            btnUpdate.Enabled = false;
            btnSave.Enabled = true;
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
            Reset();

            txtCDate.Text = DateTime.Now.ToShortDateString();
            btnUpdate.Enabled = false;
            tbMain.Visible = true;
            pnsSave.Visible = true;
            
            btnCancel.Enabled = true;
            btnSave.Visible = true;
            btnUpdate.Visible = false;
            btnSave.Enabled = true;
            
            sDataSource = ConfigurationManager.ConnectionStrings[Request.Cookies["Company"].Value].ToString();
            

            ModalPopupExtender1.Show();
        }
        catch (Exception ex)
        {
            TroyLiteExceptionManager.HandleException(ex);
        }
    }

    protected void BtnClearFilter_Click(object sender, EventArgs e)
    {
        txtSearch.Text = "";
        BindWME("", "");
        ddCriteria.SelectedIndex = 0;
    }

    protected void txtEWstartDate_TextChanged(object sender, EventArgs e)
    {
        try
        {
            string startdate = Convert.ToDateTime(txtEWstartDate.Text).ToString("dd/MM/yyyy");

            string enddate = Convert.ToDateTime(txtEWEndDate.Text).ToString("dd/MM/yyyy");

            TimeSpan ts = Convert.ToDateTime(enddate) - Convert.ToDateTime(startdate);
            int days = Convert.ToInt32(ts.TotalDays);

            txtEffortDays.Text = Convert.ToString(days);
            UpdatePanel2.Update();
        }
        catch (Exception ex)
        {
            TroyLiteExceptionManager.HandleException(ex);
        }
    }

    protected void txtEWEndDate_TextChanged(object sender, EventArgs e)
    {
        try
        {
            string startdate = Convert.ToDateTime(txtEWstartDate.Text).ToString("dd/MM/yyyy");

            string enddate = Convert.ToDateTime(txtEWEndDate.Text).ToString("dd/MM/yyyy");

            TimeSpan ts = Convert.ToDateTime(enddate) - Convert.ToDateTime(startdate);
            int days = Convert.ToInt32(ts.TotalDays);

            txtEffortDays.Text = Convert.ToString(days);
            UpdatePanel2.Update();
        }
        catch (Exception ex)
        {
            TroyLiteExceptionManager.HandleException(ex);
        }
    }

    protected void GrdWME_SelectedIndexChanged(object sender, EventArgs e)
    {
        try
        {
            BusinessLogic bl = new BusinessLogic(sDataSource);
            int Project_ID = 0;

            string connection = Request.Cookies["Company"].Value;

            if (GrdWME.SelectedDataKey.Value != null && GrdWME.SelectedDataKey.Value.ToString() != "")
                Project_ID = Convert.ToInt32(GrdWME.SelectedDataKey.Value.ToString());

            DataSet ds = bl.GetProjectForId(connection,Project_ID);

            if (ds != null)
            {
                if (ds.Tables[0].Rows.Count > 0)
                {
                    if (ds.Tables[0].Rows[0]["Expected_Start_Date"] != null)
                        txtEWstartDate.Text = Convert.ToDateTime(ds.Tables[0].Rows[0]["Expected_Start_Date"]).ToString("dd/MM/yyyy");

                    if (ds.Tables[0].Rows[0]["Expected_End_Date"] != null)
                        txtEWEndDate.Text = Convert.ToDateTime(ds.Tables[0].Rows[0]["Expected_End_Date"]).ToString("dd/MM/yyyy");

                    if (ds.Tables[0].Rows[0]["Project_Date"] != null)
                        txtCDate.Text = Convert.ToDateTime(ds.Tables[0].Rows[0]["Project_Date"]).ToString("dd/MM/yyyy");

                    if (ds.Tables[0].Rows[0]["Project_Name"] != null)
                        txtProjectName.Text = ds.Tables[0].Rows[0]["Project_Name"].ToString();

                    if (ds.Tables[0].Rows[0]["Project_Code"] != null)
                        txtProjectCode.Text = ds.Tables[0].Rows[0]["Project_Code"].ToString();

                    txtProjectCode.Enabled = false;

                    if (ds.Tables[0].Rows[0]["Project_Description"] != null)
                        txtProjectDesc.Text = ds.Tables[0].Rows[0]["Project_Description"].ToString();

                    if (ds.Tables[0].Rows[0]["Project_Description"] != null)
                        txtProjectDesc.Text = ds.Tables[0].Rows[0]["Project_Description"].ToString();

                    if (ds.Tables[0].Rows[0]["Project_Status"] != null)
                        drpProjectstatus.SelectedValue = ds.Tables[0].Rows[0]["Project_Status"].ToString();

                    if (ds.Tables[0].Rows[0]["Expected_Effort_Days"] != null)
                        txtEffortDays.Text = Convert.ToInt32(ds.Tables[0].Rows[0]["Expected_Effort_Days"]).ToString();

                    if (ds.Tables[0].Rows[0]["Project_Manager_Id"] != null)
                    {
                        string sCustomer = Convert.ToString(ds.Tables[0].Rows[0]["Project_Manager_Id"]);
                        drpIncharge.ClearSelection();
                        ListItem li = drpIncharge.Items.FindByValue(System.Web.HttpUtility.HtmlDecode(sCustomer));
                        if (li != null) li.Selected = true;
                    }
                }
            }

            
            btnUpdate.Enabled = true;
            pnsSave.Visible = true;
            btnCancel.Enabled = true;
            btnSave.Enabled = false;
            tbMain.Visible = true;
            btnSave.Visible = false;
            btnUpdate.Visible = true;
            ModalPopupExtender1.Show();
        }
        catch (Exception ex)
        {
            TroyLiteExceptionManager.HandleException(ex);
        }
    }

    protected void btnExportToExcel_Click(object sender, EventArgs e)
    {

        //try
        //{
            HtmlForm form = new HtmlForm();
            Response.Clear();
            Response.Buffer = true;
            string filename = "Projects_" + DateTime.Now.ToString() + ".xls";

            BusinessLogic objBL;
            objBL = new BusinessLogic(ConfigurationManager.ConnectionStrings[Request.Cookies["Company"].Value].ToString());

            DataSet ds = new DataSet();

            string connection = string.Empty;

            if (Request.Cookies["Company"] != null)
                connection = Request.Cookies["Company"].Value;
            else
                Response.Redirect("Login.aspx");

            ds = objBL.GetProjectList(connection, "", "");           
           
            if (ds != null)
            {
                if (ds.Tables[0].Rows.Count > 0)
                {
                    DataTable dt = new DataTable();
                    dt.Columns.Add(new DataColumn("Project Name"));
                    //dt.Columns.Add(new DataColumn("CreationDate"));
                    //dt.Columns.Add(new DataColumn("ProspectCustName"));
                    //dt.Columns.Add(new DataColumn("Address"));
                    //dt.Columns.Add(new DataColumn("Mobile"));
                    //dt.Columns.Add(new DataColumn("Landline"));
                    //dt.Columns.Add(new DataColumn("Email"));
                    //dt.Columns.Add(new DataColumn("ModeOfContact"));
                    //dt.Columns.Add(new DataColumn("PersonalResponsible"));
                    //dt.Columns.Add(new DataColumn("BusinessType"));
                    //dt.Columns.Add(new DataColumn("Branch"));
                    //dt.Columns.Add(new DataColumn("Status"));
                    //dt.Columns.Add(new DataColumn("LastCompletedAction"));
                    //dt.Columns.Add(new DataColumn("NextAction"));
                    //dt.Columns.Add(new DataColumn("Category"));
                    //dt.Columns.Add(new DataColumn("ContactedDate"));
                    //dt.Columns.Add(new DataColumn("ContactSummary"));

                    DataRow dr_export1 = dt.NewRow();
                    dt.Rows.Add(dr_export1);

                    foreach (DataRow dr in ds.Tables[0].Rows)
                    {
                            DataRow dr_export = dt.NewRow();
                            dr_export["Project Name"] = dr["Project_Name"];
                            //dr_export["CreationDate"] = dr["CreationDate"];
                            //dr_export["ProspectCustName"] = dr["ProspectCustName"];
                            //dr_export["Address"] = dr["Address"];
                            //dr_export["Mobile"] = dr["Mobile"];
                            //dr_export["Landline"] = dr["Landline"];
                            //dr_export["Email"] = dr["Email"];
                            //dr_export["ModeOfContact"] = dr["ModeOfContact"];
                            //dr_export["PersonalResponsible"] = dr["PersonalResponsible"];
                            //dr_export["BusinessType"] = dr["BusinessType"];
                            //dr_export["Branch"] = dr["Branch"];
                            //dr_export["Email"] = dr["Email"];
                            //dr_export["Status"] = dr["Status"];
                            //dr_export["LastCompletedAction"] = dr["LastCompletedAction"];
                            //dr_export["NextAction"] = dr["NextAction"];
                            //dr_export["Category"] = dr["Category"];
                            //dr_export["ContactedDate"] = dr["ContactedDate"];
                            //dr_export["ContactSummary"] = dr["ContactSummary"];
                            dt.Rows.Add(dr_export);
                    }

                    ExportToExcel(filename, dt);
                }
            }
            else
            {
                ScriptManager.RegisterStartupScript(Page, typeof(Button), "MyScript", "alert('No Data Found');", true);
            }
        //}
        //catch (Exception ex)
        //{
        //    TroyLiteExceptionManager.HandleException(ex);
        //}
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

}
