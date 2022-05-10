using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace WcfServiceLibrary1.Model
{
    [DataContract]

    public class SaleExtended
    {
        public SaleExtended(string customerFirstName, string customerLastName, string itemName, string saleDate, int itemPrice, int id, int itemID, int customerID, int quantity)
        {
            this.customerFirstName = customerFirstName;
            this.customerLastName = customerLastName;
            this.itemName = itemName;
            this.saleDate = saleDate;
            this.itemPrice = itemPrice;
            this.id = id;
            this.itemID = itemID;
            this.customerID = customerID;
            this.quantity = quantity;
        }

        [DataMember] public string customerFirstName;
        [DataMember] public string customerLastName;
        [DataMember] public string itemName;
        [DataMember] public string saleDate;
        [DataMember] public int itemPrice;
        [DataMember] public int id;
        [DataMember] public int itemID;
        [DataMember] public int customerID;
        [DataMember] public int quantity;
    }
}
