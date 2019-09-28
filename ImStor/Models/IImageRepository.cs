using System.Threading.Tasks;

namespace ImStor.Models
{
    public interface IImageRepository
    {
        Task<Image> CreateAsync(Image item);
        Task<Image> GetAsync(string md5, int size);
    }
}
