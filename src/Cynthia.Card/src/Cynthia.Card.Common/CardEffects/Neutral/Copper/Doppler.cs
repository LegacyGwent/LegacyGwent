using System.Linq;
using System.Threading.Tasks;

namespace Cynthia.Card
{
    [CardEffectId("14022")]//变形怪
    public class Doppler : CardEffect
    {//生成等同于手牌数量的战力的起始牌组之外的己方阵营中的铜色单位。若无对应战力单位，则生成1个“农民”。
        public Doppler(GameCard card) : base(card) { }
        public override async Task<int> CardUseEffect()
        {
            var handCount = Game.PlayersHandCard[Card.PlayerIndex].Count;
            var candidates = GwentMap.GetGenerateCardsId(
                x => x.Faction == Game.PlayersFaction[Card.PlayerIndex] &&
                    x.Is(Group.Copper, CardType.Unit) &&
                    x.Strength == handCount,
                Card.GetMyBaseDeck().Select(x => x.CardId),
                isHasAgent: true).ToList();
            if (candidates.Count == 0)
            {
                candidates.Add("15011"); // 农民
            }
            return await Card.CreateAndMoveStay(candidates);
        }
    }
}
