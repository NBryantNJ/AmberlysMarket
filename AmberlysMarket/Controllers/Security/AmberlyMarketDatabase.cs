using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;
using Microsoft.AspNetCore.Http;

namespace AmberlysMarket.Controllers.Security
{
    public class AmberlyMarketDatabase : Controller
    {
        //-----------GLOBAL USE CASES-------------
        private HttpContextAccessor contextAccess = new HttpContextAccessor();


        //appsettings file reference
        public static IConfiguration GetConfiguration()
        {
            var builder = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
            return builder.Build();
        }

        //Get Connection String
        private readonly static IConfiguration configuration = GetConfiguration();
        private readonly static string amberlysDBConnectionString = configuration.GetConnectionString("AmberlysMarketDB");
        private readonly MySqlConnection conn = new MySqlConnection(amberlysDBConnectionString);
        



        //------------------FUNCTIONS------------------

        //Retrieve Order Details
        public List<Models.Admin.OrdersModel> GetOrderDetails(string SortBy)
        {
            List<Models.Admin.OrdersModel> OrderModel = new List<Models.Admin.OrdersModel>();
            using (conn)
            {
                conn.Open();
                MySqlCommand cmd = null;
                switch (SortBy)
                {
                    case "Most Recent":
                         cmd = new MySqlCommand("SELECT * FROM CustomerInfoTable INNER JOIN InvoiceTB ON InvoiceTB.CustomerID = CustomerInfoTable.CustomerID ORDER BY PurchaseDate ASC", conn);
                        break;
                    case "Oldest":
                         cmd = new MySqlCommand("SELECT * FROM CustomerInfoTable INNER JOIN InvoiceTB ON InvoiceTB.CustomerID = CustomerInfoTable.CustomerID ORDER BY PurchaseDate DESC", conn);
                        break;
                    case "Name A-Z":
                        cmd = new MySqlCommand("SELECT * FROM CustomerInfoTable INNER JOIN InvoiceTB ON InvoiceTB.CustomerID = CustomerInfoTable.CustomerID ORDER BY CustomerInfoTable.CustomerFirstName ASC", conn);
                        break;
                    //If no cases match search by ordernumber
                    default:
                        cmd = new MySqlCommand("SELECT * FROM CustomerInfoTable INNER JOIN InvoiceTB ON InvoiceTB.CustomerID = CustomerInfoTable.CustomerID WHERE OrderNumber = @OrderNumber", conn);
                        cmd.Parameters.Add("@OrderNumber", MySqlDbType.VarChar).Value = SortBy; 
                        break;

                }
                MySqlDataReader reader = cmd.ExecuteReader();
                while(reader.Read())
                {

                    OrderModel.Add(new Models.Admin.OrdersModel
                    {
                        PurchaseDate = reader["PurchaseDate"].ToString(),
                        OrderNumber = reader["OrderNumber"].ToString(),
                        TotalCost = reader["TotalCost"].ToString(),
                        OrderStatus = reader["OrderStatus"].ToString(),
                        customerInfoList = (new Models.Admin.CustomerInfoModels
                        {
                            CustomerFirstName = reader["CustomerFirstName"].ToString(),
                            CustomerLastName = reader["CustomerLastName"].ToString(),
                            CustomerID = reader["CustomerID"].ToString(),
                        })

                    });  


                }
                //If Model is empty
                if (OrderModel.Count == 0)
                {
                    OrderModel.Add(new Models.Admin.OrdersModel
                    {
                        advancedDetailView = false
                    });
                 }
                return OrderModel; 
            }
        }



