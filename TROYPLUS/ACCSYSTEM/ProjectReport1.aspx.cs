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

                if (Request.QueryString["cond"] != null)
                {
                    cond = Convert.ToString(Request.QueryString["cond"].ToString());
                }

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

          


        }


        catch (Exception ex)
        {
            TroyLiteExceptionManager.HandleException(ex);
        }
    }

    
}