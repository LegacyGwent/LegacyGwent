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
            var excelFilePath = "E:\\昆特效果\\中立效果.xlsx";
            var package = new ExcelPackage(new FileInfo(excelFilePath));

            var tableReader = ExcelTableReader.ReadContiguousTableWithHeader(package.Workbook.Worksheets[1], 1);

            var pocoReader = new TableMappingReader<ExcelCard>();
            //.Map(o => o.Name)
            //.Map(o => o.Value);

            IList<ExcelCard> pocoList = pocoReader.Read(tableReader);
            Console.ReadLine();
        }
        class ExcelCard
        {

        }
    }
}
