using System.Threading;

namespace Cynthia.Card
{
    /// <summary>
    /// Thread-safe ordering gate shared with the Unity client. A slower older
    /// server response can never replace a projection requested later.
    /// </summary>
    public sealed class DeckBuildingProjectionRevisionGate
    {
        private long _latestSubmitted;
        private long _latestAccepted;

        public long LatestSubmitted => Interlocked.Read(ref _latestSubmitted);
        public long LatestAccepted => Interlocked.Read(ref _latestAccepted);

        public long BeginRequest()
            => Interlocked.Increment(ref _latestSubmitted);

        public bool IsLatest(long revision)
            => revision == LatestSubmitted;

        public bool TryAccept(long requestedRevision, DeckBuildingProjection response)
        {
            if (response == null || response.Revision != requestedRevision || requestedRevision < LatestSubmitted)
                return false;
            while (true)
            {
                var accepted = LatestAccepted;
                if (requestedRevision < accepted) return false;
                if (Interlocked.CompareExchange(ref _latestAccepted, requestedRevision, accepted) == accepted)
                    return true;
            }
        }
    }
}
