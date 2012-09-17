using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FBReader.Models;
using FBReader.Services;

namespace FBReader.ViewModels
{
    class MainPageViewModel
    {
        public ObservableCollection<FBMiniProfile> FBItems{ get; set; }

        public MainPageViewModel()
        {
            FBData fbData = (FBData)App.Current.Resources["fbData"];
            this.FBItems = fbData.ProfilesList;
        }

        public void RefreshFBItems()
        {
            FBData fbData = (FBData)App.Current.Resources["fbData"];
            fbData.ProfilesList.Clear();
            Debug.WriteLine("Cleared Fb items list");
            fbData.GetRStatusSingleFriendsAsync();
        }
    }
}
