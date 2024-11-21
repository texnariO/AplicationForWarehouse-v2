using AplicationForWarehouse_v2.Tools;
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
