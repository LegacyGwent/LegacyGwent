using System;
using System.Collections.Generic;
using System.IO;
using ExcelEi;
using ExcelEi.Read;
using OfficeOpenXml;
using System.Linq;

namespace Cynthia.Card.MakeCard
{
    class Program
    {
        static void Main(string[] args)
        {
            IList<StringCard> AllStringCardList = GetSCardList("e:\\昆特效果\\中立效果.xlsx")
            .Concat(GetSCardList("e:\\昆特效果\\怪物效果.xlsx"))
            .Concat(GetSCardList("e:\\昆特效果\\帝国效果.xlsx"))
            //.Concat(GetSCardList("e:\\昆特效果\\北方效果.xlsx"))
            //.Concat(GetSCardList("e:\\昆特效果\\松鼠效果.xlsx"))
            //.Concat(GetSCardList("e:\\昆特效果\\群岛效果.xlsx"))
            .ToList();
            var result = AllStringCardList.Select(x=>(Show:GetCardPut(x),Card:x));
            foreach(var item in result)
            {
                if(!item.Card.效果.Contains("没有特殊能力。"))
                    File.WriteAllText
                    (
                        $"e:\\昆特效果\\CardEffect\\{FactionD[item.Card.阵营]}\\{(item.Card.品质.Contains("衍生")?"Derive":GroupD[item.Card.品质])}\\{ToName(item.Card.英文名)}.cs",
                        ("using System.Linq;\n"+
                        "using System.Threading.Tasks;\n"+
                        "using Alsein.Utilities;\n\n"+
                        "namespace Cynthia.Card\n"+
                        "{\n"+
                        "\t[CardEffectId(\""+item.Card.效果Id+"\")]//"+item.Card.中文名+"\n"+
                        "\tpublic class "+ToName(item.Card.英文名)+" : CardEffect\n"+
                        "\t{//"+item.Card.效果+"\n"+
                        "\t\tpublic "+ToName(item.Card.英文名)+"(IGwentServerGame game, GameCard card) : base(game, card){}\n"+
                        (item.Card.站位区.Contains("任意")?("\t\tpublic override async Task<int> CardPlayEffect(bool isSpying)\n"+
                        "\t\t{\n"+
                        "\t\t\treturn 0;\n"+
                        "\t\t}\n"):
                        ("\t\tpublic override async Task<int> CardUseEffect()\n"+
                        "\t\t{\n"+
                        "\t\t\treturn 0;\n"+
                        "\t\t}\n"))+
                        "\t}\n"+
                        "}")
                    );
                Console.WriteLine(item.Show);
            }
            Console.ReadLine();
        }
        public static string GetCardPut(StringCard Card)
        {
            return "{\n    \""+Card.效果Id+"\",//"+Card.中文名+"\n    new GwentCard()\n    {\n        CardId =\""+Card.效果Id+"\",\n        EffectType=typeof("+ToName(Card.效果.Contains("没有特殊技能。")?"NoneEffect":Card.英文名)+"),//效果\n        Name=\""+Card.中文名+"\",\n        Strength="+Card.战力+",\n        Group=Group."+GroupD[Card.品质]+",\n        Faction = Faction."+FactionD[Card.阵营]+",\n        CardUseInfo = CardUseInfo."+(GetUseInfo(Card.卡牌介绍,Card.站位区))+",\n        CardType = CardType."+CardTypeD[Card.站位区]+",\n        IsDoomed = "+(Card.属性.Contains("退场")||Card.属性.Contains("佚亡")).ToString().ToLower()+",\n        IsCountdown = "+"false"+",\n        IsDerive = "+Card.属性.Contains("退场").ToString().ToLower()+",\n        Categories = new Categorie[]{},//需要添加\n        Flavor = \""+Card.卡牌介绍+"\",\n        Info = \""+Card.效果+"\",\n        CardArtsId = \""+Card.图片Id+"\",\n    }\n},";
        }
        public static IList<StringCard> GetSCardList(string excelFilePath)
        {
            var package = new ExcelPackage(new FileInfo(excelFilePath));
            var tableReader = ExcelTableReader.ReadContiguousTableWithHeader(package.Workbook.Worksheets[1], 1);
            var pocoReader = new TableMappingReader<StringCard>()
                .Map(o => o.图片Id)
                .Map(o => o.中文名)
                .Map(o => o.效果Id)
                .Map(o => o.英文名)
                .Map(o => o.战力)
                .Map(o => o.品质)
                .Map(o => o.阵营)
                .Map(o => o.站位区)
                .Map(o => o.属性)
                .Map(o => o.卡牌介绍)
                .Map(o => o.效果)
                .Map(o => o.稀有度)
                .Map(o => o.排序Id);
            return pocoReader.Read(tableReader);
        }
        public static string ToName(string source)
        {
            var result = "";
            var isUp = true;
            foreach(char item in source)
            {
                if(item>='a'&&item<='z')
                {
                    if(isUp)
                        result+=item.ToString().ToUpper();
                    else
                        result+=item;
                    isUp=false;
                }
                else if(item>='A'&&item<='Z')
                {
                    result+=item;
                    isUp=false;
                }
                else
                {
                    isUp = true;
                }
            }
            return result;
        }
        public static IDictionary<string,string> GroupD = new Dictionary<string,string>()
        {
            {"铜色","Copper"},
            {"银色","Silver"},
            {"金色","Gold"},
            {"领袖","Leader"},
            {"衍生铜","Copper"},
            {"衍生银","Silver"},
            {"衍生金","Gold"},
            {"衍生领袖","Leader"},
        };
        public static IDictionary<string,string> FactionD = new Dictionary<string,string>()
        {
            {"中立","Neutral"},
            {"怪兽","Monsters"},
            {"尼弗迦德","Nilfgaard"},
            {"北方领域","NorthernRealms"},
            {"松鼠党","ScoiaTael"},
            {"史凯利格","Skellige"}
        };
        public static IDictionary<string,string> CardTypeD = new Dictionary<string,string>()
        {
            {"任意","Unit"},
            {"事件","Special"}
        };
        public static string GetUseInfo(string source,string CardType)
        {
            if(CardType.Contains("任意"))
            {//单位
                if(source.Contains("间谍。"))
                {
                    return "EnemyRow";
                }
                return "MyRow";
            }
            else
            {//事件
                if(source.Contains("己")||source.Contains("友"))
                {
                    return "MyRow";
                }
                else if(source.Contains("对方")||source.Contains("敌"))
                {
                    return "EnemyRow";
                }
                return "AnyRow";
            }
        }
        public class StringCard
        {
            public string 图片Id{get;set;}
            public string 中文名{get;set;}
            public string 效果Id{get;set;}
            public string 英文名{get;set;}
            public string 战力{get;set;}
            public string 品质{get;set;}
            public string 阵营{get;set;}
            public string 站位区{get;set;}
            public string 属性{get;set;}
            public string 卡牌介绍{get;set;}
            public string 效果{get;set;}
            public string 稀有度{get;set;}
            public string 排序Id{get;set;}
        }
    }
}
