using System.Text;
using PVRPCloud.Models;

namespace PVRPCloud.ProblemFile;

public sealed class CostProfileRenderer
{
    private readonly Dictionary<string, int> _costProfiles = [];

    public IReadOnlyDictionary<string, int> CostProfiles => _costProfiles.AsReadOnly();

    public StringBuilder Render(IEnumerable<CostProfile> costProfiles)
    {
        StringBuilder sb = new();

        int pvrpId = 1;
        foreach (var costProfile in costProfiles)
        {
            _costProfiles.Add(costProfile.ID, pvrpId);
            pvrpId++;

            sb.AppendLine($"createCostProfile({costProfile.FixCost}, {costProfile.KmCost}, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, {costProfile.HourCost}, 0, 0, 0, 0)");
        }

        return sb;
    }
}
