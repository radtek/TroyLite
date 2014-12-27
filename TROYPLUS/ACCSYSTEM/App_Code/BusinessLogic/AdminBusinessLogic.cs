using DataAccessLayer;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for AdminBusinessLogic
/// </summary>
public class AdminBusinessLogic
{
	public AdminBusinessLogic()
	{
		//
		// TODO: Add constructor logic here
		//
	}

    public AdminBusinessLogic(string con)
    {
        this.ConnectionString = con;
    }

    public string ConnectionString { get; set; }

    public string CreateConnectionString(string connStr)
    {
        string connectionString = string.Empty;
        string newConnection = string.Empty;

        if (connStr.IndexOf("Provider=Microsoft.Jet.OLEDB.4.0;") > -1)
            connectionString = connStr;
        else
            connectionString = System.Configuration.ConfigurationManager.ConnectionStrings[connStr].ConnectionString;

        newConnection = connectionString.Remove(connectionString.LastIndexOf("Password=") + 9);

        newConnection = newConnection + Helper.GetPasswordForDB(connectionString);

        return newConnection;

    }

    #region Payroll Generation
    

    public void CheckQueueAndGeneratePayRoll()
    {
        DBManager manager = new DBManager(DataProvider.OleDb);
        manager.ConnectionString = CreateConnectionString(this.ConnectionString);
        string dbQry = string.Empty;
        try
        {
            manager.Open();

            dbQry = string.Format(@"SELECT * FROM tblPayrollQueue WHERE Status <> '{0}'", "Completed");
            DataSet ds = manager.ExecuteDataSet(CommandType.Text, dbQry);
            manager.Close();

            if (ds != null && ds.Tables.Count > 0)
            {
                foreach (DataRow drQueue in ds.Tables[0].Rows)
                {
                    int payrollId = 0;
                    int year = 0;
                    int month = 0;

                    if (int.TryParse(drQueue[0].ToString(), out payrollId))
                    {
                        int.TryParse(drQueue[1].ToString(), out year);
                        int.TryParse(drQueue[2].ToString(), out month);
                        GeneratePayRoll(payrollId, year, month);                        
                    }
                }
            }

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

    private void GeneratePayRoll(int payrollId, int year, int month)
    {
        // Get all the employee list.
        DBManager manager = new DBManager(DataProvider.OleDb);
        manager.ConnectionString = CreateConnectionString(this.ConnectionString);
        
        try
        {           
            if (payrollId > 0)
            {                
                string dbQuery = "SELECT EmployeeNo, EmployeeFirstName FROM tblEmployee";
                DataSet ds = manager.ExecuteDataSet(CommandType.Text, dbQuery);
                string logMessage = string.Empty;
                if (ds != null && ds.Tables.Count > 0)
                {
                    // Prepare payslip datatable.
                    DataTable dtPaySlipInfo = new DataTable();
                    dtPaySlipInfo.Columns.Add(new DataColumn("EmployeeNo"));
                    dtPaySlipInfo.Columns.Add(new DataColumn("Deductions"));
                    dtPaySlipInfo.Columns.Add(new DataColumn("Payments"));
                    UpdatePayrollStatus(payrollId, "In Progress");

                    // Prepare payroll generation logtable.
                    foreach (DataRow drEmployee in ds.Tables[0].Rows)
                    {
                        int employeeNo = 0;
                        DataRow drPayslip = dtPaySlipInfo.NewRow();
                        if (int.TryParse(drEmployee[0].ToString(), out employeeNo))
                        {
                            if (GetPayRollDetailsForEmployee(employeeNo, year, month, drPayslip, ref logMessage))
                            {
                                dtPaySlipInfo.Rows.Add(drPayslip);
                                InsertPayrollLog("Completed", payrollId, employeeNo);
                            }
                            else
                            {
                                InsertPayrollLog(logMessage, payrollId,employeeNo);
                            }
                        }
                    }
                    manager.Open();
                    manager.BeginTransaction();
                    if (InsertPayslipInfo(manager, dtPaySlipInfo, payrollId,month))
                    {
                        manager.CommitTransaction();
                        UpdatePayrollStatus(payrollId, "Completed");
                    }
                    else
                    {
                        manager.RollbackTransaction();
                        UpdatePayrollStatus(payrollId, "Failed");
                    }
                    
                    
                }
            }
        }
        catch (Exception ex)
        {            
            TroyLiteExceptionManager.HandleException(ex);
            manager.RollbackTransaction();
            UpdatePayrollStatus(payrollId, "Failed");
        }
        finally
        {
            if (manager != null)
                manager.Dispose();
        }
        
    }

    private bool InsertPayslipInfo(DBManager manager, DataTable dtPaySlipInfo, int payrollId, int month)
    {
        string dbQry = string.Empty;

        try
        {
            foreach (DataRow dr in dtPaySlipInfo.Rows)
            {
                int employeeNo = 0;
                int.TryParse(dr[0].ToString(), out employeeNo);
                dbQry = string.Format(@"INSERT INTO tblEmployeePayslip (EmployeeId,PayrollDate,PayrollMonth,Deductions,Payments,PayrollId) 
                                    VALUES ({0},'{1}',{2},{3},{4},{5})", employeeNo, DateTime.Now.Date, month, dr[1], dr[2], payrollId);

                if (manager.ExecuteNonQuery(CommandType.Text, dbQry) > 0)
                {
                    InsertPayrollLog("Payroll generated but payslip not inserted", payrollId, employeeNo);
                }
            }
            return true;

        }
        catch (Exception ex)
        {
            throw ex;
        }
        
    }

    private void UpdatePayrollStatus(int payrollId, string status)
    {
        DBManager manager = new DBManager(DataProvider.OleDb);
        manager.ConnectionString = CreateConnectionString(this.ConnectionString);
        string dbQry = string.Empty;
        try
        {
            manager.Open();

            if (status.ToUpper().Equals("COMPLETED"))
            {
                dbQry = string.Format(@"UPDATE tblPayrollQueue SET Status = '{0}', PayrollCompletedDateTime='{1}' WHERE PayrollId={2}", status, DateTime.Now, payrollId);
            }
            else
            {
                dbQry = string.Format(@"UPDATE tblPayrollQueue SET Status = '{0}' WHERE PayrollId={1}", status, payrollId);
            }

            int result = manager.ExecuteNonQuery(CommandType.Text, dbQry);

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

    private void InsertPayrollLog(string logMessage, int payrollId,int employeeNo)
    {
        DBManager manager = new DBManager(DataProvider.OleDb);
        manager.ConnectionString = CreateConnectionString(this.ConnectionString);
        string dbQry = string.Empty;
        try
        {
            manager.Open();
            dbQry = string.Format(@"INSERT INTO tblPayrollGenerationLog (PayrollId,EmployeeNo,Message) 
                                    VALUES ({0},'{1}')", payrollId, employeeNo, logMessage);

            int result = manager.ExecuteNonQuery(CommandType.Text, dbQry);
           
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
   
    public bool GetPayRollDetailsForEmployee(int employeeNo, int year, int month, DataRow payslipInfoRow, ref string logMessage)
    {
        string dbQry = string.Empty;
        logMessage = string.Empty;
        try
        {
            // Validate payroll details for employee.
            ValidatePayrollDetailsForEmployee(employeeNo, year, month, ref logMessage);

            // Get declared payable/deductiable amount for the employee.
            int totalPayable = GetEmployeeTotalPayComponent(employeeNo);
            int totalDeductions = GetEmployeeTotalDeduction(employeeNo);

            // Get loss of pay leaves.


                       
            payslipInfoRow[0] = employeeNo;
            payslipInfoRow[1] = totalDeductions;
            payslipInfoRow[2] = totalPayable;
            
            return true;
        }
        catch (Exception ex)
        {
            logMessage = ex.Message;
            TroyLiteExceptionManager.HandleException(ex);
            return false;
        }
        finally
        {

        }
    }


    private int GetEmployeeTotalPayComponent(int employeeNo)
    {
        DBManager manager = new DBManager(DataProvider.OleDb);
        manager.ConnectionString = CreateConnectionString(this.ConnectionString);
        string dbQry = string.Empty;
        int componentTotalPay=0;
        try
        {
            manager.Open();

            dbQry = string.Format(@"Select SUM(pc.DeclaredAmount) as ComponentTotalPay 
                                    FROM tblPayComponentEmployeeMapping pcm
                                    INNER JOIN tblPayComponents pc ON pc.PayComponentId=pcm.PayComponentId                                    
                                    WHERE pcm.EmployeeId={0} AND pc.PayComponentType_id={1} AND pc.IsDeduction=False", employeeNo);
            DataSet ds = manager.ExecuteDataSet(CommandType.Text, dbQry);

            if (ds != null && ds.Tables.Count > 0)
            {
                if (ds.Tables[0].Rows.Count > 0)
                {
                    var totalComponentPay = ds.Tables[0].Rows[0][0].ToString();
                    int.TryParse(totalComponentPay, out componentTotalPay);
                    return componentTotalPay;
                }
            }
            return componentTotalPay;

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

    private int GetEmployeeTotalDeduction(int employeeNo)
    {
        DBManager manager = new DBManager(DataProvider.OleDb);
        manager.ConnectionString = CreateConnectionString(this.ConnectionString);
        string dbQry = string.Empty;
        int componentTotalPay = 0;
        try
        {
            manager.Open();

            dbQry = string.Format(@"Select SUM(pc.DeclaredAmount) as TotalDeduction 
                                    FROM tblPayComponentEmployeeMapping pcm
                                    INNER JOIN tblPayComponents pc ON pc.PayComponentId=pcm.PayComponentId                                    
                                    WHERE pcm.EmployeeId={0} AND pc.PayComponentType_id={1} AND pc.IsDeduction=True", employeeNo,1);
            
            DataSet ds = manager.ExecuteDataSet(CommandType.Text, dbQry);

            if (ds != null && ds.Tables.Count > 0)
            {
                if (ds.Tables[0].Rows.Count > 0)
                {
                    var totalComponentPay = ds.Tables[0].Rows[0][0].ToString();
                    int.TryParse(totalComponentPay, out componentTotalPay);
                    return componentTotalPay;
                }
            }
            return componentTotalPay;
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

    public bool ValidatePayrollDetailsForEmployee(int empNo, int year, int month, ref string logMessage)
    {

        string dbQuery = string.Format(@"Select LeaveId,EmployeeNo,StartDate,Status from tblEmployeeLeave 
                            WHERE EmployeeNo = {0} AND
                                    ((YEAR(StartDate)={1} OR YEAR(EndDate)={2}) AND (MONTH(StartDate)={3} OR MONTH(EndDate)={4}))
                                        Status<>'{5}'", empNo, year, month, "Approved");


        DBManager manager = new DBManager(DataProvider.OleDb);
        manager.ConnectionString = CreateConnectionString(this.ConnectionString);
        DataSet ds = manager.ExecuteDataSet(CommandType.Text, dbQuery);
        if (ds != null && ds.Tables.Count > 0)
        {
            if (ds.Tables[0].Rows.Count > 0)
            {
                logMessage = string.Format("Leaves are not approved for the employees:\r\n {0}", string.Join("/ \r\n", ds.Tables[0].Rows[0].ItemArray));
                return false;
            }            
        }       

        return true;
    }
    
    #endregion
}