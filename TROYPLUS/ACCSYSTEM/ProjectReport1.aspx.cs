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

public partial class ProjectReport1 : System.Web.UI.Page
{
    private string sDataSource = string.Empty;
    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            sDataSource = ConfigurationManager.ConnectionStrings[Request.Cookies["Company"].Value].ToString();
            if (!IsPostBack)
            {
                if (Request.Cookies["Company"] == null)
                    sDataSource = ConfigurationManager.ConnectionStrings[Request.Cookies["Company"].Value].ToString();
                DataSet companyInfo = new DataSet();
                string connection = Request.Cookies["Company"].Value;
                BusinessLogic bl = new BusinessLogic(sDataSource);
                lblBillDate.Text = DateTime.Now.ToShortDateString();
                if (Request.Cookies["Company"] != null)
                {
                    companyInfo = bl.getCompanyInfo(Request.Cookies["Company"].Value);

                    if (companyInfo != null)
                    {
                        if (companyInfo.Tables[0].Rows.Count > 0)
                        {
                            foreach (DataRow dr in companyInfo.Tables[0].Rows)
                            {
                                lblTNGST.Text = Convert.ToString(dr["TINno"]);
                                lblCompany.Text = Convert.ToString(dr["CompanyName"]);
                                lblPhone.Text = Convert.ToString(dr["Phone"]);
                                lblGSTno.Text = Convert.ToString(dr["GSTno"]);

                                lblAddress.Text = Convert.ToString(dr["Address"]);
                                lblCity.Text = Convert.ToString(dr["city"]);
                                lblPincode.Text = Convert.ToString(dr["Pincode"]);
                                lblState.Text = Convert.ToString(dr["state"]);

                            }
                        }
                    }
                }
                divPrint.Visible = true;
                int empno = 0;
                int pro_Id = 0;
                string flag = "";
                int status = 0;
                int taskid = 0;
                string isactive = "";
                int deptask = 0;
                int incharge = 0;

                if (Request.QueryString["incharge"] != null)
                {
                    incharge = Convert.ToInt32(Request.QueryString["incharge"].ToString());
                }

                if (Request.QueryString["employee"] != null)
                {
                    empno = Convert.ToInt32(Request.QueryString["employee"].ToString());
                }
                if (Request.QueryString["project"] != null)
                {
                    pro_Id = Convert.ToInt32(Request.QueryString["project"].ToString());
                }
                if (Request.QueryString["BlockedTask"] != null)
                {
                    flag = Convert.ToString(Request.QueryString["BlockedTask"].ToString());
                }
                if (Request.QueryString["CompletedTask"] != null)
                {
                    status = Convert.ToInt32(Request.QueryString["CompletedTask"].ToString());
                }
                if (Request.QueryString["Task"] != null)
                {
                    taskid = Convert.ToInt32(Request.QueryString["Task"].ToString());
                }
                if (Request.QueryString["DependencyTask"] != null)
                {
                    deptask = Convert.ToInt32(Request.QueryString["DependencyTask"].ToString());
                }
                if (Request.QueryString["isactive"] != null)
                {
                    isactive = Convert.ToString(Request.QueryString["isactive"].ToString());
                }
                string cond = string.Empty;




                cond = "1=1 and "; 


                if ((pro_Id != 0))
                {


                    cond += "  tblProjects.Project_Id=" + pro_Id + "";
                }




                cond += " and tblEmployee.empno=" + incharge + "";


                 DataSet ds = new DataSet();
                 ds = bl.GetProjectManagementReport(connection,incharge, empno, pro_Id, flag, status, taskid, deptask, isactive,cond);
                 if (ds!= null)
                 {
                     if (ds.Tables[0].Rows.Count > 0)
                     {

                         gvOuts1.DataSource = ds;
                         gvOuts1.DataBind();
                     }
                     else
                     {
                         gvOuts1.DataSource = null;
                         gvOuts1.DataBind();
                     }
                 }
                 else
                 {

                     gvOuts1.DataSource = null;
                     gvOuts1.DataBind();
                 }

            }


