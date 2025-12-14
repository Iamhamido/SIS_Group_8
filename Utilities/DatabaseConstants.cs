// Utilities/DatabaseConstants.cs
namespace SIS.Utilities
{
    public static class DatabaseConstants
    {
        // Update these with your actual database credentials
        public const string Server = "localhost";
        public const string Database = "SIS_Group_8";
        public const string UserId = "root";
        public const string Password = "********";
        public const int Port = 3306;
        
        public static string ConnectionString => 
            $"server={Server};" +
            $"user={UserId};" +
            $"database={Database};" +
            $"port={Port};" +
            $"password={Password};";
    }
}