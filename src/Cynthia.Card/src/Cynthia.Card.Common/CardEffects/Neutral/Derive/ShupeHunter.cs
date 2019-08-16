using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
	[CardEffectId("15002")]//店店：猎人
	public class ShupeHunter : CardEffect
	{//派“店店”去多尔·布雷坦纳的森林。 造成15点伤害；对一个敌军随机单位造成2点伤害，连续8次；重新打出1个铜色/银色单位，并使它获得5点增益；从牌组打出1张铜色/银色单位牌；移除己方半场的所有“灾厄”效果，并使友军单位获得1点增益。
		public ShupeHunter(GameCard card) : base(card){}
		public override async Task<int> CardPlayEffect(bool isSpying,bool isReveal)
        {
            InitDict();
            var list = baseChoiceList.Mess(RNG).Take(3).ToList();
            var switchCard = await Card.GetMenuSwitch
            (
                (string.Empty, methodDesDict[list[0]]),
                (string.Empty, methodDesDict[list[1]]),
                (string.Empty, methodDesDict[list[2]])
            );
            return await UseMethodByChoice(list[switchCard]);
            
		}

        private async Task<int> UseMethodByChoice(int switchCard)
        {
            switch (switchCard)
            {
                case 1:
                    return await Damage();
                case 2:
                    return await RandomDamage();
                case 3:
                    return await PlayUnitAgain();
                case 4:
                    return await PlayUnitFromDeck();
                case 5:
                    return await RemoveHazard();
            }

            return 0;
        }

        private List<int> baseChoiceList = new List<int>{1,2,3,4,5};

        private Dictionary<int, Action<Task<int>>> methodDict;
        private Dictionary<int, string> methodDesDict;

        private void InitDict()
        {

            if (methodDesDict == null)
            {
                methodDesDict = new Dictionary<int, string>()
                {
                    {1, "造成15点伤害"},
                    {2, "对一个敌军随机单位造成2点伤害，连续8次"},
                    {3, "重新打出1个铜色/银色单位，并使它获得5点增益"},
                    {4, "从牌组打出1张铜色/银色单位牌"},
                    {5, "移除己方半场的所有“灾厄”效果，并使友军单位获得1点增益"}
                };
            }
            
        }

        private async Task<int> RemoveHazard()
        {
            var targetRows = Game.GameRowEffect[Card.PlayerIndex].Indexed()
                .Where(x => x.Value.RowStatus.IsHazard())
                .Select(x => x.Key);
            foreach (var rowIndex in targetRows)
            {
                await Game.GameRowEffect[PlayerIndex][rowIndex].SetStatus<NoneStatus>();
            }


            var list = Game.GetPlaceCards(PlayerIndex).Where(x=>x.Status.Type == CardType.Unit).ToList();

            foreach (var it in list)
            {
                await it.Effect.Boost(1, Card);
            }
            return 0;
        }

        private async Task<int> PlayUnitFromDeck()
        {
            //己方卡组乱序呈现
            var list = Game.PlayersDeck[PlayerIndex]
                .Where(x => (x.Status.Group == Group.Copper || x.Status.Group == Group.Silver) &&
                       x.Status.Type == CardType.Unit)
                .Mess(RNG)
                .ToList();
            //让玩家选择一张卡,不能不选
            var result = await Game.GetSelectMenuCards(PlayerIndex, list);
            if (result.Count == 0) return 0;//如果没有任何符合标准的牌,返回
            await result.Single().MoveToCardStayFirst();
            return 1;
        }

        private async Task<int> PlayUnitAgain()
        {
            var result = await Game.GetSelectPlaceCards(Card, filter: (x => x.Status.Group == Group.Copper || x.Status.Group == Group.Silver), selectMode: SelectModeType.MyRow);
            if (!result.Any()) return 0;
            var targetCard = result.Single();
            targetCard.Effect.Repair(true);
            await targetCard.MoveToCardStayFirst();
            await targetCard.Effect.Boost(5, Card);
            return 1;
        }

        private async Task<int> RandomDamage()
        {
            var list = Game.GetPlaceCards(AnotherPlayer).ToList();

            for (int i = 0; i < 8; i++)
            {
                var realList = list.Where(x => x.CardPoint() > 0).ToList();
                if (!realList.Any()) return 0;
                var card = realList.Mess(RNG).First();
                await card.Effect.Damage(2, Card);
            }
            return 0;
        }

        private async Task<int> Damage()
        {
            var result = await Game.GetSelectPlaceCards(Card);
            if (result.Count <= 0) return 0;
            await result.Single().Effect.Damage(15, Card);
            return 0;
        }
    }
}