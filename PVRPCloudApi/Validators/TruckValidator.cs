using FluentValidation;
using PVRPCloud;

namespace PVRPCloudApi.Validators;

public sealed class TruckValidator : AbstractValidator<PVRPCloud.Requests.PVRPCloudTruck>
{
    public TruckValidator()
    {
        RuleFor(x => x.ID)
            .NotNull().WithMessage(PVRPCloudMessages.ERR_MANDATORY);

        RuleFor(x => x.TruckTypeID)
            .NotNull().WithMessage(PVRPCloudMessages.ERR_MANDATORY);

        RuleFor(x => x.TruckName)
            .NotNull().WithMessage(PVRPCloudMessages.ERR_MANDATORY)
            .NotEmpty();

        RuleFor(x => x.StartDepotID)
            .NotNull().WithMessage(PVRPCloudMessages.ERR_MANDATORY);

        RuleFor(x => x.ArrDepotID)
            .NotNull().WithMessage(PVRPCloudMessages.ERR_MANDATORY);

        RuleFor(x => x.ArrDepotMaxTime)
            .GreaterThan(0).WithMessage(PVRPCloudMessages.ERR_ZERO);

        RuleFor(x => x.CapacityProfileID)
            .NotNull().WithMessage(PVRPCloudMessages.ERR_MANDATORY);

        RuleFor(x => x.MaxWorkTime)
            .NotNull().WithMessage(PVRPCloudMessages.ERR_MANDATORY)
            .GreaterThan(0).WithMessage(PVRPCloudMessages.ERR_ZERO);

        RuleFor(x => x.EarliestStart)
            .NotNull().WithMessage(PVRPCloudMessages.ERR_MANDATORY)
            .GreaterThanOrEqualTo(0).WithMessage(PVRPCloudMessages.ERR_ZERO);

        RuleFor(x => x.LatestStart)
            .NotNull().WithMessage(PVRPCloudMessages.ERR_MANDATORY)
            .GreaterThan(x => x.EarliestStart).WithMessage(PVRPCloudMessages.ERR_RANGE);
    }
}
