using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace ImgurRandom
{
    class Program
    {
        private static readonly Random Random = new Random();
        private static readonly string BaseUri = "https://www.i.imgur.com/";
        private static readonly char[] Chars = "0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray();
        private static readonly int CharsAvailable = Chars.Length;
        private static readonly HttpClient HttpClient = new HttpClient();
        private static int ThreadCount = 0;

        static async Task Main()
        {
            ServicePointManager.ServerCertificateValidationCallback += (sender, cert, chain, sslPolicyErrors) => true;
            await TestUrlLoop();
        }

        private static async Task TestUrlLoop()
        {
            while (ThreadCount < 10)
            {
                Task.Run(async () =>
                    {
                        while (true)
                        {
                            var url = RString(7);
                            var image = await TestUrl(BaseUri + url + ".png");
                            if (image.Height != 81) // shitty filtering for image not found image, should probably just keep a local copy and do a byte comparison or sum like that.
                            {
                                Console.WriteLine("SAVING FILE "+url);
                                image.Save($"{url}.png", ImageFormat.Png);
                            }
                        }
                    }
                );
                ThreadCount++;
            }
            await Task.Delay(-1);
        }

        private static async Task<Image> TestUrl(string url)
        {
            var httpReq = await HttpClient.GetByteArrayAsync(url);
            Console.WriteLine(url);
            MemoryStream mStream = new MemoryStream(httpReq);
            return Image.FromStream(mStream);
        }

        private static string RString(int chars)
        {
            var bytes = new byte[chars];
            var result = new char[chars];
            Random.NextBytes(bytes);
            while (chars-- > 0)
            {
                result[chars] = Chars[bytes[chars] % CharsAvailable];
            }
            return new string(result);
        }
    }
}
