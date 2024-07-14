using FluentValidation;
using PVRPCloud;
using PVRPCloud.Requests;

namespace PVRPCloudApi.Validators;

using static ValidationHelpers;

public sealed class OrderValidator : AbstractValidator<PVRPCloudOrder>
{
    public OrderValidator(PVRPCloudProject project)
    {
        var ids = IdsToArray(project.Orders);
        RuleFor(x => x.ID)
            .NotEmpty().WithMessage(PVRPCloudMessages.ERR_EMPTY)
            .NotNull().WithMessage(PVRPCloudMessages.ERR_MANDATORY)
            .Must(IsUnique(ids)).WithMessage(PVRPCloudMessages.ERR_ID_UNIQUE)
            .WithState(GetIdentifiableId);

        var clientIds = IdsToArray(project.Clients);
        RuleFor(x => x.ClientID)
            .NotNull().WithMessage(PVRPCloudMessages.ERR_MANDATORY)
            .Must(Contains(clientIds)).WithMessage(PVRPCloudMessages.ERR_NOT_FOUND)
            .WithState(GetIdentifiableId);

        RuleFor(x => x.Quantity1)
            .NotNull().WithMessage(PVRPCloudMessages.ERR_MANDATORY)
            .WithState(GetIdentifiableId);

        RuleFor(x => x.ReadyTime)
            .NotNull().WithMessage(PVRPCloudMessages.ERR_MANDATORY)
            .WithState(GetIdentifiableId);

        RuleFor(x => x.OrderServiceTime)
            .GreaterThanOrEqualTo(0).WithMessage(PVRPCloudMessages.ERR_DATEINTERVAL)
            .WithState(GetIdentifiableId);

        RuleFor(x => x.OrderMinTime)
            .GreaterThanOrEqualTo(0).WithMessage(PVRPCloudMessages.ERR_DATEINTERVAL)
            .WithState(GetIdentifiableId);

        RuleFor(x => x.OrderMaxTime)
            .GreaterThanOrEqualTo(0).WithMessage(PVRPCloudMessages.ERR_DATEINTERVAL)
            .WithState(GetIdentifiableId);

        var truckIds = IdsToArray(project.Trucks);
        RuleFor(x => x.TruckIDs)
            .Must(x => x.All(Contains(truckIds))).WithMessage(PVRPCloudMessages.ERR_NOT_FOUND)
            .WithState(GetIdentifiableId);
    }
}
