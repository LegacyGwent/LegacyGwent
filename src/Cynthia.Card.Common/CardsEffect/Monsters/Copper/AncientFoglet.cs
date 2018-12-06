using System.Linq;
using System.Threading.Tasks;
using Alsein.Utilities;

namespace Cynthia.Card
{
	[CardEffectId("24003")]//远古小雾妖
	public class AncientFoglet : CardEffect
	{//回合结束时，若场上任意位置有“蔽日浓雾”，则获得1点增益。
		public AncientFoglet(IGwentServerGame game, GameCard card) : base(game, card){}
		public override async Task RoundEnd()
		{
			var hasFog = false;
			for (int playerIndex = 0; playerIndex < 2; playerIndex++)
			{
				for (int rowIndex = 0; rowIndex < 3; rowIndex++)
				{
					if (Game.GameRowStatus[playerIndex][rowIndex] == RowStatus.BitingFrost)
					{
						hasFog = true;
					}
				}
			}
			if (hasFog)
			{
				await Boost(1);
			}
		}
	}
}