using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ImStor.Models
{
    public interface IImageRepository : IDisposable
    {
        Image FindById(int id);
    }
}
