using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArloSyncService.Logic.ArloStation.Model
{
    public class ArloStationCreateCertRequest
    {
        public ArloStationCreateCertRequest(string deviceId, string publicKey, string baseStationUniqueId)
        {
            uuid = deviceId;
            this.publicKey = publicKey.Replace("\n", "").Replace("\r", "").Replace("-----BEGIN PUBLIC KEY-----", "").Replace("-----END PUBLIC KEY-----", "").Replace("-----BEGIN RSA PUBLIC KEY-----", "").Replace("-----END RSA PUBLIC KEY-----", "");

            uniqueIds = new List<string>();
            uniqueIds.Add(baseStationUniqueId); // UniqueId 
        }

        public string uuid { get; set; }
        public string publicKey { get; set; }
        public List<string> uniqueIds { get; set; }
    }

}
