using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class Attendance_EmployeeAttendance : System.Web.UI.Page
{
    public string sDataSource = string.Empty;

    protected override void OnInit(EventArgs e)
    {
        try
        {
            base.OnInit(e);
            //TextBox search = (TextBox)Accordion1.FindControl("txtSearch");
            //DropDownList dropDown = (DropDownList)Accordion1.FindControl("ddCriteria");
            AttendanceSummaryGridSource.SelectParameters.Add(new CookieParameter("connection", "Company"));
            AttendanceSummaryGridSource.SelectParameters.Add(new ControlParameter("txtSearchInput", TypeCode.String, txtSearchInput.UniqueID, "Text"));
            AttendanceSummaryGridSource.SelectParameters.Add(new ControlParameter("searchCriteria", TypeCode.String, ddlSearchCriteria.UniqueID, "SelectedValue"));
            AttendanceSummaryGridSource.SelectParameters.Add(new CookieParameter("UserId", "LoggedUserName"));
        }
        catch (Exception ex)
        {
            TroyLiteExceptionManager.HandleException(ex);
        }
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        try
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
                    lnkBtnAddAttendance.Visible = false;
                    //grdViewAttendanceSummary.Columns[7].Visible = false;
                    //grdViewAttendanceSummary.Columns[8].Visible = false;
                }
                grdViewAttendanceSummary.PageSize = 8;

                string connection = Request.Cookies["Company"].Value;
                string usernam = Request.Cookies["LoggedUserName"].Value;
                BusinessLogic bl = new BusinessLogic(sDataSource);
                BindAttendanceSummaryFilterList();
                if (ddlSearchCriteria.SelectedIndex >= 0)
                {
                    BindAttendanceSummaryGrid(ddlSearchCriteria.SelectedValue);
                }
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
        catch (Exception ex)
        {
            TroyLiteExceptionManager.HandleException(ex);
        }
    }

    protected void lnkBtnAddAttendance_Click(object sender, EventArgs e)
    {
        try
        {
            string connection = Request.Cookies["Company"].Value;
            string username = Request.Cookies["LoggedUserName"].Value;
            BusinessLogic bl = new BusinessLogic(connection);
            if (!bl.IsAttendanceSummaryExists(username, DateTime.Today.Year.ToString(), DateTime.Today.Month.ToString()))
            {
                DataTable dtAttendanceDetails = bl.GetNewAttendanceDetailsForMonth(connection, DateTime.Today.Year, DateTime.Today.Month, username).Tables[0];

                ViewState["AttendanceYear"] = DateTime.Today.Year.ToString();
                ViewState["AttendanceMonth"] = DateTime.Today.Month.ToString();
                Session["DtAttendanceDetails"] = dtAttendanceDetails;
                GridViewAttendanceDetail.DataSource = dtAttendanceDetails;
                if (!hdnfIsGridLoaded.Value.Equals("1"))
                    ChangeGridColumnHeaderText();
                GridViewAttendanceDetail.DataBind();
                hdnfIsNewAttendance.Value = "1";

                GridViewAttendanceDetail.Visible = true;
                AttendanceDetailPopUp.Visible = true;
                ModalPopupExtender1.Show();
            }
            else
            {
                ScriptManager.RegisterStartupScript(Page, Page.GetType(), Guid.NewGuid().ToString(),
                    @"alert('Attendance has been already available for the period " + DateTime.Today.ToString("MMM", CultureInfo.InvariantCulture) + "-" + DateTime.Today.Year.ToString() + "');", true);
            }
        }

        catch (Exception ex)
        {
            TroyLiteExceptionManager.HandleException(ex);
        }
    }

    protected void GridViewAttendanceDetail_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        try
        {
            if (e.Row.RowIndex >= 0)
            {
                foreach (TableCell cell in e.Row.Cells)
                {
                    if (cell.Controls.Count > 1)
                    {
                        if (cell.Controls[1].ToString().Equals("System.Web.UI.WebControls.Button"))
                        {
                            Button btn = cell.Controls[1] as Button;
                            if (!btn.Text.Equals(string.Empty))
                            {
                                ToggleAttendanceMark(btn);
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

    protected void grdViewAttendanceSummary_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        try
        {
            if (e.CommandName.Equals("EditRecord"))
            {
                int attendanceID = 0;
                string[] args = e.CommandArgument.ToString().Split(new char[] { ':' });
                if (args.Length == 3)
                {
                    if (int.TryParse(args[0], out attendanceID))
                    {
                        hdnfAttendanceID.Value = attendanceID.ToString();
                        string connection = Request.Cookies["Company"].Value;
                        string username = Request.Cookies["LoggedUserName"].Value;
                        BusinessLogic bl = new BusinessLogic(connection);
                        DataSet dtAttendanceDetail = bl.GetAttendanceDetails(attendanceID, username);
                        if (dtAttendanceDetail != null)
                        {
                            hdnfIsNewAttendance.Value = "0";

                            ViewState["AttendanceYear"] = args[1];
                            ViewState["AttendanceMonth"] = args[2];
                            Session["DtAttendanceDetails"] = dtAttendanceDetail.Tables[0];
                            GridViewAttendanceDetail.DataSource = dtAttendanceDetail.Tables[0];
                            if (!hdnfIsGridLoaded.Value.Equals("1"))
                                ChangeGridColumnHeaderText();
                            GridViewAttendanceDetail.DataBind();

                            GridViewAttendanceDetail.Visible = true;
                            AttendanceDetailPopUp.Visible = true;
                            ModalPopupExtender1.Show();
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

    protected void ToggleAttendance_Click(object sender, EventArgs e)
    {
        try
        {
            ToggleAttendanceMark(sender as Button, true);
            updPnlAttendanceDeailsGrid.Update();
        }
        catch (Exception ex)
        {
            TroyLiteExceptionManager.HandleException(ex);
        }
    }

    protected void btnSaveAttendance_Click(object sender, EventArgs e)
    {
        try
        {
            if (SaveAttendanceDetails() > 0)
            {
                ScriptManager.RegisterStartupScript(Page, Page.GetType(), Guid.NewGuid().ToString(),
                    @"alert('Attendance has been saved successfully');", true);
                if (ddlSearchCriteria.SelectedIndex >= 0)
                {
                    BindAttendanceSummaryGrid(ddlSearchCriteria.SelectedValue);
                }
            }
            else
            {
                ScriptManager.RegisterStartupScript(Page, Page.GetType(), Guid.NewGuid().ToString(),
                    @"alert('Unable to save attendance details, please contact your Administrator.');", true);
            }
        }
        catch (Exception ex)
        {
            TroyLiteExceptionManager.HandleException(ex);
        }
    }

    protected void btnSubmitAttendance_Click(object sender, EventArgs e)
    {
        try
        {
            int attendanceID = -1;

            attendanceID = SaveAttendanceDetails();
            if (attendanceID > 0)
            {
                if (SubmitAttendance(attendanceID))
                {
                    ScriptManager.RegisterStartupScript(Page, Page.GetType(), Guid.NewGuid().ToString(),
                   @"alert('Attendance details saved and submitted successfully');", true);
                    if (ddlSearchCriteria.SelectedIndex >= 0)
                    {
                        BindAttendanceSummaryGrid(ddlSearchCriteria.SelectedValue);
                    }
                }
                else
                {
                    ScriptManager.RegisterStartupScript(Page, Page.GetType(), Guid.NewGuid().ToString(),
                   @"alert('Attendance details saved but not submitted, please contact your Administrator.');", true);
                    if (ddlSearchCriteria.SelectedIndex >= 0)
                    {
                        BindAttendanceSummaryGrid(ddlSearchCriteria.SelectedValue);
                    }
                }
            }
            else
            {
                ScriptManager.RegisterStartupScript(Page, Page.GetType(), Guid.NewGuid().ToString(),
                    @"alert('Unable to save attendance details, please contact your Administrator.');", true);
            }

        }
        catch (Exception ex)
        {
            TroyLiteExceptionManager.HandleException(ex);
        }
    }

    protected void ddlSearchCriteria_SelectedIndexChanged(object sender, EventArgs e)
    {
        try
        {
            if (ddlSearchCriteria.SelectedIndex >= 0)
            {
                BindAttendanceSummaryGrid(ddlSearchCriteria.SelectedValue);
            }
        }
        catch (Exception ex)
        {
            TroyLiteExceptionManager.HandleException(ex);
        }
    }

    #region Private Methods
    private void ToggleAttendanceMark(Button btnSender, bool isUpdate = false)
    {
        if (btnSender != null)
        {
            // verify the value and change the next applicable value for the button
            if (btnSender.Text == "Present")
            {
                if (isUpdate)
                {
                    btnSender.Text = "Leave";
                    btnSender.CssClass = "btnBts btnBts-warning";
                }
                else
                    btnSender.CssClass = "btnBts btnBts-default";
                //btnSender.BackColor = Color.AntiqueWhite;
            }
            else if (btnSender.Text == "Leave")
            {
                if (isUpdate)
                {
                    btnSender.Text = "Week Off";
                    btnSender.CssClass = "btnBts btnBts-success";
                }
                else { btnSender.CssClass = "btnBts btnBts-warning"; }

            }
            else if (btnSender.Text == "Week Off")
            {
                if (isUpdate)
                {
                    btnSender.Text = "Holiday";
                    btnSender.CssClass = "btnBts btnBts-info";
                }
                else { btnSender.CssClass = "btnBts btnBts-success"; }

            }
            else if (btnSender.Text == "Holiday")
            {
                if (isUpdate)
                {
                    btnSender.Text = "Present";
                    btnSender.CssClass = "btnBts btnBts-default";
                }
                else
                    btnSender.CssClass = "btnBts btnBts-info";

            }
            else if (btnSender.Text == "NA")
            {
                btnSender.CssClass = "btnBts btnBts-disabled";
            }
        }
    }

    private void ChangeGridColumnHeaderText()
    {
        DataTable dtGridSrc = GridViewAttendanceDetail.DataSource as DataTable;
        foreach (DataControlField column in GridViewAttendanceDetail.Columns)
        {
            if (dtGridSrc.Columns.Contains(column.HeaderText))
            {
                column.HeaderText = GetColumnHeaderTextForCell(column.HeaderText, GridViewAttendanceDetail.DataSource as DataTable);

                if (column.HeaderText.Equals("NA"))
                {
                    column.Visible = false;
                }
            }
            else
            {
                column.Visible = false;
            }
        }
        hdnfIsGridLoaded.Value = "1";
    }

    private string GetColumnHeaderTextForCell(string colHeaderName, DataTable dtSource)
    {
        DateTime dateValue;
        if (DateTime.TryParse(dtSource.Columns[colHeaderName].Caption, out dateValue))
        {
            return dateValue.ToShortDateString() + "\r\n" + dateValue.DayOfWeek;
        }
        else
        {
            return dtSource.Columns[colHeaderName].Caption;
        }
    }

    private int SaveAttendanceDetails()
    {
        DataTable dtAttendanceDetail = new DataTable();
        int attendanceId = 0;

        dtAttendanceDetail.Columns.Add(new DataColumn("EmpNo"));
        dtAttendanceDetail.Columns.Add(new DataColumn("Date"));
        dtAttendanceDetail.Columns.Add(new DataColumn("Remarks"));
        if (Session["DtAttendanceDetails"] != null)
        {
            DataTable dtGridSource = Session["DtAttendanceDetails"] as DataTable;
            int rowIndex = 0;
            foreach (GridViewRow row in GridViewAttendanceDetail.Rows)
            {
                string[] rowItem = new string[3];
                int cellIndex = 0;
                foreach (TableCell cell in row.Cells)
                {
                    if (cellIndex == 0)
                    {
                        rowItem[0] = dtGridSource.Rows[rowIndex][cellIndex].ToString();
                    }

                    if (cell.Controls.Count > 1)
                    {
                        if (cell.Controls[1].ToString().Equals("System.Web.UI.WebControls.Button"))
                        {
                            Button btn = cell.Controls[1] as Button;
                            if (!btn.Text.Equals(string.Empty) && !btn.Text.Equals("NA"))
                            {
                                // Get date value
                                string dateValue = GridViewAttendanceDetail.Columns[cellIndex].HeaderText.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries).First();
                                rowItem[1] = dateValue;
                                // Get attendance mark
                                rowItem[2] = btn.Text;

                                dtAttendanceDetail.Rows.Add(rowItem.ToArray());
                            }
                        }
                    }
                    cellIndex++;

                }
                rowIndex++;
            }

            // Update records in the database
            string connection = Request.Cookies["Company"].Value;
            string username = Request.Cookies["LoggedUserName"].Value;
            BusinessLogic bl = new BusinessLogic(connection);
            bool createSummary = false;
            if (hdnfIsNewAttendance.Value.Equals("1"))
                createSummary = true;

            if (bl.SaveAttendanceDetail(dtAttendanceDetail, username, string.Empty, ViewState["AttendanceYear"].ToString(),
                ViewState["AttendanceMonth"].ToString(), createSummary, out attendanceId))
            {
                if (!createSummary)
                    int.TryParse(hdnfAttendanceID.Value, out attendanceId);
                hdnfAttendanceID.Value = string.Empty;
                hdnfIsNewAttendance.Value = string.Empty;
                return attendanceId;
            }
        }
        return attendanceId;
    }

    private bool SubmitAttendance(int attendanceID)
    {
        string connection = Request.Cookies["Company"].Value;
        string username = Request.Cookies["LoggedUserName"].Value;
        BusinessLogic bl = new BusinessLogic(connection);
        return bl.SubmitAttendance(attendanceID);
    }

    private void BindAttendanceSummaryGrid(string attendanceYear)
    {
        string connection = Request.Cookies["Company"].Value;
        string usernam = Request.Cookies["LoggedUserName"].Value;
        BusinessLogic bl = new BusinessLogic(sDataSource);

        DataSet ds = bl.GetAttendanceSummary(attendanceYear, usernam);
        if (ds != null && ds.Tables.Count > 0)
        {
            grdViewAttendanceSummary.DataSource = ds.Tables[0];
            grdViewAttendanceSummary.DataBind();
            UpdatePanelMain.Update();
        }
        else
        {
            grdViewAttendanceSummary.DataSource = null;
            grdViewAttendanceSummary.DataBind();
            UpdatePanelMain.Update();
        }

    }

    private void BindAttendanceSummaryFilterList()
    {
        string connection = Request.Cookies["Company"].Value;
        string usernam = Request.Cookies["LoggedUserName"].Value;
        BusinessLogic bl = new BusinessLogic(sDataSource);

        DataTable dt = bl.GetAttendanceYearList(usernam);
        if (dt != null)
        {
            ddlSearchCriteria.DataSource = dt;
            ddlSearchCriteria.DataBind();
            UpdatePanelMain.Update();
        }
    }

    #endregion


}