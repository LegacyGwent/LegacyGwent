using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("70183")]//猩红盛宴 FeastOfBlood
    public class FeastOfBlood : CardEffect
    {//"Choose a Vampire ally, make it drain an ennemy by 4, if it survives, spawn a plumard on its right"
        public FeastOfBlood(GameCard card) : base(card) { }
        public override async Task<int> CardUseEffect()
        {   // Choose a Vampire ally, make it drain an ennemy by 4
            var cards = await Game.GetSelectPlaceCards
            (Card, filter: x => x.PlayerIndex == PlayerIndex &&
                x.HasAllCategorie(Categorie.Vampire));
    
            if (!cards.TrySingle(out var friend))
            {
                return 0;
            }

            var list = await Game.GetSelectPlaceCards(Card, selectMode: SelectModeType.EnemyRow);
            if (!list.TrySingle(out var enemy))
            {
                return 0;
            }

            //对决，前一个先受到伤害
            await friend.Effect.Drain(4, enemy);
            if (!enemy.IsDead) 
            {
                // if it survives, spawn a plumard on its right
                //如果敌人死亡则不触发
                await Game.CreateCard(CardId.Plumard, PlayerIndex, enemy.GetLocation() + 1);
            }
            return 0;
        }
    }
}