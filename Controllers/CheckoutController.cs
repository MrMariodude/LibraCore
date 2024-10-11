using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using LiberaryManagmentSystem.Models;
using System.Threading.Tasks;
using LiberaryManagmentSystem.Services;
using System.Security.Claims;

namespace LiberaryManagmentSystem.Controllers
{
    [Authorize]
    public class CheckoutController(ICheckoutService checkoutService) : Controller
    {
        /// <summary>
        /// Returns a checked-out book and handles any penalties.
        /// </summary>
        /// <param name="checkoutId">The ID of the checkout record.</param>
        /// <returns>Redirects to UserCheckouts or Payment on failure.</returns>
        /// <remarks>POST: /Checkout/ReturnBook</remarks>
        [HttpPost]
        public async Task<IActionResult> ReturnBook(int checkoutId)
        {
            try
            {
                await checkoutService.ReturnBookAsync(checkoutId);
                return RedirectToAction("UserCheckouts");
            }
            catch (InvalidOperationException ex)
            {
                // Handle penalties or other issues
                ViewBag.ErrorMessage = ex.Message;
                return RedirectToAction("Payment", new { checkoutId, penalty = ex.Message });
            }
        }

        /// <summary>
        /// Displays the payment page for penalties on a returned book.
        /// </summary>
        /// <param name="checkoutId">The ID of the checkout record.</param>
        /// <param name="penalty">The penalty amount.</param>
        /// <returns>View for processing payment.</returns>
        /// <remarks>GET: /Checkout/Payment</remarks>
        [HttpGet]
        public async Task<IActionResult> Payment(int checkoutId, decimal penalty)
        {
            ViewBag.CheckoutId = checkoutId;
            ViewBag.Penalty = penalty;

            var book = await checkoutService.GetCheckoutByIdAsync(checkoutId);

            return View(book);
        }

        /// <summary>
        /// Processes the payment for penalties related to a returned book.
        /// </summary>
        /// <param name="checkoutId">The ID of the checkout record.</param>
        /// <returns>Redirects to UserCheckouts on success or returns NotFound on failure.</returns>
        /// <remarks>POST: /Checkout/ProcessPayment</remarks>
        [HttpPost]
        public async Task<IActionResult> ProcessPayment(int checkoutId)
        {
            try
            {
                await checkoutService.ProcessPaymentAsync(checkoutId);
                return RedirectToAction("UserCheckouts");
            }
            catch (InvalidOperationException)
            {
                return NotFound();
            }
        }

        /// <summary>
        /// Displays the user's current checkouts.
        /// </summary>
        /// <returns>View with the user's checkout records.</returns>
        /// <remarks>GET: /Checkout/UserCheckouts</remarks>
        [HttpGet]
        public async Task<IActionResult> UserCheckouts()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "";
            var userCheckouts = await checkoutService.GetCheckoutsByUserIdAsync(userId);
            return View(userCheckouts);
        }

        /// <summary>
        /// Checks out a book for a user with a specified return date.
        /// </summary>
        /// <param name="bookId">The ID of the book to checkout.</param>
        /// <param name="returnDate">The return date for the book.</param>
        /// <returns>Redirects to BookDetails or returns with an error message on failure.</returns>
        /// <remarks>POST: /Checkout/Checkout</remarks>
        [HttpPost]
        public async Task<IActionResult> Checkout(int bookId, DateTime returnDate)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "";
            try
            {
                await checkoutService.CheckoutBookAsync(bookId, returnDate, userId);
                return RedirectToAction("BookDetails", "Book", new { id = bookId });
            }
            catch (InvalidOperationException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return RedirectToAction("BookDetails", "Book", new { id = bookId });
            }
        }
    }
}
