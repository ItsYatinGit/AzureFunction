using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Data;


namespace NZMR.Function
{
    public static class OldFetch_OracleData
    {
        // This Azure Function is triggered by an HTTP GET request
        [FunctionName("OldFetch_OracleData")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)] HttpRequest req,
            ILogger log)
        {
            // Log the start of the function execution
            log.LogInformation("Azure Function 'fetch_OracleData' started.");
            log.LogInformation("Trigger: HTTP GET request received.");

            try
            {
                log.LogInformation("Starting Oracle database operation...");

                // Execute blocking DB call in background thread, 'await' tells the function to wait for the background task to finish before moving to the next line. It ensures your function doesn't return a result until the task is complete.
                DataTable result = await Task.Run(() =>
                {
                    log.LogInformation("Instantiating OracleDbHelper...");
                    OldOracleDbHelper dbHelper = new OldOracleDbHelper();

                    log.LogInformation("Calling stored procedure in Oracle DB...");
                    var data = dbHelper.CallStoredProcedure();
                    log.LogInformation("Stored procedure execution completed.");

                    return data;
                });

                log.LogInformation("Converting DataTable result to JSON...");
                string json = JsonConvert.SerializeObject(result);

                log.LogInformation("Returning data as HTTP 200 OK.");
                return new OkObjectResult(json);
            }
            catch (Exception ex)
            {
                // Capture and log exception details
                log.LogError(ex, "Error occurred while fetching data from Oracle.");
                
                return new ObjectResult("Error while fetching data from Oracle. Please check logs for more details.")
                {
                    StatusCode = StatusCodes.Status500InternalServerError
                };
            }
            finally
            {
                log.LogInformation("Azure Function 'fetch_OracleData' execution finished.");
            }
        }
    }
}
