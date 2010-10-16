using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace PowerPointController
{
    public static class StreamExtensions
    {
        public static byte[] ToArray(this Stream stream)
        {
            var memoryStream = new MemoryStream();
            int count;
            byte[] buffer = new byte[1024];
            while ((count = stream.Read(buffer, 0, buffer.Length)) > 0)
                memoryStream.Write(buffer, 0, count);
            return memoryStream.ToArray();
        }
    }
}
