using System.Threading.Tasks;
using ImStor.Models;
using Microsoft.AspNetCore.Mvc;

namespace ImStor.Controllers
{
    [ApiController]
    [Route("/")]
    public class ImageController
    {
        private IImageRepository Repository { get; }

        public ImageController(IImageRepository repository)
        {
            Repository = repository;
        }

        [HttpGet("{i}/{j}/{k}/{l}/{m}")]
        public Task Get(string i, string j, string k, string l, string m)
        {
            var id = string.Concat(i, j, k, l, m);

            var temp = Repository.FindById(1);

            return null;
        }
    }
}
