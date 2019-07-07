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

namespace Contact.API.Services
{
    public class UserService : IUserService
    {
        public Task<BaseUserInfo> GetBaseUserInfoAsync(string userId)
        {
            throw new NotImplementedException();
        }
    }
}
