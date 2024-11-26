using AplicationForWarehouse_v2.Tools;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
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
    /// Логика взаимодействия для ChangeStatusShipment.xaml
    /// </summary>
    public partial class ChangeStatusShipment : Window
    {
        private ShortInfoAboutClient clientInfo;
        private List<Shipment> shipments;

        public ChangeStatusShipment(ShortInfoAboutClient infoAboutClient, List<Shipment> list)
        {
            InitializeComponent();
            clientInfo = infoAboutClient;
            shipments = list;
            textBlockDescription.Text = "Wybrano zmiane statusu dla przesyłek klienta: " + clientInfo.NameClient + ". Wybrano przesyłek: " + shipments.Count.ToString();
            StatusType.ItemsSource = Enum.GetValues(typeof(Status));
        }
        private string ChangeStatus()
        {
            int lvlSecurity = 0;
            if (StatusType.SelectedItem.ToString() == "Wysłana") lvlSecurity = 3;
            else lvlSecurity = 4;
            var request = ToolsFunction.Takeinfo(UserLogin.Text.Trim().ToString(), UserPassword.Text.Trim().ToString());
            Console.WriteLine(request.RequestIsSuccess);
            if (request.RequestIsSuccess == true)
            {
                if(ToolsFunction.UserHaveAccess(request,lvlSecurity))
                {
                    using(MySqlConnection connection = new MySqlConnection(GlobalSettings.connectionToDatabase))
                    {
                        try
                        {
                            connection.Open();
                            string query = "UPDATE shipment_db SET shipment_status = @SelectedItem WHERE id_shipment IN (" +
                                string.Join(",", shipments.Select((_, index) => $"@id{index}")) + ")";
                            Console.WriteLine("Heyka");
                            using (MySqlCommand command = new MySqlCommand(query, connection))
                            {
                                command.Parameters.AddWithValue("@SelectedItem", StatusType.SelectedItem.ToString());
                                for (int i = 0; i < shipments.Count; i++)
                                {
                                    command.Parameters.AddWithValue($"@id{i}", shipments[i].IdShipment);
                                }
                                int rowsAffected = command.ExecuteNonQuery();
                                if(rowsAffected > 0)
                                {
                                    erroLabel.Text = "Status został zmieniony";
                                }
                                else
                                {
                                    erroLabel.Text = "Zmiana statusu nie powiodła się";
                                }
                            }
                        }catch(Exception ex)
                        {
                            MessageBox.Show("Bląd" + ex.Message);
                            return "Błąd";
                        }
                    }
                    return "OK";
                }
                else
                {
                    return "Nie poprawny poziom dostępu";
                }
            }
            else
            {
                return "Nie poprawne dane do logowania"; 
            }
        }
        private void ButtonStartWork_Click(object sender, RoutedEventArgs e)
        {
            erroLabel.Text = string.Empty;
            if(UserLogin.Text != null && UserPassword.Text != null)
            {
                string result = ChangeStatus();
                erroLabel.Text = result;
            }
            else
            {
                erroLabel.Text = "Nie poprawne hasło lub login";
            }
        }
    }
    public enum Status
    {
        Utworzona,
        W_trakcie,
        Nieskonczona,
        Spakowane,
        Przygotowane,
        Wysłana
    }
}
