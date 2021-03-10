using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;

namespace Web_Project_MVC.Models
{
    public class UsersDbHandle
    {
        private SqlConnection con;
       
        private void Connection()
        {
            string constring = ConfigurationManager.ConnectionStrings["Context"].ToString();
            con = new SqlConnection(constring);
        }
       
        public int AddUser(Users smodel)
        {
            Connection();
            SqlCommand cmd = new SqlCommand("Add_User", con);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@UserName", smodel.User_UserName.Trim());
            cmd.Parameters.AddWithValue("@Password", smodel.User_Password.Trim());
            cmd.Parameters.AddWithValue("@Email", smodel.User_EmailAddress.Trim());
            cmd.Parameters.AddWithValue("@FirstName", smodel.User_FirstName.Trim());
            cmd.Parameters.AddWithValue("@LastName", smodel.User_LastName.Trim());
            cmd.Parameters.AddWithValue("@PhoneNumber", smodel.User_PhoneNumber.Trim());
            cmd.Parameters.AddWithValue("@Address", smodel.User_Address.Trim());
            cmd.Parameters.AddWithValue("@PostalCode", smodel.User_PostalCode.Trim());

            con.Open();
            int i = Convert.ToInt32(cmd.ExecuteScalar());
            con.Close();

            return i;
        }

        public int AddUserAdmin(Users smodel)
        {
            Connection();
            SqlCommand cmd = new SqlCommand("Add_User_Admin", con);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@UserName", smodel.User_UserName.Trim());
            cmd.Parameters.AddWithValue("@Password", smodel.User_Password.Trim());
            cmd.Parameters.AddWithValue("@Email", smodel.User_EmailAddress.Trim());
            cmd.Parameters.AddWithValue("@FirstName", smodel.User_FirstName.Trim());
            cmd.Parameters.AddWithValue("@LastName", smodel.User_LastName.Trim());
            cmd.Parameters.AddWithValue("@PhoneNumber", smodel.User_PhoneNumber.Trim());
            cmd.Parameters.AddWithValue("@Address", smodel.User_Address.Trim());
            cmd.Parameters.AddWithValue("@PostalCode", smodel.User_PostalCode.Trim());
            cmd.Parameters.AddWithValue("@UserRole", smodel.User_Role);

            con.Open();
            int i = cmd.ExecuteNonQuery();
            con.Close();

            return i;
        }

        public List<Users> GetUsers()
        {
            Connection();
            List<Users> UserList = new List<Users>();

            SqlCommand cmd = new SqlCommand("SELECT * FROM Users", con);
            cmd.CommandType = CommandType.Text;
            SqlDataAdapter sd = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();

            con.Open();
            sd.Fill(dt);
            con.Close();

            foreach (DataRow dr in dt.Rows)
            {
                UserList.Add(
                    new Users
                    {
                        User_Id = Convert.ToInt32(dr["User_Id"]),
                        User_UserName = Convert.ToString(dr["User_UserName"]),
                        User_FirstName = Convert.ToString(dr["User_FirstName"]),
                        User_LastName = Convert.ToString(dr["User_LastName"]),
                        User_Address = Convert.ToString(dr["User_Address"]),
                        User_PostalCode = Convert.ToString(dr["User_PostalCode"]),
                        User_EmailAddress = Convert.ToString(dr["User_EmailAddress"]),
                        User_PhoneNumber = Convert.ToString(dr["User_PhoneNumber"]),
                        User_Role = Convert.ToInt32(dr["User_Role"])
                    });
            }

            return UserList;
        }

        public List<Users> GetDetailsForSpecificUser(string UserName)
        {
            Connection();
            List<Users> UserList = new List<Users>();

            SqlCommand cmd = new SqlCommand("SELECT * FROM Users WHERE User_UserName = @UserName", con);
            cmd.CommandType = CommandType.Text;

            cmd.Parameters.AddWithValue("@UserName", UserName.Trim());

            SqlDataAdapter sd = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();

            con.Open();
            sd.Fill(dt);
            con.Close();

            foreach (DataRow dr in dt.Rows)
            {
                UserList.Add(
                    new Users
                    {
                        User_FirstName = Convert.ToString(dr["User_FirstName"]),
                        User_LastName = Convert.ToString(dr["User_LastName"]),
                        User_Address = Convert.ToString(dr["User_Address"]),
                        User_PostalCode = Convert.ToString(dr["User_PostalCode"]),
                        User_PhoneNumber = Convert.ToString(dr["User_PhoneNumber"]),
                        User_EmailAddress = Convert.ToString(dr["User_EmailAddress"])
                    });
            }

            return UserList;
        }

