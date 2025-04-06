namespace Mongo.Options
{
    /// <summary>
    ///     Represents the configuration settings for MongoDB.
    /// </summary>
    public class MongoDbOptions
    {
        /// <summary>
        ///     The connection string for the MongoDB server.
        /// </summary>
        public string ConnectionString =>
            $"mongodb://{(string.IsNullOrEmpty(Username) ? "" : $"{Username}:{Password}@")}{Host}:{Port}";

        /// <summary>
        ///     The host address of the MongoDB server.
        /// </summary>
        public string Host { get; set; } = "localhost";

        /// <summary>
        ///     The port number of the MongoDB server.
        /// </summary>
        public int Port { get; set; } = 27017;

        /// <summary>
        ///     The username for MongoDB authentication. Optional.
        /// </summary>
        public string? Username { get; set; }

        /// <summary>
        ///     The password for MongoDB authentication. Optional.
        /// </summary>
        public string? Password { get; set; }

        /// <summary>
        ///     The name of the MongoDB database.
        /// </summary>
        public string? DatabaseName { get; set; }
    }
}