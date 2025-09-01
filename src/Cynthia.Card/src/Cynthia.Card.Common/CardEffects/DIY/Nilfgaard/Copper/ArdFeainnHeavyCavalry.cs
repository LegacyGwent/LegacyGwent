using System.Threading.Tasks;

namespace Cynthia.Card
{
    [CardEffectId("202453")]//日轮之师重骑兵 ArdFeainnHeavyCavalry
    public class ArdFeainnHeavyCavalry : CardEffect, IHandlesEvent<AfterUnitDown>, IHandlesEvent<AfterRoundOver>
    {//Boost self by 4 when a leader is played. Decrease the boost by 1 for each time this card has been boosted by its own ability previously this round - 2 armor.
        private int boostValue = 4;

        public override async Task<int> CardPlayEffect(bool isSpying, bool isReveal)
        {
            await Card.Effect.Armor(2, Card);
            return 0;
        }

        public ArdFeainnHeavyCavalry(GameCard card) : base(card) { }
        public async Task HandleEvent(AfterUnitDown @event)
        {
            if (@event.Target.Status.Group == Group.Leader && boostValue > 0)
            {
                await Card.Effect.Boost(boostValue, Card);
                boostValue -= 1;
            }
        }
        public async Task HandleEvent(AfterRoundOver @event)
        {
            boostValue = 4;
            await Task.CompletedTask;
        }
    }
}
