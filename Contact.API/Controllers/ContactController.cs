using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Contact.API.Data;
using Contact.API.Models;
using Contact.API.Services;

namespace Contact.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ContactController : BaseController
    {
        private readonly IContactApplyRequestRepository _contactApplyRequestRepository;
        private readonly IUserService _userService;

        public ContactController(IContactApplyRequestRepository contactApplyRequestRepository, IUserService userService)
        {
            _contactApplyRequestRepository = contactApplyRequestRepository;
            _userService = userService;
        }

        /// <summary>
        /// 获取好友申请列表
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("apply-requests")]
        public async Task<IActionResult> GetApplyReqeusts()
        {
            var requests = await _contactApplyRequestRepository.GetRequestListAsync(UserIdentity.UserId);
            return Ok(requests);
        }
        /// <summary>
        /// 添加好友申请
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("apply-requests")]
        public async Task<IActionResult> AddApplyReqeusts(string userId)
        {
            var userBaseInfo = await _userService.GetBaseUserInfoAsync(userId);
            if (userBaseInfo == null)
            {
                throw new Exception("用户参数错误");
            }
            var result = await _contactApplyRequestRepository.AddRequstAsync(new ContactApplyRequest
            {
                UserId = Guid.Parse(userId),
                ApplierId = Guid.Parse(UserIdentity.UserId),
                Name = userBaseInfo.Name,
                Company = userBaseInfo.Company,
                Title = userBaseInfo.Title,
                CreateTime = DateTime.Now,
                Avatar = userBaseInfo.Avatar
            });
            if (!result)
            {
                return BadRequest();
            }
            return Ok();
        }
        /// <summary>
        /// 通过好友请求
        /// </summary>
        /// <returns></returns>
        [HttpPut]
        [Route("apply-requests")]
        public async Task<IActionResult> ApprovalApplyReqeust(string applierId)
        {
            var result = await _contactApplyRequestRepository.ApprovalAsync(applierId);
            if (!result)
            {
                return BadRequest();
            }
            return Ok();
        }
    }
}
