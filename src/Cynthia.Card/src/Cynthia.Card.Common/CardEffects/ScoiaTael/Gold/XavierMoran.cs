using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("52003")]//泽维尔·莫兰
    public class XavierMoran : CardEffect
    {//增益自身等同于最后打出的非同名“矮人”单位牌的初始战力。
        public XavierMoran(GameCard card) : base(card) { }
        public override async Task<int> CardPlayEffect(bool isSpying, bool isReveal)
        {

            var cards = Game.HistoryList
                .Where(x => (x.CardId.Status.CardId != Card.Status.CardId && x.CardId.CardInfo().Categories.Contains(Categorie.Dwarf)))
                            .ToList();
            if (cards.Count() <= 0) return 0;

            var lastDwarfCardId = cards.Last().CardId.Status.CardId;
            var boostNum = GwentMap.CardMap[lastDwarfCardId].Strength;
            await Card.Effect.Boost(boostNum, Card);
            return 0;


        }
    }
}