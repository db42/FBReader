using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FBReader.Models
{
    public struct ImageUrl
    {
        private string _smallPicUrl;
        public string SmallPicUrl 
        {
            get { return _smallPicUrl; }

        }
        private string _largePicUrl;
        public string LargePicUrl 
        {
            get { return _largePicUrl; } 
        }

        public ImageUrl(string smallPic, string largePic)
        {
            _smallPicUrl = smallPic;
            _largePicUrl = largePic;
        }
    }

    public class FBMiniProfile
    {
        public string id {get; set;}
        public string name { get; set; }
        public string gender { get; set; }
        public string relationship_status { get; set; }
        private ObservableCollection<ImageUrl> _urls = new ObservableCollection<ImageUrl>();
        public ObservableCollection<ImageUrl> urls 
        {
            get { return _urls; }
        }

        public bool IsGirlWithRStatusSingle()
        {
            if (this.gender == null ||  this.relationship_status == null)
                return false;

            if (this.gender.Equals("female") && this.relationship_status.Equals("Single"))
                return true;

            return false;
           
        }
    }
}
