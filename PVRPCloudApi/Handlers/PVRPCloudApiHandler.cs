using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using System.Reflection;
using Task = System.Threading.Tasks.Task;
using PVRPCloud;
using PVRPCloudInsightsLogger.Logger;
using PVRPCloudInsightsLogger.Settings;
using PVRPCloudApi.DTO.Request;
using PVRPCloudApi.DTO.Response;

namespace PVRPCloudApi.Handlers;

public class PVRPCloudApiHandler : IPVRPCloudApiHandler
{
    private PVRPCloudLoggerSettings Settings { get; set; }

    private ITelemetryLogger Logger { get; set; }

    public PVRPCloudApiHandler(IOptions<PVRPCloudLoggerSettings> options)
    {
        Settings = options.Value;
        Logger = TelemetryClientFactory.Create(Settings);
        Logger.LogToQueueMessage = PVRPCloudInterface.LogToQueueMessage;
    }

    public Task<Response> PVRCloudSupportAsync(SupportRequest body, CancellationToken cancellationToken = default)
    {

        var response = new Response();
        try
        {

            var initResult = PVRPCloudInterface.PVRPCloudInit(body.TaskList, body.TruckList, body.MaxTruckDistance, Settings);
            if (initResult != null)
            {
                response = initResult;
            }

            if (initResult != null && !initResult.HasError)
            {
                Task.Run(() => PVRPCloudInterface.PVRPCloudSupport(body.TaskList, body.TruckList, body.MaxTruckDistance));
            }
        }
        catch (Exception ex)
        {
            Logger.Exception(ex, Logger.GetExceptionProperty(response.RequestID), intoQueue: false);
            throw;
        }
        return Task.FromResult(response);
    }

    public Task<Response> Result(string id)
    {
        var response = new Response();
        try
        {
            //var json = Logger.Blob.GetLoggedString(id).Result;
            //Logger.Info("From blob JSON: " + json, Logger.GetExceptionProperty(response?.RequestID ?? ""), intoQueue: false);
            //response = Newtonsoft.Json.JsonConvert.DeserializeObject<PVRCloudResponse>(json);
            //Logger.Info("From blob is null: " + (response == null).ToString(), Logger.GetExceptionProperty(response?.RequestID ?? ""), intoQueue: false);
            response = Logger.Blob.GetLoggedJsonAs<Response>(id).Result;
            //var asd = response.ToJson();
            response?.Results.ForEach(x =>
            {
                //Logger.Info("Data: " + Newtonsoft.Json.JsonConvert.SerializeObject(x.Data), Logger.GetExceptionProperty(response?.RequestID ?? ""), intoQueue: false);
                if (x.Data != null)
                {
                    if (x.Status == PVRPCloud.Result.ResultStatus.RESULT)
                    {
                        x.Data = ((JToken)x.Data).ToObject<List<CalcTask>>();
                    }
                    else
                    {
                        x.Data = ((JToken)x.Data).ToObject<Dictionary<string, string>>();
                    }
                }
                else
                {
                    x.Data = new List<CalcTask>();
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

    public Task<Response> PVRCloudSupportXAsync(SupportRequest body, CancellationToken cancellationToken = default)
    {
        var response = new Response();
        try
        {
            var initResult = PVRPCloudInterface.PVRPCloudInit(body.TaskList, body.TruckList, body.MaxTruckDistance, Settings);
            if (initResult != null)
            {
                response = initResult;
            }

            if (initResult != null && !initResult.HasError)
            {
                Task.Run(() => PVRPCloudInterface.PVRPCloudSupportX(body.TaskList, body.TruckList, body.MaxTruckDistance));
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
