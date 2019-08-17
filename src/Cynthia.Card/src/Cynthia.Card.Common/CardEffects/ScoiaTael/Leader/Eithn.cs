using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
	[CardEffectId("51002")]//艾思娜
	public class Eithn : CardEffect
	{//复活1张铜色/银色“特殊”牌。
		public Eithn(GameCard card) : base(card){}
		public override async Task<int> CardPlayEffect(bool isSpying,bool isReveal)
		{
			//从我方墓地列出铜色/银色“特殊”牌
            var list = Game.PlayersCemetery[PlayerIndex].Where(x => ((x.Status.Group == Group.Copper)||(x.Status.Group == Group.Silver)) && x.CardInfo().CardType == CardType.Special ).Mess();
            if (list.Count() == 0)
            {
                return 0;
            }

            //让玩家选择一张卡
            var result = await Game.GetSelectMenuCards
            (Card.PlayerIndex, list.ToList(), 1, "选择复活一张牌");
            //如果玩家一张卡都没选择,没有效果
            if (result.Count() == 0)
            {
                return 0;
            }
            //打出特殊牌
			
            var card = result.Single();
            // card.Status.IsDoomed = true;
            await card.Effect
                .Resurrect(new CardLocation() { RowPosition = RowPosition.MyStay, CardIndex = 0 }, Card);
            return 1;
		}
	}
}