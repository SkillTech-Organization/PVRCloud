using FluentValidation;
using PVRPCloud.Requests;

namespace PVRPCloudApi.Validators;

public sealed class OrderValidator : AbstractValidator<PVRPCloudOrder>
{
    public OrderValidator()
    {
        RuleFor(x => x.ID).NotEqual(0);

        // ask
        RuleFor(x => x.ClientID);

        RuleFor(x => x.Quantity1).NotEqual(0);

        RuleFor(x => x.ReadyTime).NotEqual(0);

        RuleFor(x => x.Trucks).Must(x => x.Count >= 1);

        RuleFor(x => x.OrderServiceTime).GreaterThanOrEqualTo(0);

        RuleFor(x => x.OrderMinTime).GreaterThanOrEqualTo(0);

        RuleFor(x => x.OrderMaxTime).GreaterThanOrEqualTo(0);
    }
}
