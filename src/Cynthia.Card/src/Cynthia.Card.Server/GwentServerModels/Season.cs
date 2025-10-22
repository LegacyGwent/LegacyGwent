using System.Collections.Generic;
using System;
using Cynthia.Card.Common.Models;

namespace Cynthia.Card.Server
{
    public class SeasonData
    {
       
        public object this[string propertyName] // allows the ["property"] syntax
        {
            get { return this.GetType().GetProperty(propertyName).GetValue(this, null); }
            set { this.GetType().GetProperty(propertyName).SetValue(this, value, null); }
        }
        public bool isActive { get; set; }
        public int SeasonId { get; set; }
        public string SeasonName { get; set; }
        public string SeasonColor { get; set; }
        public bool areRewardsGranted { get; set; } = false;
        public List<SeasonReward> seasonalRewards { get; set; } = new List<SeasonReward>();
        public DateTime SeasonStartTime { get; set; } = DateTime.UtcNow;
        public DateTime SeasonEndTime { get; set; } = DateTime.UtcNow;
        public string rankingHistory { get; set; } = "";
    }
}
