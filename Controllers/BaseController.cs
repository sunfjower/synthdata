using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;
using System.Dynamic;
using OfficeOpenXml.LoadFunctions.Params;
using Newtonsoft.Json;
using System.Text;

namespace MultipleDataGenerator.Controllers
{
    public class BaseController : Controller
    {
        protected List<object> ShuffleList(List<object> listToShuffle)
        {
            //https://code-maze.com/csharp-randomize-list/

            var _rand = new Random();
            var shuffledList = listToShuffle.OrderBy(_ => _rand.Next()).ToList();
            return shuffledList;
        }

        [HttpGet]
        protected ActionResult XlsxDownload(string result) 
        {
            var jsonItems = JsonConvert.DeserializeObject<IEnumerable<ExpandoObject>>(result);

            using (var package = new ExcelPackage())
            {
                var workSheet = package.Workbook.Worksheets.Add("Table1");

                workSheet.Cells.LoadFromCollection(jsonItems, c =>
                {
                    c.PrintHeaders = true;
                    c.HeaderParsingType = HeaderParsingTypes.UnderscoreToSpace;
                });

                var excelData = package.GetAsByteArray();
                var mimeType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                var fileName = "synthdata.xlsx";

                return File(excelData, mimeType, fileName);
            }
        }

        [HttpGet]
        protected ActionResult JsonDownload(string result) 
        {
            var fileName = "synthdata.json";
            var mimeType = "text/plain";
            var jsonData = Encoding.ASCII.GetBytes(result);
            
            return File(jsonData, mimeType, fileName);
        }

        [HttpGet]
        protected ActionResult CsvDownload(string result) 
        {
            var jsonItems = JsonConvert.DeserializeObject<IEnumerable<ExpandoObject>>(result);

            using (var package = new ExcelPackage())
            {
                var workSheet = package.Workbook.Worksheets.Add("Table1");

                workSheet.Cells.LoadFromCollection(jsonItems, c =>
                {
                    c.PrintHeaders = true;
                    c.HeaderParsingType = HeaderParsingTypes.UnderscoreToSpace;
                });

                var content = workSheet.Cells["A1:J10"].ToText();

                var csvData = Encoding.ASCII.GetBytes(content);

                var fileName = "synthdata.csv";
                var mimeType = "text/csv";

                return File(csvData, mimeType,fileName);
            }
        }
    }
}
