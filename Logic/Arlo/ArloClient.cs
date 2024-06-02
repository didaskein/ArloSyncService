using ArloSyncService.Common.Configuration.Model;
using ArloSyncService.Common.Helper;
using ArloSyncService.Logic.Arlo.Model;
using ArloSyncService.Logic.ArloStation;
using ArloSyncService.Logic.ArloStation.Model;
using ArloSyncService.Logic.Mail;
using BF.Services.Logic;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ArloSyncService.Logic.Arlo
{
    public class ArloClient(ArloConfiguration ArloConfiguration, ArloHttpHelper ArloHttpHelper, MailClient MailClient, SecurityHelper SecurityHelper, DeviceActivities DeviceActivities)
    {
        public string ArloToken { get; set; }

        public ArloAuthentSessionAnswer Session { get; set; }
        public ArloDevicesAnswer Devices { get; set; }


        public DateTime ArloTokenExpirationDate { get; set; }

        public async Task<bool> GetorRefreshTokenAsync()
        {
            if (ArloConfiguration != null && (string.IsNullOrEmpty(ArloToken) || DateTime.UtcNow.AddDays(1) > ArloTokenExpirationDate))
            {
                // Authentification Level 1
                string url = $"{ArloConfiguration.ArloBaseAuthentUri}/api/auth";
                var bodyArloAuthentRequest = new ArloAuthentRequest(ArloConfiguration.AccountArloLogin, ArloConfiguration.AccountArloPassword);
                var res = await ArloHttpHelper.MakePostRequestAsync(url, true, JsonSerializer.Serialize(bodyArloAuthentRequest));
                var authent = JsonSerializer.Deserialize<ArloAuthentAnswer>(res);
                if (authent == null || authent.data == null || string.IsNullOrWhiteSpace(authent.data.token))
                {
                    throw new Exception("Authentification failed");
                }
                ArloToken = authent.data.token;


                // Get a list of all valid two factors
                url = $"{ArloConfiguration.ArloBaseAuthentUri}/api/getFactors?data%20=%20{authent?.data.authenticated}";
                res = await ArloHttpHelper.MakeGetRequestAsync(url, true, ArloToken);
                var factors = JsonSerializer.Deserialize<ArloAuthentFactorsAnswer>(res);
                var emailFactor = factors?.data.items.Where(o => o.factorType == "EMAIL").FirstOrDefault();
                if (emailFactor == null || string.IsNullOrWhiteSpace(emailFactor.factorId))
                {
                    throw new Exception("Email factor not found");
                }

                // Authentification Level 2
                url = $"{ArloConfiguration.ArloBaseAuthentUri}/api/startAuth";
                var factorBody = new ArloAuthentFactorRequest(emailFactor.factorId);
                res = await ArloHttpHelper.MakePostRequestAsync(url, true, JsonSerializer.Serialize(factorBody), ArloToken);
                var factorAuthCode = JsonSerializer.Deserialize<ArloAuthentFactorAuthCodeAnswer>(res);
                if (factorAuthCode == null || factorAuthCode?.data == null || string.IsNullOrWhiteSpace(factorAuthCode?.data?.factorAuthCode))
                {
                    throw new Exception("FactorAuthCode not found");
                }

                bool continueOtpSearch = true;
                int nbRetry = 0;
                do
                {
                    //Sleep await 10 seconds (we need to retry until we get a valid OTP)
                    await Task.Delay(10000);

                    string otp = await MailClient.ReadEmailAndGetLastOTPCodeAsync();
                    if (!string.IsNullOrWhiteSpace(otp))
                    {
                        // Authentification Level 3 (Finish with OTP)
                        url = $"{ArloConfiguration.ArloBaseAuthentUri}/api/finishAuth";
                        var finishAuth = new ArloAuthentFinishRequest(factorAuthCode.data.factorAuthCode, otp);
                        res = await ArloHttpHelper.MakePostRequestAsync(url, true, JsonSerializer.Serialize(finishAuth), ArloToken);
                        var tmpAuthent = JsonSerializer.Deserialize<ArloAuthentAnswer>(res);
                        if (tmpAuthent?.meta?.code == 200)
                        {
                            continueOtpSearch = false;
                            authent = tmpAuthent;
                            ArloToken = authent.data.token;
                            ArloTokenExpirationDate = authent.data.expiresIn;
                        }
                    }

                } while (continueOtpSearch && nbRetry++ < 6);
                if (continueOtpSearch)
                {
                    throw new Exception("OTP not found in the mail box in less than 60s");
                }

                // Authentification Level 4 (Verifiy authorization token)
                url = $"{ArloConfiguration.ArloBaseAuthentUri}/api/validateAccessToken";
                res = await ArloHttpHelper.MakeGetRequestAsync(url, true, ArloToken);
                var validate = JsonSerializer.Deserialize<ArloAuthentValidateAnswer>(res);
                if (validate != null && validate.meta.code == 200 && validate.data.tokenValidated)
                {
                    //Open Session 
                    url = $"{ArloConfiguration.ArloBaseApiUri}/hmsweb/users/session/v3";
                    res = await ArloHttpHelper.MakeGetRequestAsync(url, false, ArloToken);
                    var session = JsonSerializer.Deserialize<ArloAuthentSessionAnswer>(res);
                    if (session != null)
                    {
                        Session = session;
                    }

                    // Get a list of all devices
                    url = $"{ArloConfiguration.ArloBaseApiUri}/hmsweb/v2/users/devices";
                    res = await ArloHttpHelper.MakeGetRequestAsync(url, false, ArloToken);
                    var devices = JsonSerializer.Deserialize<ArloDevicesAnswer>(res);
                    if (devices != null)
                    {
                        Devices = devices;
                    }

                    return true;
                }

            }
            else if (!string.IsNullOrEmpty(ArloToken) && DateTime.UtcNow.AddDays(1) < ArloTokenExpirationDate)
            {
                return true; //Always valid
            }

            return false;
        }

        public async Task CheckIfStationHaveCertificateOrCreateItAsync(List<DeviceInfo> stations)
        {
            foreach (var station in stations)
            {
                if (!SecurityHelper.HasDeviceCerts(station.UniqueId))
                {
                    string url = $"{ArloConfiguration.ArloBaseApiUri}/hmsweb/users/devices/v2/security/cert/create";
                    var requestCertBody = new ArloStationCreateCertRequest(station.DeviceId, SecurityHelper.PublicKey, station.UniqueId);
                    string res = await ArloHttpHelper.MakePostRowRequestWithXCloudAsync(url, JsonSerializer.Serialize(requestCertBody), this.ArloToken, station.xCloudId);
                    var cert = JsonSerializer.Deserialize<ArloStationCreateCertAnswser>(res);
                    if (cert != null)
                    {
                        SecurityHelper.SaveDeviceCerts(station.UniqueId, cert);
                    }
                }
            }
        }

        public async Task OpenPortOfStations(List<DeviceInfo> stations)
        {
            foreach (var station in stations)
            {
                string url = $"{ArloConfiguration.ArloBaseApiUri}/hmsweb/users/devices/notify/{station.DeviceId}";
                var requestOpenPort = new ArloStationOpenPortRequest(station.UserId, station.DeviceId);
                var openPort = await ArloHttpHelper.MakePostRowRequestWithXCloudAsync(url, JsonSerializer.Serialize(requestOpenPort), this.ArloToken, station.xCloudId);
            }
        }

        public async Task GetStationToken(List<DeviceInfo> stations)
        {
            foreach (var station in stations)
            {
                var device = DeviceActivities.GetDeviceInfo(station.DeviceId);

                if (device != null && (device.Token == null || device.TokenValidTo == null || DateTime.UtcNow > device.TokenValidTo))
                {
                    // Get Station Status
                    string urlStatus = $"{ArloConfiguration.ArloBaseApiUri}/hmsweb/users/device/ratls/status/{station.DeviceId}";
                    string resStatus = await ArloHttpHelper.MakeGetRequestAsync(urlStatus, false, this.ArloToken);

                    //Get a Refresh Token for Base Station Api calls 
                    string urlGetStationToken = $"{ArloConfiguration.ArloBaseApiUri}/hmsweb/users/device/ratls/token/{station.DeviceId}";
                    string resStationToken = await ArloHttpHelper.MakeGetRequestAsync(urlGetStationToken, false, this.ArloToken);
                    var tokenDevice = JsonSerializer.Deserialize<ArloStationGetTokenAnswser>(resStationToken);
                    if (tokenDevice != null && tokenDevice.data != null && !string.IsNullOrWhiteSpace(tokenDevice.data.ratlsToken))
                    {
                        var jwt = ConvertJwtStringToJwtSecurityToken(tokenDevice.data.ratlsToken);
                        DeviceActivities.UpdateTokenStation(station.DeviceId, tokenDevice.data.ratlsToken, jwt.ValidTo);
                    }

                }

            }
        }


        public JwtSecurityToken ConvertJwtStringToJwtSecurityToken(string? jwt)
        {
            var handler = new JwtSecurityTokenHandler();
            var token = handler.ReadJwtToken(jwt);

            return token;
        }

    }
}
