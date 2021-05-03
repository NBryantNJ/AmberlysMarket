using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AmberlysMarket.Models.Admin
{
    public class ItemModel
    {
        public string ItemName { get; set; }
        public double ItemPrice { get; set; }
        public int Quantity { get; set; }
        public double TotalCostOfItemSet { get; set; }
    }
}
