using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web_Project_MVC.Models;
using System.Data;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Web_Project_MVC.Controllers
{
    public class ProfileController : Controller
    {
        public static string EncryptWithMD5(string password)
        {
            MD5 md5 = new MD5CryptoServiceProvider();
            UTF8Encoding encoder = new UTF8Encoding();

            Byte[] originalBytes = encoder.GetBytes(password);
            Byte[] encodedBytes = md5.ComputeHash(originalBytes);

            password = BitConverter.ToString(encodedBytes).Replace("-", "");
            var result = password.ToLower();

            return result;
        }

        [HttpGet]
        public ActionResult Index(string id)
        {
            if (Session["User_Name"] == null)
                return RedirectToAction("Index", "Home");

            VehiclesDbHandle vehicledbhandle = new VehiclesDbHandle();
            UsersDbHandle dbhandle = new UsersDbHandle();
            LicencesDbHandle licdbhandle = new LicencesDbHandle();

            List<Users> ListUsers = dbhandle.GetDetailsForSpecificUser(Convert.ToString(Session["User_Name"]));
            TempData["User_Details"] = ListUsers;

            var firstordefault = ListUsers.FirstOrDefault();

            ViewBag.EmailAddress = firstordefault.User_EmailAddress;
            ViewBag.Address = firstordefault.User_Address;
            ViewBag.PostalCode = firstordefault.User_PostalCode;
            ViewBag.PhoneNumber = firstordefault.User_PhoneNumber;

            ViewBag.ReservedVehicles = vehicledbhandle.GetReservedVehiclesForSpecificUser(Convert.ToString(Session["User_Name"]));

            string LicencePath = string.Empty;
            string CreditCardNumber = string.Empty;
            bool LicenceApproved = false;

            if (licdbhandle.HaveLicence(Convert.ToString(Session["User_Name"]), ref LicencePath, ref LicenceApproved))
            {
                ViewBag.HaveLicence = true;
                ViewBag.LicenceApproved = LicenceApproved;
                ViewBag.LicencePath = LicencePath;
            }

            if (dbhandle.HaveCreditCard(Convert.ToString(Session["User_Name"]), ref CreditCardNumber))
            {
                ViewBag.HaveCreditCard = true;
                ViewBag.CreditCardNumber = CreditCardNumber;
            }

            if (!string.IsNullOrEmpty(id))
            {
                if (id == "1")
                    ViewBag.DisplayManageProfile = true;
                else if (id == "2")
                    ViewBag.DisplayReservedVehicles = true;
                else if (id == "3")
                    ViewBag.DisplayLicence = true;
                else if (id == "4")
                    ViewBag.DisplayPaymentDetails = true;
            }
            else
                ViewBag.DisplayManageProfile = true;

            return View();
        }

        [HttpPost]
        public JsonResult SaveUserDetails(string EmailAddressRechek, string PhoneNumberRechek, string AddressRechek, string PostalCodeRechek, VerificationMessages msg)
        {
            UsersDbHandle dbhandle = new UsersDbHandle();

            msg.Result = 3;
            msg.Message = string.Empty;
            msg.DisplaySuccess = false;
            msg.DisplayError = false;
            msg.NeedToRedirect = false;
            
            if (((TempData["User_Details"] as IEnumerable<Users>).FirstOrDefault().User_EmailAddress) != EmailAddressRechek.Trim())
            {
                if (string.IsNullOrEmpty(EmailAddressRechek))
                    msg.Result = -2;
                else 
                {
                    msg.Result = dbhandle.Update_EmailAddress(EmailAddressRechek.Trim(), Convert.ToString(Session["User_Name"]));

                    if (msg.Result == -1)
                    {
                        msg.DisplayError = true;
                        msg.Message = "The email already exist! Please use different email address.";
                    }
                }
            }

            if (((TempData["User_Details"] as IEnumerable<Users>).FirstOrDefault().User_PhoneNumber) != PhoneNumberRechek.Trim())
            {
                if (string.IsNullOrEmpty(PhoneNumberRechek))
                    msg.Result = -2;
                else
                    msg.Result = dbhandle.Update_PhoneNumber(PhoneNumberRechek.Trim(), Convert.ToString(Session["User_Name"]));
            }

            if (((TempData["User_Details"] as IEnumerable<Users>).FirstOrDefault().User_Address) != AddressRechek.Trim())
            {
                if (string.IsNullOrEmpty(AddressRechek))
                    msg.Result = -2;
                else
                    msg.Result = dbhandle.Update_Address(AddressRechek.Trim(), Convert.ToString(Session["User_Name"]));
            }

            if (((TempData["User_Details"] as IEnumerable<Users>).FirstOrDefault().User_PostalCode) != PostalCodeRechek.Trim())
            {
                if (string.IsNullOrEmpty(PostalCodeRechek))
                    msg.Result = -2;
                else
                    msg.Result = dbhandle.Update_PostalCode(PostalCodeRechek.Trim(), Convert.ToString(Session["User_Name"]));
            }

            if (msg.Result == -2)
            {
                msg.DisplayError = true;
                msg.Message = "Please fill up the needed information!";
            }
            else if (msg.Result != 3)
            {
                msg.DisplaySuccess = true;
                msg.Message = "The information has been successfully changed!";
            }

            TempData["User_Details"] = dbhandle.GetDetailsForSpecificUser(Convert.ToString(Session["User_Name"]));

            return Json(msg);
        }

        [HttpPost]
        public JsonResult SaveUserPassword(string PasswordRechek, VerificationMessages msg)
        {
            UsersDbHandle dbhandle = new UsersDbHandle();

            msg.Result = 0;
            msg.Message = string.Empty;
            msg.DisplaySuccess = false;
            msg.DisplayError = false;
            msg.NeedToRedirect = false;

            if (string.IsNullOrEmpty(PasswordRechek) || PasswordRechek.Trim().Length < 6)
                msg.Result = -2;
            else
            {
                PasswordRechek = EncryptWithMD5(PasswordRechek);
                msg.Result = dbhandle.Update_Password(PasswordRechek.Trim(), Convert.ToString(Session["User_Name"]).Trim());
            }
            
            if (msg.Result == -2)
            {
                msg.DisplayError = true;
                msg.Message = "Please fill up the password!";
            }
            else
            {
                msg.DisplaySuccess = true;
                msg.Message = "The password has been successfully changed!";
            }

            return Json(msg);
        }

        [HttpPost]
        public JsonResult UploadUserLicence(string NumberLic, string NameLic, string BirthDateLic, string IssueDateLic, string CategoryLic, HttpPostedFileBase FileLic, VerificationMessages msg)
        {
            LicencesDbHandle dbhandle = new LicencesDbHandle();

            msg.Result = 0;
            msg.Message = string.Empty;
            msg.DisplaySuccess = false;
            msg.DisplayError = false;
            msg.NeedToRedirect = false;

            if (string.IsNullOrEmpty(NumberLic) || 
                string.IsNullOrEmpty(NameLic) || 
                string.IsNullOrEmpty(BirthDateLic) ||
                string.IsNullOrEmpty(IssueDateLic) ||
                string.IsNullOrEmpty(CategoryLic))
            {
                msg.DisplayError = true;
                msg.Message = "Invalid!";
                return Json(msg);
            }

            string extension = Path.GetExtension(FileLic.FileName);
            FileLic.SaveAs(Server.MapPath("/Content/images/user_licences/") + Convert.ToString(Session["User_Name"]).Trim() + extension);

            msg.Result = dbhandle.AddLicence(NameLic, NumberLic, BirthDateLic, IssueDateLic, CategoryLic, Convert.ToString(Session["User_Name"]).Trim(), extension);

            if (msg.Result == -1)
            {
                msg.DisplayError = true;
                msg.Message = "The licence has been been already added to your account!";
            }
            else
            {
                msg.DisplaySuccess = true;
                msg.Message = "The licence has been successfully added!";
            }

            return Json(msg);
        }

        [HttpPost]
        public JsonResult DeleteReservedVehicle(string Id, VerificationMessages msg)
        {
            VehiclesDbHandle dbhandle = new VehiclesDbHandle();

            msg.Result = 0;
            msg.Message = string.Empty;
            msg.DisplaySuccess = false;
            msg.DisplayError = false;
            msg.NeedToRedirect = false;

            if (string.IsNullOrEmpty(Id.Trim()))
            {
                msg.DisplayError = true;
                msg.Message = "Invalid!";
                return Json(msg);
            }

            msg.Result = dbhandle.DeleteReservedVehicleForUser(Convert.ToString(Session["User_Name"]).Trim(), Id.Trim());

            msg.DisplaySuccess = true;
            msg.Message = "The vehicle has been deleted!";

            return Json(msg);
        }

        public JsonResult AddCreditCard(string CCFullName, string CCNumber, string CCMM, string CCYY, string CCSecretCode, VerificationMessages msg)
        {
            UsersDbHandle dbhandle = new UsersDbHandle();

            msg.Result = 0;
            msg.Message = string.Empty;
            msg.DisplaySuccess = false;
            msg.DisplayError = false;
            msg.NeedToRedirect = false;

            if (string.IsNullOrEmpty(CCFullName) ||
                string.IsNullOrEmpty(CCNumber) ||
                string.IsNullOrEmpty(CCMM) ||
                string.IsNullOrEmpty(CCYY) ||
                string.IsNullOrEmpty(CCSecretCode))
            {
                msg.DisplayError = true;
                msg.Message = "Invalid!";
                return Json(msg);
            }

            msg.Result = dbhandle.AddCreditCard(CCFullName, CCNumber, CCMM, CCYY, CCSecretCode, Convert.ToString(Session["User_Name"]).Trim());

            if (msg.Result == -1)
            {
                msg.DisplayError = true;
                msg.Message = "You have already added a credit card to your account! Please remove it and try again.";
            }
            else
            {
                msg.DisplaySuccess = true;
                msg.Message = "The credit card has been successfully added to your account!";
            }

            return Json(msg);
        }
    }
}