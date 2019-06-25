using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using DnsClient;
using Microsoft.Extensions.Options;

namespace User.Identity.Services
{
    public class UserService : IUserService
    {
        private HttpClient _httpCliemt;
        private string _userServiceUrl;

        public UserService(HttpClient httpCliemt, IOptions<Dtos.ServiceDisvoveryOptions> options, IDnsQuery dnsQuery)
        {
            _httpCliemt = httpCliemt;
            var address = dnsQuery.ResolveService("service.consul", options.Value.UserServiceName);
            var addressList = address.First().AddressList;
            var host = addressList.Any()? addressList.First().ToString():address.First().HostName;
             var port = address.First().Port;
            _userServiceUrl = $"http://{host}:{port}";
        }

        public async Task<string> CheckOrCreate(string phone)
        {
            var form = new Dictionary<string, string> { { "phone", phone } };
            var content = new FormUrlEncodedContent(form);
            var response = await _httpCliemt.PostAsync(_userServiceUrl + "/api/user/check-or-create", content);
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var urerid = await response.Content.ReadAsStringAsync();
                Guid.TryParse(urerid, out Guid intUserId);
                return intUserId.ToString();

            }
            return "";
        }
    }
}
