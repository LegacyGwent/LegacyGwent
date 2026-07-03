using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;
using System;

namespace Cynthia.Card
{
    [CardEffectId("70072")]//雷蒂娅 Radeyah
    public class Radeyah : CardEffect
    {//若每张铜色牌在己方初始牌组中刚好有3张，则生成起始牌组中一张铜色单位牌，并使其具有佚亡。
        public Radeyah(GameCard card) : base(card) { }
        public override async Task<int> CardPlayEffect(bool isSpying, bool isReveal)
        {
            if (Game.PlayerBaseDeck[PlayerIndex].Deck
                .Where(x => x.Group == Group.Copper)
                .GroupBy(x => x.CardId).Any(x => x.Count() != 3))
                return 0;
            var cardsId = Game.PlayerBaseDeck[PlayerIndex].Deck
               .Select(x => x.CardId)
               .Distinct()
               .Where(x => GwentMap.CardMap[x].Is(Group.Copper, CardType.Unit))
               .ToArray();
            return await Game.CreateAndMoveStay(PlayerIndex, cardsId);
        }
    }
}
