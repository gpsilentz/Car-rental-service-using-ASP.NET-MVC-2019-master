using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;

namespace Web_Project_MVC.Models
{
    public class VehiclesDbHandle
    {
        private SqlConnection con;
        
        private void Connection()
        {
            string constring = ConfigurationManager.ConnectionStrings["Context"].ToString();
            con = new SqlConnection(constring);
        }

        public int AddVehicleToCart(string UserName, string VehicleId, string PickUp_Date, string Return_Date)
        {
            Connection();
            SqlCommand cmd = new SqlCommand("Add_Vehicle_ToCart", con);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@UserName", UserName.Trim());
            cmd.Parameters.AddWithValue("@VehicleId", VehicleId.Trim());
            cmd.Parameters.AddWithValue("@PickUp_Date", PickUp_Date.Trim());
            cmd.Parameters.AddWithValue("@Return_Date", Return_Date.Trim());

            con.Open();
            int i = Convert.ToInt32(cmd.ExecuteScalar());
            con.Close();

            return i;
        }

        public int DeleteVehicle(string VehicleId)
        {
            Connection();
            SqlCommand cmd = new SqlCommand("Delete_Vehicle", con);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@Id", VehicleId.Trim());

            con.Open();
            int i = cmd.ExecuteNonQuery();
            con.Close();

            return i;
        }

        public int AddVehicleAdmin(string VehicleName, string VehiclePrice, string VehicleCategories, string VehicleType, string VehicleReFillType, string VehicleYear, string VehicleLong, string VehicleLat, string UserName, string extension)
        {
            Connection();
            SqlCommand cmd = new SqlCommand("Add_Vehicle", con);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@VehicleName", VehicleName.Trim());
            cmd.Parameters.AddWithValue("@VehiclePrice", VehiclePrice.Trim());
            cmd.Parameters.AddWithValue("@VehicleCategory", VehicleCategories.Trim());
            cmd.Parameters.AddWithValue("@VehicleVType", VehicleType.Trim());
            cmd.Parameters.AddWithValue("@VehicleTType", VehicleReFillType.Trim());
            cmd.Parameters.AddWithValue("@VehicleYear", VehicleYear.Trim());
            cmd.Parameters.AddWithValue("@VehicleCoorX", VehicleLong.Trim());
            cmd.Parameters.AddWithValue("@VehicleCoorY", VehicleLat.Trim());
            cmd.Parameters.AddWithValue("@VehicleImage", "/Content/images/vehiclesclients/" + VehicleName.Trim() + UserName.Trim() + extension);

            con.Open();
            int i = cmd.ExecuteNonQuery();
            con.Close();

            return i;
        }

        public bool HaveItemsInCart(ref int VId, ref DateTime PickUp_Date, ref DateTime Return_Date, string UserName)
        {
            Connection();

            SqlCommand cmd = new SqlCommand("SELECT * FROM Cart INNER JOIN Vehicles ON Cart.Cart_VehicleId = Vehicles.Vehicle_Id WHERE Cart.User_UserName = @UserName", con);
            cmd.CommandType = CommandType.Text;

            cmd.Parameters.AddWithValue("@UserName", UserName.Trim());

            con.Open();

            SqlDataReader reader = cmd.ExecuteReader();

            bool hasRows = false;

            if (reader.Read())
            {
                if (reader.HasRows)
                {
                    VId = Convert.ToInt32(reader["Vehicle_Id"]);
                    PickUp_Date = Convert.ToDateTime(reader["Cart_StartDate"]);
                    Return_Date = Convert.ToDateTime(reader["Cart_EndDate"]);
                    hasRows = true;
                }

                reader.Close();
            }

            con.Close();

            return hasRows;
        }

        public int RemoveItemFromCart(string Id, string UserName)
        {
            Connection();

            SqlCommand cmd = new SqlCommand("Delete_Item_FromCart", con);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@User_Id", UserName.Trim());
            cmd.Parameters.AddWithValue("@VehicleId", Id.Trim());

            con.Open();
            int i = Convert.ToInt32(cmd.ExecuteScalar());
            con.Close();

            return i;
        }

