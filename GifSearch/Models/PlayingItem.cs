using GifImage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GifSearch.Models
{
    public class PlayingItem
    {
        public PlayingItem() { }

        public GifImageSource instance { get; set; }
        public bool state { get; set; }
        public void play()
        {
            if(instance != null)
            {
                instance.Start();
                state = true;
            }
        }
        public void pause()
        {
            if(instance != null)
            {
                instance.Pause();
                state = false;
            }
        }
    }
}
