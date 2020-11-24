using Books.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using Utils;

namespace Books.ImportConsole
{
    public static class ImportController
    {
        public static IEnumerable<Book> ReadBooksFromCsv()
        {
            string fileName = "books.csv";
            char seperator = '~';
            string[][] matrix = MyFile.ReadStringMatrixFromCsv(fileName, false);
            List<Author> authors = new List<Author>();
            List<BookAuthor> bookAuthors = new List<BookAuthor>();
            List<Book> books = new List<Book>();

            foreach (var line in matrix)
            {
                string[] publishers = line
                    .GroupBy(grp => line[2])
                    .Select(grp => grp.Key)
                    .ToArray();

                Book newBook = new Book
                {
                    Title = line[1],
                    Publishers = publishers
                        .SingleOrDefault(p => p
                        .Equals(line[2])),
                    Isbn = line[3]
                };

                string[] names = line[0].Split(seperator);

                foreach (var name in names)
                {
                    string fullName = GetFullName(name);
                    BookAuthor newBookAuthor = new BookAuthor();

                    if (authors.SingleOrDefault(a => a.Name.Equals(fullName)) == null)
                    {
                        Author newAuthor = new Author()
                        {
                            Name = GetFullName(name)
                        };

                        newBookAuthor.Author = newAuthor;
                        newAuthor.BookAuthors.Add(newBookAuthor);
                        authors.Add(newAuthor);
                    }
                    else
                    {
                        newBookAuthor.Author = authors
                            .SingleOrDefault(a => a.Name.Equals(fullName));
                    }

                    newBookAuthor.Book = newBook;
                    newBook.BookAuthors.Add(newBookAuthor);
                }

                books.Add(newBook);
            }

            return books;
        }

        private static string GetFullName(string name)
        {
            string[] fullName = name.Split(",");
            
            if(fullName.Length == 1)
            {
                return fullName[0];
            }
            else if(fullName.Length == 2)
            {
                return $"{fullName[0]} {fullName[1]}";
            }
            else
            {
                return $"{fullName[0]} {fullName[1]} {fullName[2]}";
            }
        }
    }
}
