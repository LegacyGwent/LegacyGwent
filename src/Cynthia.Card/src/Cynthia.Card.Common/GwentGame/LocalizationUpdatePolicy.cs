using System;

namespace Cynthia.Card
{
    public static class LocalizationUpdatePolicy
    {
        public static bool ShouldDownloadLocales(
            bool areFilesDownloaded,
            Version localesWereLastUpdatedTo,
            Version serverVersion)
        {
            if (serverVersion == null)
            {
                throw new ArgumentNullException(nameof(serverVersion));
            }

            return !areFilesDownloaded || !serverVersion.Equals(localesWereLastUpdatedTo);
        }
    }
}
