using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FBReader.Models.Json
{
    public class JsonPhotoContainer
    {
        public class Photo
        {
            public string picture { get; set; }
            public string source { get; set; }

            public imageUrl ConvertToImageUrl()
            {
                string large_pic_url = this.source;
                string small_pic_url = this.picture;
                imageUrl image = new imageUrl(small_pic_url, large_pic_url);
                return image;
            }
        }

        public Photo[] data { get; set; }



    }
}
