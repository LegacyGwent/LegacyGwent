using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("31004")]//篡位者
    public class Usurper : CardEffect
    {//间谍。生成对方阵营的1张非间谍领袖牌，使其获得1点增益。
        public Usurper(GameCard card) : base(card) { }
        public override async Task<int> CardPlayEffect(bool isSpying, bool isReveal)
        {
            var opponent = AnotherPlayer;
            var opponentFaction = Game.PlayerBaseDeck[opponent].Leader.Faction;
            var createCards = GwentMap.GetGenerateCardsId(
                x => x.Group == Group.Leader &&
                    x.Faction == opponentFaction &&
                    !x.HasAnyCategorie(Categorie.Agent));
            var count = await Game.CreateAndMoveStay(
                opponent,
                createCards.ToArray(),
                1);
            if (count == 0) return 0;
            await Game.RowToList(opponent, RowPosition.MyStay).First().Effect.Boost(1, Card);
            return 1;
        }
    }
}
