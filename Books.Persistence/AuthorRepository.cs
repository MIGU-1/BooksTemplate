using Books.Core.Contracts;
using Books.Core.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Books.Core.DataTransferObjects;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;
using Books.Persistence.Comparer;

namespace Books.Persistence
{
    public class AuthorRepository : IAuthorRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public AuthorRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Author[]> GetAllAsync()
            => await _dbContext
            .Authors
            .OrderBy(a => a.Name)
            .ToArrayAsync();
    }
}