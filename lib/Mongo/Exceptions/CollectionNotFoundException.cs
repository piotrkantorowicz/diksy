namespace Mongo.Exceptions
{
    public sealed class CollectionNotFoundException : Exception
    {
        public CollectionNotFoundException(string collectionName) : base(message:
            $"Collection '{collectionName}' not found.")
        {
        }

        public CollectionNotFoundException(string collectionName, Exception innerException) : base(
            message: $"Collection '{collectionName}' not found.",
            innerException: innerException)
        {
        }
    }
}