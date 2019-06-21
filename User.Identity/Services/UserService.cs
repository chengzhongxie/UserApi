using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace User.Identity.Services
{
    public class UserService : IUserService
    {
        private HttpClient _httpCliemt;
        private readonly string _userServiceUrl = "http://localhost:5000";

        public UserService(HttpClient httpCliemt)
        {
            _httpCliemt = httpCliemt;
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
