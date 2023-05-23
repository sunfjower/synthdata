using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace MultipleDataGenerator.Models
{
    [BindProperties]
    public class Field
    {
        public string Name { get; set; } = null!;

        public string Type { get; set; } = null!;
    }
}
