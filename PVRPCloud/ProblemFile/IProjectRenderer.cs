using Microsoft.Extensions.Logging;
using PVRPCommon;
using PVRPCommon.Models;

namespace PVRPCloud.ProblemFile;

public interface IProjectRenderer
{
    string Render(Project project,
                  List<NodeCombination> clientPairs,
                  List<PMapRoute> routes,
                  string requestID,
                  ILogger<ProjectRenderer> logger);

    PvrpData GetPvrpData();
}
