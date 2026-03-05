using System;
using Ventrix.Domain.Enums;

namespace Ventrix.Domain.Models
{
    public class InventoryItem
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public ItemCategory Category { get; set; }

        public Condition Condition { get; set; }
        public ItemStatus Status { get; set; }

        public DateTime DateAdded { get; set; } = DateTime.Now;
    }
}