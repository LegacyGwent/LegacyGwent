using System;
using System.Collections.Generic;
using System.Text;

namespace Cynthia.Card.Common.Models
{
    public interface ITranslator
    {
        int GameLanguage { get; set; }
        string GetText(string id);
    }
}
