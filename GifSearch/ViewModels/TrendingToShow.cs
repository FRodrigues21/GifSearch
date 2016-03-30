using GifSearch.Models;
using GifSearch.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;

namespace GifSearch.ViewModels
{
    class TrendingToShow : ObservableCollection<Result>, ISupportIncrementalLoading
    {
        public int lastItem = 0;
        private int limit { get; set; }
        private uint each { get; set; }
        private ProgressBar progressBar { get; set; }
        private ObservableCollection<Result> source { get; set; }

        public TrendingToShow(ProgressBar progressBar, ObservableCollection<Result> source, int limit, int each)
        {
            this.progressBar = progressBar;
            this.source = source;
            this.limit = limit;
            this.each = (uint)each;
        }

        public bool HasMoreItems
        {
            get
            {
                if (lastItem == limit)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
        }

        public IAsyncOperation<LoadMoreItemsResult> LoadMoreItemsAsync(uint count)
        {

            CoreDispatcher coreDispatcher = Window.Current.Dispatcher;

            return Task.Run<LoadMoreItemsResult>(async () =>
            {

                await coreDispatcher.RunAsync(CoreDispatcherPriority.Normal,
                    () =>
                    {
                        progressBar.IsIndeterminate = true;
                        progressBar.Visibility = Visibility.Visible;
                    });

                List<Result> list = new List<Result>();
                for (int i = 0; i < each; i++)
                {
                    list.Add(source.ElementAt(lastItem));
                    lastItem++;
                    if (lastItem == limit)
                        break;
                }

                await coreDispatcher.RunAsync(CoreDispatcherPriority.Normal,
                   () =>
                   {
                       foreach(Result datum in list)
                       {
                           this.Add(datum);
                       }
                       progressBar.Visibility = Visibility.Collapsed;
                       progressBar.IsIndeterminate = false;
                   });

                return new LoadMoreItemsResult() { Count = count };
            }).AsAsyncOperation<LoadMoreItemsResult>();
        }
    }
}