        //Get Advanced Order Details
        public List<Models.Admin.OrdersModel> GetAdvancedOrderDetails(string OrderNumber)
        {
            List<Models.Admin.OrdersModel> OrderModel = new List<Models.Admin.OrdersModel>();
            using (conn)
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand("SELECT * FROM InvoiceAdvancedTB INNER JOIN InvoiceTB ON InvoiceTB.OrderNumber = InvoiceAdvancedTB.OrderNumber INNER JOIN CustomerInfoTable ON CustomerInfoTable.CustomerID = InvoiceTB.CustomerID INNER JOIN ItemTB ON ItemTB.ItemID = InvoiceAdvancedTB.ItemID WHERE InvoiceTB.OrderNumber = @OrderNumber;", conn);
                cmd.Parameters.Add("@OrderNumber", MySqlDbType.VarChar).Value = OrderNumber; 
                
                MySqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {

                    OrderModel.Add(new Models.Admin.OrdersModel
                    {
                        //Get Basic Order Details
                        PurchaseDate = reader["PurchaseDate"].ToString(),
                        OrderNumber = reader["OrderNumber"].ToString(),
                        TotalCost = reader["TotalCost"].ToString(),
                        OrderStatus = reader["OrderStatus"].ToString(),
                        advancedDetailView = true,

                        //Get Customer Details
                        customerInfoList = (new Models.Admin.CustomerInfoModels
                        {
                            CustomerFirstName = reader["CustomerFirstName"].ToString(),
                            CustomerLastName = reader["CustomerLastName"].ToString(),
                            CustomerID = reader["CustomerID"].ToString(),
                            CustomerPhoneNumber = reader["CustomerPhoneNumber"].ToString(),
                            CustomerEmail = reader["CustomerEmail"].ToString(),
                            CustomerStreetAddress = reader["CustomerStreetAddress"].ToString(),
                            CustomerState = reader["CustomerState"].ToString(),
                            CustomerZipCode = reader["CustomerZipCode"].ToString(),
                            DoesCustomerHaveOnlineAccount = reader["DoesCustomerHaveOnlineAccount"].ToString()
                        }),

                        //Get Item Details
                        ListOfItems = (new Models.Admin.ItemModel
                        {
                            ItemName = reader["ItemName"].ToString(),
                            ItemPrice = double.Parse(reader["ItemPrice"].ToString()),
                            Quantity = int.Parse(reader["Quantity"].ToString()),
                            TotalCostOfItemSet = (double.Parse(reader["ItemPrice"].ToString()) * double.Parse(reader["Quantity"].ToString()))
                        }),

                        //Get Shipping Details
                        ShippingMethod = reader["ShippingMethod"].ToString(),
                        TrackingNumber = reader["TrackingNumber"].ToString(),
                        ShippingCost = decimal.Parse(reader["ShippingCost"].ToString())

                });   
                }
                return OrderModel;
            }
        }





        //---------------------Updating Tables---------------------------
        

        //Update InvoiceTB ShippingMethod and TrackingNumber (Used When Order has shipped)
        public bool UpdateShippingTable(string OrderNumber, string ShippingMethod, string TrackingNumber)
        {
            using (conn)
            {
                try
                {
                    conn.Open();
                    MySqlCommand cmd = new MySqlCommand("UPDATE InvoiceTB SET ShippingMethod = @ShippingMethod,  TrackingNumber = @TrackingNumber, OrderStatus = 'Order Shipped' WHERE OrderNumber = @OrderNumber", conn);
                    cmd.Parameters.Add("@ShippingMethod", MySqlDbType.VarChar).Value = ShippingMethod; 
                    cmd.Parameters.Add("@TrackingNumber", MySqlDbType.VarChar).Value = TrackingNumber; 
                    cmd.Parameters.Add("@OrderNumber", MySqlDbType.VarChar).Value = OrderNumber;
                    cmd.ExecuteNonQuery(); 
                    return true; 
                }
                catch(Exception e)
                {
                    return false; 
                }
            }
        }



        //Change Order Status to Complete
        public bool UpdateOrderStatusToComplete(string OrderNumber)
        {
            using (conn)
            {
                try
                {
                    conn.Open();
                    MySqlCommand cmd = new MySqlCommand("UPDATE InvoiceTB SET OrderStatus = 'Order Complete' WHERE OrderNumber = @OrderNumber", conn);
                    cmd.Parameters.Add("@OrderNumber", MySqlDbType.VarChar).Value = OrderNumber;
                    cmd.ExecuteNonQuery();
                    return true;
                }
                catch (Exception e)
                {
                    return false;
                }
            }
        }




