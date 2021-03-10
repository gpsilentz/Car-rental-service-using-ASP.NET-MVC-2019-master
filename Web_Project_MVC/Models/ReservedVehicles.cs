using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web_Project_MVC.Models
{
    public class ReservedVehicles
    {
        public int RVehicle_Id { get; set; }
        public int Vehicle_Id { get; set; }
        public string RVehicle_StartDate { get; set; }
        public string RVehicle_EndDate { get; set; }
        public bool RVehicle_Activated { get; set; }
        public string RVehicle_ActivateKey { get; set; }
        public string User_UserName { get; set; }
        public int RVehicle_TotalDays { get; set; }
    }
    
    public class ReservedVehicles_Vehicles
    {
        public int RVehicle_Id { get; set; }
        public int Vehicle_Id { get; set; }
        public string RVehicle_StartDate { get; set; }
        public string RVehicle_EndDate { get; set; }
        public bool RVehicle_Activated { get; set; }
        public string RVehicle_ActivateKey { get; set; }
        public string User_UserName { get; set; }
        public int RVehicle_TotalDays { get; set; }

        public string Vehicle_Name { get; set; }
    }
}