using Books.Core.Contracts;
using Books.Core.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Books.Persistence
{
    public class BookRepository : IBookRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public BookRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task AddRangeAsync(IEnumerable<Book> books)
                => await _dbContext.AddRangeAsync(books);

        public async Task<Book[]> GetAllAsync()
            => await _dbContext
            .Books
            .OrderBy(b => b.Title)
            .Include(b => b.BookAuthors)
            .ThenInclude(ba => ba.Author)
            .ToArrayAsync();

        public void Remove(Book selectedBook)
            => _dbContext.Books
            .Remove(selectedBook);
    }

}