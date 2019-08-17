using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;
using Cynthia.Card.Common.CardEffects.Neutral.Derive;

namespace Cynthia.Card
{
	[CardEffectId("15003")]//店店：法师
	public class ShupeMage : AbstractShupe
	{//派“店店”去班·阿德学院，见见那里的小伙子们。 抽1张牌；随机魅惑1个敌军单位；在对方三排随机生成一种灾厄；对1个敌军造成10点伤害，再对其相邻单位造成5点；从牌组打出1张铜色/银色“特殊”牌。
		public ShupeMage(GameCard card) : base(card){}

        protected override async Task<int> UseMethodByChoice(int switchCard)
        {
            switch (switchCard)
            {
                case 1:
                    return await DrawOneCard();
                case 2:
                    return await CharmRNG();
                case 3:
                    return await HazardRNG();
                case 4:
                    return await DamageThreeEnemy();
                case 5:
                    return await PlaySpecialFromDeck();
            }

            return 0;
        }

        private async Task<int> PlaySpecialFromDeck()
        {
            //己方卡组乱序呈现
            var list = Game.PlayersDeck[PlayerIndex]
                .Where(x => (x.Status.Group == Group.Copper || x.Status.Group == Group.Silver) &&
                            x.Status.Type == CardType.Special)
                .Mess(RNG)
                .ToList();
            //让玩家选择一张卡,不能不选
            var result = await Game.GetSelectMenuCards(PlayerIndex, list);
            if (result.Count == 0) return 0; //如果没有任何符合标准的牌,返回
            await result.Single().MoveToCardStayFirst();
            return 1;
        }

        private async Task<int> DamageThreeEnemy()
        {
            var result = await Game.GetSelectPlaceCards(Card);
            if (!result.Any()) return 0;
            var target = result[0];
            await target.Effect.Damage(10, Card);
            var Ltaget = target.GetRangeCard(1, GetRangeType.HollowLeft);
            if (Ltaget.Count() != 0 && !Ltaget.Single().Status.Conceal)
            {
                await Ltaget.Single().Effect.Damage(5, Card);
            }

            //如果右侧有单位且不是伏击卡
            var Rtaget = target.GetRangeCard(1, GetRangeType.HollowRight);
            if (Rtaget.Count() != 0 && !Rtaget.Single().Status.Conceal)
            {
                await Rtaget.Single().Effect.Damage(5, Card);
            }
            return 0;
        }

        private async Task<int> HazardRNG()
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


            var allenemylist = new List<RowPosition>() { RowPosition.EnemyRow1, RowPosition.EnemyRow2, RowPosition.EnemyRow3 };
            foreach (var row in allenemylist)
            {
                var hazard = realList.Mess(RNG).First();
                await Game.GameRowEffect[AnotherPlayer][row.Mirror().MyRowToIndex()].SetStatus(hazard);
            }


            return 0;

        }

        private async Task<int> CharmRNG()
        {

            var list = Game.GetPlaceCards(AnotherPlayer).ToList();
            if (!list.Any()) return 0;
            var target = list.Mess(RNG).First();

            await target.Effect.Charm(Card);
            return 0;

        }

        private async Task<int> DrawOneCard()
        {
            await Game.PlayerDrawCard(PlayerIndex);//抽卡
            return 0;
        }

        protected override void RealInitDict()
        {
            methodDesDict = new Dictionary<int, string>()
            {
                {1, "抽1张牌"},
                {2, "随机魅惑1个敌军单位"},
                {3, "在对方三排随机生成一种灾厄"},
                {4, "对1个敌军造成10点伤害，再对其相邻单位造成5点"},
                {5, "从牌组打出1张铜色/银色“特殊”牌"}
            };
        }
    }
}