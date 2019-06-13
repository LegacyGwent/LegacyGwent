using System.Collections.Generic;

namespace Cynthia.Card
{
    public class GameCardsPart
    {
        public bool IsSelectMyLeader { get; set; } = false;
        public bool IsSelectEnemyLeader { get; set; } = false;
        public IList<int> MyHandCards { get; set; } = new List<int>();
        public IList<int> EnemyHandCards { get; set; } = new List<int>();
        public IList<int> EnemyRow1Cards { get; set; } = new List<int>();
        public IList<int> EnemyRow2Cards { get; set; } = new List<int>();
        public IList<int> EnemyRow3Cards { get; set; } = new List<int>();
        public IList<int> MyRow1Cards { get; set; } = new List<int>();
        public IList<int> MyRow2Cards { get; set; } = new List<int>();
        public IList<int> MyRow3Cards { get; set; } = new List<int>();
        public IList<int> MyStayCards { get; set; } = new List<int>();
        public IList<int> EnemyStayCards { get; set; } = new List<int>();
    }
}