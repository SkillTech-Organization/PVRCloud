using System.Text;
using PVRPCloud.Models;

namespace PVRPCloud.ProblemFile;

public sealed class ClientRenderer
{
    private const int LatLngChange = 1_000_000;
    private const int DepotPVRPId = 1;

    private readonly StringBuilder _depotStringBuilder = new();
    private readonly StringBuilder _clientsStringBuilder = new();

    public Dictionary<string, int> ClientIds { get; } = [];

    public StringBuilder Render(Depot depot, int projectMinTime)
    {
        CreateClient(depot.DepotName, depot.Lat, depot.Lng, _depotStringBuilder);

        CreateDepot(depot);

        SetDepotInformation(depot, projectMinTime);

        CreateEntry(depot);

        return _depotStringBuilder;
    }

    private void CreateClient(string name, double lat, double lng, StringBuilder stringBuilder)
    {
        double lat2 = Math.Round(lat * LatLngChange, 0);
        double lng2 = Math.Round(lng * LatLngChange, 0);

        stringBuilder.AppendLine($"""createClient("{name}", {lat2}, {lng2})""");
    }

    private void CreateDepot(Depot depot)
    {
        _depotStringBuilder.AppendLine($"""createDepot("{depot.DepotName}", {DepotPVRPId})""");
    }

    private void SetDepotInformation(Depot depot, int projectMinTime)
    {
        _depotStringBuilder.AppendLine($"setDepotInformation({DepotPVRPId}, 1, {depot.ServiceFixTime}, {depot.ServiceVarTime}, {projectMinTime}, 0, 0, 0, 0)");
    }

    private void CreateEntry(Depot depot)
    {
        ClientIds.Add(depot.ID, DepotPVRPId);
    }

    public StringBuilder Render(IEnumerable<Client> clients)
    {
        int pvrpId = DepotPVRPId + 1;
        foreach (var client in clients)
        {
            CreateClient(client.ClientName, client.Lat, client.Lng, _clientsStringBuilder);

            SetClientInformation(pvrpId, client);

            CreateEntry(pvrpId, client);

            pvrpId++;
        }

        return _clientsStringBuilder;
    }

    private void SetClientInformation(int pvrpId, Client client)
    {
        _clientsStringBuilder.AppendLine($"setClientInformation({pvrpId}, {client.ServiceFixTime}, 0, 0, 0, 0, 0)");
    }

    private void CreateEntry(int pvrpId, Client client)
    {
        ClientIds.Add(client.ID, pvrpId);
    }
}
