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
        public string ConnectionString
        {
            get
            {
                string optionsString = Options.Any()
                    ? "?" + string.Join("&", Options.Select(kvp => $"{kvp.Key}={kvp.Value}"))
                    : string.Empty;

                string prefix = string.IsNullOrEmpty(HostSuffix) ? "mongodb" : "mongodb+";
                string credentials = string.IsNullOrEmpty(Username) ? string.Empty : $"{Username}:{Password}@";

                return $"{prefix}{HostSuffix}://{credentials}{Host}{optionsString}";
            }
        }

        /// <summary>
        ///     The host address of the MongoDB server.
        /// </summary>
        public string Host { get; set; } = "localhost:27017";

        /// <summary>
        ///     The host suffix for the MongoDB server. Optional.
        /// </summary>
        public string? HostSuffix { get; set; }

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
        public IList<string> Databases { get; set; } = [];

        /// <summary>
        /// Additional options for MongoDB connection.
        /// </summary>
        public IDictionary<string, string> Options { get; set; } = new Dictionary<string, string>();
    }
}