using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Cynthia.Card.Server;

public class Command
{
    public static IMongoClient client;
    static IMongoDatabase db;
    public static IMongoCollection<DiyCardInfo> diyCardCollection;
    public static IMongoCollection<DiyCardInfo> discussAreaCollection;
    public static IMongoCollection<AdminInfo> adminCollection;
    public static GwentDatabaseService _dbServer;
    private static string _webRootPath;

    private static readonly HashSet<string> AllowedCardTypes = new HashSet<string>(StringComparer.Ordinal)
    {
        "单位卡", "特殊卡"
    };

    private static readonly HashSet<string> AllowedFactions = new HashSet<string>(StringComparer.Ordinal)
    {
        "怪兽", "帝国", "松鼠", "北方", "群岛", "中立"
    };

    private static readonly HashSet<string> AllowedGroups = new HashSet<string>(StringComparer.Ordinal)
    {
        "领袖", "金色", "银色", "铜色"
    };

    public static void MongodbConnect(GwentDatabaseService dbServer, string webRootPath = null)
    {
        if (dbServer == null)
        {
            throw new ArgumentNullException(nameof(dbServer));
        }

        _dbServer = dbServer;
        _webRootPath = string.IsNullOrWhiteSpace(webRootPath)
            ? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot")
            : webRootPath;
        client = dbServer.GetMongoClient();
        db = client.GetDatabase("Web");
        diyCardCollection = db.GetCollection<DiyCardInfo>("DiyCards");
        discussAreaCollection = db.GetCollection<DiyCardInfo>("DiscussArea");
        adminCollection = db.GetCollection<AdminInfo>("Admin");
    }

    public static DiyCardInfo AddDiyCardInfos(DiyCardInfo diyCard)
    {
        var collection = RequireCollection(diyCardCollection, "DiyCards");
        var normalizedCard = NormalizeCard(diyCard);
        ValidateNewCard(normalizedCard);

        normalizedCard.uid = checked((int)collection.CountDocuments(FilterDefinition<DiyCardInfo>.Empty));
        normalizedCard.lastEditedDate = DateTime.Now;
        normalizedCard.commits = new List<DiyCardInfo.Commit>();
        normalizedCard.likeList = new List<string>();
        normalizedCard.dislikeList = new List<string>();
        collection.InsertOne(normalizedCard);
        return normalizedCard;
    }

    public static void AddDiscussCardInfos(DiyCardInfo diyCard)
    {
        var sourceCollection = RequireCollection(diyCardCollection, "DiyCards");
        var reviewCollection = RequireCollection(discussAreaCollection, "DiscussArea");
        var normalizedCard = NormalizeCard(diyCard);
        var filter = Builders<DiyCardInfo>.Filter.Eq(x => x._id, normalizedCard._id);
        var exists = reviewCollection.Find(filter).Any();

        if (!exists)
        {
            var reviewCard = CopyForReview(normalizedCard);
            reviewCard.uid = checked((int)reviewCollection.CountDocuments(FilterDefinition<DiyCardInfo>.Empty));
            reviewCard.lastEditedDate = DateTime.Now;
            reviewCollection.InsertOne(reviewCard);
        }

        var markSubmitted = Builders<DiyCardInfo>.Update.Set(x => x.IsInDiscuss, true);
        EnsureMatched(sourceCollection.UpdateOne(filter, markSubmitted), "DiyCards");
    }

    public static bool RemoveDiyCard(DiyCardInfo diyCard, IMongoCollection<DiyCardInfo> collection)
    {
        if (diyCard == null)
        {
            return false;
        }

        collection = RequireCollection(collection, "card collection");
        var filter = Builders<DiyCardInfo>.Filter.Eq(x => x._id, diyCard._id);
        return collection.DeleteOne(filter).DeletedCount > 0;
    }

