using Books.Core.Contracts;
using Books.Core.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Books.Core.DataTransferObjects;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;
using Books.Persistence.Comparer;
using System;

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

        public async Task<AuthorDto> GetAuthorDtoByIdAsync(int value)
        {
            var author = await _dbContext
                .Authors
                .Where(a => a.Id == value)
                .Include(a => a.BookAuthors)
                .ThenInclude(a => a.Book)
                .SingleOrDefaultAsync();

            AuthorDto newDto = new AuthorDto
            {
                Id = author.Id,
                Author = author.Name,
                BookCount = author.BookAuthors.Count,
                Books = author.BookAuthors.Select(ba => ba.Book)
            };

            var publisherNames = author.BookAuthors
                .GroupBy(ba => ba.Book.Publishers)
                .Select(grp => Tuple.Create(
                    grp.Key,
                    grp.Count()));

            foreach (var tuple in publisherNames)
            {
                if (newDto.Publishers == null)
                {
                    newDto.Publishers = $"{tuple.Item1} ({tuple.Item2})";
                }
                else
                {
                    newDto.Publishers += $" / {tuple.Item1} ({tuple.Item2})";
                }
            }

            return newDto;
        }

        public async Task<AuthorDto[]> GetAuthorDtosAsync()
        {
            List<AuthorDto> dtos = new List<AuthorDto>();

            var authors = await _dbContext
                    .Authors
                    .Include(a => a.BookAuthors)
                    .ThenInclude(a => a.Book)
                    .OrderBy(a => a.Name)
                    .ToArrayAsync();

            foreach (var author in authors)
            {
                AuthorDto newDto = new AuthorDto
                {
                    Id = author.Id,
                    Author = author.Name,
                    BookCount = author.BookAuthors.Count,
                    Books = author.BookAuthors.Select(ba => ba.Book)
                };

                var publisherNames = author.BookAuthors
                    .GroupBy(ba => ba.Book.Publishers)
                    .Select(grp => Tuple.Create(
                        grp.Key,
                        grp.Count()));

                foreach (var tuple in publisherNames)
                {
                    if (newDto.Publishers == null)
                    {
                        newDto.Publishers = $"{tuple.Item1} ({tuple.Item2})";
                    }
                    else
                    {
                        newDto.Publishers += $" / {tuple.Item1} ({tuple.Item2})";
                    }
                }

                dtos.Add(newDto);
            }

            return dtos.OrderByDescending(dto => dto.BookCount).ToArray();
        }
    }
}