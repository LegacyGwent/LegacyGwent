using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;
using Cynthia.Card.Common.CardEffects.Neutral.Derive;

namespace Cynthia.Card
{
    [CardEffectId("15002")] //店店：猎人
    public class ShupeHunter : AbstractShupe
    {
        //派“店店”去多尔·布雷坦纳的森林。 造成15点伤害；对一个敌军随机单位造成2点伤害，连续8次；重新打出1个铜色/银色单位，并使它获得5点增益；从牌组打出1张铜色/银色单位牌；移除己方半场的所有“灾厄”效果，并使友军单位获得1点增益。
        public ShupeHunter(GameCard card) : base(card)
        {
        }

        protected override async Task<int> UseMethodByChoice(int switchCard)
        {
            switch (switchCard)
            {
                case 1:
                    return await DamageEnemy();
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


        protected override void RealInitDict()
        {
            methodDesDict = new Dictionary<int, string>()
            {
                {1, "ShupeHunter_1_SingleDamage"},
                {2, "ShupeHunter_2_RepeatedDamage"},
                {3, "ShupeHunter_3_Replay"},
                {4, "ShupeHunter_4_PlayFromDeck"},
                {5, "ShupeHunter_5_ClearSky"}
            };
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


            var list = Game.GetPlaceCards(PlayerIndex).ToList();

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
            if (result.Count == 0) return 0; //如果没有任何符合标准的牌,返回
            await result.Single().MoveToCardStayFirst();
            return 1;
        }

        private async Task<int> PlayUnitAgain()
        {
            var result = await Game.GetSelectPlaceCards(Card,
                filter: (x => x.Status.Group == Group.Copper || x.Status.Group == Group.Silver),
                selectMode: SelectModeType.MyRow);
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

        private async Task<int> DamageEnemy()
        {
            var result = await Game.GetSelectPlaceCards(Card);
            if (result.Count <= 0) return 0;
            await result.Single().Effect.Damage(15, Card);
            return 0;
        }
    }
}