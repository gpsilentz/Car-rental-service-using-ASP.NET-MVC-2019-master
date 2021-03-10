using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web_Project_MVC.Models
{
    public class Vehicles
    {
        public int Vehicle_Id { get; set; }
        public string Vehicle_Name { get; set; }
        public string Vehicle_Category { get; set; }
        public string Vehicle_Price { get; set; }
        public string Vehicle_VType { get; set; }
        public string Vehicle_TType { get; set; }
        public int Vehicle_Year { get; set; }
        public bool Vehicle_Reserved { get; set; }
        public double Vehicle_CoordinateLat { get; set; }
        public double Vehicle_CoordinateLong { get; set; }
        public string Vehicle_Image { get; set; }
    }
}