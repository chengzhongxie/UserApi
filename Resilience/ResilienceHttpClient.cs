using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Polly;
using Polly.Wrap;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Linq;
using Newtonsoft.Json;

namespace Resilience
{
    public class ResilienceHttpClient : IHttpClient
    {
        private readonly HttpClient _httpClient;
        /// <summary>
        /// 根据url origin去创建 policy
        /// </summary>
        private readonly Func<string, IEnumerable<Policy>> _policyCreator;
        /// <summary>
        /// 把policyd打包组合policy wrape,进行本地缓存
        /// </summary>
        private readonly ConcurrentDictionary<string, PolicyWrap> _policyWrap;
        private ILogger<ResilienceHttpClient> _logger;
        private IHttpContextAccessor _httpContextAccessor;

        public ResilienceHttpClient(Func<string, IEnumerable<Policy>> policyCreator, ILogger<ResilienceHttpClient> logger, IHttpContextAccessor httpContextAccessor)
        {
            _httpClient = new HttpClient();
            _policyCreator = policyCreator;
            _policyWrap = new ConcurrentDictionary<string, PolicyWrap>();
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
        }
        public async Task<HttpResponseMessage> PostAsync<T>(string url, T item, string authorizationToken, string requestId = null, string authorizatinonMethod = "Bearer")
        {
            Func<HttpRequestMessage> requestMessage = () => CreateHttpRequestMessage(HttpMethod.Post, url, item);
            return await DoPostPutAsync(HttpMethod.Post, url, requestMessage, authorizationToken, requestId, authorizatinonMethod);
        }

        public async Task<HttpResponseMessage> PostAsync(string url, Dictionary<string, string> form, string authorizationToken, string requestId = null, string authorizatinonMethod = "Bearer")
        {
            Func<HttpRequestMessage> requestMessage = () => CreateHttpRequestMessage(HttpMethod.Post, url, form);
            return await DoPostPutAsync(HttpMethod.Post, url, requestMessage, authorizationToken, requestId, authorizatinonMethod);
        }

        public async Task<HttpResponseMessage> PutAsync<T>(string url, T item, string authorizationToken = null, string requestId = null, string authorizatinonMethod = "Bearer")
        {
            Func<HttpRequestMessage> requestMessage = () => CreateHttpRequestMessage(HttpMethod.Put, url, item);
            return await DoPostPutAsync(HttpMethod.Put, url, requestMessage, authorizationToken, requestId, authorizatinonMethod);
        }

        public Task<string> GetStringAsync(string url, string authorizationToken = null, string authorizatinonMethod = "Bearer")
        {
            var origin = GetOriginFromUri(url);

            return HttpInvoker(origin, async () =>
            {
                HttpRequestMessage requestMessage = new HttpRequestMessage(HttpMethod.Get, url);
                SetAuthorizationHeader(requestMessage);
                if (authorizationToken != null)
                {
                    requestMessage.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue(authorizatinonMethod, authorizationToken);
                }

                var response = await _httpClient.SendAsync(requestMessage);
                if (response.StatusCode == System.Net.HttpStatusCode.InternalServerError)
                {
                    throw new HttpRequestException();
                }
                if (!response.IsSuccessStatusCode)
                {
                    return null;
                }
                return await response.Content.ReadAsStringAsync();
            });
        }

        private Task<HttpResponseMessage> DoPostPutAsync(HttpMethod method, string url, Func<HttpRequestMessage> requestMessageFunc, string authorizationToken = null, string requestId = null, string authorizatinonMethod = "Bearer")
        {
            if (method != HttpMethod.Post && method != HttpMethod.Put)
            {
                throw new ArgumentException("method 值必须是post或者put", nameof(method));
            }
            var origin = GetOriginFromUri(url);

            return HttpInvoker(origin, async () =>
            {
                HttpRequestMessage requestMessage = requestMessageFunc();
                SetAuthorizationHeader(requestMessage);
                if (authorizationToken != null)
                {
                    requestMessage.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue(authorizatinonMethod, authorizationToken);
                }
                if (requestId != null)
                {
                    requestMessage.Headers.Add("x-requestid", requestId);
                }
                var response = await _httpClient.SendAsync(requestMessage);
                if (response.StatusCode == System.Net.HttpStatusCode.InternalServerError)
                {
                    throw new HttpRequestException();
                }
                return response;
            });
        }

        private async Task<T> HttpInvoker<T>(string origin, Func<Task<T>> action)
        {
            var normalizedOrigin = NormalizeOrigin(origin);
            if (!_policyWrap.TryGetValue(normalizedOrigin, out PolicyWrap policyWrap))
            {
                policyWrap = Policy.Wrap(_policyCreator(normalizedOrigin).ToArray());
                _policyWrap.TryAdd(normalizedOrigin, policyWrap);
            }
            return await policyWrap.Execute(() => action());
        }

        private HttpRequestMessage CreateHttpRequestMessage<T>(HttpMethod method, string url, T item)
        {
            return new HttpRequestMessage(method, url) { Content = new StringContent(JsonConvert.SerializeObject(item), Encoding.UTF8, "application/json") };
        }

        private HttpRequestMessage CreateHttpRequestMessage<T>(HttpMethod method, string url, Dictionary<string, string> form)
        {
            return new HttpRequestMessage(method, url) { Content = new FormUrlEncodedContent(form) };
        }

        private static string NormalizeOrigin(string origin)
        {
            return origin?.Trim()?.ToLower();
        }

        private static string GetOriginFromUri(string uri)
        {
            var url = new Uri(uri);
            var origin = $"{url.Scheme}://{url.DnsSafeHost}:{url.Port}";
            return origin;
        }

        private void SetAuthorizationHeader(HttpRequestMessage requestMessage)
        {
            var authorizationHeader = _httpContextAccessor.HttpContext.Request.Headers["Authorization"];
            if (!string.IsNullOrEmpty(authorizationHeader))
            {
                requestMessage.Headers.Add("Authorization", new List<string>() { authorizationHeader });
            }
        }


    }
}
