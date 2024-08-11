using CommonUtils;
using Microsoft.Extensions.Options;
using PMapCore.BLL;
using PMapCore.Common;
using PMapCore.Route;
using PVRPCloud.Settings;
using PVRPCloudInsightsLogger.Logger;
using PVRPCloudInsightsLogger.Settings;

namespace PVRPCloud;

public sealed class PVRPCloudLogic
{
    private ITelemetryLogger? _logger { get; set; }
    private LoggerSettings _loggerSettings { get; set; }
    private string _requestID { get; set; }

    public string _mapStorageConnectionString;

    public PVRPCloudLogic(LoggerSettings loggerSettings, ITelemetryLogger logger, IOptions<MapStorageSettings> mapStorageSettings)
    {
        _loggerSettings = loggerSettings;
        if (logger == null)
        {
            _logger = TelemetryClientFactory.Create(loggerSettings);
            _logger.LogToQueueMessage = LogToQueueMessage;
        }
        _mapStorageConnectionString = mapStorageSettings.Value.AzureStorageConnectionString;
    }

    public object LogToQueueMessage(params object[] args)
    {
        var typeParsed = Enum.TryParse((string)(args[1] ?? ""), out LogTypes type);
        var m = new QueueResponse
        {
            RequestID = _requestID,
            Log = new Log
            {
                Message = (string)args[0],
                Timestamp = (DateTime)args[2],
                Type = typeParsed ? type : LogTypes.STATUS
            },
            Status = QueueResponse.QueueResponseStatus.LOG
        };
        return m.ToJson();
    }


    public string GenerateRequestId()
    {
        return DateTime.UtcNow.Ticks.ToString();
    }
    public void Init(string requestId = null)
    {
        try
        {
            _requestID = string.IsNullOrWhiteSpace(requestId) ? GenerateRequestId() : requestId;

            _logger.Info(String.Format("{0} {1}", "PVRPCloud", "Init"), _logger.GetStatusProperty(_requestID));

            DateTime dtStart = DateTime.UtcNow;
            RouteData.Instance.InitFromFiles(_mapStorageConnectionString, false);
            bllRoute route = new bllRoute(null);
            _logger.Info("RouteData.InitFromFiles()  " + Util.GetSysInfo() + " Időtartam:" + (DateTime.UtcNow - dtStart).ToString());

        }
        catch (Exception)
        {
            throw;
        }




    }

}
