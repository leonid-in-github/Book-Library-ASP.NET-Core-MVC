using BookLibrary.Storage.Contexts;
using System.Threading.Tasks;

namespace BookLibrary.Storage.Repositories
{
    public class BookLibraryRepository : IDataStorage, IDataStorageCreatable
    {
        public BookLibraryRepository(AccountRepository accountRepository, BooksRepository booksRepository)
        {
            Account = accountRepository;
            Books = booksRepository;
        }
        public AccountRepository Account { get; private set; }

        public BooksRepository Books { get; private set; }

        public async Task<bool> EnsureCreated()
        {
            using var dbCtx = new BookLibraryContext();

            return await Task.FromResult(dbCtx.Database.EnsureCreated());
        }
    }
}
