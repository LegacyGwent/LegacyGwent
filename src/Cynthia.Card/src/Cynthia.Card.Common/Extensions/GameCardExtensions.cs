using System.Text.RegularExpressions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions.Extensions;
using Newtonsoft.Json;

namespace Cynthia.Card
{
    public static class GameCardExtensions
    {
        public static Task<IList<GameCard>> GetSelectPlanceCards(this GameCard card, Func<GameCard, bool> filter = null, SelectModeType selectMode = SelectModeType.AllRow, bool isEnemySwitch = false, CardType selectType = CardType.Unit, int count = 1, int range = 0)
        {
            return card.Game.GetSelectPlaceCards(card, count, isEnemySwitch, filter, selectMode, selectType, range);
        }

        public static bool Is(this GameCard card, Group? group = null, CardType? type = null, Func<GameCard, bool> filter = null, Faction? faction = null)
        {
            return (faction == null ? true : card.Status.Faction == faction) &&
            (group == null ? true : card.Status.Group == group) &&
            (filter == null ? true : filter(card)) &&
            (type == null ? true : card.Status.Type == type);
        }

        public static int GetCrewedCount(this GameCard card)
        {
            if (!card.Status.CardRow.IsOnPlace())
            {
                // return 0;
                throw new InvalidOperationException("card is not on plance");
            }
            var count = 0;
            foreach (var target in card.GetRangeCard(1, GetRangeType.HollowAll))
            {
                count += target.Status.CrewCount;
            }
            return count;
        }

        public static bool Is(this GwentCard card, Group? group = null, CardType? type = null, Func<GwentCard, bool> filter = null, Faction? faction = null)
        {
            return (faction == null ? true : card.Faction == faction) &&
            (group == null ? true : card.Group == group) &&
            (filter == null ? true : filter(card)) &&
            (type == null ? true : card.CardType == type);
        }

        public static bool Is(this CardStatus card, Group? group = null, CardType? type = null, Func<CardStatus, bool> filter = null, Faction? faction = null)
        {
            return (faction == null ? true : card.Faction == faction) &&
            (group == null ? true : card.Group == group) &&
            (filter == null ? true : filter(card)) &&
            (type == null ? true : card.Type == type);
        }
        public static IEnumerable<CardStatus> FilterCards(this IEnumerable<CardStatus> cards, Group? group = null, CardType? type = null, Func<CardStatus, bool> filter = null, Faction? faction = null)
        {
            return cards.Where(x => x.Is(group, type, filter, faction));
        }
        public static IEnumerable<GameCard> FilterCards(this IEnumerable<GameCard> cards, Group? group = null, CardType? type = null, Func<GameCard, bool> filter = null, Faction? faction = null)
        {
            return cards.Where(x => x.Is(group, type, filter, faction));
        }
        public static bool IsAnyGroup(this CardStatus card, params Group[] groups)
        {
            return groups.Any(x => card.Group == x);
        }
        public static bool IsAnyGroup(this GwentCard card, params Group[] groups)
        {
            return groups.Any(x => card.Group == x);
        }
        public static bool IsAnyGroup(this GameCard card, params Group[] groups)
        {
            return groups.Any(x => card.Status.Group == x);
        }
        public static bool IsAliveOnPlance(this GameCard card)
        {
            return card.Status.CardRow.IsOnPlace() && !card.IsDead;
        }
        public static bool HasAll<T>(this T[] item, params T[] items) where T : Enum
        {
            return !items.Any(x => !item.Contains(x));
        }
        public static bool HasAny<T>(this T[] item, params T[] items) where T : Enum
        {
            return item.Intersect(items).Any();
        }
        public static bool HasAnyCategorie(this GameCard card, params Categorie[] categories)
        {
            return card.Status.Categories.Intersect(categories).Any();
        }

        public static bool HasAllCategorie(this GameCard card, params Categorie[] categories)
        {
            return !categories.Any(x => !card.Status.Categories.Contains(x));
        }
        public static bool HasAnyCategorie(this CardStatus card, params Categorie[] categories)
        {
            return card.Categories.Intersect(categories).Any();
        }

        public static bool HasAllCategorie(this CardStatus card, params Categorie[] categories)
        {
            return !categories.Any(x => !card.Categories.Contains(x));
        }

        public static void AddEffects(this GameCard card, params string[] effectIds)
        {
            foreach (var effectId in effectIds)
            {
                card.Effects.Add(card.Game.CreateEffectInstance(effectId, card));
            }
        }
    }
}