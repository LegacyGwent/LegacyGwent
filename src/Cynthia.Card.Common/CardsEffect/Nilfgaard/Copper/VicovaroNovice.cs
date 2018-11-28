using System.Linq;
using System.Threading.Tasks;

namespace Cynthia.Card
{
    [CardEffectId("34001")]//见习医师
    public class VicovaroNovice : CardEffect
    {
        public VicovaroNovice(IGwentServerGame game, GameCard card) : base(game, card) { }
        public override async Task<int> CardPlayEffect(bool isSpying)
        {
            //从敌方墓地取铜色单位卡
            var list = Game.PlayersCemetery[Game.AnotherPlayer(Card.PlayerIndex)]
            .Where(x => x.Status.Group == Group.Copper && x.CardInfo().CardType == CardType.Unit).Mess();
            //让玩家选择一张卡
            var result = await Game.GetSelectMenuCards
            (Card.PlayerIndex, list.ToList(), 1, "选择复活一张牌");
            //如果玩家一张卡都没选择,没有效果
            if (result.Count() == 0) return 0;
            await result.First().Effect.Resurrect(new CardLocation() { RowPosition = RowPosition.EnemyStay, CardIndex = 0 }, Card);
            return 1;
        }
    }
}