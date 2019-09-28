using System;

namespace ImStor.Models
{
    public class Image
    {
        public Guid Md5 { get; set; }
        public string Ext { get; set; }
        public string Mime { get; set; }
        public byte[] Data { get; set; }
    }
}
