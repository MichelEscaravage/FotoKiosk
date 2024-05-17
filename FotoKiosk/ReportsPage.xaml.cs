using CsvHelper;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

namespace FotoKiosk
{
    public sealed partial class ReportsPage : Page
    {

        public ReportsPage()
        {
            this.InitializeComponent();
        }
        private void lvRapport_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        double ratio = 0;
        int orderAmount = 0;
        int productsSold = 0;
        double totalPrice = 0;


        private async void CsvButton_Click(object sender, RoutedEventArgs e)
        {
            var picker = new FileOpenPicker();
            picker.SuggestedStartLocation = PickerLocationId.Downloads;
            picker.FileTypeFilter.Add(".csv");


            var file = await picker.PickSingleFileAsync();
            if (file == null)
            {
                tbFileStatus.Text = "Geen geldig bestand gekozen!";
                // tbFileStatus.Text = "geen geldig bestand gekozen";
                return;
            }

            ReportSp.Visibility = Visibility.Visible;
            tbFileStatus.Text = file.Path;
            string fileNameSubString = file.Name.Substring(2, file.Name.Length - 6);
            reportHeader.Text = "Dagrapportage " + fileNameSubString;


            List<Report> reportList = new List<Report>();


            using (var fileAccess = await file.OpenReadAsync())
            {

                using (var stream = fileAccess.AsStreamForRead())
                {
                    using (var reader = new StreamReader(stream))
                    {
                        using (CsvReader csv = new CsvReader(reader, CultureInfo.InvariantCulture))
                        {
                            reportList = csv.GetRecords<Report>().ToList();


                        }
                    }
                }


                foreach (var report in reportList)
                {
                    orderAmount++;
                    productsSold = productsSold + report.Amount;
                    totalPrice = totalPrice + report.TotalPrice;
                }
                ratio = 100 * (orderAmount * 100) / (productsSold * 100);
                ratioPb.Value = ratio;
                ratioText.Text = ratio.ToString() + "%";
                orderAmountTb.Text = orderAmount.ToString();
                productsSoldTb.Text = productsSold.ToString();
                totalEarningsTb.Text = "€ " + Math.Round(totalPrice, 2).ToString();
            }
        }
        private async void saveReportB_Click(object sender, RoutedEventArgs e)
        {
            DateTime dateValue = DateTime.Now;
            int dayNum = (int)dateValue.DayOfWeek;
            int monthNum = dateValue.Month;
            int yearNum = dateValue.Year;
            int dayYear = dateValue.Day;
            int minute = dateValue.Minute;
            int hour = dateValue.Hour;
            string dayDutch = "";
            if (dayNum == 0)
            {
                dayDutch = "Zondag";
            }
            else if (dayNum == 1)
            {
                dayDutch = "Maandag";
            }
            else if (dayNum == 2)
            {
                dayDutch = "Dinsdag";
            }
            else if (dayNum == 3)
            {
                dayDutch = "Woensdag";
            }
            else if (dayNum == 4)
            {
                dayDutch = "Donderdag";
            }
            else if (dayNum == 5)
            {
                dayDutch = "Vrijdag";
            }
            else if (dayNum == 6)
            {
                dayDutch = "Zaterdag";
            }



            var savePicker = new Windows.Storage.Pickers.FileSavePicker();
            savePicker.SuggestedStartLocation = PickerLocationId.Downloads;
            savePicker.FileTypeChoices.Add("TXT Bestand", new List<string>() { ".txt" });
            savePicker.SuggestedFileName = $"Rapportage_{dayDutch}";



            Windows.Storage.StorageFile file = await savePicker.PickSaveFileAsync();
            if (file == null)
            {
                savingStatus.Text = "Opslaan geannuleerd.";
                return;
            }






            Windows.Storage.CachedFileManager.DeferUpdates(file);
            string fileText = $"Dagrapportage {dayDutch}\nOpgeslagen op: {dayYear}-{monthNum}-{yearNum} {hour}:{minute}\n\nRatio orders/bezoekers: {ratio}%\nAantal orders: {orderAmount}\nAantal producten totaal: {productsSold}\nTotale omzet: €{Math.Round(totalPrice, 2)}";
            await Windows.Storage.FileIO.WriteTextAsync(file, fileText);
            Windows.Storage.Provider.FileUpdateStatus status = await Windows.Storage.CachedFileManager.CompleteUpdatesAsync(file);



            if (status == Windows.Storage.Provider.FileUpdateStatus.Complete)
            {
                savingStatus.Text = "Bestand " + file.Name + " opgeslagen.";
            }
            else
            {
                savingStatus.Text = "Bestand opslaan mislukt.";
                return;
            }
        }
    }
}

