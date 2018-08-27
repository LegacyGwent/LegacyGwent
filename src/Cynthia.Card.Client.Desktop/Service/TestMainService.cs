using System;
using System.Threading.Tasks;
using Autofac;
using Microsoft.AspNetCore.SignalR.Client;
using Alsein.Utilities.LifetimeAnnotations;
using Alsein.Utilities;
using System.Collections.Generic;

namespace Cynthia.Card.Client
{
    [Singleton]
    public class TestMainService
    {
        public void Main(string[] args)
        {
            int index = -1;
            IList<ICommand> operations = new List<ICommand>();
            Player testPlayer = new Player();

            Console.WriteLine("\n   输入1:新操作\n   输入2:撤回操作\n   输入3:复原操作\n   输入4:清理屏幕\n");
            while (true)
            {
                switch (Console.ReadLine())
                {
                    case "1":
                        Console.WriteLine("设置玩家的金钱为:");
                        var newOperation = new PlayerSetMoneyCommand(testPlayer, int.Parse(Console.ReadLine()));
                        newOperation.execute();
                        RemoveFrom(operations, index + 1);
                        index++;
                        operations.Add(newOperation);
                        break;
                    case "2":
                        if (index < 0)
                        {
                            Console.WriteLine("已经撤回到了第一步了");
                            break;
                        }
                        operations[index].undo();
                        index--;
                        break;
                    case "3":
                        if (index + 1 >= operations.Count)
                        {
                            Console.WriteLine("已经是最新操作啦");
                            break;
                        }
                        index++;
                        operations[index].execute();
                        break;
                    case "4":
                        Console.Clear();
                        Console.WriteLine("\n   输入1:新操作\n   输入2:撤回操作\n   输入3:复原操作\n   输入4:清理屏幕\n");
                        break;
                    default:
                        Console.WriteLine("未能识别");
                        break;
                }
                Console.WriteLine($"目前玩家的金钱为:{testPlayer.Money}");
            }
        }
        public IList<T> RemoveFrom<T>(IList<T> list, int index)
        {
            if (index < 0)
                index = 0;
            if (index > list.Count)
                return list;
            for (var i = list.Count - 1; i > index; i--)
            {
                list.RemoveAt(i);
            }
            return list;
        }
    }
    public interface ICommand
    {
        void execute();
        void undo();
    }
    public class PlayerSetMoneyCommand : ICommand
    {
        private int _beforeMoney;
        private int _setMoney;
        private Player _player;
        public PlayerSetMoneyCommand(Player player, int money)
        {
            _beforeMoney = player.Money;
            _setMoney = money;
            _player = player;
        }
        public void execute()
        {
            _player.Money = _setMoney;
        }

        public void undo()
        {
            _player.Money = _beforeMoney;
        }
    }
    public class Player
    {
        public int Money { get; set; } = 0;
    }
}