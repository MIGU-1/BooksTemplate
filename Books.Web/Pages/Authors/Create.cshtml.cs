using Books.Core.Contracts;
using Books.Core.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace Books.Web.Pages.Authors
{
    public class CreateModel : PageModel
    {
        private readonly IUnitOfWork _uow;

        [BindProperty]
        [MinLength(2)]
        public string Author { get; set; }

        public CreateModel(IUnitOfWork uow)
        {
            _uow = uow;
        }

        // POST: Authors/Create
        public async Task<IActionResult> OnPost()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            try
            {
                Author newAuthor = new Author()
                {
                    Name = Author,
                };

                await _uow
                    .Authors
                    .AddAsync(newAuthor);

                await _uow.SaveChangesAsync();
                return RedirectToPage("../Index", new { authorId = newAuthor.Id });
            }
            catch (ValidationException val)
            {
                ValidationResult validation = val.ValidationResult;
                ModelState.AddModelError("", validation.ErrorMessage);
                return Page();
            }

        }
    }
}
