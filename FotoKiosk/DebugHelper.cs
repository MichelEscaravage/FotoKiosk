using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.UI.Popups;

namespace FotoKiosk
{
    public static class DebugHelper
    {
        // Gebruik deze methode om een popup met bericht te laten zien.
        // Bijvoorbeeld:
        //         await DebugHelper.ShowDialog("Hello World");
        public static async Task ShowDialog(string text)
        {
            var dialog = new MessageDialog(text);
            await dialog.ShowAsync();
        }



        // Gebruik deze methode om één popup met alle items uit een List te laten zien:
        public async static Task ShowList(IEnumerable<object> list)
        {
            string text = "";
            if (list == null)
            {
                text = "List passed was NULL";
            }
            else if(list is IEnumerable<StorageFile>)
            {
                foreach (StorageFile item in list)
                {
                    text += item.Path + "\n";
                }
            }
            else if (list is IEnumerable<StorageFolder>)
            {
                foreach (StorageFolder item in list)
                {
                    text += item.Path + "\n";
                }
            }
            else if (list is IEnumerable<StorageFile>)
            {
                foreach (StorageFile item in list)
                {
                    text += item.Path + "\n";
                }
            }
            else
            {
                foreach (var item in list)
                {
                    text += item.ToString();
                }
            }

            var dialog = new MessageDialog(text);
            await dialog.ShowAsync();
        }

        // Gebruik deze methode om één popup met alle bestanden in een folder te laten zien:
        public static async Task ShowFolder(StorageFolder folder)
        {
            string text = folder.Path + "\n";
            foreach(var item in await folder.GetFilesAsync())
            {
                text += "  \\" + item.Name + "\n";
            }

            var dialog = new MessageDialog(text);
            await dialog.ShowAsync();
        }
    }
}
