using System.IO;
using System.Net.Http;
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
        private IHttpClientFactory HttpClientFactory { get; }

        public ImageController(IImageRepository repository, IConfiguration configuration, IHttpClientFactory clientFactory)
        {
            Repository = repository;
            Configuration = configuration;
            HttpClientFactory = clientFactory;
        }

        [HttpPost("add")]
        public async Task<ActionResult> Post()
        {
            var mime = Request.ContentType;
            var callbackUrl = Request.Headers.ContainsKey("X-CallbackUrl") ? Request.Headers["X-CallbackUrl"].ToString() : null;
            var fileName = Request.Headers.ContainsKey("X-FileName") ? Request.Headers["X-FileName"].ToString() : null;

            // Передадим картинку на обработку в сервис распознования
            Task task = Task.Run(async () =>
            {
                using var client = HttpClientFactory.CreateClient();
                using var request = new HttpRequestMessage(HttpMethod.Post, Configuration.GetValue<string>("HashService"));
                using var response = await client.SendAsync(request);

                if (response.IsSuccessStatusCode)
                {
                    // Обновим данные в БД картинок


                    // Передадим загружающему изображение
                    if (!string.IsNullOrWhiteSpace(callbackUrl))
                    {
                        using var returnRequest = new HttpRequestMessage(HttpMethod.Post, callbackUrl);
                        await client.SendAsync(returnRequest);
                    }
                }
            });

            byte[] data;
            await using (var ms = new MemoryStream())
            {
                await Request.Body.CopyToAsync(ms);
                data = ms.ToArray();
            }

            var md5 = await Repository.CreateAsync(new Image {Data = data, Mime = mime});

            var name = md5.Md5.ToString().Replace("-", "").Insert(8, "/").Insert(6, "/").Insert(4, "/").Insert(2, "/");
            
            var ext = mime switch
            {
                "image/gif" => "gif",
                "image/png" => "png",
                "image/svg+xml" => "svg",
                _ => "jpg",
            };

            var uri = $"{Configuration.GetValue<string>("BaseUri")}{name}.{ext}";

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