    public static bool RemoveDiscussCard(DiyCardInfo diyCard)
    {
        if (diyCard == null)
        {
            return false;
        }

        var sourceCollection = RequireCollection(diyCardCollection, "DiyCards");
        var reviewCollection = RequireCollection(discussAreaCollection, "DiscussArea");
        var filter = Builders<DiyCardInfo>.Filter.Eq(x => x._id, diyCard._id);
        var submittedSourceFilter = Builders<DiyCardInfo>.Filter.And(
            filter,
            Builders<DiyCardInfo>.Filter.Eq(x => x.IsInDiscuss, true));
        var sourceUpdate = sourceCollection.UpdateOne(
            submittedSourceFilter,
            Builders<DiyCardInfo>.Update.Set(x => x.IsInDiscuss, false));

        try
        {
            var deleted = reviewCollection.DeleteOne(filter).DeletedCount > 0;
            if (!deleted && sourceUpdate.MatchedCount > 0)
            {
                sourceCollection.UpdateOne(
                    filter,
                    Builders<DiyCardInfo>.Update.Set(x => x.IsInDiscuss, true));
            }

            return deleted;
        }
        catch
        {
            if (sourceUpdate.MatchedCount > 0)
            {
                sourceCollection.UpdateOne(
                    filter,
                    Builders<DiyCardInfo>.Update.Set(x => x.IsInDiscuss, true));
            }

            throw;
        }
    }

    public static List<DiyCardInfo> GetDiyCardsInfo()
    {
        var collection = RequireCollection(diyCardCollection, "DiyCards");
        return NormalizeCards(collection.Find(FilterDefinition<DiyCardInfo>.Empty).ToList());
    }

    public static List<DiyCardInfo> GetDiscussCardsInfo()
    {
        var collection = RequireCollection(discussAreaCollection, "DiscussArea");
        return NormalizeCards(collection.Find(FilterDefinition<DiyCardInfo>.Empty).ToList());
    }

    public static DiyCardInfo AddDiyCardComment(
        DiyCardInfo diyCard,
        IMongoCollection<DiyCardInfo> collection,
        DiyCardInfo.Commit comment)
    {
        if (diyCard == null)
        {
            throw new ArgumentNullException(nameof(diyCard));
        }

        collection = RequireCollection(collection, "card collection");
        var normalizedComment = NormalizeComments(new[] { comment }).SingleOrDefault();
        if (normalizedComment == null || string.IsNullOrWhiteSpace(normalizedComment.text))
        {
            throw new ArgumentException("A non-empty comment is required.", nameof(comment));
        }

        ValidateLength(normalizedComment.user, 128, "Comment author");
        ValidateLength(normalizedComment.text, 2000, "Comment text");

        var filter = Builders<DiyCardInfo>.Filter.Eq("_id", diyCard._id);
        collection.UpdateOne(
            Builders<DiyCardInfo>.Filter.And(
                filter,
                Builders<DiyCardInfo>.Filter.Eq(x => x.commits, null)),
            Builders<DiyCardInfo>.Update.Set(x => x.commits, new List<DiyCardInfo.Commit>()));
        var update = Builders<DiyCardInfo>.Update.Push(x => x.commits, normalizedComment);
        var updated = collection.FindOneAndUpdate(
            filter,
            update,
            new FindOneAndUpdateOptions<DiyCardInfo>
            {
                ReturnDocument = ReturnDocument.After
            });

        if (updated == null)
        {
            throw new InvalidOperationException("Unable to add the comment: the card no longer exists.");
        }

        return NormalizeCard(updated);
    }

