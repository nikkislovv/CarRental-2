using Microsoft.Data.SqlClient;
using System.Data;

namespace CatalogApi.Data
{
    public class CatalogContext
    {
        private readonly IConfiguration _configuration;
        private readonly string _connectionString;
        public CatalogContext(
            IConfiguration configuration)
        {
            _configuration = configuration;
            _connectionString = _configuration.GetConnectionString("DefaultConnection");
        }
        public IDbConnection CreateConnection()
            => new SqlConnection(_connectionString);
    }
}
