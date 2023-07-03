using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Bson;
using MongoDB.Driver.Linq;
using MongoDB.Driver;
using MultipleDataGenerator.Models;
using System.Security.Cryptography;
using System.Text.Json.Nodes;
using System.Text.RegularExpressions;
using System.Collections;

namespace MultipleDataGenerator.Services
{
    public class DataGeneratorService
    {
        private readonly IMongoCollection<BsonDocument> _fieldsCollection;

        public DataGeneratorService(IOptions<DataGeneratorDatabaseSettings> dataGeneratorDatabaseSettings) 
        {
            var mongoClient = new MongoClient(dataGeneratorDatabaseSettings.Value.ConnectionString);
            var mongoDatabase = mongoClient.GetDatabase(dataGeneratorDatabaseSettings.Value.DatabaseName);
            _fieldsCollection = mongoDatabase.GetCollection<BsonDocument>(dataGeneratorDatabaseSettings.Value.EnglishCollectionName);
        }

        public async Task<List<BsonDocument>?> GetAsync(List<string> fieldNames, List<string> fieldTypes, int totalRows)
        {
            //  TODO: If input data null => return nothing.

            var filter = Builders<BsonDocument>.Filter.Empty;

            string projection = CreateProjection(fieldNames, fieldTypes);

            int idTypePosition = fieldTypes.IndexOf("ID");

            var result = await _fieldsCollection.Find(filter).Project(projection).Limit(totalRows).ToListAsync();

            result = ShuffleList(result);

            if (idTypePosition >= 0) 
            {
                for (int i = 0; i < result.Count; i++)
                {
                    int id = i;
                    result[i].InsertAt(idTypePosition, new BsonElement(fieldNames[idTypePosition], (++id).ToString()));
                }
            }

            return result;
        }

        private string CreateProjection(List<string> fieldNames, List<string> fieldTypes)
        {
            var namesAndTypes = fieldNames.Zip(fieldTypes, (n, t) => new { Name = n, Type = t });

            string projection = "{_id:0";

            foreach (var field in namesAndTypes)
            {
                if (field.Type != "ID")
                {
                    projection += $", {field.Name}:'${field.Type}'";
                }
            }

            projection += "}";

            //string customProjection = "{_id:0, UserName:'$Name'}";

            return projection;
        }

        private List<BsonDocument> ShuffleList(List<BsonDocument> listToShuffle)
        {
            //https://code-maze.com/csharp-randomize-list/

            var _rand = new Random();
            var shuffledList = listToShuffle.OrderBy(_ => _rand.Next()).ToList();
            return shuffledList;
        }
    }
}