    public static DiyCardInfo ToggleDiyCardVote(
        DiyCardInfo diyCard,
        IMongoCollection<DiyCardInfo> collection,
        string userName,
        bool isLike)
    {
        if (diyCard == null)
        {
            throw new ArgumentNullException(nameof(diyCard));
        }

        if (string.IsNullOrWhiteSpace(userName))
        {
            throw new ArgumentException("A user name is required.", nameof(userName));
        }

        ValidateLength(userName, 128, "Vote user name");

        collection = RequireCollection(collection, "card collection");
        var filter = Builders<DiyCardInfo>.Filter.Eq("_id", diyCard._id);
        collection.UpdateOne(
            Builders<DiyCardInfo>.Filter.And(
                filter,
                Builders<DiyCardInfo>.Filter.Eq(x => x.likeList, null)),
            Builders<DiyCardInfo>.Update.Set(x => x.likeList, new List<string>()));
        collection.UpdateOne(
            Builders<DiyCardInfo>.Filter.And(
                filter,
                Builders<DiyCardInfo>.Filter.Eq(x => x.dislikeList, null)),
            Builders<DiyCardInfo>.Update.Set(x => x.dislikeList, new List<string>()));
        var selectedField = isLike ? "likeList" : "dislikeList";
        var oppositeField = isLike ? "dislikeList" : "likeList";
        var options = new FindOneAndUpdateOptions<DiyCardInfo>
        {
            ReturnDocument = ReturnDocument.After
        };

        var toggleOffFilter = Builders<DiyCardInfo>.Filter.And(
            filter,
            Builders<DiyCardInfo>.Filter.Eq(selectedField, userName));
        var updated = collection.FindOneAndUpdate(
            toggleOffFilter,
            Builders<DiyCardInfo>.Update.Pull(selectedField, userName),
            options);
        if (updated != null)
        {
            return NormalizeCard(updated);
        }

        var toggleOnFilter = Builders<DiyCardInfo>.Filter.And(
            filter,
            Builders<DiyCardInfo>.Filter.Ne(selectedField, userName));
        var toggleOnUpdate = Builders<DiyCardInfo>.Update
            .Pull(oppositeField, userName)
            .AddToSet(selectedField, userName);
        updated = collection.FindOneAndUpdate(toggleOnFilter, toggleOnUpdate, options);
        if (updated != null)
        {
            return NormalizeCard(updated);
        }

        updated = collection.Find(filter).FirstOrDefault();
        if (updated == null)
        {
            throw new InvalidOperationException("Unable to update card votes: the card no longer exists.");
        }

        return NormalizeCard(updated);
    }

    public static bool Login(string username, string password)
    {
        var user = _dbServer.Login(username, password);
        return user != null;
    }
    public static AdminInfo GetAdmin()
    {
        var collection = RequireCollection(adminCollection, "Admin");
        var sort = Builders<AdminInfo>.Sort.Descending(a => a.uid);
        var options = new FindOptions<AdminInfo> { Sort = sort, Limit = 1 };
        AdminInfo admin;

        using (var cursor = collection.FindSync(FilterDefinition<AdminInfo>.Empty, options))
        {
            admin = cursor.FirstOrDefault();
        }

        if (admin != null)
        {
            admin.cabinetUserName = NormalizeUsers(admin.cabinetUserName);
        }

        return admin;
    }

    public static DiyCardInfo NormalizeCard(DiyCardInfo card)
    {
        if (card == null)
        {
            card = new DiyCardInfo();
        }

        if (card.cardInfo == null)
        {
            card.cardInfo = new DiyCardInfo.SavedGwentCard();
        }

        card.Fraction = card.Fraction ?? string.Empty;
        card.CardType = card.CardType ?? string.Empty;
        card.Group = card.Group ?? string.Empty;
        card.Categories = card.Categories == null
            ? new List<string>()
            : card.Categories.Where(value => value != null).ToList();
        card.imageLink = (card.imageLink ?? string.Empty).Trim();
        card.author = (card.author ?? string.Empty).Trim();
        card.cardInfo.Name = (card.cardInfo.Name ?? string.Empty).Trim();
        card.cardInfo.Info = (card.cardInfo.Info ?? string.Empty).Trim();
        card.cardInfo.Flavor = (card.cardInfo.Flavor ?? string.Empty).Trim();
        card.likeList = NormalizeUsers(card.likeList);
        card.dislikeList = NormalizeUsers(card.dislikeList);
        card.commits = NormalizeComments(card.commits);
        return card;
    }

    private static List<DiyCardInfo> NormalizeCards(IEnumerable<DiyCardInfo> cards)
    {
        return (cards ?? Enumerable.Empty<DiyCardInfo>())
            .Where(card => card != null)
            .Select(NormalizeCard)
            .ToList();
    }

    private static List<string> NormalizeUsers(IEnumerable<string> users)
    {
        return (users ?? Enumerable.Empty<string>())
            .Where(value => !string.IsNullOrWhiteSpace(value))
            .Distinct(StringComparer.Ordinal)
            .ToList();
    }

