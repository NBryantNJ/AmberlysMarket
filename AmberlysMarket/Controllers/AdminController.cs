using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace AmberlysMarket.Controllers
{
    public class AdminController : Controller
    {

        public IActionResult Index()
        {
            return View();
        }

        //-----------------------------------ORDERS----------------------------------


        //Main Page
        public IActionResult Orders(string OrderNumber, string SortBy)
        {
            //Regular View
            if (OrderNumber == null && SortBy == null)
            {
                Security.AmberlyMarketDatabase amberlyMarketDatabase = new Security.AmberlyMarketDatabase();
                List<Models.Admin.OrdersModel> ordersModel = new List<Models.Admin.OrdersModel>();
                ordersModel = amberlyMarketDatabase.GetOrderDetails("Most Recent");
                return View(ordersModel);
            }
            //Sorted Regular View
            else if(OrderNumber == null && SortBy != null)
            {
                Security.AmberlyMarketDatabase amberlyMarketDatabase = new Security.AmberlyMarketDatabase();
                List<Models.Admin.OrdersModel> ordersModel = new List<Models.Admin.OrdersModel>();
                ordersModel = amberlyMarketDatabase.GetOrderDetails(SortBy);
                return View(ordersModel);
            }
            //Advanced Orders View
            else
            {
                Security.AmberlyMarketDatabase amberlyMarketDatabase = new Security.AmberlyMarketDatabase();
                return View(amberlyMarketDatabase.GetAdvancedOrderDetails(OrderNumber));
            }
        }


        //Switch Order To Order_Shipped Status and send user email
        public bool sendEmailToClient(string OrderNumber, string MessageType, string Carrier, string TrackingNumber )
        {
            Admin.EmailFunctions emailFunctions = new Admin.EmailFunctions(); 
            switch(MessageType)
            {
                case "0":
                    return emailFunctions.sendThankyouForPlacingOrderEmail(OrderNumber, Carrier, TrackingNumber);
                    break;

            }
            return true;
        }


        //Switch Order Status To Complete
        public bool changeOrderStatusToComplete(string OrderNumber)
        {
            Security.AmberlyMarketDatabase amberlyMarketDatabase = new Security.AmberlyMarketDatabase();
            return amberlyMarketDatabase.UpdateOrderStatusToComplete(OrderNumber); 
        }



    }
}
