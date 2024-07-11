namespace DataAccess_Layer.DTOs
{
    public class MongoDBSetting : IMongoDbSetting
    {
        public string ConnectionString { get; set; }
        public string DatabaseName { get; set; }
        public string CollectionName { get; set; }
    }
    public interface IMongoDbSetting
    {
        string ConnectionString { get; set; }
        string DatabaseName { get; set; }
        string CollectionName { get; set; }
    }
}
