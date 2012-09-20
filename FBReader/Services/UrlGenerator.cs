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

        public string constructProfileUrl(string username, string access_token)
        {
            return _baseurl + username + "?" + access_token + "&fields=id,name,relationship_status,friends.fields(relationship_status,gender,name)";
        }

        public string constructAlbumsUrl(string userid, string access_token)
        {
            return _baseurl + userid + "/albums/?" + access_token;
        }

        public string constructPhotosUrl(string albumId, string access_token)
        {
            return _baseurl + albumId + "/photos?" + access_token;
        }

        public string constructAuthUrl(string facebookClientID, string facebookCallbackUrl)
        {
            return "https://www.facebook.com/dialog/oauth?client_id=" + Uri.EscapeDataString(facebookClientID) +
                               "&redirect_uri=" + Uri.EscapeDataString(facebookCallbackUrl) + "&scope=read_stream,user_relationship_details,user_relationships,friends_photos,friends_relationships,user_online_presence&display=popup&response_type=token";
        }

        public string constructValidateAuthUrl(string access_token)
        {
            return _baseurl + "me/?" + access_token;
        }

        public imageUrl GenProfileImageUrl(string userid)
        {
            string large_pic_url = "https://graph.facebook.com/" + userid + "/picture?type=large";
            string small_pic_url = "https://graph.facebook.com/" + userid + "/picture";
            imageUrl image = new imageUrl(small_pic_url, large_pic_url);
            return image;
        }


    }
}
