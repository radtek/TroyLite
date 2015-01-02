using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;
using DataAccessLayer;
using System.Text;
using System.Collections;
using System.Globalization;
using System.IO;
using System.Xml;
using System.Data.OleDb;
using System.Linq;

/// <summary>
/// Summary description for LeadBusinessLogic
/// </summary>
public class LeadBusinessLogic : BaseLogic
{


	public LeadBusinessLogic()
	{
		//
		// TODO: Add constructor logic here
		//
	}

    public LeadBusinessLogic(string con) : base(con)
    {
        //
        // TODO: Add constructor logic here
        //
    }

    public DataSet ListLeadContact(string LeadID)
    {
        DBManager manager = new DBManager(DataProvider.OleDb);
        manager.ConnectionString = CreateConnectionString(this.ConnectionString);
        DataSet ds = new DataSet();
        string dbQry = string.Empty;

        dbQry = "select ContactRefID,ContactDate,ContactSummary From tblLeadContact Where LeadID=" + LeadID;
        
        try
        {
            manager.Open();
            ds = manager.ExecuteDataSet(CommandType.Text, dbQry);

            if (ds.Tables[0].Rows.Count > 0)
                return ds;
            else
                return null;
        }
        catch (Exception ex)
        {
            throw ex;
        }
        finally
        {
            manager.Dispose();
        }

    }

    public DataSet GetLeadMasterDetails(string LeadID)
    {
        DBManager manager = new DBManager(DataProvider.OleDb);
        manager.ConnectionString = CreateConnectionString(this.ConnectionString);
        DataSet ds = new DataSet();
        string dbQry = string.Empty;

        dbQry = "select LeadID,CreationDate, ProspectCustName, Address,Mobile,Landline,Email,ModeOfContact,PersonalResponsible,BusinessType,Branch,Status,LastCompletedAction,NextAction,Category,Information1,Information2,Information3,Information4,Information5,callbackflag,Callbackdate From tblLeadMaster Where LeadID=" + LeadID;

        try
        {
            manager.Open();
            ds = manager.ExecuteDataSet(CommandType.Text, dbQry);

            if (ds.Tables[0].Rows.Count > 0)
                return ds;
            else
                return null;
        }
        catch (Exception ex)
        {
            throw ex;
        }
        finally
        {
            manager.Dispose();
        }

    }

    public DataSet GetLeadContacts(string LeadID)
    {
        DBManager manager = new DBManager(DataProvider.OleDb);
        manager.ConnectionString = CreateConnectionString(this.ConnectionString);
        DataSet ds = new DataSet();
        string dbQry = string.Empty;

        dbQry = "select ContactRefID,ContactedDate,ContactSummary,callbackdate,callbackflag From tblLeadContact Where LeadID=" + LeadID;

        try
        {
            manager.Open();
            ds = manager.ExecuteDataSet(CommandType.Text, dbQry);

            if (ds.Tables[0].Rows.Count > 0)
                return ds;
            else
                return null;
        }
        catch (Exception ex)
        {
            throw ex;
        }
        finally
        {
            manager.Dispose();
        }

    }

    public DataSet ListLeadMaster(string connection, string txtSearch, string dropDown)
    {
        DBManager manager = new DBManager(DataProvider.OleDb);
        manager.ConnectionString = CreateConnectionString(connection);
        DataSet ds = new DataSet();
        string dbQry = string.Empty;

        dbQry = "select LeadID,CreationDate, ProspectCustName, Address,Mobile,Landline,Email,ModeOfContact,PersonalResponsible,BusinessType,Branch,Status,LastCompletedAction,NextAction,Category,callbackflag,Callbackdate from tblLeadMaster Where LeadID > 0 ";

        if (txtSearch == "" || txtSearch == null)
        {

        }
        else
        {
            if (dropDown == "CreationDate")
            {
                dbQry = dbQry + " AND CreationDate = #" + DateTime.Parse(txtSearch.ToString()).ToString("MM/dd/yyyy") + "#";
            }
            else if (dropDown == "CustomerName")
            {
                dbQry = dbQry + " AND ProspectCustName like '%" + txtSearch + "%'";
            }
            else if (dropDown == "LeadID")
            {
                dbQry = dbQry + " AND LeadID = " + txtSearch + " ";
            }
            else if (dropDown == "Status")
            {
                dbQry = dbQry + " AND Status like '%" + txtSearch + "%'";
            }
            else if (dropDown == "Mobile")
            {
                dbQry = dbQry + " AND Mobile like '%" + txtSearch + "%'";
            }
            else if (dropDown == "LastCompletedAction")
            {
                dbQry = dbQry + " AND LastCompletedAction like '%" + txtSearch + "%'";
            }
            else if (dropDown == "NextAction")
            {
                dbQry = dbQry + " AND NextAction like '%" + txtSearch + "%'";
            }
            else if (dropDown == "Category")
            {
                dbQry = dbQry + " AND Category like '%" + txtSearch + "%'";
            }
            else if (dropDown == "BusinessType")
            {
                dbQry = dbQry + " AND BusinessType like '%" + txtSearch + "%'";
            }
            else if (dropDown == "Branch")
            {
                dbQry = dbQry + " AND Branch like '%" + txtSearch + "%'";
            }
        }

        dbQry = dbQry + " Order By LeadID, CreationDate Desc";

        try
        {
            manager.Open();
            ds = manager.ExecuteDataSet(CommandType.Text, dbQry);

            if (ds.Tables[0].Rows.Count > 0)
                return ds;
            else
                return null;
        }
        catch (Exception ex)
        {
            throw ex;
        }
        finally
        {
            manager.Dispose();
        }
    }

    public void DeleteLeadContact(string refID)
    {
        DBManager manager = new DBManager(DataProvider.OleDb);
        manager.ConnectionString = CreateConnectionString(this.ConnectionString); // System.Configuration.ConfigurationManager.ConnectionStrings["ACCSYS"].ToString();

        DataSet roleDs = new DataSet();

        string dbQry = string.Empty;
        string sQry = string.Empty;
        try
        {
            manager.Open();
            manager.ProviderType = DataProvider.OleDb;

            manager.BeginTransaction();

            dbQry = string.Format("Delete From tblLeadContact Where ContactRefID={0}", refID);

            manager.ExecuteNonQuery(CommandType.Text, dbQry);

            manager.CommitTransaction();

        }
        catch (Exception ex)
        {
            throw ex;
        }
        finally
        {
            manager.Dispose();
        }
    }

