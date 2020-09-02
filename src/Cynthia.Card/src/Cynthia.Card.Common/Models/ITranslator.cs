using System;
using System.Collections.Generic;
using System.Text;

namespace Cynthia.Card.Common.Models
{
    public interface ITranslator
    {
        int GameLanguage { get; set; }
        List<string> LanguageNames { get; }
        List<string> LanguageFilenames { get; }
        string GetText(string id);
    }
}
