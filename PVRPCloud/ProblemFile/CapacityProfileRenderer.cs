using System.Text;
using PVRPCloud.Requests;

namespace PVRPCloud.ProblemFile;

public sealed class CapacityProfileRenderer
{
    private readonly Dictionary<int, CapacityProfile> _profiles = [];

    public IReadOnlyDictionary<int, CapacityProfile> Profiles => _profiles.AsReadOnly();

    public StringBuilder Render(IEnumerable<CapacityProfile> capacityProfiles)
    {
        StringBuilder sb = new();

        int pvrpId = 1;
        foreach (var capacityProfile in capacityProfiles)
        {
            _profiles.Add(pvrpId, capacityProfile);
            pvrpId++;

            sb.AppendLine($"createCapacityProfile({capacityProfile.Capacity1}, {capacityProfile.Capacity2}, 0, 0, 0)");
        }

        return sb;
    }
}
