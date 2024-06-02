using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArloSyncService.Logic.Arlo.Model
{
    public class ArloAuthentFactorsAnswer
    {
        public ArloAuthentFactorsAnswer()
        {
            meta = new Meta();
            data = new Data();
        }

        public Meta meta { get; set; }
        public Data data { get; set; }

        public class Meta
        {
            public int code { get; set; }
        }

        public class Data
        {
            public Data()
            {
                items = new List<Item>();
            }
            public string _type { get; set; }
            public List<Item> items { get; set; }
            public string MFA_State { get; set; }
        }

        public class Item
        {
            public string _type { get; set; }
            public string factorId { get; set; }
            public string factorType { get; set; }
            public string displayName { get; set; }
            public string factorNickname { get; set; }
            public string applicationId { get; set; }
            public string applicationName { get; set; }
            public string factorRole { get; set; }
        }
    }





}
