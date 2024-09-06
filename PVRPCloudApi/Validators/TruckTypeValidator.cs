using FluentValidation;
using PVRPCloud;
using PVRPCloud.Models;

namespace PVRPCloudApi.Validators;

public sealed class TruckTypeValidator : AbstractValidator<TruckType>
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

    public TruckTypeValidator(Project project)
    {
        var truckTypeIds = ValidationHelpers.IdsToArray(project.TruckTypes);
        RuleFor(x => x.ID)
            .NotEmpty().WithMessage(Messages.ERR_EMPTY)
            .NotNull().WithMessage(Messages.ERR_MANDATORY)
            .Must(ValidationHelpers.IsUnique(truckTypeIds)).WithMessage(Messages.ERR_ID_UNIQUE)
            .WithState(ValidationHelpers.GetIdentifiableId);

        RuleFor(x => x.TruckTypeName)
            .NotNull().WithMessage(Messages.ERR_MANDATORY)
            .NotEmpty().WithMessage(Messages.ERR_EMPTY)
            .WithState(ValidationHelpers.GetIdentifiableId);

        RuleFor(x => x.RestrictedZones)
            .Must(x => x.All(ValidationHelpers.Contains(_restrictedZones))).WithMessage(Messages.ERR_NOT_FOUND)
            .WithState(ValidationHelpers.GetIdentifiableId)
            .When(x => x.RestrictedZones.Count > 0);

        RuleFor(x => x.Weight)
            .GreaterThanOrEqualTo(0).WithMessage(Messages.ERR_NEGATIVE)
            .WithState(ValidationHelpers.GetIdentifiableId);

        RuleFor(x => x.XHeight)
            .GreaterThanOrEqualTo(0).WithMessage(Messages.ERR_NEGATIVE)
            .WithState(ValidationHelpers.GetIdentifiableId);

        RuleFor(x => x.XWidth)
            .GreaterThanOrEqualTo(0).WithMessage(Messages.ERR_NEGATIVE)
            .WithState(ValidationHelpers.GetIdentifiableId);

        RuleFor(x => x.SpeedValues)
            .Must(CheckRoadValues).WithMessage(Messages.ERR_RANGE)
            .WithState(ValidationHelpers.GetIdentifiableId);
    }

    private static bool CheckRoadValues(IReadOnlyDictionary<int, int> speedValues)
    {
        return speedValues.Keys.All(roadType => roadType is >= 1 and <= 7);
    }
}
