using Books.Core.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;
using Books.Core.DataTransferObjects;
using System.Collections.ObjectModel;

namespace Books.Core.Contracts
{
    public interface IAuthorRepository
    {
        Task<Author[]> GetAllAsync();
        Task<AuthorDto[]> GetAuthorDtosAsync();
        Task<AuthorDto> GetAuthorDtoByIdAsync(int value);
        Task AddAsync(Author newAuthor);
    }
}