using System.Threading.Tasks;
using Cynthia.Card.AI;
using Cynthia.Card.Server;
using System;
using Alsein.Extensions;

namespace ConsoleTest
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var game = new GwentServerGame(new GeraltNovaAI(), new ReaverHunterAI());

            Console.WriteLine("游戏开始~请稍等");
            var gameResult = await game.Play();

            Console.WriteLine("游戏结束,比分如下:\n");
            Console.WriteLine($"{gameResult.BluePlayerName}:\t {gameResult.BlueScore.Join(",")}");
            Console.WriteLine($"{gameResult.RedPlayerName}:\t {gameResult.RedScore.Join(",")}");
            Console.WriteLine($"胜利者为:{(gameResult.BlueWinCount > gameResult.BlueWinCount ? gameResult.BluePlayerName : gameResult.RedPlayerName)}");

            Console.WriteLine("\n\n按任意键退出游戏");
            Console.ReadLine();
        }
    }
}
