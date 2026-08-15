using Alsein.Extensions.LifetimeAnnotations;

using Cynthia.Card;
using Cynthia.Card.Common.Models.Localization;

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

        public void RegisterRuleCards(GameFeatureManifest manifest)
        {
            if (manifest?.RuleCards == null) return;
            var language = TextLocalization.ChosenLanguage?.Filename ?? "cn";
            foreach (var rule in manifest.RuleCards)
            {
                if (string.IsNullOrWhiteSpace(rule.Id)) continue;
                TextLocalization.CardTexts[rule.Id] = new CardLocale
                {
                    Name = rule.Name.Resolve(language),
                    Info = rule.Description.Resolve(language),
                    Flavor = language.StartsWith("en")
                        ? "Local rule-system acceptance fixture."
                        : "本地规则系统验收卡。"
                };
            }
        }

    }
}
