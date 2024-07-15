using FluentValidation;
using PVRPCloud;
using PVRPCloud.Requests;

namespace PVRPCloudApi.Validators;

using static ValidationHelpers;

public sealed class ClientValidator : AbstractValidator<PVRPCloudClient>
{
    public ClientValidator(PVRPCloudProject project)
    {
        var ids = IdsToArray(project.Clients);
        RuleFor(x => x.ID)
            .NotEmpty().WithMessage(PVRPCloudMessages.ERR_EMPTY)
            .NotNull().WithMessage(PVRPCloudMessages.ERR_MANDATORY)
            .Must(IsUnique(ids)).WithMessage(PVRPCloudMessages.ERR_ID_UNIQUE)
            .WithState(GetIdentifiableId);

        RuleFor(x => x.ClientName)
            .NotNull().WithMessage(PVRPCloudMessages.ERR_MANDATORY)
            .NotEmpty().WithMessage(PVRPCloudMessages.ERR_EMPTY)
            .WithState(GetIdentifiableId);

        RuleFor(x => x.Lat)
            .NotNull().WithMessage(PVRPCloudMessages.ERR_MANDATORY)
            .InclusiveBetween(-90, 90).WithMessage(PVRPCloudMessages.ERR_RANGE)
            .WithState(GetIdentifiableId);

        RuleFor(x => x.Lng)
            .NotNull().WithMessage(PVRPCloudMessages.ERR_MANDATORY)
            .InclusiveBetween(-180, 190).WithMessage(PVRPCloudMessages.ERR_RANGE)
            .WithState(GetIdentifiableId);

        RuleFor(x => x.ServiceFixTime)
            .GreaterThanOrEqualTo(0).WithMessage(PVRPCloudMessages.ERR_DATEINTERVAL)
            .WithState(GetIdentifiableId);
    }
}
