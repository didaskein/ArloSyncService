using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArloSyncService.Logic.Arlo.Model
{
    public class ArloAuthentFinishRequest
    {
        public ArloAuthentFinishRequest(string factorAuthCode, string otp)
        {
            this.factorAuthCode = factorAuthCode;

            //Parse OTP
            this.otp = int.Parse(otp);
        }

        public string factorAuthCode { get; set; }
        public int otp { get; set; }

    }

}
