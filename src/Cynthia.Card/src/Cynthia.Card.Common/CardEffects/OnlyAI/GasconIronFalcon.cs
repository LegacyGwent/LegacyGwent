using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;
using Alsein.Extensions.Extensions;
using System.Collections.Generic;

namespace Cynthia.Card
{
    [CardEffectId("89004")]//斯科：铁隼之首 GasconIronFalcon
    public class GasconIronFalcon : CardEffect, IHandlesEvent<AfterTurnStart>, IHandlesEvent<AfterCardToCemetery>,IHandlesEvent<AfterPlayerDraw>,IHandlesEvent<BeforeCardToCemetery>
    {//无法受到任何影响，若手牌数少于对方2张，抽牌直至双方手牌数相同。抽牌时若牌组没有牌，则将9张随机铁隼牌加入牌组，并使牌组中的牌获得3点增益。
        private int Lcount = 0;
        
        public GasconIronFalcon(GameCard card) : base(card) { }
        public override async Task<int> CardPlayEffect(bool isSpying, bool isReveal)
        {
            Card.Status.IsImmue = true;
            await Task.CompletedTask;
            return 0;
        }

        public async Task HandleEvent(BeforeCardToCemetery @event)
        {
            var cards = Game.PlayersHandCard[PlayerIndex].Where(x => x.CardInfo().CardUseInfo == CardUseInfo.MyRow).Concat(Game.PlayersDeck[PlayerIndex].Where(x => x.CardInfo().CardUseInfo == CardUseInfo.MyRow)).FilterCards(filter: x => x != Card).ToList();
            if(@event.Target.Status.CardId != CardId.LandOfAThousandFables)
            {
                return;
            }

            if(Lcount == 0)
            {
                Lcount = @event.Target.CardPoint();
                if (cards.Count() == 0)
                {
                    return;
                }
                foreach (var card in cards)
                {
                    await card.Effect.Boost(Lcount, Card);
                }
                return;
            }
            
            return;
        }

        public async Task HandleEvent(AfterCardToCemetery @event)
        {
            if (@event.Target != Card)
            {
                return;
            }

            if (@event.DeathLocation.RowPosition.IsOnPlace())
            {
                await Card.Effect.Resurrect(@event.DeathLocation, Card);
                Card.Status.IsImmue = true;

            }
            await Task.CompletedTask;
        }

        public async Task HandleEvent(AfterTurnStart @event)
        {
            if (@event.PlayerIndex != Card.PlayerIndex || !Card.Status.CardRow.IsOnPlace())
            {
                return;
            }

            var HandCard1 = Game.PlayersHandCard[PlayerIndex].Count();
            var HandCard2 = Game.PlayersHandCard[AnotherPlayer].Count();
            
            if(HandCard2 - HandCard1 > 1)
            {
                for (var i = 0; i < HandCard2 - HandCard1; i++)
                {   
                    await Game.PlayerDrawCard(PlayerIndex);
                }
            }
            

        }

        public async Task HandleEvent(AfterPlayerDraw @event)
        {
            if (@event.PlayerIndex != Card.PlayerIndex || !Card.Status.CardRow.IsOnPlace())
            {
                return;
            }

            var DeckCard = Game.PlayersDeck[Card.PlayerIndex].Count();
            var cards = Game.PlayersDeck[PlayerIndex].Where(x => x.CardInfo().CardUseInfo == CardUseInfo.MyRow).FilterCards(filter: x => x != Card).ToList();
            var LcountBoost = Lcount + 3;
            if(DeckCard == 0)
            {
                for (var i = 0; i < 3; i++)
                {
                    await Game.CreateCardAtEnd(CardId.IronFalconInfantry, PlayerIndex, RowPosition.MyDeck);
                    await Game.CreateCardAtEnd(CardId.IronFalconTroubadour, PlayerIndex, RowPosition.MyDeck);
                    await Game.CreateCardAtEnd(CardId.IronFalconKnifeJuggler, PlayerIndex, RowPosition.MyDeck);
                }

                cards = Game.PlayersDeck[PlayerIndex].Where(x => x.CardInfo().CardUseInfo == CardUseInfo.MyRow).FilterCards(filter: x => x != Card).ToList();

                foreach (var card in cards)
                {
                    await card.Effect.Boost(LcountBoost, Card);
                }
                return;
                
            }

        }

        // 下面override一些方法，使得本卡无法被召唤、复活、强化、削弱、增益、伤害、魅惑、变形

        public override async Task Summon(CardLocation location, GameCard source)//召唤
        {
            // 无法被召唤
            await Task.CompletedTask;
            return;
        }

        public override async Task Strengthen(int num, GameCard source)
        {
            // 无法被强化
            await Task.CompletedTask;
            return;
        }

        public override async Task Weaken(int num, GameCard source)
        {
            // 无法被削弱
            await Task.CompletedTask;
            return;
        }

        public override async Task Boost(int num, GameCard source)
        {
            // 无法被增益
            await Task.CompletedTask;
            return;
        }

        public override async Task Damage(int num, GameCard source, BulletType showType = BulletType.Arrow, bool isPenetrate = false, DamageType damageType = DamageType.Unit)
        {
            // 无法被伤害
            await Task.CompletedTask;
            return;
        }

        public override async Task Charm(GameCard source)//被魅惑
        {
            // 无法被魅惑
            await Task.CompletedTask;
            return;
        }

        public override async Task Transform(string cardId, GameCard source, System.Action<GameCard> setting = null, bool isForce = false)//变为
        {
            // 无法被变形
            await Task.CompletedTask;
            return;
        }

    }
}
