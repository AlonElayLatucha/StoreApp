using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace WcfServiceLibrary1.Model
{
    [DataContract]

    public class Item
    {
        public Item(int id, string name, int price)
        {
            this.itemID = id;
            this.itemName = name;
            this.price = price;
        }

        [DataMember]

        public int itemID;
        [DataMember]

        public string itemName;
        [DataMember]

        public int price;
    }
}
