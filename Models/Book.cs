using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace LiberaryManagmentSystem.Models
{
    public class Book
    {
        [Key]
        public int BookId { get; set; }

        [Required]
        [MaxLength(150)]
        public required string Title { get; set; }

        [Required]
        [MaxLength(100)]
        public required string Author { get; set; }
        public string? Genre { get; set; }

        public DateTime PublishedDate { get; set; }

        public int CopiesAvailable { get; set; }

        public virtual IEnumerable<Checkout> Checkouts { get; set; } = new List<Checkout>();
    }
}