    public void DeleteLeadMaster(string leadID, string userID)
    {
        DBManager manager = new DBManager(DataProvider.OleDb);
        manager.ConnectionString = CreateConnectionString(this.ConnectionString); // System.Configuration.ConfigurationManager.ConnectionStrings["ACCSYS"].ToString();

        DataSet roleDs = new DataSet();

        string dbQry = string.Empty;
        string sQry = string.Empty;


        string sAuditStr = string.Empty;
        string dbQryData = string.Empty;
        DataSet dsOld = new DataSet();
        int OldTransNo = 0;
        string Olddcreationdate = string.Empty;
        string OldCustomer = string.Empty;
        string OldContact = string.Empty;

        try
        {
            manager.Open();
            manager.ProviderType = DataProvider.OleDb;

            manager.BeginTransaction();




            dbQryData = "select LeadID,CreationDate, ProspectCustName, Address,Mobile,Landline,Email,ModeOfContact,PersonalResponsible,BusinessType,Branch,Status,LastCompletedAction,NextAction,Category from tblLeadMaster Where LeadID =" + leadID;
            dsOld = manager.ExecuteDataSet(CommandType.Text, dbQryData.ToString());
            if (dsOld != null)
            {
                if (dsOld.Tables[0].Rows.Count > 0)
                {
                    if (dsOld.Tables[0].Rows[0]["LeadID"] != null)
                    {
                        if (dsOld.Tables[0].Rows[0]["LeadID"].ToString() != string.Empty)
                        {
                            OldTransNo = Convert.ToInt32(dsOld.Tables[0].Rows[0]["LeadID"].ToString());
                            Olddcreationdate = dsOld.Tables[0].Rows[0]["CreationDate"].ToString();
                            OldCustomer = dsOld.Tables[0].Rows[0]["ProspectCustName"].ToString();
                            OldContact = dsOld.Tables[0].Rows[0]["ModeOfContact"].ToString();
                        }
                    }
                }
            }




            dbQry = string.Format("Delete From tblLeadContact Where LeadID={0}", leadID);

            manager.ExecuteNonQuery(CommandType.Text, dbQry);

            dbQry = string.Format("Delete From tblLeadMaster Where LeadID={0}", leadID);

            manager.ExecuteNonQuery(CommandType.Text, dbQry);



            sAuditStr = "Lead Management Transaction: " + OldTransNo + " got deleted old Record Details : Customer=" + OldCustomer + ",Mode of contact=" + OldContact + ",CreationDate=" + Olddcreationdate + ", DateTime:" + DateTime.Now.ToString() + ", User:" + userID;
            dbQry = string.Format("INSERT INTO  tblAudit(Description,Command) VALUES('{0}','{1}')", sAuditStr, "Delete");
            manager.ExecuteNonQuery(CommandType.Text, dbQry);



            manager.CommitTransaction();

        }
        catch (Exception ex)
        {
            throw ex;
        }
        finally
        {
            manager.Dispose();
        }
    }


    public DataSet GetDropdownList(string connection, string type)
    {
        DBManager manager = new DBManager(DataProvider.OleDb);
        if (connection.IndexOf("Provider=Microsoft.Jet.OLEDB.4.0;") > -1)
            manager.ConnectionString = CreateConnectionString(connection);
        else
            manager.ConnectionString = CreateConnectionString(connection);

        DataSet ds = new DataSet();
        string dbQry = string.Empty;

        try
        {
            //dbQry = string.Format("select LedgerId, LedgerName from tblLedger inner join tblGroups on tblGroups.GroupID = tblLedger.GroupID Where tblGroups.GroupName IN ('{0}','{1}') Order By LedgerName Asc ", "Sundry Debtors", "Sundry Creditors");
            dbQry = string.Format("select TextValue, TextValue from tblLeadReferences Where Type = '"+ type +"' Order By 1");
            manager.Open();
            ds = manager.ExecuteDataSet(CommandType.Text, dbQry);

            if (ds.Tables[0].Rows.Count > 0)
                return ds;
            else
                return null;
        }
        catch (Exception ex)
        {
            throw ex;
        }
        finally
        {
            manager.Dispose();
        }

    }

    //public void AddUpdateLeadMasterold2(int LeadID, string creationDate, string prospectCustomer, string address, string mobile, string landline, string email, string modeOfContact, string personalResponsible, string businessType, string branch, string status, string LastCompletedAction, string nextAction, string category, DataSet dsLeadContact)
    //{
    //    DBManager manager = new DBManager(DataProvider.OleDb);
    //    manager.ConnectionString = CreateConnectionString(this.ConnectionString);
    //    DataSet ds = new DataSet();
    //    string dbQry = string.Empty;

    //    try
    //    {

    //        manager.Open();
    //        manager.ProviderType = DataProvider.OleDb;

    //        manager.BeginTransaction();

    //        if (LeadID < 1)
    //        {
    //            dbQry = string.Format("INSERT INTO tblLeadMaster(CreationDate,ProspectCustName,Address,Mobile,Landline,Email,ModeOfContact,PersonalResponsible,BusinessType,Branch,Status,LastCompletedAction,NextAction,Category) VALUES(Format('{0}', 'dd/mm/yyyy'),'{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}','{11}','{12}','{13}')",
    //                   creationDate.ToShortDateString(), prospectCustomer, address, mobile, landline, email, modeOfContact, personalResponsible, businessType, branch, status, LastCompletedAction, nextAction, category);

    //            manager.ExecuteNonQuery(CommandType.Text, dbQry);

    //            LeadID = (Int32)manager.ExecuteScalar(CommandType.Text, "SELECT MAX(LeadID) FROM tblLeadMaster");
    //            LeadID += 1;

    //        }
    //        else
    //        {
    //            dbQry = string.Format("Update tblLeadMaster Set CreationDate='{0}',ProspectCustName='{1}',Address='{2}',Mobile='{3}',Landline='{4}',Email='{5}',ModeOfContact='{6}',PersonalResponsible='{7}',BusinessType='{8}',Branch='{9}',Status='{10}',LastCompletedAction='{11}',NextAction='{12}',Category='{13}' Where LeadID={14}", creationDate.ToShortDateString(), prospectCustomer, address, mobile, landline, email, modeOfContact, personalResponsible, businessType, branch, status, LastCompletedAction, nextAction, category, LeadID);
    //            manager.ExecuteNonQuery(CommandType.Text, dbQry);
    //        }

    //        dbQry = string.Format("Delete From tblLeadContact Where LeadID={0}", LeadID);
    //        manager.ExecuteNonQuery(CommandType.Text, dbQry);

    //        if (dsLeadContact.Tables[0].Rows.Count > 0)
    //        {
    //            foreach (DataRow dr in dsLeadContact.Tables[0].Rows)
    //            {
    //                dbQry = string.Format("INSERT INTO tblLeadContact(LeadID,ContactedDate,ContactSummary) VALUES({0},Format('{1}', 'dd/mm/yyyy'),'{2}')", LeadID, dr["ContactedDate"].ToString(), dr["ContactSummary"].ToString());
    //                manager.ExecuteNonQuery(CommandType.Text, dbQry);
    //            }

    //        }

    //        manager.CommitTransaction();
    //    }
    //    catch (Exception ex)
    //    {
    //        throw ex;
    //    }
    //    finally
    //    {
    //        manager.Dispose();
    //    }

    //}

