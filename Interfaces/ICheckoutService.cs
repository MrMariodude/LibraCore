using LiberaryManagmentSystem.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LiberaryManagmentSystem.Services
{
    public interface ICheckoutService
    {
        Task<Checkout> GetCheckoutByIdAsync(int checkoutId);
        Task<IEnumerable<Checkout>> GetCheckoutsByUserIdAsync(string userId);
        Task ReturnBookAsync(int checkoutId);
        Task ProcessPaymentAsync(int checkoutId);
        Task CheckoutBookAsync(int bookId, DateTime returnDate, string userId);
    }
}
