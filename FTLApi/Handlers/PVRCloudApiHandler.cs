using FTLSupporter;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using PVRCloudApi.DTO.Request;
using PVRCloudApi.DTO.Response;
using PVRCloudApi.Handlers;
using PVRCloudInsightsLogger.Logger;
using PVRCloudInsightsLogger.Settings;
using System.Reflection;
using Task = System.Threading.Tasks.Task;

namespace FTLApi.Handlers
{
    public class PVRCloudApiHandler : IPVRCloudApiHandler
    {
        private PVRCloudLoggerSettings Settings { get; set; }

        private ITelemetryLogger Logger { get; set; }

        public PVRCloudApiHandler(IOptions<PVRCloudLoggerSettings> options)
        {
            Settings = options.Value;
            Logger = TelemetryClientFactory.Create(Settings);
            Logger.LogToQueueMessage = FTLInterface.LogToQueueMessage;
        }

        public Task<FTLResponse> PVRCloudSupportAsync(PVRCloudSupportRequest body, CancellationToken cancellationToken = default)
        {

            var response = new FTLResponse();
            try
            {

                var initResult = FTLInterface.FTLInit(body.TaskList, body.TruckList, body.MaxTruckDistance, Settings);
                if (initResult != null)
                {
                    response = initResult;
                }
                response.TaskList = body.TaskList;
                response.TruckList = body.TruckList;

                if (initResult != null && !initResult.HasError)
                {
                    Task.Run(() => FTLInterface.FTLSupport(body.TaskList, body.TruckList, body.MaxTruckDistance));
                }
            }
            catch (Exception ex)
            {
                Logger.Exception(ex, Logger.GetExceptionProperty(response.RequestID), intoQueue: false);
                throw;
            }
            return Task.FromResult(response);
        }

        public Task<FTLResponse> Result(string id)
        {
            var response = new FTLResponse();
            try
            {
                //var json = Logger.Blob.GetLoggedString(id).Result;
                //Logger.Info("From blob JSON: " + json, Logger.GetExceptionProperty(response?.RequestID ?? ""), intoQueue: false);
                //response = Newtonsoft.Json.JsonConvert.DeserializeObject<FTLResponse>(json);
                //Logger.Info("From blob is null: " + (response == null).ToString(), Logger.GetExceptionProperty(response?.RequestID ?? ""), intoQueue: false);
                response = Logger.Blob.GetLoggedJsonAs<FTLResponse>(id).Result;
                //var asd = response.ToJson();
                response?.Result.ForEach(x =>
                {
                    //Logger.Info("Data: " + Newtonsoft.Json.JsonConvert.SerializeObject(x.Data), Logger.GetExceptionProperty(response?.RequestID ?? ""), intoQueue: false);
                    if (x.Data != null)
                    {
                        if (x.Status == FTLResult.FTLResultStatus.RESULT)
                        {
                            x.Data = ((JToken)x.Data).ToObject<List<FTLSupporter.FTLCalcTask>>();
                        }
                        else
                        {
                            x.Data = ((JToken)x.Data).ToObject<Dictionary<string, string>>();
                        }
                    }
                    else
                    {
                        x.Data = new List<FTLSupporter.FTLCalcTask>();
                    }
                });
            }
            catch (Exception ex)
            {
                Logger.Exception(ex, Logger.GetExceptionProperty(response?.RequestID ?? ""), intoQueue: false);
                throw;
            }
            return Task.FromResult(response);
        }

        public Task<FTLResponse> PVRCloudSupportXAsync(PVRCloudSupportRequest body, CancellationToken cancellationToken = default)
        {
            var response = new FTLResponse();
            try
            {
                var initResult = FTLInterface.FTLInit(body.TaskList, body.TruckList, body.MaxTruckDistance, Settings);
                if (initResult != null)
                {
                    response = initResult;
                }
                response.TaskList = body.TaskList;
                response.TruckList = body.TruckList;

                if (initResult != null && !initResult.HasError)
                {
                    Task.Run(() => FTLInterface.FTLSupportX(body.TaskList, body.TruckList, body.MaxTruckDistance));
                }
            }
            catch (Exception ex)
            {
                Logger.Exception(ex, Logger.GetExceptionProperty(response.RequestID), intoQueue: false);
                throw;
            }
            return Task.FromResult(response);
        }

        public Task IsAliveAsync(CancellationToken cancellationToken = default)
        {
            return Task.FromResult(new IsAliveOk
            {
                Version = Assembly.GetExecutingAssembly().GetName().Version.ToString()
            });
        }
    }
}
