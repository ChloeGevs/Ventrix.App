using System;

namespace Ventrix.Domain.Models
{
    public enum ItemStatus
    {
        Available,
        Borrowed,
        Maintenance,
        Damaged
    }

    public enum ItemCategory
    {
        Hardware,
        Device,
        Accessory,
        Consumable
    }

    public class InventoryItem
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public ItemCategory Category { get; set; }

        public string Condition { get; set; }

        public ItemStatus Status { get; set; }

        public DateTime DateAdded { get; set; } = DateTime.Now;
    }
}