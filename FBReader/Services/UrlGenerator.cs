using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FBReader.Models;

namespace FBReader.Services
{
    public class UrlGenerator
    {
        private const string _baseurl = "https://graph.facebook.com/";

        public string ConstructProfileUrl(string username, string accessToken)
        {
            return _baseurl + username + "?" + accessToken + "&fields=id,name,relationship_status,friends.fields(relationship_status,gender,name)";
        }

        public string ConstructAlbumsUrl(string userid, string accessToken)
        {
            return _baseurl + userid + "/albums/?" + accessToken;
        }

        public string ConstructPhotosUrl(string albumId, string accessToken)
        {
            return _baseurl + albumId + "/photos?" + accessToken;
        }

        public string ConstructAuthUrl(string facebookClientID, string facebookCallbackUrl)
        {
            return "https://www.facebook.com/dialog/oauth?client_id=" + Uri.EscapeDataString(facebookClientID) +
                               "&redirect_uri=" + Uri.EscapeDataString(facebookCallbackUrl) + "&scope=read_stream,user_relationship_details,user_relationships,friends_photos,friends_relationships,user_online_presence&display=popup&response_type=token";
        }

        public string ConstructValidateAuthUrl(string accessToken)
        {
            return _baseurl + "me/?" + accessToken;
        }

        public ImageUrl GenProfileImageUrl(string userid)
        {
            string large_pic_url = "https://graph.facebook.com/" + userid + "/picture?type=large";
            string small_pic_url = "https://graph.facebook.com/" + userid + "/picture";
            ImageUrl image = new ImageUrl(small_pic_url, large_pic_url);
            return image;
        }


    }
}
