using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArloSyncService.Logic.ArloStation.Model
{
    public class ArloStationOpenPortRequest
    {
        public ArloStationOpenPortRequest(string from, string to, string action = "open", string resource = "storage/ratls", bool publishResponse = true, bool addProperties = false)
        {
            this.from = from; //_user_id,
            this.to = to; //device_id

            this.action = action;
            this.resource = resource;
            this.publishResponse = publishResponse;

            var random = new Random();
            var transId = random.Next(90000000, 100000000);
            var timestamp = DateTimeOffset.Now.ToUnixTimeMilliseconds();
            this.transId = $"web!{transId}!{timestamp}";

            if (addProperties)
            {
                properties = new Properties();
                properties.devices.Add(to);
            }
        }

        public string action { get; set; }
        public string resource { get; set; }
        public string from { get; set; }
        public string transId { get; set; }
        public string to { get; set; }
        public bool publishResponse { get; set; }

        public Properties properties { get; set; }

        public class Properties
        {

            public Properties()
            {
                devices = new List<string>();
            }

            public List<string> devices { get; set; }
        }
    }



}
