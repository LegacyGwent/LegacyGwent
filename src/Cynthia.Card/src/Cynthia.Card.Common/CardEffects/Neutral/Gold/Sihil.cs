using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("12042")]//矮人符文剑
    public class Sihil : CardEffect
    {//择一：伤害奇数/偶数战力敌军，或从牌组选择打出1个铜色/银色单位。
        public Sihil(GameCard card) : base(card) { }
        public override async Task<int> CardUseEffect()
        {
            //均衡，分裂，装备 来源 https://www.bilibili.com/video/av17561142?from=search&seid=13434231812588843367 22:15
            var switchCard = await Card.GetMenuSwitch(("均衡", "Sihil_1_DamageOdd"), ("分裂", "Sihil_2_DamageEven"), ("装备", "Sihil_3_PlayUnit"));
            if (switchCard == 0)
            {

                var cards = Game.GetAllCard(Card.PlayerIndex).Where(x => x.Status.CardRow.IsOnPlace() && x.PlayerIndex != Card.PlayerIndex && x.CardPoint() % 2 == 1).ToList();
                if (cards.Count() == 0)
                {
                    return 0;
                }
                foreach (var card in cards)
                {

                    await card.Effect.Damage(3, Card);
                }
                return 0;
            }
            if (switchCard == 1)
            {
                var cards = Game.GetAllCard(Card.PlayerIndex).Where(x => x.Status.CardRow.IsOnPlace() && x.PlayerIndex != Card.PlayerIndex && x.CardPoint() % 2 == 0).ToList();
                if (cards.Count() == 0)
                {
                    return 0;
                }
                foreach (var card in cards)
                {

                    await card.Effect.Damage(3, Card);
                }
                return 0;
            }
            if (switchCard == 2)
            {

                //列出全部铜银单位
                var list = Game.PlayersDeck[Card.PlayerIndex].Where(x => (x.Status.Group == Group.Silver || x.Status.Group == Group.Copper) && (x.CardInfo().CardType == CardType.Unit)).ToList();
                if (list.Count() == 0)
                {
                    return 0;
                }
                //选一张
                var selected = await Game.GetSelectMenuCards(PlayerIndex, list, 1);
                if (!selected.TrySingle(out var target))
                {
                    return 0;
                }
                await target.MoveToCardStayFirst();
                return 1;
            }
            return 0;
        }
    }
}
