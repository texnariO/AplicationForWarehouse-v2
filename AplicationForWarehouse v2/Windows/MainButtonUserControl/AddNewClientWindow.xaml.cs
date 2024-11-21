using AplicationForWarehouse_v2.Tools;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace AplicationForWarehouse_v2.Windows.MainButtonUserControl
{
    /// <summary>
    /// Логика взаимодействия для AddNewClientWindow.xaml
    /// </summary>
    public partial class AddNewClientWindow : Window
    {
        public AddNewClientWindow()
        {
            InitializeComponent();
        }
        private void ButtonAddClient_Click(object sender, RoutedEventArgs e)
        {
            ErrorMessage.Text = string.Empty;
            if (!checkAllFieldsNotEmpty())
            {
                ErrorMessage.Text = "Nie wszystkie pola są wypełnione";
            }
            addingNewClient();
        }
        private bool checkAllFieldsNotEmpty()
        {
            if (AddNewClientName.Text == null || AddNewClientGroup.Text == null ||
                AddNewClientCommnet.Text == null || UserLogin.Text == null ||
                UserPassword.Text == null) return false;
            return true;
        }

        private void addingNewClient()
        {
            using (MySqlConnection connection = new MySqlConnection(GlobalSettings.connectionToDatabase))
            {
                try
                {
                    connection.Open();
                    string query = "SELECT user_password, user_access_level, user_name FROM users_db WHERE user_name LIKE @user_login";
                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@user_login", "%" + UserLogin.Text.Trim().ToString() + "%");
                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                if (BCrypt.Net.BCrypt.Verify(UserPassword.Text.Trim().ToString(), reader["user_password"].ToString()))
                                {
                                    if (int.Parse(reader["user_access_level"].ToString()) <= 2)
                                    {
                                        connection.Close();
                                        connection.Open();
                                        query = "INSERT INTO client_db (client_name, client_group, client_comment)" +
                                            " VALUES(@client_name, @client_group, @client_comment)";
                                        using (MySqlCommand commandInstert = new MySqlCommand(query, connection))
                                        {
                                            commandInstert.Parameters.AddWithValue("@client_name", AddNewClientName.Text.Trim());
                                            commandInstert.Parameters.AddWithValue("@client_group", AddNewClientGroup.Text.Trim());
                                            commandInstert.Parameters.AddWithValue("@client_comment", AddNewClientCommnet.Text.Trim());
                                            int rowsAffected = commandInstert.ExecuteNonQuery();
                                            if (rowsAffected > 0)
                                            {
                                                ErrorMessage.Text = "Dodany nowy client";
                                                this.Close();
                                            }
                                            else
                                            {
                                                ErrorMessage.Text = "Klient nie został dodany";
                                            }
                                        }
                                    }
                                    else
                                    {
                                        ErrorMessage.Text = "Dostęp jest wzbroniony";
                                    }
                                }
                                else
                                {
                                    ErrorMessage.Text = "Nie poprawne hasło lub login";
                                }
                            }
                            else
                            {
                                ErrorMessage.Text = "Nie istnieje takiego użytkownika";
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Bląd: " + ex.Message);
                }
            }
        }
    }
}
