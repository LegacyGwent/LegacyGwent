using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("42013")]//菲丽芭·艾哈特
    public class PhilippaEilhart : CardEffect
    {//对敌军单位造成5、4、3、2、1点伤害。每次随机改变目标，无法对同一目标连续造成伤害。
        public PhilippaEilhart(GameCard card) : base(card) { }
        public override async Task<int> CardPlayEffect(bool isSpying, bool isReveal)
        {
            var selectList = await Game.GetSelectPlaceCards(Card, selectMode: SelectModeType.EnemyRow);
            if (!selectList.TrySingle(out var target))
            {
                return 0;
            }
            GameCard nexttarget = target;
            for (var i = 1; i < 6; i++)
            {
                await nexttarget.Effect.Damage(6 - i, Card);
                nexttarget = Game.GetAllCard(Card.PlayerIndex).Where(x => x.Status.CardRow.IsOnPlace() && x.PlayerIndex == AnotherPlayer && x != nexttarget).Mess(Game.RNG).FirstOrDefault();
                if (nexttarget == default)
                {
                    return 0;
                }
            }

            return 0;
        }
    }
}