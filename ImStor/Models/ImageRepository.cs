using System;
using System.Data;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Npgsql;
using Dapper;

namespace ImStor.Models
{
    public class ImageRepository : IImageRepository
    {
        private string ConnectionString { get; }

        public ImageRepository(IConfiguration configuration)
        {
            ConnectionString = configuration.GetConnectionString("DefaultConnection");
        }

        internal IDbConnection Connection => new NpgsqlConnection(ConnectionString);

        public async Task<Image> CreateAsync(Image item)
        {
            using var connection = Connection;
            connection.Open();

            try
            {
                item.Md5 = await connection.ExecuteScalarAsync<Guid>(
                    "INSERT INTO td_images (md5, type, data) VALUES(CAST(md5(:data) AS uuid), (SELECT T.id FROM tr_types AS T WHERE T.mime = :mime), :data) RETURNING md5",
                    new {item.Data, item.Mime});
            }
            catch (PostgresException ex)
            {
                if (ex.SqlState != "23505") throw;

                item.Md5 = await connection.ExecuteScalarAsync<Guid>("SELECT CAST(md5(:data) AS uuid)",
                    new {item.Data});
            }

            return item;
        }

        public async Task<Image> GetAsync(string md5, int size)
        {
            using var connection = Connection;
            connection.Open();

            Image result = null;
            try
            {
                result = await connection.QueryFirstOrDefaultAsync<Image>("SELECT T.ext, T.mime, I.data FROM public.td_images AS I JOIN public.tr_types AS T ON I.type = T.id WHERE I.md5 = CAST(:md5 AS uuid) AND I.size = :size", new { md5, size });
            }
            catch
            {
                // ignored
            }

            return result;
        }
    }
}
