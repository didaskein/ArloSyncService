using ArloSyncService.Common.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArloSyncService.Logic.Arlo.Model
{
    public class ArloAuthentRequest
    {
        public ArloAuthentRequest(string email, string password)
        {
            language = "fr";
            EnvSource = "prod";
            this.email = email;
            this.password = password;

            //Convert password in Base64
            this.password = password.GetBase64();
        }

        public string email { get; set; }
        public string password { get; set; }
        public string language { get; set; }
        public string EnvSource { get; set; }
    }

}
