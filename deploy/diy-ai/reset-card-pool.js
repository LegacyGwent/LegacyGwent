/* global db, EXECUTE */
(function () {
    "use strict";

    var execute = typeof EXECUTE !== "undefined" && EXECUTE === true;
    var invalidIds = [
        "70084", "70001", "70002", "70003", "70004", "70005", "70006", "70007", "70008", "70009",
        "70010", "70011", "70012", "70013", "70015", "70016", "70017", "70019", "70020", "70021",
        "70022", "70023", "70024", "70025", "70026", "70027", "70032", "70033", "70038", "70039",
        "70040", "70041", "70042", "70043", "70044", "70045", "70046", "70050", "70054", "70058",
        "70059", "70062", "70070", "70071", "70072", "70076", "70077", "70078", "70091", "70105",
        "70102", "70103", "70104", "70106", "70107", "70108", "70109", "70119", "70110", "70111",
        "70112", "70113", "70114", "70115", "70116", "70117", "70118", "130210", "130220", "130200",
        "130180", "130190", "130010", "130150", "130020", "130160", "130130", "130140", "130170", "130110",
        "130090", "130100", "130120", "130080", "130050", "130060", "130070", "130030", "130040", "640080",
        "240140", "240230", "240250", "70079", "70080", "70081", "70082", "70083", "70085", "70086",
        "70088", "70089", "70090", "70092", "70093", "70094", "70095", "70096", "70097", "70098",
        "70099", "70100", "70101", "70132", "70133", "70121", "70122", "70123", "70124", "70125",
        "70126", "70127", "70128", "70129", "70130", "70131", "70159", "70160", "70134", "70135",
        "70136", "70137", "70138", "70139", "70140", "70141", "70142", "70143", "70144", "70145",
        "70146", "70147", "70148", "70149", "70150", "70151", "70152", "70153", "70154", "70155",
        "70156", "70157", "70158", "70161", "70162", "70163", "70164", "70165", "70166", "70167",
        "70168", "70169", "70170", "70171", "70172", "70173", "70174", "70175", "70176", "70177",
        "70178", "70179", "70180", "70181", "70182", "70183", "70184", "70185", "70186", "70187",
        "70188", "70189", "70190",
        "70014", "70018", "80001", "80002", "80003", "89004", "89005", "89006", "89007", "89008"
    ];
    var invalid = {};
    invalidIds.forEach(function (id) { invalid[id] = true; });

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
        if (!deck || invalid[String(deck.Leader)]) {
            return true;
        }
        return (deck.Deck || []).some(function (id) { return invalid[String(id)]; });
    }

    var users = db.getSiblingDB("gwentdiy").getCollection("user");
    var stats = {
        mode: execute ? "execute" : "dry-run",
        invalidCardIds: invalidIds.length,
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
            ? blacklist.filter(function (id) { return !invalid[String(id)]; })
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
