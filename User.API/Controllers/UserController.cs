using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using User.API.Data;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.JsonPatch;

namespace User.API.Controllers
{
    [Route("api/user")]
    [ApiController]
    public class UserController : BaseController
    {
        private readonly UserContext _userContext;
        private readonly ILogger<UserController> _logger;

        public UserController(UserContext userContext, ILogger<UserController> logger)
        {
            _userContext = userContext;
            _logger = logger;
        }

        [Route("")]
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var user = await _userContext.Users.AsNoTracking().Include(u => u.Properties).SingleOrDefaultAsync(u => u.Id.ToString() == UserIdentity.UserId);
            if (user == null)
            {
                throw new UserOperationException($"错误的用户上下文id{UserIdentity.UserId}");
            }
            else
            {
                return Json(user);
            }
        }

        [Route("")]
        [HttpPatch]
        public async Task<IActionResult> Patch([FromBody]JsonPatchDocument<Models.AppUser> patch)
        {
            var user = await _userContext.Users.Include(u => u.Properties).SingleOrDefaultAsync(u => u.Id.ToString() == UserIdentity.UserId);
            patch.ApplyTo(user);

            //foreach (var item in user.Properties)
            //{
            //    _userContext.Entry(item).State = EntityState.Detached;
            //}

            var originProperties = await _userContext.UserProperties.AsNoTracking().Where(u => u.AppUserId == user.Id).ToListAsync();
            var allProperties = originProperties.Union(user.Properties).Distinct();

            var removedProperties = originProperties.Except(user.Properties);
            var newProperties = allProperties.Except(originProperties);

            foreach (var item in removedProperties)
            {
                _userContext.UserProperties.Remove(item);
            }
            foreach (var item in newProperties)
            {
                _userContext.UserProperties.Add(item);
            }
            _userContext.Update(user);
            _userContext.SaveChanges();
            return Json(user);
        }

        /// <summary>
        /// 检查或者创建用户（当用户手机号不存在的时候则创建用户）
        /// </summary>
        /// <param name="phone"></param>
        /// <returns></returns>
        [Route("check-or-create")]
        [HttpPost]
        public async Task<IActionResult> CheckOrCreate([FromForm] string phone)
        {
            //TBD 做手机号码的格式验证
            var user = _userContext.Users.SingleOrDefault(u => u.Phone == phone);
            if (user == null)
            {
                user = new Models.AppUser { Phone = phone };
                _userContext.Users.Add(user);
                await _userContext.SaveChangesAsync();
            }
            return Ok(user.Id.ToString());
        }
    }
}
