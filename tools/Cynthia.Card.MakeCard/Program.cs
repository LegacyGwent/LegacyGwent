using System;
using System.Collections.Generic;
using System.IO;
using ExcelEi;
using ExcelEi.Read;
using OfficeOpenXml;

namespace Cynthia.Card.MakeCard
{
    class Program
    {
        static void Main(string[] args)
        {
            IList<StringCard> NeutralStringCardList = GetSCardList("e:\\昆特效果\\中立效果.xlsx");
            IList<StringCard> MonstersStringCardList = GetSCardList("e:\\昆特效果\\怪物效果.xlsx");
            IList<StringCard> NilfgaardStringCardList = GetSCardList("e:\\昆特效果\\帝国效果.xlsx");
            IList<StringCard> NorthernRealmsStringCardList = GetSCardList("e:\\昆特效果\\北方效果.xlsx");
            IList<StringCard> ScoiaTaelStringCardList = GetSCardList("e:\\昆特效果\\松鼠效果.xlsx");
            IList<StringCard> SkelligeStringCardList = GetSCardList("e:\\昆特效果\\群岛效果.xlsx");
            Console.ReadLine();
        }
        public static string GetCardPut(String Card)
        {
            return "";
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
