using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("22012")]//织婆：咒文
    public class WeavessIncantation : CardEffect
    {//择一：使位于手牌、牌组和己方半场除自身外的所有“残物”单位获得2点强化；或从牌组打出1张铜色/银色“残物”牌，并使其获得2点强化。
        public WeavessIncantation(GameCard card) : base(card){}
        public override async Task<int> CardPlayEffect(bool isSpying,bool isReveal)
        {
            //选择选项,设置每个选项的名字和效果
            var switchCard = await Card.GetMenuSwitch
            (
                ("织婆：咒文", "使位于手牌、牌组和己方半场除自身外的所有“残物”单位获得2点强化。"),
                ("织婆：咒文", "从牌组打出1张铜色/银色“残物”牌，并使其获得2点强化。")
            );

            //选择了buff的话
            if (switchCard == 0)
            {
                //buff牌库
                var listDeck = Game.PlayersDeck[Card.PlayerIndex].Where(x => x.Status.Categories.Contains(Categorie.Relict)
                    && x.Status.CardId != CardId.WeavessIncantation).ToList();

                foreach(var target in listDeck)
                {
                    target.Effect.Boost(2);
                }

                //buff手牌
                var listHand = Game.PlayersHandCard[Card.PlayerIndex].Where(x => x.Status.Categories.Contains(Categorie.Relict)
                    && x.Status.CardId != CardId.WeavessIncantation).ToList();

                foreach(var target in listHand)
                {
                    target.Effect.Boost(2);
                }

                ////buff半场
                foreach(var row in Game.PlayersPlace[Card.PlayerIndex].ToList())
                {
                    var list = row.Where(x => x.Status.Categories.Contains(Categorie.Relict)).ToList();
                    foreach(var target in list)
                    {
                        target.Effect.Boost(2);
                    }
                }
                return 0;
            }


            //选择了打残物的话
            else if (switchCard == 1)
            {
                var list = Game.PlayersDeck[Card.PlayerIndex].Where(x => x.Status.Categories.Contains(Categorie.Relict)).ToList();
                var cards = await Game.GetSelectMenuCards(Card.PlayerIndex,list,1);

                //没有选就无事发生
                if (cards.Count() == 0) 
                {
                    return 0;
                }
                
                //打出
                var playCard = cards.Single();
                await playCard.MoveToCardStayFirst();
                await playCard.Effect.Boost(2);
                return 1;
            }
            
            return 0;
        }
    }
}