using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using FBReader.Models;
using FBReader.Services;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;

namespace FBReader.ViewModel
{
    public class MainPageViewModel : ViewModelBase
    {
        public ObservableCollection<FBMiniProfile> FBItems{ get; set; }

        private ICommand _logoutCommand;
        public ICommand LogoutCommand
        {
            get
            {
                if (_logoutCommand == null)
                {
                    _logoutCommand = new RelayCommand(Logout);
                }
                return _logoutCommand;
            }
        }

        public MainPageViewModel()
        {
            ViewModelLocator locator = (ViewModelLocator)App.Current.Resources["Locator"];
            this.FBItems = locator.FBData.FriendsList;
        }

        private void RefreshFBItems()
        {
            ViewModelLocator locator = (ViewModelLocator)App.Current.Resources["Locator"];
            locator.FBData.FriendsList.Clear();
            Debug.WriteLine("Cleared Fb items list");
            locator.FBData.GetRStatusSingleFriendsAsync();
        }

        private void Logout()
        {
            AuthService.FacebookLogout();
            RefreshFBItems();
        }
    }
}
