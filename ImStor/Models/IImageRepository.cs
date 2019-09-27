using System.Threading.Tasks;

namespace ImStor.Models
{
    public interface IImageRepository
    {
        void Create(Image item);
        Task<Image> FindById(int id);
    }
}
