using FluentValidation;
using PVRPCloud;
using PVRPCloud.Requests;

namespace PVRPCloudApi.Validators;

public sealed class TruckValidator : AbstractValidator<PVRPCloud.Requests.PVRPCloudTruck>
{
    public TruckValidator(Project project)
    {
/* PR teszt I */
        var truckIds = ValidationHelpers.IdsToArray(project.Trucks);
        RuleFor(x => x.ID)
            .NotEmpty().WithMessage(Messages.ERR_EMPTY)
            .NotNull().WithMessage(Messages.ERR_MANDATORY)
            .Must(ValidationHelpers.IsUnique(truckIds)).WithMessage(Messages.ERR_ID_UNIQUE)
            .WithState(ValidationHelpers.GetIdentifiableId);

        var truckTypeIds = ValidationHelpers.IdsToArray(project.TruckTypes);
        RuleFor(x => x.TruckTypeID)
            .NotEmpty().WithMessage(Messages.ERR_EMPTY)
            .NotNull().WithMessage(Messages.ERR_MANDATORY)
            .Must(ValidationHelpers.Contains(truckTypeIds)).WithMessage(Messages.ERR_NOT_FOUND)
            .WithState(ValidationHelpers.GetIdentifiableId);

        RuleFor(x => x.TruckName)
            .NotNull().WithMessage(Messages.ERR_MANDATORY)
            .NotEmpty().WithMessage(Messages.ERR_EMPTY)
            .WithState(ValidationHelpers.GetIdentifiableId);

        RuleFor(x => x.ArrDepotMaxTime)
            .GreaterThan(0).WithMessage(Messages.ERR_ZERO)
            .GreaterThanOrEqualTo(project.MinTime).WithMessage(Messages.ERR_DATEINTERVAL)
            .LessThanOrEqualTo(project.MaxTime).WithMessage(Messages.ERR_DATEINTERVAL)
            .WithState(ValidationHelpers.GetIdentifiableId);

        var capacityProfileIds = ValidationHelpers.IdsToArray(project.CapacityProfiles);
        RuleFor(x => x.CapacityProfileID)
            .NotEmpty().WithMessage(Messages.ERR_EMPTY)
            .NotNull().WithMessage(Messages.ERR_MANDATORY)
            .Must(ValidationHelpers.Contains(capacityProfileIds)).WithMessage(Messages.ERR_NOT_FOUND)
            .WithState(ValidationHelpers.GetIdentifiableId);

        RuleFor(x => x.MaxWorkTime)
            .NotNull().WithMessage(Messages.ERR_MANDATORY)
            .GreaterThan(0).WithMessage(Messages.ERR_ZERO)
            .WithState(ValidationHelpers.GetIdentifiableId);

        RuleFor(x => x.EarliestStart)
            .NotNull().WithMessage(Messages.ERR_MANDATORY)
            .GreaterThanOrEqualTo(0).WithMessage(Messages.ERR_ZERO)
            .WithState(ValidationHelpers.GetIdentifiableId);

        RuleFor(x => x.LatestStart)
            .NotNull().WithMessage(Messages.ERR_MANDATORY)
            .GreaterThan(x => x.EarliestStart).WithMessage(Messages.ERR_RANGE)
            .LessThan(project.MaxTime).WithMessage(Messages.ERR_DATEINTERVAL)
            .WithState(ValidationHelpers.GetIdentifiableId);
    }
}
