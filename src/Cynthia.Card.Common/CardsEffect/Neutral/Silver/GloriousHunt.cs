using System.Linq;
using System.Threading.Tasks;
using Alsein.Utilities;

namespace Cynthia.Card
{
	[CardEffectId("13042")]//豪猎而归
	public class GloriousHunt : CardEffect
	{//如果落后，生成1只“帝国蝎尾狮”；如果领先，生成“蝎尾狮毒液”。
		public GloriousHunt(IGwentServerGame game, GameCard card) : base(game, card){}
		public override async Task<int> CardUseEffect()
		{
			if(Game.WhoHeight.PlayerIndex==PlayerIndex)
			{
				await Game.CreateCard(CardId.ManticoreVenom,PlayerIndex,new CardLocation(RowPosition.MyStay,0));
				return 1;
			}
			else if(Game.WhoHeight.PlayerIndex==AnotherPlayer)
			{
				await Game.CreateCard(CardId.ImperialManticore,PlayerIndex,new CardLocation(RowPosition.MyStay,0));
				return 1;
			}
			return 0;
		}
	}
}