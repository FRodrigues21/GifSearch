using GifSearch.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Web.Http;

namespace GifSearch
{
    class GifRiffsyFacade
    {

        private static string apikey = "W6M99XAOXEPF";

        public static async Task<ObservableCollection<Result>> searchGif(string search)
        {
            HttpClient http = new HttpClient();

            String url = String.Format("http://api.riffsy.com/v1/search?key={0}&tag={1}&limit=8", apikey, search);
            Uri uri = new Uri(url);

            var response = await http.GetAsync(uri);
            var body = await response.Content.ReadAsStringAsync();

            RootObject_Riffsy data = JsonConvert.DeserializeObject<RootObject_Riffsy>(body);
            Debug.WriteLine("Search Gifs downloaded: " + data.results.Count);
            await Task.Delay(3000);
            return data.results;
        }

        public static async Task<ObservableCollection<Result>> getTrending()
        {
            HttpClient http = new HttpClient();

            String url = String.Format("http://api.riffsy.com/v1/trending?key={0}&limit=10", apikey);
            Uri uri = new Uri(url);

            var response = await http.GetAsync(uri);
            var body = await response.Content.ReadAsStringAsync();

            RootObject_Riffsy data = JsonConvert.DeserializeObject<RootObject_Riffsy>(body);
            Debug.WriteLine("Trending Gifs downloaded: " + data.results.Count);
            await Task.Delay(3000);
            return data.results;
        }


    }
}
