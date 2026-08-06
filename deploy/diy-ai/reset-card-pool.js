/* global db, EXECUTE */
(function () {
    "use strict";

    var execute = typeof EXECUTE !== "undefined" && EXECUTE === true;
    // This allowlist is the exact DiyAiCardPool.IsUserDeckCard set for CardMap
    // 1.0.0.173. An allowlist also rejects orphan IDs that never existed in the
    // current map, unlike a finite retired-card denylist.
    var allowedUserCardRanges = [
        [12001, 12042], [13001, 13044], [14001, 14027],
        [21001, 21005], [22001, 22002], [22004, 22014], [23001, 23022], [24001, 24038],
        [31001, 31004], [32001, 32015], [33001, 33023], [34001, 34033],
        [41001, 41004], [42001, 42013], [43001, 43021], [44001, 44034],
        [51001, 51004], [52001, 52013], [53001, 53021], [54001, 54032],
        [61001, 61004], [62001, 62013], [63001, 63020], [64001, 64034],
        [70001, 70002], [70005, 70005], [70007, 70008], [70011, 70011], [70025, 70027],
        [70009, 70010], [70022, 70023], [70032, 70032], [70058, 70058],
        [70072, 70072], [70083, 70085], [70088, 70088],
        [70041, 70042], [70059, 70059], [70062, 70062], [70070, 70070],
        [70091, 70091], [70110, 70110], [70119, 70119], [70131, 70133],
        [70125, 70125], [70128, 70128], [70155, 70158], [70161, 70161],
        [70045, 70045], [70149, 70149], [70172, 70172], [70179, 70179],
        [70102, 70102], [70113, 70113], [70145, 70145], [70154, 70154],
        [70106, 70106], [70124, 70124], [70129, 70129],
        [70146, 70146], [70148, 70148], [70164, 70164], [70168, 70170],
        [70176, 70177], [70180, 70180], [70183, 70183], [70185, 70185],
        [70190, 70192]
    ];

    var allowedUserCardIds = {};
    var allowedUserCardCount = 0;
    allowedUserCardRanges.forEach(function (range) {
        for (var id = range[0]; id <= range[1]; id += 1) {
            var key = String(id);
            if (!Object.prototype.hasOwnProperty.call(allowedUserCardIds, key)) {
                allowedUserCardIds[key] = true;
                allowedUserCardCount += 1;
            }
        }
    });

    function isAllowedUserCard(id) {
        return typeof id === "string"
            && Object.prototype.hasOwnProperty.call(allowedUserCardIds, id);
    }

    // Retired or derived-only test cards should normally be migrated in place
    // instead of deleting the player's whole deck. If the replacement is
    // already present, drop the old copy to avoid creating a duplicate gold.
    var cardIdReplacements = {
        "22003": "22001"
    };

    function migrateCardList(cardIds) {
        var migrated = [];
        var replacements = 0;
        var removedDuplicates = 0;

        (cardIds || []).forEach(function (id) {
            var replacement = cardIdReplacements[id];
            if (!replacement) {
                migrated.push(id);
                return;
            }

            replacements += 1;
            if (migrated.indexOf(replacement) >= 0
                || (cardIds || []).indexOf(replacement) >= 0) {
                removedDuplicates += 1;
                return;
            }
            migrated.push(replacement);
        });

        return {
            cardIds: migrated,
            replacements: replacements,
            removedDuplicates: removedDuplicates
        };
    }

    function containsInvalidCard(deck) {
        if (!deck || !isAllowedUserCard(deck.Leader)) {
            return true;
        }
        return (deck.Deck || []).some(function (id) { return !isAllowedUserCard(id); });
    }

    var users = db.getSiblingDB("gwentdiy").getCollection("user");
    var stats = {
        mode: execute ? "execute" : "dry-run",
        allowedUserCardIds: allowedUserCardCount,
        scannedUsers: 0,
        affectedUsers: 0,
        migratedDecks: 0,
        replacedDeckCards: 0,
        removedDuplicateDeckCards: 0,
        blacklistUsers: 0,
        replacedBlacklistEntries: 0,
        remainingInvalidDecks: 0
    };

    users.find({}, { Decks: 1, Blacklist: 1 }).forEach(function (user) {
        stats.scannedUsers += 1;
        var originalDecks = user.Decks || [];
        var migratedDecks = originalDecks.map(function (deck) {
            var migration = migrateCardList(deck && deck.Deck);
            if (migration.replacements === 0) {
                return deck;
            }
            stats.migratedDecks += 1;
            stats.replacedDeckCards += migration.replacements;
            stats.removedDuplicateDeckCards += migration.removedDuplicates;
            return Object.assign({}, deck, { Deck: migration.cardIds });
        });
        stats.remainingInvalidDecks += migratedDecks.filter(containsInvalidCard).length;
        var blacklist = user.Blacklist && user.Blacklist.Blacklist;
        var blacklistMigration = Array.isArray(blacklist) ? migrateCardList(blacklist) : null;
        var migratedBlacklist = blacklistMigration ? blacklistMigration.cardIds : blacklist;
        var replacedBlacklist = blacklistMigration ? blacklistMigration.replacements : 0;

        if (migratedDecks.every(function (deck, index) { return deck === originalDecks[index]; })
            && replacedBlacklist === 0) {
            return;
        }

        stats.affectedUsers += 1;
        stats.replacedBlacklistEntries += replacedBlacklist;
        if (replacedBlacklist > 0) {
            stats.blacklistUsers += 1;
        }

        if (execute) {
            var set = { Decks: migratedDecks };
            if (Array.isArray(blacklist)) {
                set["Blacklist.Blacklist"] = migratedBlacklist;
            }
            users.updateOne({ _id: user._id }, { $set: set });
        }
    });

    print(JSON.stringify(stats));
}());
