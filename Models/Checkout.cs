using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LiberaryManagmentSystem.Models
{
    public class Checkout
    {
        [Key]
        public int CheckoutId { get; set; }

        [Required]
        public int BookId { get; set; }

        [ForeignKey("BookId")]
        public Book Book { get; set; } = null!;

        [Required]
        public required string ApplicationUserId { get; set; }

        [ForeignKey("ApplicationUserId")]
        public virtual ApplicationUser ApplicationUser { get; set; } = null!;

        [Required]
        public DateTime CheckoutDate { get; set; } = DateTime.Now;

        [Required]
        public DateTime ReturnDate { get; set; }

        public bool IsReturned { get; set; } = false;

        public decimal PenaltyAmount
        {
            get
            {
                if (ReturnDate >= DateTime.Now)
                    return 0;

                int overdueDays = (DateTime.Now - ReturnDate).Days;
                return 10 + (0.5m * overdueDays);
            }
        }

    }
}
