using FluentValidation;
using PVRPCloud;
using PVRPCloud.Requests;
using static PVRPCloudApi.Validators.ValidationHelpers;

namespace PVRPCloudApi.Validators;

public sealed class TruckTypeValidator : AbstractValidator<PVRPCloudTruckType>
{
    private readonly IEnumerable<string> _restrictedZones = [
        "KP1",
        "ÉP1",
        "DB1",
        "HB1",
        "DP3",
        "DP1",
        "CS12",
        "ÉB1",
        "ÉB7",
        "CS7",
        "DP7",
        "KV3",
        "P75",
        "B35",
        "P35",
    ];

    public TruckTypeValidator(PVRPCloudProject project)
    {
        var truckTypeIds = IdsToArray(project.TruckTypes);
        RuleFor(x => x.ID)
            .NotEmpty()
            .NotNull()
            .Must(IsUnique(truckTypeIds)).WithMessage(PVRPCloudMessages.ERR_ID_UNIQUE)
            .WithState(GetIdentifiableId);

        RuleFor(x => x.TruckTypeName)
            .NotNull()
            .NotEmpty()
            .WithState(GetIdentifiableId);

        RuleFor(x => x.RestrictedZones)
            .Must((_, zones, context) => {
                List<string> invalidValues = new(20);
                bool isValid = zones.All(x => {
                    bool contains = _restrictedZones.Contains(x);
                    if (!contains)
                        invalidValues.Add(x);

                    return contains;
                });

                context.MessageFormatter.AppendArgument("CollectionValues", string.Join(",", invalidValues));
                return isValid;
            }).WithMessage(PVRPCloudMessages.ERR_NOT_FOUND)
            .WithState(GetIdentifiableId);

        RuleFor(x => x.Weight)
            .GreaterThanOrEqualTo(0)
            .WithState(GetIdentifiableId);

        RuleFor(x => x.XHeight)
            .GreaterThanOrEqualTo(0)
            .WithState(GetIdentifiableId);

        RuleFor(x => x.XWidth)
            .GreaterThanOrEqualTo(0)
            .WithState(GetIdentifiableId);

        RuleFor(x => x.SpeedValues)
            .Must(CheckRoadValues).WithMessage(PVRPCloudMessages.ERR_RANGE)
            .WithState(GetIdentifiableId);
    }

    private bool EmptyOrContainsRestrictionZones(IReadOnlyCollection<string> incomingZones)
    {
        return incomingZones.Count == 0 ||
            incomingZones.All(zone => _restrictedZones.Contains(zone));
    }

    private static bool CheckRoadValues(IReadOnlyDictionary<int, int> speedValues)
    {
        return speedValues.Keys.All(roadType => roadType is >= 1 and <= 7);
    }
}
