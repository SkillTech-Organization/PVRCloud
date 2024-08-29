using PVRPCloud.Requests;

namespace PVRPCloud.ProblemFile;

public interface IProjectRenderer
{
    string Render(Project project,
                  List<(ClientNodeIdPair From, ClientNodeIdPair To)> clientPairs,
                  IEnumerable<PMapRoute> routes);
}
