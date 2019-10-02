using System.Threading.Tasks;
using Type = ImStor.Domain.Entity.Type;

namespace ImStor.Domain.Abstract
{
    public interface ITypeRepository : IRepository<Type>
    {
        Task<Type> FindByMimeAsync(string mime);
    }
}
