using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;
using System.Dynamic;
using OfficeOpenXml.LoadFunctions.Params;
using Newtonsoft.Json;
using System.Text;
using MultipleDataGenerator.Services;
using Microsoft.IdentityModel.Tokens;
using System.Collections.Generic;

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

        protected ValidationResponse ValidateInputData(List<string> fieldNames, List<string> fieldTypes, string exportFormat, string rowsCount)
        {
            int totalRows;
            int idCounter = 0;
            var distinctList = fieldNames.Distinct().ToList();

            if (fieldNames.Count != fieldTypes.Count)
            {
                return new ValidationResponse("Oops! Looks like you have a mismatch between the \"Field Name\" and \"Field Type\" fields amount. " +
                    "Please refresh the page and try again", false);
            }

            if(fieldNames.Count != distinctList.Count)
            {
                return new ValidationResponse("Oops! Looks like you have duplicates in some of the \"Field Name\" fields. " +
                    "Please make sure that you have only unique names in \"Field Name\" fields and try again.", false);
            }

            //  TODO: Reprocess funtion Check unwanted characters
            for (int i = 0; i < fieldNames.Count && i < fieldTypes.Count; i++)
            {
                fieldNames[i] = fieldNames[i].Replace(" ", "_");
                fieldNames[i] = fieldNames[i].Replace("\"", "");
                fieldNames[i] = fieldNames[i].Replace("\'", "");
                fieldNames[i] = fieldNames[i].Replace("`", "");
                fieldTypes[i] = fieldTypes[i].Replace(" ", "");
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
                else if (fieldType.Contains("ID"))
                {
                    idCounter++;
                }

                if (idCounter > 1)
                {
                    return new ValidationResponse("\"Oops! Looks like you have duplicate entries for the \"Field Tepe\" fields. " +
                        "You must use \"ID\" data type as unique. Please make sure you are used \"ID\" only once and try again.", false);
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

            // 4. Total rows validation
            try
            {
                totalRows = Int16.Parse(rowsCount);
            }
            catch (FormatException e)
            {
                return new ValidationResponse($"Oops! Looks like {e.Message.ToLower()} " +
                    $"Please take into consideration that the \"Total Rows\" field should contain numeric format.", false);
            }

            if (totalRows < 1 || totalRows > 1000)
            {
                return new ValidationResponse("Oops! Looks like the \"Total Rows\" field contains an incorrect numeric range. " +
                    "Please take into consideration that the number of rows should range from 1 to 1000.", false);
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
