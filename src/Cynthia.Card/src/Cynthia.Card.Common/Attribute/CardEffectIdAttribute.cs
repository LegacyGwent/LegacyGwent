using System;

namespace Cynthia.Card
{
    public class CardEffectIdAttribute : Attribute
    {
        public string Id { get; }
        public CardEffectIdAttribute(string id)
        {
            Id = id;
        }
    }
}