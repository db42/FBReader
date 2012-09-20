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

            public ImageUrl ConvertToImageUrl()
            {
                string largePicUrl = this.source;
                string smallPicUrl = this.picture;
                ImageUrl image = new ImageUrl(smallPicUrl, largePicUrl);
                return image;
            }
        }

        public Photo[] data { get; set; }



    }
}
