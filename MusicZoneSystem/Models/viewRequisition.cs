using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MusicZoneSystem.Models
{
    public class viewRequisition
    {
        // For viewing existing data
        public int CanvasID { get; set; }
        public string CanvasItem { get; set; }
        public string CanvasDesc { get; set; }
        public int CanvasQuantity { get; set; }
        public string CanvasUnit { get; set; }
        public decimal CanvasPrice { get; set; }
        public decimal CanvasTotal { get; set; }
        public int ItemDelete { get; set; } //ID to delete
        public int ItemEdit_ID { get; set; } //ID to edit
        public int ItemEdit_Qty { get; set; } //Quantity to edit
        public string ItemEdit_Unit { get; set; } //Unit to edit
        public decimal ItemEdit_Total { get; set; } //Total cost to edit

    }

    public class EditItemViewModel
    {
        public int CanvasID { get; set; }
        public string CanvasItem { get; set; }
        public string CanvasDesc { get; set; }
        public int CanvasQuantity { get; set; }
        public string CanvasUnit { get; set; }
        public decimal CanvasPrice { get; set; }
        public decimal CanvasTotal { get; set; }
    }
}