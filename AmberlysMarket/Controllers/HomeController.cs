using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using AmberlysMarket.Models;
using System.Text;

namespace AmberlysMarket.Controllers
{
    public class HomeController : Controller
    {
        //Global Variables
        private AmberlysMarket.Controllers.Security.AmberlyMarketDatabase amberlyMarketDatabase = new Security.AmberlyMarketDatabase();


        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Shop()
        {
            return View(); 
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult SignIn()
        {
            return View(); 
        }

        //Sign into account
        public bool SignIntoAccount(string Username, string Password)
        {
            return amberlyMarketDatabase.SignIntoAccount(Username, Password);
        }

        public IActionResult CreateAnAccount()
        {
            return View();
        }

        //Submit new account details
        public string CreateNewCustomer(string FirstName, string LastName, string Email, string PhoneNumber, string Username, string Password)
        {
            return amberlyMarketDatabase.CreateNewCustomer(FirstName, LastName, Email, PhoneNumber, Username, Password); 
        }


        //My Account
        public IActionResult MyAccount()
        {
            return View(); 
        }

        public IActionResult GetQuote()
        {
            return View(); 
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
