namespace ProductService.Settings
{
    /// <summary>
    /// Represents the MongoDB configuration settings used to establish a database connection.
    /// </summary>
    public class MongoSettings
    {
        /// <summary>
        /// Gets or sets the MongoDB connection string.
        /// This string is used to connect to the MongoDB server.
        /// </summary>
        public string ConnectionString { get; set; } = null!;

        /// <summary>
        /// Gets or sets the name of the MongoDB database to be used by the application.
        /// </summary>
        public string DatabaseName { get; set; } = null!;
    }
}
