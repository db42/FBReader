using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;
using Windows.Web.Syndication;


namespace FBReader.Services
{
    public class FBMiniProfile
    {
        public string id {get; set;}
        public string name { get; set; }
        public string gender { get; set; }
        public string relationship_status { get; set; }

    }

    public class FBProfile : FBMiniProfile
    {
        public class FriendsListContainer
        {
            public List<FBMiniProfile> data;
        }
        public string first_name;
        public string last_name;
        public string link;
        public string username;
        public string locale;
        public FriendsListContainer friends;
    }

    

    public class FBData
    {
        private const string _baseurl = "https://graph.facebook.com/";

        private HttpClient httpClient;
        private AuthService authService;

        private ObservableCollection<FBMiniProfile> _FBItems = new ObservableCollection<FBMiniProfile>();
        public ObservableCollection<FBMiniProfile> FBItems
        {
            get { return _FBItems; }
            set { _FBItems = value; }
        }


        public FBData()
        {
            this.authService = new AuthService();
            this.httpClient = getHttpClient();
        }

        public async void FetchRStatusSingleFriendsAsync()
        {
            List<FBMiniProfile> friends = await this.GetRStatusSingleFriendsAsync();
            foreach (var friend in friends)
            {
                FBItems.Add(friend);
                Debug.WriteLine("Added {0}", friend.name);
            }
        }


        private static HttpClient getHttpClient()
        {
            HttpClient httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Add("user-agent", "Mozilla/5.0 (compatible; MSIE 10.0; Windows NT 6.2; WOW64; Trident/6.0)");
            return httpClient;
        }

        private static object parseJson(byte[] jsonResponse, Type returnObjectType)
        {

            MemoryStream stream = new MemoryStream(jsonResponse);
            DataContractJsonSerializer ser = new DataContractJsonSerializer(returnObjectType);

            Object returnObject;
            returnObject = ser.ReadObject(stream);

            return returnObject;
        }

        private string constructProfileUrl(string username, string access_token)
        {
            return _baseurl + username + "?" + access_token + "&fields=id,name,relationship_status,friends.fields(relationship_status,gender,name)";
        }

        public async Task<FBProfile> FetchUserProfile(string username, string access_token)
        {
            try
            {

                string url = constructProfileUrl(username, access_token);
                var jsonResponse = await httpClient.GetByteArrayAsync(url);
                FBProfile profile = (FBProfile)parseJson(jsonResponse, typeof(FBProfile));
                Debug.WriteLine("response {0}", profile.name);
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
            if (profile.gender.Equals("male") || profile.relationship_status == null)
                return false;

            if (profile.relationship_status.Equals("Single"))
                return true;

            return false;
           
        }

        private async Task<List<FBMiniProfile>> GetRStatusSingleFriendsAsync()
        {
            Task<string> getAccessTokenTask = this.authService.FetchAuthToken();
            string access_token = await getAccessTokenTask;
            string username = "me";

            FBProfile userProfile = await FetchUserProfile(username, access_token);

            List <FBMiniProfile> resultProfiles = new List<FBMiniProfile>();

            foreach (var profile in userProfile.friends.data)
            {
                if (IsGirlWithRStatusSingle(profile))
                    resultProfiles.Add(profile);
            }

            return resultProfiles;
        }



    }

}
