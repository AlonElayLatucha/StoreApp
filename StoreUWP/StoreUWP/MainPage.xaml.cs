using StoreUWP.ServiceReference1;
using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Diagnostics;
using System.Linq;
using Windows.UI.Notifications;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace StoreUWP
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public ServiceReference1.StoreInterfaceClient proxy = new ServiceReference1.StoreInterfaceClient();
        private bool dateChanged = false;

        public MainPage()
        {
            this.InitializeComponent();

            ElementSoundPlayer.State = ElementSoundPlayerState.On;
            ElementSoundPlayer.SpatialAudioMode = ElementSpatialAudioMode.On;
        }

        public async void Launch(string uriToLaunch)
        {
            var uri = new Uri(uriToLaunch);

            var success = await Windows.System.Launcher.LaunchUriAsync(uri);

            if (success)
            {
                // URI launched
            }
            else
            {
                // URI launch failed
            }
        }

        private void ShowToastNotification(string title, string stringContent)
        {
            ToastNotifier ToastNotifier = ToastNotificationManager.CreateToastNotifier();
            Windows.Data.Xml.Dom.XmlDocument toastXml = ToastNotificationManager.GetTemplateContent(ToastTemplateType.ToastText02);
            Windows.Data.Xml.Dom.XmlNodeList toastNodeList = toastXml.GetElementsByTagName("text");
            toastNodeList.Item(0).AppendChild(toastXml.CreateTextNode(title));
            toastNodeList.Item(1).AppendChild(toastXml.CreateTextNode(stringContent));
            Windows.Data.Xml.Dom.IXmlNode toastNode = toastXml.SelectSingleNode("/toast");
            Windows.Data.Xml.Dom.XmlElement audio = toastXml.CreateElement("audio");
            audio.SetAttribute("src", "ms-winsoundevent:Notification.SMS");

            ToastNotification toast = new ToastNotification(toastXml);
            toast.ExpirationTime = DateTime.Now.AddSeconds(4);
            ToastNotifier.Show(toast);
        }

        private void customerID_TextBox_BeforeTextChanging(TextBox sender, TextBoxBeforeTextChangingEventArgs args)
        {
            args.Cancel = args.NewText.Any(c => !char.IsDigit(c));
        }

        private void customerLastName_TextBox_BeforeTextChanging(TextBox sender, TextBoxBeforeTextChangingEventArgs args)
        {
            args.Cancel = args.NewText.Any(c => char.IsDigit(c));
        }

        private void customerFirstName_TextBox_BeforeTextChanging(TextBox sender, TextBoxBeforeTextChangingEventArgs args)
        {
            args.Cancel = args.NewText.Any(c => char.IsDigit(c));
        }

        private void customerSurvey_SendButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                proxy.AddCustomerAsync(int.Parse(customerID_TextBox.Text), customerLastName_TextBox.Text, customerFirstName_TextBox.Text);
                Sync_CustomersTable();
                ShowToastNotification("הוספת לקוח", "בוצע בהצלחה!");
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                ShowToastNotification("הוספת לקוח", "לא עבד, ח'ביבי.");
            }
            finally
            {
            }
        }

        private void DeleteConfirmation_Click(object sender, RoutedEventArgs e)
        {
            if (this.DeleteDatabase_FlyoutButton.Flyout is Flyout f)
            {
                try
                {
                    proxy.SendSQLQueryAsync(@"
                                            -- disable referential integrity
                                            EXEC sp_MSForEachTable 'ALTER TABLE ? NOCHECK CONSTRAINT ALL'");
                    proxy.SendSQLQueryAsync(@"EXEC sp_MSForEachTable 'DELETE FROM ?'");
                    proxy.SendSQLQueryAsync(@"-- enable referential integrity again
                                            EXEC sp_MSForEachTable 'ALTER TABLE ? WITH CHECK CHECK CONSTRAINT ALL'");
                    Sync_CustomersTable();
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"\n\n\n********************************\nPROBLEM!\n\n{ex}\n********************************\n\n\n");
                    ShowToastNotification("איפוס מסד נתונים", "לא עבד, ח'ביבי.");
                }
                finally
                {
                    ShowToastNotification("איפוס מסד נתונים", "בוצע בהצלחה!");
                }
            }
        }

        private void itemID_TextBox_BeforeTextChanging(TextBox sender, TextBoxBeforeTextChangingEventArgs args)
        {
            args.Cancel = args.NewText.Any(c => !char.IsDigit(c));
        }

        private void itemName_TextBox_BeforeTextChanging(TextBox sender, TextBoxBeforeTextChangingEventArgs args)
        {
            args.Cancel = args.NewText.Any(c => char.IsDigit(c));
        }

        private void itemPrice_TextBox_BeforeTextChanging(TextBox sender, TextBoxBeforeTextChangingEventArgs args)
        {
            args.Cancel = args.NewText.Any(c => !char.IsDigit(c));
        }

        private void itemsSurvey_SendButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                proxy.AddItemAsync(int.Parse(itemID_TextBox.Text), itemName_TextBox.Text, int.Parse(itemPrice_TextBox.Text));
                Sync_ItemsTable();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                ShowToastNotification("הוספת פריט", "לא עבד, ח'ביבי.");
            }
            finally
            {
                ShowToastNotification("הוספת פריט", "בוצע בהצלחה!");
            }
        }

        private void saleID_TextBox_BeforeTextChanging(TextBox sender, TextBoxBeforeTextChangingEventArgs args)
        {
            args.Cancel = args.NewText.Any(c => !char.IsDigit(c));
        }

        private void saleItemID_TextBox_BeforeTextChanging(TextBox sender, TextBoxBeforeTextChangingEventArgs args)
        {
            args.Cancel = args.NewText.Any(c => !char.IsDigit(c));
        }

        private void saleCustomerID_TextBox_BeforeTextChanging(TextBox sender, TextBoxBeforeTextChangingEventArgs args)
        {
            args.Cancel = args.NewText.Any(c => !char.IsDigit(c));
        }

        private void saleItemQuantity_TextBox_BeforeTextChanging(TextBox sender, TextBoxBeforeTextChangingEventArgs args)
        {
            args.Cancel = args.NewText.Any(c => !char.IsDigit(c));
        }

        private void sale_DataPicker_DateChanged(object sender, DatePickerValueChangedEventArgs e)
        {
            dateChanged = true;
        }

        private void salesSurvey_SendButton_Click(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine($"\n\n\n\n\n********{int.Parse(saleID_NumberBox.Text)}, {saleItemID_ComboBox.SelectedValue}, {saleCustomerID_ComboBox.SelectedValue}, {int.Parse(saleItemQuantity_NumberBox.Text)}, {sale_DatePicker.Date.UtcDateTime}********\n\n\n\n\n");
            //Console.WriteLine($"\n\n\n\n\n********{int.Parse(saleID_NumberBox.Text)}, {int.Parse(saleItemID_ComboBox.Text)}, {int.Parse(saleCustomerID_ComboBox.Text)}, {int.Parse(saleItemQuantity_NumberBox.Text)}, {sale_DatePicker.Date.UtcDateTime}********\n\n\n\n\n");
            try
            {
                _ = proxy.AddSaleAsync(int.Parse(saleID_NumberBox.Text), (int)saleItemID_ComboBox.SelectedValue, (int)saleCustomerID_ComboBox.SelectedValue, int.Parse(saleItemQuantity_NumberBox.Text), sale_DatePicker.Date.UtcDateTime);
                Sync_SalesTable();
                ShowToastNotification("הוספת מכירה", "בוצע בהצלחה!");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"\n\n\nAn error has accured: {ex}\nADD SALE CLICK ERROR\n\n");
                ShowToastNotification("הוספת מכירה", "לא עבד, ח'ביבי.");
            }
            finally
            {
            }
            dateChanged = false;
        }

        private void RickRollButton_Click(object sender, RoutedEventArgs e)
        {
            Launch($"https://youtu.be/dQw4w9WgXcQ");
        }

        private async void Sync_CustomersTable()
        {
            try
            {
                ServiceReference1.StoreInterfaceClient proxy = new ServiceReference1.StoreInterfaceClient();
                var customersResult = await proxy.SelectAllCustomersAsync();
                Debug.WriteLine($"\n\n\n{customersResult}\n\n\n");
                clientsList.ItemsSource = customersResult;

                List<string> customersNameList = new List<string>();
                List<int> customerIDList = new List<int>();
                int numOfCustomers = customersResult.Count;
                for (int i = 0; i < numOfCustomers; i++)
                {
                    var currCustomer = customersResult[i];
                    customersNameList.Add($"{currCustomer.firstName} {currCustomer.lastName}");
                    customerIDList.Add(currCustomer.customerID);
                }
                saleCustomerID_ComboBox.ItemsSource = customerIDList;

                var mostPopularCustomerName = customersNameList.GroupBy(i => i).OrderByDescending(grp => grp.Count()).Select(grp => grp.Key).Where(x => x != null).First();
                MostPopular_Customer_Name_TextBox.Text = mostPopularCustomerName;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"\n\n\nAn error has accured: {ex}\nREFRESH CUSTOMER CLICK ERROR\n\n");
            }
        }

        private async void Sync_ItemsTable()
        {
            try
            {
                ServiceReference1.StoreInterfaceClient proxy = new ServiceReference1.StoreInterfaceClient();
                var itemsResult = await proxy.SelectAllItemsAsync();
                Debug.WriteLine($"\n\n\n{itemsResult}\n\n\n");
                itemsList.ItemsSource = itemsResult;

                List<string> itemNameList = new List<string>();
                List<int> itemIDList = new List<int>();
                int numOfItems = itemsResult.Count;
                for (int i = 0; i < numOfItems; i++)
                {
                    var currItem = itemsResult[i];
                    itemNameList.Add($"{currItem.itemName}");
                    itemIDList.Add(currItem.itemID);
                }
                saleItemID_ComboBox.ItemsSource = itemIDList;

                var mostPopularName = itemNameList.GroupBy(i => i).OrderByDescending(grp => grp.Count()).Select(grp => grp.Key).Where(x => x != null).First();
                MostPopular_Item_TextBox.Text = mostPopularName;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"\n\n\nAn error has accured: {ex}\nREFRESH CUSTOMER CLICK ERROR\n\n");
            }
        }

        private async void Sync_SalesTable()
        {
            try
            {
                ServiceReference1.StoreInterfaceClient proxy = new ServiceReference1.StoreInterfaceClient();
                var salesResult = await proxy.SelectAllSalesAsync();
                Debug.WriteLine($"\n\n\n{salesResult}\n\n\n");
                salesList.ItemsSource = salesResult;

                List<string> customerFirstNameList = new List<string>();
                List<string> customerLastNameList = new List<string>();
                List<string> itemNameList = new List<string>();
                List<string> salesDateList = new List<string>();
                List<int> saleID_List = new List<int>();
                List<int> itemID_List = new List<int>();
                List<int> customerID_List = new List<int>();
                List<int> quantity_List = new List<int>();

                int numOfSales = salesResult.Count;
                for (int i = 0; i < numOfSales; i++)
                {
                    customerFirstNameList.Add(salesResult[i].customerFirstName);
                    customerLastNameList.Add(salesResult[i].customerLastName);
                    itemNameList.Add(salesResult[i].itemName);
                    salesDateList.Add(salesResult[i].saleDate);
                    saleID_List.Add(salesResult[i].id);
                    itemID_List.Add(salesResult[i].itemID);
                    customerID_List.Add(salesResult[i].customerID);
                    quantity_List.Add(salesResult[i].quantity);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"\n\n\nAn error has accured: {ex}\nREFRESH SALES CLICK ERROR\n\n");
            }
        }

        private async void Sync_ByItem_SalesTable(int id)
        {
            try
            {
                ServiceReference1.StoreInterfaceClient proxy = new ServiceReference1.StoreInterfaceClient();
                var salesResult = await proxy.SelectAllSalesByIDAsync(id);
                Debug.WriteLine($"\n\n\n{salesResult}\n\n\n");
                byCode_SalesList.ItemsSource = salesResult;

                List<int> saleID = new List<int>();
                List<int> quantity = new List<int>();
                List<string> salesDateList = new List<string>();
                int numOfSales = salesResult.Count;
                for (int i = 0; i < numOfSales; i++)
                {
                    quantity[i] = salesResult[i].quantity;

                    string[] only_date_arr = string.Concat(salesResult[i].saleDate.Date.ToShortDateString().ToString().TakeWhile((c) => c != ' ')).Split('/');
                    string only_date_string = $"{only_date_arr[1]}/{only_date_arr[0]}/{only_date_arr[2]}";

                    Debug.WriteLine(only_date_arr);

                    salesDateList.Add(only_date_string);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"\n\n\nAn error has accured: {ex}\nREFRESH SALES CLICK ERROR\n\n");
            }
        }

        private async void Items_PivotItem_Loading(FrameworkElement sender, object args)
        {
        }

        private async void Sales_PivotItem_Loading(FrameworkElement sender, object args)
        {
        }

        private async void Refresh_Customers_Click(object sender, RoutedEventArgs e)
        {
            Sync_CustomersTable();
        }

        private void Page_Loading(FrameworkElement sender, object args)
        {
            SyncAllTables();
        }

        private void Refresh_Items_Click(object sender, RoutedEventArgs e)
        {
            Sync_ItemsTable();
        }

        private void Delete_Items_Click(object sender, RoutedEventArgs e)
        {
            //proxy.RemoveAllFromTableAsync((ServiceReference1.Type)typeof(Item));
        }

        private void Delete_Sales_Click(object sender, RoutedEventArgs e)
        {
            //proxy.RemoveAllFromTableAsync((ServiceReference1.Type)typeof(Sale));
        }

        private void Refresh_Sales_Click(object sender, RoutedEventArgs e)
        {
            Sync_SalesTable();
        }

        public void SyncAllTables()
        {
            Sync_CustomersTable();
            Sync_ItemsTable();
            Sync_SalesTable();
        }

        private void SyncAllTablesButton_Click(object sender, RoutedEventArgs e)
        {
            SyncAllTables();
        }

        private void salesById_Survey_SendButton_Click(object sender, RoutedEventArgs e)
        {
            Sync_ByItem_SalesTable(int.Parse(byCode_CodeNumberBox.Text));
        }
    }
}