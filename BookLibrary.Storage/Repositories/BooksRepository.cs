﻿using BookLibrary.Storage.Contexts;
using BookLibrary.Storage.Models.Book;
using BookLibrary.Storage.Models.Records.Book;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookLibrary.Storage.Repositories
{
    public class BooksRepository : IBooksRepository
    {
        public Task<Book> GetBook(int bookId)
        {
            using var dbContext = new BookLibraryContext();
            var booksQuery = dbContext.Books.Where(book => book.ID == bookId);
            var books = SelectBooksFromBookRecords(dbContext, booksQuery);
            return Task.FromResult(books.FirstOrDefault());
        }

        public Task<List<Book>> GetBooks(
            string searchString = "",
            bool onlyAvailable = false,
            int userId = -1,
            int from = 0,
            int count = 10,
            string orderColumnName = null,
            string orderDirection = "asc")
        {
            using var dbContext = new BookLibraryContext();
            return Task.FromResult(BuildGetBooksQuery(dbContext, searchString, onlyAvailable, userId, from, count, orderColumnName, orderDirection).ToList());
        }

        public Task<List<Book>> GetAvailableBooks(
            string searchString = "",
            int from = 0,
            int count = 10,
            string orderColumnName = null,
            string orderDirection = "asc")
        {
            return GetBooks(searchString, true, -1, from, count, orderColumnName, orderDirection);
        }

        public Task<List<Book>> GetBooksByUser(
            int userId,
            string searchString = "",
            int from = 0,
            int count = 10,
            string orderColumnName = null,
            string orderDirection = "asc")
        {
            return GetBooks(searchString, false, userId, from, count, orderColumnName, orderDirection);
        }

        public Task<int> GetBooksTotalCount(string searchString = "")
        {
            using var dbContext = new BookLibraryContext();
            return Task.FromResult(BuildGetBooksQuery(dbContext, searchString).Count());
        }

        public Task<int> GetBooksByUserTotalCount(int userId, string searchString = "")
        {
            using var dbContext = new BookLibraryContext();
            return Task.FromResult(BuildGetBooksQuery(dbContext, searchString, false, userId).Count());
        }

        public Task<int> GetAvailableBooksTotalCount(string searchString = "")
        {
            using var dbContext = new BookLibraryContext();
            return Task.FromResult(BuildGetBooksQuery(dbContext, searchString, true).Count());
        }


        public Task DeleteBook(int bookId)
        {
            using var dbContext = new BookLibraryContext();
            var bookRecord = dbContext.Books.FirstOrDefault(book => book.ID == bookId);
            if (bookRecord != null)
            {
                dbContext.Books.Remove(bookRecord);
                dbContext.SaveChanges();
            }

            return Task.CompletedTask;
        }

        public Task AddBook(Book book)
        {
            using var dbContext = new BookLibraryContext();
            using var transaction = dbContext.Database.BeginTransaction();

            var bookRecord = BookRecord.FromDomain(book);
            dbContext.Books.Add(bookRecord);
            dbContext.SaveChanges();

            SaveBookAuthors(dbContext, book.Authors, bookRecord.ID);

            transaction.Commit();

            return Task.CompletedTask;
        }

        public Task UpdateBook(Book book)
        {
            using var dbContext = new BookLibraryContext();
            using var transaction = dbContext.Database.BeginTransaction();

            var bookRecord = dbContext.Books.FirstOrDefault(record => record.ID == book.ID);
            if (bookRecord is null)
            {
                AddBook(book);
                return Task.CompletedTask;
            }

            bookRecord.Name = book.Name;
            bookRecord.Year = book.Year;
            bookRecord.Availability = book.Availability ?? true;
            dbContext.BooksAuthors.RemoveRange(dbContext.BooksAuthors.Where(record => record.BookId == book.ID));
            dbContext.SaveChanges();

            SaveBookAuthors(dbContext, book.Authors, bookRecord.ID);

            transaction.Commit();

            return Task.CompletedTask;
        }

        public Task<BookTrackList> GetBookTrack(int accountId, int bookId, string tracksCount)
        {
            var result = new BookTrackList();
            using var dbContext = new BookLibraryContext();
            var bookRecord = dbContext.Books.FirstOrDefault(record => record.ID == bookId);

            result.BookId = bookRecord?.ID;
            result.BookName = bookRecord?.Name;
            result.BookAvailability = bookRecord?.Availability;

            var bookTookTracks = dbContext.BookTracking.Where(record => record.BookId == bookId && record.Action == BookAction.Took.ToString());
            var lastBookTookTrack = bookTookTracks.FirstOrDefault(record => record.ActionTime == bookTookTracks.Max(track => track.ActionTime));
            result.CanBePut = accountId == lastBookTookTrack?.AccountId;

            var accountRecord = dbContext.Accounts.FirstOrDefault(record => record.ID == accountId);
            if (accountRecord != null)
            {
                var profileRecord = dbContext.Profiles.FirstOrDefault(record => record.ID == accountRecord.ProfileId);
                if (profileRecord != null)
                {
                    var tracksQuery = dbContext.BookTracking.Where(record => record.BookId == bookId).OrderByDescending(record => record.ActionTime);
                    if (tracksCount != "All")
                    {
                        tracksQuery = tracksQuery.Take(int.Parse(tracksCount)).OrderByDescending(record => record.ActionTime);
                    }

                    var tracksQueryJoinAccounts = tracksQuery.Join(
                       dbContext.Accounts,
                       track => track.AccountId,
                       account => account.ID,
                       (track, account) => new
                       {
                           account.Login,
                           account.ProfileId,
                           track.ActionTime,
                           track.Action
                       }
                       ).OrderByDescending(record => record.ActionTime);

                    var tracksQueryJoinAccountsJoinProfiles = tracksQueryJoinAccounts.Join(
                        dbContext.Profiles,
                        track => track.ProfileId,
                        profile => profile.ID,
                        (track, profile) => new
                        {
                            track.Login,
                            profile.Email,
                            track.ActionTime,
                            track.Action
                        }
                        ).OrderByDescending(record => record.ActionTime);

                    result.TracksList = [.. tracksQueryJoinAccountsJoinProfiles.Select(bookTrack => BookTrack.FromPersistence(
                        bookId,
                        bookRecord.Name,
                        bookTrack.Login,
                        bookTrack.Email,
                        bookTrack.ActionTime,
                        bookTrack.Action
                    ))];
                }
            }

            return Task.FromResult(result);
        }

        public Task DoBookAction(BookAction action, int accountId, int bookId)
        {
            using var dbContext = new BookLibraryContext();
            using var transaction = dbContext.Database.BeginTransaction();

            var accountRecord = dbContext.Accounts.FirstOrDefault(record => record.ID == accountId);
            if (accountRecord != null)
            {
                var bookRecord = dbContext.Books.FirstOrDefault(record => record.ID == bookId);
                if (bookRecord != null)
                {
                    bookRecord.Availability = action switch
                    {
                        BookAction.Took => false,
                        BookAction.Put => true,
                        _ => throw new NotSupportedException($"Action {action} is not supported"),
                    };
                    var bookTrackingRecord = new BookTrackingRecord
                    {
                        AccountId = accountId,
                        Action = action.ToString(),
                        ActionTime = DateTime.UtcNow,
                        BookId = bookId
                    };
                    dbContext.BookTracking.Add(bookTrackingRecord);

                    dbContext.SaveChanges();
                    transaction.Commit();
                }
            }
            return Task.CompletedTask;
        }

        public Task TakeBook(int accountId, int bookId)
        {
            return DoBookAction(BookAction.Took, accountId, bookId);
        }

        public Task PutBook(int accountId, int bookId)
        {
            return DoBookAction(BookAction.Put, accountId, bookId);
        }

        #region Private

        private static IQueryable<Book> BuildGetBooksQuery(
            BookLibraryContext dbContext,
            string searchString = "",
            bool onlyAvailable = false,
            int userId = -1,
            int from = 0,
            int count = 0,
            string orderColumnName = null,
            string orderDirection = "asc")
        {
            IQueryable<BookRecord> booksQuery = dbContext.Books;
            if (userId > -1)
            {
                var bookTracks = dbContext.BookTracking
                    .Where(bookTrack => bookTrack.AccountId == userId && bookTrack.Action == BookAction.Took.ToString())
                    .GroupBy(bookTrack => bookTrack.BookId).Select(bookTrackGroup => new
                    {
                        BookId = bookTrackGroup.Key,
                        ActionTime = bookTrackGroup.Max(bookTrack => bookTrack.ActionTime)
                    }).OrderBy(bookTrack => bookTrack.ActionTime);
                booksQuery = booksQuery.Where(book => !book.Availability).Join(bookTracks, book => book.ID, bookTrack => bookTrack.BookId, (book, bookTrack) => book);
            }

            if (!string.IsNullOrEmpty(searchString))
            {
                var bookIdsByAuthor = dbContext.Authors.Where(author => author.Name.Contains(searchString)).Join(
                        dbContext.BooksAuthors,
                        author => author.ID,
                        bookAuthor => bookAuthor.AuthorId,
                        (author, bookAuthor) => bookAuthor)
                    .Select(bookAuthor => bookAuthor.BookId);
                booksQuery = booksQuery.Where(book => book.Name.Contains(searchString) || book.Year.ToString().Contains(searchString) || bookIdsByAuthor.Contains(book.ID));
            }

            if (onlyAvailable)
            {
                booksQuery = booksQuery.Where(book => book.Availability);
            }

            if (!string.IsNullOrEmpty(orderColumnName))
            {
                var isDescending = orderDirection.Equals("desc", StringComparison.OrdinalIgnoreCase);

                booksQuery = orderColumnName switch
                {
                    nameof(Book.Name) => isDescending ? booksQuery.OrderByDescending(book => book.Name) : booksQuery.OrderBy(book => book.Name),
                    nameof(Book.Year) => isDescending ? booksQuery.OrderByDescending(book => book.Year) : booksQuery.OrderBy(book => book.Year),
                    nameof(Book.Availability) => isDescending ? booksQuery.OrderByDescending(book => book.Availability)
                    : booksQuery.OrderBy(book => book.Availability),
                    _ => throw new ArgumentException("Invalid order column name")
                };

            }

            if (count > 0)
            {
                booksQuery = booksQuery.Skip(from).Take(count);
            }

            var books = SelectBooksFromBookRecords(dbContext, booksQuery);

            return books;
        }

        private static IQueryable<Book> SelectBooksFromBookRecords(BookLibraryContext dbContext, IQueryable<BookRecord> bookRecords)
        {
            var books = bookRecords.Select(book => Book.FromPersistence(
                book.ID,
                book.Name,
                dbContext.Authors.Join(
                    dbContext.BooksAuthors.Where(bookAuthor => bookAuthor.BookId == book.ID),
                    author => author.ID,
                    bookAuthor => bookAuthor.AuthorId,
                    (author, bookAuthor) => author)
                .Select(authorRecord => authorRecord.Name).ToList(),
                book.Year,
                book.Availability
            ));
            return books;
        }

        private static void SaveBookAuthors(BookLibraryContext dbContext, IEnumerable<string> authors, int bookId)
        {
            if (authors is not null)
            {
                foreach (var author in authors)
                {
                    var authorRecord = dbContext.Authors.FirstOrDefault(record => record.Name == author);
                    if (authorRecord is null)
                    {
                        dbContext.Authors.Add(new AuthorRecord { Name = author });
                        dbContext.SaveChanges();
                        authorRecord = dbContext.Authors.FirstOrDefault(record => record.Name == author);
                    }

                    dbContext.BooksAuthors.Add(new BookAuthorRecord { AuthorId = authorRecord.ID, BookId = bookId });
                    dbContext.SaveChanges();
                }
            }
        }

        #endregion
    }
}
