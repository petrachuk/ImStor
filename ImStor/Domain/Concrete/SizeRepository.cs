using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ImStor.Domain.Abstract;
using ImStor.Domain.Entity;
using Microsoft.Extensions.Configuration;

namespace ImStor.Domain.Concrete
{
    public class SizeRepository : Repository, ISizeRepository
    {
        public SizeRepository(IConfiguration configuration) : base(configuration)
        {
        }

        #region IRepository
        public Task<int> CreateAsync(Size item)
        {
            throw new NotImplementedException();
        }

        public Task<Size> FindByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Size>> GetAsync()
        {
            throw new NotImplementedException();
        }

        public Task RemoveAsync(Size item)
        {
            throw new NotImplementedException();
        }

        public Task UpdateAsync(Size item)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
