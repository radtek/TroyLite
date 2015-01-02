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
using System.Xml;
using System.IO;

public partial class CreateFormula : System.Web.UI.Page
{
    private string sDataSource = string.Empty;

    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            sDataSource = ConfigurationManager.ConnectionStrings[Request.Cookies["Company"].Value].ToString();

            string dbfileName = sDataSource.Remove(0, sDataSource.LastIndexOf(@"App_Data\") + 9);
            dbfileName = dbfileName.Remove(dbfileName.LastIndexOf(";Persist Security Info"));

            if (!Page.IsPostBack)
            {
                GridViewFormula.PageSize = 8;


                BindGrid(string.Empty);
                loadProducts();
                if (Session["TemplateItems"] != null)
                {
                    Session["TemplateItems"] = null;
                }
                //cmdCanel.PostBackUrl = Request.UrlReferrer.AbsoluteUri;
                //Session["Filename"] = "Reports//" + hdFilename.Value + "_template.xml";


                string connection = Request.Cookies["Company"].Value;
                string usernam = Request.Cookies["LoggedUserName"].Value;
                BusinessLogic bl = new BusinessLogic(sDataSource);

                if (bl.CheckUserHaveAdd(usernam, "STKMST"))
                {
                    lnkBtnAdd.Enabled = false;
                    lnkBtnAdd.ToolTip = "You are not allowed to make Add New ";
                }
                else
                {
                    lnkBtnAdd.Enabled = true;
                    lnkBtnAdd.ToolTip = "Click to Add New ";
                }

            }

            BusinessLogic objChk = new BusinessLogic(sDataSource);

            if (objChk.CheckForOffline(Server.MapPath("Offline\\" + dbfileName + ".offline")))
            {
                lnkBtnAdd.Visible = false;
                cmdUpdate.Enabled = false;
                cmdSave.Enabled = false;
            }



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
            BindGrid("");
            //ddCriteria.SelectedIndex = 0;
        }
        catch (Exception ex)
        {
            TroyLiteExceptionManager.HandleException(ex);
        }
    }

    private void loadProducts()
    {
        //string sDataSource = Server.MapPath(ConfigurationSettings.AppSettings["DataSource"].ToString());
        BusinessLogic bl = new BusinessLogic(sDataSource);
        DataSet ds = new DataSet();
        ds = bl.ListProductsIt();
        cmbProdAdd.DataSource = ds;
        cmbProdAdd.DataBind();
        cmbProdAdd.DataTextField = "ProductName";
        cmbProdAdd.DataValueField = "ItemCode";
    }

    private void BindGrid(string strFormName)
    {

        DataSet ds = new DataSet();
        //string sDataSource = Server.MapPath(ConfigurationSettings.AppSettings["DataSource"].ToString());
        BusinessLogic bl = new BusinessLogic(sDataSource);

        ds = bl.GetFormulaForName(strFormName);


        if (ds != null)
        {
            if (ds.Tables[0].Rows.Count > 0)
            {
                GridViewFormula.DataSource = ds.Tables[0].DefaultView;
                GridViewFormula.DataBind();
                PanelBill.Visible = true;
            }
        }
        else
        {
            GridViewFormula.DataSource = null;
            GridViewFormula.DataBind();
            PanelBill.Visible = true;
        }
    }


