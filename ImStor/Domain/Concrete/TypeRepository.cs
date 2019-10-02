using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Dapper;
using ImStor.Domain.Abstract;
using Microsoft.Extensions.Configuration;
using Type = ImStor.Domain.Entity.Type;

namespace ImStor.Domain.Concrete
{
    public class TypeRepository : Repository, ITypeRepository
    {
        public TypeRepository(IConfiguration configuration) : base(configuration)
        {
        }

        #region IRepository
        public Task<int> CreateAsync(Type item)
        {
            throw new NotImplementedException();
        }

        public async Task<Type> FindByIdAsync(int id)
        {
            using var connection = Connection;
            connection.Open();

            return await connection.QueryFirstOrDefaultAsync<Type>("SELECT T.id, T.name, T.ext, T.mime, T.description FROM tr_types AS T WHERE T.id = :id", new {id});
        }

        public async Task<IEnumerable<Type>> GetAsync()
        {
            using var connection = Connection;
            connection.Open();

            return await connection.QueryAsync<Type>("SELECT T.id, T.name, T.ext, T.mime, T.description FROM tr_types AS T");
        }

        public Task RemoveAsync(Type item)
        {
            throw new NotImplementedException();
        }

        public Task UpdateAsync(Type item)
        {
            throw new NotImplementedException();
        }
        #endregion

        public async Task<Type> FindByMimeAsync(string mime)
        {
            using var connection = Connection;
            connection.Open();

            return await connection.QueryFirstOrDefaultAsync<Type>("SELECT T.id, T.name, T.ext, T.mime, T.description FROM tr_types AS T WHERE T.mime = :mime", new { mime });
        }
    }
}