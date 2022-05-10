using Pastel;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Text.RegularExpressions;
using WcfServiceLibrary1.Model;

namespace WcfServiceLibrary1
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Service1" in both code and config file together.
    public class Service1 : StoreInterface
    {
        public const string ConnectionString = @"Data Source=ALONHP\ALONSQLSERVER; Initial Catalog=StoreManagementDB; Integrated Security=True";

        private SqlConnection connection = new SqlConnection(ConnectionString);

        public bool AddToTable(object itemVar)
        {
            Type myType = itemVar.GetType();

            var fieldInfos = itemVar.GetType()
              .GetFields()
              .Select(field => field.Name)
              .ToList();

            var fieldValues = itemVar.GetType()
                     .GetFields()
                     .Select(field => field.GetValue(itemVar))
                     .ToList();

            List<object> fieldValuesList = fieldValues.ToList();
            int numOfFields = fieldValuesList.Count();
            for (int i = 0; i < numOfFields; i++)
            {
                if (fieldValuesList[i].GetType() == typeof(string))
                {
                    fieldValuesList[i] = $"\'{fieldValuesList[i]}\'"; // מוסיף גרשיים כי אחרת מסד הנתונים לא קורא כסטרינג ומשתגע
                }
                else if (fieldValuesList[i].GetType() == typeof(SqlDateTime))
                {
                    string[] only_date_arr = string.Concat(fieldValuesList[i].ToString().TakeWhile((c) => c != ' ')).Split('/');
                    string only_date_string = $"{only_date_arr[1]}/{only_date_arr[0]}/{only_date_arr[2]}";
                    fieldValuesList[i] = $"\'{only_date_string}\'"; // מוסיף גרשיים כי אחרת מסד הנתונים לא קורא כסטרינג ומשתגע
                }
            }

            string joinedInfos = string.Join(", ", fieldInfos);

            string joinedValues = string.Join(", ", fieldValuesList);


            string query = string.Format($"INSERT INTO {itemVar.GetType().Name}sTable" + $" ({joinedInfos})" + $" VALUES ({joinedValues});");
            Debug.Write($"\n\n*************SENDING QUERY***************************************\n{query}\n****************************************************\n\n".Pastel(Color.Green).PastelBg("#1E90FF"));
            SqlCommand cmd = new SqlCommand(query, connection);

            try
            {
                connection.Open();
                return cmd.ExecuteNonQuery() != 0;
            }
            catch (Exception e)
            {
                Debug.Write($"an error has accured : {e}");
                return false;
            }
            finally
            {
                if (connection.State == System.Data.ConnectionState.Open)
                {
                    connection.Close();
                }
            }

        }

        public bool RemoveAllFromTable(System.Type type)
        {
            string query = string.Format($"DELETE FROM {type.Name}sTable;");
            Debug.Write($"\n\n*************SENDING QUERY***************************************\n{query}\n****************************************************\n\n".Pastel(Color.Green).PastelBg("#1E90FF"));
            SqlCommand cmd = new SqlCommand(query, connection);

            try
            {
                connection.Open();
                return cmd.ExecuteNonQuery() != 0;
            }
            catch (Exception e)
            {
                Debug.Write($"an error has accured : {e}");
                return false;
            }
            finally
            {
                if (connection.State == System.Data.ConnectionState.Open)
                {
                    connection.Close();
                }
            }
        }

        public bool SendSQLQuery(string query)
        {
            SqlCommand cmd = new SqlCommand(query, connection);
            Debug.Write($"*************SENDING QUERY*************\n{query}\n**************************");

            try
            {
                connection.Open();
                return cmd.ExecuteNonQuery() != 0;
            }
            catch (Exception e)
            {
                Debug.Write($"an error has accured : {e}");
                return false;
            }
            finally
            {
                if (connection.State == System.Data.ConnectionState.Open)
                {
                    connection.Close();
                }
            }
        }

        public bool AddItem(int id, string name, int price)
        {
            try
            {
                SqlConnection connection = new SqlConnection(ConnectionString);
                Item item = new Item(id, name, price);

                return AddToTable(item);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"\n\n\nAn error has accured: {ex}\n\nADDING ITEM ERROR!\n\n\n");
                return false;
            }
            finally
            {
                if (connection.State == System.Data.ConnectionState.Open)
                {
                    connection.Close();
                }
            }
        }

        public bool AddCustomer(int id, string lastName, string firstName)
        {
            try
            {
                SqlConnection connection = new SqlConnection(ConnectionString);
                Customer customer = new Customer(id, lastName, firstName);

                return AddToTable(customer);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"\n\n\nAn error has accured: {ex}\n\nADDING CUSTOMER ERROR!\n\n\n");
                return false;
            }
            finally
            {
                if (connection.State == System.Data.ConnectionState.Open)
                {
                    connection.Close();
                }
            }
        }

        public bool AddSale(int id, int itemID, int customerID, int quantity, DateTime saleDate)
        {
            try
            {
                SqlConnection connection = new SqlConnection(ConnectionString);
                SqlDateTime sqlDateTime = new SqlDateTime(saleDate.Year, saleDate.Month, saleDate.Day);
                Sale sale = new Sale(id, itemID, customerID, quantity, sqlDateTime);

                return AddToTable(sale);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"\n\n\nAn error has accured: {ex}\n\nADDING SALE ERROR!\n\n\n");
                return false;
            }
            finally
            {
                if (connection.State == System.Data.ConnectionState.Open)
                {
                    connection.Close();
                }
            }
        }

        public List<Item> SelectAllItems()
        {
            SqlConnection connection = new SqlConnection(ConnectionString);
            string query = string.Format("SELECT * from ItemsTable", connection);
            SqlCommand cmd = new SqlCommand(query, connection);
            try
            {
                connection.Open();
                SqlDataReader sqlReader = cmd.ExecuteReader();
                List<Item> itemList = new List<Item>();
                while (sqlReader.Read())
                {
                    int itemID_temp = (int)sqlReader["itemID"];
                    string itemName_temp = Remove_Duplicate_Spaces_From_String(sqlReader["itemName"].ToString());
                    int price_temp = (int)sqlReader["price"];
                    Item item_temp = new Item(itemID_temp, itemName_temp, price_temp);
                    itemList.Add(item_temp);
                }
                Debug.WriteLine(itemList);
                return itemList;
            }
            catch (Exception e)
            {
                Debug.WriteLine($"\n\n\nAn error has accured: {e}\n\nSELECTING ALL ITEMS ERROR!\n\n\n");
                return null;
            }
            finally
            {
                if (connection.State == System.Data.ConnectionState.Open)
                {
                    connection.Close();
                }
            }

        }

        public List<Customer> SelectAllCustomers()
        {
            SqlConnection connection = new SqlConnection(ConnectionString);
            string query = string.Format("SELECT * from CustomersTable", connection);
            SqlCommand cmd = new SqlCommand(query, connection);
            try
            {
                connection.Open();
                SqlDataReader sqlReader = cmd.ExecuteReader();
                List<Customer> customerList = new List<Customer>();
                while (sqlReader.Read())
                {
                    int customerID_temp = (int)sqlReader["customerID"];
                    string customerLastName_temp = Remove_Duplicate_Spaces_From_String(sqlReader["lastName"].ToString());
                    string customerFirstName_temp = Remove_Duplicate_Spaces_From_String(sqlReader["firstName"].ToString());
                    Customer customer_temp = new Customer(customerID_temp, customerLastName_temp, customerFirstName_temp);
                    customerList.Add(customer_temp);
                }
                Debug.WriteLine(customerList);
                return customerList;
            }
            catch (Exception e)
            {
                Debug.WriteLine($"\n\n\nAn error has accured: {e}\n\nSELECTING ALL CUSTOMERS ERROR!\n\n\n");
                return null;
            }
            finally
            {
                if (connection.State == System.Data.ConnectionState.Open)
                {
                    connection.Close();
                }
            }
        }

        public List<Sale> SelectAllSalesByID(int id)
        {
            SqlConnection connection = new SqlConnection(ConnectionString);
            string query = string.Format($"SELECT * from SalesTable WHERE id={id}", connection);
            SqlCommand cmd = new SqlCommand(query, connection);
            try
            {
                connection.Open();
                SqlDataReader sqlReader = cmd.ExecuteReader();
                List<Sale> saleList = new List<Sale>();
                while (sqlReader.Read())
                {
                    int saleID_temp = (int)sqlReader["id"];
                    int itemID_temp = (int)sqlReader["itemID"];
                    int customerID_temp = (int)sqlReader["customerID"];
                    int quantity_temp = (int)sqlReader["quantity"];
                    string date_temp = sqlReader["saleDate"].ToString();

                    string[] only_date_arr = string.Concat(date_temp.ToString().TakeWhile((c) => c != ' ')).Split('/');
                    string only_date_string = $"{only_date_arr[1]}/{only_date_arr[0]}/{only_date_arr[2]}";

                    DateTime datetime_temp = new DateTime(int.Parse(only_date_arr[2]), int.Parse(only_date_arr[1]), int.Parse(only_date_arr[0]));
                    Sale sale_temp = new Sale(saleID_temp, customerID_temp, itemID_temp, quantity_temp, new SqlDateTime(datetime_temp));
                    saleList.Add(sale_temp);
                }
                Debug.WriteLine(saleList);
                return saleList;
            }
            catch (Exception e)
            {
                Debug.WriteLine($"\n\n\nAn error has accured: {e}\n\nSELECTING ALL SALES ERROR!\n\n\n");
                return null;
            }
            finally
            {
                if (connection.State == System.Data.ConnectionState.Open)
                {
                    connection.Close();
                }
            }
        }


        public List<SaleExtended> SelectAllSales()
        {
            SqlConnection connection = new SqlConnection(ConnectionString);
            string query = string.Format("SELECT CustomersTable.firstName, CustomersTable.lastName, ItemsTable.itemName, SalesTable.saleDate, ItemsTable.price, SalesTable.ID, ItemsTable.itemID, CustomersTable.customerID, SalesTable.quantity FROM SalesTable JOIN CustomersTable ON SalesTable.customerID = CustomersTable.customerID JOIN ItemsTable ON ItemsTable.itemID = SalesTable.itemID;", connection);
            SqlCommand cmd = new SqlCommand(query, connection);
            try
            {
                connection.Open();
                SqlDataReader sqlReader = cmd.ExecuteReader();
                List<SaleExtended> saleList = new List<SaleExtended>();
                while (sqlReader.Read())
                {
                    string customerFirstName_temp = sqlReader["firstName"].ToString();
                    string customerLastName_temp = sqlReader["lastName"].ToString();
                    string itemName_temp = sqlReader["itemName"].ToString();
                    string date_temp = sqlReader["saleDate"].ToString();
                    int itemPrice_temp = (int)sqlReader["price"];
                    int saleID_temp = (int)sqlReader["id"];
                    int itemID_temp = (int)sqlReader["itemID"];
                    int customerID_temp = (int)sqlReader["customerID"];
                    int quantity_temp = (int)sqlReader["quantity"];

                    string[] only_date_arr = string.Concat(date_temp.ToString().TakeWhile((c) => c != ' ')).Split('/');
                    string only_date_string = $"{only_date_arr[1]}/{only_date_arr[0]}/{only_date_arr[2]}";

                    DateTime datetime_temp = new DateTime(int.Parse(only_date_arr[2]), int.Parse(only_date_arr[1]), int.Parse(only_date_arr[0]));
                    SaleExtended sale_temp = new SaleExtended(customerFirstName_temp, customerLastName_temp, itemName_temp, only_date_string, itemPrice_temp, saleID_temp, itemID_temp, customerID_temp, quantity_temp);
                    saleList.Add(sale_temp);
                }
                Debug.WriteLine(saleList);
                return saleList;
            }
            catch (Exception e)
            {
                Debug.WriteLine($"\n\n\nAn error has accured: {e}\n\nSELECTING ALL SALES ERROR!\n\n\n");
                return null;
            }
            finally
            {
                if (connection.State == System.Data.ConnectionState.Open)
                {
                    connection.Close();
                }
            }
        }


        public bool SelectSales()
        {
            return SendSQLQuery($"SELECT firstName, lastName, itemName, saleDate, price, quantity FROM CustomersTable, SalesTable");
        }

        private string Remove_Duplicate_Spaces_From_String(string str)
        {
            return Regex.Replace(str, @"\s+", " ").Trim();
        }
    }
}