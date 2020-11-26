using Books.Core.Validations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace Books.Core.Entities
{
    public class Book : EntityObject, IValidatableObject
    {

        public ICollection<BookAuthor> BookAuthors { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Titel muss definiert sein")]
        [MaxLength(200, ErrorMessage = "Titel darf maximal 200 Zeichen lang sein")]
        public string Title { get; set; }

        [Required, MaxLength(100)]
        public string Publishers { get; set; }

        [IsbnValidation]
        [Required, MaxLength(13)]
        public string Isbn { get; set; }

        [NotMapped]
        public string AuthorNames
        {
            get
            {
                string authorNames = "";

                if (BookAuthors != null)
                {
                    foreach (var author in BookAuthors)
                    {
                        if (authorNames != "")
                        {
                            authorNames += $"/ {author.Author.Name}";
                        }
                        else
                        {
                            authorNames = author.Author.Name;
                        }
                    }
                }

                return authorNames;
            }
        }

        public Book()
        {
            BookAuthors = new List<BookAuthor>();
        }

        /// <summary>
        /// Eine gültige ISBN-Nummer besteht aus den Ziffern 0, ... , 9,
        /// 'x' oder 'X' (nur an der letzten Stelle)
        /// Die Gesamtlänge der ISBN beträgt 10 Zeichen.
        /// Für die Ermittlung der Prüfsumme werden die Ziffern 
        /// von rechts nach links mit 1 - 10 multipliziert und die 
        /// Produkte aufsummiert. Ist das rechte Zeichen ein x oder X
        /// wird als Zahlenwert 10 verwendet.
        /// Die Prüfsumme muss modulo 11 0 ergeben.
        /// </summary>
        /// <returns>Prüfergebnis</returns>
        public static bool CheckIsbn(string isbn)
        {
            bool hasX;

            if (isbn == null)
                throw new ArgumentNullException($"{isbn} is null");

            if (isbn.Length != 10)
                return false;

            hasX = isbn.ToLower()[9]
                .Equals('x');

            int sum = 0;

            if (hasX)
            {
                for (int i = 0; i < 9; i++)
                {
                    if (Char.IsDigit(isbn[i]))
                    {
                        sum += Convert.ToInt32(isbn[i] - '0') * (i + 1);
                    }
                    else
                    {
                        return false;
                    }

                }

                sum += 100;
            }
            else
            {
                for (int i = 0; i < 10; i++)
                {
                    if (Char.IsDigit(isbn[i]))
                    {
                        sum += Convert.ToInt32(isbn[i] - '0') * (i + 1);
                    }
                    else
                    {
                        return false;
                    }
                }
            }

            return (sum % 11 == 0);
        }

        public override string ToString()
        {
            return $"{Title} {Isbn} mit {BookAuthors.Count()} Autoren";
        }

        /// <inheritdoc />
        /// <summary>
        /// Jedes Buch muss zumindest einen Autor haben.
        /// Weiters darf ein Autor einem Buch nicht mehrfach zugewiesen
        /// werden.
        /// </summary>
        /// <param name="validationContext"></param>
        /// <returns></returns>
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            yield return ValidationResult.Success;
        }
    }
}