    protected void btnSearch_Click(object sender, EventArgs e)
    {
        try
        {
            string seach = string.Empty;

            seach = Convert.ToString(txtSearch.Text);

            DataSet ds = new DataSet();
            BusinessLogic bl = new BusinessLogic(sDataSource);
            
            ds = bl.CreateFormulaSearch(seach);
            
            if (ds != null)
            {
                if (ds.Tables[0].Rows.Count > 0)
                {
                    GridViewFormula.DataSource = ds.Tables[0].DefaultView;
                    GridViewFormula.DataBind();
                    PanelBill.Visible = true;
                }
            }
            else
            {
                GridViewFormula.DataSource = null;
                GridViewFormula.DataBind();
                PanelBill.Visible = true;
                
            }


            //BindGrid(BillNo, TransNo);
            ////Accordion1.SelectedIndex = 0;
            //GrdViewItems.DataSource = null;
            //GrdViewItems.DataBind();

            //lblTotalSum.Text = "0";
            //lblTotalDis.Text = "0";
            //lblTotalVAT.Text = "0";
            //lblTotalCST.Text = "0";
            //lblFreight.Text = "0";
            //lblNet.Text = "0";

            ////PanelBill.Visible = true;
            ////PanelCmd.Visible = false;
            ////lnkBtnAdd.Visible = true;

            //Reset();

            //ResetProduct();

            //cmbProdAdd.Enabled = true;
            //cmdUpdateProduct.Enabled = false;
            //cmdSaveProduct.Enabled = true;

            //Session["productDs"] = null;

            //cmdSave.Enabled = true;
            //cmdDelete.Enabled = false;
            //cmdUpdate.Enabled = false;
            //cmdPrint.Enabled = false;
            //errPanel.Visible = false;
            //ErrMsg.Text = "";
        }
        catch (Exception ex)
        {
            TroyLiteExceptionManager.HandleException(ex);
        }
    }

