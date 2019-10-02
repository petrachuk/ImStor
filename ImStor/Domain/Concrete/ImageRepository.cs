using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using ImStor.Domain.Abstract;
using ImStor.Domain.Entity;
using System.Collections.Generic;
using System.Linq;
using Dapper;

namespace ImStor.Domain.Concrete
{
    public class ImageRepository : Repository, IImageRepository
    {
        public ImageRepository(IConfiguration configuration) : base (configuration)
        {
        }

        #region IRepository
        public async Task<int> CreateAsync(Image item)
        {
            using var connection = Connection;
            connection.Open();

            // Для подстраховки
            if (item.Md5 == Guid.Empty) item.Md5 = item.Data.GetMd5Hash();

            // Поищем в загруженных
            var exists = await FindByMd5AndSizeAsync(item.Md5, 0);
            if (exists != null) return exists.Id;

            return await connection.ExecuteScalarAsync<int>(
                "INSERT INTO td_images (md5, size, type, created, data) VALUES (:md5, :size, :type, :created, :data) RETURNING id",
                item);
        }

        public Task<Image> FindByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Image>> GetAsync()
        {
            throw new NotImplementedException();
        }

        public Task RemoveAsync(Image item)
        {
            throw new NotImplementedException();
        }

        public Task UpdateAsync(Image item)
        {
            throw new NotImplementedException();
        }
        #endregion

        public async Task<Image> FindByMd5AndSizeAsync(Guid md5, int size)
        {
            using var connection = Connection;
            connection.Open();

            return await connection.QueryFirstOrDefaultAsync<Image>(
                "SELECT T.id, T.md5, T.size, T.type, T.created, T.data FROM td_images AS T WHERE T.md5 = :md5 AND T.size = :size",
                new {md5, size});
        }
    }
}
