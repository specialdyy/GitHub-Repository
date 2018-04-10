using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public static class GZipHelper
    {
        public static string DecodeGzipStream(this Stream gzipContent)
        {
            if(gzipContent == null)
            {
                throw new ArgumentNullException("gzipContent");
            }

            using (MemoryStream dms = new MemoryStream())
            {
                using (GZipStream gzip = new GZipStream(gzipContent, CompressionMode.Decompress))
                {
                    StreamReader reader = new StreamReader(gzip);
                    return reader.ReadToEnd();
                }
                //using (GZipStream gzip = new GZipStream(response.GetResponseStream(), CompressionMode.Decompress))
                //{
                //    byte[] bytes = new byte[1024];
                //    int len = 0;
                //    //读取压缩流，同时会被解压
                //    while ((len = gzip.Read(bytes, 0, bytes.Length)) > 0)
                //    {
                //        dms.Write(bytes, 0, len);
                //    }
                //}
                //string html = Encoding.UTF8.GetString(dms.ToArray());
            }
        }
    }
}