    protected void cmdSaveProduct_Click(object sender, EventArgs e)
    {
        try
        {
            string formulaName = txtFormulaName.Text.Trim().ToUpper();
            string ItemCode = cmbProdAdd.SelectedValue.Trim();
            string Qty = txtQtyAdd.Text;
            string inOut = ddType.SelectedValue;
            BindItemsGrid();
            ModalPopupExtender1.Show();
            DataSet ds = (DataSet)GrdViewItems.DataSource;
            DataRow dr;
            DataColumn dc;

            if (ds == null)
            {
                ds = new DataSet();
                DataTable dt = new DataTable();
                DataRow drNew = dt.NewRow();
                dc = new DataColumn("FormulaId");
                dt.Columns.Add(dc);
                dc = new DataColumn("ProductName");
                dt.Columns.Add(dc);
                dc = new DataColumn("ProductDesc");
                dt.Columns.Add(dc);
                dc = new DataColumn("FormulaName");
                dt.Columns.Add(dc);
                dc = new DataColumn("ItemCode");
                dt.Columns.Add(dc);
                dc = new DataColumn("Qty");
                dt.Columns.Add(dc);
                dc = new DataColumn("InOut");
                dt.Columns.Add(dc);

                drNew["FormulaId"] = hdTempId.Value;
                drNew["ProductDesc"] = lblProdDescAdd.Text;
                drNew["ProductName"] = lblProdNameAdd.Text;
                drNew["FormulaName"] = formulaName;
                drNew["ItemCode"] = ItemCode;
                drNew["Qty"] = Qty;
                drNew["InOut"] = inOut;
                ds.Tables.Add(dt);
                ds.Tables[0].Rows.Add(drNew);
                //ds.WriteXml(Server.MapPath("Reports\\" + hdFilename.Value + "_Formula.xml"));
                Session["TemplateItems"] = ds;
                BindItemsGrid();
                ResetProduct();
            }
            else
            {

                dr = ds.Tables[0].NewRow();

                if (hdMode.Value == "New")
                {
                    hdTempId.Value = (Convert.ToInt32(hdTempId.Value) + 1).ToString();
                }
                else
                {
                    hdTempId.Value = (Convert.ToInt32(hdTempId.Value) - 1).ToString();
                }

                dr["itemCode"] = cmbProdAdd.SelectedValue;
                dr["FormulaId"] = hdTempId.Value;
                dr["ProductName"] = lblProdNameAdd.Text;
                dr["ProductDesc"] = lblProdDescAdd.Text;
                dr["Qty"] = txtQtyAdd.Text.Trim();
                dr["InOut"] = ddType.SelectedValue;

                ds.Tables[0].Rows.Add(dr);
                //ds.WriteXml(Server.MapPath("Reports\\" + hdFilename.Value + "_template.xml"));
                Session["TemplateItems"] = ds;

                BindItemsGrid();
                ResetProduct();

            }

            //BusinessLogic bl = new BusinessLogic(sDataSource);
            //bl.InsertFormulaIteam(formulaName, ItemCode, Qty, inOut);
            //System.Threading.Thread.Sleep(1000);
            //BindItemsGrid();
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
            ModalPopupExtender1.Show();
            txtFormulaName.Enabled = true;
            PanelCmd.Visible = true;
            PanelItems.Visible = true;
            //        salesPanel.Visible = true;
            cmdSaveProduct.Visible = true;
            cmdSave.Enabled = true;
            cmdUpdate.Enabled = false;
            //cmdDelete.Enabled = false;
            //AccordionPane2.Visible = true;
            lnkBtnAdd.Visible = false;
            hdMode.Value = "New";
            Session["TemplateItems"] = null;
            Reset();
            //lblTotalSum.Text = "0";
            ResetProduct();
            //txtBillDate.Text = DateTime.Now.ToShortDateString();
            XmlDocument xDoc = new XmlDocument();

            hdFilename.Value = System.Guid.NewGuid().ToString();

            if (File.Exists(Server.MapPath("Reports\\" + hdFilename.Value + "_template.xml")))
            {
                File.Delete(Server.MapPath("Reports\\" + hdFilename.Value + "_template.xml"));
            }

            GrdViewItems.DataSource = null;
            GrdViewItems.DataBind();
            GrdViewItems.Columns[6].Visible = true;
            cmdSave.Enabled = true;
            //cmdDelete.Enabled = false;
            cmdUpdate.Enabled = false;
            GridViewFormula.Visible = false;
            //MyAccordion.Visible = false;
            cmdUpdate.Visible = false;
            cmdSave.Visible = true;
            prodPanel.Visible = true;
            prodPanel.Visible = true;
            //tabContol.Visible = true;
            Button1.Visible = true;
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
            //string sDataSource = Server.MapPath(ConfigurationSettings.AppSettings["DataSource"].ToString());
            BusinessLogic bl = new BusinessLogic(sDataSource);
            DataSet ds = new DataSet();
            bool dupFlag = false;
            ModalPopupExtender1.Show();
            DataSet checkDs = (DataSet)Session["TemplateItems"];

            if (checkDs != null)
            {
                foreach (DataRow dR in checkDs.Tables[0].Rows)
                {
                    if (dR["itemCode"] != null)
                    {
                        if (dR["itemCode"].ToString().Trim() == cmbProdAdd.SelectedItem.Value.Trim())
                        {
                            dupFlag = true;
                            break;
                        }
                    }
                }
            }

            if (dupFlag)
            {
                cmbProdAdd.SelectedIndex = 0;
                ResetProduct();
                ScriptManager.RegisterStartupScript(Page, Page.GetType(), Guid.NewGuid().ToString(), "alert('Selected Product is already added.')", true);
            }

            if (cmbProdAdd.SelectedIndex != 0)
            {
                ds = bl.ListProductDetails(cmbProdAdd.SelectedItem.Value);
                //string category = lblledgerCategory.Text;
                if (ds != null)
                {
                    lblProdNameAdd.Text = Convert.ToString(ds.Tables[0].Rows[0]["productname"]);
                    lblProdDescAdd.Text = Convert.ToString(ds.Tables[0].Rows[0]["productdesc"]) + " - " + Convert.ToString(ds.Tables[0].Rows[0]["model"]);
                }
                else
                {
                    lblProdNameAdd.Text = "";
                    lblProdDescAdd.Text = "";
                }
            }
            else
            {
                //err.Text = "Select the Product";
            }
        }
        catch (Exception ex)
        {
            TroyLiteExceptionManager.HandleException(ex);
        }
    }

