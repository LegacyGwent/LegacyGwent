using System;
using System.Threading.Tasks;
using Alsein.Utilities.LifetimeAnnotations;

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
            var op1 = await _player.GetOperation();
            var op2 = await _player.GetOperation();
            var op3 = await _player.GetOperation();
            Console.WriteLine("已接收到三个指令");
        }
    }
}