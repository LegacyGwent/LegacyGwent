using System;
using System.Linq;
using System.Threading.Tasks;
using Alsein.Utilities.LifetimeAnnotations;
using Cynthia.Card.Common;

namespace Cynthia.Card.Client
{
    [Transient]
    public class GwentClientGameService
    {
        private GwentClientPlayer _player;
        public async Task Play(GwentClientPlayer player)
        {
            _player = player;
            Console.WriteLine("开始游戏");
            await _player.SendToUpstreamAsync(UserOperationType.None, "233");
            var op1 = await _player.ReceiveFromUpstreamAsync();
            Console.WriteLine("已接收到第一个指令");
            var op2 = await _player.ReceiveFromUpstreamAsync();
            Console.WriteLine("已接收到第二个指令");
            var op3 = await _player.ReceiveFromUpstreamAsync();
            Console.WriteLine("已接收到第三个指令");
        }
    }
}