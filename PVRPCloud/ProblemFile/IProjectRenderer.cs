using PVRPCloud.Models;

namespace PVRPCloud.ProblemFile;

public interface IProjectRenderer
{
    string Render(Project project,
                  List<NodeCombination> clientPairs,
                  List<PMapRoute> routes,
                  string requestID);

    PvrpData GetPvrpData();
}