        public int ReserveVehicle(string UserName)
        {
            int vId = 0;

            DateTime PickUp_Date = DateTime.Today;
            DateTime Return_Date = DateTime.Today;

            if (!HaveItemsInCart(ref vId, ref PickUp_Date, ref Return_Date, UserName))
                return 3;

            Random r = new Random();
            int num = r.Next();

            string key_to_activate = num.ToString().Trim();

            Connection();
            SqlCommand cmd = new SqlCommand("Reserve_Vehicle_User", con);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@UserName", UserName.Trim());
            cmd.Parameters.AddWithValue("@ReserveId", vId);
            cmd.Parameters.AddWithValue("@PickUp_Date", PickUp_Date);
            cmd.Parameters.AddWithValue("@Return_Date", Return_Date);
            cmd.Parameters.AddWithValue("@Key_To_Activate", key_to_activate);

            con.Open();
            int i = Convert.ToInt32(cmd.ExecuteScalar());
            con.Close();

            return i;
        }

        public List<Cart> GetCartItems(string UserName)
        {
            Connection();

            SqlCommand cmd = new SqlCommand("SELECT * FROM Cart INNER JOIN Vehicles ON Cart.Cart_VehicleId = Vehicles.Vehicle_Id WHERE Cart.User_UserName = @UserName", con);
            cmd.CommandType = CommandType.Text;

            cmd.Parameters.AddWithValue("@UserName", UserName.Trim());

            SqlDataAdapter sd = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();

            con.Open();
            sd.Fill(dt);
            con.Close();

            List<Cart> CartList = new List<Cart>();
            foreach (DataRow dr in dt.Rows)
            {
                DateTime PickUpDate = Convert.ToDateTime(dr["Cart_StartDate"].ToString().Trim());
                DateTime ReturnDate = Convert.ToDateTime(dr["Cart_EndDate"].ToString().Trim());

                int total_days = (ReturnDate - PickUpDate).Days;
                int total_price = Convert.ToInt32(dr["Vehicle_Price"]) * total_days;

                CartList.Add(new Cart
                {
                    Vehicle_Name = Convert.ToString(dr["Vehicle_Name"]),
                    Vehicle_Image = Convert.ToString(dr["Vehicle_Image"]),
                    Cart_Id = Convert.ToInt32(dr["Cart_Id"]),
                    Cart_VehicleId = Convert.ToInt32(dr["Cart_VehicleId"]),
                    Cart_StartDate = PickUpDate.ToString("dd-MM-yyyy"),
                    Cart_EndDate = ReturnDate.ToString("dd-MM-yyyy"),
                    User_UserName = Convert.ToString(dr["User_UserName"]),
                    Cart_TotalPay = total_price,
                    Cart_TotalDays = total_days
                });
            }

            return CartList;
        }

        public List<Vehicles> GetRVehicles()
        {
            Connection();

            SqlCommand cmd = new SqlCommand("SELECT TOP 6 * FROM Vehicles WHERE Vehicle_Reserved = 0 ORDER BY NEWID()", con);
            cmd.CommandType = CommandType.Text;

            SqlDataAdapter sd = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();

            con.Open();
            sd.Fill(dt);
            con.Close();
            
            List<Vehicles> VehicleList = new List<Vehicles>();
            foreach (DataRow dr in dt.Rows)
            {
                VehicleList.Add(new Vehicles
                {
                    Vehicle_Id = Convert.ToInt32(dr["Vehicle_Id"]),
                    Vehicle_Category = Convert.ToString(dr["Vehicle_Category"]),
                    Vehicle_Name = Convert.ToString(dr["Vehicle_Name"]),
                    Vehicle_Image = Convert.ToString(dr["Vehicle_Image"]),
                    Vehicle_Price = Convert.ToString(dr["Vehicle_Price"]),
                    Vehicle_TType = Convert.ToString(dr["Vehicle_TType"]),
                    Vehicle_VType = Convert.ToString(dr["Vehicle_VType"]),
                    Vehicle_Year = Convert.ToInt32(dr["Vehicle_Year"])
                });
            }

            return VehicleList;
        }

