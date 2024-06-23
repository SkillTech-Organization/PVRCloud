using FluentValidation;
using FluentValidation.AspNetCore;
using PVRPCloudApi.Validators;

namespace PVRPCloudApi;

public static class ServiceCollectionExtensions
{
    public static void AddValidation(this IServiceCollection services)
    {
        services.AddFluentValidationAutoValidation();

        services.AddValidatorsFromAssemblyContaining<ProjectValidator>();
    }
}
