using FluentValidation;
using FluentValidation.Results;
using PVRPCloud;
using PVRPCloud.Requests;

namespace PVRPCloudApi.Validators;

public sealed class ProjectValidator : AbstractValidator<PVRPCloudProject>
{
    public ProjectValidator()
    {
        RuleFor(x => x.ProjectName)
            .NotEmpty()
            .NotNull()
            .WithState(GetProjectName);

        RuleFor(x => x.MinTime)
            .NotNull()
            .WithState(GetProjectName);

        RuleFor(x => x.MaxTime)
            .NotNull()
            .GreaterThan(x => x.MinTime)
            .WithState(GetProjectName);

        RuleFor(x => x.MaxTourDuration)
            .GreaterThan(0)
            .WithState(GetProjectName);

        RuleFor(x => x.DistanceLimit)
            .GreaterThanOrEqualTo(0)
            .WithState(GetProjectName);

        RuleFor(x => x.CostProfiles)
            .NotEmpty()
            .WithState(GetProjectName);

        RuleFor(x => x.CapacityProfiles)
            .NotEmpty().WithMessage(Messages.ERR_EMPTY)
            .WithState(GetProjectName);

        RuleFor(x => x.TruckTypes)
            .NotEmpty().WithMessage(Messages.ERR_EMPTY)
            .WithState(GetProjectName);

        RuleFor(x => x.Trucks)
            .NotEmpty()
            .WithState(GetProjectName);

        RuleFor(x => x.Depot)
            .NotNull()
            .WithState(GetProjectName);

        RuleFor(x => x.Clients)
            .NotEmpty()
            .WithState(GetProjectName);

        RuleFor(x => x.Orders)
            .NotEmpty()
            .WithState(GetProjectName);

        var createTruckTypeValidator = CreateValidator(project => new TruckTypeValidator(project));
        RuleForEach(x => x.TruckTypes).SetValidator(createTruckTypeValidator);

        var createCapacityProfileValidator = CreateValidator(project => new CapacityProfileValidator(project));
        RuleForEach(x => x.CapacityProfiles).SetValidator(createCapacityProfileValidator);

        var createClientValidator = CreateValidator(project => new ClientValidator(project));
        RuleForEach(x => x.Clients).SetValidator(createClientValidator);

        var createCostProfileValidator = CreateValidator(project => new CostProfileValidator(project));
        RuleForEach(x => x.CostProfiles).SetValidator(createCostProfileValidator);

        var createTruckValidator = CreateValidator(project => new TruckValidator(project));
        RuleForEach(x => x.Trucks).SetValidator(createTruckValidator);

        var createDepotValidator = CreateValidator(project => new DepotValidator(project));
        RuleFor(x => x.Depot).SetValidator(createDepotValidator);

        var createOrderValidator = CreateValidator(project => new OrderValidator(project));
        RuleForEach(x => x.Orders).SetValidator(createOrderValidator);
    }

    public string GetProjectName(PVRPCloudProject project) => project.ProjectName;

    private Func<PVRPCloudProject, TValidator> CreateValidator<TValidator>(Func<PVRPCloudProject, TValidator> validatorProvider)
        where TValidator : class
    {
        TValidator? validator = null;
        return project => validator ??= validatorProvider(project);
    }

    public override ValidationResult Validate(ValidationContext<PVRPCloudProject> context)
    {
        ArgumentNullException.ThrowIfNull(context);

        ValidationResult result = base.Validate(context);

        if (!result.IsValid)
        {
            throw new ValidationException("", result.Errors);
        }

        return result;
    }

    public override async Task<ValidationResult> ValidateAsync(ValidationContext<PVRPCloudProject> context, CancellationToken cancellation = default)
    {
        ArgumentNullException.ThrowIfNull(context);

        ValidationResult result = await base.ValidateAsync(context, cancellation);

        if (!result.IsValid)
        {
            throw new ValidationException("", result.Errors);
        }

        return result;
    }
}
