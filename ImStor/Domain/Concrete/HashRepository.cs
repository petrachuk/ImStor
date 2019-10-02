using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Extensions.Configuration;
using ImStor.Domain.Abstract;
using ImStor.Domain.Entity;

namespace ImStor.Domain.Concrete
{
    public class HashRepository : Repository, IHashRepository
    {
        public HashRepository(IConfiguration configuration) : base(configuration)
        {
        }

        #region IRepository
        public async Task<int> CreateAsync(Hash item)
        {
            using var connection = Connection;
            connection.Open();

            var exists = await connection.QueryFirstOrDefaultAsync<Hash>("SELECT T.id, T.ahash, T.phash, T.dhash FROM td_hashes AS T WHERE T.id = :id", item);
            if (exists != null) return exists.Id;

            await connection.ExecuteAsync("INSERT INTO td_hashes (id, ahash, phash, dhash) VALUES (:id, :ahash, :phash, :dhash)", item);

            return item.Id;
        }

        public Task<Hash> FindByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Hash>> GetAsync()
        {
            throw new NotImplementedException();
        }

        public Task RemoveAsync(Hash item)
        {
            throw new NotImplementedException();
        }

        public async Task UpdateAsync(Hash item)
        {
            using var connection = Connection;
            connection.Open();

            await connection.ExecuteAsync("UPDATE td_hashes SET ahash = :ahash, phash = :phash, dhash = :dhash WHERE id = :id", item);
        }
        #endregion
    }
}
