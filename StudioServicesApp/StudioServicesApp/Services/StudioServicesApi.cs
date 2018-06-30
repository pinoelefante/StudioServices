using Newtonsoft.Json;
using SQLite;
using StudioServices.Data.Sqlite.Accounting;
using StudioServices.Data.Sqlite.Newsboard;
using StudioServices.Data.Sqlite.Registry;
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
        private KeyValueService kvSettings;
        public StudioServicesApi(WebService w, ConnectionStatus conn_stats, DatabaseService db, KeyValueService kv)
        {
            web = w;
            connectionStatus = conn_stats;
            database = db;
            kvSettings = kv;

            WS_ADDRESS = kvSettings.Get("server_address", WS_ADDRESS);

            var session_id = kvSettings.Get("session_id");
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

        public void SetNewAddress(string server_name, int port = 80, string protocol = "http")
        {
            WS_ADDRESS = $"{protocol}://{server_name}:{port}";
            kvSettings.Add("server_address", WS_ADDRESS);
        }
        public string GetServerAddress() => WS_ADDRESS;

        #region Test

        #endregion

        #region Authentication
        public async Task<ResponseMessage<bool>> Authentication_LoginAsync(string username, string password)
        {
            var address = $"{WS_ADDRESS}/api/authentication/login";
            var parameters = new ParametersList("username", username, "password", password);
            var message = await SendRequestAsync<bool>(address, HttpMethod.POST, parameters);
            if (message.Code == ResponseCode.OK && message.Data)
            {
                kvSettings.Add("username", username);
                kvSettings.Add("password", password);
                var session_id = web.GetCookie(SESSION_NAME, WS_ADDRESS);
                kvSettings.Add("session_id", session_id);
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
        public async Task<ResponseMessage<bool>> Person_CreateAsync(Person person)
        {
            var address = $"{WS_ADDRESS}/api/person/create";
            return await SendRequestAsync<bool>(address, HttpMethod.POST, person);
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
        public async Task<ResponseMessage<bool>> Person_AddDocumentAsync(IdentificationDocument document)
        {
            var address = $"{WS_ADDRESS}/api/person/document";
            return await SendRequestAsync<bool>(address, HttpMethod.POST, document);
        }
        public async Task<ResponseMessage<bool>> Person_DeleteDocumentAsync(int id)
        {
            var address = $"{WS_ADDRESS}/api/person/document";
            var prmts = new ParametersList("id", id.ToString());
            return await SendRequestAsync<bool>(address, HttpMethod.DELETE, prmts);
        }
        public async Task<ResponseMessage<bool>> Person_AddContactAsync(ContactMethod contact)
        {
            var address = $"{WS_ADDRESS}/api/person/contact";
            return await SendRequestAsync<bool>(address, HttpMethod.POST, contact);
        }
        public async Task<ResponseMessage<bool>> Person_DeleteContactAsync(int id)
        {
            var address = $"{WS_ADDRESS}/api/person/contact";
            var prmts = new ParametersList("id", id.ToString());
            return await SendRequestAsync<bool>(address, HttpMethod.DELETE, prmts);
        }
        public async Task<ResponseMessage<bool>> Person_AddAddressAsync(Address address)
        {
            var url_address = $"{WS_ADDRESS}/api/person/address";
            return await SendRequestAsync<bool>(url_address, HttpMethod.POST, address);
        }
        public async Task<ResponseMessage<bool>> Person_DeleteAddressAsync(int id)
        {
            var address = $"{WS_ADDRESS}/api/person/address";
            var prmts = new ParametersList("id", id.ToString());
            return await SendRequestAsync<bool>(address, HttpMethod.DELETE, prmts);
        }
        public async Task<ResponseMessage<bool>> Person_AddEmailAsync(Email email)
        {
            var url_address = $"{WS_ADDRESS}/api/person/email";
            return await SendRequestAsync<bool>(url_address, HttpMethod.POST, email);
        }
        public async Task<ResponseMessage<bool>> Person_DeleteEmailAsync(int id)
        {
            var address = $"{WS_ADDRESS}/api/person/email";
            var prmts = new ParametersList("id", id.ToString());
            return await SendRequestAsync<bool>(address, HttpMethod.DELETE, prmts);
        }
        #endregion

        #region News
        public async Task<ResponseMessage<List<Message>>> News_GetAllPublicMessagesAsync(long ticks = 0)
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
        public async Task<ResponseMessage<Company>> Warehouse_SaveCompanyAsync(Company company)
        {
            var address = $"{WS_ADDRESS}/api/warehouse/company";
            return await SendRequestAsync<Company>(address, HttpMethod.POST, company);
        }
        public async Task<ResponseMessage<List<Company>>> Warehouse_GetMyCompaniesAsync()
        {
            var address = $"{WS_ADDRESS}/api/warehouse/company";
            return await SendRequestAsync<List<Company>>(address, HttpMethod.GET);
        }
        public async Task<ResponseMessage<string>> Warehouse_GenerateProductCode(string productName, int companyId)
        {
            var address = $"{WS_ADDRESS}/api/warehouse/productcode_new";
            var parameters = new ParametersList("productName", productName, "company", companyId);
            return await SendRequestAsync<string>(address, HttpMethod.GET, parameters);
        }
        public async Task<ResponseMessage<List<CompanyProduct>>> Warehouse_GetCompanyProducts(int companyId)
        {
            var address = $"{WS_ADDRESS}/api/warehouse/products/{companyId}";
            return await SendRequestAsync<List<CompanyProduct>>(address, HttpMethod.GET);
        }
        public async Task<ResponseMessage<bool>> Warehouse_SaveProduct(CompanyProduct product)
        {
            var address = $"{WS_ADDRESS}/api/warehouse/product";
            return await SendRequestAsync<bool>(address, HttpMethod.POST, product);
        }
        public async Task<ResponseMessage<List<Company>>> Warehouse_ClientsSuppliersList()
        {
            var address = $"{WS_ADDRESS}/api/warehouse/clients_suppliers";
            return await SendRequestAsync<List<Company>>(address, HttpMethod.GET);
        }
        #endregion

        private async Task<ResponseMessage<T>> SendRequestAsync<T>(string url, HttpMethod method, object parameters = null, byte[] file = null, bool send_later = false)
        {
            if(!connectionStatus.IsConnected(url))
            {
                if (send_later)
                    AddRequest(url, method, parameters, file, send_later);
                return new ResponseMessage<T>() { Code = ResponseCode.INTERNET_NOT_AVAILABLE };
            }
            var res = await web.SendRequestAsync(url, method, parameters, file);
            Debug.WriteLine("URL: " + url);
            if (string.IsNullOrEmpty(res))
            {
                if(send_later)
                    AddRequest(url, method, parameters, file, send_later);
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
        private void AddRequest(string url, HttpMethod method, object parameters, byte[] file, bool later)
        {
            RequestItem item = new RequestItem()
            {
                Url = url,
                Method = method,
                ParameterObject = parameters,
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
                        var resp = await SendRequestAsync<object>(element.Url, element.Method, element.ParameterObject, element.File);
                        if(resp.IsOK)
                        {
                            runLaterQueue.Dequeue();
                            DeleteRequest(element);
                        }
                        await Task.Delay(1000);
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
        public byte[] File { get; set; }
        public bool SendLater { get; set; }

        [Ignore]
        public object ParameterObject { get; set; }

        public string ParametersString
        {
            get
            {
                return JsonConvert.SerializeObject(ParameterObject);
            }
            set
            {
                if (string.IsNullOrEmpty(value))
                    ParameterObject = null;
                else
                {
                    ParameterObject = JsonConvert.DeserializeObject(value);
                }
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
