﻿using FluentValidation;
using FluentValidation.AspNetCore;
using PVRPCloud.Models;
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

        ValidatorOptions.Global.LanguageManager = new LanguageManager();

        services.AddFluentValidationAutoValidation();

        services.AddScoped<IValidator<Project>, ProjectValidator>();
    }
}
