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

public partial class TaskEntry : System.Web.UI.Page
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
        DataSet dsd = new DataSet();
        DataSet dst = new DataSet();
        DataSet dstd = new DataSet();
        string connection = ConfigurationManager.ConnectionStrings[Request.Cookies["Company"].Value].ToString();

        drpIncharge.Items.Clear();
        drpIncharge.Items.Add(new ListItem("Select Owner", "0"));
        ds = bl.ListExecutive();
        drpIncharge.DataSource = ds;
        drpIncharge.DataBind();
        drpIncharge.DataTextField = "empFirstName";
        drpIncharge.DataValueField = "empno";

        drpTaskType.Items.Clear();
        drpTaskType.Items.Add(new ListItem("Select Task Type", "0"));
        dsd = bl.ListTaskTypesInfo(connection, "", "");
        drpTaskType.DataSource = dsd;
        drpTaskType.DataBind();
        drpTaskType.DataTextField = "Task_Type_Name";
        drpTaskType.DataValueField = "Task_Type_Id";

        drpProjectCode.Items.Clear();
        drpProjectCode.Items.Add(new ListItem("Select Project Name", "0"));
        dst = bl.GetProjectList(connection, "", "");
        drpProjectCode.DataSource = dst;
        drpProjectCode.DataBind();
        drpProjectCode.DataTextField = "Project_Name";
        drpProjectCode.DataValueField = "Project_Id";

        drpDependencyTask.Items.Clear();
        drpDependencyTask.Items.Add(new ListItem("Select Dependency Task", "0"));
        dstd = bl.GetTaskList(connection, "", "");
        drpDependencyTask.DataSource = dstd;
        drpDependencyTask.DataBind();
        drpDependencyTask.DataTextField = "Task_Name";
        drpDependencyTask.DataValueField = "Task_Id";

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

        DataSet ds = bl.GetTaskList(connection, textSearch, dropDown);

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

        string TaskDate = string.Empty;
        string EWStartDate = string.Empty;
        string EWEndDate = string.Empty;
        int Owner = 0;
        string TaskCode = string.Empty;
        string TaskName = string.Empty;
        string ActStartDate = string.Empty;
        string ActEndDate = string.Empty;
        string TaskDesc = string.Empty;
        string IsActive = string.Empty;
        int ProjectCode = 0;
        int TaskType = 0;
        int DependencyTask = 0;

        try
        {
            if (Page.IsValid)
            {

                int PId = 0;
                if (drpProjectCode.Text.Trim() != string.Empty)
                    PId = Convert.ToInt32(drpProjectCode.SelectedValue);

                string connection = ConfigurationManager.ConnectionStrings[Request.Cookies["Company"].Value].ToString();

                DataSet dsttd = bl.GetProjectForId(connection, PId);
                if (dsttd != null)
                {
                    if (dsttd.Tables[0].Rows.Count > 0)
                    {
                        if (Convert.ToDateTime(txtEWstartDate.Text) < Convert.ToDateTime(dsttd.Tables[0].Rows[0]["Expected_Start_Date"]))
                        {
                            ScriptManager.RegisterStartupScript(Page, Page.GetType(), Guid.NewGuid().ToString(), "alert('Task Expected Start Date should be Greater than to the Selected Project Expected Start Date');", true);
                            ModalPopupExtender1.Show();
                            tbMain.Visible = true;
                            return;
                        }
                        if (Convert.ToDateTime(txtEWEndDate.Text) < Convert.ToDateTime(dsttd.Tables[0].Rows[0]["Expected_End_Date"]))
                        {
                            ScriptManager.RegisterStartupScript(Page, Page.GetType(), Guid.NewGuid().ToString(), "alert('Task Expected End Date should be Greater than to the Selected Project Expected End Date');", true);
                            ModalPopupExtender1.Show();
                            tbMain.Visible = true;
                            return;
                        }
                        if (Convert.ToDateTime(txtCDate.Text) < Convert.ToDateTime(dsttd.Tables[0].Rows[0]["Project_Date"]))
                        {
                            ScriptManager.RegisterStartupScript(Page, Page.GetType(), Guid.NewGuid().ToString(), "alert('Task Date should be Greater than to the Selected Project Date');", true);
                            ModalPopupExtender1.Show();
                            tbMain.Visible = true;
                            return;
                        }                        
                    }
                }

                if (drpProjectCode.Text.Trim() != string.Empty)
                    ProjectCode = Convert.ToInt32(drpProjectCode.Text.Trim());
                if (drpDependencyTask.Text.Trim() != string.Empty)
                    DependencyTask = Convert.ToInt32(drpDependencyTask.Text.Trim());
                if (txtCDate.Text.Trim() != string.Empty)
                    TaskDate = txtCDate.Text.Trim();
                if (txtEWstartDate.Text.Trim() != string.Empty)
                    EWStartDate = txtEWstartDate.Text.Trim();
                if (txtEWEndDate.Text.Trim() != string.Empty)
                    EWEndDate = txtEWEndDate.Text.Trim();
                if (drpIncharge.Text.Trim() != string.Empty)
                    Owner = Convert.ToInt32(drpIncharge.Text.Trim());
                if (txtTaskID.Text.Trim() != string.Empty)
                    TaskCode = txtTaskID.Text.Trim();
                if (txtTaskName.Text.Trim() != string.Empty)
                    TaskName = txtTaskName.Text.Trim();
                if (drpTaskType.Text.Trim() != string.Empty)
                    TaskType = Convert.ToInt32(drpTaskType.Text.Trim());
                if (drpIsActive.Text.Trim() != string.Empty)
                    IsActive = drpIsActive.Text.Trim();
                if (txtTaskDesc.Text.Trim() != string.Empty)
                    TaskDesc = txtTaskDesc.Text.Trim();

                string Username = Request.Cookies["LoggedUserName"].Value;

                //DataSet checkemp = bl.SearchWME(WorkId, empNO, EWStartDate, EWEndDate, CreationDate, status);

                //if (checkemp == null || checkemp.Tables[0].Rows.Count == 0)
                //{
                bl.InsertTaskEntry(ProjectCode, TaskDate, EWStartDate, EWEndDate, Owner, TaskCode, TaskType, IsActive, DependencyTask, TaskDesc, Username, TaskName);

                ScriptManager.RegisterStartupScript(Page, Page.GetType(), Guid.NewGuid().ToString(), "alert('Task Entry Details Saved Successfully');", true);
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
            int TaskType = 0;
            string TaskDate = string.Empty;
            string EWStartDate = string.Empty;
            string EWEndDate = string.Empty;
            int Owner = 0;
            string Workdet = string.Empty;
            string ActStartDate = string.Empty;
            string ActEndDate = string.Empty;
            string TaskCode = string.Empty;
            string TaskName = string.Empty;
            string status = string.Empty;
            string TaskDesc = string.Empty;
            string IsActive = string.Empty;
            int ProjectCode = 0;
            int DependencyTask = 0;
            int Task_Id = 0;

            if (Page.IsValid)
            {

                int PId = 0;
                if (drpProjectCode.Text.Trim() != string.Empty)
                    PId = Convert.ToInt32(drpProjectCode.SelectedValue);

                string connection = ConfigurationManager.ConnectionStrings[Request.Cookies["Company"].Value].ToString();

                DataSet dsttd = bl.GetProjectForId(connection, PId);
                if (dsttd != null)
                {
                    if (dsttd.Tables[0].Rows.Count > 0)
                    {
                        if (Convert.ToDateTime(txtEWstartDate.Text) < Convert.ToDateTime(dsttd.Tables[0].Rows[0]["Expected_Start_Date"]))
                        {
                            ScriptManager.RegisterStartupScript(Page, Page.GetType(), Guid.NewGuid().ToString(), "alert('Task Expected Start Date should be Greater than to the Selected Project Expected Start Date');", true);
                            ModalPopupExtender1.Show();
                            tbMain.Visible = true;
                            return;
                        }
                        if (Convert.ToDateTime(txtEWEndDate.Text) < Convert.ToDateTime(dsttd.Tables[0].Rows[0]["Expected_End_Date"]))
                        {
                            ScriptManager.RegisterStartupScript(Page, Page.GetType(), Guid.NewGuid().ToString(), "alert('Task Expected End Date should be Greater than to the Selected Project Expected End Date');", true);
                            ModalPopupExtender1.Show();
                            tbMain.Visible = true;
                            return;
                        }
                        if (Convert.ToDateTime(txtCDate.Text) < Convert.ToDateTime(dsttd.Tables[0].Rows[0]["Project_Date"]))
                        {
                            ScriptManager.RegisterStartupScript(Page, Page.GetType(), Guid.NewGuid().ToString(), "alert('Task Date should be Greater than to the Selected Project Date');", true);
                            ModalPopupExtender1.Show();
                            tbMain.Visible = true;
                            return;
                        }
                    }
                }


                if (drpProjectCode.Text.Trim() != string.Empty)
                    ProjectCode = Convert.ToInt32(drpProjectCode.Text.Trim());
                if (drpDependencyTask.Text.Trim() != string.Empty)
                    DependencyTask = Convert.ToInt32(drpDependencyTask.Text.Trim());
                if (txtCDate.Text.Trim() != string.Empty)
                    TaskDate = txtCDate.Text.Trim();
                if (txtEWstartDate.Text.Trim() != string.Empty)
                    EWStartDate = txtEWstartDate.Text.Trim();
                if (txtEWEndDate.Text.Trim() != string.Empty)
                    EWEndDate = txtEWEndDate.Text.Trim();
                if (drpIncharge.Text.Trim() != string.Empty)
                    Owner = Convert.ToInt32(drpIncharge.Text.Trim());
                if (txtTaskID.Text.Trim() != string.Empty)
                    TaskCode = txtTaskID.Text.Trim();
                if (txtTaskName.Text.Trim() != string.Empty)
                    TaskName = txtTaskName.Text.Trim();
                if (drpTaskType.Text.Trim() != string.Empty)
                    TaskType = Convert.ToInt32(drpTaskType.Text.Trim());
                if (drpIsActive.Text.Trim() != string.Empty)
                    IsActive = drpIsActive.Text.Trim();
                if (txtTaskDesc.Text.Trim() != string.Empty)
                    TaskDesc = txtTaskDesc.Text.Trim();

                string Username = Request.Cookies["LoggedUserName"].Value;

                Task_Id = int.Parse(GrdWME.SelectedDataKey.Value.ToString());

                bl.UpdateTaskEntry(ProjectCode, TaskDate, EWStartDate, EWEndDate, Owner, TaskCode, TaskType, IsActive, DependencyTask, TaskDesc, Username, Task_Id, TaskName);


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
        drpIsActive.SelectedIndex = 0;
        txtTaskID.Text = "";
        txtCDate.Text = "";
        txtEWstartDate.Text = "";
        txtEWEndDate.Text = "";
        drpTaskType.SelectedIndex = 0;
        drpDependencyTask.SelectedIndex = 0;
        drpProjectCode.SelectedIndex = 0;
        txtTaskDesc.Text = "";
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

            int Task_Id = Convert.ToInt32(GrdWME.Rows[e.RowIndex].Cells[0].Text);

            BusinessLogic bl = new BusinessLogic(sDataSource);
            bl.DeleteTaskDetails(Task_Id);
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

            loadEmp();
            ModalPopupExtender1.Show();
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
            int Task_Id = 0;

            string connection = Request.Cookies["Company"].Value;

            if (GrdWME.SelectedDataKey.Value != null && GrdWME.SelectedDataKey.Value.ToString() != "")
                Task_Id = Convert.ToInt32(GrdWME.SelectedDataKey.Value.ToString());

            DataSet ds = bl.GetTaskForId(connection, Task_Id);

            if (ds != null)
            {
                if (ds.Tables[0].Rows.Count > 0)
                {
                    if (ds.Tables[0].Rows[0]["Expected_Start_Date"] != null)
                        txtEWstartDate.Text = Convert.ToDateTime(ds.Tables[0].Rows[0]["Expected_Start_Date"]).ToString("dd/MM/yyyy");

                    if (ds.Tables[0].Rows[0]["Expected_End_Date"] != null)
                        txtEWEndDate.Text = Convert.ToDateTime(ds.Tables[0].Rows[0]["Expected_End_Date"]).ToString("dd/MM/yyyy");

                    if (ds.Tables[0].Rows[0]["Task_Date"] != null)
                        txtCDate.Text = Convert.ToDateTime(ds.Tables[0].Rows[0]["Task_Date"]).ToString("dd/MM/yyyy");

                    if (ds.Tables[0].Rows[0]["Task_Code"] != null)
                        txtTaskID.Text = ds.Tables[0].Rows[0]["Task_Code"].ToString();

                    if (ds.Tables[0].Rows[0]["Task_Name"] != null)
                        txtTaskName.Text = ds.Tables[0].Rows[0]["Task_Name"].ToString();

                    if (ds.Tables[0].Rows[0]["Project_Code"] != null)
                    {
                        string sCs = Convert.ToString(ds.Tables[0].Rows[0]["Project_Code"]);
                        drpProjectCode.ClearSelection();
                        ListItem li = drpProjectCode.Items.FindByValue(System.Web.HttpUtility.HtmlDecode(sCs));
                        if (li != null) li.Selected = true;
                    }

                    if (ds.Tables[0].Rows[0]["Task_Description"] != null)
                        txtTaskDesc.Text = ds.Tables[0].Rows[0]["Task_Description"].ToString();

                    if (ds.Tables[0].Rows[0]["Dependency_Task"] != null)
                    {
                        string sCu = Convert.ToString(ds.Tables[0].Rows[0]["Dependency_Task"]);
                        drpDependencyTask.ClearSelection();
                        ListItem li = drpDependencyTask.Items.FindByValue(System.Web.HttpUtility.HtmlDecode(sCu));
                        if (li != null) li.Selected = true;
                    }

                    if (ds.Tables[0].Rows[0]["IsActive"] != null)
                        drpIsActive.SelectedValue = ds.Tables[0].Rows[0]["IsActive"].ToString();

                    if (ds.Tables[0].Rows[0]["Task_Type"] != null)
                    {
                        string sCus = Convert.ToString(ds.Tables[0].Rows[0]["Task_Type"]);
                        drpTaskType.ClearSelection();
                        ListItem li = drpTaskType.Items.FindByValue(System.Web.HttpUtility.HtmlDecode(sCus));
                        if (li != null) li.Selected = true;
                    }

                    if (ds.Tables[0].Rows[0]["Owner"] != null)
                    {
                        string sCustomer = Convert.ToString(ds.Tables[0].Rows[0]["Owner"]);
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
}
