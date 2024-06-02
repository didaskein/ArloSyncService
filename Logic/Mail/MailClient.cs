using ArloSyncService.Common.Configuration.Model;
using ArloSyncService.Logic.Arlo;
using MailKit;
using MailKit.Net.Imap;
using MailKit.Search;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ArloSyncService.Logic.Mail
{
    public class MailClient(ArloConfiguration ArloConfiguration)
    {
        public async Task<string> ReadEmailAndGetLastOTPCodeAsync()
        {
            string otp = string.Empty;
            DateTime lastDate = DateTime.Now.AddMinutes(-10);
            if (ArloConfiguration != null)
            {
                using (var client = new ImapClient())
                {
                    client.Connect(ArloConfiguration.AccountMailMFAImapServer, ArloConfiguration.AccountMailMFAImapPort, true);
                    client.Authenticate(ArloConfiguration.AccountMailMFALogin, ArloConfiguration.AccountMailMFAPassword);

                    await client.Inbox.OpenAsync(FolderAccess.ReadOnly);
                    SearchQuery seachQuery = SearchQuery.And(SearchQuery.DeliveredAfter(lastDate), SearchQuery.FromContains(ArloConfiguration.MailMFAFormEmail));
                    List<MailKit.UniqueId> uids = client.Inbox.Search(seachQuery).ToList();

                    // Invert the order of the uids
                    uids = uids.Reverse<MailKit.UniqueId>().ToList();

                    foreach (MailKit.UniqueId uid in uids)
                    {
                        var message = await client.Inbox.GetMessageAsync(uid);
                        //Console.WriteLine(message.Subject);

                        // Extract the OTP code from the email body
                        if (message.Subject.Contains(ArloConfiguration.MailMFASubjectEmail))
                        {
                            var lines = message.HtmlBody.Split('\n');
                            //Console.WriteLine(message.HtmlBody);
                            foreach (var line in lines)
                            {
                                //With Mail Forward : @"\s*(\d{6})\s*</h1>"
                                //With Mail direct form Arlo : @"^\W*(\d{6})\W*$"
                                var code = Regex.Match(line, ArloConfiguration.MailMFARegexOtpToMatch);
                                if (code.Success && code.Groups.Count == 2)
                                {
                                    otp = code.Groups[1].Value;
                                    break;
                                }
                            }
                        }

                        if (!string.IsNullOrEmpty(otp))
                        {
                            break;
                        }

                    }

                    client.Disconnect(true);
                }

            }


            return otp;

        }

    }
}
