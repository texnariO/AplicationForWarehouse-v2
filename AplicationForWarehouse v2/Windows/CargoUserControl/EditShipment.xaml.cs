using AplicationForWarehouse_v2.Tools;
using MySql.Data.MySqlClient;
using OfficeOpenXml.Drawing.Slicer.Style;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
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
    /// Логика взаимодействия для EditShipment.xaml
    /// </summary>
    public partial class EditShipment : Window, INotifyPropertyChanged
    {
        private Shipment shipmentToEdit;
        private bool canGLS;
        private string _inputWidth;
        private string _inputHeight;
        private string _inputLength;

        private string _resultVolume;
        private string _canGLSSuma;

        private string _weightPallets;
        private string _weightPallet;

        public string WeightPallets
        {
            get => _weightPallets;
            set
            {
                _weightPallets = value;
                OnPropertyChanged();
            }
        }

        public string WeightPallet
        {
            get => _weightPallet;
            set
            {
                _weightPallet = value;
                OnPropertyChanged();
            }
        }


        public string InputWidth
        {
            get => _inputWidth;
            set
            {
                _inputWidth = value;
                OnPropertyChanged();
                UpdateDynamicText();
                UpdateDynamicCanGLS();
            }
        }
        public string InputHeight
        {
            get => _inputHeight;
            set
            {
                _inputHeight = value;
                OnPropertyChanged();
                UpdateDynamicText();
                UpdateDynamicCanGLS();
            }
        }
        public string InputLength
        {
            get => _inputLength;
            set
            {
                _inputLength = value;
                OnPropertyChanged();
                UpdateDynamicCanGLS();
                UpdateDynamicText();
            }
        }
        public string ResultVolume
        {
            get => _resultVolume;
            set
            {
                _resultVolume = value;
                OnPropertyChanged();
            }
        }

        public string CanGLSSuma
        {
            get => _canGLSSuma;
            set
            {
                _canGLSSuma = value;
                OnPropertyChanged();
            }
        }
        private void UpdateDynamicText()
        {
            int.TryParse(InputWidth, out int teampShipmentWidth);
            int.TryParse(InputLength, out int teampShipmentLength);
            int.TryParse(InputHeight, out int teampShipmentHeigth);
            int suma = teampShipmentHeigth * teampShipmentLength * teampShipmentWidth;
            double newLicz =(double) (teampShipmentHeigth * teampShipmentLength * teampShipmentWidth) / 1000000;
            Console.WriteLine("new: " + newLicz);
            Console.WriteLine("new: " + suma);
            ResultVolume = newLicz.ToString();
        }
        private void UpdateDynamicCanGLS()
        {
            int.TryParse(InputWidth, out int teampShipmentWidth);
            int.TryParse(InputLength, out int teampShipmentLength);
            int.TryParse(InputHeight, out int teampShipmentHeigth);
            int suma = 0;
            if(teampShipmentHeigth >= teampShipmentWidth)
            {
                suma += teampShipmentWidth * 2;
                if(teampShipmentLength>= teampShipmentHeigth) {
                    suma += teampShipmentHeigth * 2;
                    suma += teampShipmentLength;
                }
                else
                {
                    suma += teampShipmentLength * 2;
                    suma += teampShipmentHeigth;
                }
            }
            else
            {
                suma += teampShipmentHeigth * 2;
                if (teampShipmentLength >= teampShipmentWidth)
                {
                    suma += teampShipmentWidth * 2;
                    suma += teampShipmentLength;
                }
                else
                {
                    suma += teampShipmentLength * 2;
                    suma += teampShipmentWidth;
                }
            }
            CanGLSSuma = suma.ToString();
            if (suma > 300)
            {
                CanGLSResultTextBlock.Text = "Nie, nie może";
                canGLS = false;
            }
            else
            {
                CanGLSResultTextBlock.Text = "Tak może";
                canGLS = true;
            }

        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        public EditShipment(Shipment shipment)
        {
            shipmentToEdit = shipment;
            InitializeComponent();
            DataContext = this;
            LocationTextBlock.Text = LocationTextBlock.Text+" "+ shipment.ShipmentLocation;
            StatusTextBlock.Text = StatusTextBlock.Text + " " + shipmentToEdit.ShipmentStatus;
            SektorComboBox.ItemsSource = Enum.GetValues(typeof(SelectionSektor));
            TypeComboBox.ItemsSource = Enum.GetValues(typeof(TypeOfPackage));
            InputWidth = shipment.ShipmentWidth;
            InputLength = shipment.ShipmentLenght;
            InputHeight = shipment.ShipmentHeight;
            WeightPallets = shipment.ShipmentWeight;
            WeightPallet = shipment.ShipmentWeightPallet;
            WeightOrdersResultTextBlock.Text = shipment.ShipmentWeightOrders;
            foreach(var i in TypeComboBox.Items)
            {
                if(i.ToString() == shipment.ShipmentType)
                {
                    TypeComboBox.SelectedItem = i;
                }
            }
            foreach(var i in SektorComboBox.Items)
            {
                if(i.ToString() == shipment.ShipmentSektor)
                {
                    SektorComboBox.SelectedItem = i;
                }
            }
        }


        private void NumericTextBox_PriviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex(@"^[0-9]*(,[0-9]*)?$");
            string fullText = ((TextBox) sender).Text.Insert(((TextBox)sender).SelectionStart, e.Text);
            e.Handled = !regex.IsMatch(fullText);
        }
        private void NumericTextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            // Zezwalaj na klawisze sterujące
            if (e.Key == Key.Back || e.Key == Key.Delete || e.Key == Key.Left || e.Key == Key.Right || e.Key == Key.Tab)
            {
                e.Handled = false;
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            errorLabel.Text = string.Empty;
            if(UserLogin.Text !=null && UserPassword.Text != null)
            {
                string result = UpdateInfoAboutShipment();
                errorLabel.Text = result;
            }
            else
            {
                errorLabel.Text = "Nie poprawne hasło lub login";
            }
        }

        private string UpdateInfoAboutShipment()
        {
            var request = ToolsFunction.Takeinfo(UserLogin.Text.Trim().ToString(), UserPassword.Text.Trim().ToString());
            if(request.RequestIsSuccess == true)
            {
                if (ToolsFunction.UserHaveAccess(request, 4))
                {
                    using(MySqlConnection connection = new MySqlConnection(GlobalSettings.connectionToDatabase))
                    {
                        try
                        {
                            connection.Open();
                            string query = "UPDATE shipment_db SET shipment_sektor = @shipment_sektor, shipment_type = @shipment_type, shipment_location = @shipment_location, shipment_width = @shipment_width, " +
                                "shipment_height = @shipment_height, shipment_lenght = @shipment_lenght, shipment_volume = @shipment_volume, shipment_weight = @shipment_weight, " +
                                "shipment_weight_pallet = @shipment_weight_pallet, shipment_weight_orders = @shipment_weight_orders, shipment_can_gls = @shipment_can_gls," +
                                "shipment_last_update = @shipment_last_update, shipment_last_user = @shipment_last_user WHERE (id_shipment = @id_shipment)";
                            using (MySqlCommand commandUpdate = new MySqlCommand(query,connection))
                            {
                                commandUpdate.Parameters.AddWithValue("@shipment_type",TypeComboBox.SelectionBoxItem.ToString());
                                commandUpdate.Parameters.AddWithValue("@shipment_location", LocationTextBox.Text);
                                commandUpdate.Parameters.AddWithValue("@shipment_sektor", SektorComboBox.SelectionBoxItem.ToString().Replace("_", " "));
                                commandUpdate.Parameters.AddWithValue("@shipment_width", InputWidth);
                                commandUpdate.Parameters.AddWithValue("@shipment_height",InputHeight);
                                commandUpdate.Parameters.AddWithValue("@shipment_lenght", InputLength);
                                Console.WriteLine(ResultVolume);
                                commandUpdate.Parameters.AddWithValue("@shipment_volume", ResultVolume);
                                commandUpdate.Parameters.AddWithValue("@shipment_weight", WeightPallets);
                                commandUpdate.Parameters.AddWithValue("@shipment_weight_pallet", WeightPallet);
                                commandUpdate.Parameters.AddWithValue("@shipment_weight_orders", WeightOrdersResultTextBlock.Text);
                                if(canGLS==true) commandUpdate.Parameters.AddWithValue("@shipment_can_gls", "Tak");
                                else commandUpdate.Parameters.AddWithValue("@shipment_can_gls", "Nie");
                                string formattedDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                                commandUpdate.Parameters.AddWithValue("@shipment_last_update", formattedDate);
                                commandUpdate.Parameters.AddWithValue("@shipment_last_user", request.User_name);
                                commandUpdate.Parameters.AddWithValue("@id_shipment", shipmentToEdit.IdData);
                                int rowsAffected = commandUpdate.ExecuteNonQuery();
                                if (rowsAffected > 0)
                                {
                                    errorLabel.Text = "Zmodyfikowano palete/paczke";
                                }
                                else errorLabel.Text = "Modyfikacja nie zadziaława";

                            }
                            //UPDATE `warehouse_db`.`shipment_db` SET `shipment_location` = 'Magazyn 2' WHERE (`id_shipment` = '3
                            // string formattedDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Bląd" + ex.Message);
                            return "Błąd";
                        }
                    }
                    return "OK";
                }
                else{
                    return "Niemacie dostępu do danej funkcji"; 
                }
            }
            else
            {
                return "Nie poprawne hasło lub login";
            }
        }
    }
}
