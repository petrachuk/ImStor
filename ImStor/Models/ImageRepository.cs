using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Npgsql;
using NpgsqlTypes;

namespace ImStor.Models
{
    public class ImageRepository : IImageRepository
    {
        private bool disposed = false;
        private NpgsqlConnection Connection { get; }

        public ImageRepository(IConfiguration configuration)
        {
            Connection = new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection"));
            Connection.Open();
        }


        public Image FindById(int id)
        {
            byte[] data = {};

            using (var command = Connection.CreateCommand())
            {
                command.CommandText = "SELECT T.data FROM data_inside AS T WHERE T.id = :id";
                command.CommandType = CommandType.Text;

                command.Parameters.Add(new NpgsqlParameter("id", NpgsqlDbType.Integer)).Value = id;

                using (var reader = command.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            data = (byte[]) reader.GetValue(0);
                        }
                    }
                }
            }

            return new Image {ImageId = id.ToString(), Data = data};
        }

        #region IDisposable
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposed) return;

            if (disposing)
            {
                Connection?.Dispose();
            }

            disposed = true;
        }

        ~ImageRepository()
        {
            Dispose(false);
        }
        #endregion
    }
}
