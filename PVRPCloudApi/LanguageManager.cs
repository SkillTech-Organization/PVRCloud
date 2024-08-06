using System.Globalization;
using PVRPCloud;

namespace PVRPCloudApi;

public sealed class LanguageManager : FluentValidation.Resources.LanguageManager
{
    public LanguageManager()
    {
        Culture = new CultureInfo("hu");

        AddTranslation("GreaterThanValidator", Messages.ERR_GREATHER_THAN);
        AddTranslation("GreaterThanOrEqualValidator", Messages.ERR_GREATER_THAN_OR_EQUAL);
        AddTranslation("LessThanValidator", Messages.ERR_LESS_THAN);
        AddTranslation("LessThanOrEqualValidator", Messages.ERR_LESS_THAN_OR_EQUAL);
        AddTranslation("InclusiveBetweenValidator", Messages.ERR_RANGE);
        AddTranslation("NotEmptyValidator", Messages.ERR_EMPTY);
        AddTranslation("NotNullValidator", Messages.ERR_MANDATORY);
    }

    public void AddTranslation(string key, string message)
    {
        base.AddTranslation("hu", key, message);
    }
}
