using System.Text;
using PVRPCloud.Requests;

namespace PVRPCloud.ProblemFile;

public sealed class ClientRenderer
{
    private const int LatLngChange = 1_000_000;
    private const int DepotPVRPId = 1;

    private readonly Dictionary<int, Entry> _clients = [];

    private readonly StringBuilder _depotStringBuilder = new();
    private readonly StringBuilder _clientsStringBuilder = new();

    public IReadOnlyDictionary<int, Entry> Clients => _clients.AsReadOnly();

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
        _clients.Add(DepotPVRPId, new Entry(depot));
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
        _clients.Add(pvrpId, new Entry(client));
    }

    public sealed class Entry
    {
        private readonly Depot? _depot;

        private readonly Client? _client;

        public Depot Depot => _depot ?? throw EntryMissmatchException.NotADepot();

        public Client Client => _client ?? throw EntryMissmatchException.NotAClient();

        public Entry(Depot depot)
        {
            _depot = depot;
        }

        public Entry(Client client)
        {
            _client = client;
        }
    }

    private sealed class EntryMissmatchException : Exception
    {
        public static EntryMissmatchException NotADepot() => new("Entry is not a Depot");

        public static EntryMissmatchException NotAClient() => new("Entry is not a Client");

        private EntryMissmatchException(string message): base(message) {}
    }
}
