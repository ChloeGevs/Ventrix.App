using System;
using System.Collections.Generic;
using System.Text;

namespace Ventrix.Domain
{
    public class InventoryItem
    {
        public int Id { get; set; }
        public string Name { get; set; } 
        public string Category { get; set; } 
        public string Condition { get; set; } 
        public string Status { get; set; } 
        public DateTime DateAdded { get; set; } = DateTime.Now;
    }
}