    private static List<DiyCardInfo.Commit> NormalizeComments(IEnumerable<DiyCardInfo.Commit> comments)
    {
        var normalizedComments = (comments ?? Enumerable.Empty<DiyCardInfo.Commit>())
            .Where(comment => comment != null)
            .ToList();

        foreach (var comment in normalizedComments)
        {
            comment.user = comment.user ?? string.Empty;
            comment.text = comment.text ?? string.Empty;
            comment.likeList = NormalizeUsers(comment.likeList);
            comment.dislikeList = NormalizeUsers(comment.dislikeList);
        }

        return normalizedComments;
    }

    private static DiyCardInfo CopyForReview(DiyCardInfo source)
    {
        var copiedComments = NormalizeComments(source.commits)
            .Select(comment => new DiyCardInfo.Commit
            {
                user = comment.user,
                text = comment.text,
                commitDate = comment.commitDate,
                likeList = NormalizeUsers(comment.likeList),
                dislikeList = NormalizeUsers(comment.dislikeList)
            })
            .ToList();

        return new DiyCardInfo
        {
            _id = source._id,
            uid = source.uid,
            cardInfo = source.cardInfo,
            Fraction = source.Fraction,
            CardType = source.CardType,
            Group = source.Group,
            Categories = source.Categories.ToList(),
            imageLink = source.imageLink,
            author = source.author,
            lastEditedDate = source.lastEditedDate,
            likeList = NormalizeUsers(source.likeList),
            dislikeList = NormalizeUsers(source.dislikeList),
            commits = copiedComments,
            IsInDiscuss = true
        };
    }

    private static void ValidateNewCard(DiyCardInfo card)
    {
        RequireValue(card.cardInfo.Name, "Card name");
        RequireValue(card.cardInfo.Info, "Rules text");
        RequireValue(card.author, "Designer");
        RequireValue(card.imageLink, "Artwork ID");
        ValidateLength(card.cardInfo.Name, 80, "Card name");
        ValidateLength(card.cardInfo.Info, 2000, "Rules text");
        ValidateLength(card.cardInfo.Flavor, 1000, "Flavor text");
        ValidateLength(card.author, 128, "Designer");
        ValidateLength(card.imageLink, 64, "Artwork ID");

        if (!AllowedCardTypes.Contains(card.CardType))
        {
            throw new ArgumentException("Select a valid card type.");
        }

        if (!AllowedFactions.Contains(card.Fraction))
        {
            throw new ArgumentException("Select a valid faction.");
        }

        if (!AllowedGroups.Contains(card.Group))
        {
            throw new ArgumentException("Select a valid rarity.");
        }

        if (card.cardInfo.Strength < -999 || card.cardInfo.Strength > 999)
        {
            throw new ArgumentOutOfRangeException("Strength", "Strength must be between -999 and 999.");
        }

        if (card.Categories.Count > 16 || card.Categories.Any(value => value.Length > 64))
        {
            throw new ArgumentException("Too many or overly long card tags.");
        }

        if (card.imageLink.Any(character =>
            !char.IsLetterOrDigit(character) && character != '-' && character != '_'))
        {
            throw new ArgumentException("Artwork ID contains unsupported characters.");
        }

        var artworkPath = Path.Combine(_webRootPath, "scale", card.imageLink + ".png");
        if (!File.Exists(artworkPath))
        {
            throw new ArgumentException("Artwork ID does not exist in the web preview catalog.");
        }
    }

    private static void RequireValue(string value, string fieldName)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException(fieldName + " is required.");
        }
    }

    private static void ValidateLength(string value, int maximumLength, string fieldName)
    {
        if ((value ?? string.Empty).Length > maximumLength)
        {
            throw new ArgumentException(fieldName + " is too long.");
        }
    }

    private static IMongoCollection<T> RequireCollection<T>(IMongoCollection<T> collection, string name)
    {
        if (collection == null)
        {
            throw new InvalidOperationException($"MongoDB collection '{name}' has not been initialized.");
        }

        return collection;
    }

    private static void EnsureMatched(UpdateResult result, string operation)
    {
        if (result == null || result.MatchedCount == 0)
        {
            throw new InvalidOperationException($"Unable to update {operation}: the card no longer exists.");
        }
    }
}
