using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using static System.Net.Mime.MediaTypeNames;

namespace AmberlysMarket.Controllers.Admin
{
    public class EmailFunctions : Controller
    {
        //-----------GLOBAL USE CASES-------------
        

        //appsettings file reference
        public static IConfiguration GetConfiguration()
        {
            var builder = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
            return builder.Build();
        }

        //Get Connection String
        private readonly static IConfiguration configuration = GetConfiguration();
        private readonly static string AmberlysMarketEmail = configuration.GetConnectionString("AmberlysMarketEmail"); 
        private readonly static string AmberlysMarketEmailPassword = configuration.GetConnectionString("AmberlysMarketEmailPassword");

        //Prep smtp email variable 
        private SmtpClient smtp = (new SmtpClient
        {
            Host = "Smtp.Gmail.com",
            Port = 587,
            Credentials = new NetworkCredential(AmberlysMarketEmail, AmberlysMarketEmailPassword),
            EnableSsl = true
        });









        //------------------Email Types----------------------------------------------


        //Thank you for placing order email format
        public bool sendThankyouForPlacingOrderEmail(string OrderNumber, string Carrier, string TrackingNumber)
        {
            try
            {
                //Main Variables
                List<Models.Admin.OrdersModel> ordersModels = new List<Models.Admin.OrdersModel>(); 
                AmberlysMarket.Controllers.Security.AmberlyMarketDatabase amberlyMarketDatabase = new Security.AmberlyMarketDatabase();
                string ItemListTableView = "";

                //Update Database Order Shipping/Tracking Details
                if(amberlyMarketDatabase.UpdateShippingTable(OrderNumber, Carrier, TrackingNumber) != true)
                {
                    return false; 
                }
                
                //Get all advancedOrderDetails
                ordersModels = amberlyMarketDatabase.GetAdvancedOrderDetails(OrderNumber);

                //Set Up MailMessage
                MailMessage Mail = new MailMessage();
                Mail.From = new MailAddress(AmberlysMarketEmail);
                Mail.To.Add(ordersModels[0].customerInfoList.CustomerEmail);
                Mail.Subject = "Amberlys Market - Your Order Has Shipped";

                //Include Image
                LinkedResource LinkedImage = new LinkedResource(@"wwwroot\Images\AmberlysMarketEmailLogo.PNG");
                LinkedImage.ContentId = "AmberlysMarket";
                LinkedImage.ContentType = new ContentType(MediaTypeNames.Image.Jpeg);
                
                //Populate Items In Table View
                foreach(var item in ordersModels)
                {
                    ItemListTableView += "<tr>" +
                     "<td style='padding-right:40px;'>" + item.ListOfItems.ItemName + "</td>" +
                     "<td style='padding-right:40px;'>" + item.ListOfItems.Quantity + "</td>" +
                     "</tr>";
                }
                
                //Build Email Body
                AlternateView htmlView = AlternateView.CreateAlternateViewFromString(
                  "<img src=cid:AmberlysMarket> </br>" +
                  "<p><strong>Carrier: </strong>"+ordersModels[0].ShippingMethod+"</p> " +
                  "<p><strong>Tracking Number: </strong><a href=''>"+ordersModels[0].TrackingNumber+"</a></p>" +
                  "<p><strong>Order Number: </strong>"+ordersModels[0].OrderNumber+"</p>" +
                  "<table style='text-align:left; letter-spacing:1px;'> " +
                  "<tr>" +
                  "<th>Item Name</th>" +
                  "<th>Quantity</th>" +
                  "</tr>" + ItemListTableView +
                  "</table>",
                  null, "text/html");

                htmlView.LinkedResources.Add(LinkedImage);
                Mail.AlternateViews.Add(htmlView);

                //Send Email and Return True
                smtp.Send(Mail); 
                return true; 
            }
            catch(Exception e)
            {
                return false;
            }
        }









    }
}
