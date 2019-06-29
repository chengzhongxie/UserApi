using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using DnsClient;
using Microsoft.Extensions.Options;
using Resilience;
using Microsoft.Extensions.Logging;

namespace User.Identity.Services
{
    public class UserService : IUserService
    {
        private IHttpClient _httpCliemt;
        private string _userServiceUrl="http://localhost:5002";
        private readonly ILogger<UserService> _logger;

        public UserService(IHttpClient httpCliemt, IOptions<Dtos.ServiceDisvoveryOptions> options, IDnsQuery dnsQuery, ILogger<UserService> logger)
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

        public async Task<string> CheckOrCreate(string phone)
        {
            var form = new Dictionary<string, string> { { "phone", phone } };
            //var content = new FormUrlEncodedContent(form);
            try
            {
                var response = await _httpCliemt.PostAsync(_userServiceUrl + "/api/user/check-or-create", form);
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    var urerid = await response.Content.ReadAsStringAsync();
                    Guid.TryParse(urerid, out Guid intUserId);
                    return intUserId.ToString();

                }
            }
            catch (Exception ex)
            {
                _logger.LogError("CheckOrCreate 在重试后失败," + ex.Message + "," + ex.StackTrace);
                throw ex;
            }
            return "";
        }
    }
}