    private void Reset()
    {
        //txtBillno.Text = "";
        //txtBillDate.Text = "";
        //txtFormName.Text = "";
        txtFormulaName.Text = "";

        foreach (Control control in cmbProdAdd.Controls)
        {
            if (control is HiddenField)
                ((HiddenField)control).Value = "0";
        }
        cmbProdAdd.ClearSelection();
        //txtCreditCardNo.Text = ""; // cmbBankName.SelectedItem.Text;
        //BindGrid(txtBillnoSrc.Text);
        //Accordion1.SelectedIndex = 1;
        GrdViewItems.DataSource = null;
        GrdViewItems.DataBind();
    }

    protected void cmdUpdate_Click(object sender, EventArgs e)
    {
        try
        {
            if (Page.IsValid)
            {

                if (GrdViewItems.EditIndex != -1)
                {
                    ScriptManager.RegisterStartupScript(Page, Page.GetType(), Guid.NewGuid().ToString(), "alert('Please Save the product details before Saving Stock Management defination.');", true);
                    return;
                }

                string FormulaName = txtFormulaName.Text.ToUpper();

                BindItemsGrid();
                DataSet ds = (DataSet)GrdViewItems.DataSource;
                ModalPopupExtender1.Show();
                if (ds != null)
                {

                    BusinessLogic bl = new BusinessLogic(sDataSource);

                    int InCount = 0;
                    int OutCount = 0;

                    if (ds != null)
                    {
                        if (ds.Tables.Count > 0)
                        {
                            foreach (DataRow dr in ds.Tables[0].Rows)
                            {
                                if (dr["InOut"].ToString() == "IN")
                                    InCount = InCount + 1;
                                else
                                    OutCount = OutCount + 1;
                            }
                        }
                    }

                    if ((InCount == 0) || (OutCount == 0))
                    {
                        ScriptManager.RegisterStartupScript(Page, Page.GetType(), Guid.NewGuid().ToString(), "alert('Minimum one IN and one OUT products should be added.');", true);
                        return;
                    }
                    bl.UpdateFormulaItem(FormulaName, ds);

                    //string sDataSource = Server.MapPath(ConfigurationSettings.AppSettings["DataSource"].ToString());
                    //BusinessLogic bl = new BusinessLogic(sDataSource);
                    //int billNo = bl.InsertSales(sBilldate, sCustomerID, sCustomerName, sCustomerAddress, sCustomerContact, iPaymode, sCreditCardno, iBank, dTotalAmt, purchaseReturn, prReason, Convert.ToInt32(executive), ds);
                    Reset();
                    //ResetProduct();
                    if (File.Exists(Server.MapPath("Reports\\" + hdFilename.Value + "_template.xml")))
                        File.Delete(Server.MapPath("Reports\\" + hdFilename.Value + "_template.xml"));

                    salesPanel.Visible = false;
                    lnkBtnAdd.Visible = true;
                    PanelBill.Visible = true;
                    PanelCmd.Visible = false;
                    hdMode.Value = "New";
                    //cmdPrint.Enabled = false;
                    //Accordion1.SelectedIndex = 0;
                    deleteFile();
                    BindGrid(string.Empty);
                    GridViewFormula.Visible = true;
                    //ScriptManager.RegisterStartupScript(Page, Page.GetType(), Guid.NewGuid().ToString(), "alert('Sales Details Saved Successfully. Your Bill No. is " + billNo.ToString() + "')", true);
                    //Session["salesID"] = billNo.ToString();
                    //Session["PurchaseReturn"] = purchaseReturn;
                    //Response.Redirect("PrintSalesBill.aspx");
                    GridViewFormula.Visible = true;
                    //MyAccordion.Visible = true;
                    UpdatePanelFormula.Update();
                    ModalPopupExtender1.Hide();
                    UpdatePanel16.Update();
                    //tabContol.Visible = false;
                }
            }
        }
        catch (Exception ex)
        {
            TroyLiteExceptionManager.HandleException(ex);
        }
    }

