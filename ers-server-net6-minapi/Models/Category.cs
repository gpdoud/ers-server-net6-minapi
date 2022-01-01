using System;
namespace ers_server_net6_minapi.Models {

    public class Category {
        public int Id { get; init; } = 0;
        public string Name { get; set; } = string.Empty;
        public decimal MaxAmount { get; set; } = 0m;

        public bool? Active { get; set; } = true;
        public DateTime? Created { get; set; } = DateTime.UtcNow;
        public DateTime? Updated { get; set; } = null;

        public virtual IEnumerable<Expenseline> Expenselines { get; set; } = new List<Expenseline>();
    }
}

