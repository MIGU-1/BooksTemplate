using Books.Core.Entities;
using Books.Core.Validations;
using Books.Persistence;
using Books.Wpf.Common;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace Books.Wpf.ViewModels
{
    public class BookEditCreateViewModel : BaseViewModel
    {
        private string _titel;
        private string _authorNames;
        private string _publisher;
        private string _isbn;

        public string Titel
        {
            get => _titel;
            set
            {
                _titel = value;
                OnPropertyChanged(nameof(Titel));
            }
        }
        public string AuthorNames
        {
            get => _authorNames;
            set
            {
                _authorNames = value;
                OnPropertyChanged(nameof(AuthorNames));
            }
        }
        public string Publisher
        {
            get => _publisher;
            set
            {
                _publisher = value;
                OnPropertyChanged(nameof(Publisher));
            }
        }
        public string ISBN
        {
            get => _isbn;
            set
            {
                _isbn = value;
                OnPropertyChanged(nameof(ISBN));
            }
        }

        public string WindowTitle { get; set; }

        public BookEditCreateViewModel() : base(null)
        {
        }

        public BookEditCreateViewModel(IWindowController windowController) : base(windowController)
        {
        }

        public static async Task<BaseViewModel> Create(IWindowController controller, Book book)
        {
            var model = new BookEditCreateViewModel(controller);
            await model.LoadData(book);
            return model;
        }

        private async Task LoadData(Book book)
        {
            if (book != null)
            {
                WindowTitle = "BUCH BEARBEITEN";
            }
            else
            {
                WindowTitle = "BUCH ERSTELLEN";
            }
        }

        public override IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            yield return ValidationResult.Success;
        }
    }
}
