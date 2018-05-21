using Acr.UserDialogs;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace StudioServicesApp.Services
{
    public class AlertService
    {
        private bool showing_message = false;
        private Queue<Tuple<string, string, Action>> messageQueue;
        public AlertService()
        {
            messageQueue = new Queue<Tuple<string, string, Action>>();
        }

        public void ShowMessage(string text, string title, Action okAction = null)
        {
            messageQueue.Enqueue(new Tuple<string, string, Action>(text, title, okAction));
            if(!showing_message)
            {
                showing_message = true;
                Device.BeginInvokeOnMainThread(async () =>
                {
                    while (messageQueue.Count > 0)
                    {
                        var currItem = messageQueue.Dequeue();
                        var dialog = UserDialogs.Instance.Alert(new AlertConfig() { Message = currItem.Item1, Title = currItem.Item2, OnAction = currItem.Item3, OkText = "OK" });
                        
                    }
                    showing_message = false;
            });
            }
            
        }
    }
}
