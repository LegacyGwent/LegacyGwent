using Assets.Script.ResourceManagement.Serializables;
using System.Collections.Generic;

namespace Assets.Script.ResourceManagement
{
    interface ILocalizationResourceHandler
    {
        IList<ConfigEntry> LoadConfiguration();
        GameLocale LoadResource(string filename);
    }
}
