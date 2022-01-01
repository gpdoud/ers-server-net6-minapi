using System;
namespace ers_server_net6_minapi.Models {

    public class Expenseline {
        public int Id { get; init; } = 0;
        public string Description { get; set; } = string.Empty;
        public decimal Amount { get; set; } = 0m;
        public bool Prepaid { get; set; } = false;

        public int CategoryId { get; set; } = 0;
        public virtual Category? Category { get; set; } = null;

        public int ExpenseId { get; set; } = 0;
        public virtual Expense? Expense { get; set; } = null;

        public bool? Active { get; set; } = true;
        public DateTime? Created { get; set; } = DateTime.UtcNow;
        public DateTime? Updated { get; set; } = null;

    }
}