        public List<Vehicles> GetAllVehicles()
        {
            Connection();

            SqlCommand cmd = new SqlCommand("SELECT * FROM Vehicles", con);
            cmd.CommandType = CommandType.Text;

            SqlDataAdapter sd = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();

            con.Open();
            sd.Fill(dt);
            con.Close();
            
            List<Vehicles> VehicleList = new List<Vehicles>();
            foreach (DataRow dr in dt.Rows)
            {
                VehicleList.Add(new Vehicles
                {
                    Vehicle_Id = Convert.ToInt32(dr["Vehicle_Id"]),
                    Vehicle_Category = Convert.ToString(dr["Vehicle_Category"]),
                    Vehicle_Name = Convert.ToString(dr["Vehicle_Name"]),
                    Vehicle_Image = Convert.ToString(dr["Vehicle_Image"]),
                    Vehicle_Price = Convert.ToString(dr["Vehicle_Price"]),
                    Vehicle_TType = Convert.ToString(dr["Vehicle_TType"]),
                    Vehicle_VType = Convert.ToString(dr["Vehicle_VType"]),
                    Vehicle_Year = Convert.ToInt32(dr["Vehicle_Year"])
                });
            }

            return VehicleList;
        }

        public List<Vehicles> GetFullDetailsVehicles(string Id)
        {
            Connection();

            SqlCommand cmd = new SqlCommand("SELECT * FROM Vehicles WHERE Vehicle_Id = @Id", con);
            cmd.CommandType = CommandType.Text;

            cmd.Parameters.AddWithValue("@Id", Id.Trim());

            SqlDataAdapter sd = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();

            con.Open();
            sd.Fill(dt);
            con.Close();
            
            List<Vehicles> VehicleList = new List<Vehicles>();
            foreach (DataRow dr in dt.Rows)
            {
                VehicleList.Add(
                    new Vehicles
                    {
                        Vehicle_Id = Convert.ToInt32(dr["Vehicle_Id"]),
                        Vehicle_Category = Convert.ToString(dr["Vehicle_Category"]),
                        Vehicle_Name = Convert.ToString(dr["Vehicle_Name"]),
                        Vehicle_Image = Convert.ToString(dr["Vehicle_Image"]),
                        Vehicle_Price = Convert.ToString(dr["Vehicle_Price"]),
                        Vehicle_TType = Convert.ToString(dr["Vehicle_TType"]),
                        Vehicle_VType = Convert.ToString(dr["Vehicle_VType"]),
                        Vehicle_Year = Convert.ToInt32(dr["Vehicle_Year"]),
                        Vehicle_Reserved = Convert.ToBoolean(dr["Vehicle_Reserved"]),
                        Vehicle_CoordinateLat = Convert.ToInt32(dr["Vehicle_CoordinateLat"]),
                        Vehicle_CoordinateLong = Convert.ToInt32(dr["Vehicle_CoordinateLong"])
                    });
            }

            return VehicleList;
        }

