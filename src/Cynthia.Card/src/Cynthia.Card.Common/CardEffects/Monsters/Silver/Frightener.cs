using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("23001")]//畏惧者
    public class Frightener : CardEffect
    {//间谍、力竭。 将1个敌军单位移至自身所在排，然后抽1张牌。
        public Frightener(GameCard card) : base(card) { }
        public bool IsUse = false;
        public override async Task<int> CardPlayEffect(bool isSpying, bool isReveal)
        {
            if (IsUse)
            {
                return 0;
            }

            IsUse = true;
            await Card.Effect.SetCountdown(offset: -1);

            var card = await Game.GetSelectPlaceCards(Card, isEnemySwitch: true, filter: (x => x.Status.CardRow != Card.Status.CardRow), selectMode: SelectModeType.MyRow);
            if (card.Count() == 0)
            {
                await Game.PlayerDrawCard(AnotherPlayer);//抽卡
                return 0;
            }

            var target = card.Single();
            var row = Card.Status.CardRow;
            var population = Game.RowToList(target.PlayerIndex, row).Count();

            //满了的话
            await target.Effect.Move(new CardLocation() { RowPosition = row, CardIndex = population }, Card);

            await Game.PlayerDrawCard(AnotherPlayer);//抽卡

            return 0;
        }
    }
}