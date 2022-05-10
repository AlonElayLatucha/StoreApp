using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using WcfServiceLibrary1.Model;

namespace WcfServiceLibrary1
{
    [ServiceContract]

    public interface StoreInterface
    {
        [OperationContract]
        bool AddToTable(object itemVar);

        [OperationContract]
        bool SendSQLQuery(string query);

        [OperationContract]
        bool RemoveAllFromTable(System.Type type);

        [OperationContract]
        bool AddItem(int id, string name, int price);

        [OperationContract]
        bool AddCustomer(int id, string lastName, string firstName);

        [OperationContract]
        bool AddSale(int id, int itemID, int customerID, int quantity, DateTime saleDate);

        [OperationContract]
        List<Item> SelectAllItems();

        [OperationContract]
        List<Customer> SelectAllCustomers();

        [OperationContract]
        List<SaleExtended> SelectAllSales();

        [OperationContract]
        List<Sale> SelectAllSalesByID(int id);


        [OperationContract]
        bool SelectSales();

    }
}
