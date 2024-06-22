using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using PVRCloudApi.DTO.Request;
using PVRCloudApi.DTO.Response;
using System.Reflection;
using Task = System.Threading.Tasks.Task;
using PVRPCloud;
using PVRPCloudInsightsLogger.Logger;
using PVRPCloudInsightsLogger.Settings;

namespace PVRCloudApi.Handlers;

public class PVRCloudApiHandler : IPVRCloudApiHandler
{
    private PVRPCloudLoggerSettings Settings { get; set; }

    private ITelemetryLogger Logger { get; set; }

    public PVRCloudApiHandler(IOptions<PVRPCloudLoggerSettings> options)
    {
        Settings = options.Value;
        Logger = TelemetryClientFactory.Create(Settings);
        Logger.LogToQueueMessage = PVRPCloudInterface.LogToQueueMessage;
    }

    public Task<PVRPCloudResponse> PVRCloudSupportAsync(PVRCloudSupportRequest body, CancellationToken cancellationToken = default)
    {

        var response = new PVRPCloudResponse();
        try
        {

            var initResult = PVRPCloudInterface.FTLInit(body.TaskList, body.TruckList, body.MaxTruckDistance, Settings);
            if (initResult != null)
            {
                response = initResult;
            }
            response.TaskList = body.TaskList;
            response.TruckList = body.TruckList;

            if (initResult != null && !initResult.HasError)
            {
                Task.Run(() => PVRPCloudInterface.FTLSupport(body.TaskList, body.TruckList, body.MaxTruckDistance));
            }
        }
        catch (Exception ex)
        {
            Logger.Exception(ex, Logger.GetExceptionProperty(response.RequestID), intoQueue: false);
            throw;
        }
        return Task.FromResult(response);
    }

    public Task<PVRPCloudResponse> Result(string id)
    {
        var response = new PVRPCloudResponse();
        try
        {
            //var json = Logger.Blob.GetLoggedString(id).Result;
            //Logger.Info("From blob JSON: " + json, Logger.GetExceptionProperty(response?.RequestID ?? ""), intoQueue: false);
            //response = Newtonsoft.Json.JsonConvert.DeserializeObject<PVRCloudResponse>(json);
            //Logger.Info("From blob is null: " + (response == null).ToString(), Logger.GetExceptionProperty(response?.RequestID ?? ""), intoQueue: false);
            response = Logger.Blob.GetLoggedJsonAs<PVRPCloudResponse>(id).Result;
            //var asd = response.ToJson();
            response?.Result.ForEach(x =>
            {
                //Logger.Info("Data: " + Newtonsoft.Json.JsonConvert.SerializeObject(x.Data), Logger.GetExceptionProperty(response?.RequestID ?? ""), intoQueue: false);
                if (x.Data != null)
                {
                    if (x.Status == PVRPCloudResult.PVRPCloudResultStatus.RESULT)
                    {
                        x.Data = ((JToken)x.Data).ToObject<List<PVRPCloudCalcTask>>();
                    }
                    else
                    {
                        x.Data = ((JToken)x.Data).ToObject<Dictionary<string, string>>();
                    }
                }
                else
                {
                    x.Data = new List<PVRPCloudCalcTask>();
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

    public Task<PVRPCloudResponse> PVRCloudSupportXAsync(PVRCloudSupportRequest body, CancellationToken cancellationToken = default)
    {
        var response = new PVRPCloudResponse();
        try
        {
            var initResult = PVRPCloudInterface.FTLInit(body.TaskList, body.TruckList, body.MaxTruckDistance, Settings);
            if (initResult != null)
            {
                response = initResult;
            }
            response.TaskList = body.TaskList;
            response.TruckList = body.TruckList;

            if (initResult != null && !initResult.HasError)
            {
                Task.Run(() => PVRPCloudInterface.FTLSupportX(body.TaskList, body.TruckList, body.MaxTruckDistance));
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
