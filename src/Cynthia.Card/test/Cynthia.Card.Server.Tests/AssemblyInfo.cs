using Xunit;

// Several architecture and migration tests temporarily replace entries in the
// process-wide GwentMap. Keep the suite deterministic instead of allowing an
// unrelated serializer or card-pool assertion to observe an in-flight fixture.
[assembly: CollectionBehavior(DisableTestParallelization = true)]
