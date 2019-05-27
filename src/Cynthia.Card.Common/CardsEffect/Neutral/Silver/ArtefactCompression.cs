using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
	[CardEffectId("13025")]//化器封形
	public class ArtefactCompression : CardEffect
	{//将1个铜色/银色单位变为“翡翠人偶”。
		public ArtefactCompression(IGwentServerGame game, GameCard card) : base(game, card){}
		public override async Task<int> CardUseEffect()
		{
			var result = await Game.GetSelectPlaceCards(Card,Sizer:x=>x.Status.Group==Group.Copper||x.Status.Group==Group.Silver);
			if(result.Count<=0) return 0;
			await result.Single().Effect.Transform(CardId.JadeFigurine,Card);
			return 0;
		}
	}
}