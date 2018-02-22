using GalaSoft.MvvmLight.Views;
using pinoelefante.ViewModels;
using StudioServices.Data.Newsboard;
using StudioServices.Registry.Data;
using StudioServicesApp.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace StudioServicesApp.ViewModels
{
    public class NewsPageViewModel : MyViewModel
    {
        private CacheManager cache;
        private DatabaseService db;
        public NewsPageViewModel(INavigationService n, StudioServicesApi a, CacheManager c, DatabaseService d) : base(n, a)
        {
            cache = c;
            db = d;
            Newsboard = new ObservableCollection<Message>();
        }

        public ObservableCollection<Message> Newsboard { get; }
        public override Task NavigatedToAsync(object parameter = null)
        {
            return Task.Factory.StartNew(async () =>
            {
                await LoadPersonAsync();
                await LoadNewsAsync();
            });
        }
        private async Task LoadPersonAsync()
        {
            if (cache.GetValue<Person>("person", null) == null)
            {
                var progress_person = SetBusy(true, "Recupero profilo in corso");
                var res = await SendRequestAsync(async () => await api.Person_GetAsync());
                SetBusy(false, "", progress_person);
                if (res.Code == ResponseCode.OK && res.Data != null)
                    cache.SetValue("person", res.Data);
                else
                    ShowMessage("Non è stato possibile recuperare le informazioni del profilo", "Informazioni profilo");
            }
        }
        private async Task LoadNewsAsync()
        {
            var progress_news = SetBusy(true, "Recupero le ultime news");
            // get ticks from first message
            var last_ticks = Newsboard.Count > 0 ? Newsboard[0].CreationTime.Ticks : 0;
            if (Newsboard.Count == 0) // init Newsboard
            {
                // get news from db
                var db_news = db.GetMessages();
                if (db_news.Count > 0 && last_ticks < db_news[0].CreationTime.Ticks)
                    last_ticks = db_news[0].CreationTime.Ticks;
                // insert news to list
                Device.BeginInvokeOnMainThread(() =>
                {
                    foreach (var news in db_news)
                        Newsboard.Insert(0, news);
                });
            }
            // get news from webservice using ticks
            var res = await SendRequestAsync(async () => await api.News_MessageListAsync(last_ticks));
            // insert news to db
            if (res.Code == ResponseCode.OK && res.Data != null)
                db.SaveItems(res.Data);
            // insert news to list
            Device.BeginInvokeOnMainThread(() =>
            {
                for (int i = 0; i < res.Data.Count; i++)
                    Newsboard.Insert(i, res.Data[i]);
            });
            SetBusy(false, "", progress_news);
        }
    }
}
