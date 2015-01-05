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
using System.Xml;
using System.IO;
using System.Globalization;


public partial class Purchase : System.Web.UI.Page
{  

    private string sDataSource = string.Empty;
    private double amtTotal = 0;
    double disTotalRate = 0.0;
    public double rateTotal = 0;
    public double vatTotal = 0;
    public double disTotal = 0;
    public double cstTotal = 0;
    public double WholeTotal;
    string BarCodeRequired = string.Empty;
    string EnableVat = string.Empty;   

    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            sDataSource = ConfigurationManager.ConnectionStrings[Request.Cookies["Company"].Value].ToString();

            string dbfileName = sDataSource.Remove(0, sDataSource.LastIndexOf(@"App_Data\") + 9);
            dbfileName = dbfileName.Remove(dbfileName.LastIndexOf(";Persist Security Info"));
            BusinessLogic objChk = new BusinessLogic();
            if (Session["NEWPURCHASE"] != null)
            {
                if (Session["NEWPURCHASE"].ToString() == "Y")
                {

                    //purchasePanel.Visible = true;
                    cmdSave.Enabled = true;
                    cmdUpdate.Enabled = false;
                    //cmdDelete.Enabled = false;
                    cmdUpdateProduct.Enabled = false;
                    //cmdCancelProduct.Visible = false;
                    cmdUpdateProduct.Visible = false;
                    //Label3.Visible = false;
                    cmdSaveProduct.Enabled = true;
                    cmdSaveProduct.Visible = true;
                    //Label2.Visible = true;
                    hdMode.Value = "New";
                    Reset();
                    lblTotalSum.Text = "0";
                    /*Start Purchase Loading / Unloading Freight Change - March 16*/
                    lblFreight.Text = "0";
                    /*End Purchase Loading / Unloading Freight Change - March 16*/
                    ResetProduct();
                    if (Session["PurchaseBillDate"] != null)
                        txtBillDate.Text = Session["PurchaseBillDate"].ToString();

                    Session["NEWPURCHASE"] = "N";
                    btnCancel.Enabled = true;
                    Session["PurchaseProductDs"] = null;
                    GrdViewItems.DataSource = null;
                    GrdViewItems.DataBind();


                    rowSalesRet.Visible = false;
                }
            }

            if (objChk.CheckForOffline(Server.MapPath("Offline\\" + dbfileName + ".offline")))
            {
                cmdSaveProduct.Enabled = false;
                GrdViewItems.Columns[8].Visible = false;
                GrdViewItems.Columns[9].Visible = false;
                GrdViewPurchase.Columns[14].Visible = false;
                cmdSave.Enabled = false;
                cmdUpdate.Enabled = false;
                lnkBtnAdd.Visible = false;
                pnlSearch.Visible = false;
            }


            GrdViewPurchase.PageSize = 8;


            if (!IsPostBack)
            {

                BusinessLogic bl = new BusinessLogic(sDataSource);
                string connection = Request.Cookies["Company"].Value;
                string usernam = Request.Cookies["LoggedUserName"].Value;

                if (bl.CheckUserHaveAdd(usernam, "PURCHS"))
                {
                    lnkBtnAdd.Enabled = false;
                    lnkBtnAdd.ToolTip = "You are not allowed to make Add New ";
                }
                else
                {
                    lnkBtnAdd.Enabled = true;
                    lnkBtnAdd.ToolTip = "Click to Add New item ";
                }

                CheckSMSRequired();

                if (Request.QueryString["myname"] != null)
                {
                    string myNam = Request.QueryString["myname"].ToString();
                    if (myNam == "NEWPUR")
                    {
                        ModalPopupMethod.Show();
                    }
                }

                EnableVat = bl.getEnableVatConfig();
                if (EnableVat == "YES")
                {
                    lblVATAdd.Enabled = true;
                }
                else
                {
                    lblVATAdd.Enabled = false;
                }


                BindCurrencyLabels();

                if (BarCodeRequired == "YES")
                {
                    rowBarcode.Visible = true;
                }
                else
                {
                    rowBarcode.Visible = false;
                }

                txtBarcode.Attributes.Add("onKeyPress", " return clickButton(event,'" + cmdBarcode.ClientID + "')");
                BindGrid("0", "0");
                //GenerateRoleDs();
                hdFilename.Value = System.Guid.NewGuid().ToString();
                loadSupplier("Sundry Creditors");
                //loadProducts();
                loadBanks();
                loadBilts("0");
                loadCategories();

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

                //rvBillDate.MinimumValue = System.DateTime.Now.AddYears(-100).ToShortDateString();
                //rvBillDate.MaximumValue = System.DateTime.Now.ToShortDateString();

                //valdate.MinimumValue = System.DateTime.Now.AddYears(-100).ToShortDateString();
                //valdate.MaximumValue = System.DateTime.Now.ToShortDateString();
                UpdatePnlMaster.Update();
            }
            Session["Filename"] = "Reports//" + hdFilename.Value + "_Product.xml";
            err.Text = "";
            //cmdBarcode.Click += new EventHandler(this.txtBarcode_Populated); //Jolo Barcode
            //txtBarcode.Attributes.Add("onblur", "txtBarcode_Populated();"); //Jolo Barcode     
        }
        catch (Exception ex)
        {
            TroyLiteExceptionManager.HandleException(ex);
        }
    }

    protected void BtnClearFilter_Click(object sender, EventArgs e)
    {
        try
        {
            txtSearch.Text = "";
            ddCriteria.SelectedIndex = 0;
            BindGrid("","");
        }
        catch (Exception ex)
        {
            TroyLiteExceptionManager.HandleException(ex);
        }
    }

