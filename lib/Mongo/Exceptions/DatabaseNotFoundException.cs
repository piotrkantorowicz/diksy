namespace Mongo.Exceptions
{
    public sealed class DatabaseNotFoundException : Exception
    {
        public DatabaseNotFoundException(string collectionName) : base(message:
            $"Database '{collectionName}' not found.")
        {
        }

        public DatabaseNotFoundException(string collectionName, Exception innerException) : base(
            message: $"Database '{collectionName}' not found.",
            innerException: innerException)
        {
        }
    }
}