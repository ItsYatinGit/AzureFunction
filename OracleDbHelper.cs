using System;
using System.Data;
using Oracle.ManagedDataAccess.Client;
using System.Diagnostics;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;

public class OracleDbHelper
{


    /// <summary>
    /// Calls the stored procedure 'generate_mm010_file_content' and returns the resulting DataTable.
    /// </summary>
    /// <returns>DataTable containing the fetched Oracle data</returns>
     

    // Here '_log' is a class variable which means it is a Private field that can be accessed by all methods in this class.
    private readonly ILogger _log;
    private readonly IConfiguration _configuration;
    private readonly string _connectionString;
    

    //Defining the "CONSTRUCTOR" to initialize the OracleDbHelper class with ILogger and IConfiguration
    // This constructor is used to inject dependencies, allowing the class to use logging and configuration settings
    public OracleDbHelper(ILogger logger, IConfiguration configuration)
    {
        // Store the ILogger instance from a Trigger function in a "private box" (_log) to keep it safe and secure, 
        // Only this class can access the '_log', and nobody else can touch it! therefore not using Public method (logger) this is called "encapsulation".
        _log = logger;   //// Private field(_log) to store the ILogger instance for encapsulation
        _configuration = configuration;
        _connectionString = _configuration.GetConnectionString("OracleDbConn");
    }



    public DataTable CallStoredProcedure()
    {
        DataTable dataTable = new DataTable();

        try
        {
            _log.LogInformation("Initializing Oracle connection...");
            Trace.WriteLine("Initializing Oracle connection...");

            /// 'Using' statement will take care of closing the connection, It is already properly closing the connection when it's done,.
            using (OracleConnection conn = new OracleConnection(_connectionString))
            {
                conn.Open();
                _log.LogInformation("Oracle connection established.");
                Trace.WriteLine("Oracle connection established.");

                using (OracleCommand cmd = new OracleCommand("generate_mm010_file_content", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    // Add output cursor parameter for the stored procedure
                    cmd.Parameters.Add("OUT_CURSOR", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

                    _log.LogInformation("Executing stored procedure: generate_mm010_file_content");
                    Trace.WriteLine("Executing stored procedure: generate_mm010_file_content");

                    using (OracleDataAdapter da = new OracleDataAdapter(cmd))
                    {
                        da.Fill(dataTable);
                        _log.LogInformation($"Stored procedure executed successfully. Rows returned: {dataTable.Rows.Count}");
                        Trace.WriteLine($"Stored procedure executed successfully. Rows returned: {dataTable.Rows.Count}");
                    }
                }
            }
        }
        catch (OracleException ex)
        {
            // Log Oracle-specific exceptions
            _log.LogError(ex, "OracleException occurred: {Message}", ex.Message);
            Trace.WriteLine("OracleException occurred: " + ex.Message);
            Trace.WriteLine("Stack Trace: " + ex.StackTrace);
        }
        catch (Exception ex)
        {
            // Log general exceptions
            _log.LogError(ex, "Exception occurred: {Message}", ex.Message);
            Trace.WriteLine("Exception occurred: " + ex.Message);
            Trace.WriteLine("Stack Trace: " + ex.StackTrace);
        }
        finally
        {
            _log.LogInformation("Stored procedure call completed.");
            Trace.WriteLine("Stored procedure call completed.");
        }

        return dataTable;
        
    }
}