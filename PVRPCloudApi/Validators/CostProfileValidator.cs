using FluentValidation;
using PVRPCloud.Requests;

namespace PVRPCloudApi;

public sealed class CostProfileValidator : AbstractValidator<PVRPCloudCostProfile>
{
    public CostProfileValidator()
    {
        // todo: mihez képest egyedi?
        RuleFor(x => x.ID)
            .NotEmpty()
            .NotNull();

        RuleFor(x => x.FixCost).GreaterThanOrEqualTo(0);

        RuleFor(x => x.HourCost).GreaterThanOrEqualTo(0);

        RuleFor(x => x.KmCost).GreaterThanOrEqualTo(0);
    }
}
