using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FBReader.Services
{
    class UrlGenerator
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

        public string constructPhotosUrl(string albumId,string access_token)
        {            return _baseurl + albumId + "/photos?" + access_token;
        }
    }
}
