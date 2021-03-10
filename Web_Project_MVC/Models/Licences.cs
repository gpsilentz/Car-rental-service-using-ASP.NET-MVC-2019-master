using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web_Project_MVC.Models
{
    public class Licences
    {
        public int Licence_Id { get; set; }
        public string Licence_Name { get; set; }
        public string Licence_Number { get; set; }
        public string Licence_BirthDate { get; set; }
        public string Licence_IssueDate { get; set; }
        public string Licence_Category { get; set; }
        public string Licence_Image { get; set; }
        public bool Licence_Approved { get; set; }
        public string User_UserName { get; set; }
    }
}