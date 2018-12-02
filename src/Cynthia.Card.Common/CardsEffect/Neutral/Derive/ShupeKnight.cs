using System.Linq;
using System.Threading.Tasks;
using Alsein.Utilities;

namespace Cynthia.Card
{
	[CardEffectId("15001")]//店店：骑士
	public class ShupeKnight : CardEffect
	{//派“店店”去帝国宫廷军事学院。 强化自身至25点；坚韧；与1个敌军单位对决；重置1个单位；摧毁所有战力低于4点的敌军单位。
		public ShupeKnight(IGwentServerGame game, GameCard card) : base(game, card){}
		public override async Task<int> CardPlayEffect(bool isSpying)
		{
			return 0;
		}
	}
}