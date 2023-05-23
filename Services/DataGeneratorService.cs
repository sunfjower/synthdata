using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Bson;
using MongoDB.Driver.Linq;
using MongoDB.Driver;
using MultipleDataGenerator.Models;
using System.Security.Cryptography;
using System.Text.Json.Nodes;
using System.Text.RegularExpressions;

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
        
        public async Task<List<BsonDocument>> GetAsync(string firstParam, string secondParam) 
        {
            var filter = Builders<BsonDocument>.Filter.Empty;
            var projection = Builders<BsonDocument>.Projection.Include(firstParam).Include(secondParam).Include(secondParam).Exclude("_id");

            return await _fieldsCollection.Find(filter).Project(projection).ToListAsync();

            //return await _fieldsCollection.Find<BsonDocument>(filter).ToListAsync();
            //return await _fieldsCollection.Find(FilterDefinition<BsonDocument>.Empty).Project(Builders<BsonDocument>.Projection.Include("Name").Exclude("_id")).First().ToListAsync();
        }

        public async Task<List<BsonDocument>?> GetAsync(List<string> fieldNames, List<string> fieldTypes)
        {
            //  TODO: If input data null => return nothing.

            /*if (fieldNames.IsNullOrEmpty() || fieldTypes.IsNullOrEmpty()) 
            {
                return null;
            
            }*/

            var filter = Builders<BsonDocument>.Filter.Empty;

            //  TODO: Make separate method for this functionality.
            //  START=>
            var namesAndTypes = fieldNames.Zip(fieldTypes, (n, t) => new { Name = n, Type = t });

            string projection = "{_id:0";

            foreach (var field in namesAndTypes)
            {
                projection += $", {field.Name}:'${field.Type}'"; 
            }

            projection += "}";
            //  END

            //string customProjection = "{_id:0, UserName:'$Name'}";

            var result = await _fieldsCollection.Find(filter).Project(projection).ToListAsync();

            return result;
        }
    }
}
