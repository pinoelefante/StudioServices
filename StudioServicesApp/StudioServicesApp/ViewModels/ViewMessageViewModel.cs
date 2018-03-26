using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Views;
using pinoelefante.ViewModels;
using StudioServices.Data.Newsboard;
using StudioServicesApp.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace StudioServicesApp.ViewModels
{
    public class ViewMessageViewModel : MyViewModel
    {
        private DatabaseService db;
        public ViewMessageViewModel(INavigationService n, StudioServicesApi a, DatabaseService d) : base(n, a)
        {
            db = d;
            MessengerInstance.Register<Message>(this, "PublicNewsRead", async (message) =>
            {
                if(message == null)
                {
                    ShowToast("Non ci sono ulteriori messaggi");
                    return;
                }
                await NavigatedToAsync(message);
            });
        }
        private RelayCommand _prevNewsCmd, _nextNewsCmd;
        public Message Message { get; set; }
        public override async Task NavigatedToAsync(object parameter = null)
        {
            Message = parameter as Message;
            Device.BeginInvokeOnMainThread(() =>
            {
                RaisePropertyChanged(() => Message);
            });
            await ReadMessageAsync();
        }
        private async Task ReadMessageAsync()
        {
            // Don't check IsRead because i can update ReadStatus with App Mode
            if (cache.GetValue<bool>($"message_{Message.Id}", false))
                return;
            var response = await SendRequestAsync(async () =>await api.News_SetReadAsync(Message.Id));
            if (response.Code == ResponseCode.OK && response.Data)
            {
                cache.SetValue($"message_{Message.Id}", true);
                Message.IsRead = true;
                db.SaveItem(Message);
                Debug.WriteLine("Message read setted");
            }
            else
            {
                Debug.WriteLine("Message read not setted");
                Debug.WriteLine("CODE: "+response.Code);
            }
        }
        public RelayCommand NextNewsCommand =>
            _nextNewsCmd ?? (_nextNewsCmd = new RelayCommand(() =>
            {
                MessengerInstance.Send(Message.Id, "PublicNewsNext");
            }));
        public RelayCommand PrevNewsCommand =>
            _prevNewsCmd ?? (_prevNewsCmd = new RelayCommand(() =>
            {
                MessengerInstance.Send(Message.Id, "PublicNewsPrev");
            }));
    }
}
