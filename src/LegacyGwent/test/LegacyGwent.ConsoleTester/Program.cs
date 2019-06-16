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

    abstract class Unit
    {
        public abstract string Name { get; }

        public IList<Effect> Effects { get; set; } = new List<Effect>();

        public int Health { get; set; }

        public async Task GetDamaged(int damage)
        {
            Health -= damage;
            if (Health <= 0)
            {
                var beforeDeath = new BeforeDeath();
                await Effects.RaiseEvent(beforeDeath);
                if (beforeDeath.IsCancelled)
                {
                    Console.WriteLine($"{Name} has escaped from death.");
                    Health = 1;
                }
                else
                {
                    Console.WriteLine($"{Name} is dead.");
                    await Effects.RaiseEvent<AfterDeath>();
                }
            }
        }
    }

    class Tryndamere : Unit
    {
        public override string Name => "Tryndamere";

        public Tryndamere()
        {
            Health = 10;
            Effects.Add(new UndyingRage(this));
        }
    }

    class UndyingRage : Effect, IHandlesEvent<BeforeDeath>
    {
        private const int _maxCount = 5;

        private int count = _maxCount;

        private readonly Tryndamere _target;

        public UndyingRage(Tryndamere target)
        {
            _target = target;
        }

        public Task HandleEvent(BeforeDeath @event)
        {
            if (count > 0)
            {
                @event.IsCancelled = true;
                count--;
            }
            return Task.CompletedTask;
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            var trynd = new Tryndamere();
            while (trynd.Health > 0)
            {
                _ = trynd.GetDamaged(5);
            }
        }
    }
}
