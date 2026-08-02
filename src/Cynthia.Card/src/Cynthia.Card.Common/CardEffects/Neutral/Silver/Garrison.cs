using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("13044")]//军营
    public class Garrison : CardEffect
    {//生成对方起始牌组中的1张非间谍铜色/银色单位牌，并使它获得1点增益。
        public Garrison(GameCard card) : base(card) { }
        public override async Task<int> CardUseEffect()
        {
            var cardsId = Game.PlayerBaseDeck[AnotherPlayer].Deck
                .Distinct()
                .Where(x => x.Is(type: CardType.Unit, filter: x => x.IsAnyGroup(Group.Copper, Group.Silver) && !x.HasAnyCategorie(Categorie.Agent)))
                .Select(x => x.CardId).ToArray();
            if (await Game.CreateAndMoveStay(PlayerIndex, cardsId) == 0)
            {
                return 0;
            }
            await Game.PlayersStay[PlayerIndex][0].Effect.Boost(1, Card);
            return 1;
        }
    }
}
