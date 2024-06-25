using FluentValidation;
using PVRPCloud;
using PVRPCloud.Requests;

namespace PVRPCloudApi.Validators;

public sealed class DepotValidator : AbstractValidator<PVRPCloudDepot>
{
    public DepotValidator()
    {
        RuleFor(x => x.ID)
            .NotNull().WithMessage(PVRPCloudMessages.ERR_MANDATORY);

        RuleFor(x => x.DepotName)
            .NotNull().WithMessage(PVRPCloudMessages.ERR_MANDATORY)
            .NotEmpty().WithMessage(PVRPCloudMessages.ERR_EMPTY);

        RuleFor(x => x.Lat)
            .NotNull().WithMessage(PVRPCloudMessages.ERR_MANDATORY)
            .InclusiveBetween(-90, 90).WithMessage(PVRPCloudMessages.ERR_RANGE);

        RuleFor(x => x.Lng)
            .NotNull().WithMessage(PVRPCloudMessages.ERR_MANDATORY)
            .InclusiveBetween(-180, 190).WithMessage(PVRPCloudMessages.ERR_RANGE);

        // ask
        RuleFor(x => x.DepotMinTime);

        // ask
        RuleFor(x => x.DepotMaxTime);

        RuleFor(x => x.IsCentral)
            .NotNull().WithMessage(PVRPCloudMessages.ERR_MANDATORY);

        RuleFor(x => x.ServiceFixTime)
            .GreaterThanOrEqualTo(0).WithMessage(PVRPCloudMessages.ERR_DATEINTERVAL);

        RuleFor(x => x.ServiceVarTime)
            .GreaterThanOrEqualTo(0).WithMessage(PVRPCloudMessages.ERR_DATEINTERVAL);
    }
}
