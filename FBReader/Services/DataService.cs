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
    }

    public class FBProfile : FBMiniProfile
    {
        public string first_name;
        public string last_name;
        public string link;
        public string username;
        public string gender;
        public string locale;
        public string relationship_status;
    }

    public class FriendsListContainer
    {
        public List<FBMiniProfile> data;
    }

    public class FBData
    {
        private const string _baseurl = "https://graph.facebook.com/";
        private HttpClient httpClient;

        private ObservableCollection<FBMiniProfile> _FBItems = new ObservableCollection<FBMiniProfile>();
        public ObservableCollection<FBMiniProfile> FBItems
        {
            get { return _FBItems; }
        }


        public FBData()
        {
            this.httpClient = getHttpClient();
            //this.FetchUserProfile("btaylor");
            string access_token = "AAAAAAITEghMBANKaSgSRJ6d0VP6LWBx8Ddbc2lPJlyewCmx4F1TZAG4ZB3xfZA8iZCLvwt6dB2d8TI3EiNP7POhD6SZCZAwABXT844til7UwZDZD";
            //this.FetchUserFriends("me", access_token);
            this.GetRStatusSingleFriends("me", access_token);
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

        public async Task<FBProfile> FetchUserProfile(string username, string auth_token)
        {
            try
            {

                string url = _baseurl + username + "?access_token=" + auth_token;
                var jsonResponse = await httpClient.GetByteArrayAsync(url);
                FBProfile profile = (FBProfile)parseJson(jsonResponse, typeof(FBProfile));

                //HttpResponseMessage response = await httpClient.GetAsync(_url);
                //response.EnsureSuccessStatusCode();

                //string statusText = response.StatusCode + " " + response.ReasonPhrase + Environment.NewLine;
                //string responseBodyAsText = await response.Content.ReadAsStringAsync();
                //responseBodyAsText = responseBodyAsText.Replace("<br>", Environment.NewLine); // Insert new lines
                //string outputView = responseBodyAsText;
                Debug.WriteLine("response {0}", profile.name);
                return profile;

            }
            catch (HttpRequestException hre)
            {
                Debug.WriteLine("http exception {0}", hre.ToString());
                return null;
            }
        }

        private bool IsGirlWithRStatusSingle(FBProfile profile)
        {
            if (profile.gender.Equals("male") || profile.relationship_status == null)
                return false;

            if (profile.relationship_status.Equals("Single"))
                return true;

            return false;
           
        }



        private async Task<List<FBMiniProfile>> FetchUserFriends(string username, string auth_token)
        {
            try
            {
                string url = _baseurl+ username + "/friends?access_token=" + auth_token;

                var jsonResponse = await httpClient.GetByteArrayAsync(url);
                FriendsListContainer friendListContainer = (FriendsListContainer)parseJson(jsonResponse, typeof(FriendsListContainer));

                //MemoryStream stream = new MemoryStream(jsonResponse);
                //DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(FriendsListContainer));

                //friends = (List<FBMiniProfile>)ser.ReadObject(stream);
                //FriendsListContainer test = (FriendsListContainer)ser.ReadObject(stream);

                Debug.WriteLine("friends {0}", friendListContainer.data);
                return friendListContainer.data;
            }
            catch (HttpRequestException hre)
            {
                Debug.WriteLine("http exception {0}", hre.ToString());
                return null;
            }

        }

        public async Task<List<FBProfile>> GetRStatusSingleFriends(string username, string auth_token)
        {
            List<FBProfile> singleProfiles = new List<FBProfile>();
            List<FBMiniProfile> friendsList = await FetchUserFriends(username, auth_token);

            foreach (var friend in friendsList)
            {
                FBProfile profile = await FetchUserProfile(friend.id, auth_token);
                if (IsGirlWithRStatusSingle(profile))
                    singleProfiles.Add(profile);
            }

            return singleProfiles;
        }



    }

}
