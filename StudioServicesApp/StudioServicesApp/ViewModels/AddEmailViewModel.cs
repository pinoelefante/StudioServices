using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using GalaSoft.MvvmLight.Views;
using StudioServices.Data.Sqlite.Registry;
using StudioServicesApp.Services;
using GalaSoft.MvvmLight.Command;

namespace StudioServicesApp.ViewModels
{
    public class AddEmailViewModel : MyAuthViewModel
    {
        private AssemblyFileReader assembly_files;
        public Email Email { get; set; }

        public AddEmailViewModel(INavigationService n, StudioServicesApi a, AlertService al, KeyValueService k, AssemblyFileReader ar) : base(n, a, al, k) {
            assembly_files = ar;
            Email = new Email();
        }
        public override async Task NavigatedToAsync(object parameter = null)
        {
            await base.NavigatedToAsync(parameter);
            if (parameter != null && parameter is Email)
                Email.InitFrom(parameter as Email);
            Email.PropertyChanged += Email_PropertyChanged;
        }
        public override void NavigatedFrom()
        {
            Email.PropertyChanged -= Email_PropertyChanged;
        }

        private void Email_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch(e.PropertyName)
            {
                case nameof(Email.Address):
                    InitFromAddress();
                    break;
                case nameof(Email.IsManaged):
                    if (Email.IsManaged)
                        InitFromManaged();
                    else
                        RemoveManaged();
                    break;
            }
        }

        private void RemoveManaged()
        {
            Email.FullName = null;
            Email.Password = null;
            Email.IMAPAddress = null;
            Email.IMAPPort = 0;
            Email.IMAPUsername = null;
            Email.SMTPAddress = null;
            Email.SMTPPort = 0;
            Email.SMTPUsername = null;
            Email.ServiceUsername = null;
            Email.ServicePassword = null;
            Email.AutoRenewEnabled = false;
        }
        private ServerEmailConfig serverConfig;
        public ServerEmailConfig ServerConfig { get => serverConfig; set => SetMT(ref serverConfig, value); }
        private void InitFromAddress()
        {
            if (!Email.IsManaged)
                return;
            if (string.IsNullOrEmpty(Email.Address) || !Email.Address.Contains("@"))
                return;
            var fields = Email.Address.Split(new char[] { '@' }, StringSplitOptions.None);
            var domain = fields[1];
            if (domain.Length <= 0)
                return;
            var server = GetConfigFromJson(domain);
            if (server == null)
                return;
            ServerConfig = server;
            Email.IMAPAddress = server.IMAPServer;
            Email.IMAPPort = server.IMAPPort;
            Email.IMAPUsername = server.IMAPUsername ? Email.Address : "";
            Email.SMTPAddress = server.SMTPServer;
            Email.SMTPPort = server.SMTPPort;
            Email.SMTPUsername = server.SMTPUsername ? Email.Address : "";
            if(server.HasServiceConfig)
            {
                Email.ServiceUsername = server.ServiceUsername;
            }
        }
        private void InitFromManaged()
        {
            Email.FullName = $"{Persona.Name} {Persona.Surname}";
            InitFromAddress();
        }
        private List<JsonServerEmailConfig> EmailConfigs;
        private ServerEmailConfig GetConfigFromJson(string domain)
        {
            if(EmailConfigs == null || EmailConfigs.Count == 0)
                EmailConfigs = assembly_files.ReadLocalJson<List<JsonServerEmailConfig>>("PecConfigs.json");
            foreach(var conf in EmailConfigs)
            {
                if (conf.domains.Contains(domain))
                    return conf.server;
            }
            return null;
        }
        private RelayCommand addEmailCmd;
        public RelayCommand AddEmailCommand =>
            addEmailCmd ??
            (addEmailCmd = new RelayCommand(async () =>
            {
                Email.PersonId = Persona.Id;
                if(Email.IsValid())
                {
                    var res = await SendRequestAsync(async () => await api.Person_AddEmailAsync(Email));
                    if(res.IsOK && res.Data)
                    {
                        MessengerInstance.Send(true, "AddEmailStatus");
                        await Navigation.PopPopupAsync();
                    }
                    else
                    {
                        ShowMessage($"Si è verificato un errore: {res.Code}", "Aggiungi email");
                    }
                }
                else
                {
                    ShowMessage("Controllare i dati inseriti");
                }
            }));
    }
    class JsonServerEmailConfig
    {
        public string name { get; set; }
        public List<String> domains { get; set; }
        public ServerEmailConfig server { get; set; }
    }
    public class ServerEmailConfig
    {
        public string IMAPServer { get; set; }
        public int IMAPPort { get; set; }
        public bool IMAPUsername { get; set; }
        public bool IMAP_SSL { get; set; }
        public string SMTPServer { get; set; }
        public int SMTPPort { get; set; }
        public bool SMTPUsername { get; set; }
        public bool SMTP_SSL { get; set; }
        public bool HasServiceConfig { get; set; }
        public string ServiceUsername { get; set; }
    }
}
