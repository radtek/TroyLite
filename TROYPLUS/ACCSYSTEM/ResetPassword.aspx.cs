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
using System.Text;

public partial class ResetPassword : System.Web.UI.Page
{
    public string sDataSource = string.Empty;



    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
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

                loadUserName();

                if (objChk.CheckForOffline(Server.MapPath("Offline\\" + dbfileName + ".offline")))
                {
                    lnkBtnSave.Visible = false;
                }
            }
        }
        catch (Exception ex)
        {
            TroyLiteExceptionManager.HandleException(ex);
        }
    }

    private void loadUserName()
    {
        BusinessLogic bl = new BusinessLogic(sDataSource);
        DataSet ds = new DataSet();
        ds = bl.ListUserName(sDataSource);
        drpUserName.DataSource = ds;
        drpUserName.DataBind();
        drpUserName.DataTextField = "UserName";
        drpUserName.DataValueField = "UserName";
    }


    protected void lnkBtnSave_Click(object sender, EventArgs e)
    {
        try
        {
            StringBuilder msg = new StringBuilder();

            if (!Page.IsValid)
            {
                foreach (IValidator validator in Page.Validators)
                {
                    if (!validator.IsValid)
                    {
                        msg.Append(" - " + validator.ErrorMessage);
                        msg.Append("\\n");
                    }
                }

                ScriptManager.RegisterStartupScript(Page, Page.GetType(), Guid.NewGuid().ToString(), "alert('" + msg.ToString() + "');", true);
                return;
            }

            BusinessLogic objBus = new BusinessLogic();


            if (Request.Cookies["LoggedUserName"].Value != null && Request.Cookies["Company"] != null)
            {            

                objBus.ResetPassword(drpUserName.SelectedValue.ToString(), txtNewPass.Text, Request.Cookies["Company"].Value);               
                ScriptManager.RegisterStartupScript(Page, Page.GetType(), Guid.NewGuid().ToString(), "alert('Your password has been changed successfully.');", true);
                drpUserName.SelectedIndex = 0;
                txtNewPass.Text = string.Empty;
            }

        }
        catch (Exception ex)
        {
            TroyLiteExceptionManager.HandleException(ex);
        }
    }
}
