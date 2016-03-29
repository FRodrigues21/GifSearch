using GifSearch.Controllers;
using GifSearch.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Windows.Networking.Connectivity;
using Windows.Web.Http;

namespace GifSearch
{
    class GifRiffsyFacade
    {

        private static string apikey = "W6M99XAOXEPF";

        public static Boolean checkInternet()
        {
            var connectionProfile = NetworkInformation.GetInternetConnectionProfile();
            return (connectionProfile != null && connectionProfile.GetNetworkConnectivityLevel() == NetworkConnectivityLevel.InternetAccess);
        }

        public static async Task<ObservableCollection<Result>> searchGif(string search)
        {

            if (!checkInternet())
            {
                return null;
            }

            HttpClient http = new HttpClient();

            String url = String.Format("http://api.riffsy.com/v1/search?key={0}&tag={1}&limit={2}", apikey, search, 50);
            Uri uri = new Uri(url);

            var response = (HttpResponseMessage)null;
            response = await http.GetAsync(uri);
            var body = await response.Content.ReadAsStringAsync();

            RootObject_Riffsy data = JsonConvert.DeserializeObject<RootObject_Riffsy>(body);
            Debug.WriteLine("Search Gifs downloaded: " + data.results.Count);
            return data.results;
        }

        public static async Task<ObservableCollection<Result>> getTrending()
        {

            ObservableCollection<Result> tmp = await UserFacade.getTrendingList();
            if (!checkInternet())
            {
                if (tmp != null)
                    return tmp;
                return null;
            }

            Double time = UserFacade.getLastTrendingUpdate();
            if ((time > 0 && time <= 800) && tmp != null)
            {
                Debug.WriteLine("Using cached trending list!");
                return tmp;
            }

            HttpClient http = new HttpClient();

            String url = String.Format("http://api.riffsy.com/v1/trending?key={0}&limit={1}", apikey, 50);
            Uri uri = new Uri(url);

            var response = (HttpResponseMessage)null;
            response = await http.GetAsync(uri);
            Debug.WriteLine(response);
            var body = await response.Content.ReadAsStringAsync();
            RootObject_Riffsy data = JsonConvert.DeserializeObject<RootObject_Riffsy>(body);
            Debug.WriteLine("Trending Gifs downloaded: " + data.results.Count);
            return data.results;
        }


    }
}
