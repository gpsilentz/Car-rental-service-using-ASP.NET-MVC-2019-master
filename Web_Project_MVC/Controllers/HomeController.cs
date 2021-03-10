using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data;
using System.Configuration;
using System.Security.Cryptography;
using System.Text;
using System.Data.SqlClient;
using PagedList;
using Web_Project_MVC.Models;

namespace Web_Project_MVC.Controllers
{
    public class HomeController : Controller
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
        public ActionResult Index()
        {
            VehiclesDbHandle dbhandle = new VehiclesDbHandle();
            ViewBag.Vehicles = dbhandle.GetRVehicles();

            return View();
        }

        [HttpPost]
        public ActionResult Index(string Pickup_Date, string Return_Date)
        {
            if (!string.IsNullOrEmpty(Pickup_Date.Trim()) && !string.IsNullOrEmpty(Return_Date.Trim()))
            {
                Session["PickUp_Date_Vehicle"] = Pickup_Date.Trim();
                Session["Return_Date_Vehicle"] = Return_Date.Trim();
            }

            string selectedValue = Request.Form["Select_Categories"].ToString();
            
            return selectedValue == "Choose category..." ? 
                RedirectToAction("Dashboard", "Home") : 
                RedirectToAction("Dashboard", "Home", new { id = selectedValue });
        }

        [HttpGet]
        public ActionResult Register()
        {
            if (Session["User_Name"] != null)
                return RedirectToAction("Index", "Home");

            return View();
        }

        [HttpPost]
        public JsonResult Register(Users smodel, VerificationMessages msg)
        {
            msg.Result = 0;
            msg.Message = string.Empty;
            msg.DisplaySuccess = false;
            msg.DisplayError = false;
            msg.NeedToRedirect = false;

            if (string.IsNullOrEmpty(smodel.User_UserName) ||
                string.IsNullOrEmpty(smodel.User_EmailAddress) ||
                string.IsNullOrEmpty(smodel.User_FirstName) ||
                string.IsNullOrEmpty(smodel.User_LastName) ||
                string.IsNullOrEmpty(smodel.User_Address) ||
                string.IsNullOrEmpty(smodel.User_PostalCode) ||
                (string.IsNullOrEmpty(smodel.User_PhoneNumber) || smodel.User_PhoneNumber.Trim().Length < 6 || smodel.User_PhoneNumber.Trim().Length > 20) ||
                (string.IsNullOrEmpty(smodel.User_Password) || smodel.User_Password.Trim().Length < 6 || smodel.User_Password.Trim().Length > 20))
            {
                msg.DisplayError = true;
                msg.Message = "Invalid!";
                return Json(msg);
            }

            smodel.User_Password = EncryptWithMD5(smodel.User_Password.Trim());

            UsersDbHandle dbHandle = new UsersDbHandle();
            msg.Result = dbHandle.AddUser(smodel);

            if (msg.Result == -1)
            {
                msg.Message = "The username already exists! Please use different username.";
                msg.DisplayError = true;
            }
            else if (msg.Result == -2)
            {
                msg.Message = "The email address which you are trying to use has been already taken.";
                msg.DisplayError = true;
            }
            else
            {
                msg.Message = "The registration has been done successfully! You may now login to the personal area.";
                msg.DisplaySuccess = true;
            }

            return Json(msg);
        }

        [HttpGet]
        public ActionResult Login()
        {
            if (Session["User_Name"] != null)
                return RedirectToAction("Index", "Home");

            return View();
        }

        [HttpPost]
        public JsonResult Login(string UserName, string Password, VerificationMessages msg)
        {
            msg.Result = 0;
            msg.Message = string.Empty;
            msg.RedirectLink = string.Empty;
            msg.DisplaySuccess = false;
            msg.DisplayError = false;
            msg.NeedToRedirect = false;

            if (string.IsNullOrEmpty(UserName) || string.IsNullOrEmpty(Password))
            {
                msg.DisplayError = true;
                msg.Message = "Invalid!";
                return Json(msg);
            }

            Password = EncryptWithMD5(Password.Trim());

            UsersDbHandle dbHandle = new UsersDbHandle();
            msg.Result = dbHandle.LoginUser(UserName, Password);

            if (msg.Result == -1)
            {
                msg.DisplayError = true;
                msg.Message = "The username or password are incorrect. Please try again!";
            }
            else
            {
                Session["User_Role"] = msg.Result;
                Session["User_Name"] = UserName;

                msg.NeedToRedirect = true;
                msg.RedirectLink = Url.Action("Index", "Home");
            }

            return Json(msg);
        }

