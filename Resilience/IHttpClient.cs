using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Resilience
{
    public interface IHttpClient
    {
        Task<HttpResponseMessage> PostAsync<T>(string url, T item, string authorizationToken = null, string requestId = null, string authorizatinonMethod = "Bearer");

        Task<HttpResponseMessage> PutAsync<T>(string url, T item, string authorizationToken = null, string requestId = null, string authorizatinonMethod = "Bearer");

        Task<string> GetStringAsync(string url, string authorizationToken = null, string authorizatinonMethod = "Bearer");

        Task<HttpResponseMessage> PostAsync(string url, Dictionary<string, string> form, string authorizationToken = null, string requestId = null, string authorizatinonMethod = "Bearer");
    }
}
