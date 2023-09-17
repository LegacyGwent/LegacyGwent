using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;


namespace Cynthia.Card
{
    [CardEffectId("70095")]//莱里亚重弩手 LyrianArbalest
    public class LyrianArbalest : CardEffect
    {//对一个战力低于自身的单位造成2者战力差的伤害，对大于等于自身战力的单位不造成伤害

        public LyrianArbalest(GameCard card) : base(card) { }
        public override async Task<int> CardPlayEffect(bool isSpying, bool isReveal)
        {
            var list = await Game.GetSelectPlaceCards(Card, selectMode: SelectModeType.EnemyRow);
            foreach (var target in list)
            {
                if (list.Count <= 0) return 0;
                if (target.CardPoint() < Card.CardPoint())
                    {
                        await target.Effect.Damage(Card.CardPoint() - target.CardPoint(), Card, BulletType.RedLight, true);//造成穿透伤害
                    }
            }
            return 0;
        }
    }
}
