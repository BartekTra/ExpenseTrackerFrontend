using System;
using System.ComponentModel.DataAnnotations;

namespace Frontend.Models
{
    public class Expense
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Opis jest wymagany")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Kwota jest wymagana")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Kwota musi być większa od 0")]
        public decimal Amount { get; set; }

        [Required(ErrorMessage = "Data jest wymagana")]
        public DateTime Date { get; set; }

        [Required(ErrorMessage = "Kategoria jest wymagana")]
        public int CategoryId { get; set; }

        public string UserId { get; set; }
        public Category Category { get; set; }
    }

    public class CreateExpenseDto
    {
        [Required(ErrorMessage = "Opis jest wymagany")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Kwota jest wymagana")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Kwota musi być większa od 0")]
        public decimal Amount { get; set; }

        [Required(ErrorMessage = "Data jest wymagana")]
        public DateTime Date { get; set; }

        [Required(ErrorMessage = "Kategoria jest wymagana")]
        public int CategoryId { get; set; }
    }

    public class MonthlyExpenseTotalDto
    {
        public int Year { get; set; }
        public int Month { get; set; }
        public decimal TotalAmount { get; set; }
    }

    public class CategoryExpenseSummaryDto
    {
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public decimal TotalAmount { get; set; }
    }
} 