using System;
using System.Collections.Generic;
using System.Text;

namespace Ventrix.Domain
{
    public class InventoryItem
    {
        public int Id { get; set; }
        public string Name { get; set; } // e.g., "Projector A"
        public string Category { get; set; } // "Hardware", "Device"
        public string Condition { get; set; } // "Good", "New"
        public string Status { get; set; } // "Available", "Borrowed", "Maintenance"
        public DateTime DateAdded { get; set; } = DateTime.Now;
    }
}