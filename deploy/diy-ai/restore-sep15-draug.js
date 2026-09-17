/* global db, EXECUTE, LIVE_DATABASE, PREIMAGE_DATABASE */
(function () {
    "use strict";

    var execute = typeof EXECUTE !== "undefined" && EXECUTE === true;
    var preimageDatabase = typeof PREIMAGE_DATABASE === "string"
        ? PREIMAGE_DATABASE : "gwentdiy_sep15_pre";
    var liveDatabase = typeof LIVE_DATABASE === "string" ? LIVE_DATABASE : "gwentdiy";
    var preimageUsers = db.getSiblingDB(preimageDatabase).getCollection("user");
    var liveUsers = db.getSiblingDB(liveDatabase).getCollection("user");
    var oldCardId = "22002";
    var replacementCardId = "70197";
    var stats = {
        mode: execute ? "execute" : "dry-run",
        scannedUsers: 0,
        preimageDecks: 0,
        exactMatches: 0,
        restoredDecks: 0,
        restoredCards: 0,
        alreadyRestored: 0,
        missingLiveUsers: 0,
        missingLiveDecks: 0,
        changedDecksWithReplacement: 0,
        changedDecksWithoutReplacement: 0,
        writeConflicts: 0
    };

    function sameArray(left, right) {
        if (!Array.isArray(left) || !Array.isArray(right) || left.length !== right.length) {
            return false;
        }
        for (var index = 0; index < left.length; index += 1) {
            if (left[index] !== right[index]) {
                return false;
            }
        }
        return true;
    }

    function replaceAll(cardIds, from, to) {
        return (cardIds || []).map(function (cardId) {
            return cardId === from ? to : cardId;
        });
    }

    preimageUsers.find({ "Decks.Deck": oldCardId }, { Decks: 1 }).forEach(function (preimageUser) {
        stats.scannedUsers += 1;
        var liveUser = liveUsers.findOne({ _id: preimageUser._id }, { Decks: 1 });
        var preimageDecks = (preimageUser.Decks || []).filter(function (deck) {
            return deck && Array.isArray(deck.Deck) && deck.Deck.indexOf(oldCardId) >= 0;
        });

        if (!liveUser) {
            stats.preimageDecks += preimageDecks.length;
            stats.missingLiveUsers += 1;
            return;
        }

        preimageDecks.forEach(function (preimageDeck) {
            stats.preimageDecks += 1;
            var liveDeck = (liveUser.Decks || []).filter(function (candidate) {
                return candidate && candidate._id === preimageDeck._id;
            })[0];
            if (!liveDeck) {
                stats.missingLiveDecks += 1;
                return;
            }

            if (sameArray(liveDeck.Deck, preimageDeck.Deck)) {
                stats.alreadyRestored += 1;
                return;
            }

            var migratedDeck = replaceAll(preimageDeck.Deck, oldCardId, replacementCardId);
            if (!sameArray(liveDeck.Deck, migratedDeck)) {
                if (liveDeck.Deck.indexOf(replacementCardId) >= 0) {
                    stats.changedDecksWithReplacement += 1;
                } else {
                    stats.changedDecksWithoutReplacement += 1;
                }
                return;
            }

            var restoredCardCount = preimageDeck.Deck.filter(function (cardId) {
                return cardId === oldCardId;
            }).length;
            stats.exactMatches += 1;
            if (!execute) {
                return;
            }

            var result = liveUsers.updateOne(
                {
                    _id: preimageUser._id,
                    Decks: { $elemMatch: { _id: preimageDeck._id, Deck: migratedDeck } }
                },
                { $set: { "Decks.$.Deck": preimageDeck.Deck } }
            );
            if (result.matchedCount !== 1 || result.modifiedCount !== 1) {
                stats.writeConflicts += 1;
                return;
            }
            stats.restoredDecks += 1;
            stats.restoredCards += restoredCardCount;
        });
    });

    print(JSON.stringify(stats));
}());
