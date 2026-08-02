using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("13023")]//黑血
    public class BlackBlood : CardEffect
    {//择一：生成1个己方起始牌组之外的铜色“食腐生物”或“吸血鬼”单位，并使其获得1点增益；或摧毁1个铜色/银色“食腐生物”或“吸血鬼”单位。
        public BlackBlood(GameCard card) : base(card) { }
        public override async Task<int> CardUseEffect()
        {
            //选择选项,设置每个选项的名字和效果
            var switchCard = await Card.GetMenuSwitch
            (
                ("挑拨", "BlackBlood_1_CreateVampire"),
                ("剧毒", "BlackBlood_2_DestroyVampire")
            );
            if (switchCard == 0)
            {
                var cards = GwentMap.GetGenerateCardsId(
                    x => x.Is(Group.Copper, CardType.Unit) &&
                        x.HasAnyCategorie(Categorie.Necrophage, Categorie.Vampire),
                    Card.GetMyBaseDeck().Select(x => x.CardId)).ToArray();
                if ((await Game.CreateAndMoveStay(PlayerIndex, cards, isCanOver: true)) == 1)
                {
                    await Game.PlayersStay[PlayerIndex].First().Effect.Boost(1, Card);
                    return 1;
                }
                return 0;
            }
            else if (switchCard == 1)
            {
                var target = await Game.GetSelectPlaceCards(Card, 1, false,
                x => x.HasAnyCategorie(Categorie.Necrophage, Categorie.Vampire) && x.IsAnyGroup(Group.Copper, Group.Silver));
                if (target.Count == 0) return 0;
                await target.Single().Effect.ToCemetery(CardBreakEffectType.Scorch);
            }
            return 0;
        }
    }
}
