using FluentValidation;
using PVRPCloud;
using PVRPCloud.Requests;

namespace PVRPCloudApi.Validators;

using static ValidationHelpers;

public sealed class TruckTypeValidator : ValidatorBase<PVRPCloudTruckType>
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
            .NotEmpty().WithMessage(PVRPCloudMessages.ERR_EMPTY)
            .NotNull().WithMessage(PVRPCloudMessages.ERR_MANDATORY)
            .Must(IsUnique(truckTypeIds)).WithMessage(PVRPCloudMessages.ERR_ID_UNIQUE)
            .WithState(GetIdentifiableId);

        RuleFor(x => x.TruckTypeName)
            .NotNull().WithMessage(PVRPCloudMessages.ERR_MANDATORY)
            .NotEmpty().WithMessage(PVRPCloudMessages.ERR_EMPTY)
            .WithState(GetIdentifiableId);

        RuleFor(x => x.RestrictedZones)
            .Must(x => x.All(Contains(_restrictedZones))).WithMessage(PVRPCloudMessages.ERR_NOT_FOUND)
            .WithState(GetIdentifiableId)
            .When(x => x.RestrictedZones.Count > 0);

        RuleFor(x => x.Weight)
            .GreaterThanOrEqualTo(0).WithMessage(PVRPCloudMessages.ERR_NEGATIVE)
            .WithState(GetIdentifiableId);

        RuleFor(x => x.XHeight)
            .GreaterThanOrEqualTo(0).WithMessage(PVRPCloudMessages.ERR_NEGATIVE)
            .WithState(GetIdentifiableId);

        RuleFor(x => x.XWidth)
            .GreaterThanOrEqualTo(0).WithMessage(PVRPCloudMessages.ERR_NEGATIVE)
            .WithState(GetIdentifiableId);

        RuleFor(x => x.SpeedValues)
            .Must(CheckRoadValues).WithMessage(PVRPCloudMessages.ERR_RANGE)
            .WithState(GetIdentifiableId);
    }

    private static bool CheckRoadValues(IReadOnlyDictionary<int, int> speedValues)
    {
        return speedValues.Keys.All(roadType => roadType is >= 1 and <= 7);
    }
}
