using System.Text;
using System.IO;
using System.IO.Compression;
using bpulse_sdk_csharp.bpulsesConstants;

namespace bpulse_sdk_csharp.compress
{
    internal class CompressUtil
    {
        /// <summary>
        /// Texto a comprimir
        /// </summary>
        /// <param name="val">Valor a comprimir</param>
        /// <returns>Texto comprimido</returns>
        public static string compress(string val)
        {
            try
            {
                if (string.ReferenceEquals(val, null) || val.Length == 0)
                {
                    return val;
                }

                byte[] Valuebyte = Encoding.UTF8.GetBytes(val);
                MemoryStream @out = new MemoryStream();
                GZipStream gzip = new GZipStream(@out, CompressionMode.Compress);
                gzip.Write(Valuebyte, 0, Valuebyte.Length);
                gzip.Close();
                string outStr = Encoding.GetEncoding(BPulsesConstants.CHARSET_ISO88591).GetString(@out.ToArray());
                return outStr;
            }
            catch (IOException e)
            {
                throw e;
            }
        }
    }
}