using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AmberlysMarket.Models.Admin
{
    public class OrdersModel
    {
        public string PurchaseDate { get; set; }
        public string OrderNumber { get; set; }
        public string TotalCost { get; set; }
        public string OrderStatus { get; set; }
        public Admin.CustomerInfoModels customerInfoList { get; set; }
        public bool advancedDetailView { get; set; }
        public ItemModel ListOfItems { get; set; }
        public string ShippingMethod { get; set;}
        public string TrackingNumber { get; set; }
        public decimal ShippingCost { get; set; }
      

    }
}
