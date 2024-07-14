using FluentValidation;
using FluentValidation.Results;
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
            .MustContainAll(_restrictedZones)
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

        RuleForEach(x => x.SpeedValues)
            .Custom((value, context) =>
            {
                bool isValid = value.Key is >= 1 and <= 7;

                if (isValid)
                    return;

                context.AddFailure(new ValidationFailure()
                {
                    AttemptedValue = value.Key,
                    CustomState = context.InstanceToValidate.ID,
                    PropertyName = context.PropertyName,
                    ErrorMessage = $"{context.DisplayName}: mező értéke 1 és 7 lözött kell legyen. A megadott érték: {value.Key}."
                });
            });
    }
}
