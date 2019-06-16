using System;
using System.Collections.Generic;
using System.Threading.Tasks;

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
            var trynd = new Tryndamere();
            trynd.Effects.Add(new UndyingRage());
            while (trynd.Health > 0)
            {
                await trynd.GetDamaged(5);
            }
        }
    }
}