        public List<Users> GetUserList()
        {
            Connection();
            List<Users> UserList = new List<Users>();

            SqlCommand cmd = new SqlCommand("SELECT * FROM Users", con);
            cmd.CommandType = CommandType.Text;

            SqlDataAdapter sd = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();

            con.Open();
            sd.Fill(dt);
            con.Close();

            foreach (DataRow dr in dt.Rows)
            {
                UserList.Add(
                    new Users
                    {
                        User_FirstName = Convert.ToString(dr["User_FirstName"]),
                        User_LastName = Convert.ToString(dr["User_LastName"]),
                        User_Address = Convert.ToString(dr["User_Address"]),
                        User_PostalCode = Convert.ToString(dr["User_PostalCode"]),
                        User_PhoneNumber = Convert.ToString(dr["User_PhoneNumber"]),
                        User_EmailAddress = Convert.ToString(dr["User_EmailAddress"])
                    });
            }

            return UserList;
        }

        public int DeleteUser(string id)
        {
            Connection();
            SqlCommand cmd = new SqlCommand("Delete_User", con);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@User_Id", id);

            con.Open();
            int i = cmd.ExecuteNonQuery();
            con.Close();

            return i;
        }

        public int Update_EmailAddress(string EmailAddress, string UserName)
        {
            Connection();
            SqlCommand cmd = new SqlCommand("Update_User_Email", con);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@IdUsername", UserName.Trim());
            cmd.Parameters.AddWithValue("@Email", EmailAddress.Trim());

            con.Open();
            int i = Convert.ToInt32(cmd.ExecuteScalar());
            con.Close();

            return i;
        }

        public int Update_Address(string Address, string UserName)
        {
            Connection();
            SqlCommand cmd = new SqlCommand("Update_User_Address", con);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@IdUsername", UserName.Trim());
            cmd.Parameters.AddWithValue("@Address", Address.Trim());

            con.Open();
            int i = cmd.ExecuteNonQuery();
            con.Close();

            return i;
        }

        public int Update_PhoneNumber(string PhoneNumber, string UserName)
        {
            Connection();
            SqlCommand cmd = new SqlCommand("Update_User_PhoneNumber", con);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@IdUsername", UserName.Trim());
            cmd.Parameters.AddWithValue("@PhoneNumber", PhoneNumber.Trim());

            con.Open();
            int i = cmd.ExecuteNonQuery();
            con.Close();

            return i;
        }

        public int Update_PostalCode(string PostalCode, string UserName)
        {
            Connection();
            SqlCommand cmd = new SqlCommand("Update_User_PostalCode", con);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@IdUsername", UserName.Trim());
            cmd.Parameters.AddWithValue("@PostalCode", PostalCode.Trim());

            con.Open();
            int i = cmd.ExecuteNonQuery();
            con.Close();

            return i;
        }

        public int Update_Password(string Password, string UserName)
        {
            Connection();
            SqlCommand cmd = new SqlCommand("Update_User_Password", con);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@IdUsername", UserName.Trim());
            cmd.Parameters.AddWithValue("@Password", Password.Trim());

            con.Open();
            int i = cmd.ExecuteNonQuery();
            con.Close();

            return i;
        }

        public int LoginUser(string UserName, string Password)
        {
            Connection();
            SqlCommand cmd = new SqlCommand("Validate_User", con);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@UserName", UserName.Trim());
            cmd.Parameters.AddWithValue("@Password", Password.Trim());

            con.Open();
            int i = Convert.ToInt32(cmd.ExecuteScalar());
            con.Close();

            return i;
        }

        public int AddCreditCard(string CCFullName, string CCNumber, string CCMM, string CCYY, string CCSecretCode, string UserName)
        {
            Connection();
            SqlCommand cmd = new SqlCommand("Add_Card", con);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@CreditCardFullname", CCFullName.Trim());
            cmd.Parameters.AddWithValue("@CreditCardNumber", CCNumber.Trim());
            cmd.Parameters.AddWithValue("@CreditCardMM", CCMM.Trim());
            cmd.Parameters.AddWithValue("@CreditCardYY", CCYY.Trim());
            cmd.Parameters.AddWithValue("@CreditCardCVV", CCSecretCode.Trim());
            cmd.Parameters.AddWithValue("@IdUsername", UserName.Trim());

            con.Open();
            int i = Convert.ToInt32(cmd.ExecuteScalar());
            con.Close();

            return i;
        }

        public bool HaveCreditCard(string UserName, ref string CCNumber)
        {
            Connection();

            SqlCommand cmd = new SqlCommand("SELECT * FROM CreditCards WHERE User_UserName = @UserName", con);
            cmd.CommandType = CommandType.Text;

            cmd.Parameters.AddWithValue("@UserName", UserName.Trim());

            con.Open();

            SqlDataReader reader = cmd.ExecuteReader();

            bool hasRows = false;

            if (reader.Read())
            {
                if (reader.HasRows)
                {
                    string cc_number = reader["CreditCard_Number"].ToString().Trim();
                    CCNumber = "**** **** **** " + cc_number.Substring(cc_number.Length - 4);

                    hasRows = true;
                }

                reader.Close();
            }

            con.Close();

            return hasRows;
        }
    }
}