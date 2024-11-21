using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace AplicationForWarehouse_v2.Tools
{
    public class ToolsFunction
    {
        public static RequestClient Takeinfo(string userName, string userPassword)
        {
            using (MySqlConnection connection = new MySqlConnection(GlobalSettings.connectionToDatabase))
            {
                try
                {
                    connection.Open();
                    string query = "SELECT * FROM users_db WHERE user_name = @user_login";
                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@user_login", userName.ToString());
                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                RequestClient requestClient = new RequestClient(reader, userPassword);
                                return requestClient;
                            }
                            else
                            {

                                return new RequestClient(false);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    connection.Close();
                    MessageBox.Show("Bląd:" + ex.Message);
                    return new RequestClient(false);
                }
            }
            return new RequestClient(false);
        }

        public static bool UserHaveAccess(RequestClient user, int accessLevel)
        {
            if (user.User_access_level > accessLevel) return false;
            return true;
        }


        public class RequestClient
        {
            private string user_name;
            private int user_access_level;
            private string user_data;
            private string user_password;
            private bool requestIsSuccess;

            public RequestClient(MySqlDataReader reader, string password)
            {
                User_name = reader["user_name"].ToString();
                User_access_level = int.Parse(reader["user_access_level"].ToString());
                User_data = reader["user_data"].ToString();
                User_password = reader["user_password"].ToString(); 
                RequestIsSuccess = BCrypt.Net.BCrypt.Verify(password, User_password);

            }

            public RequestClient(bool resilt)
            {

                User_name = "";
                User_access_level = 0;
                User_data = "";
                User_password = "";
                RequestIsSuccess = false;
            }

            public string User_name { get => user_name; set => user_name = value; }
            public int User_access_level { get => user_access_level; set => user_access_level = value; }
            public string User_data { get => user_data; set => user_data = value; }
            public string User_password { get => user_password; set => user_password = value; }
            public bool RequestIsSuccess { get => requestIsSuccess; set => requestIsSuccess = value; }
        }
    }
}
