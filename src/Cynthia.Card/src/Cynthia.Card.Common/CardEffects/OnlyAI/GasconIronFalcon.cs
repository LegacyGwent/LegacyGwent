using System;
using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;
using Alsein.Extensions.Extensions;
using System.Collections.Generic;
using Cynthia.Card.Common.CardEffects.Neutral.Derive;

namespace Cynthia.Card
{
    [CardEffectId("89004")]//斯科：铁隼之首 GasconIronFalcon
    public class GasconIronFalcon : CardEffect, IHandlesEvent<AfterTurnStart>,IHandlesEvent<AfterPlayerDraw>,IHandlesEvent<BeforeCardToCemetery>
    {//无法被魅惑与锁定，根据公正女神点数改变效果（0-9牌组增益、10-19随机天气、20-29摧毁最强单位），保持手牌数接近，牌组不为空。
        /*
        1.允许效果不生效,保留不被魅惑与锁定。
        2.若手牌数少于对方2张，抽牌直至双方手牌数相同。
        3.抽牌时若牌组没有牌，则将9张随机铁隼牌加入牌组。
        4.选点0-9，加斯科会使全卡组获得等额增益。
        5.选点10-19，加斯科会使全卡组获得个位数等额增益，并在回合开始时在对方全排降下随机灾厄。
        6.选点20-29，加斯科会使全卡组获得个位数等额增益，并在每两回合开始时摧毁一个对方最强的单位。
        */
        private int LCount = 0;
        private int LCountBoost = 0;
        private int DCount = 0;
        
        public GasconIronFalcon(GameCard card) : base(card) { }
        public async Task HandleEvent(BeforeCardToCemetery @event)
        {
            var cards = Game.PlayersHandCard[PlayerIndex].Where(x => x.CardInfo().CardUseInfo == CardUseInfo.MyRow).Concat(Game.PlayersDeck[PlayerIndex].Where(x => x.CardInfo().CardUseInfo == CardUseInfo.MyRow)).FilterCards(filter: x => x != Card).ToList();
            if(@event.Target.Status.CardId != CardId.LandOfAThousandFables)
            {
                return;
            }

            if(LCount == 0)
            {
                LCount = @event.Target.CardPoint();
                LCountBoost = LCount % 10;
                if (cards.Count() == 0)
                {
                    return;
                }
                if(LCount > 0)
                {
                    foreach (var card in cards)
                    {
                        await card.Effect.Boost(LCountBoost, Card);
                    }
                }
                
                return;
            }
            
            return;
        }


        public async Task HandleEvent(AfterTurnStart @event)
        {
            if (@event.PlayerIndex != Card.PlayerIndex)
            {
                return;
            }

            DCount = 0;
            if (Game.IsPlayersPass[Game.AnotherPlayer(Card.PlayerIndex)])
            {
                DCount = 1;
            }

            var HandCard1 = Game.PlayersHandCard[PlayerIndex].Count();
            var HandCard2 = Game.PlayersHandCard[AnotherPlayer].Count();
            
            if(HandCard2 - HandCard1 > 1 && DCount == 0)
            {
                for (var i = 0; i < HandCard2 - HandCard1; i++)
                {   
                    await Game.PlayerDrawCard(PlayerIndex);
                }
            }

            if(LCount > 9 && LCount < 20)
            {
                var list = Enum.GetValues(typeof(RowStatus));
                var realList = new List<RowStatus>();
                foreach (RowStatus it in list)
                {
                    if (it.IsHazard())
                    {
                        realList.Add(it);
                    }
                }

                var allenemylist = new List<RowPosition>()
                    {RowPosition.EnemyRow1, RowPosition.EnemyRow2, RowPosition.EnemyRow3};
                foreach (var row in allenemylist)
                {
                    var hazard = realList.Mess(RNG).First();
                    await Game.GameRowEffect[AnotherPlayer][row.Mirror().MyRowToIndex()].SetStatus(hazard);
                }
                return;
            }
            if(LCount > 19 && LCount < 30)
            {
                var tagetRows = Game.GameRowEffect[Card.PlayerIndex].Indexed()
                .Where(x => x.Value.RowStatus.IsHazard())
                .Select(x => x.Key);

                foreach (var rowIndex in tagetRows)
                {
                    await Game.GameRowEffect[PlayerIndex][rowIndex].SetStatus<NoneStatus>();
                }

                var cards = Game.GetAllCard(Card.PlayerIndex).Where(x => x.Status.CardRow.IsOnPlace() && x.PlayerIndex != Card.PlayerIndex).WhereAllHighest().Mess(RNG).ToList();
                if (cards.Count() != 0)
                {
                    if (Game.PlayersHandCard[PlayerIndex].Count() % 2 == 1 )
                    {
                        await cards.First().Effect.ToCemetery(CardBreakEffectType.Scorch);
                    }

                }
                return;
            }
            
        }

        public async Task HandleEvent(AfterPlayerDraw @event)
        {
            if (@event.PlayerIndex != Card.PlayerIndex)
            {
                return;
            }

            var DeckCard = Game.PlayersDeck[Card.PlayerIndex].Count();
            var cards = Game.PlayersDeck[PlayerIndex].Where(x => x.CardInfo().CardUseInfo == CardUseInfo.MyRow).FilterCards(filter: x => x != Card).ToList();
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
                    await card.Effect.Boost(LCountBoost, Card);
                }
                return;
                
            }

        }


        public override async Task Charm(GameCard source)
        {
            // 无法被魅惑
            await Task.CompletedTask;
            return;
        }

        public override async Task Lock(GameCard source)
        {
            // 无法被锁定
            await Task.CompletedTask;
            return;
        }
        public override async Task Banish()
        {
            // 无法被放逐
            await Task.CompletedTask;
            return;
        }

    }
}
