using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PT_lab5
{
    static class Compression
    {
        public static void Compress(string sourceFile, string compressedFile)
        {
            // strumien odczytu
            using (FileStream sourceStream = new FileStream(sourceFile, FileMode.OpenOrCreate))
            {
                // strumien zapisu
                using (FileStream targetStream = File.Create(compressedFile))
                {
                    // strumien archiwacji
                    using (GZipStream compressionStream = new GZipStream(targetStream, CompressionMode.Compress))
                    {
                        sourceStream.CopyTo(compressionStream);
                    }
                }
            }
        }

        public static void Decompress(string compressedFile, string targetFile)
        {
            // strumien odczytu
            using (FileStream sourceStream = new FileStream(compressedFile, FileMode.OpenOrCreate))
            {
                // strumien zapisu
                using (FileStream targetStream = File.Create(targetFile))
                {
                    // strumien archiwacji
                    using (GZipStream decompressionStream = new GZipStream(sourceStream, CompressionMode.Decompress))
                    {
                        decompressionStream.CopyTo(targetStream);
                    }
                }
            }
        }
    }
}


