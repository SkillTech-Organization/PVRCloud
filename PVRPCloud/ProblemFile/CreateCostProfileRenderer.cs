using System.Text;
using PVRPCloud.Requests;

namespace PVRPCloud.ProblemFile;

public sealed class CreateCostProfileRenderer
{
    private readonly Dictionary<int, CostProfile> _costProfiles = [];

    public IReadOnlyDictionary<int, CostProfile> CostProfiles => _costProfiles.AsReadOnly();

    public StringBuilder Render(IEnumerable<CostProfile> costProfiles)
    {
        StringBuilder sb = new();

        int pvrpId = 1;
        foreach (var costProfile in costProfiles)
        {
            _costProfiles.Add(pvrpId, costProfile);
            pvrpId++;

            sb.AppendLine($"createCostProfile({costProfile.FixCost}, {costProfile.KmCost}, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, {costProfile.HourCost}, 0, 0, 0, 0)");
        }

        return sb;
    }
}
