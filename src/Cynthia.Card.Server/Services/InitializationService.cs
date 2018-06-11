using Alsein.Utilities.LifetimeAnnotations;

namespace Cynthia.Card.Server.Services
{
    [Singleton]
    public class InitializationService
    {
        public ChatMessageCacheService cache { get; set; }
        public void Start()
        {
            cache.InitData();
            cache.AutoSaveData();
        }
    }
}