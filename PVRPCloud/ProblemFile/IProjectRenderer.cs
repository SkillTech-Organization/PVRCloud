using PVRPCloud.Models;

namespace PVRPCloud.ProblemFile;

public interface IProjectRenderer
{
    string Render(Project project,
                  List<NodeCombination> clientPairs,
                  IEnumerable<PMapRoute> routes);
}
