using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Bson;
using MultipleDataGenerator.Services;
using Newtonsoft.Json;
using System.Net;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Text.RegularExpressions;

namespace MultipleDataGenerator.Controllers
{
    public class GeneratorController : BaseController
    {
        private readonly DataGeneratorService _dataGeneratorService;

        public GeneratorController(DataGeneratorService dataGeneratorService) 
        {
            _dataGeneratorService = dataGeneratorService;
        }

        public IActionResult Preset() 
        {
            return View();
        }

        [HttpGet]
        public async Task<ActionResult> GetAll([FromQuery] string names, [FromQuery] string types, [FromQuery] string format, [FromQuery] string rowsCount)
        {
            var fieldNames = names.Split(',').ToList();
            var fieldTypes = types.Split(',').ToList();

            ValidationResponse validationResponse = ValidateInputData(fieldNames, fieldTypes, format, rowsCount);

            if (!validationResponse.Success)
            {
                return BadRequest(validationResponse.Message);
            }

            var data = await _dataGeneratorService.GetAsync(fieldNames, fieldTypes, Int16.Parse(rowsCount));

            //  TODO: If data null "Return message for user about nulleble data."
            var result = data.ConvertAll(BsonTypeMapper.MapToDotNetValue);

            var jsonResult = JsonConvert.SerializeObject(result, Formatting.Indented);

            switch (format)
            {
                case "CSV":
                    return CsvDownload(jsonResult);
                case "Excel":
                    return XlsxDownload(jsonResult);
                case "JSON":
                    return JsonDownload(jsonResult);
                default:
                    return CsvDownload(jsonResult);
            }
        }
    }
}
