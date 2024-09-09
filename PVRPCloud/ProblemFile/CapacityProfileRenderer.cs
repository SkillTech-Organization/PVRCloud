using System.Text;
using PVRPCloud.Models;

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

            sb.AppendLine($"createCapacityProfile({capacityProfile.Capacity1}, {capacityProfile.Capacity2}, 0, 0, 0)");
        }

        return sb;
    }
}