    protected void cmdSave_Click(object sender, EventArgs e)
    {
        try
        {
            if (Page.IsValid)
            {
                if (GrdViewItems.EditIndex != -1)
                {
                    ScriptManager.RegisterStartupScript(Page, Page.GetType(), Guid.NewGuid().ToString(), "alert('Please Save the product details before Saving Stock Management defination.');", true);
                    return;
                }
                string FormulaName = txtFormulaName.Text.ToUpper();

                BusinessLogic bl = new BusinessLogic(sDataSource);

                string cntB = bl.isDuplicateFormule(FormulaName);
                if (cntB != "")
                {
                    ScriptManager.RegisterStartupScript(Page, Page.GetType(), Guid.NewGuid().ToString(), "alert('Duplicate Definition Name')", true);
                    return;
                }


                BindItemsGrid();
                DataSet ds = (DataSet)GrdViewItems.DataSource;

                int InCount = 0;
                int OutCount = 0;

                if (ds != null)
                {
                    if (ds.Tables.Count > 0)
                    {
                        foreach (DataRow dr in ds.Tables[0].Rows)
                        {
                            if (dr["InOut"].ToString() == "IN")
                                InCount = InCount + 1;
                            else
                                OutCount = OutCount + 1;
                        }
                    }
                }

                if ((InCount == 0) || (OutCount == 0))
                {
                    ScriptManager.RegisterStartupScript(Page, Page.GetType(), Guid.NewGuid().ToString(), "alert('Minimum one IN and one OUT products should be added.');", true);
                    return;
                }


                if (ds != null)
                {

                    //BusinessLogic bl = new BusinessLogic(sDataSource);
                    bl.InsertFormulaItem(FormulaName, ds);

                    //string sDataSource = Server.MapPath(ConfigurationSettings.AppSettings["DataSource"].ToString());
                    //BusinessLogic bl = new BusinessLogic(sDataSource);
                    //int billNo = bl.InsertSales(sBilldate, sCustomerID, sCustomerName, sCustomerAddress, sCustomerContact, iPaymode, sCreditCardno, iBank, dTotalAmt, purchaseReturn, prReason, Convert.ToInt32(executive), ds);
                    Reset();
                    //ResetProduct();
                    if (File.Exists(Server.MapPath("Reports\\" + hdFilename.Value + "_template.xml")))
                        File.Delete(Server.MapPath("Reports\\" + hdFilename.Value + "_template.xml"));

                    salesPanel.Visible = false;
                    lnkBtnAdd.Visible = true;
                    PanelBill.Visible = true;
                    PanelCmd.Visible = false;
                    hdMode.Value = "New";
                    //cmdPrint.Enabled = false;
                    //Accordion1.SelectedIndex = 0;
                    deleteFile();
                    BindGrid(string.Empty);
                    //ScriptManager.RegisterStartupScript(Page, Page.GetType(), Guid.NewGuid().ToString(), "alert('Sales Details Saved Successfully. Your Bill No. is " + billNo.ToString() + "')", true);
                    //Session["salesID"] = billNo.ToString();
                    //Session["PurchaseReturn"] = purchaseReturn;
                    //Response.Redirect("PrintSalesBill.aspx");
                    GridViewFormula.Visible = true;
                    //MyAccordion.Visible = true;
                    //tabContol.Visible = false;
                    UpdatePanelFormula.Update();
                    ModalPopupExtender1.Hide();
                    UpdatePanel16.Update();
                }
            }
        }
        catch (Exception ex)
        {
            TroyLiteExceptionManager.HandleException(ex);
        }
    }

    private void deleteFile()
    {
        Session["TemplateItems"] = null;

        if (Session["FormulaFilename"] != null)
        {
            string delFilename = Session["FormulaFilename"].ToString();
            if (File.Exists(delFilename))
                File.Delete(delFilename);
        }
    }


