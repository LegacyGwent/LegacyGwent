using System;

namespace Cynthia.Card
{
    public class ChatMessage : ModelBase
    {
        public string Name { get; set; }
        public DateTime Time { get; set; }
        public string Content { get; set; }
    }
}