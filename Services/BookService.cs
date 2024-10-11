using LiberaryManagmentSystem.Models;
using LiberaryManagmentSystem.Repository;

namespace LiberaryManagmentSystem.Services
{
    /// <summary>
    /// Service for managing book-related operations.
    /// </summary>
    public class BookService(IRepository<Book> repository) : IBookService
    {
        /// <summary>
        /// Retrieves all books asynchronously.
        /// </summary>
        /// <returns>
        /// A collection of <see cref="Book"/> objects.
        /// </returns>
        public async Task<IEnumerable<Book>> GetAllBooksAsync()
        {
            return await repository.GetAllAsync();
        }

        /// <summary>
        /// Retrieves a book by its ID asynchronously.
        /// </summary>
        /// <param name="id">The ID of the book to retrieve.</param>
        /// <returns>
        /// A <see cref="Book"/> object if found; otherwise, <c>null</c>.
        /// </returns>
        public async Task<Book> GetBookByIdAsync(int id)
        {
            return await repository.GetByIdAsync(id);
        }

        /// <summary>
        /// Creates a new book asynchronously.
        /// </summary>
        /// <param name="book">The <see cref="Book"/> object to create.</param>
        /// <returns>
        /// The created <see cref="Book"/> object.
        /// </returns>
        public async Task<Book> CreateBookAsync(Book book)
        {
            return await repository.AddAsync(book);
        }

        /// <summary>
        /// Updates an existing book asynchronously.
        /// </summary>
        /// <param name="book">The <see cref="Book"/> object with updated information.</param>
        /// <returns>
        /// A task representing the asynchronous operation.
        /// </returns>
        public async Task UpdateBookAsync(Book book)
        {
            await repository.UpdateAsync(book);
        }

        /// <summary>
        /// Deletes a book by its ID asynchronously.
        /// </summary>
        /// <param name="id">The ID of the book to delete.</param>
        /// <returns>
        /// A task representing the asynchronous operation.
        /// </returns>
        public async Task DeleteBookAsync(int id)
        {
            await repository.DeleteAsync(id);
        }

        /// <summary>
        /// Searches for books based on specified criteria asynchronously.
        /// </summary>
        /// <param name="searchBy">The property to search by (e.g., title, author).</param>
        /// <param name="query">The search term.</param>
        /// <returns>
        /// A collection of <see cref="Book"/> objects matching the search criteria.
        /// </returns>
        public async Task<IEnumerable<Book>> SearchBooksAsync(string searchBy, string query)
        {
            return await repository.SearchBooksAsync(searchBy, query);
        }
    }
}
