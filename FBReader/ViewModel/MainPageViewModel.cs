using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FBReader.Models;
using FBReader.Services;
using GalaSoft.MvvmLight;

namespace FBReader.ViewModel
{
    public class MainPageViewModel : ViewModelBase
    {
        public ObservableCollection<FBMiniProfile> FBItems{ get; set; }

        public MainPageViewModel()
        {
            ViewModelLocator locator = (ViewModelLocator)App.Current.Resources["Locator"];
            this.FBItems = locator.FBData.ProfilesList;
        }

        public void RefreshFBItems()
        {
            ViewModelLocator locator = (ViewModelLocator)App.Current.Resources["Locator"];
            locator.FBData.ProfilesList.Clear();
            Debug.WriteLine("Cleared Fb items list");
            locator.FBData.GetRStatusSingleFriendsAsync();
        }
    }
}
