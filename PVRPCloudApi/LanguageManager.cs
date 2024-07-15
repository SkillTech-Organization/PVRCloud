using System.Globalization;
using PVRPCloud;

namespace PVRPCloudApi;

public sealed class LanguageManager : FluentValidation.Resources.LanguageManager
{
    public LanguageManager()
    {
        Culture = new CultureInfo("hu");

        AddTranslation("GreaterThanValidator", PVRPCloudMessages.ERR_GREATHER_THAN);
        AddTranslation("GreaterThanOrEqualValidator", PVRPCloudMessages.ERR_GREATER_THAN_OR_EQUAL);
        AddTranslation("LessThanValidator", PVRPCloudMessages.ERR_LESS_THAN);
        AddTranslation("LessThanOrEqualValidator", PVRPCloudMessages.ERR_LESS_THAN_OR_EQUAL);
        AddTranslation("InclusiveBetweenValidator", PVRPCloudMessages.ERR_RANGE);
        AddTranslation("NotEmptyValidator", PVRPCloudMessages.ERR_EMPTY);
        AddTranslation("NotNullValidator", PVRPCloudMessages.ERR_MANDATORY);
    }

    public void AddTranslation(string key, string message)
    {
        base.AddTranslation("hu", key, message);
    }
}
