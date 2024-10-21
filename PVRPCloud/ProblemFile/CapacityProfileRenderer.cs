using PVRPCloud.Models;
using System.Text;

namespace PVRPCloud.ProblemFile;

public sealed class CapacityProfileRenderer
{
    private readonly Dictionary<string, int> _profiles = [];

    public IReadOnlyDictionary<string, int> Profiles => _profiles.AsReadOnly();

    public StringBuilder Render(IEnumerable<CapacityProfile> capacityProfiles)
    {
        StringBuilder sb = new();

        int pvrpId = 1;
        foreach (var capacityProfile in capacityProfiles)
        {
            _profiles.Add(capacityProfile.ID, pvrpId);
            pvrpId++;

            var calcCapacity1 = Math.Ceiling((double)capacityProfile.Capacity1 * CommonUtils.Consts.Quantity1Multiplier);

            sb.AppendLine($"createCapacityProfile({calcCapacity1}, {capacityProfile.Capacity2}, 0, 0, 0)");
        }

        return sb;
    }
}