    protected void GrdViewItems_RowDeleting(object sender, GridViewDeleteEventArgs e)
    {
        try
        {
            /*
            BindItemsGrid();
            DataSet ds = (DataSet)GrdViewItems.DataSource;
            ds.Tables[0].Rows[GrdViewItems.Rows[e.RowIndex].DataItemIndex].Delete();
            ds.WriteXml(Server.MapPath("Reports\\" + hdFilename.Value + "_template.xml"));
            BindItemsGrid();*/
            ModalPopupExtender1.Show();
            if (Session["TemplateItems"] != null)
            {
                DataSet ds = (DataSet)Session["TemplateItems"];
                GridViewRow row = GrdViewItems.Rows[e.RowIndex];
                ds.Tables[0].Rows[GrdViewItems.Rows[e.RowIndex].DataItemIndex].Delete();
                ds.Tables[0].AcceptChanges();
                GrdViewItems.DataSource = ds;
                GrdViewItems.DataBind();
                Session["TemplateItems"] = ds;
            }
        }
        catch (Exception ex)
        {
            TroyLiteExceptionManager.HandleException(ex);
        }
    }
    protected void GrdViewItems_RowEditing(object sender, GridViewEditEventArgs e)
    {
        try
        {
            ModalPopupExtender1.Show();
            GrdViewItems.EditIndex = e.NewEditIndex;
            BindItemsGrid();
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
            if (Page.IsValid)
            {
                ModalPopupExtender1.Show();
                int i;
                i = GrdViewItems.Rows[e.RowIndex].DataItemIndex;

                string iD = GrdViewItems.DataKeys[e.RowIndex].Value.ToString();
                TextBox txtQtyEd = (TextBox)GrdViewItems.Rows[e.RowIndex].FindControl("txtQty");
                Label ddType = (Label)GrdViewItems.Rows[e.RowIndex].FindControl("lblInOut");

                GrdViewItems.EditIndex = -1;
                BindItemsGrid();
                DataSet ds = (DataSet)GrdViewItems.DataSource;

                ds.Tables[0].Rows[i]["Qty"] = txtQtyEd.Text;
                ds.Tables[0].Rows[i]["InOut"] = ddType.Text;

                Session["TemplateItems"] = ds;

                //BusinessLogic bl = new BusinessLogic(sDataSource);
                //bl.UpdateFormulaIteam(iD, txtQtyEd.Text.Trim(), ddType.SelectedValue.Trim());
                //System.Threading.Thread.Sleep(1000);
                //GrdViewItems.EditIndex = -1;
                BindItemsGrid();

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
                PresentationUtils.SetPagerButtonStates(GrdViewItems, e.Row, this);
            }
        }
        catch (Exception ex)
        {
            TroyLiteExceptionManager.HandleException(ex);
        }
    }
    protected void GrdViewItems_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        //if (hdMode.Value == "Edit")
        //{
        //    GrdViewItems.Columns[6].Visible = false;
        //}
    }
    protected void GrdViewItems_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        try
        {
            ModalPopupExtender1.Show();
        }
        catch (Exception ex)
        {
            TroyLiteExceptionManager.HandleException(ex);
        }
    }

    protected void GrdViewSales_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {

    }

    protected void GrdViewSales_SelectedIndexChanged(object sender, EventArgs e)
    {

    }

    protected void GrdViewItems_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
    {
        try
        {
            GrdViewItems.EditIndex = -1;
            BindItemsGrid();
            ModalPopupExtender1.Show();
        }
        catch (Exception ex)
        {
            TroyLiteExceptionManager.HandleException(ex);
        }
    }

    protected void ddlPageSelector2_SelectedIndexChanged(object sender, EventArgs e)
    {

    }


    protected void GridViewFormula_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        try
        {
            GridViewFormula.PageIndex = e.NewPageIndex;
            BindGrid(string.Empty);
        }
        catch (Exception ex)
        {
            TroyLiteExceptionManager.HandleException(ex);
        }
    }

    private void formXml()
    {
        string formName = hdFormula.Value;
        //string sDataSource = Server.MapPath(ConfigurationSettings.AppSettings["DataSource"].ToString());
        BusinessLogic bl = new BusinessLogic(sDataSource);
        //DataSet ds = new DataSet();
        DataSet ds = new DataSet();
        int cnt = 0;
        ds = bl.GetFormulaForID(formName);

        Session["TemplateItems"] = ds;
        /*
        if (ds != null)
        {
            StringWriter sWriter;
            XmlTextWriter reportXMLWriter;
            XmlDocument xmlDoc;
            string filename = string.Empty;
            sWriter = new StringWriter();
            reportXMLWriter = new XmlTextWriter(sWriter);
            reportXMLWriter.Formatting = Formatting.Indented;
            reportXMLWriter.WriteStartDocument();
            reportXMLWriter.WriteStartElement("Formula");

            if (ds.Tables[0].Rows.Count == 0)
            {
                reportXMLWriter.WriteStartElement("FormulaItem");
                reportXMLWriter.WriteElementString("FormulaID", String.Empty);
                reportXMLWriter.WriteElementString("FromulaName", String.Empty);
                reportXMLWriter.WriteElementString("ProductName", String.Empty);
                reportXMLWriter.WriteElementString("ProductDesc", String.Empty);
                reportXMLWriter.WriteElementString("ItemCode", String.Empty);
                reportXMLWriter.WriteElementString("Qty", "0");
                reportXMLWriter.WriteElementString("InOut", String.Empty);

                reportXMLWriter.WriteEndElement();
            }
            else
            {
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    reportXMLWriter.WriteStartElement("FormulaItem");
                    reportXMLWriter.WriteElementString("FormulaID", Convert.ToString(dr["FormulaID"]));
                    reportXMLWriter.WriteElementString("ItemCode", Convert.ToString(dr["ItemCode"]));
                    reportXMLWriter.WriteElementString("FormulaName", Convert.ToString(dr["FormulaName"]));
                    reportXMLWriter.WriteElementString("ProductName", Convert.ToString(dr["ProductName"]));
                    reportXMLWriter.WriteElementString("ProductDesc", Convert.ToString(dr["ProductDesc"]));
                    reportXMLWriter.WriteElementString("Qty", Convert.ToString(dr["Qty"]));
                    reportXMLWriter.WriteElementString("InOut", Convert.ToString(dr["InOut"]));

                    reportXMLWriter.WriteEndElement();
                }
            }
         
            reportXMLWriter.WriteEndElement();
            reportXMLWriter.WriteEndDocument();
            xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(sWriter.ToString());
            filename = hdFilename.Value;
            xmlDoc.Save(Server.MapPath("Reports\\" + filename + "_Formula.xml"));
            Session["FormulaFileName"] = Server.MapPath("Reports\\" + filename + "_Formula.xml");

        }*/

    }

    protected void GridViewFormula_SelectedIndexChanged(object sender, EventArgs e)
    {
        try
        {
            //if (txtFormName.Text == "")
            //BindGrid("");
            //else
            //BindGrid(txtFormName.Text);
            ModalPopupExtender1.Show();
            string formName = GridViewFormula.SelectedDataKey.Value.ToString();
            hdMode.Value = "Edit";
            hdFormula.Value = formName;
            formXml();
            BindItemsGrid();
            cmdSaveProduct.Visible = false;
            GrdViewItems.Columns[6].Visible = false;
            //BusinessLogic bl = new BusinessLogic(sDataSource);
            //DataSet ds = bl.GetFormulaForID(formName);
            //GrdViewItems.DataSource = ds;
            //GrdViewItems.DataBind();
            txtFormulaName.Text = formName;
            txtFormulaName.Enabled = false;
            PanelCmd.Visible = true;
            salesPanel.Visible = true;
            PanelItems.Visible = true;
            PanelCmd.Visible = true;
            cmdSave.Enabled = false;
            cmdUpdate.Enabled = true;
            lnkBtnAdd.Visible = false;
            GridViewFormula.Visible = false;
            //MyAccordion.Visible = false;
            cmdUpdate.Visible = true;
            cmdSave.Visible = false;
            salesPanel.Visible = false;
            prodPanel.Visible = true;
            Button1.Visible = false;
            //tabContol.Visible = true;
            DisableForOffline();
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
            lnkBtnAdd.Visible = false;
            cmdUpdate.Enabled = false;
            cmdSave.Enabled = false;
            GrdViewItems.Columns[5].Visible = false;
        }
    }

    protected void cmdCancel_Click(object sender, EventArgs e)
    {
        try
        {
            deleteFile();
            Reset();
            hdMode.Value = "New";
            lnkBtnAdd.Visible = true;
            salesPanel.Visible = false;
            PanelCmd.Visible = false;
            PanelBill.Visible = true;
            PanelItems.Visible = false;
            GridViewFormula.Visible = true;
            //MyAccordion.Visible = true;
            //tabContol.Visible = false;
            DisableForOffline();
            UpdatePanelFormula.Update();
            ModalPopupExtender1.Hide();
            UpdatePanel16.Update();
        }
        catch (Exception ex)
        {
            TroyLiteExceptionManager.HandleException(ex);
        }
    }

    private void ResetProduct()
    {
        lblProdNameAdd.Text = "";
        lblProdDescAdd.Text = "";

        txtQtyAdd.Text = "";

        foreach (Control control in cmbProdAdd.Controls)
        {
            if (control is HiddenField)
                ((HiddenField)control).Value = "0";
        }
        cmbProdAdd.ClearSelection();

    }

    private void BindItemsGrid()
    {
        string filename = string.Empty;
        filename = hdFilename.Value;
        DataSet xmlDs = new DataSet();
        //if (File.Exists(Server.MapPath("Reports\\" + filename + "_Formula.xml")))
        if (Session["TemplateItems"] != null)
        {
            //xmlDs.ReadXml(Server.MapPath("Reports\\" + filename + "_Formula.xml"));

            xmlDs = (DataSet)Session["TemplateItems"];

            if (xmlDs != null)
            {
                GrdViewItems.DataSource = xmlDs;
                GrdViewItems.DataBind();
            }
            else
            {
                GrdViewItems.DataSource = null;
                GrdViewItems.DataBind();
            }
        }

        //string formName = GridViewFormula.SelectedDataKey.Value.ToString();
        //BusinessLogic bl = new BusinessLogic(sDataSource);
        //DataSet ds = bl.GetFormulaForID(formName);
        //GrdViewItems.DataSource = ds;
        //GrdViewItems.DataBind();
    }

    protected void GridViewFormula_RowCreated(object sender, GridViewRowEventArgs e)
    {
        try
        {
            if (e.Row.RowType == DataControlRowType.Pager)
            {
                PresentationUtils.SetPagerButtonStates(GridViewFormula, e.Row, this);
            }
        }
        catch (Exception ex)
        {
            TroyLiteExceptionManager.HandleException(ex);
        }
    }
    protected void GridViewFormula_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        try
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                BusinessLogic bl = new BusinessLogic(sDataSource);
                string connection = Request.Cookies["Company"].Value;
                string usernam = Request.Cookies["LoggedUserName"].Value;

                if (bl.CheckUserHaveEdit(usernam, "STKMST"))
                {
                    ((ImageButton)e.Row.FindControl("btnEdit")).Visible = false;
                    ((ImageButton)e.Row.FindControl("btnEditDisabled")).Visible = true;
                }
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
            GridViewFormula.PageIndex = ((DropDownList)sender).SelectedIndex;
            String strBillno = string.Empty;
            string strTransNo = string.Empty;


            BindGrid(string.Empty);
        }
        catch (Exception ex)
        {
            TroyLiteExceptionManager.HandleException(ex);
        }
    }
    protected void Button1_Click(object sender, EventArgs e)
    {
        try
        {
            salesPanel.Visible = true;
            ModalPopupExtender1.Show();
        }
        catch (Exception ex)
        {
            TroyLiteExceptionManager.HandleException(ex);
        }
    }


}
