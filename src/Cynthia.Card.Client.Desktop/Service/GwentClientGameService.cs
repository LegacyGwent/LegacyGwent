using System.Threading.Tasks;
using Alsein.Utilities.LifetimeAnnotations;

namespace Cynthia.Card.Client
{
    [Transient]
    public class GwentClientGame
    {
        private GwentClientPlayer _player;
        public async Task Play(GwentClientPlayer player)
        {
            _player = player;
            var op1 = await _player.GetOperation();
            var op2 = await _player.GetOperation();
            var op3 = await _player.GetOperation();
        }
    }
}