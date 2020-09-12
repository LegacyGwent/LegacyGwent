using System;
using System.Collections.Generic;

namespace Assets.Script.Localization
{
    public interface ITranslator
    {
        event Action OnInit;
        void Initialize();
        int GameLanguage { get; set; }
        List<string> LanguageNames { get; }
        List<string> LanguageFilenames { get; }
        string GetText(string id);
        string GetCardName(string cardId);
        string GetCardInfo(string cardId);
        string GetCardFlavor(string cardId);
        LanguageFileHandler FileHandler { get; }
    }
}
