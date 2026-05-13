using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;
using Cynthia.Card.Common.CardEffects.Neutral.Derive;

namespace Cynthia.Card
{
    [CardEffectId("70188")]//艾勒的格哈特
    public class Bronibor : CardEffect, IHandlesEvent<AfterUnitDown>
    {//Spawn and play a Poor Fucking Infantry. Then, deal 1 damage to a random enemy for each soldier you control.
        public Bronibor(GameCard card) : base(card) { }

        private bool isused = false;

        public override async Task<int> CardPlayEffect(bool isSpying,bool isReveal)
        {
            await Game.CreateCard(CardId.PoorFIngInfantry, Card.PlayerIndex, new CardLocation(RowPosition.MyStay, 0));
            return 1;
        }
        public async Task HandleEvent(AfterUnitDown @event)
        {
            if (@event.Target.Status.CardId != CardId.PoorFIngInfantry || @event.Target.PlayerIndex != Card.PlayerIndex || isused || !Card.Status.CardRow.IsOnPlace())
            {
                return;
            }
            var soldierlist = Game.GetPlaceCards(Card.PlayerIndex).FilterCards(filter: x => x.HasAllCategorie(Categorie.Soldier)).ToList();
            int damage = soldierlist.Count();
            for (int i = 0; i < damage; i++)
            {
                var enemylist = Game.GetPlaceCards(AnotherPlayer).ToList();
                if (enemylist.Count() == 0)
                {
                    break;
                }
                await enemylist.Mess(Game.RNG).First().Effect.Damage(1, Card);
            }
            isused = true;
            return;
        }
    }
}