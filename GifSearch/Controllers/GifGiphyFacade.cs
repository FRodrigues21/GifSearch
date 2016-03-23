﻿using GifSearch.Controllers;
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
using Windows.Web.Http;

namespace GifSearch
{
    class GifGiphyFacade
    {

        private static string apikey = "dc6zaTOxFJmzC";

        public static async Task<ObservableCollection<Datum>> searchGif(string search)
        {
            HttpClient http = new HttpClient();

            String url = String.Format("http://api.giphy.com/v1/gifs/search?q={1}&api_key={0}&limit={2}", apikey, search, UserFacade.getLimit());
            Uri uri = new Uri(url);
            var response = (HttpResponseMessage)null;
            response = await http.GetAsync(uri);
            var body = await response.Content.ReadAsStringAsync();

            RootObject_Giphy data = JsonConvert.DeserializeObject<RootObject_Giphy>(body);
            Debug.WriteLine("Search Gifs downloaded: " + data.data.Count);
            await Task.Delay(5000);
            return data.data;
        }

        public static async Task<ObservableCollection<Datum>> getTrending()
        {
            HttpClient http = new HttpClient();

            String url = String.Format("http://api.giphy.com/v1/gifs/trending?api_key={0}&limit={1}", apikey, UserFacade.getLimit());
            Uri uri = new Uri(url);

            var response = (HttpResponseMessage)null;
            response = await http.GetAsync(uri);
            var body = await response.Content.ReadAsStringAsync();

            RootObject_Giphy data = JsonConvert.DeserializeObject<RootObject_Giphy>(body);
            Debug.WriteLine("Trending Gifs downloaded: " + data.data.Count);
            await Task.Delay(5000);
            return data.data;
        }

    }
}
