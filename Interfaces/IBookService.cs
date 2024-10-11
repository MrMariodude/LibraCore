using LiberaryManagmentSystem.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LiberaryManagmentSystem.Services
{
    public interface IBookService
    {
        Task<IEnumerable<Book>> GetAllBooksAsync();
        Task<Book> GetBookByIdAsync(int id);
        Task<Book> CreateBookAsync(Book book);
        Task UpdateBookAsync(Book book);
        Task DeleteBookAsync(int id);
        Task<IEnumerable<Book>> SearchBooksAsync(string searchBy, string query);
    }
}
