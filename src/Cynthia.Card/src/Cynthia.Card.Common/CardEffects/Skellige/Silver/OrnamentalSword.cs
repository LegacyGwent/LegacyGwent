using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("63020")]//华美的长剑
    public class OrnamentalSword : CardEffect
    {//创造1个铜色/银色史凯利格“士兵”单位，并使其获得3点强化。
        public OrnamentalSword(GameCard card) : base(card) { }
        public override async Task<int> CardUseEffect()
        {	//创造
            await Card.CreateAndMoveStay(GwentMap.GetCreateCardsId(x => x.Faction == Faction.Skellige && (x.Group == Group.Copper || x.Group == Group.Silver) && x.CardInfo().CardType == CardType.Unit && x.HasAllCategorie(Categorie.Soldier), RNG));
            //强化玩家悬牌
            await Game.PlayersStay[PlayerIndex][0].Effect.Strengthen(3, Card);
            return 0;
        }
    }
}