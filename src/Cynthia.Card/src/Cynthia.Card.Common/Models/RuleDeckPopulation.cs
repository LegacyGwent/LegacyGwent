using System;
using System.Collections.Generic;

namespace Cynthia.Card
{
    public enum RuleDeckInsufficientPolicy
    {
        FailMatch = 0,
        UseAvailable = 1
    }

    public class RuleDeckPopulationResult
    {
        public string SourceRuleCardId { get; set; } = "";
        public int PlayerIndex { get; set; }
        public int RequestedDeckCount { get; set; }
        public int FinalDeckCount { get; set; }
        public int CandidateCount { get; set; }
        public bool UsedFallback { get; set; }
        public List<string> AddedCardIds { get; set; } = new List<string>();
    }

    public sealed class RuleDeckPopulationException : InvalidOperationException
    {
        public RuleDeckPopulationException(
            string sourceRuleCardId,
            int playerIndex,
            int requestedDeckCount,
            int availableCandidateCount)
            : base($"Rule '{sourceRuleCardId}' cannot populate player {playerIndex}'s deck to " +
                   $"{requestedDeckCount}: only {availableCandidateCount} distinct candidates are available.")
        {
            SourceRuleCardId = sourceRuleCardId ?? "";
            PlayerIndex = playerIndex;
            RequestedDeckCount = requestedDeckCount;
            AvailableCandidateCount = availableCandidateCount;
        }

        public string SourceRuleCardId { get; }
        public int PlayerIndex { get; }
        public int RequestedDeckCount { get; }
        public int AvailableCandidateCount { get; }
    }
}
