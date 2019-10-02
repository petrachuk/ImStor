using System;

namespace ImStor.Domain.Entity
{
    public class Image
    {
        public int Id { get; set; }
        public Guid Md5 { get; set; }
        public int Size { get; set; }
        public int Type { get; set; }
        public DateTime Created { get; set; }
        public byte[] Data { get; set; }
    }
}
