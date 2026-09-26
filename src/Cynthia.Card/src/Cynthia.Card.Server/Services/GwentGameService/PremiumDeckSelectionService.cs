using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;

namespace Cynthia.Card.Server
{
    public sealed class PremiumDeckSelectionService : BackgroundService
    {
        private readonly GwentDatabaseService _database;
        private readonly ILogger<PremiumDeckSelectionService> _logger;
        private readonly Channel<DeckSelectionWrite> _writes = Channel.CreateUnbounded<DeckSelectionWrite>(
            new UnboundedChannelOptions { SingleReader = true, SingleWriter = false });

        public PremiumDeckSelectionService(GwentDatabaseService database, ILogger<PremiumDeckSelectionService> logger)
        {
            _database = database;
            _logger = logger;
        }

        public bool QueueSave(string username, DeckModel deck) =>
            _writes.Writer.TryWrite(new DeckSelectionWrite
            {
                Username = username,
                DeckId = deck.Id,
                Selection = new PremiumDeckSelection
                {
                    PremiumCards = new Dictionary<string, int>(deck.PremiumCards ?? new Dictionary<string, int>()),
                    LeaderId = deck.Leader,
                    PremiumLeader = deck.PremiumLeader == true
                }
            });

        public bool QueueReconcile(string username, DeckModel deck) =>
            _writes.Writer.TryWrite(new DeckSelectionWrite
            {
                Username = username,
                DeckId = deck.Id,
                Deck = new DeckModel
                {
                    Id = deck.Id,
                    Leader = deck.Leader,
                    Deck = new List<string>(deck.Deck ?? new List<string>())
                }
            });

        public bool QueueRemove(string username, string deckId) =>
            _writes.Writer.TryWrite(new DeckSelectionWrite { Username = username, DeckId = deckId, Remove = true });

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await foreach (var write in _writes.Reader.ReadAllAsync(stoppingToken))
            {
                while (!stoppingToken.IsCancellationRequested)
                {
                    try
                    {
                        if (write.Remove)
                            await _database.RemoveDeckSelection(write.Username, write.DeckId);
                        else if (write.Deck != null)
                        {
                            if (!await _database.ReconcileDeckSelection(write.Username, write.Deck))
                                _logger.LogWarning("Rejected deck appearance reconciliation. User={Username}, Deck={DeckId}", write.Username, write.DeckId);
                        }
                        else if (!await _database.SaveDeckSelection(write.Username, write.DeckId, write.Selection))
                            _logger.LogWarning("Rejected deck appearance write. User={Username}, Deck={DeckId}", write.Username, write.DeckId);
                        break;
                    }
                    catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested) { return; }
                    catch (MongoWriteException error) when (IsPermanentWriteError(error.WriteError.Code))
                    {
                        _logger.LogError(error, "Invalid deck appearance write was discarded. User={Username}, Deck={DeckId}", write.Username, write.DeckId);
                        break;
                    }
                    catch (MongoCommandException error) when (IsPermanentWriteError(error.Code))
                    {
                        _logger.LogError(error, "Invalid deck appearance command was discarded. User={Username}, Deck={DeckId}", write.Username, write.DeckId);
                        break;
                    }
                    catch (Exception error)
                    {
                        _logger.LogError(error, "Deck appearance write will retry. User={Username}, Deck={DeckId}",
                            write.Username, write.DeckId);
                        await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
                    }
                }
            }
        }

        // Retrying an immutable invalid/oversized document cannot repair it. Connection,
        // availability and write-conflict errors still take the existing retry path.
        public static bool IsPermanentWriteError(int code) =>
            code == 2 || code == 9 || code == 14 || code == 10334 || code == 17419;

        private sealed class DeckSelectionWrite
        {
            public string Username { get; set; }
            public string DeckId { get; set; }
            public PremiumDeckSelection Selection { get; set; }
            public DeckModel Deck { get; set; }
            public bool Remove { get; set; }
        }
    }
}
