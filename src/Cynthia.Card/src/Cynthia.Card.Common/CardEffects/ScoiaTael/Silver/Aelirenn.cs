using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
	[CardEffectId("53009")]//爱黎瑞恩
	public class Aelirenn : CardEffect
	{//场上有至少5个“精灵”友军单位时，在回合结束时召唤此单位。
		public Aelirenn(GameCard card) : base(card){}
	
        public async Task HandleEvent(AfterTurnStart @event)
        {
			// var list_jingling = Game.GetPlaceCards(Card.PlayerIndex).FilterCards(filter: x => x != Card && x.Status.HealthStatus < 0).ToList();
			var listElf = Game.GetPlaceCards(Card.PlayerIndex).FilterCards(filter: x => x.HasAllCategorie(Categorie.Elf)).ToList();
			int elfNum = listElf.Count();
            if (elfNum >= 5)
            {
                //召唤全部
                var cards = Game.PlayersDeck[PlayerIndex].Where(x => x.Status.CardId == Card.Status.CardId).ToList();
                foreach (var card in cards)
                {
                    //召唤到末尾
                    await card.Effect.Summon(Game.GetRandomCanPlayLocation(Card.PlayerIndex,true), Card);
                }
            }
            return;
		}

	}
}