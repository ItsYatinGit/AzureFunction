using System;
using System.Data;
using Oracle.ManagedDataAccess.Client;
using System.Diagnostics;
using Microsoft.Extensions.Logging;

public class OldOracleDbHelper
{
    // Connection string (move this to a secure config file or environment variable in production)
    private string connectionString = "User Id=IHUB_SERVICES;Password=IHUB_SERVICES;Data Source=YOUR_DSN";

    /// <summary>
    /// Calls the stored procedure 'generate_mm010_file_content' and returns the resulting DataTable.
    /// </summary>
    /// <returns>DataTable containing the fetched Oracle data</returns>
 

    public DataTable CallStoredProcedure()
    {
        DataTable dataTable = new DataTable();

        try
        {
            Trace.WriteLine("Initializing Oracle connection...");
            /// 'Using' statement will take care of closing the connection, It is already properly closing the connection when it's done,.
            using (OracleConnection conn = new OracleConnection(connectionString))
            {
                conn.Open();
                Trace.WriteLine("Oracle connection established.");

                using (OracleCommand cmd = new OracleCommand("generate_mm010_file_content", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    // Add output cursor parameter for the stored procedure
                    cmd.Parameters.Add("OUT_CURSOR", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

                    Trace.WriteLine("Executing stored procedure: generate_mm010_file_content");

                    using (OracleDataAdapter da = new OracleDataAdapter(cmd))
                    {
                        da.Fill(dataTable);
                        Trace.WriteLine($"Stored procedure executed successfully. Rows returned: {dataTable.Rows.Count}");
                    }
                }
            }
        }
        catch (OracleException ex)
        {
            // Log Oracle-specific exceptions
            Trace.WriteLine("OracleException occurred: " + ex.Message);
            Trace.WriteLine("Stack Trace: " + ex.StackTrace);
        }
        catch (Exception ex)
        {
            // Log general exceptions
            Trace.WriteLine("Exception occurred: " + ex.Message);
            Trace.WriteLine("Stack Trace: " + ex.StackTrace);
        }

        return dataTable;
    }
}

