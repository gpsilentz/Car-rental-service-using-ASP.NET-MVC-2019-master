using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web_Project_MVC.Models
{
    public class Cart
    {
        public int Cart_Id { get; set; }
        public int Cart_VehicleId { get; set; }
        public string Cart_StartDate { get; set; }
        public string Cart_EndDate { get; set; }
        public bool Cart_InsuranceNeeded { get; set; }
        public bool Card_DriverNeeded { get; set; }
        public string User_UserName { get; set; }
        public int Cart_TotalPay { get; set; }
        public int Cart_TotalDays { get; set; }
        public string Vehicle_Name { get; set; }
        public string Vehicle_Image { get; set; }
    }
}