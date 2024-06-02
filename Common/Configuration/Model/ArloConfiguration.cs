using System.ComponentModel;

namespace ArloSyncService.Common.Configuration.Model
{
    [DisplayName("Arlo")]
    public class ArloConfiguration
    {
        public ArloConfiguration()
        {

        }

        public string DeviceId { get; set; }


        public string AccountArloLogin { get; set; }
        public string AccountArloPassword { get; set; }


        public string AccountMailMFALogin { get; set; }
        public string AccountMailMFAPassword { get; set; }
        public string AccountMailMFAImapServer { get; set; }
        public int AccountMailMFAImapPort { get; set; }
        public string MailMFAFormEmail { get; set; }
        public string MailMFASubjectEmail { get; set; }
        public string MailMFARegexOtpToMatch { get; set; }


        public string FolderForCertificate { get; set; }
        public string FolderForVideos { get; set; }


        public string ArloBaseAuthentUri { get; set; }
        public string ArloBaseApiUri { get; set; }
        public bool SSEListen { get; set; }
        public bool MQTTListen { get; set; }

        public string MQTTHostName { get; set; }
        public int MQTTPort { get; set; }
        public int DayToSyncBack { get; set; }

    }
}
