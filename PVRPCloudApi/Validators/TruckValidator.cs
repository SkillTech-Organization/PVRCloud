using FluentValidation;
using PVRPCloud;
using PVRPCloud.Requests;

namespace PVRPCloudApi.Validators;

public sealed class TruckValidator : ValidatorBase<PVRPCloud.Requests.PVRPCloudTruck>
{
    public TruckValidator(PVRPCloudProject project)
    {
        var truckIds = GetIdsToArray(project.Trucks);
        RuleFor(x => x.ID)
            .NotNull().WithMessage(PVRPCloudMessages.ERR_MANDATORY)
            .Must(x => truckIds.Count(y => y == x) == 1).WithMessage(PVRPCloudMessages.ERR_ID_UNIQUE)
            .WithState(GetIdentifiableId);

        var truckTypeIds = GetIdsToArray(project.TruckTypes);
        RuleFor(x => x.TruckTypeID)
            .NotNull().WithMessage(PVRPCloudMessages.ERR_MANDATORY)
            .Must(x => truckTypeIds.Contains(x)).WithMessage(PVRPCloudMessages.ERR_NOT_FOUND)
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

        var capacityProfileIds = GetIdsToArray(project.CapacityProfiles);
        RuleFor(x => x.CapacityProfileID)
            .NotNull().WithMessage(PVRPCloudMessages.ERR_MANDATORY)
            .Must(x => capacityProfileIds.Contains(x)).WithMessage(PVRPCloudMessages.ERR_NOT_FOUND)
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

    private IEnumerable<string> GetIdsToArray(IEnumerable<IIdentifiable> identifiables) => identifiables
        .Select(GetIdentifiableId)
        .ToArray();
}
