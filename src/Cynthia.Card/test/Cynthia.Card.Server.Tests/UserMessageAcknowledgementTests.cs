using System.Collections.Generic;
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
    }
}
