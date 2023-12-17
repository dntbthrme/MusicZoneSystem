using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MusicZoneSystem.Models
{
    public class viewRequisition
    {
        // For viewing existing data
        public string CanvasItem { get; set; }
        public string CanvasDesc { get; set; }
        public int CanvasQuantity { get; set; }
        public string CanvasUnit { get; set; }
        public decimal CanvasPrice { get; set; }
        public decimal CanvasTotal { get; set; }
        public string ItemDelete { get; set; }

    }
}