using System.Collections.Generic;
using Cynthia.Card.Common.Models.Localization;

namespace Assets.Script.ResourceManagement
{
    interface ILocalizationResourceHandler
    {
        IList<ConfigEntry> LoadConfiguration();
        GameLocale LoadResource(string filename);
    }
}
