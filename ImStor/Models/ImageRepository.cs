using System.Data;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Npgsql;
using Dapper;

namespace ImStor.Models
{
    public class ImageRepository : IImageRepository
    {
        private readonly string connectionString;

        public ImageRepository(IConfiguration configuration)
        {
            connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        internal IDbConnection Connection => new NpgsqlConnection(connectionString);

        public void Create(Image item)
        {
            using IDbConnection connection = Connection;
            connection.Open();
            connection.ExecuteAsync("INSERT INTO customer (name,phone,email,address) VALUES(@Name,@Phone,@Email,@Address)", item);
        }

        public async Task<Image> FindById(int id)
        {
            using IDbConnection connection = Connection;
            connection.Open();
            return await connection.QueryFirstOrDefaultAsync<Image>("SELECT * FROM customer WHERE id = @Id", new { Id = id });
        }
    }
}
