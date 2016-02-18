using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GifSearch.Models
{
    public class Nanomp4
    {
        public string url { get; set; }
        public List<int> dims { get; set; }
        public double duration { get; set; }
        public string preview { get; set; }
    }

    public class Nanowebm
    {
        public string url { get; set; }
        public List<int> dims { get; set; }
        public string preview { get; set; }
    }

    public class Tinygif
    {
        public string url { get; set; }
        public List<int> dims { get; set; }
        public string preview { get; set; }
    }

    public class Tinymp4
    {
        public string url { get; set; }
        public List<int> dims { get; set; }
        public double duration { get; set; }
        public string preview { get; set; }
    }

    public class Tinywebm
    {
        public string url { get; set; }
        public List<int> dims { get; set; }
        public string preview { get; set; }
    }

    public class Webm
    {
        public string url { get; set; }
        public List<int> dims { get; set; }
        public string preview { get; set; }
    }

    public class Gif
    {
        public string url { get; set; }
        public List<int> dims { get; set; }
        public string preview { get; set; }
    }

    public class Mp4
    {
        public string url { get; set; }
        public List<int> dims { get; set; }
        public double duration { get; set; }
        public string preview { get; set; }
    }

    public class Nanogif
    {
        public string url { get; set; }
        public List<int> dims { get; set; }
        public string preview { get; set; }
    }

    public class Loopedmp4
    {
        public string url { get; set; }
        public List<int> dims { get; set; }
        public double duration { get; set; }
        public string preview { get; set; }
    }

    public class Medium
    {
        public Nanomp4 nanomp4 { get; set; }
        public Nanowebm nanowebm { get; set; }
        public Tinygif tinygif { get; set; }
        public Tinymp4 tinymp4 { get; set; }
        public Tinywebm tinywebm { get; set; }
        public Webm webm { get; set; }
        public Gif gif { get; set; }
        public Mp4 mp4 { get; set; }
        public Nanogif nanogif { get; set; }
        public Loopedmp4 loopedmp4 { get; set; }
    }

    public class Result
    {
        public Uri image_url {
            get {
                string url = "";
                foreach(Medium m in media)
                {
                    Debug.WriteLine(m.tinygif.preview);
                    url = m.tinygif.url;   
                }
                return new Uri(url);
            }
        }
        public string image_link
        {
            get
            {
                string url = "";
                foreach (Medium m in media)
                {
                    url = m.gif.url;
                }
                return url;
            }
        }
        public double created { get; set; }
        public string url { get; set; }
        public List<Medium> media { get; set; }
        public string title { get; set; }
        public int shares { get; set; }
        public string itemurl { get; set; }
        public object composite { get; set; }
        public bool hasaudio { get; set; }
        public string id { get; set; }
        public List<string> tags { get; set; }
        public bool? hascaption { get; set; }
    }

    public class RootObject_Riffsy
    {
        public string weburl { get; set; }
        public ObservableCollection<Result> results { get; set; }
        public string next { get; set; }
    }
}
