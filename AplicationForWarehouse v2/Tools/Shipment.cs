using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AplicationForWarehouse_v2.Tools
{
    public class Shipment
    {
        private string idShipment;
        private string idData;
        private string idClient;
        private string shipmentSektor;
        private string shipmentStatus;
        private string shipmentType;
        private string shipmentLocation;
        private string shipmentWidth;
        private string shipmentHeight;
        private string shipmentLenght;
        private string shipmentVolume;
        private string shipmentWeight;
        private string shipmentWeightPallet;
        private string shipmentWeightOrders;
        private string shipmentCanGls;
        private string shipmentLastUpdate;
        private string shipment_last_user;
        private string shipmentDimensions;
        private bool isSelected;

        public string IdShipment { get => idShipment; set => idShipment = value; }
        public bool IsSelected { get => isSelected; set => isSelected = value; }
        public string IdClient { get => idClient; set => idClient = value; }
        public string ShipmentSektor { get => shipmentSektor; set => shipmentSektor = value; }
        public string ShipmentStatus { get => shipmentStatus; set => shipmentStatus = value; }
        public string ShipmentType { get => shipmentType; set => shipmentType = value; }
        public string ShipmentWidth { get => shipmentWidth; set => shipmentWidth = value; }
        public string ShipmentHeight { get => shipmentHeight; set => shipmentHeight = value; }
        public string ShipmentLenght { get => shipmentLenght; set => shipmentLenght = value; }
        public string ShipmentVolume { get => shipmentVolume; set => shipmentVolume = value; }
        public string ShipmentWeight { get => shipmentWeight; set => shipmentWeight = value; }
        public string ShipmentWeightPallet { get => shipmentWeightPallet; set => shipmentWeightPallet = value; }
        public string ShipmentWeightOrders { get => shipmentWeightOrders; set => shipmentWeightOrders = value; }
        public string ShipmentCanGls { get => shipmentCanGls; set => shipmentCanGls = value; }
        public string ShipmentLastUpdate { get => shipmentLastUpdate; set => shipmentLastUpdate = value; }
        public string Shipment_last_user { get => shipment_last_user; set => shipment_last_user = value; }
        public string ShipmentDimensions { get => shipmentDimensions; set => shipmentDimensions = value; }
        public string IdData { get => idData; set => idData = value; }
        public string ShipmentLocation { get => shipmentLocation; set => shipmentLocation = value; }

        public Shipment(string idsShipment, string idClient, string shipmentSektor, string shipmentStatus, string shipmentType, string shipmentWidth, string shipmentHeight, string shipmentLenght, string shipmentVolume, string shipmentWeight, string shipmentWeightPallet, string shipmentWeightOrders, string shipmentCanGls, string shipmentLastUpdate, string shipment_last_user, string idData, string shipmentLocation)
        {
            IdShipment = idsShipment;
            IsSelected = false;
            IdClient = idClient;
            ShipmentSektor = shipmentSektor;
            ShipmentStatus = shipmentStatus;
            ShipmentType = shipmentType;
            ShipmentWidth = shipmentWidth;
            ShipmentHeight = shipmentHeight;
            ShipmentLenght = shipmentLenght;
            ShipmentVolume = shipmentVolume;
            ShipmentWeight = shipmentWeight;
            ShipmentWeightPallet = shipmentWeightPallet;
            ShipmentWeightOrders = shipmentWeightOrders;
            ShipmentCanGls = shipmentCanGls;
            ShipmentLastUpdate = shipmentLastUpdate;
            Shipment_last_user = shipment_last_user;
            ShipmentDimensions = ShipmentWidth + "x" + ShipmentHeight + "x" + ShipmentLenght;
            IdData = idData;
            ShipmentLocation = shipmentLocation;
        }

        public Shipment(MySqlDataReader reader, string idData)
        {
            IsSelected = false;
            IdShipment = reader["id_shipment"].ToString();
            IdClient = reader["id_client"].ToString();
            ShipmentSektor = reader["shipment_sektor"].ToString();
            ShipmentStatus = reader["shipment_status"].ToString();
            ShipmentType = reader["shipment_type"].ToString();
            ShipmentWidth = reader["shipment_width"].ToString();
            ShipmentHeight = reader["shipment_height"].ToString();
            ShipmentLenght = reader["shipment_lenght"].ToString();
            ShipmentVolume = reader["shipment_volume"].ToString();
            ShipmentWeight = reader["shipment_weight"].ToString();
            ShipmentWeightPallet = reader["shipment_weight_pallet"].ToString();
            ShipmentWeightOrders = reader["shipment_weight_orders"].ToString();
            if (reader["shipment_can_gls"].ToString().Equals(1))
                ShipmentCanGls = "Tak";
            else ShipmentCanGls = "Nie";
            ShipmentLastUpdate = reader["shipment_last_update"].ToString();
            Shipment_last_user = reader["shipment_last_user"].ToString();
            ShipmentDimensions = ShipmentWidth + "x" + ShipmentHeight + "x" + ShipmentLenght;
            IdData = idData;
            ShipmentLocation = reader["shipment_location"].ToString();
        }
    }
}
