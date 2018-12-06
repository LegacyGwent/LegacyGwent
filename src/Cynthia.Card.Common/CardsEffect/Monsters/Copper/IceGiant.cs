using System.Linq;
using System.Threading.Tasks;
using Alsein.Utilities;

namespace Cynthia.Card
{
	[CardEffectId("24021")]//寒冰巨人
	public class IceGiant : CardEffect
	{//若场上任意位置有“刺骨冰霜”，则获得6点增益。
		public IceGiant(IGwentServerGame game, GameCard card) : base(game, card){}
		public override async Task<int> CardPlayEffect(bool isSpying)
		{
			bool hasFrost = false;
			for (int playerIndex = 0; playerIndex < 2; playerIndex++)
			{
				for (int rowIndex = 0; rowIndex < 3; rowIndex++)
				{
					if (Game.GameRowStatus[playerIndex][rowIndex] == RowStatus.BitingFrost)
					{
						hasFrost = true;
					}
				}
			}
			if (hasFrost)
			{
				await Boost(10);
				return 1;
			}
			return 0;
		}
	}
}