﻿using Books.Core.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Books.Core.Contracts
{
    public interface IBookRepository
    {
        Task AddRangeAsync(IEnumerable<Book> books);
        Task<Book[]> GetAllAsync();
        void Remove(Book selectedBook);
        void Update(Book book);
    }
}