using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FBReader.Models
{
    public struct imageUrl
    {
        private string _small_pic_url;
        public string small_pic_url 
        {
            get { return _small_pic_url; }

        }
        private string _large_pic_url;
        public string large_pic_url 
        {
            get { return _large_pic_url; } 
        }

        public imageUrl(string small_pic, string large_pic)
        {
            _small_pic_url = small_pic;
            _large_pic_url = large_pic;
        }
    }

    public class FBMiniProfile
    {
        public string id {get; set;}
        public string name { get; set; }
        public string gender { get; set; }
        public string relationship_status { get; set; }
        private ObservableCollection<imageUrl> _urls = new ObservableCollection<imageUrl>();
        public ObservableCollection<imageUrl> urls 
        {
            get { return _urls; }
        }
    }
}
