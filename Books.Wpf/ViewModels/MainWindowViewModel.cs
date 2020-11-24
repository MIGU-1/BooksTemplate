using Books.Core.Contracts;
using Books.Core.Entities;
using Books.Persistence;
using Books.Wpf.Common;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Books.Wpf.ViewModels
{
    public class MainWindowViewModel : BaseViewModel
    {
        private Book[] _allBooks;
        private ObservableCollection<Book> _books;
        private Book _selectedBook;
        private string _searchText;

        public Book SelectedBook
        {
            get => _selectedBook;
            set
            {
                _selectedBook = value;
                OnPropertyChanged(nameof(SelectedBook));
            }
        }
        public ObservableCollection<Book> Books
        {
            get => _books;
            set
            {
                _books = value;
                OnPropertyChanged(nameof(Books));
            }
        }
        public string SearchText
        {
            get => _searchText;
            set
            {
                _searchText = value;
                OnPropertyChanged(nameof(SearchText));
                RefreshDataGrid();
            }
        }

        public MainWindowViewModel() : base(null)
        {
        }

        public MainWindowViewModel(IWindowController windowController) : base(windowController)
        {
        }

        /// <summary>
        /// Lädt die gefilterten Buchdaten
        /// </summary>
        public async Task LoadBooks()
        {
            await using IUnitOfWork uow = new UnitOfWork();

            _allBooks = await uow
                .Books
                .GetAllAsync();

            Books = new ObservableCollection<Book>(_allBooks);
        }

        private void RefreshDataGrid()
        {
            if (SearchText != null)
            {
                Books = new ObservableCollection<Book>(_allBooks
                    .Where(b => b.Title
                    .ToLower()
                    .Contains(SearchText.ToLower()))
                    .ToArray());
            }
            else
            {
                Books = new ObservableCollection<Book>(_allBooks);
            }
        }

        public static async Task<BaseViewModel> Create(IWindowController controller)
        {
            var model = new MainWindowViewModel(controller);
            await model.LoadBooks();
            return model;
        }

        public override IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            throw new NotImplementedException();
        }

        private ICommand _cmdEditCommand;

        public ICommand CmdEditCommand
        {
            get
            {
                if (_cmdEditCommand == null)
                {
                    _cmdEditCommand = new RelayCommand(
                        execute: async _ =>
                        {
                            var editViewModel = await BookEditCreateViewModel.Create(Controller, SelectedBook);
                            Controller.ShowWindow(editViewModel, true);
                            _ = LoadBooks();
                        },
                        canExecute: _ => SelectedBook != null
                        );
                }

                return _cmdEditCommand;
            }
            set => _cmdEditCommand = value;
        }

        private ICommand _cmdCreateCommand;

        public ICommand CmdCreateCommand
        {
            get
            {
                if (_cmdCreateCommand == null)
                {
                    _cmdCreateCommand = new RelayCommand(
                        execute: async _ =>
                        {
                            var createViewModel = await BookEditCreateViewModel.Create(Controller, null);
                            Controller.ShowWindow(createViewModel, true);
                            _ = LoadBooks();
                        },
                        canExecute: _ => true
                        );
                }

                return _cmdCreateCommand;
            }
            set => _cmdCreateCommand = value;
        }

        private ICommand _cmdDeleteCommand;

        public ICommand CmdDeleteCommand
        {
            get
            {
                if (_cmdDeleteCommand == null)
                {
                    _cmdDeleteCommand = new RelayCommand(
                        execute: async _ =>
                        {
                            try
                            {
                                await using IUnitOfWork uow = new UnitOfWork();

                                if (MessageBox.Show("Buch löschen?", "Question", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                                {
                                    uow.Books.Remove(SelectedBook);
                                    await uow.SaveChangesAsync();
                                    RefreshDataGrid();
                                }
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
                        canExecute: _ => SelectedBook != null
                        );
                }

                return _cmdDeleteCommand;
            }
            set => _cmdDeleteCommand = value;
        }
    }
}
