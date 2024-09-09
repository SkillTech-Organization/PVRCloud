using System.Text;
using PVRPCloud.Models;

namespace PVRPCloud.ProblemFile;

public sealed class ProjectRenderer : IProjectRenderer
{
    private readonly StringBuilder _sb = new();

    public string Render(Project project,
                         List<(ClientNodeIdPair From, ClientNodeIdPair To)> clientPairs,
                         IEnumerable<PMapRoute> routes)
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

        OrderRenderer orderRenderer = new(clientRenderer.Clients, truckRenderer.TruckIds);
        _sb.Append(orderRenderer.Render(project.Orders));

        RelationsRenderer relationsRenderer = new(project.TruckTypes,
                                                  truckTypeRenderer.TruckTypeIds,
                                                  clientPairs,
                                                  clientRenderer.Clients);
        _sb.Append(relationsRenderer.Render(routes));

        EnginePropertiesRenderer enginePropertiesRenderer = new();
        _sb.Append(enginePropertiesRenderer.Render(project.ProjectName));

        return _sb.ToString();
    }
}
