using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("63020")]//华美的长剑
    public class OrnamentalSword : CardEffect
    {//生成1个己方起始牌组之外的铜色史凯利格“士兵”单位，并使其获得2点强化。
        public OrnamentalSword(GameCard card) : base(card) { }
        public override async Task<int> CardUseEffect()
        {	//生成
            var count = await Card.CreateAndMoveStay(
                GwentMap.GetGenerateCardsId(
                    x => x.Faction == Faction.Skellige &&
                        x.Is(Group.Copper, CardType.Unit) &&
                        x.HasAllCategorie(Categorie.Soldier),
                    Card.GetMyBaseDeck().Select(x => x.CardId)).ToList());
            if (count == 0) return 0;
            //强化玩家悬牌
            await Game.PlayersStay[PlayerIndex][0].Effect.Strengthen(2, Card);
            return 1;
        }
    }
}
