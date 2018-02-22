﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace StudioServicesApp.Services
{
    public class WebService
    {
        private HttpClient httpClient;
        private HttpClientHandler handler;
        private CookieContainer cookieContainer;

        public WebService()
        {
            cookieContainer = new CookieContainer();
            handler = new HttpClientHandler() { CookieContainer = cookieContainer, UseCookies = true, AllowAutoRedirect = false };
            httpClient = new HttpClient(handler);
            httpClient.DefaultRequestHeaders.Add("User-Agent", "StudioServicesApp/1.0");
            httpClient.Timeout = TimeSpan.FromSeconds(10);
        }
        public async Task<string> SendGetRequestAsync(string url, params KeyValuePair<string, string>[] parameters)
        {
            try
            {
                if (parameters?.Length > 0)
                {
                    FormUrlEncodedContent content = new FormUrlEncodedContent(parameters);
                    string urlContent = await content.ReadAsStringAsync();
                    url = $"{url}?{urlContent}";
                }
                CancellationToken token = new CancellationToken();
                var res = await httpClient.GetAsync(url, HttpCompletionOption.ResponseContentRead, token);
                if (!token.IsCancellationRequested && res.IsSuccessStatusCode)
                    return await res.Content.ReadAsStringAsync();
                Debug.WriteLine("WEB - SUCCESS STATUS FAIL");
                return null;
            }
            catch (Exception e)
            {
                Debug.WriteLine("WEB - EXCEPTION : " + e.Message);
                return null;
            }
        }
        public async Task<string> SendPostRequestAsync(string url, params KeyValuePair<string, string>[] parameters)
        {
            try
            {
                FormUrlEncodedContent content = new FormUrlEncodedContent(parameters);
                CancellationToken token = new CancellationToken();
                var res = await httpClient.PostAsync(url, content, token);
                if (!token.IsCancellationRequested && res.IsSuccessStatusCode)
                    return await res.Content.ReadAsStringAsync();
                return null;
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
                return null;
            }
        }
        public async Task<string> SendMultipartPostRequestAsync(string url, byte[] file_content, params KeyValuePair<string, string>[] parameters)
        {
            try
            {
                MultipartFormDataContent content = new MultipartFormDataContent();
                if (file_content != null && file_content.Length > 0)
                    content.Add(new ByteArrayContent(file_content), "file");
                foreach (var kv in parameters)
                    content.Add(new StringContent(kv.Value), kv.Key);

                CancellationToken token = new CancellationToken();
                var res = await httpClient.PostAsync(url, content, token);
                if (!token.IsCancellationRequested && res.IsSuccessStatusCode)
                    return await res.Content.ReadAsStringAsync();
                return null;
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
                return null;
            }
        }
        public async Task<string> SendDeleteRequestAsync(string url, params KeyValuePair<string, string>[] parameters)
        {
            try
            {
                if (parameters.Length > 0)
                {
                    FormUrlEncodedContent content = new FormUrlEncodedContent(parameters);
                    var urlContent = await content.ReadAsStringAsync();
                    url = $"{url}?{urlContent}";
                }
                var token = new CancellationToken();
                var res = await httpClient.DeleteAsync(url, token);
                if (!token.IsCancellationRequested && res.IsSuccessStatusCode)
                    return await res.Content.ReadAsStringAsync();
                return null;
            }
            catch (Exception e)
            {
                return null;
            }
        }
        public async Task<string> SendRequestAsync(string url, HttpMethod method, IEnumerable<KeyValuePair<string, string>> parameters = null, byte[] file = null)
        {
            KeyValuePair<string, string>[] parameters_enum = parameters !=null ? Enumerable.ToArray(parameters) : null;
            string response = String.Empty;
            switch (method)
            {
                case HttpMethod.GET:
                    response = await SendGetRequestAsync(url, parameters_enum);
                    break;
                case HttpMethod.POST:
                    if (file == null)
                        response = await SendPostRequestAsync(url, parameters_enum);
                    else
                        response = await SendMultipartPostRequestAsync(url, file, parameters_enum);
                    break;
                case HttpMethod.DELETE:
                    response = await SendDeleteRequestAsync(url, parameters_enum);
                    break;
                case HttpMethod.PUT:
                    // TODO
                    break;
            }
            if (string.IsNullOrEmpty(response))
                return null;
            return response;
        }
        public string GetCookie(string name, string url)
        {
            var cookies = cookieContainer.GetCookies(new Uri(url));
            for(int i = 0; i < cookies.Count; i++)
            {
                if (cookies[i].Name.CompareTo(name) == 0)
                    return cookies[i].Value;
            }
            return null;
        }
        public void SetCookie(string name, string value, string url)
        {
            var uri = new Uri(url);
            Cookie cookie = new Cookie(name, value, "/", uri.Host);
            cookieContainer.Add(cookie);
        }
    }
    public enum HttpMethod
    {
        GET = 0,
        POST = 1,
        DELETE = 2,
        PUT = 3
    }
}