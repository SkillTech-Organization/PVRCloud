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

        ValidateCostProfileIds();

        ValidateTrucks();

        ValidateDepot();

        ValidateOrders();
    }

    private void ValidateCostProfileIds()
    {
        static bool CheckUniquness(List<PVRPCloudCostProfile> costProfiles)
        {
            var values = costProfiles.Select(x => x.ID).ToHashSet();

            return values.Count == costProfiles.Count;
        }

        RuleFor(x => x.CostProfiles)
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

        RuleFor(x => x.Trucks)
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
        bool AreClientIdsCorrect(PVRPCloudProject project, List<PVRPCloudOrder> orders)
        {
            var clientIds = project.Clients
                .Select(x => x.ID)
                .ToArray();

            return orders.All(order => clientIds.Contains(order.ClientID));
        }

        RuleFor(x => x.Orders)
            .Must(AreClientIdsCorrect).WithMessage(PVRPCloudMessages.ERR_RANGE);
    }
}
