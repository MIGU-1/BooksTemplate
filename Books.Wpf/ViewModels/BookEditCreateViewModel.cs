using Books.Core.Contracts;
using Books.Core.Entities;
using Books.Core.Validations;
using Books.Persistence;
using Books.Persistence.Comparer;
using Books.Wpf.Common;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Books.Wpf.ViewModels
{
    public class BookEditCreateViewModel : BaseViewModel
    {
        private Book _book;
        private string _title;
        private string _publishers;
        private string _isbn;
        private string _authorNames;
        private Author _selectedAuthor;
        private ObservableCollection<Author> _authors;
        private ObservableCollection<Author> _authorsComboBox;

        public Book Book
        {
            get => _book;
            set
            {
                _book = value;
                OnPropertyChanged(nameof(Book));
            }
        }
        public string WindowTitle { get; set; }

        [Required]
        public string Title
        {
            get => _title;
            set
            {
                _title = value;
                OnPropertyChanged(nameof(Title));
                ValidateViewModelProperties();
            }
        }
        public string Publishers
        {
            get => _publishers;
            set
            {
                _publishers = value;
                OnPropertyChanged(nameof(Publishers));
                ValidateViewModelProperties();
            }
        }
        public string AuthorNames
        {
            get => _authorNames;
            set
            {
                _authorNames = value;
                OnPropertyChanged(nameof(AuthorNames));
                ValidateViewModelProperties();
            }
        }
        public string ISBN
        {
            get => _isbn;
            set
            {
                _isbn = value;
                OnPropertyChanged(nameof(ISBN));
                ValidateViewModelProperties();
            }
        }
        public Author SelectedAuthor
        {
            get => _selectedAuthor;
            set
            {
                _selectedAuthor = value;
                OnPropertyChanged(nameof(SelectedAuthor));
            }
        }
        public List<Author> NewAuthors { get; set; }
        public ObservableCollection<Author> Authors
        {
            get => _authors;
            set
            {
                _authors = value;
                OnPropertyChanged(nameof(Authors));
            }
        }
        public ObservableCollection<Author> AuthorsComboBox
        {
            get => _authorsComboBox;
            set
            {
                _authorsComboBox = value;
                OnPropertyChanged(nameof(AuthorsComboBox));
            }
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
            await using IUnitOfWork unitOfWork = new UnitOfWork();
            {
                if (book != null)
                {
                    WindowTitle = "BUCH BEARBEITEN";
                    Book = book;
                    Title = book.Title;
                    Publishers = book.Publishers;
                    ISBN = book.Isbn;
                    NewAuthors = new List<Author>();
                    Authors = new ObservableCollection<Author>();

                    foreach (var bookAuthor in book.BookAuthors)
                    {
                        Authors.Add(bookAuthor
                            .Author);
                    }

                    AuthorNames = GetAuthorNames();

                    var allAuthors = await unitOfWork
                                                .Authors
                                                .GetAllAsync();

                    AuthorsComboBox = new ObservableCollection<Author>(allAuthors
                        .Except(Authors, new AuthorComparer()));
                }
                else
                {
                    WindowTitle = "BUCH ERSTELLEN";

                    var allAuthors = await unitOfWork
                                                .Authors
                                                .GetAllAsync();

                    NewAuthors = new List<Author>();
                    Authors = new ObservableCollection<Author>();
                    AuthorsComboBox = new ObservableCollection<Author>(allAuthors);
                    Book = new Book();
                }

                SelectedAuthor = AuthorsComboBox.FirstOrDefault();
            }
        }

        private string GetAuthorNames()
        {
            string authorNames = "";

            if (Authors != null)
            {
                foreach (var author in Authors)
                {
                    if (authorNames != "")
                    {
                        authorNames += $"/ {author.Name}";
                    }
                    else
                    {
                        authorNames = author.Name;
                    }
                }
            }

            return authorNames;
        }

        public override IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (ISBN != null && !Book.CheckIsbn(ISBN))
            {
                yield return new ValidationResult($"Isbn nicht gültig!", new string[] { nameof(ISBN) });
            }
        }

        private ICommand _cmdAddCommand;

        public ICommand CmdAddCommand
        {
            get
            {
                if (_cmdAddCommand == null)
                {
                    _cmdAddCommand = new RelayCommand(
                        execute: _ =>
                        {
                            Authors.Add(SelectedAuthor);
                            NewAuthors.Add(SelectedAuthor);
                            AuthorNames = GetAuthorNames();
                        },
                        canExecute: _ => SelectedAuthor != null
                        );
                }

                return _cmdAddCommand;
            }
            set => _cmdAddCommand = value;
        }

        private ICommand _cmdRemoveCommand;

        public ICommand CmdRemoveCommand
        {
            get
            {
                if (_cmdRemoveCommand == null)
                {
                    _cmdRemoveCommand = new RelayCommand(
                        execute: _ =>
                        {
                            Authors.Remove(SelectedAuthor);
                            NewAuthors.Remove(SelectedAuthor);
                            AuthorNames = GetAuthorNames();
                        },
                        canExecute: _ => SelectedAuthor != null
                        );
                }

                return _cmdRemoveCommand;
            }
            set => _cmdRemoveCommand = value;
        }

        private ICommand _cmdSaveCommand;

        public ICommand CmdSaveCommand
        {
            get
            {
                if (_cmdSaveCommand == null)
                {
                    _cmdSaveCommand = new RelayCommand(
                        execute: async _ =>
                        {
                            ValidateViewModelProperties();

                            try
                            {
                                await using IUnitOfWork unitOfWork = new UnitOfWork();
                                Book.Title = Title;
                                Book.Publishers = Publishers;
                                Book.Isbn = ISBN;

                                foreach (var author in NewAuthors)
                                {
                                    BookAuthor newBookAuthor = new BookAuthor()
                                    {
                                        Author = author,
                                        Book = this.Book
                                    };

                                    Book.BookAuthors.Add(newBookAuthor);
                                }
                                unitOfWork.Books.Update(Book);
                                await unitOfWork.SaveChangesAsync();
                                Controller.CloseWindow(this);
                            }
                            catch (ValidationException ex)
                            {
                                if (ex.Value is IEnumerable<string> properties)
                                {
                                    foreach (var property in properties)
                                    {
                                        Errors.Add(property, new List<string> { ex.ValidationResult.ErrorMessage });
                                    }
                                }
                                else
                                {
                                    DbError = ex.ValidationResult.ToString();
                                }
                            }
                        },
                        canExecute: _ => !HasErrors
                        );
                }

                return _cmdSaveCommand;
            }
            set => _cmdSaveCommand = value;
        }
    }
}
