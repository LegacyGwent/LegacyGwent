using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
	[CardEffectId("43004")]//没完没了的朗维德
	public class RonvidTheIncessant : CardEffect
	{//回合结束时，复活至随机排，基础战力设为1点。 操控。
		public RonvidTheIncessant(GameCard card) : base(card){}
		public override async Task<int> CardPlayEffect(bool isSpying,bool isReveal)
		{
			return 0;
		}
	}
}