using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using Books.Core.Entities;

namespace Books.Core.DataTransferObjects
{
    public class AuthorDto
    {
        public int Id { get; set; }

        [DisplayName("Autor")]
        public string Author { get; set; }

        [DisplayName("Anzahl Bücher")]
        public int BookCount { get; set; }

        public IEnumerable<Book> Books { get; set; }

        [DisplayName("Verläge")]
        public string Publishers { get; set; }

        public AuthorDto()
        {
            Books = new List<Book>();
        }
    }
}
