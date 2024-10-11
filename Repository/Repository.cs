using LiberaryManagmentSystem.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace LiberaryManagmentSystem.Repository
{
    /// <summary>
    /// Generic repository for accessing entities of type <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">The type of entity the repository handles.</typeparam>
    public class Repository<T> : IRepository<T> where T : class
    {
        private readonly ApplicationDbContext _context;
        private readonly DbSet<T> _dbSet;

        /// <summary>
        /// Initializes a new instance of the <see cref="Repository{T}"/> class.
        /// </summary>
        /// <param name="context">The application database context.</param>
        public Repository(ApplicationDbContext context)
        {
            _context = context;
            _dbSet = context.Set<T>();
        }

        /// <summary>
        /// Retrieves an entity by its identifier.
        /// </summary>
        /// <param name="id">The identifier of the entity.</param>
        /// <returns>
        /// The entity with the specified identifier, or <c>null</c> if not found.
        /// </returns>
        public async Task<T> GetByIdAsync(int id)
        {
            return await _dbSet.FindAsync(id);
        }

        /// <summary>
        /// Retrieves checkouts associated with a specific user.
        /// </summary>
        /// <param name="userId">The ID of the user whose checkouts to retrieve.</param>
        /// <returns>A collection of <see cref="Checkout"/> entities associated with the user.</returns>
        public async Task<IEnumerable<Checkout>> GetCheckoutsByUserIdAsync(string userId)
        {
            return await _context.Checkouts
                .Include(c => c.Book) // Assuming Checkout has a navigation property for Book
                .Where(c => c.ApplicationUserId == userId)
                .ToListAsync();
        }

        /// <summary>
        /// Retrieves all entities of type <typeparamref name="T"/>.
        /// </summary>
        /// <returns>A collection of all entities.</returns>
        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _dbSet.ToListAsync();
        }

        /// <summary>
        /// Adds a new entity to the repository.
        /// </summary>
        /// <param name="entity">The entity to add.</param>
        /// <returns>The added entity.</returns>
        public async Task<T> AddAsync(T entity)
        {
            await _dbSet.AddAsync(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        /// <summary>
        /// Updates an existing entity in the repository.
        /// </summary>
        /// <param name="entity">The entity to update.</param>
        public async Task UpdateAsync(T entity)
        {
            _dbSet.Update(entity);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Deletes an entity by its identifier.
        /// </summary>
        /// <param name="id">The identifier of the entity to delete.</param>
        public async Task DeleteAsync(int id)
        {
            var entity = await GetByIdAsync(id);
            if (entity != null)
            {
                _dbSet.Remove(entity);
                await _context.SaveChangesAsync();
            }
        }

        /// <summary>
        /// Finds entities matching the specified criteria.
        /// </summary>
        /// <param name="predicate">The criteria to use for filtering.</param>
        /// <returns>A collection of entities that match the specified criteria.</returns>
        public async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate)
        {
            return await _dbSet.Where(predicate).ToListAsync();
        }

        /// <summary>
        /// Searches for books based on the specified search criteria.
        /// </summary>
        /// <param name="searchBy">The field to search by (e.g., "author", "genre", "title").</param>
        /// <param name="query">The search term to find.</param>
        /// <returns>A collection of <see cref="Book"/> entities that match the search criteria.</returns>
        public async Task<IEnumerable<Book>> SearchBooksAsync(string searchBy, string query)
        {
            // Default to searching by title
            IQueryable<Book> booksQuery = _context.Books;

            switch (searchBy.ToLower()) // Using ToLower to make the search case insensitive
            {
                case "author":
                    booksQuery = booksQuery.Where(b => b.Author.Contains(query));
                    break;
                case "genre":
                    booksQuery = booksQuery.Where(b => b.Genre.Contains(query));
                    break;
                case "title":
                default:
                    booksQuery = booksQuery.Where(b => b.Title.Contains(query));
                    break;
            }

            return await booksQuery.ToListAsync();
        }
    }
}
