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

namespace AplicationForWarehouse_v2.Windows.CargoUserControl
{
    /// <summary>
    /// Логика взаимодействия для DeleteChoosenShipment.xaml
    /// </summary>
    public partial class DeleteChoosenShipment : Window
    {
        private ShortInfoAboutClient clientInfo;
        private List<Shipment> listToDelete;
        public DeleteChoosenShipment(ShortInfoAboutClient clientInfoMain, List<Shipment> list)
        {
            InitializeComponent();
            clientInfo = clientInfoMain;
            listToDelete = list;
            mainLabel.Text = "Wybrano usunięcie " + listToDelete.Count + " palet/paczek u klienta - " + clientInfo.NameClient + "\nProszę o wprowadzenie hasła oraz loginu";
        }
        private string RemoveChooosenShipment()
        {
            Console.WriteLine(UserLogin.Text.Trim().ToString());
            var request = ToolsFunction.Takeinfo(UserLogin.Text.Trim().ToString(), UserPassword.Text.Trim().ToString());
            Console.WriteLine(request.RequestIsSuccess);
            if (request.RequestIsSuccess == true)
            {
                if (ToolsFunction.UserHaveAccess(request, 3))
                {
                    using (MySqlConnection connection = new MySqlConnection(GlobalSettings.connectionToDatabase))
                    {
                        try
                        {
                            connection.Open();
                            string query = "DELETE FROM shipment_db WHERE id_shipment IN (" +
                                string.Join(",", listToDelete.Select((_, index) => $"@id{index}")) + ")";
                            using (MySqlCommand commandDelete = new MySqlCommand(query, connection))
                            {
                                Console.WriteLine("hey");
                                for (int i = 0; i < listToDelete.Count; i++)
                                {
                                    commandDelete.Parameters.AddWithValue($"@id{i}", listToDelete[i].IdShipment);
                                }
                                int rowsAffected = commandDelete.ExecuteNonQuery();
                                if (rowsAffected > 0)
                                {
                                    errorLabel.Text = "Pozycja została usunięta";
                                }
                                else errorLabel.Text = "Usunięcie nie powiodło się";
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


        private void ButtonDelete_Click(object sender, RoutedEventArgs e)
        {
            errorLabel.Text = string.Empty;
            if (UserLogin.Text != null && UserPassword.Text != null)
            {
                string result = RemoveChooosenShipment();
                errorLabel.Text = result;
            }
            else
            {
                errorLabel.Text = "Nie poprawne hasło lub login";
            }
        }
    }
}
