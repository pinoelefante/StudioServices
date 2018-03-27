﻿using GalaSoft.MvvmLight.Command;
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
        private DatabaseService db;
        public NewsPageViewModel(INavigationService n, StudioServicesApi a, DatabaseService d) : base(n, a)
        {
            db = d;
            Newsboard = new ObservableCollection<Message>();

            MessengerInstance.Register<int>(this, "PublicNewsPrev", (curr_message) =>
            {
                var last_index = markAsRead.Count > 0 ? markAsRead[markAsRead.Count - 1] : 0;
                for (int i = last_index;i<Newsboard.Count;i++)
                {
                    var message = Newsboard[i];
                    if(message.Id == curr_message)
                    {
                        var prev_message = i < Newsboard.Count - 1 ? Newsboard[i + 1] : null;
                        MessengerInstance.Send<Message>(prev_message, "PublicNewsRead");
                        if(message != null)
                        {
                            lastSelectedNews = i + 1;
                            if (!message.IsRead)
                                markAsRead.Add(i + 1);
                        }
                        break;
                    }
                }
            });
            MessengerInstance.Register<int>(this, "PublicNewsNext", (curr_message) =>
            {
                var last_index = markAsRead.Count > 0 ? markAsRead[markAsRead.Count - 1] : 0;
                for (int i = last_index; i < Newsboard.Count; i++)
                {
                    var message = Newsboard[i];
                    if (message.Id == curr_message)
                    {
                        var next_msg = i > 0 ? Newsboard[i - 1] : null;
                        MessengerInstance.Send<Message>((next_msg), "PublicNewsRead");
                        if (message != null)
                        {
                            lastSelectedNews = i - 1;
                            if (!message.IsRead)
                                markAsRead.Add(i - 1);
                        }
                        break;
                    }
                }
            });
        }
        private int lastSelectedNews = 0;
        private List<int> markAsRead = new List<int>(); 
        public ObservableCollection<Message> Newsboard { get; }
        public override Task NavigatedToAsync(object parameter = null)
        {
            return Task.Factory.StartNew(async () =>
            {
                SetRead();
                await LoadPersonAsync();
                await LoadNewsAsync();
                await base.NavigatedToAsync(null); // check admin status
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
            if(cache.GetValue<bool?>("is_admin", null) == null)
            {
                var res = await SendRequestAsync(async () => await api.Admin_IsAdminAsync());
                if (res.Code == ResponseCode.OK)
                    cache.SetValue("is_admin", res.Data);
            }
        }
        private async Task LoadNewsAsync(bool force = false)
        {
            var last_update = cache.GetValue<long>("last_update_news", 0);
            var now_time = DateTime.Now;
            DateTime last_time = new DateTime(last_update).Add(TimeSpan.FromMinutes(15));
            bool canUpdate = now_time.Ticks - last_time.Ticks > 0;

            if (!canUpdate & !force)
                return;

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
                        Newsboard.Add(news);
                });
            }
            // get news from webservice using ticks
            var res = await SendRequestAsync(async () => await api.News_PublicMessageListAsync(last_ticks));
            // insert news to db
            if (res.Code == ResponseCode.OK && res.Data != null)
            {
                db.SaveItems(res.Data);
                cache.SetValue("last_update_news", now_time.Ticks);
            }
            // insert news to list
            Device.BeginInvokeOnMainThread(() =>
            {
                for (int i = 0; i < res.Data.Count; i++)
                    Newsboard.Insert(i, res.Data[i]);
            });
            SetBusy(false, "", progress_news);
        }
        private void SetRead()
        {
            foreach (var index in markAsRead)
            {
                var msg = Newsboard[index];
                Device.BeginInvokeOnMainThread(() =>
                {
                    Newsboard.RemoveAt(index);
                    Newsboard.Insert(index, msg);
                });
            }
            markAsRead.Clear();
        }

        private RelayCommand _newsUpdate, _delDb, _newPostCmd;
        private RelayCommand<Message> _openMessageCmd;

        public RelayCommand UpdateNews =>
            _newsUpdate ?? (_newsUpdate = new RelayCommand(async () =>
            {
                await LoadNewsAsync(true);
            }));
        public RelayCommand<Message> OpenMessageCommand =>
            _openMessageCmd ?? (_openMessageCmd = new RelayCommand<Message>((message) =>
            {
                var index = Newsboard.IndexOf(message);
                if (index >= 0)
                {
                    lastSelectedNews = index;
                    if (!message.IsRead)
                        markAsRead.Add(index);
                }
                navigation.NavigateTo(ViewModelLocator.VIEW_MESSAGE_PAGE, message);
            }));
        public RelayCommand DeleteDatabaseCommand =>
            _delDb ?? (_delDb = new RelayCommand(() =>
            {
                db.DeleteDatabase();
                Newsboard.Clear();
                cache.SetValue("last_update_news", 0L);
            }));
        public RelayCommand CreatePublicNewsCommand =>
            _newPostCmd ?? (_newPostCmd = new RelayCommand(() =>
            {
                
            }));
    }
}
