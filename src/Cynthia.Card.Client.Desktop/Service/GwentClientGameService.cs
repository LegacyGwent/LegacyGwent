using System;
using System.Threading.Tasks;
using Alsein.Utilities.LifetimeAnnotations;

namespace Cynthia.Card.Client
{
    [Transient]
    public class GwentClientGameService
    {
        private LocalPlayer _player;
        public async Task Play(LocalPlayer player)
        {
            _player = player;
            Console.WriteLine("开始游戏");
            var op1 = await _player.ReceiveFromDownstreamAsync();
            Console.WriteLine("已接收到第一个指令");
            var op2 = await _player.ReceiveFromDownstreamAsync();
            Console.WriteLine("已接收到第二个指令");
            var op3 = await _player.ReceiveFromDownstreamAsync();
            Console.WriteLine("已接收到第三个指令");
        }
    }
}