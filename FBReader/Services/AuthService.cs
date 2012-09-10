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
        private String FacebookClientID = "264928293626639";
        private String FacebookCallbackUrl = "https://www.facebook.com/connect/login_success.html";
        private String access_token;
        public AuthService()
        {
            FetchAuthToken();


        }



        private async void FetchAuthToken()
        {

            try
            {
                String FacebookURL = "https://www.facebook.com/dialog/oauth?client_id=" + Uri.EscapeDataString(FacebookClientID) +
                   "&redirect_uri=" + Uri.EscapeDataString(FacebookCallbackUrl) + "&scope=read_stream,user_relationship_details,user_relationships,friends_relationships,user_online_presence&display=popup&response_type=token";
                System.Uri StartUri = new Uri(FacebookURL);
                System.Uri EndUri = new Uri(FacebookCallbackUrl);

                WebAuthenticationResult WebAuthenticationResult = await WebAuthenticationBroker.AuthenticateAsync(
                                                        WebAuthenticationOptions.None,
                                                        StartUri,
                                                        EndUri);
                if (WebAuthenticationResult.ResponseStatus == WebAuthenticationStatus.Success)
                {
                    System.Diagnostics.Debug.WriteLine("Authentication Success");
                    access_token = WebAuthenticationResult.ResponseData.ToString().Substring(WebAuthenticationResult.ResponseData.ToString().IndexOf('#') + 1);
                    System.Diagnostics.Debug.WriteLine("\nAcess Token is =" + access_token);

                }
                else if (WebAuthenticationResult.ResponseStatus == WebAuthenticationStatus.ErrorHttp)
                {
                    System.Diagnostics.Debug.WriteLine("HTTP Error returned by AuthenticateAsync() : " + WebAuthenticationResult.ResponseErrorDetail.ToString());
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("Error returned by AuthenticateAsync() : " + WebAuthenticationResult.ResponseStatus.ToString());

                }
            }
            catch (Exception Error)
            {
                System.Diagnostics.Debug.WriteLine(Error.ToString());
            }
        }
    }
}
