using PVRPCloud.Models;
using System.Text;

namespace PVRPCloud.ProblemFile;

public sealed class ProjectRenderer : IProjectRenderer
{
    private readonly StringBuilder _sb = new();
    private PvrpData? _pvrpData;

    public string Render(Project project,
                         List<NodeCombination> clientPairs,
                         List<PMapRoute> routes,
                         string _requestID)
    {
        _sb.AppendLine(new SetCustomerIdRenderer().Render());

        CostProfileRenderer costProfileRenderer = new();
        _sb.Append(costProfileRenderer.Render(project.CostProfiles));

        TruckTypeRenderer truckTypeRenderer = new();
        _sb.Append(truckTypeRenderer.Render(project.TruckTypes));

        ClientRenderer clientRenderer = new();
        _sb.Append(clientRenderer.Render(project.Depot, project.MinTime));

        CapacityProfileRenderer capacityProfileRenderer = new();
        _sb.Append(capacityProfileRenderer.Render(project.CapacityProfiles));

        TruckRenderer truckRenderer = new(truckTypeRenderer.TruckTypeIds, costProfileRenderer.CostProfiles, capacityProfileRenderer.Profiles);
        _sb.Append(truckRenderer.Render(project.Trucks));

        _sb.Append(clientRenderer.Render(project.Clients));

        OrderRenderer orderRenderer = new(clientRenderer.ClientIds, truckRenderer.TruckIds);
        _sb.Append(orderRenderer.Render(project.Orders));

        RelationsRenderer relationsRenderer = new(project.TruckTypes,
                                                  truckTypeRenderer.TruckTypeIds,
                                                  clientPairs,
                                                  clientRenderer.ClientIds);
        _sb.Append(relationsRenderer.Render(routes));

        EnginePropertiesRenderer enginePropertiesRenderer = new();
        _sb.Append(enginePropertiesRenderer.Render(_requestID));

        _pvrpData = new()
        {
            Project = project,
            Routes = routes,
            TruckIds = truckRenderer.TruckIds,
            ClientIds = clientRenderer.ClientIds,
            OrderIds = orderRenderer.OrderIds,
            ClientNodes = clientPairs.GroupBy(g => g.From).ToDictionary(k => k.Key.Identifable.ID, v => v.Key.NodeId)
        };

        return _sb.ToString();
    }

    public PvrpData GetPvrpData()
    {
        return _pvrpData ?? throw new InvalidOperationException("PvrpData is null because it's empty. Call the Render method first.");
    }
}
