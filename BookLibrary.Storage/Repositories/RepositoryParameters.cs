namespace BookLibrary.Repository.Repositories
{
    public class RepositoryParameters
    {
        public static string ConnectionString { get; set; }

        public static int SESSIONEXPIRATIONTIMEINMINUTES { get; set; }

        private RepositoryParameters() { }
    }
}
