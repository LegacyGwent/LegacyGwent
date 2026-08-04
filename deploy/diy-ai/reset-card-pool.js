/* global db, EXECUTE */
(function () {
    "use strict";

    var execute = typeof EXECUTE !== "undefined" && EXECUTE === true;
    // This allowlist is the exact DiyAiCardPool.IsUserDeckCard set for CardMap
    // 1.0.0.171. An allowlist also rejects orphan IDs that never existed in the
    // current map, unlike a finite retired-card denylist.
    var allowedUserCardRanges = [
        [12001, 12042], [13001, 13044], [14001, 14027],
        [21001, 21005], [22001, 22014], [23001, 23022], [24001, 24038],
        [31001, 31004], [32001, 32015], [33001, 33023], [34001, 34033],
        [41001, 41004], [42001, 42013], [43001, 43021], [44001, 44034],
        [51001, 51004], [52001, 52013], [53001, 53021], [54001, 54032],
        [61001, 61004], [62001, 62013], [63001, 63020], [64001, 64034],
        [70001, 70002], [70005, 70005], [70007, 70008], [70011, 70011], [70025, 70027],
        [70032, 70032], [70072, 70072],
        [70041, 70042], [70059, 70059], [70062, 70062], [70070, 70070],
        [70091, 70091], [70110, 70110], [70119, 70119], [70131, 70131],
        [70125, 70125], [70128, 70128], [70133, 70133], [70155, 70158], [70161, 70161],
        [70045, 70045], [70149, 70149], [70172, 70172], [70179, 70179],
        [70180, 70180], [70190, 70191]
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

    var starterDeck = {
        _id: "diy-ai-reset-" + ObjectId().str,
        Name: "帝国基础卡组",
        Leader: "31001",
        Deck: [
            "12001", "12002", "12003", "13001", "13002", "33004", "32001", "33001", "33002", "33003",
            "34005", "34005", "34005", "34030", "34030", "34030", "34002", "34002", "34002",
            "34003", "34003", "34003", "34004", "34004", "34004"
        ]
    };

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
        removedDecks: 0,
        seededUsers: 0,
        blacklistUsers: 0,
        removedBlacklistEntries: 0
    };

    users.find({}, { Decks: 1, Blacklist: 1 }).forEach(function (user) {
        stats.scannedUsers += 1;
        var originalDecks = user.Decks || [];
        var keptDecks = originalDecks.filter(function (deck) { return !containsInvalidCard(deck); });
        var removedDecks = originalDecks.length - keptDecks.length;
        var blacklist = user.Blacklist && user.Blacklist.Blacklist;
        var keptBlacklist = Array.isArray(blacklist)
            ? blacklist.filter(function (id) { return isAllowedUserCard(id); })
            : blacklist;
        var removedBlacklist = Array.isArray(blacklist) ? blacklist.length - keptBlacklist.length : 0;

        if (removedDecks === 0 && removedBlacklist === 0) {
            return;
        }

        stats.affectedUsers += 1;
        stats.removedDecks += removedDecks;
        stats.removedBlacklistEntries += removedBlacklist;
        if (removedBlacklist > 0) {
            stats.blacklistUsers += 1;
        }
        if (removedDecks > 0 && keptDecks.length === 0) {
            keptDecks.push(Object.assign({}, starterDeck, { _id: "diy-ai-reset-" + ObjectId().str }));
            stats.seededUsers += 1;
        }

        if (execute) {
            var set = { Decks: keptDecks };
            if (Array.isArray(blacklist)) {
                set["Blacklist.Blacklist"] = keptBlacklist;
            }
            users.updateOne({ _id: user._id }, { $set: set });
        }
    });

    print(JSON.stringify(stats));
}());
