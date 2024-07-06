using FluentValidation;
using PVRPCloud;
using PVRPCloud.Requests;

namespace PVRPCloudApi.Validators;

public sealed class ProjectValidator : ValidatorBase<PVRPCloudProject>
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

        TruckValidator? truckValidator = null;
        RuleForEach(x => x.Trucks).SetValidator(project => truckValidator ??= new TruckValidator(project));

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
