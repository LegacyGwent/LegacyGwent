using System;
using System.Collections.Generic;
using Alsein.Utilities;

namespace Cynthia.Card.Client
{
    public static class MessageProcessing
    {
        public static void PrintMessage(ChatMessage msg)
        {
            Console.WriteLine($"---------------------------------------------------");
            Console.WriteLine($"{msg.Name}   {msg.Time}");
            Console.WriteLine($"{msg.Content}");
        }
        public static void PrintMessage(IEnumerable<ChatMessage> msgs)
        {
            msgs.ForAll(x => PrintMessage(x));
        }
    }
}