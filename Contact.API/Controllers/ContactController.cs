using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Contact.API.Data;
using Contact.API.Models;
using Contact.API.Services;
using System.Threading;
using MongoDB.Driver;
using Contact.API.ViewModels;

namespace Contact.API.Controllers
{
    [Route("api/contacts")]
    [ApiController]
    public class ContactController : BaseController
    {
        private readonly IContactApplyRequestRepository _contactApplyRequestRepository;
        private readonly IContactRepository _contactRepository;
        private readonly IUserService _userService;

        public ContactController(IContactApplyRequestRepository contactApplyRequestRepository, IUserService userService, IContactRepository contactRepository)
        {
            _contactApplyRequestRepository = contactApplyRequestRepository;
            _userService = userService;
            _contactRepository = contactRepository;
        }

        /// <summary>
        /// 获取联系人列表
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("apply-get")]
        public async Task<IActionResult> Get(CancellationToken cancellationToken)
        {
            var requests = await _contactRepository.GetContactsAsync(UserIdentity.UserId, cancellationToken);
            return Ok(requests);
        }

        /// <summary>
        /// 更新好友标签
        /// </summary>
        /// <param name="tag"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("apply-tag")]
        public async Task<IActionResult> TagContact([FromBody]TagContactInputViewModel tag, CancellationToken cancellationToken)
        {
            var requests = await _contactRepository.TagContactAsync(UserIdentity.UserId, tag.ContactId, tag.Tags, cancellationToken);
            if (requests)
            {
                return Ok();
            }
            return BadRequest();
        }

        /// <summary>
        /// 获取好友申请列表
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("apply-requests")]
        public async Task<IActionResult> GetApplyReqeusts(CancellationToken cancellationToken)
        {
            var requests = await _contactApplyRequestRepository.GetRequestListAsync(UserIdentity.UserId, cancellationToken);
            return Ok(requests);
        }

        /// <summary>
        /// 添加好友申请
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("apply-requests")]
        public async Task<IActionResult> AddApplyReqeusts(string userId, CancellationToken cancellationToken)
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
                ApplyTime = DateTime.Now,
                Avatar = userBaseInfo.Avatar
            }, cancellationToken);
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
        public async Task<IActionResult> ApprovalApplyReqeust(string applierId, CancellationToken cancellationToken)
        {
            var result = await _contactApplyRequestRepository.ApprovalAsync(UserIdentity.UserId, applierId, cancellationToken);
            if (!result)
            {
                return BadRequest();
            }
            var applier = await _userService.GetBaseUserInfoAsync(applierId);
            var userinfo = await _userService.GetBaseUserInfoAsync(UserIdentity.UserId);
            await _contactRepository.AddContactAsync(UserIdentity.UserId, applier, cancellationToken);
            await _contactRepository.AddContactAsync(applierId, userinfo, cancellationToken);
            return Ok();

        }
    }
}
