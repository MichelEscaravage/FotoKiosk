using CsvHelper;
using CsvHelper.Configuration;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.ServiceModel.Security;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace FotoKiosk
{
    public sealed partial class CheckoutPage : UserControl
    {
        public static CheckoutPage Instance;
        List<Product> products = new List<Product>();
        ObservableCollection<OrderedProduct> orderedProducts = new ObservableCollection<OrderedProduct>();
        double totalPrice;
        StorageFile file;

        private string ucFirst(string str)
        {
            if (str == null || str == "")
            {
                return null;
            }
            if (str.Length > 1)
            {
                return char.ToUpper(str[0]) + str.Substring(1);
            }
            return str.ToUpper();
        }
        public CheckoutPage()
        {
            this.InitializeComponent();
            Instance = this;
            checkoutEl.Visibility = Visibility.Collapsed;
            products.Add(new Product() { productName = "Foto 10x15", productPrice = 2.55, productPriceStr = "€ 2.55" });
            products.Add(new Product() { productName = "Foto 20x30", productPrice = 4.95, productPriceStr = "€ 4.95" });
            products.Add(new Product() { productName = "Mok met foto", productPrice = 9.95, productPriceStr = "€ 9.95" });
            products.Add(new Product() { productName = "Sleutelhanger", productPrice = 6.12, productPriceStr = "€ 6.12" });
            products.Add(new Product() { productName = "T-shirt", productPrice = 11.99, productPriceStr = "€ 11.99" });

            priceList.ItemsSource = products;
            priceList1.ItemsSource = products;
            comboEl.ItemsSource = products;
        }

        private async void addButtonEl_Click(object sender, RoutedEventArgs e)
        {
            double valueToAdd = 0;
            int selectedIndex = comboEl.SelectedIndex;
            int loopindex = 0;
            string chosenProductName = "";
            foreach (Product i in products)
            {
                if (loopindex == selectedIndex)
                {
                    chosenProductName = i.productName;
                    break;
                }
                loopindex++;
            }

            double chosenProductPrice = 0;
            foreach (Product choice in products)
            {

                //index door lijst heen

                if (choice.productName == chosenProductName)
                {
                    chosenProductPrice = choice.productPrice;
                    break;
                }
            }
            bool success;
            if (int.TryParse(amtEl.Text, out int amtOfProduct))
            {
                success = true;
            }
            else
            {
                success = false;
            }

            if (success)
            {
                valueToAdd += amtOfProduct * chosenProductPrice;
            }
            totalPrice += valueToAdd;

            //convert to €00.00 format

            //-----------------------------------------------------------------------------------

            //adding cultureinfo makes sure you get a euro-sign instead of the standard option, the dollar-sign
            CultureInfo myCulture = new CultureInfo("nl-NL");
            //adding "C" as a parameter to ToString converts it to a currency, adding myculture does what the previous comment states
            string formatedTotal = totalPrice.ToString("C", myCulture);
            //-----------------------------------------------------------------------------------
            finalPriceEl.Text = /*"\u00A3 " + */formatedTotal;

            //Sprint 2

            int fotoId = int.Parse(IdEl.Text);

            double totalPrice1 = chosenProductPrice * amtOfProduct;
            string totalPrice2 = totalPrice1.ToString("C", myCulture);

            orderedProducts.Add(new OrderedProduct { productName = chosenProductName, amt = amtOfProduct, totalPrice = chosenProductPrice * amtOfProduct, totalPriceStr = totalPrice2, fotoNum = fotoId });

            //write to csv

            var configProducts = new CsvConfiguration(CultureInfo.InvariantCulture);

            var myOrderedProducts = new List<OrderedProduct>()
            {
                new OrderedProduct { productName = chosenProductName, amt = amtOfProduct, totalPrice= chosenProductPrice*amtOfProduct, fotoNum = fotoId }
            };

            //using (var stream = await ApplicationData.Current.LocalFolder.OpenStreamForWriteAsync(storageFolder1, CreationCollisionOption.OpenIfExists))


            //using (var stream = await file.OpenStreamForWriteAsync())
            //using (var writer = File.AppendText(file.Path))
            //{
            //await writer.WriteLineAsync($"\n{fotoId}, {chosenProductName}, {amtOfProduct}, {chosenProductPrice*amtOfProduct}");
            //}

            await FileIO.AppendTextAsync(file, $"{fotoId}, {chosenProductName}, {amtOfProduct}, {chosenProductPrice * amtOfProduct}\n");




            receiptEl.ItemsSource = orderedProducts;
        }
        private void resetButtonEl_Click(object sender, RoutedEventArgs e)
        {
            IdEl.Text = string.Empty;
            amtEl.Text = string.Empty;
            comboEl.SelectedItem = null;
            finalPriceEl.Text = string.Empty;
            totalPrice = 0;
            orderedProducts.Clear();
        }

        private async void fileSaver_Click(object sender, RoutedEventArgs e)
        {
            var savePicker = new Windows.Storage.Pickers.FileSavePicker();
            savePicker.SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.DocumentsLibrary;
            savePicker.FileTypeChoices.Add("CSV", new List<string>() { ".csv" });

            DateTime today = DateTime.Now;
            string dayString = today.ToString("dddd", new CultureInfo("nl-NL"));
            string dayStringUpper = ucFirst(dayString);

            savePicker.SuggestedFileName = $"{(int)today.DayOfWeek}_{dayStringUpper}";

            file = await savePicker.PickSaveFileAsync();

            if (file != null)
            {
                bool fileExitsts = await FileExistsAsync(file);

                if (fileExitsts)
                {
                    checkoutEl.Visibility = Visibility.Visible;
                    rapportEl.Visibility = Visibility.Collapsed;
                    // Clear the contents of the existing file
                    await FileIO.WriteTextAsync(file, string.Empty);
                    using (var stream = await file.OpenStreamForWriteAsync())
                    using (var writer = new StreamWriter(stream))
                    {

                        await writer.WriteLineAsync("PhotoId, Product, Amount, TotalPrice");
                    }
                }
                else
                {
                    checkoutEl.Visibility = Visibility.Visible;
                    rapportEl.Visibility = Visibility.Collapsed;
                    using (var stream = await file.OpenStreamForWriteAsync())
                    using (var writer = new StreamWriter(stream))
                    {
                        {

                            await writer.WriteLineAsync("PhotoId, Product, Amount, TotalPrice");
                        }
                    }//create the else statement for if the file already exists
                }
                async Task<bool> FileExistsAsync(StorageFile file)
                {
                    try
                    {
                        var newFile = await file.CopyAsync(ApplicationData.Current.TemporaryFolder, file.Name, NameCollisionOption.FailIfExists);
                        File.Delete(newFile.Path);
                        return true;
                    }
                    catch (FileNotFoundException)
                    {
                        return false;
                    }
                }
            }
        }
    }
}
