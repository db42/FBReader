using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FBReader.Models.Json
{
    public class JsonAlbumContainer
    {
        public class Album
        {
            public string id { get; set; }
            public string name { get; set; }            
        }

        public Album[] data { get; set; }
    }
}
