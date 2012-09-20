using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using FBReader.Models;
using FBReader.Models.Json;
using Windows.Web.Syndication;


namespace FBReader.Services
{
    public class FBDataService
    {
        private readonly HttpClient httpClient;
        private readonly AuthService authService;
        private readonly UrlGenerator urlGenerator;
        private readonly JsonHelper jsonHelper;

        private ObservableCollection<FBMiniProfile> _friendsList = new ObservableCollection<FBMiniProfile>();
        public ObservableCollection<FBMiniProfile> FriendsList
        {
            get { return _friendsList; }
        }

        public FBDataService(UrlGenerator urlGenerator, AuthService authService, JsonHelper jsonHelper, HttpClient httpClient)
        {
            this.urlGenerator = urlGenerator;
            this.authService = authService;
            this.jsonHelper = jsonHelper;
            this.httpClient = httpClient;
        }

        public async Task<FBProfile> FetchUserProfile(string username, string accessToken)
        {
            string url = urlGenerator.ConstructProfileUrl(username, accessToken);
            return await jsonHelper.FetchAndParseJson<FBProfile>(httpClient, url);
        }


        private async void FetchImageUrls(ObservableCollection<ImageUrl> urls, string userid, string accessToken)
        {
            string albumsUrl = urlGenerator.ConstructAlbumsUrl(userid, accessToken);
            string profilePhotosAlbumId = null;
            Debug.WriteLine("album url {0}", albumsUrl);

            JsonAlbumContainer albumContainer = await jsonHelper.FetchAndParseJson<JsonAlbumContainer>(httpClient, albumsUrl);
            if (albumContainer == null || albumContainer.data.Length == 0)
            {
                urls.Add(urlGenerator.GenProfileImageUrl(userid));
                return;
            }

            foreach (var album in albumContainer.data)
            {
                if (album.name.Equals("Profile Pictures"))
                {
                    profilePhotosAlbumId = album.id;
                }
            }

            if (profilePhotosAlbumId != null)
            {
                string profilePhotosUrl = urlGenerator.ConstructPhotosUrl(profilePhotosAlbumId, accessToken);
                JsonPhotoContainer photoContainer = await jsonHelper.FetchAndParseJson<JsonPhotoContainer>(httpClient, profilePhotosUrl);
                Debug.WriteLine("fetched photos length {0}", photoContainer.data.Length);
                foreach (var photo in photoContainer.data)
                {
                    urls.Add(photo.ConvertToImageUrl());
                }
            }
            return;
        }

        public async void GetRStatusSingleFriendsAsync()
        {
            Task<string> getAccessTokenTask = this.authService.FetchAuthToken();
            string accessToken = await getAccessTokenTask;
            string username = "me";

            FBProfile userProfile = await FetchUserProfile(username, accessToken);
            var friends = userProfile.friends.data;

            foreach (var friend in friends)
            {
                if (friend.IsGirlWithRStatusSingle())
                {
                    Debug.WriteLine("id {0}", friend.id);
                    FetchImageUrls(friend.urls, friend.id, accessToken);
                    FriendsList.Add(friend);
                }
            }

        }
    }

}
