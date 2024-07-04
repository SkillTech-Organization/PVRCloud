using FluentValidation;
using PVRPCloud;
using PVRPCloud.Requests;

namespace PVRPCloudApi.Validators;

public sealed class OrderValidator : ValidatorBase<PVRPCloudOrder>
{
    public OrderValidator()
    {
        RuleFor(x => x.ID)
            .NotNull().WithMessage(PVRPCloudMessages.ERR_MANDATORY);

        RuleFor(x => x.ClientID)
            .NotNull().WithMessage(PVRPCloudMessages.ERR_MANDATORY);

        RuleFor(x => x.Quantity1)
            .NotNull().WithMessage(PVRPCloudMessages.ERR_MANDATORY);

        RuleFor(x => x.ReadyTime)
            .NotNull().WithMessage(PVRPCloudMessages.ERR_MANDATORY);

        RuleFor(x => x.OrderServiceTime)
            .GreaterThanOrEqualTo(0).WithMessage(PVRPCloudMessages.ERR_DATEINTERVAL);

        RuleFor(x => x.OrderMinTime)
            .GreaterThanOrEqualTo(0).WithMessage(PVRPCloudMessages.ERR_DATEINTERVAL);

        RuleFor(x => x.OrderMaxTime)
            .GreaterThanOrEqualTo(0).WithMessage(PVRPCloudMessages.ERR_DATEINTERVAL);
    }
}
