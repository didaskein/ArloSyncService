using ArloSyncService.Common.Configuration.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArloSyncService.Logic.Listener
{
    public class SSEListener(ArloConfiguration ArloConfiguration)
    {
        public bool isRunning { get; set; }

        public async Task StartSSEListenRequestAsync(string token, CancellationToken cancellationToken)
        {
            string url = $"{ArloConfiguration.ArloBaseApiUri}/hmsweb/client/subscribe";

            //https://www.codemag.com/Article/2309051/Developing-Real-Time-Web-Applications-with-Server-Sent-Events-in-ASP.NET-7-Core
            //https://medium.com/@kova98/server-sent-events-in-net-7f700b21cdb7
            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("Authorization", token);
                client.DefaultRequestHeaders.Add("Auth-Version", "2");
                client.DefaultRequestHeaders.Add("Accept", "text/event-stream");
                client.DefaultRequestHeaders.Add("Cache-Control", "no-cache");
                client.DefaultRequestHeaders.Add("Pragma", "no-cache");

                client.DefaultRequestHeaders.Add("Accept-Encoding", "gzip, deflate, br");
                client.DefaultRequestHeaders.Add("Accept-Language", "fr,fr-FR;q=0.8,en-US;q=0.5,en;q=0.3");
                client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:126.0) Gecko/20100101 Firefox/126.0");

                client.DefaultRequestHeaders.Add("DNT", "1");
                client.DefaultRequestHeaders.Add("SchemaVersion", "1");

                client.DefaultRequestHeaders.Add("Origin", "https://my.arlo.com");
                client.DefaultRequestHeaders.Add("Referer", "https://my.arlo.com/");

                while (!cancellationToken.IsCancellationRequested)
                {
                    isRunning = true;
                    try
                    {
                        Console.WriteLine("Establishing connection" + " with the server.");
                        using (var streamReader = new StreamReader(await client.GetStreamAsync(url)))
                        {
                            while (!streamReader.EndOfStream)
                            {
                                var message = await streamReader.ReadToEndAsync();
                                Console.WriteLine(message);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        isRunning = false;
                        throw;
                    }
                }
            }

        }
    }
}
