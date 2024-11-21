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
    /// Логика взаимодействия для AddNewUserWindow.xaml
    /// </summary>
    public partial class AddNewUserWindow : Window
    {
        public AddNewUserWindow()
        {
            InitializeComponent();
        }
        private void ButtonStartWork_Click(object sender, RoutedEventArgs e)
        {
            ErrorTextBlock.Text = string.Empty;
            if (!checkAllFieldsNotEmpty())
            {
                ErrorTextBlock.Text = "Nie wszystkie pola są wypełnione";
            }
            takeAcessLevelAboutUserWhoCreate();
        }

        private void takeAcessLevelAboutUserWhoCreate()
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
                                int selectedValue;
                                if (BCrypt.Net.BCrypt.Verify(UserPassword.Text.Trim().ToString(), reader["user_password"].ToString()))
                                {
                                    if (Int32.TryParse(NewUserAccesLevel.SelectionBoxItem.ToString(), out selectedValue))
                                    {
                                        if ((int.Parse(reader["user_access_level"].ToString()) >= selectedValue) == true)
                                        {
                                            ErrorTextBlock.Text = "Dostęp użytkownika którego wy tworzycie nie może być mniejszy lub równy waszemu";
                                        }
                                        else
                                        {
                                            connection.Close();
                                            connection.Open();
                                            query = "INSERT INTO users_db (user_name, user_access_level, user_password, user_data) " +
                                                "VALUES (@user_name, @user_access_level, @user_password, @user_data)";
                                            using (MySqlCommand commandInsert = new MySqlCommand(query, connection))
                                            {
                                                commandInsert.Parameters.AddWithValue("@user_name", NewUserLogin.Text.Trim());
                                                commandInsert.Parameters.AddWithValue("@user_access_level", NewUserAccesLevel.SelectionBoxItem.ToString());
                                                commandInsert.Parameters.AddWithValue("@user_password", BCrypt.Net.BCrypt.HashPassword(NewUserPassword.Text.Trim()));
                                                commandInsert.Parameters.AddWithValue("@user_data", NewUserData.Text.TrimEnd().TrimStart());
                                                int rowsAffected = commandInsert.ExecuteNonQuery();
                                                if (rowsAffected > 0)
                                                {
                                                    ErrorTextBlock.Text = "Dodany nowy użytkownik";
                                                }
                                                else
                                                {
                                                    ErrorTextBlock.Text = "Użytkownik nie został dodany";
                                                }
                                            }
                                        }
                                    }
                                    else
                                    {
                                        ErrorTextBlock.Text = "Nie wybrano stopień dostępu";
                                    }
                                }
                                else
                                {
                                    ErrorTextBlock.Text = "Nie poprawny login/hasło";
                                }
                            }
                            else
                            {
                                ErrorTextBlock.Text = "Nie poprawny login/hasło";
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Błąd: " + ex.Message);
                }
            }
        }

        private bool checkAllFieldsNotEmpty()
        {
            if (NewUserLogin.Text == null || NewUserPassword.Text == null ||
                NewUserData.Text == null || NewUserAccesLevel.SelectedItem == null ||
                UserLogin.Text == null || UserPassword.Text == null) return false;
            return true;
        }
    }
}
