using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
	[CardEffectId("52007")]//伊欧菲斯
	public class Iorveth : CardEffect
	{//对1个敌军单位造成8点伤害。若目标被摧毁，则使手牌中所有“精灵”单位获得1点增益。
		public Iorveth(GameCard card) : base(card){}
		public override async Task<int> CardPlayEffect(bool isSpying,bool isReveal)
		{
			
			var selectList = await Game.GetSelectPlaceCards(Card, selectMode: SelectModeType.EnemyRow);
            if (!selectList.TrySingle(out var target))
            {
                return 0;
            }
			
            await target.Effect.Damage(8, Card);

			// 如果目标被杀
			var isBoost = target.IsDead;
            if (isBoost)
            {
		var cards = Game.PlayersHandCard[PlayerIndex].Where(x => x.CardInfo().CardUseInfo == CardUseInfo.MyRow).FilterCards(filter: x => x.HasAllCategorie(Categorie.Elf));

				foreach (var card in cards)
				{
					await card.Effect.Boost(1, Card);
				}
				return 0;
            }
            return 0;
		}
	}
}
