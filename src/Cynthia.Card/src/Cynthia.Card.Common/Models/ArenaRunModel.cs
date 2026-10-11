using System;
using System.Collections.Generic;

namespace Cynthia.Card
{
    public enum ArenaRunState
    {
        Drafting,   //still picking cards / leader
        Ready,      //deck complete, playing matches
        Finished    //9 wins or 3 losses reached
    }

    //One Arena run of one user. Id = UserName, so every user has at most one run.
    public class ArenaRunModel : ModelBase
    {
        public string PlayerName { get; set; }
        public ArenaRunState State { get; set; } = ArenaRunState.Drafting;
        //1-based number of the pick that is currently offered (1..ArenaDraft.TotalPicks)
        public int PickNumber { get; set; } = 1;
        //the cards the player can choose from right now (empty when not drafting)
        public List<string> Choices { get; set; } = new List<string>();
        public List<string> Deck { get; set; } = new List<string>();
        public string Leader { get; set; } = "";
        public int Wins { get; set; }
        public int Losses { get; set; }
        public List<string> Opponents { get; set; } = new List<string>();
        public DateTime StartTime { get; set; } = DateTime.UtcNow;

        public DeckModel ToDeckModel() => new DeckModel { Id = "arena", Name = "Arena", Deck = new List<string>(Deck), Leader = Leader };
    }
}
