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

        private ObservableCollection<FBMiniProfile> _ProfilesList = new ObservableCollection<FBMiniProfile>();
        public ObservableCollection<FBMiniProfile> ProfilesList
        {
            get { return _ProfilesList; }
        }


        public FBDataService(UrlGenerator urlGenerator, AuthService authService)
        {
            this.urlGenerator = urlGenerator;
            this.authService = authService;
            this.httpClient = getHttpClient();
        }

        private static HttpClient getHttpClient()
        {
            HttpClient httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Add("user-agent", "Mozilla/5.0 (compatible; MSIE 10.0; Windows NT 6.2; WOW64; Trident/6.0)");
            return httpClient;
        }



        public async Task<FBProfile> FetchUserProfile(string username, string access_token)
        {
            try
            {
                string url = urlGenerator.constructProfileUrl(username, access_token);
                var jsonResponse = await httpClient.GetByteArrayAsync(url);
                FBProfile profile = (FBProfile)JsonHelper.ParseJson(jsonResponse, typeof(FBProfile));
                return profile;

            }
            catch (HttpRequestException hre)
            {
                Debug.WriteLine("http exception {0}", hre.ToString());
                return null;
            }
        }

        private bool IsGirlWithRStatusSingle(FBMiniProfile profile)
        {
            if (profile.gender == null || profile.gender.Equals("male") || profile.relationship_status == null)
                return false;

            if (profile.relationship_status.Equals("Single"))
                return true;

            return false;
           
        }

        private async void FetchImageUrls(ObservableCollection<imageUrl> urls, string userid, string access_token)
        {
            try
            {
                string albumsUrl = urlGenerator.constructAlbumsUrl(userid, access_token); 
                var jsonResponse = await httpClient.GetByteArrayAsync(albumsUrl);
                string profilePhotosAlbumId = null;
                Debug.WriteLine("album url {0}", albumsUrl);

                JsonAlbumContainer albumContainer = (JsonAlbumContainer)JsonHelper.ParseJson(jsonResponse, typeof(JsonAlbumContainer));
                Debug.WriteLine("data length {0}", albumContainer.data.Length);

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
                    jsonResponse = await httpClient.GetByteArrayAsync(profilePhotosUrl);
                    JsonPhotoContainer photoContainer = (JsonPhotoContainer)JsonHelper.ParseJson(jsonResponse, typeof(JsonPhotoContainer));
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
            catch (HttpRequestException hre)
            {
                Debug.WriteLine("http exception {0}", hre.ToString());
                return;
            }
        }
        public async void GetRStatusSingleFriendsAsync()
        {
            Task<string> getAccessTokenTask = this.authService.FetchAuthToken();
            string access_token = await getAccessTokenTask;
            string username = "me";

            FBProfile userProfile = await FetchUserProfile(username, access_token);

            foreach (var profile in userProfile.friends.data)
            {
                if (IsGirlWithRStatusSingle(profile))
                {
                    Debug.WriteLine("id {0}", profile.id);
                    FetchImageUrls(profile.urls, profile.id, access_token);
                    ProfilesList.Add(profile);
                }
            }

        }
    }

}
