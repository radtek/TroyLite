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
public partial class ReportExcelLeadManagement : System.Web.UI.Page
{
    //DBClass objdb = new DBClass();
    BusinessLogic objBL;
    public string sDataSource = string.Empty;
    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            sDataSource = ConfigurationManager.ConnectionStrings[Request.Cookies["Company"].Value].ToString();
            objBL = new BusinessLogic(ConfigurationManager.ConnectionStrings[Request.Cookies["Company"].Value].ToString());
            //loadCodes();
            //loadModels();
            //DateEndformat();
            //DateStartformat();
            

            if (!IsPostBack)
            {
                txtStrtDt.Text = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1).ToShortDateString();
                txtEndDt.Text = DateTime.Now.ToShortDateString();
                loadCategory();
            }
        }
        catch (Exception ex)
        {
            TroyLiteExceptionManager.HandleException(ex);
        }
    }

    protected void DateEndformat()
    {
        System.DateTime dt = System.DateTime.Now.Date;
        txtEndDt.Text = string.Format("{0:dd/MM/yyyy}", dt);
    }

    protected void GridCust_RowCreated(object sender, GridViewRowEventArgs e)
    {
        try
        {
            if (e.Row.RowType == DataControlRowType.Pager)
            {
                PresentationUtils.SetPagerButtonStates(GridCust, e.Row, this);
            }
        }
        catch (Exception ex)
        {
            TroyLiteExceptionManager.HandleException(ex);
        }
    }
    
    protected void drpProjectCode_SelectedIndexChanged(object sender, EventArgs e)
    {
        
    }

    protected void drpIncharge_SelectedIndexChanged(object sender, EventArgs e)
    {

    }

    protected void drpTaskType_SelectedIndexChanged(object sender, EventArgs e)
    {

    }

    protected void drpTaskStatus_SelectedIndexChanged(object sender, EventArgs e)
    {

    }

    private void loadBrands()
    {
        
    }

    protected void DateStartformat()
    {
        DateTime dtCurrent = DateTime.Now;
        DateTime dtNew = new DateTime(dtCurrent.Year, dtCurrent.Month, 1);
        txtStrtDt.Text = string.Format("{0:dd/MM/yyyy}", dtNew);
    }

    private void loadCategory()
    {
        string connection = Request.Cookies["Company"].Value;
        BusinessLogic bl = new BusinessLogic(sDataSource);
        DataSet dst = new DataSet();

        //dst = bl.GetProjectList(connection, "", "");
        //drpProjectCode.DataSource = dst;
        //drpProjectCode.DataBind();
        //drpProjectCode.DataTextField = "Project_Name";
        //drpProjectCode.DataValueField = "Project_Id";

        //drpProjectCode.Items.Insert(0, new ListItem("All", "All"));

        //DataSet ds = new DataSet();
        //ds = bl.ListExecutive();
        //drpIncharge.DataSource = ds;
        //drpIncharge.DataBind();
        //drpIncharge.DataTextField = "empFirstName";
        //drpIncharge.DataValueField = "empno";
        //drpIncharge.Items.Insert(0, new ListItem("All", "All"));

        //DataSet dsd = new DataSet();
        //dsd = bl.ListTaskTypesInfo(connection, "", "");
        //drpTaskType.DataSource = dsd;
        //drpTaskType.DataBind();
        //drpTaskType.DataTextField = "Task_Type_Name";
        //drpTaskType.DataValueField = "Task_Type_Id";
        //drpTaskType.Items.Insert(0, new ListItem("All", "All"));

        //DataSet dsdd = new DataSet();
        //dsdd = bl.ListTaskStatusInfo(connection, "", "");
        //drpTaskStatus.DataSource = dsdd;
        //drpTaskStatus.DataBind();
        //drpTaskStatus.DataTextField = "Task_Status_Name";
        //drpTaskStatus.DataValueField = "Task_Status_Id";
        //drpTaskStatus.Items.Insert(0, new ListItem("All", "All"));
    }

    private void loaProducts()
    {
        

        //ddlproduct.Items.Insert(0, new ListItem("All", "All"));
    }

    private void loadCodes()
    {
        
    }

    //private void loadModels()
    //{
    //    //string sDataSource = Server.MapPath(ConfigurationSettings.AppSettings["DataSource"].ToString());
    //    BusinessLogic bl = new BusinessLogic();
    //    DataSet ds = new DataSet();
    //    ds = bl.ListAllModels();
    //    cmbProdAdd.DataTextField = "Model";
    //    cmbProdAdd.DataValueField = "Model";
    //    cmbProdAdd.DataSource = ds;
    //    cmbProdAdd.DataBind();
    //}

    private void BindGrid()
    {
        CreditLimitTotal = 0;
        OpenBalanceDRTotal = 0;
        DataSet ds = new DataSet();

        string connStr = string.Empty;

        if (Request.Cookies["Company"] != null)
            connStr = System.Configuration.ConfigurationManager.ConnectionStrings[Request.Cookies["Company"].Value].ToString();
        else
            Response.Redirect("~/Login.aspx");

        BusinessLogic bl = new BusinessLogic();

        
        DataSet dsSales = bl.ListProductHistory(connStr.Trim(), "");

        if (dsSales != null)
        {
            if (dsSales.Tables[0].Rows.Count > 0)
            {
                GridCust.DataSource = dsSales;
                GridCust.DataBind();
            }
        }
    }



    protected void btnData_Click(object sender, EventArgs e)
    {
        try
        {
            BindGrid();
        }
        catch (Exception ex)
        {
            TroyLiteExceptionManager.HandleException(ex);
        }
    }

    protected void GridCust_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        try
        {
            GridCust.PageIndex = e.NewPageIndex;

            BindGrid();
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
            GridCust.PageIndex = ((DropDownList)sender).SelectedIndex;
            BindGrid();
        }
        catch (Exception ex)
        {
            TroyLiteExceptionManager.HandleException(ex);
        }
    }

    protected void btnxls_Click(object sender, EventArgs e)
    {
        try
        {
            DateTime startDate;
            DateTime endDate;

            DataSet ds = new DataSet();

            startDate = Convert.ToDateTime(txtStrtDt.Text.Trim());
            endDate = Convert.ToDateTime(txtEndDt.Text.Trim());

            string connection = string.Empty;

            if (Request.Cookies["Company"] != null)
                connection = Request.Cookies["Company"].Value;
            else
                Response.Redirect("Login.aspx");

            BusinessLogic bl = new BusinessLogic();

            string condtion = "";
            condtion = getCond();

            ds = objBL.GetLeadManagementList(connection, startDate, endDate);

            if (ds != null)
            {
                if (ds.Tables[0].Rows.Count > 0)
                {
                    DataTable dt = new DataTable();
                    dt.Columns.Add(new DataColumn("Lead No"));
                    dt.Columns.Add(new DataColumn("Lead Name"));
                    dt.Columns.Add(new DataColumn("Customer Name"));
                    dt.Columns.Add(new DataColumn("Address"));
                    dt.Columns.Add(new DataColumn("Mobile"));
                    dt.Columns.Add(new DataColumn("Telephone"));
                    dt.Columns.Add(new DataColumn("Record Status"));
                    dt.Columns.Add(new DataColumn("Closing Date"));
                    dt.Columns.Add(new DataColumn("Employee Name"));
                    dt.Columns.Add(new DataColumn("Start Date"));                  
                    dt.Columns.Add(new DataColumn("Lead Status"));
                    dt.Columns.Add(new DataColumn("Contact Name"));
                    dt.Columns.Add(new DataColumn("Predicted Closing Date"));
                    dt.Columns.Add(new DataColumn("Competitor Name"));
                    dt.Columns.Add(new DataColumn("Activity Name"));
                    dt.Columns.Add(new DataColumn("Activity Date"));
                    dt.Columns.Add(new DataColumn("Activity Location"));
                    dt.Columns.Add(new DataColumn("Next Activity"));
                    dt.Columns.Add(new DataColumn("Next Activity Date"));
                    //dt.Columns.Add(new DataColumn("Activity Employee Name"));
                    //dt.Columns.Add(new DataColumn("Mode of Contact"));
                    //dt.Columns.Add(new DataColumn("Product Name"));

                    DataRow dr_final123 = dt.NewRow();
                    dt.Rows.Add(dr_final123);

                    foreach (DataRow dr in ds.Tables[0].Rows)
                    {
                        DataRow dr_final1 = dt.NewRow();
                        dr_final1["Lead No"] = dr["Lead_No"];
                        dr_final1["Lead Name"] = dr["Lead_Name"];
                        dr_final1["Customer Name"] = dr["BP_Name"];
                        dr_final1["Address"] = dr["Address"];
                        dr_final1["Telephone"] = dr["Telephone"];
                        dr_final1["Record Status"] = dr["Doc_Status"];                                              

                        string aad = dr["Closing_Date"].ToString().ToUpper().Trim();
                        string dtaad = Convert.ToDateTime(aad).ToString("dd/MM/yyyy");
                        if (dtaad != "01/01/2000")
                        {
                            dr_final1["Closing Date"] = dtaad;
                        }
                        

                        dr_final1["Employee Name"] = dr["Emp_Name"];

                        string aa = dr["Start_Date"].ToString().ToUpper().Trim();
                        string dtaa = Convert.ToDateTime(aa).ToString("dd/MM/yyyy");
                        dr_final1["Start Date"] = dtaa;

                        dr_final1["Lead Status"] = dr["Lead_Status"];
                        dr_final1["Contact Name"] = dr["Contact_Name"];                       

                        string aae = dr["Predicted_Closing_Date"].ToString().ToUpper().Trim();
                        string dtaae = Convert.ToDateTime(aae).ToString("dd/MM/yyyy");
                        dr_final1["Predicted Closing Date"] = dtaae;

                        dr_final1["Competitor Name"] = dr["Competitor_Name"];
                        dr_final1["Activity Name"] = dr["Activity_Name"];

                        string aaee = dr["Activity_Date"].ToString().ToUpper().Trim();
                        string dtaaee = Convert.ToDateTime(aaee).ToString("dd/MM/yyyy");
                        dr_final1["Activity Date"] = dtaaee;

                        dr_final1["Activity Location"] = dr["Activity_Location"];
                        dr_final1["Next Activity"] = dr["Next_Activity"];

                        string aaeee = dr["NextActivity_Date"].ToString().ToUpper().Trim();
                        string dtaaeee = Convert.ToDateTime(aaeee).ToString("dd/MM/yyyy");
                        dr_final1["Next Activity Date"] = dtaaeee;

                        //dr_final1["Activity Employee Name"] = dr["Emp_Name"];
                        //dr_final1["Mode of Contact"] = dr["ModeofContact"];
                        //dr_final1["Product Name"] = dr["Product_Name"];

                        dt.Rows.Add(dr_final1);
                    }
                    DataRow dr_final2 = dt.NewRow();
                    dt.Rows.Add(dr_final2);
                    ExportToExcel(dt);
                }
                else
                {
                    ScriptManager.RegisterStartupScript(Page, typeof(Button), "MyScript", "alert('No Data Found');", true);
                }
            }
            else
            {
                ScriptManager.RegisterStartupScript(Page, typeof(Button), "MyScript", "alert('No Data Found');", true);
            }
        }
        catch (Exception ex)
        {
            TroyLiteExceptionManager.HandleException(ex);
        }
    }

    protected string getCond()
    {
        string cond = " 1=1 ";

        //if (drpProjectCode.SelectedIndex > 0)
        //{
        //    cond += " and tblProjects.Project_Name='" + drpProjectCode.SelectedItem.Text + "' ";
        //}
        //if (drpIncharge.SelectedIndex > 0)
        //{
        //    cond += " and tblEmployee.empfirstname='" + drpIncharge.SelectedItem.Text + "' ";
        //}
        //if (drpTaskStatus.SelectedIndex > 0)
        //{
        //    cond += " and tblTaskStatus.Task_Status_Name='" + drpTaskStatus.SelectedItem.Text + "' ";
        //}
        //if (drpTaskType.SelectedIndex > 0)
        //{
        //    cond += " and tblTaskTypes.Task_Type_Name='" + drpTaskType.SelectedItem.Text + "' ";
        //}
        return cond;
    }

    public override void VerifyRenderingInServerForm(Control control)
    {

    }

    public void ExportToExcel(DataTable dt)
    {

        if (dt.Rows.Count > 0)
        {
            string filename = "Lead Management.xls";
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
    decimal CreditLimitTotal;
    decimal OpenBalanceDRTotal;

    protected void GridCust_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            
        }

        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            
        }
    }
    protected void btnReport_Click(object sender, EventArgs e)
    {
        try
        {
            if (Page.IsValid)
            {
                string connection = string.Empty;

                if (Request.Cookies["Company"] != null)
                    connection = Request.Cookies["Company"].Value;
                else
                    Response.Redirect("Login.aspx");


                DateTime startDate, endDate;
                string category = string.Empty;

                startDate = Convert.ToDateTime(txtStrtDt.Text.Trim());
                endDate = Convert.ToDateTime(txtEndDt.Text.Trim());

                string condtion = "";
                condtion = getCond();            

                DataSet BillDs = new DataSet();
                BusinessLogic bl = new BusinessLogic(sDataSource);

                BillDs = bl.GetLeadManagementList(connection, startDate, endDate);


                Response.Write("<script language='javascript'> window.open('LeadManagementReport.aspx?startDate=" + Convert.ToDateTime(startDate) + "&endDate=" + Convert.ToDateTime(endDate) + " ' , 'window','height=700,width=1000,left=172,top=10,toolbar=yes,scrollbars=yes,resizable=yes');</script>");
            }
        }
        catch (Exception ex)
        {
            TroyLiteExceptionManager.HandleException(ex);
        }
    }
}
