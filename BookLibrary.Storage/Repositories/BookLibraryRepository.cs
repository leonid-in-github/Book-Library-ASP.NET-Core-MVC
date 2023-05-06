namespace BookLibrary.Storage.Repositories
{
    public class BookLibraryRepository : IBookLibraryRepository
    {
        public BookLibraryRepository(AccountRepository accountRepository, BooksRepository booksRepository)
        {
            Account = accountRepository;
            Books = booksRepository;
        }
        public AccountRepository Account { get; private set; }

        public BooksRepository Books { get; private set; }
    }
}
