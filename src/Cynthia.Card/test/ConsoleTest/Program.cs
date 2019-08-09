using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;
using Cynthia.Card;
using Cynthia.Card.Server;

namespace ConsoleTest
{
    class Program
    {
        static async Task Main(string[] args)
        {
            await Task.CompletedTask;
            IDictionary<string, Type> _idDictionary = new ConcurrentDictionary<string, Type>();
            var assembly = typeof(CardEffect).Assembly;
            var cardEffects = assembly.GetTypes().Where(x => x.GetCustomAttributes(true).Any(a => a.GetType() == typeof(CardEffectIdAttribute)));
            foreach (var cardEffect in cardEffects)
            {
                foreach (CardEffectIdAttribute cardId in cardEffect.GetCustomAttributes(typeof(CardEffectIdAttribute), true))
                {
                    if (_idDictionary.ContainsKey(cardId.Id)) continue;
                    _idDictionary.Add(cardId.Id, cardEffect);
                }
            }
            Console.WriteLine($"程序集扫描完成,总共扫描到{_idDictionary.Count}个卡牌效果");
            foreach (var item in _idDictionary)
            {
                Console.WriteLine($"Id:{item.Key}, Value:{item.Value}");
            }
            Console.ReadLine();
        }
    }
}
