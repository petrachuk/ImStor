using System.Collections.Generic;
using System.Threading.Tasks;

namespace ImStor.Domain.Abstract
{
    public interface IRepository<TEntity> where TEntity : class
    {
        Task<int> CreateAsync(TEntity item);
        Task<TEntity> FindByIdAsync(int id);
        Task<IEnumerable<TEntity>> GetAsync();
        Task RemoveAsync(TEntity item);
        Task UpdateAsync(TEntity item);
    }
}
