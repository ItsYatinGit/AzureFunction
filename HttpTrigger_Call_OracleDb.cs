using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http; 
using System.Data;
using Oracle.ManagedDataAccess.Client;
using Microsoft.Extensions.Configuration;

namespace NZMR.Function
{
    public static class HttpTrigger_Call_OracleDb
    {
        /// <summary>
            ///Function name: HttpTrigger
            ///Trigger type: HTTP
            ///HTTP method: GET
            ///Route: /api/HttpTrigger
            ///Functionality: Calls the CallStoredProcedure method of the OracleDbHelper class to execute a stored procedure and logs the result and returns the result as a DataTable.
        /// </summary>


        [FunctionName("HttpTrigger_Call_OracleDb")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequest req,
            ILogger logParam, IConfiguration configParam)
        {
            // Log the start of the function execution, here logParam is a temporary variable of a function, not a class variable
            logParam.LogInformation($"Azure Function 'HttpTrigger_Call_OracleDb' started at {DateTime.Now}");

            try
            {
                // Create an instance of the OracleDbHelper class
                OracleDbHelper dbHelper = new OracleDbHelper(logParam, configParam);

                // Call the CallStoredProcedure method on a background thread
                DataTable data = await Task.Run(() =>
                {
                    // Log the start of the stored procedure execution
                    logParam.LogInformation("Calling stored procedure in Oracle DB...");

                    // Call the stored procedure
                    DataTable result = dbHelper.CallStoredProcedure();

                    // Log the completion of the stored procedure execution
                    logParam.LogInformation("Stored procedure execution completed.");

                    return result;
                });

                // Log the number of rows fetched from Oracle
                logParam.LogInformation($"Fetched data from Oracle: {data.Rows.Count} rows");

                // Return the fetched data as an OkObjectResult
                return new OkObjectResult(data);
            }
            catch (OracleException ex)
            {
                // Log the Oracle exception
                logParam.LogError(ex, "Oracle exception occurred while fetching data from Oracle.");

                // Return a 500 Internal Server Error response
                return new StatusCodeResult(500);
            }
            catch (Exception ex)
            {
                // Log the exception
                logParam.LogError(ex, "Error occurred while fetching data from Oracle.");

                // Return a 500 Internal Server Error response
                return new StatusCodeResult(500);
            }
            finally
            {
                // Log the completion of the function execution
                logParam.LogInformation($"Azure Function 'HttpTrigger_Call_OracleDb' execution finished at {DateTime.Now}");
            }
        }
    }
}