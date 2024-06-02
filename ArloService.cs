using ArloSyncService.Common.Configuration.Model;
using ArloSyncService.Common.Helper;
using ArloSyncService.Logic.Arlo;
using ArloSyncService.Logic.ArloStation;
using ArloSyncService.Logic.ArloStation.Model;
using ArloSyncService.Logic.Listener;
using BF.Services.Logic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ArloSyncService
{

    public sealed class ArloService(ArloConfiguration ArloConfiguration, SecurityHelper SecurityHelper, ArloClient ArloClient, SSEListener SSEListener, MQTTListener MQTTListener, DeviceActivities DeviceActivities, ArloHttpHelper ArloHttpHelper, ArloStationClient ArloStationClient)
    {
        private bool FirstTime = true;


        public async Task<string> StartSynchronisationArlo(CancellationToken cancellationToken)
        {

            while (!cancellationToken.IsCancellationRequested)
            {
                if (await ArloClient.GetorRefreshTokenAsync())
                {
                    if (!DeviceActivities.isInitialized)
                    {
                        DeviceActivities.Initialize(ArloClient.Devices.data);
                        await ArloClient.CheckIfStationHaveCertificateOrCreateItAsync(DeviceActivities.GetStations());

                        if (ArloConfiguration.SSEListen && SSEListener != null && !SSEListener.isRunning)
                        {
                            SSEListener?.StartSSEListenRequestAsync(ArloClient.ArloToken, cancellationToken);
                        }

                        if (ArloConfiguration.MQTTListen && MQTTListener != null && !MQTTListener.isRunning)
                        {
                            MQTTListener?.StartMQTTListenRequestAsync(ArloClient.ArloToken, ArloClient.Session.data.userId, ArloClient.Session.data.mqttUrl, DeviceActivities.GetDiscinctListofMqttChannels(), cancellationToken);
                        }


                        await Task.Delay(5000, cancellationToken);  //Need time to get port from MQTT
                    }

                    DeviceEvent deviceEvent;
                    if (DeviceActivities.DequeueDeviceEvent(out deviceEvent) || FirstTime)
                    {

                        var stations = DeviceActivities.GetStations();

                        if (deviceEvent != null)
                        {
                            // Process DeviceInfo
                            string a = deviceEvent.Message;

                            //Maybe filter list of stations to only the one sending a new video
                            stations = stations.Where(o => o.DeviceId == deviceEvent.ParentId).ToList();
                        }


                        await ArloClient.OpenPortOfStations(stations);
                        await ArloClient.GetStationToken(stations);

                        while (!DeviceActivities.AllStationHaveIpAndPort() && !cancellationToken.IsCancellationRequested)
                        {
                            await Task.Delay(1000, cancellationToken);  //Need time to get port from MQTT
                        }

                        await ArloStationClient.GetVideosAndSyncOnFolder(stations, FirstTime);


                        FirstTime = false;
                    }
                    else
                    {
                        await Task.Delay(10000); //Check every 10seconds the MQTT Queue
                    }


                }
            }

            return "End Of Processing & Listening";
        }


    }


}