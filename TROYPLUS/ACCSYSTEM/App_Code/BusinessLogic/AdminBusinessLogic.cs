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

            dbQry = string.Format(@"SELECT * FROM tblPayrollQueue WHERE Status = '{0}'", "Queued");
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

    public bool GeneratePayRoll(int payrollId, int year, int month)
    {
        // Get all the employee list.
        DBManager manager = new DBManager(DataProvider.OleDb);
        manager.ConnectionString = CreateConnectionString(this.ConnectionString);
        bool isPayrollGenerated = false;
        try
        {
            if (payrollId > 0)
            {
                string dbQuery = "SELECT EmpNo, EmpFirstName FROM tblEmployee";
                manager.Open();
                DataSet ds = manager.ExecuteDataSet(CommandType.Text, dbQuery);
                string logMessage = string.Empty;
                if (ds != null && ds.Tables.Count > 0)
                {
                    // Prepare payslip datatable.
                    DataTable dtPaySlipInfo = new DataTable();
                    dtPaySlipInfo.Columns.Add(new DataColumn("EmployeeNo"));
                    dtPaySlipInfo.Columns.Add(new DataColumn("Deductions"));
                    dtPaySlipInfo.Columns.Add(new DataColumn("Payments"));
                    dtPaySlipInfo.Columns.Add(new DataColumn("LossOfPayDays"));
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
                                isPayrollGenerated = true;
                            }
                            else
                            {
                                InsertPayrollLog(logMessage, payrollId, employeeNo);
                            }
                        }
                    }

                    manager.BeginTransaction();
                    if (InsertPayslipInfo(manager, dtPaySlipInfo, payrollId, month, year))
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
            return isPayrollGenerated;
        }
        catch (Exception ex)
        {
            TroyLiteExceptionManager.HandleException(ex);
            manager.RollbackTransaction();
            UpdatePayrollStatus(payrollId, "Failed");
            return false;
        }
        finally
        {
            if (manager != null)
                manager.Dispose();
        }

    }

    private bool InsertPayslipInfo(DBManager manager, DataTable dtPaySlipInfo, int payrollId, int month, int year)
    {
        string dbQry = string.Empty;

        try
        {
            foreach (DataRow dr in dtPaySlipInfo.Rows)
            {
                int employeeNo = 0;
                int.TryParse(dr[0].ToString(), out employeeNo);
                dbQry = string.Format(@"INSERT INTO tblEmployeePayslip (EmployeeId,PayrollDate,PayrollMonth,Deductions,Payments,PayrollId,PayrollYear,LossOfPayDays) 
                                    VALUES ({0},'{1}',{2},{3},{4},{5},{6},{7})", employeeNo, DateTime.Now.Date, month, dr[1], dr[2], payrollId, year, dr["LossOfPayDays"]);

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

    private void InsertPayrollLog(string logMessage, int payrollId, int employeeNo)
    {
        DBManager manager = new DBManager(DataProvider.OleDb);
        manager.ConnectionString = CreateConnectionString(this.ConnectionString);
        string dbQry = string.Empty;
        try
        {
            manager.Open();
            dbQry = string.Format(@"INSERT INTO tblPayrollGenerationLog (PayrollId,EmployeeNo,Message) 
                                    VALUES ({0},{1},'{2}')", payrollId, employeeNo, logMessage);

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
            DataTable dtEmpLeavesApplied = GetEmployeeLOPLeavesAppliedForTheMonth(employeeNo, year, month);
            double totalLeavesDaysAppliedLeaves = GetTotalLeavesInTheLeaveSummary(dtEmpLeavesApplied, year, month);

            int totalDaysInTheMonth = DateTime.DaysInMonth(year, month);



            payslipInfoRow[0] = employeeNo;
            payslipInfoRow[1] = totalDeductions;
            payslipInfoRow[2] = totalPayable;
            payslipInfoRow[3] = totalLeavesDaysAppliedLeaves;

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
        int componentTotalPay = 0;
        try
        {
            manager.Open();

            dbQry = string.Format(@"Select SUM(pcm.DeclaredAmount) as ComponentTotalPay 
                                    FROM tblPayComponentEmployeeMapping pcm
                                    INNER JOIN tblPayComponents pc ON pc.PayComponentId=pcm.PayComponentId                                    
                                    WHERE pcm.EmployeeId={0} AND pc.PayComponentType_id={1} AND pc.IsDeduction=False", employeeNo, 2);
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

            dbQry = string.Format(@"Select SUM(pcm.DeclaredAmount) as TotalDeduction 
                                    FROM tblPayComponentEmployeeMapping pcm
                                    INNER JOIN tblPayComponents pc ON pc.PayComponentId=pcm.PayComponentId                                    
                                    WHERE pcm.EmployeeId={0} AND pc.PayComponentType_id={1} AND pc.IsDeduction=True", employeeNo, 2);

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
        logMessage = string.Empty;
        if (HaveUnApprovedLeavesForTheMonth(empNo, year, month, ref  logMessage).Equals(false) ||
            HaveAppliedTheLeavesTaken(empNo, year, month, ref  logMessage).Equals(false))
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    private bool HaveUnApprovedAttendanceBySupervisor(int year, int month, ref string logMessage)
    {
        string dbQuery = string.Format(@"Select a.Remarks,Count(AttendanceDate) from ((tblAttendanceDetail a
                                            INNER JOIN tblEmployee e1 ON a.EmployeeNo = e1.EmpNo)
                                            INNER JOIN tblEmployee e2 ON e1.ManagerID = e2.EmpNo)
                                        WHERE a.EmployeeNo = {0} AND
                                            (YEAR(a.AttendanceDate)={1} AND MONTH(a.AttendanceDate)={2})
                                        GROUP BY a.Remarks", year, month, "Approved");


        DBManager manager = new DBManager(DataProvider.OleDb);
        manager.ConnectionString = CreateConnectionString(this.ConnectionString);
        manager.Open();
        DataSet ds = manager.ExecuteDataSet(CommandType.Text, dbQuery);
        if (ds != null && ds.Tables.Count > 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private bool HaveAppliedTheLeavesTaken(int empNo, int year, int month, ref string logMessage)
    {
        DataTable dtEmpAttendanceSummary = GetEmployeeAttendanceSummaryForTheMonth(empNo, year, month);
        DataTable dtEmpLeavesApplied = GetEmployeeLeavesAppliedForTheMonth(empNo, year, month);
        if (dtEmpAttendanceSummary != null && dtEmpAttendanceSummary != null)
        {
            double totalLeaveDaysByAttendance = GetTotalLeavesInTheAttendance(dtEmpAttendanceSummary);
            double totalLeavesDaysAppliedLeaves = Math.Floor(GetTotalLeavesInTheLeaveSummary(dtEmpLeavesApplied, year, month));
            if (!totalLeaveDaysByAttendance.Equals(totalLeavesDaysAppliedLeaves))
            {
                logMessage += string.Format("\r\nLeave entries and attendance mismatching for the employee '{0}{1}' (Attendance-{2} LeavesApplied-{3})", empNo, string.Empty, totalLeaveDaysByAttendance, totalLeavesDaysAppliedLeaves);
                return false;
            }
            else
            {
                return true;
            }
        }
        return true;
    }

    private double GetTotalLeavesInTheAttendance(DataTable dtEmpAttendanceSummary)
    {
        double totalLeavesByAttendance = 0;
        DataRow[] drLeaves = dtEmpAttendanceSummary.Select("Remarks='Leave'");
        if (drLeaves.Count() > 0)
        {
            if (double.TryParse(drLeaves[0][1].ToString(), out totalLeavesByAttendance))
            {
                return totalLeavesByAttendance;
            }
        }
        return totalLeavesByAttendance;
    }

    private double GetTotalLeavesInTheLeaveSummary(DataTable dtEmpLeavesApplied, int year, int month)
    {
        double totalLeavesDaysAppliedLeaves = 0;
        foreach (DataRow drLeaveEntry in dtEmpLeavesApplied.Rows)
        {
            DateTime startDate, endDate;
            string startDateSession, endDateSession;
            DateTime.TryParse(drLeaveEntry["StartDate"].ToString(), out startDate);
            DateTime.TryParse(drLeaveEntry["EndDate"].ToString(), out endDate);
            startDateSession = drLeaveEntry["StartDateSession"].ToString();
            endDateSession = drLeaveEntry["EndDateSession"].ToString();
            if (!startDate.Year.Equals(year) || !startDate.Month.Equals(month))
            {
                startDate = DateTime.Parse(string.Format("{0}-{1}-{2}", year, month, "01"));
                startDateSession = "FN";
            }
            if (!endDate.Year.Equals(year) || !endDate.Month.Equals(month))
            {
                endDate = DateTime.Parse(string.Format("{0}-{1}-{2}", year, month, DateTime.DaysInMonth(year, month)));
                endDateSession = "AN";
            }
            totalLeavesDaysAppliedLeaves += CalculateTotalLeaveDays(startDate, startDateSession, endDate, endDateSession);
        }
        return totalLeavesDaysAppliedLeaves;
    }

    private DataTable GetEmployeeLeavesAppliedForTheMonth(int empNo, int year, int month)
    {
        DBManager manager = new DBManager(DataProvider.OleDb);
        manager.ConnectionString = CreateConnectionString(this.ConnectionString);
        DataSet ds = new DataSet();
        string dbQry = string.Empty;


        dbQry = string.Format(@"SELECT a.LeaveId, a.EmployeeNo, a.StartDate,a.StartDateSession, a.EndDate,a.EndDateSession,a.TotalDays
                                FROM tblEmployeeLeave a
                                WHERE a.EmployeeNo ={0}
                                AND (YEAR(a.StartDate) = {1} AND MONTH(a.StartDate) = {2}) 
                                        OR (YEAR(a.EndDate) = {1} AND MONTH(a.EndDate) = {2})", empNo, year, month);

        try
        {
            manager.Open();
            ds = manager.ExecuteDataSet(CommandType.Text, dbQry);

            if (ds != null && ds.Tables.Count > 0)
            {
                return ds.Tables[0];
            }
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

    private DataTable GetEmployeeLOPLeavesAppliedForTheMonth(int empNo, int year, int month)
    {
        DBManager manager = new DBManager(DataProvider.OleDb);
        manager.ConnectionString = CreateConnectionString(this.ConnectionString);
        DataSet ds = new DataSet();
        string dbQry = string.Empty;


        dbQry = string.Format(@"SELECT a.LeaveId, a.EmployeeNo, a.StartDate,a.StartDateSession, a.EndDate,a.EndDateSession,a.TotalDays
                                FROM (tblEmployeeLeave a 
                                        INNER JOIN tblLeaveTypes l on a.LeaveTypeId = l.ID)
                                WHERE a.EmployeeNo ={0}
                                AND l.IsPayable=False
                                AND (YEAR(a.StartDate) = {1} AND MONTH(a.StartDate) = {2}) 
                                        OR (YEAR(a.EndDate) = {1} AND MONTH(a.EndDate) = {2})", empNo, year, month);

        try
        {
            manager.Open();
            ds = manager.ExecuteDataSet(CommandType.Text, dbQry);

            if (ds != null && ds.Tables.Count > 0)
            {
                return ds.Tables[0];
            }
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

    private DataTable GetEmployeeAttendanceSummaryForTheMonth(int empNo, int year, int month)
    {
        string dbQuery = string.Format(@"Select a.Remarks,Count(AttendanceDate) from ((tblAttendanceDetail a
                                            INNER JOIN tblEmployee e1 ON a.EmployeeNo = e1.EmpNo)
                                            INNER JOIN tblEmployee e2 ON e1.ManagerID = e2.EmpNo)
                                        WHERE a.EmployeeNo = {0} AND
                                            (YEAR(a.AttendanceDate)={1} AND MONTH(a.AttendanceDate)={2})
                                        GROUP BY a.Remarks", empNo, year, month, "Approved");


        DBManager manager = new DBManager(DataProvider.OleDb);
        manager.ConnectionString = CreateConnectionString(this.ConnectionString);
        manager.Open();
        DataSet ds = manager.ExecuteDataSet(CommandType.Text, dbQuery);
        if (ds != null && ds.Tables.Count > 0)
        {
            return ds.Tables[0];
        }
        else
        {
            return null;
        }

    }

    private bool HaveUnApprovedLeavesForTheMonth(int empNo, int year, int month, ref string logMessage)
    {
        string dbQuery = string.Format(@"Select a.EmployeeNo,e1.EmpFirstName as EmpName,a.Approver,e2.EmpFirstName as ApproverName from ((tblEmployeeLeave a
                                            INNER JOIN tblEmployee e1 ON a.EmployeeNo = e1.EmpNo)
                                            INNER JOIN tblEmployee e2 ON a.Approver = e2.EmpNo)
                            WHERE a.EmployeeNo = {0} AND
                                    ((YEAR(a.StartDate)={1} OR YEAR(a.EndDate)={1}) AND (MONTH(a.StartDate)={2} OR MONTH(a.EndDate)={2}))
                                        AND a.Status<>'{3}'", empNo, year, month, "Approved");


        DBManager manager = new DBManager(DataProvider.OleDb);
        manager.ConnectionString = CreateConnectionString(this.ConnectionString);
        manager.Open();
        DataSet ds = manager.ExecuteDataSet(CommandType.Text, dbQuery);
        if (ds != null && ds.Tables.Count > 0)
        {
            if (ds.Tables[0].Rows.Count > 0)
            {
                DataRow dRow = ds.Tables[0].Rows[0];
                logMessage += string.Format("\r\n Leaves are not approved for the employee '{0}-{1}'.{Approver: {2}-{3}}", dRow["EmployeeNo"].ToString(), dRow["EmpName"].ToString(), dRow["Approver"].ToString(), dRow["ApproverName"].ToString());
                return false;
            }
        }


        return true;
    }

    private double CalculateTotalLeaveDays(DateTime StartDate, string StartDateSession, DateTime EndDate, string EndDateSession)
    {
        double totalLeaveDays = 0;
        int dateDiffDays = new DateTimeHelper.DateDifference(StartDate, EndDate).Days;
        if (dateDiffDays.Equals(0))
        {
            if (StartDateSession.Equals("FN") && EndDateSession.Equals("AN"))
            {
                totalLeaveDays = 1.0;
            }
            else if (StartDateSession.Equals("FN") && EndDateSession.Equals("FN"))
            {
                totalLeaveDays = 0.5;
            }
            else if (StartDateSession.Equals("AN") && EndDateSession.Equals("AN"))
            {
                totalLeaveDays = 0.5;
            }
        }
        else
        {
            if (StartDateSession.Equals("FN") && EndDateSession.Equals("AN"))
            {
                totalLeaveDays = dateDiffDays + 1;
            }
            else if (StartDateSession.Equals("FN") && EndDateSession.Equals("FN"))
            {
                totalLeaveDays = dateDiffDays + 0.5;
            }
            else if (StartDateSession.Equals("AN") && EndDateSession.Equals("AN"))
            {
                totalLeaveDays = dateDiffDays + 0.5;
            }
            else if (StartDateSession.Equals("AN") && EndDateSession.Equals("FN"))
            {
                totalLeaveDays = dateDiffDays;
            }
        }
        return totalLeaveDays;
    }
    #endregion
}