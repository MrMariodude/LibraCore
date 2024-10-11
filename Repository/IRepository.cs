using LiberaryManagmentSystem.Models;
using System.Linq.Expressions;

namespace LiberaryManagmentSystem.Repository;

public interface IRepository<T> where T : class
{
    Task<T> GetByIdAsync(int id);
    Task<IEnumerable<T>> GetAllAsync();
    Task<T> AddAsync(T entity);
    Task UpdateAsync(T entity);
    Task DeleteAsync(int id);
    Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate);
    Task<IEnumerable<Checkout>> GetCheckoutsByUserIdAsync(string userId);
    Task<IEnumerable<Book>> SearchBooksAsync(string searchBy, string query);
}

