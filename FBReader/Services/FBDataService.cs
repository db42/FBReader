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

        private ObservableCollection<FBMiniProfile> _ProfilesList = new ObservableCollection<FBMiniProfile>();
        public ObservableCollection<FBMiniProfile> FriendsList
        {
            get { return _ProfilesList; }
        }

        public FBDataService(UrlGenerator urlGenerator, AuthService authService, JsonHelper jsonHelper, HttpClient httpClient)
        {
            this.urlGenerator = urlGenerator;
            this.authService = authService;
            this.jsonHelper = jsonHelper;
            this.httpClient = httpClient;
        }

        public async Task<FBProfile> FetchUserProfile(string username, string access_token)
        {
            string url = urlGenerator.constructProfileUrl(username, access_token);
            return await jsonHelper.FetchAndParseJson<FBProfile>(httpClient, url);
        }


        private async void FetchImageUrls(ObservableCollection<imageUrl> urls, string userid, string access_token)
        {
            string albumsUrl = urlGenerator.constructAlbumsUrl(userid, access_token);
            string profilePhotosAlbumId = null;
            Debug.WriteLine("album url {0}", albumsUrl);

            JsonAlbumContainer albumContainer = await jsonHelper.FetchAndParseJson<JsonAlbumContainer>(httpClient, albumsUrl);
            if (albumContainer == null || albumContainer.data.Length == 0)
            {
                string large_pic_url = "https://graph.facebook.com/" + userid + "/picture?type=large";
                string small_pic_url = "https://graph.facebook.com/" + userid + "/picture";
                imageUrl image = new imageUrl(small_pic_url, large_pic_url);
                urls.Add(image);
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
                string profilePhotosUrl = urlGenerator.constructPhotosUrl(profilePhotosAlbumId, access_token);
                JsonPhotoContainer photoContainer = await jsonHelper.FetchAndParseJson<JsonPhotoContainer>(httpClient, profilePhotosUrl);
                Debug.WriteLine("fetched photos length {0}", photoContainer.data.Length);
                foreach (var photo in photoContainer.data)
                {
                    string large_pic_url = photo.source;
                    string small_pic_url = photo.picture;
                    imageUrl image = new imageUrl(small_pic_url, large_pic_url);
                    urls.Add(image);
                }
            }
            return;
        }

        public async void GetRStatusSingleFriendsAsync()
        {
            Task<string> getAccessTokenTask = this.authService.FetchAuthToken();
            string access_token = await getAccessTokenTask;
            string username = "me";

            FBProfile userProfile = await FetchUserProfile(username, access_token);
            var friends = userProfile.friends.data;

            foreach (var friend in friends)
            {
                if (friend.IsGirlWithRStatusSingle())
                {
                    Debug.WriteLine("id {0}", friend.id);
                    FetchImageUrls(friend.urls, friend.id, access_token);
                    FriendsList.Add(friend);
                }
            }

        }
    }

}
