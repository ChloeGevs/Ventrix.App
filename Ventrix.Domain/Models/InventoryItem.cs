using System;
using System.ComponentModel.DataAnnotations;
using Ventrix.Domain.Enums;

namespace Ventrix.Domain.Models
{
    public class InventoryItem
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public ItemCategory Category { get; set; }

        [Required]
        public Condition Condition { get; set; }

        [Required]
        public ItemStatus Status { get; set; }

        public DateTime DateAdded { get; set; } = DateTime.Now;
    }
}