using System.Text.Json;
using System;
using System.IO;
using System.Text.RegularExpressions;
using Cynthia.Card;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using Xunit;

namespace Cynthia.Card.Server.Tests
{
    public sealed class UserProfileCompatibilityTests
    {
        [Fact]
        public void GameSceneBindsTheActualUnlockPrefabAndCanvas()
        {
            var root = new DirectoryInfo(AppContext.BaseDirectory);
            while (root != null && !Directory.Exists(Path.Combine(root.FullName, "src", "Cynthia.Card.Unity")))
                root = root.Parent;
            Assert.NotNull(root);
            var assets = Path.Combine(root.FullName, "src", "Cynthia.Card.Unity", "src", "Cynthia.Unity.Card", "Assets");
            var scene = File.ReadAllText(Path.Combine(assets, "Resources", "Scenes", "Game.unity"));
            var prefab = File.ReadAllText(Path.Combine(assets, "Resources", "Prefab", "Trinkets", "TrinketUnlockPrefab.prefab"));
            var meta = File.ReadAllText(Path.Combine(assets, "Resources", "Prefab", "Trinkets", "TrinketUnlockPrefab.prefab.meta"));
            var prefabReference = Regex.Match(scene, @"TrinketUnlockPrefab: \{fileID: (\d+), guid: ([a-f0-9]+), type: 3\}");
            Assert.True(prefabReference.Success);
            Assert.Contains("guid: " + prefabReference.Groups[2].Value, meta);
            Assert.Contains("--- !u!1 &" + prefabReference.Groups[1].Value, prefab);
            var canvasReference = Regex.Match(scene, @"Canevas: \{fileID: ([1-9]\d*)\}");
            Assert.True(canvasReference.Success);
            Assert.Contains("--- !u!1 &" + canvasReference.Groups[1].Value, scene);
            Assert.Contains("guid: d9465e8aa619fbe4cb1b26f51330af4a", prefab);
        }

        [Theory]
        [InlineData("{}")]
        [InlineData("{\"OwnedAvatars\":null,\"OwnedBorders\":null,\"OwnedTitles\":null,\"NewlyUnlockedTrinkets\":null}")]
        [InlineData("{\"NewlyUnlockedTrinkets\":{\"NewAvatars\":null,\"NewBorders\":null,\"NewTitles\":null}}")]
        public void FreshAndLegacyWireProfilesHaveUsableCollections(string json)
        {
            var profile = JsonSerializer.Deserialize<UserInfo>(json);
            Assert.Empty(profile.OwnedAvatars);
            Assert.Empty(profile.OwnedBorders);
            Assert.Empty(profile.OwnedTitles);
            Assert.False(profile.NewlyUnlockedTrinkets.HasNewTrinkets);
            profile.NewlyUnlockedTrinkets.Clear();
            Assert.Equal("NoAvatar", TrinketMap.ResolveAvatar(profile.CurrentAvatar).ID);
            Assert.Equal("NoBorder", TrinketMap.ResolveBorder(profile.CurrentBorder).ID);
            Assert.Equal("CARDSMITH", TrinketMap.ResolveTitle(profile.CurrentTitle).ID);
        }

        [Fact]
        public void LegacyMongoNullAndMissingFieldsFollowTheWireContract()
        {
            Program.ConfigureMongoMappings();
            var profile = BsonSerializer.Deserialize<UserInfo>(new BsonDocument
            {
                { "UserName", "different-login" }, { "PlayerName", "different-display" },
                { "OwnedTitles", BsonNull.Value }, { "NewlyUnlockedTrinkets", BsonNull.Value }
            });
            Assert.Equal("different-login", profile.UserName);
            Assert.Equal("different-display", profile.PlayerName);
            Assert.Empty(profile.OwnedTitles);
            Assert.False(profile.NewlyUnlockedTrinkets.HasNewTrinkets);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("removed-or-newer-catalog-id")]
        public void PresentationFallbackDoesNotGrantOwnershipOrRewriteSavedSelections(string id)
        {
            var profile = new UserInfo { CurrentTitle = id, CurrentAvatar = id, CurrentBorder = id };
            Assert.Equal("CARDSMITH", TrinketMap.ResolveTitle(profile.CurrentTitle).ID);
            Assert.Equal("NoAvatar", TrinketMap.ResolveAvatar(profile.CurrentAvatar).ID);
            Assert.Equal("NoBorder", TrinketMap.ResolveBorder(profile.CurrentBorder).ID);
            Assert.Equal(id, profile.CurrentTitle);
            Assert.Empty(profile.OwnedTitles);
        }

        [Fact]
        public void ValidSelectionsAndUnlockNotificationsArePreserved()
        {
            var profile = new UserInfo { CurrentAvatar = "GeraltOfRivia", CurrentTitle = "CARDSMITH" };
            profile.NewlyUnlockedTrinkets.NewTitles.Add("CARDSMITH");
            Assert.Same(TrinketMap.AvatarMap["GeraltOfRivia"], TrinketMap.ResolveAvatar(profile.CurrentAvatar));
            Assert.Same(TrinketMap.TitleMap["CARDSMITH"], TrinketMap.ResolveTitle(profile.CurrentTitle));
            Assert.True(profile.NewlyUnlockedTrinkets.HasNewTrinkets);
        }
    }
}
