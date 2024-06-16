using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace FTLApi.Attributes
{
    [AttributeUsage(validOn: AttributeTargets.Method)]
    public class ApiKeyAttribute : Attribute, IAsyncActionFilter
    {
        private const string HEADER_KEY = "X-API-KEY";
        private const string APPSETTINGS_KEY = "ApiKey";

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            if (!context.HttpContext.Request.Headers.TryGetValue(HEADER_KEY, out var apiKeyValue))
            {
                context.Result = new ContentResult()
                {
                    StatusCode = 401,
                    Content = "Missing API Key"
                };
                return;
            }

            var appSettings = context.HttpContext.RequestServices.GetRequiredService<IConfiguration>();
            var apiKey = appSettings.GetValue<string>(APPSETTINGS_KEY);

            if (!apiKey.Equals(apiKeyValue))
            {
                context.Result = new ContentResult()
                {
                    StatusCode = 401,
                    Content = "Invalid API Key"
                };
                return;
            }

            await next();
        }
    }
}
