using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Security.Cryptography.X509Certificates;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Globalization;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Shapes;
using static System.Net.Mime.MediaTypeNames;

namespace FotoKiosk
{
    public sealed partial class PhotoPage : UserControl
    {
        public int CountDown_Seconds = 15;

        
        public PhotoPage()
        {
            this.InitializeComponent();
            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(15);
            timer.Tick += Timer_Tick;
            timer.Start();
            loadPhotos();
        }
        private void Timer_Tick(object sender, object e)
        {
            loadPhotos();
        }
        private async void loadPhotos()
        {
            var now = DateTime.Now;
            int day = (int)now.DayOfWeek;
            var appFolder = Windows.ApplicationModel.Package.Current.InstalledLocation;
            var fotosFolder = await appFolder.GetFolderAsync("Assets\\Fotos");
            var dayFolders = await fotosFolder.GetFoldersAsync();

            List<string> imagePaths = new List<string>();
            List<string> filteredImagePaths = new List<string>();

            var targetDayFolder = dayFolders.FirstOrDefault(folder =>
            {
                string[] folderParts = folder.Name.Split('_');
                if (folderParts.Length >= 2 && int.TryParse(folderParts[0], out int folderDay))
                {
                    return folderDay == day;
                }
                return false;
            });
            
            if (targetDayFolder != null)
            {
                var queryOptions = new Windows.Storage.Search.QueryOptions();
                queryOptions.FolderDepth = Windows.Storage.Search.FolderDepth.Shallow;
                queryOptions.ApplicationSearchFilter = "*"; // Retrieve all files in the folder
                var queryResult = targetDayFolder.CreateFileQueryWithOptions(queryOptions);
                var dayFiles = await queryResult.GetFilesAsync();
                imagePaths.AddRange(dayFiles.Select(file => file.Path));
                

                foreach (string path in imagePaths)
                {
                    string fileName = System.IO.Path.GetFileName(path);
                    string mySubString = fileName.Substring(0,8);
                    DateTime.TryParseExact(fileName.Substring(0, 8), "HH_mm_ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime photoTime);
                    DateTime MinutesMin30 = DateTime.Now.AddMinutes(-30);
                    DateTime MinutesMinus2 = DateTime.Now.AddMinutes(-2);

                    foreach (string path2 in imagePaths)
                    {
                       
                        string fileName2 = System.IO.Path.GetFileName(path2);
                        string mySubString2 = fileName2.Substring(0, 8);
                        DateTime.TryParseExact(fileName2.Substring(0, 8), "HH_mm_ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime photoTime2);
                        DateTime addMinute = photoTime.AddMinutes(1);
                        
                        if (photoTime > MinutesMin30 && photoTime < MinutesMinus2 && photoTime2 == addMinute)
                        {
                            //addMinute = photoTime.AddMinutes(1); //Ticket 3
                            
                            filteredImagePaths.Add(path);
                            filteredImagePaths.Add(path2);


                            // if (addMinute ==  //Ticket 3
                            //filteredImagePaths.Add(); //Ticket 3
                        }
                    }

                    
                }
            }
            allPhotosGridEl.ItemsSource = filteredImagePaths;
        }

    }
}
