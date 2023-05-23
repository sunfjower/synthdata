namespace MultipleDataGenerator.Models
{
    public interface IDataGeneratorDatabaseSettings
    {
        public string DatabaseName { get; set; }

        public string ConnectionString { get; set; }

        public string EnglishCollectionName { get; set; }
    }
}
