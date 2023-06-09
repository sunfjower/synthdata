using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Bson;
using MultipleDataGenerator.Services;
using Newtonsoft.Json;
using System.Reflection.Metadata.Ecma335;

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
        public async Task<ActionResult> GetAll(string names, string types, string format)
        {
            var fieldNames = names.Split(',').ToList();
            var fieldTypes = types.Split(',').ToList();

            ValidationResponse validationResponse = ValidateInputData(fieldNames, fieldTypes, format);

            if (!validationResponse.Success)
            {
               return BadRequest(new { message = validationResponse.Message });
            }

            //  TODO: Reprocess funtion

            for (int i = 0; i < fieldNames.Count && i < fieldTypes.Count; i++)
            {
                fieldNames[i] = fieldNames[i].Replace(" ", "_");
                fieldTypes[i] = fieldTypes[i].Replace(" ", "");
            }

            var data = await _dataGeneratorService.GetAsync(fieldNames, fieldTypes);

            //  TODO: If data null "Return message for user about nulleble data."
            var result = data.ConvertAll(BsonTypeMapper.MapToDotNetValue);

            result = ShuffleList(result);

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
