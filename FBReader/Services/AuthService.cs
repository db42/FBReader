using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Security.Authentication.Web;

namespace FBReader.Services
{
    public class AuthService
    {
        private const string FacebookClientID = "328608250568415";
        private const string FacebookCallbackUrl = "https://www.facebook.com/connect/login_success.html";

        public AuthService()
        {
        }

        public async Task<string> FetchAuthToken()
        {
            try
            {
                String FacebookURL = "https://www.facebook.com/dialog/oauth?client_id=" + Uri.EscapeDataString(FacebookClientID) +
                   "&redirect_uri=" + Uri.EscapeDataString(FacebookCallbackUrl) + "&scope=read_stream,user_relationship_details,user_relationships,friends_relationships,user_online_presence&display=popup&response_type=token";
                System.Uri StartUri = new Uri(FacebookURL);
                System.Uri EndUri = new Uri(FacebookCallbackUrl);

                WebAuthenticationResult webAuthenticationResult = await WebAuthenticationBroker.AuthenticateAsync(WebAuthenticationOptions.None, StartUri, EndUri);
                if (webAuthenticationResult.ResponseStatus == WebAuthenticationStatus.Success)
                {
                    System.Diagnostics.Debug.WriteLine("Authentication Success");
                    string access_token = webAuthenticationResult.ResponseData.ToString().Substring(webAuthenticationResult.ResponseData.ToString().IndexOf('#') + 1);
                    System.Diagnostics.Debug.WriteLine("\nAcess Token is =" + access_token);
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
