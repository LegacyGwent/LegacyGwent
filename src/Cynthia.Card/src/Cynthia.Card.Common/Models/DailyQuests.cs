using System.Collections.Generic;

namespace Cynthia.Card
{
    public sealed class DailyQuestProgress
    {
        public string Day { get; set; }
        public bool LoginGranted { get; set; }
        public int Crowns { get; set; }
        public int PowderGranted { get; set; }
        // Rewarded postgame GG count (0..6) and its powder subset (0..30) for the stored day.
        public int GGReceived { get; set; }
        public int GGPowderGranted { get; set; }
        public List<string> RoundIds { get; set; } = new List<string>();
        // Persisted with the wallet, retained across daily resets, never sent to clients.
        // Includes capped settlements so their retries cannot earn a crown on another day.
        [Newtonsoft.Json.JsonIgnore]
        public List<string> ProcessedRoundIds { get; set; } = new List<string>();
        // Server-only GG ledger. Includes capped GG events so a retry on another day cannot
        // pay again; keyed by the server-issued match id of the finished human match.
        [Newtonsoft.Json.JsonIgnore]
        public List<string> ProcessedGGIds { get; set; } = new List<string>();
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
        // Postgame GG contract: powder per accepted GG and the per-day GG sub-cap.
        public int GGPowder { get; set; }
        public int GGDailyCap { get; set; }
        public List<DailyQuestTier> Tiers { get; set; }
        public PremiumCollectionResult Wallet { get; set; }
        public bool Success => Status == "ok";
    }
}
