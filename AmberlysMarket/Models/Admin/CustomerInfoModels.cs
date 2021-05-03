using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AmberlysMarket.Models.Admin
{
    public class CustomerInfoModels
    {
        public string CustomerFirstName {get;set;}
        public string CustomerLastName {get;set;}
        public string CustomerID {get;set;}
        public string CustomerPhoneNumber {get;set;}
        public string CustomerEmail {get;set;}
        public string CustomerStreetAddress {get;set;}
        public string CustomerState {get;set;}
        public string CustomerZipCode {get;set;}
        public string DoesCustomerHaveOnlineAccount {get;set;}
    }
}
