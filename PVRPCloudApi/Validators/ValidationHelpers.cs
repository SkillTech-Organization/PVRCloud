using PVRPCloud.Models;

namespace PVRPCloudApi.Validators;

public static class ValidationHelpers
{
    public static string GetIdentifiableId(IIdentifiable identifiable) => identifiable.ID;

    public static IEnumerable<string> IdsToArray(IEnumerable<IIdentifiable> identifiables) => identifiables
        .Select(GetIdentifiableId)
        .ToArray();

    public static Func<string, bool> IsUnique(IEnumerable<string> ids) => id => ids.Count(x => x == id) == 1;

    public static Func<string, bool> Contains(IEnumerable<string> ids) => id => ids.Contains(id);
}
