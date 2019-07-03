using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
	[CardEffectId("23001")]//畏惧者
	public class Frightener : CardEffect
	{//间谍、力竭。 将1个敌军单位移至自身所在排，然后抽1张牌。
		public Frightener(GameCard card) : base(card){ }
        public bool IsUse = false;
		public override async Task<int> CardPlayEffect(bool isSpying,bool isReveal)
		{
            if (IsUse) return 0;
            IsUse = true;

            var card = await Game.GetSelectPlaceCards(Card, x => x.Status.CardRow != Card.Status.CardRow, SelectModeType.MyRow);
			if(card.Count<=0) 
            {
                return 0;
            }

            target = card.Single()
            row = Card.Status.CardRow
            population = Game.RowToList(target.PlayerIndex, row).Count

            //满了的话
            if (population >= 9) 
            {
                return 0;
            }

            await target.Effect.Move(new CardLocation() { RowPosition = row, CardIndex = population }, Card);

            await Game.PlayerDrawCard(Game.AnotherPlayer(Card.PlayerIndex));//抽卡
            
            return 0;
		}
	}
}