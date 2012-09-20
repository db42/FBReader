using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using Windows.Security.Authentication.Web;
using System.Net;
using System.IO;
using System.Diagnostics;

namespace FBReader.Services
{
    public class AuthService
    {
        private const string facebookClientID = "328608250568415";
        private const string facebookCallbackUrl = "https://www.facebook.com/connect/login_success.html";
        private UrlGenerator urlGenerator;

        public AuthService(UrlGenerator urlGenerator)
        {
            this.urlGenerator = urlGenerator;
        }


        public async Task<bool> isAuthTokenValidForUser()
        {
            string accessToken;
            if (Windows.Storage.ApplicationData.Current.RoamingSettings.Values["oauth_token"] == null)
            {
                return false;
            }

            accessToken = Windows.Storage.ApplicationData.Current.RoamingSettings.Values["oauth_token"].ToString();
            string _authTokenValidationUrl = urlGenerator.ConstructValidateAuthUrl(accessToken);
            Debug.WriteLine("\n\nauthTokenValidation url {0}\n\n", _authTokenValidationUrl);
            WebRequest request = WebRequest.Create(_authTokenValidationUrl);
            try
            {
                Debug.WriteLine("\n\nauthTokenValidation url {0}\n\n", _authTokenValidationUrl);
                var jsonResponse = await request.GetResponseAsync();
                return true;
            }
            catch (WebException e)
            {
                using (WebResponse response = e.Response)
                {
                    HttpWebResponse httpResponse = (HttpWebResponse)response;
                    Debug.WriteLine("Error code: {0}", httpResponse.StatusCode);
                    using (Stream data = response.GetResponseStream())
                    {
                        string text = new StreamReader(data).ReadToEnd();
                        Debug.WriteLine(text);
                    }
                }
                return false;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("\n Exception in isAuthTokenValidForUser  " + ex.ToString());
                return false;
            }

        }

        public async Task<string> FetchAuthToken()
        {
            Task<bool> authTokenCheckTask = isAuthTokenValidForUser();
            bool isAuthTokenValid = await authTokenCheckTask;
            if (isAuthTokenValid == true)
            {
                return Windows.Storage.ApplicationData.Current.RoamingSettings.Values["oauth_token"].ToString();
            }

            try
            {
                String facebookAuthURL = urlGenerator.ConstructAuthUrl(facebookClientID, facebookCallbackUrl);
                System.Uri startUri = new Uri(facebookAuthURL);
                System.Uri endUri = new Uri(facebookCallbackUrl);
                WebAuthenticationResult webAuthenticationResult = await WebAuthenticationBroker.AuthenticateAsync(WebAuthenticationOptions.None, startUri, endUri);
                if (webAuthenticationResult.ResponseStatus == WebAuthenticationStatus.Success)
                {
                    Debug.WriteLine("Authentication Success");
                    string access_token = webAuthenticationResult.ResponseData.ToString().Substring(webAuthenticationResult.ResponseData.ToString().IndexOf('#') + 1);
                    Debug.WriteLine("\nAcess Token is =" + access_token);
                    Windows.Storage.ApplicationData.Current.RoamingSettings.Values["oauth_token"] = access_token;
                    return access_token;

                }
                else if (webAuthenticationResult.ResponseStatus == WebAuthenticationStatus.ErrorHttp)
                {
                    Debug.WriteLine("HTTP Error returned by AuthenticateAsync() : " + webAuthenticationResult.ResponseErrorDetail.ToString());
                    return null;
                }
                else
                {
                    Debug.WriteLine("Error returned by AuthenticateAsync() : " + webAuthenticationResult.ResponseStatus.ToString());
                    return null;

                }
            }
            catch (Exception Error)
            {
                Debug.WriteLine(Error.ToString());
                return null;
            }
        }

        public static void FacebookLogout()
        {
            Windows.Storage.ApplicationData.Current.RoamingSettings.Values["oauth_token"] = null;
        }


    }
}
