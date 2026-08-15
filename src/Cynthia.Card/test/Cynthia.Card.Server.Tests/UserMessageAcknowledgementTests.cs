using System.Collections.Generic;
using Cynthia.Card;
using Xunit;

namespace Cynthia.Card.Server.Tests
{
    public class UserMessageAcknowledgementTests
    {
        [Fact]
        public void RemovesExactlyTheAcknowledgedSeasonMessage()
        {
            var messages = new List<string>
            {
                "UserSeasonEndMessage|1||||3400|529|Season_DraconidSeason",
                "UserSeasonEndMessage|2||||3600|120|Season_CreaturesSeason"
            };

            var removed = GwentDatabaseService.TryRemoveUserMessage(messages, 1);

            Assert.True(removed);
            Assert.Single(messages);
            Assert.Contains("|2|", messages[0]);
        }

        [Fact]
        public void MissingOrMalformedMessagesDoNotAcknowledge()
        {
            var messages = new List<string>
            {
                "broken-message",
                "UserSeasonEndMessage|2||||3600|120|Season_CreaturesSeason"
            };

            Assert.False(GwentDatabaseService.TryRemoveUserMessage(messages, 99));
            Assert.Equal(2, messages.Count);
            Assert.False(GwentDatabaseService.TryRemoveUserMessage(null, 2));
        }

        [Fact]
        public void MessageLookupUsesLoginUsernameInsteadOfDisplayName()
        {
            var expected = "UserSeasonEndMessage|7||||3400|12|Season_WolfSeason";
            var leaked = "UserSeasonEndMessage|3||||3400|557|Season_WildHuntSeason";
            var users = new[]
            {
                new UserInfo
                {
                    UserName = "1",
                    PlayerName = "Marcopolo",
                    UserMessages = new List<string> { expected }
                },
                new UserInfo
                {
                    UserName = "11",
                    PlayerName = "1",
                    UserMessages = new List<string> { leaked }
                }
            };

            var messages = GwentDatabaseService.SelectUserMessagesByUsername(users, "1");

            Assert.Single(messages);
            Assert.Equal(expected, messages[0]);
        }
    }
}
