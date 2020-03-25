using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("70003")]//哈蒙德
    public class Hammond : CardEffect
    {//己方半场同排单位免疫来自灾厄的伤害。择一：生成一张史凯利格铜色机械单位；使战场上所有友方机械获得 3 点强化。
        public Hammond(GameCard card) : base(card) { }
        public override async Task<int> CardPlayEffect(bool isSpying, bool isReveal)
        {
            var cardsId = GwentMap.GetCards().FilterCards(Group.Copper, CardType.Unit,
                x => x.HasAllCategorie(Categorie.Machine), Faction.Skellige)
                .Select(x => x.CardId);
            return await Game.CreateAndMoveStay(PlayerIndex, cardsId.ToArray());
        }

        public async Task HandleEvent(AfterTurnStart @event)
        {
            
        }
    }
}