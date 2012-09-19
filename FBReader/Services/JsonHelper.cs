using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;

namespace FBReader.Services
{
    public class JsonHelper
    {
        private object ParseJson(byte[] jsonResponse, Type returnObjectType)
        {

            MemoryStream stream = new MemoryStream(jsonResponse);
            DataContractJsonSerializer ser = new DataContractJsonSerializer(returnObjectType);

            Object returnObject;
            returnObject = ser.ReadObject(stream);

            return returnObject;
        }

        public async Task<T> FetchAndParseJson<T>(HttpClient httpClient, string url)
        {
            try
            {
                var jsonResponse = await httpClient.GetByteArrayAsync(url);
                T profile = (T)ParseJson(jsonResponse, typeof(T));
                return profile;

            }
            catch (HttpRequestException hre)
            {
                Debug.WriteLine("http exception {0}", hre.ToString());
                return default(T);
            }
        }
    }
}
