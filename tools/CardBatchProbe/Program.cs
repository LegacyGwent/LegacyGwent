using Cynthia.Card;
using Newtonsoft.Json;

string[] properties =
[
    "CardId", "Name", "Strength", "Group", "Faction", "CardType", "CardUseInfo",
    "Info", "Categories", "IsDerive", "IsCountdown", "Countdown", "LinkedCards", "CardArtsId"
];

if (args.Length != 4 || args[0] != "--cards" || args[2] != "--output")
    throw new ArgumentException("Usage: CardBatchProbe --cards <comma-separated-ids> --output <file>");

var ids = args[1].Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
if (ids.Length == 0)
    throw new ArgumentException("At least one card ID is required.");
if (ids.Distinct(StringComparer.Ordinal).Count() != ids.Length)
    throw new ArgumentException("Duplicate card IDs are not allowed.");

var unknown = ids.Where(id => !GwentMap.CardMap.ContainsKey(id)).ToArray();
if (unknown.Length != 0)
    throw new ArgumentException("Unknown card IDs: " + string.Join(",", unknown));

var payload = new
{
    version = GwentMap.CardMapVersion.ToString(),
    properties,
    cards = ids.ToDictionary(id => id, id => GwentMap.CardMap[id], StringComparer.Ordinal)
};
File.WriteAllText(args[3], JsonConvert.SerializeObject(payload));