        //_______________________________________Add new Customer Details____________________________________________
        public string CreateNewCustomer(string FirstName, string LastName, string Email, string PhoneNumber, string Username, string Password)
        {
            using (conn)
            {
                try
                {
                    conn.Open();
                    //Check that all data is appropriate length
                    if(FirstName.Length < 3 || LastName.Length < 3 || Email.Length < 3 || PhoneNumber.Length < 3 || Username.Length < 6 || Password.Length < 6)
                    {
                        return "false"; 
                    }

                    //Check that username doesn't exist
                    MySqlCommand checkUsernameCommand = new MySqlCommand("SELECT COUNT(*) FROM CustomerInfoTable WHERE Username = @Username", conn);
                    checkUsernameCommand.Parameters.Add("@Username", MySqlDbType.VarChar).Value = Username; 
                    if(int.Parse(checkUsernameCommand.ExecuteScalar().ToString()) >= 1)
                    {
                        return "Username Already Exists";
                    }

                    //If all checks passed then insert data in database table
                    MySqlCommand cmd = new MySqlCommand("INSERT INTO CustomerInfoTable (CustomerFirstName, CustomerLastName, CustomerPhoneNumber, CustomerEmail, DoesCustomerHaveOnlineAccount, Username, UsersPassword)" +
                        "VALUES (@FirstName, @LastName, @PhoneNumber, @Email, '0', @Username, @Password)", conn);
                    cmd.Parameters.Add("FirstName", MySqlDbType.VarChar).Value = FirstName;
                    cmd.Parameters.Add("LastName", MySqlDbType.VarChar).Value = LastName;
                    cmd.Parameters.Add("PhoneNumber", MySqlDbType.VarChar).Value = PhoneNumber;
                    cmd.Parameters.Add("Email", MySqlDbType.VarChar).Value = Email;
                    cmd.Parameters.Add("Username", MySqlDbType.VarChar).Value = Username;
                    cmd.Parameters.Add("Password", MySqlDbType.VarChar).Value = Password;
                    cmd.ExecuteNonQuery(); 

                    return "true"; 
                }
                catch(Exception e)
                {
                    return "Error: Try again later."; 
                }

            }

        }



        //___________________________________SIGN IN________________________________________
        public bool SignIntoAccount(string Username, string Password)
        {
            using (conn)
            {
                try
                {
                    conn.Open();
                    MySqlCommand cmd = new MySqlCommand("SELECT COUNT(*) FROM CustomerInfoTable WHERE Username = @Username AND UsersPassword = @Password", conn);
                    cmd.Parameters.Add("Username", MySqlDbType.VarChar).Value = Username; 
                    cmd.Parameters.Add("Password", MySqlDbType.VarChar).Value = Password;
                    //If username exists create session variables with user info
                    if (int.Parse(cmd.ExecuteScalar().ToString()) == 1)
                    {
                        MySqlCommand gatherInfoCommand = new MySqlCommand("SELECT * FROM CustomerInfoTable WHERE Username = @Username", conn);
                        gatherInfoCommand.Parameters.Add("Username", MySqlDbType.VarChar).Value = Username;
                        MySqlDataReader reader = gatherInfoCommand.ExecuteReader(); 
                        while(reader.Read())
                        {
                            contextAccess.HttpContext.Session.Set("Username", Encoding.ASCII.GetBytes(Username));
                            contextAccess.HttpContext.Session.Set("Password", Encoding.ASCII.GetBytes(Password));
                            contextAccess.HttpContext.Session.Set("FirstName", Encoding.ASCII.GetBytes(reader["CustomerFirstName"].ToString()));
                            contextAccess.HttpContext.Session.Set("LastName", Encoding.ASCII.GetBytes(reader["CustomerLastName"].ToString()));
                        }
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                catch(Exception e)
                {
                    return false; 
                }
            }
        }





}

}
