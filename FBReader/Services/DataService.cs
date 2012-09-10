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
        public string id;
        public string name;
        public string gender;
        public string relationship_status;

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
        }


        public FBData()
        {
            this.authService = new AuthService();
            this.httpClient = getHttpClient();
            //this.FetchUserProfile("btaylor");
            //string access_token = "AAAAAAITEghMBANKaSgSRJ6d0VP6LWBx8Ddbc2lPJlyewCmx4F1TZAG4ZB3xfZA8iZCLvwt6dB2d8TI3EiNP7POhD6SZCZAwABXT844til7UwZDZD";
            //this.FetchUserFriends("me", access_token);
            this.test();
        }

        private async void test()
        {
            List<FBMiniProfile> friends = await this.GetRStatusSingleFriends("me", authService.access_token);
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
            return _baseurl + username + "?access_token=" + access_token + "&expires_in=5184000&fields=id,name,relationship_status,friends.fields(relationship_status,gender,name)";
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

        public async Task<List<FBMiniProfile>> GetRStatusSingleFriends(string username, string access_token)
        {
            
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
