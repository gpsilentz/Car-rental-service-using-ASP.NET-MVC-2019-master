using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;


namespace Web_Project_MVC.Models
{
    public class LicencesDbHandle
    {
        private SqlConnection con;

        private void Connection()
        {
            string constring = ConfigurationManager.ConnectionStrings["Context"].ToString();
            con = new SqlConnection(constring);
        }

        public int AddLicence(string Licence_Name, string Licence_Number, string Licence_BirthDate, string Licence_IssueDate, string Licence_Category, string UserName, string filepath)
        {
            Connection();
            SqlCommand cmd = new SqlCommand("Add_Licence_User", con);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@LicenceNumber", Licence_Number.Trim());
            cmd.Parameters.AddWithValue("@LicenceName", Licence_Name.Trim());
            cmd.Parameters.AddWithValue("@LicenceBirthDate", Licence_BirthDate.Trim());
            cmd.Parameters.AddWithValue("@LicenceIssueDate", Licence_IssueDate.Trim());
            cmd.Parameters.AddWithValue("@LicenceCategory", Licence_Category.Trim());
            cmd.Parameters.AddWithValue("@LicenceImage", "/Content/images/user_licences/" + UserName.Trim() + filepath);
            cmd.Parameters.AddWithValue("@UserName", UserName.Trim());

            con.Open();
            int i = Convert.ToInt32(cmd.ExecuteScalar());
            con.Close();

            return i;
        }

        public int DeleteLicence(string LicenceId)
        {
            Connection();
            SqlCommand cmd = new SqlCommand("Delete_Licence", con);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@Id", LicenceId.Trim());

            con.Open();
            int i = cmd.ExecuteNonQuery();
            con.Close();

            return i;
        }

        public int ApproveLicence(string LicenceId)
        {
            Connection();
            SqlCommand cmd = new SqlCommand("Approve_Licence", con);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@Id", LicenceId.Trim());

            con.Open();
            int i = cmd.ExecuteNonQuery();
            con.Close();

            return i;
        }

        public bool HaveLicence(string UserName, ref string LicencePath, ref bool LicenceApproved)
        {
            Connection();

            SqlCommand cmd = new SqlCommand("SELECT * FROM Licences WHERE User_UserName = @UserName", con);
            cmd.CommandType = CommandType.Text;

            cmd.Parameters.AddWithValue("@UserName", UserName.Trim());

            con.Open();

            SqlDataReader reader = cmd.ExecuteReader();

            bool hasRows = false;

            if (reader.Read())
            {
                if (reader.HasRows)
                {
                    LicencePath = Convert.ToString(reader["Licence_Image"]);
                    LicenceApproved = Convert.ToBoolean(reader["Licence_Approved"]);
                    hasRows = true;
                }

                reader.Close();
            }

            con.Close();

            return hasRows;
        }

        public List<Licences> GetLicenceDetails()
        {
            Connection();
            List<Licences> LicencesList = new List<Licences>();

            SqlCommand cmd = new SqlCommand("SELECT * FROM Licences", con);
            cmd.CommandType = CommandType.Text;
            SqlDataAdapter sd = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();

            con.Open();
            sd.Fill(dt);
            con.Close();

            foreach (DataRow dr in dt.Rows)
            {
                LicencesList.Add(
                    new Licences
                    {
                        Licence_Id = Convert.ToInt32(dr["Licence_Id"]),
                        User_UserName = Convert.ToString(dr["User_UserName"]),
                        Licence_BirthDate = Convert.ToString(dr["Licence_BirthDate"]),
                        Licence_Category = Convert.ToString(dr["Licence_Category"]),
                        Licence_IssueDate = Convert.ToString(dr["Licence_IssueDate"]),
                        Licence_Image = Convert.ToString(dr["Licence_Image"]),
                        Licence_Number = Convert.ToString(dr["Licence_Number"]),
                        Licence_Name = Convert.ToString(dr["Licence_Name"]),
                        Licence_Approved = Convert.ToBoolean(dr["Licence_Approved"])
                    });
            }

            return LicencesList;
        }
    }
}