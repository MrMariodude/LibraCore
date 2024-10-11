using LiberaryManagmentSystem.Models;
using LiberaryManagmentSystem.Repository;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LiberaryManagmentSystem.Services
{
    /// <summary>
    /// Service for managing checkout-related operations.
    /// </summary>
    public class CheckoutService(IRepository<Checkout> checkoutRepository, IRepository<Book> bookRepository) : ICheckoutService
    {
        /// <summary>
        /// Retrieves a checkout record by its ID asynchronously.
        /// </summary>
        /// <param name="checkoutId">The ID of the checkout to retrieve.</param>
        /// <returns>
        /// The <see cref="Checkout"/> object if found; otherwise, <c>null</c>.
        /// </returns>
        public async Task<Checkout> GetCheckoutByIdAsync(int checkoutId)
        {
            return await checkoutRepository.GetByIdAsync(checkoutId);
        }

        /// <summary>
        /// Retrieves all checkout records for a specific user asynchronously.
        /// </summary>
        /// <param name="userId">The ID of the user to retrieve checkouts for.</param>
        /// <returns>
        /// A collection of <see cref="Checkout"/> objects associated with the user.
        /// </returns>
        public async Task<IEnumerable<Checkout>> GetCheckoutsByUserIdAsync(string userId)
        {
            return await checkoutRepository.GetCheckoutsByUserIdAsync(userId);
        }

        /// <summary>
        /// Processes the return of a book associated with a checkout record.
        /// </summary>
        /// <param name="checkoutId">The ID of the checkout record.</param>
        /// <returns>
        /// A task representing the asynchronous operation.
        /// </returns>
        /// <exception cref="InvalidOperationException">
        /// Thrown if there is a penalty amount due.
        /// </exception>
        public async Task ReturnBookAsync(int checkoutId)
        {
            var checkout = await checkoutRepository.GetByIdAsync(checkoutId);
            var book = await bookRepository.GetByIdAsync(checkout.BookId);

            if (checkout.PenaltyAmount > 0)
            {
                // Handle penalty logic (e.g., redirect to payment)
                throw new InvalidOperationException("Penalty amount due.");
            }

            book.CopiesAvailable += 1;
            await bookRepository.UpdateAsync(book);

            checkout.IsReturned = true;
            await checkoutRepository.UpdateAsync(checkout);
        }

        /// <summary>
        /// Processes payment for a checkout record and marks the book as returned.
        /// </summary>
        /// <param name="checkoutId">The ID of the checkout to process payment for.</param>
        /// <returns>
        /// A task representing the asynchronous operation.
        /// </returns>
        /// <exception cref="InvalidOperationException">
        /// Thrown if the checkout or book cannot be found.
        /// </exception>
        public async Task ProcessPaymentAsync(int checkoutId)
        {
            var checkout = await checkoutRepository.GetByIdAsync(checkoutId);
            if (checkout == null)
            {
                throw new InvalidOperationException("Checkout not found.");
            }

            var book = await bookRepository.GetByIdAsync(checkout.BookId);
            if (book == null)
            {
                throw new InvalidOperationException("Book not found.");
            }

            book.CopiesAvailable += 1;
            await bookRepository.UpdateAsync(book);

            checkout.IsReturned = true;
            await checkoutRepository.UpdateAsync(checkout);
        }

        /// <summary>
        /// Checks out a book for a user, setting the return date.
        /// </summary>
        /// <param name="bookId">The ID of the book to checkout.</param>
        /// <param name="returnDate">The date by which the book should be returned.</param>
        /// <param name="userId">The ID of the user checking out the book.</param>
        /// <returns>
        /// A task representing the asynchronous operation.
        /// </returns>
        /// <exception cref="InvalidOperationException">
        /// Thrown if the book cannot be found or there are no copies available.
        /// </exception>
        public async Task CheckoutBookAsync(int bookId, DateTime returnDate, string userId)
        {
            var book = await bookRepository.GetByIdAsync(bookId);
            if (book == null)
            {
                throw new InvalidOperationException("Book not found.");
            }

            if (book.CopiesAvailable <= 0)
            {
                throw new InvalidOperationException("No copies available for this book.");
            }

            var checkout = new Checkout
            {
                BookId = bookId,
                ApplicationUserId = userId,
                CheckoutDate = DateTime.Now,
                ReturnDate = returnDate
            };

            await checkoutRepository.AddAsync(checkout);
            book.CopiesAvailable -= 1;
            await bookRepository.UpdateAsync(book);
        }
    }
}