    protected void chk_CheckedChanged(object sender, EventArgs e)
    {
        if (chk.Checked == false)
        {
            txtSupplier.Visible = true;
            cmbSupplier.Visible = false;

            txtAddress1.Enabled = true;
            txtAddress2.Enabled = true;
            txtAddress3.Enabled = true;
            txtMobile.Enabled = true;
        }
        else
        {
            cmbSupplier.Visible = true;
            txtSupplier.Visible = false;

            txtAddress1.Enabled = false;
            txtAddress2.Enabled = false;
            txtAddress3.Enabled = false;
            txtMobile.Enabled = false;
        }
        //UpdatePanel21.Update();
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
                //if (appSettings.Tables[0].Rows[i]["KEY"].ToString() == "CREDITEXD")
                //{
                //    Session["CREDITEXD"] = appSettings.Tables[0].Rows[i]["KEYVALUE"].ToString().Trim().ToUpper();
                //    hdCREDITEXD.Value = appSettings.Tables[0].Rows[i]["KEYVALUE"].ToString().Trim().ToUpper();
                //}

                if (appSettings.Tables[0].Rows[i]["KEY"].ToString() == "OWNERMOB")
                {
                    Session["OWNERMOB"] = appSettings.Tables[0].Rows[i]["KEYVALUE"].ToString();
                }

            }
        }

    }

    private void BindCurrencyLabels()
    {
        DataSet appSettings;
        string currency = string.Empty;

        if (Session["AppSettings"] != null)
        {
            appSettings = (DataSet)Session["AppSettings"];

            for (int i = 0; i < appSettings.Tables[0].Rows.Count; i++)
            {
                if (appSettings.Tables[0].Rows[i]["KEY"].ToString() == "CURRENCY")
                {
                    currency = appSettings.Tables[0].Rows[i]["KEYVALUE"].ToString();
                    Session["CurrencyType"] = currency;
                }

                if (appSettings.Tables[0].Rows[i]["KEY"].ToString() == "BARCODE")
                {
                    BarCodeRequired = appSettings.Tables[0].Rows[i]["KEYVALUE"].ToString();
                }
            }
        }

        if ((currency == "INR") || (currency.ToUpper() == "RS"))
        {
            lblDispTotal.Text = "Total (INR)";
            lblDispDisRate.Text = "Discounted Rate (INR)";
            lblDispIncVAT.Text = "VAT (INR)";
            lblDispIncCST.Text = "Inclusive CST (INR)";
            lblDispLoad.Text = "Loading / Unloading / Freight (INR)";
            lblDispGrandTtl.Text = "GRAND Total (INR)";
        }

        if (currency == "GBP")
        {
            lblDispTotal.Text = "Total (£)";
            lblDispDisRate.Text = "Discounted Rate (£)";
            lblDispIncVAT.Text = "VAT (£)";
            lblDispIncCST.Text = "Inclusive CST (£)";
            lblDispLoad.Text = "Loading / Unloading / Freight (£)";
            lblDispGrandTtl.Text = "GRAND Total (£)";

        }

    }

    private void loadBanks()
    {
        //string sDataSource = Server.MapPath(ConfigurationSettings.AppSettings["DataSource"].ToString());
        BusinessLogic bl = new BusinessLogic(sDataSource);
        DataSet ds = new DataSet();
        ds = bl.ListBanks();
        cmbBankName.DataSource = ds;
        cmbBankName.DataBind();
        cmbBankName.DataTextField = "LedgerName";
        cmbBankName.DataValueField = "LedgerID";

    }
    protected void btnSearch_Click(object sender, EventArgs e)
    {
        try
        {
            //var strBillno = txtBillnoSrc.Text.Trim();
            //var strTransno = txtTransNo.Text.Trim();
            string textt = string.Empty;
            string dropd = string.Empty;

            textt = txtSearch.Text;
            dropd = ddCriteria.SelectedValue;

            //Accordion1.SelectedIndex = 0;
            BindGrid(textt, dropd);
            GrdViewItems.DataSource = null;
            GrdViewItems.DataBind();
            lblTotalSum.Text = "0";
            delFlag.Value = "0";
            Session["PurchaseProductDs"] = null;
        }
        catch (Exception ex)
        {
            TroyLiteExceptionManager.HandleException(ex);
        }
    }

    protected void cmdCancel_Click(object sender, EventArgs e)
    {
        try
        {
            GrdViewItems.Columns[12].Visible = true;
            GrdViewItems.Columns[13].Visible = true;

            Reset();
            ResetProduct();
            txtBillDate.Text = DateTime.Now.ToShortDateString();
            cmbProdAdd.Enabled = true;
            //cmdUpdateProduct.Enabled = false;
            cmdSaveProduct.Enabled = true;
            //btnCancel.Enabled = false;
            //PanelCmd.Visible = false;
            GrdViewItems.DataSource = null;
            GrdViewItems.DataBind();
            //GridView1.DataSource = null;
            //GridView1.DataSource = null;
            Session["PurchaseProductDs"] = null;
            ////pnlRole.Visible = false;
            cmdSave.Enabled = true;
            //MyAccordion.Visible = true;
            cmdUpdate.Enabled = false;
            cmdPrint.Enabled = false;
            lblTotalDis.Text = "0";
            lblTotalCST.Text = "0";
            lblTotalSum.Text = "0";
            lblTotalVAT.Text = "0";
            lblNet.Text = "0";
            /*Start Purchase Loading / Unloading Freight Change - March 16*/
            lblFreight.Text = "0";
            /*End Purchase Loading / Unloading Freight Change - March 16*/

            //updatePnlPurchase.Update();
            ModalPopupPurchase.Hide();
            if (Session["Show"] == "Hide")
            {
                ModalPopupMethod.Hide();
            }
            else
            {
                ModalPopupMethod.Show();
            }
            UP1.Update();
            //UpdatePnlMaster.Update();
            BusinessLogic objChk = new BusinessLogic();
            string dbfileName = sDataSource.Remove(0, sDataSource.LastIndexOf(@"App_Data\") + 9);
            dbfileName = dbfileName.Remove(dbfileName.LastIndexOf(";Persist Security Info"));

            if (objChk.CheckForOffline(Server.MapPath("Offline\\" + dbfileName + ".offline")))
            {
                cmdSaveProduct.Enabled = false;
                GrdViewItems.Columns[8].Visible = false;
                GrdViewItems.Columns[9].Visible = false;
                GrdViewPurchase.Columns[14].Visible = false;
                cmdSave.Enabled = false;
                cmdUpdate.Enabled = false;
                lnkBtnAdd.Visible = false;
                pnlSearch.Visible = false;
            }
        }
        catch (Exception ex)
        {
            TroyLiteExceptionManager.HandleException(ex);
        }
    }

    private DataSet ComputateDiff(DataSet newVersion, DataSet oldVersion)
    {
        DataSet diff = null;
        oldVersion.Merge(newVersion);
        bool foundChanges = oldVersion.HasChanges();
        if (foundChanges)
        {
            diff = oldVersion.GetChanges();
        }
        return diff;
    }

    protected void cmdSave_Click(object sender, EventArgs e)
    {
        int iPurchaseId = 0;
        string connection = string.Empty;

        try
        {
            if (Page.IsValid)
            {

                string checkdate = txtInvoiveDate.Text.Trim();
                //if (checkdate == "01/12/2013")
                //{
                //    ScriptManager.RegisterStartupScript(Page, Page.GetType(), Guid.NewGuid().ToString(), "alert('Cannot make Purchase bill for this Date')", true);
                //    return;
                //}

                //DateTime checkdate2 = Convert.ToDateTime(txtInvoiveDate.Text.Trim());
                //if (checkdate2 >= Convert.ToDateTime("01/04/2014"))
                //{
                //    ScriptManager.RegisterStartupScript(Page, Page.GetType(), Guid.NewGuid().ToString(), "alert('Cannot make Purchase bill for this Date')", true);
                //    return;
                //}

                DateTime checkdate3 = Convert.ToDateTime(txtBillDate.Text.Trim());
                DateTime checkdate2 = Convert.ToDateTime(txtInvoiveDate.Text.Trim());

                if (checkdate2 < checkdate3)
                {
                    ScriptManager.RegisterStartupScript(Page, Page.GetType(), Guid.NewGuid().ToString(), "alert('Purchase Entry date can't be less than bill date')", true);
                    return;
                }


                if (Session["PurchaseProductDs"] == null)
                {
                    ScriptManager.RegisterStartupScript(Page, Page.GetType(), Guid.NewGuid().ToString(), "alert('Please select the products before save')", true);
                    return;
                }

                if (Convert.ToDouble(txtfixedtotal.Text) == 0)
                {
                    ScriptManager.RegisterStartupScript(Page, Page.GetType(), Guid.NewGuid().ToString(), "alert('Please Enter FixedTotal')", true);
                    return;
                }

                if (chk.Checked == false)
                {
                    if (txtSupplier.Text == "")
                    {
                        ScriptManager.RegisterStartupScript(Page, Page.GetType(), Guid.NewGuid().ToString(), "alert('Please Enter Supplier Name')", true);
                        return;
                    }
                }
                else
                {
                    if (cmbSupplier.SelectedValue == "0")
                    {
                        ScriptManager.RegisterStartupScript(Page, Page.GetType(), Guid.NewGuid().ToString(), "alert('Please Select Supplier')", true);
                        return;
                    }
                }

                connection = Request.Cookies["Company"].Value;
                BusinessLogic bll = new BusinessLogic();
                string recondate = txtBillDate.Text.Trim();

                string salesReturn = string.Empty;
                string intTrans = string.Empty;
                string deliveryNote = string.Empty;
                string srReason = string.Empty;

                string recondat = txtInvoiveDate.Text.Trim();
                if (!bll.IsValidDate(connection, Convert.ToDateTime(recondat)))
                {

                    ScriptManager.RegisterStartupScript(Page, Page.GetType(), Guid.NewGuid().ToString(), "alert('Date is invalid')", true);
                    return;
                }

                BusinessLogic blll = new BusinessLogic(sDataSource);
                //if (optionmethod.SelectedValue == "DeliveryNote")
                //{
                //    if (Convert.ToDouble(txtdcbillno.Text) == 0)
                //    {
                //        ScriptManager.RegisterStartupScript(Page, Page.GetType(), Guid.NewGuid().ToString(), "alert('Please Enter DC Bill No')", true);
                //        return;
                //    }

                //    DataSet dsd = new DataSet();
                //    int Dcno = Convert.ToInt32(txtdcbillno.Text);
                //    dsd = blll.GetDCNumSalesForId(Dcno);
                //    if (dsd != null && dsd.Tables[0].Rows.Count > 0)
                //    {

                //   }
                //   else
                //  {
                //       ScriptManager.RegisterStartupScript(Page, Page.GetType(), Guid.NewGuid().ToString(), "alert('Entered DC No Not Found in DC Sales')", true);
                //       return;
                //   }

                //  DataSet dsddd = new DataSet();
                //  dsddd = blll.GetDCSalesForId(Dcno);
                //  if (dsddd != null && dsddd.Tables[0].Rows.Count > 0)
                //   {

                //  }
                //  else
                //   {
                //       ScriptManager.RegisterStartupScript(Page, Page.GetType(), Guid.NewGuid().ToString(), "alert('Entered DC No is Not a DC Sales')", true);
                //       return;
                //  }

                //DataSet datd = new DataSet();
                //datd = blll.isDuplicateDCNum(Dcno);
                //if (datd != null && datd.Tables[0].Rows.Count > 0)
                //{
                //    ScriptManager.RegisterStartupScript(Page, Page.GetType(), Guid.NewGuid().ToString(), "alert('Entered DC No Already Billed')", true);
                //    return;
                //}
                //else
                //{

                //}
                // }

                string sBillno = string.Empty;
                string sInvoiceno = string.Empty;
                string narration2 = string.Empty;

                double dcbillno = 0;
                int iSupplier = 0;
                int iPaymode = 0;
                string[] sDate;
                string[] IDate;

                string sChequeno = string.Empty;
                int iBank = 0;
                int iPurchase = 0;
                string filename = string.Empty;
                double dTotalAmt = 0;
                iPurchase = Convert.ToInt32(hdPurchase.Value);
                sBillno = txtBillno.Text.Trim();
                DateTime sBilldate;
                DateTime sInvoicedate;

                double ddiscamt = 0;
                double ddiscper = 0;
                double dfixedtotal = 0;

                string delim = "/";
                char[] delimA = delim.ToCharArray();
                CultureInfo culture = new CultureInfo("pt-BR");

                salesReturn = drpSalesReturn.SelectedValue;
                intTrans = drpIntTrans.SelectedValue;
                deliveryNote = ddDeliveryNote.SelectedValue;
                srReason = txtSRReason.Text.Trim();
                iPaymode = Convert.ToInt32(cmdPaymode.SelectedItem.Value);

                if (txtdiscamt.Text == "")
                    ddiscamt = 0;
                else
                    ddiscamt = Convert.ToDouble(txtdiscamt.Text);

                if (txtdisc.Text == "")
                    ddiscper = 0;
                else
                    ddiscper = Convert.ToDouble(txtdisc.Text);

                sInvoiceno = txtInvoiveNo.Text.Trim();

                if (optionmethod.SelectedValue == "DeliveryNote")
                {
                    dcbillno = Convert.ToDouble(txtdcbillno.Text);
                }
                else
                {
                    dcbillno = 0;
                }

                int cnt = 0;

                if (intTrans == "YES")
                    cnt = cnt + 1;
                if (deliveryNote == "YES")
                    cnt = cnt + 1;
                if (salesReturn == "YES")
                    cnt = cnt + 1;

                if (cnt > 1)
                {
                    ScriptManager.RegisterStartupScript(Page, Page.GetType(), Guid.NewGuid().ToString(), "alert('Either one of Sales Return or Delivery Note or Internal Transfer should be selected')", true);
                    tabs2.ActiveTabIndex = 1;
                    //updatePnlSales.Update();
                    return;
                }

                string Paymode = cmbBankName.SelectedItem.Text;

                if (iPaymode == 2)
                {
                    sChequeno = Convert.ToString(txtChequeNo.Text);
                    iBank = Convert.ToInt32(cmbBankName.SelectedItem.Value);
                    rvCheque.Enabled = true;
                    rvbank.Enabled = true;
                }
                else
                {
                    rvbank.Enabled = false;
                    rvCheque.Enabled = false;
                }

                Page.Validate("purchaseval");

                if (!Page.IsValid)
                {
                    StringBuilder msg = new StringBuilder();

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

                try
                {
                    sDate = txtBillDate.Text.Trim().Split(delimA);
                    sBilldate = new DateTime(Convert.ToInt32(sDate[2].ToString()), Convert.ToInt32(sDate[1].ToString()), Convert.ToInt32(sDate[0].ToString()));

                    IDate = txtInvoiveDate.Text.Trim().Split(delimA);
                    sInvoicedate = new DateTime(Convert.ToInt32(IDate[2].ToString()), Convert.ToInt32(IDate[1].ToString()), Convert.ToInt32(IDate[0].ToString()));
                }
                catch (Exception ex)
                {
                    Response.Write("<b><font face='Trebuchet MS' size=2 color=red>Invalid Bill Date Format</font></b>");
                    return;
                }
                iSupplier = Convert.ToInt32(cmbSupplier.SelectedItem.Value);

                if (lblTotalSum.Text != string.Empty || lblTotalSum.Text != "0")
                    dTotalAmt = Convert.ToDouble(lblTotalSum.Text);
                /*Start Purchase Loading / Unloading Freight Change - March 16*/
                double dFreight = 0;
                double dLU = 0;
                /*March18*/
                if (txtFreight.Text.Trim() != "")
                    dFreight = Convert.ToDouble(txtFreight.Text.Trim());
                if (txtLU.Text.Trim() != "")
                    dLU = Convert.ToDouble(txtLU.Text.Trim());
                /*March18*/
                dTotalAmt = dTotalAmt + dFreight + dLU;

                dfixedtotal = Convert.ToDouble(txtfixedtotal.Text);
                narration2 = txtnarr.Text;

                int BilitID = int.Parse(ddBilts.SelectedValue);
                /*End Purchase Loading / Unloading Freight Change - March 16*/

                string usernam = Request.Cookies["LoggedUserName"].Value;


                BusinessLogic blg = new BusinessLogic(sDataSource);
                double checktotal = 0;
                double difftotal = 0;
                double dddtotal = 0;

                dddtotal = Convert.ToDouble(lblNet.Text.Trim());

                difftotal = dfixedtotal - dddtotal;
                checktotal = (difftotal / dfixedtotal) * 100;

                double Percent = blg.getconfigpercent();
                double Amt = blg.getconfigamt();

                double ddd = Math.Min(difftotal, checktotal);

                double TotalAmt = (dfixedtotal - dddtotal);
                string Total = TotalAmt.ToString("#0.00");

                if (Convert.ToDouble(Total) > 1)
                {
                    ScriptManager.RegisterStartupScript(Page, Page.GetType(), Guid.NewGuid().ToString(), "alert('Difference Between FixedTotal And GrandTotal Cannot be more than 1')", true);
                    return;
                }
                else if (Convert.ToDouble(Total) < -1)
                {
                    ScriptManager.RegisterStartupScript(Page, Page.GetType(), Guid.NewGuid().ToString(), "alert('Difference Between FixedTotal And GrandTotal Cannot be less than 1')", true);
                    return;
                }

                //if (difftotal < 0)
                //{
                //    ScriptManager.RegisterStartupScript(Page, Page.GetType(), Guid.NewGuid().ToString(), "alert('FixedTotal Cannot be less then GrandTotal')", true);
                //    return;
                //}

                //if (ddd == difftotal)
                //{
                //    if (difftotal > Amt)
                //    {
                //        ScriptManager.RegisterStartupScript(Page, Page.GetType(), Guid.NewGuid().ToString(), "alert('Difference Between FixedTotal And GrandTotal Cannot be more than " + Amt + "')", true);
                //        return;
                //    }
                //}
                //else if (ddd == checktotal)
                //{ 
                //    if (checktotal > Percent)
                //    {
                //        ScriptManager.RegisterStartupScript(Page, Page.GetType(), Guid.NewGuid().ToString(), "alert('Difference Between FixedTotal And GrandTotal Cannot be more than " + Percent + " % ')", true);
                //        return;
                //    }
                //}

                filename = hdFilename.Value;
                if (Session["PurchaseProductDs"] != null)
                {
                    DataSet ds = (DataSet)Session["PurchaseProductDs"];

                    string itemcd = string.Empty;
                    string itemcdold = string.Empty;

                    if (optionmethod.SelectedValue == "DeliveryNote")
                    {
                        DataSet datas;
                        DataTable dt;
                        DataRow drNew;
                        DataColumn dc;
                        datas = new DataSet();
                        dt = new DataTable();
                        dc = new DataColumn("ItemCode");
                        dt.Columns.Add(dc);
                        datas.Tables.Add(dt);

                        if (ds != null)
                        {
                            DataSet dsd = new DataSet();
                            int Dcno = Convert.ToInt32(txtdcbillno.Text);
                            //dsd = blll.GetDCSalesItemsForId(Dcno);

                            //if (ds.Tables[0].Rows.Count != dsd.Tables[0].Rows.Count)
                            //{
                            //    ScriptManager.RegisterStartupScript(Page, Page.GetType(), Guid.NewGuid().ToString(), "alert('Number of Products Not Match With DC Sales Bill " + Dcno + " ')", true);
                            //    return;
                            //}

                            //if (ds.Tables[0].Rows.Count > 0)
                            //{
                            //    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                            //    {
                            //        if (dsd.Tables[0].Rows.Count > 0)
                            //        {
                            //            for (int ij = 0; ij < dsd.Tables[0].Rows.Count; ij++)
                            //            {
                            //                if (dsd.Tables[0].Rows[ij]["ItemCode"].ToString() == ds.Tables[0].Rows[i]["ItemCode"].ToString())
                            //                {
                            //                    if (dsd.Tables[0].Rows[ij]["Qty"].ToString() == ds.Tables[0].Rows[i]["Qty"].ToString())
                            //                    {

                            //                    }
                            //                    else
                            //                    {
                            //                        //ScriptManager.RegisterStartupScript(Page, Page.GetType(), Guid.NewGuid().ToString(), "alert('Qty Not Matches For Item Code " + ds.Tables[0].Rows[i]["ItemCode"].ToString() + " With DC Sales Bill " + Dcno + " ')", true);
                            //                        //return;
                            //                    }
                            //                }
                            //                else
                            //                {
                            //                    itemcdold = ds.Tables[0].Rows[ij]["ItemCode"].ToString();

                            //                    drNew = dt.NewRow();
                            //                    drNew["ItemCode"] = itemcdold;
                            //                    datas.Tables[0].Rows.Add(drNew);

                            //                }

                            //            }
                            //        }

                            //    }

                            //    DataTable datad = new DataTable();

                            //    datad = RemoveDuplicateRows(dt,"ItemCode");

                            //    foreach (DataRow dr in dsd.Tables[0].Rows)
                            //    {
                            //        string check = Convert.ToString(dr["Itemcode"]);

                            //        for (int i = 0; i < datad.Rows.Count; i++)
                            //        {
                            //            if (check == datad.Rows[i]["ItemCode"].ToString())
                            //            {
                            //                datad.Rows[i].Delete();
                            //                datad.AcceptChanges();
                            //            }                        

                            //        }
                            //    }

                            //    if (datad.Rows.Count > 0)
                            //    {
                            //        for (int ijj = 0; ijj < datad.Rows.Count; ijj++)
                            //        {
                            //               itemcd = itemcd + "  " + datad.Rows[ijj]["ItemCode"].ToString();
                            //        }
                            //        //ScriptManager.RegisterStartupScript(Page, Page.GetType(), Guid.NewGuid().ToString(), "alert('Item Code " + itemcd + " Not Found in DC Sales Bill " + Dcno + " ')", true);
                            //        //return;
                            //    }

                            //}
                        }
                    }



                    if (ds != null)
                    {
                        /*March 18*/
                        if (ds.Tables[0].Rows.Count > 0)
                        {
                            /*March 18*/
                            //string sDataSource = Server.MapPath(ConfigurationSettings.AppSettings["DataSource"].ToString());
                            BusinessLogic bl = new BusinessLogic(sDataSource);
                            int cntB = bl.isDuplicateBill(sBillno, iSupplier);
                            if (cntB > 0)
                            {
                                ScriptManager.RegisterStartupScript(Page, Page.GetType(), Guid.NewGuid().ToString(), "alert('Duplicate Bill Number')", true);
                                return;
                            }

                            string sSupplierName = txtSupplier.Text;
                            string sSupplierAddress = txtAddress1.Text;
                            string sSupplierAddress2 = txtAddress2.Text;
                            string sSupplierAddress3 = txtAddress3.Text;
                            string sCustomerContact = txtMobile.Text;

                            if (chk.Checked == false)
                            {
                                if (bl.IsLedgerAlreadyFound(connection, sSupplierName))
                                {
                                    ScriptManager.RegisterStartupScript(Page, Page.GetType(), Guid.NewGuid().ToString(), "alert('Ledger " + sSupplierName + " with this name already exists.');", true);
                                    return;
                                }

                                iSupplier = bl.InsertCustomerInfoDirect(connection, sSupplierName, sSupplierName, 2, 0, 0, 0, "", sSupplierName, sSupplierAddress, sSupplierAddress2, sSupplierAddress3, "", "", 0, "", sCustomerContact, 0, 0, "NO", "NO", "NO", sSupplierName, usernam, "YES", "", 3);                                
                            }
                            else
                            {
                                sSupplierName = cmbSupplier.SelectedItem.Text;
                                iSupplier = Convert.ToInt32(cmbSupplier.SelectedItem.Value);
                            }

                            /*Start Purchase Loading / Unloading Freight Change - March 16*/
                            /*Start InvoiceNo and InvoiceDate - Jan 26*/
                            iPurchaseId = bl.InsertPurchase(sBillno, sBilldate, iSupplier, iPaymode, sChequeno, iBank, dfixedtotal, salesReturn, srReason, dFreight, dLU, BilitID, intTrans, ds, deliveryNote, sInvoiceno, sInvoicedate, ddiscamt, ddiscper, dcbillno, dTotalAmt, usernam, narration2);
                            /*End InvoiceNo and InvoiceDate - Jan 26*/
                            /*End Purchase Loading / Unloading Freight Change - March 16*/



                            string salestype = string.Empty;
                            int ScreenNo = 0;
                            string ScreenName = string.Empty;

                            if (salesReturn == "YES")
                            {
                                salestype = "Sales Return";
                                ScreenName = "Sales Return";
                            }
                            else if (intTrans == "YES")
                            {
                                salestype = "Internal Transfer";
                                ScreenName = "Purchase - Internal";
                            }
                            else if (deliveryNote == "YES")
                            {
                                salestype = "Delivery Note";
                                ScreenName = "Purchase - DC";
                            }
                            else
                            {
                                salestype = "Normal Purchase";
                                ScreenName = "Purchase - Normal";
                            }

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
                                DataSet dsd = bl.GetLedgerInfoForId(connection, iSupplier);
                                var toAddress = "";
                                var toAdd = "";
                                Int32 ModeofContact = 0;
                                string Active = string.Empty;

                                if (dsd != null)
                                {
                                    if (dsd.Tables[0].Rows.Count > 0)
                                    {
                                        foreach (DataRow dr in dsd.Tables[0].Rows)
                                        {
                                            toAdd = dr["EmailId"].ToString();
                                            ModeofContact = Convert.ToInt32(dr["ModeofContact"]);
                                        }
                                    }
                                }

                                DataSet dsdd = bl.GetDetailsForScreenNo(connection, ScreenNo, "Email");
                                if (dsdd != null)
                                {
                                    if (dsdd.Tables[0].Rows.Count > 0)
                                    {
                                        foreach (DataRow dr in dsdd.Tables[0].Rows)
                                        {
                                            Active = dr["Active"].ToString();
                                            if (Active == "YES")
                                            {
                                                if (dr["EmailId"].ToString() == "Customer")
                                                {
                                                    toAddress = toAdd;
                                                }
                                                else
                                                {
                                                    toAddress = dr["EmailId"].ToString();
                                                }

                                                if (ModeofContact == 2)
                                                {
                                                    string subject = "Added - " + Paymode + " Purchase in Branch " + Request.Cookies["Company"].Value;

                                                    string body = "\n";
                                                    body += " Branch           : " + Request.Cookies["Company"].Value + "\n";
                                                    body += " Purchase Type       : " + salestype + "\n";
                                                    //body += " Customer Name    : " + sCustomerName + "\n";
                                                    //body += " Customer Address : " + sCustomerAddress + "\n";
                                                    body += " Bill Date        : " + sBilldate + "\n";
                                                    body += " Payment Mode     : " + Paymode + "\n";
                                                    body += " User Name        : " + usernam + "\n";
                                                    body += " Total Amount     : " + dfixedtotal + "\n";

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
                            }

                            Reset();
                            ResetProduct();

                            hdMode.Value = "Edit";
                            Session["purchaseID"] = iPurchaseId.ToString();
                            deleteFile();
                            ScriptManager.RegisterStartupScript(Page, Page.GetType(), Guid.NewGuid().ToString(), "alert('Purchase Details Saved Successfully. Updated Bill No. is " + iPurchaseId.ToString() + "')", true);
                            Session["SalesReturn"] = salesReturn.ToUpper();

                            Session["PurchaseProductDs"] = null;
                            Response.Redirect("ProductPurchaseBill.aspx?SID=" + iPurchaseId.ToString() + "&RT=" + salesReturn);
                            /*March 18*/
                        }
                        /*March 18*/
                        else
                        {
                            ScriptManager.RegisterStartupScript(Page, Page.GetType(), Guid.NewGuid().ToString(), "alert('No products is choosed for the bill')", true);
                        }
                        /*March 18*/
                    }
                    delFlag.Value = "0";

                    //Accordion1.SelectedIndex = 0;
                    btnCancel.Enabled = false;
                }
            }
        }
        catch (Exception ex)
        {
            TroyLiteExceptionManager.HandleException(ex);
        }
    }

    public DataTable RemoveDuplicateRows(DataTable table, string ItemCode)
    {
        try
        {
            ArrayList UniqueRecords = new ArrayList();
            ArrayList DuplicateRecords = new ArrayList();

            // Check if records is already added to UniqueRecords otherwise,
            // Add the records to DuplicateRecords
            foreach (DataRow dRow in table.Rows)
            {
                if (UniqueRecords.Contains(dRow[ItemCode]))
                    DuplicateRecords.Add(dRow);
                else
                    UniqueRecords.Add(dRow[ItemCode]);
            }

            // Remove dupliate rows from DataTable added to DuplicateRecords
            foreach (DataRow dRow in DuplicateRecords)
            {
                table.Rows.Remove(dRow);
            }

            // Return the clean DataTable which contains unique records.
            return table;
        }
        catch (Exception ex)
        {
            return null;
        }
    }

    //cmdSave_Click
    protected void cmdUpdate_Click(object sender, EventArgs e)
    {
        try
        {
            if (Page.IsValid)
            {

                string checkdate = txtInvoiveDate.Text.Trim();
                //if (checkdate == "01/12/2013")
                //{
                //    ScriptManager.RegisterStartupScript(Page, Page.GetType(), Guid.NewGuid().ToString(), "alert('Cannot make Purchase bill for this Date')", true);
                //    return;
                //}

                //DateTime checkdate2 = Convert.ToDateTime(txtInvoiveDate.Text.Trim());
                //if (checkdate2 >= Convert.ToDateTime("01/04/2014"))
                //{
                //    ScriptManager.RegisterStartupScript(Page, Page.GetType(), Guid.NewGuid().ToString(), "alert('Cannot make Purchase bill for this Date')", true);
                //    return;
                //}

                DateTime checkdate3 = Convert.ToDateTime(txtBillDate.Text.Trim());
                DateTime checkdate2 = Convert.ToDateTime(txtInvoiveDate.Text.Trim());

                if (checkdate2 < checkdate3)
                {
                    ScriptManager.RegisterStartupScript(Page, Page.GetType(), Guid.NewGuid().ToString(), "alert('Purchase Entry date can't be less than bill date')", true);
                    return;
                }


                int iPurchaseId = 0;
                string connection = Request.Cookies["Company"].Value;
                BusinessLogic bll = new BusinessLogic();
                string recondate = txtBillDate.Text.Trim();
                string salesReturn = string.Empty;
                string intTrans = string.Empty;
                string deliveryNote = string.Empty;
                string srReason = string.Empty;
                salesReturn = drpSalesReturn.SelectedItem.Text;
                intTrans = drpIntTrans.SelectedValue;
                deliveryNote = ddDeliveryNote.SelectedValue;
                srReason = txtSRReason.Text.Trim();
                string narration2 = string.Empty;

                if (Session["PurchaseProductDs"] == null)
                {
                    ScriptManager.RegisterStartupScript(Page, Page.GetType(), Guid.NewGuid().ToString(), "alert('Please select the products before save')", true);
                    return;
                }
                else
                {
                    DataSet dsddd = (DataSet)Session["PurchaseProductDs"];

                    if (dsddd != null)
                    {
                        if (dsddd.Tables[0].Rows.Count > 0)
                        {
                        }
                        else
                        {
                            ScriptManager.RegisterStartupScript(Page, Page.GetType(), Guid.NewGuid().ToString(), "alert('Please select the products for the bill before update')", true);
                            return;
                        }
                    }
                    else
                    {
                        ScriptManager.RegisterStartupScript(Page, Page.GetType(), Guid.NewGuid().ToString(), "alert('Please select the products for the bill before update')", true);
                        return;
                    }
                }

                string recondat = txtInvoiveDate.Text.Trim();
                if (!bll.IsValidDate(connection, Convert.ToDateTime(recondat)))
                {

                    ScriptManager.RegisterStartupScript(Page, Page.GetType(), Guid.NewGuid().ToString(), "alert('Date is invalid')", true);
                    return;
                }

                if (Convert.ToDouble(txtfixedtotal.Text) == 0)
                {
                    ScriptManager.RegisterStartupScript(Page, Page.GetType(), Guid.NewGuid().ToString(), "alert('Please Enter FixedTotal')", true);
                    return;
                }

                if (chk.Checked == false)
                {
                    if (txtSupplier.Text == "")
                    {
                        ScriptManager.RegisterStartupScript(Page, Page.GetType(), Guid.NewGuid().ToString(), "alert('Please Enter Supplier Name')", true);
                        return;
                    }
                }
                else
                {
                    if (cmbSupplier.SelectedValue == "0")
                    {
                        ScriptManager.RegisterStartupScript(Page, Page.GetType(), Guid.NewGuid().ToString(), "alert('Please Select Supplier')", true);
                        return;
                    }
                }

                int cnt = 0;

                if (intTrans == "YES")
                    cnt = cnt + 1;
                if (deliveryNote == "YES")
                    cnt = cnt + 1;
                if (salesReturn == "YES")
                    cnt = cnt + 1;

                if (cnt > 1)
                {
                    ScriptManager.RegisterStartupScript(Page, Page.GetType(), Guid.NewGuid().ToString(), "alert('Either one of Sales Return or Delivery Note or Internal Transfer should be selected')", true);
                    tabs2.ActiveTabIndex = 1;
                    //updatePnlSales.Update();
                    return;
                }

                string sBillno = string.Empty;
                string sInvoiceno = string.Empty;

                double ddiscamt;
                double ddiscper;
                double dcbillno;

                double dfixedtotal;

                int iSupplier = 0;
                int iPaymode = 0;
                string sChequeno = string.Empty;
                int iBank = 0;
                int iPurchase = 0;
                string filename = string.Empty;
                double dTotalAmt = 0;

                iPurchase = Convert.ToInt32(hdPurchase.Value);
                sBillno = txtBillno.Text.Trim();
                sInvoiceno = txtInvoiveNo.Text.Trim();

                if (optionmethod.SelectedValue == "DeliveryNote")
                {
                    dcbillno = Convert.ToDouble(txtdcbillno.Text);
                }
                else
                {
                    dcbillno = 0;
                }

                if (txtdiscamt.Text == "")
                    ddiscamt = 0;
                else
                    ddiscamt = Convert.ToDouble(txtdiscamt.Text);

                if (txtdisc.Text == "")
                    ddiscper = 0;
                else
                    ddiscper = Convert.ToDouble(txtdisc.Text);

                DateTime sBilldate;
                DateTime sInvoicedate;

                string[] sDate;
                string[] IDate;

                string delim = "/";
                char[] delimA = delim.ToCharArray();
                CultureInfo culture = new CultureInfo("pt-BR");
                iPaymode = Convert.ToInt32(cmdPaymode.SelectedItem.Value);

                string Paymode = cmbBankName.SelectedItem.Text;

                if (iPaymode == 2)
                {
                    sChequeno = Convert.ToString(txtChequeNo.Text);
                    iBank = Convert.ToInt32(cmbBankName.SelectedItem.Value);
                    rvbank.Enabled = true;
                    rvCheque.Enabled = true;
                }
                else
                {
                    rvbank.Enabled = false;
                    rvCheque.Enabled = false;
                }

                Page.Validate("purchaseval");

                if (!Page.IsValid)
                {
                    StringBuilder msg = new StringBuilder();

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

                try
                {
                    sDate = txtBillDate.Text.Trim().Split(delimA);
                    sBilldate = new DateTime(Convert.ToInt32(sDate[2].ToString()), Convert.ToInt32(sDate[1].ToString()), Convert.ToInt32(sDate[0].ToString()));

                    IDate = txtInvoiveDate.Text.Trim().Split(delimA);
                    sInvoicedate = new DateTime(Convert.ToInt32(IDate[2].ToString()), Convert.ToInt32(IDate[1].ToString()), Convert.ToInt32(IDate[0].ToString()));
                }
                catch (Exception ex)
                {
                    Response.Write("<b><font face='Trebuchet MS' size=2 color=red>Invalid Bill Date Format</font></b>");
                    return;
                }
                iSupplier = Convert.ToInt32(cmbSupplier.SelectedItem.Value);

                dTotalAmt = Convert.ToDouble(lblTotalSum.Text);

                /*Start Purchase Loading / Unloading Freight Change - March 16*/
                double dFreight = 0;
                double dLU = 0;
                /*March18*/
                if (txtFreight.Text.Trim() != "")
                    dFreight = Convert.ToDouble(txtFreight.Text.Trim());
                if (txtLU.Text.Trim() != "")
                    dLU = Convert.ToDouble(txtLU.Text.Trim());
                /*March18*/
                dTotalAmt = dTotalAmt + dFreight + dLU;
                /*End Purchase Loading / Unloading Freight Change - March 16*/
                int BilitID = int.Parse(ddBilts.SelectedValue);

                string usernam = Request.Cookies["LoggedUserName"].Value;

                dfixedtotal = Convert.ToDouble(txtfixedtotal.Text);
                narration2 = txtnarr.Text;

                BusinessLogic blg = new BusinessLogic(sDataSource);

                double checktotal = 0;
                double difftotal = 0;

                double dddtotal = 0;

                dddtotal = Convert.ToDouble(lblNet.Text.Trim());

                difftotal = dfixedtotal - dddtotal;
                checktotal = (difftotal / dfixedtotal) * 100;

                double Percent = blg.getconfigpercent();
                double Amt = blg.getconfigamt();

                double ddd = Math.Min(difftotal, checktotal);

                double TotalAmt = (dfixedtotal - dddtotal);
                string Total = TotalAmt.ToString("#0.00");

                if (Convert.ToDouble(Total) > 1)
                {
                    ScriptManager.RegisterStartupScript(Page, Page.GetType(), Guid.NewGuid().ToString(), "alert('Difference Between FixedTotal And GrandTotal Cannot be more than 1')", true);
                    return;
                }
                else if (Convert.ToDouble(Total) < -1)
                {
                    ScriptManager.RegisterStartupScript(Page, Page.GetType(), Guid.NewGuid().ToString(), "alert('Difference Between FixedTotal And GrandTotal Cannot be less than 1')", true);
                    return;
                }

                //if (difftotal < 0)
                //{
                //    ScriptManager.RegisterStartupScript(Page, Page.GetType(), Guid.NewGuid().ToString(), "alert('FixedTotal Cannot be less than GrandTotal')", true);
                //    return;
                //}

                //if (ddd == difftotal)
                //{
                //    if (difftotal > Amt)
                //    {
                //        ScriptManager.RegisterStartupScript(Page, Page.GetType(), Guid.NewGuid().ToString(), "alert('Difference Between FixedTotal And GrandTotal Cannot be more than " + Amt + "')", true);
                //        return;
                //    }
                //}
                //else if (ddd == checktotal)
                //{
                //    if (checktotal > Percent)
                //    {
                //        ScriptManager.RegisterStartupScript(Page, Page.GetType(), Guid.NewGuid().ToString(), "alert('Difference Between FixedTotal And GrandTotal Cannot be more than" + Percent + "% ')", true);
                //        return;
                //    }
                //}


                filename = hdFilename.Value;
                //BindProduct();
                if (Session["PurchaseProductDs"] != null)
                {
                    DataSet ds = (DataSet)Session["PurchaseProductDs"];



                    string itemcd = string.Empty;
                    string itemcdold = string.Empty;

                    if (optionmethod.SelectedValue == "DeliveryNote")
                    {
                        //  DataSet datas;
                        //  DataTable dt;
                        //  DataRow drNew;
                        //  DataColumn dc;
                        //  datas = new DataSet();
                        //  dt = new DataTable();
                        // dc = new DataColumn("ItemCode");
                        //  dt.Columns.Add(dc);
                        //  datas.Tables.Add(dt);

                        //if (ds != null)
                        //{
                        //    DataSet dsd = new DataSet();
                        //    int Dcno = Convert.ToInt32(txtdcbillno.Text);
                        //    dsd = blg.GetDCSalesItemsForId(Dcno);

                        //    if (ds.Tables[0].Rows.Count != dsd.Tables[0].Rows.Count)
                        //   {
                        //ScriptManager.RegisterStartupScript(Page, Page.GetType(), Guid.NewGuid().ToString(), "alert('Number of Products Not Match With DC Sales Bill " + Dcno + " ')", true);
                        //return;
                        // }

                        //   if (ds.Tables[0].Rows.Count > 0)
                        //  {
                        //    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                        //     {
                        //        if (dsd.Tables[0].Rows.Count > 0)
                        //        {
                        //           for (int ij = 0; ij < dsd.Tables[0].Rows.Count; ij++)
                        //          {
                        //             if (dsd.Tables[0].Rows[ij]["ItemCode"].ToString() == ds.Tables[0].Rows[i]["ItemCode"].ToString())
                        //              {
                        //                 if (dsd.Tables[0].Rows[ij]["Qty"].ToString() == ds.Tables[0].Rows[i]["Qty"].ToString())
                        // {
                        //  }
                        //   else
                        //       {
                        //ScriptManager.RegisterStartupScript(Page, Page.GetType(), Guid.NewGuid().ToString(), "alert('Qty Not Matches For Item Code " + ds.Tables[0].Rows[i]["ItemCode"].ToString() + " With DC Sales Bill " + Dcno + " ')", true);
                        //return;
                        //      }
                        //   }
                        //   else
                        //   {
                        //       itemcdold = ds.Tables[0].Rows[ij]["ItemCode"].ToString();

                        //       drNew = dt.NewRow();
                        //       drNew["ItemCode"] = itemcdold;
                        //     datas.Tables[0].Rows.Add(drNew);

                        //   }

                        //  }
                        //  }

                        //  }

                        //  DataTable datad = new DataTable();

                        //  datad = RemoveDuplicateRows(dt, "ItemCode");

                        //  foreach (DataRow dr in dsd.Tables[0].Rows)
                        //  {
                        //      string check = Convert.ToString(dr["Itemcode"]);

                        //      for (int i = 0; i < datad.Rows.Count; i++)
                        //      {
                        //          if (check == datad.Rows[i]["ItemCode"].ToString())
                        //          {
                        //              datad.Rows[i].Delete();
                        //              datad.AcceptChanges();
                        //          }

                        //       }
                        //   }

                        //    if (datad.Rows.Count > 0)
                        //    {
                        //       for (int ijj = 0; ijj < datad.Rows.Count; ijj++)
                        //      {
                        //          itemcd = itemcd + "  " + datad.Rows[ijj]["ItemCode"].ToString();
                        //      }
                        //ScriptManager.RegisterStartupScript(Page, Page.GetType(), Guid.NewGuid().ToString(), "alert('Item Code " + itemcd + " Not Found in DC Sales Bill " + Dcno + " ')", true);
                        //return;
                        //   }

                        // }
                        // }
                    }




                    if (ds != null)
                    {
                        /*March 18*/
                        if (ds.Tables[0].Rows.Count > 0)
                        {
                            /*March 18*/
                            //string sDataSource = Server.MapPath(ConfigurationSettings.AppSettings["DataSource"].ToString());

                            BusinessLogic bl = new BusinessLogic(sDataSource);
                            /*Start Purchase Loading / Unloading Freight Change - March 16*/
                            /*Start InvoiceNo, InvoiceDate*/


                            string sSupplierName = txtSupplier.Text;
                            string sSupplierAddress = txtAddress1.Text;
                            string sSupplierAddress2 = txtAddress2.Text;
                            string sSupplierAddress3 = txtAddress3.Text;
                            string sCustomerContact = txtMobile.Text;

                            if (chk.Checked == false)
                            {
                                if (bl.IsLedgerAlreadyFound(connection, sSupplierName))
                                {
                                    ScriptManager.RegisterStartupScript(Page, Page.GetType(), Guid.NewGuid().ToString(), "alert('Ledger " + sSupplierName + " with this name already exists.');", true);
                                    return;
                                }
                                iSupplier = bl.InsertCustomerInfoDirect(connection, sSupplierName, sSupplierName, 2, 0, 0, 0, "", sSupplierName, sSupplierAddress, sSupplierAddress2, sSupplierAddress3, "", "", 0, "", sCustomerContact, 0, 0, "NO", "NO", "NO", sSupplierName, usernam, "YES", "", 3);
                            }
                            else
                            {
                                sSupplierName = cmbSupplier.SelectedItem.Text;
                                iSupplier = Convert.ToInt32(cmbSupplier.SelectedItem.Value);
                            }


                            iPurchaseId = bl.UpdatePurchase(iPurchase, sBillno, sBilldate, iSupplier, iPaymode, sChequeno, iBank, dfixedtotal, salesReturn, srReason, dFreight, dLU, BilitID, intTrans, ds, deliveryNote, sInvoiceno, sInvoicedate, ddiscamt, ddiscper, dcbillno, dTotalAmt, usernam, narration2);
                            /*End Purchase Loading / Unloading Freight Change - March 16*/
                            /*Start March 15 Modification */
                            if (iPurchaseId == -2)
                            {
                                ScriptManager.RegisterStartupScript(Page, Page.GetType(), Guid.NewGuid().ToString(), "alert('Purchase edit is not allowed for this transaction.')", true);
                                /*Start Purchase Stock Negative Change - March 16 -- (Commented the below method) */
                                //Reset();
                                //ResetProduct();
                                /*End Purchase Stock Negative Change - March 16*/
                                return;

                            }
                            /*End March 15 Modification */

                            string salestype = string.Empty;
                            int ScreenNo = 0;
                            string ScreenName = string.Empty;

                            if (salesReturn == "YES")
                            {
                                salestype = "Sales Return";
                                ScreenName = "Sales Return";
                            }
                            else if (intTrans == "YES")
                            {
                                salestype = "Internal Transfer";
                                ScreenName = "Purchase - Internal";
                            }
                            else if (deliveryNote == "YES")
                            {
                                salestype = "Delivery Note";
                                ScreenName = "Purchase - DC";
                            }
                            else
                            {
                                salestype = "Normal Purchase";
                                ScreenName = "Purchase - Normal";
                            }

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
                                DataSet dsd = bl.GetLedgerInfoForId(connection, iSupplier);
                                var toAddress = "";
                                var toAdd = "";
                                Int32 ModeofContact = 0;
                                string Active = string.Empty;

                                if (dsd != null)
                                {
                                    if (dsd.Tables[0].Rows.Count > 0)
                                    {
                                        foreach (DataRow dr in dsd.Tables[0].Rows)
                                        {
                                            toAdd = dr["EmailId"].ToString();
                                            ModeofContact = Convert.ToInt32(dr["ModeofContact"]);
                                        }
                                    }
                                }

                                DataSet dsdd = bl.GetDetailsForScreenNo(connection, ScreenNo, "Email");
                                if (dsdd != null)
                                {
                                    if (dsdd.Tables[0].Rows.Count > 0)
                                    {
                                        foreach (DataRow dr in dsdd.Tables[0].Rows)
                                        {
                                            Active = dr["Active"].ToString();
                                            if (Active == "YES")
                                            {
                                                if (dr["EmailId"].ToString() == "Customer")
                                                {
                                                    toAddress = toAdd;
                                                }
                                                else
                                                {
                                                    toAddress = dr["EmailId"].ToString();
                                                }

                                                if (ModeofContact == 2)
                                                {
                                                    string subject = "Updated - " + Paymode + " Purchase in Branch " + Request.Cookies["Company"].Value;

                                                    string body = "\n";
                                                    body += " Branch           : " + Request.Cookies["Company"].Value + "\n";
                                                    body += " Purchase Type       : " + salestype + "\n";
                                                    //body += " Customer Name    : " + sCustomerName + "\n";
                                                    //body += " Customer Address : " + sCustomerAddress + "\n";
                                                    body += " Bill Date        : " + sBilldate + "\n";
                                                    body += " Payment Mode     : " + Paymode + "\n";
                                                    body += " User Name        : " + usernam + "\n";
                                                    body += " Total Amount     : " + dfixedtotal + "\n";

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
                            }

                            Reset();
                            ResetProduct();

                            //purchasePanel.Visible = false;
                            lnkBtnAdd.Visible = true;
                            pnlSearch.Visible = true;
                            //PanelBill.Visible = false;
                            PanelCmd.Visible = false;
                            hdMode.Value = "Edit";
                            cmdPrint.Enabled = false;
                            BindGrid("0", "0");
                            /*March 18*/
                        }
                        /*March 18*/
                        else
                        {
                            ScriptManager.RegisterStartupScript(Page, Page.GetType(), Guid.NewGuid().ToString(), "alert('No products is choosed for the bill')", true);
                        }
                        /*March 18*/
                    }
                    delFlag.Value = "0";
                    deleteFile();
                    //Accordion1.SelectedIndex = 0;
                    ScriptManager.RegisterStartupScript(Page, Page.GetType(), Guid.NewGuid().ToString(), "alert('Purchase Details Updated Successfully. Updated Bill No. is " + iPurchaseId.ToString() + "')", true);
                    Session["purchaseID"] = iPurchaseId.ToString();
                    deleteFile();
                    btnCancel.Enabled = false;
                    Session["SalesReturn"] = salesReturn;
                    Session["PurchaseProductDs"] = null;
                    Response.Redirect("ProductPurchaseBill.aspx?SID=" + iPurchaseId.ToString() + "&RT=" + salesReturn.ToUpper());

                }
            }
        }
        catch (Exception ex)
        {
            TroyLiteExceptionManager.HandleException(ex);
        }
    }

    protected void GrdViewItems_SelectedIndexChanged(object sender, EventArgs e)
    {
        
        try
        {
            string strItemCode = string.Empty;
            string strRoleFlag = string.Empty;
            DataSet ds = new DataSet();
            GridViewRow row = GrdViewItems.SelectedRow;

            BusinessLogic bl = new BusinessLogic(sDataSource);

            hdCurrentRow.Value = Convert.ToString(row.DataItemIndex);

            if (row.Cells[0].Text != "&nbsp;")
            {
                strItemCode = row.Cells[0].Text.Trim().Replace("&quot;", "\"");
                cmbProdAdd.ClearSelection();
                ListItem li = cmbProdAdd.Items.FindByValue(System.Web.HttpUtility.HtmlDecode(strItemCode.Trim()));
                if (li != null) li.Selected = true;
                cmbProdAdd.Enabled = false;
                cmdSaveProduct.Visible = false;
                //Label2.Visible = false;
                cmdSaveProduct.Enabled = false;

                cmdUpdateProduct.Enabled = true;
                cmdUpdateProduct.Visible = true;
                //Label3.Visible = true;
                DataSet catData = bl.GetProductForId(sDataSource, strItemCode);

                if (catData != null)
                {
                    cmbCategory.SelectedValue = catData.Tables[0].Rows[0]["CategoryID"].ToString();
                    cmbModel.Enabled = false;
                    cmbBrand.Enabled = false;
                    cmbProdName.Enabled = false;
                    BtnClearFilter.Enabled = false;
                    cmbCategory.Enabled = false;
                    LoadProducts(this, null);
                }

                if ((catData.Tables[0].Rows[0]["ItemCode"] != null) && (catData.Tables[0].Rows[0]["ItemCode"].ToString() != ""))
                {
                    if (cmbProdAdd.Items.FindByValue(catData.Tables[0].Rows[0]["ItemCode"].ToString().Trim()) != null)
                    {
                        cmbProdAdd.ClearSelection();
                        cmbProdAdd.Items.FindByValue(catData.Tables[0].Rows[0]["ItemCode"].ToString().Trim()).Selected = true;
                    }
                }

                if ((catData.Tables[0].Rows[0]["ProductName"] != null) && (catData.Tables[0].Rows[0]["ProductName"].ToString() != ""))
                {
                    if (cmbProdName.Items.FindByValue(catData.Tables[0].Rows[0]["ProductName"].ToString().Trim()) != null)
                    {
                        cmbProdName.ClearSelection();
                        cmbProdName.Items.FindByValue(catData.Tables[0].Rows[0]["ProductName"].ToString().Trim()).Selected = true;
                    }
                }

                if ((catData.Tables[0].Rows[0]["ProductDesc"] != null) && (catData.Tables[0].Rows[0]["ProductDesc"].ToString() != ""))
                {
                    if (cmbBrand.Items.FindByValue(catData.Tables[0].Rows[0]["ProductDesc"].ToString().Trim()) != null)
                    {
                        cmbBrand.ClearSelection();
                        if (!cmbBrand.Items.FindByValue(catData.Tables[0].Rows[0]["ProductDesc"].ToString().Trim()).Selected)
                            cmbBrand.Items.FindByValue(catData.Tables[0].Rows[0]["ProductDesc"].ToString().Trim()).Selected = true;
                    }
                }

                if ((catData.Tables[0].Rows[0]["Model"] != null) && (catData.Tables[0].Rows[0]["Model"].ToString() != ""))
                {
                    if (cmbModel.Items.FindByValue(catData.Tables[0].Rows[0]["Model"].ToString().Trim()) != null)
                    {
                        cmbModel.ClearSelection();
                        if (!cmbModel.Items.FindByValue(catData.Tables[0].Rows[0]["Model"].ToString().Trim()).Selected)
                            cmbModel.Items.FindByValue(catData.Tables[0].Rows[0]["Model"].ToString().Trim()).Selected = true;

                    }
                }

            }

            txtRateAdd.Text = row.Cells[2].Text;
            txtNLPAdd.Text = row.Cells[3].Text;
            txtQtyAdd.Text = row.Cells[4].Text;
            txtQtyAdd.Focus();
            /*Start March 15 Modification */
            //hdEditQty.Value = row.Cells[].Text;
            /*End March 15 Modification */
            if (row.Cells[5].Text != "&nbsp;")
                lblUnitMrmnt.Text = row.Cells[5].Text.Trim();
            else
                lblUnitMrmnt.Text = string.Empty;

            lblDisAdd.Text = row.Cells[6].Text;

            lblVATAdd.Text = row.Cells[7].Text;
            lblCSTAdd.Text = row.Cells[8].Text;
            lbldiscamt.Text = row.Cells[9].Text;
            lblProdNameAdd.Text = row.Cells[1].Text;
            lblProdDescAdd.Text = row.Cells[1].Text;
            updatePnlProduct.Update();
            ModalPopupProduct.Show();

        }
        catch (Exception ex)
        {
            TroyLiteExceptionManager.HandleException(ex);
        }

        

    }

    protected void cmdDelete_Click(object sender, EventArgs e)
    {
        try
        {
            if (Page.IsValid)
            {
                string connection = Request.Cookies["Company"].Value;
                BusinessLogic bll = new BusinessLogic();
                string recondate = txtBillDate.Text.Trim(); ;

                string recondat = txtInvoiveDate.Text.Trim(); ;
                if (!bll.IsValidDate(connection, Convert.ToDateTime(recondat)))
                {

                    ScriptManager.RegisterStartupScript(Page, Page.GetType(), Guid.NewGuid().ToString(), "alert('Date is invalid')", true);
                    return;
                }
                int iPurchase = 0;
                string sBillNo = txtBillno.Text.Trim();
                iPurchase = Convert.ToInt32(hdPurchase.Value);
                //string sDataSource = Server.MapPath(ConfigurationSettings.AppSettings["DataSource"].ToString());
                BusinessLogic bl = new BusinessLogic(sDataSource);
                string usernam = Request.Cookies["LoggedUserName"].Value;

                int del = bl.DeletePurchase(iPurchase, sBillNo, usernam);
                /*Start Purchase Stock Negative Change - March 16*/
                if (del == -2)
                {
                    ScriptManager.RegisterStartupScript(Page, Page.GetType(), Guid.NewGuid().ToString(), "alert('Purchase edit is not allowed for this transaction.')", true);
                    return;
                }
                /*End Purchase Stock Negative Change - March 16*/
                Reset();
                ResetProduct();
                /*Start Purchase Stock Negative Change - March 16 -- (Commented the below method)*/
                //if (File.Exists(Server.MapPath("Reports\\" + hdFilename.Value + "_Product.xml")))
                //    File.Delete(Server.MapPath("Reports\\" + hdFilename.Value + "_Product.xml"));
                /*Start Purchase Stock Negative Change - March 16 -- (Commented the below method)*/
                //purchasePanel.Visible = false;
                lnkBtnAdd.Visible = true;
                pnlSearch.Visible = true;
                //PanelBill.Visible = false;
                PanelCmd.Visible = false;
                hdMode.Value = "Delete";
                cmdPrint.Enabled = false;
                delFlag.Value = "0";
                deleteFile();
                ScriptManager.RegisterStartupScript(Page, Page.GetType(), Guid.NewGuid().ToString(), "alert('Purchase Details Deleted Successfully.  Bill No. is " + sBillNo.ToString() + "')", true);
                BindGrid("0", "0");
                btnCancel.Enabled = false;
                Session["PurchaseProductDs"] = null;

                PanelCmd.Visible = false;
                //purchasePanel.Visible = false;
                lnkBtnAdd.Visible = true;
                pnlSearch.Visible = true;
            }
        }
        catch (Exception ex)
        {
            TroyLiteExceptionManager.HandleException(ex);
        }
    }

    protected void GrdViewPurchase_RowDeleting(object sender, GridViewDeleteEventArgs e)
    {
        try
        {
            string connection = Request.Cookies["Company"].Value;
            BusinessLogic bll = new BusinessLogic();
            string recondate = GrdViewPurchase.Rows[e.RowIndex].Cells[3].Text;

            string recondat = GrdViewPurchase.Rows[e.RowIndex].Cells[3].Text;
            if (!bll.IsValidDate(connection, Convert.ToDateTime(recondat)))
            {

                ScriptManager.RegisterStartupScript(Page, Page.GetType(), Guid.NewGuid().ToString(), "alert('Date is invalid')", true);
                return;
            }
            int iPurchase = 0;
            string sBillNo = GrdViewPurchase.Rows[e.RowIndex].Cells[2].Text.Trim();
            iPurchase = Convert.ToInt32(GrdViewPurchase.DataKeys[e.RowIndex].Value.ToString());
            //string sDataSource = Server.MapPath(ConfigurationSettings.AppSettings["DataSource"].ToString());
            BusinessLogic bl = new BusinessLogic(sDataSource);
            string usernam = Request.Cookies["LoggedUserName"].Value;

            int del = bl.DeletePurchase(iPurchase, sBillNo, usernam);
            /*Start Purchase Stock Negative Change - March 16*/
            if (del == -2)
            {
                ScriptManager.RegisterStartupScript(Page, Page.GetType(), Guid.NewGuid().ToString(), "alert('Purchase edit is not allowed for this transaction.')", true);
                return;
            }
            /*End Purchase Stock Negative Change - March 16*/
            //Reset();
            //ResetProduct();
            /*Start Purchase Stock Negative Change - March 16 -- (Commented the below method)*/
            //if (File.Exists(Server.MapPath("Reports\\" + hdFilename.Value + "_Product.xml")))
            //    File.Delete(Server.MapPath("Reports\\" + hdFilename.Value + "_Product.xml"));
            /*Start Purchase Stock Negative Change - March 16 -- (Commented the below method)*/
            //purchasePanel.Visible = false;
            //lnkBtnAdd.Visible = true;
            //pnlSearch.Visible = true;
            //PanelBill.Visible = false;
            //PanelCmd.Visible = false;
            //hdMode.Value = "Delete";
            //cmdPrint.Enabled = false;
            //delFlag.Value = "0";
            //deleteFile();
            ScriptManager.RegisterStartupScript(Page, Page.GetType(), Guid.NewGuid().ToString(), "alert('Purchase Details Deleted Successfully. Deleted Bill No. is " + sBillNo.ToString() + "')", true);
            BindGrid("0", "0");


            //string salestype = string.Empty;
            //int ScreenNo = 0;
            //string ScreenName = string.Empty;

            //if (salesReturn == "YES")
            //{
            //    salestype = "Sales Return";
            //    ScreenName = "Sales Return";
            //}
            //else if (intTrans == "YES")
            //{
            //    salestype = "Internal Transfer";
            //    ScreenName = "Purchase - Internal";
            //}
            //else if (deliveryNote == "YES")
            //{
            //    salestype = "Delivery Note";
            //    ScreenName = "Purchase - DC";
            //}
            //else
            //{
            //    salestype = "Normal Purchase";
            //    ScreenName = "Purchase - Normal";
            //}

            //DataSet dsddd = bl.GetScreenNoForScreenName(connection, ScreenName);
            //if (dsddd != null)
            //{
            //    if (dsddd.Tables[0].Rows.Count > 0)
            //    {
            //        foreach (DataRow dr in dsddd.Tables[0].Rows)
            //        {
            //            ScreenNo = Convert.ToInt32(dr["ScreenNo"]);
            //        }
            //    }
            //}

            //if (hdEmailRequired.Value == "YES")
            //{
            //    DataSet dsd = bl.GetLedgerInfoForId(connection, iSupplier);
            //    var toAddress = "";
            //    var toAdd = "";
            //    Int32 ModeofContact = 0;
            //    string Active = string.Empty;

            //    if (dsd != null)
            //    {
            //        if (dsd.Tables[0].Rows.Count > 0)
            //        {
            //            foreach (DataRow dr in dsd.Tables[0].Rows)
            //            {
            //                toAdd = dr["EmailId"].ToString();
            //                ModeofContact = Convert.ToInt32(dr["ModeofContact"]);
            //            }
            //        }
            //    }

            //    DataSet dsdd = bl.GetDetailsForScreenNo(connection, ScreenNo, "Email");
            //    if (dsdd != null)
            //    {
            //        if (dsdd.Tables[0].Rows.Count > 0)
            //        {
            //            foreach (DataRow dr in dsdd.Tables[0].Rows)
            //            {
            //                Active = dr["Active"].ToString();
            //                if (Active == "YES")
            //                {
            //                    if (dr["EmailId"].ToString() == "Customer")
            //                    {
            //                        toAddress = toAdd;
            //                    }
            //                    else
            //                    {
            //                        toAddress = dr["EmailId"].ToString();
            //                    }

            //                    if (ModeofContact == 2)
            //                    {
            //                        string subject = "Added - " + Paymode + " Purchase in Branch " + Request.Cookies["Company"].Value;

            //                        string body = "\n";
            //                        body += " Branch           : " + Request.Cookies["Company"].Value + "\n";
            //                        body += " Purchase Type       : " + salestype + "\n";
            //                        //body += " Customer Name    : " + sCustomerName + "\n";
            //                        //body += " Customer Address : " + sCustomerAddress + "\n";
            //                        body += " Bill Date        : " + sBilldate + "\n";
            //                        body += " Payment Mode     : " + Paymode + "\n";
            //                        body += " User Name        : " + usernam + "\n";
            //                        body += " Total Amount     : " + dfixedtotal + "\n";

            //                        string smtphostname = ConfigurationManager.AppSettings["SmtpHostName"].ToString();
            //                        int smtpport = Convert.ToInt32(ConfigurationManager.AppSettings["SmtpPortNumber"]);
            //                        var fromAddress = ConfigurationManager.AppSettings["FromAddress"].ToString();

            //                        string fromPassword = ConfigurationManager.AppSettings["FromPassword"].ToString();

            //                        EmailLogic.SendEmail(smtphostname, smtpport, fromAddress, toAddress, subject, body, fromPassword);

            //                        //ScriptManager.RegisterStartupScript(Page, Page.GetType(), Guid.NewGuid().ToString(), "alert('Email sent successfully')", true);
            //                    }

            //                }
            //            }
            //        }
            //    }
            //}

            //btnCancel.Enabled = false;
            //Session["PurchaseProductDs"] = null;

            //PanelCmd.Visible = false;
            //purchasePanel.Visible = false;
            //lnkBtnAdd.Visible = true;
            //pnlSearch.Visible = true;
        }
        catch (Exception ex)
        {
            TroyLiteExceptionManager.HandleException(ex);
        }
    }


    protected void lnkAddProduct_Click(object sender, EventArgs e)
    {
        try
        {
            if (Convert.ToDouble(txtfixedtotal.Text) == 0)
            {
                ScriptManager.RegisterStartupScript(Page, Page.GetType(), Guid.NewGuid().ToString(), "alert('Please Enter FixedTotal Before Adding Products')", true);
                return;
            }

            ResetProduct();
            cmbCategory.SelectedIndex = 0;
            cmbModel.Enabled = true;
            cmbBrand.Enabled = true;
            cmbProdAdd.Enabled = true;
            cmbProdName.Enabled = true;
            cmbCategory.Enabled = true;
            BtnClearFilter.Enabled = true;
            ClearFilter();
            cmdSaveProduct.Visible = true;
            //Label2.Visible = true;
            cmdSaveProduct.Enabled = true;
            cmdUpdateProduct.Visible = false;
            //Label3.Visible = false;
            updatePnlProduct.Update();
            ModalPopupProduct.Show();
        }
        catch (Exception ex)
        {
            TroyLiteExceptionManager.HandleException(ex);
        }
    }

    protected void cmdMethod_Click(object sender, EventArgs e)
    {
        //if (!Helper.IsLicenced(Request.Cookies["Company"].Value))
        //{
        //    ScriptManager.RegisterStartupScript(Page, Page.GetType(), Guid.NewGuid().ToString(), "alert('This is Trial Version, Please upgrade to Full Version of this Software. Thank You.');", true);
        //    return;
        //}


        //////////////////////////////////////////////////////////////////////

        try
        {
            BusinessLogic bl = new BusinessLogic(sDataSource);
            string connection = Request.Cookies["Company"].Value;
            string usernam = Request.Cookies["LoggedUserName"].Value;

            if (optionmethod.SelectedValue == "InternalTransfer")
            {
                if (bl.CheckUserHaveOptions(usernam, "INTPUR"))
                {
                    ScriptManager.RegisterStartupScript(Page, Page.GetType(), Guid.NewGuid().ToString(), "alert('You are not allowed to make Internal Transfer');", true);
                    return;
                }
            }
            else if (optionmethod.SelectedValue == "DeliveryNote")
            {
                if (bl.CheckUserHaveOptions(usernam, "DCPUR"))
                {
                    ScriptManager.RegisterStartupScript(Page, Page.GetType(), Guid.NewGuid().ToString(), "alert('You are not allowed to make Delivery Note');", true);
                    return;
                }
            }
            else if (optionmethod.SelectedValue == "SalesReturn")
            {
                if (bl.CheckUserHaveOptions(usernam, "SALRET"))
                {
                    ScriptManager.RegisterStartupScript(Page, Page.GetType(), Guid.NewGuid().ToString(), "alert('You are not allowed to make Sales Return');", true);
                    return;
                }
            }

            string enabledate = string.Empty;
            enabledate = bl.getEnableDateConfigInfoMethod();
            if (enabledate == "YES")
            {
                txtInvoiveDate.Enabled = true;
            }
            else
            {
                txtInvoiveDate.Enabled = false;
            }

            ////////////////////////////////////////////////////////////////////////


            Session["PurchaseProductDs"] = null;

            cmdSave.Enabled = true;
            cmdSave.Visible = true;

            cmdUpdateProduct.Enabled = false;
            cmdSaveProduct.Enabled = true;
            //cmdCancelProduct.Visible = false;
            cmdUpdateProduct.Visible = false;
            //Label3.Visible = false;
            cmdSaveProduct.Visible = true;
            //Label2.Visible = true;
            //cmdCancelProduct.Visible = false;

            cmdUpdate.Visible = false;
            //cmdDelete.Visible = false;
            //cmdPrint.Visible = false;
            hdMode.Value = "New";
            Reset();
            lblTotalSum.Text = "0";
            lblTotalDis.Text = "0";
            lblTotalVAT.Text = "0";
            lblTotalCST.Text = "0";
            lblFreight.Text = "0";
            lblNet.Text = "0";

            ResetProduct();
            //txtBillDate.Text = DateTime.Now.ToShortDateString();
            //txtInvoiveDate.Text = DateTime.Now.ToShortDateString();

            DateTime indianStd = TimeZoneInfo.ConvertTimeBySystemTimeZoneId(DateTime.Now, "India Standard Time");
            string dtaa = Convert.ToDateTime(indianStd).ToString("dd/MM/yyyy");
            txtBillDate.Text = dtaa;

            DateTime indianSt = TimeZoneInfo.ConvertTimeBySystemTimeZoneId(DateTime.Now, "India Standard Time");
            string dta = Convert.ToDateTime(indianSt).ToString("dd/MM/yyyy");
            txtInvoiveDate.Text = dta;

            XmlDocument xDoc = new XmlDocument();

            if (File.Exists(Server.MapPath("Reports\\" + hdFilename.Value + "_Product.xml")))
            {
                File.Delete(Server.MapPath("Reports\\" + hdFilename.Value + "_Product.xml"));
            }
            btnCancel.Enabled = true;
            GrdViewItems.DataSource = null;
            GrdViewItems.DataBind();
            rowSalesRet.Visible = false;
            loadBilts("0");
            txtInvoiveNo.Text = "";
            //txtInvoiveNo.Enabled = false;
            txtBillno.Focus();
            updatePnlPurchase.Update();
            ModalPopupPurchase.Show();

            EmptyRow();


            if (optionmethod.SelectedValue == "Purchase")
            {
                drpIntTrans.ClearSelection();
                ListItem cli = drpIntTrans.Items.FindByValue(Convert.ToString("NO"));
                if (cli != null) cli.Selected = true;

                drpSalesReturn.ClearSelection();
                ListItem c = drpSalesReturn.Items.FindByValue(Convert.ToString("NO"));
                if (c != null) c.Selected = true;

                ddDeliveryNote.ClearSelection();
                ListItem cl = ddDeliveryNote.Items.FindByValue(Convert.ToString("NO"));
                if (cl != null) cl.Selected = true;

                rqSalesReturn.Enabled = false;
                rowdcnum.Visible = false;
                rowSalesRet.Visible = false;
                RequiredFieldValidator2.Enabled = false;
                drpIntTrans.Enabled = false;
                drpSalesReturn.Enabled = false;
                ddDeliveryNote.Enabled = false;

                cmdPaymode.SelectedValue = "3";
            }
            else if (optionmethod.SelectedValue == "InternalTransfer")
            {
                drpIntTrans.ClearSelection();
                ListItem cli = drpIntTrans.Items.FindByValue(Convert.ToString("YES"));
                if (cli != null) cli.Selected = true;

                drpSalesReturn.ClearSelection();
                ListItem c = drpSalesReturn.Items.FindByValue(Convert.ToString("NO"));
                if (c != null) c.Selected = true;

                ddDeliveryNote.ClearSelection();
                ListItem cl = ddDeliveryNote.Items.FindByValue(Convert.ToString("NO"));
                if (cl != null) cl.Selected = true;

                rqSalesReturn.Enabled = false;
                rowdcnum.Visible = false;
                rowSalesRet.Visible = false;
                RequiredFieldValidator2.Enabled = false;
                drpIntTrans.Enabled = false;
                drpSalesReturn.Enabled = false;
                ddDeliveryNote.Enabled = false;

                cmdPaymode.SelectedValue = "3";

            }
            else if (optionmethod.SelectedValue == "DeliveryNote")
            {
                ddDeliveryNote.ClearSelection();
                ListItem cl = ddDeliveryNote.Items.FindByValue(Convert.ToString("YES"));
                if (cl != null) cl.Selected = true;

                drpIntTrans.ClearSelection();
                ListItem cli = drpIntTrans.Items.FindByValue(Convert.ToString("NO"));
                if (cli != null) cli.Selected = true;

                drpSalesReturn.ClearSelection();
                ListItem c = drpSalesReturn.Items.FindByValue(Convert.ToString("NO"));
                if (c != null) c.Selected = true;

                RequiredFieldValidator2.Enabled = true;
                rqSalesReturn.Enabled = false;
                rowdcnum.Visible = true;
                rowSalesRet.Visible = false;
                drpIntTrans.Enabled = false;
                drpSalesReturn.Enabled = false;
                ddDeliveryNote.Enabled = false;

                cmdPaymode.SelectedValue = "3";

            }
            else if (optionmethod.SelectedValue == "SalesReturn")
            {
                drpSalesReturn.ClearSelection();
                ListItem cliddd = drpSalesReturn.Items.FindByValue(Convert.ToString("YES"));
                if (cliddd != null) cliddd.Selected = true;

                ddDeliveryNote.ClearSelection();
                ListItem cl = ddDeliveryNote.Items.FindByValue(Convert.ToString("NO"));
                if (cl != null) cl.Selected = true;

                drpIntTrans.ClearSelection();
                ListItem cli = drpIntTrans.Items.FindByValue(Convert.ToString("NO"));
                if (cli != null) cli.Selected = true;

                rqSalesReturn.Enabled = true;
                rowdcnum.Visible = false;
                rowSalesRet.Visible = true;
                RequiredFieldValidator2.Enabled = false;
                drpIntTrans.Enabled = false;
                drpSalesReturn.Enabled = false;
                ddDeliveryNote.Enabled = false;

            }

            if (drpSalesReturn.SelectedValue == "NO")
            {
                loadSupplier("Sundry Creditors");
            }
            else
            {
                loadSupplier("Sundry Debtors");
            }
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
            Session["Show"] = "No";

            chk.Checked = true;
            optionmethod.SelectedIndex = 0;
            ModalPopupMethod.Show();
        }
        catch (Exception ex)
        {
            TroyLiteExceptionManager.HandleException(ex);
        }
    }

    private void Reset()
    {
        txtBillno.Text = "";
        txtBillDate.Text = "";

        txtInvoiveNo.Text = "";
        txtInvoiveDate.Text = "";

        txtFreight.Text = "0";
        txtLU.Text = "0";

        txtdiscamt.Text = "0";
        txtdisc.Text = "0";
        txtfixedtotal.Text = "0";
        txtnarr.Text = "";
        txtdcbillno.Text = "0";
        txtAddress1.Text = "";
        txtAddress2.Text = "";
        txtAddress3.Text = "";
        txtMobile.Text = "";

        /*Start Purchase Loading / Unloading Freight Change - March 16*/
        foreach (Control control in cmbSupplier.Controls)
        {
            if (control is HiddenField)
                ((HiddenField)control).Value = "0";
        }
        /*End Purchase Loading / Unloading Freight Change - March 16*/

        //cmbSupplier.SelectedItem.Text = "";
        cmbSupplier.ClearSelection();

        foreach (Control control in cmdPaymode.Controls)
        {
            if (control is HiddenField)
                ((HiddenField)control).Value = "1";
        }

        cmdPaymode.ClearSelection();

        foreach (Control control in cmbBankName.Controls)
        {
            if (control is HiddenField)
                ((HiddenField)control).Value = "0";
        }
        cmbBankName.ClearSelection();

        foreach (Control control in cmbProdAdd.Controls)
        {
            if (control is HiddenField)
                ((HiddenField)control).Value = "0";
        }

        cmbProdAdd.ClearSelection();
        txtChequeNo.Text = ""; // cmbBankName.SelectedItem.Text;
        //BindGrid(txtBillnoSrc.Text);
        //Accordion1.SelectedIndex = 1;
        txtSRReason.Text = "";
        drpSalesReturn.SelectedIndex = 0;
        GrdViewItems.DataSource = null;
        GrdViewItems.DataBind();

        txtSupplier.Visible = false;
        chk.Checked = true;
    }

    protected void cmbSupplier_SelectedIndexChanged(object sender, EventArgs e)
    {
        try
        {
            BusinessLogic bl = new BusinessLogic(sDataSource);

            int iLedgerID = Convert.ToInt32(cmbSupplier.SelectedItem.Value);
                           
            DataSet customerDs = bl.getAddressInfo(iLedgerID);
            string address = string.Empty;

            if (customerDs != null && customerDs.Tables[0].Rows.Count > 0)
            {
                if (customerDs.Tables[0].Rows[0]["Add1"] != null)
                    txtAddress1.Text = customerDs.Tables[0].Rows[0]["Add1"].ToString() + Environment.NewLine;

                if (customerDs.Tables[0].Rows[0]["Add2"] != null)
                    txtAddress2.Text = address + customerDs.Tables[0].Rows[0]["Add2"].ToString() + Environment.NewLine;

                if (customerDs.Tables[0].Rows[0]["Add3"] != null)
                    txtAddress3.Text = address + customerDs.Tables[0].Rows[0]["Add3"].ToString() + Environment.NewLine;

                if (customerDs.Tables[0].Rows[0]["Mobile"] != null)
                {
                    txtMobile.Text = Convert.ToString(customerDs.Tables[0].Rows[0]["Mobile"]);
                }
            }
            else
            {
                txtAddress1.Text = string.Empty;
                txtAddress2.Text = string.Empty;
                txtAddress3.Text = string.Empty;
                txtMobile.Text = string.Empty;
            }

            txtAddress1.Enabled = false;
            txtAddress2.Enabled = false;
            txtAddress3.Enabled = false;
            txtMobile.Enabled = false;
        }
        catch (Exception ex)
        {
            TroyLiteExceptionManager.HandleException(ex);
        }
    }

    private void loadSupplier(string SundryType)
    {
        //string sDataSource = Server.MapPath(ConfigurationSettings.AppSettings["DataSource"].ToString());
        BusinessLogic bl = new BusinessLogic(sDataSource);
        DataSet ds = new DataSet();

        if (SundryType == "Sundry Debtors")
        {
            //ds = bl.ListSundryDebtors(sDataSource);
            ds = bl.ListSundryDebtorsExcept(sDataSource);
        }

        if (SundryType == "Sundry Creditors")
        {
            if (drpIntTrans.SelectedValue == "YES")
            {
                ds = bl.ListLedgersTransfer(sDataSource);
                //ds = bl.ListSundryCreditorsTransfer(sDataSource);
            }
            else if (ddDeliveryNote.SelectedValue == "YES")
            {
                //ds = bl.ListSundryCreditorsDc(sDataSource);
                ds = bl.ListSundryLedgersDc(sDataSource);               
            }
            else
            {
                ds = bl.ListSundryCreditorsExcept(sDataSource);
            }
        }

        cmbSupplier.Items.Clear();
        ListItem li = new ListItem("Select Supplier", "0");
        li.Attributes.Add("style", "color:Black");
        cmbSupplier.Items.Add(li);
        cmbSupplier.DataSource = ds;
        cmbSupplier.Items[0].Attributes.Add("background-color", "color:#bce1fe");
        cmbSupplier.DataBind();
        cmbSupplier.DataTextField = "LedgerName";
        cmbSupplier.DataValueField = "LedgerID";
    }

    protected void cmdCancelMethod_Click(object sender, EventArgs e)
    {
        try
        { 
            ModalPopupMethod.Hide();
        }
        catch (Exception ex)
        {
            TroyLiteExceptionManager.HandleException(ex);
        }
    }

    private void loadBilts(string ID)
    {
        //string sDataSource = Server.MapPath(ConfigurationSettings.AppSettings["DataSource"].ToString());
        BusinessLogic bl = new BusinessLogic(sDataSource);
        DataSet ds = new DataSet();

        ds = bl.LitsOpenBilts(ID);

        ddBilts.Items.Clear();
        ListItem li = new ListItem("Select Bilty", "0");
        ddBilts.Items.Add(li);
        ddBilts.DataSource = ds;
        ddBilts.DataTextField = "Bilt";
        ddBilts.DataValueField = "ID";
        ddBilts.DataBind();
    }

    private void loadProducts()
    {
        //string sDataSource = Server.MapPath(ConfigurationSettings.AppSettings["DataSource"].ToString());
        BusinessLogic bl = new BusinessLogic(sDataSource);
        DataSet ds = new DataSet();
        ds = bl.ListProducts();
        cmbProdAdd.DataSource = ds;
        cmbProdAdd.DataBind();

        cmbProdAdd.DataTextField = "ProductName";
        cmbProdAdd.DataValueField = "ItemCode";
    }

    private bool paymodeVisible(string paymode)
    {
        if (paymode.ToUpper() != "BANK")
        {
            pnlBank.Visible = false;
            return false;
        }
        else
        {

            pnlBank.Visible = true;
            return true;
        }
    }

    protected void GrdViewPurchase_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        try
        { 
            GrdViewPurchase.PageIndex = e.NewPageIndex;
            //String strBillno = string.Empty;
            //string strTransNo = string.Empty;

            //if (txtBillnoSrc.Text.Trim() != "")
            //    strBillno = txtBillnoSrc.Text.Trim();
            //else
            //    strBillno = "0";

            //if (txtTransNo.Text.Trim() != "")
            //    strTransNo = txtTransNo.Text.Trim();
            //else
            //    strTransNo = "0";

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

    private void BindGrid(string textSearch, string dropDown)
    {
        DataSet ds = new DataSet();
        //string sDataSource = Server.MapPath(ConfigurationSettings.AppSettings["DataSource"].ToString());
        BusinessLogic bl = new BusinessLogic(sDataSource);
        
        object usernam = Session["LoggedUserName"];
        
        //if (strBillno == "0" && strTransNo == "0")
        ds = bl.GetPurchaseList(textSearch, dropDown);
        //else
        //    ds = bl.GetPurchaseForId(strBillno, strTransNo);

        if (ds != null)
        {
            if (ds.Tables[0].Rows.Count > 0)
            {
                GrdViewPurchase.DataSource = ds.Tables[0].DefaultView;
                GrdViewPurchase.DataBind();
            }
        }
        else
        {
            GrdViewPurchase.DataSource = null;
            GrdViewPurchase.DataBind();
        }
    }
    protected void GrdViewPurchase_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        try
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                string paymode = Convert.ToString(DataBinder.Eval(e.Row.DataItem, "paymode"));
                Label payMode = (Label)e.Row.FindControl("lblPaymode");
                if (paymode == "1")
                    payMode.Text = "Cash";
                else if (paymode == "2")
                    payMode.Text = "Bank";
                else
                    payMode.Text = "Credit";

                BusinessLogic bl = new BusinessLogic(sDataSource);
                string connection = Request.Cookies["Company"].Value;
                string usernam = Request.Cookies["LoggedUserName"].Value;

                if (bl.CheckUserHaveEdit(usernam, "PURCHS"))
                {
                    ((ImageButton)e.Row.FindControl("btnEdit")).Visible = false;
                    ((ImageButton)e.Row.FindControl("btnEditDisabled")).Visible = true;
                }

                if (bl.CheckUserHaveDelete(usernam, "PURCHS"))
                {
                    ((ImageButton)e.Row.FindControl("lnkB")).Visible = false;
                    ((ImageButton)e.Row.FindControl("lnkBDisabled")).Visible = true;
                }

                if (bl.CheckUserHaveView(usernam, "PURCHS"))
                {
                    ((Image)e.Row.FindControl("lnkprint")).Visible = false;
                    ((ImageButton)e.Row.FindControl("btnViewDisabled")).Visible = true;
                }
                else
                {
                    ((ImageButton)e.Row.FindControl("btnViewDisabled")).Visible = false;
                }
            }
        }
        catch (Exception ex)
        {
            TroyLiteExceptionManager.HandleException(ex);
        }
    }

    protected void GrdViewPurchase_SelectedIndexChanged(object sender, EventArgs e)
    {
        try
        {
            Session["Show"] = "Hide";
            string strPaymode = string.Empty;
            int SupplierID = 0;
            int purchaseID = 0;
            GridViewRow row = GrdViewPurchase.SelectedRow;
            string connection = Request.Cookies["Company"].Value;
            /*Start Purchase Loading / Unloading Freight Change - March 16*/
            BusinessLogic bl = new BusinessLogic(sDataSource);
            /*End Purchase Loading / Unloading Freight Change - March 16*/

            //rvBillDate.MinimumValue = System.DateTime.Now.AddYears(-100).ToShortDateString();
            //rvBillDate.MaximumValue = System.DateTime.Now.ToShortDateString();

            //valdate.MinimumValue = System.DateTime.Now.AddYears(-100).ToShortDateString();
            //valdate.MaximumValue = System.DateTime.Now.ToShortDateString();

            string enabledate = string.Empty;
            enabledate = bl.getEnableDateConfigInfoMethod();
            if (enabledate == "YES")
            {
                txtInvoiveDate.Enabled = true;
            }
            else
            {
                txtInvoiveDate.Enabled = false;
            }

            StringBuilder script = new StringBuilder();
            script.Append("alert('You are not allowed to delete the record. Please contact Administrator.');");

            string recondate = row.Cells[3].Text;
            if (!bl.IsValidDate(connection, Convert.ToDateTime(recondate)))
            {
                ScriptManager.RegisterStartupScript(Page, Page.GetType(), Guid.NewGuid().ToString(), script.ToString(), true);
                return;
            }

            //txtInvoiveNo.Enabled = false;

            purchaseID = Convert.ToInt32(GrdViewPurchase.SelectedDataKey.Value);

            /*Start Purchase Loading / Unloading Freight Change - March 16*/
            DataSet ds = bl.GetPurchaseForId(purchaseID);

            if (ds.Tables[0].Rows[0]["Billno"] != null)
                txtBillno.Text = ds.Tables[0].Rows[0]["Billno"].ToString();

            if (ds.Tables[0].Rows[0]["PurchaseId"] != null)
                txtInvoiveNo.Text = ds.Tables[0].Rows[0]["PurchaseId"].ToString();


            if (ds.Tables[0].Rows[0]["Freight"] != null)
                txtFreight.Text = Convert.ToString(ds.Tables[0].Rows[0]["Freight"]);
            else
                txtFreight.Text = "0";

            if (ds.Tables[0].Rows[0]["DiscAmount"] != null)
                txtdiscamt.Text = Convert.ToString(ds.Tables[0].Rows[0]["DiscAmount"]);
            else
                txtdiscamt.Text = "0";

            if (ds.Tables[0].Rows[0]["DiscPer"] != null)
                txtdisc.Text = Convert.ToString(ds.Tables[0].Rows[0]["DiscPer"]);
            else
                txtdisc.Text = "0";

            if (ds.Tables[0].Rows[0]["DcBillNo"] != null)
                txtdcbillno.Text = Convert.ToString(ds.Tables[0].Rows[0]["DcBillNo"]);
            else
                txtdcbillno.Text = "0";

            if (ds.Tables[0].Rows[0]["Amount"] != null)
                txtfixedtotal.Text = Convert.ToString(ds.Tables[0].Rows[0]["Amount"]);
            else
                txtfixedtotal.Text = "0";

            if (ds.Tables[0].Rows[0]["narration2"] != null)
                txtnarr.Text = Convert.ToString(ds.Tables[0].Rows[0]["narration2"]);
            else
                txtnarr.Text = "";

            if (ds.Tables[0].Rows[0]["LoadUnload"] != null)
                txtLU.Text = Convert.ToString(ds.Tables[0].Rows[0]["LoadUnload"]);
            else
                txtLU.Text = "0";
            /*End Purchase Loading / Unloading Freight Change - March 16*/

            if (ds.Tables[0].Rows[0]["SalesReturn"] != null)
            {
                drpSalesReturn.ClearSelection();
                drpSalesReturn.SelectedValue = ds.Tables[0].Rows[0]["SalesReturn"].ToString().ToUpper();
            }
            else
            {
                drpSalesReturn.SelectedIndex = 0;
            }

            if (ds.Tables[0].Rows[0]["InternalTransfer"] != null && !string.IsNullOrEmpty(ds.Tables[0].Rows[0]["InternalTransfer"].ToString()))
            {
                drpIntTrans.ClearSelection();
                drpIntTrans.SelectedValue = ds.Tables[0].Rows[0]["InternalTransfer"].ToString().ToUpper();
            }
            else
            {
                drpIntTrans.SelectedIndex = 0;
            }

            if (ds.Tables[0].Rows[0]["DeliveryNote"] != null && !string.IsNullOrEmpty(ds.Tables[0].Rows[0]["DeliveryNote"].ToString()))
            {
                ddDeliveryNote.ClearSelection();
                ddDeliveryNote.SelectedValue = ds.Tables[0].Rows[0]["DeliveryNote"].ToString().ToUpper();
            }
            else
            {
                ddDeliveryNote.SelectedIndex = 0;
            }


            if (ds.Tables[0].Rows[0]["BilitID"] != null)
            {
                ddBilts.Items.Clear();
                loadBilts(ds.Tables[0].Rows[0]["BilitID"].ToString());
                ddBilts.SelectedValue = ds.Tables[0].Rows[0]["BilitID"].ToString();
            }
            else
            {
                ddBilts.SelectedIndex = 0;
            }

            if (drpSalesReturn.SelectedValue == "NO")
            {
                rowSalesRet.Visible = false;
            }
            else
            {
                rowSalesRet.Visible = true;
            }

            if (drpSalesReturn.SelectedItem.Text == "NO")
            {
                loadSupplier("Sundry Creditors");
            }
            else
            {
                loadSupplier("Sundry Debtors");
            }

            if ((ds.Tables[0].Rows[0]["SupplierID"] != null) && (ds.Tables[0].Rows[0]["SupplierID"].ToString() != ""))
            {
                SupplierID = Convert.ToInt32(ds.Tables[0].Rows[0]["SupplierID"].ToString());
                cmbSupplier.ClearSelection();
                ListItem li = cmbSupplier.Items.FindByValue(System.Web.HttpUtility.HtmlDecode(SupplierID.ToString()));
                if (li != null) li.Selected = true;
            }

            txtSupplier.Visible = false;
            chk.Checked = true;

            DataSet customerDs = bl.getAddressInfo(Convert.ToInt32(ds.Tables[0].Rows[0]["SupplierID"].ToString()));

            if (customerDs != null && customerDs.Tables[0].Rows.Count > 0)
            {
                if (customerDs.Tables[0].Rows[0]["Add1"] != null)
                    txtAddress1.Text = customerDs.Tables[0].Rows[0]["Add1"].ToString();

                if (customerDs.Tables[0].Rows[0]["Add2"] != null)
                    txtAddress2.Text = customerDs.Tables[0].Rows[0]["Add2"].ToString();

                if (customerDs.Tables[0].Rows[0]["Add3"] != null)
                    txtAddress3.Text = customerDs.Tables[0].Rows[0]["Add3"].ToString();

                if (customerDs.Tables[0].Rows[0]["Mobile"] != null)
                {
                    txtMobile.Text = Convert.ToString(customerDs.Tables[0].Rows[0]["Mobile"]);
                }                
            }
            else
            {
                txtAddress1.Text = "";
                txtAddress2.Text = "";
                txtAddress3.Text = "";
                txtMobile.Text = "";
            }


            Label pM = (Label)row.FindControl("lblPaymode"); //row.Cells[3].Text;
            strPaymode = pM.Text;

            if (paymodeVisible(strPaymode))
            {
                if (ds.Tables[0].Rows[0]["Chequeno"] != null)
                    txtChequeNo.Text = ds.Tables[0].Rows[0]["Chequeno"].ToString();

                if (ds.Tables[0].Rows[0]["Creditor"] != null)
                {
                    cmbBankName.ClearSelection();
                    ListItem cli = cmbBankName.Items.FindByText(ds.Tables[0].Rows[0]["Creditor"].ToString());
                    if (cli != null) cli.Selected = true;
                }

            }

            if (ds.Tables[0].Rows[0]["Paymode"] != null)
            {
                cmdPaymode.ClearSelection();

                ListItem pLi = cmdPaymode.Items.FindByValue(ds.Tables[0].Rows[0]["Paymode"].ToString());
                if (pLi != null) pLi.Selected = true;
            }

            if (ds.Tables[0].Rows[0]["Billdate"] != null)
                txtBillDate.Text = Convert.ToDateTime(ds.Tables[0].Rows[0]["Billdate"].ToString()).ToString("dd/MM/yyyy");

            if (ds.Tables[0].Rows[0]["Invoicedate"] != null)
                txtInvoiveDate.Text = Convert.ToDateTime(ds.Tables[0].Rows[0]["Invoicedate"].ToString()).ToString("dd/MM/yyyy");

            if (ds.Tables[0].Rows[0]["SalesReturnReason"] != null)
                txtSRReason.Text = ds.Tables[0].Rows[0]["SalesReturnReason"].ToString();



            if (drpIntTrans.SelectedItem.Text == "YES")
            {
                optionmethod.SelectedIndex = 1;
                rowdcnum.Visible = false;
            }
            else if (ddDeliveryNote.SelectedItem.Text == "YES")
            {
                optionmethod.SelectedIndex = 2;
                rowdcnum.Visible = true;
            }
            else if (drpSalesReturn.SelectedItem.Text == "YES")
            {
                optionmethod.SelectedIndex = 3;
                rowdcnum.Visible = false;
            }
            else
            {
                optionmethod.SelectedIndex = 0;
                rowdcnum.Visible = false;
            }

            drpIntTrans.Enabled = false;
            drpSalesReturn.Enabled = false;
            ddDeliveryNote.Enabled = false;

            //if (txtBillnoSrc.Text == "")
            BindGrid("0", "0");
            //else
            //    BindGrid(txtBillnoSrc.Text, txtTransNo.Text);
            //Accordion1.SelectedIndex = 1;

            hdPurchase.Value = purchaseID.ToString();
            DataSet itemDs = formXml();
            Session["PurchaseProductDs"] = itemDs;
            GrdViewItems.DataSource = itemDs;
            GrdViewItems.DataBind();
            //BindProduct();
            calcSum();
            hdMode.Value = "Edit";

            cmdSave.Visible = false;
            cmdUpdate.Visible = true;
            cmdUpdate.Enabled = true;

            cmdPrint.Enabled = true;


            cmdUpdateProduct.Enabled = false;
            cmdSaveProduct.Enabled = true;
            cmdUpdateProduct.Visible = false;
            //Label3.Visible = false;
            cmdSaveProduct.Visible = true;
            //Label2.Visible = true;

            string dbfileName = sDataSource.Remove(0, sDataSource.LastIndexOf(@"App_Data\") + 9);
            dbfileName = dbfileName.Remove(dbfileName.LastIndexOf(";Persist Security Info"));

            if (bl.CheckForOffline(Server.MapPath("Offline\\" + dbfileName + ".offline")))
            {
                cmdSave.Enabled = false;
                //cmdDelete.Enabled = false;
                cmdUpdate.Enabled = false;
                lnkBtnAdd.Visible = false;
                pnlSearch.Visible = false;
                cmdSaveProduct.Enabled = false;
                GrdViewItems.Columns[12].Visible = false;
                GrdViewItems.Columns[13].Visible = false;
                AddNewProd.Enabled = false;

            }

            updatePnlPurchase.Update();
            ModalPopupPurchase.Show();
            ModalPopupMethod.Show();
        }
        catch (Exception ex)
        {
            TroyLiteExceptionManager.HandleException(ex);
        }
    }

    protected void btnClearFilter_Click(object sender, EventArgs e)
    {
        try
        {
            ClearFilter();
            cmbCategory.SelectedIndex = 0;
        }
        catch (Exception ex)
        {
            TroyLiteExceptionManager.HandleException(ex);
        }
    }

    private void loadCategories()
    {
        //string sDataSource = Server.MapPath(ConfigurationSettings.AppSettings["DataSource"].ToString());
        BusinessLogic bl = new BusinessLogic();
        DataSet ds = new DataSet();
        ds = bl.ListCategory(sDataSource);
        cmbCategory.DataTextField = "CategoryName";
        cmbCategory.DataValueField = "CategoryID";
        cmbCategory.DataSource = ds;
        cmbCategory.DataBind();
    }

    protected void LoadProducts(object sender, EventArgs e)
    {
        //string sDataSource = Server.MapPath(ConfigurationSettings.AppSettings["DataSource"].ToString());
        string CategoryID = cmbCategory.SelectedValue;
        BusinessLogic bl = new BusinessLogic(sDataSource);
        DataSet ds = new DataSet();
        ds = bl.ListProductsForCategoryID(CategoryID);
        cmbProdAdd.Items.Clear();
        cmbProdAdd.DataSource = ds;
        cmbProdAdd.Items.Insert(0, new ListItem("Select Product", "0"));
        cmbProdAdd.DataTextField = "ItemCode";
        cmbProdAdd.DataValueField = "ItemCode";

        cmbProdAdd.DataBind();

        ds = bl.ListModelsForCategoryID(CategoryID);
        cmbModel.Items.Clear();
        cmbModel.DataSource = ds;
        cmbModel.Items.Insert(0, new ListItem("Select Model", "0"));
        cmbModel.DataTextField = "Model";
        cmbModel.DataValueField = "Model";
        cmbModel.DataBind();

        ds = bl.ListBrandsForCategoryID(CategoryID);
        cmbBrand.Items.Clear();
        cmbBrand.DataSource = ds;
        cmbBrand.Items.Insert(0, new ListItem("Select Brand", "0"));
        cmbBrand.DataTextField = "ProductDesc";
        cmbBrand.DataValueField = "ProductDesc";
        cmbBrand.DataBind();

        ds = bl.ListProdNameForCategoryID(CategoryID);
        cmbProdName.Items.Clear();
        cmbProdName.DataSource = ds;
        cmbProdName.Items.Insert(0, new ListItem("Select ItemName", "0"));
        cmbProdName.DataTextField = "ProductName";
        cmbProdName.DataValueField = "ProductName";
        cmbProdName.DataBind();

        LoadForProduct(this, null);
    }

    protected void LoadForProductName(object sender, EventArgs e)
    {
        BusinessLogic bl = new BusinessLogic(sDataSource);
        string prodName = cmbProdName.SelectedValue;
        string CategoryID = cmbCategory.SelectedValue;
        DataSet ds = new DataSet();

        ds = bl.ListProdcutsForProductName(prodName, CategoryID);
        cmbProdAdd.Items.Clear();
        cmbProdAdd.DataSource = ds;
        cmbProdAdd.DataTextField = "ItemCode";
        cmbProdAdd.DataValueField = "ItemCode";
        cmbProdAdd.DataBind();

        ds = bl.ListBrandsForProductName(prodName, CategoryID);
        cmbBrand.Items.Clear();
        cmbBrand.DataSource = ds;
        cmbBrand.DataTextField = "ProductDesc";
        cmbBrand.DataValueField = "ProductDesc";
        cmbBrand.DataBind();

        ds = bl.ListModelsForProductName(prodName, CategoryID);
        cmbModel.Items.Clear();
        cmbModel.DataSource = ds;
        cmbModel.DataTextField = "Model";
        cmbModel.DataValueField = "Model";
        cmbModel.DataBind();

        cmbProdAdd_SelectedIndexChanged(this, null);
    }

    protected void LoadForBrand(object sender, EventArgs e)
    {
        BusinessLogic bl = new BusinessLogic(sDataSource);
        string brand = cmbBrand.SelectedValue;
        string CategoryID = cmbCategory.SelectedValue;
        //DataSet catData = bl.GetProductForId(sDataSource, itemCode);
        //cmbProdAdd.SelectedValue = itemCode;
        //cmbModel.SelectedValue = itemCode;
        DataSet ds = new DataSet();
        ds = bl.ListModelsForBrand(brand, CategoryID);
        cmbModel.Items.Clear();
        cmbModel.DataSource = ds;
        cmbModel.DataTextField = "Model";
        cmbModel.DataValueField = "Model";
        cmbModel.DataBind();

        ds = bl.ListProdcutsForBrand(brand, CategoryID);
        cmbProdAdd.Items.Clear();
        cmbProdAdd.DataSource = ds;
        cmbProdAdd.DataTextField = "ItemCode";
        cmbProdAdd.DataValueField = "ItemCode";
        cmbProdAdd.DataBind();

        ds = bl.ListProdcutNameForBrand(brand, CategoryID);
        cmbProdName.Items.Clear();
        cmbProdName.DataSource = ds;
        cmbProdName.DataTextField = "ProductName";
        cmbProdName.DataValueField = "ProductName";
        cmbProdName.DataBind();

        cmbProdAdd_SelectedIndexChanged(this, null);

    }

    protected void LoadForModel(object sender, EventArgs e)
    {
        BusinessLogic bl = new BusinessLogic(sDataSource);
        string model = cmbModel.SelectedValue;
        string CategoryID = cmbCategory.SelectedValue;
        DataSet ds = new DataSet();

        ds = bl.ListProdcutsForModel(model, CategoryID);
        cmbProdAdd.Items.Clear();
        cmbProdAdd.DataSource = ds;
        cmbProdAdd.DataTextField = "ItemCode";
        cmbProdAdd.DataValueField = "ItemCode";
        cmbProdAdd.DataBind();

        ds = bl.ListBrandsForModel(model, CategoryID);
        cmbBrand.Items.Clear();
        cmbBrand.DataSource = ds;
        cmbBrand.DataTextField = "ProductDesc";
        cmbBrand.DataValueField = "ProductDesc";
        cmbBrand.DataBind();

        ds = bl.ListProductNameForModel(model, CategoryID);
        cmbProdName.Items.Clear();
        cmbProdName.DataSource = ds;
        cmbProdName.DataTextField = "ProductName";
        cmbProdName.DataValueField = "ProductName";
        cmbProdName.DataBind();

        cmbProdAdd_SelectedIndexChanged(this, null);
    }

    protected void LoadForProduct(object sender, EventArgs e)
    {
        //string itemCode = cmbProdAdd.SelectedValue;
        //cmbModel.SelectedValue = itemCode;
        //cmbBrand.SelectedValue = itemCode;
        cmbProdAdd_SelectedIndexChanged(this, null);
    }

    private DataSet formXml()
    {
        int purchaseID = 0;
        //string sDataSource = Server.MapPath(ConfigurationSettings.AppSettings["DataSource"].ToString());
        BusinessLogic bl = new BusinessLogic(sDataSource);
        //DataSet ds = new DataSet();
        purchaseID = Convert.ToInt32(hdPurchase.Value);
        /*March18*/
        DataSet itemDs = null;
        /*March18*/
        DataTable dt;
        DataRow dr;
        DataColumn dc;
        DataSet ds = new DataSet();

        double dTotal = 0;
        double dQty = 0;
        double dRate = 0;
        double dNLP = 0;
        string strRole = string.Empty;
        string roleFlag = string.Empty;
        string strBundles = string.Empty;


        double stock = 0;

        string strItemCode = string.Empty;
        DataSet dsRole;
        ds = bl.GetPurchaseItemsForId(purchaseID);
        if (ds != null)
        {

            dt = new DataTable();

            dc = new DataColumn("PurchaseID");
            dt.Columns.Add(dc);

            dc = new DataColumn("itemCode");
            dt.Columns.Add(dc);

            dc = new DataColumn("ProductName");
            dt.Columns.Add(dc);

            dc = new DataColumn("ProductDesc");
            dt.Columns.Add(dc);

            dc = new DataColumn("PurchaseRate");
            dt.Columns.Add(dc);

            dc = new DataColumn("NLP");
            dt.Columns.Add(dc);

            dc = new DataColumn("Qty");
            dt.Columns.Add(dc);

            dc = new DataColumn("Measure_Unit");
            dt.Columns.Add(dc);

            dc = new DataColumn("Discount");
            dt.Columns.Add(dc);

            dc = new DataColumn("VAT");
            dt.Columns.Add(dc);

            dc = new DataColumn("CST");
            dt.Columns.Add(dc);

            dc = new DataColumn("Discountamt");
            dt.Columns.Add(dc);

            dc = new DataColumn("Roles");
            dt.Columns.Add(dc);

            dc = new DataColumn("IsRole");
            dt.Columns.Add(dc);


            dc = new DataColumn("Total");
            dt.Columns.Add(dc);
            /*March18*/
            itemDs = new DataSet();
            /*March18*/


            itemDs.Tables.Add(dt);
            if (ds.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow dR in ds.Tables[0].Rows)
                {
                    dr = itemDs.Tables[0].NewRow();

                    if (dR["Qty"] != null)
                        dQty = Convert.ToDouble(dR["Qty"]);
                    if (dR["PurchaseRate"] != null)
                        dRate = Convert.ToDouble(dR["PurchaseRate"]);

                    if (dR["NLP"] != null)
                    {
                        if (dR["NLP"].ToString() != "")
                            dNLP = Convert.ToDouble(dR["NLP"]);
                        else
                            dNLP = 0.0;
                    }


                    dTotal = dQty * dRate;
                    if (dR["ItemCode"] != null)
                    {
                        strItemCode = Convert.ToString(dR["ItemCode"]);
                        dr["itemCode"] = strItemCode;
                    }
                    if (dR["PurchaseID"] != null)
                    {
                        purchaseID = Convert.ToInt32(dR["PurchaseID"]);
                        dr["PurchaseID"] = Convert.ToString(purchaseID);
                    }

                    if (dR["ProductName"] != null)
                        dr["ProductName"] = Convert.ToString(dR["ProductName"]);

                    if (dR["ProductDesc"] != null)
                        dr["ProductDesc"] = Convert.ToString(dR["ProductDesc"]);

                    if (dR["Measure_Unit"] != null)
                        dr["Measure_Unit"] = Convert.ToString(dR["Measure_Unit"]);

                    dr["Qty"] = dQty.ToString();

                    dr["PurchaseRate"] = dRate.ToString();

                    dr["NLP"] = dNLP.ToString();

                    if (dR["Discount"] != null)
                        dr["Discount"] = Convert.ToString(dR["Discount"]);

                    if (dR["VAT"] != null)
                        dr["VAT"] = Convert.ToString(dR["VAT"]);

                    if (dR["CST"] != null)
                        dr["CST"] = Convert.ToString(dR["CST"]);

                    if (dR["discamt"] != null)
                        dr["Discountamt"] = Convert.ToString(dR["discamt"]);

                    if (dR["isrole"] != null)
                    {
                        roleFlag = Convert.ToString(dR["isrole"]);
                        dr["IsRole"] = roleFlag;

                    }

                    if (roleFlag == "Y")
                    {
                        strRole = Convert.ToString(dR["RoleID"]);
                    }
                    else
                    {
                        strRole = "NO ROLE";
                    }

                    if (hdStock.Value != "")
                        stock = Convert.ToDouble(hdStock.Value);
                    dr["Roles"] = strRole;
                    dr["Total"] = Convert.ToString(dTotal);
                    itemDs.Tables[0].Rows.Add(dr);
                    strRole = "";
                }
            }


        }
        return itemDs;
    }
    private void BindProduct()
    {
        if (Session["PurchaseProductDs"] != null)
        {
            GrdViewItems.DataSource = (DataSet)Session["PurchaseProductDs"];
            GrdViewItems.DataBind();
        }
    }
    private void BindProductP()
    {
        string filename = string.Empty;
        filename = hdFilename.Value;
        DataSet xmlDs = new DataSet();
        if (File.Exists(Server.MapPath("Reports\\" + filename + "_Product.xml")))
        {
            xmlDs.ReadXml(Server.MapPath("Reports\\" + filename + "_Product.xml"));
            if (xmlDs != null)
            {
                if (xmlDs.Tables.Count > 0)
                {
                    GrdViewItems.DataSource = xmlDs;
                    GrdViewItems.DataBind();
                    calcSum();

                }
                else
                {
                    GrdViewItems.DataSource = null;
                    GrdViewItems.DataBind();
                }
            }
            //File.Delete(Server.MapPath(filename + "_Product.xml")); 
        }

    }
    private void calcSum()
    {
        Double sumAmt = 0;
        //Double sumTAmt = 0;
        Double sumVat = 0;
        Double sumDis = 0;
        Double sumRate = 0;
        Double sumCST = 0;
        Double sumNet = 0;
        DataSet ds = new DataSet();
        //ds.ReadXml(Server.MapPath("Reports\\" + hdFilename.Value + "_product.xml"));

        ds = (DataSet)GrdViewItems.DataSource;
        if (ds != null)
        {
            if (ds.Tables.Count > 0)
            {
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    if (dr["Total"] != null)
                        sumAmt = sumAmt + Convert.ToDouble(GetTotal(Convert.ToDouble(dr["Qty"]), Convert.ToDouble(dr["PurchaseRate"]), Convert.ToDouble(dr["Discount"]), Convert.ToDouble(dr["VAT"]), Convert.ToDouble(dr["CST"]), Convert.ToDouble(dr["Discountamt"])));
                    //sumTAmt = sumTAmt + (Convert.ToDouble(dr["Qty"]) * Convert.ToDouble(dr["PurchaseRate"]));
                    sumDis = sumDis + GetDis();
                    sumVat = sumVat + GetVat();
                    sumCST = sumCST + GetCST();
                    sumRate = sumRate + GetTotalRate();
                }
            }
        }
        /*Start Purchase Loading / Unloading Freight Change - March 16*/
        double dFreight = 0;
        double dLU = 0;
        double sumLUFreight = 0;

        double ddiscamt = 0;
        double ddisc = 0;
        if (txtFreight.Text.Trim() != "")
        {
            dFreight = Convert.ToDouble(txtFreight.Text.Trim());
        }
        if (txtLU.Text.Trim() != "")
        {
            dLU = Convert.ToDouble(txtLU.Text.Trim());
        }
        sumLUFreight = dFreight + dLU;

        if (txtdiscamt.Text.Trim() != "")
        {
            ddiscamt = Convert.ToDouble(txtdiscamt.Text.Trim());
        }
        if (txtdisc.Text.Trim() == "0")
        {
            sumNet = (((sumNet + sumAmt + dFreight + dLU) - ddiscamt)); ;
        }
        else
        {
            if (txtdisc.Text.Trim() == "")
            {
                ddisc = 0;
                sumNet = ((sumNet + sumAmt + dFreight + dLU) - ddiscamt); ;
            }
            else
            {
                ddisc = Convert.ToDouble(txtdisc.Text.Trim());
                sumNet = ((sumNet + sumAmt + dFreight + dLU) - ddiscamt); ;
                sumNet = sumNet -(sumNet * (ddisc / 100)) ;
            }
        }

        
        /*End Purchase Loading / Unloading Freight Change - March 16*/

        lblTotalSum.Text = sumAmt.ToString("#0.00");
        lblTotalDis.Text = sumDis.ToString("#0.00");
        lblDispTotalRate.Text = sumRate.ToString("#0.00");
        lblTotalVAT.Text = sumVat.ToString("#0.00");
        lblTotalCST.Text = sumCST.ToString("#0.00");
        lblNet.Text = sumNet.ToString("#0.00");
        
        /*Start Purchase Loading / Unloading Freight Change - March 16*/
        lblFreight.Text = sumLUFreight.ToString("#0.00");
        /*End Purchase Loading / Unloading Freight Change - March 16*/
    }


    protected void cmdPrint_Click(object sender, EventArgs e)
    {
        try
        {
            Session["purchaseID"] = hdPurchase.Value;
            delFlag.Value = "0";
            deleteFile();
            Response.Redirect("ProductPurchaseBill.aspx?SID=" + hdPurchase.Value.ToString() + "&RT=" + drpSalesReturn.SelectedValue.ToUpper());
        }
        catch (Exception ex)
        {
            TroyLiteExceptionManager.HandleException(ex);
        }
    }

    protected void GrdViewPurchase_RowEditing(object sender, GridViewEditEventArgs e)
    {



    }

    protected void GrdViewPurchase_RowCommand(object sender, GridViewCommandEventArgs e)
    {

    }
    protected void BtnViewDetails_Click(object sender, EventArgs e)
    {

    }
    //public string GetTotal(Decimal qty, Decimal rate, Decimal discount, Decimal VAT)
    //{

    //    Decimal tot = 0;
    //    tot = (qty * rate) - ((qty * rate) * (discount / 100)) + ((qty * rate) * (VAT / 100));
    //    amtTotal = amtTotal + Convert.ToDouble(tot);
    //    disTotal = disTotal + discount; 
    //    rateTotal = rateTotal + rate;
    //    vatTotal = vatTotal + VAT;
    //    hdTotalAmt.Value = amtTotal.ToString("#0.00");
    //    //lblGrandTotal.Text = Convert.ToString(Convert.ToDecimal(tot) +Convert.ToDecimal(hdTotalAmt.Value));
    //    return tot.ToString("#0.00");
    //}

    public string GetTotal(double qty, double rate, double discount, double VAT, double CST, double discamt)
    {
        double dis = 0;
        double vat = 0;
        double cst = 0;
        double tot = 0;
        double disRate = 0;

        double vatat = 0;
        double cstat = 0;

        if ((discamt == 0) && (discount > 0))
        {
            tot = (qty * rate) - ((qty * rate) * (discount / 100)) + (((qty * rate) - ((qty * rate) * (discount / 100))) * (VAT / 100)) + (((qty * rate) - ((qty * rate) * (discount / 100))) * (CST / 100));
        }
        else if ((discamt > 0) && (discount == 0))
        {
            tot = (qty * rate) - (discamt);
            vatat = (tot * (VAT / 100));
            cstat = (tot * (CST / 100));
            tot = tot + vatat + cstat;
        }
        else if ((discamt > 0) && (discount > 0))
        {
            tot = (qty * rate) - ((qty * rate) * (discount / 100)) - (discamt) + ((((qty * rate) - ((qty * rate) * (discount / 100))) - discamt) * (VAT / 100)) + ((((qty * rate) - ((qty * rate) * (discount / 100))) - discamt) * (CST / 100));
        }
        else if ((discamt == 0) && (discount == 0))
        {
            tot = (qty * rate) - ((qty * rate) * (discount / 100)) - (discamt) + ((((qty * rate) - ((qty * rate) * (discount / 100))) - discamt) * (VAT / 100)) + ((((qty * rate) - ((qty * rate) * (discount / 100))) - discamt) * (CST / 100));
        }

        // tot = (qty * rate) - ((qty * rate) * (discount / 100)) + (((qty * rate) - ((qty * rate) * (discount / 100))) * (VAT / 100)) + (((qty * rate) - ((qty * rate) * (discount / 100))) * (CST / 100));

        if ((discamt == 0) && (discount > 0))
        {
            disRate = (qty * rate) - ((qty * rate) * (discount / 100));
        }
        else if ((discamt > 0) && (discount == 0))
        {
            disRate = (qty * rate) - (discamt);
        }
        else if ((discamt > 0) && (discount > 0))
        {
            disRate = (qty * rate) - ((qty * rate) * (discount / 100)) - (discamt);
        }
        else if ((discamt == 0) && (discount == 0))
        {
            disRate = (qty * rate) - ((qty * rate) * (discount / 100)) - (discamt);
        }

        if ((discamt == 0) && (discount > 0))
        {
            dis = ((qty * rate) * (discount / 100));
        }
        else if ((discamt > 0) && (discount == 0))
        {
            dis = ((qty * rate) * (discamt));
        }
        else if ((discamt > 0) && (discount > 0))
        {
            dis = ((qty * rate) * (discount / 100)) + (discamt);
        }
        else if ((discamt == 0) && (discount == 0))
        {
            dis = ((qty * rate) * (discount / 100)) + (discamt);
        }

        vat = (disRate * (VAT / 100));
        cst = (disRate * (CST / 100));
        amtTotal = amtTotal + Convert.ToDouble(tot);
        disTotal = dis;
        rateTotal = rateTotal + rate;
        vatTotal = vat;
        cstTotal = cst;
        disTotalRate = qty * rate;
        hdTotalAmt.Value = amtTotal.ToString("#0.00");
        //lblGrandTotal.Text = Convert.ToString(Convert.ToDecimal(tot) +Convert.ToDecimal(hdTotalAmt.Value));
        return tot.ToString("#0.00");
    }

    public string GetTotalOld(double qty, double rate, double discount, double VAT, double CST)
    {
        double dis = 0;
        double vat = 0;
        double cst = 0;
        double tot = 0;
        double disRate = 0;

        tot = (qty * rate) - ((qty * rate) * (discount / 100)) + (((qty * rate) - ((qty * rate) * (discount / 100))) * (VAT / 100)) + (((qty * rate) - ((qty * rate) * (discount / 100))) * (CST / 100));
        // tot = (qty * rate) - ((qty * rate) * (discount / 100)) + (((qty * rate) - ((qty * rate) * (discount / 100))) * (VAT / 100)) + (((qty * rate) - ((qty * rate) * (discount / 100))) * (CST / 100));
        disRate = (qty * rate) - ((qty * rate) * (discount / 100));
        dis = ((qty * rate) * (discount / 100));

        vat = (disRate * (VAT / 100));
        cst = (disRate * (CST / 100));
        amtTotal = amtTotal + Convert.ToDouble(tot);
        disTotal = dis;
        rateTotal = rateTotal + rate;
        vatTotal = vat;
        cstTotal = cst;
        disTotalRate = qty * rate;
        hdTotalAmt.Value = amtTotal.ToString("#0.00");
        //lblGrandTotal.Text = Convert.ToString(Convert.ToDecimal(tot) +Convert.ToDecimal(hdTotalAmt.Value));
        return tot.ToString("#0.00");
    }

    public double GetTotalRate()
    {
        return disTotalRate;
    }

    public double GetSum()
    {
        return amtTotal;// Convert.ToDouble(hdTotalAmt.Value);
    }
    public double GetDis()
    {
        return disTotal;
    }
    public double GetRate()
    {
        return rateTotal;
    }
    public double GetVat()
    {
        return vatTotal;
    }
    public double GetCST()
    {
        return cstTotal;
    }




    protected void GrdViewItems_RowEditing(object sender, GridViewEditEventArgs e)
    {
        try
        {
            GrdViewItems.EditIndex = e.NewEditIndex;

            if (Session["PurchaseProductDs"] != null)
            {
                DataSet ds = (DataSet)Session["PurchaseProductDs"];
                GrdViewItems.DataSource = ds;
                GrdViewItems.DataBind();
            }
            //BindProduct();
        }
        catch (Exception ex)
        {
            TroyLiteExceptionManager.HandleException(ex);
        }
    }
    protected void GrdViewItems_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
    {
        try
        {
            GrdViewItems.EditIndex = -1;
            if (Session["PurchaseProductDs"] != null)
            {
                DataSet ds = (DataSet)Session["PurchaseProductDs"];
                GrdViewItems.DataSource = ds;
                GrdViewItems.DataBind();
            }
        }
        catch (Exception ex)
        {
            TroyLiteExceptionManager.HandleException(ex);
        }

    }
    protected void drpSalesReturn_SelectedIndexChanged(object sender, EventArgs e)
    {
        try
        {
            if (drpSalesReturn.SelectedItem.Text == "NO")
            {
                rqSalesReturn.Enabled = false;
                rowSalesRet.Visible = false;
                loadSupplier("Sundry Creditors");
            }
            else
            {
                rqSalesReturn.Enabled = true;
                rowSalesRet.Visible = true;
                loadSupplier("Sundry Debtors");
            }
        }
        catch (Exception ex)
        {
            TroyLiteExceptionManager.HandleException(ex);
        }
    }

    protected void ddDeliveryNote_SelectedIndexChanged(object sender, EventArgs e)
    {
        try
        {
            if (ddDeliveryNote.SelectedItem.Text == "NO")
            {
                RequiredFieldValidator2.Enabled = false;
                rowdcnum.Visible = false;
            }
            else
            {
                RequiredFieldValidator2.Enabled = true;
                rowdcnum.Visible = true;
            }
        }
        catch (Exception ex)
        {
            TroyLiteExceptionManager.HandleException(ex);
        }
    }
    protected void cmdPaymode_SelectedIndexChanged(object sender, EventArgs e)
    {
        try
        {
            if (cmdPaymode.SelectedValue == "2")
            {
                pnlBank.Visible = true;
                txtChequeNo.Focus();
            }
            else
            {
                pnlBank.Visible = false;
            }
        }
        catch (Exception ex)
        {
            TroyLiteExceptionManager.HandleException(ex);
        }
    }

    protected void txtBarcode_Populated(object sender, EventArgs e) //Jolo Barcode
    {
        try
        {
            //string sDataSource = Server.MapPath(ConfigurationSettings.AppSettings["DataSource"].ToString());
            BusinessLogic bl = new BusinessLogic(sDataSource);
            bool dupFlag = false;
            DataSet checkDs;
            string itemCode = string.Empty;
            DataSet ds = new DataSet();
            string connection = System.Configuration.ConfigurationManager.ConnectionStrings[Request.Cookies["Company"].Value].ToString();

            itemCode = bl.GetItemCode(connection, this.txtBarcode.Text);

            if ((itemCode == string.Empty) || (itemCode == "0"))
            {
                ScriptManager.RegisterStartupScript(Page, Page.GetType(), Guid.NewGuid().ToString(), "alert('Product not found. Please try again.');", true);
                return;
            }

            cmbProdAdd.SelectedValue = itemCode;

            if (Session["PurchaseProductDs"] != null)
            {
                checkDs = (DataSet)Session["PurchaseProductDs"];

                foreach (DataRow dR in checkDs.Tables[0].Rows)
                {
                    if (dR["itemCode"] != null)
                    {
                        if (dR["itemCode"].ToString().Trim() == itemCode && dR["isRole"].ToString().Trim() != "Y")
                        {
                            dupFlag = true;
                            break;
                        }
                    }
                }

            }

            if (!dupFlag)
            {
                ds = bl.ListProductDetails(itemCode);

                if (ds != null)
                {
                    lblProdNameAdd.Text = Convert.ToString(ds.Tables[0].Rows[0]["productname"]);
                    lblProdDescAdd.Text = Convert.ToString(ds.Tables[0].Rows[0]["productdesc"]) + " - " + Convert.ToString(ds.Tables[0].Rows[0]["model"]);
                    lblDisAdd.Text = Convert.ToString(ds.Tables[0].Rows[0]["Discount"]);
                    lblVATAdd.Text = Convert.ToString(ds.Tables[0].Rows[0]["vat"]);
                    txtNLPAdd.Text = Convert.ToString(ds.Tables[0].Rows[0]["NLP"]);
                    txtRateAdd.Text = Convert.ToString(ds.Tables[0].Rows[0]["Rate"]);
                    hdStock.Value = Convert.ToString(ds.Tables[0].Rows[0]["Stock"]);
                    lblCSTAdd.Text = Convert.ToString(ds.Tables[0].Rows[0]["CST"]);
                    lbldiscamt.Text = "0";
                    if (lblCSTAdd.Text.Trim() == "")
                    {
                        lblCSTAdd.Text = "0";
                    }
                    err.Text = "";
                    txtQtyAdd.Text = "0";
                    hdRole.Value = "N";
                }
                else
                {
                    lblProdNameAdd.Text = "";
                    lblProdDescAdd.Text = "";
                    lblDisAdd.Text = "";
                    lblVATAdd.Text = "";
                    txtRateAdd.Text = "";
                    txtNLPAdd.Text = "";
                    hdStock.Value = "";
                    //err.Text = "Product code is not correct please choose the correct one";
                }
            }
            else
            {
                cmbProdAdd.SelectedIndex = 0;
                ScriptManager.RegisterStartupScript(Page, Page.GetType(), Guid.NewGuid().ToString(), "alert('Item Code is already present')", true);
            }
        }
        catch (Exception ex)
        {
            TroyLiteExceptionManager.HandleException(ex);
        }
    }




    protected void cmbProdAdd_SelectedIndexChanged(object sender, EventArgs e)
    {
        try
        {
            ModalPopupPurchase.Show();
            ModalPopupProduct.Show();
            ModalPopupMethod.Show();
            //string sDataSource = Server.MapPath(ConfigurationSettings.AppSettings["DataSource"].ToString());
            BusinessLogic bl = new BusinessLogic(sDataSource);
            bool dupFlag = false;
            DataSet checkDs;
            string itemCode = string.Empty;
            DataSet ds = new DataSet();
            if (cmbProdAdd.SelectedItem != null)
            {
                btnCancel.Enabled = true;
                itemCode = cmbProdAdd.SelectedItem.Value;


                if (Session["PurchaseProductDs"] != null)
                {
                    checkDs = (DataSet)Session["PurchaseProductDs"];

                    foreach (DataRow dR in checkDs.Tables[0].Rows)
                    {
                        if (dR["itemCode"] != null)
                        {
                            if (dR["itemCode"].ToString().Trim() == itemCode && dR["isRole"].ToString().Trim() != "Y")
                            {
                                dupFlag = true;
                                break;
                            }
                        }
                    }
                }
                if (!dupFlag)
                {
                    ds = bl.ListPurProductDetails(cmbProdAdd.SelectedItem.Value);

                    if (ds != null)
                    {
                        lblProdNameAdd.Text = Convert.ToString(ds.Tables[0].Rows[0]["productname"]);
                        lblProdDescAdd.Text = Convert.ToString(ds.Tables[0].Rows[0]["productdesc"]) + " - " + Convert.ToString(ds.Tables[0].Rows[0]["model"]);
                        if ((optionmethod.SelectedValue == "Purchase") || (optionmethod.SelectedValue == "SalesReturn"))
                        {
                            lblDisAdd.Text = Convert.ToString(ds.Tables[0].Rows[0]["BuyDiscount"]);
                            lblVATAdd.Text = Convert.ToString(ds.Tables[0].Rows[0]["vat"]);
                            lblCSTAdd.Text = Convert.ToString(ds.Tables[0].Rows[0]["CST"]);
                        }
                        else
                        {
                            lblDisAdd.Text = "0";
                            lblVATAdd.Text = "0";
                            lblCSTAdd.Text = "0";
                        }
                        txtRateAdd.Text = Convert.ToString(ds.Tables[0].Rows[0]["BuyRate"]);
                        txtNLPAdd.Text = Convert.ToString(ds.Tables[0].Rows[0]["NLP"]);
                        hdStock.Value = Convert.ToString(ds.Tables[0].Rows[0]["Stock"]);

                        txtstock.Text = Convert.ToString(ds.Tables[0].Rows[0]["Stock"]);
                        txtrol.Text = Convert.ToString(ds.Tables[0].Rows[0]["Rol"]);
                        lblUnitMrmnt.Text = Convert.ToString(ds.Tables[0].Rows[0]["Measure_Unit"]);
                        //lbldiscamt.Text = Convert.ToString(ds.Tables[0].Rows[0]["Discountamt"]);

                        if (lblCSTAdd.Text.Trim() == "")
                        {
                            lblCSTAdd.Text = "0";
                        }
                        err.Text = "";
                        txtQtyAdd.Text = "0";
                        lbldiscamt.Text = "0";

                        txtQtyAdd.Focus();
                        hdRole.Value = "N";

                        DataSet catData = bl.GetProductForId(sDataSource, cmbProdAdd.SelectedItem.Value.Trim());

                        if (catData != null)
                        {
                            if (cmbModel.Items.FindByValue(catData.Tables[0].Rows[0]["Model"].ToString().ToUpper()) != null)
                                cmbModel.SelectedValue = catData.Tables[0].Rows[0]["Model"].ToString().ToUpper();
                            else if (cmbModel.Items.FindByValue(catData.Tables[0].Rows[0]["Model"].ToString().ToLower()) != null)
                                cmbModel.SelectedValue = catData.Tables[0].Rows[0]["Model"].ToString().ToLower();

                            if (cmbBrand.Items.FindByValue(catData.Tables[0].Rows[0]["ProductDesc"].ToString().ToUpper()) != null)
                                cmbBrand.SelectedValue = catData.Tables[0].Rows[0]["ProductDesc"].ToString().ToUpper();
                            else if (cmbBrand.Items.FindByValue(catData.Tables[0].Rows[0]["ProductDesc"].ToString().ToLower()) != null)
                                cmbBrand.SelectedValue = catData.Tables[0].Rows[0]["ProductDesc"].ToString().ToLower();

                            if (cmbProdName.Items.FindByValue(catData.Tables[0].Rows[0]["ProductName"].ToString().ToUpper()) != null)
                                cmbProdName.SelectedValue = catData.Tables[0].Rows[0]["ProductName"].ToString().ToUpper();
                            else if (cmbProdName.Items.FindByValue(catData.Tables[0].Rows[0]["ProductName"].ToString().ToLower()) != null)
                                cmbProdName.SelectedValue = catData.Tables[0].Rows[0]["ProductName"].ToString().ToLower();
                        }

                    }
                    else
                    {
                        lblProdNameAdd.Text = "";
                        lblProdDescAdd.Text = "";
                        lblDisAdd.Text = "";
                        lblVATAdd.Text = "";
                        txtNLPAdd.Text = "";
                        txtRateAdd.Text = "";
                        txtQtyAdd.Text = "";
                        lblCSTAdd.Text = "";
                        hdStock.Value = "";
                        lblUnitMrmnt.Text = "";
                        lbldiscamt.Text = "";
                    }
                }
                else
                {
                    //cmbProdAdd.SelectedIndex = 0;
                    //ScriptManager.RegisterStartupScript(Page, Page.GetType(), Guid.NewGuid().ToString(), "alert('Item Code is already Exists')", true);
                    ds = bl.ListPurProductDetails(cmbProdAdd.SelectedItem.Value);

                    if (ds != null)
                    {
                        lblProdNameAdd.Text = Convert.ToString(ds.Tables[0].Rows[0]["productname"]);
                        lblProdDescAdd.Text = Convert.ToString(ds.Tables[0].Rows[0]["productdesc"]) + " - " + Convert.ToString(ds.Tables[0].Rows[0]["model"]);
                        if ((optionmethod.SelectedValue == "Purchase") || (optionmethod.SelectedValue == "SalesReturn"))
                        {
                            lblDisAdd.Text = Convert.ToString(ds.Tables[0].Rows[0]["BuyDiscount"]);
                            lblVATAdd.Text = Convert.ToString(ds.Tables[0].Rows[0]["Buyvat"]);
                            lblCSTAdd.Text = Convert.ToString(ds.Tables[0].Rows[0]["CST"]);
                        }
                        else
                        {
                            lblDisAdd.Text = "0";
                            lblVATAdd.Text = "0";
                            lblCSTAdd.Text = "0";
                        }
                        txtRateAdd.Text = Convert.ToString(ds.Tables[0].Rows[0]["BuyRate"]);
                        txtNLPAdd.Text = Convert.ToString(ds.Tables[0].Rows[0]["NLP"]);
                        hdStock.Value = Convert.ToString(ds.Tables[0].Rows[0]["Stock"]);

                        txtstock.Text = Convert.ToString(ds.Tables[0].Rows[0]["Stock"]);
                        txtrol.Text = Convert.ToString(ds.Tables[0].Rows[0]["Rol"]);
                        lblUnitMrmnt.Text = Convert.ToString(ds.Tables[0].Rows[0]["Measure_Unit"]);
                        //lbldiscamt.Text = Convert.ToString(ds.Tables[0].Rows[0]["Discountamt"]);

                        if (lblCSTAdd.Text.Trim() == "")
                        {
                            lblCSTAdd.Text = "0";
                        }
                        err.Text = "";
                        txtQtyAdd.Text = "0";
                        lbldiscamt.Text = "0";

                        txtQtyAdd.Focus();
                        hdRole.Value = "N";

                        DataSet catData = bl.GetProductForId(sDataSource, cmbProdAdd.SelectedItem.Value.Trim());

                        if (catData != null)
                        {
                            if (cmbModel.Items.FindByValue(catData.Tables[0].Rows[0]["Model"].ToString().ToUpper()) != null)
                                cmbModel.SelectedValue = catData.Tables[0].Rows[0]["Model"].ToString().ToUpper();
                            else if (cmbModel.Items.FindByValue(catData.Tables[0].Rows[0]["Model"].ToString().ToLower()) != null)
                                cmbModel.SelectedValue = catData.Tables[0].Rows[0]["Model"].ToString().ToLower();

                            if (cmbBrand.Items.FindByValue(catData.Tables[0].Rows[0]["ProductDesc"].ToString().ToUpper()) != null)
                                cmbBrand.SelectedValue = catData.Tables[0].Rows[0]["ProductDesc"].ToString().ToUpper();
                            else if (cmbBrand.Items.FindByValue(catData.Tables[0].Rows[0]["ProductDesc"].ToString().ToLower()) != null)
                                cmbBrand.SelectedValue = catData.Tables[0].Rows[0]["ProductDesc"].ToString().ToLower();

                            if (cmbProdName.Items.FindByValue(catData.Tables[0].Rows[0]["ProductName"].ToString().ToUpper()) != null)
                                cmbProdName.SelectedValue = catData.Tables[0].Rows[0]["ProductName"].ToString().ToUpper();
                            else if (cmbProdName.Items.FindByValue(catData.Tables[0].Rows[0]["ProductName"].ToString().ToLower()) != null)
                                cmbProdName.SelectedValue = catData.Tables[0].Rows[0]["ProductName"].ToString().ToLower();
                        }

                    }
                    else
                    {
                        lblProdNameAdd.Text = "";
                        lblProdDescAdd.Text = "";
                        lblDisAdd.Text = "";
                        lblVATAdd.Text = "";
                        txtNLPAdd.Text = "";
                        txtRateAdd.Text = "";
                        txtQtyAdd.Text = "";
                        lblCSTAdd.Text = "";
                        hdStock.Value = "";
                        lblUnitMrmnt.Text = "";
                        lbldiscamt.Text = "";
                    }
                }
            }
            else
            {
                ClearFilter();
            }

            delFlag.Value = "0";
            updatePnlProduct.Update();
        }
        catch (Exception ex)
        {
            TroyLiteExceptionManager.HandleException(ex);
        }
    }

    private void ClearFilter()
    {
        //cmbCategory.SelectedIndex = 0;
        cmbProdAdd.Items.Clear();
        lblProdDescAdd.Text = "";
        lblProdNameAdd.Text = "";
        cmbBrand.Items.Clear();
        cmbModel.Items.Clear();
        cmbProdName.Items.Clear();
        txtRateAdd.Text = "";
        txtQtyAdd.Text = "";
        txtNLPAdd.Text = "";
        lblDisAdd.Text = "";
        lbldiscamt.Text = "";
        lblVATAdd.Text = "";
        lblCSTAdd.Text = "";
        txtstock.Text = "";
        txtrol.Text = "";
    }

    protected void cmdSaveProduct_Click(object sender, EventArgs e)
    {

        try
        {
            GrdViewItems.Columns[12].Visible = true;
            GrdViewItems.Columns[13].Visible = true;

            string connection = Request.Cookies["Company"].Value;
            BusinessLogic bll = new BusinessLogic();
            string recondate = txtBillDate.Text.Trim();
            string roleFlag = hdRole.Value;
            DataSet dsRole = new DataSet();
            string strRole = string.Empty;
            string strQty = string.Empty;
            string sDiscount = "";
            string sDiscountamt = "";
            string sVat = "";
            string sCST = "";
            Double amt = 0;
            double stock = 0;
            bool dupFlag = false;
            double iQty = 0;
            string itemCode = string.Empty;
            int dotcnt = 0;
            int dotcntQ = 0;
            DataSet ds;
            DataRow dr;
            DataColumn dc;
            DataColumn dcN;
            DataTable dt;
            DataRow drNew;
            string[] arr = strRole.Split(',');
            char[] a1 = txtRateAdd.Text.ToCharArray();
            char[] q1 = txtQtyAdd.Text.ToCharArray();
            string[] arr2 = strRole.Split(',');

            string recondat = txtInvoiveDate.Text.Trim();
            if (!bll.IsValidDate(connection, Convert.ToDateTime(recondat)))
            {

                ScriptManager.RegisterStartupScript(Page, Page.GetType(), Guid.NewGuid().ToString(), "alert('Date is invalid')", true);
                return;
            }


            for (int i = 0; i < a1.Length; i++)
            {
                if (a1[i] == '.')
                    dotcnt++;
            }

            if (txtRateAdd.Text.EndsWith(".") || dotcnt > 1)
            {
                ScriptManager.RegisterStartupScript(Page, Page.GetType(), Guid.NewGuid().ToString(), "alert('Invalid Rate : " + txtRateAdd.Text + "')", true);
                return;
            }

            if (txtRateAdd.Text == "0")
            {
                ScriptManager.RegisterStartupScript(Page, Page.GetType(), Guid.NewGuid().ToString(), "alert('You have entered Product Rate as Zero.')", true);
            }


            if ((optionmethod.SelectedValue == "Purchase") || (optionmethod.SelectedValue == "SalesReturn"))
            {
                if (lblVATAdd.Text == "0")
                {
                    ScriptManager.RegisterStartupScript(Page, Page.GetType(), Guid.NewGuid().ToString(), "alert('Vat Cannot be Zero %.')", true);
                    ModalPopupProduct.Show();
                    return;
                }
            }


            for (int k = 0; k < q1.Length; k++)
            {
                if (q1[k] == '.')
                    dotcntQ++;
            }
            if (txtQtyAdd.Text.EndsWith(".") || dotcntQ > 1)
            {
                ScriptManager.RegisterStartupScript(Page, Page.GetType(), Guid.NewGuid().ToString(), "alert('Invalid Qty. : " + txtQtyAdd.Text + "')", true);
                return;
            }

            if (lblDisAdd.Text.Trim() != "")
                sDiscount = lblDisAdd.Text;
            else
                sDiscount = "0";

            if (lbldiscamt.Text.Trim() != "")
                sDiscountamt = lbldiscamt.Text;
            else
                sDiscountamt = "0";

            if (lblVATAdd.Text.Trim() != "")
                sVat = lblVATAdd.Text;
            else
                sVat = "0";
            if (lblCSTAdd.Text.Trim() != "")
                sCST = lblCSTAdd.Text;
            else
                sCST = "0";


            if (cmbProdAdd.SelectedItem.Value == "0")
            {
                ScriptManager.RegisterStartupScript(Page, Page.GetType(), Guid.NewGuid().ToString(), "alert('Product is Mandatory')", true);
                return;
            }

            BindProduct();

            //ds = (DataSet)GrdViewItems.DataSource;


            if (hdStock.Value != "")
                stock = Convert.ToDouble(hdStock.Value);

            if (txtQtyAdd.Text.Trim() != "")
                iQty = Convert.ToDouble(txtQtyAdd.Text.Trim());

            if (Session["PurchaseProductDs"] == null)
            {

                ds = new DataSet();
                dt = new DataTable();
                dc = new DataColumn("itemCode");
                dt.Columns.Add(dc);
                dc = new DataColumn("PurchaseID");
                dt.Columns.Add(dc);

                dc = new DataColumn("ProductName");
                dt.Columns.Add(dc);

                dc = new DataColumn("ProductDesc");
                dt.Columns.Add(dc);

                dc = new DataColumn("PurchaseRate");
                dt.Columns.Add(dc);

                dc = new DataColumn("NLP");
                dt.Columns.Add(dc);

                dc = new DataColumn("Qty");
                dt.Columns.Add(dc);

                dc = new DataColumn("Measure_Unit");
                dt.Columns.Add(dc);

                dc = new DataColumn("Discount");
                dt.Columns.Add(dc);

                dc = new DataColumn("VAT");
                dt.Columns.Add(dc);

                dc = new DataColumn("CST");
                dt.Columns.Add(dc);

                dc = new DataColumn("Total");
                dt.Columns.Add(dc);

                //dc = new DataColumn("Roles");
                //dt.Columns.Add(dc);

                dc = new DataColumn("DiscountAmt");
                dt.Columns.Add(dc);

                dc = new DataColumn("isRole");
                dt.Columns.Add(dc);
                ds.Tables.Add(dt);



                drNew = dt.NewRow();
                drNew["itemCode"] = Convert.ToString(cmbProdAdd.SelectedValue);
                drNew["PurchaseID"] = hdPurchase.Value;
                drNew["ProductName"] = cmbProdAdd.SelectedItem.Text;
                drNew["ProductDesc"] = cmbProdName.SelectedItem.Value;
                drNew["PurchaseRate"] = txtRateAdd.Text.Trim();
                drNew["NLP"] = txtNLPAdd.Text.Trim();
                drNew["Qty"] = txtQtyAdd.Text.Trim();
                drNew["Measure_Unit"] = lblUnitMrmnt.Text.Trim();
                drNew["Discount"] = sDiscount;


                //drNew["Roles"] = strRole;
                drNew["isRole"] = roleFlag;
                drNew["VAT"] = sVat;
                drNew["CST"] = sCST;
                drNew["Discountamt"] = sDiscountamt;
                drNew["Total"] = GetTotal(Convert.ToDouble(txtQtyAdd.Text.Trim()), Convert.ToDouble(txtRateAdd.Text.Trim()), Convert.ToDouble(lblDisAdd.Text), Convert.ToDouble(lblVATAdd.Text), Convert.ToDouble(lblCSTAdd.Text), Convert.ToDouble(lbldiscamt.Text));


                ds.Tables[0].Rows.Add(drNew);
                Session["PurchaseProductDs"] = ds;
                //ds.WriteXml(Server.MapPath("Reports\\" + hdFilename.Value + "_Product.xml"));
                //BindProduct();
                //ResetProduct();
            }
            else
            {
                ds = (DataSet)Session["PurchaseProductDs"];
                itemCode = cmbProdAdd.SelectedItem.Value;
                foreach (DataRow dR in ds.Tables[0].Rows)
                {
                    if (dR["itemCode"].ToString().Trim() == itemCode && dR["isRole"].ToString() != "Y")
                    {
                        dupFlag = true;
                        break;
                    }
                }
                if (!dupFlag)
                {

                    dr = ds.Tables[0].NewRow();
                    dr["itemCode"] = Convert.ToString(itemCode);
                    dr["PurchaseID"] = hdPurchase.Value;
                    dr["ProductName"] = cmbProdAdd.SelectedItem.Value;
                    dr["ProductDesc"] = cmbProdName.SelectedItem.Value;
                    dr["PurchaseRate"] = txtRateAdd.Text.Trim();
                    dr["NLP"] = txtNLPAdd.Text.Trim();
                    dr["Qty"] = txtQtyAdd.Text.Trim();
                    dr["Measure_Unit"] = lblUnitMrmnt.Text.Trim();
                    dr["Discount"] = sDiscount;
                    //dr["Roles"] = strRole;
                    dr["isRole"] = roleFlag;
                    dr["VAT"] = sVat;
                    dr["CST"] = sCST;
                    dr["Discountamt"] = sDiscountamt;
                    dr["Total"] = GetTotal(Convert.ToDouble(txtQtyAdd.Text.Trim()), Convert.ToDouble(txtRateAdd.Text.Trim()), Convert.ToDouble(lblDisAdd.Text), Convert.ToDouble(lblVATAdd.Text), Convert.ToDouble(lblCSTAdd.Text), Convert.ToDouble(lbldiscamt.Text));

                    ds.Tables[0].Rows.Add(dr);
                    //ds.WriteXml(Server.MapPath("Reports\\" + hdFilename.Value + "_Product.xml"));

                    //BindProduct();
                    //ResetProduct();
                    //delFlag.Value = "0";

                }
                else
                {
                    ScriptManager.RegisterStartupScript(Page, Page.GetType(), Guid.NewGuid().ToString(), "alert('Item Code is already present')", true);
                    //ModalPopupProduct.Show();
                }

            }
            GrdViewItems.DataSource = ds;
            GrdViewItems.DataBind();
            calcSum();
            ResetProduct();

            WholeTotal = Convert.ToDouble(lblNet.Text);

            //txtRole.Text = "";
            //GridView1.DataSource = null;
            //GridView1.DataBind();
            Session["data"] = null;
            //pnlRole.Visible = false;
            txtQtyAdd.Text = "0";
            hdRole.Value = "N";
            updatePnlProduct.Update();
            ModalPopupProduct.Hide();
            UpdatePanel14.Update();
        }
        catch (Exception ex)
        {
            TroyLiteExceptionManager.HandleException(ex);
        }
    }

    private void ResetProduct()
    {
        if (cmbProdAdd.Items.Count > 0)
            cmbProdAdd.SelectedIndex = 0;

        if (cmbModel.Items.Count > 0)
            cmbModel.SelectedIndex = 0;

        if (cmbProdName.Items.Count > 0)
            cmbProdName.SelectedIndex = 0;

        if (cmbBrand.Items.Count > 0)
            cmbBrand.SelectedIndex = 0;

        lblProdNameAdd.Text = "";
        lblProdDescAdd.Text = "";
        lblDisAdd.Text = "";
        lblVATAdd.Text = "";
        lblDisAdd.Text = "";
        lbldiscamt.Text = "";
        lblCSTAdd.Text = "";
        txtQtyAdd.Text = "";
        txtRateAdd.Text = "";
        txtNLPAdd.Text = "";
        lblUnitMrmnt.Text = "";
        txtstock.Text = "";
        txtrol.Text = "";

        foreach (Control control in cmbProdAdd.Controls)
        {
            if (control is HiddenField)
                ((HiddenField)control).Value = "0";
        }


    }

    protected void cmdUpdateProduct_Click(object sender, EventArgs e)
    {
        try
        {
            GrdViewItems.Columns[12].Visible = true;
            GrdViewItems.Columns[13].Visible = true;

            int i;
            i = int.Parse(hdCurrentRow.Value);
            if (lblDisAdd.Text == "")
                lblDisAdd.Text = "0";
            if (lblVATAdd.Text == "")
                lblVATAdd.Text = "0";

            double dis = 0.0;
            double vat = 0.0;
            double cst = 0.0;

            if (lblDisAdd.Text.Trim() == "")
            {
                if (lbldiscamt.Text.Trim() == "")
                {
                    dis = 0;
                }
                else
                {
                    dis = Convert.ToDouble(lbldiscamt.Text);
                }
            }
            else
            {
                dis = Convert.ToDouble(lblDisAdd.Text);
            }

            if (lblVATAdd.Text.Trim() == "")
                vat = 0;
            else
                vat = Convert.ToDouble(lblVATAdd.Text);

            if (lblCSTAdd.Text.Trim() == "")
                cst = 0;
            else
                cst = Convert.ToDouble(lblCSTAdd.Text);

            if (dis < 0 || dis > 100)
            {
                ScriptManager.RegisterStartupScript(Page, Page.GetType(), Guid.NewGuid().ToString(), "alert('Invalid Discount')", true);
                lblDisAdd.Text = "0";
                return;
            }

            if (vat < 0 || vat > 100)
            {
                ScriptManager.RegisterStartupScript(Page, Page.GetType(), Guid.NewGuid().ToString(), "alert('Invalid VAT')", true);
                lblVATAdd.Text = "0";
                return;
            }

            if (cst < 0 || cst > 100)
            {
                ScriptManager.RegisterStartupScript(Page, Page.GetType(), Guid.NewGuid().ToString(), "alert('Invalid CST')", true);
                lblCSTAdd.Text = "0";
                return;
            }

            if (txtRateAdd.Text == "0")
            {
                ScriptManager.RegisterStartupScript(Page, Page.GetType(), Guid.NewGuid().ToString(), "alert('You have entered Product Rate as Zero.')", true);
            }

            if ((optionmethod.SelectedValue == "Purchase") || (optionmethod.SelectedValue == "SalesReturn"))
            {
                if (lblVATAdd.Text == "0")
                {
                    ScriptManager.RegisterStartupScript(Page, Page.GetType(), Guid.NewGuid().ToString(), "alert('Vat Cannot be Zero %.')", true);
                    ModalPopupProduct.Show();
                    return;
                }
            }

            GrdViewItems.EditIndex = -1;
            //BindProduct();
            if (Session["PurchaseProductDs"] != null)
            {
                GrdViewItems.DataSource = (DataSet)Session["PurchaseProductDs"];
                GrdViewItems.DataBind();
                DataSet ds = (DataSet)GrdViewItems.DataSource;
                ds.Tables[0].Rows[i].BeginEdit();
                ds.Tables[0].Rows[i]["Qty"] = txtQtyAdd.Text;
                ds.Tables[0].Rows[i]["PurchaseRate"] = txtRateAdd.Text;
                ds.Tables[0].Rows[i]["NLP"] = txtNLPAdd.Text;
                ds.Tables[0].Rows[i]["Discount"] = lblDisAdd.Text;
                ds.Tables[0].Rows[i]["VAT"] = lblVATAdd.Text;
                ds.Tables[0].Rows[i]["CST"] = lblCSTAdd.Text;
                ds.Tables[0].Rows[i]["Discountamt"] = lbldiscamt.Text;
                ds.Tables[0].Rows[i].EndEdit();
                ds.Tables[0].Rows[i].AcceptChanges();

                GrdViewItems.DataSource = ds;
                GrdViewItems.DataBind();
                //ds.WriteXml(Server.MapPath("Reports\\" + hdFilename.Value + "_Product.xml"));
                //BindProduct();
                calcSum();
                ResetProduct();
                cmbProdAdd.Enabled = true;
                cmdUpdateProduct.Enabled = false;
                cmdUpdateProduct.Visible = false;
                //Label3.Visible = false;
                //cmdCancelProduct.Visible = false;
                cmdSaveProduct.Enabled = true;
                cmdSaveProduct.Visible = true;
                //Label2.Visible = true;
            }

            updatePnlProduct.Update();
            ModalPopupProduct.Hide();

            UpdatePanel14.Update();
        }
        catch (Exception ex)
        {
            TroyLiteExceptionManager.HandleException(ex);
        }
    }

    protected void cmdCancelProduct_Click(object sender, EventArgs e)
    {
        try
        {
            ResetProduct();
            cmbProdAdd.Enabled = true;
            cmdUpdateProduct.Enabled = false;
            cmdSaveProduct.Enabled = true;
            cmdSaveProduct.Visible = true;
            //Label2.Visible = true;
            cmdUpdateProduct.Visible = false;
            //Label3.Visible = false;
            //cmdCancelProduct.Visible = false;
            ModalPopupProduct.Hide();
        }
        catch (Exception ex)
        {
            TroyLiteExceptionManager.HandleException(ex);
        }
    }

    protected void GrdViewItems_RowUpdating(object sender, GridViewUpdateEventArgs e)
    {
        try
        {
            int i;
            i = GrdViewItems.Rows[e.RowIndex].DataItemIndex;
            TextBox txtQtyEd = (TextBox)GrdViewItems.Rows[e.RowIndex].FindControl("txtQty");
            TextBox txtRateEd = (TextBox)GrdViewItems.Rows[e.RowIndex].FindControl("txtRate");
            TextBox txtNLPEd = (TextBox)GrdViewItems.Rows[e.RowIndex].FindControl("txtNLP");
            TextBox txtVatEd = (TextBox)GrdViewItems.Rows[e.RowIndex].FindControl("txtVat");
            TextBox txtDisEd = (TextBox)GrdViewItems.Rows[e.RowIndex].FindControl("txtDiscount");
            TextBox txtCSTEd = (TextBox)GrdViewItems.Rows[e.RowIndex].FindControl("txtCST");
            if (txtDisEd.Text == "")
                txtDisEd.Text = "0";
            if (txtVatEd.Text == "")
                txtVatEd.Text = "0";

            double dis = 0.0;
            double vat = 0.0;
            double cst = 0.0;
            if (txtDisEd.Text.Trim() == "")
                dis = 0;
            else
                dis = Convert.ToDouble(txtDisEd.Text);

            if (txtVatEd.Text.Trim() == "")
                vat = 0;
            else
                vat = Convert.ToDouble(txtVatEd.Text);

            if (txtCSTEd.Text.Trim() == "")
                cst = 0;
            else
                cst = Convert.ToDouble(txtCSTEd.Text);
            if (dis < 0 || dis > 100)
            {
                ScriptManager.RegisterStartupScript(Page, Page.GetType(), Guid.NewGuid().ToString(), "alert('Invalid Discount')", true);
                txtDisEd.Text = "0";
                return;
            }

            if (vat < 0 || vat > 100)
            {
                ScriptManager.RegisterStartupScript(Page, Page.GetType(), Guid.NewGuid().ToString(), "alert('Invalid VAT')", true);
                txtVatEd.Text = "0";
                return;
            }
            if (cst < 0 || cst > 100)
            {
                ScriptManager.RegisterStartupScript(Page, Page.GetType(), Guid.NewGuid().ToString(), "alert('Invalid CST')", true);
                txtCSTEd.Text = "0";
                return;
            }

            GrdViewItems.EditIndex = -1;
            //BindProduct();
            if (Session["PurchaseProductDs"] != null)
            {
                GrdViewItems.DataSource = (DataSet)Session["PurchaseProductDs"];
                GrdViewItems.DataBind();
                DataSet ds = (DataSet)GrdViewItems.DataSource;
                ds.Tables[0].Rows[i]["Qty"] = txtQtyEd.Text;
                ds.Tables[0].Rows[i]["PurchaseRate"] = txtRateEd.Text;
                ds.Tables[0].Rows[i]["NLP"] = txtNLPEd.Text;
                ds.Tables[0].Rows[i]["Discount"] = txtDisEd.Text;
                ds.Tables[0].Rows[i]["VAT"] = txtVatEd.Text;
                ds.Tables[0].Rows[i]["CST"] = txtCSTEd.Text;

                GrdViewItems.DataSource = ds;
                GrdViewItems.DataBind();
                //ds.WriteXml(Server.MapPath("Reports\\" + hdFilename.Value + "_Product.xml"));
                //BindProduct();
                calcSum();
            }
        }
        catch (Exception ex)
        {
            TroyLiteExceptionManager.HandleException(ex);
        }
    }
    protected void GrdViewItems_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        try
        {
            GrdViewItems.PageIndex = e.NewPageIndex;
            BindProduct();
        }
        catch (Exception ex)
        {
            TroyLiteExceptionManager.HandleException(ex);
        }
    }
    protected void GrdViewItems_RowDeleting(object sender, GridViewDeleteEventArgs e)
    {
        try
        {
            if (Session["PurchaseProductDs"] != null)
            {
                GrdViewItems.DataSource = (DataSet)Session["PurchaseProductDs"];
                GrdViewItems.DataBind();
                DataSet ds = (DataSet)GrdViewItems.DataSource;
                ds.Tables[0].Rows[GrdViewItems.Rows[e.RowIndex].DataItemIndex].Delete();
                ds.Tables[0].AcceptChanges();
                GrdViewItems.DataSource = ds;
                GrdViewItems.DataBind();
                /*March 18*/
                Session["PurchaseProductDs"] = ds;
                /*March 18*/
                calcSum();

                UpdatePanel14.Update();
            }
        }
        catch (Exception ex)
        {
            TroyLiteExceptionManager.HandleException(ex);
        }
    }

    protected void GrdViewItems_RowCreated(object sender, GridViewRowEventArgs e)
    {
        try
        { 
            if (e.Row.RowType == DataControlRowType.Pager)
            {
                PresentationUtils.SetPagerButtonStates2(GrdViewItems, e.Row, this);
            }
        }
        catch (Exception ex)
        {
            TroyLiteExceptionManager.HandleException(ex);
        }
    }

    protected void GrdViewItems_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        try
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                if (GrdViewItems.EditIndex == e.Row.RowIndex)
                {
                    CompareValidator cv = new CompareValidator();
                    cv.ID = "cDis";
                    cv.ControlToValidate = "txtDiscount";
                    cv.ValueToCompare = "100";
                    cv.Type = ValidationDataType.Double;
                    cv.Operator = ValidationCompareOperator.LessThanEqual;
                    cv.ErrorMessage = "Invalid Discount";
                    cv.SetFocusOnError = true;
                    e.Row.Cells[5].Controls.Add(cv);

                    CompareValidator cv2 = new CompareValidator();
                    cv2.ID = "cVat";
                    cv2.ControlToValidate = "txtVAT";
                    cv2.ValueToCompare = "100";
                    cv2.Type = ValidationDataType.Double;
                    cv2.Operator = ValidationCompareOperator.LessThanEqual;
                    cv2.ErrorMessage = "Invalid VAT";
                    cv2.SetFocusOnError = true;

                    e.Row.Cells[6].Controls.Add(cv2);
                }
            }
            else if (e.Row.RowType == DataControlRowType.Footer)
            {
                calcSum();
                e.Row.Cells[9].Text = GetCurrencyType() + lblTotalSum.Text;
                e.Row.Cells[8].Text = GetCurrencyType() + lblTotalCST.Text;
                e.Row.Cells[7].Text = GetCurrencyType() + lblTotalVAT.Text;
                e.Row.Cells[6].Text = GetCurrencyType() + lblTotalDis.Text;
                e.Row.Cells[2].Text = GetCurrencyType() + lblDispTotalRate.Text;
            }
        }
        catch (Exception ex)
        {
            TroyLiteExceptionManager.HandleException(ex);
        }
    }

    private string GetCurrencyType()
    {
        if (Session["CurrencyType"] == null)
        {
            DataSet appSettings;
            string currency = string.Empty;

            if (Session["AppSettings"] == null)
            {               
                BusinessLogic bl = new BusinessLogic();
                DataSet ds = bl.GetAppSettings(Request.Cookies["Company"].Value);

                if (ds != null)
                    Session["AppSettings"] = ds;

                appSettings = (DataSet)Session["AppSettings"];

                for (int i = 0; i < appSettings.Tables[0].Rows.Count; i++)
                {
                    if (appSettings.Tables[0].Rows[i]["KEY"].ToString() == "CURRENCY")
                    {
                        currency = appSettings.Tables[0].Rows[i]["KEYVALUE"].ToString();
                        Session["CurrencyType"] = currency;
                    }

                    if (appSettings.Tables[0].Rows[i]["KEY"].ToString() == "BARCODE")
                    {
                        BarCodeRequired = appSettings.Tables[0].Rows[i]["KEYVALUE"].ToString();
                    }
                }
            }

            if ((currency == "INR") || (currency.ToUpper() == "RS"))
            {
                lblDispTotal.Text = "Total (INR)";
                lblDispDisRate.Text = "Discounted Rate (INR)";
                //lblDispIncVAT.Text = "Inclusive VAT (INR)";
                lblDispIncVAT.Text = "VAT (INR)";
                lblDispIncCST.Text = "Inclusive CST (INR)";
                lblDispLoad.Text = "Loading / Unloading / Freight (INR)";
                lblDispGrandTtl.Text = "GRAND Total (INR)";
            }

            if (currency == "GBP")
            {
                lblDispTotal.Text = "Total (£)";
                lblDispDisRate.Text = "Discounted Rate (£)";
                //lblDispIncVAT.Text = "Inclusive VAT (£)";
                lblDispIncVAT.Text = "VAT (£)";
                lblDispIncCST.Text = "Inclusive CST (£)";
                lblDispLoad.Text = "Loading / Unloading / Freight (£)";
                lblDispGrandTtl.Text = "GRAND Total (£)";

            }
        }
        if (Session["CurrencyType"].ToString() == "INR")
        {
            return "Rs";
        }
        else if (Session["CurrencyType"].ToString() == "GBP")
        {
            return "£";
        }
        else
        {
            return string.Empty;
        }

    }

    protected void GrdViewPurchase_RowCreated(object sender, GridViewRowEventArgs e)
    {
        try
        {
            if (e.Row.RowType == DataControlRowType.Pager)
            {
                PresentationUtils.SetPagerButtonStates(GrdViewPurchase, e.Row, this);
            }
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
            GrdViewPurchase.PageIndex = ((DropDownList)sender).SelectedIndex;
            //String strBillno = string.Empty;
            //string strTransNo = string.Empty;

            //if (txtBillnoSrc.Text.Trim() != "")
            //    strBillno = txtBillnoSrc.Text.Trim();
            //else
            //    strBillno = "0";

            //if (txtTransNo.Text.Trim() != "")
            //    strTransNo = txtTransNo.Text.Trim();
            //else
            //    strTransNo = "0";

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
    protected void ddlPageSelector2_SelectedIndexChanged(object sender, EventArgs e)
    {
        try
        {
            GrdViewItems.PageIndex = ((DropDownList)sender).SelectedIndex;
            BindProduct();
        }
        catch (Exception ex)
        {
            TroyLiteExceptionManager.HandleException(ex);
        }
    }
    private void deleteFile()
    {
        if (Session["Filename"] != null)
        {
            string delFilename = Session["Filename"].ToString();
            if (File.Exists(Server.MapPath("Reports\\" + delFilename)))
                File.Delete(Server.MapPath("Reports\\" + delFilename));
        }
    }

    /*
    protected void GrdView1_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
    {
        //GridView1.EditIndex = -1;
        DataSet ds = (DataSet)Session["data"];
        //GridView1.DataSource = ds;
        //GridView1.DataBind();
    }
    protected void btnCLick_Click(object sender, EventArgs e)
    {
        double dblRole = 0.0;
        double dblRate = 0.0;
        DataSet ds = (DataSet)Session["data"];
        DataRow dr = ds.Tables[0].NewRow();
        dr[0] = txtRole.Text;
        //  dr[1] = txtRate.Text;

        ds.Tables[0].Rows.Add(dr);
        //GridView1.DataSource = ds;
        //GridView1.DataBind();

        // txtRate.Text = "";
        if (txtRole.Text.Trim() != "")
        {
            dblRole = Convert.ToDouble(txtRole.Text);
        }
        
        if (txtQtyAdd.Text.Trim() != "")
            txtQtyAdd.Text = Convert.ToString(Convert.ToDouble(txtQtyAdd.Text) + dblRole);
        else
            txtQtyAdd.Text = Convert.ToString(dblRole);

        txtRole.Text = "";

    }
    protected void GrdView1_RowEditing(object sender, GridViewEditEventArgs e)
    {
        //GridView1.EditIndex = e.NewEditIndex;
        DataSet ds = (DataSet)Session["data"];

        //GridView1.DataSource = ds;
        //GridView1.DataBind();

    }
    protected void GrdView1_RowUpdating(object sender, GridViewUpdateEventArgs e)
    {
        int i;
        i = //GridView1.Rows[e.RowIndex].DataItemIndex;


        TextBox txtQty = (TextBox)//GridView1.Rows[e.RowIndex].Cells[1].Controls[0];


        //GridView1.EditIndex = -1;
        DataSet ds1 = (DataSet)Session["data"];

        //GridView1.DataSource = ds1;
        //GridView1.DataBind();

        DataSet ds = (DataSet)//GridView1.DataSource;

        ds.Tables[0].Rows[i]["Qty"] = txtQty.Text;

        //GridView1.DataSource = ds;
        //GridView1.DataBind();
    }

    private void GenerateRoleDs()
    {
        DataSet ds = new DataSet();
        DataTable dt = new DataTable();

        DataColumn dcQty = new DataColumn("Qty");


        dt.Columns.Add(dcQty);


        ds.Tables.Add(dt);
        Session["data"] = ds;
    }
    protected void GrdView1_RowDeleting(object sender, GridViewDeleteEventArgs e)
    {

        DataSet ds = (DataSet)Session["data"];
        double dblRole = 0;
        if (ds.Tables[0].Rows[//GridView1.Rows[e.RowIndex].DataItemIndex]["Qty"] != null)
            if (ds.Tables[0].Rows[//GridView1.Rows[e.RowIndex].DataItemIndex]["Qty"].ToString() != "")
                dblRole = Convert.ToDouble(ds.Tables[0].Rows[//GridView1.Rows[e.RowIndex].DataItemIndex]["Qty"]);

        ds.Tables[0].Rows[//GridView1.Rows[e.RowIndex].DataItemIndex].Delete();
        if (txtQtyAdd.Text.Trim() != "")
            txtQtyAdd.Text = Convert.ToString(Convert.ToDouble(txtQtyAdd.Text) - dblRole);

        txtRole.Text = "";
        //GridView1.DataSource = ds;
        //GridView1.DataBind();


    }
    */
    protected void txtLU_TextChanged(object sender, EventArgs e)
    {
        try
        {
            if (Session["PurchaseProductDs"] != null)
            {
                GrdViewItems.DataSource = (DataSet)Session["PurchaseProductDs"];
                GrdViewItems.DataBind();
                calcSum();
            }
        }
        catch (Exception ex)
        {
            TroyLiteExceptionManager.HandleException(ex);
        }
    }
    protected void txtFreight_TextChanged(object sender, EventArgs e)
    {
        try
        {
            if (Session["PurchaseProductDs"] != null)
            {
                GrdViewItems.DataSource = (DataSet)Session["PurchaseProductDs"];
                GrdViewItems.DataBind();
                calcSum();
            }
        }
        catch (Exception ex)
        {
            TroyLiteExceptionManager.HandleException(ex);
        }
    }


    protected void cmdcat_click(object sender, EventArgs e)
    {
        try
        { 
            Response.Redirect("SupplierInfo.aspx?myname=" + "NEWSUP");
        }
        catch (Exception ex)
        {
            TroyLiteExceptionManager.HandleException(ex);
        }
    }


    protected void CmdProd_Click(object sender, EventArgs e)
    {
        try
        { 
            Response.Redirect("ProductMaster.aspx?myname=" + "NEWPP");
        }
        catch (Exception ex)
        {
            TroyLiteExceptionManager.HandleException(ex);
        }
    }


    private void EmptyRow()
    {

        var ds = new DataSet();
        sDataSource = ConfigurationManager.ConnectionStrings[Request.Cookies["Company"].Value].ToString();

        var dt = new DataTable();

        DataRow drNew;
        DataColumn dc;

        dc = new DataColumn("itemCode");
        dt.Columns.Add(dc);

        dc = new DataColumn("Billno");
        dt.Columns.Add(dc);

        dc = new DataColumn("ProductName");
        dt.Columns.Add(dc);

        dc = new DataColumn("NLP");
        dt.Columns.Add(dc);

        dc = new DataColumn("Qty");
        dt.Columns.Add(dc);

        dc = new DataColumn("PurchaseRate");
        dt.Columns.Add(dc);

        dc = new DataColumn("Measure_unit");
        dt.Columns.Add(dc);

        dc = new DataColumn("Discount");
        dt.Columns.Add(dc);

        dc = new DataColumn("ExecCharge");
        dt.Columns.Add(dc);

        dc = new DataColumn("VAT");
        dt.Columns.Add(dc);

        dc = new DataColumn("CST");
        dt.Columns.Add(dc);

        dc = new DataColumn("DiscountAmt");
        dt.Columns.Add(dc);

        dc = new DataColumn("Roles");
        dt.Columns.Add(dc);

        dc = new DataColumn("IsRole");
        dt.Columns.Add(dc);

        dc = new DataColumn("Total");
        dt.Columns.Add(dc);

        

        dc = new DataColumn("Bundles");
        dt.Columns.Add(dc);

        dc = new DataColumn("Rods");
        dt.Columns.Add(dc);

        

        ds.Tables.Add(dt);
        drNew = dt.NewRow();
        
        string textvalue = null;

        drNew["itemCode"] = string.Empty;
        drNew["Billno"] = "";
        drNew["ProductName"] = string.Empty;
        drNew["NLP"] = string.Empty;
        drNew["Qty"] = Convert.ToDouble(textvalue);
        drNew["Measure_Unit"] = string.Empty;
        drNew["PurchaseRate"] = Convert.ToDouble(textvalue);
        drNew["Discount"] = Convert.ToDouble(textvalue);
        drNew["ExecCharge"] = Convert.ToDouble(textvalue);
        drNew["VAT"] = Convert.ToDouble(textvalue);
        drNew["CST"] = Convert.ToDouble(textvalue);
        drNew["DiscountAmt"] = Convert.ToDouble(textvalue);
        drNew["Roles"] = "";
        drNew["IsRole"] = "N";
        drNew["Total"] = string.Empty;
        
        drNew["Bundles"] = "";
        drNew["Rods"] = "";
        

        ds.Tables[0].Rows.Add(drNew);

        ds.Tables[0].AcceptChanges();

        GrdViewItems.Columns[12].Visible = false;
        GrdViewItems.Columns[13].Visible = false;

        //GrdViewItems.Columns[10].Visible = false;

        GrdViewItems.DataSource = ds;
        GrdViewItems.DataBind();

        GrdViewItems.Rows[0].Cells[2].Text = null;
        GrdViewItems.Rows[0].Cells[4].Text = null;
        GrdViewItems.Rows[0].Cells[3].Text = null;
        GrdViewItems.Rows[0].Cells[5].Text = null;
        GrdViewItems.Rows[0].Cells[7].Text = null;
        GrdViewItems.Rows[0].Cells[9].Text = null;
        GrdViewItems.Rows[0].Cells[10].Text = null;
        GrdViewItems.Rows[0].Cells[8].Text = null;
        GrdViewItems.Rows[0].Cells[6].Text = null;
    }

    protected void txtdiscamt_TextChanged(object sender, EventArgs e)
    {
        try
        {
            if (Session["PurchaseProductDs"] != null)
            {
                GrdViewItems.DataSource = (DataSet)Session["PurchaseProductDs"];
                GrdViewItems.DataBind();
                calcSum();

                //maintotal = Convert.ToDouble(lblNet.Text) - Convert.ToDouble(txtdiscamt.Text);
                //lblNet.Text = maintotal.ToString("#0.00");
            }
        }
        catch (Exception ex)
        {
            TroyLiteExceptionManager.HandleException(ex);
        }
    }

    protected void txtdisc_TextChanged(object sender, EventArgs e)
    {
        try
        {
            double wholetotal2;
            double net = Convert.ToDouble(lblNet.Text);

            if (txtdisc.Text == "")
            {
                double net2 = 0;
            }
            else
            {
                double net2 = Convert.ToDouble(txtdisc.Text);
            }


            if (Session["PurchaseProductDs"] != null)
            {
                GrdViewItems.DataSource = (DataSet)Session["PurchaseProductDs"];
                GrdViewItems.DataBind();
                calcSum();

                //wholetotal2 = net * (net2 / 100);
                //lblNet.Text = wholetotal2.ToString("#0.00");
            }
        }
        catch (Exception ex)
        {
            TroyLiteExceptionManager.HandleException(ex);
        }
    }

    //private void calcwithdisc()
    //{
    //    Double sumTotalAmt = 0;
    //    Double sumdisc = 0;
    //    Double sumDiscamt = 0;

    //    sumTotalAmt = Convert.ToDouble(lblNet.Text) - Convert.ToDouble(txtdiscamt.Text);
    //    lblNet.Text = sumTotalAmt.ToString("#0.00");

    //}

}