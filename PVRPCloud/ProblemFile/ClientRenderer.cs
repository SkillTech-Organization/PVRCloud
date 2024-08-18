using System.Text;
using PVRPCloud.Requests;

namespace PVRPCloud.ProblemFile;

public sealed class ClientRenderer
{
    private const int LatLngChange = 1_000_000;
    private const int DepotPVRPId = 1;

    private readonly Dictionary<int, Depot> _clients = [];

    private readonly StringBuilder _sb = new();

    public IReadOnlyDictionary<int, Depot> Clients => _clients.AsReadOnly();

    public StringBuilder Render(Depot depot, int projectMinTime)
    {
        CreateClient(depot.DepotName, depot.Lat, depot.Lng);

        CreateDepot(depot);

        SetDepotInformation(depot, projectMinTime);

        return _sb;
    }

    private void CreateClient(string name, double lat, double lng)
    {
        double lat2 = Math.Round(lat * LatLngChange, 0);
        double lng2 = Math.Round(lng * LatLngChange, 0);

        _sb.AppendLine($"""createClient("{name}", {lat2}, {lng2})""");
    }

    private void CreateDepot(Depot depot)
    {
        _clients.Add(DepotPVRPId, depot);

        _sb.AppendLine($"""createDepot("{depot.DepotName}", {DepotPVRPId})""");
    }

    private void SetDepotInformation(Depot depot, int projectMinTime)
    {
        _sb.AppendLine($"setDepotInformation({DepotPVRPId}, 1, {depot.ServiceFixTime}, {depot.ServiceVarTime}, {projectMinTime}, 0, 0, 0, 0)");
    }
}
