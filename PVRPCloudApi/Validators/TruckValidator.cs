using FluentValidation;
using PVRPCloud;
using PVRPCloud.Models;

namespace PVRPCloudApi.Validators;

using static ValidationHelpers;

public sealed class TruckValidator : AbstractValidator<PVRPCloud.Models.Truck>
{
    private readonly int[] _eTollCategories = [1, 2, 2, 4, 5, 6];
    private readonly int[] _environmentalClasses = [0, 1, 2, 3, 4, 5, 6, 99, 100];

    public TruckValidator(Project project)
    {
        var truckIds = IdsToArray(project.Trucks);
        RuleFor(x => x.ID)
            .NotEmpty()
            .NotNull()
            .Must(IsUnique(truckIds)).WithMessage(Messages.ERR_ID_UNIQUE)
            .WithState(GetIdentifiableId);

        var truckTypeIds = IdsToArray(project.TruckTypes);
        RuleFor(x => x.TruckTypeID)
            .NotEmpty()
            .NotNull()
            .Must(Contains(truckTypeIds)).WithMessage(Messages.ERR_NOT_FOUND)
            .WithState(GetIdentifiableId);

        RuleFor(x => x.TruckName)
            .NotNull()
            .NotEmpty()
            .WithState(GetIdentifiableId);

        RuleFor(x => x.ArrDepotMaxTime)
            .GreaterThan(0)
            .GreaterThanOrEqualTo(project.MinTime)
            .LessThanOrEqualTo(project.MaxTime)
            .WithState(GetIdentifiableId);

        var capacityProfileIds = IdsToArray(project.CapacityProfiles);
        RuleFor(x => x.CapacityProfileID)
            .NotEmpty()
            .NotNull()
            .Must(Contains(capacityProfileIds)).WithMessage(Messages.ERR_NOT_FOUND)
            .WithState(GetIdentifiableId);

        var costProdfileIds = IdsToArray(project.CostProfiles);
        RuleFor(x => x.CostProfileID)
            .NotEmpty()
            .NotNull()
            .Must(Contains(costProdfileIds)).WithMessage(Messages.ERR_NOT_FOUND)
            .WithState(GetIdentifiableId);

        RuleFor(x => x.MaxWorkTime)
            .NotNull()
            .GreaterThan(0)
            .WithState(GetIdentifiableId);

        RuleFor(x => x.EarliestStart)
            .NotNull()
            .GreaterThanOrEqualTo(0)
            .WithState(GetIdentifiableId);

        RuleFor(x => x.LatestStart)
            .NotNull()
            .GreaterThan(x => x.EarliestStart)
            .LessThan(project.MaxTime)
            .WithState(GetIdentifiableId);

        RuleFor(x => x.ETollCat)
            .NotEmpty()
            .Must(x => _eTollCategories.Contains(x)).WithMessage(Messages.ERR_INVALID)
            .WithState(GetIdentifiableId);

        RuleFor(x => x.EnvironmentalClass)
            .NotEmpty()
            .Must(x => _environmentalClasses.Contains(x)).WithMessage(Messages.ERR_INVALID)
            .WithState(GetIdentifiableId);
    }

}
