using System.ComponentModel.DataAnnotations;

namespace Frontend.Models
{
    public class Category
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Nazwa kategorii jest wymagana")]
        [MinLength(2, ErrorMessage = "Nazwa kategorii musi mieć co najmniej 2 znaki")]
        public string Name { get; set; }

        public ICollection<Expense> Expenses { get; set; } = new List<Expense>();
    }
} 