using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web_Project_MVC.Models
{
    public class VerificationMessages
    {
        public string Message { get; set; }
        public bool DisplaySuccess { get; set; }
        public bool DisplayError { get; set; }
        public int Result { get; set; }
        public bool NeedToRedirect { get; set; }
        public string RedirectLink { get; set; }
    }
}