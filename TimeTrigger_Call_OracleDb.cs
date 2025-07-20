using System;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using System.Data;
using Microsoft.Extensions.Configuration;

namespace NZMR.Function
{
    public static class TimeTrigger_Call_OracleDb
    {
             
        ///<summary>
            ///Function name: TimerTrigger
            ///Trigger type: Timer
            ///Schedule: Every hours
            ///Functionality: Calls the CallStoredProcedure method of the OracleDbHelper class to execute a stored procedure and logs the result and returns the result as a DataTable. /// 
       ///</summary>


  [FunctionName("TimeTrigger_Call_OracleDb")]
public static async Task Run(
    [TimerTrigger("0 * * * *")] TimerInfo myTimer, // runs every hour
    ILogger logParam, IConfiguration configParam)
        {
            // Log the start of the function execution, here logParam is a temporary variable of a function, not a class variable
            logParam.LogInformation($"Azure Function 'TimeTrigger_Call_OracleDb' started at {DateTime.Now}");

            try
            {
                // Create an instance of the OracleDbHelper class
                OracleDbHelper dbHelper = new OracleDbHelper(logParam, configParam);

                // Log the start of the stored procedure execution
                logParam.LogInformation("Calling stored procedure in Oracle DB...");

                // Call the stored procedure on a background thread
                DataTable data = await Task.Run(() =>
                {
                    // Call the stored procedure
                    DataTable result = dbHelper.CallStoredProcedure();

                    // Log the completion of the stored procedure execution
                    logParam.LogInformation("Stored procedure execution completed.");

                    return result;
                });

                // Log the number of rows fetched from Oracle
                logParam.LogInformation($"Fetched data from Oracle: {data.Rows.Count} rows");

                // You can store the fetched data into Dataverse tables here
            }
            catch (Oracle.ManagedDataAccess.Client.OracleException ex)
            {
                // Log the Oracle exception
                logParam.LogError(ex, "Oracle exception occurred while fetching data from Oracle.");

                // Return a failure status
                throw;
            }
            catch (Exception ex)
            {
                // Log the exception
                logParam.LogError(ex, "Error occurred while fetching data from Oracle.");

                // Return a failure status
                throw;
            }
            finally
            {
                // Log the completion of the function execution
                logParam.LogInformation($"Azure Function 'TimeTrigger_Call_OracleDb' execution finished at {DateTime.Now}");
            }
        }
    }
}