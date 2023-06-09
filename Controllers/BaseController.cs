﻿using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;
using System.Dynamic;
using OfficeOpenXml.LoadFunctions.Params;
using Newtonsoft.Json;
using System.Text;
using MultipleDataGenerator.Services;
using Microsoft.IdentityModel.Tokens;
using System.Linq.Expressions;
using Microsoft.SqlServer.Server;

namespace MultipleDataGenerator.Controllers
{
    public class BaseController : Controller
    {
        enum FieldType
        {
            ID,
            Name,
            Surname,
            FullName,
            Email,
            Username,
            Phone,
            CompanyName,
            Password,
        }

        enum ExportDataFormat
        {
            Excel,
            JSON,
            CSV,
        }

        protected List<object> ShuffleList(List<object> listToShuffle)
        {
            //https://code-maze.com/csharp-randomize-list/

            var _rand = new Random();
            var shuffledList = listToShuffle.OrderBy(_ => _rand.Next()).ToList();
            return shuffledList;
        }

        protected ValidationResponse ValidateInputData(List<string> fieldNames, List<string> fieldTypes, string exportFormat)
        {
            if(fieldNames.Count != fieldTypes.Count)
            {
                return new ValidationResponse("Oops! Looks like you have a mismatch between the \"Field Name\" and \"Field Type\" fields amount. " +
                    "Please refresh the page and try again", false);
            }

            // 1. Field names validation
            foreach (var fieldName in fieldNames)
            {
                if (fieldName.IsNullOrEmpty())
                {
                    return new ValidationResponse("Oops! Looks like you missed some required information. " +
                        "Please fill in all missed \"Field Name\" fields and try again.", false);
                }
                else if (fieldName.Length > 50)
                {
                    return new ValidationResponse("Oops! Looks like text you entered in one of \"Field Name\" field is too long. " +
                        "The maximum allowed character limit is 50 characters. Please revise your input and try again.", false);
                }
            }

            // 2. Field types validation
            foreach (var fieldType in fieldTypes)
            {
                if (fieldType.IsNullOrEmpty() || fieldType == "--Select--")
                {
                    return new ValidationResponse("Oops! Looks like you missed some required information. " +
                        "Please fill in all missed \"Field Type\" fields and try again.", false);
                }
                else if (!Enum.IsDefined(typeof(FieldType), fieldType))
                {
                    return new ValidationResponse("Oops! Looks like the data type provided in one of \"Field Type\" fileds is not valid for this field. " +
                        "Please make sure you are entering the correct data type and try again.", false);
                }
            }

            // 3. Export format validation
            if (exportFormat.IsNullOrEmpty()) 
            {
                return new ValidationResponse("Oops! Looks like you missed some required information. " +
                    "Please fill in \"Output Format\" field and try again.", false);
            }
            else if (!Enum.IsDefined(typeof(ExportDataFormat), exportFormat))
            {
                return new ValidationResponse("Oops! Looks like the data type provided is not valid for \"Output Format\" field. " +
                    "Please make sure you are entering the correct data type and try again.", false);
            }

            return new ValidationResponse("Ok", true);
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
