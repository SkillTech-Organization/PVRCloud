using FluentValidation;
using PVRPCloud.Requests;

namespace PVRPCloudApi.Validators;

public sealed class TruckValidator : AbstractValidator<PVRPCloudTruck>
{
    public TruckValidator()
    {
        RuleFor(x => x.ID).NotEqual(0);

        RuleFor(x => x.TruckTypeID).NotEqual(0);

        RuleFor(x => x.TruckName)
            .NotNull()
            .NotEmpty();

        RuleFor(x => x.StartDepotID).NotEqual(0);

        RuleFor(x => x.ArrDepotID).NotEqual(0);

        RuleFor(x => x.ArrDepotMaxTime).GreaterThan(0);

        RuleFor(x => x.CapacityProfileID).NotEqual(0);

        RuleFor(x => x.MaxWorkTime).GreaterThan(0);

        RuleFor(x => x.EarliestStart).GreaterThan(0);

        RuleFor(x => x.LatestStart).GreaterThan(x => x.EarliestStart);
    }
}
