using AplicationForWarehouse_v2.Tools;
using MySql.Data.MySqlClient;
using MySqlX.XDevAPI;
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

namespace AplicationForWarehouse_v2.Windows.CargoUserControl
{
    /// <summary>
    /// Логика взаимодействия для AddNewShipmentWindow.xaml
    /// </summary>
    public partial class AddNewShipmentWindow : Window
    {
        private ShortInfoAboutClient clientInfo;
        public AddNewShipmentWindow(ShortInfoAboutClient client)
        {
            clientInfo = client;
            InitializeComponent();
            LabelName.Text = "Dodanie nowej palety dla klienta: " + client.NameClient;
            SelectionSektor.ItemsSource = Enum.GetValues(typeof(SelectionSektor));
            SelectionType.ItemsSource = Enum.GetValues(typeof(TypeOfPackage));
        }
        private string addNewShipmentToDatabase()
        {
            var request = ToolsFunction.Takeinfo(UserLogin.Text.Trim().ToString(), UserPassword.Text.Trim().ToString());
            Console.WriteLine(request.RequestIsSuccess);
            if (request.RequestIsSuccess == true)
            {
                if (ToolsFunction.UserHaveAccess(request, 4))
                {
                    using (MySqlConnection connection = new MySqlConnection(GlobalSettings.connectionToDatabase))
                    {
                        try
                        {
                            connection.Open();
                            string query = "INSERT INTO shipment_db (id_client, shipment_sektor, shipment_status, shipment_type, shipment_last_update, shipment_last_user) " +
                                "VALUES (@user_id, @sektor_selected, 'Utworzona', @type, @date_time, @user_name)";
                            using (MySqlCommand commnadInsert = new MySqlCommand(query, connection))
                            {
                                commnadInsert.Parameters.AddWithValue("@user_id", clientInfo.IdClient);
                                commnadInsert.Parameters.AddWithValue("@sektor_selected", SelectionSektor.SelectionBoxItem.ToString().Replace("_", " "));
                                commnadInsert.Parameters.AddWithValue("@type", SelectionType.SelectionBoxItem.ToString());
                                string formattedDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                                Console.WriteLine(formattedDate);
                                commnadInsert.Parameters.AddWithValue("@date_time", formattedDate);
                                commnadInsert.Parameters.AddWithValue("@user_name", request.User_name);
                                int rowsAffected = commnadInsert.ExecuteNonQuery();
                                if (rowsAffected > 0)
                                {
                                    ErrorLabel.Text = "Dodana nowa paleta/paczka";
                                }
                                else ErrorLabel.Text = "Dodanie nie powiodło";
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Bląd" + ex.Message);
                            return "Błąd";
                        }
                    }

                    return "OK";
                }
                else
                {
                    return "Niemacie dostępu do danej funkcji";
                }
            }
            else
            {
                return "Nie poprawne hasło lub login";
            }
        }

        private void ButtonStartWork_Click(object sender, RoutedEventArgs e)
        {
            ErrorLabel.Text = string.Empty;

            if (UserLogin.Text != null && UserPassword.Text != null
                && SelectionSektor.SelectedItem != null && SelectionType.SelectedItem != null)
            {
                Console.WriteLine(UserLogin.Text);
                Console.WriteLine(UserPassword.Text);
                string result = addNewShipmentToDatabase();
                ErrorLabel.Text = result;
            }
            else
            {
                ErrorLabel.Text = "Nie poprawne hasło lub login";
            }
        }
    }
    public enum TypeOfPackage
    {
        Paleta,
        Paczka,
        Kombinowane
    }
    //TODO: need update all list

    public enum SelectionSektor
    {
        Sektor_1,
        Sektor_2,
        Sektor_3,
        Sektor_4,
        Sektor_5,
        Sektor_6,
        Sektor_7,
        Sektor_8,
        Sektor_9,
        Sektor_10,
        Sektor_11,
        Sektor_12,
        Sektor_13,
        Sektor_14,
        Sektor_15
    }
}