        [HttpGet]
        public ActionResult Details(string Id)
        {
            if (string.IsNullOrEmpty(Id))
                return RedirectToAction("Index", "Home");

            VehiclesDbHandle dbhandle = new VehiclesDbHandle();
            ViewBag.DetailsVehicle = dbhandle.GetFullDetailsVehicles(Id);

            return View();
        }

        [HttpPost]
        public JsonResult Details(string PickUp_Date, string Return_Date, string Id, VerificationMessages msg)
        {
            if (Session["User_Name"] == null)
                return Json(1);

            if (string.IsNullOrEmpty(PickUp_Date) || string.IsNullOrEmpty(Return_Date))
            {
                msg.DisplayError = true;
                msg.Message = "Please choose date!";
                return Json(msg);
            }
            
            VehiclesDbHandle dbhandle = new VehiclesDbHandle();
            ViewBag.DetailsVehicle = dbhandle.GetFullDetailsVehicles(Id);

            msg.Result = 0;
            msg.Message = string.Empty;
            msg.RedirectLink = string.Empty;
            msg.DisplaySuccess = false;
            msg.DisplayError = false;
            msg.NeedToRedirect = false;

            if (!string.IsNullOrEmpty("Id") && Session["User_Name"] != null)
            {
                msg.Result = dbhandle.AddVehicleToCart(Convert.ToString(Session["User_Name"]).Trim(), Id.Trim(), PickUp_Date.Trim(), Return_Date.Trim());

                if (msg.Result == -1)
                {
                    msg.Message = "You can only have 1 item in the cart!";
                    msg.DisplayError = true;
                }
                else if (msg.Result == -2)
                {
                    msg.Message = "This vehicle has been already reserved!";
                    msg.DisplayError = true;
                }
                else
                {
                    msg.Message = "The vehicle has been successfully added to the cart!";
                    msg.DisplaySuccess = true;
                }
            }
            else
            {
                msg.Message = "Please login to continue!";
                msg.DisplayError = true;
            }

            return Json(msg);
        }

        [HttpGet]
        public ActionResult Dashboard(string Id, string sortby, int page = 1)
        {
            if (!string.IsNullOrEmpty(Id))
            {
                ViewBag.ShowSearchResults = true;
                ViewBag.ShowSearchResultsText = Id;
            }

            VehiclesDbHandle dbhandle = new VehiclesDbHandle();
            return View(dbhandle.GetVehiclesByCategory(Id).ToPagedList(page, 4));
        }

        [HttpGet]
        public ActionResult Exit()
        {
            Session.Abandon();
            Session.Clear();
            Session.RemoveAll();

            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public ActionResult Cart()
        {
            if (Session["User_Name"] == null)
                return RedirectToAction("Index", "Home");

            VehiclesDbHandle dbhandle = new VehiclesDbHandle();
            ViewBag.TestingCart = dbhandle.GetCartItems(Convert.ToString(Session["User_Name"]).Trim());

            return View();
        }

        public ActionResult RemoveItemFromCart(string Id, VerificationMessages msg)
        {
            if (Session["User_Name"] == null || string.IsNullOrEmpty(Id))
                return RedirectToAction("Index", "Home");

            VehiclesDbHandle dbhandle = new VehiclesDbHandle();

            msg.Result = 0;
            msg.Message = string.Empty;
            msg.DisplaySuccess = false;
            msg.DisplayError = false;
            msg.NeedToRedirect = true;

            msg.Result = dbhandle.RemoveItemFromCart(Id, Convert.ToString(Session["User_Name"]));

            msg.Message = "The item has been successfully removed from the cart!";
            msg.DisplaySuccess = true;
            msg.RedirectLink = Url.Action("Index", "Home");

            return Json(msg);
        }

        [HttpPost]
        public JsonResult Cart(VerificationMessages msg)
        {
            if (Session["User_Name"] == null)
                return Json(1);

            VehiclesDbHandle dbhandle = new VehiclesDbHandle();

            msg.Result = 0;
            msg.Message = string.Empty;
            msg.DisplaySuccess = false;
            msg.DisplayError = false;
            msg.NeedToRedirect = false;

            msg.Result = dbhandle.ReserveVehicle(Convert.ToString(Session["User_Name"]));

            if (msg.Result == -1)
            {
                msg.Message = "This vehicle has been already reserved!";
                msg.DisplayError = true;
            }
            else if (msg.Result == 3)
            {
                msg.Message = "The cart is empty!";
                msg.DisplayError = true;
            }
            else
            {
                msg.Message = "You have successfully reserved the vehicle! You may find more information in your profile.";
                msg.DisplaySuccess = true;
            }

            return Json(msg);
        }
    }
}