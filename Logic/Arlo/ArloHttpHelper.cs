using ArloSyncService.Common.Configuration.Model;
using ArloSyncService.Common.Extensions;
using ArloSyncService.Common.Helper;
using ArloSyncService.Logic.Arlo.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace ArloSyncService.Logic.Arlo
{
    public class ArloHttpHelper
    {

        public ArloConfiguration ArloConfiguration { get; set; }
        public SecurityHelper SecurityHelper { get; set; }

        public ILogger<ArloHttpHelper> Logger { get; set; }

        public ArloHttpHelper(ArloConfiguration arloConfiguration, SecurityHelper securityHelper, ILogger<ArloHttpHelper> logger)
        {
            ArloConfiguration = arloConfiguration ?? throw new ArgumentNullException(nameof(arloConfiguration));
            SecurityHelper = securityHelper ?? throw new ArgumentNullException(nameof(securityHelper));
            Logger = logger;
        }

        public async Task<string> MakePostRequestAsync(string url, bool loginServeur, string body, string? token = null)
        {
            string res = string.Empty;
            var clientHandler = new HttpClientHandler() { AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate };

            using (HttpClient client = new HttpClient(clientHandler))
            {
                SetHeaders(client, loginServeur, token);

                var content = new StringContent(body, Encoding.UTF8, "application/json");

                HttpResponseMessage response = await client.PostAsync(url, content);

                if (response.IsSuccessStatusCode)
                {
                    res = await response.Content.ReadAsStringAsync();
                }
                else
                {
                    Console.WriteLine("Failed to make the POST request. Status code: " + response.StatusCode);
                }

                return res;
            }
        }


        public async Task<string> MakeGetRequestAsync(string url, bool loginServeur, string? token = null)
        {
            string res = string.Empty;
            var clientHandler = new HttpClientHandler() { AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate };

            using (HttpClient client = new HttpClient(clientHandler))
            {
                SetHeaders(client, loginServeur, token);

                HttpResponseMessage response = await client.GetAsync(url);
                res = await response.Content.ReadAsStringAsync();

            }

            return res;
        }

        public void SetHeaders(HttpClient client, bool loginServeur, string? token = null)
        {
            client.DefaultRequestHeaders.Add("Accept-Encoding", "gzip, deflate, br");
            client.DefaultRequestHeaders.Add("Accept-Language", "fr,fr-FR;q=0.8,en-US;q=0.5,en;q=0.3");
            client.DefaultRequestHeaders.Add("SchemaVersion", "1");
            client.DefaultRequestHeaders.Add("Auth-Version", "2");
            client.DefaultRequestHeaders.Add("Origin", "https://my.arlo.com");
            client.DefaultRequestHeaders.Add("Referer", "https://my.arlo.com/");
            client.DefaultRequestHeaders.Add("Pragma", "no-cache");
            client.DefaultRequestHeaders.Add("Cache-Control", "no-cache");
            client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:126.0) Gecko/20100101 Firefox/126.0");

            if (loginServeur)
            {
                client.DefaultRequestHeaders.Add("Accept", "application/json, text/plain, */*");
                client.DefaultRequestHeaders.Add("DNT", "1");
                client.DefaultRequestHeaders.Add("Host", "ocapi-app.arlo.com");
                client.DefaultRequestHeaders.Add("x-user-device-automation-name", "QlJPV1NFUg==");
                client.DefaultRequestHeaders.Add("x-user-device-type", "BROWSER");
                client.DefaultRequestHeaders.Add("source", "arloCamWeb");
                client.DefaultRequestHeaders.Add("x-user-device-id", ArloConfiguration.DeviceId);
            }
            else
            {
                client.DefaultRequestHeaders.Add("Accept", "application/json");
                client.DefaultRequestHeaders.Add("Host", "myapi.arlo.com");
            }

            if (!string.IsNullOrEmpty(token))
            {
                if (loginServeur)
                {
                    string token64 = token.GetBase64();
                    client.DefaultRequestHeaders.Add("Authorization", token64);
                }
                else
                {
                    client.DefaultRequestHeaders.Add("Authorization", token);
                }
            }

        }


        public async Task<string> MakePostRowRequestWithXCloudAsync(string url, string body, string token, string xCloudId)
        {
            string res = string.Empty;

            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("Authorization", token);
                client.DefaultRequestHeaders.Add("xcloudId", xCloudId);
                client.DefaultRequestHeaders.Add("Auth-Version", "2");

                var content = new StringContent(body);
                content = new StringContent(body, Encoding.UTF8, "application/json");

                HttpResponseMessage response = await client.PostAsync(url, content);

                if (response.IsSuccessStatusCode)
                {
                    res = await response.Content.ReadAsStringAsync();
                }
                else
                {
                    res = await response.Content.ReadAsStringAsync();
                    Console.WriteLine("Failed to make the POST request. Status code: " + response.StatusCode);
                }

                return res;
            }
        }

        public async Task<string> MakeGetRequestBaseStationSSLAsync(string url, string token, string deviceUniqueId, string outFileName = null)
        {
            string res = string.Empty;
            var handler = new HttpClientHandler
            {
                ClientCertificateOptions = ClientCertificateOption.Manual,
                SslProtocols = System.Security.Authentication.SslProtocols.Tls, //Do no support Above
                ServerCertificateCustomValidationCallback = (sender, certificate, chain, sslPolicyErrors) => true // Disable certificate validation
            };

            // Load the client certificate and private key
            var devicecrtPath = Path.Combine(ArloConfiguration.FolderForCertificate, "certs", deviceUniqueId, "device.crt"); // intermediate_cert extra_chain_cert
            var peercrtPath = Path.Combine(ArloConfiguration.FolderForCertificate, "certs", deviceUniqueId, "peer.crt"); // client_cert  cert
            //var caCertPath = Path.Combine(ArloConfiguration.FolderForCertificate, "certs", "ica.crt"); // ca_cert  ca_file

            var privateKeyPath = Path.Combine(ArloConfiguration.FolderForCertificate, "certs", "private.pem");

            //var caCert = new X509Certificate2(caCertPath);           // Load the CA certificate
            var cert = new X509Certificate2(peercrtPath);
            cert = cert.CopyWithPrivateKey(SecurityHelper.GetPrivateKeyFromFile(privateKeyPath));
            cert = new X509Certificate2(cert.Export(X509ContentType.Pfx));

            var intermediateCertificate = new X509Certificate2(devicecrtPath);

            handler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true;
            handler.ClientCertificates.Add(cert);
            handler.ClientCertificates.Add(intermediateCertificate);


            try
            {
                using (HttpClient client = new HttpClient(handler))
                {
                    client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
                    client.DefaultRequestHeaders.Add("Accept", "application/json");
                    client.DefaultRequestHeaders.Add("Accept-Language", "fr,fr-FR;q=0.8,en-US;q=0.5,en;q=0.3");
                    client.DefaultRequestHeaders.Add("Origin", "https://my.arlo.com");
                    client.DefaultRequestHeaders.Add("SchemaVersion", "1");
                    client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:126.0) Gecko/20100101 Firefox/126.0");

                    if (outFileName == null)
                    {
                        HttpResponseMessage response = await client.GetAsync(url);
                        res = await response.Content.ReadAsStringAsync();
                    }
                    else
                    {
                        FileInfo file = new FileInfo(outFileName);
                        if (file != null && !string.IsNullOrEmpty(file.DirectoryName) && !file.Exists)
                        {
                            Directory.CreateDirectory(file.DirectoryName);

                            using (var s = await client.GetStreamAsync(url))
                            {
                                using (var fs = new FileStream(outFileName, FileMode.Create))
                                {
                                    await s.CopyToAsync(fs);
                                }
                            }
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "{Message}", ex.Message);
                throw;
            }

            return res;
        }

    }

}
