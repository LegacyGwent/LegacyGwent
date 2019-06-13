using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
	[CardEffectId("53016")]//保利·达尔伯格
	public class PaulieDahlberg : CardEffect
	{//复活一个铜色非“辅助”矮人单位。
		public PaulieDahlberg(IGwentServerGame game, GameCard card) : base(game, card){}
		public override async Task<int> CardPlayEffect(bool isSpying)
		{
			return 0;
		}
	}
}