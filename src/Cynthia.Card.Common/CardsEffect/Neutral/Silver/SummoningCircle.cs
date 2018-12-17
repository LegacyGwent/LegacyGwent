using System.Linq;
using System.Threading.Tasks;
using Alsein.Utilities;

namespace Cynthia.Card
{
	[CardEffectId("13036")]//召唤法阵
	public class SummoningCircle : CardEffect
	{//生成1张上张被打出的铜色/银色非“密探”单位牌的原始同名牌。
		public SummoningCircle(IGwentServerGame game, GameCard card) : base(game, card){}
		public override async Task<int> CardUseEffect()
		{
			await Game.Debug("触发了召唤法阵的效果");
			await Game.Debug(Game.HistoryList.Select(x=>x.CardId.CardInfo().Name).Join(","));
			var cards = Game.HistoryList
				.Where(x=>((!x.CardId.CardInfo().Categories.Contains(Categorie.Agent))&&
							(x.CardId.CardInfo().Group==Group.Copper||x.CardId.CardInfo().Group==Group.Silver)&&
							(x.CardId.CardInfo().CardType == CardType.Unit)))
							.ToList();
			await Game.Debug(cards.Select(x=>x.CardId.CardInfo().Name).Join(","));
			if(cards.Count() <= 0) return 0;
			await Game.CreateCard(cards.Last().CardId,PlayerIndex,new CardLocation(RowPosition.MyStay,0));
			return 1;
		}
	}
}