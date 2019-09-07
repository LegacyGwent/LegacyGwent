using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
	[CardEffectId("24018")]//赛尔伊诺鹰身女妖
	public class CelaenoHarpy : CardEffect
	{//在左侧生成2枚“鹰身女妖蛋”。
		public CelaenoHarpy(GameCard card) : base(card){}
		public override async Task<int> CardPlayEffect(bool isSpying,bool isReveal)
		{
            for(var i = 0; i<2;i++)
            {
                await Game.CreateCard(CardId.HarpyEgg, PlayerIndex, Card.GetLocation());
            }
			return 0;
		}
	}
}