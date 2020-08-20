using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("52003")]//泽维尔·莫兰
    public class XavierMoran : CardEffect, IHandlesEvent<AfterRoundOver>, IHandlesEvent<AfterUnitDown>
    {//增益自身等同于本小局打出的“矮人”单位牌的最强基础战力。
        public XavierMoran(GameCard card) : base(card) { }
        private int boostNum = 0;
        public override async Task<int> CardPlayEffect(bool isSpying, bool isReveal)
        {
            /*
            var cards = Game.HistoryList
                .Where(x => (x.CardId.Status.CardId != Card.Status.CardId && x.CardId.CardInfo().Categories.Contains(Categorie.Dwarf)))
                            .ToList();
            if (cards.Count() <= 0) return 0;

            var lastDwarfCardId = cards.Last().CardId.Status.CardId;
            boostNum = GwentMap.CardMap[lastDwarfCardId].Strength;
            */
            await Card.Effect.Boost(boostNum, Card);
            return 0;


        }
        public async Task HandleEvent(AfterUnitDown @event) //每下一个矮人打擂台
        {
            if (@event.Target == Card) return;
            if (PlayerIndex == @event.Target.PlayerIndex && @event.Target.HasAllCategorie(Categorie.Dwarf))
            {
                if (boostNum < @event.Target.Status.Strength)
                {
                    await Task.Run(() => {

                        boostNum = @event.Target.Status.Strength;

                    });//异步运行,其实感觉不用 
                }
            }
        }

        public async Task HandleEvent(AfterRoundOver @event) //每小局结束清零
        {
            await Task.Run(() => {

                boostNum = 0;

            });//异步运行,其实感觉不用
            
        }
    }
    
}