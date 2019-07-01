﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Contact.API.Dtos;

namespace Contact.API.Data
{
    public interface IContactRepository
    {
        /// <summary>
        /// 更新联系人信息
        /// </summary>
        /// <param name="userInfo"></param>
        /// <returns></returns>
        Task<bool> UpdateContactInfoAsync(BaseUserInfo userInfo, CancellationToken cancellationToken);

        /// <summary>
        /// 添加联系人信息
        /// </summary>
        /// <param name="baseUserInfo"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<bool> AddContactAsync(string userId, BaseUserInfo baseUserInfo, CancellationToken cancellationToken);
    }
}
