using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using Windows.Security.Authentication.Web;
using System.Net;
using System.IO;

namespace FBReader.Services
{
    public class AuthService
    {
        private const string FacebookClientID = "328608250568415";
        private const string FacebookCallbackUrl = "https://www.facebook.com/connect/login_success.html";

        public AuthService()
        {
        }


        public async Task<bool> isAuthTokenValidForUser(string user_name)
        {
            string access_token;
            if (Windows.Storage.ApplicationData.Current.RoamingSettings.Values[user_name] == null) return false;
            else access_token = Windows.Storage.ApplicationData.Current.RoamingSettings.Values[user_name].ToString();
            string _authTokenValidationUrl = "https://graph.facebook.com/" + user_name + "/?" + access_token;
            System.Diagnostics.Debug.WriteLine("\n\nauthTokenValidation url {0}\n\n", _authTokenValidationUrl);
            WebRequest request = WebRequest.Create(_authTokenValidationUrl);
            try
            {
                System.Diagnostics.Debug.WriteLine("\n\nauthTokenValidation url {0}\n\n", _authTokenValidationUrl);
                var jsonResponse = await request.GetResponseAsync();
                return true;
            }
            catch (WebException e)
            {
                using (WebResponse response = e.Response)
                {
                    HttpWebResponse httpResponse = (HttpWebResponse)response;
                    System.Diagnostics.Debug.WriteLine("Error code: {0}", httpResponse.StatusCode);
                    using (Stream data = response.GetResponseStream())
                    {
                        string text = new StreamReader(data).ReadToEnd();
                        System.Diagnostics.Debug.WriteLine(text);
                    }
                }
                return false;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("\n Exception in isAuthTokenValidForUser  " + ex.ToString());
                return false;
            }

        }

        public async Task<string> FetchAuthToken()
        {
            Task<bool> authTokenCheckTask = isAuthTokenValidForUser("me");
            bool isAuthTokenValid = await authTokenCheckTask;
            
            try
            {
                String FacebookURL = "https://www.facebook.com/dialog/oauth?client_id=" + Uri.EscapeDataString(FacebookClientID) +
                   "&redirect_uri=" + Uri.EscapeDataString(FacebookCallbackUrl) + "&scope=read_stream,user_relationship_details,user_relationships,friends_photos,friends_relationships,user_online_presence&display=popup&response_type=token";
                System.Uri StartUri = new Uri(FacebookURL);
                System.Uri EndUri = new Uri(FacebookCallbackUrl);
                WebAuthenticationResult webAuthenticationResult;
                
                if (isAuthTokenValid == false)
                {
                    webAuthenticationResult = await WebAuthenticationBroker.AuthenticateAsync(WebAuthenticationOptions.None, StartUri, EndUri);
                }
                else return Windows.Storage.ApplicationData.Current.RoamingSettings.Values["me"].ToString();

                if (webAuthenticationResult.ResponseStatus == WebAuthenticationStatus.Success)
                {
                    System.Diagnostics.Debug.WriteLine("Authentication Success");
                    string access_token = webAuthenticationResult.ResponseData.ToString().Substring(webAuthenticationResult.ResponseData.ToString().IndexOf('#') + 1);
                    System.Diagnostics.Debug.WriteLine("\nAcess Token is =" + access_token);
                    Windows.Storage.ApplicationData.Current.RoamingSettings.Values["me"] = access_token;
                    return access_token;

                }
                else if (webAuthenticationResult.ResponseStatus == WebAuthenticationStatus.ErrorHttp)
                {
                    System.Diagnostics.Debug.WriteLine("HTTP Error returned by AuthenticateAsync() : " + webAuthenticationResult.ResponseErrorDetail.ToString());
                    return null;
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("Error returned by AuthenticateAsync() : " + webAuthenticationResult.ResponseStatus.ToString());
                    return null;

                }
            }
            catch (Exception Error)
            {
                System.Diagnostics.Debug.WriteLine(Error.ToString());
                return null;
            }
        }
    }
}
