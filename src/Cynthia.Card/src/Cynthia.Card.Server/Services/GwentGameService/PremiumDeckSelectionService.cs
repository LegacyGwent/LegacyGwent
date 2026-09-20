using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

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
                                throw new InvalidOperationException("Deck appearance reconciliation was rejected.");
                        }
                        else if (!await _database.SaveDeckSelection(write.Username, write.DeckId, write.Selection))
                            throw new InvalidOperationException("Deck appearance write was rejected.");
                        break;
                    }
                    catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested) { return; }
                    catch (Exception error)
                    {
                        _logger.LogError(error, "Deck appearance write will retry. User={Username}, Deck={DeckId}",
                            write.Username, write.DeckId);
                        await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
                    }
                }
            }
        }

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
