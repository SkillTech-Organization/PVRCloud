using FluentValidation;
using PVRPCloud;
using PVRPCloud.Requests;

namespace PVRPCloudApi.Validators;

public sealed class ProjectValidator : AbstractValidator<PVRPCloudProject>
{
    public ProjectValidator()
    {
        RuleFor(x => x.ProjectName)
            .NotEmpty().WithMessage(PVRPCloudMessages.ERR_EMPTY)
            .NotNull().WithMessage(PVRPCloudMessages.ERR_MANDATORY);

        RuleFor(x => x.MinTime)
            .NotNull().WithMessage(PVRPCloudMessages.ERR_MANDATORY);

        RuleFor(x => x.MaxTime)
            .NotNull().WithMessage(PVRPCloudMessages.ERR_EMPTY)
            .GreaterThan(x => x.MinTime).WithMessage(PVRPCloudMessages.ERR_DATEINTERVAL);

        RuleFor(x => x.MaxTourDuration)
            .GreaterThan(0).WithMessage(PVRPCloudMessages.ERR_ZERO);

        RuleFor(x => x.DistanceLimit)
            .GreaterThanOrEqualTo(0).WithMessage(PVRPCloudMessages.ERR_NEGATIVE);

        RuleFor(x => x.CostProfiles)
            .NotEmpty().WithMessage(PVRPCloudMessages.ERR_EMPTY);

        RuleFor(x => x.Trucks)
            .NotEmpty().WithMessage(PVRPCloudMessages.ERR_EMPTY);

        RuleFor(x => x.Depot)
            .NotNull().WithMessage(PVRPCloudMessages.ERR_MANDATORY);

        RuleFor(x => x.Clients)
            .NotEmpty().WithMessage(PVRPCloudMessages.ERR_EMPTY);

        RuleFor(x => x.Orders)
            .NotEmpty().WithMessage(PVRPCloudMessages.ERR_EMPTY);

        ValidateTruckTypes();

        ValidateCapacityProfiles();

        ValidateClients();

        ValidateCostProfileIds();

        ValidateTrucks();

        ValidateDepot();

        ValidateOrders();
    }

    private void ValidateTruckTypes()
    {
        RuleFor(x => x.TruckTypes)
            .Must(CheckUniquness).WithMessage(PVRPCloudMessages.ERR_ID_UNIQUE);
    }

    private void ValidateCostProfileIds()
    {
        RuleFor(x => x.CostProfiles)
            .Must(CheckUniquness).WithMessage(PVRPCloudMessages.ERR_ID_UNIQUE);
    }

    private void ValidateCapacityProfiles()
    {
        RuleFor(x => x.CapacityProfiles)
            .Must(CheckUniquness).WithMessage(PVRPCloudMessages.ERR_ID_UNIQUE);
    }

    private void ValidateClients()
    {
        RuleFor(x => x.Clients)
            .Must(CheckUniquness).WithMessage(PVRPCloudMessages.ERR_ID_UNIQUE);
    }

    private void ValidateTrucks()
    {
        static bool AreTruckTimesCorrect(PVRPCloudProject project, List<PVRPCloud.Requests.PVRPCloudTruck> trucks) => trucks
            .All(truck => truck.ArrDepotMaxTime >= project.MinTime && truck.ArrDepotMaxTime <= project.MaxTime);

        static bool AreCapacityProdileIdsValid(PVRPCloudProject project, List<PVRPCloud.Requests.PVRPCloudTruck> trucks)
        {
            var capacityProfileIDs = project.CapacityProfiles
                .Select(capacityProfile => capacityProfile.ID)
                .ToArray();

            return trucks
                .Select(truck => truck.CapacityProfileID)
                .All(truckCapacityProfileID => capacityProfileIDs
                    .Contains(truckCapacityProfileID));
        }

        static bool AreLatestStartsCorrect(PVRPCloudProject project, List<PVRPCloud.Requests.PVRPCloudTruck> trucks) => trucks
            .All(truck => truck.LatestStart < project.MaxTime);

        static bool AreTruckTypesIdsValid(PVRPCloudProject project, List<PVRPCloud.Requests.PVRPCloudTruck> trucks)
        {
            var truckTypeIds = project.TruckTypes
                .Select(x => x.ID)
                .ToArray();

            return trucks
                .Select(x => x.TruckTypeID)
                .All(x => truckTypeIds.Contains(x));
        }

        RuleFor(x => x.Trucks)
            .Must(CheckUniquness).WithMessage(PVRPCloudMessages.ERR_ID_UNIQUE)
            .Must(AreTruckTypesIdsValid).WithMessage(PVRPCloudMessages.ERR_ID_UNIQUE)
            .Must(AreTruckTimesCorrect).WithMessage(PVRPCloudMessages.ERR_DATEINTERVAL)
            .Must(AreCapacityProdileIdsValid).WithMessage(PVRPCloudMessages.ERR_RANGE)
            .Must(AreLatestStartsCorrect).WithMessage(PVRPCloudMessages.ERR_DATEINTERVAL);
    }

    private void ValidateDepot()
    {
        static bool AreDepoTimesCorrect(PVRPCloudProject project, PVRPCloudDepot depot) =>
            depot.DepotMinTime >= project.MinTime && depot.DepotMaxTime <= project.MinTime;

        RuleFor(x => x.Depot)
            .Must(AreDepoTimesCorrect).WithMessage(PVRPCloudMessages.ERR_DATEINTERVAL);
    }

    private void ValidateOrders()
    {
        static bool AreClientIdsCorrect(PVRPCloudProject project, List<PVRPCloudOrder> orders)
        {
            var clientIds = project.Clients
                .Select(x => x.ID)
                .ToArray();

            return orders.All(order => clientIds.Contains(order.ClientID));
        }

        static bool AreTruckIdsCorrect(PVRPCloudProject project, List<PVRPCloudOrder> orders)
        {
            if (orders.Count == 0)
                return true;

            var truckIds = project.Trucks
                .Select(x => x.ID)
                .ToArray();

            return orders
                .SelectMany(x => x.TruckList)
                .Distinct()
                .All(x => truckIds.Contains(x));
        }

        RuleFor(x => x.Orders)
            .Must(CheckUniquness).WithMessage(PVRPCloudMessages.ERR_ID_UNIQUE)
            .Must(AreClientIdsCorrect).WithMessage(PVRPCloudMessages.ERR_RANGE)
            .Must(AreTruckIdsCorrect).WithMessage(PVRPCloudMessages.ERR_ID_UNIQUE);
    }

    private bool CheckUniquness(IReadOnlyCollection<IIdentifiable> values)
    {
        var hashedValues = values.Select(x => x.ID).ToHashSet();

        return values.Count == hashedValues.Count;
    }
}