            //btnReport_Click(sender, e);

        }


        catch (Exception ex)
        {
            TroyLiteExceptionManager.HandleException(ex);
        }
    }

    protected string getCond()
    {
        int empno = 0;
        int pro_Id = 0;
        string flag = "";
        int status = 0;
        int taskid = 0;
        string isactive = "";
        int deptask = 0;
        int incharge = 0;

        if (Request.QueryString["incharge"] != null)
        {
            incharge = Convert.ToInt32(Request.QueryString["incharge"].ToString());
        }

        if (Request.QueryString["employee"] != null)
        {
            empno = Convert.ToInt32(Request.QueryString["employee"].ToString());
        }
        if (Request.QueryString["project"] != null)
        {
            pro_Id = Convert.ToInt32(Request.QueryString["project"].ToString());
        }
        if (Request.QueryString["BlockedTask"] != null)
        {
            flag = Convert.ToString(Request.QueryString["BlockedTask"].ToString());
        }
        if (Request.QueryString["CompletedTask"] != null)
        {
            status = Convert.ToInt32(Request.QueryString["CompletedTask"].ToString());
        }
        if (Request.QueryString["Task"] != null)
        {
            taskid = Convert.ToInt32(Request.QueryString["Task"].ToString());
        }
        if (Request.QueryString["DependencyTask"] != null)
        {
            deptask = Convert.ToInt32(Request.QueryString["DependencyTask"].ToString());
        }
        if (Request.QueryString["isactive"] != null)
        {
            isactive = Convert.ToString(Request.QueryString["isactive"].ToString());
        }
        string cond = string.Empty;




        cond = "1=1  ";


     

        if ((empno != 0))
        {

            cond += " and tblEmployee.empno=" + empno + "";
        }

        if (flag != "NA")
        {

            cond += " and  tblTaskUpdates.Blocked_Flag ='" + flag + "'";
        }

        if ((status != 0))
        {

            cond += " and tblTaskUpdates.Task_Status =" + Convert.ToInt32(status) + " ";

        }
        if ((taskid != 0))
        {

            cond += " and tblTasks.Task_Id =" + Convert.ToInt32(taskid) + " ";
        }

        if ((deptask != 0))
        {

            cond += " and tblTasks.Dependency_Task =" + Convert.ToInt32(deptask) + " ";
        }


        cond += " and tblTasks.IsActive ='" + isactive + "' ";


       


        return cond;
    }

    protected void gvProducts_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        try
        {
           
            if (e.Row.RowType == DataControlRowType.DataRow)
            {

                

            }
        }
        catch (Exception ex)
        {
            TroyLiteExceptionManager.HandleException(ex);
        }
    }

    protected void gvOuts1_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        try
        {
            string flag = "";
            int status = 0;
            string condition = getCond();
            //string sDataSource =  Server.MapPath("App_Data\\Store0910.mdb");
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                if (Request.QueryString["BlockedTask"] != null)
                {
                    flag = Convert.ToString(Request.QueryString["BlockedTask"].ToString());
                }
                if (Request.QueryString["CompletedTask"] != null)
                {
                    status = Convert.ToInt32(Request.QueryString["CompletedTask"].ToString());
                }

                GridView gv = e.Row.FindControl("gvProducts") as GridView;
                Label ProjectName = e.Row.FindControl("lblprojectname") as Label;
                BusinessLogic bl = new BusinessLogic();
                string connection = Request.Cookies["Company"].Value;
               
                    if (Request.Cookies["Company"] != null)
                        sDataSource = ConfigurationManager.ConnectionStrings[Request.Cookies["Company"].Value].ToString();

                    string Project_Name = Convert.ToString(ProjectName.Text);

                    DataSet ds = bl.GettaskforProjectName(connection, Project_Name, condition,flag,status);
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        gv.DataSource = ds;
                        gv.DataBind();
                    }
               

            }
        }
        catch (Exception ex)
        {
            TroyLiteExceptionManager.HandleException(ex);
        }
    }



    //protected void btnReport_Click(object sender, EventArgs e)
    //{
    //    try
    //    {
    //        int empno = 0;
    //        int pro_Id = 0;
    //        string flag = "";
    //        int status = 0;
    //        int taskid = 0;
    //        string isactive = "";
    //        int deptask = 0;
    //        int incharge = 0;
    //        string connection = Request.Cookies["Company"].Value;
    //        string cond = string.Empty;
           



    //        System.Text.StringBuilder htmlcode = new System.Text.StringBuilder();
    //        htmlcode.Append("<html><body>");
    //        //htmlcode.Append("<form id=form1 runat=server>");
    //        htmlcode.Append("<div id=divPrint style=font-family:'Trebuchet MS'; font-size:10px;  >");

    //        htmlcode.Append("<Table id = table1 border=1px solid blue cellpadding=0 cellspacing=0 class=tblLeft width=100% >");
    //        DataSet dsVat = new DataSet();
    //        DataSet dsCst = new DataSet();
    //        DataSet ds1 = new DataSet();
    //        BusinessLogic bl = new BusinessLogic(sDataSource);

    //        htmlcode.Append("<tr class=ReportHeadataRow style=text-align:left>");
    //        htmlcode.Append("<td> Project Name");
    //        htmlcode.Append("</td>");
    //        htmlcode.Append("<td> Project Date");
    //        htmlcode.Append("</td>");
    //        htmlcode.Append("<td> Expected Start Date");
    //        htmlcode.Append("</td>");
    //        htmlcode.Append("<td> Expected End Date");
    //        htmlcode.Append("</td>");
    //        htmlcode.Append("<td> Expected start date");
    //        htmlcode.Append("</td>");

    //        int i = 0;
    //        int j = 0;

    //        DataSet dspro = new DataSet();



    //        //int empno = 0;
    //        //int pro_Id = 0;
    //        //string flag = "";
    //        //int status = 0;
    //        //int taskid = 0;
    //        //string isactive = "";
    //        //int deptask = 0;
    //        //int incharge = 0;

    //        if (Request.QueryString["incharge"] != null)
    //        {
    //            incharge = Convert.ToInt32(Request.QueryString["incharge"].ToString());
    //        }

    //        if (Request.QueryString["employee"] != null)
    //        {
    //            empno = Convert.ToInt32(Request.QueryString["employee"].ToString());
    //        }
    //        if (Request.QueryString["project"] != null)
    //        {
    //            pro_Id = Convert.ToInt32(Request.QueryString["project"].ToString());
    //        }
    //        if (Request.QueryString["BlockedTask"] != null)
    //        {
    //            flag = Convert.ToString(Request.QueryString["BlockedTask"].ToString());
    //        }
    //        if (Request.QueryString["CompletedTask"] != null)
    //        {
    //            status = Convert.ToInt32(Request.QueryString["CompletedTask"].ToString());
    //        }
    //        if (Request.QueryString["Task"] != null)
    //        {
    //            taskid = Convert.ToInt32(Request.QueryString["Task"].ToString());
    //        }
    //        if (Request.QueryString["DependencyTask"] != null)
    //        {
    //            deptask = Convert.ToInt32(Request.QueryString["DependencyTask"].ToString());
    //        }
    //        if (Request.QueryString["isactive"] != null)
    //        {
    //            isactive = Convert.ToString(Request.QueryString["isactive"].ToString());
    //        }
    //      //  string cond = string.Empty;

    //        if ((pro_Id != 0))
    //        {


    //            cond += "  tblProjects.Project_Id=" + pro_Id + "";
    //        }




    //        cond += " and tblEmployee.empno=" + incharge + "";

    //        dspro = bl.GetProjectManagementReport(connection, incharge, empno, pro_Id, flag, status, taskid, deptask, isactive, cond);
    //        DataSet ds = new DataSet();
    //        if (dspro.Tables[0].Rows.Count > 0)
    //        {
    //            for (i = 0; i < dspro.Tables[0].Rows.Count; i++)
    //            {
    //                if (dspro.Tables[0].Rows[i].ItemArray[0].ToString() == "0")
    //                {
    //                    htmlcode.Append("<td> NO Data Found");
    //                    htmlcode.Append("</td>");

    //                }
    //                else
    //                {
                      
    //                }
    //            }
    //        }
    //        else
    //        {
    //            //Label.text="Error";
    //        }
    //        //dsCst = bl.ListPurchaseCst();
    //        //for (i = 1; i < dsCst.Tables[0].Rows.Count; i++)
    //        //{
    //        //    htmlcode.Append("<td>" + dsCst.Tables[0].Rows[i].ItemArray[0].ToString() + " Purchase Value");
    //        //    htmlcode.Append("</td>");
    //        //    htmlcode.Append("<td>" + dsCst.Tables[0].Rows[i].ItemArray[0].ToString() + " Input CST");
    //        //    htmlcode.Append("</td>");
    //        //}
    //        //htmlcode.Append("<td> Total Sales");
    //        //htmlcode.Append("</td>");
    //        //htmlcode.Append("</tr>");



    //        //Double[] Total;
    //        //Total = new double[50];
    //        //int TotCount = 0;

    //        //ds1 = bl.ListPurchaseVatCstAmtDet(Convert.ToDateTime(txtStartDate.Text), Convert.ToDateTime(txtEndDate.Text), Convert.ToInt16(drpLedgerName.SelectedValue), Convert.ToInt16(drpVat.SelectedValue), Convert.ToInt16(drpCst.SelectedValue));
    //        //if (ds1 != null)
    //        //{
    //        //    for (j = 0; j < ds1.Tables[0].Rows.Count; j++)
    //        //    {
    //        //        TotCount = 0;
    //        //        htmlcode.Append("<tr class=ReportdataRow>");
    //        //        htmlcode.Append("<td>" + ds1.Tables[0].Rows[j].ItemArray[0].ToString());
    //        //        htmlcode.Append("</td>");
    //        //        htmlcode.Append("<td>" + ds1.Tables[0].Rows[j].ItemArray[1].ToString());
    //        //        htmlcode.Append("</td>");
    //        //        htmlcode.Append("<td>" + ds1.Tables[0].Rows[j].ItemArray[2].ToString());
    //        //        htmlcode.Append("</td>");
    //        //        TotSal = Convert.ToDouble(ds1.Tables[0].Rows[j].ItemArray[3].ToString());
    //        //        Total[TotCount] = Total[TotCount] + TotSal;
    //        //        for (i = 0; i < dsVat.Tables[0].Rows.Count; i++)
    //        //        {

    //        //            TotCount = TotCount + 1;
    //        //            if (ds1.Tables[0].Rows[j].ItemArray[6].ToString() == dsVat.Tables[0].Rows[i].ItemArray[0].ToString())
    //        //            {
    //        //                if (ds1.Tables[0].Rows[j].ItemArray[6].ToString() == "0")
    //        //                {
    //        //                    if (ds1.Tables[0].Rows[j].ItemArray[7].ToString() == "0")
    //        //                    {
    //        //                        htmlcode.Append("<td>" + ds1.Tables[0].Rows[j].ItemArray[3].ToString());
    //        //                        htmlcode.Append("</td>");
    //        //                        Total[TotCount] = Total[TotCount] + Convert.ToDouble(ds1.Tables[0].Rows[j].ItemArray[3].ToString());
    //        //                        TotSal = TotSal + ((TotSal * Convert.ToDouble(Convert.ToDouble(ds1.Tables[0].Rows[j].ItemArray[6].ToString()))) / 100);
    //        //                        //htmlcode.Append("<td>" + ((Convert.ToDouble(ds1.Tables[0].Rows[j].ItemArray[3].ToString())) * Convert.ToDouble(ds1.Tables[0].Rows[j].ItemArray[6].ToString())) / 100);
    //        //                        //htmlcode.Append("</td>");

    //        //                    }
    //        //                    else
    //        //                    {

    //        //                        htmlcode.Append("<td>");
    //        //                        htmlcode.Append("</td>");
    //        //                        Total[TotCount] = Total[TotCount] + 0;
    //        //                    }
    //        //                }
    //        //                else
    //        //                {
    //        //                    htmlcode.Append("<td>" + ds1.Tables[0].Rows[j].ItemArray[3].ToString());
    //        //                    htmlcode.Append("</td>");

    //        //                    TotSal = TotSal + ((TotSal * Convert.ToDouble(Convert.ToDouble(ds1.Tables[0].Rows[j].ItemArray[6].ToString()))) / 100);
    //        //                    htmlcode.Append("<td>" + ((Convert.ToDouble(ds1.Tables[0].Rows[j].ItemArray[3].ToString())) * Convert.ToDouble(ds1.Tables[0].Rows[j].ItemArray[6].ToString())) / 100);
    //        //                    htmlcode.Append("</td>");
    //        //                    Total[TotCount] = Total[TotCount] + Convert.ToDouble(ds1.Tables[0].Rows[j].ItemArray[3].ToString());
    //        //                    TotCount = TotCount + 1;
    //        //                    Total[TotCount] = Total[TotCount] + (Convert.ToDouble(ds1.Tables[0].Rows[j].ItemArray[3].ToString()) * Convert.ToDouble(ds1.Tables[0].Rows[j].ItemArray[6].ToString()) / 100);
    //        //                }
    //        //            }
    //        //            else
    //        //            {
    //        //                if (dsVat.Tables[0].Rows[i].ItemArray[0].ToString() == "0")
    //        //                {
    //        //                    htmlcode.Append("<td>");
    //        //                    htmlcode.Append("</td>");
    //        //                    Total[TotCount] = Total[TotCount] + 0;
    //        //                }
    //        //                else
    //        //                {
    //        //                    htmlcode.Append("<td>");
    //        //                    htmlcode.Append("</td>");
    //        //                    htmlcode.Append("<td>");
    //        //                    htmlcode.Append("</td>");
    //        //                    Total[TotCount] = Total[TotCount] + 0;
    //        //                    TotCount = TotCount + 1;
    //        //                    Total[TotCount] = Total[TotCount] + 0;

    //        //                }
    //        //            }
    //        //        }
    //        //        for (i = 1; i < dsCst.Tables[0].Rows.Count; i++)
    //        //        {

    //        //            TotCount = TotCount + 1;
    //        //            if (ds1.Tables[0].Rows[j].ItemArray[7].ToString() == dsCst.Tables[0].Rows[i].ItemArray[0].ToString())
    //        //            {
    //        //                htmlcode.Append("<td>" + ds1.Tables[0].Rows[j].ItemArray[3].ToString());
    //        //                htmlcode.Append("</td>");
    //        //                TotSal = TotSal + ((TotSal * Convert.ToDouble(Convert.ToDouble(ds1.Tables[0].Rows[j].ItemArray[7].ToString()))) / 100);
    //        //                htmlcode.Append("<td>" + ((Convert.ToDouble(ds1.Tables[0].Rows[j].ItemArray[3].ToString())) * Convert.ToDouble(ds1.Tables[0].Rows[j].ItemArray[7].ToString())) / 100);
    //        //                htmlcode.Append("</td>");
    //        //                Total[TotCount] = Total[TotCount] + Convert.ToDouble(ds1.Tables[0].Rows[j].ItemArray[3].ToString());
    //        //                TotCount = TotCount + 1;
    //        //                Total[TotCount] = Total[TotCount] + (Convert.ToDouble(ds1.Tables[0].Rows[j].ItemArray[3].ToString()) * Convert.ToDouble(ds1.Tables[0].Rows[j].ItemArray[7].ToString()) / 100);
    //        //            }
    //        //            //else
    //        //            //{
    //        //            //    htmlcode.Append("<td>");
    //        //            //    htmlcode.Append("</td>");
    //        //            //    htmlcode.Append("<td>");
    //        //            //    htmlcode.Append("</td>");
    //        //            //    Total[TotCount] = Total[TotCount] + 0;
    //        //            //    TotCount = TotCount + 1;
    //        //            //    Total[TotCount] = Total[TotCount] + 0;
    //        //            //}
    //        //        }
    //        //        //TotCount = TotCount + 1;
    //        //        //Total[TotCount] = Total[TotCount] + TotSal;
    //        //        //htmlcode.Append("<td>" + TotSal);
    //        //        //htmlcode.Append("</td>");
    //        //        //htmlcode.Append("</tr>");

    //        //    }
    //            //htmlcode.Append("<tr class=ReportFooterRow>");
    //            //htmlcode.Append("<td>");
    //            //htmlcode.Append("</td>");
    //            //htmlcode.Append("<td>");
    //            //htmlcode.Append("</td>");
    //            //htmlcode.Append("<td>");
    //            //htmlcode.Append("</td>");
    //            //TotCount = 1;
    //            //for (i = 0; i < dsVat.Tables[0].Rows.Count; i++)
    //            //{
    //            //    if (i == 0)
    //            //    {
    //            //        htmlcode.Append("<td>" + Total[TotCount]);
    //            //        htmlcode.Append("</td>");
    //            //        TotCount = TotCount + 1;
    //            //    }
    //            //    else
    //            //    {
    //            //        htmlcode.Append("<td>" + Total[TotCount]);
    //            //        htmlcode.Append("</td>");
    //            //        TotCount = TotCount + 1;
    //            //        htmlcode.Append("<td>" + Total[TotCount]);
    //            //        htmlcode.Append("</td>");
    //            //        TotCount = TotCount + 1;
    //            //    }
    //            //}
    //            //for (i = 1; i < dsCst.Tables[0].Rows.Count; i++)
    //            //{
    //            //    htmlcode.Append("<td>" + Total[TotCount]);
    //            //    htmlcode.Append("</td>");
    //            //    TotCount = TotCount + 1;
    //            //    htmlcode.Append("<td>" + Total[TotCount]);
    //            //    htmlcode.Append("</td>");
    //            //    TotCount = TotCount + 1;
    //            //}
    //            //htmlcode.Append("<td>" + Total[TotCount]);
    //            //htmlcode.Append("</td>");
    //            //htmlcode.Append("</tr>");
    //            //htmlcode.Append("</div>");
    //            //htmlcode.Append("</Table>");

    //            ////htmlcode.Append("</form>");
    //            //htmlcode.Append("</body></html>");

    //            //string s = htmlcode.ToString();
    //            //divReport.InnerHtml = htmlcode.ToString();

    //            //ExportToExcel();
    //        htmlcode.Append("</div>");
    //        htmlcode.Append("</Table>");

    //        //htmlcode.Append("</form>");
    //        htmlcode.Append("</body></html>");

    //        string s = htmlcode.ToString();
    //        divPrint.InnerHtml = htmlcode.ToString();
    //        }
        
    //    catch (Exception ex)
    //    {
    //        TroyLiteExceptionManager.HandleException(ex);
    //    }
    //}


    
}