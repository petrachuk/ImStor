using System.IO;
using System.Threading.Tasks;
using ImStor.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace ImStor.Controllers
{
    [ApiController]
    [Route("/")]
    public class ImageController : ControllerBase
    {
        private IImageRepository Repository { get; }
        private IConfiguration Configuration { get; }

        public ImageController(IImageRepository repository, IConfiguration configuration)
        {
            Repository = repository;
            Configuration = configuration;
        }

        [HttpPost("add")]
        public async Task<ActionResult> Post()
        {
            byte[] data;
            await using (var ms = new MemoryStream())
            {
                await Request.Body.CopyToAsync(ms);
                data = ms.ToArray();
            }

            var mime = Request.ContentType;

            var md5 = await Repository.CreateAsync(new Image {Data = data, Mime = mime});

            var tmp = md5.Md5.ToString().Replace("-", "");
            tmp = tmp.Insert(8, "/");
            tmp = tmp.Insert(6, "/");
            tmp = tmp.Insert(4, "/");
            tmp = tmp.Insert(2, "/");
            
            var ext = mime switch
            {
                "image/gif" => "gif",
                "image/png" => "png",
                "image/svg+xml" => "svg",
                _ => "jpg",
            };

            var uri = $"{Configuration.GetValue<string>("BaseUri")}{tmp}.{ext}";

            return Ok(new {uri});
        }

        [HttpGet("{id1}/{id2}/{id3}/{id4}/{id5}.{ext}")]
        [HttpGet("{id1}/{id2}/{id3}/{id4}/{id5}_{size}.{ext}")]
        public async Task<ActionResult> Get(string id1, string id2, string id3, string id4, string id5, int size, string ext)
        {
            var md5 = string.Concat(id1, id2, id3, id4, id5);

            var image = await Repository.GetAsync(md5, size);

            if (image == null) return NotFound();

            var fileName = id5;
            if (size != 0) fileName += $"_{size}";
            fileName += $".{image.Ext}";

            return File(image.Data, image.Mime, fileName);
        }
    }
}
