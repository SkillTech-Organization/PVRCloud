namespace PVRPCloud
{
    public interface IPVRPCloudLogic
    {
        string GenerateRequestId();
        void Init(string requestId = null);
        object LogToQueueMessage(params object[] args);
    }
}