        public List<Vehicles> GetVehiclesByCategory(string Id)
        {
            Connection();

            string query_str = string.Empty;
            string urlstr = HttpContext.Current.Request.QueryString["sortby"];

            if (!string.IsNullOrEmpty(Id) && urlstr != null && urlstr == "lowestprice")
                query_str = "SELECT * FROM Vehicles WHERE Vehicle_Reserved = 0 AND Vehicle_Category = @Category ORDER BY Vehicle_Price";  
            else if (!string.IsNullOrEmpty(Id) && urlstr != null && urlstr == "highestprice")
                query_str = "SELECT * FROM Vehicles WHERE Vehicle_Reserved = 0 AND Vehicle_Category = @Category ORDER BY Vehicle_Price DESC";  
            else if (!string.IsNullOrEmpty(Id) && urlstr == null)
                query_str = "SELECT * FROM Vehicles WHERE Vehicle_Reserved = 0 AND Vehicle_Category = @Category";
            else if (string.IsNullOrEmpty(Id) && urlstr != null && urlstr == "lowestprice")
                query_str = "SELECT * FROM Vehicles WHERE Vehicle_Reserved = 0 ORDER BY Vehicle_Price";
            else if (string.IsNullOrEmpty(Id) && urlstr != null && urlstr == "highestprice")
                query_str = "SELECT * FROM Vehicles WHERE Vehicle_Reserved = 0 ORDER BY Vehicle_Price DESC";
            else
                query_str = "SELECT * FROM Vehicles WHERE Vehicle_Reserved = 0";

            SqlCommand cmd = new SqlCommand(query_str, con);
            
            cmd.CommandType = CommandType.Text;

            if (!string.IsNullOrEmpty(Id))
                cmd.Parameters.AddWithValue("@Category", Id.Trim());

            SqlDataAdapter sd = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();

            con.Open();
            sd.Fill(dt);
            con.Close();

            List<Vehicles> VehicleList = new List<Vehicles>();
            foreach (DataRow dr in dt.Rows)
            {
                VehicleList.Add(
                    new Vehicles
                    {
                        Vehicle_Id = Convert.ToInt32(dr["Vehicle_Id"]),
                        Vehicle_Category = Convert.ToString(dr["Vehicle_Category"]),
                        Vehicle_Name = Convert.ToString(dr["Vehicle_Name"]),
                        Vehicle_Image = Convert.ToString(dr["Vehicle_Image"]),
                        Vehicle_Price = Convert.ToString(dr["Vehicle_Price"]),
                        Vehicle_TType = Convert.ToString(dr["Vehicle_TType"]),
                        Vehicle_VType = Convert.ToString(dr["Vehicle_VType"]),
                        Vehicle_Year = Convert.ToInt32(dr["Vehicle_Year"]),
                        Vehicle_Reserved = Convert.ToBoolean(dr["Vehicle_Reserved"]),
                        Vehicle_CoordinateLat = Convert.ToInt32(dr["Vehicle_CoordinateLat"]),
                        Vehicle_CoordinateLong = Convert.ToInt32(dr["Vehicle_CoordinateLong"])
                    });
            }

            return VehicleList;
        }

        public List<ReservedVehicles_Vehicles> GetReservedVehiclesForSpecificUser(string UserName)
        {
            Connection();

            SqlCommand cmd = new SqlCommand("SELECT * FROM ReservedVehicles INNER JOIN Vehicles ON ReservedVehicles.Vehicle_Id = Vehicles.Vehicle_Id WHERE ReservedVehicles.User_UserName = @UserName", con);
            cmd.CommandType = CommandType.Text;

            cmd.Parameters.AddWithValue("@UserName", UserName.Trim());

            SqlDataAdapter sd = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();

            con.Open();
            sd.Fill(dt);
            con.Close();

            List<ReservedVehicles_Vehicles> VehicleListL = new List<ReservedVehicles_Vehicles>();
            foreach (DataRow dr in dt.Rows)
            {
                DateTime PickUpDate = Convert.ToDateTime(dr["RVehicle_StartDate"].ToString().Trim());
                DateTime ReturnDate = Convert.ToDateTime(dr["RVehicle_EndDate"].ToString().Trim());

                VehicleListL.Add(
                    new ReservedVehicles_Vehicles
                    {
                        Vehicle_Id = Convert.ToInt32(dr["Vehicle_Id"]),
                        Vehicle_Name = Convert.ToString(dr["Vehicle_Name"]),
                        RVehicle_StartDate = PickUpDate.ToString("dd-MM-yyyy"),
                        RVehicle_EndDate = ReturnDate.ToString("dd-MM-yyyy"),
                        RVehicle_Activated = Convert.ToBoolean(dr["RVehicle_Activated"]),
                        RVehicle_ActivateKey = Convert.ToString(dr["RVehicle_ActivateKey"])
                    });
            }

            return VehicleListL;
        }

        public int DeleteReservedVehicleForUser(string UserName, string Id)
        {
            Connection();

            SqlCommand cmd = new SqlCommand("Delete_ReserveVehicle_User", con);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@UserName", UserName.Trim());
            cmd.Parameters.AddWithValue("@VehicleId", Convert.ToInt32(Id));

            con.Open();
            int i = Convert.ToInt32(cmd.ExecuteScalar());
            con.Close();

            return i;
        }
    }
}