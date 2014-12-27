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
using System.Data.OleDb;
using DataAccessLayer;

public partial class ReconDate : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {

        if (!Page.IsPostBack)
        {
            string connStr = string.Empty;

            myRangeValidator.MinimumValue = System.DateTime.Now.AddYears(-100).ToShortDateString();
            myRangeValidator.MaximumValue = System.DateTime.Now.ToShortDateString();

            if (Request.Cookies["Company"] != null)
                connStr = System.Configuration.ConfigurationManager.ConnectionStrings[Request.Cookies["Company"].Value].ToString();
            else
                Response.Redirect("~/Login.aspx");

            string dbfileName = connStr.Remove(0, connStr.LastIndexOf(@"App_Data\") + 9);
            dbfileName = dbfileName.Remove(dbfileName.LastIndexOf(";Persist Security Info"));
            BusinessLogic objChk = new BusinessLogic();

            if (objChk.CheckForOffline(Server.MapPath("Offline\\" + dbfileName + ".offline")))
            {
                lnkBtnUpdate.Enabled = false;
            }

            DBManager manager = new DBManager(DataProvider.OleDb);
            manager.ConnectionString = objChk.CreateConnectionString(System.Configuration.ConfigurationManager.ConnectionStrings[Request.Cookies["Company"].Value].ConnectionString);

            try
            {
                manager.Open();
                object reconDate = manager.ExecuteScalar(CommandType.Text, "Select recon_date from last_recon");

                if (reconDate != null)
                    txtReconDate.Text = DateTime.Parse(reconDate.ToString()).ToString("dd/MM/yyyy");

                //if (Request.Cookies["Company"] != null)
                //    sDataSource = ConfigurationManager.ConnectionStrings[Request.Cookies["Company"].Value].ToString();
                BusinessLogic bl = new BusinessLogic(connStr);

                CheckSMSRequired();

                if (Session["SMSREQUIRED"] != null)
                {
                    if (Session["SMSREQUIRED"].ToString() == "NO")
                        hdSMSRequired.Value = "NO";
                    else
                        hdSMSRequired.Value = "YES";
                }
                else
                {
                    hdSMSRequired.Value = "NO";
                }



                if (Session["EMAILREQUIRED"] != null)
                {
                    if (Session["EMAILREQUIRED"].ToString() == "NO")
                        hdEmailRequired.Value = "NO";
                    else
                        hdEmailRequired.Value = "YES";
                }
                else
                {
                    hdEmailRequired.Value = "NO";
                }

                DataSet ds = bl.GetSettings();
                
                if (ds != null)
                {
                    if (ds.Tables[0] != null)
                    {
                        if (ds.Tables[0].Rows.Count > 0)
                        {
                            foreach (DataRow dr in ds.Tables[0].Rows)
                            {
                                if (dr["Key"].ToString() == "ENBLDATE")
                                {
                                    if (dr["KeyValue"] != null)
                                        rdvoudateenable.SelectedValue = dr["KeyValue"].ToString();
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
    }

    private void CheckSMSRequired()
    {
        DataSet appSettings;
        string smsRequired = string.Empty;
        string emailRequired = string.Empty;

        if (Session["AppSettings"] != null)
        {
            appSettings = (DataSet)Session["AppSettings"];

            for (int i = 0; i < appSettings.Tables[0].Rows.Count; i++)
            {
                if (appSettings.Tables[0].Rows[i]["KEY"].ToString() == "SMSREQ")
                {
                    smsRequired = appSettings.Tables[0].Rows[i]["KEYVALUE"].ToString();
                    Session["SMSREQUIRED"] = smsRequired.Trim().ToUpper();
                }
                if (appSettings.Tables[0].Rows[i]["KEY"].ToString() == "EMAILREQ")
                {
                    emailRequired = appSettings.Tables[0].Rows[i]["KEYVALUE"].ToString();
                    Session["EMAILREQUIRED"] = emailRequired.Trim().ToUpper();
                }

                if (appSettings.Tables[0].Rows[i]["KEY"].ToString() == "OWNERMOB")
                {
                    Session["OWNERMOB"] = appSettings.Tables[0].Rows[i]["KEYVALUE"].ToString();
                }

            }
        }

    }

    protected void lnkBtnUpdate_Click(object sender, EventArgs e)
    {
        BusinessLogic objBus = new BusinessLogic();
        DBManager manager = new DBManager(DataProvider.OleDb);
        manager.ConnectionString = objBus.CreateConnectionString(System.Configuration.ConfigurationManager.ConnectionStrings[Request.Cookies["Company"].Value].ConnectionString);
        
        string enabledate = string.Empty;
        enabledate = rdvoudateenable.SelectedValue;

        try
        {
            manager.Open();
            manager.ExecuteNonQuery(CommandType.Text, "Update last_recon Set recon_date=Format('" + txtReconDate.Text + "', 'dd/MM/yyyy')");

            manager.ExecuteNonQuery(CommandType.Text, "UPDATE tblSettings SET KEYVALUE = '" + enabledate.ToString() + "' WHERE KEY='ENBLDATE' ");

            //errorDisplay.AddItem("Recon Date updated successfully." , DisplayIcons.GreenTick,false);
            ScriptManager.RegisterStartupScript(Page, Page.GetType(), Guid.NewGuid().ToString(), "alert('Recon Date updated successfully.');", true);
            return;

            string salestype = string.Empty;
            int ScreenNo = 0;
            string ScreenName = string.Empty;

            BusinessLogic bl = new BusinessLogic();
            string connection = Request.Cookies["Company"].Value;

            salestype = "Lock Transaction";
            ScreenName = "Lock Transaction";
            DataSet dsddd = bl.GetScreenNoForScreenName(connection, ScreenName);
            if (dsddd != null)
            {
                if (dsddd.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow dr in dsddd.Tables[0].Rows)
                    {
                        ScreenNo = Convert.ToInt32(dr["ScreenNo"]);
                    }
                }
            }

            if (hdEmailRequired.Value == "YES")
            {
                var toAddress = "";
                
                string usernam = Request.Cookies["LoggedUserName"].Value;

                DataSet dsdd = bl.GetDetailsForScreenNo(connection, ScreenNo, "Email");
                if (dsdd != null)
                {
                    if (dsdd.Tables[0].Rows.Count > 0)
                    {
                        foreach (DataRow dr in dsdd.Tables[0].Rows)
                        {
                            
                                    toAddress = dr["EmailId"].ToString();

                                    string subject = "Updated - " + txtReconDate.Text + " Lock Transaction in Branch " + Request.Cookies["Company"].Value;

                                    string body = "\n";
                                    body += " Branch           : " + Request.Cookies["Company"].Value + "\n";
                                    body += " Date           : " + txtReconDate.Text + "\n";
                                    
                                    string smtphostname = ConfigurationManager.AppSettings["SmtpHostName"].ToString();
                                    int smtpport = Convert.ToInt32(ConfigurationManager.AppSettings["SmtpPortNumber"]);
                                    var fromAddress = ConfigurationManager.AppSettings["FromAddress"].ToString();

                                    string fromPassword = ConfigurationManager.AppSettings["FromPassword"].ToString();

                                    EmailLogic.SendEmail(smtphostname, smtpport, fromAddress, toAddress, subject, body, fromPassword);

                                    //ScriptManager.RegisterStartupScript(Page, Page.GetType(), Guid.NewGuid().ToString(), "alert('Email sent successfully')", true);
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
}
