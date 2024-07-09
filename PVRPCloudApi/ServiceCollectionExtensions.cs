using FluentValidation;
using FluentValidation.AspNetCore;
using PVRPCloud.Requests;
using PVRPCloudApi.Validators;

namespace PVRPCloudApi;

public static class ServiceCollectionExtensions
{
    public static void AddValidation(this IServiceCollection services)
    {
        ValidatorOptions.Global.DisplayNameResolver = (type, member, expression) =>
        {
            if (member is null)
                return "?";

            if (member.Name is not null)
                return member.Name;
            else
                return "?";
        };

        services.AddFluentValidationAutoValidation();

        services.AddScoped<IValidator<PVRPCloudProject>, ProjectValidator>();
    }
}
