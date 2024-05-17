using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace FotoKiosk
{
    public sealed partial class SearchPage : Page
    {
        public SearchPage()
        {
            this.InitializeComponent();
            List<Search> hourList = new List<Search>();
            List<Search> minuteList = new List<Search>();
            List<Search> dayList = new List<Search>();
            List<string> daystringList = new List<string>
            {
                "Zondag",
                "Maandag",
                "Dinsdag",
                "Woensdag",
                "Donderdag",
                "Vrijdag",
                "Zaterdag"
            };

            for (int i = 6; i <= 20; i++)
            {
                Search hour = new Search();
                hour.Time = i.ToString();
                hourList.Add(hour);
            }
            for (int j = 0; j <= 11; j++)
            {
                int minute = j * 5;
                Search minutes = new Search();
                minutes.Time = minute.ToString();
                minuteList.Add(minutes);

            }
            foreach (string daystring in daystringList)
            {
                Search day = new Search();
                day.Time = daystring;
                dayList.Add(day);
            }
            selectHourEL.ItemsSource = hourList;
            selectMinuteEL.ItemsSource = minuteList;
            selectDayEL.ItemsSource = dayList;
        }

        private async void Search_Click(object sender, RoutedEventArgs e)
        {
            var appFolder = Windows.ApplicationModel.Package.Current.InstalledLocation;
            var fotosFolder = await appFolder.GetFolderAsync("Assets\\Fotos");
            var dayFolders = await fotosFolder.GetFoldersAsync();
            var now = DateTime.Now;
            int day = (int)now.DayOfWeek;
            List<Image> photoList = new List<Image>();
            foreach (var folder in dayFolders)
            {
                string[] foldername = folder.DisplayName.Split('_', '.');
                string foldertje = foldername[1];

                string dayvalue = (string)selectDayEL.SelectedValue;
                if (dayvalue == foldertje)
                {
                    var fileList = await folder.GetFilesAsync();

                    foreach (var file in fileList)
                    {
                        var filepath = new Image();
                        filepath.Source = file.Path;
                        string hours = (string)selectHourEL.SelectedValue;
                        string minutes = (string)selectMinuteEL.SelectedValue;
                        string selectTime = $"{hours}:{minutes}";
                        string[] filetime = file.DisplayName.Split('_', '.');
                        string fileTime = $"{filetime[0]}:{filetime[1]}";

                        DateTime timeFile = DateTime.Parse(fileTime);
                        DateTime selectedTime = DateTime.Parse(selectTime);


                        if (timeFile <= selectedTime.AddMinutes(10) && timeFile >= selectedTime.AddMinutes(-10))
                        {
                            photoList.Add(filepath);
                        }
                    }
                }
            }
            gvSearchedPhotoEL.ItemsSource = photoList;
        }
    }
}
