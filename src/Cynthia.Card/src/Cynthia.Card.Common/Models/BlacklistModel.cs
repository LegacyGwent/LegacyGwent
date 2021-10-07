using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Cynthia.Card
{
    public class BlacklistModel : ModelBase
    {
        public string Name { get; set; } = "";
        public List<string> Blacklist { get; set; } = new List<string>();
    }
}