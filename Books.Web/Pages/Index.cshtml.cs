﻿using Books.Core.Contracts;
using Books.Core.DataTransferObjects;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Books.Web.Pages
{
    public class IndexModel : PageModel
    {
        private readonly IUnitOfWork _uow;
        public AuthorDto[] Authors; 

        public IndexModel(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<IActionResult> OnGet()
        {
            Authors = await _uow.Authors
                .GetAuthorOverViewAsync();

            return Page();
        }
    }
}
