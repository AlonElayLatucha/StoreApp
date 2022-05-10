using System.Runtime.Serialization;

namespace StoreUWP.Model
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