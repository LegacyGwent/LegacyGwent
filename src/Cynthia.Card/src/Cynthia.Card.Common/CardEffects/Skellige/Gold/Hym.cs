using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;
using Cynthia.Card.Common.CardEffects.Neutral.Derive;

namespace Cynthia.Card
{
    [CardEffectId("62012")] //店店：猎人
    public class Hym : Choose
    {
        //派“店店”去多尔·布雷坦纳的森林。 造成15点伤害；对一个敌军随机单位造成2点伤害，连续8次；重新打出1个铜色/银色单位，并使它获得5点增益；从牌组打出1张铜色/银色单位牌；移除己方半场的所有“灾厄”效果，并使友军单位获得1点增益。
        public Hym(GameCard card) : base(card)
        {
        }

        protected override async Task<int> UseMethodByChoice(int switchCard)
        {
            switch (switchCard)
            {
                case 1:
                    return await Playcursed();
                case 2:
                    return await Playsilver();
                
            }

            return 0;
        }


        protected override void RealInitDict()
        {
            methodDesDict = new Dictionary<int, string>()
            {
                {1, "Hym_1_Playcursed"},
                {2, "Hym_2_Playsilver"}
            };
        }


       



        private async Task<int> Playcursed()
        {
            //乱序列出诅咒生物，如果没有，什么都不做
                var list = Game.PlayersDeck[Card.PlayerIndex].Where(x => x.Status.Categories.Contains(Categorie.Cursed) &&
                       (x.Status.Group == Group.Silver || x.Status.Group == Group.Copper))
                    .Mess(Game.RNG)
                    .ToList();

                if (list.Count() == 0)
                {
                    return 0;
                }
                //选一张，如果没选，什么都不做
                var cards = await Game.GetSelectMenuCards(Card.PlayerIndex, list, 1);
                if (cards.Count() == 0)
                {
                    return 0;
                }

                //打出
                var playCard = cards.Single();
                await playCard.MoveToCardStayFirst();
                return 1;
        }

        private async Task<int> Playsilver()
        {
           //手动排除大间谍
                var cardsId = Game.PlayerBaseDeck[AnotherPlayer].Deck
                   .Select(x => x.CardId)
                   .Distinct()
                   .Where(x => !GwentMap.CardMap[x].HasAnyCategorie(Categorie.Agent) && GwentMap.CardMap[x].Is(Group.Silver, CardType.Unit))
                   .Mess(Game.RNG)
                   .Take(3).ToArray();
                return await Game.CreateAndMoveStay(PlayerIndex, cardsId);
        }
    }
}