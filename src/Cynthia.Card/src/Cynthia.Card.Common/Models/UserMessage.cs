using System.Collections.Generic;
using System.Linq;

namespace Cynthia.Card
{
    public class UserMessage : ModelBase
    {
        public object this[string propertyName] // allows the user["property"] syntax
        {
            get { return this.GetType().GetProperty(propertyName).GetValue(this, null); }
            set { this.GetType().GetProperty(propertyName).SetValue(this, value, null); }
        }
        public int MessageId;
        public string MessageName;
        public UserMessage(string MessageName, int Id)
        {
            this.MessageName = MessageName;
            this.MessageId = Id;
        }
        
        public static UserMessage ReCreateMessage(string condensedMessage)
        {
            var parts = condensedMessage.Split('|');
            string messageType = parts[0];
            switch (messageType)
            {
                case "UserSeasonEndMessage":
                    var messageId = int.Parse(parts[1]);
                    var avatars = parts[2]
                        .Split(',')
                        .Select(s => s.Trim())
                        .Where(s => !string.IsNullOrWhiteSpace(s))
                        .ToList();
                    var borders = parts[3]
                        .Split(',')
                        .Select(s => s.Trim())
                        .Where(s => !string.IsNullOrWhiteSpace(s))
                        .ToList();
                    var titles = parts[4]
                        .Split(',')
                        .Select(s => s.Trim())
                        .Where(s => !string.IsNullOrWhiteSpace(s))
                        .ToList();
                    var mmrBeforeReset = int.Parse(parts[5]);
                    var rank = int.Parse(parts[6]);
                    var seasonName = parts[7];

                    var deserializedMessage = new UserSeasonEndMessage(
                        messageType,
                        avatars,
                        borders,
                        titles,
                        mmrBeforeReset,
                        rank,
                        seasonName,
                        messageId
                    );

                    return deserializedMessage;
                    break;

                default:
                    return null;
                    break;
            }
        }
    }
    public class UserSeasonEndMessage : UserMessage
    {
        public IList<string> avatars;
        public IList<string> borders;
        public IList<string> titles;
        public int mmrBeforeReset;
        public int rank;
        public string seasonName;
        public UserSeasonEndMessage(string MessageName, IList<string> avatars, IList<string> borders,
                                    IList<string> titles, int mmrBeforeReset, int rank, string seasonName, int messageID = 0) : base(MessageName, messageID)
        {
            this.avatars = avatars;
            this.borders = borders;
            this.titles = titles;
            this.mmrBeforeReset = mmrBeforeReset;
            this.rank = rank;
            this.seasonName = seasonName;
        }
    }
    
    
}

