using GifImage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;

namespace GifSearch.Models
{
    public class PlayingItem
    {
        public PlayingItem() { }

        public Object instance { get; set; }
        public string linkType { get; set; }
        public Image control { get; set; }
        public GifImageSource sourceInstance { get; set; }
        public bool state { get; set; }
        public void play()
        {
            if(sourceInstance != null)
            {
                sourceInstance.Start();
                state = true;
            }
        }
        public void pause()
        {
            if(sourceInstance != null)
            {
                sourceInstance.Pause();
                state = false;
            }
        }
    }
}
