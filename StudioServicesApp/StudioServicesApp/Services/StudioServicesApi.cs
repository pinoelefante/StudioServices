using Newtonsoft.Json;
using Plugin.SecureStorage;
using SQLite;
using StudioServices.Data.Newsboard;
using StudioServices.Registry.Data;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudioServicesApp.Services
{
    public class StudioServicesApi
    {
        private const string SESSION_NAME = "SSWSESSID";
        private static string WS_ADDRESS = "http://localhost:5000";
        private WebService web;
        private ConnectionStatus connectionStatus;
        private DatabaseService database;
        public StudioServicesApi(WebService w, ConnectionStatus conn_stats, DatabaseService db)
        {
            web = w;
            connectionStatus = conn_stats;
            database = db;

            var session_id = CrossSecureStorage.Current.GetValue("session_id");
            if (!string.IsNullOrEmpty(session_id))
                web.SetCookie(SESSION_NAME, session_id, WS_ADDRESS);

            // Background requests
            LoadRequests();
            connectionStatus.Connectivity.ConnectivityChanged += Connectivity_ConnectivityChanged;
        }

        private void Connectivity_ConnectivityChanged(object sender, Plugin.Connectivity.Abstractions.ConnectivityChangedEventArgs e)
        {
            if(e.IsConnected)
                RunRequests();
        }

        public void SetNewAddress(string server_name, int port = 80, string protocol = "http") => WS_ADDRESS = $"{protocol}://{server_name}:{port}";
        public string GetServerAddress() => WS_ADDRESS;

        #region Authentication
        public async Task<ResponseMessage<bool>> Authentication_LoginAsync(string username, string password)
        {
            var address = $"{WS_ADDRESS}/api/authentication/login";
            var parameters = new ParametersList("username", username, "password", password);
            var message = await SendRequestAsync<bool>(address, HttpMethod.POST, parameters);
            if (message.Code == ResponseCode.OK && message.Data)
            {
                CrossSecureStorage.Current.SetValue("username", username);
                CrossSecureStorage.Current.SetValue("password", password);
                var session_id = web.GetCookie(SESSION_NAME, WS_ADDRESS);
                CrossSecureStorage.Current.SetValue("session_id", session_id);
            }
            return message;
        }
        public async Task<ResponseMessage<bool>> Authentication_RegisterAsync(string username, string password, string email, string fiscal_code, string verify_code)
        {
            var address = $"{WS_ADDRESS}/api/authentication/register";
            var parameters = new ParametersList(
                "username", username,
                "password", password,
                "email", email,
                "fiscal_code", fiscal_code,
                "verify_code", verify_code
            );
            return await SendRequestAsync<bool>(address, HttpMethod.POST, parameters);
        }
        public async Task<ResponseMessage<bool>> Authentication_IsLoggedAsync()
        {
            if(!web.HasCookie(SESSION_NAME, WS_ADDRESS))
                return new ResponseMessage<bool>() { Code = ResponseCode.OK, Data = false };
            var address = $"{WS_ADDRESS}/api/authentication/is_logged";
            return await SendRequestAsync<bool>($"{WS_ADDRESS}/api/authentication/is_logged", HttpMethod.GET);
        }
        public async Task<ResponseMessage<bool>> Authentication_LogoutAsync()
        {
            var address = $"{WS_ADDRESS}/api/authentication/logout";
            return await SendRequestAsync<bool>(address, HttpMethod.POST, null);
        }
        #endregion

        #region Person
        public async Task<ResponseMessage<bool>> Person_CreateAsync(string name, string surname, string fiscal_code, DateTime birthday, string b_place)
        {
            return await Person_CreateAsync(name, surname, fiscal_code, birthday.Year, birthday.Month, birthday.Day, b_place);
        }
        public async Task<ResponseMessage<bool>> Person_CreateAsync(string name, string surname, string fiscal_code, int b_year, int b_month, int b_day, string b_place)
        {
            var address = $"{WS_ADDRESS}/api/person/create";
            var parameters = new ParametersList("name", name, "surname", surname, "fiscal_code", fiscal_code, "b_year", b_year.ToString(), "b_month", b_month.ToString(), "b_day", b_day.ToString(), "b_place", b_place);
            return await SendRequestAsync<bool>(address, HttpMethod.POST, parameters);
        }
        public async Task<ResponseMessage<Person>> Person_GetAsync()
        {
            var address = $"{WS_ADDRESS}/api/person/get";
            return await SendRequestAsync<Person>(address, HttpMethod.GET);
        }
        public async Task<ResponseMessage<Person>> Person_SetStatusAsync(int person_id, bool status)
        {
            var address = $"{WS_ADDRESS}/api/person/status";
            var prms = new ParametersList("person_id", person_id.ToString(), "status", status.ToString());
            return await SendRequestAsync<Person>(address, HttpMethod.GET, prms);
        }
        public async Task<ResponseMessage<bool>> Person_AddDocumentAsync(int type, string number, DateTime issue, DateTime expire, byte[] file, string file_ext)
        {
            var address = $"{WS_ADDRESS}/api/person/document";
            var prmts = new ParametersList("type", type.ToString(), "number", number, "i_ticks", issue.Ticks.ToString(), "e_ticks", expire.Ticks.ToString(), "file_ext", file_ext);
            return await SendRequestAsync<bool>(address, HttpMethod.POST, prmts, file);
        }
        public async Task<ResponseMessage<bool>> Person_DeleteDocumentAsync(int id)
        {
            var address = $"{WS_ADDRESS}/api/person/document";
            var prmts = new ParametersList("id", id.ToString());
            return await SendRequestAsync<bool>(address, HttpMethod.DELETE, prmts);
        }
        public async Task<ResponseMessage<bool>> Person_AddContactAsync(int type, string number, bool whatsapp, bool telegram, int priorita)
        {
            var address = $"{WS_ADDRESS}/api/person/contact";
            var parameters = new ParametersList("type", type, "number", number, "whatsapp", whatsapp, "telegram", telegram, "priority", priorita);
            return await SendRequestAsync<bool>(address, HttpMethod.POST, parameters);
        }
        public async Task<ResponseMessage<bool>> Person_DeleteContactAsync(int id)
        {
            var address = $"{WS_ADDRESS}/api/person/contact";
            var prmts = new ParametersList("id", id.ToString());
            return await SendRequestAsync<bool>(address, HttpMethod.DELETE, prmts);
        }
        public async Task<ResponseMessage<bool>> Person_AddAddressAsync(int type, string country, string city, string province, string address, string number, string description)
        {
            var url_address = $"{WS_ADDRESS}/api/person/address";
            var parameters = new ParametersList("type", type, "country", country, "city", city, "province", province, "address", address, "number", number, "description", description);
            return await SendRequestAsync<bool>(url_address, HttpMethod.POST, parameters);
        }
        public async Task<ResponseMessage<bool>> Person_DeleteAddressAsync(int id)
        {
            var address = $"{WS_ADDRESS}/api/person/address";
            var prmts = new ParametersList("id", id.ToString());
            return await SendRequestAsync<bool>(address, HttpMethod.DELETE, prmts);
        }
        public async Task<ResponseMessage<bool>> AddEmailAsync(string address, bool pec, bool managed, string password, string fullname, string imap_address, int imap_port, string imap_username, string smtp_address, int smtp_port, string smtp_username, string service_username, string service_password, DateTime expire_day, bool renew_auto, string renew_paypal)
        {
            var url_address = $"{WS_ADDRESS}/api/person/email";
            var parameters = new ParametersList("address", address, "pec", pec, "managed", managed, "password", password, "fullname", fullname, "imap_address", imap_address, "imap_port", imap_port, "imap_username", imap_username, "smtp_address", smtp_address, "smtp_port", smtp_port, "smtp_username", smtp_username, "service_username", service_username, "service_password", service_password, "expire_day", expire_day.Ticks, "renew_auto", renew_auto, "renew_paypal", renew_paypal);
            return await SendRequestAsync<bool>(url_address, HttpMethod.POST, parameters);
        }
        public async Task<ResponseMessage<bool>> Person_DeleteEmailAsync(int id)
        {
            var address = $"{WS_ADDRESS}/api/person/email";
            var prmts = new ParametersList("id", id.ToString());
            return await SendRequestAsync<bool>(address, HttpMethod.DELETE, prmts);
        }
        #endregion

        #region News
        public async Task<ResponseMessage<List<Message>>> News_PublicMessageListAsync(long ticks = 0)
        {
            var address = $"{WS_ADDRESS}/api/news/all";
            var parameters = new ParametersList("time", ticks);
            var news = await SendRequestAsync<List<Message>>(address, HttpMethod.GET, parameters);
            return news;
        }
        public async Task<ResponseMessage<bool>> News_SetReadAsync(int message_id)
        {
            var address = $"{WS_ADDRESS}/api/news/{message_id}";
            var parameters = new ParametersList("mode", (int)ReadMode.APP);
            return await SendRequestAsync<bool>(address, HttpMethod.POST, parameters);
        }
        #endregion

        #region Admin
        public async Task<ResponseMessage<bool>> Admin_IsAdminAsync()
        {
            var address = $"{WS_ADDRESS}/api/admin/is_admin";
            return await SendRequestAsync<bool>(address, HttpMethod.GET);
        }
        public async Task<ResponseMessage<bool>> Admin_ActAsAsync(int person_id)
        {
            var address = $"{WS_ADDRESS}/api/admin/act_as/{person_id}";
            return await SendRequestAsync<bool>(address, HttpMethod.GET);
        }
        #endregion

        #region Warehouse
        public async Task<ResponseMessage<bool>> CreateCompanyAsync(string name, string vat, int address_id)
        {
            var address = $"{WS_ADDRESS}/api/company";
            var parameters = new ParametersList("name",name,"vat",vat,"address_id",address_id);
            return await SendRequestAsync<bool>(address, HttpMethod.POST, parameters, null, true);
        }
        #endregion

        private async Task<ResponseMessage<T>> SendRequestAsync<T>(string url, HttpMethod method, IEnumerable<KeyValuePair<string, string>> parameters = null, byte[] file = null, bool send_later = false)
        {
            if(!connectionStatus.IsConnected())
            {
                if (send_later)
                    AddRequest(url, method, parameters as ParametersList, file, send_later);
                return new ResponseMessage<T>() { Code = ResponseCode.INTERNET_NOT_AVAILABLE };
            }
            var res = await web.SendRequestAsync(url, method, parameters, file);
            Debug.WriteLine("URL: " + url);
            if (string.IsNullOrEmpty(res))
            {
                if(send_later)
                    AddRequest(url, method, parameters as ParametersList, file, send_later);
                return new ResponseMessage<T>() { Code = ResponseCode.COMMUNICATION_ERROR };
            }
                
            try
            {
                Debug.WriteLine("JSON: " + res);
                return JsonConvert.DeserializeObject<ResponseMessage<T>>(res);
            }
            catch(Exception e)
            {
                Debug.WriteLine(e.Message);
                return new ResponseMessage<T>() { Code = ResponseCode.SERIALIZER_ERROR };
            }
        }
        private void AddRequest(string url, HttpMethod method, ParametersList parameters, byte[] file, bool later)
        {
            RequestItem item = new RequestItem()
            {
                Url = url,
                Method = method,
                Parameters = parameters,
                File = file,
                SendLater = later
            };
            database.SaveItem(item);
        }
        private void LoadRequests()
        {
            var reqs = database.GetRequestItems();
            while(reqs.Any())
            {
                runLaterQueue.Enqueue(reqs.Last());
                reqs.RemoveAt(reqs.Count - 1);
            }
            RunRequests();
        }
        private void DeleteRequest(RequestItem item)
        {
            database.Delete(item);
        }
        private Queue<RequestItem> runLaterQueue = new Queue<RequestItem>(20);
        private Task taskLater;
        private async void RunRequests()
        {
            if(runLaterQueue.Any())
            {
                if (taskLater != null)
                    await taskLater;
                taskLater = Task.Run(async () =>
                {
                    while(runLaterQueue.Any())
                    {
                        var element = runLaterQueue.First();
                        var resp = await SendRequestAsync<object>(element.Url, element.Method, element.Parameters, element.File);
                        if(resp.IsOK)
                        {
                            runLaterQueue.Dequeue();
                            DeleteRequest(element);
                        }
                    }
                });
            }
        }
    }
    public class RequestItem
    {
        [PrimaryKey]
        public int Id { get; set; }
        public HttpMethod Method { get; set; }
        public string Url { get; set; }
        [Ignore]
        public IEnumerable<KeyValuePair<string, string>> Parameters { get; set; }
        public byte[] File { get; set; }
        public bool SendLater { get; set; }

        public string ParametersString
        {
            get
            {
                if (Parameters == null)
                    return string.Empty;
                var content = string.Empty;
                foreach(var item in Parameters)
                    content += $"{item.Key} {item.Value}";
                return content;
            }
            set
            {
                if (string.IsNullOrEmpty(value))
                    return;
                var items = value.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                if (items.Length % 2 != 0)
                    throw new Exception("must be divisible by 2");
                Parameters = new ParametersList(items);
            }
        }
    }
    public class ResponseMessage<T>
    {
        public long Ticks { get; set; }
        public ResponseCode Code { get; set; }
        public string Message { get; set; }
        public T Data { get; set; }

        public bool IsOK { get { return Code == ResponseCode.OK; } }
    }
    public enum ResponseCode
    {
        COMMUNICATION_ERROR = -1, SERIALIZER_ERROR = -2, INTERNET_NOT_AVAILABLE = -3,
        OK = 0,
        REQUIRE_LOGIN = 1,
        FAIL = 2,
        ADMIN_FUNCTION = 3,
        MAINTENANCE = 4
    }
    public class ParametersList : List<KeyValuePair<string, string>>
    {
        public ParametersList(params object[] list) : this(true, list) { }
        public ParametersList(bool skipNull, params object[] list) : base()
        {
            if (list.Length % 2 != 0)
                throw new Exception("List must be even");
            for (int i = 0; i < list.Length; i = i + 2)
            {
                if (list[i + 1] == null && skipNull)
                    continue;
                Add(list[i].ToString(), list[i + 1] != null ? list[i + 1].ToString() : "null");
            }
        }
        public void Add(string key, string value)
        {
            this.Add(new KeyValuePair<string, string>(key, value));
        }
    }
        
}
