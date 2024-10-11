using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using LiberaryManagmentSystem.Models;
using LiberaryManagmentSystem.Services;
using Microsoft.EntityFrameworkCore;

namespace LiberaryManagmentSystem.Controllers
{
    [Authorize]
    public class BookController(IBookService bookService) : Controller
    {
        /// <summary>
        /// Displays a list of all books.
        /// </summary>
        /// <returns>View of all books.</returns>
        /// <remarks>GET: /Book/Index</remarks>
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var books = await bookService.GetAllBooksAsync();
            return View(books);
        }

        /// <summary>
        /// Displays details of a specific book.
        /// </summary>
        /// <param name="id">The ID of the book.</param>
        /// <returns>View of the book details or NotFound if not found.</returns>
        /// <remarks>GET: /Book/Details/{id}</remarks>
        public async Task<IActionResult> Details(int id)
        {
            var book = await bookService.GetBookByIdAsync(id);
            if (book == null)
            {
                return NotFound();
            }
            return View(book);
        }

        /// <summary>
        /// Displays the Create Book form.
        /// </summary>
        /// <returns>View for creating a new book.</returns>
        /// <remarks>GET: /Book/Create</remarks>
        [Authorize(Roles = "Admin,Librarian")]
        public IActionResult Create()
        {
            return View();
        }

        /// <summary>
        /// Handles the creation of a new book.
        /// </summary>
        /// <param name="book">The book model to be created.</param>
        /// <returns>Redirects to Index on success, or returns the view with errors.</returns>
        /// <remarks>POST: /Book/Create</remarks>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Librarian")]
        public async Task<IActionResult> Create([Bind("Title,Author,Genre,PublishedDate,CopiesAvailable")] Book book)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await bookService.CreateBookAsync(book);
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateException ex)
                {
                    if (ex.InnerException?.Message.Contains("duplicate key") == true)
                    {
                        ModelState.AddModelError("Title", "The title must be unique.");
                    }
                    else
                    {
                        ModelState.AddModelError("", "An error occurred while saving changes.");
                    }
                }
            }
            return View(book);
        }

        /// <summary>
        /// Displays the Edit Book form for a specific book.
        /// </summary>
        /// <param name="id">The ID of the book to edit.</param>
        /// <returns>View for editing the book or NotFound if not found.</returns>
        /// <remarks>GET: /Book/Edit/{id}</remarks>
        [Authorize(Roles = "Admin,Librarian")]
        public async Task<IActionResult> Edit(int id)
        {
            var book = await bookService.GetBookByIdAsync(id);
            if (book == null)
            {
                return NotFound();
            }
            return View(book);
        }

        /// <summary>
        /// Handles the update of an existing book.
        /// </summary>
        /// <param name="id">The ID of the book to update.</param>
        /// <param name="book">The updated book model.</param>
        /// <returns>Redirects to Index on success, or returns the view with errors.</returns>
        /// <remarks>POST: /Book/Edit/{id}</remarks>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Librarian")]
        public async Task<IActionResult> Edit(int id, [Bind("BookId,Title,Author,Genre,PublishedDate,CopiesAvailable")] Book book)
        {
            if (id != book.BookId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    await bookService.UpdateBookAsync(book);
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception)
                {
                    if (!await BookExists(book.BookId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }
            return View(book);
        }

        /// <summary>
        /// Displays the Delete Book confirmation form for a specific book.
        /// </summary>
        /// <param name="id">The ID of the book to delete.</param>
        /// <returns>View for confirming deletion or NotFound if not found.</returns>
        /// <remarks>GET: /Book/Delete/{id}</remarks>
        [Authorize(Roles = "Admin,Librarian")]
        public async Task<IActionResult> Delete(int id)
        {
            var book = await bookService.GetBookByIdAsync(id);
            if (book == null)
            {
                return NotFound();
            }
            return View(book);
        }

        /// <summary>
        /// Handles the deletion of a specific book.
        /// </summary>
        /// <param name="id">The ID of the book to delete.</param>
        /// <returns>Redirects to Index after deletion.</returns>
        /// <remarks>POST: /Book/DeleteConfirmed/{id}</remarks>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Librarian")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await bookService.DeleteBookAsync(id);
            return RedirectToAction(nameof(Index));
        }

        /// <summary>
        /// Checks if a book exists by ID.
        /// </summary>
        /// <param name="id">The ID of the book to check.</param>
        /// <returns>True if the book exists; otherwise, false.</returns>
        private async Task<bool> BookExists(int id)
        {
            var book = await bookService.GetBookByIdAsync(id);
            return book != null;
        }

        /// <summary>
        /// Displays details of a specific book.
        /// </summary>
        /// <param name="id">The ID of the book.</param>
        /// <returns>View of the book details or NotFound if not found.</returns>
        /// <remarks>GET: /Book/BookDetails/{id}</remarks>
        public async Task<IActionResult> BookDetails(int id)
        {
            var book = await bookService.GetBookByIdAsync(id);
            return View(book);
        }

        /// <summary>
        /// Searches for books based on the specified criteria.
        /// </summary>
        /// <param name="searchBy">The field to search by (e.g., Title, Author).</param>
        /// <param name="query">The search term.</param>
        /// <returns>View of the filtered book list or an empty list if no query is provided.</returns>
        /// <remarks>GET: /Book/Search</remarks>
        [HttpGet]
        public async Task<IActionResult> Search(string searchBy, string query)
        {
            if (string.IsNullOrEmpty(query))
            {
                return View("Index", new List<Book>());
            }

            var books = await bookService.SearchBooksAsync(searchBy, query.Trim());
            return View("Index", books);
        }
    }
}
