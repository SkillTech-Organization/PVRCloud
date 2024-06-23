using FluentValidation;
using PVRPCloud.Requests;

namespace PVRPCloudApi.Validators;

public sealed class DepotValidator : AbstractValidator<PVRPCloudDepot>
{
    public DepotValidator()
    {
        RuleFor(x => x.ID).NotEqual(0);

        RuleFor(x => x.DepotName)
            .NotNull()
            .NotEmpty();

        RuleFor(x => x.Lat).InclusiveBetween(-90, 90);

        RuleFor(x => x.Lng).InclusiveBetween(-180, 190);

        // ask
        RuleFor(x => x.DepotMinTime);

        // ask
        RuleFor(x => x.DepotMaxTime);

        RuleFor(x => x.IsCentral).NotEqual(0);

        RuleFor(x => x.ServiceFixTime).GreaterThanOrEqualTo(0);

        RuleFor(x => x.ServiceVarTime).GreaterThanOrEqualTo(0);
    }
}
