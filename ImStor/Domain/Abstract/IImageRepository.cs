using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ImStor.Domain.Entity;

namespace ImStor.Domain.Abstract
{
    public interface IImageRepository : IRepository<Image>
    {
        Task<Image> FindByMd5AndSizeAsync(Guid md5, int size);
    }
}
