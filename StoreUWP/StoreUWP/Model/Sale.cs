using System.Data.SqlTypes;
using System.Runtime.Serialization;

namespace StoreUWP.Model
{
    [DataContract]
    public class Sale
    {
        public Sale(int id, int itemID, int customerID, int quantity, SqlDateTime saleDate)
        {
            this.id = id;
            this.itemID = itemID;
            this.customerID = customerID;
            this.quantity = quantity;
            this.saleDate = saleDate;
        }

        [DataMember]
        public int id;

        [DataMember]
        public int itemID;

        [DataMember]
        public int customerID;

        [DataMember]
        public int quantity;

        [DataMember]
        public SqlDateTime saleDate;
    }
}