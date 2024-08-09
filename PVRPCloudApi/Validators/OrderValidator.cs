using FluentValidation;
using PVRPCloud;
using PVRPCloud.Requests;

namespace PVRPCloudApi.Validators;

using static ValidationHelpers;

public sealed class OrderValidator : AbstractValidator<Order>
{
    public OrderValidator(Project project)
    {
        var ids = IdsToArray(project.Orders);
        RuleFor(x => x.ID)
            .NotEmpty()
            .NotNull()
            .Must(IsUnique(ids)).WithMessage(Messages.ERR_ID_UNIQUE)
            .WithState(GetIdentifiableId);

        var clientIds = IdsToArray(project.Clients);
        RuleFor(x => x.ClientID)
            .NotEmpty()
            .NotNull()
            .Must(Contains(clientIds)).WithMessage(Messages.ERR_NOT_FOUND)
            .WithState(GetIdentifiableId);

        RuleFor(x => x.Quantity1)
            .NotNull()
            .WithState(GetIdentifiableId);

        RuleFor(x => x.ReadyTime)
            .NotNull()
            .WithState(GetIdentifiableId);

        RuleFor(x => x.OrderServiceTime)
            .GreaterThanOrEqualTo(0)
            .WithState(GetIdentifiableId);

        RuleFor(x => x.OrderMinTime)
            .GreaterThanOrEqualTo(0)
            .WithState(GetIdentifiableId);

        RuleFor(x => x.OrderMaxTime)
            .GreaterThanOrEqualTo(0)
            .WithState(GetIdentifiableId);

        var truckIds = IdsToArray(project.Trucks);
        RuleFor(x => x.TruckIDs)
            .MustContainAll(truckIds)
            .WithState(GetIdentifiableId);
    }
}
