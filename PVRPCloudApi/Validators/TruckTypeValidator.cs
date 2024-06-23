using FluentValidation;
using PVRPCloud.Requests;

namespace PVRPCloudApi.Validators;

public sealed class TruckTypeValidator : AbstractValidator<PVRPCloudTruckType>
{
    private readonly string[] _restrictedZones = [
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

    public TruckTypeValidator()
    {
        // todo: kötelező
        RuleFor(x => x.ID).NotEqual(0);

        RuleFor(x => x.TruckName)
            .NotEmpty()
            .NotNull();

        RuleFor(x => x.RestrictedZones)
            .Must(EmptyOrContainsRestrictionZones);

        RuleFor(x => x.Weight).GreaterThanOrEqualTo(0);

        RuleFor(x => x.XHeight).GreaterThanOrEqualTo(0);

        RuleFor(x => x.XWidth).GreaterThanOrEqualTo(0);

        // ask
        RuleFor(x => x.SpeedValues);
    }

    private bool EmptyOrContainsRestrictionZones(IReadOnlyCollection<string> incomingZones)
    {
        return incomingZones.Count == 0 || incomingZones.All(zone => _restrictedZones.Contains(zone));
    }
}
