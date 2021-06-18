using Alsein.Extensions.LifetimeAnnotations;

namespace Assets.Script.Localization
{
    [Singleton]
    class LocalizationService
    {
        public TextLocalization TextLocalization = new TextLocalization();
        public AudioLocalization AudioLocalization = new AudioLocalization();

        public bool IsContainsKey(string id)
        {
            return TextLocalization.IsContainsKey(id);
        }
        public string GetText(string id)
        {
            return TextLocalization.GetText(id);
        }
        public string GetCardName(string cardId)
        {
            return TextLocalization.GetCardName(cardId);
        }
        public string GetCardInfo(string cardId)
        {
            return TextLocalization.GetCardInfo(cardId);
        }
        public string GetCardFlavor(string cardId)
        {
            return TextLocalization.GetCardFlavor(cardId);
        }

    }
}