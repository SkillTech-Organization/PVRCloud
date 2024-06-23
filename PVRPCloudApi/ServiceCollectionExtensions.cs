using System.ComponentModel;
using System.Reflection;
using FluentValidation;
using FluentValidation.AspNetCore;
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

            var labelAttr = member.GetCustomAttribute<DescriptionAttribute>();
            if (labelAttr is not null)
                return member.GetCustomAttribute<DescriptionAttribute>()?.Description;
            else
                return "?";
        };

        services.AddFluentValidationAutoValidation();

        services.AddValidatorsFromAssemblyContaining<ProjectValidator>();
    }
}
