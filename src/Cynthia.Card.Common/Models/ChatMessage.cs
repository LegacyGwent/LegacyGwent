using System;
using Cynthia.Card.Common;

namespace Cynthia.Card.Common
{
    public class ChatMessage : ModelBase
    {
        public string Name { get; set; }
        public DateTime Time { get; set; }
        public string Content { get; set; }
    }
}