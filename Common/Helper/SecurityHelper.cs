using ArloSyncService.Common.Configuration.Model;
using ArloSyncService.Common.Extensions;
using ArloSyncService.Logic.ArloStation.Model;
using Org.BouncyCastle.Asn1.X9;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace ArloSyncService.Common.Helper
{
    public class SecurityHelper
    {
        public ArloConfiguration ArloConfiguration { get; set; }
        public string PublicKey { get; set; }
        public string PrivateKey { get; set; }

        public string CertsPath => Path.Combine(ArloConfiguration.FolderForCertificate, "certs");
        private string PublicKeyPath => Path.Combine(CertsPath, "public.pem");
        private string PrivateKeyPath => Path.Combine(CertsPath, "private.pem");
        public string DeviceCertsPath(string baseStationUniqueId) => Path.Combine(CertsPath, baseStationUniqueId);
        public bool HasDeviceCerts(string baseStationUniqueId) => File.Exists(Path.Combine(DeviceCertsPath(baseStationUniqueId), "peer.crt"));

        public SecurityHelper(ArloConfiguration arloConfiguration)
        {
            ArloConfiguration = arloConfiguration ?? throw new ArgumentNullException(nameof(arloConfiguration));
            Directory.CreateDirectory(ArloConfiguration.FolderForCertificate);

            if (!LoadKeys())
            {
                GenerateKeyPair();
            }
        }

        private bool LoadKeys()
        {
            if (File.Exists(PrivateKeyPath) && File.Exists(PublicKeyPath))
            {
                PrivateKey = File.ReadAllText(PrivateKeyPath);
                PublicKey = File.ReadAllText(PublicKeyPath);
                return true;
            }
            return false;
        }

        private void GenerateKeyPair()
        {
            using (var rsa = RSA.Create(2048))
            {
                // Export the private key in PEM format
                PrivateKey = rsa.ExportRSAPrivateKeyPem();

                // Export the public key in PEM format
                PublicKey = rsa.ExportSubjectPublicKeyInfoPem();

                Directory.CreateDirectory(CertsPath);

                File.WriteAllText(PublicKeyPath, PublicKey);
                File.WriteAllText(PrivateKeyPath, PrivateKey);
            }
        }

        public void SaveDeviceCerts(string baseStationId, ArloStationCreateCertAnswser certs)
        {
            var device = certs?.data?.certsData?.First();

            if (device != null && certs?.data?.icaCert != null)
            {
                string deviceCert = device.deviceCert.FormatCert();
                string peerCert = device.peerCert.FormatCert();
                string icaCert = certs.data.icaCert.FormatCert();

                Directory.CreateDirectory(DeviceCertsPath(baseStationId));

                File.WriteAllText(Path.Combine(DeviceCertsPath(baseStationId), "device.crt"), deviceCert);
                File.WriteAllText(Path.Combine(DeviceCertsPath(baseStationId), "peer.crt"), peerCert);
                File.WriteAllText(Path.Combine(CertsPath, "ica.crt"), icaCert);
                File.WriteAllText(Path.Combine(DeviceCertsPath(baseStationId), "combined.crt"), $"{peerCert}{icaCert}");
            }
        }


        public RSA GetPrivateKeyFromFile(string privateKeyPath)
        {
            var privateKeyText = File.ReadAllText(privateKeyPath);
            var rsa = RSA.Create();
            rsa.ImportFromPem(privateKeyText.ToCharArray());
            return rsa;
        }

    }
}
