using System;
using System.Collections.Generic;

namespace Cynthia.Card
{
    /// <summary>
    /// Local-only rule cards used to exercise the complete editor and match UI.
    /// They are absent unless the host explicitly opts in through the environment.
    /// </summary>
    public static class DevelopmentRuleCardFixtures
    {
        public static bool Enabled => string.Equals(
            Environment.GetEnvironmentVariable("GWENT_ENABLE_RULE_FIXTURES"),
            "1",
            StringComparison.Ordinal);

        public static void Apply(IDictionary<string, GwentCard> cardMap)
        {
            if (!Enabled) return;

            Add(cardMap, "99001", "百卡实验", "将卡组上限提高至100张。", "11210100");
            Add(cardMap, "99002", "金银解禁", "取消金色、银色卡的数量与同名限制。", "11210200");
            Add(cardMap, "99003", "铜色试炼", "只能携带铜色卡，且铜色卡同名限1张。", "11210300");
            Add(cardMap, "99004", "饕餮回响", "吞噬能力触发两次（本地规则区事件样本）。", "11210400");
            Add(cardMap, "99005", "帝国远征令", "仅限尼弗迦德领袖；需要百卡实验，并将怪兽卡加入可选卡池。", "11210500");
        }

        private static void Add(IDictionary<string, GwentCard> cardMap, string id, string name, string info, string artId)
        {
            cardMap[id] = new GwentCard
            {
                CardId = id,
                Name = name,
                Info = info,
                Flavor = "仅用于本地规则系统验收，不会随正式卡池启用。",
                Strength = 0,
                Group = Group.Copper,
                Faction = Faction.Neutral,
                CardUseInfo = CardUseInfo.ReSet,
                CardType = CardType.Special,
                IsDoomed = false,
                IsCountdown = false,
                IsDerive = false,
                Categories = new Categorie[0],
                HideTags = new[] { HideTag.Rule },
                CardArtsId = artId,
                LinkedCards = new List<string>()
            };
        }
    }
}