    public void AddUpdateLeadMaster(int LeadID, DateTime creationDate, string prospectCustomer, string address, string mobile, string landline, string email, string modeOfContact, string personalResponsible, string businessType, string branch, string status, string LastCompletedAction, string nextAction, string category, DataSet dsLeadContact, string info1, string info2, string info3, string info4, string info5, string callbackflag, string callbackdate)
    {
        DBManager manager = new DBManager(DataProvider.OleDb);
        manager.ConnectionString = CreateConnectionString(this.ConnectionString);
        DataSet ds = new DataSet();
        string dbQry = string.Empty;
        int LeadIDD = 0;

        try
        {

            manager.Open();
            manager.ProviderType = DataProvider.OleDb;

            manager.BeginTransaction();

            dbQry = string.Format("INSERT INTO tblLeadMaster(CreationDate,ProspectCustName,Address,Mobile,Landline,Email,ModeOfContact,PersonalResponsible,BusinessType,Branch,Status,LastCompletedAction,NextAction,Category,Information1,Information2,Information3,Information4,Information5,callbackflag,Callbackdate) VALUES(Format('{0}', 'dd/mm/yyyy'),'{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}','{11}','{12}','{13}','{14}','{15}','{16}','{17}','{18}','{19}','{20}')",
                    creationDate.ToShortDateString(), prospectCustomer, address, mobile, landline, email, modeOfContact, personalResponsible, businessType, branch, status, LastCompletedAction, nextAction, category, info1, info2, info3, info4, info5,callbackflag, callbackdate);

            manager.ExecuteNonQuery(CommandType.Text, dbQry);

            LeadIDD = (Int32)manager.ExecuteScalar(CommandType.Text, "SELECT MAX(LeadID) FROM tblLeadMaster");
           
            if (dsLeadContact.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow dr in dsLeadContact.Tables[0].Rows)
                {
                    dbQry = string.Format("INSERT INTO tblLeadContact(LeadID,ContactedDate,ContactSummary,callbackflag,callbackdate) VALUES({0},Format('{1}', 'dd/mm/yyyy'),'{2}','{3}','{4}')", LeadIDD, dr["ContactedDate"].ToString(), dr["ContactSummary"].ToString(), dr["Callbackflag"].ToString(), dr["CallbackDate"].ToString());
                    manager.ExecuteNonQuery(CommandType.Text, dbQry);
                }
            }

            manager.CommitTransaction();
        }
        catch (Exception ex)
        {
            throw ex;
        }
        finally
        {
            manager.Dispose();
        }

    }

    public void UpdateLeadMaster(int LeadID, DateTime creationDate, string prospectCustomer, string address, string mobile, string landline, string email, string modeOfContact, string personalResponsible, string businessType, string branch, string status, string LastCompletedAction, string nextAction, string category, DataSet dsLeadContact, string userID, string info1, string info2, string info3, string info4, string info5, string callbackflag, string callbackdate)
    {
        DBManager manager = new DBManager(DataProvider.OleDb);
        manager.ConnectionString = CreateConnectionString(this.ConnectionString);
        DataSet ds = new DataSet();
        string dbQry = string.Empty;
        string sAuditStr = string.Empty;

        string dbQryData = string.Empty;
        DataSet dsOld = new DataSet();
        int OldTransNo = 0;
        string Olddcreationdate = string.Empty;
        string OldCustomer = string.Empty;
        string OldContact = string.Empty;



        try
        {

            manager.Open();
            manager.ProviderType = DataProvider.OleDb;

            manager.BeginTransaction();

            dbQryData = "select LeadID,CreationDate, ProspectCustName, Address,Mobile,Landline,Email,ModeOfContact,PersonalResponsible,BusinessType,Branch,Status,LastCompletedAction,NextAction,Category from tblLeadMaster Where LeadID =" + LeadID;
            dsOld = manager.ExecuteDataSet(CommandType.Text, dbQryData.ToString());
            if (dsOld != null)
            {
                if (dsOld.Tables[0].Rows.Count > 0)
                {
                    if (dsOld.Tables[0].Rows[0]["LeadID"] != null)
                    {
                        if (dsOld.Tables[0].Rows[0]["LeadID"].ToString() != string.Empty)
                        {
                            OldTransNo = Convert.ToInt32(dsOld.Tables[0].Rows[0]["LeadID"].ToString());
                            Olddcreationdate = dsOld.Tables[0].Rows[0]["CreationDate"].ToString();
                            OldCustomer = dsOld.Tables[0].Rows[0]["ProspectCustName"].ToString();
                            OldContact = dsOld.Tables[0].Rows[0]["ModeOfContact"].ToString();
                        }
                    }
                }
            }



            //if (LeadID < 1)
            //{
            //    dbQry = string.Format("INSERT INTO tblLeadMaster(CreationDate,ProspectCustName,Address,Mobile,Landline,Email,ModeOfContact,PersonalResponsible,BusinessType,Branch,Status,LastCompletedAction,NextAction,Category,Information1,Information2,Information3,Information4,Information5) VALUES(Format('{0}', 'dd/mm/yyyy'),'{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}','{11}','{12}','{13}','{14}','{15}','{16}','{17}','{18}')",
            //           creationDate.ToShortDateString(), prospectCustomer, address, mobile, landline, email, modeOfContact, personalResponsible, businessType, branch, status, LastCompletedAction, nextAction, category, info1, info2, info3, info4, info5);

            //    manager.ExecuteNonQuery(CommandType.Text, dbQry);

            //    LeadID = (Int32)manager.ExecuteScalar(CommandType.Text, "SELECT MAX(LeadID) FROM tblLeadMaster");
            //    //LeadID += 1;
            //}
            //else
            //{
            dbQry = string.Format("Update tblLeadMaster Set CreationDate=Format('{0}', 'dd/mm/yyyy'),ProspectCustName='{1}',Address='{2}',Mobile='{3}',Landline='{4}',Email='{5}',ModeOfContact='{6}',PersonalResponsible='{7}',BusinessType='{8}',Branch='{9}',Status='{10}',LastCompletedAction='{11}',NextAction='{12}',Category='{13}', Information1='{15}', Information2='{16}', Information3='{17}', Information4='{18}', Information5='{19}',callbackflag='{20}',Callbackdate='{21}'  Where LeadID={14}", creationDate.ToShortDateString(), prospectCustomer, address, mobile, landline, email, modeOfContact, personalResponsible, businessType, branch, status, LastCompletedAction, nextAction, category, LeadID, info1, info2, info3, info4, info5, callbackflag, callbackdate);
                manager.ExecuteNonQuery(CommandType.Text, dbQry);
            //}

            dbQry = string.Format("Delete From tblLeadContact Where LeadID={0}", LeadID);
            manager.ExecuteNonQuery(CommandType.Text, dbQry);

            if (dsLeadContact.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow dr in dsLeadContact.Tables[0].Rows)
                {
                    dbQry = string.Format("INSERT INTO tblLeadContact(LeadID,ContactedDate,ContactSummary,CallBackFlag,CallBackDate) VALUES({0},Format('{1}', 'dd/mm/yyyy'),'{2}','{3}','{4}')", LeadID, dr["ContactedDate"].ToString(), dr["ContactSummary"].ToString(), dr["CallBackFlag"].ToString(), dr["CallBackDate"].ToString());
                    manager.ExecuteNonQuery(CommandType.Text, dbQry);
                }

            }



            sAuditStr = "Lead Management Transaction: " + LeadID + " got edited. Old Record Details : Lead Id No=" + OldTransNo + " Customer=" + OldCustomer + ",Mode of Contact=" + OldContact + ",CreationDate=" + Olddcreationdate + " DateTime:" + DateTime.Now.ToString() + " User:" + userID;

            dbQry = string.Format("INSERT INTO  tblAudit(Description,Command) VALUES('{0}','{1}')", sAuditStr, "Edit and Update");
            manager.ExecuteNonQuery(CommandType.Text, dbQry);


            manager.CommitTransaction();
        }
        catch (Exception ex)
        {
            throw ex;
        }
        finally
        {
            manager.Dispose();
        }

    }


    public DataSet ListReferenceInfo(string connection, string txtSearch, string dropDown)
    {
        DBManager manager = new DBManager(DataProvider.OleDb);
        manager.ConnectionString = CreateConnectionString(connection);
        DataSet ds = new DataSet();
        string dbQry = string.Empty;
        txtSearch = "%" + txtSearch + "%";

        if (dropDown == "Mode of Contact")
        {
            dbQry = "select Id,Type,TextValue,TypeName from tblLeadReferences Where TextValue like '" + txtSearch + "'" + " and TypeName = 'Mode of Contact' Order By TypeName";
        }
        else if (dropDown == "Personal Responsible")
        {
            dbQry = "select Id,Type,TextValue,TypeName from tblLeadReferences Where TextValue like '" + txtSearch + "'" + " and TypeName = 'Personal Responsible' Order By TypeName";
        }
        else if (dropDown == "Business Type")
        {
            dbQry = "select Id,Type,TextValue,TypeName from tblLeadReferences Where TextValue like '" + txtSearch + "'" + " and TypeName = 'Business Type' Order By TypeName";
        }
        else if (dropDown == "Branch")
        {
            dbQry = "select Id,Type,TextValue,TypeName from tblLeadReferences Where TextValue like '" + txtSearch + "'" + " and TypeName = 'Branch' Order By TypeName";
        }
        else if (dropDown == "Last Completed Action")
        {
            dbQry = "select Id,Type,TextValue,TypeName from tblLeadReferences Where TextValue like '" + txtSearch + "'" + " and TypeName = 'Last Completed Action' Order By TypeName";
        }
        else if (dropDown == "Next Action")
        {
            dbQry = "select Id,Type,TextValue,TypeName from tblLeadReferences Where TextValue like '" + txtSearch + "'" + " and TypeName = 'Next Action' Order By TypeName";
        }
        else if (dropDown == "Category")
        {
            dbQry = "select Id,Type,TextValue,TypeName from tblLeadReferences Where TextValue like '" + txtSearch + "'" + " and TypeName = 'Category' Order By TypeName";
        }
        else if (dropDown == "Status")
        {
            dbQry = "select Id,Type,TextValue,TypeName from tblLeadReferences Where TextValue like '" + txtSearch + "'" + " and TypeName = 'Status' Order By TypeName";
        }
        else if (dropDown == "Information3")
        {
            dbQry = "select Id,Type,TextValue,TypeName from tblLeadReferences Where TextValue like '" + txtSearch + "'" + " and TypeName = 'Information3' Order By TypeName";
        }
        else if (dropDown == "Information4")
        {
            dbQry = "select Id,Type,TextValue,TypeName from tblLeadReferences Where TextValue like '" + txtSearch + "'" + " and TypeName = 'Information4' Order By TypeName";
        }
        else if (dropDown == "Information5")
        {
            dbQry = "select Id,Type,TextValue,TypeName from tblLeadReferences Where TextValue like '" + txtSearch + "'" + " and TypeName = 'Information5' Order By TypeName";
        }
        else
        {
            dbQry = string.Format("select Id,Type,TextValue,TypeName from tblLeadReferences Order By TypeName", txtSearch);
        }

        try
        {
            manager.Open();
            ds = manager.ExecuteDataSet(CommandType.Text, dbQry);

            if (ds.Tables[0].Rows.Count > 0)
                return ds;
            else
                return null;
        }
        catch (Exception ex)
        {
            throw ex;
        }
        finally
        {
            manager.Dispose();
        }

    }

    public void UpdateReference(string connection, int ID, string TextValue, string TypeName, string Types)
    {
        DBManager manager = new DBManager(DataProvider.OleDb);
        manager.ConnectionString = CreateConnectionString(connection);
        DataSet ds = new DataSet();
        string dbQry = string.Empty;

        try
        {
            manager.Open();
            manager.ProviderType = DataProvider.OleDb;

            manager.BeginTransaction();

            object exists = manager.ExecuteScalar(CommandType.Text, "SELECT Count(*) FROM tblLeadReferences Where TextValue='" + TextValue + "' And ID <> " + ID.ToString() + "");

            if (exists.ToString() != string.Empty)
            {
                if (int.Parse(exists.ToString()) > 0)
                {
                    throw new Exception("Reference Exists");
                }
            }

            dbQry = string.Format("Update tblLeadReferences SET TextValue='{1}', TypeName='{2}', Type ='{3}' WHERE ID={0}", ID, TextValue, TypeName, Types);

            manager.ExecuteNonQuery(CommandType.Text, dbQry);

            manager.CommitTransaction();

        }
        catch (Exception ex)
        {
            throw ex;
        }
        finally
        {
            if (manager != null)
                manager.Dispose();
        }
    }



    public void InsertReference(string connection, string TextValue, string TypeName, string Types)
    {
        DBManager manager = new DBManager(DataProvider.OleDb);
        manager.ConnectionString = CreateConnectionString(connection);
        DataSet ds = new DataSet();
        string dbQry = string.Empty;

        try
        {
            manager.Open();
            manager.ProviderType = DataProvider.OleDb;

            manager.BeginTransaction();

            object exists = manager.ExecuteScalar(CommandType.Text, "SELECT Count(*) FROM tblLeadReferences Where TextValue='" + TextValue + "'");

            if (exists.ToString() != string.Empty)
            {
                if (int.Parse(exists.ToString()) > 0)
                {
                    throw new Exception("Reference Exists");
                }
            }

            //int IDD = (Int32)manager.ExecuteScalar(CommandType.Text, "SELECT MAX(ID) FROM tblLeadReferences");

            dbQry = string.Format("INSERT INTO tblLeadReferences(TextValue, TypeName,Type) VALUES('{0}','{1}','{2}')",
                TextValue, TypeName, Types);

            manager.ExecuteNonQuery(CommandType.Text, dbQry);

            manager.CommitTransaction();

        }
        catch (Exception ex)
        {
            throw ex;
        }
        finally
        {
            manager.Dispose();
        }

    }


    public void DeleteReference(string connection, int ID)
    {
        DBManager manager = new DBManager(DataProvider.OleDb);
        manager.ConnectionString = CreateConnectionString(connection);
        DataSet ds = new DataSet();
        string dbQry = string.Empty;

        try
        {
            manager.Open();
            manager.ProviderType = DataProvider.OleDb;
            manager.BeginTransaction();

            dbQry = string.Format("Delete From tblLeadReferences Where ID = {0}", ID);

            manager.ExecuteNonQuery(CommandType.Text, dbQry);

            manager.CommitTransaction();

        }
        catch (Exception ex)
        {
            throw ex;
        }
        finally
        {
            manager.Dispose();
        }

    }


    public DataSet GetReferenceForId(string connection, int ID)
    {
        DBManager manager = new DBManager(DataProvider.OleDb);
        manager.ConnectionString = CreateConnectionString(connection);
        DataSet ds = new DataSet();
        string dbQry = string.Empty;

        try
        {
            dbQry = "select ID,TextValue,TypeName,Type from tblLeadReferences where ID = " + ID.ToString();
            manager.Open();

            ds = manager.ExecuteDataSet(CommandType.Text, dbQry);

            if (ds.Tables[0].Rows.Count > 0)
                return ds;
            else
                return null;
        }
        catch (Exception ex)
        {
            throw ex;
        }
        finally
        {
            if (manager != null)
                manager.Dispose();
        }
    }

    public DataSet ListLeadMasterContacts(string connection, string txtSearch, string dropDown)
    {
        DBManager manager = new DBManager(DataProvider.OleDb);
        manager.ConnectionString = CreateConnectionString(connection);
        DataSet ds = new DataSet();
        string dbQry = string.Empty;

        dbQry = "select tblLeadMaster.LeadID,CreationDate, ProspectCustName, Address,Mobile,Landline,Email,ModeOfContact,PersonalResponsible,BusinessType,Branch,Status,LastCompletedAction,NextAction,Category,tblLeadContact.ContactedDate,ContactSummary from tblLeadMaster inner join tblLeadContact on tblLeadMaster.leadid = tblLeadContact.leadid Where tblLeadMaster.LeadID > 0 ";

        if (txtSearch == "" || txtSearch == null)
        {

        }
        else
        {
            if (dropDown == "CreationDate")
            {
                dbQry = dbQry + " AND CreationDate = #" + DateTime.Parse(txtSearch.ToString()).ToString("MM/dd/yyyy") + "#";
            }
            else if (dropDown == "Status")
            {
                dbQry = dbQry + " AND Status like '%" + txtSearch + "%'";
            }
            else if (dropDown == "Mobile")
            {
                dbQry = dbQry + " AND Mobile like '%" + txtSearch + "%'";
            }
            else if (dropDown == "LastCompletedAction")
            {
                dbQry = dbQry + " AND LastCompletedAction like '%" + txtSearch + "%'";
            }
            else if (dropDown == "NextAction")
            {
                dbQry = dbQry + " AND NextAction like '%" + txtSearch + "%'";
            }
            else if (dropDown == "Category")
            {
                dbQry = dbQry + " AND Category like '%" + txtSearch + "%'";
            }
            else if (dropDown == "BusinessType")
            {
                dbQry = dbQry + " AND BusinessType like '%" + txtSearch + "%'";
            }
            else if (dropDown == "Branch")
            {
                dbQry = dbQry + " AND Branch like '%" + txtSearch + "%'";
            }
        }

        dbQry = dbQry + " Order By tblLeadMaster.LeadID, CreationDate Desc";

        try
        {
            manager.Open();
            ds = manager.ExecuteDataSet(CommandType.Text, dbQry);

            if (ds.Tables[0].Rows.Count > 0)
                return ds;
            else
                return null;
        }
        catch (Exception ex)
        {
            throw ex;
        }
        finally
        {
            manager.Dispose();
        }
    }

    public void AddLead(int LeadNo, DateTime startDate, string LeadName, string address, string mobile, string Telephone, string BpName, int BpId, string ContactName, int EmpId, string EmpName, string Status, string branch, string LeadStatus, double TotalAmount, int ClosingPer, DateTime ClosingDate, int PredictedClosing, DateTime PredictedClosingDate, double PotentialPotAmount, double PotentialWeightedAmount, string PredictedClosingPeriod, string InterestLevel, string usernam, DataSet dsStages, DataSet dsCompetitor, DataSet dsActivity, DataSet dsProduct, string check)
    {
        DBManager manager = new DBManager(DataProvider.OleDb);
        manager.ConnectionString = CreateConnectionString(this.ConnectionString);
        DataSet ds = new DataSet();
        string dbQry = string.Empty;
        int LeadIDD = 0;

        try
        {

            manager.Open();
            manager.ProviderType = DataProvider.OleDb;

            manager.BeginTransaction();

            dbQry = string.Format("INSERT INTO tblLeadHeader(Start_Date,Lead_Name,Address,Mobile,Telephone,BP_Name,Bp_Id,Contact_Name,Emp_Id,Emp_Name,Doc_Status,Branch,Lead_Status,InvoicedAmt,Closing_Per,Closing_Date,chec) VALUES(Format('{0}', 'dd/mm/yyyy'),'{1}','{2}','{3}','{4}','{5}',{6},'{7}',{8},'{9}','{10}','{11}','{12}',{13},{14},Format('{15}', 'dd/mm/yyyy'),'{16}')",
                    startDate.ToShortDateString(), LeadName, address, mobile, Telephone, BpName, BpId, ContactName, EmpId, EmpName, Status, branch, LeadStatus, TotalAmount, ClosingPer, ClosingDate, check);

            manager.ExecuteNonQuery(CommandType.Text, dbQry);

            LeadIDD = (Int32)manager.ExecuteScalar(CommandType.Text, "SELECT MAX(Lead_No) FROM tblLeadHeader");

            dbQry = string.Format("INSERT INTO tblLeadPotential(Lead_No, Predicted_Closing, Predicted_Closing_Period, Predicted_Closing_Date, Potential_Amount, Weighted_Amount, Interest_Level) VALUES({0},{1},'{2}',Format('{3}', 'dd/mm/yyyy'),{4},{5},'{6}')",
                    LeadIDD, PredictedClosing, PredictedClosingPeriod, PredictedClosingDate.ToShortDateString(), PotentialPotAmount, PotentialWeightedAmount, InterestLevel);

            manager.ExecuteNonQuery(CommandType.Text, dbQry);


            if (dsStages != null)
            {
                if (dsStages.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow dr in dsStages.Tables[0].Rows)
                    {
                        dbQry = string.Format("INSERT INTO tblStages(Lead_No,Start_Date,End_Date,Stage_Name,Stage_Setup_Id,Stage_Perc,Potential_Amount,Weighted_Amount,Remarks) VALUES({0},Format('{1}', 'dd/mm/yyyy'),Format('{2}', 'dd/mm/yyyy'),'{3}',{4},{5},{6},{7},'{8}')", LeadIDD, dr["Start_Date"].ToString(), dr["End_Date"].ToString(), dr["Stage_Name"].ToString(), Convert.ToInt32(dr["Stage_Setup_Id"]), Convert.ToInt32(dr["Stage_Perc"]), Convert.ToDouble(dr["Potential_Amount"]), Convert.ToDouble(dr["Weighted_Amount"]), dr["Remarks"].ToString());
                        manager.ExecuteNonQuery(CommandType.Text, dbQry);
                    }
                }
            }

            if (dsCompetitor != null)
            {
                if (dsCompetitor.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow dr in dsCompetitor.Tables[0].Rows)
                    {
                        dbQry = string.Format("INSERT INTO tblCompetitors(Lead_No,Competitor_Name,Threat_Level,Remarks) VALUES({0},'{1}','{2}','{3}')", LeadIDD, dr["Competitor_Name"].ToString(), dr["Threat_Level"].ToString(), dr["Remarks"].ToString());
                        manager.ExecuteNonQuery(CommandType.Text, dbQry);
                    }
                }
            }

            if (dsActivity != null)
            {
                if (dsActivity.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow dr in dsActivity.Tables[0].Rows)
                    {
                        dbQry = string.Format("INSERT INTO tblActivities(Lead_No,Start_Date,End_Date,NextActivity_Date,Activity_Name,Activity_Name_Id,Activity_Location,Next_Activity,Next_Activity_Id,FollowUp,Emp_Name,Emp_No,Remarks) VALUES({0},Format('{1}', 'dd/mm/yyyy'),Format('{2}', 'dd/mm/yyyy'),Format('{3}', 'dd/mm/yyyy'),'{4}',{5},'{6}','{7}',{8},'{9}','{10}',{11},'{12}')", LeadIDD, dr["Start_Date"].ToString(), dr["End_Date"].ToString(), dr["NextActivity_Date"].ToString(), dr["Activity_Name"].ToString(), Convert.ToInt32(dr["Activity_Name_Id"]), Convert.ToString(dr["Activity_Location"]), Convert.ToString(dr["Next_Activity"]), Convert.ToInt32(dr["Next_Activity_Id"]), Convert.ToString(dr["FollowUp"]), Convert.ToString(dr["Emp_Name"]), Convert.ToInt32(dr["Emp_No"]), dr["Remarks"].ToString());
                        manager.ExecuteNonQuery(CommandType.Text, dbQry);
                    }
                }
            }

            if (dsProduct != null)
            {
                if (dsProduct.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow dr in dsProduct.Tables[0].Rows)
                    {
                        dbQry = string.Format("INSERT INTO tblProductInterest(Lead_No,Product_Id,Product_Name,SlNo) VALUES({0},'{1}','{2}',{3})", LeadIDD, dr["Product_Id"].ToString(), dr["Product_Name"].ToString(), Convert.ToInt32(dr["SlNo"]));
                        manager.ExecuteNonQuery(CommandType.Text, dbQry);
                    }
                }
            }

            manager.CommitTransaction();
        }
        catch (Exception ex)
        {
            throw ex;
        }
        finally
        {
            manager.Dispose();
        }
    }

    public DataSet ListLead(string connection, string txtSearch, string dropDown)
    {
        DBManager manager = new DBManager(DataProvider.OleDb);
        manager.ConnectionString = CreateConnectionString(connection);
        DataSet ds = new DataSet();
        string dbQry = string.Empty;
        string dQry = string.Empty;
        dbQry = "select Lead_No,Start_Date,Lead_Name,Address,Mobile,Telephone,BP_Name,Bp_Id,Contact_Name,Emp_Id,Emp_Name,Doc_Status,Branch,Lead_Status,InvoicedAmt,Closing_Per,Closing_Date from tblLeadHeader Where 1 = 1 ";

        if (txtSearch == "" || txtSearch == null)
        {

        }
        else
        {
            if (dropDown == "StartDate")
            {
                dbQry = dbQry + " AND Start_Date = #" + DateTime.Parse(txtSearch.ToString()).ToString("MM/dd/yyyy") + "#";
            }
            else if (dropDown == "BPName")
            {
                dbQry = dbQry + " AND BP_Name like '%" + txtSearch + "%'";
            }
            else if (dropDown == "LeadNo")
            {
                dbQry = dbQry + " AND Lead_No = " + txtSearch + " ";
            }
            else if (dropDown == "LeadStatus")
            {
                dbQry = dbQry + " AND Lead_Status like '%" + txtSearch + "%'";
            }
            else if (dropDown == "DocStatus")
            {
                dbQry = dbQry + " AND Doc_Status like '%" + txtSearch + "%'";
            }
            else if (dropDown == "Mobile")
            {
                dbQry = dbQry + " AND Mobile like '%" + txtSearch + "%'";
            }
            else if (dropDown == "LeadName")
            {
                dbQry = dbQry + " AND Lead_Name like '%" + txtSearch + "%'";
            }
            else if (dropDown == "Branch")
            {
                dbQry = dbQry + " AND Branch like '%" + txtSearch + "%'";
            }
            else if (dropDown == "All" || dropDown=="0")
            {
                
            }
        }

        dbQry = dbQry + " Order By Lead_No, Start_Date Desc";

        try
        {
            manager.Open();
            ds = manager.ExecuteDataSet(CommandType.Text, dbQry);

            if (ds.Tables[0].Rows.Count > 0)
            {
                DataSet dsd;
                DataTable dt;
                DataRow drNew;
                DataColumn dc;
                dsd = new DataSet();
                dt = new DataTable();
                dc = new DataColumn("Lead_No");
                dt.Columns.Add(dc);
                dc = new DataColumn("Start_Date");
                dt.Columns.Add(dc);
                dc = new DataColumn("Lead_Name");
                dt.Columns.Add(dc);
                dc = new DataColumn("Address");
                dt.Columns.Add(dc);
                dc = new DataColumn("Mobile");
                dt.Columns.Add(dc);
                dc = new DataColumn("Telephone");
                dt.Columns.Add(dc);
                dc = new DataColumn("Branch");
                dt.Columns.Add(dc);
                dc = new DataColumn("BP_Name");
                dt.Columns.Add(dc);
                dc = new DataColumn("InvoicedAmt");
                dt.Columns.Add(dc);
                dc = new DataColumn("Doc_Status");
                dt.Columns.Add(dc);
                dc = new DataColumn("Emp_Name");
                dt.Columns.Add(dc);
                dc = new DataColumn("Emp_Id");
                dt.Columns.Add(dc);
                dc = new DataColumn("Closing_Date");
                dt.Columns.Add(dc);
                dc = new DataColumn("Lead_Status");
                dt.Columns.Add(dc);
                dc = new DataColumn("Closing_Per");
                dt.Columns.Add(dc);
                dc = new DataColumn("Contact_Name");
                dt.Columns.Add(dc);
                dc = new DataColumn("Bp_Id");
                dt.Columns.Add(dc);
                dc = new DataColumn("Activity_Name");
                dt.Columns.Add(dc);
                dc = new DataColumn("Next_Activity");
                dt.Columns.Add(dc);
                dsd.Tables.Add(dt);
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    drNew = dt.NewRow();
                    if (dr["Lead_No"] != null)
                        drNew["Lead_No"] = dr["Lead_No"].ToString();

                    if (dr["Lead_No"] != null)
                    {
                        if (dr["Lead_No"].ToString() != "")
                        {
                            if (Convert.ToInt32(dr["Lead_No"].ToString()) > 0)
                            {
                                dQry = "SELECT Next_Activity,Activity_Name FROM tblActivities WHERE Lead_No=" + Convert.ToInt32(dr["Lead_No"].ToString()) + " order by Activity_Id asc";
                                DataSet dsdd = manager.ExecuteDataSet(CommandType.Text, dQry);
                                if (dsdd.Tables[0].Rows.Count > 0)
                                {
                                    foreach (DataRow drt in dsdd.Tables[0].Rows)
                                    {
                                        if (drt["Activity_Name"] != null)
                                            drNew["Activity_Name"] = drt["Activity_Name"].ToString();
                                        if (drt["Next_Activity"] != null)
                                            drNew["Next_Activity"] = drt["Next_Activity"].ToString();
                                    }
                                }
                                else
                                {
                                    drNew["Activity_Name"] = "";
                                    drNew["Next_Activity"] = "";
                                }
                            }
                        }
                    }


                    if (dr["Start_Date"] != null)
                        drNew["Start_Date"] = dr["Start_Date"].ToString();
                    if (dr["Lead_Name"] != null)
                        drNew["Lead_Name"] = dr["Lead_Name"].ToString();
                    if (dr["Address"] != null)
                        drNew["Address"] = dr["Address"].ToString();
                    if (dr["Mobile"] != null)
                        drNew["Mobile"] = dr["Mobile"].ToString();
                    if (dr["Telephone"] != null)
                        drNew["Telephone"] = dr["Telephone"].ToString();
                    if (dr["Branch"] != null)
                        drNew["Branch"] = dr["Branch"].ToString();
                    if (dr["BP_Name"] != null)
                        drNew["BP_Name"] = dr["BP_Name"].ToString();
                    if (dr["InvoicedAmt"] != null)
                        drNew["InvoicedAmt"] = dr["InvoicedAmt"].ToString();
                    if (dr["Doc_Status"] != null)
                        drNew["Doc_Status"] = dr["Doc_Status"].ToString();
                    if (dr["Emp_Name"] != null)
                        drNew["Emp_Name"] = dr["Emp_Name"].ToString();
                    if (dr["Emp_Id"] != null)
                        drNew["Emp_Id"] = dr["Emp_Id"].ToString();
                    if (dr["Closing_Date"] != null)
                        drNew["Closing_Date"] = dr["Closing_Date"].ToString();
                    if (dr["Lead_Status"] != null)
                        drNew["Lead_Status"] = dr["Lead_Status"].ToString();
                    if (dr["Closing_Per"] != null)
                        drNew["Closing_Per"] = dr["Closing_Per"].ToString();
                    if (dr["Contact_Name"] != null)
                        drNew["Contact_Name"] = dr["Contact_Name"].ToString();
                    if (dr["Bp_Id"] != null)
                        drNew["Bp_Id"] = dr["Bp_Id"].ToString();

                    dsd.Tables[0].Rows.Add(drNew);                        
                }
                return dsd;
            }              
            else
                return null;
        }
        catch (Exception ex)
        {
            throw ex;
        }
        finally
        {
            manager.Dispose();
        }
    }

    public DataSet GetLeadDetails(int LeadNo)
    {
        DBManager manager = new DBManager(DataProvider.OleDb);
        manager.ConnectionString = CreateConnectionString(this.ConnectionString);
        DataSet ds = new DataSet();
        string dbQry = string.Empty;

        dbQry = "select Lead_No,Start_Date,Lead_Name,Address,Mobile,Telephone,BP_Name,Bp_Id,Contact_Name,Emp_Id,Emp_Name,Doc_Status,Branch,Lead_Status,InvoicedAmt,Closing_Per,Closing_Date,chec from tblLeadHeader Where Lead_No=" + LeadNo;

        try
        {
            manager.Open();
            ds = manager.ExecuteDataSet(CommandType.Text, dbQry);

            if (ds.Tables[0].Rows.Count > 0)
                return ds;
            else
                return null;
        }
        catch (Exception ex)
        {
            throw ex;
        }
        finally
        {
            manager.Dispose();
        }
    }

    public DataSet GetLeadPotential(int LeadNo)
    {
        DBManager manager = new DBManager(DataProvider.OleDb);
        manager.ConnectionString = CreateConnectionString(this.ConnectionString);
        DataSet ds = new DataSet();
        string dbQry = string.Empty;

        dbQry = "select Lead_No, Predicted_Closing, Predicted_Closing_Period, Predicted_Closing_Date, Potential_Amount, Weighted_Amount, Interest_Level from tblLeadPotential Where Lead_No=" + LeadNo;

        try
        {
            manager.Open();
            ds = manager.ExecuteDataSet(CommandType.Text, dbQry);

            if (ds.Tables[0].Rows.Count > 0)
                return ds;
            else
                return null;
        }
        catch (Exception ex)
        {
            throw ex;
        }
        finally
        {
            manager.Dispose();
        }
    }

    public void UpdateLead(int LeadNo, DateTime startDate, string LeadName, string address, string mobile, string Telephone, string BpName, int BpId, string ContactName, int EmpId, string EmpName, string Status, string branch, string LeadStatus, double TotalAmount, int ClosingPer, DateTime ClosingDate, int PredictedClosing, DateTime PredictedClosingDate, double PotentialPotAmount, double PotentialWeightedAmount, string PredictedClosingPeriod, string InterestLevel, string usernam, DataSet dsStages, DataSet dsCompetitor, DataSet dsActivity, DataSet dsProduct, string check)
    {
        DBManager manager = new DBManager(DataProvider.OleDb);
        manager.ConnectionString = CreateConnectionString(this.ConnectionString);
        DataSet ds = new DataSet();
        string dbQry = string.Empty;
        string sAuditStr = string.Empty;

        string dbQryData = string.Empty;
        DataSet dsOld = new DataSet();
        int OldTransNo = 0;
        string Olddcreationdate = string.Empty;
        string OldCustomer = string.Empty;
        string OldContact = string.Empty;

        try
        {
            manager.Open();
            manager.ProviderType = DataProvider.OleDb;

            manager.BeginTransaction();

            dbQryData = "select Lead_No,Start_Date,Lead_Name,Address,Mobile,Telephone,BP_Name,Bp_Id,Contact_Name,Emp_Id,Emp_Name,Doc_Status,Branch,Lead_Status,InvoicedAmt,Closing_Per,Closing_Date from tblLeadHeader Where Lead_No =" + LeadNo;
            dsOld = manager.ExecuteDataSet(CommandType.Text, dbQryData.ToString());
            if (dsOld != null)
            {
                if (dsOld.Tables[0].Rows.Count > 0)
                {
                    if (dsOld.Tables[0].Rows[0]["Lead_No"] != null)
                    {
                        if (dsOld.Tables[0].Rows[0]["Lead_No"].ToString() != string.Empty)
                        {
                            OldTransNo = Convert.ToInt32(dsOld.Tables[0].Rows[0]["Lead_No"].ToString());
                            Olddcreationdate = dsOld.Tables[0].Rows[0]["Start_Date"].ToString();
                            OldCustomer = dsOld.Tables[0].Rows[0]["BP_Name"].ToString();
                            OldContact = dsOld.Tables[0].Rows[0]["Lead_Name"].ToString();
                        }
                    }
                }
            }

            dbQry = string.Format("Update tblLeadHeader Set Start_Date=Format('{0}', 'dd/mm/yyyy'),Lead_Name='{1}',Address='{2}',Mobile='{3}',Telephone='{4}',BP_Name='{5}',Bp_Id={6},Contact_Name='{7}',Emp_Id={8},Emp_Name='{9}',Doc_Status='{10}',Branch='{11}',Lead_Status='{12}',InvoicedAmt={13}, Closing_Per={14}, Closing_Date=Format('{15}', 'dd/mm/yyyy'),chec='{17}'  Where Lead_No={16}", startDate.ToShortDateString(), LeadName, address, mobile, Telephone, BpName, BpId, ContactName, EmpId, EmpName, Status, branch, LeadStatus, TotalAmount, ClosingPer, ClosingDate, LeadNo, check);
            manager.ExecuteNonQuery(CommandType.Text, dbQry);


            dbQry = string.Format("Update tblLeadPotential Set Predicted_Closing_Date=Format('{0}', 'dd/mm/yyyy'),Predicted_Closing={1},Predicted_Closing_Period='{2}',Potential_Amount={3},Weighted_Amount={4},Interest_Level='{5}' Where Lead_No={6}", PredictedClosingDate.ToShortDateString(), PredictedClosing, PredictedClosingPeriod, PotentialPotAmount, PotentialWeightedAmount, InterestLevel, LeadNo);
            manager.ExecuteNonQuery(CommandType.Text, dbQry);



            dbQry = string.Format("Delete From tblStages Where Lead_No={0}", LeadNo);
            manager.ExecuteNonQuery(CommandType.Text, dbQry);
            if (dsStages != null)
            {
                if (dsStages.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow dr in dsStages.Tables[0].Rows)
                    {
                        dbQry = string.Format("INSERT INTO tblStages(Lead_No,Start_Date,End_Date,Stage_Name,Stage_Setup_Id,Stage_Perc,Potential_Amount,Weighted_Amount,Remarks) VALUES({0},Format('{1}', 'dd/mm/yyyy'),Format('{2}', 'dd/mm/yyyy'),'{3}',{4},{5},{6},{7},'{8}')", LeadNo, dr["Start_Date"].ToString(), dr["End_Date"].ToString(), dr["Stage_Name"].ToString(), Convert.ToInt32(dr["Stage_Setup_Id"]), Convert.ToInt32(dr["Stage_Perc"]), Convert.ToDouble(dr["Potential_Amount"]), Convert.ToDouble(dr["Weighted_Amount"]), dr["Remarks"].ToString());
                        manager.ExecuteNonQuery(CommandType.Text, dbQry);
                    }
                }
            }

            dbQry = string.Format("Delete From tblCompetitors Where Lead_No={0}", LeadNo);
            manager.ExecuteNonQuery(CommandType.Text, dbQry);
            if (dsCompetitor != null)
            {
                if (dsCompetitor.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow dr in dsCompetitor.Tables[0].Rows)
                    {
                        dbQry = string.Format("INSERT INTO tblCompetitors(Lead_No,Competitor_Name,Threat_Level,Remarks) VALUES({0},'{1}','{2}','{3}')", LeadNo, dr["Competitor_Name"].ToString(), dr["Threat_Level"].ToString(), dr["Remarks"].ToString());
                        manager.ExecuteNonQuery(CommandType.Text, dbQry);
                    }
                }
            }

            dbQry = string.Format("Delete From tblActivities Where Lead_No={0}", LeadNo);
            manager.ExecuteNonQuery(CommandType.Text, dbQry);
            if (dsActivity != null)
            {
                if (dsActivity.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow dr in dsActivity.Tables[0].Rows)
                    {
                        dbQry = string.Format("INSERT INTO tblActivities(Lead_No,Start_Date,End_Date,NextActivity_Date,Activity_Name,Activity_Name_Id,Activity_Location,Next_Activity,Next_Activity_Id,FollowUp,Emp_Name,Emp_No,Remarks) VALUES({0},Format('{1}', 'dd/mm/yyyy'),Format('{2}', 'dd/mm/yyyy'),Format('{3}', 'dd/mm/yyyy'),'{4}',{5},'{6}','{7}',{8},'{9}','{10}',{11},'{12}')", LeadNo, dr["Start_Date"].ToString(), dr["End_Date"].ToString(), dr["NextActivity_Date"].ToString(), dr["Activity_Name"].ToString(), Convert.ToInt32(dr["Activity_Name_Id"]), Convert.ToString(dr["Activity_Location"]), Convert.ToString(dr["Next_Activity"]), Convert.ToInt32(dr["Next_Activity_Id"]), Convert.ToString(dr["FollowUp"]), Convert.ToString(dr["Emp_Name"]), Convert.ToInt32(dr["Emp_No"]), dr["Remarks"].ToString());
                        manager.ExecuteNonQuery(CommandType.Text, dbQry);
                    }
                }
            }

            dbQry = string.Format("Delete From tblProductInterest Where Lead_No={0}", LeadNo);
            manager.ExecuteNonQuery(CommandType.Text, dbQry);
            if (dsProduct != null)
            {
                if (dsProduct.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow dr in dsProduct.Tables[0].Rows)
                    {
                        dbQry = string.Format("INSERT INTO tblProductInterest(Lead_No,Product_Id,Product_Name,SlNo) VALUES({0},'{1}','{2}',{3})", LeadNo, dr["Product_Id"].ToString(), dr["Product_Name"].ToString(), Convert.ToInt32(dr["SlNo"]));
                        manager.ExecuteNonQuery(CommandType.Text, dbQry);
                    }
                }
            }


            sAuditStr = "Lead Management LeadNo: " + LeadNo + " got edited. Old Record Details : Lead No=" + OldTransNo + " Bp Name=" + OldCustomer + ", Lead Name = " + OldContact + ", Start Date=" + Olddcreationdate + " DateTime:" + DateTime.Now.ToString() + " User: " + usernam;

            dbQry = string.Format("INSERT INTO  tblAudit(Description,Command) VALUES('{0}','{1}')", sAuditStr, "Edit and Update");
            manager.ExecuteNonQuery(CommandType.Text, dbQry);


            manager.CommitTransaction();
        }
        catch (Exception ex)
        {
            throw ex;
        }
        finally
        {
            manager.Dispose();
        }
    }

    public DataSet GetLeadStages(int LeadNo)
    {
        DBManager manager = new DBManager(DataProvider.OleDb);
        manager.ConnectionString = CreateConnectionString(this.ConnectionString);
        DataSet ds = new DataSet();
        string dbQry = string.Empty;

        dbQry = "select Lead_No,Start_Date,End_Date,Stage_Name,Stage_Setup_Id,Stage_Perc,Potential_Amount,Weighted_Amount,Remarks,Stage_Id From tblStages Where Lead_No=" + LeadNo;

        try
        {
            manager.Open();
            ds = manager.ExecuteDataSet(CommandType.Text, dbQry);

            if (ds.Tables[0].Rows.Count > 0)
                return ds;
            else
                return null;
        }
        catch (Exception ex)
        {
            throw ex;
        }
        finally
        {
            manager.Dispose();
        }
    }

    public DataSet GetLeadCompetitor(int LeadNo)
    {
        DBManager manager = new DBManager(DataProvider.OleDb);
        manager.ConnectionString = CreateConnectionString(this.ConnectionString);
        DataSet ds = new DataSet();
        string dbQry = string.Empty;

        dbQry = "select Lead_No,Competitor_Name,Remarks,Threat_Level,Competitor_Id From tblCompetitors Where Lead_No=" + LeadNo;

        try
        {
            manager.Open();
            ds = manager.ExecuteDataSet(CommandType.Text, dbQry);

            if (ds.Tables[0].Rows.Count > 0)
                return ds;
            else
                return null;
        }
        catch (Exception ex)
        {
            throw ex;
        }
        finally
        {
            manager.Dispose();
        }
    }

    public DataSet GetLeadActivity(int LeadNo)
    {
        DBManager manager = new DBManager(DataProvider.OleDb);
        manager.ConnectionString = CreateConnectionString(this.ConnectionString);
        DataSet ds = new DataSet();
        string dbQry = string.Empty;

        dbQry = "select Lead_No,Start_Date,End_Date,Activity_Name,Activity_Name_Id,Activity_Id,Activity_Location,Next_Activity,Next_Activity_Id,NextActivity_Date,FollowUp,Emp_Name,Emp_No,Remarks From tblActivities Where Lead_No=" + LeadNo;

        try
        {
            manager.Open();
            ds = manager.ExecuteDataSet(CommandType.Text, dbQry);

            if (ds.Tables[0].Rows.Count > 0)
                return ds;
            else
                return null;
        }
        catch (Exception ex)
        {
            throw ex;
        }
        finally
        {
            manager.Dispose();
        }
    }

    public DataSet GetLeadProduct(int LeadNo)
    {
        DBManager manager = new DBManager(DataProvider.OleDb);
        manager.ConnectionString = CreateConnectionString(this.ConnectionString);
        DataSet ds = new DataSet();
        string dbQry = string.Empty;

        dbQry = "select * From tblProductInterest Where Lead_No=" + LeadNo;

        try
        {
            manager.Open();
            ds = manager.ExecuteDataSet(CommandType.Text, dbQry);

            if (ds.Tables[0].Rows.Count > 0)
                return ds;
            else
                return null;
        }
        catch (Exception ex)
        {
            throw ex;
        }
        finally
        {
            manager.Dispose();
        }
    }


}