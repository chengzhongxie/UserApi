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
using System.Collections;

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
            return Ok(new
            {
                user.Id,
                user.Name,
                user.Company,
                user.Title,
                user.Avatar
            });
        }

        /// <summary>
        /// 获取用户标签选项数据
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("tags")]
        public async Task<IActionResult> GetUserTage()
        {
            return Ok(await _userContext.UserTags.Where(m => m.UserId.ToString() == UserIdentity.UserId).ToListAsync());
        }

        /// <summary>
        /// 根据手机号码查找资料
        /// </summary>
        /// <param name="phone"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("search")]
        public async Task<IActionResult> Search(string phone)
        {
            return Ok(await _userContext.Users.Include(u => u.Properties).SingleOrDefaultAsync(u => u.Id.ToString() == UserIdentity.UserId));
        }
        /// <summary>
        /// 更新用户标签数据
        /// </summary>
        /// <returns></returns>
        [HttpPut]
        [Route("tags")]
        public async Task<IActionResult> UpdateUserTage([FromBody]List<string> tages)
        {
            var originTags = await _userContext.UserTags.Where(u => u.UserId.ToString() == UserIdentity.UserId).ToListAsync();
            var newTags = tages.Except(originTags.Select(u => u.Tag));
            await _userContext.UserTags.AddRangeAsync(newTags.Select(t => new Models.UserTag
            {
                CreateTime = DateTime.Now,
                UserId = Guid.Parse(UserIdentity.UserId),
                Tag = t
            }));
            await _userContext.SaveChangesAsync();
            return Ok();
        }

        [HttpGet]
        [Route("baseinfo/{userId}")]
        public async Task<IActionResult> GetBaseInfo(string userId)
        {
            // TBD 检查用户是否好友关系
            var user = await _userContext.Users.SingleOrDefaultAsync(u => u.Id.ToString() == userId);
            if (user == null)
            {
                return NotFound();
            }
            return Ok(new
            {
                UserId = user.Id,
                user.Name,
                user.Company,
                user.Title,
                user.Avatar
            });
        }
    }
}
