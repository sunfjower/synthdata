namespace MultipleDataGenerator.Models
{
    public class DataGeneratorDatabaseSettings : IDataGeneratorDatabaseSettings
    {
        public string DatabaseName { get; set; } = null!;

        public string ConnectionString { get; set; } = null!;

        public string EnglishCollectionName { get; set; } = "EnglishData";
    }
}
