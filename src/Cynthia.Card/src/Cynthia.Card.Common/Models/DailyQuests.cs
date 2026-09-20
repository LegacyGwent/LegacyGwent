using System.Collections.Generic;

namespace Cynthia.Card
{
    public sealed class DailyQuestProgress
    {
        public string Day { get; set; }
        public bool LoginGranted { get; set; }
        public int Crowns { get; set; }
        public int PowderGranted { get; set; }
        public List<string> RoundIds { get; set; } = new List<string>();
        // Persisted with the wallet, retained across daily resets, never sent to clients.
        // Includes capped settlements so their retries cannot earn a crown on another day.
        [Newtonsoft.Json.JsonIgnore]
        public List<string> ProcessedRoundIds { get; set; } = new List<string>();
    }
    public sealed class DailyQuestTier
    {
        public int Crowns { get; set; }
        public int Powder { get; set; }
    }
    public sealed class DailyQuestResult
    {
        public string Status { get; set; }
        public string ServerUtc { get; set; }
        public string ResetUtc { get; set; }
        public string TimeZone { get; set; } = "Asia/Shanghai";
        public int LoginPowder { get; set; }
        public int DailyCap { get; set; }
        public List<DailyQuestTier> Tiers { get; set; }
        public PremiumCollectionResult Wallet { get; set; }
        public bool Success => Status == "ok";
    }
}
