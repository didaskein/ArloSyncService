using ArloSyncService.Converter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ArloSyncService.Logic.ArloStation.Model
{
    public class ArloStationListVideoAnswser
    {
        public bool success { get; set; }
        public List<Datum> data { get; set; }
    }

    public class Datum
    {
        public string ownerId { get; set; }
        public string uniqueId { get; set; }
        public string name { get; set; }
        public string bsId { get; set; }
        public string deviceId { get; set; }
        public string triggeredBy { get; set; }
        public string createdBy { get; set; }
        public string createdDate { get; set; }

        [JsonConverter(typeof(UnixSecondsDateTimeConverter))]
        public DateTime localCreatedDate { get; set; }

        [JsonConverter(typeof(UnixSecondsDateTimeConverter))]
        public DateTime utcCreatedDate { get; set; }

        [JsonConverter(typeof(UnixSecondsDateTimeConverter))]
        public DateTime lastModified { get; set; }


        public string timeZone { get; set; }
        public string mediaDuration { get; set; }
        public int mediaDurationSecond { get; set; }
        public string contentType { get; set; }
        public string codec { get; set; }
        public Audio audio { get; set; }
        public string reason { get; set; }
        public string storage { get; set; }
        public string currentState { get; set; }
        public string presignedContentUrl { get; set; }
        public string presignedThumbnailUrl { get; set; }
        public bool donated { get; set; }

        public class Audio
        {
            public int tracks { get; set; }
        }
    }



}
