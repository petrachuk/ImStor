using System.Data;
using Microsoft.Extensions.Configuration;
using Npgsql;

namespace ImStor.Domain.Abstract
{
    public abstract class Repository
    {
        internal string ConnectionString { get; }

        protected Repository(IConfiguration configuration)
        {
            ConnectionString = configuration.GetConnectionString("DefaultConnection");
        }

        internal IDbConnection Connection => new NpgsqlConnection(ConnectionString);
    }
}
