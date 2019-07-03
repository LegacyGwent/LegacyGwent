using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
	[CardEffectId("33017")]//契拉克·迪弗林
	public class CeallachDyffryn : CardEffect
	{//生成1个“大使”、“刺客”或“特使”。
		public CeallachDyffryn(GameCard card) : base(card){}
		public override async Task<int> CardPlayEffect(bool isSpying,bool isReveal)
		{
			//大使,刺客,特使
			/*
			var selectList = new List<CardStatus>(){new CardStatus("34028"),new CardStatus("34029"),new CardStatus("34002")};
			var result = await Game.GetSelectMenuCards(Card.PlayerIndex,selectList,isCanOver:true,title:"选择生成一张卡");
			if(result.Count()<=0) return 0;
			await Game.CreatCard(selectList[result.Single()].CardId,Card.PlayerIndex,new CardLocation(RowPosition.MyStay,0));*/
			return await Card.CreateAndMoveStay(CardId.Ambassador,CardId.Emissary,CardId.Assassin);
		}
	}
}