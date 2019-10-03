using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Xml;
using ImProc.Models;
using ImStor.Domain.Abstract;
using ImStor.Domain.Entity;
using ImStor.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace ImStor.Controllers
{
    [ApiController]
    [Route("/")]
    public class ImageController : ControllerBase
    {
        private IImageRepository ImageRepository { get; }
        private IHashRepository HashRepository { get; }
        private ITypeRepository TypeRepository { get; }
        private IConfiguration Configuration { get; }
        private IHttpClientFactory HttpClientFactory { get; }

        public ImageController(IImageRepository imageRepository, IHashRepository hashRepository, ITypeRepository typeRepository, IConfiguration configuration, IHttpClientFactory clientFactory)
        {
            ImageRepository = imageRepository;
            HashRepository = hashRepository;
            TypeRepository = typeRepository;
            Configuration = configuration;
            HttpClientFactory = clientFactory;
        }

        [HttpPost("add")]
        public async Task<ActionResult> Post()
        {
            // URL обратного вызова
            var callbackUrl = Request.Headers.ContainsKey("X-CallbackUrl") ? Request.Headers["X-CallbackUrl"].ToString() : null;

            // Получим информацию о файле
            var mime = Request.ContentType;
            if (mime == null) return StatusCode(415);   // Unsupported Media Type

            // var type = await TypeRepository.FindByMimeAsync(mime);
            // if (type == null) return StatusCode(415); // Unsupported Media Type

            // Сформируем объект Image
            var newImage = new Image
            {
                Size = 0,   // Исходный
                Type = 1,// type.Id,
                Created = DateTime.Now
            };

            await using (var ms = new MemoryStream())
            {
                await Request.Body.CopyToAsync(ms);
                newImage.Data = ms.ToArray();
            }

            newImage.Md5 = newImage.Data.GetMd5Hash();

            // Запрос на получение хешей
            //_ = Task.Run(async () =>
            {
                var hashServiceUrl = Configuration.GetValue<string>("HashService");

                if (!string.IsNullOrWhiteSpace(hashServiceUrl))
                {
                    using var client = HttpClientFactory.CreateClient();

                    // Передадим картинку на обработку в сервис распознования
                    using var request = new HttpRequestMessage(HttpMethod.Post, hashServiceUrl)
                    {
                        Content = new ByteArrayContent(newImage.Data)
                    };
                    request.Headers.Add("Accept", Request.Headers["Accept"].ToString());
                    request.Content.Headers.ContentType = new MediaTypeHeaderValue(mime);


                    using var response = await client.SendAsync(request);

                    if (response.IsSuccessStatusCode)
                    {
                        long ahash;
                        long phash;
                        long dhash;

                        if (response.Content.Headers.ContentType.MediaType == "application/json")
                        {
                            // var obj = Newtonsoft.Json.JsonConvert.DeserializeObject<HashResult>(await response.Content.ReadAsStringAsync());
                        }
                        else
                        {
                            var xml = new XmlDocument();
                            xml.LoadXml(await response.Content.ReadAsStringAsync());

                            foreach (XmlNode elem in xml.DocumentElement.ChildNodes)
                            {
                                var name = elem.Name;
                                var value = elem.InnerText;

                                switch (name)
                                {
                                    case "AHash":
                                        ahash = long.Parse(value);
                                        break;
                                    case "PHash":
                                        phash = long.Parse(value);
                                        break;
                                    case "DHash":
                                        dhash = long.Parse(value);
                                        break;
                                }
                            }
                        }

                        

                        


                        // Обновим данные в БД картинок
                        await HashRepository.UpdateAsync(new Hash());

                        // Передадим загружающему изображение
                        if (!string.IsNullOrWhiteSpace(callbackUrl))
                        {
                            using var returnRequest = new HttpRequestMessage(HttpMethod.Post, callbackUrl);
                            await client.SendAsync(returnRequest);
                        }
                    }
                }
            }
            //);

            // Сформируем записи в БД
            var imageId = await ImageRepository.CreateAsync(newImage);
            await HashRepository.CreateAsync(new Hash {Id = imageId});

            var fileName = newImage.Md5.ToString().Replace("-", "").Insert(8, "/").Insert(6, "/").Insert(4, "/").Insert(2, "/");
            
            var fileExt = mime switch
            {
                "image/gif" => "gif",
                "image/png" => "png",
                "image/svg+xml" => "svg",
                _ => "jpg",
            };
            
            return Ok(new UploadResult { Uri = $"{Configuration.GetValue<string>("BaseUri")}{fileName}.{fileExt}" });
        }

        [HttpGet("{id1}/{id2}/{id3}/{id4}/{id5}.{ext}")]
        [HttpGet("{id1}/{id2}/{id3}/{id4}/{id5}_{size}.{ext}")]
        public async Task<ActionResult> Get(string id1, string id2, string id3, string id4, string id5, int size, string ext)
        {
            var md5 = string.Concat(id1, id2, id3, id4, id5);

            var image = await ImageRepository.FindByMd5AndSizeAsync(new Guid(md5), size);
            if (image == null && size != 0) image = await ImageRepository.FindByMd5AndSizeAsync(new Guid(md5), 0);

            if (image == null) return NotFound();

            var type = await TypeRepository.FindByIdAsync(image.Type);

            var fileName = id5;
            if (size != 0) fileName += $"_{size}";
            fileName += $".{type.Ext}";

            return File(image.Data, type.Mime, fileName);
        }
    }
}
