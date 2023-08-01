using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("70089")]//亚提斯
    public class Artis : CardEffect
    {//部署：对一个敌军单位造成7点伤害，若摧毁目标，则在对方同排生成一张“巨熊祭品”。
        public Artis(GameCard card) : base(card) { }
        public override async Task<int> CardPlayEffect(bool isSpying, bool isReveal)
        {
            var selectList = await Game.GetSelectPlaceCards(Card, filter: x => x.Status.Group == Group.Copper || x.Status.Group == Group.Silver || x.Status.Group == Group.Gold);

            if (!selectList.TrySingle(out var target))
            {
                return 0;
            }
            var damage = 7;
            await target.Effect.Damage(damage, Card);
            if (target.IsDead){
                await Game.CreateCard(CardId.CultistOblation, target.PlayerIndex, target.GetLocation());
            }
            return 0;
        }
    }
}