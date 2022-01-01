using System;
namespace ers_server_net6_minapi.Models {

    public class Expense {
        public int Id { get; init; } = 0;
        public string Description { get; set; } = string.Empty;
        public string Payee { get; set; } = string.Empty; // who to pay
        public decimal TotalExpenses { get; set; } = 0m;
        public decimal TotalReimbursed { get; set; } = 0m;
        public bool Locked { get; set; } = false;

        public virtual IEnumerable<Expenseline> Expenselines { get; set; } = new List<Expenseline>();

        public bool? Active { get; set; } = true;
        public DateTime? Created { get; set; } = DateTime.UtcNow;
        public DateTime? Updated { get; set; } = null;
    }
}

