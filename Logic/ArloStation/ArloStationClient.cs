using ArloSyncService.Common.Configuration.Model;
using ArloSyncService.Common.Helper;
using ArloSyncService.Logic.Arlo;
using ArloSyncService.Logic.ArloStation.Model;
using BF.Services.Logic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ArloSyncService.Logic.ArloStation
{
    public class ArloStationClient(ArloHttpHelper ArloHttpHelper, DeviceActivities DeviceActivities, ArloConfiguration ArloConfiguration, ArloClient ArloClient)
    {

        //if firstTime is true, we will download all videos from the stations (1 Month max of history)
        public async Task GetVideosAndSyncOnFolder(List<DeviceInfo> stations, bool firstTime)
        {
            var options = new ParallelOptions { MaxDegreeOfParallelism = 2 };

            await Parallel.ForEachAsync(stations, options, async (station, token) =>
            //foreach (var station in stations)
            {
                var device = DeviceActivities.GetDeviceInfo(station.DeviceId);
                try
                {
                    string urlConnectivity = $"{device.StationUrl}/connectivity";
                    string resConnectivity = await ArloHttpHelper.MakeGetRequestBaseStationSSLAsync(urlConnectivity, device.Token, station.UniqueId);
                }
                catch (Exception ex)
                {
                    await ArloClient.OpenPortOfStations(stations);
                }


                DateTime startDate = DateTime.Now;
                DateTime endDate = DateTime.Now;
                if (firstTime) startDate = startDate.AddDays(-1 * ArloConfiguration.DayToSyncBack);
                do
                {
                    var urlListRecords = $"{device.StationUrl}/list/{startDate.ToString("yyyyMMdd")}/{endDate.ToString("yyyyMMdd")}";
                    string resRecords = await ArloHttpHelper.MakeGetRequestBaseStationSSLAsync(urlListRecords, device.Token, station.UniqueId);
                    var records = JsonSerializer.Deserialize<ArloStationListVideoAnswser>(resRecords);

                    if (records != null && records.data != null)
                    {
                        foreach (var record in records.data)
                        {
                            var deviceRecord = DeviceActivities.GetDeviceInfo(record.deviceId);

                            var fileToDownload = $"{device.StationUrl}/download/{record.presignedContentUrl}";
                            string[] parts = record.presignedContentUrl.Split('/');
                            var fileName = parts.LastOrDefault();
                            if (fileName != null)
                            {
                                string name = Path.Combine(ArloConfiguration.FolderForVideos, startDate.ToString("yyyyMMdd"), $"{record.localCreatedDate.ToString("hhmmss")}_{deviceRecord.DeviceName}.mp4");
                                resRecords = await ArloHttpHelper.MakeGetRequestBaseStationSSLAsync(fileToDownload, device.Token, station.UniqueId, name);
                            }

                        }
                    }

                    startDate = startDate.AddDays(1);
                } while (startDate < endDate); // We need to process data by date & we have max 30 days pasts

            });
        }
    }
}
