using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GifSearch.Models
{
    class FavoriteItem
    {

        public String type { get; set; }
        public Object obj { get; set; }

        public FavoriteItem(String type, Object obj)
        {
            this.type = type;
            this.obj = obj;
        }

    }
}
