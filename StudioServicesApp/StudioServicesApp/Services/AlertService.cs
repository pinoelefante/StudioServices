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
        private CancellationToken message_token;

        private IProgressDialog progress;
        private bool hide_progress = false;
        private List<string> progressList;
        public AlertService()
        {
            progressList = new List<string>();
        }
        public void SetBusy(string text, bool status)
        {
            /*
            if (showing_message && status == false)
                hide_progress = true;
            */
            Device.BeginInvokeOnMainThread(() =>
            {
                if (status)
                {
                    progressList.Add(text);
                    hide_progress = false;
                    try
                    {
                        if (progress == null)
                            progress = UserDialogs.Instance.Loading(text, null, null, !showing_message);
                        else
                        {
                            progress.Title = text;
                            if (!showing_message)
                                progress?.Show();
                        }
                    }
                    catch(Exception e)
                    {
                        Debug.WriteLine("BEH - "+e.Message);
                    }
                    Task.Factory.StartNew(async () =>
                    {
                        await Task.Delay(TimeSpan.FromSeconds(15));
                        if (progressList.Contains(text))
                            SetBusy(text, false);
                    });
                }
                else
                {
                    var index = progressList.IndexOf(text);
                    if (index >= 0)
                        progressList.RemoveAt(index);
                    if (progressList.Count > 0)
                    {
                        var title = progressList[progressList.Count - 1];
                        progress.Title = title;
                    }
                    else
                    {
                        hide_progress = true; // useless
                        progress.Hide();
                        Task.Factory.StartNew(async () =>
                        {
                            await Task.Delay(1000);
                            Device.BeginInvokeOnMainThread(() =>
                            {
                                
                                progress.Hide();
                            });
                        });
                    }
                }
            });
        }
        public void ShowMessage(string text, string title)
        {
            Device.BeginInvokeOnMainThread(async () =>
            {
                showing_message = true;
                if (progress.IsShowing)
                    progress.Hide();
                message_token = new CancellationToken();
                await UserDialogs.Instance.AlertAsync(text, title, null, message_token);
                showing_message = false;
                if (!hide_progress)
                    progress.Show();
            });
        }
    }
}
