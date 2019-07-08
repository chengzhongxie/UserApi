using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using DnsClient;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Contact.API.Dtos;
using Resilience;

namespace Contact.API.Services
{
    public class UserService : IUserService
    {
        private IHttpClient _httpCliemt;
        private string _userServiceUrl;
        private readonly ILogger<UserService> _logger;

        public UserService(IHttpClient httpCliemt, IOptions<Dtos.ServiceDiscoveryOptions> options, IDnsQuery dnsQuery, ILogger<UserService> logger)
        {
            _httpCliemt = httpCliemt;
            _logger = logger;
            var address = dnsQuery.ResolveService("service.consul", options.Value.UserServiceName);
            if (address.Count() > 0)
            {
                var addressList = address.First().AddressList;
                var host = addressList.Any() ? addressList.First().ToString() : address.First().HostName;
                var port = address.First().Port;
                _userServiceUrl = $"http://{host}:{port}";
            }
        }
        public async Task<UserIdentity> GetBaseUserInfoAsync(string userId)
        {
            //var content = new FormUrlEncodedContent(form);
            try
            {
                var response = await _httpCliemt.GetStringAsync(_userServiceUrl + "/api/user/baseinfo/" + userId);
                if (!string.IsNullOrWhiteSpace(response))
                {
                   
                    var userInfo = JsonConvert.DeserializeObject<UserIdentity>(response);
                    return userInfo;

                }
            }
            catch (Exception ex)
            {
                _logger.LogError("GetBaseUserInfoAsync 在重试后失败," + ex.Message + "," + ex.StackTrace);
                throw ex;
            }
            return null;
        }
    }
}
