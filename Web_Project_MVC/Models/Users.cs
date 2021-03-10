using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Web_Project_MVC.Models
{
    public class Users
    {
        public int User_Id { get; set; }
        public string User_FirstName { get; set; }
        public string User_LastName { get; set; }
        public string User_UserName { get; set; }
        public string User_EmailAddress { get; set; }
        public string User_Password { get; set; }
        public string User_Address { get; set; }
        public string User_PhoneNumber { get; set; }
        public string User_PostalCode { get; set; }
        public int User_Role { get; set; }
    }
}