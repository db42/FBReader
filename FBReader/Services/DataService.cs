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
    }

    public class FBProfile : FBMiniProfile
    {
        public string id;
        public string name;
        public string first_name;
        public string last_name;
        public string link;
        public string username;
        public string gender;
        public string locale;
    }

    public class FBData
    {
        private const string _baseurl = "https://graph.facebook.com/";
        private HttpClient httpClient;

        private ObservableCollection<FBMiniProfile> _FBItems = new ObservableCollection<FBMiniProfile>();
        public ObservableCollection<FBMiniProfile> FBItems{
            get { return _FBItems; }
        }

        public FBData()
        {
            this.httpClient = getHttpClient();
            this.FetchUserProfile("btaylor");    
        }

        private static HttpClient getHttpClient()
        {
            HttpClient httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Add("user-agent", "Mozilla/5.0 (compatible; MSIE 10.0; Windows NT 6.2; WOW64; Trident/6.0)");
            return httpClient;
        }

        private static FBProfile parseJson(byte[] jsonResponse)
        {

            MemoryStream stream = new MemoryStream(jsonResponse);
            DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(FBProfile));

            FBProfile profile;
            profile = (FBProfile)ser.ReadObject(stream);

            return profile;
        }

        public async void FetchUserProfile(string username) 
        {
            try
            {
                
                string url = _baseurl + username;
                var jsonResponse = await httpClient.GetByteArrayAsync(url);
                FBProfile profile = parseJson(jsonResponse);

                //HttpResponseMessage response = await httpClient.GetAsync(_url);
                //response.EnsureSuccessStatusCode();

                //string statusText = response.StatusCode + " " + response.ReasonPhrase + Environment.NewLine;
                //string responseBodyAsText = await response.Content.ReadAsStringAsync();
                //responseBodyAsText = responseBodyAsText.Replace("<br>", Environment.NewLine); // Insert new lines
                //string outputView = responseBodyAsText;
                Debug.WriteLine("response {0}", profile.name);
                
            }
            catch (HttpRequestException hre)
            {
                Debug.WriteLine("http exception {0}", hre.ToString());
            }
        }   



    }

}
