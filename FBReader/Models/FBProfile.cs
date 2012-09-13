using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FBReader.Models
{
    public class FBProfile
    {
        public class FriendsListContainer
        {
            public List<FBMiniProfile> data;
        }
        public string first_name;
        public string last_name;
        public string link;
        public string username;
        public string locale;
        public FriendsListContainer friends;
    }
}
