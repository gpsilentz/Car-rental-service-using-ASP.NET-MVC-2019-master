using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web_Project_MVC.Models;
using System.IO;

namespace Web_Project_MVC.Controllers
{
    public class AdminController : Controller
    {
        [HttpGet]
        public ActionResult Index(string id, string article)
        {
            if (Session["User_Name"] == null || Session["User_Role"] == null || Convert.ToInt32(Session["User_Role"]) != 1)
                return RedirectToAction("Index", "Home");

            VehiclesDbHandle vehicledbhandle = new VehiclesDbHandle();
            UsersDbHandle usersdbhandle = new UsersDbHandle();
            LicencesDbHandle licencesdbhandle = new LicencesDbHandle();

            if (!string.IsNullOrEmpty(id))
            {
                if (id == "1") 
                {
                    ViewBag.AllUsersCount = usersdbhandle.GetUsers().Count();
                    ViewBag.AllVehiclesCount = vehicledbhandle.GetAllVehicles().Count();
                    ViewBag.DisplayDashboard = true;
                }
                else if (id == "2") 
                {
                    ViewBag.AllUsers = usersdbhandle.GetUsers();
                    ViewBag.DisplayManageUsers = true;
                }
                else if (id == "3") 
                {
                    ViewBag.AllVehicles = vehicledbhandle.GetAllVehicles();
                    ViewBag.DisplayManageVehicles = true;
                }
                else if (id == "4") 
                {      
                    ViewBag.AllLicences = licencesdbhandle.GetLicenceDetails();
                    ViewBag.DisplayManageLicences = true;
                }
            }
            else 
            {
                ViewBag.AllUsersCount = usersdbhandle.GetUsers().Count();
                ViewBag.AllVehiclesCount = vehicledbhandle.GetAllVehicles().Count();
                ViewBag.DisplayDashboard = true;
            }

            return View();
        }

        [HttpPost]
        public JsonResult UploadVehicle(string VehicleName, string VehiclePrice, string VehicleYear, string VehicleLong, string VehicleLat, HttpPostedFileBase FileVehicle, FormCollection form, VerificationMessages msg)
        {
            VehiclesDbHandle dbhandle = new VehiclesDbHandle();

            msg.Result = 0;
            msg.Message = string.Empty;
            msg.DisplaySuccess = false;
            msg.DisplayError = false;
            msg.NeedToRedirect = false;

            if (string.IsNullOrEmpty(VehicleName.Trim()) ||
                string.IsNullOrEmpty(VehiclePrice.Trim()) ||
                string.IsNullOrEmpty(VehicleYear.Trim()) ||
                string.IsNullOrEmpty(VehicleLong.Trim()) ||
                string.IsNullOrEmpty(VehicleLat.Trim()))
            {
                msg.DisplayError = true;
                msg.Message = "Invalid!";
                return Json(msg);
            }

            string VehicleCategories = form["VehicleCategories"].ToString();
            string VehicleType = form["VehicleType"].ToString();
            string VehicleReFillType = form["VehicleReFillType"].ToString();

            string extension = Path.GetExtension(FileVehicle.FileName);
            FileVehicle.SaveAs(Server.MapPath("/Content/images/vehiclesclients/") + VehicleName.Trim() + Convert.ToString(Session["User_Name"]).Trim() + extension);

            msg.Result = dbhandle.AddVehicleAdmin(VehicleName, VehiclePrice, VehicleCategories, VehicleType, VehicleReFillType, VehicleYear, VehicleLong, VehicleLat, Convert.ToString(Session["User_Name"]).Trim(), extension);

            msg.DisplaySuccess = true;
            msg.Message = "The vehicle has been successfully added!";

            return Json(msg);
        }

        [HttpPost]
        public JsonResult DeleteVehicle(string Id, VerificationMessages msg)
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

            msg.Result = dbhandle.DeleteVehicle(Id.Trim());

            msg.DisplaySuccess = true;
            msg.Message = "The vehicle has been successfully deleted!";

            return Json(msg);
        }

        [HttpPost]
        public JsonResult DeleteUser(string Id, VerificationMessages msg)
        {
            UsersDbHandle dbhandle = new UsersDbHandle();

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

            msg.Result = dbhandle.DeleteUser(Id.Trim());

            msg.DisplaySuccess = true;
            msg.Message = "The user has been successfully deleted!";

            return Json(msg);
        }

        [HttpPost]
        public JsonResult DeleteLicence(string Id, VerificationMessages msg)
        {
            LicencesDbHandle dbhandle = new LicencesDbHandle();

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

            msg.Result = dbhandle.DeleteLicence(Id.Trim());

            msg.DisplaySuccess = true;
            msg.Message = "The licence has been successfully deleted!";

            return Json(msg);
        }

        [HttpPost]
        public JsonResult ApproveLicence(string Id, VerificationMessages msg)
        {
            LicencesDbHandle dbhandle = new LicencesDbHandle();

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

            msg.Result = dbhandle.ApproveLicence(Id.Trim());

            msg.DisplaySuccess = true;
            msg.Message = "The licence has been successfully approved!";

            return Json(msg);
        }
    }
}