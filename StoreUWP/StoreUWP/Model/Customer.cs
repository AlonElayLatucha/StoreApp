using System.Runtime.Serialization;

namespace StoreUWP.Model
{
    [DataContract]
    public class Customer
    {
        public Customer(int id, string lastName, string firstName)
        {
            this.customerID = id;
            this.lastName = lastName;
            this.firstName = firstName;
        }

        [DataMember]
        public int customerID;

        [DataMember]
        public string lastName;

        [DataMember]
        public string firstName;
    }
}