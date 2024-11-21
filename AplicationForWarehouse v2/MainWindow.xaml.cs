using AplicationForWarehouse_v2.Tools;
using AplicationForWarehouse_v2.Windows.CargoUserControl;
using AplicationForWarehouse_v2.Windows.MainButtonUserControl;
using MySql.Data.MySqlClient;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace AplicationForWarehouse_v2
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private List<ShortInfoAboutClient> listClients = new List<ShortInfoAboutClient>();
        private ShortInfoAboutClient actualClient = null;
        private List<Shipment> shipmentList = new List<Shipment>();
        public MainWindow()
        {
            actualClient = null;
            InitializeComponent();
            LoadComboBox();
        }
        private void LoadComboBox()
        {
            using (MySqlConnection connection = new MySqlConnection(GlobalSettings.connectionToDatabase))
            {
                try
                {
                    listClients.Clear();
                    connection.Open();
                    string query = "SELECT client_id, client_name, client_comment FROM client_db";

                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            listClients.Add(new ShortInfoAboutClient(reader["client_id"].ToString(), reader["client_name"].ToString(), reader["client_comment"].ToString()));
                        }
                        ComboBoxMain.ItemsSource = listClients;
                        ComboBoxMain.Items.Refresh();
                    }
                    connection.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Błąd: " + ex.Message);
                }
            }
        }
        private List<Shipment> loadDataToTable(string idClient)
        {
            List<Shipment> resultListWithDataShipment = new List<Shipment>();
            using (MySqlConnection connection = new MySqlConnection(GlobalSettings.connectionToDatabase))
            {
                try
                {
                    connection.Open();
                    Console.WriteLine(idClient);
                    string query = "SELECT * FROM shipment_db WHERE id_client LIKE @id_client AND shipment_status NOT LIKE 'Wysłana'";
                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@id_client", "%" + idClient + "%");
                        using (var reader = command.ExecuteReader())
                        {
                            Console.WriteLine(reader.HasRows);
                            int i = 1;
                            while (reader.Read())
                            {
                                resultListWithDataShipment.Add(new Shipment(reader, i.ToString()));
                                i++;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Błąd: " + ex.Message);
                }
            }
            return resultListWithDataShipment;
        }
        private void ReadyButton_Click(object sender, RoutedEventArgs e)
        {
            ShortInfoAboutClient selectedClient = ComboBoxMain.SelectedItem as ShortInfoAboutClient;
            if (selectedClient != null)
            {
                actualClient = selectedClient;
                ActualClient.Text = selectedClient.NameClient;
                ActualClientComment.Text = selectedClient.DescriptionClient;
                List<Shipment> itemsToBinding = loadDataToTable(selectedClient.IdClient);
                shipmentList = itemsToBinding;
                DGMain.ItemsSource = itemsToBinding;
            }
        }
        private void ButtonDelete_Click(object sender, RoutedEventArgs e)
        {
            if (actualClient != null)
            {
                List<Shipment> listToDelete = new List<Shipment>();
                foreach (var item in shipmentList)
                {
                    if (item.IsSelected) listToDelete.Add(item);
                }
                DeleteChoosenShipment deleteChooseShipment = new DeleteChoosenShipment(actualClient, listToDelete);
                deleteChooseShipment.Closed += UpdateTable;
                deleteChooseShipment.ShowDialog();
            }
        }
        private void AddClientButton_Click(object sender, RoutedEventArgs e)
        {
            AddNewClientWindow addNewClientWindow = new AddNewClientWindow();
            addNewClientWindow.Closed += DialogClose;
            addNewClientWindow.ShowDialog();
        }
        private void DialogClose(object sender, EventArgs e)
        {

            LoadComboBox();
        }
        private void UpdateTable(object sender, EventArgs e)
        {
            ShortInfoAboutClient selectedClient = ComboBoxMain.SelectedItem as ShortInfoAboutClient;
            if (selectedClient != null)
            {
                actualClient = selectedClient;
                ActualClient.Text = selectedClient.NameClient;
                ActualClientComment.Text = selectedClient.DescriptionClient;
                List<Shipment> itemsToBinding = loadDataToTable(selectedClient.IdClient);
                Console.WriteLine(itemsToBinding.Count);
                DGMain.ItemsSource = itemsToBinding;
            }
        }
        private void AddNewUserButton_Click(object sender, RoutedEventArgs e)
        {
            AddNewUserWindow addNewUserWindow = new AddNewUserWindow();
            addNewUserWindow.ShowDialog();
        }
        private void AddShipment_Click(object sender, RoutedEventArgs e)
        {
            if (actualClient != null)
            {
                AddNewShipmentWindow addNewShipmentWindow = new AddNewShipmentWindow(actualClient);
                addNewShipmentWindow.Closed += UpdateTable;
                addNewShipmentWindow.ShowDialog();
            }
            else
            {
                ErrorLabel.Text = "Nie można dodać palety bez wybranego klienta";
            }
        }
        private void ExportExcelTabel(object sender, RoutedEventArgs e)
        {
            if (actualClient != null)
            {
                List<Shipment> exportListShipment = new List<Shipment>();
                foreach (var item in shipmentList)
                {
                    if (item.IsSelected) exportListShipment.Add(item);
                }
                if (exportListShipment.Count == 0) exportListShipment = shipmentList;
                int sumWeight = 0;
                int sumVolume = 0;
                foreach (var item in exportListShipment)
                {
                    int.TryParse(item.ShipmentWeight, out int teampSumWeight);
                    sumWeight += teampSumWeight;
                    int.TryParse(item.ShipmentVolume, out int teampSumVolume);
                    sumVolume += teampSumVolume;
                }
                string startupPath = System.IO.Directory.GetCurrentDirectory();
                startupPath = startupPath + "/Wyniki/ExportPallet/" + actualClient.NameClient + ".xlsx";
                using (ExcelPackage packageToWriteExport = new ExcelPackage())
                {
                    int licznik = 1;
                    int write = 3;
                    ExcelWorksheet worksheet = packageToWriteExport.Workbook.Worksheets.Add("New");
                    worksheet.Columns.Width = 12.63;
                    worksheet.Cells["1:1"].Style.Font.Bold = true;
                    worksheet.Cells[1, 4].Value = "SUMA:";
                    worksheet.Cells[1, 5].Value = sumWeight;
                    worksheet.Cells[1, 12].Value = "SUMA:";
                    worksheet.Cells[1, 13].Value = sumVolume;
                    worksheet.Cells[1, 14].Value = "GLS";
                    worksheet.Cells["2:2"].Style.Font.Bold = true;
                    worksheet.Cells[2, 1].Value = "Klient";
                    worksheet.Cells[2, 2].Value = "Sektor";
                    worksheet.Cells[2, 3].Value = "Lokalizajca";
                    worksheet.Cells[2, 4].Value = "№";
                    worksheet.Cells[2, 5].Value = "Waga palety/kosza";
                    worksheet.Cells[2, 6].Value = "Waga towar";
                    worksheet.Cells[2, 7].Value = "Waga paleta";
                    worksheet.Cells[2, 8].Value = "Paleta/Paczka";
                    worksheet.Cells[2, 9].Value = "Komentarz";
                    worksheet.Cells[2, 10].Value = "Szerokość";
                    worksheet.Cells[2, 11].Value = "Głębokość";
                    worksheet.Cells[2, 12].Value = "Wysokość";
                    worksheet.Cells[2, 13].Value = "Objętość";
                    worksheet.Cells[2, 14].Value = "Wymiar";
                    worksheet.Cells[2, 15].Value = "Waga z kraje wagi";
                    worksheet.Cells[2, 16].Value = "Różnica";
                    worksheet.Cells[2, 17].Value = "Procentowa różnica";
                    foreach (var item in exportListShipment)
                    {
                        worksheet.Cells[write, 1].Value = actualClient.NameClient;
                        worksheet.Cells[write, 2].Value = item.ShipmentSektor;
                        worksheet.Cells[write, 3].Value = item.ShipmentLocation;
                        worksheet.Cells[write, 4].Value = licznik;
                        worksheet.Cells[write, 5].Value = item.ShipmentWeight;
                        int.TryParse(item.ShipmentWeight, out int teampShipmentWeight);
                        int.TryParse(item.ShipmentWeightPallet, out int teampShipmentWeightPallet);
                        worksheet.Cells[write, 6].Value = teampShipmentWeight - teampShipmentWeightPallet;
                        worksheet.Cells[write, 7].Value = item.ShipmentWeightPallet;
                        worksheet.Cells[write, 8].Value = item.ShipmentType;
                        worksheet.Cells[write, 9].Value = item.ShipmentStatus;
                        worksheet.Cells[write, 10].Value = item.ShipmentWidth;
                        worksheet.Cells[write, 11].Value = item.ShipmentHeight;
                        worksheet.Cells[write, 12].Value = item.ShipmentLenght;
                        worksheet.Cells[write, 13].Value = item.ShipmentVolume;
                        worksheet.Cells[write, 14].Value = item.ShipmentCanGls;
                        worksheet.Cells[write, 15].Value = item.ShipmentWeightOrders;
                        int.TryParse(item.ShipmentWeightOrders, out int teampShipmentWeightOrders);
                        worksheet.Cells[write, 16].Value = teampShipmentWeight - teampShipmentWeightOrders;
                        if (teampShipmentWeight != 0)
                            worksheet.Cells[write, 17].Value = (teampShipmentWeight - teampShipmentWeightOrders) / teampShipmentWeight;
                        licznik++;
                        write++;
                    }
                    FileInfo fileInfo = new FileInfo(startupPath);
                    packageToWriteExport.SaveAs(fileInfo);
                }
                MessageBox.Show("Plik został wyeksportowany");

            }

        }
        private void ChangeStatus_Click(object sender, RoutedEventArgs e)
        {
            if (actualClient != null)
            {
                List<Shipment> listToChange = new List<Shipment>();
                foreach (var item in shipmentList)
                {
                    if (item.IsSelected) listToChange.Add(item);
                }
                if (listToChange.Count > 0)
                {
                    ChangeStatusShipment changeStatusShipment = new ChangeStatusShipment(actualClient, listToChange);
                    changeStatusShipment.Closed += UpdateTable;
                    changeStatusShipment.ShowDialog();
                }
                else
                {
                    ErrorLabel.Text = "Nie wybrano żadnej palety";
                }
            }
            else
            {
                ErrorLabel.Text = "Nie można zmienić statusu, nie wybrano klienta";
            }
        }
    }
    public partial class ShortInfoAboutClient
    {
        private string idClient;
        private string nameClient;
        private string descriptionClient;


        public string IdClient { get => idClient; set => idClient = value; }
        public string NameClient { get => nameClient; set => nameClient = value; }
        public string DescriptionClient { get => descriptionClient; set => descriptionClient = value; }
        public ShortInfoAboutClient(string idClient, string nameClient, string clientComment)
        {
            IdClient = idClient;
            NameClient = nameClient;
            DescriptionClient = clientComment;
        }

    }

    public enum PackageStatus
    {
        Przygotowanie,
        Spakowana,
        Ostreczowana,
        Gotowa,
        Wysłana
    }
}
