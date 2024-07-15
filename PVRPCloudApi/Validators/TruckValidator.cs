using FluentValidation;
using PVRPCloud;
using PVRPCloud.Requests;

namespace PVRPCloudApi.Validators;

public sealed class TruckValidator : AbstractValidator<PVRPCloud.Requests.PVRPCloudTruck>
{
    public TruckValidator(PVRPCloudProject project)
    {
        var truckIds = IdsToArray(project.Trucks);
        RuleFor(x => x.ID)
            .NotEmpty().WithMessage(PVRPCloudMessages.ERR_EMPTY)
            .NotNull().WithMessage(PVRPCloudMessages.ERR_MANDATORY)
            .Must(IsUnique(truckIds)).WithMessage(PVRPCloudMessages.ERR_ID_UNIQUE)
            .WithState(GetIdentifiableId);

        var truckTypeIds = IdsToArray(project.TruckTypes);
        RuleFor(x => x.TruckTypeID)
            .NotEmpty().WithMessage(PVRPCloudMessages.ERR_EMPTY)
            .NotNull().WithMessage(PVRPCloudMessages.ERR_MANDATORY)
            .Must(Contains(truckTypeIds)).WithMessage(PVRPCloudMessages.ERR_NOT_FOUND)
            .WithState(GetIdentifiableId);

        RuleFor(x => x.TruckName)
            .NotNull().WithMessage(PVRPCloudMessages.ERR_MANDATORY)
            .NotEmpty().WithMessage(PVRPCloudMessages.ERR_EMPTY)
            .WithState(GetIdentifiableId);

        RuleFor(x => x.ArrDepotMaxTime)
            .GreaterThan(0).WithMessage(PVRPCloudMessages.ERR_ZERO)
            .GreaterThanOrEqualTo(project.MinTime).WithMessage(PVRPCloudMessages.ERR_DATEINTERVAL)
            .LessThanOrEqualTo(project.MaxTime).WithMessage(PVRPCloudMessages.ERR_DATEINTERVAL)
            .WithState(GetIdentifiableId);

        var capacityProfileIds = IdsToArray(project.CapacityProfiles);
        RuleFor(x => x.CapacityProfileID)
            .NotEmpty().WithMessage(PVRPCloudMessages.ERR_EMPTY)
            .NotNull().WithMessage(PVRPCloudMessages.ERR_MANDATORY)
            .Must(Contains(capacityProfileIds)).WithMessage(PVRPCloudMessages.ERR_NOT_FOUND)
            .WithState(GetIdentifiableId);

        RuleFor(x => x.MaxWorkTime)
            .NotNull().WithMessage(PVRPCloudMessages.ERR_MANDATORY)
            .GreaterThan(0).WithMessage(PVRPCloudMessages.ERR_ZERO)
            .WithState(GetIdentifiableId);

        RuleFor(x => x.EarliestStart)
            .NotNull().WithMessage(PVRPCloudMessages.ERR_MANDATORY)
            .GreaterThanOrEqualTo(0).WithMessage(PVRPCloudMessages.ERR_ZERO)
            .WithState(GetIdentifiableId);

        RuleFor(x => x.LatestStart)
            .NotNull().WithMessage(PVRPCloudMessages.ERR_MANDATORY)
            .GreaterThan(x => x.EarliestStart).WithMessage(PVRPCloudMessages.ERR_RANGE)
            .LessThan(project.MaxTime).WithMessage(PVRPCloudMessages.ERR_DATEINTERVAL)
            .WithState(GetIdentifiableId);
    }
}
