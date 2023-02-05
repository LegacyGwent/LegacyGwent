using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("32007")]//希拉德
    public class Shilard : CardEffect
    {//力竭。休战：若双方牌组都有牌，从双方牌组各抽1张牌。保留1张，将另一张给予对方。
        public Shilard(GameCard card) : base(card) { }
        public bool IsUse { get; set; } = false;
        public override async Task<int> CardPlayEffect(bool isSpying,bool isReveal)
        {
            if (Game.IsPlayersPass[AnotherPlayer] || Game.PlayersDeck[PlayerIndex].Count == 0 || Game.PlayersDeck[AnotherPlayer].Count == 0)
                return 0;
            if (IsUse)
            {
                return 0;
            }

            IsUse = true;
            await Card.Effect.SetCountdown(offset: -1);
            var selectList = new List<GameCard>() { Game.PlayersDeck[PlayerIndex].First(), Game.PlayersDeck[AnotherPlayer].First() };
            var selectCard = (await Game.GetSelectMenuCards(PlayerIndex, selectList, isCanOver: false)).Single();
            var anotherCard = selectList.First(x => x != selectCard);

            //如果选择的卡牌是对方的卡,那么移动到对方排
            await selectCard.MoveToCardStayFirst(selectCard.PlayerIndex != PlayerIndex, isShowEnemyBack: true);
            //如果另一张卡不是对方的卡,那么移动到对方排
            await anotherCard.MoveToCardStayFirst(anotherCard.PlayerIndex == PlayerIndex, isShowEnemyBack: true);

            await Game.ClientDelay(700);

            await Game.ShowCardMove(new CardLocation(RowPosition.MyHand, 0), selectCard);
            await Game.SendEvent(new AfterPlayerDraw(selectCard.PlayerIndex, selectCard, Card));
            await Game.ShowCardMove(new CardLocation(RowPosition.MyHand, 0), anotherCard);
            await Game.SendEvent(new AfterPlayerDraw(anotherCard.PlayerIndex, anotherCard, Card));
            return 0;
        }
    }
}