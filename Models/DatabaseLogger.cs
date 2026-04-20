using System;
using System.Data.SqlClient;
using System.Diagnostics;

namespace Plazma.Models
{
    public static class DatabaseLogger
    {
        public static void Log(string source, string message, string stackTrace, string userName)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(Constants.bdconnectionstring))
                using (SqlCommand command = new SqlCommand(@"INSERT INTO LOGS ([Timestamp],[Source],[Message],[StackTrace],[UserName])
VALUES (@Timestamp,@Source,@Message,@StackTrace,@UserName)", connection))
                {
                    command.Parameters.AddWithValue("@Timestamp", DateTime.UtcNow);
                    command.Parameters.AddWithValue("@Source", (object)(source ?? string.Empty));
                    command.Parameters.AddWithValue("@Message", (object)(message ?? string.Empty));
                    command.Parameters.AddWithValue("@StackTrace", (object)(stackTrace ?? string.Empty));
                    command.Parameters.AddWithValue("@UserName", (object)(userName ?? string.Empty));
                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                Trace.TraceError("DatabaseLogger.Log error: " + ex);
            }
        }
    }
}
