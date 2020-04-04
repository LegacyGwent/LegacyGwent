using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;
using System;

namespace Cynthia.Card
{
    [CardEffectId("70003")]//苏克鲁斯
    public class Sukrus : CardEffect
    {//部署：选择手牌中的一张同名牌，丢弃所有牌组中该牌的同名牌。
        public Sukrus(GameCard card) : base(card)
        {
        }
    }
}