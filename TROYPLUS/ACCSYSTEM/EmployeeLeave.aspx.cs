using System;
using System.Configuration;
using System.Data;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class EmployeeLeave : System.Web.UI.Page
{
    public string sDataSource = string.Empty;

    protected void Page_Load(object sender, EventArgs e)
    {
        String url = Request.ServerVariables["URL"];
        url = url.Remove(0, url.LastIndexOf("/") + 1);

        sDataSource = ConfigurationManager.ConnectionStrings[Request.Cookies["Company"].Value].ToString();

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

            if (objChk.CheckForOffline(Server.MapPath("Offline\\" + dbfileName + ".offline")))
            {
                lnkBtnApplyLeave.Visible = false;
                //grdViewAttendanceSummary.Columns[7].Visible = false;
                //grdViewAttendanceSummary.Columns[8].Visible = false;
            }
            grdViewLeaveSummary.PageSize = 8;

            string connection = Request.Cookies["Company"].Value;
            string usernam = Request.Cookies["LoggedUserName"].Value;
            BusinessLogic bl = new BusinessLogic(sDataSource);
            BindLeaveSummaryGrid();
            //if (bl.CheckUserHaveAdd(usernam, "SUPPINFO"))
            //{
            //    lnkBtnAddAttendance.Enabled = false;
            //    lnkBtnAddAttendance.ToolTip = "You are not allowed to make Add New ";
            //}
            //else
            //{
            //    lnkBtnAddAttendance.Enabled = true;
            //    lnkBtnAddAttendance.ToolTip = "Click to Add New ";
            //}



            if (Request.QueryString["myname"] != null)
            {

                string myNam = Request.QueryString["myname"].ToString();
                if (myNam == "NEWSUP")
                {
                    if (!Helper.IsLicenced(Request.Cookies["Company"].Value))
                    {
                        ScriptManager.RegisterStartupScript(Page, Page.GetType(), Guid.NewGuid().ToString(), "alert('This is Trial Version, Please upgrade to Full Version of this Software. Thank You.');", true);
                        return;
                    }
                }
            }
        }
    }

    protected void lnkBtnApplyLeave_Click(object sender, EventArgs e)
    {
        try
        {
            ModalPopupExtender1.Show();
            LeaveDetailPopUp.Visible = true;
            BusinessLogic bl = new BusinessLogic(sDataSource);
            UserInfo userInfo = bl.GetUserInfoByName(Request.Cookies["LoggedUserName"].Value);

            lblApproverName.Text = userInfo.ManagerEmpName;
            hdfApproverEmpNo.Value = userInfo.ManagerEmpNo.ToString();

            ViewState["PopupMode"] = "NEW";
        }
        catch (Exception ex)
        {
            TroyLiteExceptionManager.HandleException(ex);
        }
    }

    protected void grdViewLeaveSummary_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        try
        {
            int leaveId = 0;
            if (e.CommandName.Equals("EditLeave"))
            {
                if (int.TryParse(e.CommandArgument.ToString(), out leaveId))
                {
                    PopupDialogBindData(leaveId);
                    ModalPopupExtender1.Show();
                    LeaveDetailPopUp.Visible = true;
                    ViewState["PopupMode"] = "UPDATE";
                }
            }
            else if (e.CommandName.Equals("CancelLeave"))
            {
                if (int.TryParse(e.CommandArgument.ToString(), out leaveId))
                {
                    if (CancelLeave(leaveId))
                    {
                        ScriptManager.RegisterStartupScript(Page, Page.GetType(), Guid.NewGuid().ToString(), "alert('Your leave request has been cancelled successfully');", true);
                        BindLeaveSummaryGrid();
                        UpdatePanelMain.Update();
                    }
                }
            }
        }
        catch (Exception ex)
        {
            TroyLiteExceptionManager.HandleException(ex);
        }
    }

    protected void btnApplyLeave_Click(object sender, EventArgs e)
    {
        try
        {
            if (ViewState["PopupMode"] != null && ViewState["PopupMode"].ToString() == "NEW")
            {
                if (InsertLeave())
                {
                    ScriptManager.RegisterStartupScript(Page, Page.GetType(), Guid.NewGuid().ToString(), "alert('Your leave request has been submitted successfully');", true);
                    ClearPopupData();
                    BindLeaveSummaryGrid();
                    UpdatePanelMain.Update();
                }
            }
            else if (ViewState["PopupMode"] != null && ViewState["PopupMode"].ToString() == "UPDATE")
            {
                if (UpdateLeave())
                {
                    ScriptManager.RegisterStartupScript(Page, Page.GetType(), Guid.NewGuid().ToString(), "alert('Your leave request has been updated successfully');", true);
                    ClearPopupData();
                    BindLeaveSummaryGrid();
                    UpdatePanelMain.Update();

                }
            }
        }
        catch (Exception ex)
        {
            TroyLiteExceptionManager.HandleException(ex);
        }
    }

    protected void btnClearFilter_Click(object sender, EventArgs e)
    {
        this.txtSearchInput.Text = string.Empty;
        this.BindLeaveSummaryGrid();
    }

    protected void btnSearchAttendance_Click(object sender, EventArgs e)
    {
        this.BindLeaveSummaryGrid();
    }

    protected void ddlLeaveType_SelectedIndexChanged(object sender, EventArgs e)
    {
        try
        {
            if (ddlLeaveType.SelectedIndex >= 0)
            {
                int leaveTypeId = 0;
                if (int.TryParse(ddlLeaveType.SelectedValue, out leaveTypeId))
                {
                    BusinessLogic bl = new BusinessLogic(sDataSource);
                    string username = Request.Cookies["LoggedUserName"].Value;
                    int employeeNo = bl.GetUserInfoByName(username).EmpNo;
                    double leaveLimit = double.Parse(bl.GetLeaveLimit(leaveTypeId, employeeNo).ToString());
                    double leavesTaken = bl.GetTotalLeavesTaken(DateTime.Today.Year, leaveTypeId, employeeNo);
                    double leaveBalance = (leaveLimit - leavesTaken);
                    txtBalanceLeaves.Text = string.Format("{0}", leaveBalance.ToString());
                    ModalPopupExtender1.Show();
                }
            }
        }
        catch (Exception ex)
        {
            TroyLiteExceptionManager.HandleException(ex);
        }
    }

    protected void txtEndDate_TextChanged(object sender, EventArgs e)
    {
        txtTotalLeaveDays.Text = string.Empty;
    }

    protected void btnCalculateTotalLeaveDays_Click(object sender, EventArgs e)
    {
        try
        {
            BusinessLogic bl = new BusinessLogic(sDataSource);
            txtTotalLeaveDays.Text = string.Empty;
            if (!string.IsNullOrEmpty(txtStartDate.Text) && !string.IsNullOrEmpty(txtEndDate.Text))
            {
                DateTime StartDate = DateTime.Parse(txtStartDate.Text.Trim());
                string StartDateSession = ddlStartDateSession.SelectedValue.Trim(); ;
                DateTime EndDate = DateTime.Parse(txtEndDate.Text.Trim());
                string EndDateSession = ddlEndDateSession.SelectedValue.Trim();

                txtTotalLeaveDays.Text = bl.CalculateTotalLeaveDays(StartDate, StartDateSession, EndDate, EndDateSession).ToString();
            }
            ModalPopupExtender1.Show();
        }
        catch (Exception ex)
        {
            TroyLiteExceptionManager.HandleException(ex);
        }

    }

    protected void btnCancelNew_Click(object sender, EventArgs e)
    {
        ClearPopupData();
    }
    protected void ddlStartDateSession_SelectedIndexChanged(object sender, EventArgs e)
    {
        txtTotalLeaveDays.Text = string.Empty;
    }
    #region Private Methods
    private void BindLeaveSummaryGrid()
    {
        grdViewLeaveSummary.DataSource = null;
        grdViewLeaveSummary.DataBind();

        string connection = Request.Cookies["Company"].Value;
        string usernam = Request.Cookies["LoggedUserName"].Value;
        BusinessLogic bl = new BusinessLogic(sDataSource);

        DataSet ds = bl.GetLeaveSummary(usernam, txtSearchInput.Text.Trim());
        if (ds != null && ds.Tables.Count > 0)
        {
            grdViewLeaveSummary.DataSource = ds.Tables[0];
            grdViewLeaveSummary.DataBind();
            UpdatePanelMain.Update();
        }
    }

    private bool InsertLeave()
    {
        string connection = Request.Cookies["Company"].Value;
        string EmployeeNo = Request.Cookies["LoggedUserName"].Value;
        DateTime StartDate = DateTime.Parse(txtStartDate.Text.Trim());
        string StartDateSession = ddlStartDateSession.SelectedValue.Trim(); ;
        DateTime EndDate = DateTime.Parse(txtEndDate.Text.Trim());
        string EndDateSession = ddlEndDateSession.SelectedValue.Trim();
        DateTime DateApplied = DateTime.Now;
        string LeaveTypeId = ddlLeaveType.SelectedValue.Trim();
        string Reason = txtReason.Text.Trim();
        string Approver = hdfApproverEmpNo.Value.Trim();
        string EmailContact = txtEmailContact.Text.Trim();
        string PhoneContact = txtPhoneContact.Text.Trim();

        BusinessLogic bl = new BusinessLogic(connection);
        int leaveId = bl.ApplyLeave(EmployeeNo, StartDate, StartDateSession, EndDate, EndDateSession, DateApplied, LeaveTypeId, Reason, Approver, EmailContact, PhoneContact);
        if (leaveId > 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private bool UpdateLeave()
    {
        string connection = Request.Cookies["Company"].Value;
        string EmployeeNo = Request.Cookies["LoggedUserName"].Value;
        string leaveId = hdfLeaveId.Value;
        DateTime StartDate = DateTime.Parse(txtStartDate.Text.Trim());
        string StartDateSession = ddlStartDateSession.SelectedValue.Trim(); ;
        DateTime EndDate = DateTime.Parse(txtEndDate.Text.Trim());
        string EndDateSession = ddlEndDateSession.SelectedValue.Trim();
        DateTime DateApplied = DateTime.Now;
        string LeaveTypeId = ddlLeaveType.SelectedValue.Trim();
        string Reason = txtReason.Text.Trim();
        string Approver = hdfApproverEmpNo.Value.Trim();
        string EmailContact = txtEmailContact.Text.Trim();
        string PhoneContact = txtPhoneContact.Text.Trim();

        BusinessLogic bl = new BusinessLogic(connection);
        int result = bl.UpdateLeave(leaveId, EmployeeNo, StartDate, StartDateSession, EndDate, EndDateSession, DateApplied, LeaveTypeId, Reason, Approver, EmailContact, PhoneContact);

        if (result > 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private bool CancelLeave(int leaveId)
    {
        string connection = Request.Cookies["Company"].Value;

        BusinessLogic bl = new BusinessLogic(connection);
        int result = bl.DeleteLeave(leaveId.ToString());

        if (result > 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private void PopupDialogBindData(int leaveId)
    {
        BusinessLogic bl = new BusinessLogic(sDataSource);
        DataTable dt = bl.GetLeaveDetailsById(leaveId);
        DateTime date;

        hdfLeaveId.Value = leaveId.ToString();
        if (DateTime.TryParse(dt.Rows[0]["StartDate"].ToString(), out date))
        {
            txtStartDate.Text = date.ToShortDateString();
        }
        ddlStartDateSession.SelectedValue = dt.Rows[0]["StartDateSession"].ToString();

        if (DateTime.TryParse(dt.Rows[0]["EndDate"].ToString(), out date))
        {
            txtEndDate.Text = date.ToShortDateString();
        }
        ddlEndDateSession.SelectedValue = dt.Rows[0]["EndDateSession"].ToString();

        ddlLeaveType.SelectedValue = dt.Rows[0]["LeaveTypeId"].ToString();
        txtReason.Text = dt.Rows[0]["Reason"].ToString();
        lblApproverName.Text = dt.Rows[0]["ApproverName"].ToString();
        hdfApproverEmpNo.Value = dt.Rows[0]["Approver"].ToString();
        txtEmailContact.Text = dt.Rows[0]["EmailContact"].ToString();
        txtPhoneContact.Text = dt.Rows[0]["PhoneContact"].ToString();
    }

    private void ClearPopupData()
    {
        hdfLeaveId.Value = string.Empty;
        txtStartDate.Text = string.Empty;
        ddlStartDateSession.SelectedValue = "FN";
        txtEndDate.Text = string.Empty;
        ddlEndDateSession.SelectedValue = "AN";
        txtTotalLeaveDays.Text = string.Empty;
        txtBalanceLeaves.Text = string.Empty;
        txtReason.Text = string.Empty;
        lblApproverName.Text = string.Empty;
        hdfApproverEmpNo.Value = string.Empty;
        txtEmailContact.Text = string.Empty;
        txtPhoneContact.Text = string.Empty;
    }
    #endregion
}