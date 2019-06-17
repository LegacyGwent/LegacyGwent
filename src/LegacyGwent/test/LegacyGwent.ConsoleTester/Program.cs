using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LegacyGwent.Events;

namespace LegacyGwent.ConsoleTester
{
    class BeforeDeath : Event
    {
        public bool IsCancelled { get; set; } = false;
    }

    class AfterDeath : Event
    {

    }
    abstract class Unit : IHasEffects
    {
        public abstract string Name { get; }

        public EffectSet Effects { get; }

        public int Health { get; set; }

        public Unit() => Effects = new EffectSet(this);

        public async Task GetDamaged(int damage)
        {
            Health -= damage;
            if (Health <= 0)
            {
                await Die();
            }
        }

        private async Task Die()
        {
            if ((await Effects.RaiseEvent<BeforeDeath>()).IsCancelled)
            {
                Console.WriteLine($"{Name} has escaped from death.");
                Health = 1;
            }
            else
            {
                Console.WriteLine($"{Name} is dead.");
                Health = 0;
                await Effects.RaiseEvent<AfterDeath>();
            }
        }
    }

    class Tryndamere : Unit
    {
        public override string Name => "Tryndamere";

        public Tryndamere()
        {
            Health = 10;
        }
    }

    class DisableUndyingRage : Effect, IHandlesEvent<RaisingEvent>
    {
        public Task HandleEvent(RaisingEvent @event)
        {
            if (@event.Effect is UndyingRage)
            {
                @event.IsCancelled = true;
            }

            return Task.CompletedTask;
        }
    }

    class UndyingRage : Effect, IHandlesEvent<BeforeDeath>
    {
        private const int _maxCount = 5;

        private int count = _maxCount;

        public Task HandleEvent(BeforeDeath @event)
        {
            @event.IsCancelled = true;
            count--;
            if (count <= 0)
            {
                Dispose();
            }
            return Task.CompletedTask;
        }
    }

    class Program
    {
        static async Task Main(string[] args)
        {
            // var trynd = new Tryndamere();
            // trynd.Effects.Add(new UndyingRage());
            // var disableUndyingRage = new DisableUndyingRage();
            // trynd.Effects.Add(disableUndyingRage);
            // trynd.Effects.Remove(disableUndyingRage);
            // while (trynd.Health > 0)
            // {
            //     await trynd.GetDamaged(5);
            // }
            await Task.CompletedTask;
            var test = new AsyncBufferList<int>();
            // test.Finish();
            // test.Reveice(1);
            Console.ReadLine();
        }
        static async Task Runer(Action action, int count = 10, int delayTime = 500)
        {
            for (var i = 0; i < count; i++)
            {
                await Task.Delay(delayTime);
                action();
            }
        }
        static async Task Runer(IAsyncEnumerable<int> receiver, string name)
        {
            await foreach (var item in receiver)
            {
                Console.WriteLine($"{name}接收到内容:{item}");
            }
            Console.WriteLine($"{name}监听结束");
        }
    }
}
