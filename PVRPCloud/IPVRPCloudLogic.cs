using PVRPCloud.Requests;

namespace PVRPCloud
{
    public interface IPVRPCloudLogic
    {
        Task<string> Handle(Project project, CancellationToken cancellationToken);
    }
}