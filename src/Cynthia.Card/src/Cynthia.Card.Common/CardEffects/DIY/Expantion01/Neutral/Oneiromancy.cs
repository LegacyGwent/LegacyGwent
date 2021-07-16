using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;
using System;
using System.Collections.Generic;

namespace Cynthia.Card
{
    [CardEffectId("70091")]//解梦术 Oneiromancy
    public class Oneiromancy : CardEffect
    {//择一：创造己方起始牌组非佚亡牌中的1张非“密探”牌；或将3张随机非同名手牌变为“解梦术”。

        public Oneiromancy(GameCard card) : base(card) { }

        public override async Task<int> CardUseEffect()
        {
            var switchCard = await Card.GetMenuSwitch(("解梦", "创造己方起始牌组中的1张非“密探”牌。"), ("入梦", "将3张随机非同名手牌变为“解梦术”。"));
            if (switchCard == 0)
            {
                var list = Game.PlayerBaseDeck[PlayerIndex].Deck;
                return await Card.CreateAndMoveStay(list.Where(x=>!x.HasAnyCategorie(Categorie.Agent) && !x.HasAnyCategorie(Categorie.Doomed)).Select(x => x.CardId).Mess(RNG).Take(3).ToArray());
            }
            if (switchCard == 1)
            {
                var cards = Game.PlayersHandCard[PlayerIndex].Where(x=>x.CardInfo().CardId!=Card.CardInfo().CardId).Mess(RNG).Take(3);
				foreach (var card in cards)
				{
                    await card.Effect.Transform(CardId.Oneiromancy, Card, isForce:true);
				}
            }
            return 0;
        }
    }
}
